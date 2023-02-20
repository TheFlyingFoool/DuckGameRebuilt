// Decompiled with JetBrains decompiler
// Type: DuckGame.BIOSScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BIOSScreen : Level
    {
        private BitmapFont _font;
        private float _wait = 1f;
        private bool _playedMusic;
        private float _moveWait = 1f;
        private float _shiftText;
        //private int great;

        public BIOSScreen() => _centeredView = true;

        public override void Initialize()
        {
            if (!Steam.IsInitialized())
            {
                bool flag = false;
                if (!MonoMain.breakSteam)
                    flag = Steam.InitializeCore();
                if (flag)
                    Steam.Initialize();
            }
            _font = new BitmapFont("biosFont", 8);
            base.Initialize();
        }

        public override void Update()
        {
            TitleScreen.SpargLogic();

            _wait -= 0.008f;
            if (_wait >= 0.0)
                return;
            if (!_playedMusic)
            {
                Music.Play("Title");
                _playedMusic = true;
            }
            _moveWait -= 0.015f;
            if (_moveWait >= 0.0)
                return;
            _shiftText += 3.5f;
            if (_shiftText <= 300.0)
                return;
            current = new CorptronLogo();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.Game)
                return;
            _font.Draw("   PRODUCED BY OR", 80f + _shiftText, 66f, Color.White);
            _font.Draw(" UNDER LICENSE FROM", 80f - _shiftText, 82f, Color.White);
            _font.Draw("CORPTRON SYSTEMS LTD.", 80f + _shiftText, 98f, Color.White);
        }
    }
}
