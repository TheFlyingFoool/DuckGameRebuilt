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

        public virtual Sprite _texture
        {
            get => _textureInternal;
            set => _textureInternal = value;
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
            FancyBitmapFont.InitializeKanjis();
            _texture = new Sprite(image);
            if (!FancyBitmapFont.widthMap.TryGetValue(image, out _widths))
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
            FancyBitmapFont.widthMap[image] = _widths;
            if (_widths.Count > 0)
                _charHeight = (int)_widths[0].height;
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
            ++_letterIndex;
            string str = "";
            bool flag = false;
            for (; _letterIndex != text.Length; ++_letterIndex)
            {
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    flag = true;
                    break;
                }
                if (text[_letterIndex] == ' ' || text[_letterIndex] == '\n')
                {
                    --_letterIndex;
                    break;
                }
                str += text[_letterIndex].ToString();
            }
            if (chatFont && !flag)
                return null;
            Sprite sprite = null;
            if (input != null)
                sprite = input.GetTriggerImage(str);
            if (sprite == null)
                sprite = Input.GetTriggerSprite(str);
            if (sprite == null && Options.Data.mojiFilter != 0)
            {
                sprite = DuckFile.GetMoji(str, _currentConnection);
                if (sprite == null && str.Contains("!"))
                    return Input.GetTriggerSprite("blankface");
            }
            return sprite;
        }

        public Color ParseColor(string text)
        {
            ++_letterIndex;
            string color = "";
            for (; _letterIndex != text.Length && text[_letterIndex] != ' ' && text[_letterIndex] != '|'; ++_letterIndex)
                color += text[_letterIndex].ToString();
            return color == "PREV" ? new Color(_previousColor.r, _previousColor.g, _previousColor.b) : Colors.ParseColor(color);
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
                        int character = FancyBitmapFont._characterMap[index];
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
                                float num2 = (float)((this as RasterFont).data.fontSize * RasterFont.fontScaleFactor / 10.0);
                                Sprite sprite3 = sprite1;
                                sprite3.scale *= num2;
                                sprite1.scale = new Vec2((float)Math.Round(sprite1.scale.x * 2.0) / 2f);
                            }
                            num1 += (float)(sprite1.width * sprite1.scale.x + 1.0);
                            sprite1.scale = scale;
                        }
                        else
                            num1 += thinButtons ? 6f : (float)(sprite1.width * sprite1.scale.x + 1.0);
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
                        int index2 = FancyBitmapFont._characterMap[index1];
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
                            for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, FancyBitmapFont._characterMap.Length - 1);
                                int character = FancyBitmapFont._characterMap[index2];
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
                            char index = (char)Maths.Clamp(text[_letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index];
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
                            char index = (char)Maths.Clamp(text[_letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index];
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
                            for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                            {
                                char index2 = (char)Maths.Clamp(text[index1], 0, FancyBitmapFont._characterMap.Length - 1);
                                int character = FancyBitmapFont._characterMap[index2];
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
                            char index3 = (char)Maths.Clamp(text[_letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            Rectangle width = _widths[FancyBitmapFont._characterMap[index3]];
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
                            char index4 = (char)Maths.Clamp(text[_letterIndex], 0, FancyBitmapFont._characterMap.Length - 1);
                            int character = FancyBitmapFont._characterMap[index4];
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

        public void Draw(
          string text,
          float xpos,
          float ypos,
          Color c,
          Depth deep = default(Depth),
          bool colorSymbols = false)
        {
            _previousColor = c;
            if (string.IsNullOrWhiteSpace(text))
                return;
            Color color1 = new Color(byte.MaxValue - c.r, byte.MaxValue - c.g, byte.MaxValue - c.b);
            float num1 = 0f;
            float num2 = 0f;
            int num3 = 0;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                bool flag1 = false;
                if (text[_letterIndex] == '@' || chatFont && text[_letterIndex] == ':')
                {
                    int letterIndex = _letterIndex;
                    Sprite sprite1 = ParseSprite(text, null);
                    if (sprite1 != null)
                    {
                        float alpha = sprite1.alpha;
                        sprite1.alpha = this.alpha * c.ToVector4().w;
                        if (sprite1 != null)
                        {
                            float num4 = characterHeight / 2 - sprite1.height / 2 + symbolYOffset;
                            if (colorSymbols)
                                sprite1.color = c;
                            if (chatFont)
                            {
                                Vec2 scale = sprite1.scale;
                                Sprite sprite2 = sprite1;
                                sprite2.scale *= (this.scale.x / 2f);
                                if (this is RasterFont)
                                {
                                    float num5 = (float)((this as RasterFont).data.fontSize * RasterFont.fontScaleFactor / 10.0);
                                    Sprite sprite3 = sprite1;
                                    sprite3.scale *= num5;
                                    sprite1.scale = new Vec2((float)Math.Round(sprite1.scale.x * 2.0) / 2f);
                                }
                                float num6 = (float)(characterHeight * this.scale.y / 2.0 - sprite1.height * sprite1.scale.y / 2.0);
                                Graphics.Draw(sprite1, xpos + num2, ypos + num1 + num6, deep + 10 + (int)((ypos + num1) / 10.0));
                                num2 += (float)(sprite1.width * sprite1.scale.x + 1.0);
                                sprite1.scale = scale;
                            }
                            else if (_rasterData != null)
                            {
                                Vec2 scale = sprite1.scale;
                                float num7 = _rasterData.fontHeight / 24f;
                                Sprite sprite4 = sprite1;
                                sprite4.scale *= num7;
                                Graphics.Draw(sprite1, xpos + num2, (float)(ypos + num1 + 1.0 * num7), deep);
                                num2 += (float)(sprite1.width * sprite1.scale.x + 1.0);
                                sprite1.scale = scale;
                            }
                            else
                            {
                                Graphics.Draw(sprite1, xpos + num2, ypos + num1 + num4, deep);
                                num2 += (float)(sprite1.width * sprite1.scale.x + 1.0);
                            }
                            sprite1.color = Color.White;
                        }
                        sprite1.alpha = alpha;
                        flag1 = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    Color color2 = ParseColor(text);
                    if (color2 != Colors.Transparent)
                    {
                        _previousColor = c;
                        if (!_drawingOutline)
                        {
                            float w = c.ToVector4().w;
                            c = color2;
                            c *= w;
                        }
                        flag1 = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                if (!flag1)
                {
                    bool flag2 = false;
                    if (maxWidth > 0)
                    {
                        if (text[_letterIndex] == ' ' || text[_letterIndex] == '|' || text[_letterIndex] == '@')
                        {
                            int index1 = _letterIndex + 1;
                            if (enforceWidthByWord)
                            {
                                char index2 = ' ';
                                float width1 = _widths[FancyBitmapFont._characterMap[(byte)index2]].width;
                                for (; index1 < text.Count<char>() && text[index1] != ' ' && text[index1] != '|' && text[index1] != '@'; ++index1)
                                {
                                    byte index3 = (byte)Maths.Clamp(text[index1], 0, 254);
                                    Rectangle width2 = _widths[FancyBitmapFont._characterMap[index3]];
                                    width1 += (width2.width - 1f) * scale.x;
                                }
                                if (num2 + width1 > maxWidth)
                                {
                                    num1 += _charHeight * scale.y;
                                    num2 = 0f;
                                    ++num3;
                                    flag2 = true;
                                    if (singleLine)
                                        break;
                                }
                            }
                        }
                        else
                        {
                            byte index = (byte)Maths.Clamp(text[_letterIndex], 0, 254);
                            Rectangle width = _widths[FancyBitmapFont._characterMap[index]];
                            if (num2 + width.width * scale.x > maxWidth)
                            {
                                num1 += _charHeight * scale.y;
                                num2 = 0f;
                                ++num3;
                                if (singleLine)
                                    break;
                            }
                        }
                    }
                    if (maxRows != 0 && num3 >= maxRows)
                        break;
                    if (!flag2)
                    {
                        if (text[_letterIndex] == '\n')
                        {
                            num1 += (_charHeight + lineGap) * scale.y;
                            num2 = 0f;
                            ++num3;
                        }
                        else
                        {
                            char index4 = text[_letterIndex];
                            if (index4 >= 'ぁ')
                            {
                                int kanji = FancyBitmapFont._kanjiMap[index4];
                                FancyBitmapFont._kanjiSprite.frame = kanji;
                                FancyBitmapFont._kanjiSprite.scale = scale;
                                FancyBitmapFont._kanjiSprite.color = c;
                                FancyBitmapFont._kanjiSprite.alpha = alpha;
                                Graphics.Draw(_kanjiSprite, (float)(xpos + num2 + 1.0), (float)(ypos + num1 + 1.0), deep);
                                num2 += 8f * scale.x;
                            }
                            else
                            {
                                int index5 = FancyBitmapFont._characterMap[index4];
                                if (index5 >= _widths.Count)
                                    index5 = _widths.Count - 1;
                                if (index5 < 0)
                                    break;
                                Rectangle width = _widths[index5];
                                _texture.scale = scale;
                                if (_highlightStart != -1 && _highlightStart != _highlightEnd && (_highlightStart < _highlightEnd && _letterIndex >= _highlightStart && _letterIndex < _highlightEnd || _letterIndex < _highlightStart && _letterIndex >= _highlightEnd))
                                {
                                    Graphics.DrawRect(new Vec2(xpos + num2, ypos + num1), new Vec2(xpos + num2, ypos + num1) + new Vec2(width.width * scale.x, _charHeight * scale.y), c, deep - 5);
                                    _texture.color = color1;
                                }
                                else
                                    _texture.color = c;
                                _texture.alpha = alpha;
                                if (_characterInfos != null)
                                {
                                    if (index5 < _characterInfos.Count)
                                    {
                                        float num8 = num2 + _characterInfos[index5].leading * scale.x;
                                        Graphics.Draw(_texture, xpos + num8, ypos + num1, width, deep);
                                        num2 = num8 + _characterInfos[index5].trailing * scale.x + _characterInfos[index5].width * scale.x;
                                    }
                                }
                                else
                                {
                                    Graphics.Draw(_texture, xpos + num2, ypos + num1, width, deep);
                                    num2 += (width.width - 1f) * scale.x;
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
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                bool flag = false;
                if (text[_letterIndex] == '|')
                {
                    int letterIndex = _letterIndex;
                    Color color2 = ParseColor(text);
                    if (color2 != Colors.Transparent)
                    {
                        _previousColor = color1;
                        color1 = color2;
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
