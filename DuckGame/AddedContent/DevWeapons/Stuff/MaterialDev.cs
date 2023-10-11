using System;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialDev : Material
    {
        private Thing _thing;
        private float transWave = 0.2f;
        private bool secondScan;
        //private bool scanSwitch;
        public bool finished;

        public Color IN;
        public MaterialDev(Thing t, Color c)
        {
            IN = c;
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/wireFrameDEV");
            _thing = t;
        }

        public override void Update()
        {
            if (MonoMain.UpdateLerpState)
            {
                transWave -= 0.08f;
                if (Math.Sin(transWave) < -0.7f && !secondScan)
                {
                    secondScan = true;
                    transWave -= 2f;
                }
                else if (Math.Sin(transWave) > 0.9f && secondScan) finished = true;
                base.Update();
            }
        }

        public override void Apply()
        {

            Matrix fullMatrix = Layer.Game.fullMatrix;
            Vec3 vec3_1 = Vec3.Transform(new Vec3(_thing.x - 28f, _thing.y, 0f), fullMatrix);
            Vec3 vec3_2 = Vec3.Transform(new Vec3(_thing.x + 28f, _thing.y, 0f), fullMatrix);
            SetValue("scan", vec3_1.x + (float)((Math.Sin(transWave) + 1f) / 2f * (vec3_2.x - vec3_1.x)));
            SetValue("secondScan", secondScan ? 1f : 0f);
            SetValue("R", IN.r / 255f);
            SetValue("G", IN.g / 255f);
            SetValue("B", IN.b / 255f);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
