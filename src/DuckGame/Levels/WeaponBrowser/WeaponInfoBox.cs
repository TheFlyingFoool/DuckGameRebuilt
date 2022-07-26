// Decompiled with JetBrains decompiler
// Type: DuckGame.WeaponInfoBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WeaponInfoBox : Thing
    {
        private BitmapFont _font;
        private Sprite _image;

        public WeaponInfoBox(float xpos, float ypos, Gun gun)
          : base(xpos, ypos)
        {
            this._font = new BitmapFont("duckFont", 8);
            this._font.scale = new Vec2(0.5f, 0.5f);
            this.layer = Layer.HUD;
            this._image = gun.GetEditorImage(0, 0, true);
            this._image.depth = (Depth)1f;
            this._image.scale = new Vec2(4f, 4f);
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.position, this.position + new Vec2(100f, 100f), new Color(100, 100, 100), (Depth)0.8f);
            Graphics.Draw(this._image, this.position.x, this.position.y);
        }
    }
}
