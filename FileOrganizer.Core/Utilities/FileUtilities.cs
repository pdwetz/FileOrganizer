/*
    FileOrganizer - Moves files to folders by loosely matching names
    Copyright (C) 2018 Peter Wetzel

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
using System.IO;

namespace FileOrganizer.Core.Utilities
{
    public class FileUtilities
    {
        /// <summary>
        /// Will create subfolder if it doesn't already exist
        /// </summary>
        /// <returns>Full path for child folder</returns>
        public static string SetupFolder(string parentPath, string folderName)
        {
            if (string.IsNullOrWhiteSpace(parentPath))
            {
                throw new ArgumentException("Parent path is required", nameof(parentPath));
            }
            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentException("Folder name is required", nameof(folderName));
            }
            if (!Directory.Exists(parentPath))
            {
                throw new ArgumentException("Parent path does not exist", nameof(parentPath));
            }
            string path = Path.Combine(parentPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// Will attempt to move the given file to target folder, creating folder if necessary and renaming file if the name already exists in target location.
        /// </summary>
        public static void SafeMoveFile(string existingFilePath, string targetFolderPath)        
        {
            var f = new FileInfo(existingFilePath);
            SafeMoveFile(f, targetFolderPath);
        }

        /// <summary>
        /// Will attempt to move the given file to target folder, creating folder if necessary and renaming file if the name already exists in target location.
        /// </summary>
        public static void SafeMoveFile(FileInfo f, string targetFolderPath)
        {
            if (!f.Exists)
            {
                return;
            }
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
            }
            var newFileName = Path.GetFileNameWithoutExtension(f.Name);
            var newFilePath = Path.Combine(targetFolderPath, $"{newFileName}{f.Extension}");
            while (File.Exists(newFilePath))
            {
                int index = newFileName.LastIndexOf('-');
                if (index < 0)
                {
                    newFileName += "-1";
                }
                else
                {
                    string val = newFileName.Substring(index + 1);
                    if (!Int32.TryParse(val, out int i))
                    {
                        newFileName += "-1";
                    }
                    else
                    {
                        i++;
                        newFileName = newFileName.Substring(0, index);
                        newFileName += $"-{i}";
                    }
                }
                newFilePath = Path.Combine(targetFolderPath, $"{newFileName}{f.Extension}");
            }
            File.Move(f.FullName, newFilePath);
        }
    }
}
