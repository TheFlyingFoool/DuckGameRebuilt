using System;
using System.Collections;

namespace DuckGame
{
    public class DeskVessel : HoldableVessel
    {
        public DeskVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Desk));
            //_flip float compress
            //flipped is a bool
            AddSynncl("infoed_ds", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DeskVessel v = new DeskVessel(new Desk(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Desk d = (Desk)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_ds") });

            bool b1 = br[0];
            bool b2 = br[1];
            if (b1) d.flipped = 1;
            else if (b2) d.flipped = -1;
            else d.flipped = 0;

            int div = 32;
            int val = 0;
            for (int i = 2; i < 8; i++)
            {
                if (br[i]) val += div;
                div /= 2;
            }
            d._flip = ((float)val / 50f) - 0.1f;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Desk d = (Desk)t;
            BitArray br = new BitArray(8);
            br[0] = d.flipped > 0;
            br[1] = d.flipped < 0;
            int z = (int)Math.Round((d._flip + 0.1f) * 50);
            br[2] = (z & 32) > 0;
            br[3] = (z & 16) > 0;
            br[4] = (z & 8) > 0;
            br[5] = (z & 4) > 0;
            br[6] = (z & 2) > 0;
            br[7] = (z & 1) > 0;
            addVal("infoed_ds", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
