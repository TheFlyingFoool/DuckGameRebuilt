namespace DuckGame
{
    public class WumpPistolVessel : GunVessel
    {
        public WumpPistolVessel(Thing th) : base(th)
        {
            IndexPriority = 1;
            tatchedTo.Add(typeof(WumpPistol));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            WumpPistolVessel v = new WumpPistolVessel(new WumpPistol(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        
    }
}
