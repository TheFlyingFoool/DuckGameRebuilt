using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialFrozen : Material // to fix remeber both
    {
        private Tex2D _frozenTexture;
        private Thing _thing;
        public float intensity;

        public MaterialFrozen(Thing t)
        {
            spsupport = false;
            _effect = Content.Load<MTEffect>("Shaders/frozen");
            _frozenTexture = Content.Load<Tex2D>("frozen");
            _thing = t;
        }

        public override void Apply()
        {
            Tex2D tex;
            if (_thing.graphic != null)
            {
                tex = _thing.graphic.texture;
            }
            else
            {
                tex = Graphics.device.Textures[0] as Texture2D;
            }
            if (tex != null)
            {
                SetValue("width", tex.frameWidth / (tex.width * 0.75f));
                SetValue("height", tex.frameHeight / (tex.height * 0.75f));
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);
                SetValue("intensity", intensity);
            }
            Graphics.device.Textures[1] = (Texture2D)_frozenTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            base.Apply();
        }
    }
}
