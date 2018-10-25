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
using FileOrganizer.Core;
using FileOrganizer.Images;
using System;
using StructureMap;

namespace FileOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("FileOrganizer   Copyright (C) 2018 Peter Wetzel");
            Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY; for details see license.txt.");

            var container = new Container(new ImageRegistry());
            var settings = container.GetInstance<IFileOrganizerSettings>();
            var handlers = container.GetAllInstances<IProcessorPhase>();

            Console.WriteLine("Current configuration settings...");
            Console.WriteLine("Master Root Path: {0}", settings.MasterRootPath);
            Console.WriteLine("Master Min Level (1=Root): {0}", settings.MasterMinLevel);
            Console.WriteLine("Master Max Level (1=Root): {0}", settings.MasterMaxLevel);
            Console.WriteLine("Master Override Score: {0}", settings.MasterOverrideScore);
            Console.WriteLine("Min Score Required: {0}", settings.MinScore);
            Console.WriteLine("File Root Path: {0}", settings.FileRootPath);
            if (settings.IsDebugOnly) Console.WriteLine("Debug only. No files will be moved.");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Do you want to re-use Master Root data if it exists? [y/N]");
            bool reuseMaster = Console.ReadKey().Key == ConsoleKey.Y;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Processor p = new Processor(settings, new ConsoleOutput(), handlers);
            p.Process(reuseMaster);
            Console.WriteLine("{0} FileOrganizer processing complete", DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}