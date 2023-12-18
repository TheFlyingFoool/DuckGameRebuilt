using System.Collections;

namespace DuckGame
{
    //yes this is a copy paste of the sniper vessel
    public class SharpshotVessel : GunVessel
    {
        public SharpshotVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            tatchedTo.Add(typeof(Sharpshot));
            //destruction of bytes
            AddSynncl("MEGAINFOED", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SharpshotVessel v = new SharpshotVessel(new Sharpshot(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void DoUpdateThing()
        {
            Gun g = (Gun)t;
            if (g.kick > 0) g.kick -= 0.2f;
            else g.kick = 0;
            if (g._flareAlpha > 0f) g._flareAlpha -= 0.5f;
            else g._flareAlpha = 0f;
        }
        public override void PlaybackUpdate()
        {
            Sharpshot s = (Sharpshot)t;
            s.active = true;
            byte fuck = (byte)valOf("MEGAINFOED");
            BitArray br = new BitArray(new byte[] { fuck });
            int divide = 4;
            int plus = 0;
            for (int i = 0; i < 3; i++)
            {
                if (br[i]) plus += divide;
                divide /= 2;
            }
            if (plus == 0) s.infiniteAmmoVal = true;
            else s.ammo = plus - 1;

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

            if (bArray[7]) ApplyFire();
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Sharpshot s = (Sharpshot)t;
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
            addVal("MEGAINFOED", BitCrusher.BitArrayToByte(br));
            bArray[7] = s.recordKick;
            base.RecordUpdate();

        }
    }
}
