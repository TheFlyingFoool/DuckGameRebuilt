// Decompiled with JetBrains decompiler
// Type: DuckGame.Teleprompter
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Teleprompter : Thing
    {
        private BitmapFont _font;
        private List<DuckStory> _lines = new List<DuckStory>();
        private string _currentLine = "";
        private List<TextLine> _lineProgress = new List<TextLine>();
        private float _waitLetter = 1f;
        private float _waitAfterLine = 1f;
        private SpriteMap _newsCaster;
        private CasterMood _mood;
        private float _talkMove;
        private bool _paused;
        private SinWave _pitchSin = (SinWave)0.15f;
        private bool _demoWait;
        private bool _tried;
        private Sound s;

        public bool finished => _lines.Count == 0 && _currentLine == "";

        public Teleprompter(float xpos, float ypos, SpriteMap newsCaster)
          : base(xpos, ypos)
        {
            _newsCaster = newsCaster;
        }

        public void Pause() => _paused = true;

        public void Resume() => _paused = false;

        public void ReadLine(DuckStory line) => _lines.Add(line);

        public void InsertLine(string line, int index) => _lines.Insert(index, new DuckStory()
        {
            text = line
        });

        public void ReadStory(DuckStory line) => _lines.Add(line);

        public void SkipToClose()
        {
            _lines.RemoveAll(x => x.section < NewsSection.BeforeClosing);
            _lines.Insert(0, new DuckStory()
            {
                section = NewsSection.DemoClosing,
                text = "|EXCITED|OK! We'll cut it short, But I'd like to talk |RED|Demo|WHITE|!*"
            });
        }

        public void ClearLines()
        {
            _currentLine = "";
            _lines.Clear();
        }

        public override void Initialize()
        {
            _font = new BitmapFont("biosFont", 8);
            layer = Layer.HUD;
            base.Initialize();
        }

        public void SetCasterFrame(int frame)
        {
            if (_mood == CasterMood.Excited)
                _newsCaster.frame = frame + 3;
            else if (_mood == CasterMood.Suave)
                _newsCaster.frame = frame + 6;
            else
                _newsCaster.frame = frame;
        }

        public int GetCasterFrame()
        {
            if (_mood == CasterMood.Excited)
                return _newsCaster.frame - 3;
            return _mood == CasterMood.Suave ? _newsCaster.frame - 6 : _newsCaster.frame;
        }

        public override void Update()
        {
            if (_demoWait)
            {
                bool flag1 = true;
                bool flag2 = false;
                if (!_tried)
                {
                    if (Input.Pressed(Triggers.Menu2))
                    {
                        HUD.CloseAllCorners();
                        SFX.Play("rockHitGround", 0.9f);
                        flag1 = false;
                        _tried = true;
                    }
                    if (Input.Pressed(Triggers.Select))
                    {
                        HUD.CloseAllCorners();
                        flag2 = true;
                        _tried = true;
                    }
                }
                if (!flag1)
                {
                    //Main.isDemo = false;
                    _lines.Clear();
                    _lines.Add(new DuckStory()
                    {
                        section = NewsSection.Closing,
                        text = "|EXCITED|THANK YOU! You now have |GREEN|FULL Duck Game|WHITE|! You're the best!!!*"
                    });
                    _currentLine = "";
                    _demoWait = false;
                    HighlightLevel._cancelSkip = true;
                }
                else
                {
                    if (!flag2)
                        return;
                    _lines.Insert(0, new DuckStory()
                    {
                        section = NewsSection.Closing,
                        text = "Ah, alright then!"
                    });
                    _demoWait = false;
                }
            }
            else
            {
                if (_paused)
                    return;
                if (_lines.Count > 0 && _currentLine == "")
                {
                    _waitAfterLine -= 0.03f;
                    _talkMove += 0.75f;
                    if (_talkMove > 1.0)
                    {
                        SetCasterFrame(0);
                        _talkMove = 0f;
                    }
                    if (_waitAfterLine <= 0.0)
                    {
                        _lineProgress.Clear();
                        if (!_lines[0].text.StartsWith("CUE%"))
                        {
                            _currentLine = _lines[0].text;
                            _waitAfterLine = 1.2f;
                        }
                        _lines[0].DoCallback();
                        _lines.RemoveAt(0);
                        _mood = CasterMood.Normal;
                    }
                }
                if (_currentLine != "")
                {
                    _waitLetter -= 0.5f;
                    if (_waitLetter >= 0.0)
                        return;
                    _talkMove += 0.75f;
                    if (_talkMove > 1.0)
                    {
                        if (_currentLine[0] != ' ' && GetCasterFrame() == 0)
                            SetCasterFrame(Rando.Int(1) + 1);
                        else
                            SetCasterFrame(0);
                        _talkMove = 0f;
                    }
                    _waitLetter = 1f;
                    char ch1;
                    while (_currentLine[0] == '|')
                    {
                        _currentLine = _currentLine.Remove(0, 1);
                        string str1 = "";
                        for (; _currentLine[0] != '|' && _currentLine.Length > 0; _currentLine = _currentLine.Remove(0, 1))
                        {
                            string str2 = str1;
                            ch1 = _currentLine[0];
                            string str3 = ch1.ToString();
                            str1 = str2 + str3;
                        }
                        if (_currentLine.Length <= 1)
                        {
                            _currentLine = "";
                            return;
                        }
                        _currentLine = _currentLine.Remove(0, 1);
                        Color c = Color.White;
                        bool flag = false;
                        if (str1 == "RED")
                        {
                            flag = true;
                            c = Color.Red;
                        }
                        else if (str1 == "WHITE")
                        {
                            flag = true;
                            c = Color.White;
                        }
                        else if (str1 == "BLUE")
                        {
                            flag = true;
                            c = Color.Blue;
                        }
                        else if (str1 == "GREEN")
                        {
                            flag = true;
                            c = Color.LimeGreen;
                        }
                        else if (str1 == "EXCITED")
                            _mood = CasterMood.Excited;
                        else if (str1 == "SUAVE")
                            _mood = CasterMood.Suave;
                        else if (str1 == "CALM")
                            _mood = CasterMood.Normal;
                        else if (str1 == "DEMOWAIT")
                        {
                            HUD.CloseAllCorners();
                            HUD.AddCornerControl(HUDCorner.BottomLeft, "PAY THE MAN@MENU2@");
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@NO!");
                            _demoWait = true;
                            return;
                        }
                        if (flag)
                        {
                            if (_lineProgress.Count == 0)
                                _lineProgress.Insert(0, new TextLine()
                                {
                                    lineColor = c
                                });
                            else
                                _lineProgress[0].SwitchColor(c);
                        }
                    }
                    string str4 = "";
                    int index1 = 1;
                    if (_currentLine[0] == ' ')
                    {
                        while (index1 < _currentLine.Length && _currentLine[index1] != ' ')
                        {
                            if (_currentLine[index1] == '|')
                            {
                                int index2 = index1 + 1;
                                while (index2 < _currentLine.Length && _currentLine[index2] != '|')
                                    ++index2;
                                index1 = index2 + 1;
                            }
                            else
                            {
                                string str5 = str4;
                                ch1 = _currentLine[index1];
                                string str6 = ch1.ToString();
                                str4 = str5 + str6;
                                ++index1;
                            }
                        }
                    }
                    if (_lineProgress.Count == 0 || _currentLine[0] == ' ' && _lineProgress[0].Length() + str4.Length > 21)
                    {
                        Color color = Color.White;
                        if (_lineProgress.Count > 0)
                            color = _lineProgress[0].lineColor;
                        _lineProgress.Insert(0, new TextLine()
                        {
                            lineColor = color
                        });
                        if (_currentLine[0] != ' ')
                            return;
                        _currentLine = _currentLine.Remove(0, 1);
                    }
                    else
                    {
                        if (_currentLine[0] == '!' || _currentLine[0] == '?' || _currentLine[0] == '.')
                            _waitLetter = 5f;
                        else if (_currentLine[0] == ',')
                            _waitLetter = 3f;
                        if (_currentLine[0] == '*')
                        {
                            _waitLetter = 5f;
                        }
                        else
                        {
                            _lineProgress[0].Add(_currentLine[0]);
                            ch1 = _currentLine[0];
                            char ch2 = ch1.ToString().ToLowerInvariant()[0];
                            if (ch2 >= 'a' && ch2 <= 'z' || ch2 >= '0' && ch2 <= '9')
                                s = SFX.Play("tinyNoise" + Convert.ToString(Rando.Int(1, 5)), pitch: Rando.Float(-0.8f, -0.4f));
                        }
                        _currentLine = _currentLine.Remove(0, 1);
                    }
                }
                else
                {
                    _talkMove += 0.75f;
                    if (_talkMove <= 1.0)
                        return;
                    SetCasterFrame(0);
                    _talkMove = 0f;
                }
            }
        }

        public override void Draw()
        {
            int num = 0;
            for (int index1 = _lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float width = _font.GetWidth(_lineProgress[index1].text);
                float ypos = 140 - (_lineProgress.Count - 1) * 9 + num * 9;
                Graphics.DrawRect(new Vec2((float)(132.0 - width / 2.0 - 1.0), ypos - 1f), new Vec2((float)(132.0 + width / 2.0), ypos + 9f), Color.Black, (Depth)0.84f);
                float xpos = (float)(132.0 - width / 2.0);
                for (int index2 = _lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    _font.Draw(_lineProgress[index1].segments[index2].text, xpos, ypos, _lineProgress[index1].segments[index2].color, (Depth)0.85f);
                    xpos += _lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num;
            }
        }
    }
}
