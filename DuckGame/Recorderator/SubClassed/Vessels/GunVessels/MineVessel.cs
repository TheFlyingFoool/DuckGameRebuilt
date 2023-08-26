using System.Collections;

namespace DuckGame
{
    public class MineVessel : GunVessel
    {
        public MineVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Mine));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_h");
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            MineVessel v = new MineVessel(new Mine(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Mine m = (Mine)t;
            byte ohNo_A_Plague_Upon_Thee = (byte)valOf("infoed");
            BitArray big_boy_array = new BitArray(new byte[] { ohNo_A_Plague_Upon_Thee });
            bool pinned = big_boy_array[0];
            m.visible = big_boy_array[1];
            m.grounded = big_boy_array[2];
            m.sleeping = big_boy_array[3];
            m.offDir = (sbyte)(big_boy_array[4] ? -1 : 1);
            m.infiniteAmmoVal = big_boy_array[5];
            m._armed = big_boy_array[6];
            if (m._pin && !pinned) SFX.Play("minePullPin");
            m._pin = pinned;
            m._timer = 100;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Mine m = (Mine)t;
            BitArray another_array = new BitArray(8);
            another_array[0] = m._pin;
            another_array[1] = m.visible;
            another_array[2] = m.grounded;
            another_array[3] = m.sleeping;
            another_array[4] = m.offDir > 0;
            another_array[5] = m.infiniteAmmoVal;
            another_array[6] = m._armed;
            addVal("infoed", Extensions.BitArrayToByte(another_array));
            base.RecordUpdate();
        }
    }
}
