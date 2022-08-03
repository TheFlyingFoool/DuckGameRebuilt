// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeGroup
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class ChallengeGroup
    {
        public List<string> challenges = new List<string>();
        public List<string> required = new List<string>();
        public int trophiesRequired;
        public string name;

        public string GetNameForDisplay() => name.ToUpperInvariant();
    }
}
