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
            tatchedTo.Add(typeof(LaserBulletPurple));
            tatchedTo.Add(typeof(WumpMagnumbullet));
            tatchedTo.Add(typeof(WumpMissile));
            Bullet b = (Bullet)th;
            if (b != null)
            {
                position = b.position;
                zeAmmo = b.ammo;
                ang = b.angle;
                owned = b.owner;
                bulletRange = b.range;
                bulletPenetration = b.ammo.penetration;
                bulletSpeed = b.ammo.bulletSpeed;
                bulletThickness = b.ammo.bulletThickness;
                if (doDestroy) b.active = false;
            }
        }
        public AmmoType zeAmmo;
        public float ang;
        public Thing owned;
        public float bulletRange;
        public float bulletPenetration;
        public float bulletSpeed;
        public float bulletThickness;

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
            byte ammotypeIndex = b.ReadByte();
            AmmoType at;
            if (ammotypeIndex > 240) //AHHHHHHHHH -nIk0
            {
                switch (ammotypeIndex)
                {
                    case 255:
                        at = new ATWumpMagnum();
                        break;
                    case 254:
                        at = new ATWumpMissile();
                        break;
                    case 253:
                        at = new ATDeathCaliber();
                        break;
                    default:
                        throw new Exception("This replay has some broken bullet type, please report it to a dev <3");
                }

            }
            else at = (AmmoType)Activator.CreateInstance(AmmoType.indexTypeMap[ammotypeIndex]);
            float angle = b.ReadFloat();
            bulletRange = b.ReadFloat();
            bulletPenetration = b.ReadFloat();
            bulletSpeed = b.ReadFloat();
            bulletThickness = b.ReadFloat();
            at.range = bulletRange;
            at.penetration = bulletPenetration;
            at.bulletSpeed = bulletSpeed;
            at.bulletThickness = bulletThickness;
            at.accuracy = 1;
            int z = b.ReadUShort() - 1;
            
            BulletVessel vb;
            if (at.bulletType != typeof(Bullet))
            {
                Bullet bullet = at.GetBullet(v.x, v.y, null, -angle);
                bullet.rebound = false;
                if (at.bulletType == typeof(GrenadeBullet) || at.bulletType == typeof(LaserBullet)) bullet.rebound = true;
                vb = new BulletVessel(bullet);
                vb.wowownerd = z;
            }
            else
            {
                vb = new BulletVessel(new Bullet(v.x, v.y, at, angle, null));
                vb.wowownerd = z;
                if (at is ATPlasmaBlaster) ((Bullet)vb.t).color = Color.Orange;
            }
            vb.playBack = true;
            return vb;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(position);

            if (zeAmmo.forcedIndex == 0) prevBuffer.Write(AmmoType.indexTypeMap[zeAmmo.GetType()]);
            else prevBuffer.Write(zeAmmo.forcedIndex);
            prevBuffer.Write(ang);
            prevBuffer.Write(bulletRange);
            prevBuffer.Write(bulletPenetration);
            prevBuffer.Write(bulletSpeed);
            prevBuffer.Write(bulletThickness);
            //owner cant even exist
            if (owned != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(owned)) prevBuffer.Write((ushort)(Corderator.instance.somethingMap[owned] + 1));
            else prevBuffer.Write((ushort)0);
            return prevBuffer;
        }
    }
}
