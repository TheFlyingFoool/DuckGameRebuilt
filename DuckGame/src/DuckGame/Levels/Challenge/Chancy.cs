// Decompiled with JetBrains decompiler
// Type: DuckGame.Chancy
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class Chancy
    {
        public static Chancy context = new Chancy();
        public static float alpha = 0f;
        public static bool atCounter = true;
        public static Vec2 standingPosition = Vec2.Zero;
        public static bool lookingAtList = false;
        public static bool lookingAtChallenge = false;
        public static bool hover = false;
        private static FancyBitmapFont _font;
        private static SpriteMap _dealer;
        private static Sprite _tail;
        private static Sprite _photo;
        private static Sprite _tape;
        private static Sprite _tapePaper;
        private static SpriteMap _paperclip;
        private static SpriteMap _sticker;
        private static Sprite _completeStamp;
        private static Sprite _pencil;
        private static SpriteMap _tinyStars;
        public static Sprite body;
        public static Sprite hoverSprite;
        public static Sprite listPaper;
        public static Sprite challengePaper;
        private static List<string> _lines = new List<string>();
        private static DealerMood _mood;
        private static string _currentLine = "";
        private static List<TextLine> _lineProgress = new List<TextLine>();
        private static float _waitLetter = 1f;
        private static float _waitAfterLine = 1f;
        private static float _talkMove = 0f;
        private static float _listLerp = 0f;
        private static float _challengeLerp = 0f;
        private static float _chancyLerp = 0f;
        private static ChallengeSaveData _save;
        private static ChallengeSaveData _realSave;
        private static SpriteMap _previewPhoto;
        private static ChallengeData _challengeData;
        private static RenderTarget2D _bestTextTarget;
        private static Random _random;
        private static float _stampAngle = 0f;
        private static float _paperAngle = 0f;
        private static float _tapeAngle = 0f;
        private static int _challengeSelection;
        public static int _giveTickets = 0;
        public static bool afterChallenge = false;
        public static float afterChallengeWait = 0f;
        private static List<ChallengeData> _chancyChallenges = new List<ChallengeData>();

        public static int frame
        {
            get
            {
                if (_mood == DealerMood.Concerned)
                    return _dealer.frame - 4;
                return _mood == DealerMood.Point ? _dealer.frame - 2 : _dealer.frame;
            }
            set
            {
                if (_mood == DealerMood.Concerned)
                    _dealer.frame = value + 4;
                else if (_mood == DealerMood.Point)
                    _dealer.frame = value + 2;
                else
                    _dealer.frame = value;
            }
        }

        public static void Clear()
        {
            _lines.Clear();
            _waitLetter = 0f;
            _waitAfterLine = 0f;
            _currentLine = "";
            _mood = DealerMood.Normal;
        }

        public static void Add(string line) => _lines.Add(line);

        public static ChallengeData activeChallenge
        {
            get => _challengeData;
            set => _challengeData = value;
        }

        public static void UpdateRandoms()
        {
            if (_challengeData == null || _paperclip == null)
                return;
            _random = new Random(_challengeData.name.GetHashCode());
            Random generator = Rando.generator;
            Rando.generator = _random;
            _paperclip.frame = Rando.Int(5);
            _stampAngle = Rando.Float(14f) - 7f;
            Rando.generator = new Random(GetChallengeBestString(_save, _challengeData).GetHashCode());
            _paperAngle = Rando.Float(4f) - 2f;
            _tapeAngle = _paperAngle + Rando.Float(-1f, 1f);
            Rando.generator = generator;
        }

        public static ChallengeData selectedChallenge => _chancyChallenges.Count == 0 ? null : _chancyChallenges[_challengeSelection];

        public static void AddProposition(ChallengeData challenge, Vec2 duckPos)
        {
            if (challenge.preview != null)
            {
                Texture2D tex = Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(challenge.preview)));
                _previewPhoto = new SpriteMap((Tex2D)tex, tex.Width, tex.Height)
                {
                    scale = new Vec2(0.25f)
                };
            }
            _challengeData = challenge;
            _realSave = Profiles.active[0].GetSaveData(_challengeData.levelID);
            _save = _realSave.Clone();
            UpdateRandoms();
            atCounter = false;
            Vec2 p1_1 = duckPos;
            bool flag1 = false;
            Vec2 position;
            if (Level.CheckLine<Block>(duckPos, duckPos + new Vec2(36f, 0f), out position) != null)
            {
                position.x -= 8f;
                if ((position - duckPos).length > 16f)
                {
                    p1_1 = position;
                    flag1 = true;
                }
            }
            else
            {
                p1_1 = duckPos + new Vec2(36f, 0f);
                flag1 = true;
            }
            if (flag1)
            {
                if (Level.CheckLine<Block>(p1_1, p1_1 + new Vec2(0f, 20f), out position) == null)
                {
                    flag1 = false;
                }
                else
                {
                    standingPosition = position - new Vec2(0f, 25f);
                    body.flipH = true;
                }
            }
            if (flag1)
                return;
            Vec2 p1_2;
            //bool flag2;
            if (Level.CheckLine<Block>(duckPos, duckPos + new Vec2(-36f, 0f), out position) != null)
            {
                position.x += 8f;
                p1_2 = position;
                //flag2 = true;
            }
            else
            {
                p1_2 = duckPos + new Vec2(-36f, 0f);
                //flag2 = true;
            }
            Level.CheckLine<Block>(p1_2, p1_2 + new Vec2(0f, 20f), out position);
            standingPosition = position - new Vec2(0f, 25f);
            body.flipH = false;
        }

        public static void Initialize()
        {
            if (_dealer != null)
                return;
            _dealer = new SpriteMap("arcade/schooly", 100, 100);
            _tail = new Sprite("arcade/bubbleTail");
            body = new Sprite("arcade/chancy");
            hoverSprite = new Sprite("arcade/chancyHover");
            challengePaper = new Sprite("arcade/challengePaper");
            listPaper = new Sprite("arcade/challengePaperTall");
            _font = new FancyBitmapFont("smallFont");
            _photo = new Sprite("arcade/challengePhoto");
            _paperclip = new SpriteMap("arcade/paperclips", 13, 45);
            _sticker = new SpriteMap("arcade/stickers", 29, 29)
            {
                frame = 2
            };
            _tinyStars = new SpriteMap("arcade/tinyStars", 10, 8);
            _tinyStars.CenterOrigin();
            _completeStamp = new Sprite("arcade/completeStamp");
            _completeStamp.CenterOrigin();
            _pencil = new Sprite("arcade/pencil")
            {
                center = new Vec2(sbyte.MaxValue, 4f)
            };
            _tape = new Sprite("arcade/tape");
            _tape.CenterOrigin();
            _tapePaper = new Sprite("arcade/tapePaper");
            _tapePaper.CenterOrigin();
        }

        public static void OpenChallengeView() => ResetChallengeDialogue();

        public static void ResetChallengeDialogue()
        {
            Clear();
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            List<string> stringList = new List<string>()
              {
                "You interested in a little challenge?",
                "Bet you can't finish this one!",
                "You look up for a challenge."
              };
            if (_save == null)
            {
                if (challengeSkillIndex > 0.75f)
                    stringList = new List<string>()
                  {
                    "You could do this one easy.",
                    "This should be no problem for you!",
                    "This one's gonna be a breeze.",
                    "Hot off the grill, just for you."
                  };
                else if (challengeSkillIndex > 0.3f)
                    stringList = new List<string>()
                  {
                    "Wanna try something different?",
                    "Hey, check this out.",
                    "I've been playin with this new thing."
                  };
            }
            else if (_save != null && _save.trophy > TrophyType.Gold)
            {
                if (challengeSkillIndex > 0.75f)
                    stringList = new List<string>()
          {
            "Just never good enough huh?",
            "You still gotta top that score?"
          };
                else if (challengeSkillIndex > 0.3f)
                    stringList = new List<string>()
          {
            "|CONCERNED|Woah, you think you can beat that score?",
            "|CONCERNED|You're gonna try to beat THAT!?"
          };
                else
                    stringList = new List<string>()
          {
            "You already dominated this one.",
            "|CONCERNED|Huh? |CALM|You already got PLATINUM!"
          };
            }
            else if (_save != null && _save.trophy > TrophyType.Silver)
            {
                if (challengeSkillIndex > 0.75f)
                    stringList = new List<string>()
          {
            "You know you can do better than gold.",
            "Pretty good, but you can do better."
          };
                else if (challengeSkillIndex > 0.3f)
                    stringList = new List<string>()
          {
            "Not bad.",
            "Yeah that's getting there, gold is alright."
          };
                else
                    stringList = new List<string>()
          {
            "Gold is pretty rad, you still wanna do better?",
            "Still wanna improve that score?"
          };
            }
            else if (_save != null && _save.trophy > TrophyType.Baseline)
            {
                if (challengeSkillIndex > 0.75f)
                    stringList = new List<string>()
          {
            "Nice, lets try to top that.",
            "What? Not bad but you really could do better.",
            "I know you're not just gonna leave it at that."
          };
                else if (challengeSkillIndex > 0.3f)
                    stringList = new List<string>()
          {
            "You did it, but you can do better.",
            "You're pretty good, you beat it."
          };
                else
                    stringList = new List<string>()
          {
            "Well, you beat it! Can you do better?",
            "Not bad, you managed to do it!"
          };
            }
            Add(stringList[Rando.Int(stringList.Count - 1)]);
        }

        public static void OpenChallengeList()
        {
            _challengeSelection = 0;
            _chancyChallenges = Challenges.GetEligibleChancyChallenges(Profiles.active[0]);
        }

        public static void MakeConfetti()
        {
            for (int index = 0; index < 40; ++index)
                Level.Add(new ChallengeConfetti(index * 8 + Rando.Float(-10f, 10f), Rando.Float(110f) - 124f));
        }

        public static bool HasNewTrophy() => _realSave.trophy != _save.trophy;

        public static bool HasNewTime() => _realSave.bestTime != _save.bestTime || _realSave.goodies != _save.goodies || _realSave.targets != _save.targets;

        public static int GiveTrophy()
        {
            int num = 0;
            if (_save.trophy != _realSave.trophy)
            {
                for (int index = (int)(_save.trophy + 1); (TrophyType)index <= _realSave.trophy; ++index)
                {
                    switch (index)
                    {
                        case 1:
                            num += Challenges.valueBronze;
                            break;
                        case 2:
                            num += Challenges.valueSilver;
                            break;
                        case 3:
                            num += Challenges.valueGold;
                            break;
                        case 4:
                            num += Challenges.valuePlatinum;
                            break;
                    }
                }
                _save.trophy = _realSave.trophy;
            }
            return num;
        }

        public static void GiveTime()
        {
            if (_save.bestTime != _realSave.bestTime)
                _save.bestTime = _realSave.bestTime;
            if (_save.goodies != _realSave.goodies)
                _save.goodies = _realSave.goodies;
            if (_save.targets != _realSave.targets)
                _save.targets = _realSave.targets;
            UpdateRandoms();
        }

        public static void StopShowingChallengeList()
        {
            _listLerp = 0f;
            _challengeLerp = 0f;
            _challengeLerp = 0f;
            lookingAtChallenge = false;
            lookingAtList = false;
        }

        public static void Update()
        {
            bool flag1 = lookingAtList && _challengeLerp < 0.3f;
            bool flag2 = lookingAtChallenge && _listLerp < 0.3f;
            bool flag3 = (lookingAtChallenge || UnlockScreen.open) && _listLerp < 0.3f;
            _listLerp = Lerp.FloatSmooth(_listLerp, flag1 ? 1f : 0f, 0.2f, 1.05f);
            _challengeLerp = Lerp.FloatSmooth(_challengeLerp, flag2 ? 1f : 0f, 0.2f, 1.05f);
            _chancyLerp = Lerp.FloatSmooth(_chancyLerp, flag3 ? 1f : 0f, 0.2f, 1.05f);
            if (lookingAtList)
            {
                if (Input.Pressed(Triggers.MenuUp))
                {
                    --_challengeSelection;
                    if (_challengeSelection < 0)
                        _challengeSelection = 0;
                    else
                        SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed(Triggers.MenuDown))
                {
                    ++_challengeSelection;
                    if (_challengeSelection > _chancyChallenges.Count - 1)
                        _challengeSelection = _chancyChallenges.Count - 1;
                    else
                        SFX.Play("textLetter", 0.7f);
                }
            }
            if (!UnlockScreen.open && !lookingAtChallenge)
                return;
            alpha = UnlockScreen.open || lookingAtChallenge ? Lerp.Float(alpha, 1f, 0.05f) : Lerp.Float(alpha, 0f, 0.05f);
            if (afterChallenge)
            {
                if (afterChallengeWait > 0f) afterChallengeWait -= 0.03f;
                else if (HasNewTime() || HasNewTrophy())
                {
                    SFX.Play("dacBang", pitch: -0.7f);
                    GiveTime();
                    _giveTickets = GiveTrophy();
                    afterChallengeWait = 1f;
                    MakeConfetti();
                }
                else if (_giveTickets != 0)
                {
                    Profiles.active[0].ticketCount += _giveTickets;
                    afterChallengeWait = 2f;
                    _giveTickets = 0;
                    SFX.Play("ching");
                }
                else
                {
                    ResetChallengeDialogue();
                    afterChallengeWait = 0f;
                    afterChallenge = false;
                    foreach (ArcadeHUD arcadeHud in Level.current.things[typeof(ArcadeHUD)])
                    {
                        arcadeHud.FinishChallenge();
                        arcadeHud.launchChallenge = false;
                        arcadeHud.selected = null;
                    }
                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@ACCEPT");
                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@CANCEL");
                    Profiles.Save(Profiles.active[0]);
                }
            }
            if (_save != null && activeChallenge != null)
            {
                if (_bestTextTarget == null)
                    _bestTextTarget = new RenderTarget2D(120, 8);
                Graphics.SetRenderTarget(_bestTextTarget);
                Graphics.Clear(Color.Transparent);
                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
                string challengeBestString = GetChallengeBestString(_save, activeChallenge);
                _font.Draw(challengeBestString, new Vec2((int)Math.Round(_bestTextTarget.width / 2f - _font.GetWidth(challengeBestString) / 2f), 0f), Color.Black * 0.7f);
                Graphics.screen.End();
                Graphics.SetRenderTarget(null);
            }
            Initialize();
            if (_lines.Count > 0 && _currentLine == "")
            {
                _waitAfterLine -= 0.03f;
                _talkMove += 0.75f;
                if (_talkMove > 1f)
                {
                    frame = 0;
                    _talkMove = 0f;
                }
                if (_waitAfterLine <= 0f)
                {
                    _lineProgress.Clear();
                    _currentLine = _lines[0];
                    _lines.RemoveAt(0);
                    _waitAfterLine = 1.5f;
                    _mood = DealerMood.Normal;
                }
            }
            if (_currentLine != "")
            {
                _waitLetter -= 0.8f;
                if (_waitLetter >= 0f) return;
                _talkMove += 0.75f;
                if (_talkMove > 1f)
                {
                    frame = _currentLine[0] == ' ' || frame != 0 ? 0 : Rando.Int(1);
                    _talkMove = 0f;
                }
                _waitLetter = 1f;
                char ch1;
                while (_currentLine[0] == '@')
                {
                    ch1 = _currentLine[0];
                    string str1 = ch1.ToString() ?? "";
                    for (_currentLine = _currentLine.Remove(0, 1); _currentLine[0] != '@' && _currentLine.Length > 0; _currentLine = _currentLine.Remove(0, 1))
                    {
                        string str2 = str1;
                        ch1 = _currentLine[0];
                        string str3 = ch1.ToString();
                        str1 = str2 + str3;
                    }
                    _currentLine = _currentLine.Remove(0, 1);
                    string val = str1 + "@";
                    _lineProgress[0].Add(val);
                    _waitLetter = 3f;
                    if (_currentLine.Length == 0)
                    {
                        _currentLine = "";
                        return;
                    }
                }
                while (_currentLine[0] == '|')
                {
                    _currentLine = _currentLine.Remove(0, 1);
                    string str4 = "";
                    for (; _currentLine[0] != '|' && _currentLine.Length > 0; _currentLine = _currentLine.Remove(0, 1))
                    {
                        string str5 = str4;
                        ch1 = _currentLine[0];
                        string str6 = ch1.ToString();
                        str4 = str5 + str6;
                    }
                    if (_currentLine.Length <= 1)
                    {
                        _currentLine = "";
                        return;
                    }
                    _currentLine = _currentLine.Remove(0, 1);
                    Color c = Color.White;
                    bool flag4 = false;
                    if (str4 == "RED")
                    {
                        flag4 = true;
                        c = Color.Red;
                    }
                    else if (str4 == "WHITE")
                    {
                        flag4 = true;
                        c = Color.White;
                    }
                    else if (str4 == "BLUE")
                    {
                        flag4 = true;
                        c = Color.Blue;
                    }
                    else if (str4 == "ORANGE")
                    {
                        flag4 = true;
                        c = new Color(235, 137, 51);
                    }
                    else if (str4 == "YELLOW")
                    {
                        flag4 = true;
                        c = new Color(247, 224, 90);
                    }
                    else if (str4 == "GREEN")
                    {
                        flag4 = true;
                        c = Color.LimeGreen;
                    }
                    else if (str4 == "CONCERNED")
                        _mood = DealerMood.Concerned;
                    else if (str4 == "CALM")
                        _mood = DealerMood.Normal;
                    else if (str4 == "PEEK")
                        _mood = DealerMood.Point;
                    if (flag4)
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
                string str7 = "";
                int index1 = 1;
                if (_currentLine[0] == ' ')
                {
                    while (index1 < _currentLine.Length && _currentLine[index1] != ' ' && _currentLine[index1] != '^')
                    {
                        if (_currentLine[index1] == '|')
                        {
                            int index2 = index1 + 1;
                            while (index2 < _currentLine.Length && _currentLine[index2] != '|')
                                ++index2;
                            index1 = index2 + 1;
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
                            string str8 = str7;
                            ch1 = _currentLine[index1];
                            string str9 = ch1.ToString();
                            str7 = str8 + str9;
                            ++index1;
                        }
                    }
                }
                if (_lineProgress.Count == 0 || _currentLine[0] == '^' || _currentLine[0] == ' ' && _lineProgress[0].Length() + str7.Length > 34)
                {
                    Color color = Color.White;
                    if (_lineProgress.Count > 0)
                        color = _lineProgress[0].lineColor;
                    _lineProgress.Insert(0, new TextLine()
                    {
                        lineColor = color
                    });
                    if (_currentLine[0] != ' ' && _currentLine[0] != '^')
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
                        if ((ch2 < 'a' || ch2 > 'z') && ch2 >= '0')
                            ;
                    }
                    _currentLine = _currentLine.Remove(0, 1);
                }
            }
            else
            {
                _talkMove += 0.75f;
                if (_talkMove <= 1f) return;
                frame = 0;
                _talkMove = 0f;
            }
        }

        public static string GetChallengeBestString(
          ChallengeSaveData dat,
          ChallengeData chal,
          bool canNull = false)
        {
            if (chal.trophies[1].timeRequirement > 0 || chal.trophies[2].timeRequirement > 0 || chal.trophies[3].timeRequirement > 0)
            {
                string str = MonoMain.TimeString(TimeSpan.FromMilliseconds(dat.bestTime), small: true);
                if (dat.bestTime > 0)
                    return "BEST: " + str;
                return !canNull ? "|RED|N/A" : null;
            }
            if (chal.trophies[1].targets != -1)
            {
                if (dat.targets > 0)
                    return "BEST: " + dat.targets.ToString();
                return !canNull ? "|RED|N/A" : null;
            }
            if (chal.trophies[1].goodies == -1)
                return "";
            if (dat.goodies > 0)
                return "BEST: " + dat.goodies.ToString();
            return !canNull ? "|RED|N/A" : null;
        }

        public static void Draw()
        {
            Vec2 vec2_1 = new Vec2((float)(_listLerp * 270f - 200f), 20f);
            if (lookingAtList || _listLerp > 0.01f)
            {
                listPaper.depth = (Depth)0.8f;
                Graphics.Draw(listPaper, vec2_1.x, vec2_1.y);
                _font.depth = (Depth)0.85f;
                _font.scale = new Vec2(1f);
                _font.Draw("Chancy Challenges", vec2_1 + new Vec2(11f, 6f), Colors.BlueGray, (Depth)0.85f);
                float num = 9f;
                List<ChallengeData> chancyChallenges = _chancyChallenges;
                int index = 0;
                foreach (ChallengeData challengeData in chancyChallenges)
                {
                    _font.Draw(challengeData.name, vec2_1 + new Vec2(19f, 12f + num), Colors.DGRed, (Depth)0.85f);
                    Vec2 vec2_2 = vec2_1 + new Vec2(12f, (12f + num + 4f));
                    if (index == _challengeSelection)
                    {
                        _pencil.depth = (Depth)0.9f;
                        Graphics.Draw(_pencil, vec2_2.x, vec2_2.y);
                        Graphics.DrawLine(vec2_1 + new Vec2(19f, (12f + num + 8.5f)), vec2_1 + new Vec2(19f + _font.GetWidth(challengeData.name), (float)(12f + num + 8.5f)), Colors.SuperDarkBlueGray, depth: ((Depth)0.9f));
                    }
                    ChallengeSaveData saveData = Profiles.active[0].GetSaveData(_chancyChallenges[index].levelID);
                    if (saveData != null && saveData.trophy > TrophyType.Baseline)
                    {
                        _tinyStars.frame = (int)(saveData.trophy - 1);
                        _tinyStars.depth = (Depth)0.85f;
                        Graphics.Draw(_tinyStars, vec2_2.x + 2f, vec2_2.y);
                    }
                    num += 9f;
                    ++index;
                }
            }
            if (_challengeLerp < 0.01f && _chancyLerp < 0.01f)
                return;
            Vec2 vec2_3 = new Vec2((100f * (1f - _chancyLerp)), (100f * (1f - _chancyLerp)));
            Vec2 vec2_4 = new Vec2(280f, 20f);
            Vec2 vec2_5 = new Vec2(20f, 132f) + vec2_3;
            Graphics.DrawRect(vec2_5 + new Vec2(-2f, 0f), vec2_5 + vec2_4 + new Vec2(2f, 0f), Color.Black);
            int num1 = 0;
            for (int index1 = _lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float stringWidth = Graphics.GetStringWidth(_lineProgress[index1].text);
                float y = vec2_5.y + 2f + num1 * 9;
                float x = (vec2_5.x + vec2_4.x / 2f - stringWidth / 2f);
                for (int index2 = _lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    Graphics.DrawString(_lineProgress[index1].segments[index2].text, new Vec2(x, y), _lineProgress[index1].segments[index2].color, (Depth)0.85f);
                    x += _lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num1;
            }
            if (_challengeLerp > 0.01f && _challengeData != null)
            {
                vec2_1 = new Vec2(40f, 28f);
                vec2_1 = new Vec2((_challengeLerp * 240f - 200f), 28f);
                challengePaper.depth = (Depth)0.8f;
                Graphics.Draw(challengePaper, vec2_1.x, vec2_1.y);
                _paperclip.depth = (Depth)0.92f;
                _photo.depth = (Depth)0.87f;
                Graphics.Draw(_photo, vec2_1.x + 135f, vec2_1.y - 3f);
                Graphics.Draw(_paperclip, vec2_1.x + 140f, vec2_1.y - 10f);
                if (_previewPhoto != null)
                {
                    _previewPhoto.depth = (Depth)0.89f;
                    _previewPhoto.angleDegrees = 12f;
                    Graphics.Draw(_previewPhoto, vec2_1.x + 146f, vec2_1.y + 0f);
                }
                if (_save != null)
                {
                    if (_save.trophy > TrophyType.Baseline)
                    {
                        _sticker.depth = (Depth)0.9f;
                        _sticker.frame = (int)(_save.trophy - 1);
                        Graphics.Draw(_sticker, vec2_1.x + 123f, vec2_1.y + 2f);
                        _completeStamp.depth = (Depth)0.9f;
                        _completeStamp.angleDegrees = _stampAngle;
                        _completeStamp.alpha = 0.9f;
                        Graphics.Draw(_completeStamp, vec2_1.x + 72f, vec2_1.y + 82f);
                    }
                    string challengeBestString = GetChallengeBestString(_save, _challengeData, true);
                    if (challengeBestString != null && challengeBestString != "")
                    {
                        _tapePaper.depth = (Depth)0.9f;
                        _tapePaper.angleDegrees = _paperAngle;
                        Graphics.Draw(_tapePaper, vec2_1.x + 64f, vec2_1.y + 22f);
                        _tape.depth = (Depth)0.95f;
                        _tape.angleDegrees = _tapeAngle;
                        Graphics.Draw(_tape, vec2_1.x + 64f, vec2_1.y + 22f);
                        if (_bestTextTarget != null)
                            Graphics.Draw((Tex2D)(Texture2D)_bestTextTarget, new Vec2(vec2_1.x + 64f, vec2_1.y + 22f), new Rectangle?(), Color.White, Maths.DegToRad(_paperAngle), new Vec2(_bestTextTarget.width / 2, _bestTextTarget.height / 2), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.92f);
                    }
                }
                _font.depth = (Depth)0.85f;
                _font.scale = new Vec2(1f);
                _font.Draw(_challengeData.name, vec2_1 + new Vec2(9f, 7f), Colors.DGRed, (Depth)0.85f);
                _font.scale = new Vec2(1f);
                _font.maxWidth = 120;
                _font.Draw(_challengeData.description, vec2_1 + new Vec2(5f, 30f), Colors.BlueGray, (Depth)0.85f);
                _font.scale = new Vec2(1f);
                _font.maxWidth = 300;
                Unlockable unlock = Unlockables.GetUnlock(_challengeData.reward);
                if (unlock != null)
                {
                    if (unlock is UnlockableHat)
                        _font.Draw("|MENUORANGE|Reward - " + unlock.name + " hat", vec2_1 + new Vec2(5f, 84f), Colors.BlueGray, (Depth)0.85f);
                }
                else
                    _font.Draw("|MENUORANGE|Reward - TICKETS", vec2_1 + new Vec2(5f, 84f), Colors.BlueGray, (Depth)0.85f);
                ChallengeTrophy trophy = _challengeData.trophies[1];
                if (trophy.targets != -1)
                    _font.Draw("|DGBLUE|break at least " + trophy.targets.ToString() + " targets", vec2_1 + new Vec2(5f, 75f), Colors.BlueGray, (Depth)0.85f);
                else if (trophy.timeRequirement > 0)
                    _font.Draw("|DGBLUE|beat it in " + trophy.timeRequirement.ToString() + " seconds", vec2_1 + new Vec2(5f, 75f), Colors.BlueGray, (Depth)0.85f);
            }
            _tail.flipV = true;
            Graphics.Draw(_tail, 222f + vec2_3.x, 117f + vec2_3.y);
            bool flag = true;
            if (Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]))
                flag = false;
            if (!flag)
                _dealer.frame += 6;
            _dealer.depth = (Depth)0.5f;
            _dealer.alpha = alpha;
            Graphics.Draw(_dealer, 216f + vec2_3.x, 32f + vec2_3.y);
            if (flag)
                return;
            _dealer.frame -= 6;
        }

        public static void DrawGameLayer()
        {
            if (atCounter)
                return;
            body.depth = (Depth)0f;
            Graphics.Draw(body, standingPosition.x, standingPosition.y);
            if (hover)
                hoverSprite.alpha = Lerp.Float(hoverSprite.alpha, 1f, 0.05f);
            else
                hoverSprite.alpha = Lerp.Float(hoverSprite.alpha, 0f, 0.05f);
            if (hoverSprite.alpha <= 0.01f)
                return;
            hoverSprite.depth = (Depth)0f;
            hoverSprite.flipH = body.flipH;
            if (hoverSprite.flipH)
                Graphics.Draw(hoverSprite, standingPosition.x + 1f, standingPosition.y - 1f);
            else
                Graphics.Draw(hoverSprite, standingPosition.x - 1f, standingPosition.y - 1f);
        }
    }
}
