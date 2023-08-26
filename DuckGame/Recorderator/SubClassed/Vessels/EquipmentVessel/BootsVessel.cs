namespace DuckGame
{
    public class BootsVessel : EquipmentVessel
    {
        public BootsVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Boots));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            BootsVessel v = new BootsVessel(new Boots(0, -2000));
            return v;
        }
    }
}
