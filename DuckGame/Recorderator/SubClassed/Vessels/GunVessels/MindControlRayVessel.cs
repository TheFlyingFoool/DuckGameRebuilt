using System.Collections;

namespace DuckGame
{
    public class MindControlRayVessel : GunVessel
    {
        public MindControlRayVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(MindControlRay));
            RemoveSynncl("infoed_x");
            RemoveSynncl("infoed_g");
            AddSynncl("infoed_mcr", new SomethingSync(typeof(byte)));
            AddSynncl("controlling", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            MindControlRayVessel v = new MindControlRayVessel(new MindControlRay(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            MindControlRay mcr = (MindControlRay)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_mcr") });

            mcr._triggerHeld = br[6];
            mcr.infiniteAmmoVal = br[7];

            int controlling = (ushort)valOf("controlling") - 1;
            
            if (controlling != -1 && Corderator.instance.somethingMap.Contains(controlling))
            {
                Duck cdd = (Duck)Corderator.instance.somethingMap[controlling];
                if (mcr._controlledDuck == null)
                {
                    mcr.ControlDuck(cdd);
                }
                mcr._controlledDuck = cdd;
            }
            else
            {
                if (mcr._controlledDuck != null)
                {
                    mcr.LoseControl();
                }
                mcr._controlledDuck = null;
            }

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {//3
            MindControlRay mcr = (MindControlRay)t;
            BitArray br = new BitArray(8);
            br[6] = mcr._triggerHeld;
            br[7] = mcr.infiniteAmmoVal;
            addVal("infoed_mcr", BitCrusher.BitArrayToByte(br));

            if (mcr.controlledDuck != null && Corderator.instance != null && Corderator.instance.somethingMap.ContainsValue(mcr.controlledDuck)) addVal("controlling", (ushort)(Corderator.instance.somethingMap[mcr.controlledDuck] + 1));
            else addVal("controlling", (ushort)0);

            base.RecordUpdate();
        }
    }
}
