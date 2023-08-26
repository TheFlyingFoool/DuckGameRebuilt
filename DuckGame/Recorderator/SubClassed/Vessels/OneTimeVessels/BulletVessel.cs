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
                if (doDestroy) b.active = false;
            }
        }
        public AmmoType zeAmmo;
        public float ang;
        public Thing owned;
        public float rangfe;
        public float penetration;
        public float speedo;

        private int wowownerd;
        public override void OnAdd()
        {
            if (Corderator.instance.somethingMap.ContainsKey(wowownerd)) ((Bullet)t).owner = Corderator.instance.somethingMap[wowownerd];
        }
        public override void PlaybackUpdate()
        {
            if (alpha > 0)
            {
                DevConsole.Log("boolet " + addTime);
                alpha = 0;
            }
            y = -30000;
            if (t.removeFromLevel) Level.Remove(this);
            base.PlaybackUpdate();
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = b.ReadVec2();
            //Main.SpecialCode = "it crashed here";
            AmmoType at = (AmmoType)Activator.CreateInstance(AmmoType.indexTypeMap[b.ReadByte()]);
            float f = b.ReadFloat();
            rangfe = b.ReadFloat();
            penetration = b.ReadFloat();
            speedo = b.ReadFloat();
            at.range = rangfe;
            at.penetration = penetration;
            at.bulletSpeed = speedo;
            int z = b.ReadUShort() - 1;
            
            //Main.SpecialCode = "actually it crashed here: " + z;
            
            //Main.SpecialCode = "nvm duck game fucking sucks";
            BulletVessel vb;
            if (at.bulletType != typeof(Bullet))
            {
                //i forgor
                Bullet boolet = at.GetBullet(v.x, v.y, null, -f);
                boolet.rebound = false;
                vb = new BulletVessel(boolet);
                vb.wowownerd = z;
            }
            else
            {
                //i rember!
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
            //owner cant even exist
            if (owned != null && Corderator.instance.somethingMap.Contains(owned)) prevBuffer.Write((ushort)(Corderator.instance.somethingMap[owned] + 1));
            else prevBuffer.Write((ushort)0);
            return prevBuffer;
        }
    }
}
