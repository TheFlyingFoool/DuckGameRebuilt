namespace DuckGame
{
    public class DoorOffHingesVessel : SomethingSomethingVessel
    {
        public DoorOffHingesVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(DoorOffHinges));
            AddSynncl("position", new SomethingSync(typeof(int)));
            AddSynncl("visible", new SomethingSync(typeof(bool)));
            AddSynncl("spin", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            bool zx = b.ReadBool();
            DoorOffHingesVessel v = new DoorOffHingesVessel(new DoorOffHinges(0, -2000, zx));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(((DoorOffHinges)t)._secondaryFrame);
            return base.RecSerialize(prevBuffer);
        }
        public bool init;
        public override void PlaybackUpdate()
        {
            DoorOffHinges dfh = (DoorOffHinges)t;
            dfh.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            if (!dfh.visible && (bool)valOf("visible"))
            {
                dfh.MakeEffects();
            }
            dfh.visible = (bool)valOf("visible");
            dfh._throwSpin = (int)valOf("spin");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            DoorOffHinges dfh = (DoorOffHinges)t;
            addVal("visible", dfh.visible);
            if (dfh.visible) addVal("position", CompressedVec2Binding.GetCompressedVec2(dfh.position, 10000));
            else if (exFrames == 0) addVal("position", CompressedVec2Binding.GetCompressedVec2(new Vec2(0, -2000), 10000));
            addVal("spin", (int)dfh._throwSpin);
        }
    }
}
