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
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FileOrganizer.Test
{
    [TestFixture]
    public class FileFilterTester
    {
        private string _rootPath;

        [SetUp]
        public void SetUp()
        {
            _rootPath = TestHelper.GetWorkingPath();
            System.Diagnostics.Trace.WriteLine(_rootPath);
        }

        [Test]
        public void document_filter()
        {
            const string validFileName = "validFileName.txt";
            const string invalidFileName = "invalidFileName.jpg";
            string docPath = TestHelper.SetupFolder(_rootPath, "Documents");
            string sFilePath = Path.Combine(docPath, validFileName);
            TestHelper.CreateTextFile(sFilePath, 1);
            sFilePath = Path.Combine(docPath, invalidFileName);
            TestHelper.CreateTextFile(sFilePath, 1);

            Assert.AreEqual(Directory.GetFiles(docPath).Count(), 1);
        }
    }
}
