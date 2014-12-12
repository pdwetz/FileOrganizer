/*
    FileOrganizer - Moves files to folders by loosely matching names
    Copyright (C) 2014 Peter Wetzel

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

namespace FileOrganizer.Core
{
    public class Processor
    {
        private readonly FileOrgSession _session;

        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }

        private List<string> ValidExtensions { get; set; }

        private List<MasterFolder> MasterFolders { get; set; }

        public List<NameHash> MasterHashes { get; set; }

        public List<TargetFile> TargetFiles { get; set; }

        public Processor(string masterRootPath, string fileRootPath, List<string> validExtensions = null, bool isDebugOnly = false)
        {
            if (!Directory.Exists(masterRootPath))
            {
                throw new ApplicationException("Master root path does not exist: " + masterRootPath);
            }

            if (!Directory.Exists(fileRootPath))
            {
                throw new ApplicationException("File root path does not exist: " + fileRootPath);
            }

            _session = new FileOrgSession(masterRootPath, fileRootPath, isDebugOnly);
            MasterFolders = new List<MasterFolder>();
            MasterHashes = new List<NameHash>();
            TargetFiles = new List<TargetFile>();
            ValidExtensions = validExtensions;
        }

        public void Process()
        {
            var overallTimer = new Stopwatch();
            overallTimer.Start();
            ProcessMasterFolder(_session.MasterRootPath, 0);
            foreach (var f in MasterFolders)
            {
                MasterHashes.AddRange(f.NameHashes);
            }
            _session.MasterHashes = MasterHashes.Count();
            _session.MasterFolderRuntimeMS = overallTimer.ElapsedMilliseconds;

            var filesTimer = new Stopwatch();
            filesTimer.Start();
            ProcessFiles();
            filesTimer.Stop();
            overallTimer.Stop();
            _session.TargetFilesRuntimeMS = filesTimer.ElapsedMilliseconds;
            _session.TotalRuntimeMS = overallTimer.ElapsedMilliseconds;

            LogResults();
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
            if (level >= MinLevel && level <= MaxLevel)
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
                if (ValidExtensions != null && !ValidExtensions.Contains(ext))
                {
                    continue;
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
                if (match.MasterFolder.FullPath.StartsWith(_session.MasterRootPath) && match.Score < 1030)
                {
                    continue;
                }

                TargetFiles.Add(new TargetFile(filepath, f.Name, match.MasterFolder.FullPath));
                _session.TargetFilesMoved++;

                var newFileName = f.Name.Replace(f.Extension, "");
                var newFilePath = Path.Combine(match.MasterFolder.FullPath, newFileName + f.Extension);
                while (File.Exists(newFilePath))
                {
                    int index = newFileName.LastIndexOf('-');
                    if (index < 0)
                    {
                        newFileName += "-1";
                    }
                    else
                    {
                        int i;
                        string val = newFileName.Substring(index + 1);
                        if (!Int32.TryParse(val, out i))
                        {
                            newFileName += "-1";
                        }
                        else
                        {
                            i++;
                            newFileName = newFileName.Substring(0, index);
                            newFileName += "-" + i.ToString();
                        }
                    }
                    newFilePath = Path.Combine(match.MasterFolder.FullPath, newFileName + f.Extension);
                }
                if (!_session.IsDebugOnly)
                {
                    File.Move(filepath, newFilePath);
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