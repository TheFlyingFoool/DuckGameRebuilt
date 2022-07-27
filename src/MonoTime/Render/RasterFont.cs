// Decompiled with JetBrains decompiler
// Type: DuckGame.RasterFont
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class RasterFont : FancyBitmapFont
    {
        public static readonly RasterFont None = new RasterFont(null, 0.0f);
        public RasterFont.Data data;

        public static float fontScaleFactor => Resolution.current.y / 72 / 10f;

        public float size
        {
            get => this.data.fontSize;
            set
            {
                if ((double)value == data.fontSize)
                    return;
                this.Rebuild(this.data.name, value);
            }
        }

        public RasterFont(string pFont, float pSize) => this.Rebuild(pFont, pSize);

        public override Sprite _texture
        {
            get
            {
                if (this._textureInternal == null)
                {
                    Tex2D tex = new Tex2D(this.data.colorsWidth, this.data.colorsHeight);
                    if (this.data.colors != null)
                        tex.SetData<uint>(this.data.colors);
                    this._textureInternal = new Sprite(tex);
                }
                return this._textureInternal;
            }
            set => this._textureInternal = value;
        }

        ~RasterFont()
        {
            if (this._textureInternal == null)
                return;
            this._textureInternal.texture.Dispose();
        }

        public static string GetName(string pFont) => FontGDIContext.GetName(pFont);

        public void Rebuild(string pFont, float pSize)
        {
            if (pFont != null && pFont != "NULLDUCKFONTDATA")
            {
                this.data = FontGDIContext.CreateRasterFontData(pFont, pSize);
                this._texture = null;
                this._widths = new List<Rectangle>();
                foreach (BitmapFont_CharacterInfo character in this.data.characters)
                    this._widths.Add(character.area);
                this._charHeight = (int)this.data.fontHeight;
                this._characterInfos = this.data.characters;
                this._rasterData = this.data;
            }
            else
                this.data = new RasterFont.Data()
                {
                    name = "NULLDUCKFONTDATA",
                    fontSize = pSize
                };
            if (this._widths != null && this._widths.Count != 0)
                return;
            this.Construct("smallFont");
        }

        public string Serialize() => this.data.name + "^" + this.data.fontSize.ToString();

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
