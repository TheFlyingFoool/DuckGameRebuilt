namespace DuckGame
{
    public enum DuckNetError
    {
        NewSessionRequested = 0,
        ControlledDisconnect = 1,
        HostDisconnected = 2,
        ClientDisconnected = 3,
        ConnectionTimeout = 4,
        VersionMismatch = 5,
        FullServer = 6,
        InvalidLevel = 7,
        InvalidUser = 8,
        InvalidCustomHat = 9,
        GameInProgress = 10, // 0x0000000A
        Kicked = 11, // 0x0000000B
        HostIgnoredMessage = 12, // 0x0000000C
        InvalidConnectionInformation = 13, // 0x0000000D
        ModsIncompatible = 14, // 0x0000000E
        ConnectionTrouble = 15, // 0x0000000F
        ConnectionLost = 16, // 0x00000010
        EveryoneDisconnected = 17, // 0x00000011
        YourVersionTooNew = 18, // 0x00000012
        YourVersionTooOld = 19, // 0x00000013
        ParentalControls = 20, // 0x00000014
        GameNotFoundOrClosed = 21, // 0x00000015
        InvalidPassword = 22, // 0x00000016
        ClientCrashed = 23, // 0x00000017
        Banned = 24, // 0x00000018
        HostIsABlockedUser = 25, // 0x00000019
        UnknownError = 100, // 0x00000064
        None = 101, // 0x00000065
    }
}
