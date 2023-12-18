namespace DuckGame
{
    public class TapedGunVessel : GunVessel
    {
        public TapedGunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(TapedGun));
            AddSynncl("gun1", new SomethingSync(typeof(ushort)));
            AddSynncl("gun2", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            TapedGunVessel v = new TapedGunVessel(new TapedGun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TapedGun tg = (TapedGun)t;
            int gun1 = (ushort)valOf("gun1") - 1;
            int gun2 = (ushort)valOf("gun2") - 1;
            if (gun1 != -1 && Corderator.instance.somethingMap.Contains(gun1))
            {
                tg.gun1 = (Holdable)Corderator.instance.somethingMap[gun1];
            }
            else tg.gun1 = null;

            if (gun2 != -1 && Corderator.instance.somethingMap.Contains(gun2))
            {
                tg.gun2 = (Holdable)Corderator.instance.somethingMap[gun2];
            }
            else tg.gun2 = null;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            TapedGun tg = (TapedGun)t;
            addVal("gun1", Corderator.Indexify(tg.gun1));
            addVal("gun2", Corderator.Indexify(tg.gun2));
            base.RecordUpdate();
        }
    }
}
