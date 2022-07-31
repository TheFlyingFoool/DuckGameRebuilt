// Decompiled with JetBrains decompiler
// Type: DuckGame.MainOptions
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class MainOptions : Thing
    {
        private List<string> _options;
        private BitmapFont _font = new BitmapFont("biosFont", 8);
        private float _menuWidth;

        public MainOptions(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.layer = Layer.HUD;
            this._font.scale = new Vec2(4f, 4f);
            this._options = new List<string>()
      {
        "MULTIPLAYER",
        "OPTIONS",
        "QUIT"
      };
            float num = 0f;
            foreach (string option in this._options)
            {
                float width = this._font.GetWidth(option);
                if (width > num)
                    num = width;
            }
            this._menuWidth = num + 80f;
        }

        public override void Draw()
        {
            Graphics.DrawRect(new Vec2((float)(Graphics.width / 2.0 - _menuWidth / 2.0), this.y), new Vec2((float)(Graphics.width / 2.0 + _menuWidth / 2.0), this.y + 250f), Color.Black, (Depth)0.9f);
            int num = 0;
            foreach (string option in this._options)
            {
                float width = this._font.GetWidth(option);
                this._font.Draw(option, (float)(Graphics.width / 2.0 - width / 2.0), this.y + 30f + num * 60, Color.White);
                ++num;
            }
        }
    }
}
