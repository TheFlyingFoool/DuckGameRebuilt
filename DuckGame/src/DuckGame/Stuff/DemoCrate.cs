using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Props", EditorItemType.Normal)]
    public class DemoCrate : Holdable, IPlatform
    {
        public float baseExplosionRange = 50f;
        private float damageMultiplier = 1f;
        private SpriteMap _sprite;

        public DemoCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _canFlip = false;
            _editorName = "Demo Crate";
            editorTooltip = "Makes a whole lotta mess.";
            collideSounds.Add("rockHitGround2");
            _sprite = new SpriteMap("demoCrate", 20, 20);
            graphic = _sprite;
            center = new Vec2(10f, 10f);
            collisionOffset = new Vec2(-10f, -10f);
            collisionSize = new Vec2(20f, 19f);
            depth = -0.5f;
            _editorName = "Demo Crate";
            thickness = 2f;
            weight = 10f;
            buoyancy = 1f;
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0.3f;
            _placementCost += 15;
        }

        [NetworkAction]
        private void BlowUp(Vec2 pPosition, float pFlyX)
        {
            if (DGRSettings.ActualParticleMultiplier > 0)
            {
                Level.Add(new ExplosionPart(pPosition.x, pPosition.y));
                int num1 = 6;
                if (Graphics.effectsLevel < 2)
                    num1 = 3;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * num1; ++index)
                {
                    float deg = index * 60f + Rando.Float(-10f, 10f);
                    float num2 = Rando.Float(12f, 20f);
                    Level.Add(new ExplosionPart(pPosition.x + (float)Math.Cos(Maths.DegToRad(deg)) * num2, pPosition.y - (float)Math.Sin(Maths.DegToRad(deg)) * num2));
                }
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(pPosition.x + Rando.Float(-6f, 6f), pPosition.y + Rando.Float(-6f, 6f));
                    smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                    Level.Add(new CampingSmoke(pPosition.x - 5f + Rando.Float(10f), (float)(pPosition.y + 6 - 3 + Rando.Float(6f) - index * 1))
                    {
                        move = {
            x = (Rando.Float(0.6f) - 0.3f),
            y = (Rando.Float(1f) - 0.5f)
          }
                    });
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(pPosition.x - 8f + Rando.Float(16f), pPosition.y - 8f + Rando.Float(16f));
                    woodDebris.hSpeed = (float)((Rando.Float(1f) > 0.5f ? 1 : -1) * Rando.Float(3f) + Math.Sign(pFlyX) * 0.5);
                    woodDebris.vSpeed = -Rando.Float(1f);
                    Level.Add(woodDebris);
                }
            }
            foreach (Window ignore in Level.CheckCircleAll<Window>(pPosition, 40f))
            {
                if (Level.CheckLine<Block>(pPosition, ignore.position, ignore) == null)
                    ignore.Destroy(new DTImpact(this));
            }
            SFX.Play("explode", pitch: Rando.Float(0.1f, 0.3f));
            RumbleManager.AddRumbleEvent(pPosition, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!isServerForObject)
                return false;
            if (removeFromLevel)
                return true;
            _hitPoints = 0f;
            Level.Remove(this);
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            SyncNetworkAction(new Action<Vec2, float>(BlowUp), position, vec2.x);
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 20; ++index)
            {
                float num = (float)(index * 18 - 5) + Rando.Float(10f);
                ATShrapnel type1 = new ATShrapnel
                {
                    range = baseExplosionRange - 20f + Rando.Float(18f)
                };
                Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 6), y - (float)(Math.Sin(Maths.DegToRad(num)) * 6), type1, num)
                {
                    firedFrom = this
                };
                varBullets.Add(bullet);
                Level.Add(bullet);
            }
            DoBlockDestruction();
            if (Network.isActive)
                Send.Message(new NMExplodingProp(varBullets), NetMessagePriority.ReliableOrdered);
            return true;
        }

        public virtual void DoBlockDestruction() => ATMissile.DestroyRadius(position, baseExplosionRange, this);

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            if (_hitPoints <= 0f)
                return base.Hit(bullet, hitPos);
            Destroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            base.Update();
            if (damageMultiplier > 1f)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            if (_hitPoints <= 0f && !_destroyed)
                Destroy(new DTImpact(this));
            if (!_onFire || burnt >= 0.9f)
                return;
            float num = 1f - burnt;
            if (_hitPoints > num * _maxHealth)
                _hitPoints = num * _maxHealth;
            _sprite.color = new Color(num, num, num);
        }
    }
}
