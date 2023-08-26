namespace DuckGame
{
    public class ChestplateVessel : EquipmentVessel
    {
        public ChestplateVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(ChestPlate));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ChestplateVessel v = new ChestplateVessel(new ChestPlate(0, -2000));
            return v;
        }
    }
}
