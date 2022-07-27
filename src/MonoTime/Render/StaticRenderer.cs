// Decompiled with JetBrains decompiler
// Type: DuckGame.StaticRenderer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class StaticRenderer
    {
        private static MultiMap<Layer, StaticRenderSection> _targets = new MultiMap<Layer, StaticRenderSection>();
        private static Vec2 _position = new Vec2(sbyte.MinValue, sbyte.MinValue);
        private static int _size = 128;
        private static int _numSections = 8;

        public static void InitializeLayer(Layer layer)
        {
            if (StaticRenderer._targets.ContainsKey(layer))
                return;
            for (int index1 = 0; index1 < StaticRenderer._numSections; ++index1)
            {
                for (int index2 = 0; index2 < StaticRenderer._numSections; ++index2)
                    StaticRenderer._targets.Add(layer, new StaticRenderSection()
                    {
                        target = new RenderTarget2D(StaticRenderer._size, StaticRenderer._size),
                        position = new Vec2(StaticRenderer._position.x + index2 * StaticRenderer._size, StaticRenderer._position.y + index1 * StaticRenderer._size)
                    });
            }
        }

        public static void ProcessThing(Thing t)
        {
            Layer background = Layer.Background;
            Vec2 vec2_1 = t.position - t.center - StaticRenderer._position;
            int num1 = (int)Math.Floor(vec2_1.x / (double)StaticRenderer._size);
            int num2 = (int)Math.Floor(vec2_1.y / (double)StaticRenderer._size);
            StaticRenderer.InitializeLayer(background);
            Vec2 vec2_2 = t.position - t.center + new Vec2(t.graphic.width, t.graphic.height) - StaticRenderer._position;
            int num3 = (int)Math.Floor(vec2_2.x / (double)StaticRenderer._size);
            int num4 = (int)Math.Floor(vec2_2.y / (double)StaticRenderer._size);
            StaticRenderer._targets[background][num2 * StaticRenderer._numSections + num1].things.Add(t);
            if (num1 != num3)
                StaticRenderer._targets[background][num2 * StaticRenderer._numSections + num3].things.Add(t);
            if (num2 != num4)
                StaticRenderer._targets[background][num4 * StaticRenderer._numSections + num1].things.Add(t);
            if (num1 == num3 || num2 == num4)
                return;
            StaticRenderer._targets[background][num4 * StaticRenderer._numSections + num3].things.Add(t);
        }

        public static void Update()
        {
        }

        public static void RenderLayer(Layer layer)
        {
        }
    }
}
