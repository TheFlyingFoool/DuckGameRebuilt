using NAudio.MediaFoundation;
using System;
using System.Collections;

namespace DuckGame
{
    public class EnergyscimitarVessel : GunVessel
    {
        public EnergyscimitarVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(EnergyScimitar));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            RemoveSynncl("angledeg");
            AddSynncl("infoed_energy", new SomethingSync(typeof(byte)));
            AddSynncl("infoed_energy2", new SomethingSync(typeof(byte)));
            AddSynncl("infoed_energy3", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            EnergyscimitarVessel v = new EnergyscimitarVessel(new EnergyScimitar(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            EnergyScimitar e = (EnergyScimitar)t;
            byte z = (byte)valOf("infoed_energy");
            BitArray br = new BitArray(new byte[] { z });

            int divide = 4;
            int stance = 0;
            for (int i = 0; i < 3; i++)
            {
                if (br[i]) stance += divide;
                divide /= 2;
            }
            e.stanceInt = stance;



            z = (byte)valOf("infoed_energy2");
            br = new BitArray(new byte[] { z });
            e.offDir = (sbyte)(br[0] ? 1 : -1);
            e.visible = br[1];
            e._grounded = br[2];
            e._stuck = br[3];
            e.sleeping = br[4];
            e._airFly = br[5];
            e._wasLifted = br[6];
            e.infiniteAmmoVal = br[7];

            z = (byte)valOf("infoed_energy3");
            br = new BitArray(new byte[] { z });
            divide = 64;
            float ang = 0;
            for (int i = 1; i < 8; i++)
            {
                if (br[i]) ang += divide;
                divide /= 2;
            }
            ang *= 2.8125f;
            ang *= br[0] ? -1 : 1;
            e._airFlyAngle = ang;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            EnergyScimitar e = (EnergyScimitar)t;
            BitArray br = new BitArray(8);
            //_airFly _wasLifted stanceInt

            int w = Maths.Clamp(e.stanceInt, 0, 6);

            br[0] = (w & 4) > 0;
            br[1] = (w & 2) > 0;
            br[2] = (w & 1) > 0;


            addVal("infoed_energy", BitCrusher.BitArrayToByte(br));

            br = new BitArray(8);
            br[0] = e.offDir > 0;
            br[1] = e.visible;
            br[2] = e._grounded;
            br[3] = e.stuck;
            br[4] = e.sleeping;
            br[5] = e._airFly;
            br[6] = e._wasLifted;
            br[7] = e.infiniteAmmoVal;
            addVal("infoed_energy2", BitCrusher.BitArrayToByte(br));

            br = new BitArray(8);
            int c = (int)Math.Abs(Math.Round((e._airFlyAngle % 360) / 2.8125f)); //we divide for 2.8125f because 360/2.8125f returns exactly 128 which is as much as we can store here
            br[0] = e._airFlyAngle < 0;
            br[1] = (c & 64) > 0;
            br[2] = (c & 32) > 0;
            br[3] = (c & 16) > 0;
            br[4] = (c & 8) > 0;
            br[5] = (c & 4) > 0;
            br[6] = (c & 2) > 0;
            br[7] = (c & 1) > 0;
            //_airFlyAngle compressed into 7 non negative

            addVal("infoed_energy3", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
    }
}
