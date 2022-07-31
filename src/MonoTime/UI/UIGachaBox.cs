// Decompiled with JetBrains decompiler
// Type: DuckGame.UIGachaBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public static Furniture GetRandomFurniture(int minRarity) => UIGachaBox.GetRandomFurniture(minRarity, 1)[0];

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
            IOrderedEnumerable<Furniture> source = RoomEditor.AllFurnis().Where<Furniture>(x => x.rarity >= minRarity).OrderBy<Furniture, int>(x => Rando.Int(999999));
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
                        if (UIGachaBox.useNumGachas && Global.data.numGachas % 8 == 0)
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
                    source = source.Where<Furniture>(x => x != winner).OrderBy<Furniture, int>(x => Rando.Int(999999));
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
            this._openOnClose = openOnClose;
            this._rare = rare;
            this._duckCoin = new SpriteMap("duckCoin", 18, 18);
            this._duckCoin.CenterOrigin();
            Graphics.fade = 1f;
            this._frame = new Sprite("unlockFrame");
            this._frame.CenterOrigin();
            this._furni = new Sprite("furni/tub");
            this._furni.center = new Vec2(this._furni.width / 2, _furni.height);
            this._star = new Sprite("prettyStar");
            this._star.CenterOrigin();
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFontGacha");
            this._gachaEgg = new SpriteMap("gachaEgg", 44, 36);
            bool flag = false;
            if (Rando.Int(10) == 5)
                flag = true;
            this._contains = UIGachaBox.GetRandomFurniture(this._rare ? Rarity.VeryVeryRare : Rarity.Common, 1, flag ? 0.75f : (this._rare ? 0.75f : 1f), true)[0];
            this._rareCapsule = this._contains.rarity >= Rarity.VeryVeryRare;
            if (this._rareCapsule)
            {
                this._gachaEgg.frame = 36;
            }
            else
            {
                this._gachaEgg.frame = Rando.Int(2) * 12;
                if (Rando.Int(1000) == 1)
                    this._gachaEgg.frame += 9;
                else if (Rando.Int(500) == 1)
                    this._gachaEgg.frame += 6;
                else if (Rando.Int(100) == 1)
                    this._gachaEgg.frame += 3;
            }
            this._gachaEgg.CenterOrigin();
        }

        public override void OnClose()
        {
            Profiles.Save(Profiles.experienceProfile);
            if (this._openOnClose == null)
                return;
            MonoMain.pauseMenu = _openOnClose;
        }

        public override void Open() => base.Open();

        public override void UpdateParts()
        {
            if (!this.doubleUpdating && Input.Down("SELECT"))
            {
                this.doubleUpdating = true;
                this.UpdateParts();
                this.doubleUpdating = false;
            }
            if (yOffset < 1.0)
            {
                if (_insertCoin < 1.0)
                {
                    this._insertCoinInc += 0.008f;
                    this._insertCoin += this._insertCoinInc;
                }
                else
                {
                    if (!this._chinged)
                    {
                        SFX.Play("ching", pitch: Rando.Float(0.4f, 0.6f));
                        this._chinged = true;
                    }
                    this._insertCoin = 1f;
                    if (_afterInsertWait < 1.0)
                    {
                        this._afterInsertWait += 0.32f;
                    }
                    else
                    {
                        if (_gachaWait >= 0.5 && !this.played)
                        {
                            this.played = true;
                            SFX.Play("gachaSound", pitch: Rando.Float(-0.1f, 0.1f));
                        }
                        this._gachaWait += 0.1f;
                        if (_gachaWait >= 1.0)
                        {
                            this.gachaSpeed += 0.25f;
                            if (gachaSpeed > 6.0)
                                this.gachaSpeed = 6f;
                            this.gachaY += this.gachaSpeed;
                            if (gachaY > 50.0 && gachaSpeed > 0.0)
                            {
                                if (gachaSpeed > 0.800000011920929)
                                    SFX.Play("gachaBounce", pitch: 0.2f);
                                this.gachaY = 50f;
                                this.gachaSpeed = (float)(-this.gachaSpeed * 0.400000005960464);
                            }
                            this._openWait += 0.019f;
                            if (_openWait >= 1.0)
                            {
                                if (!this.opened)
                                {
                                    this.opened = true;
                                    SFX.Play("gachaOpen", pitch: Rando.Float(0.1f, 0.3f));
                                    this._gachaEgg.frame += 2;
                                }
                                this._swapWait += 0.06f;
                                if (_swapWait >= 1.0)
                                {
                                    if (!this._swapped)
                                    {
                                        SFX.Play("harp");
                                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                                        Profiles.experienceProfile.SetNumFurnitures(_contains.index, Profiles.experienceProfile.GetNumFurnitures(_contains.index) + 1);
                                    }
                                    this._starGrow += 0.05f;
                                    this._swapped = true;
                                }
                            }
                        }
                    }
                }
            }
            this.yOffset = Lerp.FloatSmooth(this.yOffset, this.down ? 150f : 0f, 0.4f, 1.1f);
            if (this.down)
            {
                if (this._swapped)
                {
                    this.finished = true;
                    this.Close();
                }
                else
                {
                    this._downWait -= 0.06f;
                    if (_downWait <= 0.0)
                    {
                        this._downWait = 1f;
                        this.down = false;
                        SFX.Play("gachaGet", pitch: -0.4f);
                    }
                }
            }
            if (this._swapped && Input.Pressed("SELECT"))
            {
                HUD.CloseAllCorners();
                SFX.Play("resume", 0.6f);
                this.down = true;
            }
            base.UpdateParts();
        }

        public override void Draw()
        {
            this.y += this.yOffset;
            this._frame.depth = -0.9f;
            Graphics.Draw(this._frame, this.x, this.y);
            this._frame.depth = -0.7f;
            Graphics.Draw(this._frame, this.x, this.y, new Rectangle(0f, 0f, 125f, 36f));
            if (this._swapped)
            {
                this._contains.Draw(this.position + new Vec2(0f, 10f), -0.8f);
                if (_starGrow <= 1.0)
                {
                    this._star.depth = (Depth)0.9f;
                    this._star.scale = new Vec2((float)(2.5 + _starGrow * 3.0));
                    this._star.alpha = 1f - this._starGrow;
                    Graphics.Draw(this._star, this.x, this.y + 10f);
                }
            }
            else if (gachaY > 10.0)
            {
                this._gachaEgg.depth = -0.8f;
                Graphics.Draw(_gachaEgg, this.x, this.y - 38f + this.gachaY);
            }
            string text1 = "@LWING@NEW TOY@RWING@";
            if (this._rare)
                text1 = "@LWING@RARE TOY@RWING@";
            Vec2 vec2_1 = new Vec2((float)-(this._font.GetWidth(text1) / 2.0), -42f);
            this._font.DrawOutline(text1, this.position + vec2_1, this._rare ? Colors.DGYellow : Color.White, Color.Black, this.depth + 2);
            string text2 = "  ???  ";
            if (this._swapped)
                text2 = "} " + this._contains.name + " }";
            this._fancyFont.scale = new Vec2(1f, 1f);
            Vec2 vec2_2 = new Vec2((float)-(this._fancyFont.GetWidth(text2) / 2.0), -25f);
            this._fancyFont.DrawOutline(text2, this.position + vec2_2, this._rare || this._swapped && this._rareCapsule ? Colors.DGYellow : Color.White, Color.Black, this.depth + 2);
            this._fancyFont.scale = new Vec2(0.5f, 0.5f);
            if (_insertCoin > 0.00999999977648258)
            {
                this._duckCoin.frame = this._rare ? 1 : 0;
                this._duckCoin.depth = -0.8f;
                Graphics.Draw(_duckCoin, this.x + 40f, (float)(this.y - 100.0 + _insertCoin * 65.0));
            }
            if (this._swapped)
            {
                string text3 = this._contains.description;
                int num = Profiles.experienceProfile.GetNumFurnitures(_contains.index) - 1;
                if (num > 0)
                    text3 = "I've already got " + (num - 1 >= this.numberNames.Count ? num.ToString() : this.numberNames[num - 1]) + " of these...";
                Vec2 vec2_3 = new Vec2((float)-(this._fancyFont.GetWidth(text3) / 2.0), 38f);
                this._fancyFont.DrawOutline(text3, this.position + vec2_3, num > 0 ? Colors.DGYellow : Colors.DGGreen, Color.Black, this.depth + 2, 0.5f);
            }
            this.y -= this.yOffset;
        }
    }
}
