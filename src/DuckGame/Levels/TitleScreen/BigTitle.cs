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
            get => this._fade;
            set => this._fade = value;
        }

        public BigTitle()
          : base()
        {
            this._sprite = new Sprite("duckGameTitle");
            this._demo = new Sprite("demoPro");
            this.graphic = this._sprite;
            this.depth = (Depth)0.6f;
            this.graphic.color = Color.Black;
            this.centery = this.graphic.height / 2;
            this.alpha = 0f;
            this.layer = Layer.HUD;
            this._currentColor = this._lerpColors[0];
        }

        public override void Initialize()
        {
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.position + new Vec2(-300f, -30f), this.position + new Vec2(300f, 30f), Color.Black * 0.6f * this.alpha, this.depth - 100);
            if (this._showFart)
            {
                this._demo.alpha = this.alpha;
                this._demo.depth = (Depth)0.7f;
                Graphics.Draw(this._demo, this.x + 28f, this.y + 32f);
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
            ++this._wait;
            if (this._fade)
            {
                this.alpha -= 0.05f;
                if (this.alpha >= 0f)
                    return;
                Level.Remove(this);
            }
            else
            {
                if (this._wait <= 30 || this._count >= this._maxCount)
                    return;
                this._lerpNum = (int)((_count / this._maxCount) * _lerpColors.Count - 0.01f);
                int num = this._maxCount / this._lerpColors.Count;
                this._currentColor = Color.Lerp(this._currentColor, this._lerpColors[this._lerpNum], 0.1f);
                this._currentColor.a = (byte)(_alpha * byte.MaxValue);
                this._alpha -= 0.02f;
                if (_alpha < 0f)
                    this._alpha = 0f;
                ++this._count;
            }
        }
    }
}
