using System;
using System.Collections;
using System.Linq;

namespace DuckGame
{
    public class RagdollVessel : HoldableVessel
    {
        public RagdollVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_h");
            RemoveSynncl("angledeg");
            AddSynncl("owner", new SomethingSync(typeof(ushort)));
            AddSynncl("p1", new SomethingSync(typeof(ushort)));
            AddSynncl("p2", new SomethingSync(typeof(ushort)));
            AddSynncl("p3", new SomethingSync(typeof(ushort)));
            AddSynncl("infoed", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(Ragdoll));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            byte persona = b.ReadByte();
            RagdollVessel v = new RagdollVessel(new Ragdoll(0, -2000, null, false, 0, 0, Vec2.Zero, Persona.all.ElementAt(persona)));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write((byte)((Ragdoll)t)._duck.persona.index);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {

            Ragdoll rd = (Ragdoll)t;
            int owner = (ushort)valOf("owner") - 1;
            if (owner != -1)
            {
                rd._duck = (Duck)Corderator.instance.somethingMap[owner];
                rd._duck._ragdollInstance = rd;
                if (rd.y > -8000) rd._duck.ragdoll = rd;
                else rd._duck.ragdoll = null;
            }


            int p1 = (ushort)valOf("p1") - 1;
            if (p1 != -1)
            {
                rd.part1 = (RagdollPart)Corderator.instance.somethingMap[p1];
            }
            int p2 = (ushort)valOf("p2") - 1;
            if (p2 != -1)
            {
                rd.part2 = (RagdollPart)Corderator.instance.somethingMap[p2];
            }
            int p3 = (ushort)valOf("p3") - 1;
            if (p3 != -1)
            {
                rd.part3 = (RagdollPart)Corderator.instance.somethingMap[p3];
            }
            rd.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);

            BitArray br = new BitArray(new byte[] { (byte)valOf("infoed") });
            rd.inSleepingBag = br[0];
            DoUpdateThing();
        }
        public override void RecordUpdate()
        {
            Ragdoll rd = (Ragdoll)t;
            if (rd._duck != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd._duck)) addVal("owner", (ushort)(Corderator.instance.somethingMap[rd._duck] + 1));
            else addVal("owner", (ushort)0);

            if (rd.part1 != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd.part1)) addVal("p1", (ushort)(Corderator.instance.somethingMap[rd.part1] + 1));
            else addVal("p1", (ushort)0);
            if (rd.part1 != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd.part2)) addVal("p2", (ushort)(Corderator.instance.somethingMap[rd.part2] + 1));
            else addVal("p2", (ushort)0);
            if (rd.part1 != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rd.part3)) addVal("p3", (ushort)(Corderator.instance.somethingMap[rd.part3] + 1));
            else addVal("p3", (ushort)0);

            addVal("position", CompressedVec2Binding.GetCompressedVec2(rd.position, 10000));

            BitArray br = new BitArray(8);
            br[0] = rd.inSleepingBag;

            addVal("infoed", BitCrusher.BitArrayToByte(br));
        }
    }
}
