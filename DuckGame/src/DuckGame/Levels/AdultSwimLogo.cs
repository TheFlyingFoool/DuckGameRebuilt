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
            _font = new BitmapFont("biosFont", 8);
            _logo = new Sprite("aslogo");
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
                current = !MonoMain.startInEditor ? new TitleScreen() : Main.editor;
            }
            if (_wait < 0.0f || Input.Pressed(Triggers.Start) || Input.Pressed(Triggers.Select))
            {
                _fading = true;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer != Layer.Game)
                return;
            _logo.scale = new Vec2(0.25f, 0.25f);
            Graphics.Draw(_logo, 110f, 38f);
        }
    }
}
