// Decompiled with JetBrains decompiler
// Type: DuckGame.Portal
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Portal : Thing
    {
        private PortalGun _gun;
        private List<PortalDoor> _doors = new List<PortalDoor>();
        private Layer _fakeLayer = new Layer("FAKE");
        private HashSet<PortalDrawTransformer> _inPortal = new HashSet<PortalDrawTransformer>();

        public PortalGun gun => _gun;

        public Portal(PortalGun Gun)
          : base()
        {
            _gun = Gun;
        }

        public IEnumerable<MaterialThing> CheckRectAll(
          Vec2 TopLeft,
          Vec2 BottomRight)
        {
            List<MaterialThing> source = new List<MaterialThing>();
            foreach (PortalDoor door in _doors)
            {
                if (Collision.Rect(TopLeft, BottomRight, door.rect))
                {
                    foreach (Block t in door.collision)
                    {
                        if (Collision.Rect(TopLeft, BottomRight, t))
                            source.Add(t);
                    }
                }
            }
            return source.AsEnumerable<MaterialThing>();
        }

        public List<PortalDoor> GetDoors() => _doors;

        public PortalDoor GetOtherDoor(PortalDoor door) => door == _doors[0] ? _doors[1] : _doors[0];

        public void AddPortalDoor(PortalDoor door)
        {
            if (_doors.Count > 1)
            {
                PortalDoor door1 = _doors[0];
                door1.point1 = door.point1;
                door1.point2 = door.point2;
                door1.center = door.center;
                door1.horizontal = door.horizontal;
                door = door1;
                _doors.Reverse();
            }
            else
                _doors.Add(door);
            door.collision.Clear();
            if (door.horizontal)
                return;
            AutoBlock autoBlock1 = Level.CheckLine<AutoBlock>(door.point2 + new Vec2(-8f, 0f), door.point2 + new Vec2(8f, 0f));
            if (autoBlock1 != null)
            {
                Vec2 topLeft = autoBlock1.topLeft;
                if (topLeft.y < door.bottom)
                    topLeft.y = door.bottom;
                float hi = autoBlock1.bottom - topLeft.y;
                if (hi < 8.0)
                    hi = 8f;
                door.collision.Add(new Block(topLeft.x, topLeft.y, autoBlock1.width, hi));
            }
            AutoBlock autoBlock2 = Level.CheckLine<AutoBlock>(door.point1 + new Vec2(-8f, 0f), door.point1 + new Vec2(8f, 0f));
            if (autoBlock2 != null)
            {
                Vec2 bottomLeft = autoBlock1.bottomLeft;
                if (bottomLeft.y > door.top)
                    bottomLeft.y = door.top;
                float hi = bottomLeft.y - autoBlock2.top;
                if (hi < 8.0)
                    hi = 8f;
                door.collision.Add(new Block(bottomLeft.x, bottomLeft.y - hi, autoBlock2.width, hi));
            }
            if (door.layer == null)
            {
                door.layer = new Layer("PORTAL", Layer.Game.depth, Layer.Game.camera)
                {
                    scissor = (Rectangle)Graphics.viewport.Bounds
                };
                Layer.Add(door.layer);
            }
            door.isLeft = Level.CheckPoint<AutoBlock>(door.center + new Vec2(-8f, 0f)) != null;
            door.rect = new Rectangle((int)door.point1.x - 8, (int)door.point1.y, 16f, (int)door.point2.y - (int)door.point1.y);
        }

        public override void Initialize()
        {
        }

        public override void Terminate()
        {
            foreach (PortalDoor door in _doors)
                Layer.Remove(door.layer);
        }

        public override void Update()
        {
            if (_doors.Count != 2)
                return;
            IEnumerable<ITeleport> teleports = null;
            foreach (PortalDoor door in _doors)
            {
                IEnumerable<ITeleport> second = door.horizontal ? Level.CheckRectAll<ITeleport>(door.point1 + new Vec2(0f, -8f), door.point2 + new Vec2(0f, 8f)) : Level.CheckRectAll<ITeleport>(door.point1 + new Vec2(-8f, 0f), door.point2 + new Vec2(8f, 0f));
                teleports = teleports != null ? teleports.Concat<ITeleport>(second) : second;
            }
            List<PortalDrawTransformer> portalDrawTransformerList = new List<PortalDrawTransformer>();
            foreach (PortalDrawTransformer portalDrawTransformer in _inPortal)
            {
                if (!teleports.Contains<ITeleport>(portalDrawTransformer.thing as ITeleport))
                    portalDrawTransformerList.Add(portalDrawTransformer);
            }
            foreach (PortalDrawTransformer portalDrawTransformer in portalDrawTransformerList)
            {
                _inPortal.Remove(portalDrawTransformer);
                portalDrawTransformer.thing.portal = null;
                portalDrawTransformer.thing.layer = Layer.Game;
                foreach (PortalDoor door in _doors)
                    door.layer.Remove(portalDrawTransformer);
            }
            foreach (ITeleport teleport in teleports)
            {
                ITeleport t = teleport;
                if (_inPortal.FirstOrDefault<PortalDrawTransformer>(v => v.thing == t) == null)
                {
                    PortalDrawTransformer portalDrawTransformer = new PortalDrawTransformer(t as Thing, this);
                    _inPortal.Add(portalDrawTransformer);
                    (t as Thing).portal = this;
                    (t as Thing).layer = _fakeLayer;
                    foreach (PortalDoor door in _doors)
                        door.layer.Add(portalDrawTransformer);
                }
            }
            foreach (PortalDoor door in _doors)
            {
                door.Update();
                foreach (PortalDrawTransformer portalDrawTransformer in _inPortal)
                {
                    if (door.isLeft && portalDrawTransformer.thing.x < door.center.x)
                    {
                        Thing thing = portalDrawTransformer.thing;
                        thing.position += (GetOtherDoor(door).center - door.center);
                    }
                    else if (!door.isLeft && portalDrawTransformer.thing.x > door.center.x)
                    {
                        Thing thing = portalDrawTransformer.thing;
                        thing.position += (GetOtherDoor(door).center - door.center);
                    }
                }
            }
        }

        public override void Draw()
        {
            foreach (PortalDoor door in _doors)
                door.Draw();
        }
    }
}
