// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialEnergyBlade
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Windows.Documents;
using static DuckGame.CMD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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
            _effect = Content.Load<MTEffect>("Shaders/energyBlade");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _thing = t;
        }

        public MaterialEnergyBlade(EnergyScimitar t)
        {
            _effect = Content.Load<MTEffect>("Shaders/energyBlade");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _thing2 = t;
        }

        public override void Apply()
        {
            _time += 0.016f;
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                if (_thing != null)
                {
                   
                    if (!DuckGame.Content.offests.ContainsKey("energyTex") || !DuckGame.Content.offests.ContainsKey(_thing._blade.texture.Namebase))
                    {
                        return;
                    }
                    Microsoft.Xna.Framework.Rectangle r = DuckGame.Content.offests[_thing._blade.texture.Namebase];
                    Microsoft.Xna.Framework.Rectangle r2 = DuckGame.Content.offests["energyTex"]; //_thing2._blade.texture
                    SetValue("width", _thing.graphic.texture.frameWidth / _thing.graphic.texture.width);
                    SetValue("height", _thing.graphic.texture.frameHeight / _thing.graphic.texture.height);
                    SetValue("xpos", _thing.x);
                    SetValue("ypos", _thing.y);
                    SetValue("time", _time);
                    SetValue("glow", glow);
                    SetValue("bladeColor", _thing.swordColor);
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
                    if (!DuckGame.Content.offests.ContainsKey("energyTex") || !DuckGame.Content.offests.ContainsKey(_thing2._blade.texture.Namebase))
                    {
                        return;
                    }
                    Microsoft.Xna.Framework.Rectangle r = DuckGame.Content.offests[_thing2._blade.texture.Namebase];
                    Microsoft.Xna.Framework.Rectangle r2 = DuckGame.Content.offests["energyTex"]; //_thing2._blade.texture
                    SetValue("width", _thing2._blade.texture.frameWidth / _thing2._blade.texture.width); // / _thing2._blade.texture.width
                    SetValue("height", _thing2._blade.texture.frameHeight / _thing2._blade.texture.height); // / _thing2._blade.texture.height
                    SetValue("xpos", _thing2.x);
                    SetValue("ypos", _thing2.y);
                    SetValue("time",_time); // _time
                    SetValue("glow", glow);
                    SetValue("bladeColor", _thing2.swordColor);
                    SetValue("sasize", new Vec2(Content.Thick.width,Content.Thick.height));
                    SetValue("xoffset", r.X / (float)Content.Thick.width);
                    SetValue("yoffset", r.Y / (float)Content.Thick.height);
                    SetValue("spritesizex", r.Width / (float)Content.Thick.width);
                    SetValue("spritesizey", r.Height / (float)Content.Thick.height);
                    SetValue("goldxoffset", r2.X);
                    SetValue("goldyoffset", r2.Y);
                    SetValue("goldsizex", r2.Width);
                    SetValue("goldsizey", r2.Height);



                }
            }
            DuckGame.Graphics.device.Textures[1] = (Texture2D)_energyTexture;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
