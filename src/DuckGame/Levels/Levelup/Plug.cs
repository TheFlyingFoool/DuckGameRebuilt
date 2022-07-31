// Decompiled with JetBrains decompiler
// Type: DuckGame.Plug
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            Plug._lines.Clear();
            Plug._waitLetter = 0f;
            Plug._waitAfterLine = 0f;
            Plug._currentLine = "";
        }

        public static void Add(string line) => Plug._lines.Add(line);

        public static void Open()
        {
            Plug._lineProgress.Clear();
            //Plug._showLerp = 0f;
            Plug._allowMovement = false;
            Plug._waitAfterLine = 1f;
            Plug._waitLetter = 1f;
            Plug.open = true;
        }

        public static void Initialize()
        {
            if (Plug._dealer != null)
                return;
            Plug._dealer = new SpriteMap("arcade/plug", 133, 160);
            Plug._font = new FancyBitmapFont("smallFont");
        }

        public static void Update()
        {
            Plug.Initialize();
            Plug.alpha = !FurniShopScreen.open ? Lerp.Float(Plug.alpha, 0f, 0.05f) : Lerp.Float(Plug.alpha, 1f, 0.05f);
            bool flag1 = true;
            Plug._chancyLerp = Lerp.FloatSmooth(Plug._chancyLerp, flag1 ? 1f : 0f, 0.2f, 1.05f);
            bool flag2 = !Plug._allowMovement && Input.Down("SELECT");
            if (Plug._lines.Count > 0 && Plug._currentLine == "")
            {
                int num = _waitAfterLine <= 0.0 ? 1 : 0;
                Plug._waitAfterLine -= 0.045f;
                if (flag2)
                    Plug._waitAfterLine -= 0.045f;
                if (Plug.killSkip)
                    Plug._waitAfterLine -= 0.1f;
                Plug._talkMove += 0.75f;
                if (_talkMove > 1.0)
                {
                    Plug.frame = 0;
                    Plug._talkMove = 0f;
                }
                if (num == 0 && _waitAfterLine <= 0.0)
                    HUD.AddCornerMessage(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                if (Plug._lineProgress.Count == 0 || Input.Pressed("SELECT"))
                {
                    Plug._lineProgress.Clear();
                    Plug._currentLine = Plug._lines[0];
                    Plug.colorLetters = 0;
                    Plug._lines.RemoveAt(0);
                    Plug._waitAfterLine = 1.3f;
                    Plug.killSkip = false;
                }
            }
            if (Plug._currentLine != "")
            {
                Plug._waitLetter -= 0.8f;
                if (flag2)
                    Plug._waitLetter -= 0.8f;
                if (_waitLetter >= 0.0)
                    return;
                Plug._talkMove += 0.75f;
                if (_talkMove > 1.0)
                {
                    Plug.frame = Plug._currentLine[0] == ' ' || Plug.frame != 1 || _extraWait > 0.0 ? 1 : 2;
                    Plug._talkMove = 0f;
                }
                Plug._waitLetter = 1f;
                while (Plug._currentLine[0] == '@')
                {
                    string str = Plug._currentLine[0].ToString() ?? "";
                    for (Plug._currentLine = Plug._currentLine.Remove(0, 1); Plug._currentLine[0] != '@' && Plug._currentLine.Length > 0; Plug._currentLine = Plug._currentLine.Remove(0, 1))
                        str += Plug._currentLine[0].ToString();
                    Plug._currentLine = Plug._currentLine.Remove(0, 1);
                    string val = str + "@";
                    Plug._lineProgress[0].Add(val);
                    Plug._waitLetter = 3f;
                    if (Plug._currentLine.Length == 0)
                    {
                        Plug._currentLine = "";
                        return;
                    }
                }
                float num1 = 0f;
                while (Plug._currentLine[0] == '|')
                {
                    Plug._currentLine = Plug._currentLine.Remove(0, 1);
                    string str = "";
                    int num2 = 0;
                    while (Plug._currentLine[0] != '|' && Plug._currentLine.Length > 0)
                    {
                        str += Plug._currentLine[0].ToString();
                        Plug._currentLine = Plug._currentLine.Remove(0, 1);
                        ++num2;
                    }
                    bool flag3 = false;
                    if (Plug._currentLine.Length <= 1)
                    {
                        Plug._currentLine = "";
                        flag3 = true;
                    }
                    else
                        Plug._currentLine = Plug._currentLine.Remove(0, 1);
                    bool flag4 = true;
                    if (str == "0")
                    {
                        Plug.killSkip = true;
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
                        Plug.colorLetters += num2 + 2;
                        Plug._lineProgress[0].Add("|" + str + "|");
                    }
                    else if (flag3)
                        return;
                }
                string str1 = "";
                int index1 = 1;
                if (Plug._currentLine[0] == ' ')
                {
                    while (index1 < Plug._currentLine.Length && Plug._currentLine[index1] != ' ' && Plug._currentLine[index1] != '^')
                    {
                        if (Plug._currentLine[index1] == '|')
                        {
                            ++Plug.colorLetters;
                            int index2 = index1 + 1;
                            while (index2 < Plug._currentLine.Length && Plug._currentLine[index2] != '|')
                            {
                                ++index2;
                                ++Plug.colorLetters;
                            }
                            index1 = index2 + 1;
                            ++Plug.colorLetters;
                        }
                        else if (Plug._currentLine[index1] == '@')
                        {
                            int index3 = index1 + 1;
                            while (index3 < Plug._currentLine.Length && Plug._currentLine[index3] != '@')
                                ++index3;
                            index1 = index3 + 1;
                        }
                        else
                        {
                            str1 += Plug._currentLine[index1].ToString();
                            ++index1;
                        }
                    }
                }
                if (Plug._lineProgress.Count == 0 || Plug._currentLine[0] == '^' || Plug._currentLine[0] == ' ' && Plug._lineProgress[0].Length() + (str1.Length - Plug.colorLetters) > 44)
                {
                    Color color = Color.White;
                    if (Plug._lineProgress.Count > 0)
                        color = Plug._lineProgress[0].lineColor;
                    Plug._lineProgress.Insert(0, new TextLine()
                    {
                        lineColor = color
                    });
                    Plug.colorLetters = 0;
                    if (Plug._currentLine[0] == ' ' || Plug._currentLine[0] == '^')
                        Plug._currentLine = Plug._currentLine.Remove(0, 1);
                }
                else
                {
                    if (Plug._currentLine[0] == '!' || Plug._currentLine[0] == '?' || Plug._currentLine[0] == '.')
                        Plug._waitLetter = 8f;
                    else if (Plug._currentLine[0] == ',')
                        Plug._waitLetter = 14f;
                    Plug._lineProgress[0].Add(Plug._currentLine[0]);
                    char ch = Plug._currentLine[0].ToString().ToLowerInvariant()[0];
                    if (Plug.wait > 0)
                        --Plug.wait;
                    if ((ch < 'a' || ch > 'z') && (ch < '0' || ch > '9') && ch != '\'' && Plug.lastWord != "")
                    {
                        int num3 = (int)CRC32.Generate(Plug.lastWord.Trim());
                        Plug.lastWord = "";
                    }
                    else
                        Plug.lastWord += ch.ToString();
                    if (Plug.wait > 0)
                    {
                        --Plug.wait;
                    }
                    else
                    {
                        Plug.wait = 2;
                        SFX.Play("tinyTick", 0.4f, 0.2f);
                    }
                    Plug._currentLine = Plug._currentLine.Remove(0, 1);
                }
                Plug._waitLetter += num1;
            }
            else
            {
                Plug._talkMove += 0.75f;
                if (_talkMove <= 1.0)
                    return;
                Plug.frame = 0;
                Plug._talkMove = 0f;
            }
        }

        public static void Draw()
        {
            Plug.Initialize();
            Vec2 vec2_1 = new Vec2((float)(100.0 * (1.0 - _chancyLerp)), (float)(100.0 * (1.0 - _chancyLerp) - 4.0));
            Vec2 vec2_2 = new Vec2(280f, 30f);
            Vec2 vec2_3 = new Vec2(20f, 132f) + vec2_1;
            int num = 0;
            for (int index1 = Plug._lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float width = Plug._font.GetWidth(Plug._lineProgress[index1].text);
                float y = vec2_3.y + 2f + num * 9;
                float x = (float)(vec2_3.x + vec2_2.x / 2.0 - (double)width / 2.0);
                for (int index2 = Plug._lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    Plug._font.Draw(Plug._lineProgress[index1].segments[index2].text, new Vec2(x, y), Plug._lineProgress[index1].segments[index2].color, (Depth)0.98f);
                    x += Plug._lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num;
            }
            Plug._dealer.depth = (Depth)0.96f;
            Plug._dealer.alpha = 1f;
            Graphics.Draw(_dealer, 214f + vec2_1.x, 6f + vec2_1.y);
            Graphics.DrawRect(vec2_3 + new Vec2(-2f, 0f), vec2_3 + vec2_2 + new Vec2(2f, 0f), Color.Black, (Depth)0.97f);
        }
    }
}
