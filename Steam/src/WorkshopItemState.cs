using System;

[Flags]
public enum WorkshopItemState : uint {
    None = 0,
    DownloadPending = 32,
    Downloading = 16,
    NeedsUpdate = 8,
    Installed = 4,
    LegacyItem = 2,
    Subscribed = 1
}
