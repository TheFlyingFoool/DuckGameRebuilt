// Decompiled with JetBrains decompiler
// Type: DuckGame.FurniShopScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class FurniShopScreen : Thing
    {
        public static bool close;
        private Sprite _tail;
        public bool quitOut;
        private PlasmaLayer _plasma;
        private Layer _treeLayer;
        //private static bool _prevOpen;
        public static bool open;
        public static VincentProduct attemptBuy;
        public static bool giveYoYo;
        public static bool giveVooDoo;
        public static bool givePerimeterDefence;
        public static int attemptBuyIndex;
        private UIComponent _pauseGroup;
        private UIMenu _confirmMenu;
        private MenuBoolean _confirm = new MenuBoolean();
        //private UnlockData _tryBuy;

        public override bool visible
        {
            get => this.alpha >= 0.01f && base.visible;
            set => base.visible = value;
        }

        public FurniShopScreen()
          : base()
        {
            this._tail = new Sprite("arcade/bubbleTail");
            this.layer = Layer.HUD;
        }

        public override void Initialize()
        {
            this._plasma = new PlasmaLayer("PLASMA", -85);
            Layer.Add(_plasma);
            this._treeLayer = new Layer("TREE", -95, new Camera());
            Layer.Add(this._treeLayer);
        }

        public void OpenBuyConfirmation(UnlockData unlock)
        {
            if (this._pauseGroup != null)
            {
                Level.Remove(_pauseGroup);
                this._pauseGroup = null;
            }
            this._confirm.value = false;
            this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            this._confirmMenu = new UIMenu("UNLOCK FEATURE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@CANCEL  @SELECT@BUY");
            this._confirmMenu.Add(new UIText(unlock.name, Color.Green), true);
            this._confirmMenu.Add(new UIText(" ", Color.White), true);
            float num = 190f;
            string str1 = unlock.longDescription;
            string textVal = "";
            string str2 = "";
            while (true)
            {
                if (str1.Length > 0 && str1[0] != ' ')
                {
                    str2 += str1[0].ToString();
                }
                else
                {
                    if ((textVal.Length + str2.Length) * 8 > (double)num)
                    {
                        this._confirmMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
                        textVal = "";
                    }
                    if (textVal.Length > 0)
                        textVal += " ";
                    textVal += str2;
                    str2 = "";
                }
                if (str1.Length != 0)
                    str1 = str1.Remove(0, 1);
                else
                    break;
            }
            if (str2.Length > 0)
            {
                if (textVal.Length > 0)
                    textVal += " ";
                textVal += str2;
            }
            if (textVal.Length > 0)
                this._confirmMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
            this._confirmMenu.Add(new UIText(" ", Color.White), true);
            this._confirmMenu.Add(new UIMenuItem("CANCEL", new UIMenuActionCloseMenu(this._pauseGroup), c: Colors.MenuOption, backButton: true), true);
            this._confirmMenu.Add(new UIMenuItem("BUY UNLOCK |WHITE|(|LIME|" + unlock.cost.ToString() + "|WHITE| TICKETS)", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._confirm)), true);
            this._confirmMenu.Close();
            this._pauseGroup.Add(_confirmMenu, false);
            this._pauseGroup.Close();
            Level.Add(_pauseGroup);
            for (int index = 0; index < 10; ++index)
            {
                this._pauseGroup.Update();
                this._confirmMenu.Update();
            }
            this._pauseGroup.Open();
            this._confirmMenu.Open();
            MonoMain.pauseMenu = this._pauseGroup;
            SFX.Play("pause", 0.6f);
            //this._tryBuy = unlock;
        }

        public void ChangeSpeech() => Chancy.Clear();

        public void MakeActive()
        {
            HUD.AddCornerCounter(HUDCorner.TopRight, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            if ((double)this.alpha < 0.01f)
                return;
            Graphics.DrawRect(new Vec2(26f, 22f), new Vec2(Layer.HUD.width - 105f, Layer.HUD.height - 51f), new Color(20, 20, 20) * this.alpha * 0.7f, -0.9f);
            Vec2 p1 = new Vec2(20f, 8f);
            Vec2 vec2 = new Vec2(226f, 11f);
            Graphics.DrawRect(p1, p1 + vec2, Color.Black, (Depth)0.96f);
            string text = "what a name";
            Graphics.DrawString(text, p1 + new Vec2(((vec2.x - 27f) / 2f - Graphics.GetStringWidth(text) / 2f), 2f), new Color(163, 206, 39) * this.alpha, (Depth)0.97f);
            this._tail.depth = (Depth)0.5f;
            this._tail.alpha = this.alpha;
            this._tail.flipH = false;
            this._tail.flipV = false;
            Graphics.Draw(this._tail, 222f, 18f);
            Chancy.alpha = this.alpha;
            Chancy.Draw();
        }
    }
}
