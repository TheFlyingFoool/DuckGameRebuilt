using System.Collections;

namespace DuckGame
{
    public class PlasmaBlasterVessel : GunVessel
    {
        public PlasmaBlasterVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(PlasmaBlaster));
            RemoveSynncl("infoed_g");
            AddSynncl("infoed_p", new SomethingSync(typeof(byte)));
        }//0.9f
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PlasmaBlasterVessel v = new PlasmaBlasterVessel(new PlasmaBlaster(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void ApplyFire()
        {
            PlasmaBlaster p = (PlasmaBlaster)t;
            p._flareAlpha = 1;
            base.ApplyFire();
        }
        public override void PlaybackUpdate()
        {
            PlasmaBlaster p = (PlasmaBlaster)t;
            byte value = (byte)valOf("infoed_p");
            
            BitArray br = BitCrusher.ByteToBitArray(value);

            p.infiniteAmmoVal = br[0];
            p._accuracyLost = BitCrusher.DecompressFloat(br, 2, 7, 0.9f);

            if (br[1]) ApplyFire();

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            PlasmaBlaster p = (PlasmaBlaster)t;
            float lostaccuracy = p._accuracyLost;

            BitArray br = new BitArray(8);

            br[0] = p.infiniteAmmoVal;
            br[1] = p.kick == 1 || (p.kick > lastKick && p.kick > 0.4f);
            lastKick = p.kick;

            BitCrusher.CompressFloat(br, 2, 7, lostaccuracy, 0.9f);

            addVal("infoed_p", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
