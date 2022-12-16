// Decompiled with JetBrains decompiler
// Type: DuckGame.UploadDialogue
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UploadDialogue : ContextMenu
    {
        private new string _text = "";
        private BitmapFont _font;
        private bool _hoverOk;
        private WorkshopItem _item;
        //private int _uploadIndex;

        public UploadDialogue()
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

        public void Open(string text, WorkshopItem pItem)
        {
            opened = true;
            _text = text;
            SFX.Play("openClick", 0.4f);
            _item = pItem;
            //this._uploadIndex = 0;
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
            float num1 = 300f;
            float num2 = 80f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            Vec2 vec2_3 = vec2_1 + new Vec2(18f, 28f);
            Vec2 vec2_4 = new Vec2(120f, 40f);
            Vec2 vec2_5 = vec2_1 + new Vec2(160f, 28f);
            Vec2 vec2_6 = new Vec2(120f, 40f);
            _hoverOk = Mouse.x > vec2_3.x && Mouse.x < vec2_3.x + vec2_4.x && Mouse.y > vec2_3.y && Mouse.y < vec2_3.y + vec2_4.y;
            if (!Editor.tookInput && Input.Pressed(Triggers.MenuLeft))
                --_selectedIndex;
            else if (!Editor.tookInput && Input.Pressed(Triggers.MenuRight))
                ++_selectedIndex;
            if (_selectedIndex < 0)
                _selectedIndex = 0;
            if (_selectedIndex <= 1)
                return;
            _selectedIndex = 1;
        }

        public override void Draw()
        {
            if (!opened)
                return;
            base.Draw();
            float num1 = 300f;
            float num2 = 60f;
            Vec2 p1_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 p2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), depth + 1);
            Graphics.DrawString(_text, p1_1 + new Vec2(5f, 5f), Color.White, depth + 2);
            _font.scale = new Vec2(1f, 1f);
            Vec2 p1_2 = p1_1 + new Vec2(14f, 38f);
            Vec2 vec2 = new Vec2(270f, 16f);
            TransferProgress uploadProgress = _item.GetUploadProgress();
            float x = uploadProgress.bytesDownloaded / (float)uploadProgress.bytesTotal;
            Graphics.DrawRect(p1_2, p1_2 + vec2 * new Vec2(x, 1f), _hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), depth + 2);
            if (uploadProgress.bytesTotal == 0UL)
                _font.Draw("Waiting...", p1_2.x, p1_2.y - 12f, Color.White, depth + 3);
            else
                _font.Draw("Uploading " + uploadProgress.bytesDownloaded.ToString() + "/" + uploadProgress.bytesTotal.ToString() + "B", p1_2.x, (float)(p1_2.y - 12.0), Color.White, depth + 3);
        }
    }
}
