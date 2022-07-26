// Decompiled with JetBrains decompiler
// Type: DuckGame.MessageDialogue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
          : base((IContextListener)null)
        {
            this._ownerMenu = pOwnerMenu;
        }

        public MessageDialogue()
          : base((IContextListener)null)
        {
        }

        public override void Initialize()
        {
            this.layer = Layer.HUD;
            this.depth = (Depth)0.95f;
            float num1 = 300f;
            float num2 = 40f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            this.position = vec2_1 + new Vec2(4f, 20f);
            this.itemSize = new Vec2(490f, 16f);
            this._root = true;
            this._font = new BitmapFont("biosFont", 8);
            this._font.allowBigSprites = true;
        }

        public void Open(string text, string startingText = "", string pDescription = null)
        {
            this.opened = true;
            this._text = text;
            if (pDescription == null)
                this._description = (string[])null;
            else
                this._description = pDescription.Split('\n');
            SFX.Play("openClick", 0.4f);
        }

        public void Close() => this.opened = false;

        public override void Selected(ContextMenu item)
        {
        }

        public override void Update()
        {
            if (!this.opened)
                return;
            if (this._opening)
            {
                this._opening = false;
                this._selectedIndex = 1;
            }
            if (Keyboard.Pressed(Keys.Enter))
            {
                this.result = true;
                this.CompleteDialogue();
            }
            if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed || Input.Pressed("CANCEL"))
            {
                this.result = false;
                this.CompleteDialogue();
            }
            float num1 = 300f;
            float num2 = 80f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0) + this.windowYOffsetAdd);
            Vec2 vec2_2 = new Vec2(vec2_1.x + 18f, this.bottomRightPos - 50f);
            Vec2 vec2_3 = new Vec2(120f, 40f);
            float num3 = (float)(((double)this.layer.width / 2.0 + (double)num1 / 2.0 - (double)vec2_1.x) / 2.0);
            if (this.okayOnly)
                vec2_2 = new Vec2((float)((double)vec2_1.x + (double)num3 - (double)vec2_3.x / 2.0), this.bottomRightPos - 50f);
            Vec2 vec2_4 = new Vec2(vec2_1.x + 160f, this.bottomRightPos - 50f);
            Vec2 vec2_5 = new Vec2(120f, 40f);
            Rectangle pRect1 = new Rectangle(vec2_2.x, vec2_2.y, vec2_3.x, vec2_3.y);
            Rectangle pRect2 = new Rectangle(vec2_4.x, vec2_4.y, vec2_5.x, vec2_5.y);
            bool flag1 = false;
            bool flag2 = false;
            switch (Editor.inputMode)
            {
                case EditorInput.Mouse:
                    this._hoverOk = (double)Mouse.x > (double)vec2_2.x && (double)Mouse.x < (double)vec2_2.x + (double)vec2_3.x && (double)Mouse.y > (double)vec2_2.y && (double)Mouse.y < (double)vec2_2.y + (double)vec2_3.y;
                    if (!this.okayOnly)
                    {
                        this._hoverCancel = (double)Mouse.x > (double)vec2_4.x && (double)Mouse.x < (double)vec2_4.x + (double)vec2_5.x && (double)Mouse.y > (double)vec2_4.y && (double)Mouse.y < (double)vec2_4.y + (double)vec2_5.y;
                        break;
                    }
                    break;
                case EditorInput.Touch:
                    if (!this.okayOnly)
                    {
                        if (TouchScreen.GetTap().Check(pRect1, Layer.HUD.camera))
                        {
                            this._selectedIndex = 0;
                            this._hoverOk = true;
                            flag1 = true;
                        }
                        else
                            this._hoverOk = false;
                        if (TouchScreen.GetTap().Check(pRect2, Layer.HUD.camera))
                        {
                            this._selectedIndex = 1;
                            this._hoverCancel = true;
                            flag2 = true;
                            break;
                        }
                        this._hoverCancel = false;
                        break;
                    }
                    break;
                default:
                    this._hoverOk = this._hoverCancel = false;
                    break;
            }
            if (!this._hoverOk && !this._hoverCancel)
            {
                if (!Editor.tookInput && Input.Pressed("MENULEFT"))
                    --this._selectedIndex;
                else if (!Editor.tookInput && Input.Pressed("MENURIGHT"))
                    ++this._selectedIndex;
                if (this._selectedIndex < 0)
                    this._selectedIndex = 0;
                if (this._selectedIndex > 1)
                    this._selectedIndex = 1;
                if (Editor.inputMode == EditorInput.Gamepad)
                {
                    if (this.okayOnly)
                    {
                        this._hoverOk = true;
                    }
                    else
                    {
                        this._hoverOk = this._hoverCancel = false;
                        if (this._selectedIndex == 0)
                            this._hoverOk = true;
                        else
                            this._hoverCancel = true;
                    }
                }
            }
            if (!Editor.tookInput && this._hoverOk && ((Editor.inputMode != EditorInput.Mouse || Mouse.left != InputState.Pressed ? (Input.Pressed("SELECT") ? 1 : 0) : 1) | (flag1 ? 1 : 0)) != 0)
            {
                this.result = true;
                this.CompleteDialogue();
            }
            if (Editor.tookInput || !this._hoverCancel || ((Editor.inputMode != EditorInput.Mouse || Mouse.left != InputState.Pressed ? (Input.Pressed("SELECT") ? 1 : 0) : 1) | (flag2 ? 1 : 0)) == 0)
                return;
            this.result = false;
            this.CompleteDialogue();
        }

        private void CompleteDialogue()
        {
            this.opened = false;
            Editor.tookInput = true;
            Editor.lockInput = (ContextMenu)null;
            if (this._ownerMenu == null)
            {
                if (!(Level.current is Editor))
                    return;
                (Level.current as Editor).CompleteDialogue(this.confirmItem);
            }
            else
            {
                if (this.confirmItem == null || this._ownerMenu == null || !this.result)
                    return;
                this._ownerMenu.Selected(this.confirmItem);
            }
        }

        public override void Draw()
        {
            if (!this.opened)
                return;
            base.Draw();
            Graphics.DrawRect(new Vec2(0.0f, 0.0f), new Vec2(Layer.HUD.width, Layer.HUD.height), Color.Black * 0.5f, this.depth - 2);
            float num1 = 300f;
            float num2 = 80f;
            Vec2 p1_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0) + this.windowYOffsetAdd);
            Vec2 p2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0) + this.windowYOffsetAdd);
            float num3 = (float)(((double)p2.x - (double)p1_1.x) / 2.0);
            if (this._description != null)
            {
                int num4 = 18;
                foreach (string text in this._description)
                {
                    float stringWidth = Graphics.GetStringWidth(text);
                    Graphics.DrawString(text, p1_1 + new Vec2(num3 - stringWidth / 2f, (float)(5 + num4)), Color.White, this.depth + 2);
                    num4 += 8;
                    p2.y += 8f;
                }
            }
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), this.depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), this.depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), this.depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), this.depth + 1);
            float stringWidth1 = Graphics.GetStringWidth(this._text);
            Graphics.DrawString(this._text, p1_1 + new Vec2(num3 - stringWidth1 / 2f, 5f), Color.White, this.depth + 2);
            this._font.scale = new Vec2(2f, 2f);
            if (this.okayOnly)
            {
                Vec2 vec2 = new Vec2(120f, 40f);
                Vec2 p1_2 = new Vec2((float)((double)this.x + (double)num3 - (double)vec2.x / 2.0 - 2.0), p2.y - 50f);
                Graphics.DrawRect(p1_2, p1_2 + vec2, this._hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
                this._font.Draw("OK", (float)((double)p1_2.x + (double)vec2.x / 2.0 - (double)this._font.GetWidth("OK") / 2.0), p1_2.y + 12f, Color.White, this.depth + 3);
            }
            else
            {
                Vec2 p1_3 = new Vec2(p1_1.x + 18f, p2.y - 50f);
                Vec2 vec2_1 = new Vec2(120f, 40f);
                Graphics.DrawRect(p1_3, p1_3 + vec2_1, this._hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
                this._font.Draw("OK", (float)((double)p1_3.x + (double)vec2_1.x / 2.0 - (double)this._font.GetWidth("OK") / 2.0), p1_3.y + 12f, Color.White, this.depth + 3);
                Vec2 p1_4 = new Vec2(p1_1.x + 160f, p2.y - 50f);
                Vec2 vec2_2 = new Vec2(120f, 40f);
                Graphics.DrawRect(p1_4, p1_4 + vec2_2, this._hoverCancel ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
                this._font.Draw("CANCEL", (float)((double)p1_4.x + (double)vec2_2.x / 2.0 - (double)this._font.GetWidth("CANCEL") / 2.0), p1_4.y + 12f, Color.White, this.depth + 3);
            }
            this.bottomRightPos = p2.y;
        }
    }
}
