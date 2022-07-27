// Decompiled with JetBrains decompiler
// Type: DuckGame.UploadDialogue
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        }

        public void Open(string text, WorkshopItem pItem)
        {
            this.opened = true;
            this._text = text;
            SFX.Play("openClick", 0.4f);
            this._item = pItem;
            //this._uploadIndex = 0;
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
            float num2 = 80f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            Vec2 vec2_3 = vec2_1 + new Vec2(18f, 28f);
            Vec2 vec2_4 = new Vec2(120f, 40f);
            Vec2 vec2_5 = vec2_1 + new Vec2(160f, 28f);
            Vec2 vec2_6 = new Vec2(120f, 40f);
            this._hoverOk = (double)Mouse.x > (double)vec2_3.x && (double)Mouse.x < (double)vec2_3.x + (double)vec2_4.x && (double)Mouse.y > (double)vec2_3.y && (double)Mouse.y < (double)vec2_3.y + (double)vec2_4.y;
            if (!Editor.tookInput && Input.Pressed("MENULEFT"))
                --this._selectedIndex;
            else if (!Editor.tookInput && Input.Pressed("MENURIGHT"))
                ++this._selectedIndex;
            if (this._selectedIndex < 0)
                this._selectedIndex = 0;
            if (this._selectedIndex <= 1)
                return;
            this._selectedIndex = 1;
        }

        public override void Draw()
        {
            if (!this.opened)
                return;
            base.Draw();
            float num1 = 300f;
            float num2 = 60f;
            Vec2 p1_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 p2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            Graphics.DrawRect(p1_1, p2, new Color(70, 70, 70), this.depth, false);
            Graphics.DrawRect(p1_1, p2, new Color(30, 30, 30), this.depth - 1);
            Graphics.DrawRect(p1_1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), this.depth + 1);
            Graphics.DrawRect(p1_1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1_1.y + 16f), new Color(70, 70, 70), this.depth + 1);
            Graphics.DrawString(this._text, p1_1 + new Vec2(5f, 5f), Color.White, this.depth + 2);
            this._font.scale = new Vec2(1f, 1f);
            Vec2 p1_2 = p1_1 + new Vec2(14f, 38f);
            Vec2 vec2 = new Vec2(270f, 16f);
            TransferProgress uploadProgress = this._item.GetUploadProgress();
            float x = (float)uploadProgress.bytesDownloaded / (float)uploadProgress.bytesTotal;
            Graphics.DrawRect(p1_2, p1_2 + vec2 * new Vec2(x, 1f), this._hoverOk ? new Color(80, 80, 80) : new Color(30, 30, 30), this.depth + 2);
            if (uploadProgress.bytesTotal == 0UL)
                this._font.Draw("Waiting...", p1_2.x, p1_2.y - 12f, Color.White, this.depth + 3);
            else
                this._font.Draw("Uploading " + uploadProgress.bytesDownloaded.ToString() + "/" + uploadProgress.bytesTotal.ToString() + "B", p1_2.x, (float)((double)p1_2.y - 12.0), Color.White, this.depth + 3);
        }
    }
}
