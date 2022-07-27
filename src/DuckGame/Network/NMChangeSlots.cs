// Decompiled with JetBrains decompiler
// Type: DuckGame.NMChangeSlots
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [FixedNetworkID(30231)]
    public class NMChangeSlots : NMDuckNetworkEvent
    {
        public List<byte> slots = new List<byte>();
        public bool originalConfiguration;

        public NMChangeSlots()
        {
        }

        public NMChangeSlots(List<byte> pSlots, bool pOriginalConfiguration)
        {
            this.slots = pSlots;
            this.originalConfiguration = pOriginalConfiguration;
        }

        protected override void OnSerialize()
        {
            this._serializedData.Write(this.originalConfiguration);
            this._serializedData.Write((byte)this.slots.Count);
            for (int index = 0; index < this.slots.Count; ++index)
                this._serializedData.Write(this.slots[index]);
            base.OnSerialize();
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            this.originalConfiguration = msg.ReadBool();
            this.slots = new List<byte>();
            byte num = msg.ReadByte();
            for (int index = 0; index < num; ++index)
                this.slots.Add(msg.ReadByte());
            base.OnDeserialize(msg);
        }

        public override void Activate()
        {
            if (!Network.isServer)
            {
                int index = 0;
                foreach (int slot in this.slots)
                {
                    if (index < DuckNetwork.profiles.Count)
                    {
                        DuckNetwork.profiles[index].slotType = (SlotType)slot;
                        if (this.originalConfiguration && index < DG.MaxPlayers)
                            DuckNetwork.profiles[index].originalSlotType = (SlotType)slot;
                    }
                    ++index;
                }
            }
            base.Activate();
        }
    }
}
