using System.Reflection;

[assembly: AssemblyTitle("FileOrganizer")]
[assembly: AssemblyDescription("Moves files to folders by loosely matching names")]
[assembly: AssemblyConfiguration("FileOrganizer")]
[assembly: AssemblyProduct("FileOrganizer")]
[assembly: AssemblyCopyright("Copyright ©  2015 Peter Wetzel")]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
/*
 * Version 1.2.0.0
 * - Redesigned app to be more DI-friendly. Includes ability to call out to handlers for additional validation on files.
 * - Added support for testing images (both valid files and minimum size).
 * Version 1.1.0.0
 * - Minor release, primarily with quality-of-life improvements for user
 * Version 1.0.0.0
 * - Initial release
*/