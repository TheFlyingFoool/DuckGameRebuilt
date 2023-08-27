namespace DuckGame
{
    public class NetGunVessel : GunVessel
    {
        public NetGunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(NetGunVessel));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            NetGunVessel v = new NetGunVessel(new NetGun(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            NetGun g = (NetGun)t;
            int cAmmo = g.ammo;
            base.PlaybackUpdate();
            if (g.ammo != cAmmo)
            {
                g._barrelSteam.speed = 1f;
                g._barrelSteam.frame = 0;
            }
        }
    }
}
