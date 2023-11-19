using System.Collections;

namespace DuckGame
{
    public class IceSpikeVessel : SomethingSomethingVessel
    {
        public IceSpikeVessel(Thing th) : base(th)
        {
            IndexPriority = 1;
            AddSynncl("position", new SomethingSync(typeof(int)));
            AddSynncl("velocity", new SomethingSync(typeof(int)));
            AddSynncl("alpha", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(IceSpike));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            IceSpikeVessel v = new IceSpikeVessel(new IceSpike(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            IceSpike ice = (IceSpike)t;
            ice.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            ice.velocity = CompressedVec2Binding.GetUncompressedVec2((int)valOf("velocity"), 12);
            ice.alpha = BitCrusher.ByteToFloat((byte)valOf("alpha"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            IceSpike ice = (IceSpike)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(ice.position, 10000));
            addVal("velocity", CompressedVec2Binding.GetCompressedVec2(ice.velocity, 12));
            addVal("alpha", BitCrusher.FloatToByte(ice.alpha));
            base.RecordUpdate();
        }
    }
}
