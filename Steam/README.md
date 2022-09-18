# Duck Game `Steam.dll` for Linux
### Proxy `Steam.dll` into [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET)
##### MIT-licensed
----

### But... why?
When trying to run Duck Game on Linux natively with [XnaToFna](https://github.com/0x0ade/XnaToFna), Duck Game will try to access the Steam API (Steamworks) through its `Steam.dll`.
The only issue with that is... that it's a mixed-mode assembly, thus restricted to Windows.
Furthermore, it's restricted to x86, which isn't that horrible for XNA games, but FNA supports making use of all 64 bits in your CPU(tm).

This `Steam.dll` replacement just tries to "proxy" any `Steam.dll` calls to Steamworks.NET, which supports all Steam platforms: x86 and x64 Windows, Linux and macOS.

### Notes:
* When using XnaToFna, you'll need to pass `-nothreading -nomods`.
* On Linux, Duck Game will complain about missing paths as it forgets to create the save profile directories. Currently working on a separate fix.
* _It's not a complete replacement yet_, it'll be outdated with future Duck Game updates and I'm stuck on a 1G connection right now, which makes testing this a pain in the a~
* You need to copy the proper files from the Steamworks.NET directory to the Duck Game directory. For Linux, you need to rename `Steamworks.NET-linux.dll` to just `Steamworks.NET.dll`. The x64 variants of steam_api and CSteamworks are supplied.
* You should be able to use a stubbed Steamworks.NET to get Duck Game running "DRM-free" on non-Windows platforms. I've uploaded one for FNADroid, it should just work(tm): https://github.com/0x0ade/FNADroid/blob/master/FNADroid/libs/managed/Steamworks.NET.dll
