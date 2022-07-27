// Decompiled with JetBrains decompiler
// Type: DuckGame.TSData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class TSData
    {
        public int fingerId;
        public Vec2 touchXY;
        public int diameterX;
        public int diameterY;
        public int rotationAngle;
        public long msTimeElapsed;

        public TSData(int initValue)
        {
            this.fingerId = initValue;
            this.touchXY.x = this.touchXY.y = initValue;
            this.diameterX = initValue;
            this.diameterY = initValue;
            this.rotationAngle = initValue;
            this.msTimeElapsed = initValue;
        }
    }
}
