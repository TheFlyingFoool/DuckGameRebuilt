namespace DuckGame
{
    public class NetVessel : SomethingSomethingVessel
    {
        public NetVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Net));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            AddSynncl("velocity", new SomethingSync(typeof(Vec2)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            NetVessel v = new NetVessel(new Net(0, -2000, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Net n = (Net)t;
            n.position = (Vec2)valOf("position");
            n.velocity = (Vec2)valOf("velocity");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Net n = (Net)t;
            addVal("position", n.position);
            addVal("velocity", n.velocity);
        }
    }
}
