using System.Linq;

namespace DuckGame
{
    public class PinkBoxVessel : SomethingSomethingVessel
    {
        public PinkBoxVessel(Thing th) : base(th)
        {
            IndexPriority = 1;
            tatchedTo.Add(typeof(PinkBox));
            AddSynncl("position", new SomethingSync(typeof(int)));

        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            return new PinkBoxVessel(new PinkBox(0, -2000));
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            PinkBox p = (PinkBox)t;

            p.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            p.d = (Duck)Corderator.Unindexify((ushort)valOf("d"));

            base.PlaybackUpdate();

            p._hit = bArray[7];
        }
        public override void RecordUpdate()
        {
            PinkBox p = (PinkBox)t;

            addVal("position", CompressedVec2Binding.GetCompressedVec2(p.position, 10000));
            addVal("d", Corderator.Indexify(p.d));

            base.RecordUpdate();

            bArray[7] = p._hit;
        }
    }
}
