using System.Collections;

namespace DuckGame
{
    public class DoorVessel : SomethingSomethingVessel
    {
        public DoorVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Door));
            tatchedTo.Add(typeof(FlimsyDoor));
            AddSynncl("locked", new SomethingSync(typeof(bool)));
            AddSynncl("hitpoints", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 position = b.ReadVec2();
            byte by = b.ReadByte();
            BitArray br = new BitArray(new byte[] { by });
            bool isLocked = br[0];
            bool flimsy = br[1];
            if (flimsy)
            {
                DoorVessel v = new DoorVessel(new FlimsyDoor(position.x, position.y));
                ((Door)v.t).locked = isLocked;
                return v;
            }
            else
            {
                DoorVessel v = new DoorVessel(new Door(position.x, position.y));
                ((Door)v.t).locked = isLocked;
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
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Door d = (Door)t;
            d.locked = (bool)valOf("locked");
            d._hitPoints = (float)valOf("hitpoints");
            d.active = true;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Door d = (Door)t;
            addVal("locked", d.locked);
            addVal("hitpoints", d._hitPoints);
        }
    }
}
