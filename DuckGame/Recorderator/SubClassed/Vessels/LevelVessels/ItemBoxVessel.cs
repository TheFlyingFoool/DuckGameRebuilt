using System.Linq;

namespace DuckGame
{
    public class ItemBoxVessel : SomethingSomethingVessel
    {
        public ItemBoxVessel(Thing th) : base(th)
        {
            somethingCrash = "???";
            tatchedTo.Add(typeof(ItemBox));
            tatchedTo.Add(typeof(ItemBoxOneTime));
            tatchedTo.Add(typeof(ItemBoxRandom));
            tatchedTo.Add(typeof(PurpleBlock));

            AddSynncl("containing", new SomethingSync(typeof(ushort)));
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            AddSynncl("_hit", new SomethingSync(typeof(bool)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            ushort s = b.ReadUShort();
            Thing t = null;
            try
            {
                t = Editor.CreateThing(Editor.IDToType[s]);
            }
            catch
            {
                destroyedReason = "Couldn't create itembox " + s;
                DevConsole.Log("Itembox couldn't create thing " + s);
            }
            ItemBoxVessel v = new ItemBoxVessel(t);
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            //can optimize this to a single byte because theres only a few itembox types
            prevBuffer.Write(Editor.IDToType[t.GetType()]);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            ItemBox i = (ItemBox)t;
            i.position = (Vec2)valOf("position");
            i._hit = (bool)valOf("_hit");
            int hObj = (ushort)valOf("containing") - 1;
            if (hObj == -1 && i.containedObject != null)
            {
                i.containedObject.visible = true;
                i.containedObject = null;
            }
            else if (hObj != -1 && i.containedObject == null && Corderator.instance.somethingMap.Contains(hObj))
            {
                if (Corderator.instance == null) Main.SpecialCode = "corderator was null";
                i.containedObject = (PhysicsObject)Corderator.instance.somethingMap[hObj];
                i.containedObject.visible = false;
            }
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            ItemBox i = (ItemBox)t;
            if (i.containedObject != null)
            {
                if (Corderator.instance.somethingMap.Contains(i.containedObject))
                {
                    addVal("containing", (ushort)(Corderator.instance.somethingMap[i.containedObject] + 1));
                }
                else addVal("containing", (ushort)0);
            }
            else addVal("containing", (ushort)0);
            addVal("position", i.position);
            addVal("_hit", i._hit);
        }
    }
}
