// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialGrid
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class MaterialGrid : Material // works with base & Spriteatlas
    {
        private Thing _thing;
        private float transWave = 0.2f;
        private bool secondScan;
        //private bool scanSwitch;
        public bool finished;

        public MaterialGrid(Thing t)
        {
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
            else if (Math.Sin(transWave) > 0.9f && secondScan)
                finished = true;
            base.Update();
        }

        public override void Apply()
        {
            Matrix fullMatrix = Layer.Game.fullMatrix;
            Vec3 vec3_1 = Vec3.Transform(new Vec3(_thing.x - 28f, _thing.y, 0f), fullMatrix);
            Vec3 vec3_2 = Vec3.Transform(new Vec3(_thing.x + 28f, _thing.y, 0f), fullMatrix);
            SetValue("scan", vec3_1.x + (float)((Math.Sin(transWave) + 1f) / 2f * (vec3_2.x - vec3_1.x)));
            SetValue("secondScan", secondScan ? 1f : 0f);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
