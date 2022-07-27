// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomChallengeDownloader
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class RandomChallengeDownloader
    {
        private static WorkshopQueryAll _currentQuery = null;
        public static bool ready = false;
        public static int numToHaveReady = 10;
        public static int numSinceLowRating = 0;
        private static List<LevelData> _readyChallenges = new List<LevelData>();
        private static int _toFetchIndex = -1;
        private static int _numFetch = 0;
        private static WorkshopItem _downloading;
        private static WorkshopQueryFilterOrder _orderMode = WorkshopQueryFilterOrder.RankedByVote;

        public static LevelData GetNextChallenge()
        {
            if (RandomChallengeDownloader._readyChallenges.Count == 0)
                return null;
            LevelData nextChallenge = RandomChallengeDownloader._readyChallenges.First<LevelData>();
            RandomChallengeDownloader._readyChallenges.RemoveAt(0);
            return nextChallenge;
        }

        public static LevelData PeekNextChallenge() => RandomChallengeDownloader._readyChallenges.Count == 0 ? null : RandomChallengeDownloader._readyChallenges.First<LevelData>();

        private static void Fetched(object sender, WorkshopQueryResult result)
        {
            if (RandomChallengeDownloader._toFetchIndex == -1)
                RandomChallengeDownloader._toFetchIndex = Rando.Int((int)(sender as WorkshopQueryAll).numResultsFetched);
            if (RandomChallengeDownloader._toFetchIndex == RandomChallengeDownloader._numFetch && Steam.DownloadWorkshopItem(result.details.publishedFile))
                RandomChallengeDownloader._downloading = result.details.publishedFile;
            RandomChallengeDownloader._currentQuery = null;
            ++RandomChallengeDownloader._numFetch;
        }

        private static void FinishedTotalQuery(object sender)
        {
            WorkshopQueryAll workshopQueryAll = sender as WorkshopQueryAll;
            if (workshopQueryAll.numResultsTotal <= 0U)
                return;
            int num = Rando.Int((int)(workshopQueryAll.numResultsTotal / 50U)) + 1;
            if (RandomChallengeDownloader.numSinceLowRating > 3)
                RandomChallengeDownloader.numSinceLowRating = 0;
            else
                num %= 10;
            RandomChallengeDownloader._orderMode = RandomChallengeDownloader.numSinceLowRating != 2 ? WorkshopQueryFilterOrder.RankedByVote : WorkshopQueryFilterOrder.RankedByTrend;
            if (num == 0)
                num = 1;
            ++RandomChallengeDownloader.numSinceLowRating;
            WorkshopQueryAll queryAll = Steam.CreateQueryAll(RandomChallengeDownloader._orderMode, WorkshopType.Items);
            queryAll.requiredTags.Add("Arcade Machine");
            queryAll.ResultFetched += new WorkshopQueryResultFetched(RandomChallengeDownloader.Fetched);
            queryAll.page = (uint)num;
            queryAll.justOnePage = true;
            queryAll.Request();
        }

        private static void SearchDirLevels(string dir, LevelLocation location)
        {
            foreach (string path in location == LevelLocation.Content ? Content.GetFiles(dir) : DuckFile.GetFiles(dir, "*.*"))
                RandomChallengeDownloader.ProcessChallenge(path);
            foreach (string dir1 in location == LevelLocation.Content ? Content.GetDirectories(dir) : DuckFile.GetDirectories(dir))
                RandomChallengeDownloader.SearchDirLevels(dir1, location);
        }

        private static void ProcessChallenge(string path) // removed , LevelLocation loation
        {
            Main.SpecialCode = "Loading Challenge " + path != null ? path : "null";
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
                    RandomChallengeDownloader._readyChallenges.Add(levelData);
                    DevConsole.Log(DCSection.Steam, "Downloaded random challenge " + RandomChallengeDownloader._readyChallenges.Count.ToString() + "/" + RandomChallengeDownloader.numToHaveReady.ToString());
                }
                else
                    DevConsole.Log(DCSection.Steam, "Downloaded challenge had incompatible mods, and was ignored!");
            }
            catch (Exception)
            {
            }
        }

        public static void Update()
        {
            if (!Steam.IsInitialized() || !Network.isServer)
                return;
            if (RandomChallengeDownloader._downloading != null)
            {
                if (!RandomChallengeDownloader._downloading.finishedProcessing)
                    return;
                if (RandomChallengeDownloader._downloading.downloadResult == SteamResult.OK)
                    RandomChallengeDownloader.SearchDirLevels(RandomChallengeDownloader._downloading.path, LevelLocation.Workshop);
                RandomChallengeDownloader._downloading = null;
            }
            else
            {
                if (RandomChallengeDownloader._currentQuery != null || RandomChallengeDownloader._readyChallenges.Count >= RandomChallengeDownloader.numToHaveReady)
                    return;
                RandomChallengeDownloader._toFetchIndex = -1;
                RandomChallengeDownloader._numFetch = 0;
                RandomChallengeDownloader._currentQuery = Steam.CreateQueryAll(RandomChallengeDownloader._orderMode, WorkshopType.Items);
                RandomChallengeDownloader._currentQuery.requiredTags.Add("Arcade Machine");
                RandomChallengeDownloader._currentQuery.QueryFinished += new WorkshopQueryFinished(RandomChallengeDownloader.FinishedTotalQuery);
                RandomChallengeDownloader._currentQuery.fetchedData = WorkshopQueryData.TotalOnly;
                RandomChallengeDownloader._currentQuery.Request();
                DevConsole.Log(DCSection.Steam, "Querying for random Challenges.");
            }
        }
    }
}
