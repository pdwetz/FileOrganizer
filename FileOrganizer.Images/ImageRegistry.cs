﻿/*
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
using StructureMap;

namespace FileOrganizer.Images
{
    public class ImageRegistry : Registry
    {
        public ImageRegistry()
        {
            For<IFileOrganizerSettings>().Use<ImageSettings>();
            For<IProcessorPhase>().Use<ImageHandler>();
            For<IConsoleOutput>().Use<DebugOutput>();
        }
    }
}