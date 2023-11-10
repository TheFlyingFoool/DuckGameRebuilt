using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Furniture
    {
        public static FurnitureGroup Cheap = new FurnitureGroup()
        {
            name = "cheap",
            color = new Color(222, 181, 144),
            priceMultiplier = 0.4f,
            additionalRarity = 5
        };
        public static FurnitureGroup Fancy = new FurnitureGroup()
        {
            name = "fancy",
            color = new Color(245, 199, 91),
            font = new BitmapFont("furni/ornateFont", 8),
            priceMultiplier = 5f,
            additionalRarity = 300
        };
        public static FurnitureGroup Everyday = new FurnitureGroup()
        {
            name = "everyday",
            color = new Color(141, 182, 201),
            additionalRarity = 5
        };
        public static FurnitureGroup Flowers = new FurnitureGroup()
        {
            name = "flowers",
            color = new Color(237, 165, 206),
            font = new BitmapFont("furni/italFont", 8),
            additionalRarity = 80
        };
        public static FurnitureGroup Bathroom = new FurnitureGroup()
        {
            name = "bathroom",
            color = new Color(211, 227, 163),
            additionalRarity = 100
        };
        public static FurnitureGroup Outdoor = new FurnitureGroup()
        {
            name = "outdoor",
            color = new Color(192, 163, 227),
            additionalRarity = 20
        };
        public static FurnitureGroup Stone = new FurnitureGroup()
        {
            name = "stone",
            color = new Color(128, 159, 178),
            priceMultiplier = 3f,
            additionalRarity = 160
        };
        public static FurnitureGroup Instruments = new FurnitureGroup()
        {
            name = "music",
            color = new Color(279, 170, 150),
            priceMultiplier = 3.5f,
            additionalRarity = 120
        };
        public static FurnitureGroup Momento = new FurnitureGroup()
        {
            name = "momento",
            color = new Color(241, 71, 85)
        };
        public static FurnitureGroup Bar = new FurnitureGroup()
        {
            name = "bar",
            color = new Color(180, 211, 89),
            additionalRarity = 50
        };
        public static FurnitureGroup Ship = new FurnitureGroup()
        {
            name = "ship",
            color = new Color(157, 157, 157),
            additionalRarity = 35
        };
        public static FurnitureGroup Freezer = new FurnitureGroup()
        {
            name = "freezer",
            color = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue),
            additionalRarity = 180
        };
        public static FurnitureGroup Office = new FurnitureGroup()
        {
            name = "office",
            color = new Color(47, 72, 78),
            additionalRarity = 70
        };
        public static FurnitureGroup Characters = new FurnitureGroup()
        {
            name = "characters",
            color = new Color(160, 21, 35),
            priceMultiplier = 2f,
            additionalRarity = 8
        };
        public static FurnitureGroup Default = new FurnitureGroup()
        {
            name = "default",
            color = Colors.DGPurple
        };
        private List<VariatingSprite> _eggSprites = new List<VariatingSprite>();
        public float ballRot;
        public bool rareGen;
        private SpriteMap _photoSprite;
        public float yOffset;
        public bool isSurface;
        public bool stickToFloor;
        public bool stickToRoof;
        public SpriteMap sprite;
        public string name;
        public FurnitureType type;
        public SpriteMap icon;
        public SpriteMap background;
        public BitmapFont font;
        public FurnitureGroup group;
        public short index;
        public float topOffset;
        public int deep;
        private int _rarity;
        public int max;
        public string description;
        public bool canFlip;
        public bool neverFlip;
        public bool visible = true;
        public bool alwaysHave;
        public bool isFlag;
        public bool canGetInGacha = true;

        public int price
        {
            get
            {
                if (type == FurnitureType.Prop)
                    return (int)Math.Ceiling(sprite.width * sprite.height / 12f * group.priceMultiplier * (1f + rarity / 100f));
                if (type == FurnitureType.Theme)
                    return (int)Math.Ceiling(100f * group.priceMultiplier * (1f + rarity / 100f));
                return type == FurnitureType.Font ? (int)Math.Ceiling(60f * group.priceMultiplier * (1f + rarity / 100f)) : 9999;
            }
        }

        public VariatingSprite GetSprite(ulong id, int variation, VSType t)
        {
            VariatingSprite sprite = _eggSprites.FirstOrDefault(x => (long)x.id == (long)id && x.variation == variation && x.type == t);
            if (sprite != null)
                return sprite;
            _eggSprites.Add(new VariatingSprite()
            {
                variation = variation,
                id = id,
                type = t,
                sprite = t != VSType.Egg ? Profile.GetPaintingSprite(variation, id) : Profile.GetEggSprite(variation, id)
            });
            return null;
        }

        public void Draw(
          Vec2 pos,
          Depth depth,
          int variation = 0,
          Profile profile = null,
          bool affectScale = false,
          bool halfscale = false,
          float angle = 0f)
        {
            ulong num1 = 0;
            if (profile == null)
            {
                if (Profiles.experienceProfile != null)
                {
                    profile = Profiles.experienceProfile;
                    num1 = Profiles.experienceProfile.steamID;
                }
            }
            else
                num1 = profile.steamID;
            SpriteMap g = sprite;
            if (icon != null)
                g = icon;
            if (g != null && neverFlip)
                g.flipH = false;
            if (isFlag && profile != null)
            {
                int idx = Network.isActive ? profile.flagIndex : Global.data.flag;
                if (idx < 0)
                    idx = profile.flagIndex;
                if (idx >= 0)
                {
                    Sprite flag = UIFlagSelection.GetFlag(idx, true);
                    if (flag != null)
                    {
                        float y = 0.39f * g.scale.x;
                        for (int index = 0; index < 30; ++index)
                        {
                            float num2 = (float)Math.Sin(Graphics.frame / 10f + index * 0.18f);
                            Vec2 vec2 = pos + new Vec2((g.flipH ? -2f : 2f) * g.scale.x, -9f * g.scale.y);
                            Graphics.Draw(flag.texture, vec2 + new Vec2((float)(index * 2 * y * (g.flipH ? -1f : 1f)), (float)(num2 * 1.4f * (index / 51f))), new Rectangle?(new Rectangle(index * 2, 0f, 3f, 41f)), Color.White, 0f, Vec2.Zero, g.flipH ? new Vec2(-y, y) : new Vec2(y), SpriteEffects.None, depth - 2);
                        }
                    }
                }
            }
            if (name == "EGG")
            {
                VariatingSprite sprite = GetSprite(num1, variation, VSType.Egg);
                if (sprite != null && sprite.sprite.texture != null && sprite.sprite.texture != null)
                {
                    sprite.sprite.depth = depth + 6;
                    sprite.sprite.scale = this.sprite.scale;
                    Graphics.Draw(sprite.sprite, pos.x - 8f * sprite.sprite.xscale, pos.y - 12f * sprite.sprite.yscale);
                    g.frame = 0;
                }
            }
            else if (name == "PHOTO")
            {
                if (_photoSprite == null)
                    _photoSprite = new SpriteMap("littleMan", 16, 16);
                _photoSprite.frame = UILevelBox.LittleManFrame(variation, 7, num1);
                _photoSprite.depth = depth + 6;
                _photoSprite.scale = sprite.scale;
                Graphics.Draw(_photoSprite, pos.x - 6f * _photoSprite.xscale, pos.y - 4f * _photoSprite.yscale, new Rectangle(2f, 0f, 12f, 10f));
                Graphics.DrawRect(pos + new Vec2(-6f * _photoSprite.xscale, -6f * _photoSprite.yscale), pos + new Vec2(6f * _photoSprite.xscale, 6f * _photoSprite.yscale), Colors.DGBlue, depth - 4);
                g.frame = 0;
            }
            else if (name == "EASEL")
            {
                VariatingSprite sprite = GetSprite(num1, variation, VSType.Portrait);
                if (sprite != null && sprite.sprite.texture != null)
                {
                    sprite.sprite.depth = depth + 6;
                    sprite.sprite.scale = this.sprite.scale;
                    Graphics.Draw(sprite.sprite, pos.x - 9f * sprite.sprite.xscale, pos.y - 8f * sprite.sprite.yscale);
                    g.frame = 0;
                }
                else
                    DevConsole.Log(DCSection.General, "null easel");
            }
            else
                g.frame = variation;
            if (font != null && sprite == null)
            {
                font.scale = new Vec2(1f, 1f);
                font.Draw("F", pos + new Vec2(-3.5f, -3f), Color.Black, depth + 8);
            }
            if (affectScale)
            {
                if (halfscale && (g.width > 30 || g.height > 30))
                    g.scale = new Vec2(0.5f);
                else
                    g.scale = new Vec2(1f);
            }
            g.depth = depth;
            g.angle = angle;
            Graphics.Draw(g, pos.x, pos.y - yOffset);
            g.scale = new Vec2(1f);
        }

        public int rarity => _rarity + group.additionalRarity;

        public Furniture(
          bool canflip,
          bool neverflip,
          string desc,
          int rarityval,
          string spr,
          int wide,
          int high,
          string nam,
          FurnitureGroup gr,
          SpriteMap ico = null,
          FurnitureType t = FurnitureType.Prop,
          BitmapFont f = null,
          string bak = null,
          bool stickToroof = false,
          bool stickTofloor = false,
          int deepval = 0,
          int maxval = -1,
          bool canGacha = true)
        {
            neverFlip = neverflip;
            canGetInGacha = canGacha;
            canFlip = canflip;
            stickToFloor = stickTofloor;
            stickToRoof = stickToroof;
            max = maxval;
            _rarity = rarityval;
            description = desc;
            if (spr != null) sprite = new SpriteMap("furni/" + gr.name + "/" + spr, wide, high);
            if (bak != null) background = new SpriteMap("furni/" + gr.name + "/" + bak, wide, high);
            name = nam;
            icon = ico;
            type = t;
            font = f;
            group = gr;
            deep = deepval;
            if (stickToroof) deep += 20;
            if (!stickToroof && !stickTofloor) deep--;
            if (sprite != null)
            {
                sprite.CenterOrigin();
                if (sprite.height / 2f - Math.Floor(sprite.height / 2f) == 0f) sprite.centery--;
                else sprite.centery = (float)Math.Floor(sprite.height / 2f);
            }
            if (icon != null)
                icon.CenterOrigin();
            if (background == null)
                return;
            background.CenterOrigin();
        }

        public Furniture(
          bool canflip,
          bool neverflip,
          string desc,
          int rarityval,
          string spr,
          int wide,
          int high,
          string nam,
          FurnitureGroup gr,
          SpriteMap ico,
          bool stickTofloor,
          bool stickToroof = false,
          bool surface = false,
          float topoff = 0f,
          int maxval = -1,
          bool canGacha = true)
        {
            neverFlip = neverflip;
            canGetInGacha = canGacha;
            canFlip = canflip;
            stickToFloor = stickTofloor;
            stickToRoof = stickToroof;
            isSurface = surface;
            topOffset = topoff;
            description = desc;
            max = maxval;
            _rarity = rarityval;
            if (spr != null) sprite = new SpriteMap("furni/" + gr.name + "/" + spr, wide, high);
            if (stickToroof) deep += 20;
            if (!stickToroof && !stickTofloor) deep--;
            name = nam;
            icon = ico;
            type = FurnitureType.Prop;
            group = gr;
            if (sprite != null)
            {
                sprite.CenterOrigin();
                if (sprite.height / 2f - Math.Floor(sprite.height / 2f) == 0f) --sprite.centery;
                else sprite.centery = (float)Math.Floor(sprite.height / 2f);
            }
            if (icon != null) icon.CenterOrigin();
            if (background == null) return;
            background.CenterOrigin();
        }
    }
}
