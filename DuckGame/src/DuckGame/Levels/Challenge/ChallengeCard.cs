// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeCard
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class ChallengeCard : Thing
    {
        private ChallengeData _challenge;
        private SpriteMap _thumb;
        private SpriteMap _preview;
        private SpriteMap _medalNoRibbon;
        private SpriteMap _medalRibbon;
        private BitmapFont _font;
        public bool hover;
        private bool _unlocked;
        public bool testing;
        public bool expand;
        public bool contract;
        private float _size = 42f;
        public float _alphaMul = 1f;
        public float _dataAlpha;
        private ChallengeSaveData _save;
        private ChallengeSaveData _realSave;
        private FancyBitmapFont _fancyFont;

        public ChallengeData challenge => _challenge;

        public bool unlocked
        {
            get => _unlocked;
            set => _unlocked = value;
        }

        public ChallengeCard(float xpos, float ypos, ChallengeData c)
          : base(xpos, ypos)
        {
            _challenge = c;
            _thumb = new SpriteMap("arcade/challengeThumbnails", 38, 38)
            {
                frame = 1
            };
            _font = new BitmapFont("biosFont", 8);
            _medalNoRibbon = new SpriteMap("arcade/medalNoRibbon", 18, 18);
            _realSave = Profiles.active[0].GetSaveData(_challenge.levelID);
            _save = _realSave.Clone();
            _medalRibbon = new SpriteMap("arcade/medalRibbon", 18, 27)
            {
                center = new Vec2(6f, 3f)
            };
            _fancyFont = new FancyBitmapFont("smallFont");
        }

        public override void Update()
        {
            if (_preview == null && _challenge.preview != null)
            {
                Texture2D tex = Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(_challenge.preview)));
                _preview = new SpriteMap((Tex2D)tex, tex.Width, tex.Height)
                {
                    scale = new Vec2(0.25f)
                };
            }
            _size = Lerp.Float(_size, contract ? 1f : (expand ? 130f : 42f), 8f);
            _alphaMul = Lerp.Float(_alphaMul, contract ? 0f : 1f, 0.1f);
            _dataAlpha = Lerp.Float(_dataAlpha, _size <= 126f || !expand ? 0f : 1f, !expand ? 1f : 0.2f);
        }

        public string MakeQuestionMarks(string val)
        {
            for (int index = 0; index < val.Length; ++index)
            {
                if (val[index] != ' ')
                {
                    val = val.Remove(index, 1);
                    val = val.Insert(index, "?");
                }
            }
            return val;
        }

        public bool HasNewTrophy() => _realSave.trophy != _save.trophy;

        public bool HasNewBest() => _realSave.bestTime != _save.bestTime || _realSave.targets != _save.targets || _realSave.goodies != _save.goodies;

        public int GiveTrophy()
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
            return testing ? 0 : num;
        }

        public void GiveTime()
        {
            if (_save.bestTime != _realSave.bestTime)
                _save.bestTime = _realSave.bestTime;
            if (_save.goodies != _realSave.goodies)
                _save.goodies = _realSave.goodies;
            if (_save.targets == _realSave.targets)
                return;
            _save.targets = _realSave.targets;
        }

        public void UnlockAnimation()
        {
            SFX.Play("landTV", pitch: -0.3f);
            SmallSmoke smallSmoke = SmallSmoke.New(x + 2f, y + 2f);
            smallSmoke.layer = Layer.HUD;
            Level.Add(smallSmoke);
        }

        public override void Draw()
        {
            float num1 = alpha * (hover ? 1f : 0.6f) * _alphaMul;
            _font.alpha = num1;
            Graphics.DrawRect(position, position + new Vec2(258f, _size), Color.White * num1, (Depth)(0.8f + num1 * 0.04f), false);
            if (_save.trophy != TrophyType.Baseline)
            {
                _medalRibbon.depth = (Depth)(0.81f + num1 * 0.04f);
                _medalRibbon.color = new Color(num1, num1, num1);
                _medalRibbon.alpha = ArcadeHUD.alphaVal;
                if (_save.trophy == TrophyType.Bronze)
                    _medalRibbon.frame = 0;
                else if (_save.trophy == TrophyType.Silver)
                    _medalRibbon.frame = 1;
                else if (_save.trophy == TrophyType.Gold)
                    _medalRibbon.frame = 2;
                else if (_save.trophy == TrophyType.Platinum)
                    _medalRibbon.frame = 3;
                else if (_save.trophy == TrophyType.Developer)
                    _medalRibbon.frame = 4;
                Graphics.Draw(_medalRibbon, position.x, position.y);
            }
            else if (!_unlocked)
            {
                _medalRibbon.depth = (Depth)(0.81f + num1 * 0.04f);
                _medalRibbon.color = new Color(num1, num1, num1);
                _medalRibbon.frame = 5;
                Graphics.Draw(_medalRibbon, position.x, position.y);
            }
            _thumb.alpha = num1;
            _thumb.depth = (Depth)(0.8f + num1 * 0.04f);
            _thumb.frame = _unlocked ? 1 : 0;
            if (_unlocked && _preview != null)
            {
                _preview.alpha = num1;
                _preview.depth = (Depth)(0.8f + num1 * 0.04f);
                Graphics.Draw(_preview, this.x + 2f, y + 2f);
            }
            else
                Graphics.Draw(_thumb, this.x + 2f, y + 2f);
            _font.maxWidth = 200;
            string str1 = _challenge.GetNameForDisplay();
            if (!_unlocked)
                str1 = MakeQuestionMarks(str1);
            _font.Draw(str1, this.x + 41f, y + 2f, Color.White * num1, (Depth)1f);
            Color c1 = new Color(247, 224, 89);
            string str2 = _challenge.description;
            if (!_unlocked)
                str2 = MakeQuestionMarks(str2);
            _fancyFont.maxWidth = 200;
            _fancyFont.alpha = num1;
            _fancyFont.xscale = _fancyFont.yscale = 0.75f;
            _fancyFont.Draw(str2, this.x + 41f, y + 12f, c1, (Depth)0.8f);
            if (_dataAlpha <= 0.01f)
                return;
            float num2 = _dataAlpha * num1;

#if DEBUG
            Graphics.DrawLine(position + new Vec2(0f, 42f), position + new Vec2(170f, 42f), Color.White * num2, depth: (Depth)(0.8f + num1 * 0.04f));
            Graphics.DrawLine(position + new Vec2(0f, 64f), position + new Vec2(170, 64f), Color.White * num2, depth: (Depth)(0.8f + num1 * 0.04f));
            Graphics.DrawRect(position + new Vec2(170, 34), position + new Vec2(270, 130), Color.White * num2, (Depth)(0.8f + num1 * 0.04f + 2), false);
            Graphics.DrawRect(position + new Vec2(170, 34), position + new Vec2(270, 130), Color.Black, 0.91f , true);

            for (int i = 0; i < 5; i++)
            {
                Color c = Color.White;
                if (i == 0) c = Colors.Developer;
                else if (i == 1) c = Colors.Platinum;
                else if (i == 2) c = Colors.Gold;
                else if (i == 3) c = Colors.Silver;
                else if (i == 4) c = Colors.Bronze;

                _fancyFont.Draw($"#{i + 1} player\n00:00:00", this.x + 184, y + 40 + 18 * i, c, (Depth)1f);
                _fancyFont.Draw($"pfp", this.x + 170, y + 40 + 18 * i, Color.White, (Depth)1f);

            }
#else
            Graphics.DrawLine(position + new Vec2(0f, 42f), position + new Vec2(258, 42f), Color.White * num2, depth: (Depth)(0.8f + num1 * 0.04f));
            Graphics.DrawLine(position + new Vec2(0f, 64f), position + new Vec2(258, 64f), Color.White * num2, depth: (Depth)(0.8f + num1 * 0.04f));
#endif

            _font.alpha = num2;
            Color color = new Color(245, 165, 36);
            Color c2 = Colors.DGRed;
            if (_save.trophy == TrophyType.Bronze)
                c2 = Colors.Bronze;
            else if (_save.trophy == TrophyType.Silver)
                c2 = Colors.Silver;
            else if (_save.trophy == TrophyType.Gold)
                c2 = Colors.Gold;
            else if (_save.trophy == TrophyType.Platinum)
                c2 = Colors.Platinum;
            else if (_save.trophy == TrophyType.Developer)
                c2 = Colors.Developer;
            _fancyFont.Draw("|DGBLUE|" + _challenge.goal, this.x + 4f, y + 45f, Color.White, (Depth)0.9f);
            _font.Draw(Chancy.GetChallengeBestString(_save, _challenge), this.x + 4f, (float)(y + 45f + 9f), c2, (Depth)1f);
            bool flag1 = false;
            _medalNoRibbon.depth = (Depth)(0.8f + num1 * 0.04f);
            _medalNoRibbon.alpha = num2;
            _medalNoRibbon.frame = 2;
            float x = this.x + 6f;
            float num3 = y + 68f;
            Graphics.Draw(_medalNoRibbon, x, num3);
            Color c3 = new Color(245, 165, 36);
            _font.Draw("GOLD", x + 22f, num3, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy1 = _challenge.trophies.FirstOrDefault(val => val.type == TrophyType.Gold);
            string text1 = "";
            bool flag2 = false;
            if (challengeTrophy1.timeRequirement > 0)
            {
                TimeSpan span = TimeSpan.FromSeconds(challengeTrophy1.timeRequirement);
                text1 = text1 + MonoMain.TimeString(span, small: true) + " ";
                flag1 = true;
                flag2 = true;
            }
            int num4;
            if (challengeTrophy1.targets > 0)
            {
                if (text1 != "")
                    text1 += ", ";
                string str3 = text1;
                num4 = challengeTrophy1.targets;
                string str4 = num4.ToString();
                text1 = str3 + "|LIME|" + str4 + " TARGETS";
            }
            if (challengeTrophy1.goodies > 0)
            {
                if (text1 != "")
                    text1 += ", ";
                string str5 = "GOODIES";
                if (_challenge.prefix != "")
                    str5 = _challenge.prefix;
                string[] strArray = new string[5]
                {
          text1,
          "|ORANGE|",
          null,
          null,
          null
                };
                num4 = challengeTrophy1.goodies;
                strArray[2] = num4.ToString();
                strArray[3] = " ";
                strArray[4] = str5;
                text1 = string.Concat(strArray);
            }
            _font.Draw(text1, x + 22f, num3 + 9f, Color.White, (Depth)1f);
            float num5 = (y + 68f + 20f);
            _medalNoRibbon.alpha = num2;
            _medalNoRibbon.frame = 1;
            Graphics.Draw(_medalNoRibbon, x, num5);
            c3 = new Color(173, 173, 173);
            _font.Draw("SILVER", x + 22f, num5, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy2 = _challenge.trophies.FirstOrDefault(val => val.type == TrophyType.Silver);
            string text2 = "";
            if (flag2 && challengeTrophy2.timeRequirement == 0 && _challenge.trophies[0].timeRequirement != 0)
                challengeTrophy2.timeRequirement = _challenge.trophies[0].timeRequirement;
            if (challengeTrophy2.timeRequirement > 0)
            {
                TimeSpan span = TimeSpan.FromSeconds(challengeTrophy2.timeRequirement);
                text2 = text2 + MonoMain.TimeString(span, small: true) + " ";
                flag1 = true;
                flag2 = true;
            }
            else if (flag1 && _challenge.trophies[0].timeRequirement == 0)
                text2 = "ANY TIME ";
            if (challengeTrophy2.targets > 0)
            {
                if (text2 != "")
                    text2 += ", ";
                string str6 = text2;
                num4 = challengeTrophy2.targets;
                string str7 = num4.ToString();
                text2 = str6 + "|LIME|" + str7 + " TARGETS";
            }
            if (challengeTrophy2.goodies > 0)
            {
                if (text2 != "")
                    text2 += ", ";
                string str8 = "GOODIES";
                if (_challenge.prefix != "")
                    str8 = _challenge.prefix;
                string[] strArray = new string[5]
                {
          text2,
          "|ORANGE|",
          null,
          null,
          null
                };
                num4 = challengeTrophy2.goodies;
                strArray[2] = num4.ToString();
                strArray[3] = " ";
                strArray[4] = str8;
                text2 = string.Concat(strArray);
            }
            _font.Draw(text2, x + 22f, num5 + 9f, Color.White, (Depth)1f);
            float num6 = (y + 68f + 40f);
            _medalNoRibbon.alpha = num2;
            _medalNoRibbon.frame = 0;
            Graphics.Draw(_medalNoRibbon, x, num6);
            c3 = new Color(181, 86, 3);
            _font.Draw("BRONZE", x + 22f, num6, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy3 = _challenge.trophies.FirstOrDefault(val => val.type == TrophyType.Bronze);
            string text3 = "";
            if (flag2 && challengeTrophy3.timeRequirement == 0 && _challenge.trophies[0].timeRequirement != 0)
                challengeTrophy3.timeRequirement = _challenge.trophies[0].timeRequirement;
            if (challengeTrophy3.timeRequirement > 0)
            {
                TimeSpan span = TimeSpan.FromSeconds(challengeTrophy3.timeRequirement);
                text3 = text3 + MonoMain.TimeString(span, small: true) + " ";
            }
            else if (flag1 && _challenge.trophies[0].timeRequirement == 0)
                text3 = "ANY TIME ";
            if (challengeTrophy3.targets > 0)
            {
                if (text3 != "")
                    text3 += ", ";
                string str9 = text3;
                num4 = challengeTrophy3.targets;
                string str10 = num4.ToString();
                text3 = str9 + "|LIME|" + str10 + " TARGETS";
            }
            if (challengeTrophy3.goodies > 0)
            {
                if (text3 != "")
                    text3 += ", ";
                string str11 = "GOODIES";
                if (_challenge.prefix != "")
                    str11 = _challenge.prefix;
                string[] strArray = new string[5]
                {
          text3,
          "|ORANGE|",
          null,
          null,
          null
                };
                num4 = challengeTrophy3.goodies;
                strArray[2] = num4.ToString();
                strArray[3] = " ";
                strArray[4] = str11;
                text3 = string.Concat(strArray);
            }
            _font.Draw(text3, x + 22f, num6 + 9f, Color.White, (Depth)1f);
        }
    }
}
