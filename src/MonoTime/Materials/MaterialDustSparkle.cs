// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialDustSparkle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialDustSparkle : Material
    {
        private Tex2D _cone;
        public Vec2 position;
        public Vec2 size;
        public float fade;

        public MaterialDustSparkle(Vec2 pos, Vec2 s, bool wide, bool lit)
        {
            this._effect = Content.Load<MTEffect>("Shaders/dustsparkle");
            if (!lit)
            {
                this._cone = Content.Load<Tex2D>("arcade/lightSphere");
                pos.y += 10f;
            }
            else
                this._cone = !wide ? Content.Load<Tex2D>("arcade/lightCone") : Content.Load<Tex2D>("arcade/bigLightCone");
            this.position = pos;
            this.size = s;
        }

        public override void Apply()
        {
            DuckGame.Graphics.device.Textures[1] = (Texture)(Texture2D)this._cone;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            this.SetValue("topLeft", this.position);
            this.SetValue("size", this.size);
            this.SetValue("fade", Layer.Game.fade * this.fade);
            this.SetValue("viewMatrix", DuckGame.Graphics.screen.viewMatrix);
            this.SetValue("projMatrix", DuckGame.Graphics.screen.projMatrix);
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
