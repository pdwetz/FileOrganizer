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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FileOrganizer.Core
{
    public class FileOrganizerSettings : IFileOrganizerSettings
    {
        public string MasterRootPath { get; set; }
        public string FileRootPath { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public List<string> ValidExtensions { get; set; }
        public bool IsDebugOnly { get; set; }

        public FileOrganizerSettings()
        {
            MasterRootPath = ConfigurationManager.AppSettings["MasterRootPath"];
            FileRootPath = ConfigurationManager.AppSettings["FileRootPath"];
            MinLevel = Convert.ToInt32(ConfigurationManager.AppSettings["MinLevel"]);
            MaxLevel = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLevel"]);
            string ext = ConfigurationManager.AppSettings["Extensions"];
            if (!string.IsNullOrWhiteSpace(ext))
            {
                ValidExtensions = ext.Split(',').ToList();
            }
            IsDebugOnly = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebugOnly"]);
        }
    }
}
