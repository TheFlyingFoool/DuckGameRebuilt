using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace DuckGame
{
    public class MaterialLavaWobble : Material
    {
        public MaterialLavaWobble(FluidPuddle thing)
        {
            theOne = thing;
            _effect = Content.Load<MTEffect>("Shaders/lavaWobble");
        }
        public float time;
        public Vec2 topLeft;
        public Vec2 bottomRight;
        public FluidPuddle theOne;
        public override void Apply()
        {
            time += 0.005f;

            topLeft = theOne.topLeft - new Vec2(48, 64);
            bottomRight = theOne.topRight + new Vec2(48, 16);

            //24 0

            //0-1 left 0 right 1
            //0-1 bottom 0 top 1
            //transform from game to screen to uv
            SetValue("time", time);
            SetValue("glow", 0.1f);
            SetValue("gL", topLeft.x);
            SetValue("gR", bottomRight.x);
            SetValue("gT", topLeft.y);
            SetValue("gB", bottomRight.y);

            Vec3 vec3 = Vec3.Transform(new Vec3(topLeft.x, topLeft.y, 0f), Matrix.Invert(Matrix.CreateScale(1, 1, 1)) * Level.current.camera.getMatrix());
            SetValue("uvL", vec3.x / Resolution.current.x);
            SetValue("uvB", vec3.y / Resolution.current.y);
            vec3 = Vec3.Transform(new Vec3(bottomRight.x, bottomRight.y, 0f), Matrix.Invert(Matrix.CreateScale(1, 1, 1)) * Level.current.camera.getMatrix());
            DevConsole.Log(Math.Abs(vec3.y / Resolution.current.y - 1));
            SetValue("uvT", vec3.y / Resolution.current.y);
            SetValue("uvR", vec3.x / Resolution.current.x);

            /*SetValue("uvL", Level.current.camera.transformWorldVector(topLeft).x / Resolution.current.x);
            SetValue("uvR", Level.current.camera.transformWorldVector(bottomRight).x / Resolution.current.x);
            SetValue("uvB", Level.current.camera.transformWorldVector(topLeft).y / Resolution.current.y);
            SetValue("uvT", Level.current.camera.transformWorldVector(bottomRight).y / Resolution.current.y);*/

            //Graphics.device.Textures[0] = thing.graphic.texture;
            //Graphics.device.Textures[1] = thing.graphic.texture;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            foreach (EffectPass effectPass in this._effect.effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
            }
        }
    }
}
