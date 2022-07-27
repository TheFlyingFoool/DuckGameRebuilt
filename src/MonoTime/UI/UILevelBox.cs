// Decompiled with JetBrains decompiler
// Type: DuckGame.UILevelBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private List<Tex2D> _eggs = new List<Tex2D>();
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
            this._frames.Add(new Sprite("levWindow_lev0"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev1"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev2"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev4"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev4"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev5"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev6"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._frames.Add(new Sprite("levWindow_lev6"));
            this._frames[this._frames.Count - 1].CenterOrigin();
            this._barFront = new Sprite("online/barFront");
            this._barFront.center = new Vec2((float)this._barFront.w, 0.0f);
            this._addXPBar = new Sprite("online/xpAddBar");
            this._addXPBar.CenterOrigin();
            this._bigFont = new BitmapFont("intermissionFont", 24, 23);
            this._littleMan = new SpriteMap("littleMan", 16, 16);
            this._thickBiosNum = new BitmapFont("thickBiosNum", 16, 16);
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFont");
            this._chalkFont = new FancyBitmapFont("online/chalkFont");
            this._xpBar = new Sprite("online/xpBar");
            this._gachaBar = new Sprite("online/gachaBar");
            this._sandwichBar = new Sprite("online/sandwichBar");
            this._duckCoin = new SpriteMap("duckCoin", 18, 18);
            this._duckCoin.CenterOrigin();
            this._sandwich = new Sprite("sandwich");
            this._sandwich.CenterOrigin();
            this._heart = new Sprite("heart");
            this._heart.CenterOrigin();
            this._milk = new SpriteMap("milk", 10, 22);
            this._milk.CenterOrigin();
            this._taxi = new Sprite("taxi");
            this._taxi.CenterOrigin();
            this._circle = new SpriteMap("circle", 27, 31);
            this._circle.CenterOrigin();
            this._cross = new SpriteMap("scribble", 26, 21);
            this._cross.CenterOrigin();
            this._days = new SpriteMap("calanderDays", 27, 31);
            this._days.CenterOrigin();
            this._weekDays = new SpriteMap("weekDays", 27, 31);
            this._weekDays.CenterOrigin();
            this._sandwichCard = new SpriteMap("sandwichCard", 115, 54);
            this._sandwichCard.CenterOrigin();
            this._sandwichStamp = new SpriteMap("sandwichStamp", 14, 14);
            this._sandwichStamp.CenterOrigin();
            this._xpPoint = new Sprite("online/xpPlus");
            this._xpPoint.CenterOrigin();
            this._xpPointOutline = new Sprite("online/xpPlusOutline");
            this._xpPointOutline.CenterOrigin();
            this._talkBubble = new Sprite("talkBubble");
            this._lev = new SpriteMap("levs", 27, 14);
            this._egg = Profile.GetEggSprite(Profiles.experienceProfile.numLittleMen);
            this._currentLevel = 0;
            this._desiredLevel = 0;
            if (Profiles.experienceProfile != null)
            {
                this._xpValue = Profiles.experienceProfile.xp;
                this._newXPValue = this._oldXPValue = (float)this._xpValue;
                if (this._xpValue >= DuckNetwork.GetLevel(9999).xpRequired)
                {
                    this._desiredLevel = this._currentLevel = DuckNetwork.GetLevel(9999).num;
                }
                else
                {
                    while (this._xpValue >= DuckNetwork.GetLevel(this._desiredLevel + 1).xpRequired && this._xpValue < DuckNetwork.GetLevel(9999).xpRequired)
                    {
                        ++this._desiredLevel;
                        ++this._currentLevel;
                    }
                }
                if (this._desiredLevel >= 3)
                {
                    this._gachaValue = (this._xpValue - DuckNetwork.GetLevel(3).xpRequired) % this.gachaNeed;
                    this._newGachaValue = (float)this._gachaValue;
                    this._oldGachaValue = (float)this._gachaValue;
                }
                if (this._desiredLevel >= 4)
                {
                    this._sandwichValue = (this._xpValue - DuckNetwork.GetLevel(4).xpRequired) % this.sandwichNeed;
                    this._newSandwichValue = (float)this._sandwichValue;
                    this._oldSandwichValue = (float)this._sandwichValue;
                }
                if (this._desiredLevel >= 7)
                {
                    this._milkValue = (this._xpValue - DuckNetwork.GetLevel(7).xpRequired) % this.milkNeed;
                    this._newMilkValue = (float)this._milkValue;
                    this._oldMilkValue = (float)this._milkValue;
                }
            }
            this._newGrowthLevel = Profiles.experienceProfile.littleManLevel;
            UILevelBox.gachas = 0;
            UILevelBox.rareGachas = 0;
            this._roundsPlayed = Profiles.experienceProfile.roundsSinceXP;
            this._startRoundsPlayed = this._roundsPlayed;
            Profiles.experienceProfile.roundsSinceXP = 0;
            this.time = Profiles.experienceProfile.timeOfDay;
            this._currentDay = Profiles.experienceProfile.currentDay;
            SFX.Play("pause");
            this._totalXP = DuckNetwork.GetTotalXPEarned();
            this._originalXP = this._xpValue;
            if (Profiles.experienceProfile.GetNumFurnituresPlaced((int)RoomEditor.GetFurniture("VOODOO VINCENT").index) <= 0)
                return;
            HUD.CloseAllCorners();
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@START@SKIP XP");
        }

        public static bool menuOpen => UILevelBox._confirmMenu != null && UILevelBox._confirmMenu.open;

        public override void Close()
        {
            base.Close();
            HUD.ClearCorners();
        }

        public override void OnClose()
        {
            if (!FurniShopScreen.open && this._prevShopOpen)
                Music.Play("CharacterSelect");
            DuckNetwork.finishedMatch = false;
            DuckNetwork._xpEarned.Clear();
            Profiles.experienceProfile.xp = this._xpValue;
            Profiles.Save(Profiles.experienceProfile);
            UIMenu uiMenu = (UIMenu)null;
            Graphics.fadeAdd = 0.0f;
            Graphics.flashAdd = 0.0f;
            if (Unlockables.HasPendingUnlocks())
                uiMenu = (UIMenu)new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            if (UILevelBox.rareGachas > 0 || UILevelBox.gachas > 0)
            {
                UIGachaBoxNew.skipping = this.skipping;
                UIGachaBoxNew uiGachaBoxNew = new UIGachaBoxNew(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, rare: true, openOnClose: uiMenu);
                if (!this.skipping)
                    uiMenu = (UIMenu)uiGachaBoxNew;
            }
            Global.Save();
            Profiles.Save(Profiles.experienceProfile);
            if (this._gotEgg && Profiles.experienceProfile.numLittleMen > 8)
                uiMenu = (UIMenu)new UIFuneral(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, link: ((UIMenu)new UIWillBox(UIGachaBox.GetRandomFurniture(Rarity.SuperRare, 1, 0.3f)[0], Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f, link: uiMenu)));
            if (uiMenu == null)
                return;
            MonoMain.pauseMenu = (UIComponent)uiMenu;
        }

        public static int LittleManFrame(int idx, int curLev, ulong seed = 0, bool bottomBar = false)
        {
            if (seed == 0UL && Profiles.experienceProfile != null)
                seed = Profiles.experienceProfile.steamID;
            if (bottomBar && idx < Profiles.experienceProfile.numLittleMen - 6)
                curLev = 7;
            int num1 = curLev - 3;
            if (curLev < 0)
                num1 = 2 + new Random((int)((long)seed + (long)idx)).Next(0, 2);
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
            if (day % 10 == 7 && this._currentLevel >= 3)
                return DayType.PawnDay;
            if (day == 3)
                return DayType.Sandwich;
            if (day == 6)
                return DayType.ToyDay;
            if (day % 5 == 4 && day % 20 != 19 && this._currentLevel >= 3)
                return DayType.Shop;
            if (day % 5 == 4 && day % 20 == 19 && this._currentLevel >= 3)
                return DayType.SaleDay;
            if (day % 20 == 0 && day > 6 && this._currentLevel >= 3)
                return DayType.ImportDay;
            if (day % 10 == 8 && this._currentLevel >= 5)
                return DayType.PayDay;
            if (day % 40 == 3 && day > 6 && this._currentLevel >= 3)
                return DayType.Special;
            if (day % 20 == 13 && this._currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 20 == 1 && day > 5 && this._currentLevel >= 3 || day % 20 == 10 && day > 5 && this._currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 40 == 6 && this._currentLevel >= 3)
                return DayType.Empty;
            if (day % 60 == 57 && day > 5 && this._currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 60 == 37 && this._currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 20 == 12 && this._currentLevel >= 3 || day % 40 == 7 && this._currentLevel >= 3 || day % 40 == 26 && this._currentLevel >= 3)
                return DayType.ToyDay;
            if (day % 80 == 25 && this._currentLevel >= 3)
                return DayType.Special;
            if (day % 80 == 65 && this._currentLevel >= 3 || day % 40 == 16 && this._currentLevel >= 3)
                return DayType.FreeXP;
            if (day % 40 == 36 && day > 5 && this._currentLevel >= 3)
                return DayType.Sandwich;
            if (day % 20 == 2 && this._currentLevel >= 3)
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
            return Unlockables.lockedItems.Count > 18 || Unlockables.lockedItems.Count > 14 && (double)num2 > 0.200000002980232 || Unlockables.lockedItems.Count > 10 && (double)num2 > 0.300000011920929 || Unlockables.lockedItems.Count > 5 && (double)num2 > 0.5 || Unlockables.lockedItems.Count > 0 && (double)num2 > 0.75 ? DayType.HintDay : DayType.Empty;
        }

        private bool IsVinceDay(DayType d) => d == DayType.Special || d == DayType.PawnDay || d == DayType.ImportDay || d == DayType.SaleDay || d == DayType.Shop || d == DayType.HintDay;

        public void AdvanceDay()
        {
            int day = (int)this.GetDay(Profiles.experienceProfile.currentDay);
        }

        public void Say(string s) => this.sayQueue.Add(s);

        public override void UpdateParts()
        {
            if (!FurniShopScreen.open && this._prevShopOpen)
                Music.Play("CharacterSelect");
            this._prevShopOpen = FurniShopScreen.open;
            InputProfile.repeat = false;
            Keyboard.repeat = false;
            base.UpdateParts();
            this._sound.Update();
            UILevelBox.currentLevel = this._currentLevel;
            if (UILevelBox._confirmMenu != null)
                UILevelBox._confirmMenu.DoUpdate();
            while (this.littleEggs.Count < Math.Min(Profiles.experienceProfile.numLittleMen, 8))
                this.littleEggs.Add(Profile.GetEggSprite(Math.Max(0, Profiles.experienceProfile.numLittleMen - 9) + this.littleEggs.Count));
            if ((Input.Pressed("SELECT") || !FurniShopScreen.open && Input.Pressed("START") && Profiles.experienceProfile.GetNumFurnituresPlaced((int)RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0) && this._finned)
            {
                if (Input.Pressed("START") && Profiles.experienceProfile.GetNumFurnituresPlaced((int)RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && (!this.skipping || this._finned))
                {
                    SFX.skip = false;
                    SFX.Play("dacBang");
                    this.skipping = true;
                }
                FurniShopScreen.open = false;
                Vincent.Clear();
                this.Close();
                this._finned = false;
                SFX.Play("resume");
            }
            if (this.queryVisitShop && this._menuBool.value)
            {
                Vincent.Open(this.GetDay(Profiles.experienceProfile.currentDay));
                FurniShopScreen.open = true;
                this.queryVisitShop = false;
            }
            if (FurniShopScreen.open && !Vincent.showingDay)
            {
                if (this.skipping)
                    FurniShopScreen.close = true;
                if (FurniShopScreen.close)
                {
                    FurniShopScreen.close = false;
                    FurniShopScreen.open = false;
                    Vincent.Clear();
                    this._menuBool.value = false;
                    this._state = UILevelBoxState.UpdateTime;
                }
                if (FurniShopScreen.giveYoYo)
                {
                    if (!this._attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        this._menuBool.value = false;
                        UILevelBox._confirmMenu = (UIMenu)new UIPresentBox(RoomEditor.GetFurniture("YOYO"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        this._attemptingGive = true;
                    }
                    if (this._attemptingGive && UILevelBox._confirmMenu != null && !UILevelBox._confirmMenu.open)
                    {
                        UILevelBox._confirmMenu = (UIMenu)null;
                        this._attemptingGive = false;
                        FurniShopScreen.giveYoYo = false;
                    }
                }
                else if (FurniShopScreen.giveVooDoo)
                {
                    if (!this._attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        this._menuBool.value = false;
                        UILevelBox._confirmMenu = (UIMenu)new UIPresentBox(RoomEditor.GetFurniture("VOODOO VINCENT"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        this._attemptingGive = true;
                    }
                    if (this._attemptingGive && UILevelBox._confirmMenu != null && !UILevelBox._confirmMenu.open)
                    {
                        UILevelBox._confirmMenu = (UIMenu)null;
                        this._attemptingGive = false;
                        FurniShopScreen.giveVooDoo = false;
                    }
                }
                else if (FurniShopScreen.givePerimeterDefence)
                {
                    if (!this._attemptingGive)
                    {
                        HUD.CloseAllCorners();
                        this._menuBool.value = false;
                        UILevelBox._confirmMenu = (UIMenu)new UIPresentBox(RoomEditor.GetFurniture("PERIMETER DEFENCE"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        this._attemptingGive = true;
                    }
                    if (this._attemptingGive && UILevelBox._confirmMenu != null && !UILevelBox._confirmMenu.open)
                    {
                        UILevelBox._confirmMenu = (UIMenu)null;
                        this._attemptingGive = false;
                        FurniShopScreen.givePerimeterDefence = false;
                    }
                }
                else if (FurniShopScreen.attemptBuy != null)
                {
                    if (!this._attemptingBuy)
                    {
                        this._menuBool.value = false;
                        UILevelBox._confirmMenu = new UIMenu(Vincent.type == DayType.PawnDay ? "SELL TO VINCENT?" : "BUY FROM VINCENT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: (Vincent.type == DayType.PawnDay ? "@CANCEL@CANCEL @SELECT@SELECT" : "@CANCEL@CANCEL @SELECT@SELECT"));
                        UILevelBox._confirmMenu.Add((UIComponent)new UIText(FurniShopScreen.attemptBuy.name, Color.Green), true);
                        UILevelBox._confirmMenu.Add((UIComponent)new UIText(" ", Color.White), true);
                        UILevelBox._confirmMenu.Add((UIComponent)new UIText(" ", Color.White), true);
                        UILevelBox._confirmMenu.Add((UIComponent)new UIMenuItem(Vincent.type == DayType.PawnDay ? "SELL |WHITE|(|LIME|$" + FurniShopScreen.attemptBuy.cost.ToString() + "|WHITE|)" : "BUY |WHITE|(|LIME|$" + FurniShopScreen.attemptBuy.cost.ToString() + "|WHITE|)", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean((UIComponent)UILevelBox._confirmMenu, this._menuBool)), true);
                        UILevelBox._confirmMenu.Add((UIComponent)new UIMenuItem("CANCEL", (UIMenuAction)new UIMenuActionCloseMenu((UIComponent)UILevelBox._confirmMenu), c: Colors.MenuOption, backButton: true), true);
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        UILevelBox._confirmMenu.DoInitialize();
                        UILevelBox._confirmMenu.Close();
                        for (int index = 0; index < 10; ++index)
                            UILevelBox._confirmMenu.DoUpdate();
                        UILevelBox._confirmMenu.Open();
                        this._attemptingBuy = true;
                    }
                    if (this._attemptingBuy && UILevelBox._confirmMenu != null && !UILevelBox._confirmMenu.open)
                    {
                        if (this._menuBool.value)
                        {
                            this._attemptingBuy = false;
                            UILevelBox._confirmMenu = (UIMenu)null;
                            SFX.Play("ching");
                            if (Vincent.type == DayType.PawnDay)
                            {
                                Profiles.experienceProfile.littleManBucks += FurniShopScreen.attemptBuy.cost;
                                Profiles.experienceProfile.SetNumFurnitures((int)FurniShopScreen.attemptBuy.furnitureData.index, Profiles.experienceProfile.GetNumFurnitures((int)FurniShopScreen.attemptBuy.furnitureData.index) - 1);
                                using (IEnumerator<Profile> enumerator = Profiles.all.GetEnumerator())
                                {
                                label_52:
                                    if (enumerator.MoveNext())
                                    {
                                        Profile current = enumerator.Current;
                                        FurniturePosition furniturePosition1;
                                        do
                                        {
                                            furniturePosition1 = (FurniturePosition)null;
                                            foreach (FurniturePosition furniturePosition2 in current.furniturePositions)
                                            {
                                                if (current.GetNumFurnituresPlaced((int)furniturePosition2.id) > Profiles.experienceProfile.GetNumFurnitures((int)furniturePosition2.id))
                                                {
                                                    furniturePosition1 = furniturePosition2;
                                                    break;
                                                }
                                            }
                                            current.furniturePositions.Remove(furniturePosition1);
                                        }
                                        while (furniturePosition1 != null);
                                        goto label_52;
                                    }
                                }
                                Vincent.Clear();
                                Vincent.Add("|POINT|THANKS! |CONCERNED|I DON'T REGRET BUYING THIS AT ALL...");
                            }
                            else
                            {
                                Profiles.experienceProfile.littleManBucks -= FurniShopScreen.attemptBuy.cost;
                                if (FurniShopScreen.attemptBuy.furnitureData != null)
                                    Profiles.experienceProfile.SetNumFurnitures((int)FurniShopScreen.attemptBuy.furnitureData.index, Profiles.experienceProfile.GetNumFurnitures((int)FurniShopScreen.attemptBuy.furnitureData.index) + 1);
                                else if (FurniShopScreen.attemptBuy.teamData != null)
                                    Global.boughtHats.Add(FurniShopScreen.attemptBuy.teamData.name);
                            }
                            Vincent.Sold();
                            if (Vincent.products.Count == 1 && Vincent.type != DayType.PawnDay)
                            {
                                FurniShopScreen.open = false;
                                Vincent.Clear();
                                this._state = UILevelBoxState.UpdateTime;
                            }
                            FurniShopScreen.attemptBuy = (VincentProduct)null;
                        }
                        else
                        {
                            this._attemptingBuy = false;
                            UILevelBox._confirmMenu = (UIMenu)null;
                            if (Vincent.type == DayType.PawnDay)
                            {
                                Vincent.Clear();
                                Vincent.Add("|CONCERNED|HAVING SECOND THOUGHTS ABOUT SELLING THAT, HUH?");
                            }
                            FurniShopScreen.attemptBuy = (VincentProduct)null;
                            this.skipUpdate = true;
                        }
                    }
                }
                else
                {
                    if (!this._attemptingVincentClose && Input.Pressed("CANCEL") && !Vincent._willGiveVooDoo && !Vincent._willGiveYoYo && !Vincent._willGivePerimeterDefence)
                    {
                        this._menuBool.value = false;
                        UILevelBox._confirmMenu = new UIMenu("LEAVE VINCENT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        UILevelBox._confirmMenu.Add((UIComponent)new UIMenuItem("YES!", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean((UIComponent)UILevelBox._confirmMenu, this._menuBool)), true);
                        UILevelBox._confirmMenu.Add((UIComponent)new UIMenuItem("NO!", (UIMenuAction)new UIMenuActionCloseMenu((UIComponent)UILevelBox._confirmMenu), backButton: true), true);
                        UILevelBox._confirmMenu.DoInitialize();
                        UILevelBox._confirmMenu.Close();
                        for (int index = 0; index < 10; ++index)
                            UILevelBox._confirmMenu.DoUpdate();
                        UILevelBox._confirmMenu.Open();
                        this._attemptingVincentClose = true;
                    }
                    if (this._attemptingVincentClose && UILevelBox._confirmMenu != null && !UILevelBox._confirmMenu.open)
                    {
                        if (this._menuBool.value)
                        {
                            FurniShopScreen.open = false;
                            Vincent.Clear();
                            this._menuBool.value = false;
                            this._state = UILevelBoxState.UpdateTime;
                        }
                        else
                        {
                            this._attemptingVincentClose = false;
                            UILevelBox._confirmMenu = (UIMenu)null;
                        }
                    }
                }
            }
            if (FurniShopScreen.open && !this.skipUpdate)
                Vincent.Update();
            this.skipUpdate = false;
            if (FurniShopScreen.open && !Vincent.showingDay || UILevelBox._confirmMenu != null && UILevelBox._confirmMenu.open)
                return;
            if (!this.doubleUpdating && Input.Down("SELECT"))
            {
                this.doubleUpdating = true;
                this.UpdateParts();
                this.UpdateParts();
                this.doubleUpdating = false;
            }
            if (Input.Pressed("START") && !this._littleManLeave && Profiles.experienceProfile.GetNumFurnituresPlaced((int)RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && !FurniShopScreen.open)
                this.skipping = true;
            if (!this.doubleUpdating && this.skipping)
            {
                this.doubleUpdating = true;
                for (int index = 0; index < 200; ++index)
                {
                    SFX.skip = true;
                    this.UpdateParts();
                    SFX.skip = false;
                    if (this._littleManLeave || !this.skipping)
                        break;
                }
                this.doubleUpdating = false;
                if (this._finned)
                {
                    Graphics.fadeAdd = 1f;
                    this.skipping = false;
                    SFX.skip = false;
                    SFX.Play("dacBang");
                }
            }
            if ((double)this._genericWait > 0.0)
            {
                this._genericWait -= Maths.IncFrameTimer();
            }
            else
            {
                this._overrideSlide = (string)null;
                if (this._desiredLevel != this._currentLevel || this._newGrowthLevel != Profiles.experienceProfile.littleManLevel)
                {
                    if (!this._didSlide)
                    {
                        this._levelSlideWait += 0.08f;
                        if ((double)this._levelSlideWait < 1.0)
                            return;
                        int num = this._currentLevel;
                        if (Profiles.experienceProfile.numLittleMen > 0)
                            num = Profiles.experienceProfile.littleManLevel;
                        if (!this._unSlide && !this.playedSound)
                        {
                            SFX.Play("dukget");
                            this.playedSound = true;
                        }
                        this._overrideSlide = "GROWING UP";
                        if (num == 1)
                        {
                            this._overrideSlide = "EGG GET";
                            this._gotEgg = true;
                        }
                        if (num == 2)
                            this._overrideSlide = "EGG HATCH";
                        if (num == 6)
                            this._overrideSlide = "GROWN UP";
                        if (num == 7)
                            this._overrideSlide = "MAX OUT!";
                        if (this._unSlide)
                        {
                            this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 0.0f, 0.45f);
                            if ((double)this._intermissionSlide > 0.00999999977648258)
                                return;
                            this.playedSound = false;
                            this._unSlide = false;
                            this._didSlide = true;
                            this._intermissionSlide = 0.0f;
                            SFX.Play("levelUp");
                            return;
                        }
                        this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 1f, 0.21f, 1.05f);
                        if ((double)this._intermissionSlide < 1.0)
                            return;
                        this._slideWait += 0.08f;
                        if ((double)this._slideWait < 1.0)
                            return;
                        this._unSlide = true;
                        this._slideWait = 0.0f;
                        return;
                    }
                    this._levelSlideWait = 0.0f;
                    Graphics.fadeAdd = Lerp.Float(Graphics.fadeAdd, 1f, 0.1f);
                    if ((double)Graphics.fadeAdd < 1.0)
                        return;
                    this._didSlide = false;
                    this._currentLevel = this._desiredLevel;
                    this._talkLine = "";
                    this._feedLine = "I AM A HUNGRY\nLITTLE MAN.";
                    this._startFeedLine = this._feedLine;
                    Profiles.experienceProfile.littleManLevel = this._newGrowthLevel;
                    int num1 = this._currentLevel;
                    if (Profiles.experienceProfile.numLittleMen > 0)
                        num1 = Profiles.experienceProfile.littleManLevel;
                    if (this._currentLevel == 3)
                    {
                        this._oldGachaValue = (float)this._gachaValue;
                        this._newGachaValue = (float)this._gachaValue + (this._newXPValue - (float)this._xpValue);
                    }
                    if (this._currentLevel == 4)
                    {
                        this._oldSandwichValue = (float)this._sandwichValue;
                        this._newSandwichValue = (float)this._sandwichValue + (this._newXPValue - (float)this._xpValue);
                    }
                    if (this._currentLevel >= 7)
                    {
                        this._oldMilkValue = (float)this._milkValue;
                        this._newMilkValue = (float)this._milkValue + (this._newXPValue - (float)this._xpValue);
                    }
                    int currentLevel = this._currentLevel;
                    if (num1 >= 7)
                    {
                        this._littleManLeave = true;
                        HUD.CloseAllCorners();
                    }
                }
                else
                {
                    Graphics.fadeAdd = Lerp.Float(Graphics.fadeAdd, 0.0f, 0.1f);
                    if ((double)Graphics.fadeAdd > 0.00999999977648258)
                        return;
                }
                if (!this._talking)
                {
                    this._talk = 0.0f;
                }
                else
                {
                    this._talkWait += 0.2f;
                    if ((double)this._talkWait >= 1.0)
                    {
                        this._talkWait = 0.0f;
                        if (this._feedLine.Length > 0)
                        {
                            this._talkLine += this._feedLine[0].ToString();
                            if (this._feedLine[0] == '.' || this._feedLine[0] == '!' || this._feedLine[0] == '?')
                            {
                                SFX.Play("tinyTick", pitch: Rando.Float(-0.1f, 0.1f));
                                this._talkWait = -2f;
                            }
                            else
                                SFX.Play("tinyNoise1", pitch: Rando.Float(-0.1f, 0.1f));
                            this._feedLine = this._feedLine.Remove(0, 1);
                        }
                    }
                    this._alwaysClose = false;
                    if (this._talking && this._talkLine == this._startFeedLine)
                    {
                        this._alwaysClose = true;
                        this._finishTalkWait += 0.1f;
                        if ((double)this._finishTalkWait > 6.0 && !this.eggTalk)
                        {
                            this._talking = false;
                            this._talkLine = "";
                            this._finishTalkWait = 0.0f;
                        }
                    }
                    if (this._talkLine.Length > 0 && this._talkLine[this._talkLine.Length - 1] == '.')
                        this._alwaysClose = true;
                    this._talk += !this.close ? 0.2f : -0.2f;
                    if ((double)this._talk > 2.0)
                    {
                        this._talk = 2f;
                        this.close = true;
                    }
                    if ((double)this._talk < 0.0)
                    {
                        this._talk = 0.0f;
                        if (!this._alwaysClose)
                            this.close = false;
                    }
                }
                if (this.sayQueue.Count > 0 && !this._talking)
                {
                    this._talking = true;
                    this._talkLine = "";
                    this._feedLine = this.sayQueue.First<string>();
                    this.sayQueue.RemoveAt(0);
                    this._startFeedLine = this._feedLine;
                }
                if (this._littleManLeave)
                {
                    if (this.skipping)
                    {
                        this.skipping = false;
                        SFX.skip = false;
                        SFX.Play("dacBang");
                        Graphics.fadeAdd = 1f;
                    }
                    this._littleManStartWait += 0.02f;
                    if ((double)this._littleManStartWait < 1.0)
                        return;
                    if (this._driveAway)
                    {
                        this._sound.lerpVolume = 1f;
                        this._taxiDrive = Lerp.Float(this._taxiDrive, 1f, 0.03f);
                        if ((double)this._taxiDrive < 1.0)
                            return;
                        SFX.Play("doorOpen");
                        this._driveAway = false;
                        this._genericWait = 0.5f;
                        this._sound.volume = this._sound.lerpVolume = 0.0f;
                    }
                    else if ((double)this._taxiDrive > 0.0)
                    {
                        if (!this._inTaxi)
                        {
                            SFX.Play("doorClose");
                            this._inTaxi = true;
                            this._genericWait = 0.5f;
                        }
                        else
                        {
                            this._sound.lerpVolume = 1f;
                            this._taxiDrive = Lerp.Float(this._taxiDrive, 2f, 0.03f);
                            if ((double)this._taxiDrive < 2.0)
                                return;
                            this._sound.lerpVolume = 0.0f;
                            ++Profiles.experienceProfile.numLittleMen;
                            Profiles.experienceProfile.littleManLevel = 1;
                            this._newGrowthLevel = Profiles.experienceProfile.littleManLevel + 1;
                            this._egg = Profile.GetEggSprite(Profiles.experienceProfile.numLittleMen);
                            this._littleManStartWait = 0.0f;
                            this._littleManLeave = false;
                            this._driveAway = false;
                            this._taxiDrive = 0.0f;
                            this._inTaxi = false;
                            this._startedLittleManLeave = false;
                            this.littleEggs.Clear();
                        }
                    }
                    else if (!this._startedLittleManLeave)
                    {
                        if (Profiles.experienceProfile.numLittleMen == 0)
                        {
                            this.Say("I AM A FULL\nLITTLE MAN.");
                            this.Say("THANK YOU FOR\nRAISING ME.");
                            this.Say("I MUST LEAVE\nNOW.");
                            this.Say("I LOVE MY\nPARENT.");
                            this.Say("...");
                            this.Say("PLEASE ACCEPT\nTHIS GIFT.");
                        }
                        else
                        {
                            this.Say("I LOVE MY\nPARENT.");
                            this.Say("...");
                            this.Say("PLEASE ACCEPT\nTHIS GIFT.");
                        }
                        this._startedLittleManLeave = true;
                    }
                    else
                    {
                        if (this._talking)
                            return;
                        UILevelBox._confirmMenu = (UIMenu)new UIPresentBox(Profiles.experienceProfile.numLittleMen != 0 ? (Profiles.experienceProfile.numLittleMen != 1 ? (Profiles.experienceProfile.numLittleMen != 2 ? (Profiles.experienceProfile.numLittleMen != 3 ? (Profiles.experienceProfile.numLittleMen != 4 ? (Profiles.experienceProfile.numLittleMen != 5 ? (Profiles.experienceProfile.numLittleMen != 6 ? (Profiles.experienceProfile.numLittleMen != 7 ? UIGachaBox.GetRandomFurniture(Rarity.VeryRare, 1, 0.75f)[0] : RoomEditor.GetFurniture("JUKEBOX")) : RoomEditor.GetFurniture("EASEL")) : RoomEditor.GetFurniture("JUNK")) : RoomEditor.GetFurniture("WINE")) : RoomEditor.GetFurniture("GIFT BASKET")) : RoomEditor.GetFurniture("PLATE")) : RoomEditor.GetFurniture("PHOTO")) : RoomEditor.GetFurniture("EGG"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                        UILevelBox._confirmMenu.depth = (Depth)0.98f;
                        UILevelBox._confirmMenu.DoInitialize();
                        UILevelBox._confirmMenu.Open();
                        this._genericWait = 0.5f;
                        this._driveAway = true;
                    }
                }
                else
                {
                    this._inTaxi = false;
                    this._stampWobbleSin += 0.8f;
                    if (this._showCard)
                        this._stampCardLerp = Lerp.FloatSmooth(this._stampCardLerp, this._stampCard ? 1f : 0.0f, 0.18f, 1.05f);
                    foreach (LittleHeart heart in this._hearts)
                    {
                        heart.position += heart.velocity;
                        heart.alpha -= 0.02f;
                    }
                    this._hearts.RemoveAll((Predicate<LittleHeart>)(t => (double)t.alpha <= 0.0));
                    this._coinLerp2 = Lerp.Float(this._coinLerp2, 1f, 0.08f);
                    this._stampWobble = Lerp.Float(this._stampWobble, 0.0f, 0.08f);
                    if (this._stampCard || (double)this._stampCardLerp > 0.00999999977648258)
                    {
                        if (!this._showCard)
                        {
                            if (!this._sandwichShift)
                            {
                                this._sandwichLerp = Lerp.Float(this._sandwichLerp, 1f, 0.12f);
                                if ((double)this._sandwichLerp >= 1.0)
                                    this._sandwichShift = true;
                            }
                            else if (this._burp)
                            {
                                this._extraMouthOpen = Lerp.FloatSmooth(this._extraMouthOpen, 0.0f, 0.17f, 1.05f);
                                this._finalWait += 0.1f;
                                if ((double)this._finalWait >= 1.0)
                                {
                                    this._finalWait = 0.0f;
                                    this._finishEat = false;
                                    this._afterEatWait = 0.0f;
                                    this._burp = false;
                                    this._sandwichLerp = 0.0f;
                                    this._extraMouthOpen = 0.0f;
                                    this._sandwichShift = false;
                                    this._eatWait = 0.0f;
                                    this._openWait = 0.0f;
                                    this._showCard = true;
                                    this._sandwichEat = 0.0f;
                                    this._finishingNewStamp = false;
                                }
                            }
                            else if (this._finishEat)
                            {
                                this._extraMouthOpen = Lerp.FloatSmooth(this._extraMouthOpen, 0.0f, 0.17f, 1.05f);
                                this._afterEatWait += 0.08f;
                                if ((double)this._afterEatWait >= 1.0)
                                {
                                    SFX.Play("healthyEat");
                                    for (int index = 0; index < 8; ++index)
                                        this._hearts.Add(new LittleHeart()
                                        {
                                            position = this.littleManPos + new Vec2(8f + Rando.Float(-4f, 4f), 8f + Rando.Float(-6f, 6f)),
                                            velocity = new Vec2(0.0f, Rando.Float(-0.2f, -0.4f))
                                        });
                                    this._burp = true;
                                }
                            }
                            else
                            {
                                this._sandwichLerp = Lerp.Float(this._sandwichLerp, 0.0f, 0.12f);
                                if ((double)this._sandwichLerp <= 0.0)
                                {
                                    this._extraMouthOpen = Lerp.FloatSmooth(this._extraMouthOpen, 15f, 0.18f, 1.05f);
                                    if ((double)this._extraMouthOpen >= 1.0)
                                    {
                                        this._openWait += 0.08f;
                                        if ((double)this._openWait >= 1.0)
                                        {
                                            this._sandwichEat = Lerp.Float(this._sandwichEat, 1f, 0.08f);
                                            if ((double)this._sandwichEat >= 1.0)
                                            {
                                                if ((double)this._eatWait == 0.0)
                                                    SFX.Play("swallow");
                                                this._eatWait += 0.08f;
                                                if ((double)this._eatWait >= 1.0)
                                                    this._finishEat = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if ((double)this._stampCardLerp < 0.990000009536743)
                            return;
                        this._stampWait += 0.2f;
                        if ((double)this._stampWait < 1.0)
                            return;
                        if ((double)this._stampWait2 == 0.0)
                        {
                            ++Profiles.experienceProfile.numSandwiches;
                            this._finishingNewStamp = true;
                            this._stampWobble = 1f;
                            SFX.Play("dacBang");
                        }
                        this._stampWait2 += 0.06f;
                        if ((double)this._stampWait2 < 1.0)
                            return;
                        if (Profiles.experienceProfile.numSandwiches > 0 && Profiles.experienceProfile.numSandwiches % 6 == 0 && (double)this._coin2Wait == 0.0)
                        {
                            this._coinLerp2 = 0.0f;
                            this._coin2Wait = 1f;
                            SFX.Play("ching", pitch: 0.2f);
                            ++UILevelBox.rareGachas;
                        }
                        this._coin2Wait -= 0.08f;
                        if ((double)this._coin2Wait > 0.0)
                            return;
                        this._stampCard = false;
                        this._stampWait2 = 0.0f;
                        this._stampWait = 0.0f;
                        this._coin2Wait = 0.0f;
                    }
                    else
                    {
                        this._showCard = false;
                        Vec2 vec2 = new Vec2(this.x - 80f, this.y - 10f);
                        if (!this.open)
                            return;
                        if (this._currentLevel == this._desiredLevel)
                        {
                            if (this._currentLevel > 3 && this._finned && Input.Pressed("MENU2"))
                            {
                                this._talking = true;
                                this._finishTalkWait = 0.0f;
                                this._talkLine = "";
                                if (Profiles.experienceProfile.littleManLevel <= 2 && this._currentLevel > 6)
                                {
                                    this._feedLine = "";
                                    if (!Global.data.hadTalk)
                                    {
                                        if (this._eggFeedIndex < this._eggFeedLines.Count)
                                        {
                                            if (this._eggFeedIndex > 2)
                                                this.eggTalk = true;
                                            this._feedLine = this._eggFeedLines[this._eggFeedIndex];
                                            ++this._eggFeedIndex;
                                            if (this._eggFeedIndex >= this._eggFeedLines.Count)
                                            {
                                                Global.data.hadTalk = true;
                                                HUD.CloseAllCorners();
                                                this.eggTalk = false;
                                                this._finned = false;
                                            }
                                        }
                                        else
                                            this._feedLine = "...";
                                    }
                                }
                                else
                                {
                                    this._feedLine = "I AM A HUNGRY\nLITTLE MAN.";
                                    if (Rando.Int(1000) == 1)
                                        this._feedLine = "I... AM A HUNGRY\nLITTLE MAN.";
                                    DateTime localTime = MonoMain.GetLocalTime();
                                    if (!UILevelBox.saidSpecial)
                                    {
                                        if (localTime.Month == 4 && localTime.Day == 20)
                                            this._feedLine = "HAPPY BIRTHDAY!";
                                        else if (localTime.Month == 3 && localTime.Day == 9)
                                            this._feedLine = "HAPPY BIRTHDAY!!";
                                        else if (localTime.Month == 1 && localTime.Day == 1)
                                            this._feedLine = "HAPPY NEW YEAR!";
                                        else if (localTime.Month == 6 && localTime.Day == 4)
                                            this._feedLine = "HAPPY BIRTHDAY\nDUCK GAME!";
                                        else if (Rando.Int(190000) == 1)
                                            this._feedLine = "LET'S DANCE!";
                                        else if (Rando.Int(80000) == 1)
                                            this._feedLine = "HAPPY BIRTHDAY!";
                                        UILevelBox.saidSpecial = true;
                                    }
                                    if (Rando.Int(100000) == 1)
                                        this._feedLine = "I AM A HANGRY\nLITTLE MAN.";
                                    else if (Rando.Int(150000) == 1)
                                        this._feedLine = "I AM A HAPPY\nLITTLE MAN.";
                                }
                                this._startFeedLine = this._feedLine;
                            }
                            if (this._state == UILevelBoxState.LogWinLoss)
                            {
                                if (Input.Pressed("SELECT"))
                                {
                                    if (Profiles.experienceProfile != null)
                                        Profiles.experienceProfile.xp = this._xpValue;
                                    SFX.Play("rockHitGround2", pitch: 0.5f);
                                    this.Close();
                                }
                            }
                            else if (this._state == UILevelBoxState.Wait)
                            {
                                this._startWait -= 0.09f;
                                if ((double)this._startWait < 0.0)
                                {
                                    this._startWait = 1f;
                                    this._state = UILevelBoxState.ShowXPBar;
                                    SFX.Play("rockHitGround2", pitch: 0.5f);
                                }
                            }
                            else if (this._state == UILevelBoxState.UpdateTime)
                            {
                                this._advancedDay = false;
                                this._fallVel = 0.0f;
                                this._finned = false;
                                //this._updateTime = false;
                                this._markedNewDay = false;
                                this._advanceDayWait = 0.0f;
                                this._dayFallAway = 0.0f;
                                this._dayScroll = 0.0f;
                                this._newCircleLerp = 0.0f;
                                this._popDay = false;
                                this._slideWait = 0.0f;
                                this._unSlide = false;
                                this._intermissionSlide = 0.0f;
                                this._intermissionWait = 0.0f;
                                this._gaveToy = false;
                                if (this._roundsPlayed > 0)
                                {
                                    this._updateTimeWait += 0.08f;
                                    if ((double)this._updateTimeWait >= 1.0)
                                    {
                                        this._dayTake += 0.8f;
                                        if ((double)this._dayTake >= 1.0)
                                        {
                                            this._dayTake = 0.0f;
                                            --this._roundsPlayed;
                                        }
                                        this._dayProgress = (float)(1.0 - (double)this._roundsPlayed / (double)this._startRoundsPlayed);
                                        this.time += 0.08f;
                                    }
                                    if ((double)this.time >= 1.0)
                                    {
                                        --this.time;
                                        this._state = UILevelBoxState.AdvanceDay;
                                        this._updateTimeWait = 0.0f;
                                        this._dayTake = 0.0f;
                                    }
                                }
                                else
                                    this._state = UILevelBoxState.Finished;
                            }
                            else if (this._state == UILevelBoxState.RunDay)
                            {
                                DayType day = this.GetDay(Profiles.experienceProfile.currentDay);
                                switch (day)
                                {
                                    case DayType.Empty:
                                        this._state = UILevelBoxState.UpdateTime;
                                        break;
                                    case DayType.Sandwich:
                                        if (!this._gaveToy)
                                        {
                                            this._stampCard = true;
                                            this._state = UILevelBoxState.UpdateTime;
                                            break;
                                        }
                                        break;
                                    case DayType.FreeXP:
                                        //this._didXPDay = true;
                                        DuckNetwork.GiveXP("FREE XP DAY", 0, 75);
                                        this._state = UILevelBoxState.Wait;
                                        break;
                                    case DayType.ToyDay:
                                        if (!this._gaveToy)
                                        {
                                            this._gaveToy = true;
                                            ++UILevelBox.gachas;
                                            this._coinLerp = 0.0f;
                                            SFX.Play("ching", pitch: 0.2f);
                                        }
                                        this._finishDayWait += 0.04f;
                                        if ((double)this._finishDayWait >= 1.0)
                                        {
                                            this._state = UILevelBoxState.UpdateTime;
                                            break;
                                        }
                                        break;
                                    case DayType.PayDay:
                                        if (this._giveMoney == 0)
                                        {
                                            int num = 75;
                                            if (this._currentLevel > 5)
                                                num = 100;
                                            if (this._currentLevel > 6)
                                                num = 125;
                                            this._giveMoney = num + Profiles.experienceProfile.numLittleMen * 25;
                                            Profiles.experienceProfile.littleManBucks += this._giveMoney;
                                            SFX.Play("ching");
                                        }
                                        this._giveMoneyRise = Lerp.Float(this._giveMoneyRise, 1f, 0.05f);
                                        this._finishDayWait += 0.04f;
                                        if ((double)this._finishDayWait >= 1.0)
                                        {
                                            this._giveMoneyRise = 1f;
                                            this._giveMoney = 0;
                                            this._state = UILevelBoxState.UpdateTime;
                                            break;
                                        }
                                        break;
                                    case DayType.Allowance:
                                        if (this._giveMoney == 0)
                                        {
                                            this._giveMoney = 200;
                                            Profiles.experienceProfile.littleManBucks += this._giveMoney;
                                            SFX.Play("ching");
                                        }
                                        this._giveMoneyRise = Lerp.Float(this._giveMoneyRise, 1f, 0.05f);
                                        this._finishDayWait += 0.04f;
                                        if ((double)this._finishDayWait >= 1.0)
                                        {
                                            this._giveMoneyRise = 1f;
                                            this._giveMoney = 0;
                                            this._state = UILevelBoxState.UpdateTime;
                                            break;
                                        }
                                        break;
                                    default:
                                        if (this.IsVinceDay(day) && !Input.Down("SELECT"))
                                        {
                                            this._state = UILevelBoxState.UpdateTime;
                                            Vincent.showingDay = false;
                                            FurniShopScreen.close = true;
                                            break;
                                        }
                                        break;
                                }
                            }
                            else if (this._state == UILevelBoxState.AdvanceDay)
                            {
                                if (!this._advancedDay)
                                {
                                    if (!this._popDay)
                                    {
                                        this._popDay = true;
                                        this._fallVel = -0.025f;
                                        this._ranRot = Rando.Float(-0.3f, 0.3f);
                                    }
                                    if ((double)this._dayFallAway < 1.0)
                                    {
                                        this._dayFallAway += this._fallVel;
                                        this._fallVel += 0.005f;
                                    }
                                    else if ((double)this._dayScroll < 1.0)
                                    {
                                        this._dayScroll = Lerp.Float(this._dayScroll, 1f, 0.1f);
                                    }
                                    else
                                    {
                                        this._advancedDay = true;
                                        ++Profiles.experienceProfile.currentDay;
                                        this._dayFallAway = 0.0f;
                                        this._dayScroll = 0.0f;
                                    }
                                }
                                else
                                {
                                    this._advanceDayWait += 0.1f;
                                    if ((double)this._advanceDayWait >= 1.0)
                                    {
                                        if (!this._markedNewDay)
                                        {
                                            SFX.Play("chalk");
                                            this._currentDay = Profiles.experienceProfile.currentDay;
                                            this._markedNewDay = true;
                                            if (this.IsVinceDay(this.GetDay(Profiles.experienceProfile.currentDay)))
                                            {
                                                this.skipping = false;
                                                SFX.skip = false;
                                                SFX.Play("dacBang");
                                                Graphics.fadeAdd = 1f;
                                            }
                                        }
                                        if (this._markedNewDay)
                                        {
                                            this._intermissionWait += 0.15f;
                                            if ((double)this._intermissionWait >= 1.0)
                                            {
                                                if (this._unSlide)
                                                {
                                                    this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 0.0f, 0.42f);
                                                    if ((double)this._intermissionSlide <= 0.0199999995529652)
                                                    {
                                                        this._intermissionSlide = 0.0f;
                                                        this._dayStartWait += 0.11f;
                                                        if ((double)this._dayStartWait >= 1.0)
                                                        {
                                                            this.AdvanceDay();
                                                            this._state = UILevelBoxState.RunDay;
                                                        }
                                                        if (this.IsVinceDay(this.GetDay(Profiles.experienceProfile.currentDay)))
                                                            Vincent.showingDay = false;
                                                    }
                                                }
                                                else
                                                {
                                                    this._intermissionSlide = Lerp.FloatSmooth(this._intermissionSlide, 1f, 0.2f, 1.05f);
                                                    if ((double)this._intermissionSlide >= 1.0)
                                                    {
                                                        DayType day = this.GetDay(Profiles.experienceProfile.currentDay);
                                                        if (this.IsVinceDay(day) && !Vincent.showingDay)
                                                        {
                                                            Vincent.Clear();
                                                            Vincent.showingDay = true;
                                                            Vincent.Open(day);
                                                            FurniShopScreen.open = true;
                                                            this._roundsPlayed = 0;
                                                        }
                                                        this._slideWait += 0.11f;
                                                        if ((double)this._slideWait >= 1.79999995231628)
                                                            this._unSlide = true;
                                                    }
                                                }
                                            }
                                        }
                                        this._newCircleLerp = Lerp.Float(this._newCircleLerp, 1f, 0.2f);
                                    }
                                }
                            }
                            else if (this._state == UILevelBoxState.Finished)
                            {
                                if (!this._finned)
                                {
                                    HUD.CloseAllCorners();
                                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                                    if (this._currentLevel > 3 && (!Global.data.hadTalk || Profiles.experienceProfile.littleManLevel > 2))
                                        HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@TALK");
                                    if (Profiles.experienceProfile.GetNumFurnituresPlaced((int)RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && (UILevelBox.rareGachas > 0 || UILevelBox.gachas > 0))
                                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@START@AUTO TOYS");
                                    this._finned = true;
                                }
                            }
                            else if (this._state == UILevelBoxState.ShowXPBar)
                            {
                                this._firstParticleIn = false;
                                this._drain = 1f;
                                if (this._currentStat.Key == null)
                                    this._currentStat = DuckNetwork.TakeXPStat();
                                if (this._currentStat.Key == null)
                                {
                                    this._state = this._roundsPlayed <= 0 || this._currentLevel < 4 ? UILevelBoxState.Finished : UILevelBoxState.UpdateTime;
                                }
                                else
                                {
                                    this._slideXPBar = Lerp.FloatSmooth(this._slideXPBar, 1f, 0.18f, 1.1f);
                                    if ((double)this._slideXPBar >= 1.0)
                                    {
                                        this._oldXPValue = (float)this._xpValue;
                                        this._newXPValue = (float)(this._xpValue + this._currentStat.Value.xp);
                                        if (this._currentLevel > 2)
                                        {
                                            this._oldGachaValue = (float)this._gachaValue;
                                            this._newGachaValue = (float)(this._gachaValue + this._currentStat.Value.xp);
                                            if (this._currentLevel > 3)
                                            {
                                                this._oldSandwichValue = (float)this._sandwichValue;
                                                this._newSandwichValue = (float)(this._sandwichValue + this._currentStat.Value.xp);
                                                if (this._currentLevel >= 7)
                                                {
                                                    this._oldMilkValue = (float)this._milkValue;
                                                    this._newMilkValue = (float)(this._milkValue + this._currentStat.Value.xp);
                                                }
                                            }
                                        }
                                        this._state = UILevelBoxState.WaitXPBar;
                                        SFX.Play("scoreDing", pitch: 0.5f);
                                    }
                                }
                            }
                            else if (this._state == UILevelBoxState.WaitXPBar)
                            {
                                this._startWait -= 0.15f;
                                if ((double)this._startWait < 0.0)
                                {
                                    this._startWait = 1f;
                                    this._state = UILevelBoxState.DrainXPBar;
                                }
                            }
                            else if (this._state == UILevelBoxState.DrainXPBar)
                            {
                                this._drain -= 0.04f;
                                if ((double)this._drain > 0.0)
                                {
                                    this._particleWait -= 0.4f;
                                    if ((double)this._particleWait < 0.0)
                                    {
                                        float y = 30f;
                                        if (this._currentLevel == 3)
                                            y = 25f;
                                        if (this._currentLevel >= 4)
                                            y = 20f;
                                        if (this._currentLevel >= 4)
                                            y = 0.0f;
                                        if (this._currentLevel >= 7)
                                            y = -12f;
                                        if (this._currentStat.Value.type == 0 || this._currentStat.Value.type == 4)
                                            this._particles.Add(new XPPlus()
                                            {
                                                position = new Vec2(this.x - 72f, this.y - 58f),
                                                velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                                target = vec2 + new Vec2(0.0f, y),
                                                color = Colors.DGGreen
                                            });
                                        if (this._currentLevel >= 3 && (this._currentStat.Value.type == 1 || this._currentStat.Value.type == 4))
                                            this._particles.Add(new XPPlus()
                                            {
                                                position = new Vec2(this.x - 72f, this.y - 58f),
                                                velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                                target = vec2 + new Vec2(0.0f, 10f + y),
                                                color = Colors.DGRed
                                            });
                                        if (this._currentLevel >= 4 && (this._currentStat.Value.type == 2 || this._currentStat.Value.type == 4))
                                            this._particles.Add(new XPPlus()
                                            {
                                                position = new Vec2(this.x - 72f, this.y - 58f),
                                                velocity = new Vec2(-Rando.Float(3f, 6f), -Rando.Float(1f, 4f)),
                                                target = vec2 + new Vec2(0.0f, 20f + y),
                                                color = Colors.DGBlue
                                            });
                                        ++this._xpLost;
                                        SFX.Play("tinyTick");
                                        this._particleWait = 1f;
                                    }
                                }
                                if (this._firstParticleIn)
                                {
                                    this._addLerp += 0.04f;
                                    this._xpValue = (int)Lerp.FloatSmooth(this._oldXPValue, this._newXPValue, this._addLerp);
                                    this._gachaValue = (int)Lerp.FloatSmooth(this._oldGachaValue, this._newGachaValue, this._addLerp);
                                    this._sandwichValue = (int)Lerp.FloatSmooth(this._oldSandwichValue, this._newSandwichValue, this._addLerp);
                                    this._milkValue = (int)Lerp.FloatSmooth(this._oldMilkValue, this._newMilkValue, this._addLerp);
                                    this._xpProgress = (float)(this._xpValue - this._originalXP) / (float)this._totalXP;
                                }
                                if ((double)this._drain < 0.0)
                                    this._drain = 0.0f;
                                if ((double)this._drain <= 0.0 && (double)this._addLerp >= 1.0)
                                {
                                    this._drain = 0.0f;
                                    this._addLerp = 0.0f;
                                    this._state = UILevelBoxState.HideXPBar;
                                }
                            }
                            else if (this._state == UILevelBoxState.HideXPBar)
                            {
                                this._slideXPBar = Lerp.FloatSmooth(this._slideXPBar, 0.0f, 0.2f, 1.1f);
                                if ((double)this._slideXPBar <= 0.0199999995529652)
                                {
                                    this._currentStat = new KeyValuePair<string, XPPair>();
                                    this._state = UILevelBoxState.ShowXPBar;
                                    SFX.Play("rockHitGround2", pitch: 0.5f);
                                    this._slideXPBar = 0.0f;
                                }
                            }
                        }
                        if (this._currentLevel == this._desiredLevel)
                        {
                            this._coinLerp = Lerp.Float(this._coinLerp, 1f, 0.1f);
                            foreach (XPPlus particle in this._particles)
                            {
                                particle.position += particle.velocity;
                                if (!particle.splash)
                                {
                                    particle.position = Lerp.Vec2Smooth(particle.position, particle.target, particle.time);
                                    particle.time += 0.01f;
                                }
                                else
                                {
                                    particle.velocity.y += 0.2f;
                                    particle.alpha -= 0.05f;
                                }
                            }
                        }
                        int count = this._particles.Count;
                        this._particles.RemoveAll((Predicate<XPPlus>)(part => (double)(part.position - part.target).lengthSq < 64.0));
                        if (this._particles.Count != count)
                            this._firstParticleIn = true;
                        if (this._xpValue >= DuckNetwork.GetLevel(this._desiredLevel + 1).xpRequired && this._currentLevel != 20)
                            ++this._desiredLevel;
                        if (this._currentLevel <= 2)
                            return;
                        if (this._gachaValue >= this.gachaNeed)
                        {
                            this._gachaValue -= this.gachaNeed;
                            this._newGachaValue -= (float)this.gachaNeed;
                            this._oldGachaValue -= (float)this.gachaNeed;
                            ++UILevelBox.gachas;
                            this._coinLerp = 0.0f;
                            SFX.Play("ching", pitch: 0.2f);
                        }
                        if (this._milkValue >= this.milkNeed)
                        {
                            this._milkValue -= this.milkNeed;
                            this._newMilkValue -= (float)this.milkNeed;
                            this._oldMilkValue -= (float)this.milkNeed;
                            this._newGrowthLevel = Profiles.experienceProfile.littleManLevel + 1;
                            if (this._newGrowthLevel > 7)
                                this._newGrowthLevel = 7;
                        }
                        if (this._currentLevel <= 3 || this._sandwichValue < this.sandwichNeed)
                            return;
                        this._sandwichValue -= this.sandwichNeed;
                        this._newSandwichValue -= (float)this.sandwichNeed;
                        this._oldSandwichValue -= (float)this.sandwichNeed;
                        this._stampCard = true;
                    }
                }
            }
        }

        public override void Draw()
        {
            if (this.skipping)
                return;
            int num1 = 33;
            if (this._currentLevel >= 3)
                num1 = 38;
            if (this._currentLevel >= 4)
                num1 = 63;
            if (this._currentLevel >= 7)
                num1 = 75;
            Vec2 vec2_1 = new Vec2(this.x, this.y - (float)num1 * this._slideXPBar);
            int num2 = this._currentLevel;
            if (num2 > 8)
                num2 = 8;
            Sprite frame = this._frames[num2 - 1];
            float num3 = 30f;
            if (this._currentLevel == 3)
                num3 = 25f;
            if (this._currentLevel >= 4)
                num3 = 20f;
            if (this._currentLevel >= 4)
                num3 = 0.0f;
            if (this._currentLevel >= 7)
                num3 = -12f;
            frame.depth = this.depth;
            Graphics.Draw(frame, this.x, this.y);
            string text1 = "@LWING@" + Profiles.experienceProfile.name + "@RWING@";
            float x1 = 0.0f;
            float y1 = 0.0f;
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
            this._font.scale = vec2_2;
            Vec2 vec2_3 = new Vec2((float)-((double)this._font.GetWidth(text1) / 2.0), num3 - 50f);
            this._font.DrawOutline(text1, this.position + vec2_3 + new Vec2(x1, y1), Color.White, Color.Black, this.depth + 2);
            this._font.scale = new Vec2(1f, 1f);
            this._lev.depth = this.depth + 2;
            this._lev.frame = this._currentLevel - 1;
            Graphics.Draw((Sprite)this._lev, this.x - 90f, this.y - 34f + num3);
            this._font.DrawOutline(this._currentLevel.ToString()[0].ToString() ?? "", this.position + new Vec2(-84f, num3 - 30f), Color.White, Color.Black, this.depth + 2);
            if (this._currentLevel > 9)
            {
                this._thickBiosNum.scale = new Vec2(0.5f, 0.5f);
                this._thickBiosNum.Draw(this._currentLevel.ToString()[1].ToString() ?? "", this.position + new Vec2(-78f, num3 - 32f), Color.White, this.depth + 20);
            }
            float num4 = 85f;
            if (this._currentLevel == 1)
                num4 = 175f;
            if (this._currentLevel == 2)
                num4 = 154f;
            if (this._currentLevel == 3)
                num4 = 122f;
            if (this._currentLevel >= 4)
                num4 = 94f;
            if (this._currentLevel >= 7)
                num4 = 83f;
            int xpRequired1 = DuckNetwork.GetLevel(this._currentLevel + 1).xpRequired;
            int num5 = 0;
            if (this._currentLevel > 0)
                num5 = DuckNetwork.GetLevel(this._currentLevel).xpRequired;
            int num6 = (int)Math.Round((double)xpRequired1 * ((double)this._xpValue / (double)xpRequired1));
            int xpRequired2 = DuckNetwork.GetLevel(9999).xpRequired;
            if (num6 > xpRequired2)
                num6 = xpRequired2;
            float num7 = xpRequired1 - num5 != 0 ? (float)(this._xpValue - num5) / (float)(xpRequired1 - num5) : 1f;
            string str1 = xpRequired1.ToString();
            if (str1.Length > 5)
                str1 = str1.Substring(0, 3) + "k";
            else if (str1.Length > 4)
                str1 = str1.Substring(0, 2) + "k";
            string text2 = "|DGGREEN|" + num6.ToString() + "|WHITE|/|DGBLUE|" + str1 + "|WHITE|";
            float num8 = 0.0f;
            if (this._currentLevel == 1)
                num8 = 94f;
            if (this._currentLevel == 2)
                num8 = 70f;
            if (this._currentLevel == 3)
                num8 = 38f;
            if (this._currentLevel >= 4)
                num8 = 10f;
            if (this._currentLevel >= 7)
                num8 = -1f;
            this._fancyFont.DrawOutline(text2, this.position + new Vec2(num8 - 8f, num3 - 31f) - new Vec2(this._fancyFont.GetWidth(text2), 0.0f), Colors.DGYellow, Color.Black, this.depth + 2);
            if ((double)num7 < 0.0234999991953373)
                num7 = 0.0235f;
            float num9 = num4 * num7;
            this._xpBar.depth = this.depth + 2;
            this._xpBar.xscale = 1f;
            Vec2 vec2_4 = new Vec2(this.x - 87f, this.y - 18f);
            Graphics.Draw(this._xpBar, vec2_4.x, vec2_4.y + num3, new Rectangle(0.0f, 0.0f, 3f, 6f));
            this._xpBar.xscale = num9 - 4f;
            Graphics.Draw(this._xpBar, vec2_4.x + 3f, vec2_4.y + num3, new Rectangle(2f, 0.0f, 1f, 6f));
            this._xpBar.depth = this.depth + 7;
            this._xpBar.xscale = 1f;
            Graphics.Draw(this._xpBar, vec2_4.x + (num9 - 2f), vec2_4.y + num3, new Rectangle(3f, 0.0f, 3f, 6f));
            int x2 = 0;
            this._barFront.depth = this.depth + 10;
            if ((double)num9 < 13.0)
                x2 = 13 - (int)num9;
            this._barHeat += Math.Abs(this._lastFill - num7) * 8f;
            if ((double)this._barHeat > 1.0)
                this._barHeat = 1f;
            this._barFront.alpha = this._barHeat;
            Graphics.Draw(this._barFront, vec2_4.x + num9 + (float)x2, vec2_4.y + num3, new Rectangle((float)x2, 0.0f, (float)(this._barFront.width - x2), 6f));
            this._barHeat = Maths.CountDown(this._barHeat, 0.04f);
            if (this._currentLevel >= 3)
            {
                float num10 = (float)this._gachaValue / (float)this.gachaNeed;
                float num11 = 110f;
                if (this._currentLevel == 3)
                    num11 = 149f;
                if (this._currentLevel >= 4)
                    num11 = 122f;
                float num12 = (float)Math.Floor((double)num11 * (double)num10);
                if ((double)num12 < 2.0)
                    num12 = 2f;
                this._gachaBar.depth = this.depth + 2;
                this._gachaBar.xscale = 1f;
                Vec2 vec2_5 = new Vec2(this.x - 87f, this.y - 5f);
                Graphics.Draw(this._gachaBar, vec2_5.x, vec2_5.y + num3, new Rectangle(0.0f, 0.0f, 3f, 3f));
                this._gachaBar.xscale = num12 - 5f;
                Graphics.Draw(this._gachaBar, vec2_5.x + 3f, vec2_5.y + num3, new Rectangle(2f, 0.0f, 1f, 3f));
                this._gachaBar.depth = this.depth + 7;
                this._gachaBar.xscale = 1f;
                Graphics.Draw(this._gachaBar, vec2_5.x + (num12 - 2f), vec2_5.y + num3, new Rectangle(3f, 0.0f, 3f, 3f));
                this._duckCoin.frame = 0;
                this._duckCoin.alpha = (float)(1.0 - (double)Math.Max(this._coinLerp - 0.5f, 0.0f) * 2.0);
                this._duckCoin.depth = (Depth)0.9f;
                Graphics.Draw((Sprite)this._duckCoin, (float)((double)vec2_5.x + ((double)num11 - 2.0) + 15.0), (float)((double)vec2_5.y + (double)num3 - 8.0 - (double)this._coinLerp * 18.0));
            }
            if (this._currentLevel >= 4)
            {
                float num13 = (float)this._sandwichValue / (float)this.sandwichNeed;
                float num14 = 154f;
                float num15 = num14 * num13;
                if ((double)num15 < 2.0)
                    num15 = 2f;
                this._sandwichBar.depth = this.depth + 2;
                this._sandwichBar.xscale = 1f;
                Vec2 vec2_6 = new Vec2(this.x - 87f, this.y + 5f);
                Graphics.Draw(this._sandwichBar, vec2_6.x, vec2_6.y + num3, new Rectangle(0.0f, 0.0f, 3f, 3f));
                this._sandwichBar.xscale = num15 - 5f;
                Graphics.Draw(this._sandwichBar, vec2_6.x + 3f, vec2_6.y + num3, new Rectangle(2f, 0.0f, 1f, 3f));
                this._sandwichBar.depth = this.depth + 7;
                this._sandwichBar.xscale = 1f;
                Graphics.Draw(this._sandwichBar, vec2_6.x + (num15 - 2f), vec2_6.y + num3, new Rectangle(3f, 0.0f, 3f, 3f));
                this._sandwich.depth = (Depth)0.88f;
                float num16 = this._sandwichLerp * -150f;
                float num17 = 0.0f;
                float val1 = 0.0f;
                if (this._sandwichShift)
                {
                    num16 -= 20f;
                    num17 = (float)(-42.0 - (double)this._sandwichEat * 30.0);
                    val1 = -52f - num17;
                    if (this._currentLevel >= 7)
                        num17 -= 10f;
                }
                float x3 = Math.Max(val1, 0.0f);
                if ((double)x3 < (double)this._sandwich.width)
                    Graphics.Draw(this._sandwich, (float)((double)vec2_6.x + ((double)num14 - 2.0) + 12.0 + (double)num17 + (double)x3 + 1.0), (float)((double)vec2_6.y + (double)num3 - 16.0) + num16, new Rectangle(x3, 0.0f, (float)this._sandwich.width - x3, (float)this._sandwich.height), (Depth)0.88f);
            }
            if (this._currentStat.Key != null)
            {
                this._addXPBar.depth = this.depth - 20;
                this._addXPBar.xscale = 1f;
                Graphics.Draw(this._addXPBar, vec2_1.x, vec2_1.y);
                this._fancyFont.DrawOutline(this._currentStat.Value.num == 0 ? this._currentStat.Key : this._currentStat.Value.num.ToString() + " " + this._currentStat.Key, vec2_1 + new Vec2((float)(-(this._addXPBar.width / 2) + 4), -2f), Color.White, Color.Black, this.depth - 10);
                Vec2 p1 = vec2_1 + new Vec2((float)(-(this._addXPBar.width / 2) + 2), -7.5f);
                Graphics.DrawLine(p1, p1 + new Vec2((float)(this._addXPBar.width - 5) * this._drain, 0.0f), Color.Lime, depth: (this._addXPBar.depth + 2));
                string text3 = ((int)((double)this._currentStat.Value.xp * (double)this._drain)).ToString() + "|DGBLUE|XP";
                this._fancyFont.DrawOutline(text3, vec2_1 + new Vec2((float)((double)(this._addXPBar.width / 2) - (double)this._fancyFont.GetWidth(text3) - 4.0), -2f), Colors.DGGreen, Color.Black, this.depth - 10);
            }
            foreach (XPPlus particle in this._particles)
            {
                int num18 = 20;
                if (particle.splash)
                    num18 = 40;
                float num19 = Math.Min((particle.position - particle.target).length, 30f) / 30f;
                this._xpPoint.scale = new Vec2(num19);
                this._xpPointOutline.scale = new Vec2(num19);
                this._xpPoint.color = particle.color;
                this._xpPoint.alpha = particle.alpha * num19;
                this._xpPoint.depth = this.depth + num18;
                Graphics.Draw(this._xpPoint, particle.position.x, particle.position.y);
                this._xpPointOutline.alpha = particle.alpha * num19;
                this._xpPointOutline.depth = this.depth + (num18 - 5);
                Graphics.Draw(this._xpPointOutline, particle.position.x, particle.position.y);
            }
            foreach (LittleHeart heart in this._hearts)
            {
                this._heart.alpha = heart.alpha;
                this._heart.depth = (Depth)0.98f;
                this._heart.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(this._heart, heart.position.x, heart.position.y);
            }
            int curLev = this._currentLevel;
            if (Profiles.experienceProfile.numLittleMen > 0)
                curLev = Profiles.experienceProfile.littleManLevel;
            if (curLev > 7)
                curLev = 7;
            if (this._currentLevel >= 2)
            {
                float num20 = (float)Math.Round((double)this._talk) + this._extraMouthOpen;
                int num21 = 0;
                if (curLev <= 4)
                    num21 = 1;
                if (curLev <= 3)
                    num21 = 2;
                if (curLev <= 2)
                {
                    if (curLev == 2)
                    {
                        this._egg.depth = (Depth)0.85f;
                        this._egg.yscale = 1f;
                        int num22 = 8;
                        Vec2 vec2_7 = new Vec2(this.x + num8, this.y - 29f + num3 + (float)num22 + (float)num21);
                        Graphics.Draw(this._egg, vec2_7.x, vec2_7.y, new Rectangle(0.0f, (float)(num22 + num21), 16f, (float)(16 - num22 - num21)));
                        Graphics.Draw(this._egg, this.x + num8, this.y - 29f + num3 - num20, new Rectangle(0.0f, 0.0f, 16f, (float)(num22 + num21)));
                        Vec2 center = this._egg.center;
                        this._egg.yscale = num20;
                        this._egg.center = center;
                    }
                }
                else
                {
                    this._littleMan.frame = UILevelBox.LittleManFrame(Profiles.experienceProfile.numLittleMen, curLev);
                    this._littleMan.depth = (Depth)0.85f;
                    this._littleMan.yscale = 1f;
                    this.littleManPos = new Vec2(this.x + num8, (float)((double)this.y - 29.0 + (double)num3 + 4.0) + (float)num21);
                    if (!this._inTaxi)
                    {
                        Graphics.Draw((Sprite)this._littleMan, this.littleManPos.x, this.littleManPos.y, new Rectangle(0.0f, (float)(4 + num21), 16f, (float)(12 - num21)));
                        Graphics.Draw((Sprite)this._littleMan, this.x + num8, this.y - 29f + num3 - num20, new Rectangle(0.0f, 0.0f, 16f, (float)(4 + num21)));
                        Vec2 center = this._littleMan.center;
                        this._littleMan.yscale = num20;
                        Graphics.Draw((Sprite)this._littleMan, this.x + num8, (float)((double)this.y - 29.0 + ((double)num3 - (double)num20) + 4.0) + (float)num21, new Rectangle(0.0f, (float)(4 + num21), 16f, 1f));
                        this._littleMan.center = center;
                    }
                }
                this._talkBubble.depth = (Depth)0.9f;
                string talkLine = this._talkLine;
                if (this._talkLine.Length > 0)
                {
                    Vec2 vec2_8 = new Vec2((float)((double)this.x + (double)num8 + 16.0), this.y - 28f + num3);
                    this._talkBubble.xscale = 1f;
                    Graphics.Draw(this._talkBubble, vec2_8.x, vec2_8.y, new Rectangle(0.0f, 0.0f, 8f, 8f));
                    float num23 = Graphics.GetStringWidth(talkLine) - 5f;
                    float y2 = Graphics.GetStringHeight(talkLine) + 2f;
                    this._talkBubble.xscale = num23;
                    Graphics.Draw(this._talkBubble, vec2_8.x + 8f, vec2_8.y, new Rectangle(5f, 0.0f, 1f, 2f));
                    Graphics.Draw(this._talkBubble, vec2_8.x + 8f, vec2_8.y + y2, new Rectangle(5f, 10f, 1f, 2f));
                    this._talkBubble.xscale = 1f;
                    Graphics.Draw(this._talkBubble, vec2_8.x, vec2_8.y + (y2 - 2f), new Rectangle(0.0f, 8f, 8f, 4f));
                    Graphics.Draw(this._talkBubble, (float)((double)vec2_8.x + (double)num23 + 8.0), vec2_8.y + (y2 - 2f), new Rectangle(8f, 8f, 4f, 4f));
                    Graphics.Draw(this._talkBubble, (float)((double)vec2_8.x + (double)num23 + 8.0), vec2_8.y, new Rectangle(8f, 0.0f, 4f, 4f));
                    Graphics.DrawRect(vec2_8 + new Vec2(5f, 2f), vec2_8 + new Vec2(num23 + 11f, y2), Color.White, (Depth)0.9f);
                    Graphics.DrawLine(vec2_8 + new Vec2(4.5f, 5f), vec2_8 + new Vec2(4.5f, y2 - 1f), Color.Black, depth: ((Depth)0.9f));
                    Graphics.DrawLine(vec2_8 + new Vec2(11.5f + num23, 4f), vec2_8 + new Vec2(11.5f + num23, y2 - 1f), Color.Black, depth: ((Depth)0.9f));
                    Graphics.DrawString(talkLine, vec2_8 + new Vec2(6f, 2f), Color.Black, (Depth)0.95f);
                }
            }
            if ((double)this._stampCardLerp > 0.00999999977648258)
            {
                float num24 = (float)(-((1.0 - (double)this._stampCardLerp) * 200.0) + Math.Sin((double)this._stampWobbleSin) * (double)this._stampWobble * 4.0);
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f * this._stampCardLerp, (Depth)0.96f);
                Graphics.Draw((Sprite)this._sandwichCard, this.x, this.y + num24, (Depth)0.97f);
                Random generator = Rando.generator;
                Random random = new Random(365023);
                Rando.generator = random;
                int num25 = Profiles.experienceProfile.numSandwiches % 6;
                if (Profiles.experienceProfile.numSandwiches > 0 && Profiles.experienceProfile.numSandwiches % 6 == 0 && this._finishingNewStamp)
                    num25 = 6;
                for (int index = 0; index < num25; ++index)
                {
                    float num26 = (float)(index % 2 * 16);
                    float num27 = (float)(index / 2 * 16);
                    this._sandwichStamp.angle = Rando.Float(-0.2f, 0.2f);
                    this._sandwichStamp.frame = Rando.Int(3);
                    Graphics.Draw((Sprite)this._sandwichStamp, this.x + 30f + num26 + Rando.Float(-2f, 2f), this.y - 15f + num27 + Rando.Float(-2f, 2f) + num24, (Depth)0.98f);
                    if (index == 5)
                    {
                        this._duckCoin.frame = 1;
                        this._duckCoin.alpha = (float)(1.0 - (double)Math.Max(this._coinLerp2 - 0.5f, 0.0f) * 2.0);
                        this._duckCoin.depth = (Depth)0.99f;
                        Graphics.Draw((Sprite)this._duckCoin, this.x + 30f + num26, (float)((double)this.y - 15.0 + (double)num27 + (double)num24 - (double)this._coinLerp2 * 18.0));
                    }
                }
                Rando.generator = random;
            }
            if (this._currentLevel >= 7)
            {
                this._milk.depth = (Depth)0.7f;
                this._milk.frame = (int)((double)this._milkValue / (double)this.milkNeed * 15.0);
                Graphics.Draw((Sprite)this._milk, this.x + 26f, this.y - 33f);
                Vec2 vec2_9 = this.position + new Vec2(-88f, 44f);
                int num28 = 0;
                foreach (Sprite littleEgg in this.littleEggs)
                {
                    littleEgg.depth = (Depth)0.85f;
                    Graphics.Draw(littleEgg, (float)((double)vec2_9.x + (double)(num28 * 23) - 3.0), vec2_9.y - 3f);
                    this._littleMan.frame = UILevelBox.LittleManFrame(Math.Max(Profiles.experienceProfile.numLittleMen - 8, 0) + num28, -1, bottomBar: true);
                    this._littleMan.depth = (Depth)0.9f;
                    this._littleMan.yscale = 1f;
                    Graphics.Draw((Sprite)this._littleMan, (float)((double)vec2_9.x + (double)(num28 * 23) + 3.0), vec2_9.y + 1f);
                    ++num28;
                }
            }
            float num29 = 0.0f;
            if (this._currentLevel >= 7)
                num29 = -12f;
            Vec2 vec2_10 = this.position + new Vec2(75.5f, 33f + num29);
            Vec2 p1_1 = vec2_10 + new Vec2(0.0f, -7f);
            if (this._currentLevel >= 4)
            {
                int littleManBucks = Profiles.experienceProfile.littleManBucks;
                string str2 = "|DGGREEN|$";
                string text4 = littleManBucks <= 9999 ? str2 + littleManBucks.ToString() : str2 + (littleManBucks / 1000).ToString() + "K";
                Graphics.DrawRect(vec2_10 + new Vec2(-16f, 9f), vec2_10 + new Vec2(15f, 18f), Color.Black, (Depth)0.89f);
                this._fancyFont.Draw(text4, vec2_10 + new Vec2(-16f, 9f) + new Vec2(30f - this._fancyFont.GetWidth(text4), 0.0f), Color.White, (Depth)0.9f);
                if (this._giveMoney > 0 && (double)this._giveMoneyRise < 0.949999988079071)
                {
                    string text5 = "+" + this._giveMoney.ToString();
                    Color dgGreen = Colors.DGGreen;
                    Color black = Color.Black;
                    this._fancyFont.DrawOutline(text5, vec2_10 + new Vec2(-16f, 9f) + new Vec2(30f - this._fancyFont.GetWidth(text5), (float)-(10.0 + (double)this._giveMoneyRise * 10.0)), dgGreen, black, (Depth)0.97f);
                }
                Vec2 vec2_11 = new Vec2();
                vec2_11.x = (float)(-Math.Sin((double)this.time * 12.0 * 6.28318548202515 - 3.14159274101257) * 8.0);
                vec2_11.y = (float)Math.Cos((double)this.time * 12.0 * 6.28318548202515 - 3.14159274101257) * 8f;
                Vec2 vec2_12 = new Vec2();
                vec2_12.x = (float)(-Math.Sin((double)this.time * 6.28318548202515 - 3.14159274101257) * 5.0);
                vec2_12.y = (float)Math.Cos((double)this.time * 6.28318548202515 - 3.14159274101257) * 5f;
                Graphics.DrawLine(p1_1, p1_1 + vec2_11, Color.Black, depth: ((Depth)0.9f));
                Graphics.DrawLine(p1_1, p1_1 + vec2_12, Color.Black, 1.5f, (Depth)0.9f);
                Random random = new Random(0);
                Random generator = Rando.generator;
                Rando.generator = random;
                for (int index = 0; index < Profiles.experienceProfile.currentDay; ++index)
                {
                    double num30 = (double)Rando.Float(1f);
                }
                Math.Floor((double)Profiles.experienceProfile.currentDay / 5.0);
                for (int index = 0; index < 5; ++index)
                {
                    float num31 = 0.0f;
                    if (index == 0)
                        num31 += 0.1f;
                    float num32 = Rando.Float(-0.1f, 0.1f);
                    int num33 = (int)(((double)num32 + 0.100000001490116) / 0.200000002980232 * 10.0);
                    if (this._popDay && index == 0 && (double)this._dayFallAway != 0.0)
                        this._weekDays.angle = this._ranRot;
                    else if (this._currentLevel < 6)
                        this._weekDays.angle = num32;
                    else
                        this._weekDays.angle = 0.0f;
                    if (num33 == 3 && this._currentLevel < 5)
                        this._weekDays.angle += 3.141593f;
                    float num34 = 0.0f;
                    if (index == 0)
                        num34 = this._dayFallAway * 100f;
                    float num35 = (float)(-(double)this._dayScroll * 26.0);
                    if (index == 0)
                    {
                        this._circle.depth = (Depth)(0.85f + num31);
                        this._circle.angle = this._weekDays.angle;
                        if (index == 0 && this._advancedDay)
                            Graphics.Draw((Sprite)this._circle, this.position.x - 71f + (float)(index * 28) + num35, this.position.y + 33f + num29 + num34, new Rectangle(0.0f, 0.0f, (float)this._circle.width * this._newCircleLerp, (float)this._circle.height));
                        else
                            Graphics.Draw((Sprite)this._circle, this.position.x - 71f + (float)(index * 28) + num35, this.position.y + 33f + num29 + num34);
                    }
                    this._weekDays.depth = (Depth)(0.83f + num31);
                    this._weekDays.frame = (Profiles.experienceProfile.currentDay + index) % 5;
                    this._weekDays.frame += (int)Math.Floor((double)(Profiles.experienceProfile.currentDay + index) / 20.0) % 4 * 6;
                    Graphics.Draw((Sprite)this._weekDays, this.position.x - 71f + (float)(index * 28) + num35, this.position.y + 33f + num34 + num29);
                    DayType day = this.GetDay(Profiles.experienceProfile.currentDay + index);
                    if (day != DayType.Empty)
                    {
                        this._days.depth = (Depth)(0.84f + num31);
                        this._days.frame = (int)day;
                        this._days.angle = this._weekDays.angle;
                        Graphics.Draw((Sprite)this._days, this.position.x - 71f + (float)(index * 28) + num35, (float)((double)this.position.y + (double)num29 + 33.0) + num34);
                    }
                }
                Rando.generator = generator;
            }
            if (UILevelBox._confirmMenu != null && UILevelBox._confirmMenu.open)
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f, (Depth)(275f * (float)Math.PI / 887f));
            if (FurniShopScreen.open)
            {
                Graphics.DrawRect(new Vec2(-1000f, -1000f), new Vec2(1000f, 1000f), Color.Black * 0.5f, (Depth)0.95f);
                FurniShopScreen.open = true;
                Vincent.Draw();
            }
            if ((double)this._taxiDrive > 0.0)
            {
                Vec2 vec2_13 = new Vec2((float)((double)this.position.x - 200.0 + (double)this._taxiDrive * 210.0), this.position.y - 33f);
                this._taxi.depth = (Depth)0.97f;
                Graphics.Draw(this._taxi, vec2_13.x, vec2_13.y);
                if (this._inTaxi)
                {
                    this._littleMan.frame = UILevelBox.LittleManFrame(Profiles.experienceProfile.numLittleMen, curLev);
                    Graphics.Draw((Sprite)this._littleMan, vec2_13.x - 16f, vec2_13.y - 8f, new Rectangle(0.0f, 0.0f, 16f, 6f));
                }
            }
            if ((double)this._intermissionSlide > 0.00999999977648258)
            {
                float x4 = (float)((double)this._intermissionSlide * 320.0 - 320.0);
                float y3 = 60f;
                Graphics.DrawRect(new Vec2(x4, y3), new Vec2(x4 + 320f, y3 + 30f), Color.Black, (Depth)0.98f);
                float x5 = (float)(320.0 - (double)this._intermissionSlide * 320.0);
                float num36 = 60f;
                Graphics.DrawRect(new Vec2(x5, num36 + 30f), new Vec2(x5 + 320f, num36 + 60f), Color.Black, (Depth)0.98f);
                string text6 = "ADVANCE DAY";
                switch (this.GetDay(Profiles.experienceProfile.currentDay))
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
                if (this._overrideSlide != null)
                    text6 = this._overrideSlide;
                this._bigFont.Draw(text6, new Vec2((float)((double)this._intermissionSlide * (320.0 + (double)Layer.HUD.width / 2.0 - (double)this._bigFont.GetWidth(text6) / 2.0) - 320.0), num36 + 18f), Color.White, (Depth)0.99f);
            }
            this._lastFill = num7;
            if (UILevelBox._confirmMenu == null)
                return;
            UILevelBox._confirmMenu.DoDraw();
        }
    }
}
