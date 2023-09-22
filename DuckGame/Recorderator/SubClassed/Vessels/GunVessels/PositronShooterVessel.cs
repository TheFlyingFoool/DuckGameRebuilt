using System.Collections;

﻿namespace DuckGame
{
    public class PositronShooterVessel : GunVessel
    {
        public PositronShooterVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(PositronShooter));
            AddSynncl("wind", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PositronShooterVessel v = new PositronShooterVessel(new PositronShooter(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            PositronShooter p = (PositronShooter)t;
            p._wind = (float)valOf("wind");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            PositronShooter p = (PositronShooter)t;
            addVal("wind", p._wind);
            base.RecordUpdate();
        }
    }
}
