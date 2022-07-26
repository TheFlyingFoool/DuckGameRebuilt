// Decompiled with JetBrains decompiler
// Type: DuckGame.ParallaxBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private Sprite _sprite;
        private Dictionary<int, ParallaxZone> _zones = new Dictionary<int, ParallaxZone>();
        private int _hRepeat = 1;
        public float xmove;
        public Rectangle scissor;
        public ParallaxBackground.Definition definition;
        public bool restrictBottom = true;

        public ParallaxBackground(string image, float vx, float vdepth, int hRepeat = 1)
          : base()
        {
            this._sprite = new Sprite(image);
            this.graphic = this._sprite;
            this.x = vx;
            this.depth = (Depth)vdepth;
            this.layer = Layer.Parallax;
            this._hRepeat = hRepeat;
            this._opaque = true;
            this.definition = Content.LoadParallaxDefinition(image);
            if (this.definition == null)
                return;
            foreach (ParallaxBackground.Definition.Zone zone in this.definition.zones)
                this.AddZone(zone.index, zone.distance, zone.speed, zone.moving);
            foreach (ParallaxBackground.Definition.Zone sprite in this.definition.sprites)
                this.AddZoneSprite(sprite.sprite.Clone(), sprite.index, sprite.distance, sprite.speed, sprite.moving);
        }

        public ParallaxBackground(Texture2D t)
          : base()
        {
            this._sprite = new Sprite((Tex2D)t);
            this.graphic = this._sprite;
            this.x = 0.0f;
            this.depth = (Depth)0.0f;
            this.layer = Layer.Parallax;
            this._hRepeat = 3;
            this._opaque = true;
        }

        public void AddZone(int yPos, float distance, float speed, bool moving = false, bool vis = true) => this._zones[yPos] = new ParallaxZone(distance, speed, moving, vis);

        public void AddZoneSprite(
          Sprite s,
          int yPos,
          float distance,
          float speed,
          bool moving = false,
          float wrapMul = 1f)
        {
            if (!this._zones.ContainsKey(yPos))
                this._zones[yPos] = new ParallaxZone(distance, speed, moving, false)
                {
                    wrapMul = wrapMul
                };
            this._zones[yPos].AddSprite(s);
        }

        public void AddZoneThing(Thing s, int yPos, float distance, float speed, bool moving = false)
        {
            this._zones[yPos] = new ParallaxZone(distance, speed, moving, false);
            this._zones[yPos].AddThing(s);
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            foreach (KeyValuePair<int, ParallaxZone> zone in this._zones)
                zone.Value.Update(this.xmove);
        }

        public override void Draw()
        {
            if ((double)this.scissor.width != 0.0)
                this.layer.scissor = this.scissor;
            if ((double)this.position.y > 0.0)
                this.position.y = 0.0f;
            if (this.restrictBottom && (double)this.position.y + (double)this._sprite.texture.height < (double)Layer.Parallax.camera.bottom)
                this.position.y = Layer.Parallax.camera.bottom - (float)this._sprite.texture.height;
            for (int index = 0; index < this._hRepeat; ++index)
            {
                for (int key = 0; key < this.graphic.height / 8; ++key)
                {
                    if (this._zones.ContainsKey(key))
                    {
                        ParallaxZone zone = this._zones[key];
                        if (index == 0)
                            zone.RenderSprites(this.position);
                        if (zone.visible)
                            DuckGame.Graphics.Draw(this._sprite.texture, this.position + new Vec2(0.0f, this.FUCKINGYOFFSET) + new Vec2((zone.scroll % (float)this.graphic.width - (float)this.graphic.width + (float)(index * this.graphic.width)) * this.scale.x, (float)(key * 8) * this.scale.y), new Rectangle?(new Rectangle(0.0f, (float)(key * 8), (float)this.graphic.width, 8f)), this.color, 0.0f, new Vec2(), new Vec2(this.scale.x, this.scale.y), SpriteEffects.None, this.depth);
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
