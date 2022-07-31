// Decompiled with JetBrains decompiler
// Type: DuckGame.Feather
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Feather : Thing
    {
        private static int kMaxObjects = 64;
        private static Feather[] _objects = new Feather[Feather.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        private bool _rested;

        public static Feather New(float xpos, float ypos, DuckPersona who)
        {
            if (who == null)
                who = Persona.Duck1;
            Feather feather;
            if (NetworkDebugger.enabled)
                feather = new Feather();
            else if (Feather._objects[Feather._lastActiveObject] == null)
            {
                feather = new Feather();
                Feather._objects[Feather._lastActiveObject] = feather;
            }
            else
                feather = Feather._objects[Feather._lastActiveObject];
            Level.Remove(feather);
            Feather._lastActiveObject = (Feather._lastActiveObject + 1) % Feather.kMaxObjects;
            feather.Init(xpos, ypos, who);
            feather.ResetProperties();
            feather._sprite.globalIndex = Thing.GetGlobalIndex();
            feather.globalIndex = Thing.GetGlobalIndex();
            return feather;
        }

        private Feather()
          : base()
        {
            this._sprite = new SpriteMap("feather", 12, 4)
            {
                speed = 0.3f
            };
            this._sprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
            this.graphic = _sprite;
            this.center = new Vec2(6f, 1f);
        }

        private void Init(float xpos, float ypos, DuckPersona who)
        {
            this.position.x = xpos;
            this.position.y = ypos;
            this.alpha = 1f;
            this.hSpeed = Rando.Float(6f) - 3f;
            this.vSpeed = (float)((double)Rando.Float(2f) - 1.0 - 1.0);
            this._sprite = who.featherSprite.CloneMap();
            this._sprite.SetAnimation("feather");
            this._sprite.frame = Rando.Int(3);
            if (Rando.Double() > 0.5)
                this._sprite.flipH = true;
            else
                this._sprite.flipH = false;
            this.graphic = _sprite;
            this._rested = false;
        }

        public override void Update()
        {
            if (this._rested)
                return;
            if ((double)this.hSpeed > 0f)
                this.hSpeed -= 0.1f;
            if ((double)this.hSpeed < 0f)
                this.hSpeed += 0.1f;
            if ((double)this.hSpeed < 0.1f && (double)this.hSpeed > -0.1f)
                this.hSpeed = 0f;
            if ((double)this.vSpeed < 1f)
                this.vSpeed += 0.06f;
            if ((double)this.vSpeed < 0f)
            {
                this._sprite.speed = 0f;
                if (Level.CheckPoint<Block>(this.x, this.y - 7f) != null)
                    this.vSpeed = 0f;
            }
            else if (Level.CheckPoint<IPlatform>(this.x, this.y + 3f) is Thing thing)
            {
                this.vSpeed = 0f;
                this._sprite.speed = 0f;
                if (thing is Block)
                    this._rested = true;
            }
            else
                this._sprite.speed = 0.3f;
            this.x += this.hSpeed;
            this.y += this.vSpeed;
        }
    }
}
