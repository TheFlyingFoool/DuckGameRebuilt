// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class UnlockScreen : Thing
    {
        private Sprite _tail;
        public bool quitOut;
        private UnlockTree _tree;
        private PlasmaLayer _plasma;
        private Layer _treeLayer;
        public static bool open;
        private UIComponent _pauseGroup;
        private UIMenu _confirmMenu;
        private MenuBoolean _confirm = new MenuBoolean();
        private UnlockData _tryBuy;

        public override bool visible
        {
            get => alpha >= 0.01f && base.visible;
            set => base.visible = value;
        }

        public UnlockScreen()
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
            _tree = new UnlockTree(this, _treeLayer);
            Level.Add(_tree);
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
            _confirmMenu.Add(new UIText(unlock.GetNameForDisplay(), Color.Green), true);
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
            _confirmMenu.Add(new UIMenuItem("CANCEL", new UIMenuActionCloseMenu(_pauseGroup), c: Colors.MenuOption, backButton: true), true);
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
            _tryBuy = unlock;
        }

        public void ChangeSpeech()
        {
            Chancy.Clear();
            string str = "";
            if (_tree.selected.type == UnlockType.Hat)
                str = "HAT";
            else if (_tree.selected.type == UnlockType.Level)
                str = "LEVEL";
            else if (_tree.selected.type == UnlockType.Modifier)
                str = "GAMEPLAY MODIFIER";
            else if (_tree.selected.type == UnlockType.Weapon)
                str = "WEAPON";
            else if (_tree.selected.type == UnlockType.Special)
                str = "SPECIAL";
            string line = _tree.selected.description + "^|ORANGE|" + str + "|WHITE| - ";
            if (_tree.selected.ProfileUnlocked(Profiles.active[0]))
                line += "|GREEN|UNLOCKED";
            else if (_tree.selected.parent != null)
            {
                List<UnlockData> treeLayer = Unlocks.GetTreeLayer(_tree.selected.parent.layer);
                bool flag = false;
                foreach (UnlockData unlockData in treeLayer)
                {
                    if (unlockData.children.Contains(_tree.selected) && !unlockData.ProfileUnlocked(Profiles.active[0]))
                    {
                        line += "|RED|LOCKED";
                        flag = true;
                        List<string> stringList = new List<string>();
                        stringList.Add("Wonder what this one is?");
                        stringList.Add("I think you're gonna like this one.");
                        stringList.Add("This one is just perfect for you.");
                        stringList.Add("Yeah this ones out of stock.");
                        line = stringList[Rando.Int(stringList.Count - 1)];
                        break;
                    }
                }
                if (!flag)
                    line = line + "|YELLOW|COSTS @TICKET@ " + Convert.ToString(_tree.selected.cost);
            }
            else
                line = line + "|YELLOW|COSTS @TICKET@ " + Convert.ToString(_tree.selected.cost);
            Chancy.Add(line);
        }

        public void SelectionChanged()
        {
            if (_tree.selected.AllParentsUnlocked(Profiles.active[0]))
            {
                if (_tree.selected.ProfileUnlocked(Profiles.active[0]))
                    HUD.AddCornerMessage(HUDCorner.BottomRight, "|LIME|UNLOCKED");
                else if (_tree.selected.cost <= Profiles.active[0].ticketCount)
                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@|LIME|BUY");
                else
                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@|RED|BUY");
            }
            else
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@|RED|LOCKED");
        }

        public void MakeActive()
        {
            HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            SelectionChanged();
        }

        public override void Update()
        {
            float num1 = Graphics.width / (_treeLayer.camera.width * 2f);
            float num2 = Graphics.height / (_treeLayer.camera.height * 2f);
            _treeLayer.scissor = new Rectangle(50f * num1, 44f * num1, Graphics.width - 180f * num1, 214f * num2);
            if (_confirmMenu != null && !_confirmMenu.open && _tryBuy != null)
            {
                if (_confirm.value)
                {
                    SFX.Play("ching");
                    Profiles.active[0].ticketCount -= _tryBuy.cost;
                    if (!Profiles.active[0].unlocks.Contains(_tryBuy.id))
                        Profiles.active[0].unlocks.Add(_tryBuy.id);
                    Profiles.Save(Profiles.active[0]);
                    SelectionChanged();
                }
                else
                    SFX.Play("resume");
                _tryBuy = null;
            }
            if (_confirmMenu != null && !_confirmMenu.open && _pauseGroup != null)
            {
                Level.Remove(_pauseGroup);
                _pauseGroup = null;
                _confirmMenu = null;
            }
            if (!Layer.Contains(_plasma))
                Layer.Add(_plasma);
            if (!Layer.Contains(_treeLayer))
                Layer.Add(_treeLayer);
            _plasma.alpha = alpha;
            _tree.alpha = alpha;
            if (alpha > 0.9f)
            {
                UnlockScreen.open = true;
                if (!Input.Pressed("CANCEL"))
                    return;
                SFX.Play("menu_back");
                quitOut = true;
            }
            else
                UnlockScreen.open = false;
        }

        public override void Draw()
        {
            if (alpha < 0.01f)
                return;
            Graphics.DrawRect(new Vec2(26f, 22f), new Vec2(Layer.HUD.width - 105f, 140f), new Color(20, 20, 20) * alpha * 0.7f, -0.9f);
            Vec2 p1 = new Vec2(20f, 8f);
            Vec2 vec2 = new Vec2(226f, 11f);
            Graphics.DrawRect(p1, p1 + vec2, Color.Black);
            bool flag1 = _tree.selected.ProfileUnlocked(Profiles.active[0]);
            bool flag2 = true;
            if (!_tree.selected.AllParentsUnlocked(Profiles.active[0]))
                flag2 = false;
            string text = _tree.selected.GetNameForDisplay();
            if (!flag2)
                text = "???";
            Graphics.DrawString(text, p1 + new Vec2((float)((vec2.x - 27.0) / 2.0 - Graphics.GetStringWidth(text) / 2.0), 2f), (flag1 ? new Color(163, 206, 39) : Color.Red) * alpha, (Depth)0.5f);
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
