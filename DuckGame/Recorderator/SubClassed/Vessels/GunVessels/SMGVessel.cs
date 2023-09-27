namespace DuckGame
{
    public class SMGVessel : GunVessel
    {
        public SMGVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(SMG));
            AddSynncl("acc", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SMGVessel v = new SMGVessel(new SMG(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }//0.8f
        public override void PlaybackUpdate()
        {
            SMG smg = (SMG)t;
            smg._accuracyLost = BitCrusher.ByteToFloat((byte)valOf("acc"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            SMG smg = (SMG)t;
            addVal("acc", BitCrusher.FloatToByte(smg._accuracyLost));
            base.RecordUpdate();
        }
    }
}
