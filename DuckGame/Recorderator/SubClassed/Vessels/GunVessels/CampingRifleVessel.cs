using System.Collections;

namespace DuckGame
{
    public class CampingRifleVessel : GunVessel
    {
        public CampingRifleVessel(Thing th) : base(th)
        {
            //RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(CampingRifle));
            AddSynncl("loadProgress", new SomethingSync(typeof(sbyte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            CampingRifleVessel v = new CampingRifleVessel(new CampingRifle(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void ApplyFire()
        {
            CampingRifle c = (CampingRifle)t;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
            {
                CampingSmoke campingSmoke = new CampingSmoke((c.barrelPosition.x - 8f + Rando.Float(8f) + offDir * 8f), c.barrelPosition.y - 8f + Rando.Float(8f))
                {
                    depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                };
                if (index < 3)
                    campingSmoke.move -= c.barrelVector * Rando.Float(0.05f);
                else
                    campingSmoke.fly += c.barrelVector * (1f + Rando.Float(2.8f));
                Level.Add(campingSmoke);
            }
            base.ApplyFire();
        }
        public override void PlaybackUpdate()
        {
            CampingRifle c = (CampingRifle)t;
            c._loadProgress = (sbyte)valOf("loadProgress");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            CampingRifle c = (CampingRifle)t;
            addVal("loadProgress", c._loadProgress);
            base.RecordUpdate();
        }
    }
}
