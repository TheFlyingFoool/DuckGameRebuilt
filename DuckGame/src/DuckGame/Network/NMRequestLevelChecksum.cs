using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMRequestLevelChecksum : NMConditionalEvent
    {
        public string level;
        public uint checksum;
        public int levelIndex;

        public NMRequestLevelChecksum()
        {
        }

        public NMRequestLevelChecksum(string pLevel, uint pChecksum, int pLevelIndex)
        {
            level = pLevel;
            checksum = pChecksum;
            levelIndex = pLevelIndex;
        }

        public override bool Update() => Level.current.networkIndex >= levelIndex || Math.Abs(Level.current.networkIndex - levelIndex) > 100;

        public override void Activate()
        {
            if (!(Level.current is GameLevel) || Level.current.networkIndex != levelIndex)
                return;
            List<LevelData> allLevels = Content.GetAllLevels(level);
            foreach (LevelData levelData2 in allLevels)
            {
                if ((int)levelData2.GetChecksum() == (int)checksum)
                {
                    LevelData levelData1 = levelData2;
                    break;
                }
            }
            LevelData levelData3 = null;
            (Level.current as XMLLevel)._level = level;
            if (levelData3 == null)
            {
                ++DuckNetwork.core.levelTransferSession;
                DuckNetwork.core.compressedLevelData = null;
                DuckNetwork.core.levelTransferSize = 0;
                DuckNetwork.core.levelTransferProgress = 0;
                Send.Message(new NMClientNeedsLevelData(Level.current.networkIndex, DuckNetwork.core.levelTransferSession), connection);
            }
            else
            {
                (Level.current as GameLevel).data = levelData3;
                Level.current.waitingOnNewData = false;
            }
        }
    }
}
