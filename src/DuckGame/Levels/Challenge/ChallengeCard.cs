// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeCard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public ChallengeData challenge => this._challenge;

        public bool unlocked
        {
            get => this._unlocked;
            set => this._unlocked = value;
        }

        public ChallengeCard(float xpos, float ypos, ChallengeData c)
          : base(xpos, ypos)
        {
            this._challenge = c;
            this._thumb = new SpriteMap("arcade/challengeThumbnails", 38, 38)
            {
                frame = 1
            };
            this._font = new BitmapFont("biosFont", 8);
            this._medalNoRibbon = new SpriteMap("arcade/medalNoRibbon", 18, 18);
            this._realSave = Profiles.active[0].GetSaveData(this._challenge.levelID);
            this._save = this._realSave.Clone();
            this._medalRibbon = new SpriteMap("arcade/medalRibbon", 18, 27)
            {
                center = new Vec2(6f, 3f)
            };
            this._fancyFont = new FancyBitmapFont("smallFont");
        }

        public override void Update()
        {
            if (this._preview == null && this._challenge.preview != null)
            {
                Texture2D tex = Texture2D.FromStream(DuckGame.Graphics.device, new MemoryStream(Convert.FromBase64String(this._challenge.preview)));
                this._preview = new SpriteMap((Tex2D)tex, tex.Width, tex.Height)
                {
                    scale = new Vec2(0.25f)
                };
            }
            this._size = Lerp.Float(this._size, this.contract ? 1f : (this.expand ? 130f : 42f), 8f);
            this._alphaMul = Lerp.Float(this._alphaMul, this.contract ? 0.0f : 1f, 0.1f);
            this._dataAlpha = Lerp.Float(this._dataAlpha, _size <= 126.0 || !this.expand ? 0.0f : 1f, !this.expand ? 1f : 0.2f);
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

        public bool HasNewTrophy() => this._realSave.trophy != this._save.trophy;

        public bool HasNewBest() => this._realSave.bestTime != this._save.bestTime || this._realSave.targets != this._save.targets || this._realSave.goodies != this._save.goodies;

        public int GiveTrophy()
        {
            int num = 0;
            if (this._save.trophy != this._realSave.trophy)
            {
                for (int index = (int)(this._save.trophy + 1); (TrophyType)index <= this._realSave.trophy; ++index)
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
                this._save.trophy = this._realSave.trophy;
            }
            return this.testing ? 0 : num;
        }

        public void GiveTime()
        {
            if (this._save.bestTime != this._realSave.bestTime)
                this._save.bestTime = this._realSave.bestTime;
            if (this._save.goodies != this._realSave.goodies)
                this._save.goodies = this._realSave.goodies;
            if (this._save.targets == this._realSave.targets)
                return;
            this._save.targets = this._realSave.targets;
        }

        public void UnlockAnimation()
        {
            SFX.Play("landTV", pitch: -0.3f);
            SmallSmoke smallSmoke = SmallSmoke.New(this.x + 2f, this.y + 2f);
            smallSmoke.layer = Layer.HUD;
            Level.Add(smallSmoke);
        }

        public override void Draw()
        {
            float num1 = this.alpha * (this.hover ? 1f : 0.6f) * this._alphaMul;
            this._font.alpha = num1;
            DuckGame.Graphics.DrawRect(this.position, this.position + new Vec2(258f, this._size), Color.White * num1, (Depth)(0.8f + (double)num1 * 0.04f), false);
            if (this._save.trophy != TrophyType.Baseline)
            {
                this._medalRibbon.depth = (Depth)(0.81f + num1 * 0.04f);
                this._medalRibbon.color = new Color(num1, num1, num1);
                this._medalRibbon.alpha = ArcadeHUD.alphaVal;
                if (this._save.trophy == TrophyType.Bronze)
                    this._medalRibbon.frame = 0;
                else if (this._save.trophy == TrophyType.Silver)
                    this._medalRibbon.frame = 1;
                else if (this._save.trophy == TrophyType.Gold)
                    this._medalRibbon.frame = 2;
                else if (this._save.trophy == TrophyType.Platinum)
                    this._medalRibbon.frame = 3;
                else if (this._save.trophy == TrophyType.Developer)
                    this._medalRibbon.frame = 4;
                DuckGame.Graphics.Draw(_medalRibbon, this.position.x, this.position.y);
            }
            else if (!this._unlocked)
            {
                this._medalRibbon.depth = (Depth)(0.81f + num1 * 0.04f);
                this._medalRibbon.color = new Color(num1, num1, num1);
                this._medalRibbon.frame = 5;
                DuckGame.Graphics.Draw(_medalRibbon, this.position.x, this.position.y);
            }
            this._thumb.alpha = num1;
            this._thumb.depth = (Depth)(0.8f + num1 * 0.04f);
            this._thumb.frame = this._unlocked ? 1 : 0;
            if (this._unlocked && this._preview != null)
            {
                this._preview.alpha = num1;
                this._preview.depth = (Depth)(0.8f + (double)num1 * 0.04f);
                DuckGame.Graphics.Draw(_preview, this.x + 2f, this.y + 2f);
            }
            else
                DuckGame.Graphics.Draw(_thumb, this.x + 2f, this.y + 2f);
            this._font.maxWidth = 200;
            string str1 = this._challenge.GetNameForDisplay();
            if (!this._unlocked)
                str1 = this.MakeQuestionMarks(str1);
            this._font.Draw(str1, this.x + 41f, this.y + 2f, Color.White * num1, (Depth)1f);
            Color c1 = new Color(247, 224, 89);
            string str2 = this._challenge.description;
            if (!this._unlocked)
                str2 = this.MakeQuestionMarks(str2);
            this._fancyFont.maxWidth = 200;
            this._fancyFont.alpha = num1;
            this._fancyFont.xscale = this._fancyFont.yscale = 0.75f;
            this._fancyFont.Draw(str2, this.x + 41f, this.y + 12f, c1, (Depth)1f);
            if (_dataAlpha <= 0.01f)
                return;
            float num2 = this._dataAlpha * num1;
            DuckGame.Graphics.DrawLine(this.position + new Vec2(0.0f, 42f), this.position + new Vec2(258f, 42f), Color.White * num2, depth: (Depth)(0.8f + (double)num1 * 0.04f));
            DuckGame.Graphics.DrawLine(this.position + new Vec2(0.0f, 64f), this.position + new Vec2(258f, 64f), Color.White * num2, depth: (Depth)(0.8f + (double)num1 * 0.04f));
            this._font.alpha = num2;
            Color color = new Color(245, 165, 36);
            Color c2 = Colors.DGRed;
            if (this._save.trophy == TrophyType.Bronze)
                c2 = Colors.Bronze;
            else if (this._save.trophy == TrophyType.Silver)
                c2 = Colors.Silver;
            else if (this._save.trophy == TrophyType.Gold)
                c2 = Colors.Gold;
            else if (this._save.trophy == TrophyType.Platinum)
                c2 = Colors.Platinum;
            else if (this._save.trophy == TrophyType.Developer)
                c2 = Colors.Developer;
            this._fancyFont.Draw("|DGBLUE|" + this._challenge.goal, this.x + 6f, this.y + 45f, Color.White, (Depth)1f);
            this._font.Draw(Chancy.GetChallengeBestString(this._save, this._challenge), this.x + 6f, (float)(this.y + 45f + 9f), c2, (Depth)1f);
            bool flag1 = false;
            this._medalNoRibbon.depth = (Depth)(0.8f + (double)num1 * 0.04f);
            this._medalNoRibbon.alpha = num2;
            this._medalNoRibbon.frame = 2;
            float x = this.x + 6f;
            float num3 = this.y + 68f;
            DuckGame.Graphics.Draw(_medalNoRibbon, x, num3);
            Color c3 = new Color(245, 165, 36);
            this._font.Draw("GOLD", x + 22f, num3, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy1 = this._challenge.trophies.FirstOrDefault<ChallengeTrophy>(val => val.type == TrophyType.Gold);
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
                if (this._challenge.prefix != "")
                    str5 = this._challenge.prefix;
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
            this._font.Draw(text1, x + 22f, num3 + 9f, Color.White, (Depth)1f);
            float num5 = (float)((double)this.y + 68.0 + 20.0);
            this._medalNoRibbon.alpha = num2;
            this._medalNoRibbon.frame = 1;
            DuckGame.Graphics.Draw(_medalNoRibbon, x, num5);
            c3 = new Color(173, 173, 173);
            this._font.Draw("SILVER", x + 22f, num5, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy2 = this._challenge.trophies.FirstOrDefault<ChallengeTrophy>(val => val.type == TrophyType.Silver);
            string text2 = "";
            if (flag2 && challengeTrophy2.timeRequirement == 0 && this._challenge.trophies[0].timeRequirement != 0)
                challengeTrophy2.timeRequirement = this._challenge.trophies[0].timeRequirement;
            if (challengeTrophy2.timeRequirement > 0)
            {
                TimeSpan span = TimeSpan.FromSeconds(challengeTrophy2.timeRequirement);
                text2 = text2 + MonoMain.TimeString(span, small: true) + " ";
                flag1 = true;
                flag2 = true;
            }
            else if (flag1 && this._challenge.trophies[0].timeRequirement == 0)
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
                if (this._challenge.prefix != "")
                    str8 = this._challenge.prefix;
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
            this._font.Draw(text2, x + 22f, num5 + 9f, Color.White, (Depth)1f);
            float num6 = (float)((double)this.y + 68.0 + 40.0);
            this._medalNoRibbon.alpha = num2;
            this._medalNoRibbon.frame = 0;
            DuckGame.Graphics.Draw(_medalNoRibbon, x, num6);
            c3 = new Color(181, 86, 3);
            this._font.Draw("BRONZE", x + 22f, num6, c3, (Depth)1f);
            ChallengeTrophy challengeTrophy3 = this._challenge.trophies.FirstOrDefault<ChallengeTrophy>(val => val.type == TrophyType.Bronze);
            string text3 = "";
            if (flag2 && challengeTrophy3.timeRequirement == 0 && this._challenge.trophies[0].timeRequirement != 0)
                challengeTrophy3.timeRequirement = this._challenge.trophies[0].timeRequirement;
            if (challengeTrophy3.timeRequirement > 0)
            {
                TimeSpan span = TimeSpan.FromSeconds(challengeTrophy3.timeRequirement);
                text3 = text3 + MonoMain.TimeString(span, small: true) + " ";
            }
            else if (flag1 && this._challenge.trophies[0].timeRequirement == 0)
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
                if (this._challenge.prefix != "")
                    str11 = this._challenge.prefix;
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
            this._font.Draw(text3, x + 22f, num6 + 9f, Color.White, (Depth)1f);
        }
    }
}
