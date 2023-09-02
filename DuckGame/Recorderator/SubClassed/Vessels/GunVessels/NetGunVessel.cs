namespace DuckGame
{
    public class NetGunVessel : GunVessel
    {
        public NetGunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(NetGun));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            NetGunVessel v = new NetGunVessel(new NetGun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void AmmoDecreased()
        {
            NetGun g = (NetGun)t;

            g._barrelSteam.speed = 1f;
            g._barrelSteam.frame = 0;
            base.AmmoDecreased();
        }
    }
}
