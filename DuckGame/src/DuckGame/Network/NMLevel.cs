using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class NMLevel : NMEvent
    {
        public string level;
        public new byte levelIndex;
        public int seed;
        public bool needsChecksum;
        public uint checksum;
        private Level _level;
        private bool _connectionFailure;

        public NMLevel() => manager = BelongsToManager.EventManager;

        public NMLevel(Level pLevel)
        {
            manager = BelongsToManager.EventManager;
            _level = pLevel;
            level = pLevel.networkIdentifier;
            levelIndex = pLevel.networkIndex = DuckNetwork.levelIndex;
            if (pLevel is GameLevel)
            {
                GameLevel gameLevel = pLevel as GameLevel;
                if (gameLevel.clientLevel)
                {
                    needsChecksum = true;
                    checksum = 0U;
                    pLevel.waitingOnNewData = true;
                }
                else if (gameLevel.customLevel)
                {
                    needsChecksum = true;
                    checksum = gameLevel.checksum;
                    DuckNetwork.compressedLevelData = new MemoryStream(gameLevel.compressedData, 0, gameLevel.compressedData.Length, false, true);
                    DuckNetwork.compressedLevelName = gameLevel.displayName;
                }
            }
            if (pLevel is XMLLevel)
                seed = (pLevel as XMLLevel).seed;
            _level.levelMessages.Add(this);
        }

        public override void CopyTo(NetMessage pMessage)
        {
            _level.levelMessages.Add(pMessage as NMLevel);
            (pMessage as NMLevel).levelIndex = levelIndex;
            (pMessage as NMLevel).level = level;
            (pMessage as NMLevel)._level = _level;
            (pMessage as NMLevel).needsChecksum = needsChecksum;
            (pMessage as NMLevel).seed = seed;
            base.CopyTo(pMessage);
        }

        public override bool MessageIsCompleted()
        {
            if (_connectionFailure || !_level.initialized && DuckNetwork.levelIndex == levelIndex)
                return false;
            DevConsole.Log(DCSection.DuckNet, "|DGORANGE|Loading new level.. (" + levelIndex.ToString() + ").");
            return true;
        }

        public bool ChecksumsFinished()
        {
            if (!needsChecksum)
                return true;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection != DuckNetwork.localConnection && !_level.HasChecksumReply(profile.connection))
                    return false;
            }
            return true;
        }

        public bool OnLevelLoaded()
        {
            if (DuckNetwork.levelIndex != levelIndex || !ChecksumsFinished())
                return false;
            Level.current.things.RefreshState();
            GhostManager.context.RefreshGhosts();
            if (connection != null)
            {
                Send.Message(new NMLevelDataBegin(_level.networkIndex), connection);
                GhostManager.context.UpdateGhostSync(connection, false, true, NetMessagePriority.ReliableOrdered);
                Send.Message(new NMLevelData(_level), connection);
                _level.SendLevelData(connection);
            }
            return true;
        }

        public override void Activate()
        {
            if (DuckNetwork.status != DuckNetStatus.Connected)
            {
                if (_connectionFailure)
                    return;
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Game started during connection."));
                _connectionFailure = true;
            }
            else
            {
                Network.ContextSwitch(levelIndex);
                if (level == "@TEAMSELECT")
                    _level = new TeamSelect2(true);
                else if (level == "@ROCKINTRO")
                {
                    GameMode.numMatchesPlayed = 0;
                    _level = new RockIntro(null);
                }
                else if (level == "@ROCKTHROW|SHOWSCORE")
                    _level = new RockScoreboard();
                else if (level == "@ROCKTHROW|SHOWWINNER")
                    _level = new RockScoreboard(mode: ScoreBoardMode.ShowWinner);
                else if (level == "@ROCKTHROW|SHOWEND")
                {
                    Graphics.fade = 0f;
                    _level = new RockScoreboard(mode: ScoreBoardMode.ShowWinner, afterHighlights: true);
                }
                else
                {
                    if (needsChecksum)
                    {
                        GameLevel gameLevel = new GameLevel(level, seed);
                        if (level.EndsWith(".client"))
                        {
                            int num = 0;
                            try
                            {
                                num = Convert.ToInt32(level[0].ToString() ?? "");
                            }
                            catch (Exception)
                            {
                            }
                            int networkIndex = DuckNetwork.GetProfiles(DuckNetwork.localConnection)[0].networkIndex;
                            if (num == networkIndex)
                            {
                                string str1 = Deathmatch.RandomLevelString("", "deathmatch", true);
                                if (str1 != "")
                                {
                                    string str2 = str1.Substring(0, str1.Length - 7);
                                    LevelData level = Content.GetLevel(str2);
                                    uint checksum = level.GetChecksum();
                                    byte[] compressedLevelData = XMLLevel.GetCompressedLevelData(level, str2);
                                    DuckNetwork.compressedLevelData = new MemoryStream(compressedLevelData, 0, compressedLevelData.Length, true, true);
                                    DuckNetwork.levelIndex = levelIndex;
                                    Send.Message(new NMRequestLevelChecksum(str2, checksum, levelIndex));
                                    gameLevel.data = level;
                                    gameLevel.waitingOnNewData = false;
                                    DuckNetwork.compressedLevelName = gameLevel.displayName;
                                }
                            }
                            else
                            {
                                gameLevel.waitingOnNewData = true;
                                gameLevel.networkIndex = levelIndex;
                            }
                            _level = gameLevel;
                        }
                        else
                        {
                            List<LevelData> allLevels = Content.GetAllLevels(level);
                            LevelData levelData1 = null;
                            foreach (LevelData levelData2 in allLevels)
                            {
                                if ((int)levelData2.GetChecksum() == (int)checksum)
                                {
                                    levelData1 = levelData2;
                                    break;
                                }
                            }
                            if (levelData1 == null || NetworkDebugger.enabled)
                            {
                                ++DuckNetwork.core.levelTransferSession;
                                DuckNetwork.core.compressedLevelData = null;
                                DuckNetwork.core.levelTransferSize = 0;
                                DuckNetwork.core.levelTransferProgress = 0;
                                Send.Message(new NMClientNeedsLevelData(levelIndex, DuckNetwork.core.levelTransferSession), connection);
                                gameLevel.waitingOnNewData = true;
                                gameLevel.networkIndex = levelIndex;
                            }
                            else
                            {
                                gameLevel.data = levelData1;
                                Send.Message(new NMLevelFileReady(levelIndex), connection);
                            }
                            _level = gameLevel;
                        }
                    }
                    else
                        _level = new GameLevel(level, seed);
                    if (_level != null && _level is XMLLevel)
                        (_level as XMLLevel).seed = seed;
                }
                if (Network.inLobby || Level.current is RockScoreboard && !(_level is RockScoreboard))
                    Music.Stop();
                Level.current = _level;
                _level.transferCompleteCalled = false;
                _level.networkIndex = levelIndex;
            }
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (level != null)
                str = str + "(" + level + ", " + seed.ToString() + ")";
            return str;
        }
    }
}
