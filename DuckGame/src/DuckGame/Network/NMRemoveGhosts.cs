using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMRemoveGhosts : NetMessage
    {
        public List<NetIndex16> remove = new List<NetIndex16>();
        public new byte levelIndex;
        protected GhostManager _manager;

        public NMRemoveGhosts() => manager = BelongsToManager.GhostManager;

        public NMRemoveGhosts(GhostManager pManager)
        {
            levelIndex = DuckNetwork.levelIndex;
            manager = BelongsToManager.GhostManager;
            _manager = pManager;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMRemoveGhosts).remove = remove;
            (pMessage as NMRemoveGhosts).levelIndex = levelIndex;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            byte val = (byte)Math.Min(32, _manager._destroyedGhosts.Count + _manager._destroyResends.Count);
            _serializedData.Write(levelIndex);
            _serializedData.Write(val);
            for (int index = 0; index < _manager._destroyResends.Count && val != 0; index = index - 1 + 1)
            {
                _serializedData.Write((ushort)(int)_manager._destroyResends[index]);
                remove.Add(_manager._destroyResends[index]);
                _manager._destroyResends.RemoveAt(index);
                --val;
            }
            for (int index = 0; index < _manager._destroyedGhosts.Count && val != 0; index = index - 1 + 1)
            {
                _serializedData.Write((ushort)(int)_manager._destroyedGhosts[index].ghostObjectIndex);
                remove.Add(_manager._destroyedGhosts[index].ghostObjectIndex);
                _manager._destroyedGhosts.RemoveAt(index);
                --val;
            }
        }

        public override void OnDeserialize(BitBuffer pData)
        {
            levelIndex = pData.ReadByte();
            ushort num = pData.ReadByte();
            for (int index = 0; index < num; ++index)
                remove.Add((NetIndex16)pData.ReadUShort());
        }
    }
}
