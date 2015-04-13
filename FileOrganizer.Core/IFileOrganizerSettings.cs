using System.Collections.Generic;

namespace FileOrganizer.Core
{
    public interface IFileOrganizerSettings
    {
        string MasterRootPath { get; set; }
        string FileRootPath { get; set; }
        int MasterMinLevel { get; set; }
        int MasterMaxLevel { get; set; }
        int MinScore { get; set; }
        int MasterOverrideScore { get; set; }
        List<string> ValidExtensions { get; set; }
        bool IsDebugOnly { get; set; }
    }
}