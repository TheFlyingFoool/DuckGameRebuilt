// Decompiled with JetBrains decompiler
// Type: DuckGame.NMParticles
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMParticles : NetMessage
    {
        public Dictionary<byte, List<PhysicsParticle>> particles = new Dictionary<byte, List<PhysicsParticle>>();
        public new byte levelIndex;
        public byte count;
        public System.Type type;
        public BitBuffer data;

        public void Add(PhysicsParticle pParticle)
        {
            byte netTypeIndex = PhysicsParticle.TypeToNetTypeIndex(pParticle.GetType());
            List<PhysicsParticle> physicsParticleList;
            if (!this.particles.TryGetValue(netTypeIndex, out physicsParticleList))
                physicsParticleList = this.particles[netTypeIndex] = new List<PhysicsParticle>();
            physicsParticleList.Add(pParticle);
        }

        public NMParticles()
        {
            this.manager = BelongsToManager.GhostManager;
            this.levelIndex = DuckNetwork.levelIndex;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMParticles).particles = this.particles;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(this.levelIndex);
            foreach (KeyValuePair<byte, List<PhysicsParticle>> particle in this.particles)
            {
                bitBuffer.Write(particle.Key);
                bitBuffer.Write((byte)particle.Value.Count);
                foreach (PhysicsParticle physicsParticle in particle.Value)
                {
                    bitBuffer.Write(physicsParticle.netIndex);
                    physicsParticle.NetSerialize(bitBuffer);
                }
            }
            bitBuffer.Write(byte.MaxValue);
            this._serializedData.Write(bitBuffer, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this.data = d.ReadBitBuffer();
            this.levelIndex = this.data.ReadByte();
        }
    }
}
