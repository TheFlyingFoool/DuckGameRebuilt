// Decompiled with JetBrains decompiler
// Type: DuckGame.NMParticlesRemoved
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.manager = BelongsToManager.GhostManager;
            this.levelIndex = DuckNetwork.levelIndex;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMParticlesRemoved).removeParticles = this.removeParticles;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            if (this.removeParticles.Count > byte.MaxValue)
                throw new Exception("NMParticlesRemoved.removeParticles should not have more than 255 particles.");
            this._serializedData.Write(this.levelIndex);
            this._serializedData.Write((byte)this.removeParticles.Count);
            foreach (ushort removeParticle in this.removeParticles)
                this._serializedData.Write(removeParticle);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this.levelIndex = d.ReadByte();
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                this.removeParticles.Add(d.ReadUShort());
        }
    }
}
