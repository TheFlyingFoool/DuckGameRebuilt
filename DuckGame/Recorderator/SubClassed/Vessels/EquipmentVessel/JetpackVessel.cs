using System.Collections;

namespace DuckGame
{
    public class JetpackVessel : EquipmentVessel
    {
        public JetpackVessel(Thing th) : base(th)
        {
            AddSynncl("fuel", new SomethingSync(typeof(byte)));
            RemoveSynncl("infoed_h");
            AddSynncl("infoed_j", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(Jetpack));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            JetpackVessel v = new JetpackVessel(new Jetpack(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Jetpack j = (Jetpack)t;
            //float pH = j._heat; if jetpack particles dont work then make a system with this to do odfo fosdsgoasigd hasgu h<asiu ogs -mniko
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_j") });
            j.offDir = (sbyte)(br[0] ? 1 : -1);
            j.visible = br[1];
            j.grounded = br[2];
            j.solid = br[3];
            j.sleeping = br[4];
            j._on = br[5];
            j._heat = BitCrusher.ByteToFloat((byte)valOf("fuel"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Jetpack j = (Jetpack)t;
            BitArray br = new BitArray(8);
            br[0] = j.offDir > 0;
            br[1] = j.visible;
            br[2] = j.grounded;
            br[3] = j.solid;
            br[4] = j.sleeping;
            br[5] = j._on;
            addVal("infoed_j", BitCrusher.BitArrayToByte(br));
            addVal("fuel", BitCrusher.FloatToByte(j._heat, 1));
            base.RecordUpdate();
        }
    }
}
