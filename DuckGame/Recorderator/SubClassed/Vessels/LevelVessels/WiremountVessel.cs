using System.Collections;

namespace DuckGame
{
    public class WiremountVessel : SomethingSomethingVessel
    {
        public WiremountVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(WireMount));
            AddSynncl("holding", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
            float f = b.ReadFloat();

            WireMount wm = new WireMount(v.x, v.y);
            wm.mountAngle = f;

            return new WiremountVessel(wm);
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(position, 10000));
            prevBuffer.Write(((WireMount)t).mountAngle);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            WireMount w = (WireMount)t;
            int h = (ushort)valOf("holding") - 1;
            if (h != -1) w._containedThing = Corderator.instance.somethingMap[h];
            else w._containedThing = null;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            WireMount w = (WireMount)t;

            ushort unsh = Corderator.Indexify(w._containedThing);
            addVal("holding", unsh);
            if (unsh != 0 && Corderator.instance.somethingMapped.Contains(unsh - 1))
            {
                Corderator.instance.somethingMapped[unsh - 1].skipAngles = 1;
            }
        }
    }
}