namespace DuckGame
{
    public class TromboneVessel : GunVessel
    {
        public TromboneVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Trombone));
            AddSynncl("pitch", new SomethingSync(typeof(byte)));
            AddSynncl("hpitch", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            TromboneVessel v = new TromboneVessel(new Trombone(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Trombone tb = (Trombone)t;
            tb.notePitch = BitCrusher.ByteToFloat((byte)valOf("pitch"));
            tb.handPitch = BitCrusher.ByteToFloat((byte)valOf("hpitch"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Trombone tb = (Trombone)t;
            addVal("pitch", BitCrusher.FloatToByte(tb.notePitch));
            addVal("hpitch", BitCrusher.FloatToByte(tb.handPitch));
            base.RecordUpdate();
        }
    }
}
