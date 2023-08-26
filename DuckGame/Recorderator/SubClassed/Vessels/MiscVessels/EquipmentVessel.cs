//wrote this on phone
namespace DuckGame
{
    public class EquipmentVessel : HoldableVessel
    {
        public EquipmentVessel(Thing th) : base(th)
        {
            AddSynncl("equipped", new SomethingSync(typeof(int)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            EquipmentVessel v = new EquipmentVessel(Editor.CreateThing(Editor.IDToType[b.ReadUShort()]));
            v.t.y = -2000;
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
             prevBuffer.Write(Editor.IDToType[t.GetType()]);
             return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Equipment e = (Equipment)t;
            int hObj = (int)valOf("equipped");
            if (hObj == -1)
            {
                if (e._equippedDuck != null)
                {
                    e._equippedDuck.Unequip(e);
                }
                e._equippedDuck = null;
            }
            else if (hObj != -1 && Corderator.instance.somethingMap.Contains(hObj))
            {
                Duck d = (Duck)Corderator.instance.somethingMap[hObj];
                if (e._equippedDuck == null)
                {
                    d.Equip(e, false);
                }
                e._equippedDuck = d;
            }
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Equipment e = (Equipment)t;
            if (e._equippedDuck != null)
            {
                if (Corderator.instance.somethingMap.Contains(e._equippedDuck))
                {
                    addVal("equipped", Corderator.instance.somethingMap[e._equippedDuck]);
                }
                else addVal("equipped", -1);
            }
            else addVal("equipped", -1);
            base.RecordUpdate();
        }
    }
}
