// Decompiled with JetBrains decompiler
// Type: DuckGame.UILevelBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UILevelBox : UIMenu
    {
        private List<Sprite> _frames = new List<Sprite>();
        private BitmapFont _font;
        private BitmapFont _thickBiosNum;
        private FancyBitmapFont _fancyFont;
        private FancyBitmapFont _chalkFont;
        private Sprite _xpBar;
        private Sprite _barFront;
        private Sprite _addXPBar;
        private SpriteMap _lev;
        private SpriteMap _littleMan;
        private Sprite _egg;
        //private Sprite _levelUpArrow;
        private Sprite _talkBubble;
        private SpriteMap _duckCoin;
        private Sprite _sandwich;
        private Sprite _heart;
        private SpriteMap _sandwichCard;
        private SpriteMap _sandwichStamp;
        private SpriteMap _weekDays;
        private Sprite _taxi;
        private SpriteMap _circle;
        private SpriteMap _cross;
        private SpriteMap _days;
        private Sprite _gachaBar;
        private Sprite _sandwichBar;
        private UILevelBoxState _state;
        private float _xpLost;
        private float _slideXPBar;
        private float _startWait = 1f;
        private float _drain = 1f;
        private float _addLerp;
        private float _coinLerp = 1f;
        private float _coinLerp2 = 1f;
        private bool _firstParticleIn;
        private Sprite _xpPoint;
        private Sprite _xpPointOutline;
        private SpriteMap _milk;
        private List<XPPlus> _particles = new List<XPPlus>();
        private List<LittleHeart> _hearts = new List<LittleHeart>();
        private float _particleWait = 1f;
        private int _xpValue;
        public static int currentLevel;
        private int _currentLevel = 4;
        private int _desiredLevel = 4;
        //private int _arrowsLevel = -1;
        private float _newXPValue;
        private float _oldXPValue;
        private int _gachaValue;
        private float _newGachaValue;
        private float _oldGachaValue;
        private int _sandwichValue;
        private float _newSandwichValue;
        private float _oldSandwichValue;
        private int _milkValue;
        private float _newMilkValue;
        private float _oldMilkValue;
        private bool _stampCard;
        private float _stampCardLerp;
        private int _roundsPlayed;
        private BitmapFont _bigFont;
        private int _currentDay;
        //private List<Tex2D> _eggs = new List<Tex2D>();
        //private static bool _firstOpen;
        private int _originalXP;
        private int _totalXP;
        private int _startRoundsPlayed;
        private int _newGrowthLevel = 1;
        private MenuBoolean _menuBool = new MenuBoolean();
        public KeyValuePair<string, XPPair> _currentStat;
        private float _talk;
        private bool close;
        private string _talkLine = "";
        private string _feedLine = "I AM A HUNGRY\nLITTLE MAN.";
        private string _startFeedLine = "I AM A HUNGRY\nLITTLE MAN.";
        private float _talkWait;
        private bool _talking;
        private float _finishTalkWait;
        private bool _alwaysClose;
        private float _stampWait;
        private float _stampWait2;
        private float _stampWobble;
        private float _stampWobbleSin;
        private float _extraMouthOpen;
        private float _openWait;
        private float _sandwichEat;
        private float _eatWait;
        private float _afterEatWait;
        private bool _finishEat;
        private bool _burp;
        private float _finalWait;
        private float _coin2Wait;
        private float _intermissionSlide;
        //private float _newCrossLerp;
        private float _newCircleLerp;
        private float _taxiDrive;
        private float _genericWait;
        private bool queryVisitShop;
        private float _sandwichLerp;
        private bool _sandwichShift;
        private bool _showCard;
        private float _dayTake;
        //private bool _updateTime;
        private float _updateTimeWait;
        private bool _finned;
        private float _advanceDayWait;
        private bool _advancedDay;
        private bool _markedNewDay;
        private float _fallVel;
        //private float _afterScrollWait;
        private float _intermissionWait;
        private float _slideWait;
        private bool _unSlide;
        private bool _inTaxi;
        private int _giveMoney;
        private float _giveMoneyRise;
        private float _dayStartWait;
        private bool _popDay;
        private bool _attemptingBuy;
        private float _dayFallAway;
        private float _dayScroll;
        private float _ranRot;
        private bool _attemptingVincentClose;
        private bool _gaveToy;
        //private bool _didXPDay;
        private float _finishDayWait;
        ////private bool earlyExit;
        //private bool doubleUpdate;
        private bool doubleUpdating;
        private bool _startedLittleManLeave;
        private float _littleManStartWait;
        private bool _finishingNewStamp;
        private List<string> sayQueue = new List<string>();
        public int _eggFeedIndex;
        public List<string> _eggFeedLines = new List<string>()
    {
      "..",
      "...",
      "EGGS CANNOT\nTALK.",
      "EGGS CANNOT\nTALK..",
      "EGGS CANNOT\nTALK!",
      "I SAID, EGGS\nCANNOT TALK!",
      "LOOK AT YOURSELF,\nTRYING TO TALK\nTO AN EGG.",
      "I GUESS I'VE\nBEEN THERE.",
      "YOU KNOW, PEOPLE\nARE GONNA CALL\nYOU CRAZY..",
      "CRAZY FOR TALKING\nTO AN EGG.",
      "BUT IF IT WASN'T\nFOR DUCKS LIKE\nYOU, WELL..",
      "THIS EGG WOULD\nHAVE NOBODY\nTO TALK TO.",
      "PEOPLE DON'T\nREALLY TAKE EGGS\nSERIOUSLY.",
      "MAYBE THEY'RE\nTHINKING,",
      "WHAT WOULD AN\nEGG SAY? IT'S\nJUST AN EGG.",
      "EGGS DON'T\nKNOW MUCH OF\nANYTHING!",
      "WELL..\nTHAT MAY BE\nTRUE.",
      "I KNOW ONLY\nWHAT'S INSIDE\nMY SHELL!",
      "BUT..",
      "EGGS HAVE\nA LOT OF TIME\nTO THINK ABOUT\nTHINGS.",
      "I THINK A LOT\nABOUT HOW TO\nBE A GOOD EGG.",
      "...",
      "I KNOW THAT\nI SHOULD BE\nKIND.",
      "KIND TO\nEVERYONE, EGG\nOR OTHERWISE.",
      "AND THE KEY\nTO KINDNESS IS\nREMEMBERING:",
      "WE DON'T KNOW\nWHAT'S GOING ON\nINSIDE ANYONE\nELSE'S SHELL!",
      "...",
      "PEOPLE WHO THINK\nTHEY KNOW,",
      "THEY MIGHT THINK\nTHAT EGGS DON'T\nTALK!",
      "...",
      "WELL, THANKS FOR\nLISTENING TO\nTHIS EGG.",
      "THANKS FOR\nCARING ABOUT\nWHAT GOES ON\nIN MY SHELL.",
      "THAT'S ALL I\nHAVE TO SAY.",
      "I NEED TO\nGET BACK TO\nGROWING UP!",
      "SO LONG BUDDY!"
    };
        public bool eggTalk;
        public float _xpProgress;
        public float _dayProgress;
        private bool _gotEgg;
        private bool _littleManLeave;
        public string _overrideSlide;
        private bool _didSlide;
        private float _levelSlideWait;
        private bool _driveAway;
        private bool _attemptingGive;
        private ConstantSound _sound = new ConstantSound("chainsawIdle", multiSound: "chainsawIdleMulti");
        private List<Sprite> littleEggs = new List<Sprite>();
        private bool playedSound;
        private bool _prevShopOpen;
        private bool skipping;
        private bool skipUpdate;
        private float time;
        private int gachaNeed = 200;
        private int sandwichNeed = 500;
        private int milkNeed = 1400;
        private Vec2 littleManPos;
        private float _lastFill;
        private float _barHeat;
        //private int _lastNum;

        private static bool saidSpecial
        {
            get => MonoMain.core.saidSpecial;
            set => MonoMain.core.saidSpecial = value;
        }

        public static int gachas
        {
            get => MonoMain.core.gachas;
            set => MonoMain.core.gachas = value;
        }

        public static int rareGachas
        {
            get => MonoMain.core.rareGachas;
            set => MonoMain.core.rareGachas = value;
        }

        public static UIMenu _confirmMenu
        {
            get => MonoMain.core._confirmMenu;
            set => MonoMain.core._confirmMenu = value;
        }

        public UILevelBox(
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "")
          : base(title, xpos, ypos, wide, high, conString)
        {
            Graphics.fade = 1f;
            //UILevelBox._firstOpen = true;
            _frames.Add(new Sprite("levWindow_lev0"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev1"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev2"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev4"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev4"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev5"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev6"));
            _frames[_frames.Count - 1].CenterOrigin();
            _frames.Add(new Sprite("levWindow_lev6"));
            _frames[_frames.Count - 1].CenterOrigin();
            _barFront = new Sprite("online/barFront");
            _barFront.center = new Vec2(_barFront.w, 0f);
            _addXPBar = new Sprite("online/xpAddBar");
            _addXPBar.CenterOrigin();
            _bigFont = new BitmapFont("intermissionFont", 24, 23);
            _littleMan = new SpriteMap("littleMan", 16, 16);
            _thickBiosNum = new BitmapFont("thickBiosNum", 16, 16);
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFont");
            _chalkFont = new FancyBitmapFont("online/chalkFont");
            _xpBar = new Sprite("online/xpBar");
            _gachaBar = new Sprite("online/gachaBar");
            _sandwichBar = new Sprite("online/sandwichBar");
            _duckCoin = new SpriteMap("duckCoin", 18, 18);
            _duckCoin.CenterOrigin();
            _sandwich = new Sprite("sandwich");
            _sandwich.CenterOrigin();
            _heart = new Sprite("heart");
            _heart.CenterOrigin();
            _milk = new SpriteMap("milk", 10, 22);
            _milk.CenterOrigin();
            _taxi = new Sprite("taxi");
            _taxi.CenterOrigin();
            _circle = new SpriteMap("circle", 27, 31);
            _circle.CenterOrigin();
            _cross = new SpriteMap("scribble", 26, 21);
            _cross.CenterOrigin();
            _days = new SpriteMap("calanderDays", 27, 31);
            _days.CenterOrigin();
            _weekDays = new SpriteMap("weekDays", 27, 31);
            _weekDays.CenterOrigin();
            _sandwichCard = new SpriteMap("sandwichCard", 115, 54);
            _sandwichCard.CenterOrigin();
            _sandwichStamp = new SpriteMap("sandwichStamp", 14, 14);
            _sandwichStamp.CenterOrigin();
            _xpPoint = new Sprite("online/xpPlus");
            _xpPoint.CenterOrigin();
            _xpPointOutline = new Sprite("online/xpPlusOutline");
            _xpPointOutline.CenterOrigin();
            _talkBubble = new Sprite("talkBubble");
            _lev = new SpriteMap("levs", 27, 14);
            _egg = Profile.GetEggSprite(Profiles.experienceProfile.numLittleMen);
            _currentLevel = 0;
            _desiredLevel = 0;
            if (Profiles.experienceProfile != null)
            {
                _xpValue = Profiles.experienceProfile.xp;
                _newXPValue = _oldXPValue = _xpValue;
                if (_xpValue >= DuckNetwork.GetLevel(9999).xpRequired)
                {
                    _desiredLevel = _currentLevel = DuckNetwork.GetLevel(9999).num;
                }
                else
                {
                    while (_xpValue >= DuckNetwork.GetLevel(_desiredLevel + 1).xpRequired && _xpValue < DuckNetwork.GetLevel(9999).xpRequired)
                    {
                        ++_desiredLevel;
                        ++_currentLevel;
                    }
                }
                if (_desiredLevel >= 3)
                {
                    _gachaValue = (_xpValue - DuckNetwork.GetLevel(3).xpRequired) % gachaNeed;
                    _newGachaValue = _gachaValue;
                    _oldGachaValue = _gachaValue;
                }
                if (_desiredLevel >= 4)
                {
                    _sandwichValue = (_xpValue - DuckNetwork.GetLevel(4).xpRequired) % sandwichNeed;
                    _newSandwichValue = _sandwichValue;
                    _oldSandwichValue = _sandwichValue;
                }
                if (_desiredLevel >= 7)
                {
                    _milkValue = (_xpValue - DuckNetwork.GetLevel(7).xpRequired) % milkNeed;
                    _newMilkValue = _milkValue;
                    _oldMilkValue = _milkValue;
                }
            }
            _newGrowthLevel = Profiles.experienceProfile.littleManLevel;
            gachas = 0;
            rareGachas = 0;
            _roundsPlayed = Profiles.experienceProfile.roundsSinceXP;
            _startRoundsPlayed = _roundsPlayed;
            Profiles.experienceProfile.roundsSinceXP = 0;
            time = Profiles.experienceProfile.timeOfDay;
            _currentDay = Profiles.experienceProfile.currentDay;
            SFX.Play("pause");
            _totalXP = DuckNetwork.GetTotalXPEarned();
            _originalXP = _xpValue;
            if (Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) <= 0)
                return;
            HUD.CloseAllCorners();
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@START@SKIP XP");
        }

        public static bool menuOpen => _confirmMenu != null && _confirmMenu.open;

        public override void Close()
        {
            base.Close();
            HUD.ClearCorners();
        }

        public override void OnClose()
        {
            if (!FurniShopScreen.open && _prevShopOpen)
                Music.Play("CharacterSelect");
            DuckNetwork.finishedMatch = false;
            DuckNetwork._xpEarned.Clear();
            Profiles.experienceProfile.xp = _xpValue;
            Profiles.Save(Profiles.experienceProfile);
            UIMenu uiMenu = null;
            Graphics.fadeAdd = 0f;
            Graphics.flashAdd = 0f;
            if (Unlockables.HasPendingUnlocks())
                uiMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            if (rareGachas > 0 || gachas > 0)
            {
                UIGachaBoxNew.skipping = skipping;
                UIGachaBoxNew uiGachaBoxNew = new UIGachaBoxNew(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, rare: true, openOnClose: uiMenu);
                if (!skipping)
                    uiMenu = uiGachaBoxNew;
            }
            Global.Save();
            Profiles.Save(Profiles.experienceProfile);
            if (_gotEgg && Profiles.experienceProfile.numLittleMen > 8)
                uiMenu = new UIFuneral(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, link: new UIWillBox(UIGachaBox.GetRandomFurniture(Rarity.SuperRare, 1, 0.3f)[0], Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, link: uiMenu));
            if (uiMenu == null)
                return;
            MonoMain.pauseMenu = uiMenu;
        }

        public static int LittleManFrame(int idx, int curLev, ulong seed = 0, bool bottomBar = false)
        {
            if (seed == 0UL && Profiles.experienceProfile != null)
                seed = Profiles.experienceProfile.steamID;
            if (bottomBar && idx < Profiles.experienceProfile.numLittleMen - 6)
                curLev = 7;
            int num1 = curLev - 3;
            if (curLev < 0)
                num1 = 2 + new Random((int)((long)seed + idx)).Next(0, 2);
            Random generator = Rando.generator;
            Rando.generator = Profile.GetLongGenerator(seed);
            for (int index = 0; index < idx * 4; ++index)
                Rando.Int(100);
            int num2 = Rando.Int(5) * 20;
            int num3 = 0;
            switch (num1)
            {
                case 1:
                    num3 = Rando.Int(3);
                    break;
                case 2:
                    num3 = Rando.Int(3);
                    break;
                case 3:
                    num3 = Rando.Int(2);
                    break;
            }
            for (int index = 0; index < curLev; ++index)
                Rando.Int(10);
            if (Rando.Int(30) == 0)
                num2 = (num2 + 60) % 120;
            int num4 = num2 + num1 + num3 * 5;
            if (bottomBar && idx < Profiles.experienceProfile.numLittleMen - 6)
                num4 += (1 + Rando.Int(2)) * 5;
            Rando.generator = generator;
            return num4;
        }

        public DayType GetDay(int day)
        {
            if (day % 10 == 7 && _currentLevel >= 3)
                return DayType.PawnDay;
            if (day == 3)
                return DayType.Sandwich;
            if (day == 6)
                return DayType.ToyDay;
            if (day % 5 == 4 && day % 20 != 19 && _currentLevel >= 3)
                return DayType.Shop;
            if (day % 5 == 4 && day % 20 == 19 && _currentLevel >= 3)
                return DayType.SaleDay;
            if (day % 20 == 0 && day > 6 && _currentLevel >= 3)
                return DayType.ImportDay;
            if (day % 10 == 8 && _currentLevel >= 5)
                return DayType.PayDay;
            if (day % 40 == 3 && day > 6 && _currentLevel >= 3)
                return DayType.Special;
            if (day % 20 == 13 && _currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 20 == 1 && day > 5 && _currentLevel >= 3 || day % 20 == 10 && day > 5 && _currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 40 == 6 && _currentLevel >= 3)
                return DayType.Empty;
            if (day % 60 == 57 && day > 5 && _currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 60 == 37 && _currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 20 == 12 && _currentLevel >= 3 || day % 40 == 7 && _currentLevel >= 3 || day % 40 == 26 && _currentLevel >= 3)
                return DayType.ToyDay;
            if (day % 80 == 25 && _currentLevel >= 3)
                return DayType.Special;
            if (day % 80 == 65 && _currentLevel >= 3 || day % 40 == 16 && _currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 40 == 36 && day > 5 && _currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 20 == 2 && _currentLevel >= 3)
                return DayType.Allowance;
            Random generator = Rando.generator;
            Rando.generator = new Random(day);
            int num1 = Rando.Int(32);
            float num2 = Rando.Float(1f);
            Rando.generator = generator;
            if (num1 < 4)
            {
                switch (num1)
                {
                    case 0:
                        return DayType.FreeXP;
                    case 1:
                        return day > 6 ? DayType.Sandwich : DayType.FreeXP;
                    case 2:
                        return day < 5 ? DayType.ToyDay : DayType.Special;
                    case 3:
                        return DayType.ToyDay;
                }
            }
            return Unlockables.lockedItems.Count > 18 || Unlockables.lockedItems.Count > 14 && num2 > 0.2f || Unlockables.lockedItems.Count > 10 && num2 > 0.3f || Unlockables.lockedItems.Count > 5 && num2 > 0.5 || Unlockables.lockedItems.Count > 0 && num2 > 0.75 ? DayType.HintDay : DayType.Empty;
        }

        private bool IsVinceDay(DayType d) => d == DayType.Special || d == DayType.PawnDay || d == DayType.ImportDay || d == DayType.SaleDay || d == DayType.Shop || d == DayType.HintDay;

        public void AdvanceDay()
        {
            int day = (int)GetDay(Profiles.experienceProfile.currentDay);
        }

        public void Say(string s) => sayQueue.Add(s);

        public override void UpdateParts()
        {
            if (!FurniShopScreen.open && _prevShopOpen)
            {
                Music.Play("CharacterSelect", true, 0f);
            }
            _prevShopOpen = FurniShopScreen.open;
            InputProfile.repeat = false;
            Keyboard.repeat = false;
            base.UpdateParts();
            _sound.Update();
            currentLevel = _currentLevel;
            if (_confirmMenu != null)
            {
                _confirmMenu.DoUpdate();
            }
            while (littleEggs.Count < Math.Min(Profiles.experienceProfile.numLittleMen, 8))
            {
                littleEggs.Add(Profile.GetEggSprite(Math.Max(0, Profiles.experienceProfile.numLittleMen - 9) + littleEggs.Count, 0UL));
            }
            if ((Input.Pressed(Triggers.Select, "Any") || (!FurniShopScreen.open && Input.Pressed(Triggers.Start, "Any") && Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0)) && _finned)
            {
                if (Input.Pressed(Triggers.Start, "Any") && Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && (!skipping || _finned))
                {
                    SFX.skip = false;
                    SFX.Play("dacBang", 1f, 0f, 0f, false);
                    skipping = true;
                }
                FurniShopScreen.open = false;
                Vincent.Clear();
                Close();
                _finned = false;
                SFX.Play("resume", 1f, 0f, 0f, false);
            }
            if (queryVisitShop && _menuBool.value)
            {
                Vincent.Open(GetDay(Profiles.experienceProfile.currentDay));
                FurniShopScreen.open = true;
                queryVisitShop = false;
            }
            if (FurniShopScreen.open && !Vincent.showingDay)
            {
                if (skipping)
                {
                    FurniShopScreen.close = true;
                }
                if (FurniShopScreen.close)
                {
                    FurniShopScreen.close = false;
                    FurniShopScreen.open = false;
                    Vincent.Clear();
                    _menuBool.value = false;
                    _state = UILevelBoxState.UpdateTime;
                }
                if (FurniShopScreen.giveYoYo)
                {
                    if (!_attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        _menuBool.value = false;
                        _confirmMenu = new UIPresentBox(RoomEditor.GetFurniture("YOYO"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, -1f)
                        {
                            depth = 0.98f
                        };
                        _attemptingGive = true;
                    }
                    if (_attemptingGive && _confirmMenu != null && !_confirmMenu.open)
                    {
                        _confirmMenu = null;
                        _attemptingGive = false;
                        FurniShopScreen.giveYoYo = false;
                    }
                }
                else if (FurniShopScreen.giveVooDoo)
                {
                    if (!_attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        _menuBool.value = false;
                        _confirmMenu = new UIPresentBox(RoomEditor.GetFurniture("VOODOO VINCENT"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, -1f)
                        {
                            depth = 0.98f
                        };
                        _attemptingGive = true;
                    }
                    if (_attemptingGive && _confirmMenu != null && !_confirmMenu.open)
                    {
                        _confirmMenu = null;
                        _attemptingGive = false;
                        FurniShopScreen.giveVooDoo = false;
                    }
                }
                else if (FurniShopScreen.givePerimeterDefence)
                {
                    if (!_attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        _menuBool.value = false;
                        _confirmMenu = new UIPresentBox(RoomEditor.GetFurniture("PERIMETER DEFENCE"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, -1f)
                        {
                            depth = 0.98f
                        };
                        _attemptingGive = true;
                    }
                    if (_attemptingGive && _confirmMenu != null && !_confirmMenu.open)
                    {
                        _confirmMenu = null;
                        _attemptingGive = false;
                        FurniShopScreen.givePerimeterDefence = false;
                    }
                }
                else if (FurniShopScreen.attemptBuy != null)
                {
                    if (!_attemptingBuy)
                    {
                        _menuBool.value = false;
                        _confirmMenu = new UIMenu((Vincent.type == DayType.PawnDay) ? "SELL TO VINCENT?" : "BUY FROM VINCENT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, -1f, (Vincent.type == DayType.PawnDay) ? "@CANCEL@CANCEL @SELECT@SELECT" : "@CANCEL@CANCEL @SELECT@SELECT", null, false);
                        _confirmMenu.Add(new UIText(FurniShopScreen.attemptBuy.name, Color.Green, UIAlign.Center, 0f, null), true);
                        _confirmMenu.Add(new UIText(" ", Color.White, UIAlign.Center, 0f, null), true);
                        _confirmMenu.Add(new UIText(" ", Color.White, UIAlign.Center, 0f, null), true);
                        _confirmMenu.Add(new UIMenuItem((Vincent.type == DayType.PawnDay) ? ("SELL |WHITE|(|LIME|$" + FurniShopScreen.attemptBuy.cost.ToString() + "|WHITE|)") : ("BUY |WHITE|(|LIME|$" + FurniShopScreen.attemptBuy.cost.ToString() + "|WHITE|)"), new UIMenuActionCloseMenuSetBoolean(_confirmMenu, _menuBool), UIAlign.Center, default(Color), false), true);
                        _confirmMenu.Add(new UIMenuItem(Triggers.Cancel, new UIMenuActionCloseMenu(_confirmMenu), UIAlign.Center, Colors.MenuOption, true), true);
                        _confirmMenu.depth = 0.98f;
                        _confirmMenu.DoInitialize();
                        _confirmMenu.Close();
                        for (int i = 0; i < 10; i++)
                        {
                            _confirmMenu.DoUpdate();
                        }
                        _confirmMenu.Open();
                        _attemptingBuy = true;
                    }
                    if (_attemptingBuy && _confirmMenu != null && !_confirmMenu.open)
                    {
                        if (_menuBool.value)
                        {
                            _attemptingBuy = false;
                            _confirmMenu = null;
                            SFX.Play("ching", 1f, 0f, 0f, false);
                            if (Vincent.type == DayType.PawnDay)
                            {
                                Profiles.experienceProfile.littleManBucks += FurniShopScreen.attemptBuy.cost;
                                Profiles.experienceProfile.SetNumFurnitures(FurniShopScreen.attemptBuy.furnitureData.index, Profiles.experienceProfile.GetNumFurnitures(FurniShopScreen.attemptBuy.furnitureData.index) - 1);
                                foreach (Profile p in Profiles.all)
                                {
                                    FurniturePosition remove;
                                    do
                                    {
                                        remove = null;
                                        foreach (FurniturePosition pos in p.furniturePositions)
                                        {
                                            if (p.GetNumFurnituresPlaced(pos.id) > Profiles.experienceProfile.GetNumFurnitures(pos.id))
                                            {
                                                remove = pos;
                                                break;
                                            }
                                        }
                                        p.furniturePositions.Remove(remove);
                                    }
                                    while (remove != null);
                                }
                                Vincent.Clear();
                                Vincent.Add("|POINT|THANKS! |CONCERNED|I DON'T REGRET BUYING THIS AT ALL...");
                            }
                            else
                            {
                                Profiles.experienceProfile.littleManBucks -= FurniShopScreen.attemptBuy.cost;
                                if (FurniShopScreen.attemptBuy.furnitureData != null)
                                {
                                    Profiles.experienceProfile.SetNumFurnitures(FurniShopScreen.attemptBuy.furnitureData.index, Profiles.experienceProfile.GetNumFurnitures(FurniShopScreen.attemptBuy.furnitureData.index) + 1);
                                }
                                else if (FurniShopScreen.attemptBuy.teamData != null)
                                {
                                    Global.boughtHats.Add(FurniShopScreen.attemptBuy.teamData.name);
                                }
                            }
                            Vincent.Sold();
                            if (Vincent.products.Count == 1 && Vincent.type != DayType.PawnDay)
                            {
                                FurniShopScreen.open = false;
                                Vincent.Clear();
                                _state = UILevelBoxState.UpdateTime;
                            }
                            FurniShopScreen.attemptBuy = null;
                        }
                        else
                        {
                            _attemptingBuy = false;
                            _confirmMenu = null;
                            if (Vincent.type == DayType.PawnDay)
                            {
                                Vincent.Clear();
                                Vincent.Add("|CONCERNED|HAVING SECOND THOUGHTS ABOUT SELLING THAT, HUH?");
                            }
                            FurniShopScreen.attemptBuy = null;
                            skipUpdate = true;
                        }
                    }
                }
                else
                {
                    if (!_attemptingVincentClose && Input.Pressed(Triggers.Cancel, "Any") && !Vincent._willGiveVooDoo && !Vincent._willGiveYoYo && !Vincent._willGivePerimeterDefence)
                    {
                        _menuBool.value = false;
                        _confirmMenu = new UIMenu("LEAVE VINCENT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, -1f, "@SELECT@SELECT", null, false)
                        {
                            depth = 0.98f
                        };
                        _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_confirmMenu, _menuBool), UIAlign.Center, default(Color), false), true);
                        _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(_confirmMenu), UIAlign.Center, default(Color), true), true);
                        _confirmMenu.DoInitialize();
                        _confirmMenu.Close();
                        for (int j = 0; j < 10; j++)
                        {
                            _confirmMenu.DoUpdate();
                        }
                        _confirmMenu.Open();
                        _attemptingVincentClose = true;
                    }
                    if (_attemptingVincentClose && _confirmMenu != null && !_confirmMenu.open)
                    {
                        if (_menuBool.value)
                        {
                            FurniShopScreen.open = false;
                            Vincent.Clear();
                            _menuBool.value = false;
                            _state = UILevelBoxState.UpdateTime;
                        }
                        else
                        {
                            _attemptingVincentClose = false;
                            _confirmMenu = null;
                        }
                    }
                }
            }
            if (FurniShopScreen.open && !skipUpdate)
            {
                Vincent.Update();
            }
            skipUpdate = false;
            if ((FurniShopScreen.open && !Vincent.showingDay) || (_confirmMenu != null && _confirmMenu.open))
            {
                return;
            }
            if (!doubleUpdating && Input.Down(Triggers.Select, "Any"))
            {
                doubleUpdating = true;
                UpdateParts();
                UpdateParts();
                doubleUpdating = false;
            }
            if (Input.Pressed(Triggers.Start, "Any") && !_littleManLeave && Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && !FurniShopScreen.open)
            {
                skipping = true;
            }
            if (!doubleUpdating && skipping)
            {
                doubleUpdating = true;
                for (int k = 0; k < 200; k++)
                {
                    SFX.skip = true;
                    UpdateParts();
                    SFX.skip = false;
                    if (_littleManLeave || !skipping)
                    {
                        break;
                    }
                }
                doubleUpdating = false;
                if (_finned)
                {
                    Graphics.fadeAdd = 1f;
                    skipping = false;
                    SFX.skip = false;
                    SFX.Play("dacBang", 1f, 0f, 0f, false);
                }
            }
            if (_genericWait > 0f)
            {
                _genericWait -= Maths.IncFrameTimer();
                return;
            }
            _overrideSlide = null;
            if (_desiredLevel != _currentLevel || _newGrowthLevel != Profiles.experienceProfile.littleManLevel)
            {
                if (!_didSlide)
                {
                    _levelSlideWait += 0.08f;
                    if (_levelSlideWait >= 1f)
                    {
                        int curLev = _currentLevel;
                        if (Profiles.experienceProfile.numLittleMen > 0)
                        {
                            curLev = Profiles.experienceProfile.littleManLevel;
                        }
                        if (!_unSlide && !playedSound)
                        {
                            SFX.Play("dukget", 1f, 0f, 0f, false);
                            playedSound = true;
                        }
                        _overrideSlide = "GROWING UP";
                        if (curLev == 1)
                        {
                            _overrideSlide = "EGG GET";
                            _gotEgg = true;
                        }
                        if (curLev == 2)
                        {
                            _overrideSlide = "EGG HATCH";
                        }
                        if (curLev == 6)
                        {
                            _overrideSlide = "GROWN UP";
                        }
                        if (curLev == 7)
                        {
                            _overrideSlide = "MAX OUT!";
                        }
                        if (_unSlide)
                        {
                            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 0f, 0.45f, 1f);
                            if (_intermissionSlide <= 0.01f)
                            {
                                playedSound = false;
                                _unSlide = false;
                                _didSlide = true;
                                _intermissionSlide = 0f;
                                SFX.Play("levelUp", 1f, 0f, 0f, false);
                                return;
                            }
                        }
                        else
                        {
                            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 1f, 0.21f, 1.05f);
                            if (_intermissionSlide >= 1f)
                            {
                                _slideWait += 0.08f;
                                if (_slideWait >= 1f)
                                {
                                    _unSlide = true;
                                    _slideWait = 0f;
                                }
                            }
                        }
                    }
                    return;
                }
                _levelSlideWait = 0f;
                Graphics.fadeAdd = Lerp.Float(Graphics.fadeAdd, 1f, 0.1f);
                if (Graphics.fadeAdd < 1f)
                {
                    return;
                }
                _didSlide = false;
                _currentLevel = _desiredLevel;
                _talkLine = "";
                _feedLine = "I AM A HUNGRY\nLITTLE MAN.";
                _startFeedLine = _feedLine;
                Profiles.experienceProfile.littleManLevel = _newGrowthLevel;
                int curLev2 = _currentLevel;
                if (Profiles.experienceProfile.numLittleMen > 0)
                {
                    curLev2 = Profiles.experienceProfile.littleManLevel;
                }
                if (_currentLevel == 3)
                {
                    _oldGachaValue = _gachaValue;
                    _newGachaValue = _gachaValue + (_newXPValue - _xpValue);
                }
                if (_currentLevel == 4)
                {
                    _oldSandwichValue = _sandwichValue;
                    _newSandwichValue = _sandwichValue + (_newXPValue - _xpValue);
                }
                if (_currentLevel >= 7)
                {
                    _oldMilkValue = _milkValue;
                    _newMilkValue = _milkValue + (_newXPValue - _xpValue);
                }
                int num = _currentLevel;
                if (curLev2 >= 7)
                {
                    _littleManLeave = true;
                    HUD.CloseAllCorners();
                }
            }
            else
            {
                Graphics.fadeAdd = Lerp.Float(Graphics.fadeAdd, 0f, 0.1f);
                if (Graphics.fadeAdd > 0.01f)
                {
                    return;
                }
            }
            if (!_talking)
            {
                _talk = 0f;
            }
            else
            {
                _talkWait += 0.2f;
                if (_talkWait >= 1f)
                {
                    _talkWait = 0f;
                    if (_feedLine.Length > 0)
                    {
                        _talkLine += _feedLine[0].ToString();
                        if (_feedLine[0] == '.' || _feedLine[0] == '!' || _feedLine[0] == '?')
                        {
                            SFX.Play("tinyTick", 1f, Rando.Float(-0.1f, 0.1f), 0f, false);
                            _talkWait = -2f;
                        }
                        else
                        {
                            SFX.Play("tinyNoise1", 1f, Rando.Float(-0.1f, 0.1f), 0f, false);
                        }
                        _feedLine = _feedLine.Remove(0, 1);
                    }
                }
                _alwaysClose = false;
                if (_talking && _talkLine == _startFeedLine)
                {
                    _alwaysClose = true;
                    _finishTalkWait += 0.1f;
                    if (_finishTalkWait > 6f && !eggTalk)
                    {
                        _talking = false;
                        _talkLine = "";
                        _finishTalkWait = 0f;
                    }
                }
                if (_talkLine.Length > 0 && _talkLine[_talkLine.Length - 1] == '.')
                {
                    _alwaysClose = true;
                }
                _talk += ((!close) ? 0.2f : -0.2f);
                if (_talk > 2f)
                {
                    _talk = 2f;
                    close = true;
                }
                if (_talk < 0f)
                {
                    _talk = 0f;
                    if (!_alwaysClose)
                    {
                        close = false;
                    }
                }
            }
            if (sayQueue.Count > 0 && !_talking)
            {
                _talking = true;
                _talkLine = "";
                _feedLine = sayQueue.First();
                sayQueue.RemoveAt(0);
                _startFeedLine = _feedLine;
            }
            if (_littleManLeave)
            {
                if (skipping)
                {
                    skipping = false;
                    SFX.skip = false;
                    SFX.Play("dacBang", 1f, 0f, 0f, false);
                    Graphics.fadeAdd = 1f;
                }
                _littleManStartWait += 0.02f;
                if (_littleManStartWait >= 1f)
                {
                    if (_driveAway)
                    {
                        _sound.lerpVolume = 1f;
                        _taxiDrive = Lerp.Float(_taxiDrive, 1f, 0.03f);
                        if (_taxiDrive >= 1f)
                        {
                            SFX.Play("doorOpen", 1f, 0f, 0f, false);
                            _driveAway = false;
                            _genericWait = 0.5f;
                            _sound.volume = (_sound.lerpVolume = 0f);
                            return;
                        }
                    }
                    else if (_taxiDrive > 0f)
                    {
                        if (!_inTaxi)
                        {
                            SFX.Play("doorClose", 1f, 0f, 0f, false);
                            _inTaxi = true;
                            _genericWait = 0.5f;
                            return;
                        }
                        _sound.lerpVolume = 1f;
                        _taxiDrive = Lerp.Float(_taxiDrive, 2f, 0.03f);
                        if (_taxiDrive >= 2f)
                        {
                            _sound.lerpVolume = 0f;
                            Profiles.experienceProfile.numLittleMen++;
                            Profiles.experienceProfile.littleManLevel = 1;
                            _newGrowthLevel = Profiles.experienceProfile.littleManLevel + 1;
                            _egg = Profile.GetEggSprite(Profiles.experienceProfile.numLittleMen, 0UL);
                            _littleManStartWait = 0f;
                            _littleManLeave = false;
                            _driveAway = false;
                            _taxiDrive = 0f;
                            _inTaxi = false;
                            _startedLittleManLeave = false;
                            littleEggs.Clear();
                            return;
                        }
                    }
                    else
                    {
                        if (!_startedLittleManLeave)
                        {
                            if (Profiles.experienceProfile.numLittleMen == 0)
                            {
                                Say("I AM A FULL\nLITTLE MAN.");
                                Say("THANK YOU FOR\nRAISING ME.");
                                Say("I MUST LEAVE\nNOW.");
                                Say("I LOVE MY\nPARENT.");
                                Say("...");
                                Say("PLEASE ACCEPT\nTHIS GIFT.");
                            }
                            else
                            {
                                Say("I LOVE MY\nPARENT.");
                                Say("...");
                                Say("PLEASE ACCEPT\nTHIS GIFT.");
                            }
                            _startedLittleManLeave = true;
                            return;
                        }
                        if (!_talking)
                        {
                            Furniture specialGift;
                            if (Profiles.experienceProfile.numLittleMen == 0)
                            {
                                specialGift = RoomEditor.GetFurniture("EGG");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 1)
                            {
                                specialGift = RoomEditor.GetFurniture("PHOTO");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 2)
                            {
                                specialGift = RoomEditor.GetFurniture("PLATE");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 3)
                            {
                                specialGift = RoomEditor.GetFurniture("GIFT BASKET");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 4)
                            {
                                specialGift = RoomEditor.GetFurniture("WINE");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 5)
                            {
                                specialGift = RoomEditor.GetFurniture("JUNK");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 6)
                            {
                                specialGift = RoomEditor.GetFurniture("EASEL");
                            }
                            else if (Profiles.experienceProfile.numLittleMen == 7)
                            {
                                specialGift = RoomEditor.GetFurniture("JUKEBOX");
                            }
                            else
                            {
                                specialGift = UIGachaBox.GetRandomFurniture(Rarity.VeryRare, 1, 0.75f, false, 0, false, false)[0];
                            }
                            _confirmMenu = new UIPresentBox(specialGift, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, -1f)
                            {
                                depth = 0.98f
                            };
                            _confirmMenu.DoInitialize();
                            _confirmMenu.Open();
                            _genericWait = 0.5f;
                            _driveAway = true;
                        }
                    }
                }
                return;
            }
            _inTaxi = false;
            _stampWobbleSin += 0.8f;
            if (_showCard)
            {
                _stampCardLerp = Lerp.FloatSmooth(_stampCardLerp, _stampCard ? 1f : 0f, 0.18f, 1.05f);
            }
            foreach (LittleHeart heart in _hearts)
            {
                heart.position += heart.velocity;
                heart.alpha -= 0.02f;
            }
            _hearts.RemoveAll((LittleHeart t) => t.alpha <= 0f);
            _coinLerp2 = Lerp.Float(_coinLerp2, 1f, 0.08f);
            _stampWobble = Lerp.Float(_stampWobble, 0f, 0.08f);
            if (_stampCard || _stampCardLerp > 0.01f)
            {
                if (!_showCard)
                {
                    if (!_sandwichShift)
                    {
                        _sandwichLerp = Lerp.Float(_sandwichLerp, 1f, 0.12f);
                        if (_sandwichLerp >= 1f)
                        {
                            _sandwichShift = true;
                        }
                    }
                    else if (_burp)
                    {
                        _extraMouthOpen = Lerp.FloatSmooth(_extraMouthOpen, 0f, 0.17f, 1.05f);
                        _finalWait += 0.1f;
                        if (_finalWait >= 1f)
                        {
                            _finalWait = 0f;
                            _finishEat = false;
                            _afterEatWait = 0f;
                            _burp = false;
                            _sandwichLerp = 0f;
                            _extraMouthOpen = 0f;
                            _sandwichShift = false;
                            _eatWait = 0f;
                            _openWait = 0f;
                            _showCard = true;
                            _sandwichEat = 0f;
                            _finishingNewStamp = false;
                        }
                    }
                    else if (_finishEat)
                    {
                        _extraMouthOpen = Lerp.FloatSmooth(_extraMouthOpen, 0f, 0.17f, 1.05f);
                        _afterEatWait += 0.08f;
                        if (_afterEatWait >= 1f)
                        {
                            SFX.Play("healthyEat", 1f, 0f, 0f, false);
                            for (int l = 0; l < 8; l++)
                            {
                                _hearts.Add(new LittleHeart
                                {
                                    position = littleManPos + new Vec2(8f + Rando.Float(-4f, 4f), 8f + Rando.Float(-6f, 6f)),
                                    velocity = new Vec2(0f, Rando.Float(-0.2f, -0.4f))
                                });
                            }
                            _burp = true;
                        }
                    }
                    else
                    {
                        _sandwichLerp = Lerp.Float(_sandwichLerp, 0f, 0.12f);
                        if (_sandwichLerp <= 0f)
                        {
                            _extraMouthOpen = Lerp.FloatSmooth(_extraMouthOpen, 15f, 0.18f, 1.05f);
                            if (_extraMouthOpen >= 1f)
                            {
                                _openWait += 0.08f;
                                if (_openWait >= 1f)
                                {
                                    _sandwichEat = Lerp.Float(_sandwichEat, 1f, 0.08f);
                                    if (_sandwichEat >= 1f)
                                    {
                                        if (_eatWait == 0f)
                                        {
                                            SFX.Play("swallow", 1f, 0f, 0f, false);
                                        }
                                        _eatWait += 0.08f;
                                        if (_eatWait >= 1f)
                                        {
                                            _finishEat = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (_stampCardLerp >= 0.99f)
                {
                    _stampWait += 0.2f;
                    if (_stampWait >= 1f)
                    {
                        if (_stampWait2 == 0f)
                        {
                            Profiles.experienceProfile.numSandwiches++;
                            _finishingNewStamp = true;
                            _stampWobble = 1f;
                            SFX.Play("dacBang", 1f, 0f, 0f, false);
                        }
                        _stampWait2 += 0.06f;
                        if (_stampWait2 >= 1f)
                        {
                            if (Profiles.experienceProfile.numSandwiches > 0 && Profiles.experienceProfile.numSandwiches % 6 == 0 && _coin2Wait == 0f)
                            {
                                _coinLerp2 = 0f;
                                _coin2Wait = 1f;
                                SFX.Play("ching", 1f, 0.2f, 0f, false);
                                rareGachas++;
                            }
                            _coin2Wait -= 0.08f;
                            if (_coin2Wait <= 0f)
                            {
                                _stampCard = false;
                                _stampWait2 = 0f;
                                _stampWait = 0f;
                                _coin2Wait = 0f;
                            }
                        }
                    }
                }
                return;
            }
            _showCard = false;
            Vec2 target = new Vec2(x - 80f, y - 10f);
            if (open)
            {
                if (_currentLevel == _desiredLevel)
                {
                    if (_currentLevel > 3 && _finned && Input.Pressed(Triggers.Menu2, "Any"))
                    {
                        _talking = true;
                        _finishTalkWait = 0f;
                        _talkLine = "";
                        if (Profiles.experienceProfile.littleManLevel <= 2 && _currentLevel > 6)
                        {
                            _feedLine = "";
                            if (!Global.data.hadTalk)
                            {
                                if (_eggFeedIndex < _eggFeedLines.Count)
                                {
                                    if (_eggFeedIndex > 2)
                                    {
                                        eggTalk = true;
                                    }
                                    _feedLine = _eggFeedLines[_eggFeedIndex];
                                    _eggFeedIndex++;
                                    if (_eggFeedIndex >= _eggFeedLines.Count)
                                    {
                                        Global.data.hadTalk = true;
                                        HUD.CloseAllCorners();
                                        eggTalk = false;
                                        _finned = false;
                                    }
                                }
                                else
                                {
                                    _feedLine = "...";
                                }
                            }
                        }
                        else
                        {
                            _feedLine = "I AM A HUNGRY\nLITTLE MAN.";
                            if (Rando.Int(1000) == 1)
                            {
                                _feedLine = "I... AM A HUNGRY\nLITTLE MAN.";
                            }
                            DateTime now = MonoMain.GetLocalTime();
                            if (!saidSpecial)
                            {
                                if (now.Month == 4 && now.Day == 20)
                                {
                                    _feedLine = "HAPPY BIRTHDAY!";
                                }
                                else if (now.Month == 3 && now.Day == 9)
                                {
                                    _feedLine = "HAPPY BIRTHDAY!!";
                                }
                                else if (now.Month == 1 && now.Day == 1)
                                {
                                    _feedLine = "HAPPY NEW YEAR!";
                                }
                                else if (now.Month == 6 && now.Day == 4)
                                {
                                    _feedLine = "HAPPY BIRTHDAY\nDUCK GAME!";
                                }
                                else if (Rando.Int(190000) == 1)
                                {
                                    _feedLine = "LET'S DANCE!";
                                }
                                else if (Rando.Int(80000) == 1)
                                {
                                    _feedLine = "HAPPY BIRTHDAY!";
                                }
                                saidSpecial = true;
                            }
                            if (Rando.Int(100000) == 1)
                            {
                                _feedLine = "I AM A HANGRY\nLITTLE MAN.";
                            }
                            else if (Rando.Int(150000) == 1)
                            {
                                _feedLine = "I AM A HAPPY\nLITTLE MAN.";
                            }
                        }
                        _startFeedLine = _feedLine;
                    }
                    if (_state == UILevelBoxState.LogWinLoss)
                    {
                        if (Input.Pressed(Triggers.Select, "Any"))
                        {
                            if (Profiles.experienceProfile != null)
                            {
                                Profiles.experienceProfile.xp = _xpValue;
                            }
                            SFX.Play("rockHitGround2", 1f, 0.5f, 0f, false);
                            Close();
                        }
                    }
                    else if (_state == UILevelBoxState.Wait)
                    {
                        _startWait -= 0.09f;
                        if (_startWait < 0f)
                        {
                            _startWait = 1f;
                            _state = UILevelBoxState.ShowXPBar;
                            SFX.Play("rockHitGround2", 1f, 0.5f, 0f, false);
                        }
                    }
                    else if (_state == UILevelBoxState.UpdateTime)
                    {
                        _advancedDay = false;
                        _fallVel = 0f;
                        _finned = false;
                        //this._updateTime = false;
                        _markedNewDay = false;
                        _advanceDayWait = 0f;
                        _dayFallAway = 0f;
                        _dayScroll = 0f;
                        _newCircleLerp = 0f;
                        _popDay = false;
                        _slideWait = 0f;
                        _unSlide = false;
                        _intermissionSlide = 0f;
                        _intermissionWait = 0f;
                        _gaveToy = false;
                        if (_roundsPlayed > 0)
                        {
                            _updateTimeWait += 0.08f;
                            if (_updateTimeWait >= 1f)
                            {
                                _dayTake += 0.8f;
                                if (_dayTake >= 1f)
                                {
                                    _dayTake = 0f;
                                    _roundsPlayed--;
                                }
                                _dayProgress = 1f - _roundsPlayed / (float)_startRoundsPlayed;
                                time += 0.08f;
                            }
                            if (time >= 1f)
                            {
                                time -= 1f;
                                _state = UILevelBoxState.AdvanceDay;
                                _updateTimeWait = 0f;
                                _dayTake = 0f;
                            }
                        }
                        else
                        {
                            _state = UILevelBoxState.Finished;
                        }
                    }
                    else if (_state == UILevelBoxState.RunDay)
                    {
                        DayType t4 = GetDay(Profiles.experienceProfile.currentDay);
                        if (t4 == DayType.Allowance)
                        {
                            if (_giveMoney == 0)
                            {
                                _giveMoney = 200;
                                Profiles.experienceProfile.littleManBucks += _giveMoney;
                                SFX.Play("ching", 1f, 0f, 0f, false);
                            }
                            _giveMoneyRise = Lerp.Float(_giveMoneyRise, 1f, 0.05f);
                            _finishDayWait += 0.04f;
                            if (_finishDayWait >= 1f)
                            {
                                _giveMoneyRise = 1f;
                                _giveMoney = 0;
                                _state = UILevelBoxState.UpdateTime;
                            }
                        }
                        else if (t4 == DayType.PayDay)
                        {
                            if (_giveMoney == 0)
                            {
                                int wage = 75;
                                if (_currentLevel > 5)
                                {
                                    wage = 100;
                                }
                                if (_currentLevel > 6)
                                {
                                    wage = 125;
                                }
                                _giveMoney = wage + Profiles.experienceProfile.numLittleMen * 25;
                                Profiles.experienceProfile.littleManBucks += _giveMoney;
                                SFX.Play("ching", 1f, 0f, 0f, false);
                            }
                            _giveMoneyRise = Lerp.Float(_giveMoneyRise, 1f, 0.05f);
                            _finishDayWait += 0.04f;
                            if (_finishDayWait >= 1f)
                            {
                                _giveMoneyRise = 1f;
                                _giveMoney = 0;
                                _state = UILevelBoxState.UpdateTime;
                            }
                        }
                        else if (t4 == DayType.ToyDay)
                        {
                            if (!_gaveToy)
                            {
                                _gaveToy = true;
                                gachas++;
                                _coinLerp = 0f;
                                SFX.Play("ching", 1f, 0.2f, 0f, false);
                            }
                            _finishDayWait += 0.04f;
                            if (_finishDayWait >= 1f)
                            {
                                _state = UILevelBoxState.UpdateTime;
                            }
                        }
                        else if (t4 == DayType.Sandwich)
                        {
                            if (!_gaveToy)
                            {
                                _stampCard = true;
                                _state = UILevelBoxState.UpdateTime;
                            }
                        }
                        else if (t4 == DayType.FreeXP)
                        {
                           // this._didXPDay = true;
                            DuckNetwork.GiveXP("FREE XP DAY", 0, 75, 4, 9999999, 9999999, 9999999);
                            _state = UILevelBoxState.Wait;
                        }
                        else if (t4 == DayType.Empty)
                        {
                            _state = UILevelBoxState.UpdateTime;
                        }
                        else if (IsVinceDay(t4) && !Input.Down(Triggers.Select, "Any"))
                        {
                            _state = UILevelBoxState.UpdateTime;
                            Vincent.showingDay = false;
                            FurniShopScreen.close = true;
                        }
                    }
                    else if (_state == UILevelBoxState.AdvanceDay)
                    {
                        if (!_advancedDay)
                        {
                            if (!_popDay)
                            {
                                _popDay = true;
                                _fallVel = -0.025f;
                                _ranRot = Rando.Float(-0.3f, 0.3f);
                            }
                            if (_dayFallAway < 1f)
                            {
                                _dayFallAway += _fallVel;
                                _fallVel += 0.005f;
                            }
                            else if (_dayScroll < 1f)
                            {
                                _dayScroll = Lerp.Float(_dayScroll, 1f, 0.1f);
                            }
                            else
                            {
                                _advancedDay = true;
                                Profiles.experienceProfile.currentDay++;
                                _dayFallAway = 0f;
                                _dayScroll = 0f;
                            }
                        }
                        else
                        {
                            _advanceDayWait += 0.1f;
                            if (_advanceDayWait >= 1f)
                            {
                                if (!_markedNewDay)
                                {
                                    SFX.Play("chalk", 1f, 0f, 0f, false);
                                    _currentDay = Profiles.experienceProfile.currentDay;
                                    _markedNewDay = true;
                                    DayType t2 = GetDay(Profiles.experienceProfile.currentDay);
                                    if (IsVinceDay(t2))
                                    {
                                        skipping = false;
                                        SFX.skip = false;
                                        SFX.Play("dacBang", 1f, 0f, 0f, false);
                                        Graphics.fadeAdd = 1f;
                                    }
                                }
                                if (_markedNewDay)
                                {
                                    _intermissionWait += 0.15f;
                                    if (_intermissionWait >= 1f)
                                    {
                                        if (_unSlide)
                                        {
                                            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 0f, 0.42f, 1f);
                                            if (_intermissionSlide <= 0.02f)
                                            {
                                                _intermissionSlide = 0f;
                                                _dayStartWait += 0.11f;
                                                if (_dayStartWait >= 1f)
                                                {
                                                    AdvanceDay();
                                                    _state = UILevelBoxState.RunDay;
                                                }
                                                if (IsVinceDay(GetDay(Profiles.experienceProfile.currentDay)))
                                                {
                                                    Vincent.showingDay = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _intermissionSlide = Lerp.FloatSmooth(_intermissionSlide, 1f, 0.2f, 1.05f);
                                            if (_intermissionSlide >= 1f)
                                            {
                                                DayType t3 = GetDay(Profiles.experienceProfile.currentDay);
                                                if (IsVinceDay(t3) && !Vincent.showingDay)
                                                {
                                                    Vincent.Clear();
                                                    Vincent.showingDay = true;
                                                    Vincent.Open(t3);
                                                    FurniShopScreen.open = true;
                                                    _roundsPlayed = 0;
                                                }
                                                _slideWait += 0.11f;
                                                if (_slideWait >= 1.8f)
                                                {
                                                    _unSlide = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                _newCircleLerp = Lerp.Float(_newCircleLerp, 1f, 0.2f);
                            }
                        }
                    }
                    else if (_state == UILevelBoxState.Finished)
                    {
                        if (!_finned)
                        {
                            HUD.CloseAllCorners();
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE", null, false);
                            if (_currentLevel > 3 && (!Global.data.hadTalk || Profiles.experienceProfile.littleManLevel > 2))
                            {
                                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@TALK", null, false);
                            }
                            if (Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && (rareGachas > 0 || gachas > 0))
                            {
                                HUD.AddCornerControl(HUDCorner.BottomLeft, "@START@AUTO TOYS", null, false);
                            }
                            _finned = true;
                        }
                    }
                    else if (_state == UILevelBoxState.ShowXPBar)
                    {
                        _firstParticleIn = false;
                        _drain = 1f;
                        if (_currentStat.Key == null)
                        {
                            _currentStat = DuckNetwork.TakeXPStat();
                        }
                        if (_currentStat.Key == null)
                        {
                            if (_roundsPlayed > 0 && _currentLevel >= 4)
                            {
                                _state = UILevelBoxState.UpdateTime;
                            }
                            else
                            {
                                _state = UILevelBoxState.Finished;
                            }
                        }
                        else
                        {
                            _slideXPBar = Lerp.FloatSmooth(_slideXPBar, 1f, 0.18f, 1.1f);
                            if (_slideXPBar >= 1f)
                            {
                                _oldXPValue = _xpValue;
                                _newXPValue = _xpValue + _currentStat.Value.xp;
                                if (_currentLevel > 2)
                                {
                                    _oldGachaValue = _gachaValue;
                                    _newGachaValue = _gachaValue + _currentStat.Value.xp;
                                    if (_currentLevel > 3)
                                    {
                                        _oldSandwichValue = _sandwichValue;
                                        _newSandwichValue = _sandwichValue + _currentStat.Value.xp;
                                        if (_currentLevel >= 7)
                                        {
                                            _oldMilkValue = _milkValue;
                                            _newMilkValue = _milkValue + _currentStat.Value.xp;
                                        }
                                    }
                                }
                                _state = UILevelBoxState.WaitXPBar;
                                SFX.Play("scoreDing", 1f, 0.5f, 0f, false);
                            }
                        }
                    }
                    else if (_state == UILevelBoxState.WaitXPBar)
                    {
                        _startWait -= 0.15f;
                        if (_startWait < 0f)
                        {
                            _startWait = 1f;
                            _state = UILevelBoxState.DrainXPBar;
                        }
                    }
                    else if (_state == UILevelBoxState.DrainXPBar)
                    {
                        _drain -= 0.04f;
                        if (_drain > 0f)
                        {
                            _particleWait -= 0.4f;
                            if (_particleWait < 0f)
                            {
                                float fullYOffset = 30f;
                                if (_currentLevel == 3)
                                {
                                    fullYOffset = 25f;
                                }
                                if (_currentLevel >= 4)
                                {
                                    fullYOffset = 20f;
                                }
                                if (_currentLevel >= 4)
                                {
                                    fullYOffset = 0f;
                                }
                                if (_currentLevel >= 7)
                                {
                                    fullYOffset = -12f;
                                }
                                if (_currentStat.Value.type == 0 || _currentStat.Value.type == 4)
                                {
                                    _particles.Add(new XPPlus
                                    {
                                        position = new Vec2(x - 72f, y - 58f),
                                        velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                        target = target + new Vec2(0f, fullYOffset),
                                        color = Colors.DGGreen
                                    });
                                }
                                if (_currentLevel >= 3 && (_currentStat.Value.type == 1 || _currentStat.Value.type == 4))
                                {
                                    _particles.Add(new XPPlus
                                    {
                                        position = new Vec2(x - 72f, y - 58f),
                                        velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                        target = target + new Vec2(0f, 10f + fullYOffset),
                                        color = Colors.DGRed
                                    });
                                }
                                if (_currentLevel >= 4 && (_currentStat.Value.type == 2 || _currentStat.Value.type == 4))
                                {
                                    _particles.Add(new XPPlus
                                    {
                                        position = new Vec2(x - 72f, y - 58f),
                                        velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                        target = target + new Vec2(0f, 20f + fullYOffset),
                                        color = Colors.DGBlue
                                    });
                                }
                                _xpLost += 1f;
                                SFX.Play("tinyTick", 1f, 0f, 0f, false);
                                _particleWait = 1f;
                            }
                        }
                        if (_firstParticleIn)
                        {
                            _addLerp += 0.04f;
                            _xpValue = (int)Lerp.FloatSmooth(_oldXPValue, _newXPValue, _addLerp, 1f);
                            _gachaValue = (int)Lerp.FloatSmooth(_oldGachaValue, _newGachaValue, _addLerp, 1f);
                            _sandwichValue = (int)Lerp.FloatSmooth(_oldSandwichValue, _newSandwichValue, _addLerp, 1f);
                            _milkValue = (int)Lerp.FloatSmooth(_oldMilkValue, _newMilkValue, _addLerp, 1f);
                            _xpProgress = (_xpValue - _originalXP) / (float)_totalXP;
                        }
                        if (_drain < 0f)
                        {
                            _drain = 0f;
                        }
                        if (_drain <= 0f && _addLerp >= 1f)
                        {
                            _drain = 0f;
                            _addLerp = 0f;
                            _state = UILevelBoxState.HideXPBar;
                        }
                    }
                    else if (_state == UILevelBoxState.HideXPBar)
                    {
                        _slideXPBar = Lerp.FloatSmooth(_slideXPBar, 0f, 0.2f, 1.1f);
                        if (_slideXPBar <= 0.02f)
                        {
                            _currentStat = default(KeyValuePair<string, XPPair>);
                            _state = UILevelBoxState.ShowXPBar;
                            SFX.Play("rockHitGround2", 1f, 0.5f, 0f, false);
                            _slideXPBar = 0f;
                        }
                    }
                }
                if (_currentLevel == _desiredLevel)
                {
                    _coinLerp = Lerp.Float(_coinLerp, 1f, 0.1f);
                    foreach (XPPlus particle in _particles)
                    {
                        particle.position += particle.velocity;
                        if (!particle.splash)
                        {
                            particle.position = Lerp.Vec2Smooth(particle.position, particle.target, particle.time);
                            particle.time += 0.01f;
                        }
                        else
                        {
                            XPPlus xpplus = particle;
                            xpplus.velocity.y = xpplus.velocity.y + 0.2f;
                            particle.alpha -= 0.05f;
                        }
                    }
                }
                int c = _particles.Count;
                _particles.RemoveAll((XPPlus part) => (part.position - part.target).lengthSq < 64f);
                if (_particles.Count != c)
                {
                    _firstParticleIn = true;
                }
                if (_xpValue >= DuckNetwork.GetLevel(_desiredLevel + 1).xpRequired && _currentLevel != 20)
                {
                    _desiredLevel++;
                }
                if (_currentLevel > 2)
                {
                    if (_gachaValue >= gachaNeed)
                    {
                        _gachaValue -= gachaNeed;
                        _newGachaValue -= gachaNeed;
                        _oldGachaValue -= gachaNeed;
                        gachas++;
                        _coinLerp = 0f;
                        SFX.Play("ching", 1f, 0.2f, 0f, false);
                    }
                    if (_milkValue >= milkNeed)
                    {
                        _milkValue -= milkNeed;
                        _newMilkValue -= milkNeed;
                        _oldMilkValue -= milkNeed;
                        _newGrowthLevel = Profiles.experienceProfile.littleManLevel + 1;
                        if (_newGrowthLevel > 7)
                        {
                            _newGrowthLevel = 7;
                        }
                    }
                    if (_currentLevel > 3 && _sandwichValue >= sandwichNeed)
                    {
                        _sandwichValue -= sandwichNeed;
                        _newSandwichValue -= sandwichNeed;
                        _oldSandwichValue -= sandwichNeed;
                        _stampCard = true;
                    }
                }
            }
        }

        public override void Draw()
        {
            if (skipping)
                return;
            int num1 = 33;
            if (_currentLevel >= 3)
                num1 = 38;
            if (_currentLevel >= 4)
                num1 = 63;
            if (_currentLevel >= 7)
                num1 = 75;
            Vec2 vec2_1 = new Vec2(x, y - num1 * _slideXPBar);
            int num2 = _currentLevel;
            if (num2 > 8)
                num2 = 8;
            Sprite frame = _frames[num2 - 1];
            float num3 = 30f;
            if (_currentLevel == 3)
                num3 = 25f;
            if (_currentLevel >= 4)
                num3 = 20f;
            if (_currentLevel >= 4)
                num3 = 0f;
            if (_currentLevel >= 7)
                num3 = -12f;
            frame.depth = depth;
            Graphics.Draw(frame, x, y);
            string text1 = "@LWING@" + Profiles.experienceProfile.name + "@RWING@";
            float x1 = 0f;
            float y1 = 0f;
            Vec2 vec2_2 = new Vec2(1f, 1f);
            if (Profiles.experienceProfile.name.Length > 9)
            {
                vec2_2 = new Vec2(0.75f, 0.75f);
                y1 = 1f;
                x1 = 1f;
            }
            if (Profiles.experienceProfile.name.Length > 12)
            {
                vec2_2 = new Vec2(0.5f, 0.5f);
                y1 = 2f;
                x1 = 1f;
            }
            _font.scale = vec2_2;
            Vec2 vec2_3 = new Vec2((float)-(_font.GetWidth(text1) / 2.0), num3 - 50f);
            _font.DrawOutline(text1, position + vec2_3 + new Vec2(x1, y1), Color.White, Color.Black, depth + 2);
            _font.scale = new Vec2(1f, 1f);
            _lev.depth = depth + 2;
            _lev.frame = _currentLevel - 1;
            Graphics.Draw(_lev, x - 90f, y - 34f + num3);
            _font.DrawOutline(_currentLevel.ToString()[0].ToString() ?? "", position + new Vec2(-84f, num3 - 30f), Color.White, Color.Black, depth + 2);
            if (_currentLevel > 9)
            {
                _thickBiosNum.scale = new Vec2(0.5f, 0.5f);
                _thickBiosNum.Draw(_currentLevel.ToString()[1].ToString() ?? "", position + new Vec2(-78f, num3 - 32f), Color.White, depth + 20);
            }
            float num4 = 85f;
            if (_currentLevel == 1)
                num4 = 175f;
            if (_currentLevel == 2)
                num4 = 154f;
            if (_currentLevel == 3)
                num4 = 122f;
            if (_currentLevel >= 4)
                num4 = 94f;
            if (_currentLevel >= 7)
                num4 = 83f;
            int xpRequired1 = DuckNetwork.GetLevel(_currentLevel + 1).xpRequired;
            int num5 = 0;
            if (_currentLevel > 0)
                num5 = DuckNetwork.GetLevel(_currentLevel).xpRequired;

            //keep these casts the same or else the math goes south and returns only 0
            //-NiK0
            int num6 = (int)Math.Round((double)(xpRequired1 * (_xpValue / (float)xpRequired1)));
            int xpRequired2 = DuckNetwork.GetLevel(9999).xpRequired;
            if (num6 > xpRequired2)
                num6 = xpRequired2;
            float num7 = xpRequired1 - num5 != 0 ? (_xpValue - num5) / (float)(xpRequired1 - num5) : 1f;
            string str1 = xpRequired1.ToString();
            if (str1.Length > 5)
                str1 = str1.Substring(0, 3) + "k";
            else if (str1.Length > 4)
                str1 = str1.Substring(0, 2) + "k";
            string text2 = "|DGGREEN|" + num6.ToString() + "|WHITE|/|DGBLUE|" + str1 + "|WHITE|";
            float num8 = 0f;
            if (_currentLevel == 1)
                num8 = 94f;
            if (_currentLevel == 2)
                num8 = 70f;
            if (_currentLevel == 3)
                num8 = 38f;
            if (_currentLevel >= 4)
                num8 = 10f;
            if (_currentLevel >= 7)
                num8 = -1f;
            _fancyFont.DrawOutline(text2, position + new Vec2(num8 - 8f, num3 - 31f) - new Vec2(_fancyFont.GetWidth(text2), 0f), Colors.DGYellow, Color.Black, depth + 2);
            if (num7 < 0.0235f)
                num7 = 0.0235f;
            float num9 = num4 * num7;
            _xpBar.depth = depth + 2;
            _xpBar.xscale = 1f;
            Vec2 vec2_4 = new Vec2(x - 87f, y - 18f);
            Graphics.Draw(_xpBar, vec2_4.x, vec2_4.y + num3, new Rectangle(0f, 0f, 3f, 6f));
            _xpBar.xscale = num9 - 4f;
            Graphics.Draw(_xpBar, vec2_4.x + 3f, vec2_4.y + num3, new Rectangle(2f, 0f, 1f, 6f));
            _xpBar.depth = depth + 7;
            _xpBar.xscale = 1f;
            Graphics.Draw(_xpBar, vec2_4.x + (num9 - 2f), vec2_4.y + num3, new Rectangle(3f, 0f, 3f, 6f));
            int x2 = 0;
            _barFront.depth = depth + 10;
            if (num9 < 13.0)
                x2 = 13 - (int)num9;
            _barHeat += Math.Abs(_lastFill - num7) * 8f;
            if (_barHeat > 1.0)
                _barHeat = 1f;
            _barFront.alpha = _barHeat;
            Graphics.Draw(_barFront, vec2_4.x + num9 + x2, vec2_4.y + num3, new Rectangle(x2, 0f, _barFront.width - x2, 6f));
            _barHeat = Maths.CountDown(_barHeat, 0.04f);
            if (_currentLevel >= 3)
            {
                float num10 = _gachaValue / (float)gachaNeed;
                float num11 = 110f;
                if (_currentLevel == 3)
                    num11 = 149f;
                if (_currentLevel >= 4)
                    num11 = 122f;
                float num12 = (float)Math.Floor(num11 * num10);
                if (num12 < 2.0)
                    num12 = 2f;
                _gachaBar.depth = depth + 2;
                _gachaBar.xscale = 1f;
                Vec2 vec2_5 = new Vec2(x - 87f, y - 5f);
                Graphics.Draw(_gachaBar, vec2_5.x, vec2_5.y + num3, new Rectangle(0f, 0f, 3f, 3f));
                _gachaBar.xscale = num12 - 5f;
                Graphics.Draw(_gachaBar, vec2_5.x + 3f, vec2_5.y + num3, new Rectangle(2f, 0f, 1f, 3f));
                _gachaBar.depth = depth + 7;
                _gachaBar.xscale = 1f;
                Graphics.Draw(_gachaBar, vec2_5.x + (num12 - 2f), vec2_5.y + num3, new Rectangle(3f, 0f, 3f, 3f));
                _duckCoin.frame = 0;
                _duckCoin.alpha = (float)(1.0 - Math.Max(_coinLerp - 0.5f, 0f) * 2.0);
                _duckCoin.depth = (Depth)0.9f;
                Graphics.Draw(_duckCoin, (float)(vec2_5.x + (num11 - 2.0) + 15.0), (float)(vec2_5.y + num3 - 8.0 - _coinLerp * 18.0));
            }
            if (_currentLevel >= 4)
            {
                float num13 = _sandwichValue / (float)sandwichNeed;
                float num14 = 154f;
                float num15 = num14 * num13;
                if (num15 < 2.0)
                    num15 = 2f;
                _sandwichBar.depth = depth + 2;
                _sandwichBar.xscale = 1f;
                Vec2 vec2_6 = new Vec2(x - 87f, y + 5f);
                Graphics.Draw(_sandwichBar, vec2_6.x, vec2_6.y + num3, new Rectangle(0f, 0f, 3f, 3f));
                _sandwichBar.xscale = num15 - 5f;
                Graphics.Draw(_sandwichBar, vec2_6.x + 3f, vec2_6.y + num3, new Rectangle(2f, 0f, 1f, 3f));
                _sandwichBar.depth = depth + 7;
                _sandwichBar.xscale = 1f;
                Graphics.Draw(_sandwichBar, vec2_6.x + (num15 - 2f), vec2_6.y + num3, new Rectangle(3f, 0f, 3f, 3f));
                _sandwich.depth = (Depth)0.88f;
                float num16 = _sandwichLerp * -150f;
                float num17 = 0f;
                float val1 = 0f;
                if (_sandwichShift)
                {
                    num16 -= 20f;
                    num17 = (float)(-42.0 - _sandwichEat * 30.0);
                    val1 = -52f - num17;
                    if (_currentLevel >= 7)
                        num17 -= 10f;
                }
                float x3 = Math.Max(val1, 0f);
                if (x3 < _sandwich.width)
                    Graphics.Draw(_sandwich, (float)(vec2_6.x + (num14 - 2.0) + 12.0 + num17 + x3 + 1.0), (float)(vec2_6.y + num3 - 16.0) + num16, new Rectangle(x3, 0f, _sandwich.width - x3, _sandwich.height), (Depth)0.88f);
            }
            if (_currentStat.Key != null)
            {
                _addXPBar.depth = depth - 20;
                _addXPBar.xscale = 1f;
                Graphics.Draw(_addXPBar, vec2_1.x, vec2_1.y);
                _fancyFont.DrawOutline(_currentStat.Value.num == 0 ? _currentStat.Key : _currentStat.Value.num.ToString() + " " + _currentStat.Key, vec2_1 + new Vec2(-(_addXPBar.width / 2) + 4, -2f), Color.White, Color.Black, depth - 10);
                Vec2 p1 = vec2_1 + new Vec2(-(_addXPBar.width / 2) + 2, -7.5f);
                Graphics.DrawLine(p1, p1 + new Vec2((_addXPBar.width - 5) * _drain, 0f), Color.Lime, depth: (_addXPBar.depth + 2));
                string text3 = ((int)(_currentStat.Value.xp * _drain)).ToString() + "|DGBLUE|XP";
                _fancyFont.DrawOutline(text3, vec2_1 + new Vec2((float)(_addXPBar.width / 2 - _fancyFont.GetWidth(text3) - 4.0), -2f), Colors.DGGreen, Color.Black, depth - 10);
            }
            foreach (XPPlus particle in _particles)
            {
                int num18 = 20;
                if (particle.splash)
                    num18 = 40;
                float num19 = Math.Min((particle.position - particle.target).length, 30f) / 30f;
                _xpPoint.scale = new Vec2(num19);
                _xpPointOutline.scale = new Vec2(num19);
                _xpPoint.color = particle.color;
                _xpPoint.alpha = particle.alpha * num19;
                _xpPoint.depth = depth + num18;
                Graphics.Draw(_xpPoint, particle.position.x, particle.position.y);
                _xpPointOutline.alpha = particle.alpha * num19;
                _xpPointOutline.depth = depth + (num18 - 5);
                Graphics.Draw(_xpPointOutline, particle.position.x, particle.position.y);
            }
            foreach (LittleHeart heart in _hearts)
            {
                _heart.alpha = heart.alpha;
                _heart.depth = (Depth)0.98f;
                _heart.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(_heart, heart.position.x, heart.position.y);
            }
            int curLev = _currentLevel;
            if (Profiles.experienceProfile.numLittleMen > 0)
                curLev = Profiles.experienceProfile.littleManLevel;
            if (curLev > 7)
                curLev = 7;
            if (_currentLevel >= 2)
            {
                float num20 = (float)Math.Round(_talk) + _extraMouthOpen;
                int num21 = 0;
                if (curLev <= 4)
                    num21 = 1;
                if (curLev <= 3)
                    num21 = 2;
                if (curLev <= 2)
                {
                    if (curLev == 2)
                    {
                        _egg.depth = (Depth)0.85f;
                        _egg.yscale = 1f;
                        int num22 = 8;
                        Vec2 vec2_7 = new Vec2(x + num8, y - 29f + num3 + num22 + num21);
                        Graphics.Draw(_egg, vec2_7.x, vec2_7.y, new Rectangle(0f, num22 + num21, 16f, 16 - num22 - num21));
                        Graphics.Draw(_egg, x + num8, y - 29f + num3 - num20, new Rectangle(0f, 0f, 16f, num22 + num21));
                        Vec2 center = _egg.center;
                        _egg.yscale = num20;
                        _egg.center = center;
                    }
                }
                else
                {
                    _littleMan.frame = LittleManFrame(Profiles.experienceProfile.numLittleMen, curLev);
                    _littleMan.depth = (Depth)0.85f;
                    _littleMan.yscale = 1f;
                    littleManPos = new Vec2(x + num8, (float)(y - 29.0 + num3 + 4.0) + num21);
                    if (!_inTaxi)
                    {
                        Graphics.Draw(_littleMan, littleManPos.x, littleManPos.y, new Rectangle(0f, 4 + num21, 16f, 12 - num21));
                        Graphics.Draw(_littleMan, x + num8, y - 29f + num3 - num20, new Rectangle(0f, 0f, 16f, 4 + num21));
                        Vec2 center = _littleMan.center;
                        _littleMan.yscale = num20;
                        Graphics.Draw(_littleMan, x + num8, (float)(y - 29.0 + (num3 - num20) + 4.0) + num21, new Rectangle(0f, 4 + num21, 16f, 1f));
                        _littleMan.center = center;
                    }
                }
                _talkBubble.depth = (Depth)0.9f;
                string talkLine = _talkLine;
                if (_talkLine.Length > 0)
                {
                    Vec2 vec2_8 = new Vec2((float)(x + num8 + 16.0), y - 28f + num3);
                    _talkBubble.xscale = 1f;
                    Graphics.Draw(_talkBubble, vec2_8.x, vec2_8.y, new Rectangle(0f, 0f, 8f, 8f));
                    float num23 = Graphics.GetStringWidth(talkLine) - 5f;
                    float y2 = Graphics.GetStringHeight(talkLine) + 2f;
                    _talkBubble.xscale = num23;
                    Graphics.Draw(_talkBubble, vec2_8.x + 8f, vec2_8.y, new Rectangle(5f, 0f, 1f, 2f));
                    Graphics.Draw(_talkBubble, vec2_8.x + 8f, vec2_8.y + y2, new Rectangle(5f, 10f, 1f, 2f));
                    _talkBubble.xscale = 1f;
                    Graphics.Draw(_talkBubble, vec2_8.x, vec2_8.y + (y2 - 2f), new Rectangle(0f, 8f, 8f, 4f));
                    Graphics.Draw(_talkBubble, (float)(vec2_8.x + num23 + 8.0), vec2_8.y + (y2 - 2f), new Rectangle(8f, 8f, 4f, 4f));
                    Graphics.Draw(_talkBubble, (float)(vec2_8.x + num23 + 8.0), vec2_8.y, new Rectangle(8f, 0f, 4f, 4f));
                    Graphics.DrawRect(vec2_8 + new Vec2(5f, 2f), vec2_8 + new Vec2(num23 + 11f, y2), Color.White, (Depth)0.9f);
                    Graphics.DrawLine(vec2_8 + new Vec2(4.5f, 5f), vec2_8 + new Vec2(4.5f, y2 - 1f), Color.Black, depth: ((Depth)0.9f));
                    Graphics.DrawLine(vec2_8 + new Vec2(11.5f + num23, 4f), vec2_8 + new Vec2(11.5f + num23, y2 - 1f), Color.Black, depth: ((Depth)0.9f));
                    Graphics.DrawString(talkLine, vec2_8 + new Vec2(6f, 2f), Color.Black, (Depth)0.95f);
                }
            }
            if (_stampCardLerp > 0.01f)
            {
                float num24 = (float)(-((1.0 - _stampCardLerp) * 200.0) + Math.Sin(_stampWobbleSin) * _stampWobble * 4.0);
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f * _stampCardLerp, (Depth)0.96f);
                Graphics.Draw(_sandwichCard, x, y + num24, (Depth)0.97f);
                Random generator = Rando.generator;
                Random random = new Random(365023);
                Rando.generator = random;
                int num25 = Profiles.experienceProfile.numSandwiches % 6;
                if (Profiles.experienceProfile.numSandwiches > 0 && Profiles.experienceProfile.numSandwiches % 6 == 0 && _finishingNewStamp)
                    num25 = 6;
                for (int index = 0; index < num25; ++index)
                {
                    float num26 = index % 2 * 16;
                    float num27 = index / 2 * 16;
                    _sandwichStamp.angle = Rando.Float(-0.2f, 0.2f);
                    _sandwichStamp.frame = Rando.Int(3);
                    Graphics.Draw(_sandwichStamp, x + 30f + num26 + Rando.Float(-2f, 2f), y - 15f + num27 + Rando.Float(-2f, 2f) + num24, (Depth)0.98f);
                    if (index == 5)
                    {
                        _duckCoin.frame = 1;
                        _duckCoin.alpha = (float)(1.0 - Math.Max(_coinLerp2 - 0.5f, 0f) * 2.0);
                        _duckCoin.depth = (Depth)0.99f;
                        Graphics.Draw(_duckCoin, x + 30f + num26, (float)(y - 15.0 + num27 + num24 - _coinLerp2 * 18.0));
                    }
                }
                Rando.generator = random;
            }
            if (_currentLevel >= 7)
            {
                _milk.depth = (Depth)0.7f;
                _milk.frame = (int)(_milkValue / milkNeed * 15.0);
                Graphics.Draw(_milk, x + 26f, y - 33f);
                Vec2 vec2_9 = position + new Vec2(-88f, 44f);
                int num28 = 0;
                foreach (Sprite littleEgg in littleEggs)
                {
                    littleEgg.depth = (Depth)0.85f;
                    Graphics.Draw(littleEgg, (float)(vec2_9.x + (num28 * 23) - 3.0), vec2_9.y - 3f);
                    _littleMan.frame = LittleManFrame(Math.Max(Profiles.experienceProfile.numLittleMen - 8, 0) + num28, -1, bottomBar: true);
                    _littleMan.depth = (Depth)0.9f;
                    _littleMan.yscale = 1f;
                    Graphics.Draw(_littleMan, (float)(vec2_9.x + (num28 * 23) + 3.0), vec2_9.y + 1f);
                    ++num28;
                }
            }
            float num29 = 0f;
            if (_currentLevel >= 7)
                num29 = -12f;
            Vec2 clockPos = position + new Vec2(75.5f, 33f + num29);
            Vec2 clockPos2 = clockPos + new Vec2(0f, -7f);
            if (_currentLevel >= 4)
            {
                int littleManBucks = Profiles.experienceProfile.littleManBucks;
                string str2 = "|DGGREEN|$";
                string text4 = littleManBucks <= 9999 ? str2 + littleManBucks.ToString() : str2 + (littleManBucks / 1000).ToString() + "K";
                Graphics.DrawRect(clockPos + new Vec2(-16f, 9f), clockPos + new Vec2(15f, 18f), Color.Black, (Depth)0.89f);
                _fancyFont.Draw(text4, clockPos + new Vec2(-16f, 9f) + new Vec2(30f - _fancyFont.GetWidth(text4), 0f), Color.White, (Depth)0.9f);
                if (_giveMoney > 0 && _giveMoneyRise < 0.95f)
                {
                    string text5 = "+" + _giveMoney.ToString();
                    Color dgGreen = Colors.DGGreen;
                    Color black = Color.Black;
                    _fancyFont.DrawOutline(text5, clockPos + new Vec2(-16f, 9f) + new Vec2(30f - _fancyFont.GetWidth(text5), (float)-(10.0 + _giveMoneyRise * 10.0)), dgGreen, black, (Depth)0.97f);
                }
                Vec2 minuteHand = new Vec2
                {
                    x = -(float)Math.Sin(((time * 12) * ((float)Math.PI * 2)) - (float)Math.PI) * 8,
                    y = (float)Math.Cos(((time * 12) * ((float)Math.PI * 2)) - (float)Math.PI) * 8
                };

                Vec2 hourHand = new Vec2
                {
                    x = -(float)Math.Sin((time * ((float)Math.PI * 2)) - (float)Math.PI) * 5,
                    y = (float)Math.Cos((time * ((float)Math.PI * 2)) - (float)Math.PI) * 5
                };

                Graphics.DrawLine(clockPos2, clockPos2 + minuteHand, Color.Black, depth: ((Depth)0.9f));
                Graphics.DrawLine(clockPos2, clockPos2 + hourHand, Color.Black, 1.5f, (Depth)0.9f);
                Random random = new Random(0);
                Random generator = Rando.generator;
                Rando.generator = random;
                for (int index = 0; index < Profiles.experienceProfile.currentDay; ++index)
                {
                    double num30 = Rando.Float(1f);
                }
                Math.Floor(Profiles.experienceProfile.currentDay / 5.0);
                for (int index = 0; index < 5; ++index)
                {
                    float num31 = 0f;
                    if (index == 0)
                        num31 += 0.1f;
                    float num32 = Rando.Float(-0.1f, 0.1f);
                    int num33 = (int)((num32 + 0.1f) / 0.2f * 10.0);
                    if (_popDay && index == 0 && _dayFallAway != 0.0)
                        _weekDays.angle = _ranRot;
                    else if (_currentLevel < 6)
                        _weekDays.angle = num32;
                    else
                        _weekDays.angle = 0f;
                    if (num33 == 3 && _currentLevel < 5)
                        _weekDays.angle += 3.141593f;
                    float num34 = 0f;
                    if (index == 0)
                        num34 = _dayFallAway * 100f;
                    float num35 = (float)(-_dayScroll * 26.0);
                    if (index == 0)
                    {
                        _circle.depth = (Depth)(0.85f + num31);
                        _circle.angle = _weekDays.angle;
                        if (index == 0 && _advancedDay)
                            Graphics.Draw(_circle, position.x - 71f + index * 28 + num35, position.y + 33f + num29 + num34, new Rectangle(0f, 0f, _circle.width * _newCircleLerp, _circle.height));
                        else
                            Graphics.Draw(_circle, position.x - 71f + index * 28 + num35, position.y + 33f + num29 + num34);
                    }
                    _weekDays.depth = (Depth)(0.83f + num31);
                    _weekDays.frame = (Profiles.experienceProfile.currentDay + index) % 5;
                    _weekDays.frame += (int)Math.Floor((Profiles.experienceProfile.currentDay + index) / 20.0) % 4 * 6;
                    Graphics.Draw(_weekDays, position.x - 71f + index * 28 + num35, position.y + 33f + num34 + num29);
                    DayType day = GetDay(Profiles.experienceProfile.currentDay + index);
                    if (day != DayType.Empty)
                    {
                        _days.depth = (Depth)(0.84f + num31);
                        _days.frame = (int)day;
                        _days.angle = _weekDays.angle;
                        Graphics.Draw(_days, position.x - 71f + index * 28 + num35, (float)(position.y + num29 + 33.0) + num34);
                    }
                }
                Rando.generator = generator;
            }
            if (_confirmMenu != null && _confirmMenu.open)
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f, (Depth)(275f * (float)Math.PI / 887f));
            if (FurniShopScreen.open)
            {
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f, (Depth)0.95f);
                FurniShopScreen.open = true;
                Vincent.Draw();
            }
            if (_taxiDrive > 0.0)
            {
                Vec2 vec2_13 = new Vec2((float)(position.x - 200.0 + _taxiDrive * 210.0), position.y - 33f);
                _taxi.depth = (Depth)0.97f;
                Graphics.Draw(_taxi, vec2_13.x, vec2_13.y);
                if (_inTaxi)
                {
                    _littleMan.frame = LittleManFrame(Profiles.experienceProfile.numLittleMen, curLev);
                    Graphics.Draw(_littleMan, vec2_13.x - 16f, vec2_13.y - 8f, new Rectangle(0f, 0f, 16f, 6f));
                }
            }
            if (_intermissionSlide > 0.01f)
            {
                float x4 = (float)(_intermissionSlide * 320.0 - 320.0);
                float y3 = 60f;
                Graphics.DrawRect(new Vec2(x4, y3), new Vec2(x4 + 320f, y3 + 30f), Color.Black, (Depth)0.98f);
                float x5 = (float)(320.0 - _intermissionSlide * 320.0);
                float num36 = 60f;
                Graphics.DrawRect(new Vec2(x5, num36 + 30f), new Vec2(x5 + 320f, num36 + 60f), Color.Black, (Depth)0.98f);
                string text6 = "ADVANCE DAY";
                switch (GetDay(Profiles.experienceProfile.currentDay))
                {
                    case DayType.Sandwich:
                        text6 = "SANDWICH DAY";
                        break;
                    case DayType.FreeXP:
                        text6 = "TRAINING DAY";
                        break;
                    case DayType.Shop:
                        text6 = "VINCENT";
                        break;
                    case DayType.ToyDay:
                        text6 = "FREE TOY";
                        break;
                    case DayType.PayDay:
                        text6 = "PAY DAY";
                        break;
                    case DayType.Special:
                        text6 = "VINCENT";
                        break;
                    case DayType.Allowance:
                        text6 = "ALLOWANCE";
                        break;
                    case DayType.SaleDay:
                        text6 = "SUPER SALE";
                        break;
                    case DayType.ImportDay:
                        text6 = "FANCY IMPORTS";
                        break;
                    case DayType.PawnDay:
                        text6 = "VINCENT";
                        break;
                    case DayType.HintDay:
                        text6 = "RUMOURS";
                        break;
                }
                if (_overrideSlide != null)
                    text6 = _overrideSlide;
                _bigFont.Draw(text6, new Vec2((float)(_intermissionSlide * (320.0 + Layer.HUD.width / 2.0 - _bigFont.GetWidth(text6) / 2.0) - 320.0), num36 + 18f), Color.White, (Depth)0.99f);
            }
            _lastFill = num7;
            if (_confirmMenu == null)
                return;
            _confirmMenu.DoDraw();
        }
    }
}
