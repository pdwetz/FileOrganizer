using System.Collections.Generic;

namespace FileOrganizer.Core
{
    public interface IFileOrganizerSettings
    {
        string MasterRootPath { get; set; }
        string FileRootPath { get; set; }
        int MinLevel { get; set; }
        int MaxLevel { get; set; }
        List<string> ValidExtensions { get; set; }
        bool IsDebugOnly { get; set; }
    }
}