// Decompiled with JetBrains decompiler
// Type: DuckGame.WireTrapDoorShutter
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WireTrapDoorShutter : MaterialThing, IPlatform, IShutter
    {
        private WireTrapDoor _button;
        private bool _open;
        private Vec2 _colSize;
        private SpriteMap _sprite;

        public void UpdateSprite()
        {
            if (this._sprite == null || this._button == null)
                return;
            this._sprite.frame = (int)this._button.length - 1 + (int)this._button.color * 4;
            this._colSize = new Vec2(7 + 16 * (int)this._button.length, 7f);
            this.UpdateOpenState();
        }

        public WireTrapDoorShutter(float xpos, float ypos, WireTrapDoor b)
          : base(xpos, ypos)
        {
            this._button = b;
            this.collisionSize = new Vec2(39f, 7f);
            this.collisionOffset = new Vec2(-3f, -3f);
            this.center = new Vec2(3f, 3f);
            this._sprite = new SpriteMap("wireTrapDoorArm", 71, 7);
            this.graphic = _sprite;
        }

        public override void Initialize()
        {
            this.UpdateSprite();
            base.Initialize();
        }

        private void UpdateOpenState()
        {
            if (this.angleDegrees == 0.0)
            {
                this.collisionSize = this._colSize;
                this._open = false;
            }
            else
            {
                if (this.angleDegrees == 0.0)
                    return;
                if (!this._open)
                {
                    if (!(Level.current is Editor))
                    {
                        foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(0f, -8f), this.bottomRight))
                            physicsObject.sleeping = false;
                    }
                    this._open = true;
                }
                this.collisionSize = new Vec2(1f, 1f);
            }
        }

        public override void Update()
        {
            this.UpdateOpenState();
            base.Update();
        }

        public override void Draw()
        {
            this.graphic.flipH = this.offDir < 0;
            base.Draw();
        }
    }
}
