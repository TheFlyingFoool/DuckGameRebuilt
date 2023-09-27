// Decompiled with JetBrains decompiler
// Type: DuckGame.StreetLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Normal)]
    [BaggedProperty("isInDemo", true)]
    public class StreetLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public StreetLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("streetLight");
            center = new Vec2(6f, 54f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -2f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Floor;
            layer = Layer.Game;
            editorCycleType = typeof(Sun);
        }
        public override void Update()
        {
            if (tim > 0)
            {
                pl._range = Rando.Float(200);
                if (tim <= 1)
                {
                    pl._range = 200;
                }
                pl.forceRefresh = true;
                tim--;
            }
            else
            {
                if (Rando.Int(480) == 0)
                {
                    tim = Rando.Int(10);
                }
            }
        }
        public int tim;
        public PointLight pl;
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            if (flipHorizontal)
            {
                Vec2 vec2 = new Vec2(x - 16f, (float)(y - 32 - 32));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(8f, 5f), vec2 + new Vec2(-1f, -4f), new Color(0.4f, 0.4f, 0.4f)));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(1f, -4f), vec2 + new Vec2(-8f, 5f), new Color(0.4f, 0.4f, 0.4f)));
                pl = new PointLight(vec2.x, vec2.y + 1f, new Color(247, 198, 120), 200f, _occluders);
                Level.Add(pl);
            }
            else
            {
                Vec2 vec2 = new Vec2(x + 16f, (float)(y - 32 - 32));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(-8f, 5f), vec2 + new Vec2(1f, -4f), new Color(0.4f, 0.4f, 0.4f)));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(-1f, -4f), vec2 + new Vec2(8f, 5f), new Color(0.4f, 0.4f, 0.4f)));
                pl = new PointLight(vec2.x, vec2.y + 1f, new Color(247, 198, 120), 200f, _occluders);
                Level.Add(pl);
            }
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            base.Draw();
        }
    }
}
