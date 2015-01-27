/*
    FileOrganizer - Moves files to folders by loosely matching names
    Copyright (C) 2015 Peter Wetzel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using CsvHelper;
using FileOrganizer.Core.Data;
using FileOrganizer.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileOrganizer.Core
{
    public class Processor
    {
        public const string MasterOutputFileFormat = "mf{0}.bin";

        public IConsoleOutput ConsoleOutput { get; set; }

        private readonly IFileOrganizerSettings _settings;
        private readonly IEnumerable<IProcessorPhase> _handlers;
        private readonly FileOrgSession _session;

        private List<MasterFolder> MasterFolders { get; set; }

        private List<NameHash> MasterHashes { get; set; }

        private List<TargetFile> TargetFiles { get; set; }

        private string MasterOutputFilePath { get; set; }

        public Processor(IFileOrganizerSettings settings, IConsoleOutput consoleOutput, IEnumerable<IProcessorPhase> handlers)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            _settings = settings;

            ConsoleOutput = consoleOutput;
            if (ConsoleOutput == null) { ConsoleOutput = new DebugOutput(); }

            if (!Directory.Exists(settings.MasterRootPath))
            {
                throw new ApplicationException("Master root path does not exist: " + _settings.MasterRootPath);
            }

            if (!Directory.Exists(settings.FileRootPath))
            {
                throw new ApplicationException("File root path does not exist: " + _settings.FileRootPath);
            }

            _handlers = handlers;
            _session = new FileOrgSession(_settings);
            MasterOutputFilePath = string.Format(MasterOutputFileFormat, _session.MasterRootPath.GetHashCode());
            MasterFolders = new List<MasterFolder>();
            MasterHashes = new List<NameHash>();
            TargetFiles = new List<TargetFile>();
        }

        public FileOrgSession Process(bool reuseMaster = false)
        {   
            var overallTimer = new Stopwatch();
            overallTimer.Start();
            bool writeMasterData = true;
            if (reuseMaster && File.Exists(MasterOutputFilePath))
            {
                ConsoleOutput.WriteLine("{0} Loading existing Master Folder data", DateTime.Now.ToString("HH:mm:ss.fff"));
                using (Stream stream = File.OpenRead(MasterOutputFilePath))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    MasterFolders = (List<MasterFolder>)deserializer.Deserialize(stream);
                    stream.Close();
                }
                writeMasterData = false;
                _session.MasterFolders = MasterFolders.Count;
            }
            else
            {
                ConsoleOutput.WriteLine("{0} Parsing Master Folder", DateTime.Now.ToString("HH:mm:ss.fff"));
                ProcessMasterFolder(_session.MasterRootPath, 0);
            }
            ConsoleOutput.WriteLine("{0} Master folders: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), MasterFolders.Count);
            foreach (var f in MasterFolders)
            {
                MasterHashes.AddRange(f.NameHashes);
            }
            _session.MasterHashes = MasterHashes.Count();
            ConsoleOutput.WriteLine("{0} Master hashes: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), _session.MasterHashes);
            _session.MasterFolderRuntimeMS = overallTimer.ElapsedMilliseconds;

            var filesTimer = new Stopwatch();
            filesTimer.Start();
            ConsoleOutput.WriteLine("{0} Processing files", DateTime.Now.ToString("HH:mm:ss.fff"));
            ProcessFiles();
            ConsoleOutput.WriteLine("{0} Files parsed: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), _session.TargetFilesParsed);
            ConsoleOutput.WriteLine("{0} Files moved: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), _session.TargetFilesMoved);

            ConsoleOutput.WriteLine("{0} Saving Master Folder data", DateTime.Now.ToString("HH:mm:ss.fff"));
            if (writeMasterData)
            {
                using (Stream stream = File.Create(MasterOutputFilePath))
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(stream, MasterFolders);
                    stream.Close();
                }
            }

            filesTimer.Stop();
            overallTimer.Stop();
            _session.TargetFilesRuntimeMS = filesTimer.ElapsedMilliseconds;
            _session.TotalRuntimeMS = overallTimer.ElapsedMilliseconds;

            ConsoleOutput.WriteLine("{0} Logging results", DateTime.Now.ToString("HH:mm:ss.fff"));
            LogResults();
            return _session;
        }

        private void ProcessMasterFolder(string path, int level)
        {
            if (path.Contains("misc", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            level++;
            var masterDirectories = Directory.EnumerateDirectories(path);
            foreach (var dir in masterDirectories)
            {
                if (!dir.StartsWith("misc", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessMasterFolder(dir, level);
                }
            }
            if (level >= _settings.MinLevel && level <= _settings.MaxLevel)
            {
                MasterFolders.Add(new MasterFolder(_session.MasterRootPath, path));
                _session.MasterFolders++;
            }
        }

        private void ProcessFiles()
        {
            var filepaths = Directory.EnumerateFiles(_session.FileRootPath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (var filepath in filepaths)
            {
                _session.TargetFilesParsed++;
                FileInfo f = new FileInfo(filepath);
                if (f.Attributes.HasFlag(FileAttributes.System))
                {
                    continue;
                }

                var ext = f.Extension.ToLower();
                if (_settings.ValidExtensions != null && !_settings.ValidExtensions.Contains(ext))
                {
                    continue;
                }

                if (_handlers != null)
                {
                    bool seemsokay = true;
                    foreach (var h in _handlers)
                    {
                        if (!h.ValidateFile(f))
                        {
                            seemsokay = false;
                            break;
                        }
                    }
                    if (!seemsokay) { continue; }
                }

                var match = MasterHashes.Where(x => f.Name.Contains(x.Hash, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Score).FirstOrDefault();
                if (match == null)
                {
                    continue;
                }
                if (f.DirectoryName == match.MasterFolder.FullPath)
                {
                    continue;
                }
                if (f.FullName.StartsWith(_session.MasterRootPath) && match.Score < 1030)
                {
                    continue;
                }

                TargetFiles.Add(new TargetFile(filepath, f.Name, match.MasterFolder.FullPath));
                _session.TargetFilesMoved++;

                if (!_session.IsDebugOnly)
                {
                    FileUtilities.SafeMoveFile(f, match.MasterFolder.FullPath);
                }
            }
        }

        public void LogResults()
        {
            string sPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Results");
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            string sFilePath = Path.Combine(sPath, "sessions.csv");
            bool bSessionFileExists = File.Exists(sFilePath);
            using (var textWriter = File.AppendText(sFilePath))
            {
                using (var writer = new CsvWriter(textWriter))
                {
                    if (!bSessionFileExists)
                    {
                        writer.WriteHeader<FileOrgSession>();
                    }
                    writer.WriteRecord(_session);
                }
            }
            
            sFilePath = sFilePath = Path.Combine(sPath, string.Format("{0}-master-hashes.csv", _session.Id));
            using (var textWriter = File.CreateText(sFilePath))
            {
                using (var writer = new CsvWriter(textWriter))
                {
                    writer.WriteRecords(MasterHashes);
                }
            }

            sFilePath = sFilePath = Path.Combine(sPath, string.Format("{0}-target-files.csv", _session.Id));
            using (var textWriter = File.CreateText(sFilePath))
            {
                using (var writer = new CsvWriter(textWriter))
                {
                    writer.WriteRecords(TargetFiles);
                }
            }
        }
    }
}