// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialEnergyBlade
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialEnergyBlade : Material
    {
        private MTEffect _baseeffect;
        private MTEffect _sbeffect;
        private Tex2D _energyTexture;
        private OldEnergyScimi _thing;
        private EnergyScimitar _thing2;
        private float _time;
        public float glow;

        public MaterialEnergyBlade(OldEnergyScimi t)
        {
            spsupport = true;
            _baseeffect = Content.Load<MTEffect>("Shaders/energyBlade");
            _sbeffect = Content.Load<MTEffect>("Shaders/sbenergyBlade");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _effect = _baseeffect;
            _thing = t;
        }

        public MaterialEnergyBlade(EnergyScimitar t)
        {
            spsupport = true;
            _baseeffect = Content.Load<MTEffect>("Shaders/energyBlade");
            _sbeffect = Content.Load<MTEffect>("Shaders/sbenergyBlade");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _effect = _baseeffect;
            _thing2 = t;
        }
        public override void Apply()
        {
            if(MonoMain.UpdateLerpState)
                _time += 0.016f;
            bool isspritebatch = false;
            if (batchItem != null && batchItem.NormalTexture != null && batchItem.NormalTexture.Namebase != null && Content.offests.ContainsKey("energyTex") && Content.offests.ContainsKey(batchItem.NormalTexture.Namebase))
            {
                _effect = _sbeffect;
                isspritebatch = true;
            }
            else
            {
                _effect = _baseeffect;
            }
            if (Graphics.device.Textures[0] != null)
            {
                if (isspritebatch)
                {
                    if (_thing != null)
                    {
                        Microsoft.Xna.Framework.Rectangle r = Content.offests[batchItem.NormalTexture.Namebase];
                        Microsoft.Xna.Framework.Rectangle r2 = Content.offests["energyTex"]; //_thing2._blade.texture
                        SetValue("width", batchItem.NormalTexture.frameWidth / batchItem.NormalTexture.width);
                        SetValue("height", batchItem.NormalTexture.frameHeight / batchItem.NormalTexture.height);
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
                        Microsoft.Xna.Framework.Rectangle r = Content.offests[batchItem.NormalTexture.Namebase];
                        Microsoft.Xna.Framework.Rectangle r2 = Content.offests["energyTex"];
                        SetValue("width", batchItem.NormalTexture.frameWidth / batchItem.NormalTexture.width);
                        SetValue("height", batchItem.NormalTexture.frameHeight / batchItem.NormalTexture.height);
                        SetValue("xpos", _thing2.x);
                        SetValue("ypos", _thing2.y);
                        SetValue("time", _time); // _time
                        SetValue("glow", glow);
                        SetValue("bladeColor", _thing2.swordColor);
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
                }
                else
                {
                    Tex2D texture = Graphics.device.Textures[0] as Texture2D;
                    SetValue("width", texture.frameWidth / texture.width);
                    SetValue("height", texture.frameHeight / texture.height);
                    if (_thing != null)
                    {
                        SetValue("xpos", _thing.x);
                        SetValue("ypos", _thing.y);
                        SetValue("time", _time);
                        SetValue("glow", glow);
                        SetValue("bladeColor", _thing.swordColor);
                    }
                    else
                    {
                        SetValue("xpos", _thing2.x);
                        SetValue("ypos", _thing2.y);
                        SetValue("time", _time);
                        SetValue("glow", glow);
                        SetValue("bladeColor", _thing2.swordColor);
                    }
                }
            }
            Graphics.device.Textures[1] = (Texture2D)_energyTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
