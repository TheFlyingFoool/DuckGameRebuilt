// Decompiled with JetBrains decompiler
// Type: DuckGame.FurniShopScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => alpha >= 0.01f && base.visible;
            set => base.visible = value;
        }

        public FurniShopScreen()
          : base()
        {
            _tail = new Sprite("arcade/bubbleTail");
            layer = Layer.HUD;
        }

        public override void Initialize()
        {
            _plasma = new PlasmaLayer("PLASMA", -85);
            Layer.Add(_plasma);
            _treeLayer = new Layer("TREE", -95, new Camera());
            Layer.Add(_treeLayer);
        }

        public void OpenBuyConfirmation(UnlockData unlock)
        {
            if (_pauseGroup != null)
            {
                Level.Remove(_pauseGroup);
                _pauseGroup = null;
            }
            _confirm.value = false;
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _confirmMenu = new UIMenu("UNLOCK FEATURE", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@CANCEL  @SELECT@BUY");
            _confirmMenu.Add(new UIText(unlock.name, Color.Green), true);
            _confirmMenu.Add(new UIText(" ", Color.White), true);
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
                    if ((textVal.Length + str2.Length) * 8 > num)
                    {
                        _confirmMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
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
                _confirmMenu.Add(new UIText(textVal, Color.White, UIAlign.Left), true);
            _confirmMenu.Add(new UIText(" ", Color.White), true);
            _confirmMenu.Add(new UIMenuItem(Triggers.Cancel, new UIMenuActionCloseMenu(_pauseGroup), c: Colors.MenuOption, backButton: true), true);
            _confirmMenu.Add(new UIMenuItem("BUY UNLOCK |WHITE|(|LIME|" + unlock.cost.ToString() + "|WHITE| TICKETS)", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _confirm)), true);
            _confirmMenu.Close();
            _pauseGroup.Add(_confirmMenu, false);
            _pauseGroup.Close();
            Level.Add(_pauseGroup);
            for (int index = 0; index < 10; ++index)
            {
                _pauseGroup.Update();
                _confirmMenu.Update();
            }
            _pauseGroup.Open();
            _confirmMenu.Open();
            MonoMain.pauseMenu = _pauseGroup;
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
            if (alpha < 0.01f)
                return;
            Graphics.DrawRect(new Vec2(26f, 22f), new Vec2(Layer.HUD.width - 105f, Layer.HUD.height - 51f), new Color(20, 20, 20) * alpha * 0.7f, -0.9f);
            Vec2 p1 = new Vec2(20f, 8f);
            Vec2 vec2 = new Vec2(226f, 11f);
            Graphics.DrawRect(p1, p1 + vec2, Color.Black, (Depth)0.96f);
            string text = "what a name";
            Graphics.DrawString(text, p1 + new Vec2(((vec2.x - 27f) / 2f - Graphics.GetStringWidth(text) / 2f), 2f), new Color(163, 206, 39) * alpha, (Depth)0.97f);
            _tail.depth = (Depth)0.5f;
            _tail.alpha = alpha;
            _tail.flipH = false;
            _tail.flipV = false;
            Graphics.Draw(_tail, 222f, 18f);
            Chancy.alpha = alpha;
            Chancy.Draw();
        }
    }
}
