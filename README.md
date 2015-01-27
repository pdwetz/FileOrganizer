FileOrganizer
==============

Moves files to folders by loosely matching names.

This was primarily a project to scratch an itch of mine and isn't particularly meant for mass consumption. Everything is processed in memory with the results dumped into CSV files for review. The latest version includes a helper Image library for doing some additional processing/cleanup for small or invalid images.

##Features##
- Console application, but settings handled via config file or via code.
- Currently just a simple single-threaded, non-parallel, non-async app.
- For large file systems (500k+ files), processing can take 10-20 minutes on a modern machine.
- For small workloads (<10k files), it should run very quickly (a few seconds at most).

##Configuration##

The app now uses DI via StructureMap for setup and configuration. You can setup your own implementation of IFileOrganizerSettings and Registry classes in a local build, of course alongside custom IProcessorPhase classes if you want to do any custom processing.

- MasterRootPath: The root folder for where you want the files to go (e.g. "D:\Files\Docs")
- FileRootPath: The root folder for where the files are coming from (e.g. "D:\Temp")
- MinLevel: Master folder level where processing should start (where root = 1)
- MaxLevel: Master folder level where processing should stop (where root = 1)
- IsDebugOnly: If true, no files are moved, but all other processing and logging occurs
- Extensions: List of file extensions to process. Everything else (as well as system files) are ignored. (e.g. ".txt,.docx,".odt")
- New Image library (via ImageSettings class)
  - Extensions are optional (default to ".jpg,.png,.gif,.jpeg")
  - SequesterSmallImages - true/false flag for whether we should move small images to a subfolder
  - MinHeightPixels - Any images below this will be moved if appropriate
  - MinWidthPixels - Any images below this will be moved if appropriate
  - SequesterFolderSmall - Name of the folder to move small images (defaults to "tmp-small-img")
  - SequesterInvalidImages - true/false flag for whether we should move invalid/corrupt images to a subfolder
  - SequesterFolderInvalid - ame of the folder to move invalid/corrupt images (defaults to "tmp-invalid-img")
  
##Future##
Nothing major planned, aside from adding additional helper libraries. At the moment, I'm leaning towards music, although movies are another option as well.
