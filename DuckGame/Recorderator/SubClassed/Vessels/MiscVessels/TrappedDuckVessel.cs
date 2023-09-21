using System;
using System.Collections;
using System.Linq;

namespace DuckGame
{
    public class TrappedDuckVessel : HoldableVessel
    {
        public TrappedDuckVessel(Thing th) : base(th)
        {
            AddSynncl("shake", new SomethingSync(typeof(byte)));
            AddSynncl("traptime", new SomethingSync(typeof(byte)));
            AddSynncl("owner", new SomethingSync(typeof(ushort)));
            tatchedTo.Add(typeof(TrappedDuck));
        }
        public int ushh;
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            TrappedDuckVessel v = new TrappedDuckVessel(new TrappedDuck(0, -2000, null));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TrappedDuck td = (TrappedDuck)t;
        
            td._trapTime = BitCrusher.ByteToFloat((byte)valOf("traptime"), 1);
            td._shakeMult = BitCrusher.ByteToFloat((byte)valOf("shake"), 1);
            int ls = (ushort)valOf("owner") - 1;
            if (ls != -1 && Corderator.instance.somethingMap.Contains(ls))
            {
                td._duckOwner = (Duck)Corderator.instance.somethingMap[ls];
                if (td.y > -1500) td._duckOwner._trapped = td;
                else td._duckOwner = null;
            }
            else
            {
                if (td._duckOwner != null) td._duckOwner._trapped = null;
                td._duckOwner = null;
            }

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            TrappedDuck td = (TrappedDuck)t;
            addVal("traptime", BitCrusher.FloatToByte(td._trapTime, 1));
            addVal("shake", BitCrusher.FloatToByte(td._shakeMult, 1));
            if (td._duckOwner != null && Corderator.instance.somethingMap.Contains(td._duckOwner) && td.y > -9000) addVal("owner", (ushort)(Corderator.instance.somethingMap[td._duckOwner] + 1));
            else addVal("owner", (ushort)0);
            base.RecordUpdate();
        }
    }
}
