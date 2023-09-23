using System.Collections;

namespace DuckGame
{
    public class RCCarVessel : HoldableVessel
    {
        public RCCarVessel(Thing th) : base (th)
        {
            tatchedTo.Add(typeof(RCCar));
            AddSynncl("infoed_rc", new SomethingSync(typeof(byte)));
            AddSynncl("velocity", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            return new RCCarVessel(new RCCar(0, 0));
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            RCCar rc = (RCCar)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_rc") });

            rc.receivingSignal = br[0];
            rc._idleSpeed = BitCrusher.DecompressFloat(br, 1, 5, 1);
            DevConsole.Log(rc._idleSpeed);
           

            rc.velocity = CompressedVec2Binding.GetUncompressedVec2((int)valOf("velocity"), 20);

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            RCCar rc = (RCCar)t;
            BitArray br = new BitArray(8);

            br[0] = rc.receivingSignal;
            BitCrusher.CompressFloat(br, 1, 5, rc._idleSpeed, 1);

            addVal("infoed_rc", BitCrusher.BitArrayToByte(br));
            addVal("velocity", CompressedVec2Binding.GetCompressedVec2(rc.velocity, 20));

            base.RecordUpdate();
        }
    }
}
