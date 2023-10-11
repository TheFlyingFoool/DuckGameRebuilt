namespace DuckGame
{
    public class DeathBeamVessel : SomethingSomethingVessel
    {
        public DeathBeamVessel(Thing th) : base(th)
        {
            doIndex = false;
            tatchedTo.Add(typeof(DeathBeam));
            tatchedTo.Add(typeof(EnergyScimitarBlast));
            if (th is DeathBeam d)
            {
                v1 = d.position;
                v2 = Extensions.GetPrivateFieldValue<Vec2>(d, "_target");
            }
            else if (th is EnergyScimitarBlast e)
            {
                isSkimitar = true;
                v1 = e.position;
                v2 = Extensions.GetPrivateFieldValue<Vec2>(e, "_target");
            }
            else
            {
                if (th != null) Level.Add(new NilVessel(th.position, "Deathbeamvessel dind't have neither a deathbeam or energyscimi blast"));
            }
        }
        public Vec2 v1;
        public Vec2 v2;
        public bool isSkimitar;
        public override void Update()
        {
            if (playBack)
            {
                //DevConsole.Log("deathlaser " + addTime);
                y = -3000;
                Level.Remove(this);
            }
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v1 = b.ReadVec2();
            Vec2 v2 = b.ReadVec2();
            bool isSkimi = b.ReadBool();
            Thing Halcyon; // :)
            if (isSkimi) Halcyon = new EnergyScimitarBlast(v1, v2);
            else Halcyon = new DeathBeam(v1, v2);
            DeathBeamVessel db = new DeathBeamVessel(Halcyon);
            db.playBack = true;
            return db;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(v1);
            prevBuffer.Write(v2);
            prevBuffer.Write(isSkimitar);
            return prevBuffer;
        }
    }
}
