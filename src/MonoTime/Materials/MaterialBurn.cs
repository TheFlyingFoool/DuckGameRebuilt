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
            get => _burnVal;
            set => _burnVal = value;
        }

        public MaterialBurn(float burnVal = 0f)
        {
            _effect = Content.Load<MTEffect>("Shaders/burn");
            _burnTexture = Content.Load<Tex2D>("burn");
            _burnVal = burnVal;
        }

        public override void Apply()
        {
            Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
            DuckGame.Graphics.device.Textures[1] = (Texture2D)_burnTexture;
            SetValue("width", texture.frameWidth / texture.width);
            SetValue("height", texture.frameHeight / texture.height);
            SetValue("burn", _burnVal);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
