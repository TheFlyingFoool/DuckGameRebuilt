// Decompiled with JetBrains decompiler
// Type: DuckGame.CrowdCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
