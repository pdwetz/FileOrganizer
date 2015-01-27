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
using FileOrganizer.Core;
using System;
using System.Configuration;
using System.Linq;

namespace FileOrganizer.Images
{
    public class ImageSettings : FileOrganizerSettings
    {
        public const int DefaultMinWidth = 300;
        public const int DefaultMinHeight = 300;
        public const string DefaultSequesterFolderSmall = "tmp-small-img";
        public const string DefaultSequesterFolderInvalid = "tmp-invalid-img";
        public const string DefaultExtensions = ".jpg,.png,.gif,.jpeg";

        public bool SequesterInvalidImages { get; set; }
        public bool SequesterSmallImages { get; set; }
        public int MinHeightPixels { get; set; }
        public int MinWidthPixels { get; set; }
        public string SequesterFolderSmall { get; set; }
        public string SequesterFolderInvalid { get; set; }

        public ImageSettings()
            : base()
        {
            string ext = ConfigurationManager.AppSettings["Extensions"];
            if (string.IsNullOrWhiteSpace(ext))
            {
                ValidExtensions = (DefaultExtensions).Split(',').ToList();
            }
            else
            {
                ValidExtensions = ext.Split(',').ToList();
            }
            SequesterSmallImages = Convert.ToBoolean(ConfigurationManager.AppSettings["SequesterSmallImages"]);
            SequesterInvalidImages = Convert.ToBoolean(ConfigurationManager.AppSettings["SequesterInvalidImages"]);
            if (SequesterSmallImages || SequesterInvalidImages)
            {
                MinHeightPixels = Convert.ToInt32(ConfigurationManager.AppSettings["MinHeightPixels"]);
                if (MinHeightPixels <= 0) { MinHeightPixels = DefaultMinHeight; }
                MinWidthPixels = Convert.ToInt32(ConfigurationManager.AppSettings["MinWidthPixels"]);
                if (MinWidthPixels <= 0) { MinWidthPixels = DefaultMinWidth; }
                SequesterFolderSmall = ConfigurationManager.AppSettings["SequesterFolderSmall"];
                if (string.IsNullOrWhiteSpace(SequesterFolderSmall))
                {
                    SequesterFolderSmall = DefaultSequesterFolderSmall;
                }
                SequesterFolderInvalid = ConfigurationManager.AppSettings["SequesterFolderInvalid"];
                if (string.IsNullOrWhiteSpace(SequesterFolderInvalid))
                {
                    SequesterFolderInvalid = DefaultSequesterFolderInvalid;
                }
            }
        }
    }
}
