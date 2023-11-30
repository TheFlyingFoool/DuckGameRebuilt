using System.Collections;

namespace DuckGame
{
    public class TVVessel : HoldableVessel
    {
        public TVVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_h");
            AddSynncl("infoed_t", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(TV));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            TVVessel tv = new TVVessel(new TV(0, -2000));
            return tv;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TV tv = (TV)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_t") });
            tv.offDir = (sbyte)(br[0] ? 1 : -1);
            tv.visible = br[1];
            tv.grounded = br[2];
            tv.solid = br[3];
            tv.sleeping = br[4];
            tv._ruined = br[5];
            if (tv.channel != br[6])
            {
                tv.SwitchChannelEffect();
            }
            tv.channel = br[6];

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            TV tv = (TV)t;
            BitArray br = new BitArray(8);
            br[0] = tv.offDir > 0;
            br[1] = tv.visible;
            br[2] = tv.grounded;
            br[3] = tv.solid;
            br[4] = tv.sleeping;
            br[5] = tv._ruined;
            br[6] = tv.channel;
            addVal("infoed_t", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
