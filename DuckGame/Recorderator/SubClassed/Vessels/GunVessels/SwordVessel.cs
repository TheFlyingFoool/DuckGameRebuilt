using System.Collections;

namespace DuckGame
{
    public class SwordVessel : GunVessel
    {
        public SwordVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("angledeg");
            tatchedTo.Add(typeof(Sword));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            AddSynncl("velocity", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SwordVessel v = new SwordVessel(new Sword(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Sword s = (Sword)t;

            BitArray b_array = new BitArray(new byte[] { (byte)valOf("infoed") });
            s.infiniteAmmoVal = b_array[0];
            s._jabStance = b_array[1];
            s._crouchStance = b_array[2];
            s._slamStance = b_array[3];
            s._swinging = b_array[4];
            s.forceAction = b_array[5];
            s.visible = b_array[7];

            s.velocity = CompressedVec2Binding.GetUncompressedVec2((int)valOf("velocity"), 12);
            /*Vec2 hF = CompressedVec2Binding.GetUncompressedVec2((int)valOf("holdoffset"), 1000);

            if (s.crouchStance) s.handOffset = hF + new Vec2(3, -4);
            else s.handOffset = hF + new Vec2(4, -4);
            s._holdOffset = hF;
            s.visible = b_array[7];*/
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Sword s = (Sword)t;
            BitArray br = new BitArray(8);
            br[0] = s.infiniteAmmoVal;
            br[1] = s._jabStance;
            br[2] = s._crouchStance;
            br[3] = s._slamStance;
            br[4] = s._swinging;
            br[5] = s.action;
            br[7] = s.visible;
            addVal("infoed", BitCrusher.BitArrayToByte(br));

            addVal("velocity", CompressedVec2Binding.GetCompressedVec2(s.velocity, 12));
            base.RecordUpdate();
        }
    }
}
