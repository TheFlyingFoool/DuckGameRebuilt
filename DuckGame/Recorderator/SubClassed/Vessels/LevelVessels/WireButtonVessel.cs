using System.Collections;

namespace DuckGame
{
    public class WireButtonVessel : SomethingSomethingVessel
    {
        public WireButtonVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(WireButton));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);

            BitArray br = new BitArray(new byte[] { b.ReadByte() });

            WireButton wm = new WireButton(v.x, v.y);
            wm.invert = br[0];
            wm.releaseOnly = br[1];
            wm.offSignal = br[2];

            int val = 0;
            if (br[3]) val += 2;
            if (br[4]) val += 1; //why here
            wm.orientation = val;
            wm.holdTime = BitCrusher.ByteToFloat(b.ReadByte());

            return new WireButtonVessel(wm);
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            /* public EditorProperty<bool> offSignal = new EditorProperty<bool>(false);
        public EditorProperty<float> holdTime = new EditorProperty<float>(0f);
        public EditorProperty<bool> releaseOnly = new EditorProperty<bool>(false);
        public EditorProperty<bool> invert = new EditorProperty<bool>(false);
        public EditorProperty<int> orientation = new EditorProperty<int>(0, max: 3f, increment: 1f);
            */
            WireButton w = (WireButton)t;
            prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(t.position, 10000));

            BitArray br = new BitArray(8);
            br[0] = w.invert;
            br[1] = w.releaseOnly;
            br[2] = w.offSignal;
            br[3] = (w.orientation.value & 2) > 0;
            br[4] = (w.orientation.value & 1) > 0;
            prevBuffer.Write(BitCrusher.BitArrayToByte(br));
            prevBuffer.Write(BitCrusher.FloatToByte(w.holdTime));

            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            WireButton w = (WireButton)t;


            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            WireButton w = (WireButton)t;
            base.RecordUpdate();
        }
    }
}