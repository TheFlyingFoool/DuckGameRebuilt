namespace DuckGame
{
    public class RomanCandleVessel : GunVessel
    {
        public RomanCandleVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(RomanCandle));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            RomanCandleVessel v = new RomanCandleVessel(new RomanCandle(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            RomanCandle rc = (RomanCandle)t;
            rc.lit = bArray[7];
            base.PlaybackUpdate();
        }
        public override void ApplyFire()
        {
            Gun g = (Gun)t;
            g.kick = 1;
        }
        public override void RecordUpdate()
        {
            RomanCandle rc = (RomanCandle)t;
            bArray[7] = rc.lit;
            base.RecordUpdate();
        }
    }
}
