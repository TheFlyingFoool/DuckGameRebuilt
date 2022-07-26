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
                this.SetValue("width", texture.frameWidth / (float)texture.width);
                this.SetValue("height", texture.frameHeight / (float)texture.height);
                this.SetValue("frameWidth", texture.frameWidth);
                this.SetValue("amount", this.amount);
                this.SetValue("yoff", this.yoffset);
                this.SetValue("xpos", this._thing.x);
                this.SetValue("ypos", this._thing.y);
            }
            DuckGame.Graphics.device.Textures[1] = (Texture)(Texture2D)this._goldTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
            if (this.lockframes > 0)
            {
                --this.lockframes;
            }
            else
            {
                if ((double)Rando.Float(1f) > 0.850000023841858)
                    this.lockframes = Rando.Int(2, 12);
                if ((double)Rando.Float(1f) > 0.200000002980232)
                    this.amount = Lerp.Float(this.amount, 0.0f, 0.05f);
                if ((double)Rando.Float(1f) <= 0.980000019073486)
                    return;
                this.amount += 0.3f;
            }
        }
    }
}
