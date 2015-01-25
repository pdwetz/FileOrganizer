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
using System.IO;
using System.Reflection;

namespace FileOrganizer.Test
{
    internal class TestHelper
    {
        public const string FillerText = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n";

        public static string GetWorkingPath()
        {
            return SetupFolder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles");
        }

        public static string SetupFolder(string sParentPath, string sName)
        {
            string sPath = Path.Combine(sParentPath, sName);
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            return sPath;
        }

        public static void CreateTextFile(string sFilePath, long fillerlines, string sFillerText = FillerText)
        {
            if (File.Exists(sFilePath))
            {
                return;
            }
            using (var stream = File.CreateText(sFilePath))
            {
                for (long i = 0; i < fillerlines; i++)
                {
                    stream.Write(sFillerText);
                }
                stream.Flush();
            }
        }
    }
}
