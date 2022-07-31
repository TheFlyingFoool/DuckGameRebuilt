// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialGlitch
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialGlitch : Material
    {
        private Tex2D _goldTexture;
        private Thing _thing;
        public float amount;
        public float yoffset;
        private int lockframes;

        public MaterialGlitch(Thing t)
        {
            this._effect = Content.Load<MTEffect>("Shaders/glitch");
            this._goldTexture = Content.Load<Tex2D>("glitchMap3");
            this._thing = t;
        }

        public override void Apply()
        {
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                this.SetValue("width", texture.frameWidth / texture.width);
                this.SetValue("height", texture.frameHeight / texture.height);
                this.SetValue("frameWidth", texture.frameWidth);
                this.SetValue("amount", this.amount);
                this.SetValue("yoff", this.yoffset);
                this.SetValue("xpos", this._thing.x);
                this.SetValue("ypos", this._thing.y);
            }
            DuckGame.Graphics.device.Textures[1] = (Texture2D)this._goldTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
            if (this.lockframes > 0)
            {
                --this.lockframes;
            }
            else
            {
                if (Rando.Float(1f) > 0.85f)
                    this.lockframes = Rando.Int(2, 12);
                if (Rando.Float(1f) > 0.2f)
                    this.amount = Lerp.Float(this.amount, 0f, 0.05f);
                if (Rando.Float(1f) <= 0.98f)
                    return;
                this.amount += 0.3f;
            }
        }
    }
}
