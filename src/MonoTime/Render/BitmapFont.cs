// Decompiled with JetBrains decompiler
// Type: DuckGame.BitmapFont
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    public class BitmapFont : Transform
    {
        private SpriteMap _texture;
        public static SpriteMap _japaneseCharacters;
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
      '¿',
      'Ё',
      'ё'
        };
        private static int[] _characterMap = new int[(int)ushort.MaxValue];
        private const int kTilesPerRow = 16;
        private int _tileSize = 8;
        public int fallbackIndex;
        private BitmapFont _fallbackFont;
        private InputProfile _inputProfile;
        private Sprite _titleWing;
        private int _maxWidth;
        public bool allowBigSprites;
        private int _letterIndex;
        public bool singleLine;
        public bool enforceWidthByWord = true;
        private Color _previousColor;
        public int characterYOffset;
        public Vec2 spriteScale = new Vec2(1f, 1f);
        public Color colorOverride;

        public float height => (float)this._texture.height * this.scale.y;

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

        public BitmapFont(string image, int size, int ysize = -1)
        {
            FancyBitmapFont.InitializeKanjis();
            if (ysize < 0)
                ysize = size;
            this._texture = new SpriteMap(image, size, ysize);
            this._tileSize = size;
            if (!BitmapFont._mapInitialized)
            {
                for (int index1 = 0; index1 < (int)ushort.MaxValue; ++index1)
                {
                    char ch = (char)index1;
                    BitmapFont._characterMap[index1] = 91;
                    for (int index2 = 0; index2 < BitmapFont._characters.Length; ++index2)
                    {
                        if ((int)BitmapFont._characters[index2] == (int)ch)
                        {
                            BitmapFont._characterMap[index1] = index2;
                            break;
                        }
                    }
                }
                BitmapFont._mapInitialized = true;
            }
            this._titleWing = new Sprite("arcade/titleWing");
        }

        public Sprite ParseSprite(string text, InputProfile input)
        {
            if (!this.allowBigSprites && text.StartsWith("_!"))
                return (Sprite)null;
            ++this._letterIndex;
            string str = "";
            for (; this._letterIndex != text.Length && text[this._letterIndex] != ' ' && text[this._letterIndex] != '@'; ++this._letterIndex)
                str += text[this._letterIndex].ToString();
            Sprite sprite = (Sprite)null;
            if (input != null)
            {
                sprite = input.GetTriggerImage(str);
                if (sprite == null && Triggers.IsTrigger(str))
                    return new Sprite();
            }
            if (sprite == null)
                sprite = Input.GetTriggerSprite(str);
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

        public float GetWidth(string text, bool thinButtons = false, InputProfile input = null)
        {
            bool flag1 = false;
            if (input == null)
            {
                if (!MonoMain.started)
                {
                    input = InputProfile.DefaultPlayer1;
                }
                else
                {
                    input = this._inputProfile != null ? this._inputProfile : Input.lastActiveProfile;
                    if (this._inputProfile == null && Profiles.active.Count > 0 && !Network.isActive)
                        input = Profiles.GetLastProfileWithInput().inputProfile;
                }
            }
            float num = 0.0f;
            float width = 0.0f;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                bool flag2 = false;
                if (text[this._letterIndex] == '@')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite = this.ParseSprite(text, input);
                    if (sprite != null)
                    {
                        if (sprite.texture != null)
                        {
                            num += !thinButtons || flag1 ? (float)((double)sprite.width * (double)sprite.scale.x + 1.0) : 6f;
                            flag1 = true;
                        }
                        flag2 = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    if (this.ParseColor(text) != Colors.Transparent)
                        flag2 = true;
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '\n')
                {
                    if ((double)num > (double)width)
                        width = num;
                    num = 0.0f;
                }
                if (!flag2)
                    num += (float)this._tileSize * this.scale.x;
            }
            if ((double)num > (double)width)
                width = num;
            return width;
        }

        public void DrawOutline(string text, Vec2 pos, Color c, Color outline, Depth deep = default(Depth))
        {
            this.Draw(text, pos + new Vec2(-1f * this.scale.x, 0.0f), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(1f * this.scale.x, 0.0f), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(0.0f, -1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(0.0f, 1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(-1f * this.scale.x, -1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(1f * this.scale.x, -1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(-1f * this.scale.x, 1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos + new Vec2(1f * this.scale.x, 1f * this.scale.y), outline, deep + 2, colorSymbols: true);
            this.Draw(text, pos, c, deep + 5);
        }

        public void Draw(
          string text,
          Vec2 pos,
          Color c,
          Depth deep = default(Depth),
          InputProfile input = null,
          bool colorSymbols = false)
        {
            this.Draw(text, pos.x, pos.y, c, deep, input, colorSymbols);
        }

        public void Draw(
          string text,
          float xpos,
          float ypos,
          Color c,
          Depth deep = default(Depth),
          InputProfile input = null,
          bool colorSymbols = false)
        {
            if (this.colorOverride != new Color())
                c = this.colorOverride;
            this._previousColor = c;
            if (input == null)
            {
                if (!MonoMain.started)
                {
                    input = InputProfile.DefaultPlayer1;
                }
                else
                {
                    input = this._inputProfile != null ? this._inputProfile : Input.lastActiveProfile;
                    if (this._inputProfile == null && Profiles.active.Count > 0 && !Network.isActive)
                        input = Profiles.GetLastProfileWithInput().inputProfile;
                }
            }
            float num1 = 0.0f;
            float num2 = 0.0f;
            for (this._letterIndex = 0; this._letterIndex < text.Length; ++this._letterIndex)
            {
                bool flag = false;
                if (text[this._letterIndex] == '@')
                {
                    int letterIndex = this._letterIndex;
                    Sprite sprite1 = this.ParseSprite(text, input);
                    if (sprite1 != null)
                    {
                        if (sprite1.texture != null)
                        {
                            float alpha = sprite1.alpha;
                            sprite1.alpha = this.alpha * c.ToVector4().w;
                            if (sprite1 != null)
                            {
                                Vec2 scale = sprite1.scale;
                                Sprite sprite2 = sprite1;
                                sprite2.scale = sprite2.scale * this.spriteScale;
                                float num3 = (float)((int)((double)this._texture.height * (double)this.spriteScale.y / 2.0) - (int)((double)sprite1.height * (double)this.spriteScale.y / 2.0));
                                if (sprite1.moji)
                                {
                                    if (sprite1.height == 28)
                                    {
                                        Sprite sprite3 = sprite1;
                                        sprite3.scale = sprite3.scale * (0.25f * this.scale);
                                        num3 += 10f * this.scale.y;
                                    }
                                    else
                                    {
                                        Sprite sprite4 = sprite1;
                                        sprite4.scale = sprite4.scale * (0.25f * this.scale);
                                        num3 += 3f * this.scale.y;
                                    }
                                }
                                if (colorSymbols)
                                    sprite1.color = c;
                                Graphics.Draw(sprite1, xpos + num2, ypos + num1 + num3, deep);
                                num2 += (float)((double)sprite1.width * (double)sprite1.scale.x + 1.0);
                                sprite1.scale = scale;
                                sprite1.color = Color.White;
                            }
                            sprite1.alpha = alpha;
                        }
                        flag = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                else if (text[this._letterIndex] == '|')
                {
                    int letterIndex = this._letterIndex;
                    Color color = this.ParseColor(text);
                    if (this.colorOverride != new Color())
                        color = this.colorOverride;
                    if (color != Colors.Transparent)
                    {
                        this._previousColor = c;
                        float w = c.ToVector4().w;
                        c = color;
                        c *= w;
                        flag = true;
                    }
                    else
                        this._letterIndex = letterIndex;
                }
                if (!flag)
                {
                    if (this.maxWidth > 0)
                    {
                        string source = "";
                        int letterIndex = this._letterIndex;
                        while (letterIndex < text.Count<char>() && text[letterIndex] != ' ' && text[letterIndex] != '|' && text[letterIndex] != '@')
                        {
                            source += text[letterIndex].ToString();
                            ++letterIndex;
                            if (!this.enforceWidthByWord)
                                break;
                        }
                        if ((double)num2 + (double)source.Count<char>() * ((double)this._tileSize * (double)this.scale.x) > (double)this.maxWidth)
                        {
                            num1 += (float)this._texture.height * this.scale.y;
                            num2 = 0.0f;
                            if (this.singleLine)
                                break;
                        }
                    }
                    if (text[this._letterIndex] == '\n')
                    {
                        num1 += (float)this._texture.height * this.scale.y;
                        num2 = 0.0f;
                    }
                    else
                    {
                        SpriteMap g = this._texture;
                        char index = text[this._letterIndex];
                        int num4;
                        if (index >= 'ぁ')
                        {
                            g = FancyBitmapFont._kanjiSprite;
                            num4 = (int)FancyBitmapFont._kanjiMap[(int)index];
                        }
                        else
                            num4 = BitmapFont._characterMap[(int)text[this._letterIndex]];
                        if (this.fallbackIndex != 0 && num4 >= this.fallbackIndex)
                        {
                            if (this._fallbackFont == null)
                                this._fallbackFont = new BitmapFont("biosFont", 8);
                            g = this._fallbackFont._texture;
                        }
                        g.frame = num4;
                        g.scale = this.scale;
                        g.color = c;
                        g.alpha = this.alpha;
                        Graphics.Draw((Sprite)g, xpos + num2, ypos + num1 + (float)this.characterYOffset, deep);
                        num2 += (float)this._tileSize * this.scale.x;
                    }
                }
            }
        }
    }
}
