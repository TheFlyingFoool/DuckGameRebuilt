namespace DuckGame
{
    public class SpikeHelmVessel : EquipmentVessel
    {
        public SpikeHelmVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(SpikeHelm));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SpikeHelmVessel v = new SpikeHelmVessel(new SpikeHelm(0, -2000));
            return v;
        }
    }
}
