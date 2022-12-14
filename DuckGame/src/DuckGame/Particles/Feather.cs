// Decompiled with JetBrains decompiler
// Type: DuckGame.Feather
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Feather : Thing
    {
        public static int kMaxObjects = 64;
        public static Feather[] _objects = new Feather[kMaxObjects];
        public static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        private bool _rested;

        public static Feather New(float xpos, float ypos, DuckPersona who)
        {
            if (who == null)
                who = Persona.Duck1;
            Feather feather;
            if (NetworkDebugger.enabled)
                feather = new Feather();
            else if (_objects[_lastActiveObject] == null)
            {
                feather = new Feather();
                _objects[_lastActiveObject] = feather;
            }
            else
                feather = _objects[_lastActiveObject];
            Level.Remove(feather);
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            feather.Init(xpos, ypos, who);
            feather.ResetProperties();
            feather._sprite.globalIndex = GetGlobalIndex();
            feather.globalIndex = GetGlobalIndex();
            return feather;
        }

        private Feather()
          : base()
        {
            _sprite = new SpriteMap("feather", 12, 4)
            {
                speed = 0.3f
            };
            _sprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
            graphic = _sprite;
            center = new Vec2(6f, 1f);
        }

        private void Init(float xpos, float ypos, DuckPersona who)
        {
            position.x = xpos;
            position.y = ypos;
            alpha = 1f;
            hSpeed = Rando.Float(6f) - 3f;
            vSpeed = (float)(Rando.Float(2f) - 1.0 - 1.0);
            _sprite = who.featherSprite.CloneMap();
            _sprite.SetAnimation("feather");
            _sprite.frame = Rando.Int(3);
            if (Rando.Double() > 0.5)
                _sprite.flipH = true;
            else
                _sprite.flipH = false;
            graphic = _sprite;
            _rested = false;
        }
        private Thing lastthing;
        public override void Update()
        {
            if (_rested && this.lastthing != null & !this.lastthing.removeFromLevel)
                return;
            if (hSpeed > 0f)
                hSpeed -= 0.1f;
            if (hSpeed < 0f)
                hSpeed += 0.1f;
            if (hSpeed < 0.1f && hSpeed > -0.1f)
                hSpeed = 0f;
            if (vSpeed < 1f)
                vSpeed += 0.06f;
            if (vSpeed < 0f)
            {
                _sprite.speed = 0f;
                if (Level.CheckPoint<Block>(x, y - 7f) is Thing thing && thing.solid && thing.y >= this.top)
                {
                    vSpeed = 0f;
                    lastthing = thing;
                }
            }
            else if (Level.CheckPoint<IPlatform>(x, y + 3f) is Thing thing && thing.solid && thing.y >= this.top)
            {
                lastthing = thing;
                vSpeed = 0f;
                _sprite.speed = 0f;
                if (thing is Block)
                    _rested = true;
            }
            else
                _sprite.speed = 0.3f;
            x += hSpeed;
            y += vSpeed;
        }
    }
}
