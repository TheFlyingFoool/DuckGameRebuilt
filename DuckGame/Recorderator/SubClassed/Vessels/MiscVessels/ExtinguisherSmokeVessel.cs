using System.CodeDom;

namespace DuckGame
{
    public class ExtinguisherSmokeVessel : SomethingSomethingVessel
    {
        public ExtinguisherSmokeVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(ExtinguisherSmoke));
            AddSynncl("X", new SomethingSync(typeof(short)));
            AddSynncl("Y", new SomethingSync(typeof(short)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ExtinguisherSmokeVessel v = new ExtinguisherSmokeVessel(new ExtinguisherSmoke(0, -2000, true));
            return v;
        }
        public override void PlaybackUpdate()
        {
            ExtinguisherSmoke s = (ExtinguisherSmoke)t;
            s.isLocal = false;
            s.netLerpPosition = new Vec2((short)valOf("X"), (short)valOf("Y"));

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            ExtinguisherSmoke s = (ExtinguisherSmoke)t;

            if (skipPositioning == 0 || exFrames == 0)
            {
                addVal("X", (short)s.x);
                addVal("Y", (short)s.y);
            }
            base.RecordUpdate();
        }
    }
}
