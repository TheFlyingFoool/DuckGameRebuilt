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
            get => _activeChallengeGroup;
            set
            {
                _activeChallengeGroup = value;
                _cards.Clear();
                foreach (string challenge in _activeChallengeGroup.challenges)
                {
                    ChallengeCard challengeCard = new ChallengeCard(0f, 0f, Challenges.GetChallenge(challenge));
                    if (Level.current is ArcadeLevel && (Level.current as ArcadeLevel).customMachine != null)
                        challengeCard.testing = true;
                    _cards.Add(challengeCard);
                }
                UnlockChallenges();
                _cards[0].hover = true;
                _selected = 0;
            }
        }

        public ChallengeCard selected
        {
            get => _viewing;
            set => _viewing = value;
        }

        public void FinishChallenge()
        {
            _afterChallengeWait = 0f;
            _lastPlayed = null;
            _afterChallenge = false;
        }

        public static float alphaVal => _curAlpha;

        public override bool visible
        {
            get => alpha >= 0.01f && base.visible;
            set => base.visible = value;
        }

        public ArcadeHUD()
          : base()
        {
            _font = new BitmapFont("biosFont", 8);
            layer = Layer.HUD;
            _titleWing = new Sprite("arcade/titleWing");
        }

        public override void Initialize()
        {
        }

        public void MakeActive()
        {
            if (!_afterChallenge)
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
            bool first = true;
            bool prevWon = false;
            foreach (ChallengeCard card in _cards)
            {
                if (FireDebug.Debugging)
                {
                    card.unlocked = true;
                    continue;
                }
                if (DGRSettings.TemporaryUnlockAll)
                {
                    card.unlocked = true;
                    continue;
                }
                if (first)
                    card.unlocked = true;
                else if (!card.unlocked && prevWon)
                {
                    card.unlocked = prevWon;
                    if (animate)
                        card.UnlockAnimation();
                }
                prevWon = Profiles.active[0].GetSaveData(card.challenge.levelID).trophy != 0;
                first = false;
            }
        }

        public bool CanUnlockChallenges()
        {
            bool flag1 = true;
            bool flag2 = false;
            foreach (ChallengeCard card in _cards)
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
            _curAlpha = alpha;
            if (launchChallenge)
            {
                _afterChallenge = true;
                _afterChallengeWait = 1f;
            }
            else
            {
                open = alpha > 0.95f;
                if (alpha <= 0.01f)
                    return;
                if (!_afterChallenge && !_goBack)
                {
                    if (_viewing == null)
                    {
                        if (Input.Pressed(Triggers.MenuDown))
                        {
                            ++_selected;
                            if (_selected >= _cards.Count)
                                _selected = _cards.Count - 1;
                            else
                                SFX.Play("menuBlip01");
                        }
                        else if (Input.Pressed(Triggers.MenuUp))
                        {
                            --_selected;
                            if (_selected < 0)
                                _selected = 0;
                            else
                                SFX.Play("menuBlip01");
                        }
                        else if (Input.Pressed(Triggers.Select))
                        {
                            int num1 = 0;
                            bool flag = false;
                            foreach (ChallengeCard card in _cards)
                            {
                                if (num1 == _selected)
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
                                foreach (ChallengeCard card in _cards)
                                {
                                    if (num2 == _selected)
                                    {
                                        card.expand = true;
                                        card.contract = false;
                                        _lerpOffset = card.y;
                                        _viewing = card;
                                        _oldLerpOffset = card.y;
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
                        else if (Input.Pressed(Triggers.Cancel))
                        {
                            SFX.Play("menu_back");
                            quitOut = true;
                        }
                    }
                    else if (Input.Pressed(Triggers.Cancel))
                    {
                        SFX.Play("menu_back");
                        foreach (ChallengeCard card in _cards)
                        {
                            card.contract = false;
                            card.expand = false;
                        }
                        _goBack = true;
                    }
                    else if (Input.Pressed(Triggers.Select))
                    {
                        if (!selected.unlocked)
                        {
                            SFX.Play("scanFail");
                        }
                        else
                        {
                            SFX.Play("selectItem");
                            launchChallenge = true;
                        }
                    }
                }
                if (_afterChallenge)
                {
                    if (_afterChallengeWait > 0f)
                        _afterChallengeWait -= 0.03f;
                    else if (_lastPlayed == null)
                    {
                        _lastPlayed = selected;
                        foreach (ChallengeCard card in _cards)
                        {
                            card.contract = false;
                            card.expand = false;
                            _goBack = true;
                        }
                        _afterChallengeWait = 1f;
                        SFX.Play("menu_back");
                    }
                    else if (_lastPlayed.HasNewBest() || _lastPlayed.HasNewTrophy())
                    {
                        _lastPlayed.GiveTime();
                        _giveTickets = _lastPlayed.GiveTrophy();
                        _afterChallengeWait = 1f;
                        MakeConfetti();
                    }
                    else if (_giveTickets != 0)
                    {
                        Profiles.active[0].ticketCount += _giveTickets;
                        _afterChallengeWait = 2f;
                        _giveTickets = 0;
                        SFX.Play("ching");
                    }
                    else if (CanUnlockChallenges())
                    {
                        UnlockChallenges(true);
                        _afterChallengeWait = 1f;
                    }
                    else
                    {
                        _afterChallengeWait = 0f;
                        _lastPlayed = null;
                        _afterChallenge = false;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@SELECT");
                        Profiles.Save(Profiles.active[0]);
                    }
                }
                if (_goBack)
                {
                    _lerpOffset = Lerp.Float(_lerpOffset, _oldLerpOffset, 8f);
                    if (_lerpOffset == _oldLerpOffset)
                    {
                        _goBack = false;
                        _viewing = null;
                    }
                }
                else if (_viewing != null)
                    _lerpOffset = Lerp.Float(_lerpOffset, 28f, 8f);
                int num = 0;
                foreach (ChallengeCard card in _cards)
                {
                    card.hover = num == _selected;
                    card.Update();
                    ++num;
                }
            }
        }

        public override void Draw()
        {
            if (alpha <= 0.01f || _activeChallengeGroup == null)
                return;
            float ypos = 16f;
            string nameForDisplay = _activeChallengeGroup.GetNameForDisplay();
            _font.alpha = alpha;
            float width = _font.GetWidth(nameForDisplay);
            _font.Draw(nameForDisplay, (160f - width / 2f), ypos, Color.White);
            _titleWing.alpha = alpha;
            _titleWing.flipH = false;
            _titleWing.x = (160f - width / 2f) - (_titleWing.width + 1);
            _titleWing.y = ypos;
            _titleWing.Draw();
            _titleWing.flipH = true;
            _titleWing.x = (160f + width / 2f) + _titleWing.width;
            _titleWing.y = ypos;
            _titleWing.Draw();
            int num = 0;
            foreach (ChallengeCard card in _cards)
            {
                card.alpha = alpha;
                if (num == _selected && card == _viewing)
                    card.position = new Vec2(31f, _lerpOffset);
                else
                    card.position = new Vec2(31f, ypos + 12f + num * 44);
                card.Draw();
                ++num;
            }
        }
    }
}
