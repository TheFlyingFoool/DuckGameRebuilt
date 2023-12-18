using System.Collections;

namespace DuckGame
{
    public class ChaingunVessel : GunVessel
    {
        public ChaingunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Chaingun));
            RemoveSynncl("infoed_x");
            AddSynncl("infoed_cd", new SomethingSync(typeof(byte)));
            AddSynncl("barrelheat", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ChaingunVessel v = new ChaingunVessel(new Chaingun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Chaingun cg = (Chaingun)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_cd") });
            cg._spin = BitCrusher.DecompressFloat(br, 1, 7, 1);
            if (!cg.spinning && br[0])
            {
                cg.spinDown.Volume = 0f;
                cg.spinDown.Stop();
                cg.spinUp.Volume = 1f;
                cg.spinUp.Play();
            }
            if (cg.spinning && !br[0])
            {
                cg.spinUp.Volume = 0f;
                cg.spinUp.Stop();
                if (cg._spin > 0.9f)
                {
                    cg.spinDown.Volume = 1f;
                    cg.spinDown.Play();
                }
            }
            cg.spinning = br[0];
            cg._barrelHeat = BitCrusher.ByteToFloat((byte)valOf("barrelheat"));
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Chaingun cg = (Chaingun)t;
            BitArray br = new BitArray(8);
            br[0] = cg.spinning;
            BitCrusher.CompressFloat(br, 1, 7, cg._spin, 1);

            addVal("infoed_cd", BitCrusher.BitArrayToByte(br));
            addVal("barrelheat", BitCrusher.FloatToByte(cg._barrelHeat));
            base.RecordUpdate();
        }
    }
}
