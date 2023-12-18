namespace DuckGame
{
    public class ShotgunVessel : GunVessel
    {
        public ShotgunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Shotgun));
            tatchedTo.Add(typeof(VirtualShotgun));
            AddSynncl("loadprogress", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Shotgun sh;
            if (b.ReadBool()) sh = new VirtualShotgun(0, -2000);
            else sh = new Shotgun(0, -2000);

            ShotgunVessel v = new ShotgunVessel(sh);
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(t is VirtualShotgun);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Shotgun sg = (Shotgun)t;
            sg._loadAnimation = BitCrusher.UShortToFloat((ushort)valOf("loadprogress"), 2) - 1;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Shotgun sg = (Shotgun)t;
            addVal("loadprogress", BitCrusher.FloatToUShort(sg._loadAnimation + 1, 2));
            base.RecordUpdate();
        }
    }
}
