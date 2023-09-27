namespace DuckGame
{
    public class GrenadeLauncherVessel : GunVessel
    {
        public GrenadeLauncherVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(GrenadeLauncher));
            AddSynncl("ang", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GrenadeLauncherVessel v = new GrenadeLauncherVessel(new GrenadeLauncher(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {//_aimAngle
            GrenadeLauncher gl = (GrenadeLauncher)t;
            gl._fireAngle = BitCrusher.UShortToFloat((ushort)valOf("ang"), 720) - 360;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            GrenadeLauncher gl = (GrenadeLauncher)t;
            addVal("ang", BitCrusher.FloatToUShort(gl._fireAngle % 360 + 360, 720));
            base.RecordUpdate();
        }
    }
}
