// Decompiled with JetBrains decompiler
// Type: DuckGame.MessageDialogue
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MessageDialogue : ContextMenu
    {
        private new string _text = "";
        private string[] _description;
        public bool result;
        private BitmapFont _font;
        private ContextMenu _ownerMenu;
        public ContextMenu confirmItem;
        public string contextString;
        private bool _hoverOk;
        private bool _hoverCancel;
        public float windowYOffsetAdd;
        private float bottomRightPos;
        public bool okayOnly;

        public MessageDialogue(ContextMenu pOwnerMenu)
          : base(null)
        {
            _ownerMenu = pOwnerMenu;
        }

        public MessageDialogue()
          : base(null)
        {
        }

        public override void Initialize()
        {
            layer = Layer.HUD;
            depth = (Depth)0.95f;
            float num1 = 300f;
            float num2 = 40f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            position = vec2_1 + new Vec2(4f, 20f);
            itemSize = new Vec2(490f, 16f);
            _root = true;
            _font = new BitmapFont("biosFont", 8)
            {
                allowBigSprites = true
            };
        }

        public void Open(string text, string startingText = "", string pDescription = null)
        {
            opened = true;
            _text = text;
            if (pDescription == null)
                _description = null;
            else
                _description = pDescription.Split('\n');
            SFX.Play("openClick", 0.4f);
        }

        public void Close() => opened = false;

        public override void Selected(ContextMenu item)
        {
        }

        public override void Update()
        {
            if (!opened)
                return;
            if (_opening)
            {
                _opening = false;
                _selectedIndex = 1;
            }
            if (Keyboard.Pressed(Keys.Enter))
            {
                result = true;
                CompleteDialogue();
            }
            if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed || Input.Pressed(Triggers.Cancel))
            {
                result = false;
                CompleteDialogue();
            }
            float num1 = 300f;
            float num2 = 80f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0) + windowYOffsetAdd);
            Vec2 vec2_2 = new Vec2(vec2_1.x + 18f, bottomRightPos - 50f);
            Vec2 vec2_3 = new Vec2(120f, 40f);
            float num3 = (float)((layer.width / 2.0 + num1 / 2.0 - vec2_1.x) / 2.0);
            if (okayOnly)
                vec2_2 = new Vec2((float)(vec2_1.x + num3 - vec2_3.x / 2.0), bottomRightPos - 50f);
            Vec2 vec2_4 = new Vec2(vec2_1.x + 160f, bottomRightPos - 50f);
            Vec2 vec2_5 = new Vec2(120f, 40f);
            Rectangle pRect1 = new Rectangle(vec2_2.x, vec2_2.y, vec2_3.x, vec2_3.y);
            Rectangle pRect2 = new Rectangle(vec2_4.x, vec2_4.y, vec2_5.x, vec2_5.y);
            bool flag1 = false;
            bool flag2 = false;
            switch (Editor.inputMode)
            {
                case EditorInput.Mouse:
                    _hoverOk = Mouse.x > vec2_2.x && Mouse.x < vec2_2.x + vec2_3.x && Mouse.y > vec2_2.y && Mouse.y < vec2_2.y + vec2_3.y;
                    if (!okayOnly)
                    {
                        _hoverCancel = Mouse.x > vec2_4.x && Mouse.x < vec2_4.x + vec2_5.x && Mouse.y > vec2_4.y && Mouse.y < vec2_4.y + vec2_5.y;
                        break;
                    }
                    break;
                case EditorInput.Touch:
                    if (!okayOnly)
                    {
                        if (TouchScreen.GetTap().Check(pRect1, Layer.HUD.camera))
                        {
                            _selectedIndex = 0;
                            _hoverOk = true;
                            flag1 = true;
                        }
                        else
                            _hoverOk = false;
                        if (TouchScreen.GetTap().Check(pRect2, Layer.HUD.camera))
                        {
                            _selectedIndex = 1;
                            _hoverCancel = true;
                            flag2 = true;
                            break;
                        }
                        _hoverCancel = false;
                        break;
                    }
                    break;
                default:
                    _hoverOk = _hoverCancel = false;
                    break;
            }
            if (!_hoverOk && !_hoverCancel)
            {
                if (!Editor.tookInput && Input.Pressed(Triggers.MenuLeft))
                    --_selectedIndex;
                else if (!Editor.tookInput && Input.Pressed(Triggers.MenuRight))
                    ++_selectedIndex;
                if (_selectedIndex < 0)
                    _selectedIndex = 0;
                if (_selectedIndex > 1)
                    _selectedIndex = 1;
                if (Editor.inputMode == EditorInput.Gamepad)
                {
                    if (okayOnly)
                    {
                        _hoverOk = true;
                    }
                    else
                    {
                        _hoverOk = _hoverCancel = false;
                        if (_selectedIndex == 0)
                            _hoverOk = true;
                        else
                            _hoverCancel = true;
                    }
                }
            }
            if (!Editor.tookInput && _hoverOk && ((Editor.inputMode != EditorInput.Mouse || Mouse.left != InputState.Pressed ? (Input.Pressed(Triggers.Select) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) != 0)
            {
                result = true;
                CompleteDialogue();
            }
            if (Editor.tookInput || !_hoverCancel || ((Editor.inputMode != EditorInput.Mouse || Mouse.left != InputState.Pressed ? (Input.Pressed(Triggers.Select) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) == 0)
                return;
            result = false;
            CompleteDialogue();
        }

        private void CompleteDialogue()
        {
            opened = false;
            Editor.tookInput = true;
            Editor.lockInput = null;
            if (_ownerMenu == null)
            {
                if (!(Level.current is Editor))
                    return;
                (Level.current as Editor).CompleteDialogue(confirmItem);
            }
            else
            {
                if (confirmItem == null || _ownerMenu == null || !result)
                    return;
                _ownerMenu.Selected(confirmItem);
            }
        }

        public override void Draw()
        {
            if (!opened)
                return;
            base.Draw();
            Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, depth - 2);
            float num1 = 300f;
            float num2 = 80f;
            Vec2 p1_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0) + windowYOffsetAdd);
            Vec2 p2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0) + windowYOffsetAdd);
            float num3 = (float)((p2.x - p1_1.x) / 2.0);
            if (_description != null)
            {
                int num4 = 18;
                foreach (string text in _description)
                {
                    float stringWidth = Graphics.GetStringWidth(text);
                    Graphics.DrawString(text, p1_1 + new Vec2(num3 - stringWidth / 2f, 5 + num4), Color.White, depth + 2);
                    num4 += 8;
                    p2.y += 8f;
                }
            }
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), depth + 1);
            float stringWidth1 = Graphics.GetStringWidth(_text);
            Graphics.DrawString(_text, p1_1 + new Vec2(num3 - stringWidth1 / 2f, 5f), Color.White, depth + 2);
            _font.scale = new Vec2(2f, 2f);
            if (okayOnly)
            {
                Vec2 vec2 = new Vec2(120f, 40f);
                Vec2 p1_2 = new Vec2((float)(x + num3 - vec2.x / 2.0 - 2.0), p2.y - 50f);
                Graphics.DrawRect(p1_2, p1_2 + vec2, _hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
                _font.Draw("OK", (float)(p1_2.x + vec2.x / 2.0 - _font.GetWidth("OK") / 2.0), p1_2.y + 12f, Color.White, depth + 3);
            }
            else
            {
                Vec2 p1_3 = new Vec2(p1_1.x + 18f, p2.y - 50f);
                Vec2 vec2_1 = new Vec2(120f, 40f);
                Graphics.DrawRect(p1_3, p1_3 + vec2_1, _hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
                _font.Draw("OK", (float)(p1_3.x + vec2_1.x / 2.0 - _font.GetWidth("OK") / 2.0), p1_3.y + 12f, Color.White, depth + 3);
                Vec2 p1_4 = new Vec2(p1_1.x + 160f, p2.y - 50f);
                Vec2 vec2_2 = new Vec2(120f, 40f);
                Graphics.DrawRect(p1_4, p1_4 + vec2_2, _hoverCancel ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
                _font.Draw(Triggers.Cancel, (float)(p1_4.x + vec2_2.x / 2.0 - _font.GetWidth(Triggers.Cancel) / 2.0), p1_4.y + 12f, Color.White, depth + 3);
            }
            bottomRightPos = p2.y;
        }
    }
}
