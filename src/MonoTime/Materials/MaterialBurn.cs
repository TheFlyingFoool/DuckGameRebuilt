// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialBurn
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialBurn : Material
    {
        private Tex2D _burnTexture;
        private float _burnVal;

        public float burnVal
        {
            get => this._burnVal;
            set => this._burnVal = value;
        }

        public MaterialBurn(float burnVal = 0f)
        {
            this._effect = Content.Load<MTEffect>("Shaders/burn");
            this._burnTexture = Content.Load<Tex2D>("burn");
            this._burnVal = burnVal;
        }

        public override void Apply()
        {
            Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
            DuckGame.Graphics.device.Textures[1] = (Texture2D)this._burnTexture;
            this.SetValue("width", texture.frameWidth / texture.width);
            this.SetValue("height", texture.frameHeight / texture.height);
            this.SetValue("burn", this._burnVal);
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
