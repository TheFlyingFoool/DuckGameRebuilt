namespace DuckGame
{
    public class GrappleVessel : EquipmentVessel
    {
        public GrappleVessel(Thing th) : base(th)
        {
            AddSynncl("grappleData", new SomethingSync(typeof(BitBuffer)));
            tatchedTo.Add(typeof(Grapple));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GrappleVessel v = new GrappleVessel(new Grapple(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Grapple g = (Grapple)t;
            g.ropeData = (BitBuffer)valOf("grappleData");
            g.ropeData.position = 0;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Grapple g = (Grapple)t;
            BitBuffer bf = new BitBuffer(g.ropeData.buffer);
            addVal("grappleData", bf);
            base.RecordUpdate();
        }
    }
}
