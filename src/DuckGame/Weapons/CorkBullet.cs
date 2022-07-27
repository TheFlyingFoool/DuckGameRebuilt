// Decompiled with JetBrains decompiler
// Type: DuckGame.CorkBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CorkBullet : Bullet
    {
        private Sprite _cork;
        private CorkObject _corkObject;

        public CorkBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            this._cork = new Sprite("cork")
            {
                center = new Vec2(3f, 2.5f)
            };
        }

        public override void Update()
        {
            base.Update();
            if (!this.doneTravelling || this._corkObject != null)
                return;
            this._corkObject = new CorkObject(this.drawEnd.x - this.travelDirNormalized.x * 4f, this.drawEnd.y - this.travelDirNormalized.y * 4f, this.firedFrom);
            if (this.firedFrom != null && this.firedFrom is CorkGun)
                (this.firedFrom as CorkGun).corkObject = this._corkObject;
            Level.Add(_corkObject);
            Level.Remove(this);
        }

        public override void Draw()
        {
            Graphics.Draw(this._cork, this.drawEnd.x, this.drawEnd.y);
            base.Draw();
        }
    }
}
