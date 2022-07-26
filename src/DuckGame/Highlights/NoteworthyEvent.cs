// Decompiled with JetBrains decompiler
// Type: DuckGame.NoteworthyEvent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NoteworthyEvent
    {
        public static string GoodKillDeathRatio = nameof(GoodKillDeathRatio);
        public static string BadKillDeathRatio = nameof(BadKillDeathRatio);
        public static string ManyFallDeaths = nameof(ManyFallDeaths);
        public string eventTag;
        public Profile who;
        public float quality;

        public NoteworthyEvent(string tag, Profile owner, float q)
        {
            this.eventTag = tag;
            this.who = owner;
            this.quality = q;
        }
    }
}
