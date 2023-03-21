// Decompiled with JetBrains decompiler
// Type: DuckGame.TroubleLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Normal)]
    [BaggedProperty("isInDemo", true)]
    public class TroubleLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public TroubleLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("troubleLight");
            center = new Vec2(7f, 5f);
            _collisionSize = new Vec2(10f, 10f);
            _collisionOffset = new Vec2(-3f, -4f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Floor;
            layer = Layer.Game;
            editorCycleType = typeof(Bulb);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            if (flipHorizontal)
            {
                Vec2 vec2 = new Vec2(x - 1f, y - 1f);
                _occluders.Add(new LightOccluder(vec2 + new Vec2(2f, 4f), vec2 + new Vec2(2f, -4f), new Color(0.4f, 0.4f, 0.4f)));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(2f, 2f), vec2 + new Vec2(-6f, 5f), new Color(0.4f, 0.4f, 0.4f)));
                Level.Add(new PointLight(vec2.x + 1f, vec2.y + 1f, new Color(247, 198, 150), 170f, _occluders));
            }
            else
            {
                Vec2 vec2 = new Vec2(x + 1f, y - 1f);
                _occluders.Add(new LightOccluder(vec2 + new Vec2(-2f, 4f), vec2 + new Vec2(-2f, -4f), new Color(0.4f, 0.4f, 0.4f)));
                _occluders.Add(new LightOccluder(vec2 + new Vec2(-2f, 2f), vec2 + new Vec2(6f, 5f), new Color(0.4f, 0.4f, 0.4f)));
                Level.Add(new PointLight(vec2.x - 1f, vec2.y + 1f, new Color(247, 198, 150), 170f, _occluders));
            }
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            base.Draw();
        }
    }
}
