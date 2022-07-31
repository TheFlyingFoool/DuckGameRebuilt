// Decompiled with JetBrains decompiler
// Type: DuckGame.Desk
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this._sprite = new SpriteMap("desk", 19, 12);
            this.graphic = _sprite;
            this.center = new Vec2(9f, 6f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(17f, 6f);
            this.depth = -0.5f;
            this._editorName = nameof(Desk);
            this.thickness = 8f;
            this.weight = 8f;
            this._holdOffset = new Vec2(2f, 2f);
            this.collideSounds.Add("thud");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorTooltip = "This is where we get all the important work done.";
            this.holsterAngle = 90f;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            this._hitPoints = 0f;
            SFX.Play("crateDestroy");
            Level.Remove(this);
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < 6; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(3f) + Math.Sign(vec2.x) * 0.5);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
            }
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-6f, 6f), this.y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_flip < 0.05f && hitPos.y > this.top + 4f)
                return false;
            if (_hitPoints <= 0f)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < 1f + damageMultiplier; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (Math.Sign(bullet.travel.x) * Rando.Float(2f) + Math.Sign(bullet.travel.x) * 0.5f);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
            }
            SFX.Play("woodHit");
            if (this.isServerForObject && bullet.isLocal)
            {
                this._hitPoints -= this.damageMultiplier;
                this.damageMultiplier += 2f;
                if (_hitPoints <= 0f)
                    this.Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            base.Update();
            this.offDir = 1;
            if (damageMultiplier > 1.0)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            this._sprite.frame = (int)Math.Floor((1.0 - _hitPoints / (double)this._maxHealth) * 4.0);
            if (_hitPoints <= 0.0 && !this._destroyed)
                this.Destroy(new DTImpact(this));
            this._flip = MathHelper.Lerp(this._flip, this.flipped != 0 ? 1.1f : -0.1f, 0.2f);
            if (_flip > 1.0)
                this._flip = 1f;
            if (_flip < 0.0)
                this._flip = 0f;
            if (this.owner != null && this.flipped != 0)
                this.flipped = 0;
            Vec2 collisionSize = this.collisionSize;
            Vec2 collisionOffset = this.collisionOffset;
            if (_flip == 0.0)
            {
                if (!this.landed)
                    this.Land();
                this.collisionOffset = new Vec2(-8f, -6f);
                this.collisionSize = new Vec2(17f, 11f);
            }
            else if (_flip == 1.0)
            {
                if (!this.landed)
                    this.Land();
                if (this.flipped > 0)
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
                this.landed = false;
                this.collisionOffset = new Vec2(-2f, 4f);
                this.collisionSize = new Vec2(4f, 1f);
            }
            if (!this.firstFrame && (collisionOffset != this.collisionOffset || collisionSize != this.collisionSize))
                this.ReturnItemToWorld(this);
            if (this.flipped != 0)
            {
                this.centerx = (float)(9.0 + 4.0 * _flip * (this.flipped > 0 ? 1.0 : -1.0));
                this.centery = (float)(6.0 + 4.0 * _flip);
                this.angle = this._flip * (float)(1.5 * (this.flipped > 0 ? 1.0 : -1.0));
            }
            else
            {
                this.centerx = (float)(9.0 + 4.0 * _flip * ((double)this.angle > 0.0 ? 1.0 : -1.0));
                this.centery = (float)(6.0 + 4.0 * _flip);
                this.angle = this._flip * (float)(1.5 * ((double)this.angle > 0.0 ? 1.0 : -1.0));
            }
            this.firstFrame = false;
        }

        public void Flip(bool left)
        {
            if (this.owner != null || !this.isServerForObject)
                return;
            SFX.Play("swipe", 0.5f);
            if (this.grounded)
            {
                if (this.flipped == 0)
                    this.vSpeed -= 1.4f;
                else
                    --this.vSpeed;
            }
            if (this.flipped == 0)
                this.flipped = left ? -1 : 1;
            else
                this.flipped = 0;
        }

        public void Land()
        {
            this.landed = true;
            if (this.owner == null)
                SFX.Play("rockHitGround2", 0.7f);
            if (this.flipped > 0)
            {
                for (int index = 0; index < 2; ++index)
                    Level.Add(SmallSmoke.New(this.bottomRight.x, this.bottomRight.y));
            }
            else
            {
                if (this.flipped >= 0)
                    return;
                for (int index = 0; index < 2; ++index)
                    Level.Add(SmallSmoke.New(this.bottomLeft.x, this.bottomLeft.y));
            }
        }
    }
}
