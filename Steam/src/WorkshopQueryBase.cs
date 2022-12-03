using System;
using Steamworks;

public delegate void WorkshopQueryFinished(object sender);
public delegate void WorkshopQueryResultFetched(object sender, WorkshopQueryResult result);

public abstract class WorkshopQueryBase : IDisposable {

    internal readonly static bool _hasSetReturnOnlyIDs = typeof(SteamUGC).GetMethod("SetReturnOnlyIDs") != null;

    internal CallResult<SteamUGCQueryCompleted_t> _completedCallResult;

    public event WorkshopQueryFinished QueryFinished;
    public event WorkshopQueryResultFetched ResultFetched;

    internal UGCQueryHandle_t _handle;
    internal ulong handle => _handle.m_UGCQueryHandle;

    public uint numResultsFetched { get; internal set; }

    public uint numResultsTotal { get; internal set; }

    public uint maxCachedTime { get; set; }

    public bool justOnePage { get; set; }

    public WorkshopQueryData fetchedData { get; set; }

    public uint page { get; set; }

    public bool onlyQueryIDs { get; set; }

    internal unsafe WorkshopQueryBase() {
        numResultsTotal = 0;
        numResultsFetched = 0;
        fetchedData = WorkshopQueryData.Details;
        maxCachedTime = 0;
        _completedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
        page = 1;
        _handle = new UGCQueryHandle_t();
    }

    unsafe ~WorkshopQueryBase() {
        Dispose(true);
    }

    internal abstract void Create();

    internal unsafe virtual void Destroy() 
    {
        SteamUGC.ReleaseQueryUGCRequest(_handle);
        _handle = new UGCQueryHandle_t();
    }

    internal unsafe virtual void SetQueryData() 
    {
        if (!Steam.initialized)
            return;
        WorkshopQueryData dataToFetch = fetchedData;
        if (dataToFetch == WorkshopQueryData.TotalOnly) {
            SteamUGC.SetReturnTotalOnly(_handle, true);
        } else {
            SteamUGC.SetReturnLongDescription(_handle, (dataToFetch & WorkshopQueryData.LongDescription) != 0);
            SteamUGC.SetReturnMetadata(_handle, (dataToFetch & WorkshopQueryData.Metadata) != 0);
            SteamUGC.SetReturnChildren(_handle, (dataToFetch & WorkshopQueryData.Children) != 0);
            SteamUGC.SetReturnAdditionalPreviews(_handle, (dataToFetch & WorkshopQueryData.AdditionalPreviews) != 0);
        }
        if (maxCachedTime != 0)
            SteamUGC.SetAllowCachedResponse(_handle, maxCachedTime);
        if (onlyQueryIDs && _hasSetReturnOnlyIDs)
            try {
                SetQueryData_SetReturnOnlyIDs(_handle, true);
            } catch (MissingMethodException) {
                // We're definitely using the stubbed Steamworks.NET now... unless someone dropped in an outdated version? But why?
            }
    }

    internal unsafe void SetQueryData_SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs) 
    {
        SteamUGC.SetReturnOnlyIDs(_handle, true);
    }

    public unsafe void Request() {
        try 
        {
            _Request();
        } catch (TypeLoadException) {
            // We're definitely using the stubbed Steamworks.NET now... unless someone dropped in an outdated version? But why?
            QueryFinished?.Invoke(this);
        }
    }

    protected unsafe virtual void _Request() 
    {
        if (handle == 0)
            Create();
        SetQueryData();
        SteamAPICall_t call = SteamUGC.SendQueryUGCRequest(_handle);
        if (call.m_SteamAPICall == 0)
            OnSteamUGCQueryCompleted(new SteamUGCQueryCompleted_t(), false);
        else
            _completedCallResult.Set(call);
    }

    private unsafe void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t queryCompleted, bool ioFailure) 
    {
        numResultsTotal = queryCompleted.m_unTotalMatchingResults;

        if (queryCompleted.m_unNumResultsReturned == 0 || fetchedData == WorkshopQueryData.TotalOnly)
        {
            QueryFinished?.Invoke(this);
            return;
        }
        numResultsFetched += queryCompleted.m_unNumResultsReturned;
        for (uint resulti = 0; resulti < queryCompleted.m_unNumResultsReturned; resulti++)
        {
            WorkshopQueryResult result = new WorkshopQueryResult();
            SteamUGC.GetQueryUGCPreviewURL(queryCompleted.m_handle, resulti, out result.previewURL, 260);

            SteamUGCDetails_t ugcDetails;
            SteamUGC.GetQueryUGCResult(queryCompleted.m_handle, resulti, out ugcDetails);
            WorkshopQueryResultDetails resultDetails = result.details = new WorkshopQueryResultDetails();

            resultDetails.acceptedForUse =ugcDetails.m_bAcceptedForUse;
            resultDetails.banned =ugcDetails.m_bBanned;
            resultDetails.description = ugcDetails.m_rgchDescription;

            resultDetails.file = ugcDetails.m_hFile.m_UGCHandle;
            resultDetails.fileName = ugcDetails.m_pchFileName;
            resultDetails.fileSize = ugcDetails.m_nFileSize;
            resultDetails.fileType = ugcDetails.m_eFileType;
            resultDetails.numChildren = ugcDetails.m_unNumChildren;
            resultDetails.previewFile = ugcDetails.m_hPreviewFile.m_UGCHandle;
            resultDetails.previewFileSize = ugcDetails.m_nPreviewFileSize;
            resultDetails.publishedFile = WorkshopItem.GetItem(ugcDetails.m_nPublishedFileId);

            resultDetails.result = ugcDetails.m_eResult;
            resultDetails.score = ugcDetails.m_flScore;
            resultDetails.steamIDOwner = ugcDetails.m_ulSteamIDOwner;
            resultDetails.tags = ugcDetails.m_rgchTags.Split(',');
            resultDetails.tagsTruncated = ugcDetails.m_bTagsTruncated;
            resultDetails.timeAddedToUserList = ugcDetails.m_rtimeAddedToUserList;
            resultDetails.timeCreated = ugcDetails.m_rtimeCreated;
            resultDetails.timeUpdated = ugcDetails.m_rtimeUpdated;
            resultDetails.title = ugcDetails.m_rgchTitle;
            resultDetails.URL = ugcDetails.m_rgchURL;
            resultDetails.visibility = ugcDetails.m_eVisibility;
            resultDetails.votesDown = ugcDetails.m_unVotesDown;
            resultDetails.votesUp = ugcDetails.m_unVotesUp;

            if ((fetchedData & WorkshopQueryData.Children) != 0)
            {
                PublishedFileId_t[] children = new PublishedFileId_t[resultDetails.numChildren];
                if (SteamUGC.GetQueryUGCChildren(queryCompleted.m_handle, resulti, children, (uint)children.Length))
                    result.fileList = _.GetArray(children, id => WorkshopItem.GetItem(id));
            }

            if ((fetchedData & WorkshopQueryData.Metadata) != 0)
            {
                SteamUGC.GetQueryUGCMetadata(queryCompleted.m_handle, resulti, out result.metadata, 260);
            }

            if ((fetchedData & WorkshopQueryData.AdditionalPreviews) != 0)
            {
                WorkshopQueryResultAdditionalPreview[] previews = result.additionalPreviews = new WorkshopQueryResultAdditionalPreview[SteamUGC.GetQueryUGCNumAdditionalPreviews(queryCompleted.m_handle, resulti)];
                for (uint previewi = 0; previewi < previews.Length; previewi++)
                {
                    string url;
                    string name;
                    EItemPreviewType type;
                    if (SteamUGC.GetQueryUGCAdditionalPreview(queryCompleted.m_handle, resulti, previewi, out url, 260, out name, 260, out type))
                        previews[previewi] = new WorkshopQueryResultAdditionalPreview(type == EItemPreviewType.k_EItemPreviewType_Image, url);
                }
            }

            if ((fetchedData & WorkshopQueryData.Statistics) != 0)
            {
                uint[] stats = result.statistics = new uint[8];
                for (WorkshopResultStatistic stat = WorkshopResultStatistic.NumSubscriptions; (int)stat < stats.Length; stat++)
                {
                    ulong val;
                    if (SteamUGC.GetQueryUGCStatistic(queryCompleted.m_handle, resulti, (EItemStatistic)stat, out val))
                        stats[(int)stat] = (uint)val;
                }
            }

            ResultFetched?.Invoke(this, result);
        }

        if (numResultsFetched == numResultsTotal || justOnePage)
        {
            QueryFinished?.Invoke(this);
        }
        else
        {
            Destroy();
            page++;
            Create();
            Request();
        }
    }

    protected virtual void Dispose(bool flag) {
        Destroy();

        _completedCallResult?.Cancel();
        (_completedCallResult as IDisposable)?.Dispose();
        _completedCallResult = null;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}
