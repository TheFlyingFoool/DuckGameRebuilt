using System.Collections;

namespace DuckGame
{
    public class SniperVessel : GunVessel
    {
        public SniperVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            tatchedTo.Add(typeof(Sniper));
            //destruction of bytes
            AddSynncl("MEGAINFOED", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SniperVessel v = new SniperVessel(new Sniper(0, -2000));
            return v;
        }
        public override void DoUpdateThing()
        {
        }
        public override void PlaybackUpdate()
        {
            Sniper s = (Sniper)t;
            s.active = true;
            byte fuck = (byte)valOf("MEGAINFOED");
            BitArray br = new BitArray(new byte[] { fuck });
            int divide = 4;
            int NinjasPlusPlus = 0;
            for (int i = 0; i < 3; i++)
            {
                if (br[i]) NinjasPlusPlus += divide;
                divide /= 2;
            }
            if (NinjasPlusPlus == 0) s.infiniteAmmoVal = true;
            else s.ammo = NinjasPlusPlus - 1;

            int lod = -1;
            divide = 4;
            for (int i = 3; i < 6; i++)
            {
                if (br[i]) lod += divide;
                divide /= 2;
            }
            s._loadState = lod;
            s.offDir = (sbyte)(br[6] ? 1 : -1);
            s.loaded = br[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Sniper s = (Sniper)t;
            BitArray br = new BitArray(8);
            int ammod = s.ammo + 1;
            int lod = s._loadState + 1;
            if (s.infiniteAmmoVal) ammod = 0;
            br[0] = (ammod & 4) > 0;
            br[1] = (ammod & 2) > 0;
            br[2] = (ammod & 1) > 0;
            br[3] = (lod & 4) > 0;
            br[4] = (lod & 2) > 0;
            br[5] = (lod & 1) > 0;
            br[6] = s.offDir > 0;
            br[7] = s.loaded;
            addVal("MEGAINFOED", Extensions.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
