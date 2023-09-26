using System.Collections;

namespace DuckGame
{
    public class GoodBookVessel : GunVessel
    {
        public GoodBookVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_x");
            AddSynncl("infoed_b", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(GoodBook));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GoodBookVessel v = new GoodBookVessel(new GoodBook(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {

            GoodBook gb = (GoodBook)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_b") });
            int val = 0;
            if (br[0]) val += 16;
            if (br[1]) val += 8;
            if (br[2]) val += 4;
            if (br[3]) val += 2;
            if (br[4]) val += 1;
            gb._ringPulse = val / 20f;
            gb._raiseArm = br[5] ? 1 : 0;
            gb._triggerHeld = br[6];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {//only need 20 for _ringPulse
            GoodBook gb = (GoodBook)t;
            BitArray br = new BitArray(8);
            int val = (int)(gb._ringPulse * 20);
            br[0] = (val & 16) > 0;
            br[1] = (val & 8) > 0;
            br[2] = (val & 4) > 0;
            br[3] = (val & 2) > 0;
            br[4] = (val & 1) > 0;
            br[5] = gb._raiseArm > 0;
            br[6] = gb._triggerHeld;
            addVal("infoed_b", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
