﻿namespace DuckGame
{
    //yes this vessel only exists to deactivate
    //the grenade launcher
    public class GrenadeLauncherVessel : GunVessel
    {
        public GrenadeLauncherVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(GrenadeLauncher));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GrenadeLauncherVessel v = new GrenadeLauncherVessel(new GrenadeLauncher(0, -2000));
            return v;
        }
        public override void DoUpdateThing()
        {
        }
    }
}