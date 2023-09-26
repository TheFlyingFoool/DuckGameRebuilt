namespace DuckGame
{
    public class SaxaphoneVessel : GunVessel
    {
        public SaxaphoneVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Saxaphone));
            AddSynncl("pitch", new SomethingSync(typeof(byte)));
            AddSynncl("hpitch", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SaxaphoneVessel v = new SaxaphoneVessel(new Saxaphone(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Saxaphone tb = (Saxaphone)t;
            tb.notePitch = BitCrusher.ByteToFloat((byte)valOf("pitch"));
            tb.handPitch = BitCrusher.ByteToFloat((byte)valOf("hpitch"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Saxaphone tb = (Saxaphone)t;
            addVal("pitch", BitCrusher.FloatToByte(tb.notePitch));
            addVal("hpitch", BitCrusher.FloatToByte(tb.handPitch));
            base.RecordUpdate();
        }
    }
}
