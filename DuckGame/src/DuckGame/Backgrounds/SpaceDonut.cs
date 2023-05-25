// Decompiled with JetBrains decompiler
// Type: DuckGame.SpaceDonut
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class SpaceDonut : Thing
    {
        private float sinInc;
        private SpriteMap _donuroid;
        private List<Donuroid> _roids = new List<Donuroid>();

        public SpaceDonut(float xpos, float ypos)
          : base(xpos, ypos)
        {
            Sprite sprite = new Sprite("background/donut")
            {
                depth = -0.9f
            };
            graphic = sprite;
            _donuroid = new SpriteMap("background/donuroids", 32, 32);
            _donuroid.CenterOrigin();
            Random random = new Random(4562280);
            Random generator = Rando.generator;
            Rando.generator = random;
            Vec2 vec2_1 = new Vec2(-22f, -14f);
            Vec2 vec2_2 = new Vec2(130f, 120f);
            for (int index = 0; index < 20; ++index)
            {
                _roids.Add(new Donuroid(vec2_2.x + Rando.Float(-6f, 6f), vec2_2.y + Rando.Float(-18f, 18f), _donuroid, Rando.Int(0, 7), (Depth)1f, 1f));
                _roids.Add(new Donuroid(vec2_2.x + Rando.Float(-6f, -1f), (float)(vec2_2.y + Rando.Float(-10f, 0f) - 10f), _donuroid, Rando.Int(0, 7), depth - 20, 0.5f));
                _roids.Add(new Donuroid(vec2_2.x + Rando.Float(6f, 1f), (float)(vec2_2.y + Rando.Float(10f, 0f) - 10f), _donuroid, Rando.Int(0, 7), depth - 20, 0.5f));
                _roids.Add(new Donuroid(vec2_2.x + Rando.Float(-6f, -1f), (float)(vec2_2.y + Rando.Float(-10f, 0f) - 20f), _donuroid, Rando.Int(0, 7), depth - 30, 0.25f));
                _roids.Add(new Donuroid(vec2_2.x + Rando.Float(6f, 1f), (float)(vec2_2.y + Rando.Float(10f, 0f) - 20f), _donuroid, Rando.Int(0, 7), depth - 30, 0.25f));
                vec2_2 += vec2_1;
                vec2_1.y += 1.4f;
            }
            Rando.generator = generator;
        }

        public override void Draw()
        {
            sinInc += 0.02f;
            Graphics.Draw(graphic, x, y + (float)Math.Sin(sinInc) * 2f, (Depth)0.9f);
            foreach (Donuroid roid in _roids)
                roid.Draw(position);
        }
    }
}
