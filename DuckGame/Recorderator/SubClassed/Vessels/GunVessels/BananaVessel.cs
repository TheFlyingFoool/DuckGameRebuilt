using System.Collections;

﻿namespace DuckGame
{
    public class BananaVessel : GunVessel
    {
        public BananaVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_g");
            tatchedTo.Add(typeof(Banana));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            BananaVessel v = new BananaVessel(new Banana(0, -2000));
            return v;
        }
        public override void PlaybackUpdate()
        {
            Banana b = (Banana)t; 
            byte what = (byte)valOf("infoed");
            BitArray br = new BitArray(new byte[] { what });
            b._pin = br[0];
            b._thrown = br[1];
            b.infiniteAmmoVal = br[2];

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Banana b = (Banana)t;
            BitArray br = new BitArray(8);
            br[0] = b._pin;
            br[1] = b._thrown;
            br[2] = b.infiniteAmmoVal;
            addVal("infoed", Extensions.BitArrayToByte(br));

            base.RecordUpdate();
        }
    }
}
