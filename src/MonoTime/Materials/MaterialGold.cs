// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialGold
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialGold : Material
    {
        private Tex2D _goldTexture;
        private Thing _thing;

        public MaterialGold(Thing t)
        {
            _effect = Content.Load<MTEffect>("Shaders/gold");
            _goldTexture = Content.Load<Tex2D>("bigGold");
            _thing = t;
        }
        //float xoffset;
        //float yoffset;
        public override void Apply()
        {
            if (_thing.graphic != null && _thing.graphic.texture != null)
            {
                if (!DuckGame.Content.offests.ContainsKey("bigGold") || !DuckGame.Content.offests.ContainsKey(_thing.graphic.texture.Namebase))
                {
                    return;
                }
                Microsoft.Xna.Framework.Rectangle r = DuckGame.Content.offests[_thing.graphic.texture.Namebase];
                Microsoft.Xna.Framework.Rectangle r2 = DuckGame.Content.offests["bigGold"];
                //bigGold
                SetValue("width", _thing.graphic.texture.frameWidth / (float)_thing.graphic.texture.width);
                SetValue("height", _thing.graphic.texture.frameHeight / (float)_thing.graphic.texture.height);
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);
                SetValue("xoffset", r.X / (float)Content.Thick.width);
                SetValue("yoffset", r.Y / (float)Content.Thick.height);
                SetValue("spritesizex", r.Width / (float)Content.Thick.width);
                SetValue("spritesizey", r.Height / (float)Content.Thick.height);

                SetValue("goldxoffset", r2.X / (float)Content.Thick.width);
                SetValue("goldyoffset", r2.Y / (float)Content.Thick.height);
                SetValue("goldsizex", r2.Width / (float)Content.Thick.width);
                SetValue("goldsizey", r2.Height / (float)Content.Thick.height);
            }
            //DuckGame.Graphics.device.Textures[1] = (Texture2D)_goldTexture;
            
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
