namespace DuckGame
{
    public class DeadlyIcicle : Holdable, IPlatform
    {
        private SpriteMap _sprite;

        public DeadlyIcicle(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("icicleBig", 10, 18);
            graphic = _sprite;
            center = new Vec2(5f, 5f);
            collisionOffset = new Vec2(-3f, -4f);
            collisionSize = new Vec2(6f, 12f);
            depth = -0.5f;
            thickness = 4f;
            weight = 5f;
            flammable = 0f;
            collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Plastic;
        }

        public override void Update()
        {
            base.Update();
            heat = -1f;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = -bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f);
                glassParticle.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            return base.Hit(bullet, hitPos);
        }
    }
}
