using System;
using System.Collections;

namespace DuckGame
{
    public class GrenadeCannonVessel : GunVessel
    {
        public GrenadeCannonVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(GrenadeCannon));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_x");
            AddSynncl("fireang", new SomethingSync(typeof(ushort)));
            AddSynncl("infoed_gc", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GrenadeCannonVessel v = new GrenadeCannonVessel(new GrenadeCannon(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }//max ammo is 4 1 2 4
        public override void ApplyFire()
        {
            Gun g = (Gun)t;
            g.kick = 1;
        }
        public override void PlaybackUpdate()
        {
            GrenadeCannon gc = (GrenadeCannon)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_gc") });

            int val = -1;
            if (br[0]) val += 4;
            if (br[1]) val += 2;
            if (br[2]) val += 1;
            if (br[3]) ApplyFire();

            if (val == -1) gc.infiniteAmmoVal = true;
            else
            {
                gc.infiniteAmmoVal = false;
                gc.ammo = val;
            }
            gc._aiming = br[4];
            gc.laserSight = br[5];
            if (!gc._doLoad && br[6])
            {
                gc.sprite.SetAnimation("load" + Math.Min(gc.ammo, 4).ToString());
            }
            gc._doLoad = br[6];

            gc._fireAngle = BitCrusher.UShortToFloat((ushort)valOf("fireang"), 720) - 360;

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            GrenadeCannon gc = (GrenadeCannon)t;
            BitArray br = new BitArray(8);

            int ammoval = gc.ammo + 1;
            if (gc.infiniteAmmoVal) ammoval = 0;
            br[0] = (ammoval & 4) > 0;
            br[1] = (ammoval & 2) > 0;
            br[2] = (ammoval & 1) > 0;
            br[3] = gc.recordKick;
            br[4] = gc._aiming;
            br[5] = gc.laserSight;
            br[6] = gc._doLoad;
            addVal("infoed_gc", BitCrusher.BitArrayToByte(br));
            addVal("fireang", BitCrusher.FloatToUShort(gc._fireAngle % 360 + 360, 720));
            base.RecordUpdate();
        }
    }
}
