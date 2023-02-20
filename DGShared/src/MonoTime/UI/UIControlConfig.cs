// Decompiled with JetBrains decompiler
// Type: DuckGame.UIControlConfig
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UIControlConfig : UIMenu
    {
        public int playerSelected;
        public int inputMode;
        public int inputConfigType;
        private List<UIBox> _playerBoxes = new List<UIBox>();
        private UIMenuItemToggle _configuringToggle;
        private List<string> inputTypes = new List<string>()
    {
      "GAMEPAD",
      "KEYBOARD"
    };
        private List<DeviceInputMapping> inputMaps = new List<DeviceInputMapping>();
        public UIMenu _confirmMenu;
        public UIMenu _warningMenu;
        private UIBox _controlBox;
        private UIMenu _openOnClose;
        private List<UIControlElement> _controlElements = new List<UIControlElement>();
        private static bool showWarning;
        private bool _showingMenu;

        public void SwitchPlayerProfile()
        {
            inputTypes.Clear();
            inputMaps.Clear();
            for (int index = 0; index < 4; ++index)
            {
                XInputPad device = Input.GetDevice<XInputPad>(index);
                if (device != null && device.isConnected)
                {
                    inputTypes.Add("XBOX GAMEPAD");
                    inputMaps.Add(Input.GetDefaultMapping(device.productName, device.productGUID).Clone());
                    break;
                }
            }
            //if (Input._dinputEnabled)
            //{
            //    List<string> stringList = new List<string>();
            //    for (int index = 0; index < 8; ++index)
            //    {
            //        if (DInput.GetState(index) != null)
            //        {
            //            string productName = DInput.GetProductName(index);
            //            string productGuid = DInput.GetProductGUID(index);
            //            string str = productName + productGuid;
            //            if (!stringList.Contains(str))
            //            {
            //                stringList.Add(str);
            //                inputMaps.Add(Input.GetDefaultMapping(productName, productGuid).Clone());
            //                if (productName.Length > 24)
            //                    productName = productName.Substring(0, 24);
            //                inputTypes.Add(productName);
            //            }
            //        }
            //    }
            //}
            inputTypes.Add("KEYBOARD P1");
            inputMaps.Add(Input.GetDefaultMapping("KEYBOARD P1", "").Clone());
            inputTypes.Add("KEYBOARD P2");
            inputMaps.Add(Input.GetDefaultMapping("KEYBOARD P2", "").Clone());
            inputConfigType = 0;
            SwitchConfigType();
        }

        public void SwitchConfigType()
        {
            foreach (UIControlElement controlElement in _controlElements)
            {
                if (inputConfigType < inputMaps.Count)
                    controlElement.inputMapping = inputMaps[inputConfigType];
            }
        }

        public void ResetToDefault()
        {
            if (inputConfigType < inputMaps.Count)
                inputMaps[inputConfigType] = Input.GetDefaultMapping(inputMaps[inputConfigType].deviceName, inputMaps[inputConfigType].deviceGUID, true).Clone();
            SwitchConfigType();
        }

        public void CloseMenu()
        {
            _showingMenu = false;
            Close();
            _openOnClose.Open();
            _confirmMenu.Close();
            _warningMenu.Close();
            inputMaps.Clear();
            HUD.CloseAllCorners();
        }

        public void CloseMenuSaving()
        {
            _showingMenu = false;
            foreach (DeviceInputMapping inputMap in inputMaps)
                Input.SetDefaultMapping(inputMap);
            Input.ApplyDefaultMappings();
            Input.Save();
            Close();
            _openOnClose.Open();
            _confirmMenu.Close();
            _warningMenu.Close();
            inputMaps.Clear();
            HUD.CloseAllCorners();
        }

        public UIControlConfig(
          UIMenu openOnClose,
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "",
          InputProfile conProfile = null)
          : base(title, xpos, ypos, wide, high, conString, conProfile)
        {
            _openOnClose = openOnClose;
            List<string> stringList1 = new List<string>()
      {
        "P1   ",
        "P2   ",
        "P3   ",
        "P4"
      };
            List<string> stringList2 = new List<string>()
      {
        "GAMEPAD",
        "KEYBOARD",
        "PAD + KEYS"
      };
            BitmapFont bitmapFont = new BitmapFont("smallBiosFontUI", 7, 5);
            UIBox uiBox = new UIBox(isVisible: false);
            _configuringToggle = new UIMenuItemToggle("", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(SwitchConfigType)), new FieldBinding(this, nameof(inputConfigType)), multi: inputTypes, compressedMulti: true, tiny: true);
            uiBox.Add(_configuringToggle, true);
            UIText uiText = new UIText(" ", Color.White);
            _controlElements.Add(new UIControlElement("|DGBLUE|{LEFT", Triggers.Left, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|/RIGHT", Triggers.Right, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|}UP", Triggers.Up, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|~DOWN", Triggers.Down, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|JUMP", Triggers.Jump, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|FIRE", Triggers.Shoot, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|GRAB", Triggers.Grab, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|QUACK", Triggers.Quack, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|STRAFE", Triggers.Strafe, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGBLUE|RAGDOLL", Triggers.Ragdoll, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            uiBox.Add(new UIText(" ", Color.White, heightAdd: -6f), true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|{MENU LEFT", Triggers.MenuLeft, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|/MENU RIGHT", Triggers.MenuRight, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|}MENU UP", Triggers.MenuUp, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|~MENU DOWN", Triggers.MenuDown, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|ACCEPT", Triggers.Select, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|MENU 1", Triggers.Menu1, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|MENU 2", Triggers.Menu2, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|CANCEL", Triggers.Cancel, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGPURPLE|START", Triggers.Start, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            uiBox.Add(new UIText(" ", Color.White, heightAdd: -6f), true);
            _controlElements.Add(new UIControlElement("|DGGREEN|MOVE STICK", Triggers.LeftStick, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGGREEN|LICK STICK", Triggers.RightStick, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGGREEN|QUACK PITCH", Triggers.LeftTrigger, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            _controlElements.Add(new UIControlElement("|DGGREEN|ZOOM   ", Triggers.RightTrigger, new DeviceInputMapping(), field: new FieldBinding(Options.Data, "sfxVolume")));
            uiBox.Add(_controlElements[_controlElements.Count - 1], true);
            UIMenuItem component1 = new UIMenuItem("|RED|REVERT TO DEFAULT", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(ResetToDefault)));
            component1.SetFont(bitmapFont);
            uiBox.Add(component1, true);
            UIText component2 = new UIText(" ", Color.White);
            component2.SetFont(bitmapFont);
            uiBox.Add(component2, true);
            UIText component3 = new UIText("Personal controls can be", Color.White);
            component3.SetFont(bitmapFont);
            uiBox.Add(component3, true);
            UIText component4 = new UIText("set in profile screen.", Color.White);
            component4.SetFont(bitmapFont);
            uiBox.Add(component4, true);
            _controlBox = uiBox;
            _playerBoxes.Add(uiBox);
            Add(_playerBoxes[0], true);
            _confirmMenu = new UIMenu("SAVE CHANGES?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT @CANCEL@BACK");
            _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(CloseMenuSaving))), true);
            _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(CloseMenu))), true);
            _confirmMenu.SetBackFunction(new UIMenuActionOpenMenu(_confirmMenu, this));
            _confirmMenu.Close();
            _warningMenu = new UIMenu("WARNING!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 180f, conString: "@SELECT@ I see...");
            UIMenu warningMenu1 = _warningMenu;
            UIText component5 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu1.Add(component5, true);
            UIMenu warningMenu2 = _warningMenu;
            UIText component6 = new UIText("One or more profiles have", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu2.Add(component6, true);
            UIMenu warningMenu3 = _warningMenu;
            UIText component7 = new UIText("|DGBLUE|custom controls|PREV| defined, which will", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu3.Add(component7, true);
            UIMenu warningMenu4 = _warningMenu;
            UIText component8 = new UIText("|DGRED|override|PREV| any controls set here!", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu4.Add(component8, true);
            UIMenu warningMenu5 = _warningMenu;
            UIText component9 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu5.Add(component9, true);
            UIMenu warningMenu6 = _warningMenu;
            UIText component10 = new UIText("If these controls are not working,", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu6.Add(component10, true);
            UIMenu warningMenu7 = _warningMenu;
            UIText component11 = new UIText("enter the hat console in the lobby", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu7.Add(component11, true);
            UIMenu warningMenu8 = _warningMenu;
            UIText component12 = new UIText("and press |DGORANGE|EDIT|PREV| on your profile name.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu8.Add(component12, true);
            UIMenu warningMenu9 = _warningMenu;
            UIText component13 = new UIText("Select |DGORANGE|CONTROLS|PREV|, select your desired", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu9.Add(component13, true);
            UIMenu warningMenu10 = _warningMenu;
            UIText component14 = new UIText("input device, go to |DGORANGE|PAGE 2|PREV|", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu10.Add(component14, true);
            UIMenu warningMenu11 = _warningMenu;
            UIText component15 = new UIText("and select |DGORANGE|RESET|PREV|.", Color.White, heightAdd: -4f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu11.Add(component15, true);
            UIMenu warningMenu12 = _warningMenu;
            UIText component16 = new UIText("", Color.White, heightAdd: -3f)
            {
                scale = new Vec2(0.5f)
            };
            warningMenu12.Add(component16, true);
            _warningMenu.SetAcceptFunction(new UIMenuActionOpenMenu(_warningMenu, this));
            _warningMenu.SetBackFunction(new UIMenuActionOpenMenu(_warningMenu, this));
            _warningMenu.Close();
        }

        public static void ResetWarning()
        {
            showWarning = false;
            foreach (Profile profile in Profiles.active)
            {
                if (profile.inputMappingOverrides != null && profile.inputMappingOverrides.Count > 0)
                {
                    showWarning = true;
                    break;
                }
            }
        }

        public override void Open()
        {
            SwitchPlayerProfile();
            base.Open();
        }

        public override void Update()
        {
            if (open)
            {
                if (!globalUILock && showWarning)
                {
                    new UIMenuActionOpenMenu(this, _warningMenu).Activate();
                    showWarning = false;
                    return;
                }
                if (!globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.OemTilde)))
                {
                    new UIMenuActionOpenMenu(this, _confirmMenu).Activate();
                    return;
                }
                if (Input.uiDevicesHaveChanged)
                {
                    SwitchPlayerProfile();
                    Input.uiDevicesHaveChanged = false;
                }
            }
            if (_controlBox.selection > 0 && _controlBox.selection < 17)
            {
                if (!_showingMenu && inputConfigType < inputMaps.Count && inputMaps[inputConfigType].deviceName != "KEYBOARD P1" && inputMaps[inputConfigType].deviceName != "KEYBOARD P2")
                {
                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@STYLE");
                    _showingMenu = true;
                }
            }
            else if (_showingMenu)
            {
                HUD.CloseAllCorners();
                _showingMenu = false;
            }
            base.Update();
        }
    }
}
