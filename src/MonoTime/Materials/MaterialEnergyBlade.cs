// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialEnergyBlade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialEnergyBlade : Material
    {
        private Tex2D _energyTexture;
        private OldEnergyScimi _thing;
        private EnergyScimitar _thing2;
        private float _time;
        public float glow;

        public MaterialEnergyBlade(OldEnergyScimi t)
        {
            this._effect = Content.Load<MTEffect>("Shaders/energyBlade");
            this._energyTexture = Content.Load<Tex2D>("energyTex");
            this._thing = t;
        }

        public MaterialEnergyBlade(EnergyScimitar t)
        {
            this._effect = Content.Load<MTEffect>("Shaders/energyBlade");
            this._energyTexture = Content.Load<Tex2D>("energyTex");
            this._thing2 = t;
        }

        public override void Apply()
        {
            this._time += 0.016f;
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                this.SetValue("width", texture.frameWidth / (float)texture.width);
                this.SetValue("height", texture.frameHeight / (float)texture.height);
                if (this._thing != null)
                {
                    this.SetValue("xpos", this._thing.x);
                    this.SetValue("ypos", this._thing.y);
                    this.SetValue("time", this._time);
                    this.SetValue("glow", this.glow);
                    this.SetValue("bladeColor", this._thing.swordColor);
                }
                else
                {
                    this.SetValue("xpos", this._thing2.x);
                    this.SetValue("ypos", this._thing2.y);
                    this.SetValue("time", this._time);
                    this.SetValue("glow", this.glow);
                    this.SetValue("bladeColor", this._thing2.swordColor);
                }
            }
            DuckGame.Graphics.device.Textures[1] = (Texture)(Texture2D)this._energyTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
