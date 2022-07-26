// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostObject
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    [DebuggerDisplay("Thing = {_thing}")]
    public class GhostObject
    {
        public byte removeLogCooldown;
        public Vec2 prevPosition;
        public float prevRotation;
        private Dictionary<NetworkConnection, GhostConnectionData> _connectionData = new Dictionary<NetworkConnection, GhostConnectionData>();
        public long lastWrittenMask;
        public int oldGhostTicks;
        private BitBuffer _body = new BitBuffer();
        public bool destroyMessageSent;
        public bool permaOldGhost;
        private NetIndex16 _ghostObjectIndex;
        public bool didDestroyRefresh;
        private BufferedGhostState _networkState;
        private int _finalPositionSyncFrames = 3;
        private long tickIncFrame;
        public bool wrote;
        public BitBuffer previouslySerializedData;
        public List<BufferedGhostState> _stateTimeline = new List<BufferedGhostState>();
        public long[] _last3StateMasks = new long[3];
        public int _last3MaskInc;
        private int delay;
        public BufferedGhostProperty netPositionProperty;
        public BufferedGhostProperty netVelocityProperty;
        public BufferedGhostProperty netAngleProperty;
        public bool isOldGhost;
        private bool _shouldRemove;
        private byte _storedInputStates;
        private ushort[] _inputStates = new ushort[NetworkConnection.packetsEvery];
        private int framesSinceRequestInitialize = 999;
        private int _stateTimelineIndex;
        private const int kStateHistoryLength = 360;
        public static GhostObject applyContext;
        private Thing _prevOwner;
        private Thing _thing;
        private bool initializedCached;
        private List<StateBinding> _fields = new List<StateBinding>();
        private GhostManager _manager;
        public ITakeInput _inputObject;

        private static BufferedGhostProperty MakeBufferedProperty(
          StateBinding state,
          object value,
          int index = 0,
          NetIndex16 tick = default(NetIndex16))
        {
            if (state.type == typeof(float))
            {
                return new BufferedGhostProperty<float>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(bool))
            {
                return new BufferedGhostProperty<bool>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(byte))
            {
                return new BufferedGhostProperty<byte>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(ushort))
            {
                return new BufferedGhostProperty<ushort>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(short))
            {
                return new BufferedGhostProperty<short>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state is CompressedVec2Binding || state is InterpolatedVec2Binding)
            {
                return new BufferedGhostProperty<Vec2>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state is CompressedFloatBinding)
            {
                return new BufferedGhostProperty<float>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(int))
            {
                return new BufferedGhostProperty<int>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(Vec2))
            {
                return new BufferedGhostProperty<Vec2>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(NetIndex4))
            {
                return new BufferedGhostProperty<NetIndex4>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(NetIndex8))
            {
                return new BufferedGhostProperty<NetIndex8>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(NetIndex16))
            {
                return new BufferedGhostProperty<NetIndex16>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            if (state.type == typeof(sbyte))
            {
                return new BufferedGhostProperty<sbyte>
                {
                    binding = state,
                    value = value,
                    index = index,
                    tick = tick
                };
            }
            return new BufferedGhostProperty<object>
            {
                binding = state,
                value = value,
                index = index,
                tick = tick
            };
        }

        public GhostConnectionData GetConnectionData(NetworkConnection c)
        {
            if (c == null)
                return (GhostConnectionData)null;
            GhostConnectionData connectionData = (GhostConnectionData)null;
            if (!this._connectionData.TryGetValue(c, out connectionData))
            {
                connectionData = new GhostConnectionData();
                this._connectionData[c] = connectionData;
            }
            return connectionData;
        }

        public void ClearConnectionData(NetworkConnection c)
        {
            if (!this._connectionData.ContainsKey(c))
                return;
            this._connectionData.Remove(c);
        }

        private void WriteMinimalStateMask(GhostConnectionData dat, BitBuffer b)
        {
            this.lastWrittenMask = dat.connectionStateMask;
            int count = this._fields.Count;
            if (count <= 8)
                b.Write((byte)this.lastWrittenMask);
            else if (count <= 16)
                b.Write((ushort)this.lastWrittenMask);
            else if (count <= 32)
                b.Write((uint)this.lastWrittenMask);
            else
                b.Write(this.lastWrittenMask);
        }

        public static long ReadMinimalStateMask(System.Type t, BitBuffer b) => b.ReadBits<long>(Editor.AllStateFields[t].Length);

        private object GetMinimalStateMask(NetworkConnection c)
        {
            long connectionStateMask = this.GetConnectionData(c).connectionStateMask;
            int count = this._fields.Count;
            if (count <= 8)
                return (object)(byte)connectionStateMask;
            if (count <= 16)
                return (object)(short)connectionStateMask;
            return count <= 32 ? (object)(int)connectionStateMask : (object)connectionStateMask;
        }

        public static long ReadMask(System.Type t, BitBuffer b)
        {
            int length = Editor.AllStateFields[t].Length;
            if (length <= 8)
                return (long)b.ReadByte();
            if (length <= 16)
                return (long)b.ReadUShort();
            return length <= 32 ? (long)b.ReadUInt() : b.ReadLong();
        }

        public static bool MaskIsMaxValue(System.Type t, long mask)
        {
            int length = Editor.AllStateFields[t].Length;
            if (length <= 8)
                return mask == (long)byte.MaxValue;
            if (length <= 16)
                return mask == (long)short.MaxValue;
            return length <= 32 ? mask == (long)int.MaxValue : mask == long.MaxValue;
        }

        public void ClearStateMask(NetworkConnection c) => this.GetConnectionData(c).connectionStateMask = 0L;

        public void ClearStateMask(NetworkConnection c, NetIndex8 pAuthority)
        {
            this.GetConnectionData(c).connectionStateMask = 0L;
            this.GetConnectionData(c).authority = pAuthority;
        }

        public void DirtyStateMask(long mask, NetworkConnection c) => this.GetConnectionData(c).connectionStateMask |= mask;

        public void SuperDirtyStateMask()
        {
            List<NetworkConnection> connections = Network.activeNetwork.core.connections;
            for (int index = 0; index < connections.Count; ++index)
                this.GetConnectionData(connections[index]).connectionStateMask = long.MaxValue;
        }

        public bool NeedsSync(NetworkConnection pConnection) => this.IsDirty(pConnection) || this.isDestroyed;

        public bool IsDirty(NetworkConnection c)
        {
            GhostConnectionData connectionData = this.GetConnectionData(c);
            return connectionData.connectionStateMask != 0L || connectionData.authority != this.thing.authority || (int)connectionData.prevInputState != (int)this._inputStates[0];
        }

        public bool isDestroyed => this._thing.removeFromLevel;

        public NetIndex16 ghostObjectIndex
        {
            get => this._ghostObjectIndex;
            set => this._ghostObjectIndex = value;
        }

        public void RefreshStateMask(List<NetworkConnection> nclist)
        {
            if (DevConsole.core.constantSync)
            {
                for (int index = 0; index < nclist.Count; ++index)
                    this.GetConnectionData(nclist[index]).connectionStateMask |= long.MaxValue;
            }
            else
            {
                long num1 = 0;
                int num2 = 0;
                List<BufferedGhostProperty> properties = this._networkState.properties;
                for (int index = 0; index < properties.Count; ++index)
                {
                    BufferedGhostProperty bufferedGhostProperty = properties[index];
                    if ((this.thing.owner == null || !(bufferedGhostProperty.binding is InterpolatedVec2Binding) || this._finalPositionSyncFrames > 0) && bufferedGhostProperty.Refresh())
                        num1 |= 1L << num2;
                    ++num2;
                }
                for (int index = 0; index < nclist.Count; ++index)
                    this.GetConnectionData(nclist[index]).connectionStateMask |= num1;
                if (this.thing.owner == null)
                    this._finalPositionSyncFrames = 3;
                else
                    --this._finalPositionSyncFrames;
            }
        }

        public BitBuffer GetNetworkStateData(NetworkConnection pConnection, bool pMinimal)
        {
            this.wrote = true;
            BitBuffer pBuffer = new BitBuffer();
            if (this.isDestroyed)
            {
                pBuffer.Write((ushort)(int)this.ghostObjectIndex);
                this.lastWrittenMask = long.MaxValue;
            }
            else
            {
                GhostConnectionData connectionData = this.GetConnectionData(pConnection);
                ++connectionData.lastTickSent;
                GhostObjectHeader.Serialize(pBuffer, this, connectionData.lastTickSent, true, pMinimal);
                pBuffer.Write(this.FillStateData(connectionData, false), true);
                connectionData.prevInputState = this._inputStates[0];
            }
            return pBuffer;
        }

        public BitBuffer GetNetworkStateData()
        {
            this.wrote = true;
            BitBuffer pBuffer = new BitBuffer();
            if (this.isDestroyed)
            {
                pBuffer.Write((ushort)(int)this.ghostObjectIndex);
                this.lastWrittenMask = long.MaxValue;
            }
            else
            {
                GhostObjectHeader.Serialize(pBuffer, this, (NetIndex16)0, false, true);
                pBuffer.Write(this.FillStateData((GhostConnectionData)null, true), true);
            }
            return pBuffer;
        }

        /// <summary>
        /// Fills a bit buffer with this objects network state data
        /// </summary>
        /// <param name="pConnectionData">The connection data object of the NetworkConnection this data is being sent to</param>
        /// <param name="pForceFull">If true, the buffer will be filled with all data instead of delta data.</param>
        /// <returns></returns>
        private BitBuffer FillStateData(GhostConnectionData pConnectionData, bool pForceFull)
        {
            this._body.Clear();
            short num = 0;
            bool flag = !pForceFull;
            if (flag)
            {
                this.WriteMinimalStateMask(pConnectionData, this._body);
                if (this.isLocalController)
                {
                    this._body.Write(true);
                    for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                        this._body.Write(this._inputStates[((int)this._storedInputStates + index) % NetworkConnection.packetsEvery]);
                }
                else
                    this._body.Write(false);
            }
            else
                this.lastWrittenMask = long.MaxValue;
            StateBinding stateBinding = (StateBinding)null;
            try
            {
                foreach (StateBinding field in this._fields)
                {
                    stateBinding = field;
                    if (!flag || (pConnectionData.connectionStateMask & 1L << (int)num) != 0L)
                    {
                        if (field is DataBinding)
                            this._body.Write(field.GetNetValue() as BitBuffer, true);
                        else
                            this._body.WriteBits(field.GetNetValue(), field.bits);
                    }
                    ++num;
                }
            }
            catch (Exception ex)
            {
                Main.SpecialCode = Main.SpecialCode + "Writing " + stateBinding.ToString();
                throw ex;
            }
            if (pConnectionData != null)
            {
                pConnectionData.connectionStateMask = 0L;
                pConnectionData.authority = this.thing.authority;
            }
            return this._body;
        }

        public void ReadInNetworkData(
          NMGhostState ghostState,
          long mask,
          NetworkConnection c,
          bool constructed)
        {
            ++NetworkDebugger.ghostsReceived[NetworkDebugger.currentIndex];
            GhostConnectionData connectionData = this.GetConnectionData(c);
            ghostState.mask = mask;
            ghostState.ghost = this;
            if (connectionData.lastTickReceived == 0)
                connectionData.lastTickReceived = (NetIndex16)((int)ghostState.tick - 1);
            if (Math.Abs(NetIndex16.Difference(ghostState.tick, connectionData.lastTickReceived)) > 600)
            {
                this._stateTimelineIndex = this._stateTimeline.Count + 1;
                this.ReapplyStates();
                this.KillNetworkData();
                connectionData.lastTickReceived = (NetIndex16)((int)ghostState.tick - 1);
            }
            bool flag = false;
            if (connectionData.lastTickReceived < ghostState.tick)
            {
                if ((int)ghostState.tick - (int)connectionData.lastTickReceived > 10)
                    this.delay = 2;
                connectionData.lastTickReceived = ghostState.tick;
            }
            else
            {
                flag = true;
                for (int index = this._stateTimeline.Count - 1; index > 0; --index)
                {
                    if (this._stateTimeline[index].tick == ghostState.tick)
                        return;
                }
            }
            if (ghostState.authority > this.thing.authority)
                this.thing.authority = ghostState.authority;
            BufferedGhostState pState = new BufferedGhostState();
            pState.tick = ghostState.tick;
            pState.mask = mask;
            pState.authority = ghostState.authority;
            if (ghostState.header.delta && ghostState.data.ReadBool())
            {
                pState.inputStates.Clear();
                for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                    pState.inputStates.Add(ghostState.data.ReadUShort());
            }
            short index1 = 0;
            foreach (StateBinding field in this._fields)
            {
                long num = 1L << (int)index1;
                if ((ghostState.mask & num) != 0L)
                {
                    pState.properties.Add(GhostObject.MakeBufferedProperty(field, field.ReadNetValue(ghostState.data), (int)index1, pState.tick));
                    field.initialized = true;
                }
                else
                    pState.properties.Add(this._networkState.properties[(int)index1]);
                ++index1;
            }
            if (!this.IsInitialized())
            {
                foreach (BufferedGhostProperty property in pState.properties)
                {
                    if (!property.isNetworkStateValue && ghostState.tick > this._networkState.properties[property.index].tick)
                    {
                        this._networkState.properties[property.index].value = property.value;
                        this._networkState.properties[property.index].tick = ghostState.tick;
                        this._networkState.properties[property.index].initialized = true;
                    }
                }
                if (!this.IsInitialized())
                    return;
                this._networkState.ApplyImmediately();
                if (!(this.thing is Holdable))
                    return;
                Holdable thing = this.thing as Holdable;
                if (!thing.isSpawned || thing.didSpawn)
                    return;
                thing.spawnAnimation = true;
                thing.didSpawn = true;
            }
            else
            {
                this._last3MaskInc = (this._last3MaskInc + 1) % 3;
                this._last3StateMasks[this._last3MaskInc] = ghostState.mask;
                if (flag)
                {
                    int index2 = 0;
                    for (int index3 = this._stateTimeline.Count - 1; index3 > 0; --index3)
                    {
                        if (this._stateTimeline[index3].tick > pState.tick)
                            index2 = index3;
                    }
                    this._stateTimeline.Insert(index2, pState);
                    if (index2 < this._stateTimelineIndex)
                        pState.ApplyImmediately(this._networkState);
                }
                else
                {
                    this.AddState(pState);
                    if (this._stateTimeline.Count == 1)
                        pState.ApplyImmediately(this._networkState);
                }
                if (!(this._thing is PhysicsObject))
                    return;
                (this._thing as PhysicsObject).sleeping = false;
            }
        }

        private void AddState(BufferedGhostState pState)
        {
            this._stateTimeline.Add(pState);
            if (this._stateTimeline.Count <= 1)
                return;
            pState.previousState = this._stateTimeline[this._stateTimeline.Count - 2];
            pState.previousState.nextState = this._stateTimeline[this._stateTimeline.Count - 1];
        }

        public BufferedGhostState GetCurrentState()
        {
            BufferedGhostState currentState = new BufferedGhostState();
            int count = this._fields.Count;
            for (int index = 0; index < count; ++index)
            {
                StateBinding field = this._fields[index];
                BufferedGhostProperty bufferedGhostProperty = GhostObject.MakeBufferedProperty(field, field.classValue);
                bufferedGhostProperty.initialized = this.thing.connection == DuckNetwork.localConnection;
                bufferedGhostProperty.isNetworkStateValue = true;
                currentState.properties.Add(bufferedGhostProperty);
                if (field.name == "netPosition")
                    this.netPositionProperty = bufferedGhostProperty;
                else if (field.name == "netVelocity")
                    this.netVelocityProperty = bufferedGhostProperty;
                else if (field.name == "_angle")
                    this.netAngleProperty = bufferedGhostProperty;
            }
            return currentState;
        }

        private Vec2 Slerp(Vec2 from, Vec2 to, float step)
        {
            if ((double)step == 0.0)
                return from;
            if (from == to || (double)step == 1.0)
                return to;
            double a = Math.Acos((double)Vec2.Dot(from, to));
            if (a == 0.0)
                return to;
            double num = Math.Sin(a);
            return (float)(Math.Sin((1.0 - (double)step) * a) / num) * from + (float)(Math.Sin((double)step * a) / num) * to;
        }

        public bool isLocalController => this._inputObject != null && this._inputObject.inputProfile != null && this._inputObject.inputProfile.virtualDevice == null;

        public bool shouldRemove
        {
            get => this._shouldRemove || this.thing.removeFromLevel;
            set => this._shouldRemove = value;
        }

        public void KillNetworkData()
        {
            this._stateTimeline.Clear();
            this._stateTimelineIndex = 0;
            foreach (BufferedGhostProperty property in this._networkState.properties)
                property.tick = (NetIndex16)0;
        }

        public void TakeOwnership()
        {
            foreach (KeyValuePair<NetworkConnection, GhostConnectionData> keyValuePair in this._connectionData)
            {
                keyValuePair.Value.connectionStateMask |= this._last3StateMasks[0];
                keyValuePair.Value.connectionStateMask |= this._last3StateMasks[1];
                keyValuePair.Value.connectionStateMask |= this._last3StateMasks[2];
            }
        }

        public bool IsInitialized()
        {
            if (!this.initializedCached)
            {
                this.initializedCached = true;
                foreach (BufferedGhostProperty property in this._networkState.properties)
                {
                    if (!property.initialized)
                    {
                        this.initializedCached = false;
                        break;
                    }
                }
            }
            return this.initializedCached;
        }

        public void UpdateTick()
        {
            if (!this.isLocalController)
                return;
            if (MonoMain.pauseMenu != null)
            {
                this._inputStates[(int)this._storedInputStates] = (ushort)0;
                this._storedInputStates = (byte)(((int)this._storedInputStates + 1) % NetworkConnection.packetsEvery);
            }
            else
            {
                this._inputStates[(int)this._storedInputStates] = this._inputObject.inputProfile.state;
                this._storedInputStates = (byte)(((int)this._storedInputStates + 1) % NetworkConnection.packetsEvery);
            }
        }

        public void Update()
        {
            if (this.removeLogCooldown > (byte)0)
                --this.removeLogCooldown;
            if (this._thing == null)
                return;
            this._thing.isLocal = false;
            if (!this.IsInitialized() || this._thing.level == null)
            {
                if (this.framesSinceRequestInitialize <= 15 || this.thing == null || this.thing.connection == null)
                    return;
                if (this._thing.level == null)
                    DevConsole.Log(DCSection.DuckNet, "|DGYELLOW|Skipping ghost update (" + this.ghostObjectIndex.ToString() + ", " + this.thing.GetType().ToString() + ")(LEVEL NULL)...");
                else
                    DevConsole.Log(DCSection.DuckNet, "|DGYELLOW|Skipping ghost update (" + this.ghostObjectIndex.ToString() + ", " + this.thing.GetType().ToString() + ")(NOT INITIALIZED)...");
                this.framesSinceRequestInitialize = 0;
            }
            else
            {
                if (this._thing.active)
                    this._thing.DoUpdate();
                if (this._thing.owner != null || this._thing.isServerForObject)
                    return;
                if (this.netPositionProperty != null)
                    this.netPositionProperty.Apply(1f);
                if (this.netVelocityProperty != null)
                    this.netVelocityProperty.Apply(1f);
                if (this.netAngleProperty == null)
                    return;
                this.netAngleProperty.Apply(1f);
            }
        }

        public void UpdateRemoval()
        {
            if (this._thing.ghostType != (ushort)0 && (this._thing.level == null || this._thing.level == Level.current || Level.core.nextLevel != null))
                return;
            this._shouldRemove = true;
        }

        public BufferedGhostState GetStateForTick(NetIndex16 t)
        {
            for (int index = this._stateTimeline.Count - 1; index >= 0; --index)
            {
                BufferedGhostState stateForTick = this._stateTimeline[index];
                if ((int)stateForTick.tick <= (int)t)
                    return stateForTick;
            }
            return (BufferedGhostState)null;
        }

        private BufferedGhostState GetStateToProcess() => this._stateTimelineIndex < this._stateTimeline.Count ? this._stateTimeline[this._stateTimelineIndex] : this._stateTimeline.LastOrDefault<BufferedGhostState>();

        public void ReapplyStates()
        {
            GhostObject.applyContext = this;
            for (int index = 0; index < this._stateTimeline.Count; ++index)
            {
                if (index < this._stateTimelineIndex)
                {
                    this._stateTimeline[index]._framesApplied = NetworkConnection.packetsEvery - 1;
                    this._stateTimeline[index].Apply(1f, this._networkState);
                }
            }
            GhostObject.applyContext = (GhostObject)null;
        }

        private void ApplyState(
          BufferedGhostState pState,
          float pLerp,
          BufferedGhostState pNetworkState)
        {
            GhostObject.applyContext = this;
            pState.Apply(pLerp, pNetworkState);
            this.ApplyStateInput(pState);
            GhostObject.applyContext = (GhostObject)null;
        }

        public void ReleaseReferences(bool pFull = true)
        {
            if (this.thing != null && this.thing.ghostObject == this)
                this.thing.ghostObject = (GhostObject)null;
            if (!pFull)
                return;
            this._thing = (Thing)null;
            this._stateTimeline = (List<BufferedGhostState>)null;
            this._networkState = (BufferedGhostState)null;
            this._manager = (GhostManager)null;
            this._fields = (List<StateBinding>)null;
            this._inputObject = (ITakeInput)null;
            this._prevOwner = (Thing)null;
            this.netPositionProperty = (BufferedGhostProperty)null;
            this.netVelocityProperty = (BufferedGhostProperty)null;
            this.netAngleProperty = (BufferedGhostProperty)null;
        }

        private void ApplyStateInput(BufferedGhostState pState)
        {
            if (pState.inputStates == null)
                return;
            int index = Math.Min(pState._framesApplied - 1, pState.inputStates.Count - 1);
            if (this._inputObject == null || this._inputObject.inputProfile == null || this._inputObject.inputProfile.virtualDevice == null)
                return;
            if (pState.previousState != null && pState.previousState.inputStates.Count > 0 && pState.nextState != null)
                this._inputObject.inputProfile.virtualDevice.SetState(pState.previousState.inputStates[index]);
            this._inputObject.inputProfile.virtualDevice.SetState(pState.inputStates[index]);
        }

        public int constipation
        {
            get
            {
                if (this._stateTimelineIndex < this._stateTimeline.Count - 70)
                    return 3;
                return this._stateTimelineIndex < this._stateTimeline.Count - 5 ? 2 : 1;
            }
        }

        public void UpdateState()
        {
            if (this.delay > 0)
            {
                --this.delay;
            }
            else
            {
                for (int index = 0; index < 2; ++index)
                {
                    if (this._stateTimeline.Count > 360)
                    {
                        if (this._stateTimelineIndex == 0)
                        {
                            BufferedGhostState pState = this._stateTimeline[0];
                            this.ApplyState(pState, 1f, this._networkState);
                            pState._framesApplied = NetworkConnection.packetsEvery;
                        }
                        if (this._stateTimeline.Count > 0)
                        {
                            BufferedGhostState bufferedGhostState = this._stateTimeline.ElementAt<BufferedGhostState>(0);
                            bufferedGhostState.previousState = (BufferedGhostState)null;
                            bufferedGhostState.nextState = (BufferedGhostState)null;
                            this._stateTimeline.RemoveAt(0);
                            this._stateTimelineIndex = Math.Max(0, this._stateTimelineIndex - 1);
                        }
                    }
                }
                BufferedGhostState stateToProcess = this.GetStateToProcess();
                if (stateToProcess != null && stateToProcess._framesApplied < NetworkConnection.packetsEvery)
                {
                    if (stateToProcess._framesApplied >= NetworkConnection.packetsEvery - 1)
                    {
                        this.ApplyState(stateToProcess, 1f, this._networkState);
                        stateToProcess._framesApplied = NetworkConnection.packetsEvery;
                        this._stateTimelineIndex = Math.Min(this._stateTimeline.Count, this._stateTimelineIndex + 1);
                    }
                    else
                        this.ApplyState(stateToProcess, 0.5f, this._networkState);
                }
                else if (stateToProcess != null)
                {
                    this.ApplyState(stateToProcess, 1f, this._networkState);
                    stateToProcess._framesApplied = NetworkConnection.packetsEvery;
                    this._stateTimelineIndex = Math.Min(this._stateTimeline.Count, this._stateTimelineIndex + 1);
                }
                if (this.thing.owner != this._prevOwner)
                    this.ReapplyStates();
                this._prevOwner = this.thing.owner;
                if (this._thing.ghostType != (ushort)0)
                    return;
                this._shouldRemove = true;
            }
        }

        public Thing thing => this._thing;

        public GhostManager manager => this._manager;

        public GhostObject()
        {
        }

        public static Profile IndexToProfile(NetIndex16 pIndex)
        {
            int index = (int)((double)pIndex._index / (double)GhostManager.kGhostIndexMax);
            return index < 0 || index >= DuckNetwork.profiles.Count ? (Profile)null : DuckNetwork.profiles[index];
        }

        public GhostObject(Thing thing, GhostManager manager, int ghostIndex = -1, bool levelInit = false)
        {
            try
            {
                this._thing = thing;
                this._thing.ghostObject = this;
                this._inputObject = this._thing as ITakeInput;
                if (ghostIndex == -1 && this._thing.fixedGhostIndex != 0)
                    ghostIndex = (int)this._thing.fixedGhostIndex;
                this.initializedCached = false;
                foreach (FieldInfo fieldInfo in Editor.AllStateFields[this._thing.GetType()])
                {
                    StateBinding stateBinding = fieldInfo.GetValue((object)this._thing) as StateBinding;
                    stateBinding.Connect(this._thing);
                    this._fields.Add(stateBinding);
                }
                this._networkState = this.GetCurrentState();
                this._manager = manager;
                if (ghostIndex != -1)
                {
                    this._ghostObjectIndex = new NetIndex16(ghostIndex);
                    this._thing.ghostType = Editor.IDToType[this._thing.GetType()];
                }
                else
                {
                    this._ghostObjectIndex = this._manager.GetGhostIndex(levelInit);
                    if (!levelInit || Network.isServer)
                        this._thing.connection = DuckNetwork.localConnection;
                }
                DevConsole.Log(DCSection.GhostMan, "|DGBLUE|Creating|PREV| ghost (" + this.ghostObjectIndex.ToString() + "|PREV|)");
                manager._ghostIndexMap[this._ghostObjectIndex] = this;
            }
            catch (Exception ex)
            {
                Main.SpecialCode = "GhostObject Constructor(" + thing.GetType().Name + ")";
                throw ex;
            }
        }
    }
}
