using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class SuperRCCar : RCCar
    {
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public SuperRCCar(float xpos, float ypos) : base(xpos, ypos)
        {
            tapeable = false;
            weight = 5;
            collisionOffset = new Vec2(-16f, 0f);
            collisionSize = new Vec2(32f, 22f);
            scale = new Vec2(2);
            _hitPoints = 20;
        }
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0 || TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                if (bullet.isLocal && owner == null) Fondle(this, DuckNetwork.localConnection);
                if (bullet.isLocal) Destroy(new DTShot(bullet));
                return false;
            }
            _hitPoints -= damageMultiplier;
            damageMultiplier += 2f;
            return false;
        }
        public float damageMultiplier;
        public override void Update()
        {
            if (damageMultiplier > 1f)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            base.Update();
        }
        protected override bool OnDestroy(DestroyType type = null)
        {
            RumbleManager.AddRumbleEvent(position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            if (!isServerForObject) return false;
            new ATRCShrapnel().MakeNetEffect(position, false);
            List<Bullet> varBullets = new List<Bullet>();

            Bullet resp = null;
            for (int index = 0; index < 36; ++index)
            {
                float num = (float)(index * 10 - 5) + Rando.Float(10f);
                ATRCShrapnel type1 = new ATRCShrapnel
                {
                    range = 120 + Rando.Float(34f)
                };
                Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 6), y - (float)(Math.Sin(Maths.DegToRad(num)) * 6), type1, num)
                {
                    firedFrom = this
                };
                varBullets.Add(bullet);
                Level.Add(bullet);
                resp = bullet;
            }
            if (Network.isActive)
            {
                Send.Message(new NMFireGun(null, varBullets, 0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            Level.Remove(this);

            ATMissile.DestroyRadius(position, 96, resp); //gotta have a bullet here so dg doesn't crash because reasons
            if (Level.current.camera is FollowCam camera) camera.Remove(this);
            if (Recorder.currentRecording != null) Recorder.currentRecording.LogBonus();
            return true;
        }
    }
}
