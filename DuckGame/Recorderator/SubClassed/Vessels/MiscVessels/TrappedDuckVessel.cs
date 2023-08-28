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
            tatchedTo.Add(typeof(TrappedDuck));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            int ud = b.ReadUShort() - 1;
            Duck d = null;
            if (Corderator.instance.somethingMap.ContainsKey(ud))
            {
                d = (Duck)Corderator.instance.somethingMap[ud];
            }
            TrappedDuckVessel v = new TrappedDuckVessel(new TrappedDuck(0, -2000, d));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            TrappedDuck td = (TrappedDuck)t;
            if (td.duck != null && Corderator.instance.somethingMap.Contains(td.duck)) prevBuffer.Write((ushort)(Corderator.instance.somethingMap[td.duck] + 1));
            else prevBuffer.Write((ushort)0);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TrappedDuck td = (TrappedDuck)t;
            td._trapTime = BitCrusher.ByteToFloat((byte)valOf("traptime"), 1);
            td._shakeMult = BitCrusher.ByteToFloat((byte)valOf("shake"), 1);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            TrappedDuck td = (TrappedDuck)t;
            addVal("traptime", BitCrusher.FloatToByte(td._trapTime, 1));
            addVal("shake", BitCrusher.FloatToByte(td._shakeMult, 1));
            base.RecordUpdate();
        }
    }
}
