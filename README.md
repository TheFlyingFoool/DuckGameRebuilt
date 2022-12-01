diff --git a/README.md b/README.md
--- README.md
+++ README.md
@@ -1,18 +1,24 @@
 # Duck Game Rebuilt
-Decompilation of DG source to fix bugs, improve compatability, archive source, and add quality of life features for devs and users.  
+Duck Game Rebuilt is a decompilation of Duck Game source with massive improvements to performance, compatibility, and quality of life features.
+
+⚙️ A list of all improvements to Duck Game Rebuilt can be seen [here](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Changelog)
+
 🖥️ To view hardware and software compatibility check the [wiki](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Architectures-and-Devices)
 
+## Installation
+
 📥 For average users go to the [releases](https://github.com/TheFlyingFoool/DuckGameRebuilt/releases) page and download the latest release.
 
 🐧 On Linux you need to install `mono` (from your package manager is fine)
 
 ## For Developers
-
 Welcome to the repo, enjoy your stay, please unfuck the code. thanks
 
 ### Building on Windows
 
+* Please note that if you are the average user **YOU DO NOT NEED TO BUILD TO PLAY REBUILT**. This is for developers or for those with different preferences.
+
 * Make sure you have [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) installed and have a functioning IDE for C#
 
 Recommended IDE: [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022)
 
@@ -22,12 +28,10 @@
 ```
 
 * Find the repo folder and launch the .sln file
 
-* In `C:\Program Files (x86)\Steam\steamapps\common`: Do `ctrl+A`, then deselect `DuckGame.exe` by holding `ctrl` and left clicking the file, then do ctrl+C`. And finally, in `DuckGames\bin` do `ctrl+V`
+* In `C:\Program Files (x86)\Steam\steamapps\common`: Do `ctrl+A`, then deselect `DuckGame.exe` by holding `ctrl` and left clicking the file, then copy the files with ctrl+C. Then, in `DuckGames\bin` paste the files with `ctrl+V`
 
-* In the `DuckGames` directory, copy over the following 16 files to `DuckGames\bin`
-
 * Build the project
 ```
 dotnet build
 ```
