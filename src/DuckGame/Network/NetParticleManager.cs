// Decompiled with JetBrains decompiler
// Type: DuckGame.NetParticleManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            ++this._nextParticleIndex;
            if (this._nextParticleIndex > (ushort)4000)
                this.ResetParticleIndex();
            return this._nextParticleIndex;
        }

        public void ResetParticleIndex() => this._nextParticleIndex = (ushort)1;

        private void ClearParticle(PhysicsParticle pParticle)
        {
            this._particles.Remove(pParticle.netIndex);
            pParticle.netRemove = true;
            Level.Remove((Thing)pParticle);
            this.removedParticleIndexes.Add(pParticle.netIndex);
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
                            PhysicsParticle pParticle = (PhysicsParticle)null;
                            if (this._particles.TryGetValue(current, out pParticle))
                                this.ClearParticle(pParticle);
                        }
                        break;
                    }
                case NMParticles _:
                    ushort num1 = 0;
                    this._lastPacketNumbers.TryGetValue(m.packet.connection, out num1);
                    this._lastPacketNumbers[m.packet.connection] = m.packet.order;
                    bool flag = Math.Abs((int)num1 - (int)m.packet.order) < 1000 && (int)m.packet.order < (int)num1;
                    NMParticles nmParticles = m as NMParticles;
                    if ((int)nmParticles.levelIndex != (int)DuckNetwork.levelIndex)
                        break;
                    label_9:
                    byte pNetType = nmParticles.data.ReadByte();
                    if (pNetType == byte.MaxValue)
                        break;
                    System.Type typeIndex = PhysicsParticle.NetTypeToTypeIndex(pNetType);
                    byte num2 = nmParticles.data.ReadByte();
                    for (int index1 = 0; index1 < (int)num2; ++index1)
                    {
                        ushort key = nmParticles.data.ReadUShort();
                        PhysicsParticle physicsParticle = (PhysicsParticle)null;
                        if (!this._particles.TryGetValue(key, out physicsParticle) | flag && typeIndex != (System.Type)null)
                        {
                            if (typeIndex == typeof(SmallFire))
                                physicsParticle = (PhysicsParticle)SmallFire.New(Vec2.NetMin.x, Vec2.NetMin.y, 0.0f, 0.0f, canMultiply: false, network: true);
                            else if (typeIndex == typeof(ExtinguisherSmoke))
                                physicsParticle = (PhysicsParticle)new ExtinguisherSmoke(Vec2.NetMin.x, Vec2.NetMin.y, true);
                            else if (typeIndex == typeof(Firecracker))
                                physicsParticle = (PhysicsParticle)new Firecracker(Vec2.NetMin.x, Vec2.NetMin.y, true);
                            if (!flag)
                            {
                                physicsParticle.netIndex = key;
                                physicsParticle.isLocal = false;
                                if (this.removedParticleIndexes.Count > 3000)
                                {
                                    for (int index2 = 0; index2 < 10; ++index2)
                                    {
                                        if (this.removedParticleIndexes.Contains((ushort)((uint)key - (uint)index2)))
                                            this.removedParticleIndexes.Remove((ushort)((uint)key - (uint)index2));
                                    }
                                }
                                if (!this.removedParticleIndexes.Contains(key))
                                {
                                    if (this._particles.Count > 200)
                                    {
                                        PhysicsParticle pParticle = (PhysicsParticle)null;
                                        if (this._particles.TryGetValue((ushort)((uint)key - 100U), out pParticle))
                                            this.ClearParticle(pParticle);
                                    }
                                    this._particles[key] = physicsParticle;
                                    Level.Add((Thing)physicsParticle);
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
            p.netIndex = (ushort)((uint)this.GetParticleIndex() + (uint)DuckNetwork.localProfile.networkIndex * 4000U);
            this._particles[p.netIndex] = p;
            if (this._particles.Count <= 200)
                return;
            PhysicsParticle p1 = (PhysicsParticle)null;
            if (!this._particles.TryGetValue((ushort)((uint)p.netIndex - 100U), out p1))
                return;
            this.RemoveParticle(p1);
            Level.Remove((Thing)p1);
        }

        public void Clear()
        {
            this._particles.Clear();
            this._inProgressParticleLists.Clear();
            this._pendingParticles.Clear();
            this.removedParticleIndexes.Clear();
            this.ResetParticleIndex();
            this.updateOrder = (byte)0;
        }

        public List<PhysicsParticle> GetParticleList(System.Type t)
        {
            List<PhysicsParticle> particleList = (List<PhysicsParticle>)null;
            if (!this._inProgressParticleLists.TryGetValue(t, out particleList) || particleList.Count >= 20)
            {
                particleList = new List<PhysicsParticle>();
                this._inProgressParticleLists[t] = particleList;
                this._pendingParticles.Enqueue(particleList);
            }
            return particleList;
        }

        public void RemoveParticle(PhysicsParticle p) => p.netRemove = true;

        public void Notify(NetMessage m, bool dropped)
        {
            if (!dropped || !(m is NMParticlesRemoved) || (int)(m as NMParticlesRemoved).levelIndex != (int)DuckNetwork.levelIndex)
                return;
            Send.Message((NetMessage)new NMParticlesRemoved()
            {
                removeParticles = (m as NMParticlesRemoved).removeParticles
            }, NetMessagePriority.Volatile, m.connection);
        }

        public void Update()
        {
            List<PhysicsParticle> physicsParticleList = (List<PhysicsParticle>)null;
            int num1 = 0;
            while (true)
            {
                int num2 = num1;
                int num3 = 0;
                foreach (KeyValuePair<ushort, PhysicsParticle> particle in this._particles)
                {
                    PhysicsParticle pParticle = particle.Value;
                    if (pParticle.isLocal)
                    {
                        if (pParticle.netRemove)
                        {
                            if (physicsParticleList == null)
                                physicsParticleList = new List<PhysicsParticle>();
                            this.removedParticleIndexes.Add(pParticle.netIndex);
                            physicsParticleList.Add(pParticle);
                        }
                        else
                        {
                            ++num3;
                            if ((int)pParticle.updateOrder != (int)this.updateOrder)
                            {
                                pParticle.updateOrder = this.updateOrder;
                                this.currentParticleList.Add(pParticle);
                                ++num1;
                            }
                        }
                    }
                    else if (pParticle.netRemove)
                    {
                        if (physicsParticleList == null)
                            physicsParticleList = new List<PhysicsParticle>();
                        this.removedParticleIndexes.Add(pParticle.netIndex);
                        physicsParticleList.Add(pParticle);
                        Level.Remove((Thing)pParticle);
                    }
                    if (num1 > 30)
                        break;
                }
                if (num1 == num2 && num3 > 0)
                    ++this.updateOrder;
                else
                    break;
            }
            if (this.currentParticleList.particles.Count > 0)
            {
                Send.Message((NetMessage)this.currentParticleList, NetMessagePriority.Volatile);
                this.currentParticleList = new NMParticles();
            }
            if (physicsParticleList != null)
            {
                NMParticlesRemoved particlesRemoved = new NMParticlesRemoved();
                this._particleRemoveMessages.Enqueue(particlesRemoved);
                foreach (PhysicsParticle physicsParticle in physicsParticleList)
                {
                    if (particlesRemoved.removeParticles.Count >= 32)
                    {
                        particlesRemoved = new NMParticlesRemoved();
                        this._particleRemoveMessages.Enqueue(particlesRemoved);
                    }
                    particlesRemoved.removeParticles.Add(physicsParticle.netIndex);
                    this._particles.Remove(physicsParticle.netIndex);
                }
                physicsParticleList.Clear();
            }
            if (this._particleRemoveMessages.Count <= 0)
                return;
            Send.Message((NetMessage)this._particleRemoveMessages.Dequeue(), NetMessagePriority.Volatile);
        }
    }
}
