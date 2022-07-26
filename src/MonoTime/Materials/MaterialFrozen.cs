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
            this._effect = Content.Load<MTEffect>("Shaders/frozen");
            this._frozenTexture = Content.Load<Tex2D>("frozen");
            this._thing = t;
        }

        public override void Apply()
        {
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                this.SetValue("width", texture.frameWidth / ((float)texture.width * 0.75f));
                this.SetValue("height", texture.frameHeight / ((float)texture.height * 0.75f));
                this.SetValue("xpos", this._thing.x);
                this.SetValue("ypos", this._thing.y);
                this.SetValue("intensity", this.intensity);
            }
            DuckGame.Graphics.device.Textures[1] = (Texture)(Texture2D)this._frozenTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
