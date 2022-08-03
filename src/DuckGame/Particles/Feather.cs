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

        public override void Update()
        {
            if (_rested)
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
                if (Level.CheckPoint<Block>(x, y - 7f) != null)
                    vSpeed = 0f;
            }
            else if (Level.CheckPoint<IPlatform>(x, y + 3f) is Thing thing)
            {
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
