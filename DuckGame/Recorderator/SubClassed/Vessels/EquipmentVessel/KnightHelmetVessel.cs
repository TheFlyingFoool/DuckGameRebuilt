namespace DuckGame
{
    public class KnightHelmetVessel : EquipmentVessel
    {
        public KnightHelmetVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(KnightHelmet));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            KnightHelmetVessel v = new KnightHelmetVessel(new KnightHelmet(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
    }
}
