namespace DuckGame
{
    public class PhaserVessel : GunVessel
    {
        public PhaserVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Phaser));
            AddSynncl("charge", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PhaserVessel v = new PhaserVessel(new Phaser(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Phaser p = (Phaser)t;
            p._chargeFade = BitCrusher.ByteToFloat((byte)valOf("charge"), 1, false);
            base.PlaybackUpdate();
        }
        public override void DoUpdateThing()
        {
            Gun g = (Gun)t;
            if (g.kick > 0) g.kick -= 0.2f;
            else g.kick = 0;
        }
        public override void RecordUpdate()
        {
            Phaser p = (Phaser)t;
            addVal("charge", BitCrusher.FloatToByte(p._chargeFade, 1, false));
            base.RecordUpdate();
        }
    }
}
