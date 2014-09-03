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
using System;

namespace FileOrganizer.Core.Utilities
{
    public static class StringExtensions
    {
        public static bool SafeEquals(this string source, string other, StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            if (source == null)
            {
                return other == null;
            }
            return source.Equals(other, comp);
        }

        /// <summary>
        /// Case-insensitive override for contains
        /// Source:
        /// http://stackoverflow.com/a/444818/21865
        /// (Creative Commons Attribution Share Alike)
        /// </summary>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source == null || toCheck == null) { return false; }
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}