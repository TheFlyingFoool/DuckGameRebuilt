namespace DuckGame
{
    public class QuadLaserBulletVessel : SomethingSomethingVessel
    {
        public QuadLaserBulletVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(QuadLaserBullet));
            AddSynncl("position", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            QuadLaserBulletVessel v = new QuadLaserBulletVessel(new QuadLaserBullet(0, -2000, Vec2.Zero));
            return v;
        }
        public override void PlaybackUpdate()
        {
            QuadLaserBullet q = (QuadLaserBullet)t;
            q.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            QuadLaserBullet q = (QuadLaserBullet)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(q.position, 10000));
        }
    }
}
