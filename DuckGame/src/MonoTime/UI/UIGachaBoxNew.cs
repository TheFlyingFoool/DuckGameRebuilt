// Decompiled with JetBrains decompiler
// Type: DuckGame.UIGachaBoxNew
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIGachaBoxNew : UIMenu
    {
        private Sprite _frame;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private SpriteMap _gachaEgg;
        private Sprite _furni;
        private Sprite _star;
        private Furniture _contains;
        private Sprite _whiteCircle;
        //private bool _flash;
        private float yOffset = -250f;
        private UIMenu _openOnClose;
        private SpriteMap _duckCoin;
        private SpriteMap _capsule;
        private SpriteMap _capsuleBorder;
        private bool _rare;
        private bool _rareCapsule;
        private Material _flatColor = new MaterialFlatColor();
        private MaterialRainbow _rainbowMaterial = new MaterialRainbow();
        private Sprite _rainbow;
        private Sprite _gachaMachine;
        private Sprite _gachaGlass;
        private Sprite _gachaDoor;
        private Sprite _gachaTwister;
        private Sprite _gachaTwisterShadow;
        private SpriteMap _gachaBall;
        private SpriteMap _coin;
        private Sprite _coinSlot;
        private List<Furniture> prizes = new List<Furniture>();
        private bool didSkipPrompt;
        public static bool skipping;
        public int numGenerate = 3;
        public int numGenerateRare = 3;
        //private string _oldSong;
        private bool played;
        private float _gachaWait;
        private float _openWait;
        public bool finished;
        private bool opened;
        private float _swapWait;
        private bool _swapped;
        private float _starGrow;
        private float _insertCoin;
        private float _insertCoinInc;
        private float _afterInsertWait;
        private bool _chinged;
        public bool down = true;
        private float _downWait = 1f;
        private float gachaY;
        private float gachaSpeed;
        private bool doubleUpdating;
        private Vec2 _eggOffset = Vec2.Zero;
        private Vec2 _toyPosition = Vec2.Zero;
        private Vec2 _toyVelocity = Vec2.Zero;
        private Vec2 _lastStick = Vec2.Zero;
        private float _toyAngle;
        private float _toyAngleLerp;
        private bool _coined;
        private float _initialWait;
        private bool didOpenToyCorner;
        private int _prizesGiven;
        private List<string> numberNames = new List<string>()
    {
      "one",
      "two",
      "three",
      "four",
      "five",
      "six",
      "seven",
      "eight",
      "nine",
      "ten"
    };
        //private float rott;
        private int seed = 359392;

        public UIGachaBoxNew(
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          bool rare = false,
          UIMenu openOnClose = null)
          : base("", xpos, ypos, wide, high)
        {
            _openOnClose = openOnClose;
            _rare = rare;
            _duckCoin = new SpriteMap("duckCoin", 18, 18);
            _duckCoin.CenterOrigin();
            _gachaMachine = new Sprite("arcade/gotcha/machine");
            _gachaMachine.CenterOrigin();
            _gachaGlass = new Sprite("arcade/gotcha/glass");
            _gachaGlass.CenterOrigin();
            _gachaDoor = new Sprite("arcade/gotcha/door");
            _gachaDoor.CenterOrigin();
            _gachaTwister = new Sprite("arcade/gotcha/twister");
            _gachaTwister.CenterOrigin();
            _gachaBall = new SpriteMap("arcade/gotcha/balls", 40, 42);
            _gachaBall.CenterOrigin();
            _gachaTwisterShadow = new Sprite("arcade/gotcha/twisterShadow");
            _gachaTwisterShadow.CenterOrigin();
            _whiteCircle = new Sprite("furni/whiteCircle");
            _whiteCircle.CenterOrigin();
            _coin = new SpriteMap("arcade/gotcha/coin", 22, 22);
            _coin.CenterOrigin();
            _coinSlot = new Sprite("arcade/gotcha/coinSlot");
            _coinSlot.CenterOrigin();
            _rainbow = new Sprite("arcade/rainbow");
            Graphics.fade = 1f;
            _frame = new Sprite("unlockFrame");
            _frame.CenterOrigin();
            _furni = new Sprite("furni/stone");
            _furni.center = new Vec2(_furni.width / 2, _furni.height);
            _star = new Sprite("prettyStar");
            _star.CenterOrigin();
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFontGacha");
            _gachaEgg = new SpriteMap("gachaEgg", 44, 36);
            _capsule = new SpriteMap("arcade/egg", 40, 23);
            _capsule.CenterOrigin();
            _capsuleBorder = new SpriteMap("arcade/eggBorder", 66, 65);
            _capsuleBorder.CenterOrigin();
            _rare = false;
            numGenerate = MonoMain.core.gachas;
            numGenerateRare = MonoMain.core.rareGachas;
            for (int index = 0; index < numGenerate; ++index)
            {
                UIGachaBox.useNumGachas = true;
                Furniture furniture = UIGachaBox.GetRandomFurniture(Rarity.Common, 1, gacha: true)[0];
                UIGachaBox.useNumGachas = false;
                ++Global.data.numGachas;
                furniture.ballRot = Rando.Float(360f);
                furniture.rareGen = false;
                prizes.Add(furniture);
            }
            for (int index = 0; index < numGenerateRare; ++index)
            {
                UIGachaBox.useNumGachas = true;
                Furniture furniture = UIGachaBox.GetRandomFurniture(Rarity.VeryVeryRare, 1, 0.4f, true).OrderBy(x => -x.rarity).ElementAt(0);
                UIGachaBox.useNumGachas = false;
                ++Global.data.numGachas;
                furniture.ballRot = Rando.Float(360f);
                furniture.rareGen = true;
                prizes.Add(furniture);
            }
            for (int index = 0; index < 3; ++index)
            {
                UIGachaBox.useNumGachas = true;
                Furniture furniture = UIGachaBox.GetRandomFurniture(Rarity.Common, 1, gacha: true)[0];
                UIGachaBox.useNumGachas = false;
                ++Global.data.numGachas;
                furniture.ballRot = Rando.Float(360f);
                furniture.rareGen = false;
                prizes.Add(furniture);
            }
            if (skipping)
            {
                while (prizes.Count > 3)
                {
                    LoadNextPrize();
                    Profiles.experienceProfile.SetNumFurnitures(_contains.index, Profiles.experienceProfile.GetNumFurnitures(_contains.index) + 1);
                    prizes.RemoveAt(0);
                }
                SFX.Play("harp");
                skipping = false;
            }
            LoadNextPrize();
            _gachaEgg.CenterOrigin();
        }

        public int FigureFrame(Furniture f)
        {
            if (f.rarity >= Rarity.SuperRare)
                return 2;
            return f.rarity >= Rarity.VeryVeryRare ? 0 : 1;
        }

        public void LoadNextPrize()
        {
            _contains = prizes.ElementAt(0);
            _capsule.frame = FigureFrame(_contains);
        }

        public override void OnClose()
        {
            MonoMain.core.gachas = 0;
            MonoMain.core.rareGachas = 0;
            numGenerate = 0;
            numGenerateRare = 0;
            Profiles.Save(Profiles.experienceProfile);
            if (_openOnClose == null)
                return;
            MonoMain.pauseMenu = _openOnClose;
        }

        public override void Open() => base.Open();

        public override void UpdateParts()
        {
            if (Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0 && Input.Pressed("START"))
            {
                skipping = true;
                SFX.Play("dacBang");
            }
            if (skipping)
            {
                while (prizes.Count > 3)
                {
                    LoadNextPrize();
                    Profiles.experienceProfile.SetNumFurnitures(_contains.index, Profiles.experienceProfile.GetNumFurnitures(_contains.index) + 1);
                    prizes.RemoveAt(0);
                }
                skipping = false;
                finished = true;
                Close();
                HUD.CloseAllCorners();
            }
            else
            {
                if (!didSkipPrompt)
                {
                    if (Profiles.experienceProfile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0)
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@START@SKIP");
                    didSkipPrompt = true;
                }
                if (!doubleUpdating && Input.Down("SELECT"))
                {
                    doubleUpdating = true;
                    UpdateParts();
                    doubleUpdating = false;
                }
                if (!down && yOffset > -1.0)
                {
                    if (_initialWait < 1.0)
                        _initialWait += 0.1f;
                    else if (_insertCoin < 1.0)
                    {
                        if (!_chinged)
                        {
                            SFX.Play("ching", pitch: Rando.Float(0.4f, 0.6f));
                            _chinged = true;
                        }
                        _insertCoinInc += 0.008f;
                        _insertCoin += _insertCoinInc;
                    }
                    else
                    {
                        _insertCoin = 1f;
                        if (_afterInsertWait < 1.0)
                        {
                            _afterInsertWait += 0.2f;
                        }
                        else
                        {
                            if (_gachaWait >= 0.2f && !played)
                            {
                                played = true;
                                SFX.Play("gachaSound", pitch: Rando.Float(-0.1f, 0.1f));
                            }
                            if (!_coined && _gachaWait > 0.2f)
                            {
                                SFX.Play("gachaCoin", pitch: Rando.Float(0.4f, 0.6f));
                                _coined = true;
                            }
                            _gachaWait += 0.06f;
                            if (_gachaWait >= 1.0)
                            {
                                _gachaWait = 1f;
                                gachaSpeed += 0.25f;
                                if (gachaSpeed > 6.0)
                                    gachaSpeed = 6f;
                                gachaY += gachaSpeed;
                                if (gachaY > 50.0 && gachaSpeed > 0.0)
                                {
                                    if (gachaSpeed > 0.8f)
                                        SFX.Play("gachaBounce", pitch: 0.2f);
                                    gachaY = 50f;
                                    gachaSpeed = -gachaSpeed * 0.04f;
                                }
                                float num = 8f;
                                _toyVelocity.y += 0.2f;
                                Vec2 toyPosition1 = _toyPosition;
                                if (toyPosition1.length > num)
                                {
                                    Vec2 toyPosition2 = _toyPosition;
                                    _toyPosition = toyPosition1.normalized * num;
                                    Vec2 vec2 = _toyPosition - toyPosition2;
                                    _toyVelocity += vec2;
                                    if (vec2.length > 1.0)
                                        SFX.Play("gachaBounce", pitch: (0.7f + Rando.Float(0.2f)));
                                    _toyAngleLerp = Maths.PointDirection(Vec2.Zero, _toyPosition);
                                }
                                Vec2 leftStick = InputProfile.active.leftStick;
                                if (InputProfile.active.lastActiveDevice is Keyboard)
                                {
                                    if (InputProfile.active.Down("LEFT"))
                                        leftStick.x = -1f;
                                    if (InputProfile.active.Down("RIGHT"))
                                        leftStick.x = 1f;
                                    if (InputProfile.active.Down("UP"))
                                        leftStick.y = 1f;
                                    if (InputProfile.active.Down("DOWN"))
                                        leftStick.y = -1f;
                                }
                                _toyVelocity += (_lastStick - leftStick) * new Vec2(2f, -2f);
                                _lastStick = leftStick;
                                _toyVelocity.x = Math.Max(Math.Min(_toyVelocity.x, 3f), -3f);
                                _toyVelocity.y = Math.Max(Math.Min(_toyVelocity.y, 3f), -3f);
                                _toyPosition += _toyVelocity;
                                if (!opened)
                                {
                                    _toyAngle = Lerp.FloatSmooth(_toyAngle, _toyAngleLerp, 0.1f);
                                    _eggOffset = Lerp.Vec2Smooth(_eggOffset, leftStick * 8f, 0.3f);
                                }
                                else
                                {
                                    _toyAngle = Lerp.FloatSmooth(_toyAngle, -90f, 0.1f);
                                    _eggOffset = Lerp.Vec2Smooth(_eggOffset, Vec2.Zero, 0.3f);
                                    _toyPosition = Lerp.Vec2Smooth(_toyPosition, Vec2.Zero, 0.3f);
                                }
                                _openWait += 0.029f;
                                if (_openWait >= 1.0)
                                {
                                    if (!didOpenToyCorner)
                                    {
                                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@OPEN TOY");
                                        didOpenToyCorner = true;
                                    }
                                    if (Input.Pressed("SELECT") && !opened)
                                    {
                                        opened = true;
                                        SFX.Play("gachaOpen", pitch: Rando.Float(0.1f, 0.3f));
                                        _gachaEgg.frame += 2;
                                    }
                                    if (opened)
                                    {
                                        _swapWait += 0.06f;
                                        if (_swapWait >= 1.0)
                                        {
                                            if (!_swapped)
                                            {
                                                SFX.Play("harp");
                                                HUD.CloseAllCorners();
                                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                                                Profiles.experienceProfile.SetNumFurnitures(_contains.index, Profiles.experienceProfile.GetNumFurnitures(_contains.index) + 1);
                                            }
                                            _starGrow += 0.05f;
                                            _swapped = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                yOffset = Lerp.FloatSmooth(yOffset, down ? -250f : 0f, 0.4f);
                if (down)
                {
                    if (_swapped)
                    {
                        finished = true;
                        Close();
                    }
                    else
                    {
                        _downWait -= 0.06f;
                        if (_downWait <= 0.0)
                        {
                            _downWait = 1f;
                            down = false;
                            SFX.Play("gachaGet", pitch: -0.4f);
                        }
                    }
                }
                if (!down && _swapped && Input.Pressed("SELECT"))
                {
                    played = false;
                    _gachaWait = 0f;
                    _openWait = 0f;
                    finished = false;
                    opened = false;
                    _swapWait = 0f;
                    _swapped = false;
                    _starGrow = 0f;
                    _insertCoin = 0f;
                    _insertCoinInc = 0f;
                    _afterInsertWait = 0f;
                    _chinged = false;
                    gachaY = 0f;
                    gachaSpeed = 0f;
                    doubleUpdating = false;
                    ++_prizesGiven;
                    _eggOffset = Vec2.Zero;
                    _toyPosition = Vec2.Zero;
                    _toyVelocity = Vec2.Zero;
                    _lastStick = Vec2.Zero;
                    _toyAngle = 0f;
                    _toyAngleLerp = 0f;
                    _coined = false;
                    _initialWait = 0f;
                    didOpenToyCorner = false;
                    HUD.CloseAllCorners();
                    SFX.Play("resume", 0.6f);
                    if (prizes.Count > 4)
                    {
                        prizes.RemoveAt(0);
                        LoadNextPrize();
                    }
                    else
                    {
                        down = true;
                        _swapped = true;
                    }
                }
                base.UpdateParts();
            }
        }

        public override void Draw()
        {
            if (animating)
                return;
            y += yOffset;
            Random generator = Rando.generator;
            Rando.generator = new Random(seed);
            _gachaMachine.depth = -0.8f;
            Graphics.Draw(_gachaMachine, x - 14f, y);
            _coinSlot.depth = -0.795f;
            Graphics.Draw(_coinSlot, x - 13f, y - 13f);
            _coin.depth = (Depth)0.9f;
            for (int index = 0; index < numGenerate + numGenerateRare - (_prizesGiven + 1); ++index)
            {
                _coin.frame = numGenerate - (_prizesGiven + 1) <= 0 || index >= numGenerate - (_prizesGiven + 1) ? 1 : 0;
                _coin.depth = (Depth)(float)(0.9f - index * 0.01f);
                Graphics.Draw(_coin, 16 + index * 4, 16f);
            }
            _coin.frame = !_contains.rareGen ? 0 : 1;
            float num1 = Math.Min(_gachaWait * 2f, 1f);
            _coin.depth = -0.798f;
            Graphics.Draw(_coin, (float)(x - 15.0 + num1 * 21.0), (float)(y - 25.0 - 40.0 * (1.0 - _insertCoin) + num1 * 4.0));
            _gachaGlass.depth = -0.9f;
            Graphics.Draw(_gachaGlass, x - 14f, y - 10f);
            _gachaDoor.depth = -0.84f;
            Graphics.Draw(_gachaDoor, x - 14f, y);
            Vec2 vec2_1 = Vec2.Zero;
            _gachaBall.depth = -0.85f;
            _gachaBall.alpha = 0.3f;
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num2 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num2 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num2 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x - 56f + vec2_1.x, y - 54f + vec2_1.y);
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num3 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num3 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num3 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x - 26f + vec2_1.x, y - 74f + vec2_1.y);
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num4 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num4 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num4 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x - 62f + vec2_1.x, y - 94f + vec2_1.y);
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num5 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num5 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num5 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x + 6f + vec2_1.x, y - 44f + vec2_1.y);
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num6 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num6 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num6 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x + 31f + vec2_1.x, y - 64f + vec2_1.y);
            _gachaBall.angleDegrees = Rando.Float(360f);
            _gachaBall.frame = Rando.Int(2);
            float num7 = Rando.Float(4f, 8f);
            vec2_1 = new Vec2((float)Math.Sin(_gachaWait * num7 + Rando.Float(4f)) * Rando.Float(1f, 2f), (float)Math.Sin(_gachaWait * num7 + Rando.Float(4f)) * Rando.Float(1f, 2f));
            Graphics.Draw(_gachaBall, x + 8f + vec2_1.x, y - 92f + vec2_1.y);
            _gachaBall.angleDegrees = prizes[2].ballRot;
            _gachaBall.frame = FigureFrame(prizes[2]);
            Graphics.Draw(_gachaBall, (float)(x + 31.0 - 42.0 + _gachaWait * 42.0), (float)(y - 16.0 - 12.0 + _gachaWait * 12.0));
            _gachaBall.angleDegrees = prizes[1].ballRot;
            _gachaBall.frame = FigureFrame(prizes[1]);
            Graphics.Draw(_gachaBall, x + 31f, (float)(y - 16.0 + _gachaWait * 42.0));
            _gachaBall.angleDegrees = prizes[0].ballRot;
            _gachaBall.frame = FigureFrame(prizes[0]);
            Graphics.Draw(_gachaBall, (float)(x + 31.0 - _gachaWait * 42.0), (float)(y - 16.0 + 42.0));
            _gachaBall.alpha = 1f;
            _gachaBall.angleDegrees = 0f;
            _gachaBall.frame = Rando.Int(2);
            _gachaTwister.angleDegrees = _gachaTwisterShadow.angleDegrees = _gachaWait * 360f;
            Vec2 vec2_2 = new Vec2(0f, -4f);
            _gachaTwister.depth = -0.1f;
            Graphics.Draw(_gachaTwister, x - 14f + vec2_2.x, y + vec2_2.y);
            Vec2 vec2_3 = new Vec2(2f, 2f);
            _gachaTwisterShadow.depth = -0.11f;
            _gachaTwisterShadow.alpha = 0.5f;
            Graphics.Draw(_gachaTwisterShadow, x - 14f + vec2_2.x + vec2_3.x, y + vec2_2.y + vec2_3.y);
            Material material1 = Graphics.material;
            Graphics.material = _rainbowMaterial;
            _rainbowMaterial.offset += 0.05f;
            _rainbowMaterial.offset2 += 0.02f;
            _rainbow.alpha = 0.25f;
            _rainbow.depth = -0.95f;
            Graphics.Draw(_rainbow, 0f, 0f);
            Graphics.material = material1;
            Rando.generator = generator;
            _frame.depth = -0.9f;
            if (_swapped)
            {
                _contains.Draw(position + new Vec2(0f, 10f), depth - 20);
                _whiteCircle.color = _contains.group.color;
                _whiteCircle.depth = depth - 30;
                Graphics.Draw(_whiteCircle, position.x, position.y + 10f);
                if (_starGrow <= 1.0)
                {
                    _star.depth = (Depth)0.9f;
                    _star.scale = new Vec2((float)(2.5 + _starGrow * 3.0));
                    _star.alpha = 1f - _starGrow;
                    Graphics.Draw(_star, x, y + 10f);
                }
            }
            else if (gachaY > 10.0)
            {
                Vec2 vec2_4 = new Vec2(-25f, 40f);
                float num8 = 0f;
                if (opened)
                    num8 = 3f;
                _capsule.depth = -0.84f;
                Graphics.Draw(_capsule, x + _eggOffset.x + vec2_4.x, (float)(y - 38.0 + gachaY - _eggOffset.y - (10.0 + num8)) + vec2_4.y);
                Material material2 = Graphics.material;
                Graphics.material = _flatColor;
                _contains.Draw(new Vec2(x + _eggOffset.x + _toyPosition.x + vec2_4.x, (float)(y - 38.0 + gachaY - _eggOffset.y - 10.0 + _toyPosition.y + 8.0) + vec2_4.y), -0.835f, affectScale: true, halfscale: (!_swapped), angle: Maths.DegToRad(_toyAngle + 90f));
                Graphics.material = material2;
                _capsule.depth = -0.83f;
                _capsule.frame += 3;
                Graphics.Draw(_capsule, x + _eggOffset.x + vec2_4.x, (float)(y - 38.0 + gachaY - _eggOffset.y + (11.0 + num8)) + vec2_4.y, new Rectangle(0f, 2f, _capsule.width, _capsule.height - 2));
                _capsule.frame -= 3;
                if (gachaY > 30.0 && !opened)
                {
                    _capsuleBorder.depth = -0.81f;
                    _capsuleBorder.frame = 0;
                    Graphics.Draw(_capsuleBorder, x + _eggOffset.x + vec2_4.x, (float)(y - 38.0 + gachaY - _eggOffset.y - 2.0) + vec2_4.y);
                }
            }
            if (_swapped)
            {
                string text1 = "@LWING@NEW TOY@RWING@";
                if (_rare)
                    text1 = "@LWING@RARE TOY@RWING@";
                Vec2 vec2_5 = new Vec2((float)-(_font.GetWidth(text1) / 2.0), -42f);
                string text2 = "  ???  ";
                if (_swapped)
                    text2 = "} " + _contains.name + " }";
                _fancyFont.scale = new Vec2(1f, 1f);
                Vec2 vec2_6 = new Vec2((float)-(_fancyFont.GetWidth(text2) / 2.0), -25f);
                _fancyFont.DrawOutline(text2, position + vec2_6, _rare || _swapped && _rareCapsule ? Colors.DGYellow : Color.White, Color.Black, depth + 2);
                Graphics.DrawRect(position + new Vec2((float)-(_fancyFont.GetWidth(text2) / 2.0 + 4.0), -26f), position + new Vec2((float)(_fancyFont.GetWidth(text2) / 2.0 + 4.0), -14f), Color.Black, depth - 4);
                _fancyFont.scale = new Vec2(0.5f, 0.5f);
                if (_insertCoin > 0.01f)
                {
                    _duckCoin.frame = _rare ? 1 : 0;
                    _duckCoin.depth = -0.8f;
                    Graphics.Draw(_duckCoin, x + 40f, (float)(y - 100.0 + _insertCoin * 65.0));
                }
                string text3 = _contains.description;
                int num9 = Profiles.experienceProfile.GetNumFurnitures(_contains.index) - 1;
                if (num9 > 0)
                    text3 = "I've already got " + (num9 - 1 >= numberNames.Count ? (num9 - 1).ToString() : numberNames[num9 - 1]) + " of these...";
                Vec2 vec2_7 = new Vec2((float)-(_fancyFont.GetWidth(text3) / 2.0), 38f);
                _fancyFont.DrawOutline(text3, position + vec2_7, num9 > 0 ? Colors.DGYellow : Colors.DGGreen, Color.Black, depth + 2, 0.5f);
                Graphics.DrawRect(position + new Vec2((float)-(_fancyFont.GetWidth(text3) / 2.0 + 4.0), 37f), position + new Vec2((float)(_fancyFont.GetWidth(text3) / 2.0 + 4.0), 44f), Color.Black, depth - 4);
                Graphics.DrawRect(new Vec2(-100f, -100f), new Vec2(2000f, 2000f), Color.Black * 0.6f, depth - 100);
            }
            y -= yOffset;
        }
    }
}
