// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialFrozen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialFrozen : Material
    {
        private Tex2D _frozenTexture;
        private Thing _thing;
        public float intensity;

        public MaterialFrozen(Thing t)
        {
            _effect = Content.Load<MTEffect>("Shaders/frozen");
            _frozenTexture = Content.Load<Tex2D>("frozen");
            _thing = t;
        }

        public override void Apply()
        {
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                SetValue("width", texture.frameWidth / (texture.width * 0.75f));
                SetValue("height", texture.frameHeight / (texture.height * 0.75f));
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);
                SetValue("intensity", intensity);
            }
            DuckGame.Graphics.device.Textures[1] = (Texture2D)_frozenTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
