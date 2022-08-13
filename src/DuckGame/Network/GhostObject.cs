// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        //private long tickIncFrame;
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
        //private const int kStateHistoryLength = 360;
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
                return null;
            GhostConnectionData connectionData;
            if (!_connectionData.TryGetValue(c, out connectionData))
            {
                connectionData = new GhostConnectionData();
                _connectionData[c] = connectionData;
            }
            return connectionData;
        }

        public void ClearConnectionData(NetworkConnection c)
        {
            if (!_connectionData.ContainsKey(c))
                return;
            _connectionData.Remove(c);
        }

        private void WriteMinimalStateMask(GhostConnectionData dat, BitBuffer b)
        {
            lastWrittenMask = dat.connectionStateMask;
            int count = _fields.Count;
            if (count <= 8)
                b.Write((byte)lastWrittenMask);
            else if (count <= 16)
                b.Write((ushort)lastWrittenMask);
            else if (count <= 32)
                b.Write((uint)lastWrittenMask);
            else
                b.Write(lastWrittenMask);
        }

        public static long ReadMinimalStateMask(System.Type t, BitBuffer b) => b.ReadBits<long>(Editor.AllStateFields[t].Length);

        //private object GetMinimalStateMask(NetworkConnection c)
        //{
        //    long connectionStateMask = this.GetConnectionData(c).connectionStateMask;
        //    int count = this._fields.Count;
        //    if (count <= 8)
        //        return (object)(byte)connectionStateMask;
        //    if (count <= 16)
        //        return (object)(short)connectionStateMask;
        //    return count <= 32 ? (object)(int)connectionStateMask : (object)connectionStateMask;
        //}

        public static long ReadMask(System.Type t, BitBuffer b)
        {
            int length = Editor.AllStateFields[t].Length;
            if (length <= 8)
                return b.ReadByte();
            if (length <= 16)
                return b.ReadUShort();
            return length <= 32 ? b.ReadUInt() : b.ReadLong();
        }

        public static bool MaskIsMaxValue(System.Type t, long mask)
        {
            int length = Editor.AllStateFields[t].Length;
            if (length <= 8)
                return mask == byte.MaxValue;
            if (length <= 16)
                return mask == short.MaxValue;
            return length <= 32 ? mask == int.MaxValue : mask == long.MaxValue;
        }

        public void ClearStateMask(NetworkConnection c) => GetConnectionData(c).connectionStateMask = 0L;

        public void ClearStateMask(NetworkConnection c, NetIndex8 pAuthority)
        {
            GetConnectionData(c).connectionStateMask = 0L;
            GetConnectionData(c).authority = pAuthority;
        }

        public void DirtyStateMask(long mask, NetworkConnection c) => GetConnectionData(c).connectionStateMask |= mask;

        public void SuperDirtyStateMask()
        {
            List<NetworkConnection> connections = Network.activeNetwork.core.connections;
            for (int index = 0; index < connections.Count; ++index)
                GetConnectionData(connections[index]).connectionStateMask = long.MaxValue;
        }

        public bool NeedsSync(NetworkConnection pConnection) => IsDirty(pConnection) || isDestroyed;

        public bool IsDirty(NetworkConnection c)
        {
            GhostConnectionData connectionData = GetConnectionData(c);
            return connectionData.connectionStateMask != 0L || connectionData.authority != thing.authority || connectionData.prevInputState != _inputStates[0];
        }

        public bool isDestroyed => _thing.removeFromLevel;

        public NetIndex16 ghostObjectIndex
        {
            get => _ghostObjectIndex;
            set => _ghostObjectIndex = value;
        }

        public void RefreshStateMask(List<NetworkConnection> nclist)
        {
            if (DevConsole.core.constantSync)
            {
                for (int index = 0; index < nclist.Count; ++index)
                    GetConnectionData(nclist[index]).connectionStateMask |= long.MaxValue;
            }
            else
            {
                long num1 = 0;
                int num2 = 0;
                List<BufferedGhostProperty> properties = _networkState.properties;
                for (int index = 0; index < properties.Count; ++index)
                {
                    BufferedGhostProperty bufferedGhostProperty = properties[index];
                    if ((thing.owner == null || !(bufferedGhostProperty.binding is InterpolatedVec2Binding) || _finalPositionSyncFrames > 0) && bufferedGhostProperty.Refresh())
                        num1 |= 1L << num2;
                    ++num2;
                }
                for (int index = 0; index < nclist.Count; ++index)
                    GetConnectionData(nclist[index]).connectionStateMask |= num1;
                if (thing.owner == null)
                    _finalPositionSyncFrames = 3;
                else
                    --_finalPositionSyncFrames;
            }
        }

        public BitBuffer GetNetworkStateData(NetworkConnection pConnection, bool pMinimal)
        {
            wrote = true;
            BitBuffer pBuffer = new BitBuffer();
            if (isDestroyed)
            {
                pBuffer.Write((ushort)(int)ghostObjectIndex);
                lastWrittenMask = long.MaxValue;
            }
            else
            {
                GhostConnectionData connectionData = GetConnectionData(pConnection);
                ++connectionData.lastTickSent;
                GhostObjectHeader.Serialize(pBuffer, this, connectionData.lastTickSent, true, pMinimal);
                pBuffer.Write(FillStateData(connectionData, false), true);
                connectionData.prevInputState = _inputStates[0];
            }
            return pBuffer;
        }

        public BitBuffer GetNetworkStateData()
        {
            wrote = true;
            BitBuffer pBuffer = new BitBuffer();
            if (isDestroyed)
            {
                pBuffer.Write((ushort)(int)ghostObjectIndex);
                lastWrittenMask = long.MaxValue;
            }
            else
            {
                GhostObjectHeader.Serialize(pBuffer, this, (NetIndex16)0, false, true);
                pBuffer.Write(FillStateData(null, true), true);
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
            _body.Clear();
            short num = 0;
            bool flag = !pForceFull;
            if (flag)
            {
                WriteMinimalStateMask(pConnectionData, _body);
                if (isLocalController)
                {
                    _body.Write(true);
                    for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                        _body.Write(_inputStates[(_storedInputStates + index) % NetworkConnection.packetsEvery]);
                }
                else
                    _body.Write(false);
            }
            else
                lastWrittenMask = long.MaxValue;
            StateBinding stateBinding = null;
            try
            {
                foreach (StateBinding field in _fields)
                {
                    stateBinding = field;
                    if (!flag || (pConnectionData.connectionStateMask & 1L << num) != 0L)
                    {
                        if (field is DataBinding)
                            _body.Write(field.GetNetValue() as BitBuffer, true);
                        else
                            _body.WriteBits(field.GetNetValue(), field.bits);
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
                pConnectionData.authority = thing.authority;
            }
            return _body;
        }

        public void ReadInNetworkData(
          NMGhostState ghostState,
          long mask,
          NetworkConnection c,
          bool constructed)
        {
            ++NetworkDebugger.ghostsReceived[NetworkDebugger.currentIndex];
            GhostConnectionData connectionData = GetConnectionData(c);
            ghostState.mask = mask;
            ghostState.ghost = this;
            if (connectionData.lastTickReceived == 0)
                connectionData.lastTickReceived = (NetIndex16)((int)ghostState.tick - 1);
            if (Math.Abs(NetIndex16.Difference(ghostState.tick, connectionData.lastTickReceived)) > 600)
            {
                _stateTimelineIndex = _stateTimeline.Count + 1;
                ReapplyStates();
                KillNetworkData();
                connectionData.lastTickReceived = (NetIndex16)((int)ghostState.tick - 1);
            }
            bool flag = false;
            if (connectionData.lastTickReceived < ghostState.tick)
            {
                if ((int)ghostState.tick - (int)connectionData.lastTickReceived > 10)
                    delay = 2;
                connectionData.lastTickReceived = ghostState.tick;
            }
            else
            {
                flag = true;
                for (int index = _stateTimeline.Count - 1; index > 0; --index)
                {
                    if (_stateTimeline[index].tick == ghostState.tick)
                        return;
                }
            }
            if (ghostState.authority > thing.authority)
                thing.authority = ghostState.authority;
            BufferedGhostState pState = new BufferedGhostState
            {
                tick = ghostState.tick,
                mask = mask,
                authority = ghostState.authority
            };
            if (ghostState.header.delta && ghostState.data.ReadBool())
            {
                pState.inputStates.Clear();
                for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                    pState.inputStates.Add(ghostState.data.ReadUShort());
            }
            short index1 = 0;
            foreach (StateBinding field in _fields)
            {
                long num = 1L << index1;
                if ((ghostState.mask & num) != 0L)
                {
                    pState.properties.Add(GhostObject.MakeBufferedProperty(field, field.ReadNetValue(ghostState.data), index1, pState.tick));
                    field.initialized = true;
                }
                else
                    pState.properties.Add(_networkState.properties[index1]);
                ++index1;
            }
            if (!IsInitialized())
            {
                foreach (BufferedGhostProperty property in pState.properties)
                {
                    if (!property.isNetworkStateValue && ghostState.tick > _networkState.properties[property.index].tick)
                    {
                        _networkState.properties[property.index].value = property.value;
                        _networkState.properties[property.index].tick = ghostState.tick;
                        _networkState.properties[property.index].initialized = true;
                    }
                }
                if (!IsInitialized())
                    return;
                _networkState.ApplyImmediately();
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
                _last3MaskInc = (_last3MaskInc + 1) % 3;
                _last3StateMasks[_last3MaskInc] = ghostState.mask;
                if (flag)
                {
                    int index2 = 0;
                    for (int index3 = _stateTimeline.Count - 1; index3 > 0; --index3)
                    {
                        if (_stateTimeline[index3].tick > pState.tick)
                            index2 = index3;
                    }
                    _stateTimeline.Insert(index2, pState);
                    if (index2 < _stateTimelineIndex)
                        pState.ApplyImmediately(_networkState);
                }
                else
                {
                    AddState(pState);
                    if (_stateTimeline.Count == 1)
                        pState.ApplyImmediately(_networkState);
                }
                if (!(_thing is PhysicsObject))
                    return;
                (_thing as PhysicsObject).sleeping = false;
            }
        }

        private void AddState(BufferedGhostState pState)
        {
            _stateTimeline.Add(pState);
            if (_stateTimeline.Count <= 1)
                return;
            pState.previousState = _stateTimeline[_stateTimeline.Count - 2];
            pState.previousState.nextState = _stateTimeline[_stateTimeline.Count - 1];
        }

        public BufferedGhostState GetCurrentState()
        {
            BufferedGhostState currentState = new BufferedGhostState();
            int count = _fields.Count;
            for (int index = 0; index < count; ++index)
            {
                StateBinding field = _fields[index];
                BufferedGhostProperty bufferedGhostProperty = GhostObject.MakeBufferedProperty(field, field.classValue);
                bufferedGhostProperty.initialized = thing.connection == DuckNetwork.localConnection;
                bufferedGhostProperty.isNetworkStateValue = true;
                currentState.properties.Add(bufferedGhostProperty);
                if (field.name == "netPosition")
                    netPositionProperty = bufferedGhostProperty;
                else if (field.name == "netVelocity")
                    netVelocityProperty = bufferedGhostProperty;
                else if (field.name == "_angle")
                    netAngleProperty = bufferedGhostProperty;
            }
            return currentState;
        }

        //private Vec2 Slerp(Vec2 from, Vec2 to, float step)
        //{
        //    if (step == 0.0)
        //        return from;
        //    if (from == to || step == 1.0)
        //        return to;
        //    double a = Math.Acos(Vec2.Dot(from, to));
        //    if (a == 0.0)
        //        return to;
        //    double num = Math.Sin(a);
        //    return (float)(Math.Sin((1.0 - step) * a) / num) * from + (float)(Math.Sin(step * a) / num) * to;
        //}

        public bool isLocalController => _inputObject != null && _inputObject.inputProfile != null && _inputObject.inputProfile.virtualDevice == null;

        public bool shouldRemove
        {
            get => _shouldRemove || thing.removeFromLevel;
            set => _shouldRemove = value;
        }

        public void KillNetworkData()
        {
            _stateTimeline.Clear();
            _stateTimelineIndex = 0;
            foreach (BufferedGhostProperty property in _networkState.properties)
                property.tick = (NetIndex16)0;
        }

        public void TakeOwnership()
        {
            foreach (KeyValuePair<NetworkConnection, GhostConnectionData> keyValuePair in _connectionData)
            {
                keyValuePair.Value.connectionStateMask |= _last3StateMasks[0];
                keyValuePair.Value.connectionStateMask |= _last3StateMasks[1];
                keyValuePair.Value.connectionStateMask |= _last3StateMasks[2];
            }
        }

        public bool IsInitialized()
        {
            if (!initializedCached)
            {
                initializedCached = true;
                foreach (BufferedGhostProperty property in _networkState.properties)
                {
                    if (!property.initialized)
                    {
                        initializedCached = false;
                        break;
                    }
                }
            }
            return initializedCached;
        }

        public void UpdateTick()
        {
            if (!isLocalController)
                return;
            if (MonoMain.pauseMenu != null)
            {
                _inputStates[_storedInputStates] = 0;
                _storedInputStates = (byte)((_storedInputStates + 1) % NetworkConnection.packetsEvery);
            }
            else
            {
                _inputStates[_storedInputStates] = _inputObject.inputProfile.state;
                _storedInputStates = (byte)((_storedInputStates + 1) % NetworkConnection.packetsEvery);
            }
        }

        public void Update()
        {
            if (removeLogCooldown > 0)
                --removeLogCooldown;
            if (_thing == null)
                return;
            _thing.isLocal = false;
            if (!IsInitialized() || _thing.level == null)
            {
                if (framesSinceRequestInitialize <= 15 || thing == null || thing.connection == null)
                    return;
                if (_thing.level == null)
                    DevConsole.Log(DCSection.DuckNet, "|DGYELLOW|Skipping ghost update (" + ghostObjectIndex.ToString() + ", " + thing.GetType().ToString() + ")(LEVEL NULL)...");
                else
                    DevConsole.Log(DCSection.DuckNet, "|DGYELLOW|Skipping ghost update (" + ghostObjectIndex.ToString() + ", " + thing.GetType().ToString() + ")(NOT INITIALIZED)...");
                framesSinceRequestInitialize = 0;
            }
            else
            {
                if (_thing.active)
                    _thing.DoUpdate();
                if (_thing.owner != null || _thing.isServerForObject)
                    return;
                if (netPositionProperty != null)
                    netPositionProperty.Apply(1f);
                if (netVelocityProperty != null)
                    netVelocityProperty.Apply(1f);
                if (netAngleProperty == null)
                    return;
                netAngleProperty.Apply(1f);
            }
        }

        public void UpdateRemoval()
        {
            if (_thing.ghostType != 0 && (_thing.level == null || _thing.level == Level.current || Level.core.nextLevel != null))
                return;
            _shouldRemove = true;
        }

        public BufferedGhostState GetStateForTick(NetIndex16 t)
        {
            for (int index = _stateTimeline.Count - 1; index >= 0; --index)
            {
                BufferedGhostState stateForTick = _stateTimeline[index];
                if ((int)stateForTick.tick <= (int)t)
                    return stateForTick;
            }
            return null;
        }

        private BufferedGhostState GetStateToProcess() => _stateTimelineIndex < _stateTimeline.Count ? _stateTimeline[_stateTimelineIndex] : _stateTimeline.LastOrDefault<BufferedGhostState>();

        public void ReapplyStates()
        {
            GhostObject.applyContext = this;
            for (int index = 0; index < _stateTimeline.Count; ++index)
            {
                if (index < _stateTimelineIndex)
                {
                    _stateTimeline[index]._framesApplied = NetworkConnection.packetsEvery - 1;
                    _stateTimeline[index].Apply(1f, _networkState);
                }
            }
            GhostObject.applyContext = null;
        }

        private void ApplyState(
          BufferedGhostState pState,
          float pLerp,
          BufferedGhostState pNetworkState)
        {
            GhostObject.applyContext = this;
            pState.Apply(pLerp, pNetworkState);
            ApplyStateInput(pState);
            GhostObject.applyContext = null;
        }

        public void ReleaseReferences(bool pFull = true)
        {
            if (thing != null && thing.ghostObject == this)
                thing.ghostObject = null;
            if (!pFull)
                return;
            _thing = null;
            _stateTimeline = null;
            _networkState = null;
            _manager = null;
            _fields = null;
            _inputObject = null;
            _prevOwner = null;
            netPositionProperty = null;
            netVelocityProperty = null;
            netAngleProperty = null;
        }

        private void ApplyStateInput(BufferedGhostState pState)
        {
            if (pState.inputStates == null)
                return;
            int index = Math.Min(pState._framesApplied - 1, pState.inputStates.Count - 1);
            if (_inputObject == null || _inputObject.inputProfile == null || _inputObject.inputProfile.virtualDevice == null)
                return;
            if (pState.previousState != null && pState.previousState.inputStates.Count > 0 && pState.nextState != null)
                _inputObject.inputProfile.virtualDevice.SetState(pState.previousState.inputStates[index]);
            _inputObject.inputProfile.virtualDevice.SetState(pState.inputStates[index]);
        }

        public int constipation
        {
            get
            {
                if (_stateTimelineIndex < _stateTimeline.Count - 70)
                    return 3;
                return _stateTimelineIndex < _stateTimeline.Count - 5 ? 2 : 1;
            }
        }

        public void UpdateState()
        {
            if (delay > 0)
            {
                --delay;
            }
            else
            {
                for (int index = 0; index < 2; ++index)
                {
                    if (_stateTimeline.Count > 360)
                    {
                        if (_stateTimelineIndex == 0)
                        {
                            BufferedGhostState pState = _stateTimeline[0];
                            ApplyState(pState, 1f, _networkState);
                            pState._framesApplied = NetworkConnection.packetsEvery;
                        }
                        if (_stateTimeline.Count > 0)
                        {
                            BufferedGhostState bufferedGhostState = _stateTimeline.ElementAt<BufferedGhostState>(0);
                            bufferedGhostState.previousState = null;
                            bufferedGhostState.nextState = null;
                            _stateTimeline.RemoveAt(0);
                            _stateTimelineIndex = Math.Max(0, _stateTimelineIndex - 1);
                        }
                    }
                }
                BufferedGhostState stateToProcess = GetStateToProcess();
                if (stateToProcess != null && stateToProcess._framesApplied < NetworkConnection.packetsEvery)
                {
                    if (stateToProcess._framesApplied >= NetworkConnection.packetsEvery - 1)
                    {
                        ApplyState(stateToProcess, 1f, _networkState);
                        stateToProcess._framesApplied = NetworkConnection.packetsEvery;
                        _stateTimelineIndex = Math.Min(_stateTimeline.Count, _stateTimelineIndex + 1);
                    }
                    else
                        ApplyState(stateToProcess, 0.5f, _networkState);
                }
                else if (stateToProcess != null)
                {
                    ApplyState(stateToProcess, 1f, _networkState);
                    stateToProcess._framesApplied = NetworkConnection.packetsEvery;
                    _stateTimelineIndex = Math.Min(_stateTimeline.Count, _stateTimelineIndex + 1);
                }
                if (thing.owner != _prevOwner)
                    ReapplyStates();
                _prevOwner = thing.owner;
                if (_thing.ghostType != 0)
                    return;
                _shouldRemove = true;
            }
        }

        public Thing thing => _thing;

        public GhostManager manager => _manager;

        public GhostObject()
        {
        }

        public static Profile IndexToProfile(NetIndex16 pIndex)
        {
            int index = (int)(pIndex._index / GhostManager.kGhostIndexMax);
            return index < 0 || index >= DuckNetwork.profiles.Count ? null : DuckNetwork.profiles[index];
        }

        public GhostObject(Thing thing, GhostManager manager, int ghostIndex = -1, bool levelInit = false)
        {
            try
            {
                _thing = thing;
                _thing.ghostObject = this;
                _inputObject = _thing as ITakeInput;
                if (ghostIndex == -1 && _thing.fixedGhostIndex != 0)
                    ghostIndex = (int)_thing.fixedGhostIndex;
                initializedCached = false;
                foreach (FieldInfo fieldInfo in Editor.AllStateFields[_thing.GetType()])
                {
                    StateBinding stateBinding = fieldInfo.GetValue(_thing) as StateBinding;
                    stateBinding.Connect(_thing);
                    _fields.Add(stateBinding);
                }
                _networkState = GetCurrentState();
                _manager = manager;
                if (ghostIndex != -1)
                {
                    _ghostObjectIndex = new NetIndex16(ghostIndex);
                    _thing.ghostType = Editor.IDToType[_thing.GetType()];
                }
                else
                {
                    _ghostObjectIndex = _manager.GetGhostIndex(levelInit);
                    if (!levelInit || Network.isServer)
                        _thing.connection = DuckNetwork.localConnection;
                }
                DevConsole.Log(DCSection.GhostMan, "|DGBLUE|Creating|PREV| ghost (" + ghostObjectIndex.ToString() + "|PREV|)");
                manager._ghostIndexMap[_ghostObjectIndex] = this;
            }
            catch (Exception ex)
            {
                Main.SpecialCode = "GhostObject Constructor(" + thing.GetType().Name + ")";
                throw ex;
            }
        }
    }
}
