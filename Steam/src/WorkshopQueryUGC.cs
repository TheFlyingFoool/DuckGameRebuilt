using System.Collections.Generic;
using Steamworks;

public abstract class WorkshopQueryUGC : WorkshopQueryBase {

    public IList<string> requiredTags { get; internal set; }

    public IList<string> excludedTags { get; internal set; }

    internal WorkshopQueryUGC() {
        requiredTags = new List<string>();
        excludedTags = new List<string>();
    }

    internal override unsafe void SetQueryData() {
        base.SetQueryData();

        foreach (string tag in requiredTags)
            SteamUGC.AddRequiredTag(_handle, tag);

        foreach (string tag in excludedTags)
            SteamUGC.AddExcludedTag(_handle, tag);

    }

}
