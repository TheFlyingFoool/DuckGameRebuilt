namespace DuckGame
{
    public class WumpBazookaVessel : TampingWeaponVessel
    {
        public WumpBazookaVessel(Thing th) : base(th)
        {
            IndexPriority = 1;
            tatchedTo.Clear();//dumb DUMB DUMB BUT IT WORKS -NiK0
            tatchedTo.Add(typeof(WumpBazooka));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            WumpBazookaVessel v = new WumpBazookaVessel(new WumpBazooka(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        
    }
}
