using System.Collections;

namespace DuckGame
{
    public class KeytarVessel : GunVessel
    {
        public KeytarVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Keytar));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_x");
            AddSynncl("pitch", new SomethingSync(typeof(byte)));
            AddSynncl("hpitch", new SomethingSync(typeof(byte)));
            AddSynncl("bender", new SomethingSync(typeof(byte)));
            AddSynncl("infoed_k", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            KeytarVessel v = new KeytarVessel(new Keytar(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Keytar tb = (Keytar)t;
            tb.notePitch = BitCrusher.ByteToFloat((byte)valOf("pitch"));
            tb.handPitch = BitCrusher.ByteToFloat((byte)valOf("hpitch"));
            tb.bender = BitCrusher.ByteToFloat((byte)valOf("bender"));

            BitArray br = new BitArray(16);
            BitCrusher.UShortIntoArray((ushort)valOf("infoed_k"), ref br);
            tb.infiniteAmmoVal = br[0];
            int brokenKey = 0;
            if (br[1]) brokenKey += 8;
            if (br[2]) brokenKey += 4;
            if (br[3]) brokenKey += 2;
            if (br[4]) brokenKey += 1;
            tb.brokenKey = (byte)brokenKey;
            int color = 0;
            if (br[5]) color += 4;
            if (br[6]) color += 2;
            if (br[7]) color += 1;
            tb.colorVariation = (byte)color;
            int pres = 0;
            if (br[8]) pres += 4;
            if (br[9]) pres += 2;
            if (br[10]) pres += 1;
            tb.preset = (sbyte)pres;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Keytar tb = (Keytar)t;
            addVal("pitch", BitCrusher.FloatToByte(tb.notePitch));
            addVal("hpitch", BitCrusher.FloatToByte(tb.handPitch));
            addVal("bender", BitCrusher.FloatToByte(tb.bender));

            BitArray br = new BitArray(16);
            br[0] = tb.infiniteAmmoVal;
            br[1] = (tb.brokenKey & 8) > 0;
            br[2] = (tb.brokenKey & 4) > 0;
            br[3] = (tb.brokenKey & 2) > 0;
            br[4] = (tb.brokenKey & 1) > 0;
            br[5] = (tb.colorVariation & 4) > 0;
            br[6] = (tb.colorVariation & 2) > 0;
            br[7] = (tb.colorVariation & 1) > 0;
            br[8] = (tb.preset & 4) > 0;
            br[9] = (tb.preset & 2) > 0;
            br[10] = (tb.preset & 1) > 0;

            addVal("infoed_k", BitCrusher.BitArrayToUShort(br));

            base.RecordUpdate();
        }
    }
}
