using System.Collections;

namespace DuckGame
{
    public class EnergyscimitarVessel : GunVessel
    {
        public EnergyscimitarVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(EnergyScimitar));
            //destruction of bytes
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            EnergyscimitarVessel v = new EnergyscimitarVessel(new EnergyScimitar(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            EnergyScimitar e = (EnergyScimitar)t;
            base.PlaybackUpdate();
        }
        public override void DoUpdateThing()
        {
        }
        public override void RecordUpdate()
        {
            EnergyScimitar e = (EnergyScimitar)t;            
            base.RecordUpdate();
        }
    }
}
