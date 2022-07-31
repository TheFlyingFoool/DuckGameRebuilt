// Decompiled with JetBrains decompiler
// Type: DuckGame.AdultSwimLogo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class AdultSwimLogo : Level
    {
        private BitmapFont _font;
        private Sprite _logo;
        private float _wait = 1f;
        private bool _fading;

        public override void Initialize()
        {
            this._font = new BitmapFont("biosFont", 8);
            this._logo = new Sprite("aslogo");
            Graphics.fade = 0f;
        }

        public override void Update()
        {
            if (!this._fading)
            {
                if (Graphics.fade < 1f)
                    Graphics.fade += 0.013f;
                else
                    Graphics.fade = 1f;
            }
            else if (Graphics.fade > 0f)
            {
                Graphics.fade -= 0.013f;
            }
            else
            {
                Graphics.fade = 0f;
                Level.current = !MonoMain.startInEditor ? new TitleScreen() : Main.editor;
            }
            this._wait -= 3f / 500f;
            if (_wait >= 0f && !Input.Pressed("START") && !Input.Pressed("SELECT"))
                return;
            this._fading = true;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.Game)
                return;
            this._logo.scale = new Vec2(0.25f, 0.25f);
            Graphics.Draw(this._logo, 110f, 38f);
        }
    }
}
