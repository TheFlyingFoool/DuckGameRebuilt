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
            if (!particles.TryGetValue(netTypeIndex, out physicsParticleList))
                physicsParticleList = particles[netTypeIndex] = new List<PhysicsParticle>();
            physicsParticleList.Add(pParticle);
        }

        public NMParticles()
        {
            manager = BelongsToManager.GhostManager;
            levelIndex = DuckNetwork.levelIndex;
        }

        public override void CopyTo(NetMessage pMessage)
        {
            (pMessage as NMParticles).particles = particles;
            base.CopyTo(pMessage);
        }

        protected override void OnSerialize()
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(levelIndex);
            foreach (KeyValuePair<byte, List<PhysicsParticle>> particle in particles)
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
            _serializedData.Write(bitBuffer, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            data = d.ReadBitBuffer();
            levelIndex = data.ReadByte();
        }
    }
}
