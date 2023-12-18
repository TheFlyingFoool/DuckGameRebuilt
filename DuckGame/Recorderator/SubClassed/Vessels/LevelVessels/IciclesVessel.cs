using System.Collections;

namespace DuckGame
{
    public class IciclesVessel : SomethingSomethingVessel
    {
        public IciclesVessel(Thing th) : base(th) 
        {
            tatchedTo.Add(typeof(Icicles));
        }
        public override void PlaybackUpdate()
        {
            Icicles ic = (Icicles)t;

            int val = 0;
            if (bArray[5]) val += 4;
            if (bArray[6]) val += 2;
            if (bArray[7]) val += 1;
            ic.frame = val;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Icicles ic = (Icicles)t; // icu ○w○

            bArray[5] = (ic.frame & 4) > 0;
            bArray[6] = (ic.frame & 2) > 0;
            bArray[7] = (ic.frame & 1) > 0;

            base.RecordUpdate();
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            BitArray br = new BitArray(new byte[] { b.ReadByte() });
            Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
            Icicles ic = new Icicles(v.x, v.y, 0);
            ic.background = br[0];
            ic.flipHorizontal = br[1];
            if (br[2]) ic.style += 2;
            if (br[3]) ic.style++;//lol

            return new IciclesVessel(ic);
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            Icicles ic = (Icicles)t;
            BitArray br = new BitArray(8);
            br[0] = ic.background;
            br[1] = ic.flipHorizontal;
            br[2] = (ic.style & 2) > 0;
            br[3] = (ic.style & 1) > 0;
            prevBuffer.Write(BitCrusher.BitArrayToByte(br));

            prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(ic.position, 10000));

            return base.RecSerialize(prevBuffer);
        }
    }
}
