// Decompiled with JetBrains decompiler
// Type: DuckGame.TestSuccessDialogue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class TestSuccessDialogue : ContextMenu
    {
        private new string _text = "";
        public bool result;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private bool _hoverOk;
        private bool _hoverCancel;
        private string _caption;

        public TestSuccessDialogue()
          : base(null)
        {
        }

        public override void Initialize()
        {
            this.layer = Layer.HUD;
            this.depth = (Depth)0.97f;
            float num1 = 300f;
            float num2 = 40f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            this.position = vec2_1 + new Vec2(4f, 20f);
            this.itemSize = new Vec2(490f, 16f);
            this._root = true;
            this._font = new BitmapFont("biosFont", 8);
            this._fancyFont = new FancyBitmapFont("smallFont");
        }

        public void Open(string text)
        {
            this.opened = true;
            this._text = text;
            this._caption = "It's Good!";
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
            float num1 = 300f;
            float num2 = 90f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            Vec2 vec2_3 = new Vec2(vec2_1.x - 5f, vec2_2.y - 70f) + new Vec2(18f, 28f);
            Vec2 vec2_4 = new Vec2(135f, 30f);
            Vec2 vec2_5 = new Vec2(vec2_1.x + 136f, vec2_2.y - 70f) + new Vec2(18f, 28f);
            Vec2 vec2_6 = new Vec2(135f, 30f);
            this._hoverOk = (double)Mouse.x > vec2_3.x && (double)Mouse.x < vec2_3.x + (double)vec2_4.x && (double)Mouse.y > vec2_3.y && (double)Mouse.y < vec2_3.y + (double)vec2_4.y;
            this._hoverCancel = (double)Mouse.x > vec2_5.x && (double)Mouse.x < vec2_5.x + (double)vec2_6.x && (double)Mouse.y > vec2_5.y && (double)Mouse.y < vec2_5.y + (double)vec2_6.y;
            if (this._selectedIndex < 0)
                this._selectedIndex = 0;
            if (this._selectedIndex > 1)
                this._selectedIndex = 1;
            if (Editor.inputMode == EditorInput.Gamepad)
            {
                this._hoverOk = false;
                if (this._selectedIndex == 0)
                    this._hoverOk = true;
            }
            if (!Editor.tookInput && this._hoverOk && (Mouse.left == InputState.Pressed || Input.Pressed("SELECT")))
            {
                this.result = true;
                this.opened = false;
                Editor.tookInput = true;
            }
            else
            {
                if (Editor.tookInput || !this._hoverCancel || Mouse.left != InputState.Pressed && !Input.Pressed("SELECT"))
                    return;
                this.result = false;
                this.opened = false;
                Editor.tookInput = true;
            }
        }

        public override void Draw()
        {
            if (!this.opened)
                return;
            base.Draw();
            float num1 = 300f;
            float num2 = 90f;
            Vec2 p1_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 p2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), this.depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), this.depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), this.depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), this.depth + 1);
            Graphics.DrawString(this._caption, p1_1 + new Vec2(5f, 5f), Color.White, this.depth + 2);
            this._fancyFont.maxWidth = 300;
            this._fancyFont.Draw(this._text, p1_1 + new Vec2(6f, 22f), Color.White, this.depth + 5);
            this._font.scale = new Vec2(2f, 2f);
            Vec2 p1_2 = new Vec2(p1_1.x - 5f, p2.y - 70f) + new Vec2(18f, 28f);
            Vec2 vec2_1 = new Vec2(135f, 30f);
            Vec2 p1_3 = new Vec2(p1_1.x + 136f, p2.y - 70f) + new Vec2(18f, 28f);
            Vec2 vec2_2 = new Vec2(135f, 30f);
            Graphics.DrawRect(p1_2, p1_2 + vec2_1, this._hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
            string text1 = "PUBLISH!";
            this._font.Draw(text1, (float)(p1_2.x + vec2_1.x / 2.0 - (double)this._font.GetWidth(text1) / 2.0), p1_2.y + 8f, Color.White, this.depth + 3);
            Graphics.DrawRect(p1_3, p1_3 + vec2_2, this._hoverCancel ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
            string text2 = "CANCEL!";
            this._font.Draw(text2, (float)(p1_3.x + vec2_2.x / 2.0 - (double)this._font.GetWidth(text2) / 2.0), p1_2.y + 8f, Color.White, this.depth + 3);
        }
    }
}
