namespace DuckGame
{
    public class DartGunVessel : GunVessel
    {
        public DartGunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(DartGun));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DartGunVessel v = new DartGunVessel(new DartGun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void ApplyFire()
        {
            Gun g = (Gun)t;
            g.kick = 1;
        }
    }
}
