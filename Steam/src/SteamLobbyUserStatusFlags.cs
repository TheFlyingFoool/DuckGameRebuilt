using System;

[Flags]
public enum SteamLobbyUserStatusFlags {
    Banned = 16,
    Kicked = 8,
    Disconnected = 4,
    Entered = 1,
    Left = 2
}
