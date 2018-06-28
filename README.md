# window-manager
Windows specific: Restores window positions for use when your windows rearrange on their own (post windows unlock) in multiple monitor / laptop usage scenarios.

## Problem
Running windows 10.  I'm constantly locking my machine for brief periods.  If it stays locked for more than about a minute, *something* rearranges my window positions.  This happens only when my laptop is docked at certain docks (home doesn't cause this; work does).  So, at work, after each unlock, I have to manually undo the arrangement that the OS did (moves every window).  I searched for way too long trying to find an answer to this problem; nobody had any solution (actually, some folks found super-strange or complex solutions, but *nothing* worked consistently).  A coworker suggested I make use of some of his olde fashioned win32 window management code to put something together, so I did.  It's not perfect, but for me it works about 95% of the way (every once in awhile a window is not restored where it was during persist).

### Projects (using Visual Studio 2017)
1. WindowsPositionPersisterLibrary: Native Win32 (still relevant) stuff along with the manager that does persistence / re-application of windows.
2. WindowPositionPersisterConsole: The entry point for invoking manually.
3. WindowPositionPersisterMain:  A WinForms (old, but intended to be invisible anyway) application that I tested, but couldn't get working correctly (see comment "This is the reason this can't run as a windows service" for starters).  Won't work running as windows service, and couldn't get the OS lock/unlock event hooks to work properly either.

### How it works (WindowPositionPersisterConsole)
* When invoked passing "persist", it caches (to a temp file) all window positions.  When invoked passing "restore",  it restores them.  It would be a beautiful thing if the caching could take place automatically at session lock/unlock instead of having to be invoked manually, but I couldn't get that to work (see WindowPositionPersisterMain above).
* I created powershell functions that I use to invoke these (before locking desktop and after) since I always have a powershell window open, but you could use a desktop shortcut or batch file.

### Dependency: .Net Framework v4.7