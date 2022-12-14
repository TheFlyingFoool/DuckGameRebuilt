// Decompiled with JetBrains decompiler
// Type: DuckGame.UIUnlockBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _frame = new Sprite("unlockFrame");
            _frame.CenterOrigin();
            _wrappedFrame = new Sprite("unlockFrameWrapped");
            _wrappedFrame.CenterOrigin();
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFont");
            _unlocks = unlocks;
            _unlock = _unlocks[0];
        }

        public override void Open() => base.Open();

        public override void Update()
        {
            yOffset = Lerp.FloatSmooth(yOffset, down ? 150f : 0f, 0.3f, 1.1f);
            if (down)
            {
                if (_unlocks.Count == 0)
                {
                    if (!finished)
                    {
                        finished = true;
                        Close();
                    }
                }
                else
                {
                    _downWait -= 0.06f;
                    if (_downWait <= 0.0)
                    {
                        _openWait = 1f;
                        _wrapped = true;
                        _downWait = 1f;
                        _unlock = _unlocks[0];
                        _unlocks.RemoveAt(0);
                        down = false;
                        SFX.Play("pause", 0.6f);
                    }
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
                            if (_unlock != null && _unlock.name == "UR THE BEST")
                            {
                                _oldSong = Music.currentSong;
                                Music.Play("jollyjingle");
                            }
                            SFX.Play("harp");
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@CONTINUE");
                            _unlock.DoUnlock();
                        }
                        _flash = false;
                    }
                }
                else
                    Graphics.flashAdd = Lerp.Float(Graphics.flashAdd, 0f, 0.2f);
                if (!_wrapped && Input.Pressed(Triggers.Select))
                {
                    HUD.CloseAllCorners();
                    SFX.Play("resume", 0.6f);
                    if (_oldSong != null && _unlock != null && _unlock.name == "UR THE BEST")
                        Music.Play(_oldSong);
                    down = true;
                }
            }
            base.Update();
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
                _frame.depth = -0.9f;
                Graphics.Draw(_frame, x, y);
                string text1 = "@LWING@UNLOCK@RWING@";
                if (_unlock.name == "UR THE BEST")
                    text1 = "@LWING@WOAH!@RWING@";
                Vec2 vec2_1 = new Vec2((float)-(_font.GetWidth(text1) / 2.0), -42f);
                _font.DrawOutline(text1, position + vec2_1, Color.White, Color.Black, depth + 2);
                string text2 = "} " + _unlock.name + " }";
                _fancyFont.scale = new Vec2(1f, 1f);
                Vec2 vec2_2 = new Vec2((float)-(_fancyFont.GetWidth(text2) / 2.0), -25f);
                _fancyFont.DrawOutline(text2, position + vec2_2, Colors.DGYellow, Color.Black, depth + 2);
                _fancyFont.scale = new Vec2(0.5f, 0.5f);
                string description = _unlock.description;
                Vec2 vec2_3 = new Vec2((float)-(_fancyFont.GetWidth(description) / 2.0), 38f);
                _fancyFont.DrawOutline(description, position + vec2_3, Colors.DGGreen, Color.Black, depth + 2, 0.5f);
                _unlock.Draw(x, y + 10f, depth + 4);
            }
            y -= yOffset;
        }
    }
}
