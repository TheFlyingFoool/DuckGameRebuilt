using System;
using System.Collections.Generic;
using Steamworks;

public class WorkshopQueryFileDetails : WorkshopQueryBase {

    public IList<ulong> files { get; internal set; }

    public WorkshopQueryFileDetails() {
        files = new List<ulong>();
    }

    internal unsafe override void Create() {
        _handle = SteamUGC.CreateQueryUGCDetailsRequest(_.GetArray(files, id => new PublishedFileId_t(id)), (uint) files.Count);
    }

}
