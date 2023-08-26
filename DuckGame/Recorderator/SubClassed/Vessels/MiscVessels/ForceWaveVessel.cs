using System;
using System.Collections;

namespace DuckGame
{
    public class ForceWaveVessel : SomethingSomethingVessel
    {
        public ForceWaveVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(ForceWave));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ForceWaveVessel v = new ForceWaveVessel(new ForceWave(0, -2000, 0, 0, 0, 0, null));
            return v;
        }
        public override void PlaybackUpdate()
        {
            ForceWave f = (ForceWave)t;
            byte infoed = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { infoed });
            float a = 0;
            float div = 0.8f;
            for (int i = 0; i < 4; i++)
            {
                if (br[i]) a += div;
                div /= 2;
            }
            f.alpha = a;
            f.offDir = (sbyte)(br[7] ? 1 : -1);
            f.position = (Vec2)valOf("position");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            ForceWave f = (ForceWave)t;
            BitArray br = new BitArray(8);
            int yay = (int)Maths.Clamp((float)Math.Round(f.alpha * 10), 0, 15);
            br[0] = (yay & 8) > 0;
            br[1] = (yay & 4) > 0;
            br[2] = (yay & 2) > 0;
            br[3] = (yay & 1) > 0;

            br[7] = f.offDir > 0;
            addVal("infoed", BitCrusher.BitArrayToByte(br));
            addVal("position", f.position);
            base.RecordUpdate();
        }
    }
}
