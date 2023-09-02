using System.Collections;
using System.Drawing.Design;

namespace DuckGame
{
    public class PistolVessel : GunVessel
    {
        public PistolVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(Pistol));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            PistolVessel v = new PistolVessel(new Pistol(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Pistol op = (Pistol)t;
            byte infoed = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { infoed }); //What.


            int div = 2;
            int value = 0;
            for (int i = 2; i < 4; i++)
            {
                if (br[i]) value += div;
                div /= 2;
            }
            op.frame = value;

            div = 8;
            value = 0;
            for (int i = 4; i < 8; i++)
            {
                if (br[i]) value += div;
                div /= 2;
            }
            op.ammo = value;
            op.infiniteAmmoVal = op.ammo > 9;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Pistol op = (Pistol)t;
            BitArray br = new BitArray(8);

            //1 2 4 8 [4 bits for ammo
            //1 2 [2 bits for sprite


            int ammo = op.ammo;
            int sp = op.frame;

            br[2] = (sp & 2) > 0;
            br[3] = (sp & 1) > 0;
            br[4] = (ammo & 8) > 0; //if ammo > 9 then infinite
            br[5] = (ammo & 4) > 0;
            br[6] = (ammo & 2) > 0;
            br[7] = (ammo & 1) > 0;
            addVal("infoed", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
