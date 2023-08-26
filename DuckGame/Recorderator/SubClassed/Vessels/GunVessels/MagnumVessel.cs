namespace DuckGame
{
    public class MagnumVessel : GunVessel
    {
        public MagnumVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Magnum));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            MagnumVessel v = new MagnumVessel(new Magnum(0, -2000));
            return v;
        }
        public override void DoUpdateThing()
        {
        }
    }
}
