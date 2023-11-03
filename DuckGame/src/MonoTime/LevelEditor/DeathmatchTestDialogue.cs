namespace DuckGame
{
    public class DeathmatchTestDialogue : ContextMenu
    {
        private new string _text = "";
        public int result = -1;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        public static bool success;
        public static bool tooSlow;
        private bool _hoverOk;
        private bool _hoverCancel;
        private bool _hoverBack;
        public static Editor currentEditor;
        private string _caption;

        public DeathmatchTestDialogue()
          : base(null)
        {
        }

        public override void Initialize()
        {
            layer = Layer.HUD;
            depth = (Depth)0.97f;
            float num1 = 300f;
            float num2 = 40f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2f - num1 / 2f), (float)(layer.height / 2f - num2 / 2f));
            //Vec2 vec2_2 = new Vec2((float)(layer.width / 2f + num1 / 2f), (float)(layer.height / 2f + num2 / 2f)); what -NiK0
            position = vec2_1 + new Vec2(4f, 20f);
            itemSize = new Vec2(490f, 16f);
            _root = true;
            _font = new BitmapFont("biosFont", 8);
            _fancyFont = new FancyBitmapFont("smallFont");
        }

        public void Open(string text)
        {
            tooSlow = false;
            opened = true;
            _text = text;
            _caption = "Deathmatch Validity Test!";
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
                result = 0;
                opened = false;
            }
            if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed)
            {
                result = 2;
                opened = false;
            }
            float num1 = 316f;
            float num2 = 155f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2f - num1 / 2f), (float)(layer.height / 2f - num2 / 2f));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2f + num1 / 2f), (float)(layer.height / 2f + num2 / 2f));
            Vec2 vec2_3 = new Vec2(vec2_1.x - 5f, vec2_2.y - 100f) + new Vec2(18f, 28f);
            Vec2 vec2_4 = new Vec2(180f, 30f);
            Vec2 vec2_5 = new Vec2(vec2_1.x + 185f, vec2_2.y - 100f) + new Vec2(18f, 28f);
            Vec2 vec2_6 = new Vec2(100f, 30f);
            Vec2 vec2_7 = new Vec2(vec2_1.x - 5f, vec2_2.y - 66f) + new Vec2(18f, 28f);
            Vec2 vec2_8 = new Vec2(290f, 30f);
            _hoverOk = Mouse.x > vec2_3.x && Mouse.x < vec2_3.x + vec2_4.x && Mouse.y > vec2_3.y && Mouse.y < vec2_3.y + vec2_4.y;
            _hoverCancel = Mouse.x > vec2_5.x && Mouse.x < vec2_5.x + vec2_6.x && Mouse.y > vec2_5.y && Mouse.y < vec2_5.y + vec2_6.y;
            _hoverBack = Mouse.x > vec2_7.x && Mouse.x < vec2_7.x + vec2_8.x && Mouse.y > vec2_7.y && Mouse.y < vec2_7.y + vec2_8.y;
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
            if (!Editor.tookInput && _hoverOk && (Mouse.left == InputState.Pressed || Input.Pressed(Triggers.Select)))
            {
                result = 0;
                opened = false;
                Editor.tookInput = true;
            }
            else if (!Editor.tookInput && _hoverCancel && (Mouse.left == InputState.Pressed || Input.Pressed(Triggers.Select)))
            {
                result = 1;
                opened = false;
                Editor.tookInput = true;
            }
            else
            {
                if (Editor.tookInput || !_hoverBack || Mouse.left != InputState.Pressed && !Input.Pressed(Triggers.Select))
                    return;
                result = 2;
                opened = false;
                Editor.tookInput = true;
            }
        }

        public override void Draw()
        {
            if (!opened)
                return;
            base.Draw();
            float num1 = 316f;
            float num2 = 155f;
            Vec2 p1_1 = new Vec2((float)(layer.width / 2f - num1 / 2f), (float)(layer.height / 2f - num2 / 2f));
            Vec2 p2 = new Vec2((float)(layer.width / 2f + num1 / 2f), (float)(layer.height / 2f + num2 / 2f));
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), depth + 1);
            Graphics.DrawString(_caption, p1_1 + new Vec2(5f, 5f), Color.White, depth + 2);
            _fancyFont.maxWidth = 300;
            _fancyFont.Draw(_text, p1_1 + new Vec2(6f, 22f), Color.White, depth + 5);
            _font.scale = new Vec2(2f, 2f);
            Vec2 p1_2 = new Vec2(p1_1.x - 5f, p2.y - 100f) + new Vec2(18f, 28f);
            Vec2 vec2_1 = new Vec2(180f, 30f);
            Vec2 p1_3 = new Vec2(p1_1.x + 185f, p2.y - 100f) + new Vec2(18f, 28f);
            Vec2 vec2_2 = new Vec2(100f, 30f);
            Vec2 p1_4 = new Vec2(p1_1.x - 5f, p2.y - 66f) + new Vec2(18f, 28f);
            Vec2 vec2_3 = new Vec2(290f, 30f);
            Graphics.DrawRect(p1_2, p1_2 + vec2_1, _hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
            string text1 = "LETS DO IT!";
            _font.Draw(text1, (float)(p1_2.x + vec2_1.x / 2f - _font.GetWidth(text1) / 2f), p1_2.y + 8f, Color.White, depth + 3);
            Graphics.DrawRect(p1_3, p1_3 + vec2_2, _hoverCancel ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
            string text2 = "NOPE!";
            _font.Draw(text2, (float)(p1_3.x + vec2_2.x / 2f - _font.GetWidth(text2) / 2f), p1_2.y + 8f, Color.White, depth + 3);
            Graphics.DrawRect(p1_4, p1_4 + vec2_3, _hoverBack ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
            _font.Draw(Triggers.Cancel, (float)(p1_4.x + vec2_3.x / 2f - _font.GetWidth(text2) / 2f - 4f), p1_4.y + 8f, Color.White, depth + 3);
        }
    }
}
