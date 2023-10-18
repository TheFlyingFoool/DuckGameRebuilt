// Decompiled with JetBrains decompiler
// Type: DuckGame.BitmapFont
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    public class BitmapFont : Transform
    {
        public float ySpacing = 0;
        private SpriteMap _texture;
        public static SpriteMap _japaneseCharacters;
        public int charcolorindex;
        public int startingcoloroverride = -1;
        public int startingcolorindex = -1;
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
        private static int[] _characterMap = new int[ushort.MaxValue];
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
        public char spritechar = '@';
        public char colorchar = '|';
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

        public BitmapFont(string image, int size, int ysize = -1)
        {
            FancyBitmapFont.InitializeKanjis();
            if (ysize < 0)
                ysize = size;
            _texture = new SpriteMap(image, size, ysize);
            _tileSize = size;
            if (!_mapInitialized)
            {
                for (int index2 = 0; index2 < _characters.Length; ++index2)
                {
                    char ch = _characters[index2];
                    _characterMap[ch] = index2;
                }
                _mapInitialized = true;
            }
            _titleWing = new Sprite("arcade/titleWing");
        }

        public Sprite ParseSprite(string text, InputProfile input)
        {
            if (!allowBigSprites && text.StartsWith("_!"))
                return null;
            ++_letterIndex;
            string trigger = "";
            for (; _letterIndex != text.Length && text[_letterIndex] != ' ' && text[_letterIndex] != spritechar; ++_letterIndex)
                trigger += text[_letterIndex].ToString();
            Sprite sprite = null;
            if (input != null)
            {
                sprite = input.GetTriggerImage(trigger);
                if (sprite == null && Triggers.IsTrigger(trigger))
                    return new Sprite();
            }

            if (sprite == null)
                sprite = Input.GetTriggerSprite(trigger);

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
            for (; _letterIndex != text.Length && text[_letterIndex] != ' ' && text[_letterIndex] != colorchar; ++_letterIndex)
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

        public float GetWidth(string text, bool thinButtons = false, InputProfile input = null)
        {
            text = LangHandler.Convert(text);
            bool flag1 = false;
            if (input == null)
            {
                if (!MonoMain.started)
                {
                    input = InputProfile.DefaultPlayer1;
                }
                else
                {
                    input = _inputProfile != null ? _inputProfile : Input.lastActiveProfile;
                    if (_inputProfile == null && !Network.isActive)
                    {
                        Profile profileWithInput = Profiles.GetLastProfileWithInput();
                        if (profileWithInput != null)
                        {
                            input = profileWithInput.inputProfile;
                        }
                    }
                }
            }
            float num = 0f;
            float width = 0f;
            for (_letterIndex = 0; _letterIndex < text.Length; ++_letterIndex)
            {
                bool flag2 = false;
                if (text[_letterIndex] == spritechar)
                {
                    int letterIndex = _letterIndex;
                    Sprite sprite = ParseSprite(text, input);
                    if (sprite != null)
                    {
                        if (sprite.texture != null)
                        {
                            num += !thinButtons || flag1 ? (float)(sprite.width * sprite.scale.x + 1f) : 6f;
                            flag1 = true;
                        }
                        flag2 = true;
                    }
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == colorchar)
                {
                    int letterIndex = _letterIndex;
                    if (ParseColor(text) != Colors.Transparent)
                        flag2 = true;
                    else
                        _letterIndex = letterIndex;
                }
                else if (text[_letterIndex] == '\n')
                {
                    if (num > width)
                        width = num;
                    num = 0f;
                }
                if (!flag2)
                    num += _tileSize * scale.x;
            }
            if (num > width)
                width = num;
            return width;
        }

        public void DrawOutline(string text, Vec2 pos, Color c, Color outline, Depth deep = default(Depth))
        {
            if (Program.gay)
            {
                Random t = new Random(text.Length + (int)pos.y + (int)pos.x);
                startingcolorindex = t.Next(0, Color.RainbowColors.Count);
                //this.startingcoloroverride = startingcolorindex;
                //if (startingcolorindex == -1)
                //{
                //    startingcolorindex = Rando.Int(Color.RainbowColors.Count - 1);
                //}
                startingcoloroverride = (Color.RainbowColors.Count - 1) - startingcolorindex;
                
                string cleanText = text.CleanFormatting(Extensions.CleanMethod.Color);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, 0f), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, 0f), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(0f, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(0f, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                int newcolor = startingcolorindex;
                if (newcolor == startingcoloroverride)
                {
                    newcolor += 2;
                    if (newcolor >= Colors.Rainbow.Length)
                    {
                        newcolor = 0;
                    }
                }
                startingcoloroverride = newcolor;
                Draw(text, pos, c, deep + 5);
            }
            else
            {
                string cleanText = text.CleanFormatting(Extensions.CleanMethod.Color);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, 0f), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, 0f), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(0f, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(0f, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, -1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(-1f * scale.x, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(cleanText, pos + new Vec2(1f * scale.x, 1f * scale.y), outline, deep + 2, colorSymbols: true);
                Draw(text, pos, c, deep + 5);
            }
        }

        public void Draw(
          string text,
          Vec2 pos,
          Color c,
          Depth deep = default(Depth),
          InputProfile input = null,
          bool colorSymbols = false)
        {
            Draw(text, pos.x, pos.y, c, deep, input, colorSymbols);
        }

        public void Draw(string text, float xpos, float ypos, Color c, Depth deep = default(Depth), InputProfile input = null, bool colorSymbols = false)
		{
			if (colorOverride != default)
			{
				c = colorOverride;
			}
			_previousColor = c;
			if (input == null)
			{
				if (!MonoMain.started)
				{
					input = InputProfile.DefaultPlayer1;
				}
				else
				{
					input = _inputProfile ?? Input.lastActiveProfile;
					if (_inputProfile == null && Profiles.active.Count > 0 && !Network.isActive)
					{
						input = Profiles.GetLastProfileWithInput().inputProfile;
					}
				}
			}
			float yOff = 0f;
			float xOff = 0f;
			_letterIndex = 0;
			while (_letterIndex < text.Length)
			{
				bool processedSpecialCharacter = false;
				if (text[_letterIndex] == '@')
				{
					int iPos = _letterIndex;
					Sprite spr = ParseSprite(text, input);
					if (spr != null)
					{
						if (spr.texture != null)
						{
							float al = spr.alpha;
							spr.alpha = alpha * c.ToVector4().w;
							if (spr != null)
							{
								Vec2 sc = spr.scale;
								spr.scale *= spriteScale;
								float yCenter = (int)(_texture.height * spriteScale.y / 2f) - (int)(spr.height * spriteScale.y / 2f);
								if (spr.moji)
								{
									if (spr.height == 28)
									{
										spr.scale *= 0.25f * scale;
										yCenter += 10f * scale.y;
									}
									else
									{
										spr.scale *= 0.25f * scale;
										yCenter += 3f * scale.y;
									}
								}
								if (colorSymbols)
								{
									spr.color = c;
								}
								Graphics.Draw(spr, xpos + xOff, ypos + yOff + yCenter, deep);
								xOff += spr.width * spr.scale.x + 1f;
								spr.scale = sc;
								spr.color = Color.White;
							}
							spr.alpha = al;
						}
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
					if (colorOverride != default(Color))
					{
						col = colorOverride;
					}
					if (col != Colors.Transparent)
					{
						_previousColor = c;
						float al2 = c.ToVector4().w;
						c = col;
						// c *= al2;
						processedSpecialCharacter = true;
					}
					else
					{
						_letterIndex = iPos2;
					}
				}
				if (!processedSpecialCharacter)
				{
					if (maxWidth > 0)
					{
						string nextWord = "";
						int index = _letterIndex;
						while (index < text.Length && text[index] != ' ' && text[index] != '|' && text[index] != '@')
						{
							nextWord += text[index].ToString();
							index++;
							if (!enforceWidthByWord)
							{
								break;
							}
						}
						if (xOff + nextWord.Count() * (_tileSize * scale.x) > maxWidth)
						{
							yOff += _texture.height * scale.y;
							xOff = 0f;
							if (singleLine)
							{
								return;
							}
						}
					}
					if (text[_letterIndex] == '\n')
					{
						yOff += _texture.height * scale.y;
						xOff = 0f;
					}
					else
					{
						SpriteMap fontTexture = _texture;
						char character = text[_letterIndex];
						int charIndex;
						if (character >= 'ぁ')
						{
							fontTexture = FancyBitmapFont._kanjiSprite;
							charIndex = FancyBitmapFont._kanjiMap[character];
						}
						else
						{
							charIndex = _characterMap[text[_letterIndex]];
						}
						if (fallbackIndex != 0 && charIndex >= fallbackIndex)
						{
							if (_fallbackFont == null)
							{
								_fallbackFont = new BitmapFont("biosFont", 8, -1);
							}
							fontTexture = _fallbackFont._texture;
						}
						fontTexture.frame = charIndex;
						fontTexture.scale = scale;
						fontTexture.color = c;
						fontTexture.alpha = alpha;
						Graphics.Draw(fontTexture, xpos + xOff, ypos + yOff + characterYOffset, deep);
						xOff += _tileSize * scale.x;
					}
				}
				_letterIndex++;
			}
		}
    }
}
