using System.Collections;

namespace DuckGame
{
    public class TampingWeaponVessel : GunVessel
    {
        public TampingWeaponVessel(Thing th) : base(th)
        {
            //this was here for reasons dont ask me
            //tatchedTo.Add(typeof(TampingWeapon));
            tatchedTo.Add(typeof(Blunderbuss));
            tatchedTo.Add(typeof(Musket));
            tatchedTo.Add(typeof(Bazooka));
            RemoveSynncl("infoed_g");
            RemoveSynncl("infoed_x");
            AddSynncl("yOff", new SomethingSync(typeof(float))); //optimize this later
            AddSynncl("tampPos", new SomethingSync(typeof(float)));
            AddSynncl("tampInc", new SomethingSync(typeof(float)));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));

            //optimization ideas for later
            //tampInc could be removed altogether and be put into infoed as a bool that checks when shoot was pressed to increase it by 0.14f
            //tampPos could probably be fit into infoed
            //yOff can be fit into a byte
        }
        public override void ApplyFire()
        {
            TampingWeapon tw = (TampingWeapon)t;
            int num = 0;
            for (int index = 0; index < 14 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); ++index)
            {
                MusketSmoke musketSmoke = new MusketSmoke(tw.barrelPosition.x - 16f + Rando.Float(32f), tw.barrelPosition.y - 16f + Rando.Float(32f))
                {
                    depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                };
                if (num < 6) musketSmoke.move -= tw.barrelVector * Rando.Float(0.1f);
                if (num > 5 && num < 10) musketSmoke.fly += tw.barrelVector * (2f + Rando.Float(7.8f));
                Level.Add(musketSmoke);
                num++;
            }
            base.ApplyFire();
        }
        public override void PlaybackUpdate()
        {//tamped tamping rotating

            TampingWeapon tw = (TampingWeapon)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed") });
            tw.tamping = br[0];
            tw._tamped = br[1];
            tw._rotating = br[2];

            if (br[3]) ApplyFire();
            if (br[4]) tw.DoAmmoClick();

            tw.infiniteAmmoVal = br[7];
            tw._offsetY = (float)valOf("yOff");
            tw.tampPos = (float)valOf("tampPos");
            tw._tampInc = (float)valOf("tampInc");

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            TampingWeapon tw = (TampingWeapon)t;
            BitArray br = new BitArray(8);
            br[0] = tw.tamping;
            br[1] = tw._tamped;
            br[2] = tw._rotating;

            br[3] = tw.recordKick;
            br[4] = tw.recordPuff;

            /*g.recordPopShell;
            br2[1] = g.recordPuff;
             * */
            br[7] = tw.infiniteAmmoVal;
            addVal("yOff", tw._offsetY);
            addVal("tampPos", tw.tampPos);
            addVal("tampInc", tw._tampInc);
            addVal("infoed", BitCrusher.BitArrayToByte(br));
            base.RecordUpdate();
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Gun g = null;
            byte bXD = b.ReadByte();
            if (bXD == 0) g = new Blunderbuss(0, -2000);
            else if (bXD == 1) g = new Musket(0, -2000);
            else if (bXD == 2) g = new Bazooka(0, -2000);
            TampingWeaponVessel v = new TampingWeaponVessel(g);
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            if (t is Blunderbuss) prevBuffer.Write((byte)0);
            else if (t is Musket) prevBuffer.Write((byte)1);
            else if (t is Bazooka) prevBuffer.Write((byte)2);
            return prevBuffer;
        }
        public override void DoUpdateThing()
        {
            Gun g = (Gun)t;
            if (g.kick > 0) g.kick -= 0.2f;
            else g.kick = 0;
            if (g._flareAlpha > 0f) g._flareAlpha -= 0.5f;
            else g._flareAlpha = 0f;
        }
    }
}