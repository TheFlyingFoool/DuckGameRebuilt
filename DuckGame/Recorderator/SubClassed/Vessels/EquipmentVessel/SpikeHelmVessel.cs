namespace DuckGame
{
    public class SpikeHelmVessel : EquipmentVessel
    {
        public SpikeHelmVessel(Thing th) : base(th)
        {
            AddSynncl("hold", new SomethingSync(typeof(ushort)));//x
            tatchedTo.Add(typeof(SpikeHelm));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SpikeHelmVessel v = new SpikeHelmVessel(new SpikeHelm(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public PhysicsObject lastHold;
        public override void DoUpdateThing()
        {
            base.DoUpdateThing();
        }
        public override void PlaybackUpdate()
        {
            SpikeHelm sh = (SpikeHelm)t;
            int hObj = ((ushort)valOf("hold")) - 1;
            if (hObj == -1 && lastHold != null)
            {
                sh.ReleasePokedObject();
                lastHold.owner = null;
            }
            else if (hObj != -1 && lastHold == null && Corderator.instance.somethingMap.Contains(hObj)) sh.poked = (PhysicsObject)Corderator.instance.somethingMap[hObj];
            if (sh.poked != null) sh.poked.owner = sh;

            lastHold = sh.poked;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            SpikeHelm sh = (SpikeHelm)t;
            addVal("hold", Corderator.Indexify(sh.poked));
            base.RecordUpdate();
        }
    }
}
