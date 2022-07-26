// Decompiled with JetBrains decompiler
// Type: DuckGame.UIWillBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._frame = new Sprite("unlockFrame");
            this._frame.CenterOrigin();
            this._wrappedFrame = new Sprite("unlockFrameWrapped");
            this._wrappedFrame.CenterOrigin();
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFont");
            this._furni = f;
            this._link = link;
            this._eggSprite = Profile.GetEggSprite();
        }

        public override void OnClose()
        {
            base.OnClose();
            Profiles.Save(Profiles.experienceProfile);
            if (this._link == null)
                return;
            if (UIFuneral.oldSong != null)
                Music.Play(UIFuneral.oldSong);
            MonoMain.pauseMenu = (UIComponent)this._link;
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
                this.yOffset = Lerp.FloatSmooth(this.yOffset, this.down ? 150f : 0.0f, 0.3f, 1.1f);
                if (this.down)
                {
                    this._downWait -= 0.06f;
                    if ((double)this._downWait <= 0.0)
                    {
                        if (this._doneDown)
                        {
                            this.finished = true;
                            this.Close();
                            return;
                        }
                        this._openWait = 1f;
                        this._wrapped = true;
                        this._downWait = 1f;
                        this.down = false;
                        SFX.Play("pause", 0.6f);
                    }
                }
                else
                {
                    this._openWait -= 0.06f;
                    if ((double)this._openWait <= 0.0 && this._wrapped && !this._flash)
                        this._flash = true;
                    if (this._flash)
                    {
                        Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 1f, 0.2f);
                        if ((double)Graphics.flashAdd > 0.990000009536743)
                        {
                            this._wrapped = !this._wrapped;
                            if (!this._wrapped)
                            {
                                this._oldSong = Music.currentSong;
                                Profiles.experienceProfile.SetNumFurnitures((int)this._furni.index, Profiles.experienceProfile.GetNumFurnitures((int)this._furni.index) + 1);
                                SFX.Play("harp");
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                            }
                            this._flash = false;
                        }
                    }
                    else
                        Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 0.0f, 0.2f);
                    if (!this._wrapped && Input.Pressed("SELECT"))
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
            if (this._wrapped)
            {
                this._wrappedFrame.depth = this.depth;
                Graphics.Draw(this._wrappedFrame, this.x, this.y);
            }
            else
            {
                this._frame.depth = this.depth;
                Graphics.Draw(this._frame, this.x, this.y);
                string text1 = "LAST WISH";
                Vec2 vec2_1 = new Vec2((float)-((double)this._font.GetWidth(text1) / 2.0), -42f);
                this._font.DrawOutline(text1, this.position + vec2_1, Color.White, Color.Black, this.depth + 2);
                string text2 = "} " + this._furni.name + " }";
                this._fancyFont.scale = new Vec2(1f, 1f);
                Vec2 vec2_2 = new Vec2((float)-((double)this._fancyFont.GetWidth(text2) / 2.0), -25f);
                this._fancyFont.DrawOutline(text2, this.position + vec2_2, Colors.DGYellow, Color.Black, this.depth + 2);
                this._fancyFont.scale = new Vec2(0.5f, 0.5f);
                string text3 = "Your little man wanted you to have this.";
                Vec2 vec2_3 = new Vec2((float)-((double)this._fancyFont.GetWidth(text3) / 2.0), 38f);
                this._fancyFont.DrawOutline(text3, this.position + vec2_3, Colors.DGGreen, Color.Black, this.depth + 2, 0.5f);
                this._furni.Draw(this.position + new Vec2(0.0f, 10f), this.depth + 4, this._furni.name == "PHOTO" ? 1 : (this._furni.name == "EASEL" ? 6 : 0));
            }
            this.y -= this.yOffset;
        }
    }
}
