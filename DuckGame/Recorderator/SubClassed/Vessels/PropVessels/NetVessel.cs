namespace DuckGame
{
    public class NetVessel : SomethingSomethingVessel
    {
        public NetVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Net));
            AddSynncl("position", new SomethingSync(typeof(int)));
            AddSynncl("velocity", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            NetVessel v = new NetVessel(new Net(0, -2000, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Net n = (Net)t;
            n.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            n.velocity = CompressedVec2Binding.GetUncompressedVec2((int)valOf("velocity"), 20);
            n.solid = false;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Net n = (Net)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(n.position, 10000));
            addVal("velocity", CompressedVec2Binding.GetCompressedVec2(n.velocity, 20));
        }
    }
}
