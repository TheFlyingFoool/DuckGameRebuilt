// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialGold
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialGold : Material  //todo make non sprite batch version, works with Spriteatlas
    {
        private Tex2D _goldTexture;
        private Thing _thing;

        public MaterialGold(Thing t)
        {
            _effect = Content.Load<MTEffect>("Shaders/gold");
            _goldTexture = Content.Load<Tex2D>("bigGold");
            _thing = t;
        }
        public override void Apply()
        {
            if (this.batchItem != null && this.batchItem.NormalTexture != null)
            {
                if (!DuckGame.Content.offests.ContainsKey("bigGold") || !DuckGame.Content.offests.ContainsKey(this.batchItem.NormalTexture.Namebase))
                {
                    return;
                }
                Microsoft.Xna.Framework.Rectangle r = DuckGame.Content.offests[this.batchItem.NormalTexture.Namebase];
                Microsoft.Xna.Framework.Rectangle r2 = DuckGame.Content.offests["bigGold"];
                //bigGold
                SetValue("width", this.batchItem.NormalTexture.frameWidth / (float)this.batchItem.NormalTexture.width);
                SetValue("height", this.batchItem.NormalTexture.frameHeight / (float)this.batchItem.NormalTexture.height);
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);


                //SetValue("xoffset", r.X);
                //SetValue("yoffset", r.Y);
                //SetValue("spritesizex", r.Width);
                //SetValue("spritesizey", r.Height;

                //SetValue("goldxoffset", r2.X);
                //SetValue("goldyoffset", r2.Y);
                //SetValue("goldsizex", r2.Width);
                //SetValue("goldsizey", r2.Height);

                SetValue("sasize", new Vec2(Content.Thick.width, Content.Thick.height));
                SetValue("xoffset", r.X / (float)Content.Thick.width);
                SetValue("yoffset", r.Y / (float)Content.Thick.height);
                SetValue("spritesizex", r.Width / (float)Content.Thick.width);
                SetValue("spritesizey", r.Height / (float)Content.Thick.height);
                SetValue("goldxoffset", r2.X);
                SetValue("goldyoffset", r2.Y);
                SetValue("goldsizex", r2.Width);
                SetValue("goldsizey", r2.Height);
            }
            else
            {
                DevConsole.Log("fck");
            }
            DuckGame.Graphics.device.Textures[1] = (Texture2D)_goldTexture;
            
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
