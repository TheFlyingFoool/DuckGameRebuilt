// Decompiled with JetBrains decompiler
// Type: DuckGame.WireTrapDoorShutterSolid
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WireTrapDoorShutterSolid : Block, IShutter
    {
        private WireTrapDoor _button;
        private bool _open;
        private Vec2 _colSize;
        private SpriteMap _sprite;

        public void UpdateSprite()
        {
            _sprite.frame = (int)_button.length - 1 + (int)_button.color * 4;
            _colSize = new Vec2(7 + 16 * (int)_button.length, 12f);
            collisionSize = _colSize;
            UpdateOpenState();
        }

        public WireTrapDoorShutterSolid(float xpos, float ypos, WireTrapDoor b)
          : base(xpos, ypos)
        {
            _button = b;
            collisionSize = new Vec2(39f, 12f);
            collisionOffset = new Vec2(-3f, -3f);
            center = new Vec2(3f, 3f);
            _sprite = new SpriteMap("wireTrapDoorArmBig", 71, 13);
            graphic = _sprite;
        }

        public override void Initialize()
        {
            UpdateSprite();
            base.Initialize();
        }

        private void UpdateOpenState()
        {
            if (angleDegrees == 0)
            {
                collisionSize = _colSize;
                _open = false;
            }
            else
            {
                if (angleDegrees == 0)
                    return;
                if (!_open)
                {
                    if (!(Level.current is Editor))
                    {
                        foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(0f, -8f), bottomRight))
                            physicsObject.sleeping = false;
                    }
                    _open = true;
                }
                collisionSize = new Vec2(1f, 1f);
            }
        }

        public override void Update()
        {
            UpdateOpenState();
            base.Update();
        }

        public override void Draw()
        {
            graphic.flipH = offDir < 0;
            base.Draw();
        }
    }
}
