// Decompiled with JetBrains decompiler
// Type: DuckGame.PortalDrawTransformer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PortalDrawTransformer : Thing
    {
        private Portal _portal;
        private Thing _thing;

        public new Portal portal => this._portal;

        public Thing thing => this._thing;

        public PortalDrawTransformer(Thing t, Portal p)
          : base()
        {
            this._portal = p;
            this._thing = t;
        }

        public override void Draw()
        {
            Vec2 position = this._thing.position;
            foreach (PortalDoor door in this._portal.GetDoors())
            {
                if (Graphics.currentLayer == door.layer)
                {
                    if (door.isLeft && (double)this._thing.x > (double)door.center.x + 32.0)
                        this._thing.position = this._thing.position + (door.center - this._portal.GetOtherDoor(door).center);
                    else if (!door.isLeft && (double)this._thing.x < (double)door.center.x - 32.0)
                        this._thing.position = this._thing.position + (this._portal.GetOtherDoor(door).center - door.center);
                    this._thing.DoDraw();
                    this._thing.position = position;
                }
            }
        }
    }
}
