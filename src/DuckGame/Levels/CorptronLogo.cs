// Decompiled with JetBrains decompiler
// Type: DuckGame.CorptronLogo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CorptronLogo : Level
    {
        private BitmapFont _font;
        private Sprite _logo;
        private float _wait = 1f;
        private bool _fading;

        public CorptronLogo() => _centeredView = true;

        public override void Initialize()
        {
            _font = new BitmapFont("biosFont", 8);
            _logo = new Sprite("corptron");
            Graphics.fade = 0f;
        }

        public override void Update()
        {
            TitleScreen.SpargLogic();
            if (!_fading)
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
                Level.current = new AdultSwimLogo();
            }
            _wait -= 0.06f;
            if (_wait >= 0f)
                return;
            _fading = true;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.Game)
                return;
            Graphics.Draw(_logo, 32f, 70f);
            _font.Draw("PRESENTED BY", 50f, 60f, Color.White);
        }
    }
}
