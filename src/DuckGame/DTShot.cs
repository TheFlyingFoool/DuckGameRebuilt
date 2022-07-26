// Decompiled with JetBrains decompiler
// Type: DuckGame.DTShot
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DTShot : DestroyType
    {
        private Bullet _bullet;

        public Bullet bullet => this._bullet;

        public Thing bulletOwner => this._bullet == null ? (Thing)null : this._bullet.owner;

        public Thing bulletFiredFrom => this._bullet == null ? (Thing)null : this._bullet.firedFrom;

        public DTShot(Bullet b)
          : base((Thing)b)
        {
            this._bullet = b;
        }
    }
}
