// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialKatanaman
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialKatanaman : Material
    {
        private Tex2D _lighting;
        public TeamHat _hat;

        public MaterialKatanaman(TeamHat hat)
        {
            _effect = Content.Load<MTEffect>("shaders/katanaman");
            _lighting = Content.Load<Tex2D>("hats/katanaman_lightmap");
            _hat = hat;
        }

        public override void Apply()
        {
            DuckGame.Graphics.device.Textures[1] = (Texture2D)_lighting;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            SetValue("xpos", _hat.position.x - 32f);
            SetValue("ypos", _hat.position.y - 32f);
            if (_hat.graphic != null)
                SetValue("flipSub", _hat.graphic.flipH ? 1f : 0f);
            float num1 = 150f;
            float num2 = num1 / 64f;
            Vec2 vec2_1 = new Vec2(15f, 11f) * num2;
            Vec2 vec2_2 = Maths.Snap(_hat.position - (vec2_1 - new Vec2(num1 / 2f)), num1, num1);
            SetValue("light1x", vec2_2.x + vec2_1.x);
            SetValue("light1y", vec2_2.y + vec2_1.y);
            Vec2 vec2_3 = new Vec2(49f, 25f) * num2;
            Vec2 vec2_4 = Maths.Snap(_hat.position - (vec2_3 - new Vec2(num1 / 2f)), num1, num1);
            SetValue("light2x", vec2_4.x + vec2_3.x);
            SetValue("light2y", vec2_4.y + vec2_3.y);
            Vec2 vec2_5 = new Vec2(21f, 49f) * num2;
            Vec2 vec2_6 = Maths.Snap(_hat.position - (vec2_5 - new Vec2(num1 / 2f)), num1, num1);
            SetValue("light3x", vec2_6.x + vec2_5.x);
            SetValue("light3y", vec2_6.y + vec2_5.y);
            SetValue("add", Layer.kGameLayerAdd);
            SetValue("fade", Layer.kGameLayerFade);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
