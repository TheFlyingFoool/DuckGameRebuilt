using System.Collections;

namespace DuckGame
{
    public class ChaindartVessel : GunVessel
    {
        public ChaindartVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Chaindart));
            RemoveSynncl("infoed_x");
            AddSynncl("infoed_cd", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ChaindartVessel v = new ChaindartVessel(new Chaindart(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Chaindart cd = (Chaindart)t;
            BitArray br = new BitArray(new byte[] {(byte)valOf("infoed_cd")});
            cd._spin = BitCrusher.DecompressFloat(br, 1, 7, 1);
            if (!cd.spinning && br[0])
            {
                cd.spinDown.Volume = 0f;
                cd.spinDown.Stop();
                cd.spinUp.Volume = 1f;
                cd.spinUp.Play();
            }
            if (cd.spinning && !br[0])
            {
                cd.spinUp.Volume = 0f;
                cd.spinUp.Stop();
                if (cd._spin > 0.9f)
                {
                    cd.spinDown.Volume = 1f;
                    cd.spinDown.Play();
                }
            }
            cd.spinning = br[0];

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()//_spin
        {
            Chaindart cd = (Chaindart)t;
            BitArray br = new BitArray(8);
            br[0] = cd.spinning;
            BitCrusher.CompressFloat(br, 1, 7, cd._spin, 1);
            
            addVal("infoed_cd", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
