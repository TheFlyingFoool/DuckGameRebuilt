// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSpawnPlayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMSpawnPlayer : NMObjectMessage
    {
        public float xpos;
        public float ypos;
        public int duckID;
        public bool isPlayerDuck;

        public NMSpawnPlayer()
        {
        }

        public NMSpawnPlayer(float xVal, float yVal, int duck, bool playerDuck, ushort objectID)
          : base(objectID)
        {
            this.xpos = xVal;
            this.ypos = yVal;
            this.duckID = duck;
            this.isPlayerDuck = playerDuck;
        }
    }
}
