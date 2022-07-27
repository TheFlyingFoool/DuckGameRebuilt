// Decompiled with JetBrains decompiler
// Type: DuckGame.DemoCrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this._canFlip = false;
            this._editorName = "Demo Crate";
            this.editorTooltip = "Makes a whole lotta mess.";
            this.collideSounds.Add("rockHitGround2");
            this._sprite = new SpriteMap("demoCrate", 20, 20);
            this.graphic = _sprite;
            this.center = new Vec2(10f, 10f);
            this.collisionOffset = new Vec2(-10f, -10f);
            this.collisionSize = new Vec2(20f, 19f);
            this.depth = - 0.5f;
            this._editorName = "Demo Crate";
            this.thickness = 2f;
            this.weight = 10f;
            this.buoyancy = 1f;
            this._holdOffset = new Vec2(2f, 0.0f);
            this.flammable = 0.3f;
            this._placementCost += 15;
        }

        [NetworkAction]
        private void BlowUp(Vec2 pPosition, float pFlyX)
        {
            Level.Add(new ExplosionPart(pPosition.x, pPosition.y));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add(new ExplosionPart(pPosition.x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, pPosition.y - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2));
            }
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(pPosition.x + Rando.Float(-6f, 6f), pPosition.y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            for (int index = 0; index < 3; ++index)
                Level.Add(new CampingSmoke(pPosition.x - 5f + Rando.Float(10f), (float)(pPosition.y + 6.0 - 3.0 + (double)Rando.Float(6f) - index * 1.0))
                {
                    move = {
            x = (Rando.Float(0.6f) - 0.3f),
            y = (Rando.Float(1f) - 0.5f)
          }
                });
            for (int index = 0; index < 6; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(pPosition.x - 8f + Rando.Float(16f), pPosition.y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(3f) + Math.Sign(pFlyX) * 0.5);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
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
            if (!this.isServerForObject)
                return false;
            if (this.removeFromLevel)
                return true;
            this._hitPoints = 0.0f;
            Level.Remove(this);
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            this.SyncNetworkAction<Vec2, float>(new Action<Vec2, float>(this.BlowUp), this.position, vec2.x);
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 20; ++index)
            {
                float num = (float)(index * 18.0 - 5.0) + Rando.Float(10f);
                ATShrapnel type1 = new ATShrapnel
                {
                    range = this.baseExplosionRange - 20f + Rando.Float(18f)
                };
                Bullet bullet = new Bullet(this.x + (float)(Math.Cos((double)Maths.DegToRad(num)) * 6.0), this.y - (float)(Math.Sin((double)Maths.DegToRad(num)) * 6.0), type1, num)
                {
                    firedFrom = this
                };
                varBullets.Add(bullet);
                Level.Add(bullet);
            }
            this.DoBlockDestruction();
            if (Network.isActive)
                Send.Message(new NMExplodingProp(varBullets), NetMessagePriority.ReliableOrdered);
            return true;
        }

        public virtual void DoBlockDestruction() => ATMissile.DestroyRadius(this.position, this.baseExplosionRange, this);

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (_hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            this.Destroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void Update()
        {
            base.Update();
            if (damageMultiplier > 1.0)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            if (_hitPoints <= 0.0 && !this._destroyed)
                this.Destroy(new DTImpact(this));
            if (!this._onFire || burnt >= 0.899999976158142)
                return;
            float num = 1f - this.burnt;
            if (_hitPoints > (double)num * _maxHealth)
                this._hitPoints = num * this._maxHealth;
            this._sprite.color = new Color(num, num, num);
        }
    }
}
