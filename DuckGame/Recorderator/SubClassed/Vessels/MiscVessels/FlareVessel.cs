namespace DuckGame
{
    public class FlareVessel : SomethingSomethingVessel
    {
        public FlareVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Flare));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            FlareVessel v = new FlareVessel(new Flare(0, 0, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Flare f = (Flare)t;
            f.position = (Vec2)valOf("position");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Flare f = (Flare)t;
            addVal("position", f.position);
            base.RecordUpdate();
        }
    }
}
