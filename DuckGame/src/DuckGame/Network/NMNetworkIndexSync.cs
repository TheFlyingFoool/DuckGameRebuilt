using System.Collections.Generic;

namespace DuckGame
{
    public class NMNetworkIndexSync : NMEvent
    {
        public List<byte> indexes = new List<byte>();

        protected override void OnSerialize()
        {
            for (int index = 0; index < DuckNetwork.profiles.Count; ++index)
                _serializedData.Write(DuckNetwork.profiles[index].fixedGhostIndex);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            for (int index = 0; index < DuckNetwork.profiles.Count; ++index)
                indexes.Add(msg.ReadByte());
        }

        public override void Activate()
        {
            for (int index = 0; index < indexes.Count; ++index)
                DuckNetwork.profiles[index].SetFixedGhostIndex(indexes[index]);
            DuckNetwork.core.ReorderFixedList();
        }
    }
}
