# <img src="icon.png" height="32"> Duck Game Rebuilt

Duck Game Rebuilt is a decompilation of Duck Game with massive improvements to performance, compatibility, and quality of life features.

> [!TIP]
> Join [our Discord server](https://discord.gg/XkAjt744hz) if you have any questions, need help, or want to report bugs.

Notable wiki pages:

- [Frequently Asked Questions](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/FAQ)
- [A list of all improvements to Duck Game Rebuilt](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Changelog)
- [Hardware and software compatibility](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Architectures-and-Devices)

## Installation 📥

- **Windows** — subscribe to [the Steam Workshop mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3132351890).
- **Linux** — follow the [Linux Installation Guide](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/Linux-Installation-Guide).

## For Developers 🚧

Welcome to the repo, enjoy your stay, please unfuck the code. thanks

> [!NOTE]
> Your IDE will scream at you with 200+ warnings when building. That's normal.

### Prerequisites

| Platform | Required |
|---|---|
| Windows | [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) with the **".NET desktop development"** workload (includes MSBuild, NuGet, and the [.NET Framework 4.8 targeting pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)) |
| Linux | `mono-complete` from the [official mono repos](https://www.mono-project.com/download/stable/), plus the `nuget` CLI |
| All | [Steam](https://store.steampowered.com/) running at launch time; clone with `--recursive` for submodules |

### Cloning

This repository uses git submodules. Either clone with the `--recursive` flag, or run this command after a normal clone:

```bash
git submodule update --init --recursive
```

### Building on Windows

1. Open `DuckGame.sln` in Visual Studio 2022.
2. Restore NuGet packages (most IDEs do this automatically).
3. Set **DuckGame** as the startup project — not CrashWindow, FNA, Rebuilder, or anything else.
4. Build the solution with `Ctrl+Shift+B`.

### Building on GNU/Linux

1. Add the [official monoproject repos](https://www.mono-project.com/download/stable/) (unless you're firebreak, apparently).
2. Install the `mono-complete` package.
3. `cd` to the solution's directory.
4. Restore NuGet packages if your IDE hasn't:
   ```bash
   nuget restore
   ```
5. Copy DLL dependencies from `./DuckGame/lib/` into `./bin/`:
   ```bash
   mkdir ./bin/
   cp ./DuckGame/lib/* ./bin/
   ```
6. Build the solution:
   ```bash
   msbuild -m -p:Configuration=Debug
   ```

### Running

> [!IMPORTANT]
> Steam must be running before launching the game, or it will crash on startup.

- **Windows** — press `F5` in Visual Studio to launch in Debug mode.
- **Linux** — run the built executable under mono:
  ```bash
  mono ./bin/DuckGame.exe
  ```

## Contributing

Pull requests are welcome. The usual workflow:

1. Fork this repo on GitHub.
2. Create a feature branch — `fix/<topic>`, `feat/<topic>`, or `docs/<topic>` matches the naming convention recent merged PRs use.
3. Commit with a `Feat:` / `Fix:` / `Docs:` prefix in the message subject.
4. Open a PR against `master`.

For end-user questions or help, see the [wiki FAQ](https://github.com/TheFlyingFoool/DuckGameRebuilt/wiki/FAQ) or hop into the [Discord](https://discord.gg/XkAjt744hz).
