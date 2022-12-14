// Decompiled with JetBrains decompiler
// Type: DuckGame.UIGachaBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIGachaBox : UIMenu
    {
        private Sprite _frame;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private SpriteMap _gachaEgg;
        private Sprite _furni;
        private Sprite _star;
        private Furniture _contains;
        private float gachaY;
        private float gachaSpeed;
        public static bool useNumGachas;
        //private bool _flash;
        private float yOffset = 150f;
        public bool down = true;
        private float _downWait = 1f;
        private UIMenu _openOnClose;
        private SpriteMap _duckCoin;
        private bool _rare;
        private bool _rareCapsule;
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
        private bool doubleUpdating;
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

        public static Furniture GetRandomFurniture(int minRarity) => GetRandomFurniture(minRarity, 1)[0];

        public static List<Furniture> GetRandomFurniture(
          int minRarity,
          int num,
          float rarityMult = 1f,
          bool gacha = false,
          int numDupes = 0,
          bool avoidDupes = false,
          bool rareDupesChance = false)
        {
            List<Furniture> randomFurniture = new List<Furniture>();
            IOrderedEnumerable<Furniture> source = RoomEditor.AllFurnis().Where(x => x.rarity >= minRarity).OrderBy(x => Rando.Int(999999));
            for (int index = 0; index < num; ++index)
            {
                Furniture winner = null;
                Furniture furniture1 = null;
                Furniture furniture2 = null;
                List<int> intList = new List<int>();
                foreach (Furniture furniture3 in (IEnumerable<Furniture>)source)
                {
                    if (!gacha || furniture3.canGetInGacha)
                    {
                        if (furniture2 == null)
                            furniture2 = furniture3;
                        bool flag = Profiles.experienceProfile.GetNumFurnitures(furniture3.index) > 0;
                        int _max = 35;
                        if (furniture3.rarity >= Rarity.VeryRare)
                            _max = 25;
                        if (furniture3.rarity >= Rarity.SuperRare)
                            _max = 10;
                        if (useNumGachas && Global.data.numGachas % 8 == 0)
                            _max *= 4;
                        if (avoidDupes)
                            _max *= 8;
                        if (!flag || furniture3.type == FurnitureType.Prop && (Rando.Int(_max) == 0 || numDupes > 0))
                        {
                            if (furniture1 == null)
                                furniture1 = furniture3;
                            if (Profiles.experienceProfile.GetNumFurnitures(furniture3.index) <= 0 || Rando.Int(2) == 0)
                                intList.Add(furniture3.rarity);
                            if (furniture1 == null || furniture3.rarity < furniture1.rarity)
                                furniture1 = furniture3;
                            if (furniture2 == null || furniture3.rarity < furniture2.rarity)
                                furniture2 = furniture3;
                            if (winner == null || furniture3.rarity > winner.rarity)
                            {
                                int num1 = furniture3.rarity;
                                if (rareDupesChance & flag && furniture3.rarity > minRarity)
                                    num1 = (int)(num1 * 0.5);
                                if (furniture3.rarity == Rarity.Common || Rando.Int((int)(num1 * rarityMult)) == 0)
                                    winner = furniture3;
                            }
                        }
                    }
                }
                if (winner == null)
                    winner = furniture1;
                if (winner == null)
                    winner = furniture2;
                if (Profiles.experienceProfile.GetNumFurnitures(winner.index) > 0)
                    --numDupes;
                randomFurniture.Add(winner);
                if (index != num - 1)
                    source = source.Where(x => x != winner).OrderBy(x => Rando.Int(999999));
            }
            return randomFurniture;
        }

        public UIGachaBox(
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
            Graphics.fade = 1f;
            _frame = new Sprite("unlockFrame");
            _frame.CenterOrigin();
            _furni = new Sprite("furni/tub");
            _furni.center = new Vec2(_furni.width / 2, _furni.height);
            _star = new Sprite("prettyStar");
            _star.CenterOrigin();
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFontGacha");
            _gachaEgg = new SpriteMap("gachaEgg", 44, 36);
            bool flag = false;
            if (Rando.Int(10) == 5)
                flag = true;
            _contains = GetRandomFurniture(_rare ? Rarity.VeryVeryRare : Rarity.Common, 1, flag ? 0.75f : (_rare ? 0.75f : 1f), true)[0];
            _rareCapsule = _contains.rarity >= Rarity.VeryVeryRare;
            if (_rareCapsule)
            {
                _gachaEgg.frame = 36;
            }
            else
            {
                _gachaEgg.frame = Rando.Int(2) * 12;
                if (Rando.Int(1000) == 1)
                    _gachaEgg.frame += 9;
                else if (Rando.Int(500) == 1)
                    _gachaEgg.frame += 6;
                else if (Rando.Int(100) == 1)
                    _gachaEgg.frame += 3;
            }
            _gachaEgg.CenterOrigin();
        }

        public override void OnClose()
        {
            Profiles.Save(Profiles.experienceProfile);
            if (_openOnClose == null)
                return;
            MonoMain.pauseMenu = _openOnClose;
        }

        public override void Open() => base.Open();

        public override void UpdateParts()
        {
            if (!doubleUpdating && Input.Down("SELECT"))
            {
                doubleUpdating = true;
                UpdateParts();
                doubleUpdating = false;
            }
            if (yOffset < 1.0)
            {
                if (_insertCoin < 1.0)
                {
                    _insertCoinInc += 0.008f;
                    _insertCoin += _insertCoinInc;
                }
                else
                {
                    if (!_chinged)
                    {
                        SFX.Play("ching", pitch: Rando.Float(0.4f, 0.6f));
                        _chinged = true;
                    }
                    _insertCoin = 1f;
                    if (_afterInsertWait < 1.0)
                    {
                        _afterInsertWait += 0.32f;
                    }
                    else
                    {
                        if (_gachaWait >= 0.5 && !played)
                        {
                            played = true;
                            SFX.Play("gachaSound", pitch: Rando.Float(-0.1f, 0.1f));
                        }
                        _gachaWait += 0.1f;
                        if (_gachaWait >= 1.0)
                        {
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
                            _openWait += 0.019f;
                            if (_openWait >= 1.0)
                            {
                                if (!opened)
                                {
                                    opened = true;
                                    SFX.Play("gachaOpen", pitch: Rando.Float(0.1f, 0.3f));
                                    _gachaEgg.frame += 2;
                                }
                                _swapWait += 0.06f;
                                if (_swapWait >= 1.0)
                                {
                                    if (!_swapped)
                                    {
                                        SFX.Play("harp");
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
            yOffset = Lerp.FloatSmooth(yOffset, down ? 150f : 0f, 0.4f, 1.1f);
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
            if (_swapped && Input.Pressed("SELECT"))
            {
                HUD.CloseAllCorners();
                SFX.Play("resume", 0.6f);
                down = true;
            }
            base.UpdateParts();
        }

        public override void Draw()
        {
            y += yOffset;
            _frame.depth = -0.9f;
            Graphics.Draw(_frame, x, y);
            _frame.depth = -0.7f;
            Graphics.Draw(_frame, x, y, new Rectangle(0f, 0f, 125f, 36f));
            if (_swapped)
            {
                _contains.Draw(position + new Vec2(0f, 10f), -0.8f);
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
                _gachaEgg.depth = -0.8f;
                Graphics.Draw(_gachaEgg, x, y - 38f + gachaY);
            }
            string text1 = "@LWING@NEW TOY@RWING@";
            if (_rare)
                text1 = "@LWING@RARE TOY@RWING@";
            Vec2 vec2_1 = new Vec2((float)-(_font.GetWidth(text1) / 2.0), -42f);
            _font.DrawOutline(text1, position + vec2_1, _rare ? Colors.DGYellow : Color.White, Color.Black, depth + 2);
            string text2 = "  ???  ";
            if (_swapped)
                text2 = "} " + _contains.name + " }";
            _fancyFont.scale = new Vec2(1f, 1f);
            Vec2 vec2_2 = new Vec2((float)-(_fancyFont.GetWidth(text2) / 2.0), -25f);
            _fancyFont.DrawOutline(text2, position + vec2_2, _rare || _swapped && _rareCapsule ? Colors.DGYellow : Color.White, Color.Black, depth + 2);
            _fancyFont.scale = new Vec2(0.5f, 0.5f);
            if (_insertCoin > 0.01f)
            {
                _duckCoin.frame = _rare ? 1 : 0;
                _duckCoin.depth = -0.8f;
                Graphics.Draw(_duckCoin, x + 40f, (float)(y - 100.0 + _insertCoin * 65.0));
            }
            if (_swapped)
            {
                string text3 = _contains.description;
                int num = Profiles.experienceProfile.GetNumFurnitures(_contains.index) - 1;
                if (num > 0)
                    text3 = "I've already got " + (num - 1 >= numberNames.Count ? num.ToString() : numberNames[num - 1]) + " of these...";
                Vec2 vec2_3 = new Vec2((float)-(_fancyFont.GetWidth(text3) / 2.0), 38f);
                _fancyFont.DrawOutline(text3, position + vec2_3, num > 0 ? Colors.DGYellow : Colors.DGGreen, Color.Black, depth + 2, 0.5f);
            }
            y -= yOffset;
        }
    }
}
