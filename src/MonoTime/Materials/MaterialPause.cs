// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialPause
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialPause : Material
    {
        private Tex2D _watermark;
        private float _fade;
        private float _scrollX;
        private float _scrollY;
        private float _rot;
        private float _rot2;
        public float dim = 0.6f;

        public float fade
        {
            get => this._fade;
            set => this._fade = value;
        }

        public MaterialPause()
        {
            this._effect = Content.Load<MTEffect>("Shaders/pause");
            this._watermark = Content.Load<Tex2D>("dc5");
        }

        public override void Apply()
        {
            DuckGame.Graphics.device.Textures[1] = (Texture2D)this._watermark;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            this.SetValue("fade", this._fade);
            this.SetValue("dim", this.dim);
            this.SetValue("scrollX", this._scrollX);
            this.SetValue("scrollY", this._scrollY);
            this.SetValue("aspect", Resolution.current.aspect);
            float num = 0.0003f;
            this._rot += num;
            this._rot2 += num;
            this._scrollX = this._rot;
            this._scrollY = -this._rot2;
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
