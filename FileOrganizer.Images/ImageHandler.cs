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
using System;
using System.IO;
using System.Text;
using FileOrganizer.Core;
using FileOrganizer.Core.Utilities;

namespace FileOrganizer.Images
{
    public class ImageHandler : IProcessorPhase
    {
        private readonly IFileOrganizerSettings _settings;
        private readonly IConsoleOutput _consoleOutput;
        private bool _hasImageSettings;

        public ImageHandler(IFileOrganizerSettings settings, IConsoleOutput consoleOutput)
        {
            _settings = settings;
            _consoleOutput = consoleOutput;
            _hasImageSettings = _settings.GetType() == typeof(ImageSettings);
        }

        public bool ValidateFile(FileInfo f)
        {
            if (!_hasImageSettings)
            {
                return true;
            }

            // TODO Is this necessary? Can the private member and/or constructor param use the expected type?
            ImageSettings imgSettings = (ImageSettings)_settings;

            if (!imgSettings.SequesterSmallImages && !imgSettings.SequesterInvalidImages)
            {
                return true;
            }

            if (f.DirectoryName.Equals(imgSettings.SequesterFolderSmall, StringComparison.InvariantCultureIgnoreCase)
                || f.DirectoryName.Equals(imgSettings.SequesterFolderInvalid, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            TagLib.File file = null;
            try
            {
                file = TagLib.File.Create(f.FullName);
            }
            catch (TagLib.UnsupportedFormatException)
            {
                HandleInvalidFile(f, "Unsupported file: ");
                return false;
            }
            catch (TagLib.CorruptFileException)
            {
                HandleInvalidFile(f, "Corrupt file: ");
                return false;
            }
            catch (Exception)
            {
                HandleInvalidFile(f, "TagLib Unable to open file: ");
                return false;
            }

            var image = file as TagLib.Image.File;
            if (image == null)
            {
                HandleInvalidFile(f, "Not an image file: ");
                return false;
            }

            if (image.PossiblyCorrupt)
            {
                StringBuilder msg = new StringBuilder();
                foreach (string reason in image.CorruptionReasons)
                {
                    msg.AppendLine("    * " + reason);
                }
                HandleInvalidFile(f, "Possibly Corrupt Image: ", msg.ToString());
                return false;
            }
            else if (image.Properties != null && imgSettings.SequesterSmallImages)
            {
                if (image.Properties.PhotoWidth < imgSettings.MinWidthPixels || image.Properties.PhotoHeight < imgSettings.MinHeightPixels)
                {
                    Console.WriteLine("Small image found: " + f.FullName);
                    var smallDirPath = Path.Combine(f.Directory.FullName, imgSettings.SequesterFolderSmall);
                    if (!_settings.IsDebugOnly)
                    {
                        FileUtilities.SafeMoveFile(f, smallDirPath);
                    }
                    return false;
                }
            }

            return true;
        }

        private void HandleInvalidFile(FileInfo f, string message, string msgPostfix = "")
        {
            _consoleOutput.WriteLine(string.Concat(message, f.FullName));
            if (!string.IsNullOrWhiteSpace(msgPostfix))
            {
                _consoleOutput.WriteLine(msgPostfix);
            }
            if (((ImageSettings)_settings).SequesterInvalidImages)
            {
                var smallDirPath = Path.Combine(f.Directory.FullName, ((ImageSettings)_settings).SequesterFolderInvalid);
                if (!_settings.IsDebugOnly)
                {
                    FileUtilities.SafeMoveFile(f, smallDirPath);
                }
            }
        }
    }
}
