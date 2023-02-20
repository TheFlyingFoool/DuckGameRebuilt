// Decompiled with JetBrains decompiler
// Type: DuckGame.StaticRenderer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            if (_targets.ContainsKey(layer))
                return;
            for (int index1 = 0; index1 < _numSections; ++index1)
            {
                for (int index2 = 0; index2 < _numSections; ++index2)
                    _targets.Add(layer, new StaticRenderSection()
                    {
                        target = new RenderTarget2D(_size, _size),
                        position = new Vec2(_position.x + index2 * _size, _position.y + index1 * _size)
                    });
            }
        }

        public static void ProcessThing(Thing t)
        {
            Layer background = Layer.Background;
            Vec2 vec2_1 = t.position - t.center - _position;
            int num1 = (int)Math.Floor(vec2_1.x / _size);
            int num2 = (int)Math.Floor(vec2_1.y / _size);
            InitializeLayer(background);
            Vec2 vec2_2 = t.position - t.center + new Vec2(t.graphic.width, t.graphic.height) - _position;
            int num3 = (int)Math.Floor(vec2_2.x / _size);
            int num4 = (int)Math.Floor(vec2_2.y / _size);
            _targets[background][num2 * _numSections + num1].things.Add(t);
            if (num1 != num3)
                _targets[background][num2 * _numSections + num3].things.Add(t);
            if (num2 != num4)
                _targets[background][num4 * _numSections + num1].things.Add(t);
            if (num1 == num3 || num2 == num4)
                return;
            _targets[background][num4 * _numSections + num3].things.Add(t);
        }

        public static void Update()
        {
        }

        public static void RenderLayer(Layer layer)
        {
        }
    }
}
