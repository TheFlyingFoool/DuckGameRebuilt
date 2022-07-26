// Decompiled with JetBrains decompiler
// Type: DuckGame.CornerDisplay
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CornerDisplay
    {
        public HUDCorner corner;
        public float slide;
        public string text;
        public bool closing;
        public Timer timer;
        public int lowTimeTick = 4;
        public FieldBinding counter;
        public int curCount;
        public int realCount;
        public int addCount;
        public float addCountWait;
        public int maxCount;
        public bool animateCount;
        public bool isControl;
        public InputProfile profile;
        public bool stack;
        public float life = 1f;
        public bool willDie;
    }
}
