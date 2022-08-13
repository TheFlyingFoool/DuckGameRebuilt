// Decompiled with JetBrains decompiler
// Type: DuckGame.NMParticlesRemoved
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMParticlesRemoved : NetMessage
    {
        public new byte levelIndex;
        public HashSet<ushort> removeParticles = new HashSet<ushort>();

        public NMParticlesRemoved()
        {
            manager = BelongsToManager.GhostManager;
            levelIndex = DuckNetwork.levelIndex;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMParticlesRemoved).removeParticles = removeParticles;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            if (removeParticles.Count > byte.MaxValue)
                throw new Exception("NMParticlesRemoved.removeParticles should not have more than 255 particles.");
            _serializedData.Write(levelIndex);
            _serializedData.Write((byte)removeParticles.Count);
            foreach (ushort removeParticle in removeParticles)
                _serializedData.Write(removeParticle);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                removeParticles.Add(d.ReadUShort());
        }
    }
}
