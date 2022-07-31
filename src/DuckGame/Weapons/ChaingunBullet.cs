// Decompiled with JetBrains decompiler
// Type: DuckGame.ChaingunBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.graphic = new Sprite("chainBullet");
            this.center = new Vec2(4f, 3f);
            this.depth = (Depth)0.8f;
        }

        public ChaingunBullet(float xpos, float ypos, bool dart)
          : base(xpos, ypos)
        {
            if (dart)
            {
                this.graphic = new SpriteMap(nameof(dart), 16, 16);
                this.center = new Vec2(7f, 7f);
            }
            else
            {
                this.graphic = new Sprite("chainBullet");
                this.center = new Vec2(4f, 3f);
            }
            this.depth = (Depth)0.8f;
        }

        public override void Update()
        {
            this.wave += 0.1f + this.waveSpeed;
            if (this.childThing == null)
                return;
            this.childThing.Update();
        }

        public override void Draw()
        {
            if (this.parentThing != null)
            {
                this.position = this.parentThing.position + this.chainOffset + new Vec2(0f, 2f);
                this.graphic.flipH = this.parentThing.graphic.flipH;
                this.desiredSway = 0f;
                this.desiredSway = !(this.parentThing is Gun parentThing1) || parentThing1.owner == null ? -this.parentThing.hSpeed : -parentThing1.owner.hSpeed;
                this.shake += Math.Abs(this.lastDesiredSway - this.desiredSway) * 0.3f;
                if (shake > 0.0)
                    this.shake -= 0.01f;
                else
                    this.shake = 0f;
                if (shake > 1.5)
                {
                    this.shake = 1.5f;
                    this.waveSpeed += 0.02f;
                }
                if (waveSpeed > 0.1f)
                    this.waveSpeed = 0.1f;
                if (waveSpeed > 0.0)
                    this.waveSpeed -= 0.01f;
                else
                    this.waveSpeed = 0f;
                this.lastDesiredSway = this.desiredSway;
                if (this.parentThing is ChaingunBullet parentThing2)
                    this.desiredSway += parentThing2.sway * 0.7f;
                this.desiredSway += (float)Math.Sin(wave + (double)this.waveAdd) * this.shake;
                this.sway = MathHelper.Lerp(this.sway, this.desiredSway, 1f);
                this.position.x += this.sway;
            }
            base.Draw();
            if (this.childThing == null)
                return;
            this.childThing.depth = this.depth - 1;
            this.childThing.Draw();
        }
    }
}
