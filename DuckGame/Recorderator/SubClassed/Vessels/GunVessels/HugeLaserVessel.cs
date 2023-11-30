using System.Collections;

﻿namespace DuckGame
{
    public class HugeLaserVessel : GunVessel
    {
        public HugeLaserVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(HugeLaser));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            HugeLaserVessel v = new HugeLaserVessel(new HugeLaser(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            HugeLaser h = (HugeLaser)t; 
            byte what = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { what });
            int xd = 0;
            int divide = 32;
            for (int i = 0; i < 6; i++)
            {
                if (br[i]) xd += divide;
                divide /= 2;
            }
            h.netAnimationIndex = (byte)xd;
            h.infiniteAmmoVal = br[6];

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            HugeLaser h = (HugeLaser)t;
            BitArray br = new BitArray(8);
            int z = h.netAnimationIndex;
            br[0] = (z & 32) > 0;
            br[1] = (z & 16) > 0;
            br[2] = (z & 8) > 0;
            br[3] = (z & 4) > 0;
            br[4] = (z & 2) > 0;
            br[5] = (z & 1) > 0;
            br[6] = h.infiniteAmmoVal;
            addVal("infoed", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
