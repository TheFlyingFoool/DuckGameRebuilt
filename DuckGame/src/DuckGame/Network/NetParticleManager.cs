// Decompiled with JetBrains decompiler
// Type: DuckGame.NetParticleManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NetParticleManager
    {
        private Dictionary<ushort, PhysicsParticle> _particles = new Dictionary<ushort, PhysicsParticle>();
        private ushort _nextParticleIndex = 1;
        private Dictionary<NetworkConnection, ushort> _lastPacketNumbers = new Dictionary<NetworkConnection, ushort>();
        public HashSet<ushort> removedParticleIndexes = new HashSet<ushort>();
        private Queue<List<PhysicsParticle>> _pendingParticles = new Queue<List<PhysicsParticle>>();
        private Dictionary<System.Type, List<PhysicsParticle>> _inProgressParticleLists = new Dictionary<System.Type, List<PhysicsParticle>>();
        public static int _particleSyncSpread = 2;
        public static int _syncWait = 4;
        private byte updateOrder;
        private Queue<NMParticlesRemoved> _particleRemoveMessages = new Queue<NMParticlesRemoved>();
        private NMParticles currentParticleList = new NMParticles();

        public ushort GetParticleIndex()
        {
            ++_nextParticleIndex;
            if (_nextParticleIndex > 4000)
                ResetParticleIndex();
            return _nextParticleIndex;
        }

        public void ResetParticleIndex() => _nextParticleIndex = 1;

        private void ClearParticle(PhysicsParticle pParticle)
        {
            _particles.Remove(pParticle.netIndex);
            pParticle.netRemove = true;
            Level.Remove(pParticle);
            removedParticleIndexes.Add(pParticle.netIndex);
        }

        public void OnMessage(NetMessage m)
        {
            switch (m)
            {
                case NMParticlesRemoved _:
                    using (HashSet<ushort>.Enumerator enumerator = (m as NMParticlesRemoved).removeParticles.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            ushort current = enumerator.Current;
                            PhysicsParticle pParticle;
                            if (_particles.TryGetValue(current, out pParticle))
                                ClearParticle(pParticle);
                        }
                        break;
                    }
                case NMParticles _:
                    ushort num1;
                    _lastPacketNumbers.TryGetValue(m.packet.connection, out num1);
                    _lastPacketNumbers[m.packet.connection] = m.packet.order;
                    bool flag = Math.Abs(num1 - m.packet.order) < 1000 && m.packet.order < num1;
                    NMParticles nmParticles = m as NMParticles;
                    if (nmParticles.levelIndex != DuckNetwork.levelIndex)
                        break;
                    label_9:
                    byte pNetType = nmParticles.data.ReadByte();
                    if (pNetType == byte.MaxValue)
                        break;
                    System.Type typeIndex = PhysicsParticle.NetTypeToTypeIndex(pNetType);
                    byte num2 = nmParticles.data.ReadByte();
                    for (int index1 = 0; index1 < num2; ++index1)
                    {
                        ushort key = nmParticles.data.ReadUShort();
                        PhysicsParticle physicsParticle;
                        if (!_particles.TryGetValue(key, out physicsParticle) | flag && typeIndex != null)
                        {
                            if (typeIndex == typeof(SmallFire))
                                physicsParticle = SmallFire.New(Vec2.NetMin.x, Vec2.NetMin.y, 0f, 0f, canMultiply: false, network: true);
                            else if (typeIndex == typeof(ExtinguisherSmoke))
                                physicsParticle = new ExtinguisherSmoke(Vec2.NetMin.x, Vec2.NetMin.y, true);
                            else if (typeIndex == typeof(Firecracker))
                                physicsParticle = new Firecracker(Vec2.NetMin.x, Vec2.NetMin.y, true);
                            if (!flag)
                            {
                                physicsParticle.netIndex = key;
                                physicsParticle.isLocal = false;
                                if (removedParticleIndexes.Count > 3000)
                                {
                                    for (int index2 = 0; index2 < 10; ++index2)
                                    {
                                        if (removedParticleIndexes.Contains((ushort)(key - (uint)index2)))
                                            removedParticleIndexes.Remove((ushort)(key - (uint)index2));
                                    }
                                }
                                if (!removedParticleIndexes.Contains(key))
                                {
                                    if (_particles.Count > 200)
                                    {
                                        PhysicsParticle pParticle;
                                        if (_particles.TryGetValue((ushort)(key - 100U), out pParticle))
                                            ClearParticle(pParticle);
                                    }
                                    _particles[key] = physicsParticle;
                                    Level.Add(physicsParticle);
                                }
                            }
                        }
                        physicsParticle.NetDeserialize(nmParticles.data);
                    }
                    goto label_9;
            }
        }

        public void AddLocalParticle(PhysicsParticle p)
        {
            if (DuckNetwork.localProfile == null)
                return;
            p.connection = DuckNetwork.localConnection;
            p.netIndex = (ushort)(GetParticleIndex() + DuckNetwork.localProfile.networkIndex * 4000U);
            _particles[p.netIndex] = p;
            if (_particles.Count <= 200)
                return;
            PhysicsParticle p1;
            if (!_particles.TryGetValue((ushort)(p.netIndex - 100U), out p1))
                return;
            RemoveParticle(p1);
            Level.Remove(p1);
        }

        public void Clear()
        {
            _particles.Clear();
            _inProgressParticleLists.Clear();
            _pendingParticles.Clear();
            removedParticleIndexes.Clear();
            ResetParticleIndex();
            updateOrder = 0;
        }

        public List<PhysicsParticle> GetParticleList(System.Type t)
        {
            List<PhysicsParticle> particleList;
            if (!_inProgressParticleLists.TryGetValue(t, out particleList) || particleList.Count >= 20)
            {
                particleList = new List<PhysicsParticle>();
                _inProgressParticleLists[t] = particleList;
                _pendingParticles.Enqueue(particleList);
            }
            return particleList;
        }

        public void RemoveParticle(PhysicsParticle p) => p.netRemove = true;

        public void Notify(NetMessage m, bool dropped)
        {
            if (!dropped || !(m is NMParticlesRemoved) || (m as NMParticlesRemoved).levelIndex != DuckNetwork.levelIndex)
                return;
            Send.Message(new NMParticlesRemoved()
            {
                removeParticles = (m as NMParticlesRemoved).removeParticles
            }, NetMessagePriority.Volatile, m.connection);
        }

        public void Update()
        {
            List<PhysicsParticle> physicsParticleList = null;
            int num1 = 0;
            while (true)
            {
                int num2 = num1;
                int num3 = 0;
                foreach (KeyValuePair<ushort, PhysicsParticle> particle in _particles)
                {
                    PhysicsParticle pParticle = particle.Value;
                    if (pParticle.isLocal)
                    {
                        if (pParticle.netRemove)
                        {
                            if (physicsParticleList == null)
                                physicsParticleList = new List<PhysicsParticle>();
                            removedParticleIndexes.Add(pParticle.netIndex);
                            physicsParticleList.Add(pParticle);
                        }
                        else
                        {
                            ++num3;
                            if (pParticle.updateOrder != updateOrder)
                            {
                                pParticle.updateOrder = updateOrder;
                                currentParticleList.Add(pParticle);
                                ++num1;
                            }
                        }
                    }
                    else if (pParticle.netRemove)
                    {
                        if (physicsParticleList == null)
                            physicsParticleList = new List<PhysicsParticle>();
                        removedParticleIndexes.Add(pParticle.netIndex);
                        physicsParticleList.Add(pParticle);
                        Level.Remove(pParticle);
                    }
                    if (num1 > 30)
                        break;
                }
                if (num1 == num2 && num3 > 0)
                    ++updateOrder;
                else
                    break;
            }
            if (currentParticleList.particles.Count > 0)
            {
                Send.Message(currentParticleList, NetMessagePriority.Volatile);
                currentParticleList = new NMParticles();
            }
            if (physicsParticleList != null)
            {
                NMParticlesRemoved particlesRemoved = new NMParticlesRemoved();
                _particleRemoveMessages.Enqueue(particlesRemoved);
                foreach (PhysicsParticle physicsParticle in physicsParticleList)
                {
                    if (particlesRemoved.removeParticles.Count >= 32)
                    {
                        particlesRemoved = new NMParticlesRemoved();
                        _particleRemoveMessages.Enqueue(particlesRemoved);
                    }
                    particlesRemoved.removeParticles.Add(physicsParticle.netIndex);
                    _particles.Remove(physicsParticle.netIndex);
                }
                physicsParticleList.Clear();
            }
            if (_particleRemoveMessages.Count <= 0)
                return;
            Send.Message(_particleRemoveMessages.Dequeue(), NetMessagePriority.Volatile);
        }
    }
}
