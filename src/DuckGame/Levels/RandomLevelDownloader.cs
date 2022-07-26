// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelDownloader
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

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
        private static object _currentWorkshopLevelQuery = (object)null;
        private static float _fetchDelay = 0.0f;
        private static int _totalMaps;
        private static WorkshopQueryFilterOrder _orderMode = WorkshopQueryFilterOrder.RankedByVote;

        public static LevelData GetNextLevel()
        {
            if (RandomLevelDownloader._readyLevels.Count == 0)
                return (LevelData)null;
            LevelData nextLevel = RandomLevelDownloader._readyLevels.First<LevelData>();
            RandomLevelDownloader._readyLevels.RemoveAt(0);
            return nextLevel;
        }

        public static LevelData PeekNextLevel() => RandomLevelDownloader._readyLevels.Count == 0 ? (LevelData)null : RandomLevelDownloader._readyLevels.First<LevelData>();

        private static void Fetched(object sender, WorkshopQueryResult result)
        {
            RandomLevelDownloader._fetchDelay = 0.0f;
            if (RandomLevelDownloader._currentWorkshopLevelQuery == null || RandomLevelDownloader._currentWorkshopLevelQuery != sender)
            {
                RandomLevelDownloader._numFetch = 0;
                RandomLevelDownloader._toFetchIndex = Rando.Int((int)(sender as WorkshopQueryAll).numResultsFetched - 1);
                RandomLevelDownloader._currentWorkshopLevelQuery = sender;
            }
            if (RandomLevelDownloader._toFetchIndex == RandomLevelDownloader._numFetch)
            {
                if (Global.data.blacklist.Contains(result.details.publishedFile.id))
                {
                    if (RandomLevelDownloader._numFetch < 49)
                        ++RandomLevelDownloader._numFetch;
                }
                else if (Steam.DownloadWorkshopItem(result.details.publishedFile))
                    RandomLevelDownloader._downloadingItems.Add(result.details.publishedFile);
                else
                    RandomLevelDownloader.ProcessWorkshopItem(result.details.publishedFile);
            }
            ++RandomLevelDownloader._numFetch;
        }

        public static void ProcessWorkshopItem(WorkshopItem pItem) => RandomLevelDownloader.ProcessLevel(pItem.path);

        public static void DownloadRandomMap()
        {
            int num = Rando.Int(RandomLevelDownloader._totalMaps / 50) + 1;
            if (RandomLevelDownloader.numSinceLowRating > 3)
            {
                RandomLevelDownloader.numSinceLowRating = 0;
                if ((double)Rando.Float(1f) > 0.800000011920929)
                    num %= 100;
            }
            else
            {
                num %= 12;
                if ((double)Rando.Float(1f) > 0.800000011920929)
                    num %= 30;
            }
            RandomLevelDownloader._orderMode = RandomLevelDownloader.numSinceLowRating != 2 ? WorkshopQueryFilterOrder.RankedByVote : WorkshopQueryFilterOrder.RankedByTrend;
            if ((double)Rando.Float(1f) > 0.699999988079071)
            {
                switch (Rando.Int(5))
                {
                    case 0:
                        RandomLevelDownloader._orderMode = WorkshopQueryFilterOrder.FavoritedByFriendsRankedByPublicationDate;
                        break;
                    case 1:
                        RandomLevelDownloader._orderMode = WorkshopQueryFilterOrder.CreatedByFriendsRankedByPublicationDate;
                        break;
                    default:
                        RandomLevelDownloader._orderMode = WorkshopQueryFilterOrder.RankedByTotalUniqueSubscriptions;
                        break;
                }
            }
            if (num == 0)
                num = 1;
            ++RandomLevelDownloader.numSinceLowRating;
            WorkshopQueryAll queryAll = Steam.CreateQueryAll(RandomLevelDownloader._orderMode, WorkshopType.Items);
            queryAll.requiredTags.Add("Deathmatch");
            queryAll.excludedTags.Add("Exclude From Random");
            queryAll.ResultFetched += new WorkshopQueryResultFetched(RandomLevelDownloader.Fetched);
            queryAll.page = (uint)num;
            queryAll.justOnePage = true;
            queryAll.Request();
            RandomLevelDownloader._fetchDelay = 5f;
        }

        private static void FinishedTotalQuery(object sender)
        {
            WorkshopQueryAll workshopQueryAll = sender as WorkshopQueryAll;
            if (workshopQueryAll.numResultsTotal <= 0U)
                return;
            RandomLevelDownloader._totalMaps = (int)workshopQueryAll.numResultsTotal;
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
                    RandomLevelDownloader._readyLevels.Add(levelData);
                    DevConsole.Log(DCSection.Steam, "Downloaded random level " + RandomLevelDownloader._readyLevels.Count.ToString() + "/" + RandomLevelDownloader.numToHaveReady.ToString());
                }
                else
                    DevConsole.Log(DCSection.Steam, "Downloaded level had incompatible mods, and was ignored!");
            }
            catch (Exception ex)
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
                RandomLevelDownloader.GetLevelList(pItemPath, pLevels);
            return pLevels;
        }

        public static void Update()
        {
            RandomLevelDownloader._fetchDelay = Lerp.Float(RandomLevelDownloader._fetchDelay, 0.0f, Maths.IncFrameTimer());
            if (!Steam.IsInitialized() || !Network.isServer || TeamSelect2.GetSettingInt("workshopmaps") <= 0)
                return;
            if (RandomLevelDownloader._downloadingItems.Count > 0)
            {
                for (int index = 0; index < RandomLevelDownloader._downloadingItems.Count; ++index)
                {
                    if (RandomLevelDownloader._downloadingItems[index].finishedProcessing)
                    {
                        if (RandomLevelDownloader._downloadingItems[index].downloadResult == SteamResult.OK)
                        {
                            List<string> levelList = RandomLevelDownloader.GetLevelList(RandomLevelDownloader._downloadingItems[index].path);
                            RandomLevelDownloader.ProcessLevel(levelList[Rando.Int(levelList.Count - 1)]);
                        }
                        RandomLevelDownloader._downloadingItems.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (RandomLevelDownloader._readyLevels.Count >= RandomLevelDownloader.numToHaveReady)
                    return;
                if (RandomLevelDownloader._totalMaps == 0)
                {
                    RandomLevelDownloader._toFetchIndex = -1;
                    RandomLevelDownloader._numFetch = 0;
                    WorkshopQueryAll queryAll = Steam.CreateQueryAll(RandomLevelDownloader._orderMode, WorkshopType.Items);
                    queryAll.requiredTags.Add("Deathmatch");
                    queryAll.excludedTags.Add("Exclude From Random");
                    queryAll.QueryFinished += new WorkshopQueryFinished(RandomLevelDownloader.FinishedTotalQuery);
                    queryAll.fetchedData = WorkshopQueryData.TotalOnly;
                    queryAll.Request();
                    RandomLevelDownloader._totalMaps = -1;
                    DevConsole.Log(DCSection.Steam, "Querying for random levels.");
                }
                else
                {
                    if (RandomLevelDownloader._totalMaps == -1 || (double)RandomLevelDownloader._fetchDelay > 0.0)
                        return;
                    RandomLevelDownloader.DownloadRandomMap();
                }
            }
        }
    }
}
