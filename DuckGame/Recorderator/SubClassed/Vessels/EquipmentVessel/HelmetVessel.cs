namespace DuckGame
{
    public class HelmetVessel : EquipmentVessel
    {
        public HelmetVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Helmet));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            HelmetVessel v = new HelmetVessel(new Helmet(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
    }
}
