// Decompiled with JetBrains decompiler
// Type: DuckGame.UIControlElement
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UIControlElement : UIMenuItem
    {
        private FieldBinding _field;
        private List<string> _captionList = new List<string>();
        private bool _editing;
        private bool _skipStep;
        public int randomAssIntField;
        public DeviceInputMapping inputMapping;
        private Sprite _styleBubble;
        private Sprite _styleTray;
        private string _realTrigger;
        private bool _selectStyle;
        private int _selectionIndex;
        private UIText _uiText;

        public override Vec2 collisionSize
        {
            get => new Vec2(160f, 2f);
            set => _collisionSize = value;
        }

        public string _trigger
        {
            set => _realTrigger = value;
            get
            {
                if (_realTrigger == Triggers.LeftStick && inputMapping != null && inputMapping.device is Keyboard)
                    return Triggers.Chat;
                if (_realTrigger == Triggers.RightStick && inputMapping != null && inputMapping.device is Keyboard)
                    return Triggers.VoiceRegister;
                return _realTrigger == Triggers.RightTrigger && inputMapping != null && inputMapping.device is Keyboard ? "PLAYERINDEX" : _realTrigger;
            }
        }

        public UIControlElement(
          string text,
          string trigger,
          DeviceInputMapping map,
          UIMenuAction action = null,
          FieldBinding field = null,
          Color c = default(Color))
          : base(action)
        {
            _trigger = trigger;
            if (c == new Color())
                c = Colors.MenuOption;
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIDivider component1 = new UIDivider(true, 0f);
            _uiText = new UIText(text, c);
            _uiText.SetFont(f);
            _uiText.align = UIAlign.Left;
            _uiText.specialScale = 0.5f;
            component1.leftSection.Add(_uiText, true);
            UIMultiToggle component2 = new UIMultiToggle(-1f, -1f, new FieldBinding(this, nameof(randomAssIntField)), _captionList, true);
            component2.SetFont(f);
            component2.align = UIAlign.Right;
            component1.rightSection.Add(component2, true);
            rightSection.Add(component1, true);
            component2.specialScale = 0.5f;
            _arrow = new UIImage("littleContextArrowRight")
            {
                scale = new Vec2(0.5f, 0.5f),
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _styleBubble = new Sprite("buttons/styleBubble")
            {
                center = new Vec2(0f, 11f)
            };
            _styleTray = new Sprite("buttons/styleTray");
            _styleTray.CenterOrigin();
            _field = field;
            inputMapping = map;
        }

        public override void Update()
        {
            collisionSize = new Vec2(collisionSize.x, 2.5f);
            _captionList.Clear();
            if (!_editing)
            {
                string localtrigger = _trigger;
                string str = !(inputMapping.device is Keyboard) || !(localtrigger == Triggers.LeftStick) && !(localtrigger == Triggers.RightStick) && !(localtrigger == Triggers.LeftTrigger) && !(localtrigger == Triggers.RightTrigger) ? (localtrigger == Triggers.LeftStick || localtrigger == Triggers.RightStick || localtrigger == Triggers.LeftTrigger || localtrigger == Triggers.RightTrigger ? "|DGYELLOW|" : "|WHITE|") : "|GRAY|";
                if (localtrigger == Triggers.LeftStick)
                {
                    _uiText.text = "|DGGREEN|MOVE STICK";
                    if (inputMapping.device is Keyboard)
                        _uiText.text = "|GRAY|MOVE STICK";
                }
                if (localtrigger == Triggers.RightStick)
                {
                    _uiText.text = "|DGGREEN|LICK STICK";
                    if (inputMapping.device is Keyboard)
                        _uiText.text = "|GRAY|LICK STICK";
                }
                if (localtrigger == Triggers.LeftTrigger)
                {
                    _uiText.text = "|DGGREEN|QUACK PITCH";
                    if (inputMapping.device is Keyboard)
                        _uiText.text = "|GRAY|QUACK PITCH";
                }
                if (localtrigger == Triggers.RightTrigger)
                {
                    _uiText.text = "|DGGREEN|ZOOM   ";
                    if (inputMapping.device is Keyboard)
                        _uiText.text = "|GRAY|ZOOM   ";
                }
                string mappingString = inputMapping.GetMappingString(localtrigger);
                if (localtrigger == Triggers.Chat)
                    _uiText.text = "|PINK|CHAT      ";
                if (localtrigger == Triggers.VoiceRegister)
                    _uiText.text = "|PINK|JAM BUTTON";
                if (localtrigger == "PLAYERINDEX")
                {
                    _uiText.text = "|LIME|PLAYER#";
                    if (inputMapping.device.productName == "KEYBOARD P1")
                        mappingString = (Options.Data.keyboard1PlayerIndex + 1).ToString();
                    else if (inputMapping.device.productName == "KEYBOARD P2")
                        mappingString = (Options.Data.keyboard2PlayerIndex + 1).ToString();
                }
                _captionList.Add(str + mappingString + "  ");
            }
            else
            {
                if (_skipStep)
                {
                    _skipStep = false;
                    return;
                }
                if (!_selectStyle)
                {
                    _captionList.Add("_");
                    if (Keyboard.Pressed(Keys.OemTilde))
                    {
                        _editing = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                    }
                    else if (inputMapping.RunMappingUpdate(_trigger))
                    {
                        _editing = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                        if (!(inputMapping.deviceName != "KEYBOARD P1") || !(inputMapping.deviceName != "KEYBOARD P1"))
                            return;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@STYLE");
                        return;
                    }
                }
                else
                {
                    bool flag = false;
                    if (Input.Pressed(Triggers.MenuLeft))
                    {
                        --_selectionIndex;
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed(Triggers.MenuRight))
                    {
                        ++_selectionIndex;
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed(Triggers.MenuUp))
                    {
                        _selectionIndex -= 4;
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed(Triggers.MenuDown))
                    {
                        _selectionIndex += 4;
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (_selectionIndex < 0)
                        _selectionIndex = 0;
                    if (_selectionIndex >= Input.buttonStyles.Count)
                        _selectionIndex = Input.buttonStyles.Count - 1;
                    if (Input.Pressed(Triggers.Cancel))
                    {
                        flag = true;
                        SFX.Play("consoleError");
                    }
                    if (Input.Pressed(Triggers.Select))
                    {
                        flag = true;
                        int key;
                        if (inputMapping.map.TryGetValue(_trigger, out key))
                        {
                            inputMapping.graphicMap[key] = Input.buttonStyles[_selectionIndex].texture.textureName;
                            SFX.Play("consoleSelect");
                        }
                    }
                    if (flag)
                    {
                        _editing = false;
                        _selectStyle = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                        if (!(inputMapping.deviceName != "KEYBOARD P1") || !(inputMapping.deviceName != "KEYBOARD P1"))
                            return;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@STYLE");
                        return;
                    }
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (_arrow.visible)
            {
                _styleBubble.depth = (Depth)0.9f;
                Vec2 vec2_1 = new Vec2(x + 76f, y);
                if (_selectStyle)
                {
                    vec2_1 = new Vec2(x + 85f, y);
                    _styleBubble.flipH = true;
                }
                else
                    _styleBubble.flipH = false;
                Graphics.Draw(ref _styleBubble, vec2_1.x, vec2_1.y);
                string localtrigger = _trigger;
                if (inputMapping.map.ContainsKey(localtrigger))
                {
                    Sprite g = inputMapping.GetSprite(inputMapping.map[localtrigger]) ?? inputMapping.device.DoGetMapImage(inputMapping.map[localtrigger], true);
                    if (g != null)
                    {
                        g.depth = (Depth)0.95f;
                        Graphics.Draw(g, vec2_1.x + (_selectStyle ? -22f : 9f), vec2_1.y - 7f);
                    }
                }
                if (_selectStyle)
                {
                    _styleTray.depth = (Depth)0.92f;
                    Graphics.Draw(ref _styleTray, x + 118f, Layer.HUD.camera.height / 2f);
                    Vec2 vec2_2 = new Vec2(x + 90f, (float)(Layer.HUD.camera.height / 2.0 - 80.0));
                    int num = 0;
                    foreach (Sprite buttonStyle in Input.buttonStyles)
                    {
                        Vec2 vec2_3 = vec2_2 + new Vec2(num % 4 * 14, num / 4 * 14);
                        buttonStyle.depth = (Depth)0.95f;
                        buttonStyle.color = Color.White * (num == _selectionIndex ? 1f : 0.4f);
                        Graphics.Draw(buttonStyle, vec2_3.x, vec2_3.y);
                        ++num;
                    }
                }
            }
            base.Draw();
        }

        public override void Activate(string trigger)
        {
            string localtrigger = _trigger;
            if (trigger == Triggers.MenuRight)
            {
                if (localtrigger == "PLAYERINDEX")
                {
                    if (inputMapping.device.productName == "KEYBOARD P1")
                    {
                        ++Options.Data.keyboard1PlayerIndex;
                        if (Options.Data.keyboard1PlayerIndex > 7)
                            Options.Data.keyboard1PlayerIndex = 0;
                    }
                    else if (inputMapping.device.productName == "KEYBOARD P2")
                    {
                        ++Options.Data.keyboard2PlayerIndex;
                        if (Options.Data.keyboard2PlayerIndex > 7)
                            Options.Data.keyboard2PlayerIndex = 0;
                    }
                    SFX.Play("consoleSelect");
                }
            }
            else if (trigger == Triggers.MenuLeft && localtrigger == "PLAYERINDEX")
            {
                if (inputMapping.device.productName == "KEYBOARD P1")
                {
                    --Options.Data.keyboard1PlayerIndex;
                    if (Options.Data.keyboard1PlayerIndex < 0)
                        Options.Data.keyboard1PlayerIndex = 7;
                }
                else if (inputMapping.device.productName == "KEYBOARD P2")
                {
                    --Options.Data.keyboard2PlayerIndex;
                    if (Options.Data.keyboard2PlayerIndex < 0)
                        Options.Data.keyboard2PlayerIndex = 7;
                }
                SFX.Play("consoleSelect");
            }
            if (trigger == Triggers.Select)
            {
                if (inputMapping.device is Keyboard && (localtrigger == Triggers.LeftStick || localtrigger == Triggers.RightStick || localtrigger == Triggers.LeftTrigger || localtrigger == Triggers.RightTrigger))
                    SFX.Play("consoleError");
                else if (localtrigger == "PLAYERINDEX")
                {
                    if (inputMapping.device.productName == "KEYBOARD P1")
                    {
                        ++Options.Data.keyboard1PlayerIndex;
                        if (Options.Data.keyboard1PlayerIndex > 7)
                            Options.Data.keyboard1PlayerIndex = 0;
                    }
                    else if (inputMapping.device.productName == "KEYBOARD P2")
                    {
                        ++Options.Data.keyboard2PlayerIndex;
                        if (Options.Data.keyboard2PlayerIndex > 7)
                            Options.Data.keyboard2PlayerIndex = 0;
                    }
                    SFX.Play("consoleSelect");
                }
                else
                {
                    UIMenu.globalUILock = true;
                    _editing = true;
                    _skipStep = true;
                    SFX.Play("consoleSelect");
                    HUD.CloseAllCorners();
                    HUD.AddCornerControl(HUDCorner.TopLeft, "@CONSOLE@CANCEL");
                }
            }
            else
            {
                if (!(trigger == Triggers.Menu2) || !(inputMapping.deviceName != "KEYBOARD P1") || !(inputMapping.deviceName != "KEYBOARD P2"))
                    return;
                _selectStyle = true;
                UIMenu.globalUILock = true;
                _editing = true;
                _skipStep = true;
                int mapping;
                if (inputMapping.map.TryGetValue(localtrigger, out mapping))
                {
                    int num = 0;
                    Sprite sprite = inputMapping.GetSprite(mapping);
                    if (sprite != null)
                    {
                        foreach (Sprite buttonStyle in Input.buttonStyles)
                        {
                            if (sprite.texture != null && sprite.texture.textureName == buttonStyle.texture.textureName)
                            {
                                _selectionIndex = num;
                                break;
                            }
                            ++num;
                        }
                    }
                }
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@CANCEL");
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@SELECT");
            }
        }
    }
}
