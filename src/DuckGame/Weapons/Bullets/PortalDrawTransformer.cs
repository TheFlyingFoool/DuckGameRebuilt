// Decompiled with JetBrains decompiler
// Type: DuckGame.PortalDrawTransformer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PortalDrawTransformer : Thing
    {
        private Portal _portal;
        private Thing _thing;

        public new Portal portal => _portal;

        public Thing thing => _thing;

        public PortalDrawTransformer(Thing t, Portal p)
          : base()
        {
            _portal = p;
            _thing = t;
        }

        public override void Draw()
        {
            Vec2 position = _thing.position;
            foreach (PortalDoor door in _portal.GetDoors())
            {
                if (Graphics.currentLayer == door.layer)
                {
                    if (door.isLeft && _thing.x > door.center.x + 32.0)
                        _thing.position += (door.center - _portal.GetOtherDoor(door).center);
                    else if (!door.isLeft && _thing.x < door.center.x - 32.0)
                        _thing.position += (_portal.GetOtherDoor(door).center - door.center);
                    _thing.DoDraw();
                    _thing.position = position;
                }
            }
        }
    }
}
