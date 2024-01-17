using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class MaterialGrid : Material
    {
        private Thing _thing;
        private float transWave = 0.2f;
        private bool secondScan;
        //private bool scanSwitch;
        public bool finished;

        public MaterialGrid(Thing t)
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/wireframeTexOuya");
            _thing = t;
        }

        public override void Update()
        {
            transWave -= 0.12f;
            if (Math.Sin(transWave) < -0.7f && !secondScan)
            {
                secondScan = true;
                transWave -= 2f;
            }
            else if (Math.Sin(transWave) > 0.9f && secondScan) finished = true;
            base.Update();
        }

        public override void Apply()
        {
            Matrix fullMatrix = Layer.Game.fullMatrix;
            Vec3 vec3_1 = Vec3.Transform(new Vec3(_thing.x - 28f, _thing.y, 0f), fullMatrix);
            Vec3 vec3_2 = Vec3.Transform(new Vec3(_thing.x + 28f, _thing.y, 0f), fullMatrix);
            SetValue("scan", vec3_1.x + (float)((Math.Sin(transWave) + 1f) / 2f * (vec3_2.x - vec3_1.x)));
            SetValue("secondScan", secondScan ? 1f : 0f);
            base.Apply();
        }
    }
}
