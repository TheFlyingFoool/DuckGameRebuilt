using System.Collections;

namespace DuckGame
{
    public class SnubbyPistolVessel : GunVessel
    {
        public SnubbyPistolVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            AddSynncl("infoed_snv", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(SnubbyPistol));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SnubbyPistolVessel v = new SnubbyPistolVessel(new SnubbyPistol(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            SnubbyPistol sn = (SnubbyPistol)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_snv") });
            int val = -1;
            int div = 4;
            for (int i = 0; i < 3; i++)
            {
                if (br[i]) val += div;
                div /= 2;
            }
            if (val == -1) sn.infiniteAmmoVal = true;
            else
            {
                sn.infiniteAmmoVal = false;
                sn.ammo = val;
            }

            if (br[3])
            {
                sn._loadBurst = 1;
                sn.PopShell();
            }
            if (br[4]) ApplyFire();

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            SnubbyPistol sn = (SnubbyPistol)t;
            BitArray br = new BitArray(8);

            int am = sn.ammo + 1;
            if (sn.infiniteAmmoVal) am = 0;
            br[0] = (am & 4) > 0;
            br[1] = (am & 2) > 0;
            br[2] = (am & 1) > 0;

            br[3] = sn._loadBurst > 0.82f;
            br[4] = sn.recordKick;

            addVal("infoed_snv", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
