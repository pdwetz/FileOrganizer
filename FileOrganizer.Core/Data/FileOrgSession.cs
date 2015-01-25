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
using System;

namespace FileOrganizer.Core.Data
{
    public class FileOrgSession
    {
        public long Id { get; set; }

        public string MasterRootPath { get; set; }
        public string FileRootPath { get; set; }

        public long TotalRuntimeMS { get; set; }

        public long MasterFolderRuntimeMS { get; set; }

        public long TargetFilesRuntimeMS { get; set; }

        public int MasterFolders { get; set; }

        public int MasterHashes { get; set; }

        public int TargetFilesParsed { get; set; }

        public int TargetFilesMoved { get; set; }

        public bool IsDebugOnly { get; set; }

        public FileOrgSession(string masterRootPath, string fileRootPath, bool isDebugOnly = false)
        {
            Id = DateTime.Now.Ticks;
            MasterRootPath = masterRootPath;
            FileRootPath = fileRootPath;
            IsDebugOnly = isDebugOnly;
        }
    }
}