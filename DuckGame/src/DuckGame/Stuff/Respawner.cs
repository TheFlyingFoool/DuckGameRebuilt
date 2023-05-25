// Decompiled with JetBrains decompiler
// Type: DuckGame.Respawner
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [BaggedProperty("isInDemo", true)]
    public class Respawner : Thing, IDrawToDifferentLayers
    {
        private SpriteMap _sprite;
        private float _animate;
        //private float _noiseOffset;

        public Respawner(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("respawner", 18, 10);
            graphic = _sprite;
            center = new Vec2(9f, 5f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 4f);
            hugWalls = WallHug.Floor;
            layer = Layer.Blocks;
            depth = (Depth)0.8f;
            _animate = Rando.Float(100f);
            editorTooltip = "";
        }

        public override void Update() => base.Update();

        public void OnDrawLayer(Layer pLayer)
        {
            if (pLayer != Layer.Game)
                return;
            _animate += 0.05f;
            //double y = this.y; what -NiK0
            int num1 = 6;
            for (int index = 0; index < num1; ++index)
            {
                Vec2 p1 = new Vec2(x - 6f, (float)(y - index * 4 - _animate % 1 * 4));
                float num2 = (float)(1 - (y - p1.y) / 24);
                float width = num2 * 3f;
                p1.y += width / 2f;
                Graphics.DrawLine(p1, p1 + new Vec2(12f, 0f), Colors.DGBlue * (num2 * 0.8f), width, -0.75f);
            }
            Vec2 vec2_1 = new Vec2(7f, 8f);
            Vec2 vec2_2 = position + new Vec2(-7f, -24f);
            for (int index = 0; index < vec2_1.x * vec2_1.y; ++index)
            {
                Vec2 vec2_3 = new Vec2((int)(index % vec2_1.x), (int)(index / vec2_1.y));
                float num3 = (float)((Noise.Generate(vec2_3.x * 32f, 0f) + 1) / 2 * 1.5f + 0.1f);
                float num4 = _animate * 0.1f - (int)(_animate * num3 / 1f);
                float num5 = Noise.Generate(vec2_3.x + 100f, (float)((vec2_3.y + 100f - num4) * 0.5f));
                if (num5 > 0.25)
                {
                    vec2_3.y -= (float)(_animate * num3 % 1);
                    float num6 = 1f - Math.Abs((float)((vec2_1.x / 2 - vec2_3.x) / vec2_1.x * 2));
                    float num7 = (float)((num5 - 0.25f) / 0.75f) * num6 * Math.Max(0f, Math.Min((float)((vec2_3.y / vec2_1.y - 0.1f) * 2f), 1f));
                    vec2_3 *= 2f;
                    vec2_3.y *= 2f;
                    Graphics.DrawRect(vec2_3 + vec2_2, vec2_3 + vec2_2 + new Vec2(1f, 1f), Color.White * num7, -0.5f);
                }
            }
        }
    }
}
