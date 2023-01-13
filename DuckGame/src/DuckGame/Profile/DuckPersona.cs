// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckPersona
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public bool mallard => _colorDark != Vec3.Zero;

        public int index
        {
            get
            {
                if (_index < 0)
                {
                    DuckPersona duckPersona1 = null;
                    foreach (DuckPersona duckPersona2 in Persona.alllist)
                    {
                        ++_index;
                        if (duckPersona2.color == color)
                        {
                            duckPersona1 = duckPersona2;
                            if (_index > Persona.alllist.Count)
                                _index = 0;
                            break;
                        }
                    }
                    if (duckPersona1 == null)
                    {
                        _index = -1;
                    }
                }
                return _index;
            }
            set => _index = value;
        }

        public Vec3 color
        {
            get => _color;
            set => _color = value;
        }

        public Vec3 colorDark => _colorDark == Vec3.Zero ? _color * 0.7f : _colorDark;

        public Vec3 colorLight => _colorLight;

        public Color colorUsable => new Color((byte)_color.x, (byte)_color.y, (byte)_color.z);

        public SpriteMap skipSprite
        {
            get => _skipSprite;
            set => _skipSprite = value;
        }

        public SpriteMap arrowSprite
        {
            get => _arrowSprite;
            set => _arrowSprite = value;
        }

        public SpriteMap fingerPositionSprite
        {
            get => _fingerPositionSprite;
            set => _fingerPositionSprite = value;
        }

        public SpriteMap featherSprite
        {
            get => _featherSprite;
            set => _featherSprite = value;
        }

        public SpriteMap crowdSprite
        {
            get => _crowdSprite;
            set => _crowdSprite = value;
        }

        public SpriteMap sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public SpriteMap armSprite
        {
            get => _armSprite;
            set => _armSprite = value;
        }

        public SpriteMap quackSprite
        {
            get => _quackSprite;
            set => _quackSprite = value;
        }

        public SpriteMap controlledSprite
        {
            get => _controlledSprite;
            set => _controlledSprite = value;
        }

        public SpriteMap defaultHead
        {
            get => _defaultHead;
            set => _defaultHead = value;
        }

        public RenderTarget2D iconMap
        {
            get
            {
                if (_iconMap == null || _iconMap.IsDisposed || (_iconMap.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).IsContentLost)
                    _iconMap = new RenderTarget2D(96, 96, false, RenderTargetUsage.PreserveContents);
                return _iconMap;
            }
        }

        public Tex2D Recolor(Tex2D pTex) => Graphics.RecolorOld(pTex, _color);
        public DuckPersona(Vec3 varCol)
         : this(varCol, Vec3.Zero, Vec3.Zero)
        {
        }
        public DuckPersona(Vec3 varCol, int index)
          : this(varCol, Vec3.Zero, Vec3.Zero, index)
        {
        }

        public DuckPersona(Vec3 varCol, Vec3 varCol2, Vec3 varCol3)
        {
            _color = varCol;
            _colorDark = varCol2;
            _colorLight = varCol3;
            material = new MaterialPersona(this);
            try
            {
                if (varCol2 != Vec3.Zero)
                {
                    Color color1 = new Color(varCol.x / byte.MaxValue, varCol.y / byte.MaxValue, varCol.z / byte.MaxValue);
                    Color color2 = new Color(varCol2.x / byte.MaxValue, varCol2.y / byte.MaxValue, varCol2.z / byte.MaxValue);
                    Color color3 = new Color(varCol3.x / byte.MaxValue, varCol3.y / byte.MaxValue, varCol3.z / byte.MaxValue);
                    _skipSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("skipSign_m"), color1, color2, color3), 52, 18);
                    _skipSprite.center = new Vec2(_skipSprite.width - 3, 15f);
                    _arrowSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("startArrow_m"), color1, color2, color3), 24, 16);
                    _arrowSprite.CenterOrigin();
                    _sprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("duck_m"), color1, color2, color3), 32, 32);
                    _sprite.CenterOrigin();
                    _crowdSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("seatDuck_m"), color1, color2, color3), 19, 23);
                    _crowdSprite.CenterOrigin();
                    _sprite.ClearAnimations();
                    _sprite.AddAnimation("idle", 1f, true, new int[1]);
                    _sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    _sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    _sprite.AddAnimation("slide", 1f, true, 10);
                    _sprite.AddAnimation("crouch", 1f, true, 11);
                    _sprite.AddAnimation("groundSlide", 1f, true, 12);
                    _sprite.AddAnimation("dead", 1f, true, 13);
                    _sprite.AddAnimation("netted", 1f, true, 14);
                    _sprite.AddAnimation("listening", 1f, true, 16);
                    _sprite.SetAnimation("idle");
                    _featherSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("feather_m"), color1, color2, color3), 12, 4)
                    {
                        speed = 0.3f
                    };
                    _featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    _fingerPositionSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("fingerPositions_m"), color1, color2, color3), 16, 12);
                    _fingerPositionSprite.CenterOrigin();
                    _quackSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("quackduck_m"), color1, color2, color3), 32, 32);
                    _quackSprite.CenterOrigin();
                    _armSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("duckArms_m"), color1, color2, color3), 16, 16);
                    _armSprite.CenterOrigin();
                    _controlledSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("controlledDuck_m"), color1, color2, color3), 32, 32);
                    _controlledSprite.CenterOrigin();
                    _defaultHead = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("hats/default_m"), color1, color2, color3), 32, 32);
                    _defaultHead.CenterOrigin();
                    chatBust = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("chatBust_m"), color1, color2, color3), 14, 13);
                    chatBust.CenterOrigin();

                }
                else
                {
                    _skipSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("skipSign"), _color), 52, 18);
                    _skipSprite.center = new Vec2(_skipSprite.width - 3, 15f);
                    _arrowSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("startArrow"), _color), 24, 16);
                    _arrowSprite.CenterOrigin();
                    _sprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("duck"), _color), 32, 32);
                    _sprite.CenterOrigin();
                    _crowdSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("seatDuck"), _color), 19, 23);
                    _crowdSprite.CenterOrigin();
                    _sprite.ClearAnimations();
                    _sprite.AddAnimation("idle", 1f, true, new int[1]);
                    _sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    _sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    _sprite.AddAnimation("slide", 1f, true, 10);
                    _sprite.AddAnimation("crouch", 1f, true, 11);
                    _sprite.AddAnimation("groundSlide", 1f, true, 12);
                    _sprite.AddAnimation("dead", 1f, true, 13);
                    _sprite.AddAnimation("netted", 1f, true, 14);
                    _sprite.AddAnimation("listening", 1f, true, 16);
                    _sprite.SetAnimation("idle");
                    _featherSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("feather"), _color), 12, 4)
                    {
                        speed = 0.3f
                    };
                    _featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    _fingerPositionSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("fingerPositions"), _color), 16, 12);
                    _fingerPositionSprite.CenterOrigin();
                    _quackSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("quackduck"), _color), 32, 32);
                    _quackSprite.CenterOrigin();
                    _armSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("duckArms"), _color), 16, 16);
                    _armSprite.CenterOrigin();
                    _controlledSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("controlledDuck"), _color), 32, 32);
                    _controlledSprite.CenterOrigin();
                    _defaultHead = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("hats/default"), _color), 32, 32);
                    _defaultHead.CenterOrigin();
                    chatBust = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>(nameof(chatBust)), _color), 14, 13);
                    chatBust.CenterOrigin();
                }
            }
            catch (Exception)
            {
            }
        }
        public DuckPersona(Vec3 varCol, Vec3 varCol2, Vec3 varCol3, int i)
        {
            index = i;
            _color = varCol;
            _colorDark = varCol2;
            _colorLight = varCol3;
            material = new MaterialPersona(this);
            try
            {
                if (varCol2 != Vec3.Zero)
                {
                    Color color1 = new Color(varCol.x / byte.MaxValue, varCol.y / byte.MaxValue, varCol.z / byte.MaxValue);
                    Color color2 = new Color(varCol2.x / byte.MaxValue, varCol2.y / byte.MaxValue, varCol2.z / byte.MaxValue);
                    Color color3 = new Color(varCol3.x / byte.MaxValue, varCol3.y / byte.MaxValue, varCol3.z / byte.MaxValue);
                    _skipSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("skipSign_m"), color1, color2, color3), 52, 18);
                    _skipSprite.center = new Vec2(_skipSprite.width - 3, 15f);
                    _arrowSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("startArrow_m"), color1, color2, color3), 24, 16);
                    _arrowSprite.CenterOrigin();
                    _sprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("duck_m"), color1, color2, color3), 32, 32);
                    _sprite.CenterOrigin();
                    _crowdSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("seatDuck_m"), color1, color2, color3), 19, 23);
                    _crowdSprite.CenterOrigin();
                    _sprite.ClearAnimations();
                    _sprite.AddAnimation("idle", 1f, true, new int[1]);
                    _sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    _sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    _sprite.AddAnimation("slide", 1f, true, 10);
                    _sprite.AddAnimation("crouch", 1f, true, 11);
                    _sprite.AddAnimation("groundSlide", 1f, true, 12);
                    _sprite.AddAnimation("dead", 1f, true, 13);
                    _sprite.AddAnimation("netted", 1f, true, 14);
                    _sprite.AddAnimation("listening", 1f, true, 16);
                    _sprite.SetAnimation("idle");
                    _featherSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("feather_m"), color1, color2, color3), 12, 4)
                    {
                        speed = 0.3f
                    };
                    _featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    _fingerPositionSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("fingerPositions_m"), color1, color2, color3), 16, 12);
                    _fingerPositionSprite.CenterOrigin();
                    _quackSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("quackduck_m"), color1, color2, color3), 32, 32);
                    _quackSprite.CenterOrigin();
                    _armSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("duckArms_m"), color1, color2, color3), 16, 16);
                    _armSprite.CenterOrigin();
                    _controlledSprite = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("controlledDuck_m"), color1, color2, color3), 32, 32);
                    _controlledSprite.CenterOrigin();
                    _defaultHead = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("hats/default_m"), color1, color2, color3), 32, 32);
                    _defaultHead.CenterOrigin();
                    chatBust = new SpriteMap(Graphics.RecolorM(Content.Load<Tex2D>("chatBust_m"), color1, color2, color3), 14, 13);
                    chatBust.CenterOrigin();
                }
                else
                {
                    _skipSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("skipSign"), _color), 52, 18);
                    _skipSprite.center = new Vec2(_skipSprite.width - 3, 15f);
                    _arrowSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("startArrow"), _color), 24, 16);
                    _arrowSprite.CenterOrigin();
                    _sprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("duck"), _color), 32, 32);
                    _sprite.CenterOrigin();
                    _crowdSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("seatDuck"), _color), 19, 23);
                    _crowdSprite.CenterOrigin();
                    _sprite.ClearAnimations();
                    _sprite.AddAnimation("idle", 1f, true, new int[1]);
                    _sprite.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
                    _sprite.AddAnimation("jump", 1f, true, 7, 8, 9);
                    _sprite.AddAnimation("slide", 1f, true, 10);
                    _sprite.AddAnimation("crouch", 1f, true, 11);
                    _sprite.AddAnimation("groundSlide", 1f, true, 12);
                    _sprite.AddAnimation("dead", 1f, true, 13);
                    _sprite.AddAnimation("netted", 1f, true, 14);
                    _sprite.AddAnimation("listening", 1f, true, 16);
                    _sprite.SetAnimation("idle");
                    _featherSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("feather"), _color), 12, 4)
                    {
                        speed = 0.3f
                    };
                    _featherSprite.AddAnimation("feather", 1f, true, 0, 1, 2, 3);
                    _fingerPositionSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("fingerPositions"), _color), 16, 12);
                    _fingerPositionSprite.CenterOrigin();
                    _quackSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("quackduck"), _color), 32, 32);
                    _quackSprite.CenterOrigin();
                    _armSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("duckArms"), _color), 16, 16);
                    _armSprite.CenterOrigin();
                    _controlledSprite = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("controlledDuck"), _color), 32, 32);
                    _controlledSprite.CenterOrigin();
                    _defaultHead = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>("hats/default"), _color), 32, 32);
                    _defaultHead.CenterOrigin();
                    chatBust = new SpriteMap(Graphics.RecolorOld(Content.Load<Tex2D>(nameof(chatBust)), _color), 14, 13);
                    chatBust.CenterOrigin();
                }
                _skipSprite.texture.Namebase = "_skipSprite" + i.ToString();
                Content.textures[_skipSprite.texture.Namebase] = _skipSprite.texture;

                _arrowSprite.texture.Namebase = "_arrowSprite" + i.ToString();
                Content.textures[_arrowSprite.texture.Namebase] = _arrowSprite.texture;

                _crowdSprite.texture.Namebase = "_crowdSprite" + i.ToString();
                Content.textures[_crowdSprite.texture.Namebase] = _crowdSprite.texture;

                _sprite.texture.Namebase = "_sprite" + i.ToString();
                Content.textures[_sprite.texture.Namebase] = _sprite.texture;

                _featherSprite.texture.Namebase = "_featherSprite" + i.ToString();
                Content.textures[_featherSprite.texture.Namebase] = _featherSprite.texture;

                _fingerPositionSprite.texture.Namebase = "_fingerPositionSprite" + i.ToString();
                Content.textures[_fingerPositionSprite.texture.Namebase] = _fingerPositionSprite.texture;
                _quackSprite.texture.Namebase = "_quackSprite" + i.ToString();
                Content.textures[_quackSprite.texture.Namebase] = _quackSprite.texture;


                _armSprite.texture.Namebase = "_armSprite" + i.ToString();
                Content.textures[_armSprite.texture.Namebase] = _armSprite.texture;
                _controlledSprite.texture.Namebase = "_controlledSprite" + i.ToString();
                Content.textures[_controlledSprite.texture.Namebase] = _controlledSprite.texture;
                _defaultHead.texture.Namebase = "_defaultHead" + i.ToString();
                Content.textures[_defaultHead.texture.Namebase] = _defaultHead.texture;
                chatBust.texture.Namebase = "chatBust" + i.ToString();
                Content.textures[chatBust.texture.Namebase] = chatBust.texture;
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
