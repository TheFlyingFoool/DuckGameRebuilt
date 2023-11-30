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
            Triggers.Up,
            Triggers.Up,
            Triggers.Down,
            Triggers.Down,
            Triggers.Left,
            Triggers.Right,
            Triggers.Left,
            Triggers.Right,
            Triggers.Quack,
            Triggers.Jump
          }
        };
        public InputCode konamiCodeAlternate = new InputCode()
        {
            triggers = new List<string>()
          {
            "UP|JUMP",
            "UP|JUMP",
            Triggers.Down,
            Triggers.Down,
            Triggers.Left,
            Triggers.Right,
            Triggers.Left,
            Triggers.Right,
            Triggers.Quack,
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
