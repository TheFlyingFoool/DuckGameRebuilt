// Decompiled with JetBrains decompiler
// Type: DuckGame.FancyBitmapFont
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DuckGame
{
    public class FancyBitmapFont : Transform
    {
        protected RasterFont.Data _rasterData;
        private static Dictionary<string, List<Rectangle>> widthMap = new Dictionary<string, List<Rectangle>>();
        private SpriteMap swearSprites = new SpriteMap("lagturtle", 16, 16);
        protected Sprite _textureInternal;
        private static bool _mapInitialized = false;
        public static char[] _characters = new char[317]
        {
      ' ',
      '!',
      '"',
      '#',
      '$',
      '%',
      '&',
      '\'',
      '(',
      ')',
      '*',
      '+',
      ',',
      '-',
      '.',
      '/',
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      ':',
      ';',
      '>',
      '=',
      '<',
      '?',
      '@',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      '[',
      '\\',
      ']',
      '^',
      '_',
      '`',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '{',
      '|',
      '}',
      '~',
      '`',
      'À',
      'Á',
      'Â',
      'Ã',
      'Ä',
      'Å',
      'Æ',
      'Ç',
      'È',
      'É',
      'Ê',
      'Ë',
      'Ì',
      'Í',
      'Î',
      'Ï',
      'Ð',
      'Ñ',
      'Ò',
      'Ó',
      'Ô',
      'Õ',
      'Ö',
      'Ø',
      'Ù',
      'Ú',
      'Û',
      'Ü',
      'Ý',
      'Þ',
      'ß',
      'à',
      'á',
      'â',
      'ã',
      'ä',
      'å',
      'æ',
      'ç',
      'è',
      'é',
      'ê',
      'ë',
      'ì',
      'í',
      'î',
      'ï',
      'ð',
      'ñ',
      'ò',
      'ó',
      'ô',
      'õ',
      'ö',
      'ø',
      'ù',
      'ú',
      'û',
      'ü',
      'ý',
      'þ',
      'ÿ',
      'Ā',
      'ā',
      'Ă',
      'ă',
      'Ą',
      'ą',
      'Ć',
      'ć',
      'Ċ',
      'ċ',
      'Č',
      'č',
      'Ď',
      'ď',
      'Ē',
      'ē',
      'Ę',
      'ę',
      'Ě',
      'ě',
      'Ğ',
      'ğ',
      'Ġ',
      'ġ',
      'Ģ',
      'ģ',
      'Ħ',
      'ħ',
      'Ī',
      'ī',
      'Į',
      'į',
      'İ',
      'ı',
      'Ĳ',
      'ĳ',
      'Ķ',
      'ķ',
      'Ĺ',
      'ĺ',
      'Ļ',
      'ļ',
      'Ľ',
      'ľ',
      'Ł',
      'ł',
      'Ń',
      'ń',
      'Ņ',
      'ņ',
      'Ň',
      'ň',
      'Ō',
      'ō',
      'Ő',
      'ő',
      'Œ',
      'œ',
      'Ŕ',
      'ŕ',
      'Ř',
      'ř',
      'Ś',
      'ś',
      'Ş',
      'ş',
      'Š',
      'š',
      'Ţ',
      'ţ',
      'Ť',
      'ť',
      'Ū',
      'ū',
      'Ů',
      'ů',
      'Ű',
      'ű',
      'Ų',
      'ų',
      'Ÿ',
      'Ź',
      'ź',
      'Ż',
      'ż',
      'Ž',
      'ž',
      'ǅ',
      'ǆ',
      'ǲ',
      'ǳ',
      'А',
      'Б',
      'В',
      'Г',
      'Д',
      'Е',
      'Ё',
      'Ж',
      'З',
      'И',
      'Й',
      'К',
      'Л',
      'М',
      'Н',
      'О',
      'П',
      'Р',
      'С',
      'Т',
      'У',
      'Ф',
      'Х',
      'Ц',
      'Ч',
      'Ш',
      'Щ',
      'Ъ',
      'Ы',
      'Ь',
      'Э',
      'Ю',
      'Я',
      'а',
      'б',
      'в',
      'г',
      'д',
      'е',
      'ё',
      'ж',
      'з',
      'и',
      'й',
      'к',
      'л',
      'м',
      'н',
      'о',
      'п',
      'р',
      'с',
      'т',
      'у',
      'ф',
      'х',
      'ц',
      'ч',
      'ш',
      'щ',
      'ъ',
      'ы',
      'ь',
      'э',
      'ю',
      'я',
      '¡',
      '¿'
        };
        public static SpriteMap _kanjiSprite;
        public static ushort[] _kanjiMap;
        private static int[] _characterMap = new int[ushort.MaxValue];
        private const int kTilesPerRow = 16;
        private InputProfile _inputProfile;
        private int _maxWidth;
        protected List<Rectangle> _widths;
        protected List<BitmapFont_CharacterInfo> _characterInfos;
        protected int _charHeight;
        private int _firstYPixel;
        public bool chatFont;
        public NetworkConnection _currentConnection;
        private int _letterIndex;
        private bool _drawingOutline;
        public float symbolYOffset;
        public float lineGap;
        public int _highlightStart = -1;
        public int _highlightEnd = -1;
        private Color _previousColor;
        public bool singleLine;
        public bool enforceWidthByWord = true;

        public virtual Sprite _texture
        {
            get => this._textureInternal;
            set => this._textureInternal = value;
        }

        public static void InitializeKanjis()
        {
            if (FancyBitmapFont._kanjiSprite != null)
                return;
            FancyBitmapFont._kanjiSprite = new SpriteMap("kanji_font", 8, 8);
            string str = DuckFile.ReadAllText(DuckFile.contentDirectory + "kanji_map.txt");
            FancyBitmapFont._kanjiMap = new ushort[ushort.MaxValue];
            for (int index = 0; index < str.Length; ++index)
                FancyBitmapFont._kanjiMap[str[index]] = (ushort)index;
        }

        public float height => _texture.height * this.scale.y;

        public InputProfile inputProfile
        {
            get => this._inputProfile;
            set => this._inputProfile = value;
        }

        public int maxWidth
        {
            get => this._maxWidth;
            set => this._maxWidth = value;
        }

        public int maxRows { get; set; }

        public FancyBitmapFont()
        {
        }

        public int characterHeight => this._charHeight;

        public FancyBitmapFont(string image) => this.Construct(image);

        protected void Construct(string image)
        {
            FancyBitmapFont.InitializeKanjis();
            this._texture = new Sprite(image);
            if (!FancyBitmapFont.widthMap.TryGetValue(image, out this._widths))
            {
                this._widths = new List<Rectangle>();
                Color[] data = this._texture.texture.GetData();
                bool flag = false;
                int x = -1;
                for (int y = 1; y < this._texture.height; y += this._charHeight + 1)
                {
                    for (int index1 = 0; index1 < this._texture.width; ++index1)
                    {
                        if (data[index1 + y * this._texture.width].r == 0 && data[index1 + y * this._texture.width].g == 0 && data[index1 + y * this._texture.width].b == 0 && data[index1 + y * this._texture.width].a == 0)
                        {
                            if (x == -1)
                                x = index1;
                        }
                        else if (x != -1)
                        {
                            if (this._charHeight == 0)
                            {
                                this._firstYPixel = y;
                                int num = index1 - 1;
                                for (int index2 = y + 1; index2 < this._texture.height; ++index2)
                                {
                                    if (data[num + index2 * this._texture.width].r != 0 || data[num + index2 * this._texture.width].g != 0 || data[num + index2 * this._texture.width].b != 0 || data[num + index2 * this._texture.width].a != 0)
                                    {
                                        this._charHeight = index2 - y;
                                        break;
                                    }
                                }
                                index1 = num + 1;
                            }
                            this._widths.Add(new Rectangle(x, y, index1 - x, _charHeight));
                            x = -1;
                        }
                    }
                    if (flag)
                        break;
                }
            }
            FancyBitmapFont.widthMap[image] = this._widths;
            if (this._widths.Count > 0)
                this._charHeight = (int)this._widths[0].height;
            if (FancyBitmapFont._mapInitialized)
                return;
            for (int index3 = 0; index3 < ushort.MaxValue; ++index3)
            {
                char ch = (char)index3;
                FancyBitmapFont._characterMap[index3] = 91;
                for (int index4 = 0; index4 < FancyBitmapFont._characters.Length; ++index4)
                {
                    if (FancyBitmapFont._characters[index4] == ch)
                    {
                        FancyBitmapFont._characterMap[index3] = index4;
                        break;
                    }
                }
            }
            FancyBitmapFont._mapInitialized = true;
        }

        public Sprite ParseSprite(string text, InputProfile input)
        {
            if (text.StartsWith("_!"))
                return null;
            ++this._letterIndex;
            string str = "";
            bool flag = false;
            for (; this._letterIndex != text.Length; ++this._letterIndex)
            {
                if (text[this._letterIndex] == '@' || this.chatFont && text[this._letterIndex] == ':')
                {
                    flag = true;
                    break;
                }
                if (text[this._letterIndex] == ' ' || text[this._letterIndex] == '\n')
                {
                    --this._letterIndex;
                    break;
                }
                str += text[this._letterIndex].ToString();
            }
            if (this.chatFont && !flag)
                return null;
            Sprite sprite = null;
            if (input != null)
                sprite = input.GetTriggerImage(str);
            if (sprite == null)
                sprite = Input.GetTriggerSprite(str);
            if (sprite == null && Options.Data.mojiFilter != 0)
            {
                sprite = DuckFile.GetMoji(str, this._currentConnection);
                if (sprite == null && str.Contains("!"))
                    return Input.GetTriggerSprite("blankface");
            }
            return sprite;
        }

        public Color ParseColor(string text)
        {
            ++this._letterIndex;
            string color = "";
            for (; this._letterIndex != text.Length && text[this._letterIndex] != ' ' && text[this._letterIndex] != '|'; ++this._letterIndex)
                color += text[this._letterIndex].ToString();
            return color == "PREV" ? new Color(this._previousColor.r, this._previousColor.g, this._previousColor.b) : Colors.ParseColor(color);
        }

        public InputProfile GetInputProfile(InputProfile input)
        {
            if (input == null)
                input = this._inputProfile != null ? this._inputProfile : InputProfile.FirstProfileWithDevice;
            return input;
        }

        public string FormatWithNewlines(string pText, float maxWidth, bool thinButtons = false)
        {
            float num1 = 0.0f;
            float num2 = 0.0f;
            char[] charArray = pText.ToCharArray();
            for (this._letterIndex = 0; this._letterIndex < charArray.Length; ++this._letterIndex)
            {
                bool flag = false;
                if (charArray[this._letterIndex] == ' ' && (double)num1 > (double)maxWidth)
                    charArray[this._letterIndex] = '\n';
                if (charArray[this._letterIndex] == '@' || this.chatFont && charArray[this._letterIndex] == ':')
                {
                    pText = new string(charArray);
                    int letterIndex = this._letterIndex;
                    Sprite sprite = this.ParseSprite(pText, null);
                    if (sprite != null)
                    {
                        num1 += thinButtons ? 6f : (float)(sprite.width * (double)sprite.scale.x + 1.0);
                        flag = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (charArray[this._letterIndex] == '|')
                {
                    pText = new string(charArray);
                    int letterIndex = this._letterIndex;
                    if (this.ParseColor(pText) != Colors.Transparent)
                        flag = true;
                    else
                        this._letterIndex = letterIndex;
                }
                else if (charArray[this._letterIndex] == '\n')
                {
                    if ((double)num1 > (double)num2)
                        num2 = num1;
                    num1 = 0.0f;
                }
                if (!flag)
                {
                    char index = charArray[this._letterIndex];
                    if (index >= 'ぁ')
                    {
                        num1 += 8f * this.scale.x;
                    }
                    else
                    {
                        int character = FancyBitmapFont._characterMap[index];
                        if (this._characterInfos != null)
                        {
                            if (character < this._characterInfos.Count)
                                num1 += (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                        }
                        else if (character < this._widths.Count)
                        {
                            Rectangle width = this._widths[character];
                            num1 += (width.width - 1f) * this.scale.x;
                        }
                    }
                }
            }
            if ((double)num1 > (double)num2)
                ;
            return new string(charArray);
        }

        public float GetWidth(string text, bool thinButtons = false)
        {
            float num1 = 0.0f;
            float width1 = 0.0f;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                bool flag = false;
                if (text[this._letterIndex] == '@' || this.chatFont && text[this._letterIndex] == ':')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite1 = this.ParseSprite(text, null);
                    if (sprite1 != null)
                    {
                        if (this.chatFont)
                        {
                            Vec2 scale = sprite1.scale;
                            Sprite sprite2 = sprite1;
                            sprite2.scale *= (this.scale.x / 2f);
                            if (this is RasterFont)
                            {
                                float num2 = (float)((this as RasterFont).data.fontSize * (double)RasterFont.fontScaleFactor / 10.0);
                                Sprite sprite3 = sprite1;
                                sprite3.scale *= num2;
                                sprite1.scale = new Vec2((float)Math.Round(sprite1.scale.x * 2.0) / 2f);
                            }
                            num1 += (float)(sprite1.width * (double)sprite1.scale.x + 1.0);
                            sprite1.scale = scale;
                        }
                        else
                            num1 += thinButtons ? 6f : (float)(sprite1.width * (double)sprite1.scale.x + 1.0);
                        flag = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    if (this.ParseColor(text) != Colors.Transparent)
                        flag = true;
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '\n')
                {
                    if ((double)num1 > (double)width1)
                        width1 = num1;
                    num1 = 0.0f;
                    flag = true;
                }
                if (!flag)
                {
                    char index1 = text[this._letterIndex];
                    if (index1 >= 'ぁ')
                    {
                        num1 += 8f * this.scale.x;
                    }
                    else
                    {
                        int index2 = FancyBitmapFont._characterMap[index1];
                        if (index2 >= this._widths.Count)
                            index2 = this._widths.Count - 1;
                        if (index2 < 0)
                            return width1;
                        if (this._characterInfos != null)
                        {
                            if (index2 < this._characterInfos.Count)
                                num1 += (this._characterInfos[index2].width + this._characterInfos[index2].trailing + this._characterInfos[index2].leading) * this.scale.x;
                        }
                        else
                        {
                            Rectangle width2 = this._widths[index2];
                            num1 += (width2.width - 1f) * this.scale.x;
                        }
                    }
                }
            }
            if ((double)num1 > (double)width1)
                width1 = num1;
            return width1;
        }

        public int GetCharacterIndex(
          string text,
          float xPosition,
          float yPosition,
          int maxRows = 2147483647,
          bool thinButtons = false)
        {
            float num1 = 0.0f;
            float num2 = 0.0f;
            int num3 = 0;
            float num4 = 0.0f;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                if ((double)num1 >= (double)xPosition && (double)yPosition < (double)num2 + _charHeight * (double)this.scale.y || num3 >= maxRows)
                    return this._letterIndex - 1;
                bool flag1 = false;
                if (text[this._letterIndex] == '@' || this.chatFont && text[this._letterIndex] == ':')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite = this.ParseSprite(text, null);
                    if (sprite != null)
                    {
                        num1 += thinButtons ? 6f : (float)(sprite.width * (double)sprite.scale.x + 1.0);
                        flag1 = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    if (this.ParseColor(text) != Colors.Transparent)
                        flag1 = true;
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '\n')
                {
                    if ((double)num1 > (double)num4)
                        num4 = num1;
                    num1 = 0.0f;
                    ++num3;
                    num2 += _charHeight * this.scale.y;
                    flag1 = true;
                    if (num3 >= maxRows)
                        return this._letterIndex;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (this.maxWidth > 0)
                    {
                        if (text[this._letterIndex] == ' ' || text[this._letterIndex] == '|' || text[this._letterIndex] == '@')
                        {
                            int index1 = this._letterIndex + 1;
                            float num5 = 0.0f;
                            for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, FancyBitmapFont._characterMap.Length - 1);
                                int character = FancyBitmapFont._characterMap[index2];
                                if (this._characterInfos != null)
                                {
                                    num5 += (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                                }
                                else
                                {
                                    Rectangle width = this._widths[character];
                                    num5 += (width.width - 1f) * this.scale.x;
                                }
                            }
                            if ((double)num1 + (double)num5 > maxWidth)
                            {
                                ++num3;
                                num2 += _charHeight * this.scale.y;
                                num1 = 0.0f;
                                flag2 = true;
                                if (num3 >= maxRows)
                                    return this._letterIndex;
                            }
                        }
                        else
                        {
                            char index = (char)Maths.Clamp(text[this._letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index];
                            if (this._characterInfos != null)
                            {
                                float num6 = (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                                if ((double)num1 + (double)num6 * scale.x > maxWidth)
                                {
                                    ++num3;
                                    num2 += _charHeight * this.scale.y;
                                    num1 = 0.0f;
                                    if (num3 >= maxRows)
                                        return this._letterIndex;
                                }
                            }
                            else
                            {
                                Rectangle width = this._widths[character];
                                if ((double)num1 + width.width * (double)this.scale.x > maxWidth)
                                {
                                    ++num3;
                                    num2 += _charHeight * this.scale.y;
                                    num1 = 0.0f;
                                    if (num3 >= maxRows)
                                        return this._letterIndex;
                                }
                            }
                        }
                    }
                    if (!flag2)
                    {
                        if (text[this._letterIndex] >= 'ぁ')
                        {
                            num1 += 8f * this.scale.x;
                        }
                        else
                        {
                            char index = (char)Maths.Clamp(text[this._letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index];
                            if (this._characterInfos != null)
                            {
                                if (character < this._characterInfos.Count)
                                    num1 += (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                                else
                                    continue;
                            }
                            else
                            {
                                Rectangle width = this._widths[character];
                                num1 += (width.width - 1f) * this.scale.x;
                            }
                        }
                    }
                }
                if ((double)num2 > (double)yPosition)
                    return this._letterIndex;
            }
            return this._letterIndex;
        }

        public Vec2 GetCharacterPosition(string text, int index, bool thinButtons = false)
        {
            float x = 0.0f;
            float y = 0.0f;
            float num1 = 0.0f;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                if (this._letterIndex >= index)
                    return new Vec2(x, y);
                bool flag1 = false;
                if (text[this._letterIndex] == '@' || this.chatFont && text[this._letterIndex] == ':')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite = this.ParseSprite(text, null);
                    if (sprite != null)
                    {
                        x += thinButtons ? 6f : (float)(sprite.width * (double)sprite.scale.x + 1.0);
                        flag1 = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    if (this.ParseColor(text) != Colors.Transparent)
                        flag1 = true;
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '\n')
                {
                    if ((double)x > (double)num1)
                        num1 = x;
                    x = 0.0f;
                    y += _charHeight * this.scale.y;
                    flag1 = true;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (this.maxWidth > 0)
                    {
                        if (text[this._letterIndex] == ' ' || text[this._letterIndex] == '|' || text[this._letterIndex] == '@')
                        {
                            int index1 = this._letterIndex + 1;
                            float num2 = 0.0f;
                            for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, FancyBitmapFont._characterMap.Length - 1);
                                int character = FancyBitmapFont._characterMap[index2];
                                if (this._characterInfos != null)
                                {
                                    num2 += (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                                }
                                else
                                {
                                    Rectangle width = this._widths[character];
                                    num2 += (width.width - 1f) * this.scale.x;
                                }
                            }
                            if ((double)x + (double)num2 > maxWidth)
                            {
                                y += _charHeight * this.scale.y;
                                x = 0.0f;
                                flag2 = true;
                            }
                        }
                        else
                        {
                            char index3 = (char)Maths.Clamp(text[this._letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            Rectangle width = this._widths[FancyBitmapFont._characterMap[index3]];
                            if ((double)x + width.width * (double)this.scale.x > maxWidth)
                            {
                                y += _charHeight * this.scale.y;
                                x = 0.0f;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        if (text[this._letterIndex] >= 'ぁ')
                        {
                            x += 8f * this.scale.x;
                        }
                        else
                        {
                            char index4 = (char)Maths.Clamp(text[this._letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index4];
                            if (this._characterInfos != null)
                            {
                                if (character < this._characterInfos.Count)
                                    x += (this._characterInfos[character].width + this._characterInfos[character].trailing + this._characterInfos[character].leading) * this.scale.x;
                            }
                            else
                            {
                                Rectangle width = this._widths[character];
                                x += (width.width - 1f) * this.scale.x;
                            }
                        }
                    }
                }
            }
            return new Vec2(x, y);
        }

        public void DrawOutline(
          string text,
          Vec2 pos,
          Color c,
          Color outline,
          Depth deep = default(Depth),
          float outlineThickness = 1f)
        {
            this._drawingOutline = true;
            this.Draw(text, pos + new Vec2(-outlineThickness, 0.0f), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(outlineThickness, 0.0f), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(0.0f, -outlineThickness), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(0.0f, outlineThickness), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(-outlineThickness, -outlineThickness), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(outlineThickness, -outlineThickness), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(-outlineThickness, outlineThickness), outline, deep + 2, true);
            this.Draw(text, pos + new Vec2(outlineThickness, outlineThickness), outline, deep + 2, true);
            this._drawingOutline = false;
            this.Draw(text, pos, c, deep + 5);
        }

        public void Draw(string text, Vec2 pos, Color c, Depth deep = default(Depth), bool colorSymbols = false) => this.Draw(text, pos.x, pos.y, c, deep, colorSymbols);

        public void Draw(
          string text,
          float xpos,
          float ypos,
          Color c,
          Depth deep = default(Depth),
          bool colorSymbols = false)
        {
            this._previousColor = c;
            if (string.IsNullOrWhiteSpace(text))
                return;
            Color color1 = new Color(byte.MaxValue - c.r, byte.MaxValue - c.g, byte.MaxValue - c.b);
            float num1 = 0.0f;
            float num2 = 0.0f;
            int num3 = 0;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                bool flag1 = false;
                if (text[this._letterIndex] == '@' || this.chatFont && text[this._letterIndex] == ':')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite1 = this.ParseSprite(text, null);
                    if (sprite1 != null)
                    {
                        float alpha = sprite1.alpha;
                        sprite1.alpha = this.alpha * c.ToVector4().w;
                        if (sprite1 != null)
                        {
                            float num4 = this.characterHeight / 2 - sprite1.height / 2 + this.symbolYOffset;
                            if (colorSymbols)
                                sprite1.color = c;
                            if (this.chatFont)
                            {
                                Vec2 scale = sprite1.scale;
                                Sprite sprite2 = sprite1;
                                sprite2.scale *= (this.scale.x / 2f);
                                if (this is RasterFont)
                                {
                                    float num5 = (float)((this as RasterFont).data.fontSize * (double)RasterFont.fontScaleFactor / 10.0);
                                    Sprite sprite3 = sprite1;
                                    sprite3.scale *= num5;
                                    sprite1.scale = new Vec2((float)Math.Round(sprite1.scale.x * 2.0) / 2f);
                                }
                                float num6 = (float)(characterHeight * (double)this.scale.y / 2.0 - sprite1.height * (double)sprite1.scale.y / 2.0);
                                Graphics.Draw(sprite1, xpos + num2, ypos + num1 + num6, deep + 10 + (int)(((double)ypos + (double)num1) / 10.0));
                                num2 += (float)(sprite1.width * (double)sprite1.scale.x + 1.0);
                                sprite1.scale = scale;
                            }
                            else if (this._rasterData != null)
                            {
                                Vec2 scale = sprite1.scale;
                                float num7 = this._rasterData.fontHeight / 24f;
                                Sprite sprite4 = sprite1;
                                sprite4.scale *= num7;
                                Graphics.Draw(sprite1, xpos + num2, (float)((double)ypos + (double)num1 + 1.0 * (double)num7), deep);
                                num2 += (float)(sprite1.width * (double)sprite1.scale.x + 1.0);
                                sprite1.scale = scale;
                            }
                            else
                            {
                                Graphics.Draw(sprite1, xpos + num2, ypos + num1 + num4, deep);
                                num2 += (float)(sprite1.width * (double)sprite1.scale.x + 1.0);
                            }
                            sprite1.color = Color.White;
                        }
                        sprite1.alpha = alpha;
                        flag1 = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    Color color2 = this.ParseColor(text);
                    if (color2 != Colors.Transparent)
                    {
                        this._previousColor = c;
                        if (!this._drawingOutline)
                        {
                            float w = c.ToVector4().w;
                            c = color2;
                            c *= w;
                        }
                        flag1 = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (this.maxWidth > 0)
                    {
                        if (text[this._letterIndex] == ' ' || text[this._letterIndex] == '|' || text[this._letterIndex] == '@')
                        {
                            int index1 = this._letterIndex + 1;
                            if (this.enforceWidthByWord)
                            {
                                char index2 = ' ';
                                float width1 = this._widths[FancyBitmapFont._characterMap[(byte)index2]].width;
                                for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                                {
                                    byte index3 = (byte)Maths.Clamp(text[index1], 0, 254);
                                    Rectangle width2 = this._widths[FancyBitmapFont._characterMap[index3]];
                                    width1 += (width2.width - 1f) * this.scale.x;
                                }
                                if ((double)num2 + (double)width1 > maxWidth)
                                {
                                    num1 += _charHeight * this.scale.y;
                                    num2 = 0.0f;
                                    ++num3;
                                    flag2 = true;
                                    if (this.singleLine)
                                        break;
                                }
                            }
                        }
                        else
                        {
                            byte index = (byte)Maths.Clamp(text[this._letterIndex], 0, 254);
                            Rectangle width = this._widths[FancyBitmapFont._characterMap[index]];
                            if ((double)num2 + width.width * (double)this.scale.x > maxWidth)
                            {
                                num1 += _charHeight * this.scale.y;
                                num2 = 0.0f;
                                ++num3;
                                if (this.singleLine)
                                    break;
                            }
                        }
                    }
                    if (this.maxRows != 0 && num3 >= this.maxRows)
                        break;
                    if (!flag2)
                    {
                        if (text[this._letterIndex] == '\n')
                        {
                            num1 += (_charHeight + this.lineGap) * this.scale.y;
                            num2 = 0.0f;
                            ++num3;
                        }
                        else
                        {
                            char index4 = text[this._letterIndex];
                            if (index4 >= 'ぁ')
                            {
                                int kanji = FancyBitmapFont._kanjiMap[index4];
                                FancyBitmapFont._kanjiSprite.frame = kanji;
                                FancyBitmapFont._kanjiSprite.scale = this.scale;
                                FancyBitmapFont._kanjiSprite.color = c;
                                FancyBitmapFont._kanjiSprite.alpha = this.alpha;
                                Graphics.Draw(_kanjiSprite, (float)((double)xpos + (double)num2 + 1.0), (float)((double)ypos + (double)num1 + 1.0), deep);
                                num2 += 8f * this.scale.x;
                            }
                            else
                            {
                                int index5 = FancyBitmapFont._characterMap[index4];
                                if (index5 >= this._widths.Count)
                                    index5 = this._widths.Count - 1;
                                if (index5 < 0)
                                    break;
                                Rectangle width = this._widths[index5];
                                this._texture.scale = this.scale;
                                if (this._highlightStart != -1 && this._highlightStart != this._highlightEnd && (this._highlightStart < this._highlightEnd && this._letterIndex >= this._highlightStart && this._letterIndex < this._highlightEnd || this._letterIndex < this._highlightStart && this._letterIndex >= this._highlightEnd))
                                {
                                    Graphics.DrawRect(new Vec2(xpos + num2, ypos + num1), new Vec2(xpos + num2, ypos + num1) + new Vec2(width.width * this.scale.x, _charHeight * this.scale.y), c, deep - 5);
                                    this._texture.color = color1;
                                }
                                else
                                    this._texture.color = c;
                                this._texture.alpha = this.alpha;
                                if (this._characterInfos != null)
                                {
                                    if (index5 < this._characterInfos.Count)
                                    {
                                        float num8 = num2 + this._characterInfos[index5].leading * this.scale.x;
                                        Graphics.Draw(this._texture, xpos + num8, ypos + num1, width, deep);
                                        num2 = num8 + this._characterInfos[index5].trailing * this.scale.x + this._characterInfos[index5].width * this.scale.x;
                                    }
                                }
                                else
                                {
                                    Graphics.Draw(this._texture, xpos + num2, ypos + num1, width, deep);
                                    num2 += (width.width - 1f) * this.scale.x;
                                }
                            }
                        }
                    }
                }
            }
        }

        public RichTextBox MakeRTF(string text)
        {
            RichTextBox richTextBox = new RichTextBox();
            Color color1 = Color.Black;
            string text1 = "";
            richTextBox.SelectionColor = System.Drawing.Color.Black;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                bool flag = false;
                if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    Color color2 = this.ParseColor(text);
                    if (color2 != Colors.Transparent)
                    {
                        this._previousColor = color1;
                        color1 = color2;
                        if (color2 == Color.White)
                            color2 = Color.Black;
                        richTextBox.AppendText(text1);
                        text1 = "";
                        richTextBox.SelectionColor = System.Drawing.Color.FromArgb(color2.r, color2.g, color2.b);
                        flag = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                if (text[this._letterIndex] == '\n')
                {
                    richTextBox.AppendText(text1);
                    text1 = "";
                    richTextBox.SelectionColor = System.Drawing.Color.Black;
                }
                if (!flag)
                    text1 += text[this._letterIndex].ToString();
            }
            richTextBox.AppendText(text1);
            return richTextBox;
        }
    }
}
