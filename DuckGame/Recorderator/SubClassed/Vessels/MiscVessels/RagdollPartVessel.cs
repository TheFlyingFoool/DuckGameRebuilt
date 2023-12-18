using System.Linq;

namespace DuckGame
{
    public class RagdollPartVessel : HoldableVessel
    {
        public RagdollPartVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_h");
            RemoveSynncl("angledeg");
            AddSynncl("owner", new SomethingSync(typeof(ushort)));
            AddSynncl("joint", new SomethingSync(typeof(ushort)));
            tatchedTo.Add(typeof(RagdollPart));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            byte part = b.ReadByte();
            byte persona = b.ReadByte();
            RagdollPartVessel v = new RagdollPartVessel(new RagdollPart(0, -2000, part, Persona.all.ElementAt(persona), 0, null));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            RagdollPart part = (RagdollPart)t;
            prevBuffer.Write((byte)part.part);
            prevBuffer.Write((byte)part._persona.index);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {

            RagdollPart rd = (RagdollPart)t;
            int owner = (ushort)valOf("owner") - 1;
            if (owner != -1)
            {
                rd.doll = (Ragdoll)Corderator.instance.somethingMap[owner];
            }
            int joint = (ushort)valOf("joint") - 1;//THIS SHIT LACED
            if (joint != -1)
            {
                rd.joint = (RagdollPart)Corderator.instance.somethingMap[joint];
            }

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            RagdollPart rd = (RagdollPart)t;
            if (rd.doll != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd.doll)) addVal("owner", (ushort)(Corderator.instance.somethingMap[rd.doll] + 1));
            else addVal("owner", (ushort)0);
            if (rd.joint != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd.joint)) addVal("joint", (ushort)(Corderator.instance.somethingMap[rd.joint] + 1));
            else addVal("joint", (ushort)0);
            //
            base.RecordUpdate();
        }
    }
}
