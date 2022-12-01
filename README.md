# Duck Game Rebuilt
Duck Game Rebuilt is a decompilation of Duck Game source with massive improvements to performance, compatibility, and quality of life features.

⚙️ A list of all improvements to Duck Game Rebuilt can be seen [here](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Changelog)

🖥️ To view hardware and software compatibility check the [wiki](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Architectures-and-Devices)

### Make sure to join the [Discord](https://discord.gg/XkAjt744hz)!

## Installation

📥 For average users go to the [releases](https://github.com/TheFlyingFoool/DuckGameRebuilt/releases) page and download the latest release.

🐧 On Linux you need to install `mono` (from your package manager is fine)

## For Developers
Welcome to the repo, enjoy your stay, please unfuck the code. thanks

### Building on Windows

* Please note that if you are the average user **YOU DO NOT NEED TO BUILD TO PLAY REBUILT**. This is for developers or for those with different preferences.

* Make sure you have [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) installed and have a functioning IDE for C#

Recommended IDE: [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022)

* Clone The Repository
```
git clone https://www.github.com/nikled/duckgames
```

* Find the repo folder and launch the .sln file

* In `C:\Program Files (x86)\Steam\steamapps\common\Duck Game`: Do `ctrl+A` to select all files, then deselect `DuckGame.exe` by holding `ctrl` and left clicking the file, then copy the files with ctrl+C. Then, in `DuckGames\bin` paste the files with `ctrl+V`

* Build the project
```
dotnet build
```

* \[OPTIONAL FOR BETTER DEBUGGING\] Set Startup program to the exe produced in the bin

<img src="https://user-images.githubusercontent.com/22122579/182766499-9b46ee7a-1291-4fbc-8c3e-7d7467ab8411.png" width="500">

### Building on GNU/Linux

* add official up to date mono repos from monoproject: https://www.mono-project.com/

* Install the packages `mono-complete` and `msbuild`

* Copy the following DLLs from root into the bin folder like so:
  ```
  cp System.Memory.dll bin/
  cp System.Buffers.dll bin/
  cp System.Runtime.CompilerServices.Unsafe.dll bin/
  cp System.Speech.dll bin/
  cp PresentationFramework.dll bin/
  ```

* Go to your own DG steam somewhere like `~/.steam/steam/steamapps/common/Duck Game/` and copy the Content folder to the `bin/` directory.

* Now finally you can run the build command, `msbuild`  
_Note: you may get over 200 warnings, but don't worry about those. give yourself a pat on the back. you did it._

* When you are finished and want to use the output build for release `msbuild -p:Configuration=Release`

* \[Optional\] You may wish to use an IDE such as [MonoDevelop](https://www.monodevelop.com/) for working on the project.
