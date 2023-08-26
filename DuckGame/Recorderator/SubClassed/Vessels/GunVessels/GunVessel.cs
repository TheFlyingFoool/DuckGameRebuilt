using System.Collections;
using System.Linq;

namespace DuckGame
{
    //both gun and holdable vessels
    //are placeholder vessels for stuff that
    //isn't tatched to anything meaning that most
    //stuff that is just an empty gunvessel wont be
    //replayed properly, this be why replays wont be compatible with most mods
    public class GunVessel : HoldableVessel
    {
        public GunVessel(Thing th) : base(th)
        {
            AddSynncl("angledeg", new SomethingSync(typeof(ushort)));
            AddSynncl("infoed_g", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GunVessel v = new GunVessel(Editor.CreateThing(Editor.IDToType[b.ReadUShort()]));
            v.t.y = -2000;
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            if (GetType() == typeof(GunVessel)) prevBuffer.Write(Editor.IDToType[t.GetType()]);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Gun g = (Gun)t;
            if (!g.active)
            {
                if (g.owner != null) g.depth = g.owner.depth + 10;
                else g.depth = -0.1f;
            }
            if (syncled.ContainsKey("angledeg")) g.angleDegrees = BitCrusher.UShortToFloat((ushort)valOf("angledeg"), 360);
            if (syncled.ContainsKey("infoed_g"))
            {
                byte z = (byte)valOf("infoed_g");//lol? nvm it isn't as bad as it was before but imagine bad code being here ok?
                BitArray b_ARR = new BitArray(new byte[] { z });
                int divide = 64;
                int ammo = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (b_ARR[i]) ammo += divide;
                    divide /= 2;
                }
                g.ammo = ammo;
                g.infiniteAmmoVal = b_ARR[7];
            }
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Gun g = (Gun)t;
            float f = g.angleDegrees % 360;
            addVal("angledeg", BitCrusher.FloatToUShort(f, 360));
            BitArray array_o_bits = new BitArray(8);

            int w = Maths.Clamp(g.ammo, 0, 100);
            array_o_bits[0] = (w & 64) > 0;
            array_o_bits[1] = (w & 32) > 0;
            array_o_bits[2] = (w & 16) > 0;
            array_o_bits[3] = (w & 8) > 0;
            array_o_bits[4] = (w & 4) > 0;
            array_o_bits[5] = (w & 2) > 0;
            array_o_bits[6] = (w & 1) > 0;
            array_o_bits[7] = g.infiniteAmmoVal;
            addVal("infoed_g", Extensions.BitArrayToByte(array_o_bits));
            base.RecordUpdate();
        }
    }
}
