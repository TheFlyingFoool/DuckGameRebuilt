// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckPersona
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    public class DuckPersona
    {
        private int _index = -1;
        private Vec3 _color;
        private Vec3 _colorDark;
        private Vec3 _colorLight;
        private SpriteMap _skipSprite;
        private SpriteMap _arrowSprite;
        private SpriteMap _fingerPositionSprite;
        private SpriteMap _featherSprite;
        private SpriteMap _crowdSprite;
        private SpriteMap _sprite;
        private SpriteMap _armSprite;
        private SpriteMap _quackSprite;
        private SpriteMap _controlledSprite;
        private SpriteMap _defaultHead;
        public SpriteMap chatBust;
        private RenderTarget2D _iconMap;
        public MaterialPersona material;

        public bool mallard => this._colorDark != Vec3.Zero;

        public int index
        {
            get
            {
                if (this._index < 0)
                {
                    DuckPersona duckPersona1 = Persona.all.FirstOrDefault<DuckPersona>(x => x.color == this.color);
                    if (duckPersona1 != null)
                    {
                        ++this._index;
                        foreach (DuckPersona duckPersona2 in Persona.all)
                        {
                            if (duckPersona2 != duckPersona1)
                                ++this._index;
                            else
                                break;
                        }
                        if (this._index > Persona.all.Count<DuckPersona>())
                            this._index = 0;
                    }
                }
                return this._index;
            }
            set => this._index = value;
        }

        public Vec3 color
        {
            get => this._color;
            set => this._color = value;
        }

        public Vec3 colorDark => this._colorDark == Vec3.Zero ? this._color * 0.7f : this._colorDark;

        public Vec3 colorLight => this._colorLight;

        public Color colorUsable => new Color((byte)this._color.x, (byte)this._color.y, (byte)this._color.z);

        public SpriteMap skipSprite
        {
            get => this._skipSprite;
            set => this._skipSprite = value;
        }

        public SpriteMap arrowSprite
        {
            get => this._arrowSprite;
            set => this._arrowSprite = value;
        }

        public SpriteMap fingerPositionSprite
        {
            get => this._fingerPositionSprite;
            set => this._fingerPositionSprite = value;
        }

        public SpriteMap featherSprite
        {
            get => this._featherSprite;
            set => this._featherSprite = value;
        }

        public SpriteMap crowdSprite
        {
            get => this._crowdSprite;
            set => this._crowdSprite = value;
        }

        public SpriteMap sprite
        {
            get => this._sprite;
            set => this._sprite = value;
        }

        public SpriteMap armSprite
        {
            get => this._armSprite;
            set => this._armSprite = value;
        }

        public SpriteMap quackSprite
        {
            get => this._quackSprite;
            set => this._quackSprite = value;
        }

        public SpriteMap controlledSprite
        {
            get => this._controlledSprite;
            set => this._controlledSprite = value;
        }

        public SpriteMap defaultHead
        {
            get => this._defaultHead;
            set => this._defaultHead = value;
        }

        public RenderTarget2D iconMap
        {
            get
            {
                if (this._iconMap == null || this._iconMap.IsDisposed || (this._iconMap.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).IsContentLost)
                    this._iconMap = new RenderTarget2D(96, 96, false, RenderTargetUsage.PreserveContents);
                return this._iconMap;
            }
        }

        public Tex2D Recolor(Tex2D pTex) => DuckGame.Graphics.RecolorOld(pTex, this._color);

        public DuckPersona(Vec3 varCol)
          : this(varCol, Vec3.Zero, Vec3.Zero)
        {
        }

        public DuckPersona(Vec3 varCol, Vec3 varCol2, Vec3 varCol3)
        {
            this._color = varCol;
            this._colorDark = varCol2;
            this._colorLight = varCol3;
            this.material = new MaterialPersona(this);
            try
            {
                if (varCol2 != Vec3.Zero)
                {
                    Color color1 = new Color(varCol.x / byte.MaxValue, varCol.y / byte.MaxValue, varCol.z / byte.MaxValue);
                    Color color2 = new Color(varCol2.x / byte.MaxValue, varCol2.y / byte.MaxValue, varCol2.z / byte.MaxValue);
                    Color color3 = new Color(varCol3.x / byte.MaxValue, varCol3.y / byte.MaxValue, varCol3.z / byte.MaxValue);
                    this._skipSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("skipSign_m"), color1, color2, color3), 52, 18);
                    this._skipSprite.center = new Vec2(this._skipSprite.width - 3, 15f);
                    this._arrowSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("startArrow_m"), color1, color2, color3), 24, 16);
                    this._arrowSprite.CenterOrigin();
                    this._sprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("duck_m"), color1, color2, color3), 32, 32);
                    this._sprite.CenterOrigin();
                    this._crowdSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("seatDuck_m"), color1, color2, color3), 19, 23);
                    this._crowdSprite.CenterOrigin();
                    this._sprite.ClearAnimations();
                    this._sprite.AddAnimation("idle", 1f, true, new int[1]);
                    this._sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    this._sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    this._sprite.AddAnimation("slide", 1f, true, 10);
                    this._sprite.AddAnimation("crouch", 1f, true, 11);
                    this._sprite.AddAnimation("groundSlide", 1f, true, 12);
                    this._sprite.AddAnimation("dead", 1f, true, 13);
                    this._sprite.AddAnimation("netted", 1f, true, 14);
                    this._sprite.AddAnimation("listening", 1f, true, 16);
                    this._sprite.SetAnimation("idle");
                    this._featherSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("feather_m"), color1, color2, color3), 12, 4)
                    {
                        speed = 0.3f
                    };
                    this._featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    this._fingerPositionSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("fingerPositions_m"), color1, color2, color3), 16, 12);
                    this._fingerPositionSprite.CenterOrigin();
                    this._quackSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("quackduck_m"), color1, color2, color3), 32, 32);
                    this._quackSprite.CenterOrigin();
                    this._armSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("duckArms_m"), color1, color2, color3), 16, 16);
                    this._armSprite.CenterOrigin();
                    this._controlledSprite = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("controlledDuck_m"), color1, color2, color3), 32, 32);
                    this._controlledSprite.CenterOrigin();
                    this._defaultHead = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("hats/default_m"), color1, color2, color3), 32, 32);
                    this._defaultHead.CenterOrigin();
                    this.chatBust = new SpriteMap(DuckGame.Graphics.RecolorM(Content.Load<Tex2D>("chatBust_m"), color1, color2, color3), 14, 13);
                    this.chatBust.CenterOrigin();
                }
                else
                {
                    this._skipSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("skipSign"), this._color), 52, 18);
                    this._skipSprite.center = new Vec2(this._skipSprite.width - 3, 15f);
                    this._arrowSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("startArrow"), this._color), 24, 16);
                    this._arrowSprite.CenterOrigin();
                    this._sprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("duck"), this._color), 32, 32);
                    this._sprite.CenterOrigin();
                    this._crowdSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("seatDuck"), this._color), 19, 23);
                    this._crowdSprite.CenterOrigin();
                    this._sprite.ClearAnimations();
                    this._sprite.AddAnimation("idle", 1f, true, new int[1]);
                    this._sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    this._sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    this._sprite.AddAnimation("slide", 1f, true, 10);
                    this._sprite.AddAnimation("crouch", 1f, true, 11);
                    this._sprite.AddAnimation("groundSlide", 1f, true, 12);
                    this._sprite.AddAnimation("dead", 1f, true, 13);
                    this._sprite.AddAnimation("netted", 1f, true, 14);
                    this._sprite.AddAnimation("listening", 1f, true, 16);
                    this._sprite.SetAnimation("idle");
                    this._featherSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("feather"), this._color), 12, 4)
                    {
                        speed = 0.3f
                    };
                    this._featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    this._fingerPositionSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("fingerPositions"), this._color), 16, 12);
                    this._fingerPositionSprite.CenterOrigin();
                    this._quackSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("quackduck"), this._color), 32, 32);
                    this._quackSprite.CenterOrigin();
                    this._armSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("duckArms"), this._color), 16, 16);
                    this._armSprite.CenterOrigin();
                    this._controlledSprite = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("controlledDuck"), this._color), 32, 32);
                    this._controlledSprite.CenterOrigin();
                    this._defaultHead = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>("hats/default"), this._color), 32, 32);
                    this._defaultHead.CenterOrigin();
                    this.chatBust = new SpriteMap(DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>(nameof(chatBust)), this._color), 14, 13);
                    this.chatBust.CenterOrigin();
                }
            }
            catch (Exception)
            {
            }
        }

        public void Recreate()
        {
        }
    }
}
