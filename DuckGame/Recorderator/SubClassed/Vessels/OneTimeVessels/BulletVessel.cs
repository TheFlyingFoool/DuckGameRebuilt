using System;

namespace DuckGame
{
    //time to try to sync bullets
    //this is gonna be fun
    public class BulletVessel : SomethingSomethingVessel
    {
        public BulletVessel(Thing th) : base(th)
        {
            doIndex = false;
            tatchedTo.Add(typeof(Bullet));
            tatchedTo.Add(typeof(MagBullet));
            tatchedTo.Add(typeof(GrenadeBullet));
            tatchedTo.Add(typeof(LaserBullet));
            tatchedTo.Add(typeof(PelletBullet));
            tatchedTo.Add(typeof(LaserBulletOrange));
            Bullet b = (Bullet)th;
            if (b != null)
            {
                position = b.position;
                zeAmmo = b.ammo;
                ang = b.angle;
                owned = b.owner;
                rangfe = b.range;
                penetration = b.ammo.penetration;
                speedo = b.ammo.bulletSpeed;
                thickness = b.ammo.bulletThickness;
                ACCURACY = b.ammo.accuracy;
                if (doDestroy) b.active = false;
            }
        }
        public AmmoType zeAmmo;
        public float ang;
        public Thing owned;
        public float rangfe;
        public float penetration;
        public float speedo;
        public float thickness;
        public float ACCURACY;

        private int wowownerd;
        public override void OnAdd()
        {
            if (Corderator.instance.somethingMap.ContainsKey(wowownerd)) ((Bullet)t).owner = Corderator.instance.somethingMap[wowownerd];
        }
        public override void PlaybackUpdate()
        {
            if (alpha > 0)
            {
                //DevConsole.Log("boolet " + addTime);
                alpha = 0;
            }
            y = -30000;
            if (t.removeFromLevel) Level.Remove(this);
            base.PlaybackUpdate();
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = b.ReadVec2();
            AmmoType at = (AmmoType)Activator.CreateInstance(AmmoType.indexTypeMap[b.ReadByte()]);
            float f = b.ReadFloat();
            rangfe = b.ReadFloat();
            penetration = b.ReadFloat();
            speedo = b.ReadFloat();
            thickness = b.ReadFloat();
            ACCURACY = b.ReadFloat();
            at.range = rangfe;
            at.penetration = penetration;
            at.bulletSpeed = speedo;
            at.bulletThickness = thickness;
            at.accuracy = ACCURACY;
            int z = b.ReadUShort() - 1;
            
            BulletVessel vb;
            if (at.bulletType != typeof(Bullet))
            {
                Bullet bullet = at.GetBullet(v.x, v.y, null, -f);
                bullet.rebound = false;
                if (at.bulletType == typeof(GrenadeBullet) || at.bulletType == typeof(LaserBullet)) bullet.rebound = true;
                vb = new BulletVessel(bullet);
                vb.wowownerd = z;
            }
            else
            {
                vb = new BulletVessel(new Bullet(v.x, v.y, at, f, null));
                vb.wowownerd = z;
                if (at is ATPlasmaBlaster) ((Bullet)vb.t).color = Color.Orange;
            }
            vb.playBack = true;
            return vb;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(position);
            prevBuffer.Write(AmmoType.indexTypeMap[zeAmmo.GetType()]);
            prevBuffer.Write(ang);
            prevBuffer.Write(rangfe);
            prevBuffer.Write(penetration);
            prevBuffer.Write(speedo);
            prevBuffer.Write(thickness);
            prevBuffer.Write(ACCURACY);
            //owner cant even exist
            if (owned != null && Corderator.instance.somethingMap.Contains(owned)) prevBuffer.Write((ushort)(Corderator.instance.somethingMap[owned] + 1));
            else prevBuffer.Write((ushort)0);
            return prevBuffer;
        }
    }
}
