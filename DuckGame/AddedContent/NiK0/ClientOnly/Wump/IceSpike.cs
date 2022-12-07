using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace DuckGame
{
    [ClientOnly]
    public class IceSpike : PhysicsObject
    {
        public StateBinding _alphaBinding = new StateBinding("alpha");
        public SpriteMap sprite;
        public Duck ignore;
        public IceSpike(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("blunderbussspike", 7, 6);
            Content.textures[sprite.Namebase] = sprite.texture;
            sprite.frame = Rando.Int(3);
            graphic = sprite;
            scale = new Vec2(1.5f);

            center = new Vec2(3.5f, 3f);
            collisionSize = new Vec2(7);
            _collisionOffset = new Vec2(-3.5f);
            weight = 1;
            bouncy = 0.5f;
        }
        public int existance;
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && connection != DuckNetwork.localConnection)
            {
                return;
            }
            if (with is IAmADuck && alpha > 0.6f && (existance < 16 || with != ignore))
            {
                SuperFondle(with, DuckNetwork.localConnection);
                with.velocity = velocity;
                with.Destroy(new DTImpale(this));
                SFX.Play("glassBreak");
                Level.Remove(this);
            }
            else if (with is Block && CalculateImpactPower(with, from) > 1)
            {
                Level.Remove(this);
                SFX.Play("glassBreak");
                for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 3; i++)
                {
                    Level.Add(new GlassParticle(x, y, velocity.normalized));
                }
            }
            base.OnSoftImpact(with, from);
        }
        public override void Update()
        {
            existance++;
            if (isServerForObject)
            {
                if (grounded)
                {
                    angleDegrees = -90;
                    alpha -= 0.01f;
                    if (alpha <= 0)
                    {
                        Level.Remove(this);
                    }
                }
                else
                {
                    angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
                }
            }
            base.Update();
        }
    }
}
