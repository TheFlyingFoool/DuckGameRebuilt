// Decompiled with JetBrains decompiler
// Type: DuckGame.UIControlConfig
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.inputTypes.Clear();
            this.inputMaps.Clear();
            for (int index = 0; index < 4; ++index)
            {
                XInputPad device = Input.GetDevice<XInputPad>(index);
                if (device != null && device.isConnected)
                {
                    this.inputTypes.Add("XBOX GAMEPAD");
                    this.inputMaps.Add(Input.GetDefaultMapping(device.productName, device.productGUID).Clone());
                    break;
                }
            }
            if (Input._dinputEnabled)
            {
                List<string> stringList = new List<string>();
                for (int index = 0; index < 8; ++index)
                {
                    if (DInput.GetState(index) != null)
                    {
                        string productName = DInput.GetProductName(index);
                        string productGuid = DInput.GetProductGUID(index);
                        string str = productName + productGuid;
                        if (!stringList.Contains(str))
                        {
                            stringList.Add(str);
                            this.inputMaps.Add(Input.GetDefaultMapping(productName, productGuid).Clone());
                            if (productName.Length > 24)
                                productName = productName.Substring(0, 24);
                            this.inputTypes.Add(productName);
                        }
                    }
                }
            }
            this.inputTypes.Add("KEYBOARD P1");
            this.inputMaps.Add(Input.GetDefaultMapping("KEYBOARD P1", "").Clone());
            this.inputTypes.Add("KEYBOARD P2");
            this.inputMaps.Add(Input.GetDefaultMapping("KEYBOARD P2", "").Clone());
            this.inputConfigType = 0;
            this.SwitchConfigType();
        }

        public void SwitchConfigType()
        {
            foreach (UIControlElement controlElement in this._controlElements)
            {
                if (this.inputConfigType < this.inputMaps.Count)
                    controlElement.inputMapping = this.inputMaps[this.inputConfigType];
            }
        }

        public void ResetToDefault()
        {
            if (this.inputConfigType < this.inputMaps.Count)
                this.inputMaps[this.inputConfigType] = Input.GetDefaultMapping(this.inputMaps[this.inputConfigType].deviceName, this.inputMaps[this.inputConfigType].deviceGUID, true).Clone();
            this.SwitchConfigType();
        }

        public void CloseMenu()
        {
            this._showingMenu = false;
            this.Close();
            this._openOnClose.Open();
            this._confirmMenu.Close();
            this._warningMenu.Close();
            this.inputMaps.Clear();
            HUD.CloseAllCorners();
        }

        public void CloseMenuSaving()
        {
            this._showingMenu = false;
            foreach (DeviceInputMapping inputMap in this.inputMaps)
                Input.SetDefaultMapping(inputMap);
            Input.ApplyDefaultMappings();
            Input.Save();
            this.Close();
            this._openOnClose.Open();
            this._confirmMenu.Close();
            this._warningMenu.Close();
            this.inputMaps.Clear();
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
            this._openOnClose = openOnClose;
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
            this._configuringToggle = new UIMenuItemToggle("", (UIMenuAction)new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.SwitchConfigType)), new FieldBinding((object)this, nameof(inputConfigType)), multi: this.inputTypes, compressedMulti: true, tiny: true);
            uiBox.Add((UIComponent)this._configuringToggle, true);
            UIText uiText = new UIText(" ", Color.White);
            this._controlElements.Add(new UIControlElement("|DGBLUE|{LEFT", "LEFT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|/RIGHT", "RIGHT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|}UP", "UP", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|~DOWN", "DOWN", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|JUMP", "JUMP", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|FIRE", "SHOOT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|GRAB", "GRAB", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|QUACK", "QUACK", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|STRAFE", "STRAFE", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGBLUE|RAGDOLL", "RAGDOLL", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            uiBox.Add((UIComponent)new UIText(" ", Color.White, heightAdd: -6f), true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|{MENU LEFT", "MENULEFT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|/MENU RIGHT", "MENURIGHT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|}MENU UP", "MENUUP", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|~MENU DOWN", "MENUDOWN", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|ACCEPT", "SELECT", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|MENU 1", "MENU1", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|MENU 2", "MENU2", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|CANCEL", "CANCEL", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGPURPLE|START", "START", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            uiBox.Add((UIComponent)new UIText(" ", Color.White, heightAdd: -6f), true);
            this._controlElements.Add(new UIControlElement("|DGGREEN|MOVE STICK", "LSTICK", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGGREEN|LICK STICK", "RSTICK", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGGREEN|QUACK PITCH", "LTRIGGER", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            this._controlElements.Add(new UIControlElement("|DGGREEN|ZOOM   ", "RTRIGGER", new DeviceInputMapping(), field: new FieldBinding((object)Options.Data, "sfxVolume")));
            uiBox.Add((UIComponent)this._controlElements[this._controlElements.Count - 1], true);
            UIMenuItem component1 = new UIMenuItem("|RED|REVERT TO DEFAULT", (UIMenuAction)new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.ResetToDefault)));
            component1.SetFont(bitmapFont);
            uiBox.Add((UIComponent)component1, true);
            UIText component2 = new UIText(" ", Color.White);
            component2.SetFont(bitmapFont);
            uiBox.Add((UIComponent)component2, true);
            UIText component3 = new UIText("Personal controls can be", Color.White);
            component3.SetFont(bitmapFont);
            uiBox.Add((UIComponent)component3, true);
            UIText component4 = new UIText("set in profile screen.", Color.White);
            component4.SetFont(bitmapFont);
            uiBox.Add((UIComponent)component4, true);
            this._controlBox = uiBox;
            this._playerBoxes.Add(uiBox);
            this.Add((UIComponent)this._playerBoxes[0], true);
            this._confirmMenu = new UIMenu("SAVE CHANGES?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT @CANCEL@BACK");
            this._confirmMenu.Add((UIComponent)new UIMenuItem("YES!", (UIMenuAction)new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.CloseMenuSaving))), true);
            this._confirmMenu.Add((UIComponent)new UIMenuItem("NO!", (UIMenuAction)new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.CloseMenu))), true);
            this._confirmMenu.SetBackFunction((UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._confirmMenu, (UIComponent)this));
            this._confirmMenu.Close();
            this._warningMenu = new UIMenu("WARNING!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 180f, conString: "@SELECT@ I see...");
            UIMenu warningMenu1 = this._warningMenu;
            UIText component5 = new UIText("", Color.White, heightAdd: -3f);
            component5.scale = new Vec2(0.5f);
            warningMenu1.Add((UIComponent)component5, true);
            UIMenu warningMenu2 = this._warningMenu;
            UIText component6 = new UIText("One or more profiles have", Color.White, heightAdd: -4f);
            component6.scale = new Vec2(0.5f);
            warningMenu2.Add((UIComponent)component6, true);
            UIMenu warningMenu3 = this._warningMenu;
            UIText component7 = new UIText("|DGBLUE|custom controls|PREV| defined, which will", Color.White, heightAdd: -4f);
            component7.scale = new Vec2(0.5f);
            warningMenu3.Add((UIComponent)component7, true);
            UIMenu warningMenu4 = this._warningMenu;
            UIText component8 = new UIText("|DGRED|override|PREV| any controls set here!", Color.White, heightAdd: -4f);
            component8.scale = new Vec2(0.5f);
            warningMenu4.Add((UIComponent)component8, true);
            UIMenu warningMenu5 = this._warningMenu;
            UIText component9 = new UIText("", Color.White, heightAdd: -3f);
            component9.scale = new Vec2(0.5f);
            warningMenu5.Add((UIComponent)component9, true);
            UIMenu warningMenu6 = this._warningMenu;
            UIText component10 = new UIText("If these controls are not working,", Color.White, heightAdd: -4f);
            component10.scale = new Vec2(0.5f);
            warningMenu6.Add((UIComponent)component10, true);
            UIMenu warningMenu7 = this._warningMenu;
            UIText component11 = new UIText("enter the hat console in the lobby", Color.White, heightAdd: -4f);
            component11.scale = new Vec2(0.5f);
            warningMenu7.Add((UIComponent)component11, true);
            UIMenu warningMenu8 = this._warningMenu;
            UIText component12 = new UIText("and press |DGORANGE|EDIT|PREV| on your profile name.", Color.White, heightAdd: -4f);
            component12.scale = new Vec2(0.5f);
            warningMenu8.Add((UIComponent)component12, true);
            UIMenu warningMenu9 = this._warningMenu;
            UIText component13 = new UIText("Select |DGORANGE|CONTROLS|PREV|, select your desired", Color.White, heightAdd: -4f);
            component13.scale = new Vec2(0.5f);
            warningMenu9.Add((UIComponent)component13, true);
            UIMenu warningMenu10 = this._warningMenu;
            UIText component14 = new UIText("input device, go to |DGORANGE|PAGE 2|PREV|", Color.White, heightAdd: -4f);
            component14.scale = new Vec2(0.5f);
            warningMenu10.Add((UIComponent)component14, true);
            UIMenu warningMenu11 = this._warningMenu;
            UIText component15 = new UIText("and select |DGORANGE|RESET|PREV|.", Color.White, heightAdd: -4f);
            component15.scale = new Vec2(0.5f);
            warningMenu11.Add((UIComponent)component15, true);
            UIMenu warningMenu12 = this._warningMenu;
            UIText component16 = new UIText("", Color.White, heightAdd: -3f);
            component16.scale = new Vec2(0.5f);
            warningMenu12.Add((UIComponent)component16, true);
            this._warningMenu.SetAcceptFunction((UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._warningMenu, (UIComponent)this));
            this._warningMenu.SetBackFunction((UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._warningMenu, (UIComponent)this));
            this._warningMenu.Close();
        }

        public static void ResetWarning()
        {
            UIControlConfig.showWarning = false;
            foreach (Profile profile in Profiles.active)
            {
                if (profile.inputMappingOverrides != null && profile.inputMappingOverrides.Count > 0)
                {
                    UIControlConfig.showWarning = true;
                    break;
                }
            }
        }

        public override void Open()
        {
            this.SwitchPlayerProfile();
            base.Open();
        }

        public override void Update()
        {
            if (this.open)
            {
                if (!UIMenu.globalUILock && UIControlConfig.showWarning)
                {
                    new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._warningMenu).Activate();
                    UIControlConfig.showWarning = false;
                    return;
                }
                if (!UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.OemTilde)))
                {
                    new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._confirmMenu).Activate();
                    return;
                }
                if (Input.uiDevicesHaveChanged)
                {
                    this.SwitchPlayerProfile();
                    Input.uiDevicesHaveChanged = false;
                }
            }
            if (this._controlBox.selection > 0 && this._controlBox.selection < 17)
            {
                if (!this._showingMenu && this.inputConfigType < this.inputMaps.Count && this.inputMaps[this.inputConfigType].deviceName != "KEYBOARD P1" && this.inputMaps[this.inputConfigType].deviceName != "KEYBOARD P2")
                {
                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@STYLE");
                    this._showingMenu = true;
                }
            }
            else if (this._showingMenu)
            {
                HUD.CloseAllCorners();
                this._showingMenu = false;
            }
            base.Update();
        }
    }
}
