namespace DuckGame
{
    public class PlasmaBlasterVessel : GunVessel
    {
        public PlasmaBlasterVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(PlasmaBlaster));
            AddSynncl("lostaccuracy", new SomethingSync(typeof(float)));
            AddSynncl("flarealph", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PlasmaBlasterVessel v = new PlasmaBlasterVessel(new PlasmaBlaster(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            PlasmaBlaster p = (PlasmaBlaster)t;
            p._accuracyLost = (float)valOf("lostaccuracy");
            Extensions.SetPrivateFieldValue(p, "_flareAlpha", (float)valOf("flarealph"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            PlasmaBlaster p = (PlasmaBlaster)t;
            addVal("lostaccuracy", p._accuracyLost);
            addVal("flarealph", Extensions.GetPrivateFieldValue<float>(p, "_flareAlpha"));
            base.RecordUpdate();
        }
    }
}
