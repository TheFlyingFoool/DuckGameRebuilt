using System.Collections;

namespace DuckGame
{
    public class OldPistolVessel : GunVessel
    {
        public OldPistolVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            tatchedTo.Add(typeof(OldPistol));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            OldPistolVessel v = new OldPistolVessel(new OldPistol(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            OldPistol op = (OldPistol)t;
            byte infoed = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { infoed }); //What.
            int nplusplus = 0;
            int divide = 8;
            for (int i = 0; i < 4; i++)
            {
                if (br[i]) nplusplus += divide;
                divide /= 2;
            }
            op._loadState = nplusplus - 1;
            op.infiniteAmmoVal = br[4];
            op.offDir = (sbyte)(br[5] ? 1 : -1);
            op.visible = br[6];
            if (op.loaded && !br[7]) ApplyFire();
            op.loaded = br[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            OldPistol op = (OldPistol)t;
            BitArray br = new BitArray(8);
            int z = op._loadState + 1;
            br[0] = (z & 8) > 0;
            br[1] = (z & 4) > 0;
            br[2] = (z & 2) > 0;
            br[3] = (z & 1) > 0;
            br[4] = op.infiniteAmmoVal;
            br[5] = op.offDir > 0;
            br[6] = op.visible;
            br[7] = op.loaded;
            addVal("infoed", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
