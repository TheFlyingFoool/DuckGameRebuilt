# Duck Game Rebuilt
Duck Game Rebuilt is a decompilation of Duck Game with massive improvements to performance, compatibility, and quality of life features.

Join [our Discord server](https://discord.gg/XkAjt744hz) if you have any questions, need help, or want to report bugs

Notable wiki pages:
* [Frequently Asked Questions](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/FAQ)
* [A list of all improvements to Duck Game Rebuilt](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Changelog)
* [Hardware and software compatibility](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Architectures-and-Devices)

## Installation

For **Windows** users, download the [Latest Release](https://github.com/TheFlyingFoool/DuckGameRebuilt/releases), and run the executable.

For **Linux** users, follow the [Linux Installation Guide](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Linux-Installation-Guide)

## For Developers
Welcome to the repo, enjoy your stay, please unfuck the code. thanks

Note: your IDE will scream at you with 200+ warnings when building, which is normal

### Building on Windows

* Make sure you have [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) installed and have a functioning IDE (like [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022)) for C#
* Make sure the Startup Program leads to the exe produced in the ./bin folder after compiling
* Restore the NuGet packages (most IDEs automatically do this anyway)
* Build the solution with `Ctrl+Shift+B`
* Run the game in Debug Mode with `F5` (will crash unless Steam is running)
  * Make sure the currently selected project is DuckGame and not CrashWindow/FNA/anything else

### Building on GNU/Linux

* Add the [official monoproject repos](https://www.mono-project.com/download/stable/) (unless you're firebreak appearantly)
* Install the `mono-complete` package<!-- * Install the `msbuild` package ..I think msbuild is a dependency of mono-complete -->
* `cd` to the solution's directory
* Restore the NuGet packages if your IDE hasn't
  * `nuget restore`
* Add missing DLL dependencies from Windows located in ./DuckGame/lib/
  * `mkdir ./bin/`
  * `cp ./DuckGame/lib/* ./bin/`
* Build the solution
  * `msbuild -m -p:Configuration=Debug`
* Run the game (will crash unless Steam is currently running)
  * `mono ./bin/DuckGame.exe`
