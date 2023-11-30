namespace DuckGame
{
    public class FlamethrowerVessel : GunVessel
    {
        public FlamethrowerVessel(Thing th) : base(th)
        {
            //RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(FlameThrower));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            FlamethrowerVessel v = new FlamethrowerVessel(new FlameThrower(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            FlameThrower f = (FlameThrower)t;
            f._firing = bArray[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            FlameThrower f = (FlameThrower)t;

            bArray[7] = f._firing;

            base.RecordUpdate();
        }
    }
}
