using System.Collections;

namespace DuckGame
{
    public class PelletGunVessel : GunVessel
    {
        public PelletGunVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(PelletGun));
            AddSynncl("ang", new SomethingSync(typeof(ushort)));
            RemoveSynncl("infoed_g");
            AddSynncl("infoed_d", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PelletGunVessel v = new PelletGunVessel(new PelletGun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            PelletGun gl = (PelletGun)t;
            gl._aimAngle = BitCrusher.UShortToFloat((ushort)valOf("ang"), 1) - 0.4f;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_d") });

            int val = -1;
            if (br[0]) val += 4;
            if (br[1]) val += 2;
            if (br[2]) val += 1;

            gl._loadState = val;
            gl.infiniteAmmoVal = br[7];

            int fireTilFail = 0;
            if (br[3]) fireTilFail += 8;
            if (br[4]) fireTilFail += 4;
            if (br[5]) fireTilFail += 2;
            if (br[6]) fireTilFail += 1;
            gl.firesTillFail = fireTilFail;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            PelletGun gl = (PelletGun)t;
            addVal("ang", BitCrusher.FloatToUShort(gl._aimAngle + 0.4f, 1));

            BitArray br = new BitArray(8);
            int stat = gl._loadState + 1;
            br[0] = (stat & 4) > 0;
            br[1] = (stat & 2) > 0;
            br[2] = (stat & 1) > 0;

            
            br[3] = (gl.firesTillFail & 8) > 0;
            br[4] = (gl.firesTillFail & 4) > 0;
            br[5] = (gl.firesTillFail & 2) > 0;
            br[6] = (gl.firesTillFail & 1) > 0;


            br[7] = gl.infiniteAmmoVal;
            addVal("infoed_d", BitCrusher.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
