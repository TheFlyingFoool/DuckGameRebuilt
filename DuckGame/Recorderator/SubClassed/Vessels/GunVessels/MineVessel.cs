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
            byte inFoed = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { inFoed });
            bool pinned = br[0];
            m.visible = br[1];
            m.grounded = br[2];
            m.sleeping = br[3];
            m.offDir = (sbyte)(br[4] ? -1 : 1);
            m.infiniteAmmoVal = br[5];
            m._armed = br[6];
            if (m._pin && !pinned) SFX.Play("minePullPin");
            m._pin = pinned;
            m._timer = 100;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Mine m = (Mine)t;
            BitArray array = new BitArray(8);
            array[0] = m._pin;
            array[1] = m.visible;
            array[2] = m.grounded;
            array[3] = m.sleeping;
            array[4] = m.offDir > 0;
            array[5] = m.infiniteAmmoVal;
            array[6] = m._armed;
            addVal("infoed", BitCrusher.BitArrayToByte(array));
            base.RecordUpdate();
        }
    }
}
