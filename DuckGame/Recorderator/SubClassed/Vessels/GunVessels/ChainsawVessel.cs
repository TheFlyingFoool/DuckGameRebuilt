using System.Collections;

﻿namespace DuckGame
{
    public class ChainsawVessel : GunVessel
    {
        public ChainsawVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(Chainsaw));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ChainsawVessel v = new ChainsawVessel(new Chainsaw(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Chainsaw b = (Chainsaw)t; 
            byte what = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { what });

            int pull = 0;
            int div = 4;
            for (int i = 1; i < 4; i++)
            {
                if (br[i]) pull += div;
                div /= 2;
            }
            b._pullState = pull - 1;
            //_hold

            b._triggerHeld = br[0];
            b._flooded = br[4];
            b._throttle = br[5];
            b._started = br[6];
            b.infiniteAmmoVal = br[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Chainsaw b = (Chainsaw)t;
            BitArray br = new BitArray(8);
            byte pull = (byte)(b._pullState + 1);

            br[0] = b._triggerHeld;

            br[1] = (pull & 4) > 0;
            br[2] = (pull & 2) > 0;
            br[3] = (pull & 1) > 0;

            br[4] = b._flooded;
            br[5] = b._throttle;
            br[6] = b._started;
            br[7] = b.infiniteAmmoVal;
            addVal("infoed", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
