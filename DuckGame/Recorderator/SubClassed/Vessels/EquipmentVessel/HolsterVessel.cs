using System.Collections;

namespace DuckGame
{
    public class HolsterVessel : EquipmentVessel
    {
        public HolsterVessel(Thing th) : base(th)
        {
            AddSynncl("hold", new SomethingSync(typeof(ushort)));//x
            tatchedTo.Add(typeof(Holster));
            tatchedTo.Add(typeof(PowerHolster));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            BitArray br = new BitArray(new byte[] { b.ReadByte() });
            Holster h;
            if (br[0]) h = new PowerHolster(0, -2000);
            else h = new Holster(0, -2000);
            //DevConsole.Log("is ph " + br[0]);

            h.chained = br[1];
            HolsterVessel v = new HolsterVessel(h);
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            BitArray br = new BitArray(8);
            br[0] = t is PowerHolster;
            br[1] = ((Holster)t).chained;
            prevBuffer.Write(BitCrusher.BitArrayToByte(br));
            return prevBuffer;
        }
        public Holdable lastHold;
        public override void DoUpdateThing()
        {
            base.DoUpdateThing();
        }
        public override void PlaybackUpdate()
        {
            Holster h = (Holster)t;

            if (h._equippedDuck != null && h is PowerHolster ph)
            {
                ph.trigger = h._equippedDuck.quack > 0;
                //DevConsole.Log(ph.trigger);
            }

            int hObj = ((ushort)valOf("hold")) - 1;
            //DevConsole.Log(hObj);
            if (hObj == -1 && lastHold != null) lastHold.owner = null;
            else if (hObj != -1 && lastHold == null && Corderator.instance.somethingMap.Contains(hObj)) h.containedObject = (Holdable)Corderator.instance.somethingMap[hObj];
            if (h.containedObject != null) h.containedObject.owner = h;

            //DevConsole.Log(h.containedObject != null);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Holster h = (Holster)t;
            if (h.containedObject != null && Corderator.instance.somethingMap.Contains(h.containedObject)) addVal("hold", (ushort)(Corderator.instance.somethingMap[h.containedObject] + 1));
            else addVal("hold", (ushort)0);
            base.RecordUpdate();
        }
    }
}
