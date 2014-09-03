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
using FileOrganizer.Core;
using System.Linq;
using System;
using System.Configuration;

namespace FileOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string masterRootPath = ConfigurationManager.AppSettings["MasterRootPath"];
            string fileRootPath = ConfigurationManager.AppSettings["FileRootPath"];
            int minLevel = Convert.ToInt32(ConfigurationManager.AppSettings["MinLevel"]);
            int maxLevel = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLevel"]);
            string ext = ConfigurationManager.AppSettings["Extensions"];
            string[] extensions = ext.Split(',');
            bool isDebugOnly = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebugOnly"]);

            Processor p = new Processor(masterRootPath, fileRootPath, extensions.ToList(), isDebugOnly);
            p.MinLevel = minLevel;
            p.MaxLevel = maxLevel;
            p.Process();
            Console.WriteLine("FileOrganizer processing complete");
        }
    }
}