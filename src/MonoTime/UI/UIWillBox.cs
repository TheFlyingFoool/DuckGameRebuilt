// Decompiled with JetBrains decompiler
// Type: DuckGame.UIWillBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIWillBox : UIMenu
    {
        private Sprite _frame;
        private Sprite _wrappedFrame;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private bool _wrapped = true;
        private bool _flash;
        private Furniture _furni;
        private float yOffset = 150f;
        public bool down = true;
        private float _downWait = 1f;
        private Sprite _eggSprite;
        private UIMenu _link;
        private string _oldSong;
        private bool _doneDown;
        private float _openWait = 1f;
        public bool finished;

        public UIWillBox(Furniture f, float xpos, float ypos, float wide = -1f, float high = -1f, UIMenu link = null)
          : base("", xpos, ypos, wide, high)
        {
            Graphics.fade = 1f;
            _frame = new Sprite("unlockFrame");
            _frame.CenterOrigin();
            _wrappedFrame = new Sprite("unlockFrameWrapped");
            _wrappedFrame.CenterOrigin();
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFont");
            _furni = f;
            _link = link;
            _eggSprite = Profile.GetEggSprite();
        }

        public override void OnClose()
        {
            base.OnClose();
            Profiles.Save(Profiles.experienceProfile);
            if (_link == null)
                return;
            if (UIFuneral.oldSong != null)
                Music.Play(UIFuneral.oldSong);
            MonoMain.pauseMenu = _link;
        }

        public override void Open() => base.Open();

        public override void Update()
        {
            if (finished)
            {
                _animating = false;
            }
            else
            {
                yOffset = Lerp.FloatSmooth(yOffset, down ? 150f : 0f, 0.3f, 1.1f);
                if (down)
                {
                    _downWait -= 0.06f;
                    if (_downWait <= 0.0)
                    {
                        if (_doneDown)
                        {
                            finished = true;
                            Close();
                            return;
                        }
                        _openWait = 1f;
                        _wrapped = true;
                        _downWait = 1f;
                        down = false;
                        SFX.Play("pause", 0.6f);
                    }
                }
                else
                {
                    _openWait -= 0.06f;
                    if (_openWait <= 0.0 && _wrapped && !_flash)
                        _flash = true;
                    if (_flash)
                    {
                        Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 1f, 0.2f);
                        if (Graphics.flashAdd > 0.99f)
                        {
                            _wrapped = !_wrapped;
                            if (!_wrapped)
                            {
                                _oldSong = Music.currentSong;
                                Profiles.experienceProfile.SetNumFurnitures(_furni.index, Profiles.experienceProfile.GetNumFurnitures(_furni.index) + 1);
                                SFX.Play("harp");
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                            }
                            _flash = false;
                        }
                    }
                    else
                        Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 0f, 0.2f);
                    if (!_wrapped && Input.Pressed("SELECT"))
                    {
                        HUD.CloseAllCorners();
                        SFX.Play("resume", 0.6f);
                        down = true;
                        _doneDown = true;
                    }
                }
                base.Update();
            }
        }

        public override void Draw()
        {
            y += yOffset;
            if (_wrapped)
            {
                _wrappedFrame.depth = depth;
                Graphics.Draw(_wrappedFrame, x, y);
            }
            else
            {
                _frame.depth = depth;
                Graphics.Draw(_frame, x, y);
                string text1 = "LAST WISH";
                Vec2 vec2_1 = new Vec2((float)-(_font.GetWidth(text1) / 2.0), -42f);
                _font.DrawOutline(text1, position + vec2_1, Color.White, Color.Black, depth + 2);
                string text2 = "} " + _furni.name + " }";
                _fancyFont.scale = new Vec2(1f, 1f);
                Vec2 vec2_2 = new Vec2((float)-(_fancyFont.GetWidth(text2) / 2.0), -25f);
                _fancyFont.DrawOutline(text2, position + vec2_2, Colors.DGYellow, Color.Black, depth + 2);
                _fancyFont.scale = new Vec2(0.5f, 0.5f);
                string text3 = "Your little man wanted you to have this.";
                Vec2 vec2_3 = new Vec2((float)-(_fancyFont.GetWidth(text3) / 2.0), 38f);
                _fancyFont.DrawOutline(text3, position + vec2_3, Colors.DGGreen, Color.Black, depth + 2, 0.5f);
                _furni.Draw(position + new Vec2(0f, 10f), depth + 4, _furni.name == "PHOTO" ? 1 : (_furni.name == "EASEL" ? 6 : 0));
            }
            y -= yOffset;
        }
    }
}
