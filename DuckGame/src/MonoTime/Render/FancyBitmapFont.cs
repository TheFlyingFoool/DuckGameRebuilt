// Decompiled with JetBrains decompiler
// Type: DuckGame.FancyBitmapFont
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private int startingcoloroverride = -1;
        private int startingcolorindex;
        private int charcolorindex;

        public virtual Sprite _texture
        {
            get => _textureInternal;
            set => _textureInternal = value;
        }

        public static void InitializeKanjis()
        {
            if (_kanjiSprite != null)
                return;
            _kanjiSprite = new SpriteMap("kanji_font", 8, 8);
            string str = DuckFile.ReadAllText(DuckFile.contentDirectory + "kanji_map.txt");
            _kanjiMap = new ushort[ushort.MaxValue];
            for (int index = 0; index < str.Length; ++index)
                _kanjiMap[str[index]] = (ushort)index;
        }

        public float height => _texture.height * scale.y;

        public InputProfile inputProfile
        {
            get => _inputProfile;
            set => _inputProfile = value;
        }

        public int maxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        public int maxRows { get; set; }

        public FancyBitmapFont()
        {
        }

        public int characterHeight => _charHeight;

        public FancyBitmapFont(string image) => Construct(image);

        protected void Construct(string image)
        {
            InitializeKanjis();
            _texture = new Sprite(image);
            if (!widthMap.TryGetValue(image, out _widths))
            {
                _widths = new List<Rectangle>();
                Color[] data = _texture.texture.GetData();
                bool flag = false;
                int x = -1;
                for (int y = 1; y < _texture.height; y += _charHeight + 1)
                {
                    for (int index1 = 0; index1 < _texture.width; ++index1)
                    {
                        if (data[index1 + y * _texture.width].r == 0 && data[index1 + y * _texture.width].g == 0 && data[index1 + y * _texture.width].b == 0 && data[index1 + y * _texture.width].a == 0)
                        {
                            if (x == -1)
                                x = index1;
                        }
                        else if (x != -1)
                        {
                            if (_charHeight == 0)
                            {
                                _firstYPixel = y;
                                int num = index1 - 1;
                                for (int index2 = y + 1; index2 < _texture.height; ++index2)
                                {
                                    if (data[num + index2 * _texture.width].r != 0 || data[num + index2 * _texture.width].g != 0 || data[num + index2 * _texture.width].b != 0 || data[num + index2 * _texture.width].a != 0)
                                    {
                                        _charHeight = index2 - y;
                                        break;
                                    }
                                }
                                index1 = num + 1;
                            }
                            _widths.Add(new Rectangle(x, y, index1 - x, _charHeight));
                            x = -1;
                        }
                    }
                    if (flag)
                        break;
                }
            }
            widthMap[image] = _widths;
            if (_widths.Count > 0)
                _charHeight = (int)_widths[0].height;
            if (_mapInitialized)
                return;
            for (int index3 = 0; index3 < ushort.MaxValue; ++index3)
            {
                char ch = (char)index3;
                _characterMap[index3] = 91;
                for (int index4 = 0; index4 < _characters.Length; ++index4)
                {
                    if (_characters[index4] == ch)
                    {
                        _characterMap[index3] = index4;
                        break;
                    }
                }
            }
            _mapInitialized = true;
        }

        public Sprite ParseSprite(string text, InputProfile input)
        {
            if (text.StartsWith("_!"))
                return null;
            ++_letterIndex;
            string trigger = "";
            bool brokeWithAt = false;
            for (; _letterIndex != text.Length; ++_letterIndex)
            {
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    brokeWithAt = true;
                    break;
                }
                if (text[_letterIndex] == ' ' || text[_letterIndex] == '\n')
                {
                    --_letterIndex;
                    break;
                }
                trigger += text[_letterIndex].ToString();
            }
            if (chatFont && !brokeWithAt)
                return null;
            Sprite sprite = null;
            if (input != null)
                sprite = input.GetTriggerImage(trigger);
            if (sprite == null)
                sprite = Input.GetTriggerSprite(trigger);
            if (sprite == null && Options.Data.mojiFilter != 0)
            {
                sprite = DuckFile.GetMoji(trigger, _currentConnection);
                if (sprite == null && trigger.Contains("!"))
                    return Input.GetTriggerSprite("blankface");
            }

            Sprite spriteClone = null;
            if (sprite != null)
            {
                spriteClone = sprite.Clone();
                spriteClone.xscale = sprite.xscale * xscale;
                spriteClone.yscale = sprite.yscale * yscale;
            }

            return spriteClone;
        }

        public Color ParseColor(string text)
        {
            ++_letterIndex;
            string color = "";
            for (; _letterIndex != text.Length && text[_letterIndex] != ' ' && text[_letterIndex] != '|'; ++_letterIndex)
                color += text[_letterIndex].ToString();
            if (color == "PREV")
            {
                return new Color(_previousColor.r, _previousColor.g, _previousColor.b);
            }
            else
            {
                return Colors.ParseColor(color);
            }
        }

        public InputProfile GetInputProfile(InputProfile input)
        {
            if (input == null)
                input = _inputProfile != null ? _inputProfile : InputProfile.FirstProfileWithDevice;
            return input;
        }

        public string FormatWithNewlines(string pText, float maxWidth, bool thinButtons = false)
        {
            float num1 = 0f;
            float num2 = 0f;
            char[] charArray = pText.ToCharArray();
            for (_letterIndex = 0; _letterIndex < charArray.Length; ++_letterIndex)
            {
                bool flag = false;
                if (charArray[_letterIndex] == ' ' && num1 > maxWidth)
                    charArray[_letterIndex] = '\n';
                if (charArray[_letterIndex] == '@' || chatFont && charArray[_letterIndex] == ':')
                {
                    pText = new string(charArray);
                    int letterIndex = _letterIndex;
                    Sprite sprite = ParseSprite(pText, null);
                    if (sprite != null)
                    {
                        num1 += thinButtons ? 6f : (float)(sprite.width * sprite.scale.x + 1.0);
                        flag = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (charArray[_letterIndex] == '|')
                {
                    pText = new string(charArray);
                    int letterIndex = _letterIndex;
                    if (ParseColor(pText) != Colors.Transparent)
                        flag = true;
                    else
                        _letterIndex = letterIndex;
                }
                else if (charArray[_letterIndex] == '\n')
                {
                    if (num1 > num2)
                        num2 = num1;
                    num1 = 0f;
                }
                if (!flag)
                {
                    char index = charArray[_letterIndex];
                    if (index >= 'ぁ')
                    {
                        num1 += 8f * scale.x;
                    }
                    else
                    {
                        int character = _characterMap[index];
                        if (_characterInfos != null)
                        {
                            if (character < _characterInfos.Count)
                                num1 += (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                        }
                        else if (character < _widths.Count)
                        {
                            Rectangle width = _widths[character];
                            num1 += (width.width - 1f) * scale.x;
                        }
                    }
                }
            }
            if (num1 > num2)
                ;
            return new string(charArray);
        }

        public float GetWidth(string text, bool thinButtons = false)
        {
            text = LangHandler.Convert(text);
            float num1 = 0f;
            float width1 = 0f;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                bool flag = false;
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    int letterIndex = _letterIndex;
                    Sprite sprite1 = ParseSprite(text, null);
                    if (sprite1 != null)
                    {
                        if (chatFont)
                        {
                            Vec2 scale = sprite1.scale;
                            Sprite sprite2 = sprite1;
                            sprite2.scale *= (this.scale.x / 2f);
                            if (this is RasterFont)
                            {
                                float num2 = (float)((this as RasterFont).data.fontSize * RasterFont.fontScaleFactor / 10);
                                Sprite sprite3 = sprite1;
                                sprite3.scale *= num2;
                                sprite1.scale = new Vec2((float)Math.Round(sprite1.scale.x * 2) / 2f);
                            }
                            num1 += (float)(sprite1.width * sprite1.scale.x + 1);
                            sprite1.scale = scale;
                        }
                        else
                            num1 += thinButtons ? 6f : (float)(sprite1.width * sprite1.scale.x + 1);
                        flag = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    if (ParseColor(text) != Colors.Transparent)
                        flag = true;
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '\n')
                {
                    if (num1 > width1)
                        width1 = num1;
                    num1 = 0f;
                    flag = true;
                }
                if (!flag)
                {
                    char index1 = text[_letterIndex];
                    if (index1 >= 'ぁ')
                    {
                        num1 += 8f * scale.x;
                    }
                    else
                    {
                        int index2 = _characterMap[index1];
                        if (index2 >= _widths.Count)
                            index2 = _widths.Count - 1;
                        if (index2 < 0)
                            return width1;
                        if (_characterInfos != null)
                        {
                            if (index2 < _characterInfos.Count)
                                num1 += (_characterInfos[index2].width + _characterInfos[index2].trailing + _characterInfos[index2].leading) * scale.x;
                        }
                        else
                        {
                            Rectangle width2 = _widths[index2];
                            num1 += (width2.width - 1f) * scale.x;
                        }
                    }
                }
            }
            if (num1 > width1)
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
            float num1 = 0f;
            float num2 = 0f;
            int num3 = 0;
            float num4 = 0f;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                if (num1 >= xPosition && yPosition < num2 + _charHeight * scale.y || num3 >= maxRows)
                    return _letterIndex - 1;
                bool flag1 = false;
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    int letterIndex = _letterIndex;
                    Sprite sprite = ParseSprite(text, null);
                    if (sprite != null)
                    {
                        num1 += thinButtons ? 6f : (float)(sprite.width * sprite.scale.x + 1.0);
                        flag1 = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    if (ParseColor(text) != Colors.Transparent)
                        flag1 = true;
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '\n')
                {
                    if (num1 > num4)
                        num4 = num1;
                    num1 = 0f;
                    ++num3;
                    num2 += _charHeight * scale.y;
                    flag1 = true;
                    if (num3 >= maxRows)
                        return _letterIndex;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (maxWidth > 0)
                    {
                        if (text[_letterIndex] == ' ' || text[_letterIndex] == '|' || text[_letterIndex] == '@')
                        {
                            int index1 = _letterIndex + 1;
                            float num5 = 0f;
                            for (; index1 < text.Length && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, _characterMap.Length - 1);
                                int character = _characterMap[index2];
                                if (_characterInfos != null)
                                {
                                    num5 += (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                                }
                                else
                                {
                                    Rectangle width = _widths[character];
                                    num5 += (width.width - 1f) * scale.x;
                                }
                            }
                            if (num1 + num5 > maxWidth)
                            {
                                ++num3;
                                num2 += _charHeight * scale.y;
                                num1 = 0f;
                                flag2 = true;
                                if (num3 >= maxRows)
                                    return _letterIndex;
                            }
                        }
                        else
                        {
                            char index = (char)Maths.Clamp(text[_letterIndex], 0, _characterMap.Length - 1);
                            int character = _characterMap[index];
                            if (_characterInfos != null)
                            {
                                float num6 = (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                                if (num1 + num6 * scale.x > maxWidth)
                                {
                                    ++num3;
                                    num2 += _charHeight * scale.y;
                                    num1 = 0f;
                                    if (num3 >= maxRows)
                                        return _letterIndex;
                                }
                            }
                            else
                            {
                                Rectangle width = _widths[character];
                                if (num1 + width.width * scale.x > maxWidth)
                                {
                                    ++num3;
                                    num2 += _charHeight * scale.y;
                                    num1 = 0f;
                                    if (num3 >= maxRows)
                                        return _letterIndex;
                                }
                            }
                        }
                    }
                    if (!flag2)
                    {
                        if (text[_letterIndex] >= 'ぁ')
                        {
                            num1 += 8f * scale.x;
                        }
                        else
                        {
                            char index = (char)Maths.Clamp(text[_letterIndex], 0, _characterMap.Length - 1);
                            int character = _characterMap[index];
                            if (_characterInfos != null)
                            {
                                if (character < _characterInfos.Count)
                                    num1 += (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                                else
                                    continue;
                            }
                            else
                            {
                                Rectangle width = _widths[character];
                                num1 += (width.width - 1f) * scale.x;
                            }
                        }
                    }
                }
                if (num2 > yPosition)
                    return _letterIndex;
            }
            return _letterIndex;
        }

        public Vec2 GetCharacterPosition(string text, int index, bool thinButtons = false)
        {
            float x = 0f;
            float y = 0f;
            float num1 = 0f;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                if (_letterIndex >= index)
                    return new Vec2(x, y);
                bool flag1 = false;
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    int letterIndex = _letterIndex;
                    Sprite sprite = ParseSprite(text, null);
                    if (sprite != null)
                    {
                        x += thinButtons ? 6f : (float)(sprite.width * sprite.scale.x + 1.0);
                        flag1 = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    if (ParseColor(text) != Colors.Transparent)
                        flag1 = true;
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '\n')
                {
                    if (x > num1)
                        num1 = x;
                    x = 0f;
                    y += _charHeight * scale.y;
                    flag1 = true;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (maxWidth > 0)
                    {
                        if (text[_letterIndex] == ' ' || text[_letterIndex] == '|' || text[_letterIndex] == '@')
                        {
                            int index1 = _letterIndex + 1;
                            float num2 = 0f;
                            for (; index1 < text.Length && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, _characterMap.Length - 1);
                                int character = _characterMap[index2];
                                if (_characterInfos != null)
                                {
                                    num2 += (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                                }
                                else
                                {
                                    Rectangle width = _widths[character];
                                    num2 += (width.width - 1f) * scale.x;
                                }
                            }
                            if (x + num2 > maxWidth)
                            {
                                y += _charHeight * scale.y;
                                x = 0f;
                                flag2 = true;
                            }
                        }
                        else
                        {
                            char index3 = (char)Maths.Clamp(text[_letterIndex], 0, _characterMap.Length - 1);
                            Rectangle width = _widths[_characterMap[index3]];
                            if (x + width.width * scale.x > maxWidth)
                            {
                                y += _charHeight * scale.y;
                                x = 0f;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        if (text[_letterIndex] >= 'ぁ')
                        {
                            x += 8f * scale.x;
                        }
                        else
                        {
                            char index4 = (char)Maths.Clamp(text[_letterIndex], 0, _characterMap.Length - 1);
                            int character = _characterMap[index4];
                            if (_characterInfos != null)
                            {
                                if (character < _characterInfos.Count)
                                    x += (_characterInfos[character].width + _characterInfos[character].trailing + _characterInfos[character].leading) * scale.x;
                            }
                            else
                            {
                                Rectangle width = _widths[character];
                                x += (width.width - 1f) * scale.x;
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
            _drawingOutline = true;
            Draw(text, pos + new Vec2(-outlineThickness, 0f), outline, deep + 2, true);
            Draw(text, pos + new Vec2(outlineThickness, 0f), outline, deep + 2, true);
            Draw(text, pos + new Vec2(0f, -outlineThickness), outline, deep + 2, true);
            Draw(text, pos + new Vec2(0f, outlineThickness), outline, deep + 2, true);
            Draw(text, pos + new Vec2(-outlineThickness, -outlineThickness), outline, deep + 2, true);
            Draw(text, pos + new Vec2(outlineThickness, -outlineThickness), outline, deep + 2, true);
            Draw(text, pos + new Vec2(-outlineThickness, outlineThickness), outline, deep + 2, true);
            Draw(text, pos + new Vec2(outlineThickness, outlineThickness), outline, deep + 2, true);
            _drawingOutline = false;
            Draw(text, pos, c, deep + 5);
        }

        public void Draw(string text, Vec2 pos, Color c, Depth deep = default(Depth), bool colorSymbols = false) => Draw(text, pos.x, pos.y, c, deep, colorSymbols);

        public void Draw(string text, float xpos, float ypos, Color c, Depth deep = default(Depth), bool colorSymbols = false)
		{
			_previousColor = c;
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			Color highlight = new Color(byte.MaxValue - c.r, byte.MaxValue - c.g, byte.MaxValue - c.b);
			float yOff = 0f;
			float xOff = 0f;
			int curRow = 0;
			_letterIndex = 0;
			while (_letterIndex < text.Length)
			{
				bool processedSpecialCharacter = false;
                Vec4 cNormalized = c.ToVector4();
                cNormalized.w = 1;
                if (text[_letterIndex] == '@' || (chatFont && text[_letterIndex] == ':'))
				{
					int iPos = _letterIndex;
					Sprite spr = ParseSprite(text, null);
					if (spr != null)
					{
						float al = spr.alpha;
						spr.alpha = alpha * cNormalized.w;
						if (spr != null)
						{
							float yCenter = characterHeight / 2 - spr.height / 2;
							yCenter += symbolYOffset;
							if (colorSymbols)
							{
								spr.color = c;
							}
							if (chatFont)
							{
								Vec2 sprScale = spr.scale;
								spr.scale *= scale.x / 2f;
								if (this is RasterFont)
								{
									float scaleFac = (this as RasterFont).data.fontSize * RasterFont.fontScaleFactor / 10f;
									spr.scale *= scaleFac;
									spr.scale = new Vec2((float)Math.Round(spr.scale.x * 2f) / 2f);
								}
								yCenter = characterHeight * scale.y / 2f - spr.height * spr.scale.y / 2f;
								Graphics.Draw(spr, xpos + xOff, ypos + yOff + yCenter, deep + 10 + (int)((ypos + yOff) / 10f));
								xOff += spr.width * spr.scale.x + 1f;
								spr.scale = sprScale;
							}
							else if (_rasterData != null)
							{
								Vec2 sprScale2 = spr.scale;
								float sizeDif = _rasterData.fontHeight / 24f;
								spr.scale *= sizeDif;
								Graphics.Draw(spr, xpos + xOff, ypos + yOff + 1f * sizeDif, deep);
								xOff += spr.width * spr.scale.x + 1f;
								spr.scale = sprScale2;
							}
							else
							{
								Graphics.Draw(spr, xpos + xOff, ypos + yOff + yCenter, deep);
								xOff += spr.width * spr.scale.x + 1f;
							}
							spr.color = Color.White;
						}
						spr.alpha = al;
						processedSpecialCharacter = true;
					}
					else
					{
						_letterIndex = iPos;
					}
				}
				else if (text[_letterIndex] == '|')
				{
					int iPos2 = _letterIndex;
					Color col = ParseColor(text);
					if (col != Colors.Transparent)
					{
						_previousColor = c;
						if (!_drawingOutline)
						{
							float al2 = cNormalized.w;
							c = col;
							c *= al2;
						}
						processedSpecialCharacter = true;
					}
					else
					{
						_letterIndex = iPos2;
					}
				}
				if (!processedSpecialCharacter)
				{
					bool skippedLine = false;
					if (maxWidth > 0)
					{
						if (text[_letterIndex] == ' ' || text[_letterIndex] == '|' || text[_letterIndex] == '@')
						{
							int index = _letterIndex + 1;
							if (enforceWidthByWord)
							{
								char sp = ' ';
								float additionalWidth = _widths[_characterMap[(byte)sp]].width;
								while (index < text.Count<char>() && text[index] != ' ' && text[index] != '|' && text[index] != '@')
								{
									byte charVal = (byte)Maths.Clamp(text[index], 0, 254);
									int cIndex = _characterMap[charVal];
									Rectangle widthData = _widths[cIndex];
									additionalWidth += (widthData.width - 1f) * scale.x;
									index++;
								}
								if (xOff + additionalWidth > maxWidth)
								{
									yOff += _charHeight * scale.y;
									xOff = 0f;
									curRow++;
									skippedLine = true;
									if (singleLine)
									{
										return;
									}
								}
							}
						}
						else
						{
							byte charVal2 = (byte)Maths.Clamp(text[_letterIndex], 0, 254);
							int cIndex2 = _characterMap[charVal2];
							Rectangle widthData2 = _widths[cIndex2];
							if (xOff + widthData2.width * scale.x > maxWidth)
							{
								yOff += _charHeight * scale.y;
								xOff = 0f;
								curRow++;
								if (singleLine)
								{
									return;
								}
							}
						}
					}
					if (maxRows != 0 && curRow >= maxRows)
					{
						return;
					}
					if (!skippedLine)
					{
						if (text[_letterIndex] == '\n')
						{
							yOff += (_charHeight + lineGap) * scale.y;
							xOff = 0f;
							curRow++;
						}
						else
						{
							char charVal3 = text[_letterIndex];
							if (charVal3 >= 'ぁ')
							{
								int charIndex = _kanjiMap[charVal3];
								_kanjiSprite.frame = charIndex;
								_kanjiSprite.scale = scale;
								_kanjiSprite.color = c;
								_kanjiSprite.alpha = alpha;
								Graphics.Draw(_kanjiSprite, xpos + xOff + 1f, ypos + yOff + 1f, deep);
								xOff += 8f * scale.x;
							}
							else
							{
								int charIndex = _characterMap[charVal3];
								if (charIndex >= _widths.Count)
								{
									charIndex = _widths.Count - 1;
								}
								if (charIndex < 0)
								{
									return;
								}
								Rectangle dat = _widths[charIndex];
								_texture.scale = scale;
								if (_highlightStart != -1 && _highlightStart != _highlightEnd && ((_highlightStart < _highlightEnd && _letterIndex >= _highlightStart && _letterIndex < _highlightEnd) || (_letterIndex < _highlightStart && _letterIndex >= _highlightEnd)))
								{
									Graphics.DrawRect(new Vec2(xpos + xOff, ypos + yOff), new Vec2(xpos + xOff, ypos + yOff) + new Vec2(dat.width * scale.x, _charHeight * scale.y), c, deep - 5, true, 1f);
									_texture.color = highlight;
								}
								else
								{
									_texture.color = c;
								}
								_texture.alpha = alpha;
								if (_characterInfos != null)
								{
									if (charIndex < _characterInfos.Count)
									{
										xOff += _characterInfos[charIndex].leading * scale.x;
										Graphics.Draw(_texture, xpos + xOff, ypos + yOff, dat, deep);
										xOff += _characterInfos[charIndex].trailing * scale.x;
										xOff += _characterInfos[charIndex].width * scale.x;
									}
								}
								else
								{
									Graphics.Draw(_texture, xpos + xOff, ypos + yOff, dat, deep);
									xOff += (dat.width - 1f) * scale.x;
								}
							}
						}
					}
				}
				_letterIndex++;
			}
		}

        public RichTextBox MakeRTF(string text)
        {
            RichTextBox richTextBox = new RichTextBox();
            Color color2 = Color.Black;
            string text1 = "";
            richTextBox.SelectionColor = System.Drawing.Color.Black;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                bool flag = false;
                if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    if (color2 != Colors.Transparent)
                    {
                        _previousColor = color2;
                    }
                    color2 = ParseColor(text);
                    if (color2 != Colors.Transparent)
                    {
                        if (color2 == Color.White)
                            color2 = Color.Black;
                        richTextBox.AppendText(text1);
                        text1 = "";
                        richTextBox.SelectionColor = System.Drawing.Color.FromArgb(color2.r, color2.g, color2.b);
                        flag = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                if (text[_letterIndex] == '\n')
                {
                    richTextBox.AppendText(text1);
                    text1 = "";
                    richTextBox.SelectionColor = System.Drawing.Color.Black;
                }
                if (!flag)
                    text1 += text[_letterIndex].ToString();
            }
            richTextBox.AppendText(text1);
            return richTextBox;
        }
    }
}
