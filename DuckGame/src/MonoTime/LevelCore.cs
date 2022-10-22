// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class LevelCore
    {
        public Dictionary<Profile, StoredItem> _storedItems = new Dictionary<Profile, StoredItem>();
        public bool sendCustomLevels;
        private Level _nextLevel;
        private Level _currentLevel;
        public bool gameInProgress;
        public bool gameFinished;
        public bool endedGameInProgress;
        public Queue<List<DrawCall>> drawCalls = new Queue<List<DrawCall>>();
        public List<DrawCall> currentFrameCalls = new List<DrawCall>();
        public Thing currentDrawingObject;
        public bool skipFrameLog;
        public SmallFire[] firePool = new SmallFire[SmallFire.kMaxObjects];
        public int firePoolIndex;
        public List<float> _chanceGroups = new List<float>()
        {
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f
        };
        public List<float> _chanceGroups2 = new List<float>()
        {
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f,
          0f
        };
        public InputCode konamiCode = new InputCode()
        {
            triggers = new List<string>()
          {
            "UP",
            "UP",
            "DOWN",
            "DOWN",
            "LEFT",
            "RIGHT",
            "LEFT",
            "RIGHT",
            "QUACK",
            "JUMP"
          }
        };
        public InputCode konamiCodeAlternate = new InputCode()
        {
            triggers = new List<string>()
          {
            "UP|JUMP",
            "UP|JUMP",
            "DOWN",
            "DOWN",
            "LEFT",
            "RIGHT",
            "LEFT",
            "RIGHT",
            "QUACK",
            "UP|JUMP"
          }
        };

        public Level nextLevel
        {
            get => _nextLevel;
            set => _nextLevel = value;
        }

        public Level currentLevel
        {
            get => _currentLevel;
            set => _currentLevel = value;
        }
    }
}
