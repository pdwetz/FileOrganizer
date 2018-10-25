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
using NUnit.Framework;
using System.IO;
using System.Linq;
using FileOrganizer.Core;
using FileOrganizer.Images;
using FileOrganizer.Core.Utilities;

namespace FileOrganizer.Test
{
    [TestFixture]
    public class ProcessorTester
    {
        private string _testingPath;

        [SetUp]
        public void SetUp()
        {
            _testingPath = TestHelper.GetWorkingPath();
            System.Diagnostics.Trace.WriteLine(_testingPath);
        }

        [Test]
        public void document_filter()
        {
            const string validFileName = "am-i-valid.txt";
            const string invalidFileName = "am-i-valid.jpg";
            string masterPath = FileUtilities.SetupFolder(_testingPath, "Documents\\valid");
            string targetPath = FileUtilities.SetupFolder(_testingPath, "Stuff");
            string sFilePath = Path.Combine(targetPath, validFileName);
            TestHelper.CreateTextFile(sFilePath, 1);
            sFilePath = Path.Combine(targetPath, invalidFileName);
            TestHelper.CreateTextFile(sFilePath, 1);

            var settings = new FileOrganizerSettings
            {
                MasterRootPath = masterPath,
                FileRootPath = targetPath,
                MasterMinLevel = 1,
                MasterMaxLevel = 1,
                MinScore = 1000,
                MasterOverrideScore = 1030,
                IsDebugOnly = true,
                ValidExtensions = ".txt,.doc".Split(',').ToList()
            };

            var processor = new Processor(settings, null, null);
            var results = processor.Process();
            Assert.AreEqual(2, results.TargetFilesParsed);
            Assert.AreEqual(1, results.TargetFilesMoved);
        }

        [Test]
        public void image_filter()
        {
            const string validFileName = "am-i-valid.txt";
            const string invalidFileName = "am-i-valid.jpg";
            string masterPath = FileUtilities.SetupFolder(_testingPath, "Images\\valid");
            string targetPath = FileUtilities.SetupFolder(_testingPath, "Stuff");
            string sFilePath = Path.Combine(targetPath, validFileName);
            TestHelper.CreateTextFile(sFilePath, 1);
            sFilePath = Path.Combine(targetPath, invalidFileName);
            TestHelper.CreateTextFile(sFilePath, 1);

            var settings = new ImageSettings
            {
                MasterRootPath = masterPath,
                FileRootPath = targetPath,
                MasterMinLevel = 1,
                MasterMaxLevel = 1,
                MinScore = 1000,
                MasterOverrideScore = 1030,
                IsDebugOnly = true
            };

            var console = new DebugOutput();
            var handlers = new IProcessorPhase[] { new ImageHandler(settings, console) };
            var processor = new Processor(settings, console, handlers);
            var results = processor.Process();
            Assert.AreEqual(2, results.TargetFilesParsed);
            Assert.AreEqual(1, results.TargetFilesMoved);
        }

        [Test]
        public void image_small_filter()
        {
            string masterPath = FileUtilities.SetupFolder(_testingPath, "Images\\sample");
            string sPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets\\small");

            var settings = new ImageSettings
            {
                MasterRootPath = masterPath,
                FileRootPath = sPath,
                MasterMinLevel = 1,
                MasterMaxLevel = 1,
                MinScore = 1000,
                MasterOverrideScore = 1030,
                IsDebugOnly = true,
                SequesterSmallImages = true,
                SequesterFolderSmall = ImageSettings.DefaultSequesterFolderSmall,
                MinHeightPixels = 200,
                MinWidthPixels = 200
            };

            var console = new DebugOutput();
            var handlers = new IProcessorPhase[] { new ImageHandler(settings, console) };
            var processor = new Processor(settings, console, handlers);
            var results = processor.Process();
            Assert.AreEqual(3, results.TargetFilesParsed);
            Assert.AreEqual(2, results.TargetFilesMoved);
        }

        [Test]
        public void image_invalid_filter()
        {
            string masterPath = FileUtilities.SetupFolder(_testingPath, "Images\\sample");
            string sPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets\\invalid");

            var settings = new ImageSettings
            {
                MasterRootPath = masterPath,
                FileRootPath = sPath,
                MasterMinLevel = 1,
                MasterMaxLevel = 2,
                MinScore = 1000,
                MasterOverrideScore = 1030,
                IsDebugOnly = true,
                SequesterInvalidImages = true,
                SequesterFolderInvalid = ImageSettings.DefaultSequesterFolderInvalid
            };

            var console = new DebugOutput();
            var handlers = new IProcessorPhase[] { new ImageHandler(settings, console) };
            var processor = new Processor(settings, console, handlers);
            var results = processor.Process();
            Assert.AreEqual(2, results.TargetFilesParsed);
            Assert.AreEqual(1, results.TargetFilesMoved);
        }
    }
}