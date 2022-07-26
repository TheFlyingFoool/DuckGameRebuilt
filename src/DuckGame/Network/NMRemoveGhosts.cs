// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRemoveGhosts
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMRemoveGhosts : NetMessage
    {
        public List<NetIndex16> remove = new List<NetIndex16>();
        public new byte levelIndex;
        protected GhostManager _manager;

        public NMRemoveGhosts() => this.manager = BelongsToManager.GhostManager;

        public NMRemoveGhosts(GhostManager pManager)
        {
            this.levelIndex = DuckNetwork.levelIndex;
            this.manager = BelongsToManager.GhostManager;
            this._manager = pManager;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMRemoveGhosts).remove = this.remove;
            (pMessage as NMRemoveGhosts).levelIndex = this.levelIndex;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            byte val = (byte)Math.Min(32, this._manager._destroyedGhosts.Count + this._manager._destroyResends.Count);
            this._serializedData.Write(this.levelIndex);
            this._serializedData.Write(val);
            for (int index = 0; index < this._manager._destroyResends.Count && val != (byte)0; index = index - 1 + 1)
            {
                this._serializedData.Write((ushort)(int)this._manager._destroyResends[index]);
                this.remove.Add(this._manager._destroyResends[index]);
                this._manager._destroyResends.RemoveAt(index);
                --val;
            }
            for (int index = 0; index < this._manager._destroyedGhosts.Count && val != (byte)0; index = index - 1 + 1)
            {
                this._serializedData.Write((ushort)(int)this._manager._destroyedGhosts[index].ghostObjectIndex);
                this.remove.Add(this._manager._destroyedGhosts[index].ghostObjectIndex);
                this._manager._destroyedGhosts.RemoveAt(index);
                --val;
            }
        }

        public override void OnDeserialize(BitBuffer pData)
        {
            this.levelIndex = pData.ReadByte();
            ushort num = (ushort)pData.ReadByte();
            for (int index = 0; index < (int)num; ++index)
                this.remove.Add((NetIndex16)(int)pData.ReadUShort());
        }
    }
}
