using System.Collections;

namespace DuckGame
{
    public class GrenadeVessel : GunVessel
    {
        public GrenadeVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Grenade));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            GrenadeVessel v = new GrenadeVessel(new Grenade(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Grenade g = (Grenade)t;
            byte infoed = (byte)valOf("infoed");
            BitArray big_boy_array = new BitArray(new byte[] { infoed });
            bool pinned = big_boy_array[0];
            g.visible = big_boy_array[1];
            g.grounded = big_boy_array[2];
            g.sleeping = big_boy_array[3];
            g.offDir = (sbyte)(big_boy_array[4] ? -1 : 1);
            g.infiniteAmmoVal = big_boy_array[5];
            if (g._pin && !pinned)
            {
                Level.Add(new GrenadePin(g.x, g.y) { hSpeed = -g.offDir * (1.5f + Rando.Float(0.5f)), vSpeed = -2f });
                SFX.Play("pullPin");
            }
            g._pin = pinned;
            g._timer = 100;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Grenade g = (Grenade)t;
            BitArray another_array = new BitArray(8);
            another_array[0] = g._pin;
            another_array[1] = g.visible;
            another_array[2] = g.grounded;
            another_array[3] = g.sleeping;
            another_array[4] = g.offDir > 0;
            another_array[5] = g.infiniteAmmoVal;
            addVal("infoed", BitCrusher.BitArrayToByte(another_array));
            base.RecordUpdate();
        }
    }
}
