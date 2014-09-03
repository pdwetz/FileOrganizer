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
using FileOrganizer.Core.Data;
using NUnit.Framework;
using System.Linq;

namespace FileOrganizer.Test
{
    [TestFixture]
    public class MasterFolderTester
    {
        private string _rootPath;

        [SetUp]
        public void SetUp()
        {
            _rootPath = TestHelper.GetWorkingPath();
            System.Diagnostics.Trace.WriteLine(_rootPath);
        }

        [Test]
        public void strip_unneeded_chars()
        {
            string initial = "misc - something ";
            string validResult = "something";

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            string result = folder.NameHashes.FirstOrDefault().Hash;

            Assert.IsNotNull(result);
            Assert.AreEqual(validResult, result);
        }

        [Test]
        public void single_folder_many_names()
        {
            string initial = "some thing (alpha, bravo, charlie delta)";
            int validResultCount = 19;

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            int resultCount = folder.NameHashes.Count;

            Assert.AreEqual(validResultCount, resultCount);
        }

        [Test]
        public void single_folder_simple_hash()
        {
            string initial = "Single";
            int validResultCount = 1;

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            int resultCount = folder.NameHashes.Count;

            Assert.AreEqual(validResultCount, resultCount);
        }

        [Test]
        public void single_folder_complex_hash()
        {
            string initial = "Single Folder";
            int validResultCount = 11;

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            int resultCount = folder.NameHashes.Count;

            Assert.AreEqual(validResultCount, resultCount);
        }

        [Test]
        public void score_first_by_hashing_order()
        {
            string initial = "Single Folder";

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            var first = folder.NameHashes.OrderByDescending(x => x.Score).First();

            Assert.AreEqual(initial, first.Hash);
        }

        [Test]
        public void score_last_by_hashing_order()
        {
            string initial = "Single Folder";

            string masterPath = TestHelper.SetupFolder(_rootPath, initial);
            var folder = new MasterFolder(masterPath);
            var last = folder.NameHashes.OrderBy(x => x.Score).First();

            Assert.AreEqual("Single", last.Hash);
        }

        [Test]
        public void score_misc_lowers_score()
        {
            string nameMisc = "misc - something";
            string nameNormal = "something";

            var miscFolder = new MasterFolder(TestHelper.SetupFolder(_rootPath, nameMisc));
            var normalFolder = new MasterFolder(TestHelper.SetupFolder(_rootPath, nameNormal));

            var miscScore = miscFolder.NameHashes.FirstOrDefault().Score;
            var normalScore = normalFolder.NameHashes.FirstOrDefault().Score;

            Assert.Less(miscScore, normalScore);
        }
    }
}
