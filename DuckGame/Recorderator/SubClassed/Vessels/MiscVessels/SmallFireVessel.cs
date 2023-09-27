using System.CodeDom;

namespace DuckGame
{
    public class SmallFireVessel : SomethingSomethingVessel
    {
        public SmallFireVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(SmallFire));
            AddSynncl("X", new SomethingSync(typeof(short)));
            AddSynncl("Y", new SomethingSync(typeof(short)));
            AddSynncl("stickoffX", new SomethingSync(typeof(sbyte)));
            AddSynncl("stickoffY", new SomethingSync(typeof(sbyte)));
            AddSynncl("stick", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SmallFireVessel v = new SmallFireVessel(SmallFire.New(0, -2000, 0, 0, false, null, false, null, true));
            return v;
        }
        public override void PlaybackUpdate()
        {
            SmallFire s = (SmallFire)t;
            s.isLocal = false;
            s.netLerpPosition = new Vec2((short)valOf("X"), (short)valOf("Y"));

            Vec2 v = new Vec2((sbyte)valOf("stickoffX"), (sbyte)valOf("stickoffY"));
            s.stickOffset = v;

            int stick = (ushort)valOf("stick") - 1;
            if (stick != -1 && Corderator.instance.somethingMap.Contains(stick))
            {
                s.stick = (MaterialThing)Corderator.instance.somethingMap[stick];
            }
            else s.stick = null;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            SmallFire s = (SmallFire)t;

            bool addedX = false;
            if (s.stick != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(s.stick)) addVal("stick", (ushort)(Corderator.instance.somethingMap[s.stick] + 1));
            else
            {
                addedX = true;
                addVal("X", (short)s.x);
                addVal("Y", (short)s.y);
                addVal("stick", (ushort)0);
            }
            if ((!addedX && skipPositioning == 0) || exFrames == 0)
            {//safeguard because recorderator EXPLODES if theres not at least one value added to this
                addVal("X", (short)s.x);
                addVal("Y", (short)s.y);
            }

            addVal("stickoffX", (sbyte)s.stickOffset.x);
            addVal("stickoffY", (sbyte)s.stickOffset.x);
            base.RecordUpdate();
        }
    }
}
