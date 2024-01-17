using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace DuckGame
{
    public class MaterialLavaWobble : FullscreenMaterial
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
        public float mult;
        public override void Apply()
        {
            if (MonoMain.UpdateLerpState) time += 0.07f;

            topLeft = theOne.topLeft - new Vec2(Maths.Clamp(theOne.collisionSize.x / 2f, 0, 16), Maths.Clamp(theOne.collisionSize.y, 16, 48));
            bottomRight = theOne.bottomRight  + new Vec2(Maths.Clamp(theOne.collisionSize.x / 2f, 0, 16), 0);

            //24 0

            //0-1 left 0 right 1
            //0-1 bottom 0 top 1
            //transform from game to screen to uv
            SetValue("time", time);
            SetValue("mult", mult * DGRSettings.HeatWaveMultiplier);
            SetValue("gL", topLeft.x);
            SetValue("gR", bottomRight.x);
            SetValue("gT", topLeft.y);
            SetValue("gB", bottomRight.y);


            int width = (int)Resolution._device.PreferredBackBufferWidth;
            int height = (int)Resolution._device.PreferredBackBufferHeight;

            if (Math.Abs(((float)width / (float)height) - Level.current.camera.aspect) > 0.01f)
            {
                height = (int)(width / Level.current.camera.aspect) + 1;
            }

            Vec3 vec3 = Vec3.Transform(new Vec3(topLeft.x, topLeft.y, 0f), Matrix.Invert(Matrix.CreateScale(1, 1, 1)) * Level.current.camera.getMatrix());
            SetValue("uvL", vec3.x / width);
            SetValue("uvB", vec3.y / height);
            vec3 = Vec3.Transform(new Vec3(bottomRight.x, bottomRight.y, 0f), Matrix.Invert(Matrix.CreateScale(1, 1, 1)) * Level.current.camera.getMatrix());
            SetValue("uvT", vec3.y / height);
            SetValue("uvR", vec3.x / width);

            /*SetValue("uvL", Level.current.camera.transformWorldVector(topLeft).x / Resolution.current.x);
            SetValue("uvR", Level.current.camera.transformWorldVector(bottomRight).x / Resolution.current.x);
            SetValue("uvB", Level.current.camera.transformWorldVector(topLeft).y / Resolution.current.y);
            SetValue("uvT", Level.current.camera.transformWorldVector(bottomRight).y / Resolution.current.y);*/

            //Graphics.device.Textures[0] = thing.graphic.texture;
            //Graphics.device.Textures[1] = thing.graphic.texture;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            base.Apply();
        }
    }
}
