// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialPause
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _fade;
            set => _fade = value;
        }

        public MaterialPause()
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/pause");
            _watermark = Content.Load<Tex2D>("dc5");
        }

        public override void Apply()
        {
            Graphics.device.Textures[1] = (Texture2D)_watermark;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            SetValue("fade", _fade);
            SetValue("dim", dim);
            SetValue("scrollX", _scrollX);
            SetValue("scrollY", _scrollY);
            SetValue("aspect", Resolution.current.aspect);
            float num = 0.0003f;
            if (MonoMain.UpdateLerpState)
            {
                _rot += num;
                _rot2 += num;
                _scrollX = _rot;
                _scrollY = -_rot2;
            }
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
