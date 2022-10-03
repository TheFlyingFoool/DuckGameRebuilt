using Steamworks;

public class WorkshopQueryResultDetails {
    public WorkshopQueryResultDetails() {
    }

    public WorkshopItem publishedFile;

    public EResult result;

    public EWorkshopFileType fileType;

    public string title;

    public string description;

    public ulong steamIDOwner;

    public uint timeCreated;

    public uint timeUpdated;

    public uint timeAddedToUserList;

    public ERemoteStoragePublishedFileVisibility visibility;

    public bool banned;

    public bool acceptedForUse;

    public bool tagsTruncated;

    public string[] tags;

    public ulong file;

    public ulong previewFile;

    public string fileName;

    public int fileSize;

    public int previewFileSize;

    public string URL;

    public uint votesUp;

    public uint votesDown;

    public float score;

    public uint numChildren;
}
