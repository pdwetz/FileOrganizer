FileOrganizer
==============

Moves files to folders by loosely matching names.

This was primarily a project to scratch an itch of mine and isn't particularly meant for mass consumption. Everything is processed in memory with the results dumped into CSV files for review.

Existing features (unchanged)
- Console application, but settings handled via config file.
- Currently just a simple single-threaded, non-parallel, non-async app.
- For large file systems (500k+ files), processing can take 10-20 minutes on a modern machine.
- For small workloads (<10k files), it should run very quickly (a few seconds at most).

*Configuration*

- MasterRootPath: The root folder for where you want the files to go (e.g. "D:\Files\Docs")
- FileRootPath: The root folder for where the files are coming from (e.g. "D:\Temp")
- MinLevel: Master folder level where processing should start (where root = 1)
- MaxLevel: Master folder level where processing should stop (where root = 1)
- IsDebugOnly: If true, no files are moved, but all other processing and logging occurs
- Extensions: List of file extensions to process. Everything else (as well as system files) are ignored. (e.g. ".txt,.docx,".odt")