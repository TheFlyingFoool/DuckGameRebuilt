// Decompiled with JetBrains decompiler
// Type: DuckGame.BigTitle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class BigTitle : Thing
    {
        private Sprite _sprite;
        private int _wait;
        private int _count;
        private int _maxCount = 50;
        private float _alpha = 1f;
        private float _fartWait = 1f;
        private bool _showFart;
        private bool _fade;
        private int _lerpNum;
        private List<Color> _lerpColors = new List<Color>()
    {
      Color.White,
      Color.PaleVioletRed,
      Color.Red,
      Color.OrangeRed,
      Color.Orange,
      Color.Yellow,
      Color.YellowGreen,
      Color.Green,
      Color.BlueViolet,
      Color.Purple,
      Color.Pink
    };
        private Color _currentColor;
        private Sprite _demo;

        public bool fade
        {
            get => _fade;
            set => _fade = value;
        }

        public BigTitle()
          : base()
        {
            _sprite = new Sprite("duckGameTitle");
            _demo = new Sprite("demoPro");
            graphic = _sprite;
            depth = (Depth)0.6f;
            graphic.color = Color.Black;
            centery = graphic.height / 2;
            alpha = 0f;
            layer = Layer.HUD;
            _currentColor = _lerpColors[0];
        }

        public override void Initialize()
        {
        }

        public override void Draw()
        {
            Graphics.DrawRect(position + new Vec2(-300f, -30f), position + new Vec2(300f, 30f), Color.Black * 0.6f * alpha, depth - 100);
            if (_showFart)
            {
                _demo.alpha = alpha;
                _demo.depth = (Depth)0.7f;
                Graphics.Draw(_demo, x + 28f, y + 32f);
            }
            base.Draw();
        }

        public override void Update()
        {
            //if (Main.isDemo)
            //{
            //    this._fartWait -= 0.008f;
            //    if (_fartWait < 0.0 && !this._showFart)
            //    {
            //        this._showFart = true;
            //        SFX.Play("fart" + Rando.Int(3).ToString());
            //    }
            //}
            ++_wait;
            if (_fade)
            {
                alpha -= 0.05f;
                if (alpha >= 0f)
                    return;
                Level.Remove(this);
            }
            else
            {
                if (_wait <= 30 || _count >= _maxCount)
                    return;
                _lerpNum = (int)((_count / _maxCount) * _lerpColors.Count - 0.01f);
                int num = _maxCount / _lerpColors.Count;
                _currentColor = Color.Lerp(_currentColor, _lerpColors[_lerpNum], 0.1f);
                _currentColor.a = (byte)(_alpha * byte.MaxValue);
                _alpha -= 0.02f;
                if (_alpha < 0f)
                    _alpha = 0f;
                ++_count;
            }
        }
    }
}
