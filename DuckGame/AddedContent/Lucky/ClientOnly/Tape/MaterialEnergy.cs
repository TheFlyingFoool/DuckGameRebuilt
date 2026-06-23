using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialEnergy : Material
    {
        private MTEffect _baseeffect;
        private MTEffect _sbeffect;
        private Tex2D _energyTexture;
        private Thing _thing;
        private float _time;
        public float glow;
        public float timeMulti = 1;


        public MaterialEnergy(Thing t)
        {
            spsupport = true;
            _baseeffect = Content.Load<MTEffect>("Shaders/energyBlade");
            _sbeffect = Content.Load<MTEffect>("Shaders/sbenergyBlade");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _effect = _baseeffect;
            _thing = t;
        }
        public override void Update()
        {
            _time += 0.016f * timeMulti;
            base.Update();
        }
        public Color color;
        public override void Apply()
        {
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
                        SetValue("bladeColor", color);
                        SetValue("sasize", new Vec2(Content.SpriteAtlasTex2D.width, Content.SpriteAtlasTex2D.height));
                        SetValue("xoffset", r.X / (float)Content.SpriteAtlasTex2D.width);
                        SetValue("yoffset", r.Y / (float)Content.SpriteAtlasTex2D.height);
                        SetValue("spritesizex", r.Width / (float)Content.SpriteAtlasTex2D.width);
                        SetValue("spritesizey", r.Height / (float)Content.SpriteAtlasTex2D.height);
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
                        SetValue("bladeColor", color);
                    }
                }
            }
            Graphics.device.Textures[1] = (Texture2D)_energyTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            base.Apply();
        }
    }
}
