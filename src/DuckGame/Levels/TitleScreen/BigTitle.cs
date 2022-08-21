// Decompiled with JetBrains decompiler
// Type: DuckGame.BigTitle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

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
        public const string RebuiltSprite = "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";
        public BigTitle()
          : base()
        {
            _sprite = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(RebuiltSprite))), "title"));
<<<<<<< Updated upstream
=======
            _sprite.Namebase = "Rebuilttitle";
            Content.textures[_sprite.Namebase] = _sprite.texture;
>>>>>>> Stashed changes
            _demo = new Sprite("demoPro");
            graphic = _sprite;
            depth = (Depth)0.6f;
            graphic.color = Color.Black;
            centery = graphic.height / 2 - 14;
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
