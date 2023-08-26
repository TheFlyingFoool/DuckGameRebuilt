namespace DuckGame
{
    public class QuadLaserBulletVessel : SomethingSomethingVessel
    {
        public QuadLaserBulletVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(QuadLaserBullet));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            QuadLaserBulletVessel v = new QuadLaserBulletVessel(new QuadLaserBullet(0, -2000, Vec2.Zero));
            return v;
        }
        public override void PlaybackUpdate()
        {
            QuadLaserBullet q = (QuadLaserBullet)t;
            q.position = (Vec2)valOf("position");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            QuadLaserBullet q = (QuadLaserBullet)t;
            addVal("position", q.position);
        }
    }
}
