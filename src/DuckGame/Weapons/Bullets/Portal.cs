// Decompiled with JetBrains decompiler
// Type: DuckGame.Portal
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
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

        public PortalGun gun => this._gun;

        public Portal(PortalGun Gun)
          : base()
        {
            this._gun = Gun;
        }

        public IEnumerable<MaterialThing> CheckRectAll(
          Vec2 TopLeft,
          Vec2 BottomRight)
        {
            List<MaterialThing> source = new List<MaterialThing>();
            foreach (PortalDoor door in this._doors)
            {
                if (Collision.Rect(TopLeft, BottomRight, door.rect))
                {
                    foreach (Block t in door.collision)
                    {
                        if (Collision.Rect(TopLeft, BottomRight, (Thing)t))
                            source.Add((MaterialThing)t);
                    }
                }
            }
            return source.AsEnumerable<MaterialThing>();
        }

        public List<PortalDoor> GetDoors() => this._doors;

        public PortalDoor GetOtherDoor(PortalDoor door) => door == this._doors[0] ? this._doors[1] : this._doors[0];

        public void AddPortalDoor(PortalDoor door)
        {
            if (this._doors.Count > 1)
            {
                PortalDoor door1 = this._doors[0];
                door1.point1 = door.point1;
                door1.point2 = door.point2;
                door1.center = door.center;
                door1.horizontal = door.horizontal;
                door = door1;
                this._doors.Reverse();
            }
            else
                this._doors.Add(door);
            door.collision.Clear();
            if (door.horizontal)
                return;
            AutoBlock autoBlock1 = Level.CheckLine<AutoBlock>(door.point2 + new Vec2(-8f, 0.0f), door.point2 + new Vec2(8f, 0.0f));
            if (autoBlock1 != null)
            {
                Vec2 topLeft = autoBlock1.topLeft;
                if ((double)topLeft.y < (double)door.bottom)
                    topLeft.y = door.bottom;
                float hi = autoBlock1.bottom - topLeft.y;
                if ((double)hi < 8.0)
                    hi = 8f;
                door.collision.Add(new Block(topLeft.x, topLeft.y, autoBlock1.width, hi));
            }
            AutoBlock autoBlock2 = Level.CheckLine<AutoBlock>(door.point1 + new Vec2(-8f, 0.0f), door.point1 + new Vec2(8f, 0.0f));
            if (autoBlock2 != null)
            {
                Vec2 bottomLeft = autoBlock1.bottomLeft;
                if ((double)bottomLeft.y > (double)door.top)
                    bottomLeft.y = door.top;
                float hi = bottomLeft.y - autoBlock2.top;
                if ((double)hi < 8.0)
                    hi = 8f;
                door.collision.Add(new Block(bottomLeft.x, bottomLeft.y - hi, autoBlock2.width, hi));
            }
            if (door.layer == null)
            {
                door.layer = new Layer("PORTAL", Layer.Game.depth, Layer.Game.camera);
                door.layer.scissor = (Rectangle)Graphics.viewport.Bounds;
                Layer.Add(door.layer);
            }
            door.isLeft = Level.CheckPoint<AutoBlock>(door.center + new Vec2(-8f, 0.0f)) != null;
            door.rect = new Rectangle((float)((int)door.point1.x - 8), (float)(int)door.point1.y, 16f, (float)((int)door.point2.y - (int)door.point1.y));
        }

        public override void Initialize()
        {
        }

        public override void Terminate()
        {
            foreach (PortalDoor door in this._doors)
                Layer.Remove(door.layer);
        }

        public override void Update()
        {
            if (this._doors.Count != 2)
                return;
            IEnumerable<ITeleport> teleports = (IEnumerable<ITeleport>)null;
            foreach (PortalDoor door in this._doors)
            {
                IEnumerable<ITeleport> second = door.horizontal ? Level.CheckRectAll<ITeleport>(door.point1 + new Vec2(0.0f, -8f), door.point2 + new Vec2(0.0f, 8f)) : Level.CheckRectAll<ITeleport>(door.point1 + new Vec2(-8f, 0.0f), door.point2 + new Vec2(8f, 0.0f));
                teleports = teleports != null ? teleports.Concat<ITeleport>(second) : second;
            }
            List<PortalDrawTransformer> portalDrawTransformerList = new List<PortalDrawTransformer>();
            foreach (PortalDrawTransformer portalDrawTransformer in this._inPortal)
            {
                if (!teleports.Contains<ITeleport>(portalDrawTransformer.thing as ITeleport))
                    portalDrawTransformerList.Add(portalDrawTransformer);
            }
            foreach (PortalDrawTransformer portalDrawTransformer in portalDrawTransformerList)
            {
                this._inPortal.Remove(portalDrawTransformer);
                portalDrawTransformer.thing.portal = (Portal)null;
                portalDrawTransformer.thing.layer = Layer.Game;
                foreach (PortalDoor door in this._doors)
                    door.layer.Remove((Thing)portalDrawTransformer);
            }
            foreach (ITeleport teleport in teleports)
            {
                ITeleport t = teleport;
                if (this._inPortal.FirstOrDefault<PortalDrawTransformer>((Func<PortalDrawTransformer, bool>)(v => v.thing == t)) == null)
                {
                    PortalDrawTransformer portalDrawTransformer = new PortalDrawTransformer(t as Thing, this);
                    this._inPortal.Add(portalDrawTransformer);
                    (t as Thing).portal = this;
                    (t as Thing).layer = this._fakeLayer;
                    foreach (PortalDoor door in this._doors)
                        door.layer.Add((Thing)portalDrawTransformer);
                }
            }
            foreach (PortalDoor door in this._doors)
            {
                door.Update();
                foreach (PortalDrawTransformer portalDrawTransformer in this._inPortal)
                {
                    if (door.isLeft && (double)portalDrawTransformer.thing.x < (double)door.center.x)
                    {
                        Thing thing = portalDrawTransformer.thing;
                        thing.position = thing.position + (this.GetOtherDoor(door).center - door.center);
                    }
                    else if (!door.isLeft && (double)portalDrawTransformer.thing.x > (double)door.center.x)
                    {
                        Thing thing = portalDrawTransformer.thing;
                        thing.position = thing.position + (this.GetOtherDoor(door).center - door.center);
                    }
                }
            }
        }

        public override void Draw()
        {
            foreach (PortalDoor door in this._doors)
                door.Draw();
        }
    }
}
