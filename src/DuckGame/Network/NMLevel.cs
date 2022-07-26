// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public NMLevel() => this.manager = BelongsToManager.EventManager;

        public NMLevel(Level pLevel)
        {
            this.manager = BelongsToManager.EventManager;
            this._level = pLevel;
            this.level = pLevel.networkIdentifier;
            this.levelIndex = pLevel.networkIndex = DuckNetwork.levelIndex;
            if (pLevel is GameLevel)
            {
                GameLevel gameLevel = pLevel as GameLevel;
                if (gameLevel.clientLevel)
                {
                    this.needsChecksum = true;
                    this.checksum = 0U;
                    pLevel.waitingOnNewData = true;
                }
                else if (gameLevel.customLevel)
                {
                    this.needsChecksum = true;
                    this.checksum = gameLevel.checksum;
                    DuckNetwork.compressedLevelData = new MemoryStream(gameLevel.compressedData, 0, gameLevel.compressedData.Length, false, true);
                    DuckNetwork.compressedLevelName = gameLevel.displayName;
                }
            }
            if (pLevel is XMLLevel)
                this.seed = (pLevel as XMLLevel).seed;
            this._level.levelMessages.Add(this);
        }

        public override void CopyTo(NetMessage pMessage)
        {
            this._level.levelMessages.Add(pMessage as NMLevel);
            (pMessage as NMLevel).levelIndex = this.levelIndex;
            (pMessage as NMLevel).level = this.level;
            (pMessage as NMLevel)._level = this._level;
            (pMessage as NMLevel).needsChecksum = this.needsChecksum;
            (pMessage as NMLevel).seed = this.seed;
            base.CopyTo(pMessage);
        }

        public override bool MessageIsCompleted()
        {
            if (this._connectionFailure || !this._level.initialized && (int)DuckNetwork.levelIndex == (int)this.levelIndex)
                return false;
            DevConsole.Log(DCSection.DuckNet, "|DGORANGE|Loading new level.. (" + this.levelIndex.ToString() + ").");
            return true;
        }

        public bool ChecksumsFinished()
        {
            if (!this.needsChecksum)
                return true;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection != null && profile.connection != DuckNetwork.localConnection && !this._level.HasChecksumReply(profile.connection))
                    return false;
            }
            return true;
        }

        public bool OnLevelLoaded()
        {
            if ((int)DuckNetwork.levelIndex != (int)this.levelIndex || !this.ChecksumsFinished())
                return false;
            Level.current.things.RefreshState();
            GhostManager.context.RefreshGhosts();
            if (this.connection != null)
            {
                Send.Message((NetMessage)new NMLevelDataBegin(this._level.networkIndex), this.connection);
                GhostManager.context.UpdateGhostSync(this.connection, false, true, NetMessagePriority.ReliableOrdered);
                Send.Message((NetMessage)new NMLevelData(this._level), this.connection);
                this._level.SendLevelData(this.connection);
            }
            return true;
        }

        public override void Activate()
        {
            if (DuckNetwork.status != DuckNetStatus.Connected)
            {
                if (this._connectionFailure)
                    return;
                Network.DisconnectClient(DuckNetwork.localConnection, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Game started during connection."));
                this._connectionFailure = true;
            }
            else
            {
                Network.ContextSwitch(this.levelIndex);
                if (this.level == "@TEAMSELECT")
                    this._level = (Level)new TeamSelect2(true);
                else if (this.level == "@ROCKINTRO")
                {
                    GameMode.numMatchesPlayed = 0;
                    this._level = (Level)new RockIntro((Level)null);
                }
                else if (this.level == "@ROCKTHROW|SHOWSCORE")
                    this._level = (Level)new RockScoreboard();
                else if (this.level == "@ROCKTHROW|SHOWWINNER")
                    this._level = (Level)new RockScoreboard(mode: ScoreBoardMode.ShowWinner);
                else if (this.level == "@ROCKTHROW|SHOWEND")
                {
                    Graphics.fade = 0.0f;
                    this._level = (Level)new RockScoreboard(mode: ScoreBoardMode.ShowWinner, afterHighlights: true);
                }
                else
                {
                    if (this.needsChecksum)
                    {
                        GameLevel gameLevel = new GameLevel(this.level, this.seed);
                        if (this.level.EndsWith(".client"))
                        {
                            int num = 0;
                            try
                            {
                                num = Convert.ToInt32(this.level[0].ToString() ?? "");
                            }
                            catch (Exception ex)
                            {
                            }
                            int networkIndex = (int)DuckNetwork.GetProfiles(DuckNetwork.localConnection).First<Profile>().networkIndex;
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
                                    DuckNetwork.levelIndex = this.levelIndex;
                                    Send.Message((NetMessage)new NMRequestLevelChecksum(str2, checksum, (int)this.levelIndex));
                                    gameLevel.data = level;
                                    gameLevel.waitingOnNewData = false;
                                    DuckNetwork.compressedLevelName = gameLevel.displayName;
                                }
                            }
                            else
                            {
                                gameLevel.waitingOnNewData = true;
                                gameLevel.networkIndex = this.levelIndex;
                            }
                            this._level = (Level)gameLevel;
                        }
                        else
                        {
                            List<LevelData> allLevels = Content.GetAllLevels(this.level);
                            LevelData levelData1 = (LevelData)null;
                            foreach (LevelData levelData2 in allLevels)
                            {
                                if ((int)levelData2.GetChecksum() == (int)this.checksum)
                                {
                                    levelData1 = levelData2;
                                    break;
                                }
                            }
                            if (levelData1 == null || NetworkDebugger.enabled)
                            {
                                ++DuckNetwork.core.levelTransferSession;
                                DuckNetwork.core.compressedLevelData = (MemoryStream)null;
                                DuckNetwork.core.levelTransferSize = 0;
                                DuckNetwork.core.levelTransferProgress = 0;
                                Send.Message((NetMessage)new NMClientNeedsLevelData(this.levelIndex, DuckNetwork.core.levelTransferSession), this.connection);
                                gameLevel.waitingOnNewData = true;
                                gameLevel.networkIndex = this.levelIndex;
                            }
                            else
                            {
                                gameLevel.data = levelData1;
                                Send.Message((NetMessage)new NMLevelFileReady(this.levelIndex), this.connection);
                            }
                            this._level = (Level)gameLevel;
                        }
                    }
                    else
                        this._level = (Level)new GameLevel(this.level, this.seed);
                    if (this._level != null && this._level is XMLLevel)
                        (this._level as XMLLevel).seed = this.seed;
                }
                if (Network.InLobby() || Level.current is RockScoreboard && !(this._level is RockScoreboard))
                    Music.Stop();
                Level.current = this._level;
                this._level.transferCompleteCalled = false;
                this._level.networkIndex = this.levelIndex;
            }
        }

        public override string ToString()
        {
            string str = base.ToString();
            if (this.level != null)
                str = str + "(" + this.level + ", " + this.seed.ToString() + ")";
            return str;
        }
    }
}
