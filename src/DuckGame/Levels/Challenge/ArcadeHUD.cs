// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeHUD
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class ArcadeHUD : Thing
    {
        private BitmapFont _font;
        private Sprite _titleWing;
        private ChallengeGroup _activeChallengeGroup;
        private int _selected;
        private float _lerpOffset;
        private float _oldLerpOffset;
        private bool _goBack;
        private ChallengeCard _viewing;
        public bool quitOut;
        public bool launchChallenge;
        private bool _afterChallenge;
        private float _afterChallengeWait = 1f;
        public static bool open;
        private static float _curAlpha;
        private int _giveTickets;
        private List<ChallengeCard> _cards = new List<ChallengeCard>();
        private ChallengeCard _lastPlayed;

        public ChallengeGroup activeChallengeGroup
        {
            get => this._activeChallengeGroup;
            set
            {
                this._activeChallengeGroup = value;
                this._cards.Clear();
                foreach (string challenge in this._activeChallengeGroup.challenges)
                {
                    ChallengeCard challengeCard = new ChallengeCard(0.0f, 0.0f, Challenges.GetChallenge(challenge));
                    if (Level.current is ArcadeLevel && (Level.current as ArcadeLevel).customMachine != null)
                        challengeCard.testing = true;
                    this._cards.Add(challengeCard);
                }
                this.UnlockChallenges();
                this._cards[0].hover = true;
                this._selected = 0;
            }
        }

        public ChallengeCard selected
        {
            get => this._viewing;
            set => this._viewing = value;
        }

        public void FinishChallenge()
        {
            this._afterChallengeWait = 0.0f;
            this._lastPlayed = null;
            this._afterChallenge = false;
        }

        public static float alphaVal => ArcadeHUD._curAlpha;

        public override bool visible
        {
            get => this.alpha >= 0.01f && base.visible;
            set => base.visible = value;
        }

        public ArcadeHUD()
          : base()
        {
            this._font = new BitmapFont("biosFont", 8);
            this.layer = Layer.HUD;
            this._titleWing = new Sprite("arcade/titleWing");
        }

        public override void Initialize()
        {
        }

        public void MakeActive()
        {
            if (!this._afterChallenge)
            {
                HUD.CloseAllCorners();
                HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@SELECT");
            }
            else
            {
                HUD.CloseAllCorners();
                HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
            }
        }

        public void MakeConfetti()
        {
            for (int index = 0; index < 40; ++index)
                Level.Add(new ChallengeConfetti(index * 8 + Rando.Float(-10f, 10f), Rando.Float(110f) - 124f));
            SFX.Play("dacBang", pitch: -0.7f);
        }

        public void UnlockChallenges(bool animate = false)
        {
            bool flag1 = true;
            bool flag2 = false;
            foreach (ChallengeCard card in this._cards)
            {
                if (flag1)
                    card.unlocked = true;
                else if (!card.unlocked && flag2)
                {
                    card.unlocked = flag2;
                    if (animate)
                        card.UnlockAnimation();
                }
                flag2 = Profiles.active[0].GetSaveData(card.challenge.levelID).trophy != 0;
                flag1 = false;
            }
        }

        public bool CanUnlockChallenges()
        {
            bool flag1 = true;
            bool flag2 = false;
            foreach (ChallengeCard card in this._cards)
            {
                if (flag1 && !card.unlocked || flag2 && !card.unlocked)
                    return true;
                flag2 = Profiles.active[0].GetSaveData(card.challenge.levelID).trophy != 0;
                flag1 = false;
            }
            return false;
        }

        public override void Update()
        {
            ArcadeHUD._curAlpha = this.alpha;
            if (this.launchChallenge)
            {
                this._afterChallenge = true;
                this._afterChallengeWait = 1f;
            }
            else
            {
                ArcadeHUD.open = this.alpha > 0.95f;
                if (this.alpha <= 0.01f)
                    return;
                if (!this._afterChallenge && !this._goBack)
                {
                    if (this._viewing == null)
                    {
                        if (Input.Pressed("MENUDOWN"))
                        {
                            ++this._selected;
                            if (this._selected >= this._cards.Count)
                                this._selected = this._cards.Count - 1;
                            else
                                SFX.Play("menuBlip01");
                        }
                        else if (Input.Pressed("MENUUP"))
                        {
                            --this._selected;
                            if (this._selected < 0)
                                this._selected = 0;
                            else
                                SFX.Play("menuBlip01");
                        }
                        else if (Input.Pressed("SELECT"))
                        {
                            int num1 = 0;
                            bool flag = false;
                            foreach (ChallengeCard card in this._cards)
                            {
                                if (num1 == this._selected)
                                {
                                    if (card.unlocked)
                                    {
                                        flag = true;
                                        break;
                                    }
                                    break;
                                }
                                ++num1;
                            }
                            int num2 = 0;
                            if (flag)
                            {
                                foreach (ChallengeCard card in this._cards)
                                {
                                    if (num2 == this._selected)
                                    {
                                        card.expand = true;
                                        card.contract = false;
                                        this._lerpOffset = card.y;
                                        this._viewing = card;
                                        this._oldLerpOffset = card.y;
                                        SFX.Play("menu_select");
                                    }
                                    else
                                    {
                                        card.contract = true;
                                        card.expand = false;
                                    }
                                    ++num2;
                                }
                            }
                            else
                                SFX.Play("scanFail");
                        }
                        else if (Input.Pressed("CANCEL"))
                        {
                            SFX.Play("menu_back");
                            this.quitOut = true;
                        }
                    }
                    else if (Input.Pressed("CANCEL"))
                    {
                        SFX.Play("menu_back");
                        foreach (ChallengeCard card in this._cards)
                        {
                            card.contract = false;
                            card.expand = false;
                        }
                        this._goBack = true;
                    }
                    else if (Input.Pressed("SELECT"))
                    {
                        if (!this.selected.unlocked)
                        {
                            SFX.Play("scanFail");
                        }
                        else
                        {
                            SFX.Play("selectItem");
                            this.launchChallenge = true;
                        }
                    }
                }
                if (this._afterChallenge)
                {
                    if (_afterChallengeWait > 0.0)
                        this._afterChallengeWait -= 0.03f;
                    else if (this._lastPlayed == null)
                    {
                        this._lastPlayed = this.selected;
                        foreach (ChallengeCard card in this._cards)
                        {
                            card.contract = false;
                            card.expand = false;
                            this._goBack = true;
                        }
                        this._afterChallengeWait = 1f;
                        SFX.Play("menu_back");
                    }
                    else if (this._lastPlayed.HasNewBest() || this._lastPlayed.HasNewTrophy())
                    {
                        this._lastPlayed.GiveTime();
                        this._giveTickets = this._lastPlayed.GiveTrophy();
                        this._afterChallengeWait = 1f;
                        this.MakeConfetti();
                    }
                    else if (this._giveTickets != 0)
                    {
                        Profiles.active[0].ticketCount += this._giveTickets;
                        this._afterChallengeWait = 2f;
                        this._giveTickets = 0;
                        SFX.Play("ching");
                    }
                    else if (this.CanUnlockChallenges())
                    {
                        this.UnlockChallenges(true);
                        this._afterChallengeWait = 1f;
                    }
                    else
                    {
                        this._afterChallengeWait = 0.0f;
                        this._lastPlayed = null;
                        this._afterChallenge = false;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@SELECT");
                        Profiles.Save(Profiles.active[0]);
                    }
                }
                if (this._goBack)
                {
                    this._lerpOffset = Lerp.Float(this._lerpOffset, this._oldLerpOffset, 8f);
                    if (_lerpOffset == (double)this._oldLerpOffset)
                    {
                        this._goBack = false;
                        this._viewing = null;
                    }
                }
                else if (this._viewing != null)
                    this._lerpOffset = Lerp.Float(this._lerpOffset, 28f, 8f);
                int num = 0;
                foreach (ChallengeCard card in this._cards)
                {
                    card.hover = num == this._selected;
                    card.Update();
                    ++num;
                }
            }
        }

        public override void Draw()
        {
            if ((double)this.alpha <= 0.01f || this._activeChallengeGroup == null)
                return;
            float ypos = 16f;
            string nameForDisplay = this._activeChallengeGroup.GetNameForDisplay();
            this._font.alpha = this.alpha;
            float width = this._font.GetWidth(nameForDisplay);
            this._font.Draw(nameForDisplay, (160f - width / 2f), ypos, Color.White);
            this._titleWing.alpha = this.alpha;
            this._titleWing.flipH = false;
            this._titleWing.x = (160f - width / 2f) - (this._titleWing.width + 1);
            this._titleWing.y = ypos;
            this._titleWing.Draw();
            this._titleWing.flipH = true;
            this._titleWing.x = (160f + width / 2f) + _titleWing.width;
            this._titleWing.y = ypos;
            this._titleWing.Draw();
            int num = 0;
            foreach (ChallengeCard card in this._cards)
            {
                card.alpha = this.alpha;
                if (num == this._selected && card == this._viewing)
                    card.position = new Vec2(31f, this._lerpOffset);
                else
                    card.position = new Vec2(31f, ypos + 12f + num * 44);
                card.Draw();
                ++num;
            }
        }
    }
}
