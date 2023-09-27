namespace DuckGame
{
    public class MagBlasterVessel : GunVessel
    {
        public MagBlasterVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(MagBlaster));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            MagBlasterVessel v = new MagBlasterVessel(new MagBlaster(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void ApplyFire()
        {
            MagBlaster mg = (MagBlaster)t;
            mg._sprite.SetAnimation("fire");
            base.ApplyFire();
        }
        public override void PlaybackUpdate()
        {
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            base.RecordUpdate();
        }
    }
}
