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
                if (Chancy._mood == DealerMood.Concerned)
                    return Chancy._dealer.frame - 4;
                return Chancy._mood == DealerMood.Point ? Chancy._dealer.frame - 2 : Chancy._dealer.frame;
            }
            set
            {
                if (Chancy._mood == DealerMood.Concerned)
                    Chancy._dealer.frame = value + 4;
                else if (Chancy._mood == DealerMood.Point)
                    Chancy._dealer.frame = value + 2;
                else
                    Chancy._dealer.frame = value;
            }
        }

        public static void Clear()
        {
            Chancy._lines.Clear();
            Chancy._waitLetter = 0f;
            Chancy._waitAfterLine = 0f;
            Chancy._currentLine = "";
            Chancy._mood = DealerMood.Normal;
        }

        public static void Add(string line) => Chancy._lines.Add(line);

        public static ChallengeData activeChallenge
        {
            get => Chancy._challengeData;
            set => Chancy._challengeData = value;
        }

        public static void UpdateRandoms()
        {
            if (Chancy._challengeData == null || Chancy._paperclip == null)
                return;
            Chancy._random = new Random(Chancy._challengeData.name.GetHashCode());
            Random generator = Rando.generator;
            Rando.generator = Chancy._random;
            Chancy._paperclip.frame = Rando.Int(5);
            Chancy._stampAngle = Rando.Float(14f) - 7f;
            Rando.generator = new Random(Chancy.GetChallengeBestString(Chancy._save, Chancy._challengeData).GetHashCode());
            Chancy._paperAngle = Rando.Float(4f) - 2f;
            Chancy._tapeAngle = Chancy._paperAngle + Rando.Float(-1f, 1f);
            Rando.generator = generator;
        }

        public static ChallengeData selectedChallenge => Chancy._chancyChallenges.Count == 0 ? null : Chancy._chancyChallenges[Chancy._challengeSelection];

        public static void AddProposition(ChallengeData challenge, Vec2 duckPos)
        {
            if (challenge.preview != null)
            {
                Texture2D tex = Texture2D.FromStream(DuckGame.Graphics.device, new MemoryStream(Convert.FromBase64String(challenge.preview)));
                Chancy._previewPhoto = new SpriteMap((Tex2D)tex, tex.Width, tex.Height)
                {
                    scale = new Vec2(0.25f)
                };
            }
            Chancy._challengeData = challenge;
            Chancy._realSave = Profiles.active[0].GetSaveData(Chancy._challengeData.levelID);
            Chancy._save = Chancy._realSave.Clone();
            Chancy.UpdateRandoms();
            Chancy.atCounter = false;
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
                    Chancy.standingPosition = position - new Vec2(0f, 25f);
                    Chancy.body.flipH = true;
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
            Chancy.standingPosition = position - new Vec2(0f, 25f);
            Chancy.body.flipH = false;
        }

        public static void Initialize()
        {
            if (Chancy._dealer != null)
                return;
            Chancy._dealer = new SpriteMap("arcade/schooly", 100, 100);
            Chancy._tail = new Sprite("arcade/bubbleTail");
            Chancy.body = new Sprite("arcade/chancy");
            Chancy.hoverSprite = new Sprite("arcade/chancyHover");
            Chancy.challengePaper = new Sprite("arcade/challengePaper");
            Chancy.listPaper = new Sprite("arcade/challengePaperTall");
            Chancy._font = new FancyBitmapFont("smallFont");
            Chancy._photo = new Sprite("arcade/challengePhoto");
            Chancy._paperclip = new SpriteMap("arcade/paperclips", 13, 45);
            Chancy._sticker = new SpriteMap("arcade/stickers", 29, 29)
            {
                frame = 2
            };
            Chancy._tinyStars = new SpriteMap("arcade/tinyStars", 10, 8);
            Chancy._tinyStars.CenterOrigin();
            Chancy._completeStamp = new Sprite("arcade/completeStamp");
            Chancy._completeStamp.CenterOrigin();
            Chancy._pencil = new Sprite("arcade/pencil")
            {
                center = new Vec2(sbyte.MaxValue, 4f)
            };
            Chancy._tape = new Sprite("arcade/tape");
            Chancy._tape.CenterOrigin();
            Chancy._tapePaper = new Sprite("arcade/tapePaper");
            Chancy._tapePaper.CenterOrigin();
        }

        public static void OpenChallengeView() => Chancy.ResetChallengeDialogue();

        public static void ResetChallengeDialogue()
        {
            Chancy.Clear();
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            List<string> stringList = new List<string>()
              {
                "You interested in a little challenge?",
                "Bet you can't finish this one!",
                "You look up for a challenge."
              };
            if (Chancy._save == null)
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
            else if (Chancy._save != null && Chancy._save.trophy > TrophyType.Gold)
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
            else if (Chancy._save != null && Chancy._save.trophy > TrophyType.Silver)
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
            else if (Chancy._save != null && Chancy._save.trophy > TrophyType.Baseline)
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
            Chancy.Add(stringList[Rando.Int(stringList.Count - 1)]);
        }

        public static void OpenChallengeList()
        {
            Chancy._challengeSelection = 0;
            Chancy._chancyChallenges = Challenges.GetEligibleChancyChallenges(Profiles.active[0]);
        }

        public static void MakeConfetti()
        {
            for (int index = 0; index < 40; ++index)
                Level.Add(new ChallengeConfetti(index * 8 + Rando.Float(-10f, 10f), Rando.Float(110f) - 124f));
        }

        public static bool HasNewTrophy() => Chancy._realSave.trophy != Chancy._save.trophy;

        public static bool HasNewTime() => Chancy._realSave.bestTime != Chancy._save.bestTime || Chancy._realSave.goodies != Chancy._save.goodies || Chancy._realSave.targets != Chancy._save.targets;

        public static int GiveTrophy()
        {
            int num = 0;
            if (Chancy._save.trophy != Chancy._realSave.trophy)
            {
                for (int index = (int)(Chancy._save.trophy + 1); (TrophyType)index <= Chancy._realSave.trophy; ++index)
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
                Chancy._save.trophy = Chancy._realSave.trophy;
            }
            return num;
        }

        public static void GiveTime()
        {
            if (Chancy._save.bestTime != Chancy._realSave.bestTime)
                Chancy._save.bestTime = Chancy._realSave.bestTime;
            if (Chancy._save.goodies != Chancy._realSave.goodies)
                Chancy._save.goodies = Chancy._realSave.goodies;
            if (Chancy._save.targets != Chancy._realSave.targets)
                Chancy._save.targets = Chancy._realSave.targets;
            Chancy.UpdateRandoms();
        }

        public static void StopShowingChallengeList()
        {
            Chancy._listLerp = 0f;
            Chancy._challengeLerp = 0f;
            Chancy._challengeLerp = 0f;
            Chancy.lookingAtChallenge = false;
            Chancy.lookingAtList = false;
        }

        public static void Update()
        {
            bool flag1 = Chancy.lookingAtList && _challengeLerp < 0.3f;
            bool flag2 = Chancy.lookingAtChallenge && _listLerp < 0.3f;
            bool flag3 = (Chancy.lookingAtChallenge || UnlockScreen.open) && _listLerp < 0.3f;
            Chancy._listLerp = Lerp.FloatSmooth(Chancy._listLerp, flag1 ? 1f : 0f, 0.2f, 1.05f);
            Chancy._challengeLerp = Lerp.FloatSmooth(Chancy._challengeLerp, flag2 ? 1f : 0f, 0.2f, 1.05f);
            Chancy._chancyLerp = Lerp.FloatSmooth(Chancy._chancyLerp, flag3 ? 1f : 0f, 0.2f, 1.05f);
            if (Chancy.lookingAtList)
            {
                if (Input.Pressed("MENUUP"))
                {
                    --Chancy._challengeSelection;
                    if (Chancy._challengeSelection < 0)
                        Chancy._challengeSelection = 0;
                    else
                        SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENUDOWN"))
                {
                    ++Chancy._challengeSelection;
                    if (Chancy._challengeSelection > Chancy._chancyChallenges.Count - 1)
                        Chancy._challengeSelection = Chancy._chancyChallenges.Count - 1;
                    else
                        SFX.Play("textLetter", 0.7f);
                }
            }
            if (!UnlockScreen.open && !Chancy.lookingAtChallenge)
                return;
            Chancy.alpha = UnlockScreen.open || Chancy.lookingAtChallenge ? Lerp.Float(Chancy.alpha, 1f, 0.05f) : Lerp.Float(Chancy.alpha, 0f, 0.05f);
            if (Chancy.afterChallenge)
            {
                if (afterChallengeWait > 0.0)
                    Chancy.afterChallengeWait -= 0.03f;
                else if (Chancy.HasNewTime() || Chancy.HasNewTrophy())
                {
                    SFX.Play("dacBang", pitch: -0.7f);
                    Chancy.GiveTime();
                    Chancy._giveTickets = Chancy.GiveTrophy();
                    Chancy.afterChallengeWait = 1f;
                    Chancy.MakeConfetti();
                }
                else if (Chancy._giveTickets != 0)
                {
                    Profiles.active[0].ticketCount += Chancy._giveTickets;
                    Chancy.afterChallengeWait = 2f;
                    Chancy._giveTickets = 0;
                    SFX.Play("ching");
                }
                else
                {
                    Chancy.ResetChallengeDialogue();
                    Chancy.afterChallengeWait = 0f;
                    Chancy.afterChallenge = false;
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
            if (Chancy._save != null && Chancy.activeChallenge != null)
            {
                if (Chancy._bestTextTarget == null)
                    Chancy._bestTextTarget = new RenderTarget2D(120, 8);
                DuckGame.Graphics.SetRenderTarget(Chancy._bestTextTarget);
                DuckGame.Graphics.Clear(Color.Transparent);
                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
                string challengeBestString = Chancy.GetChallengeBestString(Chancy._save, Chancy.activeChallenge);
                Chancy._font.Draw(challengeBestString, new Vec2((int)Math.Round(_bestTextTarget.width / 2f - Chancy._font.GetWidth(challengeBestString) / 2f), 0f), Color.Black * 0.7f);
                DuckGame.Graphics.screen.End();
                DuckGame.Graphics.SetRenderTarget(null);
            }
            Chancy.Initialize();
            if (Chancy._lines.Count > 0 && Chancy._currentLine == "")
            {
                Chancy._waitAfterLine -= 0.03f;
                Chancy._talkMove += 0.75f;
                if (_talkMove > 1f)
                {
                    Chancy.frame = 0;
                    Chancy._talkMove = 0f;
                }
                if (_waitAfterLine <= 0f)
                {
                    Chancy._lineProgress.Clear();
                    Chancy._currentLine = Chancy._lines[0];
                    Chancy._lines.RemoveAt(0);
                    Chancy._waitAfterLine = 1.5f;
                    Chancy._mood = DealerMood.Normal;
                }
            }
            if (Chancy._currentLine != "")
            {
                Chancy._waitLetter -= 0.8f;
                if (_waitLetter >= 0.0)
                    return;
                Chancy._talkMove += 0.75f;
                if (_talkMove > 1.0)
                {
                    Chancy.frame = Chancy._currentLine[0] == ' ' || Chancy.frame != 0 ? 0 : Rando.Int(1);
                    Chancy._talkMove = 0f;
                }
                Chancy._waitLetter = 1f;
                char ch1;
                while (Chancy._currentLine[0] == '@')
                {
                    ch1 = Chancy._currentLine[0];
                    string str1 = ch1.ToString() ?? "";
                    for (Chancy._currentLine = Chancy._currentLine.Remove(0, 1); Chancy._currentLine[0] != '@' && Chancy._currentLine.Length > 0; Chancy._currentLine = Chancy._currentLine.Remove(0, 1))
                    {
                        string str2 = str1;
                        ch1 = Chancy._currentLine[0];
                        string str3 = ch1.ToString();
                        str1 = str2 + str3;
                    }
                    Chancy._currentLine = Chancy._currentLine.Remove(0, 1);
                    string val = str1 + "@";
                    Chancy._lineProgress[0].Add(val);
                    Chancy._waitLetter = 3f;
                    if (Chancy._currentLine.Length == 0)
                    {
                        Chancy._currentLine = "";
                        return;
                    }
                }
                while (Chancy._currentLine[0] == '|')
                {
                    Chancy._currentLine = Chancy._currentLine.Remove(0, 1);
                    string str4 = "";
                    for (; Chancy._currentLine[0] != '|' && Chancy._currentLine.Length > 0; Chancy._currentLine = Chancy._currentLine.Remove(0, 1))
                    {
                        string str5 = str4;
                        ch1 = Chancy._currentLine[0];
                        string str6 = ch1.ToString();
                        str4 = str5 + str6;
                    }
                    if (Chancy._currentLine.Length <= 1)
                    {
                        Chancy._currentLine = "";
                        return;
                    }
                    Chancy._currentLine = Chancy._currentLine.Remove(0, 1);
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
                        Chancy._mood = DealerMood.Concerned;
                    else if (str4 == "CALM")
                        Chancy._mood = DealerMood.Normal;
                    else if (str4 == "PEEK")
                        Chancy._mood = DealerMood.Point;
                    if (flag4)
                    {
                        if (Chancy._lineProgress.Count == 0)
                            Chancy._lineProgress.Insert(0, new TextLine()
                            {
                                lineColor = c
                            });
                        else
                            Chancy._lineProgress[0].SwitchColor(c);
                    }
                }
                string str7 = "";
                int index1 = 1;
                if (Chancy._currentLine[0] == ' ')
                {
                    while (index1 < Chancy._currentLine.Length && Chancy._currentLine[index1] != ' ' && Chancy._currentLine[index1] != '^')
                    {
                        if (Chancy._currentLine[index1] == '|')
                        {
                            int index2 = index1 + 1;
                            while (index2 < Chancy._currentLine.Length && Chancy._currentLine[index2] != '|')
                                ++index2;
                            index1 = index2 + 1;
                        }
                        else if (Chancy._currentLine[index1] == '@')
                        {
                            int index3 = index1 + 1;
                            while (index3 < Chancy._currentLine.Length && Chancy._currentLine[index3] != '@')
                                ++index3;
                            index1 = index3 + 1;
                        }
                        else
                        {
                            string str8 = str7;
                            ch1 = Chancy._currentLine[index1];
                            string str9 = ch1.ToString();
                            str7 = str8 + str9;
                            ++index1;
                        }
                    }
                }
                if (Chancy._lineProgress.Count == 0 || Chancy._currentLine[0] == '^' || Chancy._currentLine[0] == ' ' && Chancy._lineProgress[0].Length() + str7.Length > 34)
                {
                    Color color = Color.White;
                    if (Chancy._lineProgress.Count > 0)
                        color = Chancy._lineProgress[0].lineColor;
                    Chancy._lineProgress.Insert(0, new TextLine()
                    {
                        lineColor = color
                    });
                    if (Chancy._currentLine[0] != ' ' && Chancy._currentLine[0] != '^')
                        return;
                    Chancy._currentLine = Chancy._currentLine.Remove(0, 1);
                }
                else
                {
                    if (Chancy._currentLine[0] == '!' || Chancy._currentLine[0] == '?' || Chancy._currentLine[0] == '.')
                        Chancy._waitLetter = 5f;
                    else if (Chancy._currentLine[0] == ',')
                        Chancy._waitLetter = 3f;
                    if (Chancy._currentLine[0] == '*')
                    {
                        Chancy._waitLetter = 5f;
                    }
                    else
                    {
                        Chancy._lineProgress[0].Add(Chancy._currentLine[0]);
                        ch1 = Chancy._currentLine[0];
                        char ch2 = ch1.ToString().ToLowerInvariant()[0];
                        if ((ch2 < 'a' || ch2 > 'z') && ch2 >= '0')
                            ;
                    }
                    Chancy._currentLine = Chancy._currentLine.Remove(0, 1);
                }
            }
            else
            {
                Chancy._talkMove += 0.75f;
                if (_talkMove <= 1.0)
                    return;
                Chancy.frame = 0;
                Chancy._talkMove = 0f;
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
            Vec2 vec2_1 = new Vec2((float)(_listLerp * 270.0 - 200.0), 20f);
            if (Chancy.lookingAtList || _listLerp > 0.01f)
            {
                Chancy.listPaper.depth = (Depth)0.8f;
                DuckGame.Graphics.Draw(Chancy.listPaper, vec2_1.x, vec2_1.y);
                Chancy._font.depth = (Depth)0.85f;
                Chancy._font.scale = new Vec2(1f);
                Chancy._font.Draw("Chancy Challenges", vec2_1 + new Vec2(11f, 6f), Colors.BlueGray, (Depth)0.85f);
                float num = 9f;
                List<ChallengeData> chancyChallenges = Chancy._chancyChallenges;
                int index = 0;
                foreach (ChallengeData challengeData in chancyChallenges)
                {
                    Chancy._font.Draw(challengeData.name, vec2_1 + new Vec2(19f, 12f + num), Colors.DGRed, (Depth)0.85f);
                    Vec2 vec2_2 = vec2_1 + new Vec2(12f, (12f + num + 4f));
                    if (index == Chancy._challengeSelection)
                    {
                        Chancy._pencil.depth = (Depth)0.9f;
                        DuckGame.Graphics.Draw(Chancy._pencil, vec2_2.x, vec2_2.y);
                        DuckGame.Graphics.DrawLine(vec2_1 + new Vec2(19f, (12f + num + 8.5f)), vec2_1 + new Vec2(19f + Chancy._font.GetWidth(challengeData.name), (float)(12f + num + 8.5f)), Colors.SuperDarkBlueGray, depth: ((Depth)0.9f));
                    }
                    ChallengeSaveData saveData = Profiles.active[0].GetSaveData(Chancy._chancyChallenges[index].levelID);
                    if (saveData != null && saveData.trophy > TrophyType.Baseline)
                    {
                        Chancy._tinyStars.frame = (int)(saveData.trophy - 1);
                        Chancy._tinyStars.depth = (Depth)0.85f;
                        DuckGame.Graphics.Draw(_tinyStars, vec2_2.x + 2f, vec2_2.y);
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
            DuckGame.Graphics.DrawRect(vec2_5 + new Vec2(-2f, 0f), vec2_5 + vec2_4 + new Vec2(2f, 0f), Color.Black);
            int num1 = 0;
            for (int index1 = Chancy._lineProgress.Count - 1; index1 >= 0; --index1)
            {
                float stringWidth = DuckGame.Graphics.GetStringWidth(Chancy._lineProgress[index1].text);
                float y = vec2_5.y + 2f + num1 * 9;
                float x = (vec2_5.x + vec2_4.x / 2f - stringWidth / 2f);
                for (int index2 = Chancy._lineProgress[index1].segments.Count - 1; index2 >= 0; --index2)
                {
                    DuckGame.Graphics.DrawString(Chancy._lineProgress[index1].segments[index2].text, new Vec2(x, y), Chancy._lineProgress[index1].segments[index2].color, (Depth)0.85f);
                    x += Chancy._lineProgress[index1].segments[index2].text.Length * 8;
                }
                ++num1;
            }
            if (_challengeLerp > 0.01f && Chancy._challengeData != null)
            {
                vec2_1 = new Vec2(40f, 28f);
                vec2_1 = new Vec2((_challengeLerp * 240f - 200f), 28f);
                Chancy.challengePaper.depth = (Depth)0.8f;
                DuckGame.Graphics.Draw(Chancy.challengePaper, vec2_1.x, vec2_1.y);
                Chancy._paperclip.depth = (Depth)0.92f;
                Chancy._photo.depth = (Depth)0.87f;
                DuckGame.Graphics.Draw(Chancy._photo, vec2_1.x + 135f, vec2_1.y - 3f);
                DuckGame.Graphics.Draw(_paperclip, vec2_1.x + 140f, vec2_1.y - 10f);
                if (Chancy._previewPhoto != null)
                {
                    Chancy._previewPhoto.depth = (Depth)0.89f;
                    Chancy._previewPhoto.angleDegrees = 12f;
                    DuckGame.Graphics.Draw(_previewPhoto, vec2_1.x + 146f, vec2_1.y + 0f);
                }
                if (Chancy._save != null)
                {
                    if (Chancy._save.trophy > TrophyType.Baseline)
                    {
                        Chancy._sticker.depth = (Depth)0.9f;
                        Chancy._sticker.frame = (int)(Chancy._save.trophy - 1);
                        DuckGame.Graphics.Draw(_sticker, vec2_1.x + 123f, vec2_1.y + 2f);
                        Chancy._completeStamp.depth = (Depth)0.9f;
                        Chancy._completeStamp.angleDegrees = Chancy._stampAngle;
                        Chancy._completeStamp.alpha = 0.9f;
                        DuckGame.Graphics.Draw(Chancy._completeStamp, vec2_1.x + 72f, vec2_1.y + 82f);
                    }
                    string challengeBestString = Chancy.GetChallengeBestString(Chancy._save, Chancy._challengeData, true);
                    if (challengeBestString != null && challengeBestString != "")
                    {
                        Chancy._tapePaper.depth = (Depth)0.9f;
                        Chancy._tapePaper.angleDegrees = Chancy._paperAngle;
                        DuckGame.Graphics.Draw(Chancy._tapePaper, vec2_1.x + 64f, vec2_1.y + 22f);
                        Chancy._tape.depth = (Depth)0.95f;
                        Chancy._tape.angleDegrees = Chancy._tapeAngle;
                        DuckGame.Graphics.Draw(Chancy._tape, vec2_1.x + 64f, vec2_1.y + 22f);
                        if (Chancy._bestTextTarget != null)
                            DuckGame.Graphics.Draw((Tex2D)(Texture2D)_bestTextTarget, new Vec2(vec2_1.x + 64f, vec2_1.y + 22f), new Rectangle?(), Color.White, Maths.DegToRad(Chancy._paperAngle), new Vec2(Chancy._bestTextTarget.width / 2, Chancy._bestTextTarget.height / 2), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.92f);
                    }
                }
                Chancy._font.depth = (Depth)0.85f;
                Chancy._font.scale = new Vec2(1f);
                Chancy._font.Draw(Chancy._challengeData.name, vec2_1 + new Vec2(9f, 7f), Colors.DGRed, (Depth)0.85f);
                Chancy._font.scale = new Vec2(1f);
                Chancy._font.maxWidth = 120;
                Chancy._font.Draw(Chancy._challengeData.description, vec2_1 + new Vec2(5f, 30f), Colors.BlueGray, (Depth)0.85f);
                Chancy._font.scale = new Vec2(1f);
                Chancy._font.maxWidth = 300;
                Unlockable unlock = Unlockables.GetUnlock(Chancy._challengeData.reward);
                if (unlock != null)
                {
                    if (unlock is UnlockableHat)
                        Chancy._font.Draw("|MENUORANGE|Reward - " + unlock.name + " hat", vec2_1 + new Vec2(5f, 84f), Colors.BlueGray, (Depth)0.85f);
                }
                else
                    Chancy._font.Draw("|MENUORANGE|Reward - TICKETS", vec2_1 + new Vec2(5f, 84f), Colors.BlueGray, (Depth)0.85f);
                ChallengeTrophy trophy = Chancy._challengeData.trophies[1];
                if (trophy.targets != -1)
                    Chancy._font.Draw("|DGBLUE|break at least " + trophy.targets.ToString() + " targets", vec2_1 + new Vec2(5f, 75f), Colors.BlueGray, (Depth)0.85f);
                else if (trophy.timeRequirement > 0)
                    Chancy._font.Draw("|DGBLUE|beat it in " + trophy.timeRequirement.ToString() + " seconds", vec2_1 + new Vec2(5f, 75f), Colors.BlueGray, (Depth)0.85f);
            }
            Chancy._tail.flipV = true;
            DuckGame.Graphics.Draw(Chancy._tail, 222f + vec2_3.x, 117f + vec2_3.y);
            bool flag = true;
            if (Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]))
                flag = false;
            if (!flag)
                Chancy._dealer.frame += 6;
            Chancy._dealer.depth = (Depth)0.5f;
            Chancy._dealer.alpha = Chancy.alpha;
            DuckGame.Graphics.Draw(_dealer, 216f + vec2_3.x, 32f + vec2_3.y);
            if (flag)
                return;
            Chancy._dealer.frame -= 6;
        }

        public static void DrawGameLayer()
        {
            if (Chancy.atCounter)
                return;
            Chancy.body.depth = (Depth)0f;
            DuckGame.Graphics.Draw(Chancy.body, Chancy.standingPosition.x, Chancy.standingPosition.y);
            if (Chancy.hover)
                Chancy.hoverSprite.alpha = Lerp.Float(Chancy.hoverSprite.alpha, 1f, 0.05f);
            else
                Chancy.hoverSprite.alpha = Lerp.Float(Chancy.hoverSprite.alpha, 0f, 0.05f);
            if (Chancy.hoverSprite.alpha <= 0.01f)
                return;
            Chancy.hoverSprite.depth = (Depth)0f;
            Chancy.hoverSprite.flipH = Chancy.body.flipH;
            if (Chancy.hoverSprite.flipH)
                DuckGame.Graphics.Draw(Chancy.hoverSprite, Chancy.standingPosition.x + 1f, Chancy.standingPosition.y - 1f);
            else
                DuckGame.Graphics.Draw(Chancy.hoverSprite, Chancy.standingPosition.x - 1f, Chancy.standingPosition.y - 1f);
        }
    }
}
