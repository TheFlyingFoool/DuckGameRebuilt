using System.Collections;

namespace DuckGame
{
    public class TampingWeaponVessel : GunVessel
    {
        public TampingWeaponVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Blunderbuss));
            tatchedTo.Add(typeof(Musket));
            tatchedTo.Add(typeof(Bazooka));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Gun g = null;
            byte bXD = b.ReadByte();
            if (bXD == 0) g = new Blunderbuss(0, -2000);
            else if (bXD == 1) g = new Musket(0, -2000);
            else if (bXD == 2) g = new Bazooka(0, -2000);
            TampingWeaponVessel v = new TampingWeaponVessel(g);
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            if (t is Blunderbuss) prevBuffer.Write((byte)0);
            else if (t is Musket) prevBuffer.Write((byte)1);
            else if (t is Bazooka) prevBuffer.Write((byte)2);
            return prevBuffer;
        }
        public override void DoUpdateThing()
        {
            Gun g = (Gun)t;
            if (g.kick > 0) g.kick -= 0.2f;
            else g.kick = 0;
        }
    }
}