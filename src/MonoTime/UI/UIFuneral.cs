// Decompiled with JetBrains decompiler
// Type: DuckGame.UIFuneral
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIFuneral : UIMenu
    {
        private Sprite _frame;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private float yOffset = 150f;
        public bool down = true;
        private float _downWait = 1f;
        private SpriteMap _portraitSprite;
        private Sprite _portraitFrame;
        private bool _shown;
        private UIMenu _link;
        public static string oldSong;
        private bool _doneDown;
        //private float _openWait = 1f;
        public bool finished;

        public UIFuneral(float xpos, float ypos, float wide = -1f, float high = -1f, UIMenu link = null)
          : base("", xpos, ypos, wide, high)
        {
            Graphics.fade = 1f;
            this._frame = new Sprite("deathFrame");
            this._frame.CenterOrigin();
            this._link = link;
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFont");
            this._portraitFrame = new Sprite("funeralPic");
            this._portraitSprite = new SpriteMap("littleMan", 16, 16)
            {
                frame = UILevelBox.LittleManFrame(Profiles.experienceProfile.numLittleMen - 9, -1)
            };
        }

        public override void OnClose()
        {
            base.OnClose();
            Profiles.Save(Profiles.experienceProfile);
            if (this._link == null)
                return;
            MonoMain.pauseMenu = _link;
        }

        public override void Open() => base.Open();

        public override void Update()
        {
            if (this.finished)
            {
                this._animating = false;
            }
            else
            {
                this.yOffset = Lerp.FloatSmooth(this.yOffset, this.down ? 150f : 0f, 0.3f, 1.1f);
                if (this.down)
                {
                    this._downWait -= 0.06f;
                    if (_downWait <= 0.0)
                    {
                        if (this._doneDown)
                        {
                            this.finished = true;
                            this.Close();
                            return;
                        }
                        //this._openWait = 1f;
                        this._downWait = 1f;
                        this.down = false;
                        SFX.Play("pause", 0.6f);
                    }
                }
                else
                {
                    if (!this._shown)
                    {
                        UIFuneral.oldSong = Music.currentSong;
                        Music.Play("littlesad", false);
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                        this._shown = true;
                    }
                    if (Input.Pressed("SELECT"))
                    {
                        HUD.CloseAllCorners();
                        SFX.Play("resume", 0.6f);
                        this.down = true;
                        this._doneDown = true;
                    }
                }
                base.Update();
            }
        }

        public override void Draw()
        {
            this.y += this.yOffset;
            this._frame.depth = this.depth;
            Graphics.Draw(this._frame, this.x, this.y);
            string text1 = "FAREWELL";
            Vec2 vec2_1 = new Vec2((float)-((double)this._font.GetWidth(text1) / 2.0), -34f);
            this._font.DrawOutline(text1, this.position + vec2_1, Color.White, Color.Black, this.depth + 2);
            string text2 = "Little man who's come to pass,\nRest in peace beneath the grass.\nHeavy souls weigh on this day,\nwe send a little man on his way.\n\n\nMay Angles Lead You In.";
            Vec2 vec2_2 = new Vec2(-33f, -15f);
            this._fancyFont.scale = new Vec2(0.5f, 0.5f);
            this._fancyFont.Draw(text2, this.position + vec2_2, new Color(27, 38, 50), this.depth + 2);
            Vec2 vec2_3 = new Vec2(-53f, -4f);
            this._portraitSprite.depth = this.depth + 2;
            this._portraitFrame.depth = this.depth + 4;
            Graphics.Draw(_portraitSprite, (float)(position.x + (double)vec2_3.x + 1.0), (float)(position.y + (double)vec2_3.y + 1.0), new Rectangle(2f, 0f, 12f, 10f));
            Graphics.Draw(this._portraitFrame, (float)(position.x + (double)vec2_3.x - 2.0), (float)(position.y + (double)vec2_3.y - 2.0));
            Graphics.DrawRect(this.position + vec2_3, this.position + vec2_3 + new Vec2(13f, 13f), Colors.DGBlue, this.depth + 1);
            this.y -= this.yOffset;
        }
    }
}
