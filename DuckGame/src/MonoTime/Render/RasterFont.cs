// Decompiled with JetBrains decompiler
// Type: DuckGame.RasterFont
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class RasterFont : FancyBitmapFont
    {
        public static readonly RasterFont None = new RasterFont(null, 0f);
        public RasterFont.Data data;

        public static float fontScaleFactor => Resolution.current.y / 72 / 10f;

        public float size
        {
            get => data.fontSize;
            set
            {
                if (value == data.fontSize)
                    return;
                Rebuild(data.name, value);
            }
        }

        public RasterFont(string pFont, float pSize) => Rebuild(pFont, pSize);

        public override Sprite _texture
        {
            get
            {
                if (_textureInternal == null)
                {
                    Tex2D tex = new Tex2D(data.colorsWidth, data.colorsHeight);
                    if (data.colors != null)
                        tex.SetData<uint>(data.colors);
                    _textureInternal = new Sprite(tex);
                }
                return _textureInternal;
            }
            set => _textureInternal = value;
        }

        ~RasterFont()
        {
            if (_textureInternal == null)
                return;
            _textureInternal.texture.Dispose();
        }

        public static string GetName(string pFont) => FontGDIContext.GetName(pFont);

        public void Rebuild(string pFont, float pSize)
        {
            if (pFont != null && pFont != "NULLDUCKFONTDATA")
            {
                data = FontGDIContext.CreateRasterFontData(pFont, pSize);
                _texture = null;
                _widths = new List<Rectangle>();
                foreach (BitmapFont_CharacterInfo character in data.characters)
                    _widths.Add(character.area);
                _charHeight = (int)data.fontHeight;
                _characterInfos = data.characters;
                _rasterData = data;
            }
            else
                data = new RasterFont.Data()
                {
                    name = "NULLDUCKFONTDATA",
                    fontSize = pSize
                };
            if (_widths != null && _widths.Count != 0)
                return;
            Construct("smallFont");
        }

        public string Serialize() => data.name + "^" + data.fontSize.ToString();

        public static RasterFont Deserialize(string pData)
        {
            try
            {
                string[] strArray = pData.Split('^');
                if (strArray.Length == 2)
                {
                    string str = strArray[0];
                    int pSize = Math.Min(Convert.ToInt32(strArray[1]), 120);
                    if (FontGDIContext.GetName(str) != null)
                        return new RasterFont(str, pSize);
                }
            }
            catch (Exception)
            {
            }
            return RasterFont.None;
        }

        public class Data
        {
            public string name;
            public float fontHeight;
            public float fontSize;
            public List<BitmapFont_CharacterInfo> characters = new List<BitmapFont_CharacterInfo>();
            public uint[] colors;
            public int colorsWidth;
            public int colorsHeight;
        }
    }
}
