// Decompiled with JetBrains decompiler
// Type: DuckGame.UIUnlockBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIUnlockBox : UIMenu
    {
        private Sprite _frame;
        private Sprite _wrappedFrame;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private bool _wrapped = true;
        private bool _flash;
        private Unlockable _unlock;
        private List<Unlockable> _unlocks;
        private float yOffset = 150f;
        public bool down = true;
        private float _downWait = 1f;
        private string _oldSong;
        private float _openWait = 1f;
        public bool finished;

        public UIUnlockBox(List<Unlockable> unlocks, float xpos, float ypos, float wide = -1f, float high = -1f)
          : base("", xpos, ypos, wide, high)
        {
            Graphics.fade = 1f;
            this._frame = new Sprite("unlockFrame");
            this._frame.CenterOrigin();
            this._wrappedFrame = new Sprite("unlockFrameWrapped");
            this._wrappedFrame.CenterOrigin();
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFont");
            this._unlocks = unlocks;
            this._unlock = this._unlocks.First<Unlockable>();
        }

        public override void Open() => base.Open();

        public override void Update()
        {
            this.yOffset = Lerp.FloatSmooth(this.yOffset, this.down ? 150f : 0f, 0.3f, 1.1f);
            if (this.down)
            {
                if (this._unlocks.Count == 0)
                {
                    if (!this.finished)
                    {
                        this.finished = true;
                        this.Close();
                    }
                }
                else
                {
                    this._downWait -= 0.06f;
                    if (_downWait <= 0.0)
                    {
                        this._openWait = 1f;
                        this._wrapped = true;
                        this._downWait = 1f;
                        this._unlock = this._unlocks.First<Unlockable>();
                        this._unlocks.RemoveAt(0);
                        this.down = false;
                        SFX.Play("pause", 0.6f);
                    }
                }
            }
            else
            {
                this._openWait -= 0.06f;
                if (_openWait <= 0.0 && this._wrapped && !this._flash)
                    this._flash = true;
                if (this._flash)
                {
                    Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 1f, 0.2f);
                    if ((double)Graphics.flashAdd > 0.990000009536743)
                    {
                        this._wrapped = !this._wrapped;
                        if (!this._wrapped)
                        {
                            if (this._unlock != null && this._unlock.name == "UR THE BEST")
                            {
                                this._oldSong = Music.currentSong;
                                Music.Play("jollyjingle");
                            }
                            SFX.Play("harp");
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                            this._unlock.DoUnlock();
                        }
                        this._flash = false;
                    }
                }
                else
                    Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 0f, 0.2f);
                if (!this._wrapped && Input.Pressed("SELECT"))
                {
                    HUD.CloseAllCorners();
                    SFX.Play("resume", 0.6f);
                    if (this._oldSong != null && this._unlock != null && this._unlock.name == "UR THE BEST")
                        Music.Play(this._oldSong);
                    this.down = true;
                }
            }
            base.Update();
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
                this._frame.depth = -0.9f;
                Graphics.Draw(this._frame, this.x, this.y);
                string text1 = "@LWING@UNLOCK@RWING@";
                if (this._unlock.name == "UR THE BEST")
                    text1 = "@LWING@WOAH!@RWING@";
                Vec2 vec2_1 = new Vec2((float)-((double)this._font.GetWidth(text1) / 2.0), -42f);
                this._font.DrawOutline(text1, this.position + vec2_1, Color.White, Color.Black, this.depth + 2);
                string text2 = "} " + this._unlock.name + " }";
                this._fancyFont.scale = new Vec2(1f, 1f);
                Vec2 vec2_2 = new Vec2((float)-((double)this._fancyFont.GetWidth(text2) / 2.0), -25f);
                this._fancyFont.DrawOutline(text2, this.position + vec2_2, Colors.DGYellow, Color.Black, this.depth + 2);
                this._fancyFont.scale = new Vec2(0.5f, 0.5f);
                string description = this._unlock.description;
                Vec2 vec2_3 = new Vec2((float)-((double)this._fancyFont.GetWidth(description) / 2.0), 38f);
                this._fancyFont.DrawOutline(description, this.position + vec2_3, Colors.DGGreen, Color.Black, this.depth + 2, 0.5f);
                this._unlock.Draw(this.x, this.y + 10f, this.depth + 4);
            }
            this.y -= this.yOffset;
        }
    }
}
