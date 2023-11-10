using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class Desk : Holdable, IPlatform
    {
        public StateBinding _flippedBinding = new StateBinding(nameof(flipped));
        public StateBinding _flipBinding = new StateBinding(nameof(_flip));
        private float damageMultiplier = 1f;
        private SpriteMap _sprite;
        public int flipped;
        public bool landed = true;
        public float _flip;
        private bool firstFrame = true;

        public Desk(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _sprite = new SpriteMap("desk", 19, 12);
            graphic = _sprite;
            center = new Vec2(9f, 6f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(17f, 6f);
            depth = -0.5f;
            _editorName = nameof(Desk);
            thickness = 8f;
            weight = 8f;
            _holdOffset = new Vec2(2f, 2f);
            collideSounds.Add("thud");
            physicsMaterial = PhysicsMaterial.Metal;
            editorTooltip = "This is where we get all the important work done.";
            holsterAngle = 90f;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            SFX.Play("crateDestroy");
            Level.Remove(this);
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (float)((Rando.Float(1f) > 0.5f ? 1 : -1) * Rando.Float(3f) + Math.Sign(vec2.x) * 0.5f);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
            }
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-6f, 6f), y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_flip < 0.05f && hitPos.y > top + 4f)
                return false;
            if (_hitPoints <= 0f)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier); ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (Math.Sign(bullet.travel.x) * Rando.Float(2f) + Math.Sign(bullet.travel.x) * 0.5f);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
            }
            SFX.Play("woodHit");
            if (isServerForObject && bullet.isLocal)
            {
                _hitPoints -= damageMultiplier;
                damageMultiplier += 2f;
                if (_hitPoints <= 0f)
                    Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            base.Update();
            offDir = 1;
            if (damageMultiplier > 1)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            _sprite.frame = (int)Math.Floor((1 - _hitPoints / _maxHealth) * 4f);
            if (_hitPoints <= 0f && !_destroyed)
                Destroy(new DTImpact(this));
            _flip = MathHelper.Lerp(_flip, flipped != 0 ? 1.1f : -0.1f, 0.2f);
            if (_flip > 1)
                _flip = 1f;
            if (_flip < 0)
                _flip = 0f;
            if (owner != null && flipped != 0)
                flipped = 0;
            Vec2 collisionSize = this.collisionSize;
            Vec2 collisionOffset = this.collisionOffset;
            if (_flip == 0)
            {
                if (!landed)
                    Land();
                this.collisionOffset = new Vec2(-8f, -6f);
                this.collisionSize = new Vec2(17f, 11f);
            }
            else if (_flip == 1)
            {
                if (!landed)
                    Land();
                if (flipped > 0)
                {
                    this.collisionOffset = new Vec2(0f, -12f);
                    this.collisionSize = new Vec2(8f, 17f);
                }
                else
                {
                    this.collisionOffset = new Vec2(-10f, -13f);
                    this.collisionSize = new Vec2(8f, 17f);
                }
            }
            else
            {
                landed = false;
                this.collisionOffset = new Vec2(-2f, 4f);
                this.collisionSize = new Vec2(4f, 1f);
            }
            if (!firstFrame && (collisionOffset != this.collisionOffset || collisionSize != this.collisionSize))
                ReturnItemToWorld(this);
            if (flipped != 0)
            {
                centerx = (float)(9 + 4 * _flip * (flipped > 0 ? 1 : -1));
                centery = (float)(6 + 4 * _flip);
                angle = _flip * (float)(1.5f * (flipped > 0 ? 1 : -1));
            }
            else
            {
                centerx = (float)(9 + 4 * _flip * (angle > 0 ? 1 : -1));
                centery = (float)(6 + 4 * _flip);
                angle = _flip * (float)(1.5f * (angle > 0 ? 1 : -1));
            }
            firstFrame = false;
        }

        public void Flip(bool left)
        {
            if (owner != null || !isServerForObject)
                return;
            SFX.Play("swipe", 0.5f);
            if (grounded)
            {
                if (flipped == 0)
                    vSpeed -= 1.4f;
                else
                    --vSpeed;
            }
            if (flipped == 0)
                flipped = left ? -1 : 1;
            else
                flipped = 0;
        }

        public void Land()
        {
            landed = true;
            if (owner == null)
                SFX.Play("rockHitGround2", 0.7f);
            if (flipped > 0)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                    Level.Add(SmallSmoke.New(bottomRight.x, bottomRight.y));
            }
            else
            {
                if (flipped >= 0)
                    return;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                    Level.Add(SmallSmoke.New(bottomLeft.x, bottomLeft.y));
            }
        }
    }
}
