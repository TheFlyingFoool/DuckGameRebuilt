using System.Collections.Generic;

namespace DuckGame
{
    public class CrowdCore
    {
        public Mood _mood;
        public Mood _newMood;
        public float _moodWait = 1f;
        public List<List<CrowdDuck>> _members = new List<List<CrowdDuck>>();
        public int fansUsed;

        public Mood mood
        {
            get => _mood;
            set => _newMood = value;
        }
    }
}
