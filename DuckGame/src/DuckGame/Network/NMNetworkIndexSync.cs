// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNetworkIndexSync
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
