namespace DuckGame
{
    public class CampingBallVessel : SomethingSomethingVessel
    {
        public CampingBallVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(CampingBall));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            AddSynncl("velocity", new SomethingSync(typeof(Vec2)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            CampingBallVessel v = new CampingBallVessel(new CampingBall(0, -2000, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            CampingBall c = (CampingBall)t;
            c.position = (Vec2)valOf("position");
            c.velocity = (Vec2)valOf("velocity");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            CampingBall n = (CampingBall)t;
            addVal("position", n.position);
            addVal("velocity", n.velocity);
        }
    }
}
