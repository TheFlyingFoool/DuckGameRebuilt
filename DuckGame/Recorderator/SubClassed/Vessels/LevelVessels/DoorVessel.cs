using System.Collections;

namespace DuckGame
{
    public class DoorVessel : SomethingSomethingVessel
    {
        public DoorVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Door));
            tatchedTo.Add(typeof(FlimsyDoor));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            AddSynncl("hitpoints", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 position = b.ReadVec2();
            byte by = b.ReadByte();
            Vec2 vv = b.ReadVec2();
            BitArray br = new BitArray(new byte[] { by });
            bool isLocked = br[0];
            bool flimsy = br[1];
            if (flimsy)
            {
                DoorVessel v = new DoorVessel(new FlimsyDoor(position.x, position.y));
                ((Door)v.t).locked = isLocked;
                ((Door)v.t).ps = vv;
                return v;
            }
            else
            {
                DoorVessel v = new DoorVessel(new Door(position.x, position.y));
                ((Door)v.t).locked = isLocked;
                ((Door)v.t).ps = vv;
                return v;
            }
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(t.position);
            BitArray br = new BitArray(8);
            br[0] = ((Door)t)._lockDoor;
            br[1] = ((Door)t).secondaryFrame;
            prevBuffer.Write(BitCrusher.BitArrayToByte(br));
            prevBuffer.Write(((Door)t).ps);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Door d = (Door)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed")});
            d.locked = br[0];
            if (!d.didUnlock && br[1])
            {
                d.DoUnlock(d.ps);
            }

            d._hitPoints = (float)valOf("hitpoints");
            d.active = true;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Door d = (Door)t;
            BitArray br = new BitArray(8);
            br[0] = d.locked;
            br[1] = d.didUnlock;

            addVal("infoed", BitCrusher.BitArrayToByte(br));
            addVal("hitpoints", d._hitPoints);
        }
    }
}
