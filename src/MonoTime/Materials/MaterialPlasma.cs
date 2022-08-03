// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialPlasma
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialPlasma : Material
    {
        public float offset;
        public float offset2;
        public float scroll;
        public float scroll2;
        public float gradientOffset;
        public float gradientOffset2;
        public Color color1;
        public Color color2;
        private Texture2D _gradient;
        private Texture2D _plasma2;

        public MaterialPlasma()
        {
            _effect = Content.Load<MTEffect>("Shaders/plasma");
            _gradient = Content.Load<Texture2D>("arcade/gradient");
            _plasma2 = Content.Load<Texture2D>("arcade/plasma2");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            _effect.effect.Parameters["offset"].SetValue(offset);
            _effect.effect.Parameters["offset2"].SetValue(offset2);
            _effect.effect.Parameters["scroll"].SetValue(scroll);
            _effect.effect.Parameters["scroll2"].SetValue(scroll2);
            _effect.effect.Parameters["gradientOffset"].SetValue(gradientOffset);
            _effect.effect.Parameters["gradientOffset2"].SetValue(gradientOffset2);
            _effect.effect.Parameters["color1"].SetValue((Vector4)color1.ToVector4());
            _effect.effect.Parameters["color2"].SetValue((Vector4)color2.ToVector4());
            DuckGame.Graphics.device.Textures[1] = _gradient;
            DuckGame.Graphics.device.Textures[2] = _plasma2;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            DuckGame.Graphics.device.SamplerStates[0] = SamplerState.PointWrap;
            DuckGame.Graphics.device.SamplerStates[2] = SamplerState.PointWrap;
        }
    }
}
