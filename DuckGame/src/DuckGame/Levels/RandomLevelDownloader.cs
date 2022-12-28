// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelDownloader
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class RandomLevelDownloader
    {
        public static bool ready = false;
        public static int numToHaveReady = 10;
        public static int numSinceLowRating = 0;
        private static List<LevelData> _readyLevels = new List<LevelData>();
        private static int _toFetchIndex = -1;
        private static int _numFetch = 0;
        public static List<WorkshopItem> _downloadingItems = new List<WorkshopItem>();
        private static object _currentWorkshopLevelQuery = null;
        private static float _fetchDelay = 0f;
        private static int _totalMaps;
        private static WorkshopQueryFilterOrder _orderMode = WorkshopQueryFilterOrder.RankedByVote;

        public static LevelData GetNextLevel()
        {
            if (_readyLevels.Count == 0)
                return null;
            LevelData nextLevel = _readyLevels[0];
            _readyLevels.RemoveAt(0);
            return nextLevel;
        }

        public static LevelData PeekNextLevel() => _readyLevels.Count == 0 ? null : _readyLevels[0];

        private static void Fetched(object sender, WorkshopQueryResult result)
        {
            _fetchDelay = 0f;
            if (_currentWorkshopLevelQuery == null || _currentWorkshopLevelQuery != sender)
            {
                _numFetch = 0;
                _toFetchIndex = Rando.Int((int)(sender as WorkshopQueryAll)._numResultsFetched - 1);
                _currentWorkshopLevelQuery = sender;
            }
            if (_toFetchIndex == _numFetch)
            {
                if (Global.data.blacklist.Contains(result.details.publishedFile.id))
                {
                    if (_numFetch < 49)
                        ++_numFetch;
                }
                else if (Steam.DownloadWorkshopItem(result.details.publishedFile))
                    _downloadingItems.Add(result.details.publishedFile);
                else
                    ProcessWorkshopItem(result.details.publishedFile);
            }
            ++_numFetch;
        }

        public static void ProcessWorkshopItem(WorkshopItem pItem) => ProcessLevel(pItem.path);

        public static void DownloadRandomMap()
        {
            int num = Rando.Int(_totalMaps / 50) + 1;
            if (numSinceLowRating > 3)
            {
                numSinceLowRating = 0;
                if (Rando.Float(1f) > 0.8f)
                    num %= 100;
            }
            else
            {
                num %= 12;
                if (Rando.Float(1f) > 0.8f)
                    num %= 30;
            }
            _orderMode = numSinceLowRating != 2 ? WorkshopQueryFilterOrder.RankedByVote : WorkshopQueryFilterOrder.RankedByTrend;
            if (Rando.Float(1f) > 0.7f)
            {
                switch (Rando.Int(5))
                {
                    case 0:
                        _orderMode = WorkshopQueryFilterOrder.FavoritedByFriendsRankedByPublicationDate;
                        break;
                    case 1:
                        _orderMode = WorkshopQueryFilterOrder.CreatedByFriendsRankedByPublicationDate;
                        break;
                    default:
                        _orderMode = WorkshopQueryFilterOrder.RankedByTotalUniqueSubscriptions;
                        break;
                }
            }
            if (num == 0)
                num = 1;
            ++numSinceLowRating;
            WorkshopQueryAll queryAll = Steam.CreateQueryAll(_orderMode, WorkshopType.Items);
            queryAll.requiredTags.Add("Deathmatch");
            queryAll.excludedTags.Add("Exclude From Random");
            queryAll.ResultFetched += new WorkshopQueryResultFetched(Fetched);
            queryAll._page = (uint)num;
            queryAll.justOnePage = true;
            queryAll.Request();
            _fetchDelay = 5f;
        }

        private static void FinishedTotalQuery(object sender)
        {
            WorkshopQueryAll workshopQueryAll = sender as WorkshopQueryAll;
            if (workshopQueryAll._numResultsTotal <= 0U)
                return;
            _totalMaps = (int)workshopQueryAll._numResultsTotal;
        }

        private static void ProcessLevel(string path)
        {
            Main.SpecialCode = "Loading Level " + path != null ? path : "null";
            try
            {
                if (!path.EndsWith(".lev"))
                    return;
                path = path.Replace('\\', '/');
                LevelData levelData = DuckFile.LoadLevel(path);
                levelData.SetPath(path);
                path = path.Substring(0, path.Length - 4);
                path.Substring(path.IndexOf("/levels/") + 8);
                bool flag1 = true;
                if (levelData.modData.workshopIDs.Count != 0)
                {
                    foreach (ulong workshopId in levelData.modData.workshopIDs)
                    {
                        bool flag2 = false;
                        foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                        {
                            if (accessibleMod.configuration != null && (long)accessibleMod.configuration.workshopID == (long)workshopId)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            flag1 = false;
                            break;
                        }
                    }
                }
                if (flag1 && !levelData.modData.hasLocalMods)
                {
                    _readyLevels.Add(levelData);
                    DevConsole.Log(DCSection.Steam, "Downloaded random level " + _readyLevels.Count.ToString() + "/" + numToHaveReady.ToString());
                }
                else
                    DevConsole.Log(DCSection.Steam, "Downloaded level had incompatible mods, and was ignored!");
            }
            catch (Exception)
            {
            }
        }

        private static List<string> GetLevelList(string pItemPath, List<string> pLevels = null)
        {
            if (pLevels == null)
                pLevels = new List<string>();
            foreach (string file in DuckFile.GetFiles(pItemPath, "*.lev"))
                pLevels.Add(file);
            foreach (string directory in DuckFile.GetDirectories(pItemPath))
                GetLevelList(pItemPath, pLevels);
            return pLevels;
        }

        public static void Update()
        {
            _fetchDelay = Lerp.Float(_fetchDelay, 0f, Maths.IncFrameTimer());
            if (!Steam.IsInitialized() || !Network.isServer || TeamSelect2.GetSettingInt("workshopmaps") <= 0)
                return;
            if (_downloadingItems.Count > 0)
            {
                for (int index = 0; index < _downloadingItems.Count; ++index)
                {
                    if (_downloadingItems[index].finishedProcessing)
                    {
                        if (_downloadingItems[index].downloadResult == SteamResult.OK)
                        {
                            List<string> levelList = GetLevelList(_downloadingItems[index].path);
                            ProcessLevel(levelList[Rando.Int(levelList.Count - 1)]);
                        }
                        _downloadingItems.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (_readyLevels.Count >= numToHaveReady)
                    return;
                if (_totalMaps == 0)
                {
                    _toFetchIndex = -1;
                    _numFetch = 0;
                    WorkshopQueryAll queryAll = Steam.CreateQueryAll(_orderMode, WorkshopType.Items);
                    queryAll.requiredTags.Add("Deathmatch");
                    queryAll.excludedTags.Add("Exclude From Random");
                    queryAll.QueryFinished += new WorkshopQueryFinished(FinishedTotalQuery);
                    queryAll._dataToFetch = WorkshopQueryData.TotalOnly;
                    queryAll.Request();
                    _totalMaps = -1;
                    DevConsole.Log(DCSection.Steam, "Querying for random levels.");
                }
                else
                {
                    if (_totalMaps == -1 || _fetchDelay > 0.0)
                        return;
                    DownloadRandomMap();
                }
            }
        }
    }
}
