// Decompiled with JetBrains decompiler
// Type: DuckGame.NotifyDialogue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NotifyDialogue : ContextMenu
    {
        private new string _text = "";
        public bool result;
        private BitmapFont _font;
        private bool _hoverOk;

        public NotifyDialogue()
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
            _font = new BitmapFont("biosFont", 8);
        }

        public void Open(string text, string startingText = "")
        {
            opened = true;
            _text = text;
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
                opened = false;
            }
            if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed)
            {
                result = false;
                opened = false;
            }
            float num1 = 300f;
            float num2 = 80f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            Vec2 vec2_3 = vec2_1 + new Vec2(18f, 28f);
            Vec2 vec2_4 = new Vec2(num1 - 40f, 40f);
            Vec2 vec2_5 = vec2_1 + new Vec2(160f, 28f);
            Vec2 vec2_6 = new Vec2(120f, 40f);
            _hoverOk = Mouse.x > vec2_3.x && Mouse.x < vec2_3.x + vec2_4.x && Mouse.y > vec2_3.y && Mouse.y < vec2_3.y + vec2_4.y;
            if (!Editor.tookInput && Input.Pressed("MENULEFT"))
                --_selectedIndex;
            else if (!Editor.tookInput && Input.Pressed("MENURIGHT"))
                ++_selectedIndex;
            if (_selectedIndex < 0)
                _selectedIndex = 0;
            if (_selectedIndex > 1)
                _selectedIndex = 1;
            if (Editor.inputMode == EditorInput.Gamepad)
            {
                _hoverOk = false;
                if (_selectedIndex == 0)
                    _hoverOk = true;
            }
            if (Editor.tookInput || !_hoverOk || Mouse.left != InputState.Pressed && !Input.Pressed("SELECT"))
                return;
            result = true;
            opened = false;
            Editor.tookInput = true;
        }

        public override void Draw()
        {
            if (!opened)
                return;
            base.Draw();
            float num1 = 300f;
            float num2 = 80f;
            Vec2 p1_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 p2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), depth + 1);
            Graphics.DrawString(_text, p1_1 + new Vec2(5f, 5f), Color.White, depth + 2);
            _font.scale = new Vec2(2f, 2f);
            Vec2 p1_2 = p1_1 + new Vec2(18f, 28f);
            Vec2 vec2 = new Vec2(num1 - 36f, 40f);
            Graphics.DrawRect(p1_2, p1_2 + vec2, _hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
            _font.Draw("OK", (float)(p1_2.x + vec2.x / 2.0 - _font.GetWidth("OK") / 2.0), p1_2.y + 12f, Color.White, depth + 3);
            Graphics.DrawString(_text, p1_1 + new Vec2(5f, 5f), Color.White, depth + 2);
        }
    }
}
