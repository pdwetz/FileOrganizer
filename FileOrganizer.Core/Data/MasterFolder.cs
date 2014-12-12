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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileOrganizer.Core.Data
{
    public class MasterFolder
    {
        private const string Pattern = @"[a-z] +[a-z]*|[\w'-]*";

        private readonly string _masterRootPath;

        public string FullPath { get; set; }

        public string Name { get; set; }

        public List<NameHash> NameHashes { get; set; }

        public MasterFolder(string masterRootPath, string path)
        {
            _masterRootPath = masterRootPath;
            FullPath = path;
            Name = Path.GetFileName(path);
            NameHashes = new List<NameHash>();
            Process();
        }

        public void Process()
        {
            if (Name.Equals("misc", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            long test;
            if (long.TryParse(Name, out test))
            {
                return;
            }

            if (!Name.Contains('(') && !Name.Contains(')'))
            {
                if (Name.StartsWith("misc -", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessName(Name.Substring(6), 200);
                }
                else if (Name.StartsWith("misc", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessName(Name.Substring(4), 200);
                }
                else
                {
                    ProcessName(Name, 1000);
                }
            }
            else
            {
                int startGroup = Name.IndexOf('(');
                int endGroup = Name.IndexOf(')');
                string main = Name.Substring(0, startGroup);
                ProcessName(main, 1000);
                string other = Name.Substring(startGroup + 1, endGroup - startGroup - 1);
                if (other.Contains(','))
                {
                    var names = other.Split(new char[] { ',' });
                    foreach (var name in names)
                    {
                        ProcessName(name, 100);
                    }
                }
                else
                {
                    ProcessName(other, 100);
                }
            }
        }

        private void ProcessName(string name, int baseScore)
        {
            name = name.Trim();

            // Note: We're getting matches with a blank string on every entry for some reason... seems fine when using online regex tools.
            var matches = Regex.Matches(name, Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (matches.Count < 2)
            {
                return;
            }
            StringBuilder hash = new StringBuilder();

            int fullNameScore = baseScore + 3;
            if (matches.Count > 4)
            {
                fullNameScore = baseScore + 50;
                // First-Middle-Last
                hash.Clear();
                for (int i = 0; i < matches.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(matches[i].Value)) { continue; }
                    if (i > 0) { hash.Append("-"); }
                    hash.Append(matches[i].Value);
                }
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 45));

                // First_Middle_Last
                hash.Clear();
                for (int i = 0; i < matches.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(matches[i].Value)) { continue; }
                    if (i > 0) { hash.Append("_"); }
                    hash.Append(matches[i].Value);
                }
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 44));

                // FirstMiddleLast
                hash.Clear();
                for (int i = 0; i < matches.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(matches[i].Value)) { continue; }
                    hash.Append(matches[i].Value);
                }
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 43));
            }

            if (matches.Count > 2)
            {
                fullNameScore = baseScore + 50;
                var first = matches[0].Value;
                var last = matches[matches.Count - 2].Value;

                // First-Last
                hash.Clear();
                hash.Append(first);
                hash.Append("-");
                hash.Append(last);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 42));

                // First_Last
                hash.Clear();
                hash.Append(first);
                hash.Append("_");
                hash.Append(last);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 41));

                // FirstLast
                hash.Clear();
                hash.Append(first);
                hash.Append(last);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 40));

                // Last First
                hash.Clear();
                hash.Append(last);
                hash.Append(" ");
                hash.Append(first);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 35));

                // Last-First
                hash.Clear();
                hash.Append(last);
                hash.Append("-");
                hash.Append(first);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 34));

                // Last_First
                hash.Clear();
                hash.Append(last);
                hash.Append("_");
                hash.Append(first);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 33));

                // LastFirst
                hash.Clear();
                hash.Append(last);
                hash.Append(first);
                NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 30));

                // FLast
                if (last.Length > 5)
                {
                    hash.Clear();
                    hash.Append(first[0]);
                    hash.Append(last);
                    NameHashes.Add(new NameHash(this, hash.ToString(), baseScore + 25));
                }

                // Last
                if (last.Length > 5)
                {
                    NameHashes.Add(new NameHash(this, last, baseScore + 10));
                }

                // First
                if (first.Length > 5)
                {
                    NameHashes.Add(new NameHash(this, first, baseScore + 1));
                }
            }
            NameHashes.Add(new NameHash(this, name, fullNameScore));
        }
    }
}