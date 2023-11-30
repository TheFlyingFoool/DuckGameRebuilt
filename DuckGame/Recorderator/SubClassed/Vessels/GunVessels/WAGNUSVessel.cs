using System.Collections;

﻿namespace DuckGame
{
    public class WAGNUSVessel : GunVessel
    {
        public WAGNUSVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_x");
            tatchedTo.Add(typeof(Warpgun));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            AddSynncl("infoed_w", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            WAGNUSVessel v = new WAGNUSVessel(new Warpgun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Warpgun b = (Warpgun)t; 
            byte what = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { what });
            int val = 0;
            if (br[0]) val += 4;
            if (br[1]) val += 2;
            if (br[2]) val += 1;
            b.shotsSinceGrounded = val;
            val = 0;
            if (br[3]) val += 8;
            if (br[4]) val += 4;
            if (br[5]) val += 2;
            if (br[6]) val += 1;
            b.shotsSinceDuckWasGrounded = val;
            b.infiniteAmmoVal = br[7];

            what = (byte)valOf("infoed_w");
            br = new BitArray(new byte[] { what });
            if (br[0]) ApplyFire();
            b.warped = br[1];
            b.gravMultTime = br[2] ? 1 : 0;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Warpgun b = (Warpgun)t;
            BitArray br = new BitArray(8);
            br[0] = (b.shotsSinceGrounded & 4) > 0;
            br[1] = (b.shotsSinceGrounded & 2) > 0;
            br[2] = (b.shotsSinceGrounded & 1) > 0;
            br[3] = (b.shotsSinceDuckWasGrounded & 8) > 0;
            br[4] = (b.shotsSinceDuckWasGrounded & 4) > 0;
            br[5] = (b.shotsSinceDuckWasGrounded & 2) > 0;
            br[6] = (b.shotsSinceDuckWasGrounded & 1) > 0;
            br[7] = b.infiniteAmmoVal;
            addVal("infoed", BitCrusher.BitArrayToByte(br));

            br = new BitArray(8);
            br[0] = b.recordKick;
            br[1] = b.warped;
            br[2] = b.gravMultTime > 0;
            addVal("infoed_w", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
