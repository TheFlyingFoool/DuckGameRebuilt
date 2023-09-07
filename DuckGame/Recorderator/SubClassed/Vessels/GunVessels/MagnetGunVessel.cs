using System.Collections;

namespace DuckGame
{
    public class MagnetGunVessel : GunVessel
    {
        public MagnetGunVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            AddSynncl("infoed_mg", new SomethingSync(typeof(byte)));
            AddSynncl("hold", new SomethingSync(typeof(ushort)));//x
            tatchedTo.Add(typeof(MagnetGun));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            MagnetGunVessel v = new MagnetGunVessel(new MagnetGun(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            MagnetGun mg = (MagnetGun)t;
            int hObj = ((ushort)valOf("hold")) - 1;

            if (hObj != -1 && Corderator.instance.somethingMap.Contains(hObj)) mg.grabbed = (Holdable)Corderator.instance.somethingMap[hObj];
            else mg.grabbed = null;

            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_mg") });

            mg.infiniteAmmoVal = br[0];
            mg._magnetActive = br[1];


            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            MagnetGun mg = (MagnetGun)t;
            if (mg.grabbed != null && Corderator.instance.somethingMap.Contains(mg.grabbed)) addVal("hold", (ushort)(Corderator.instance.somethingMap[mg.grabbed] + 1));
            else addVal("hold", (ushort)0);

            BitArray br = new BitArray(8);
            br[0] = mg.infiniteAmmoVal;
            br[1] = mg._magnetActive;


            addVal("infoed_mg", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
