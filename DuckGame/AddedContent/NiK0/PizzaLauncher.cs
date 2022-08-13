using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt")]
    public class PizzaLauncher : Gun
    {
        public const string OvenSprite = "iVBORw0KGgoAAAANSUhEUgAAABEAAAAUCAYAAABroNZJAAAAAXNSR0IArs4c6QAAAQFJREFUOE9jZICA/1CaHIqREWRAQnkNw4LOFrABIDYIwPgwU2HiMDl9Dz8GQ309sDoUQ2ASMIXSakZwl7kGesHZyBbCDSHHD8h6wC75/5+0IJk3bx5DUlISA4hOTk6GeAdkCCM4eEgDsLBEMQRkGLIt+IwEuQAUhhd3bKLMJRiGEOsCkOsO37rPYKumCKbhsQPzRl3nVIant84RDBhQ1DeVZ2M3BORP5LQhoiQDN/DNvScMID6IBlk0d+5cwobgMgBkKlGG4DMAJAeKEbwuAYU4DCB7ASRGlHcoCliY0whGC1TB+YuXwDkYRMMTG7IXiDUIpA4WU9TLgFTxDilewKYWAOOe+wL1uWk5AAAAAElFTkSuQmCC";
        public const string PizzaSprite = "iVBORw0KGgoAAAANSUhEUgAAAB0AAAARCAMAAAAWj1dGAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAhUExURQAAAIQAAP/HTbLc7////81rHaPOJ2B3fCkZOy9ITgAAAM/3o3IAAAALdFJOU/////////////8ASk8B8gAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAHBJREFUKFOdzlkOgDAIBNARusn9DyxLbRSjH07TNOEFCuQ9wIdi+1BsoTphJupRIHIFl1JYT1laa6VG5OoI1ntVouYK67QkjV5jBqd/29qqXztnzp0F3ZJU4JPnpIxe9meMp1lC9/+qo/+rrhfvLSIHW4IOVZz4HV4AAAAASUVORK5CYII=";
        public Sprite oven;
        public PizzaLauncher(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(PizzaSprite))), "pizer"));
            oven = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(OvenSprite))), "ovensprite"));
            oven.center = new Vec2(8.5f, 10);
            center = new Vec2(14.5f, 8.5f);
            collisionSize = new Vec2(27, 17);
            _collisionOffset = new Vec2(-13.5f, -8.5f);
            ammo = 3;
            _barrelOffsetTL = new Vec2(34, 5.5f);
            _kickForce = 4;
            _fireWait = 1;
        }
        public override void Fire()
        {

        }
        public override void Draw()
        {
            Vec2 v = Offset(new Vec2(2f, -3));
            oven.angle = angle;
            oven.alpha = alpha;
            oven.scale = new Vec2(xscale * 0.45f);
            Graphics.Draw(oven, v.x, v.y, depth);
            base.Draw();
        }
        public override void Update()
        {

            base.Update();
        }
    }
}