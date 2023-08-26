//wrote this on phone
namespace DuckGame
{
    public class PresentVessel : HoldableVessel
    {
        public PresentVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Present));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            byte col = b.ReadByte();
            PresentVessel v = new PresentVessel(new Present(0, -2000));
            v.t.frame = col;
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write((byte)((Present)t).frame);
            return prevBuffer;
        }
    }
}
