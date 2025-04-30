using System;

namespace DuckGame
{
    public class ChaingunBullet : Thing
    {
        public Thing parentThing;
        public Thing childThing;
        public Vec2 chainOffset;
        public float sway;
        public float desiredSway;
        public float lastDesiredSway;
        public float wave;
        public float shake;
        public float waveSpeed;
        public float waveAdd = 0.07f;

        public ChaingunBullet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("chainBullet");
            center = new Vec2(4f, 3f);
            depth = (Depth)0.8f;
        }

        public ChaingunBullet(float xpos, float ypos, bool dart)
          : base(xpos, ypos)
        {
            if (dart)
            {
                graphic = new SpriteMap(nameof(dart), 16, 16);
                center = new Vec2(7f, 7f);
            }
            else
            {
                graphic = new Sprite("chainBullet");
                center = new Vec2(4f, 3f);
            }
            depth = (Depth)0.8f;
        }

        public override void Update()
        {
            wave += 0.1f + waveSpeed;
            if (childThing == null)
                return;
            childThing.Update();
        }

        public override void Draw()
        {
            if (parentThing != null && MonoMain.UpdateLerpState)
            {
                position = parentThing.position + chainOffset + new Vec2(0f, 2f);
                graphic.flipH = parentThing.graphic.flipH;
                desiredSway = 0f;
                desiredSway = !(parentThing is Gun parentThing1) || parentThing1.owner == null ? -parentThing.hSpeed : -parentThing1.owner.hSpeed;
                shake += Math.Abs(lastDesiredSway - desiredSway) * 0.3f;
                if (shake > 0f)
                    shake -= 0.01f;
                else
                    shake = 0f;
                if (shake > 1.5f)
                {
                    shake = 1.5f;
                    waveSpeed += 0.02f;
                }
                if (waveSpeed > 0.1f)
                    waveSpeed = 0.1f;
                if (waveSpeed > 0f)
                    waveSpeed -= 0.01f;
                else
                    waveSpeed = 0f;
                lastDesiredSway = desiredSway;
                if (parentThing is ChaingunBullet parentThing2)
                    desiredSway += parentThing2.sway * 0.7f;
                desiredSway += (float)Math.Sin(wave + waveAdd) * shake;
                sway = MathHelper.Lerp(sway, desiredSway, 1f);
                position.x += sway;
            }
            base.Draw();
            if (childThing == null)
                return;
            childThing.depth = depth - 1;
            childThing.Draw();
        }
    }
}
