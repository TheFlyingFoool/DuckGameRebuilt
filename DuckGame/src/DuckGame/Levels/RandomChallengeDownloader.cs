using System;
using System.Collections.Generic;

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
            if (_readyChallenges.Count == 0)
                return null;
            LevelData nextChallenge = _readyChallenges[0];
            _readyChallenges.RemoveAt(0);
            return nextChallenge;
        }

        public static LevelData PeekNextChallenge() => _readyChallenges.Count == 0 ? null : _readyChallenges[0];

        private static void Fetched(object sender, WorkshopQueryResult result)
        {
            if (_toFetchIndex == -1)
                _toFetchIndex = Rando.Int((int)(sender as WorkshopQueryAll)._numResultsFetched);
            if (_toFetchIndex == _numFetch && Steam.DownloadWorkshopItem(result.details.publishedFile))
                _downloading = result.details.publishedFile;
            _currentQuery = null;
            ++_numFetch;
        }

        private static void FinishedTotalQuery(object sender)
        {
            WorkshopQueryAll workshopQueryAll = sender as WorkshopQueryAll;
            if (workshopQueryAll._numResultsTotal <= 0U)
                return;
            int num = Rando.Int((int)(workshopQueryAll._numResultsTotal / 50U)) + 1;
            if (numSinceLowRating > 3)
                numSinceLowRating = 0;
            else
                num %= 10;
            _orderMode = numSinceLowRating != 2 ? WorkshopQueryFilterOrder.RankedByVote : WorkshopQueryFilterOrder.RankedByTrend;
            if (num == 0)
                num = 1;
            ++numSinceLowRating;
            WorkshopQueryAll queryAll = Steam.CreateQueryAll(_orderMode, WorkshopType.Items);
            queryAll.requiredTags.Add("Arcade Machine");
            queryAll.ResultFetched += new WorkshopQueryResultFetched(Fetched);
            queryAll._page = (uint)num;
            queryAll.justOnePage = true;
            queryAll.Request();
        }

        private static void SearchDirLevels(string dir, LevelLocation location)
        {
            foreach (string path in location == LevelLocation.Content ? Content.GetFiles(dir) : DuckFile.GetFiles(dir, "*.*"))
                ProcessChallenge(path);
            foreach (string dir1 in location == LevelLocation.Content ? Content.GetDirectories(dir) : DuckFile.GetDirectories(dir))
                SearchDirLevels(dir1, location);
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
                    _readyChallenges.Add(levelData);
                    DevConsole.Log(DCSection.Steam, "Downloaded random challenge " + _readyChallenges.Count.ToString() + "/" + numToHaveReady.ToString());
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
            if (_downloading != null)
            {
                if (!_downloading.finishedProcessing)
                    return;
                if (_downloading.downloadResult == SteamResult.OK)
                    SearchDirLevels(_downloading.path, LevelLocation.Workshop);
                _downloading = null;
            }
            else
            {
                if (_currentQuery != null || _readyChallenges.Count >= numToHaveReady)
                    return;
                _toFetchIndex = -1;
                _numFetch = 0;
                _currentQuery = Steam.CreateQueryAll(_orderMode, WorkshopType.Items);
                _currentQuery.requiredTags.Add("Arcade Machine");
                _currentQuery.QueryFinished += new WorkshopQueryFinished(FinishedTotalQuery);
                _currentQuery._dataToFetch = WorkshopQueryData.TotalOnly;
                _currentQuery.Request();
                DevConsole.Log(DCSection.Steam, "Querying for random Challenges.");
            }
        }
    }
}
