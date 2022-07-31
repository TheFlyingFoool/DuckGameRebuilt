// Decompiled with JetBrains decompiler
// Type: DuckGame.UIControlElement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            set => this._collisionSize = value;
        }

        public string _trigger
        {
            set => this._realTrigger = value;
            get
            {
                if (this._realTrigger == "LSTICK" && this.inputMapping != null && this.inputMapping.device is Keyboard)
                    return "CHAT";
                if (this._realTrigger == "RSTICK" && this.inputMapping != null && this.inputMapping.device is Keyboard)
                    return "VOICEREG";
                return this._realTrigger == "RTRIGGER" && this.inputMapping != null && this.inputMapping.device is Keyboard ? "PLAYERINDEX" : this._realTrigger;
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
            this._trigger = trigger;
            if (c == new Color())
                c = Colors.MenuOption;
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIDivider component1 = new UIDivider(true, 0f);
            this._uiText = new UIText(text, c);
            this._uiText.SetFont(f);
            this._uiText.align = UIAlign.Left;
            this._uiText.specialScale = 0.5f;
            component1.leftSection.Add(_uiText, true);
            UIMultiToggle component2 = new UIMultiToggle(-1f, -1f, new FieldBinding(this, nameof(randomAssIntField)), this._captionList, true);
            component2.SetFont(f);
            component2.align = UIAlign.Right;
            component1.rightSection.Add(component2, true);
            this.rightSection.Add(component1, true);
            component2.specialScale = 0.5f;
            this._arrow = new UIImage("littleContextArrowRight")
            {
                scale = new Vec2(0.5f, 0.5f),
                align = UIAlign.Right,
                visible = false
            };
            this.leftSection.Add(_arrow, true);
            this._styleBubble = new Sprite("buttons/styleBubble")
            {
                center = new Vec2(0f, 11f)
            };
            this._styleTray = new Sprite("buttons/styleTray");
            this._styleTray.CenterOrigin();
            this._field = field;
            this.inputMapping = map;
        }

        public override void Update()
        {
            this.collisionSize = new Vec2(this.collisionSize.x, 2.5f);
            this._captionList.Clear();
            if (!this._editing)
            {
                string str = !(this.inputMapping.device is Keyboard) || !(this._trigger == "LSTICK") && !(this._trigger == "RSTICK") && !(this._trigger == "LTRIGGER") && !(this._trigger == "RTRIGGER") ? (this._trigger == "LSTICK" || this._trigger == "RSTICK" || this._trigger == "LTRIGGER" || this._trigger == "RTRIGGER" ? "|DGYELLOW|" : "|WHITE|") : "|GRAY|";
                if (this._trigger == "LSTICK")
                {
                    this._uiText.text = "|DGGREEN|MOVE STICK";
                    if (this.inputMapping.device is Keyboard)
                        this._uiText.text = "|GRAY|MOVE STICK";
                }
                if (this._trigger == "RSTICK")
                {
                    this._uiText.text = "|DGGREEN|LICK STICK";
                    if (this.inputMapping.device is Keyboard)
                        this._uiText.text = "|GRAY|LICK STICK";
                }
                if (this._trigger == "LTRIGGER")
                {
                    this._uiText.text = "|DGGREEN|QUACK PITCH";
                    if (this.inputMapping.device is Keyboard)
                        this._uiText.text = "|GRAY|QUACK PITCH";
                }
                if (this._trigger == "RTRIGGER")
                {
                    this._uiText.text = "|DGGREEN|ZOOM   ";
                    if (this.inputMapping.device is Keyboard)
                        this._uiText.text = "|GRAY|ZOOM   ";
                }
                string mappingString = this.inputMapping.GetMappingString(this._trigger);
                if (this._trigger == "CHAT")
                    this._uiText.text = "|PINK|CHAT      ";
                if (this._trigger == "VOICEREG")
                    this._uiText.text = "|PINK|JAM BUTTON";
                if (this._trigger == "PLAYERINDEX")
                {
                    this._uiText.text = "|LIME|PLAYER#";
                    if (this.inputMapping.device.productName == "KEYBOARD P1")
                        mappingString = (Options.Data.keyboard1PlayerIndex + 1).ToString();
                    else if (this.inputMapping.device.productName == "KEYBOARD P2")
                        mappingString = (Options.Data.keyboard2PlayerIndex + 1).ToString();
                }
                this._captionList.Add(str + mappingString + "  ");
            }
            else
            {
                if (this._skipStep)
                {
                    this._skipStep = false;
                    return;
                }
                if (!this._selectStyle)
                {
                    this._captionList.Add("_");
                    if (Keyboard.Pressed(Keys.OemTilde))
                    {
                        this._editing = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                    }
                    else if (this.inputMapping.RunMappingUpdate(this._trigger))
                    {
                        this._editing = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                        if (!(this.inputMapping.deviceName != "KEYBOARD P1") || !(this.inputMapping.deviceName != "KEYBOARD P1"))
                            return;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@STYLE");
                        return;
                    }
                }
                else
                {
                    bool flag = false;
                    if (Input.Pressed("MENULEFT"))
                    {
                        --this._selectionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed("MENURIGHT"))
                    {
                        ++this._selectionIndex;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed("MENUUP"))
                    {
                        this._selectionIndex -= 4;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (Input.Pressed("MENUDOWN"))
                    {
                        this._selectionIndex += 4;
                        SFX.Play("textLetter", 0.7f);
                    }
                    if (this._selectionIndex < 0)
                        this._selectionIndex = 0;
                    if (this._selectionIndex >= Input.buttonStyles.Count)
                        this._selectionIndex = Input.buttonStyles.Count - 1;
                    if (Input.Pressed("CANCEL"))
                    {
                        flag = true;
                        SFX.Play("consoleError");
                    }
                    if (Input.Pressed("SELECT"))
                    {
                        flag = true;
                        int key;
                        if (this.inputMapping.map.TryGetValue(this._trigger, out key))
                        {
                            this.inputMapping.graphicMap[key] = Input.buttonStyles[this._selectionIndex].texture.textureName;
                            SFX.Play("consoleSelect");
                        }
                    }
                    if (flag)
                    {
                        this._editing = false;
                        this._selectStyle = false;
                        UIMenu.globalUILock = false;
                        HUD.CloseAllCorners();
                        if (!(this.inputMapping.deviceName != "KEYBOARD P1") || !(this.inputMapping.deviceName != "KEYBOARD P1"))
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
            if (this._arrow.visible)
            {
                this._styleBubble.depth = (Depth)0.9f;
                Vec2 vec2_1 = new Vec2(this.x + 76f, this.y);
                if (this._selectStyle)
                {
                    vec2_1 = new Vec2(this.x + 85f, this.y);
                    this._styleBubble.flipH = true;
                }
                else
                    this._styleBubble.flipH = false;
                Graphics.Draw(this._styleBubble, vec2_1.x, vec2_1.y);
                if (this.inputMapping.map.ContainsKey(this._trigger))
                {
                    Sprite g = this.inputMapping.GetSprite(this.inputMapping.map[this._trigger]) ?? this.inputMapping.device.DoGetMapImage(this.inputMapping.map[this._trigger], true);
                    if (g != null)
                    {
                        g.depth = (Depth)0.95f;
                        Graphics.Draw(g, vec2_1.x + (this._selectStyle ? -22f : 9f), vec2_1.y - 7f);
                    }
                }
                if (this._selectStyle)
                {
                    this._styleTray.depth = (Depth)0.92f;
                    Graphics.Draw(this._styleTray, this.x + 118f, Layer.HUD.camera.height / 2f);
                    Vec2 vec2_2 = new Vec2(this.x + 90f, (float)(Layer.HUD.camera.height / 2.0 - 80.0));
                    int num = 0;
                    foreach (Sprite buttonStyle in Input.buttonStyles)
                    {
                        Vec2 vec2_3 = vec2_2 + new Vec2(num % 4 * 14, num / 4 * 14);
                        buttonStyle.depth = (Depth)0.95f;
                        buttonStyle.color = Color.White * (num == this._selectionIndex ? 1f : 0.4f);
                        Graphics.Draw(buttonStyle, vec2_3.x, vec2_3.y);
                        ++num;
                    }
                }
            }
            base.Draw();
        }

        public override void Activate(string trigger)
        {
            if (trigger == "MENURIGHT")
            {
                if (this._trigger == "PLAYERINDEX")
                {
                    if (this.inputMapping.device.productName == "KEYBOARD P1")
                    {
                        ++Options.Data.keyboard1PlayerIndex;
                        if (Options.Data.keyboard1PlayerIndex > 7)
                            Options.Data.keyboard1PlayerIndex = 0;
                    }
                    else if (this.inputMapping.device.productName == "KEYBOARD P2")
                    {
                        ++Options.Data.keyboard2PlayerIndex;
                        if (Options.Data.keyboard2PlayerIndex > 7)
                            Options.Data.keyboard2PlayerIndex = 0;
                    }
                    SFX.Play("consoleSelect");
                }
            }
            else if (trigger == "MENULEFT" && this._trigger == "PLAYERINDEX")
            {
                if (this.inputMapping.device.productName == "KEYBOARD P1")
                {
                    --Options.Data.keyboard1PlayerIndex;
                    if (Options.Data.keyboard1PlayerIndex < 0)
                        Options.Data.keyboard1PlayerIndex = 7;
                }
                else if (this.inputMapping.device.productName == "KEYBOARD P2")
                {
                    --Options.Data.keyboard2PlayerIndex;
                    if (Options.Data.keyboard2PlayerIndex < 0)
                        Options.Data.keyboard2PlayerIndex = 7;
                }
                SFX.Play("consoleSelect");
            }
            if (trigger == "SELECT")
            {
                if (this.inputMapping.device is Keyboard && (this._trigger == "LSTICK" || this._trigger == "RSTICK" || this._trigger == "LTRIGGER" || this._trigger == "RTRIGGER"))
                    SFX.Play("consoleError");
                else if (this._trigger == "PLAYERINDEX")
                {
                    if (this.inputMapping.device.productName == "KEYBOARD P1")
                    {
                        ++Options.Data.keyboard1PlayerIndex;
                        if (Options.Data.keyboard1PlayerIndex > 7)
                            Options.Data.keyboard1PlayerIndex = 0;
                    }
                    else if (this.inputMapping.device.productName == "KEYBOARD P2")
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
                    this._editing = true;
                    this._skipStep = true;
                    SFX.Play("consoleSelect");
                    HUD.CloseAllCorners();
                    HUD.AddCornerControl(HUDCorner.TopLeft, "@CONSOLE@CANCEL");
                }
            }
            else
            {
                if (!(trigger == "MENU2") || !(this.inputMapping.deviceName != "KEYBOARD P1") || !(this.inputMapping.deviceName != "KEYBOARD P2"))
                    return;
                this._selectStyle = true;
                UIMenu.globalUILock = true;
                this._editing = true;
                this._skipStep = true;
                int mapping;
                if (this.inputMapping.map.TryGetValue(this._trigger, out mapping))
                {
                    int num = 0;
                    Sprite sprite = this.inputMapping.GetSprite(mapping);
                    if (sprite != null)
                    {
                        foreach (Sprite buttonStyle in Input.buttonStyles)
                        {
                            if (sprite.texture != null && sprite.texture.textureName == buttonStyle.texture.textureName)
                            {
                                this._selectionIndex = num;
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
