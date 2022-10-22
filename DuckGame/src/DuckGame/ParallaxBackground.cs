// Decompiled with JetBrains decompiler
// Type: DuckGame.ParallaxBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class ParallaxBackground : Thing
    {
        public float FUCKINGYOFFSET;
        public Color color = Color.White;
        public Sprite _sprite;
        private Dictionary<int, ParallaxZone> _zones = new Dictionary<int, ParallaxZone>();
        private int _hRepeat = 1;
        public float xmove;
        public Rectangle scissor;
        public ParallaxBackground.Definition definition;
        public bool restrictBottom = true;

        public ParallaxBackground(string image, float vx, float vdepth, int hRepeat = 1)
          : base()
        {
            shouldbegraphicculled = false;
            _sprite = new Sprite(image);
            graphic = _sprite;
            x = vx;
            depth = (Depth)vdepth;
            layer = Layer.Parallax;
            _hRepeat = hRepeat;
            _opaque = true;
            definition = Content.LoadParallaxDefinition(image);
            if (definition == null)
                return;
            foreach (ParallaxBackground.Definition.Zone zone in definition.zones)
                AddZone(zone.index, zone.distance, zone.speed, zone.moving);
            foreach (ParallaxBackground.Definition.Zone sprite in definition.sprites)
                AddZoneSprite(sprite.sprite.Clone(), sprite.index, sprite.distance, sprite.speed, sprite.moving);
        }

        public ParallaxBackground(Texture2D t)
          : base()
        {
            shouldbegraphicculled = false;
            _sprite = new Sprite((Tex2D)t);
            graphic = _sprite;
            x = 0f;
            depth = (Depth)0f;
            layer = Layer.Parallax;
            _hRepeat = 3;
            _opaque = true;
        }

        public void AddZone(int yPos, float distance, float speed, bool moving = false, bool vis = true) => _zones[yPos] = new ParallaxZone(distance, speed, moving, vis);

        public void AddZoneSprite(
          Sprite s,
          int yPos,
          float distance,
          float speed,
          bool moving = false,
          float wrapMul = 1f)
        {
            if (!_zones.ContainsKey(yPos))
                _zones[yPos] = new ParallaxZone(distance, speed, moving, false)
                {
                    wrapMul = wrapMul
                };
            _zones[yPos].AddSprite(s);
        }

        public void AddZoneThing(Thing s, int yPos, float distance, float speed, bool moving = false)
        {
            _zones[yPos] = new ParallaxZone(distance, speed, moving, false);
            _zones[yPos].AddThing(s);
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            foreach (KeyValuePair<int, ParallaxZone> zone in _zones)
                zone.Value.Update(xmove);
        }

        public override void Draw()
        {
            if (scissor.width != 0.0)
                layer.scissor = scissor;
            if (position.y > 0.0)
                position.y = 0f;
            if (restrictBottom && position.y + _sprite.texture.height < Layer.Parallax.camera.bottom)
                position.y = Layer.Parallax.camera.bottom - _sprite.texture.height;
            for (int index = 0; index < _hRepeat; ++index)
            {
                for (int key = 0; key < graphic.height / 8; ++key)
                {
                    if (_zones.ContainsKey(key))
                    {
                        ParallaxZone zone = _zones[key];
                        if (index == 0)
                            zone.RenderSprites(position);
                        if (zone.visible)
                            DuckGame.Graphics.Draw(_sprite.texture, position + new Vec2(0f, FUCKINGYOFFSET) + new Vec2((zone.scroll % graphic.width - graphic.width + index * graphic.width) * scale.x, key * 8 * scale.y), new Rectangle?(new Rectangle(0f, key * 8, graphic.width, 8f)), color, 0f, new Vec2(), new Vec2(scale.x, scale.y), SpriteEffects.None, depth);
                    }
                }
            }
        }

        public class Definition
        {
            public List<ParallaxBackground.Definition.Zone> zones = new List<ParallaxBackground.Definition.Zone>();
            public List<ParallaxBackground.Definition.Zone> sprites = new List<ParallaxBackground.Definition.Zone>();

            public struct Zone
            {
                public int index;
                public float distance;
                public float speed;
                public bool moving;
                public Sprite sprite;
            }
        }
    }
}
