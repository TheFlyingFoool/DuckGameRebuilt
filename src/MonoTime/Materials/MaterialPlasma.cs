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
            this._effect = Content.Load<MTEffect>("Shaders/plasma");
            this._gradient = Content.Load<Texture2D>("arcade/gradient");
            this._plasma2 = Content.Load<Texture2D>("arcade/plasma2");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            this._effect.effect.Parameters["offset"].SetValue(this.offset);
            this._effect.effect.Parameters["offset2"].SetValue(this.offset2);
            this._effect.effect.Parameters["scroll"].SetValue(this.scroll);
            this._effect.effect.Parameters["scroll2"].SetValue(this.scroll2);
            this._effect.effect.Parameters["gradientOffset"].SetValue(this.gradientOffset);
            this._effect.effect.Parameters["gradientOffset2"].SetValue(this.gradientOffset2);
            this._effect.effect.Parameters["color1"].SetValue((Vector4)this.color1.ToVector4());
            this._effect.effect.Parameters["color2"].SetValue((Vector4)this.color2.ToVector4());
            DuckGame.Graphics.device.Textures[1] = (Texture)this._gradient;
            DuckGame.Graphics.device.Textures[2] = (Texture)this._plasma2;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            DuckGame.Graphics.device.SamplerStates[0] = SamplerState.PointWrap;
            DuckGame.Graphics.device.SamplerStates[2] = SamplerState.PointWrap;
        }
    }
}
