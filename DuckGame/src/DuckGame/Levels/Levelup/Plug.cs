using System.Collections.Generic;

namespace DuckGame
{
    public class Plug
    {
        public static Plug context = new Plug();
        public static List<VincentProduct> products = new List<VincentProduct>();
        public static float alpha = 0f;
        private static FancyBitmapFont _font;
        private static SpriteMap _dealer;
        private static List<string> _lines = new List<string>();
        private static string _currentLine = "";
        private static List<TextLine> _lineProgress = new List<TextLine>();
        private static float _waitLetter = 1f;
        private static float _waitAfterLine = 1f;
        private static float _talkMove = 0f;
        //private static float _showLerp = 0f;
        private static bool _allowMovement = false;
        public static bool open = false;
        private static int frame = 0;
        private static bool killSkip = false;
        private static float _extraWait = 0f;
        private static int colorLetters = 0;
        private static float _chancyLerp = 0f;
        private static string lastWord = "";
        private static int wait = 0;

        public static void Clear()
        {
            _lines.Clear();
            _waitLetter = 0f;
            _waitAfterLine = 0f;
            _currentLine = "";
        }

        public static void Add(string line) => _lines.Add(line);

        public static void Open()
        {
            _lineProgress.Clear();
            //Plug._showLerp = 0f;
            _allowMovement = false;
            _waitAfterLine = 1f;
            _waitLetter = 1f;
            open = true;
        }

        public static void Initialize()
        {
            if (_dealer != null)
                return;
            _dealer = new SpriteMap("arcade/plug", 133, 160);
            _font = new FancyBitmapFont("smallFont");
        }

        public static void Update()
        {
            Initialize();
            alpha = !FurniShopScreen.open ? Lerp.Float(alpha, 0f, 0.05f) : Lerp.Float(alpha, 1f, 0.05f);
            bool flag1 = true;
            _chancyLerp = Lerp.FloatSmooth(_chancyLerp, flag1 ? 1f : 0f, 0.2f, 1.05f);
            bool flag2 = !_allowMovement && Input.Down(Triggers.Select);
            if (_lines.Count > 0 && _currentLine == "")
            {
                int num = _waitAfterLine <= 0f ? 1 : 0;
                _waitAfterLine -= 0.045f;
                if (flag2)
                    _waitAfterLine -= 0.045f;
                if (killSkip)
                    _waitAfterLine -= 0.1f;
                _talkMove += 0.75f;
                if (_talkMove > 1f)
                {
                    frame = 0;
                    _talkMove = 0f;
                }
                if (num == 0 && _waitAfterLine <= 0f)
                    HUD.AddCornerMessage(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                if (_lineProgress.Count == 0 || Input.Pressed(Triggers.Select))
                {
                    _lineProgress.Clear();
                    _currentLine = _lines[0];
                    colorLetters = 0;
                    _lines.RemoveAt(0);
                    _waitAfterLine = 1.3f;
                    killSkip = false;
                }
            }
            if (_currentLine != "")
            {
                _waitLetter -= 0.8f;
                if (flag2)
                    _waitLetter -= 0.8f;
                if (_waitLetter >= 0f)
                    return;
                _talkMove += 0.75f;
                if (_talkMove > 1f)
                {
                    frame = _currentLine[0] == ' ' || frame != 1 || _extraWait > 0f ? 1 : 2;
                    _talkMove = 0f;
                }
                _waitLetter = 1f;
                while (_currentLine[0] == '@')
                {
                    string str = _currentLine[0].ToString() ?? "";
                    for (_currentLine = _currentLine.Remove(0, 1); _currentLine[0] != '@' && _currentLine.Length > 0; _currentLine = _currentLine.Remove(0, 1))
                        str += _currentLine[0].ToString();
                    _currentLine = _currentLine.Remove(0, 1);
                    string val = str + "@";
                    _lineProgress[0].Add(val);
                    _waitLetter = 3f;
                    if (_currentLine.Length == 0)
                    {
                        _currentLine = "";
                        return;
                    }
                }
                float num1 = 0f;
                while (_currentLine[0] == '|')
                {
                    _currentLine = _currentLine.Remove(0, 1);
                    string str = "";
                    int num2 = 0;
                    while (_currentLine[0] != '|' && _currentLine.Length > 0)
                    {
                        str += _currentLine[0].ToString();
                        _currentLine = _currentLine.Remove(0, 1);
                        ++num2;
                    }
                    bool flag3 = false;
                    if (_currentLine.Length <= 1)
                    {
                        _currentLine = "";
                        flag3 = true;
                    }
                    else
                        _currentLine = _currentLine.Remove(0, 1);
                    bool flag4 = true;
                    if (str == "0")
                    {
                        killSkip = true;
                        flag4 = false;
                    }
                    else if (str == "1")
                    {
                        num1 = 5f;
                        flag4 = false;
                    }
                    else if (str == "2")
                    {
                        num1 = 10f;
                        flag4 = false;
                    }
                    else if (str == "3")
                    {
                        num1 = 15f;
                        flag4 = false;
                    }
                    if (flag4)
                    {
                        colorLetters += num2 + 2;
                        _lineProgress[0].Add("|" + str + "|");
                    }
                    else if (flag3)
                        return;
                }
                string str1 = "";
                int index1 = 1;
                if (_currentLine[0] == ' ')
                {
                    while (index1 < _currentLine.Length && _currentLine[index1] != ' ' && _currentLine[index1] != '^')
                    {
                        if (_currentLine[index1] == '|')
                        {
                            ++colorLetters;
                            int index2 = index1 + 1;
                            while (index2 < _currentLine.Length && _currentLine[index2] != '|')
                            {
                                ++index2;
                                ++colorLetters;
                            }
                            index1 = index2 + 1;
                            ++colorLetters;
                        }
                        else if (_currentLine[index1] == '@')
                        {
                            int index3 = index1 + 1;
                            while (index3 < _currentLine.Length && _currentLine[index3] != '@')
                                ++index3;
                            index1 = index3 + 1;
                        }
                        else
                        {
                            str1 += _currentLine[index1].ToString();
                            ++index1;
                        }
                    }
                }
                if (_lineProgress.Count == 0 || _currentLine[0] == '^' || _currentLine[0] == ' ' && _lineProgress[0].Length() + (str1.Length - colorLetters) > 44)
                {
                    Color color = Color.White;
                    if (_lineProgress.Count > 0)
                        color = _lineProgress[0].lineColor;
                    _lineProgress.Insert(0, new TextLine()
                    {
                        lineColor = color
                    });
                    colorLetters = 0;
                    if (_currentLine[0] == ' ' || _currentLine[0] == '^')
                        _currentLine = _currentLine.Remove(0, 1);
                }
                else
                {
                    if (_currentLine[0] == '!' || _currentLine[0] == '?' || _currentLine[0] == '.')
                        _waitLetter = 8f;
                    else if (_currentLine[0] == ',')
                        _waitLetter = 14f;
                    _lineProgress[0].Add(_currentLine[0]);
                    char ch = _currentLine[0].ToString().ToLowerInvariant()[0];
                    if (wait > 0)
                        --wait;
                    if ((ch < 'a' || ch > 'z') && (ch < '0' || ch > '9') && ch != '\'' && lastWord != "")
                    {
                        int num3 = (int)CRC32.Generate(lastWord.Trim());
                        lastWord = "";
                    }
                    else
                        lastWord += ch.ToString();
                    if (wait > 0)
                    {
                        --wait;
                    }
                    else
                    {
                        wait = 2;
                        SFX.Play("tinyTick", 0.4f, 0.2f);
                    }
                    _currentLine = _currentLine.Remove(0, 1);
                }
                _waitLetter += num1;
            }
            else
            {
                _talkMove += 0.75f;
                if (_talkMove <= 1f)
                    return;
                frame = 0;
                _talkMove = 0f;
            }
        }

        public static void Draw()
        {
            Initialize();
            Vec2 vec2_1 = new Vec2((float)(100f * (1f - _chancyLerp)), (float)(100f * (1f - _chancyLerp) - 4f));
            Vec2 vec2_2 = new Vec2(280f, 30f);
            Vec2 vec2_3 = new Vec2(20f, 132f) + vec2_1;
            int num = 0;
            for (int index1 = _lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float width = _font.GetWidth(_lineProgress[index1].text);
                float y = vec2_3.y + 2f + num * 9;
                float x = (float)(vec2_3.x + vec2_2.x / 2f - width / 2f);
                for (int index2 = _lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    _font.Draw(_lineProgress[index1].segments[index2].text, new Vec2(x, y), _lineProgress[index1].segments[index2].color, (Depth)0.98f);
                    x += _lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num;
            }
            _dealer.depth = (Depth)0.96f;
            _dealer.alpha = 1f;
            Graphics.Draw(_dealer, 214f + vec2_1.x, 6f + vec2_1.y);
            Graphics.DrawRect(vec2_3 + new Vec2(-2f, 0f), vec2_3 + vec2_2 + new Vec2(2f, 0f), Color.Black, (Depth)0.97f);
        }
    }
}
