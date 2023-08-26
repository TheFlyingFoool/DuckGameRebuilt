using System.Collections;

namespace DuckGame
{
    public class SwordVessel : GunVessel
    {
        public SwordVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            RemoveSynncl("angledeg");
            tatchedTo.Add(typeof(Sword));
            AddSynncl("holdoffset", new SomethingSync(typeof(Vec2)));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            AddSynncl("ang", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            SwordVessel v = new SwordVessel(new Sword(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Sword s = (Sword)t;
            if (s.owner != null) s._hold = (float)valOf("ang");
            else s.angleDegrees = (float)valOf("ang");
            BitArray b_array = new BitArray(new byte[] { (byte)valOf("infoed") });
            s.infiniteAmmoVal = b_array[0];
            s._jabStance = b_array[1];
            s._crouchStance = b_array[2];
            s._slamStance = b_array[3];
            s._swinging = b_array[4];
            s._volatile = b_array[5];
            if (s.crouchStance) s.handOffset = (Vec2)valOf("holdoffset") + new Vec2(3, -4);
            else s.handOffset = (Vec2)valOf("holdoffset") + new Vec2(4, -4);
            s._holdOffset = (Vec2)valOf("holdoffset");
            if (b_array[6]) s.vSpeed = 1.8f;
            else s.hSpeed = 0;
            s.visible = b_array[7];
            base.PlaybackUpdate();
        }
        public override void DoUpdateThing()
        {
        }
        public override void RecordUpdate()
        {
            Sword s = (Sword)t;
            addVal("holdoffset", s._holdOffset);
            if (s.owner != null) addVal("ang", s._hold + s._swing);
            else addVal("ang", s.angleDegrees);
            BitArray brFilhoDaPuta = new BitArray(8);
            brFilhoDaPuta[0] = s.infiniteAmmoVal;
            brFilhoDaPuta[1] = s._jabStance;
            brFilhoDaPuta[2] = s._crouchStance;
            brFilhoDaPuta[3] = s._slamStance;
            brFilhoDaPuta[4] = s._swinging;
            brFilhoDaPuta[5] = s._volatile;
            brFilhoDaPuta[6] = (s.velocity.length > 1 && s.owner == null) || s._swing != 0;
            brFilhoDaPuta[7] = s.visible;
            addVal("infoed", Extensions.BitArrayToByte(brFilhoDaPuta));
            base.RecordUpdate();
        }
    }
}
