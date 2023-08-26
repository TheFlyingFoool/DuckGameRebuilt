//made in phone tm
namespace DuckGame
{
    public class PhaserVessel : GunVessel
    {
        public PhaserVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Phaser));
            AddSynncl("charge", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PhaserVessel v = new PhaserVessel(new Phaser(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Phaser p = (Phaser)t;
            float hat = (float)valOf("charge");
            Extensions.SetPrivateFieldValue(p, "_chargeFade", hat);
            base.PlaybackUpdate();
        }
        public override void DoUpdateThing()
        {
        }
        public override void RecordUpdate()
        {
            Phaser p = (Phaser)t;
            addVal("charge", Extensions.GetPrivateFieldValue<float>(p, "_chargeFade"));
            base.RecordUpdate();
        }
    }
}
