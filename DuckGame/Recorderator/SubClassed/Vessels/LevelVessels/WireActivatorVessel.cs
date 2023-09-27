namespace DuckGame
{
    public class WireActivatorVessel : SomethingSomethingVessel
    {
        public WireActivatorVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(WireActivator));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);

            WireActivator wm = new WireActivator(v.x, v.y);
            return new WireActivatorVessel(wm);
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            //WireButton w = (WireButton)t;
            prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(t.position, 10000));

            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            WireActivator w = (WireActivator)t;

            w.action = bArray[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            WireActivator w = (WireActivator)t;

            bArray[7] = w.action;
            base.RecordUpdate();
        }
    }
}