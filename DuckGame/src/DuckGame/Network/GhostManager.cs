// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostManager
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class GhostManager
    {
        private static int kglobalID;
        public int globalID;
        public List<SynchronizedNetMessage> _synchronizedEvents = new List<SynchronizedNetMessage>();
        private NetParticleManager _particleManager = new NetParticleManager();
        private GhostObject _managerState;
        public bool inGhostLerpLoop;
        public HashSet<GhostObject> _tempGhosts = new HashSet<GhostObject>();
        public HashSet<GhostObject> _ghosts = new HashSet<GhostObject>();
        public HashSet<GhostObject> pendingBitBufferGhosts = new HashSet<GhostObject>();
        private NetIndex16 ghostObjectIndex = new NetIndex16(32);
        public static readonly int kGhostIndexMax = 2500;
        private int _framesSinceClear;
        public Dictionary<NetIndex16, GhostObject> _ghostIndexMap = new Dictionary<NetIndex16, GhostObject>();
        public static bool inGhostLoop = false;
        public static bool receivingDestroyMessage = false;
        public static bool changingGhostType = false;
        public static bool updatingBullets = false;
        public List<GhostObject> _destroyedGhosts = new List<GhostObject>();
        public List<NetIndex16> _destroyResends = new List<NetIndex16>();
        private Dictionary<ushort, Thing> _specialSyncMap = new Dictionary<ushort, Thing>();
        private static HelperPhysicsIndexSorter helperPhysicsIndexSorter = new HelperPhysicsIndexSorter();
        private HashSet<GhostObject> _removeList = new HashSet<GhostObject>();

        public void UpdateSynchronizedEvents()
        {
            for (int index = 0; index < _synchronizedEvents.Count; ++index)
            {
                if (_synchronizedEvents[index].Update())
                {
                    DevConsole.Log(DCSection.DuckNet, "@received Activating |WHITE|" + _synchronizedEvents[index].ToString() + "|PREV|", DuckNetwork.localConnection);
                    if (_synchronizedEvents[index] is NMSynchronizedEvent)
                        (_synchronizedEvents[index] as NMSynchronizedEvent).Activate();
                    _synchronizedEvents.RemoveAt(index);
                    --index;
                }
            }
        }

        public NetParticleManager particleManager => _particleManager;

        public NetIndex16 predictionIndex
        {
            get => (_managerState.thing as GhostManagerState).predictionIndex;
            set => (_managerState.thing as GhostManagerState).predictionIndex = value;
        }

        public void TransferPendingGhosts()
        {
            foreach (GhostObject pendingBitBufferGhost in pendingBitBufferGhosts)
                AddGhost(pendingBitBufferGhost);
            pendingBitBufferGhosts.Clear();
        }

        public static GhostManager context => Network.activeNetwork.core.ghostManager;

        public GhostManager() => globalID = kglobalID++;

        public NetIndex16 currentGhostIndex => ghostObjectIndex;

        public NetIndex16 GetGhostIndex() => GetGhostIndex(false);

        public NetIndex16 GetGhostIndex(bool levelInit)
        {
            int fixedGhostIndex = DuckNetwork.localProfile.fixedGhostIndex;
            if (levelInit)
                fixedGhostIndex = DuckNetwork.hostProfile.fixedGhostIndex;
            NetIndex16 ghostObjectIndex1 = ghostObjectIndex;
            while (_ghostIndexMap.ContainsKey(ghostObjectIndex + fixedGhostIndex * kGhostIndexMax))
            {
                ++ghostObjectIndex;
                if (ghostObjectIndex > kGhostIndexMax - 10)
                    ghostObjectIndex = (NetIndex16)32;
                if (ghostObjectIndex == ghostObjectIndex1 || ghostObjectIndex1 < 32)
                    break;
            }
            NetIndex16 ghostObjectIndex2 = ghostObjectIndex;
            ++ghostObjectIndex;
            return ghostObjectIndex2 + fixedGhostIndex * kGhostIndexMax;
        }

        public void SetGhostIndex(NetIndex16 idx)
        {
            ghostObjectIndex = idx;
            Clear();
        }

        public void ResetGhostIndex(byte levelIndex)
        {
            ghostObjectIndex = levelIndex != 0 ? (levelIndex % 2 != 1 ? (NetIndex16)300 : (NetIndex16)(kGhostIndexMax / 2 + 100)) : (DuckNetwork.localProfile == null ? (NetIndex16)(Rando.Int(kGhostIndexMax - 500) + 5) : (NetIndex16)((ushort)(int)DuckNetwork.localProfile.latestGhostIndex + 25));
            Clear();
        }

        public void Clear()
        {
            foreach (GhostObject ghost in _ghosts)
                ghost.ReleaseReferences();
            _ghosts.Clear();
            _ghostIndexMap.Clear();
            foreach (Profile profile in DuckNetwork.profiles)
                profile.removedGhosts.Clear();
            if (NetworkDebugger.enabled)
                NetworkDebugger.ClearGhostDebug();
            DevConsole.Log(DCSection.GhostMan, "Clearing all ghost data.");
            particleManager.Clear();
            _specialSyncMap.Clear();
            _destroyedGhosts.Clear();
            _framesSinceClear = 0;
        }

        public void Clear(NetworkConnection c)
        {
            bool flag = false;
            foreach (GhostObject ghost in _ghosts)
            {
                ghost.ClearConnectionData(c);
                flag = true;
            }
            if (Network.host != null)
            {
                foreach (GhostObject ghost in _ghosts)
                {
                    if (ghost.thing.connection == c)
                    {
                        Thing.SuperFondle(ghost.thing, Network.host);
                        flag = true;
                    }
                }
            }
            if (!flag)
                return;
            DevConsole.Log(DCSection.GhostMan, "Clearing ghost data for " + c.identifier);
        }

        public GhostObject GetGhost(NetIndex16 id)
        {
            GhostObject ghostObject = null;
            _ghostIndexMap.TryGetValue(id, out ghostObject);
            return ghostObject == null && pendingBitBufferGhosts.Count > 0 ? pendingBitBufferGhosts.FirstOrDefault(x => x.ghostObjectIndex == id) : ghostObject;
        }

        public GhostObject GetGhost(Thing thing) => thing.ghostObject;

        public int crashIndex;
        public void OnMessage(NetMessage m)
        {
            crashIndex = -1;
            string specialCode = "";
            try
            {
                switch (m)
                {
                    case NMParticles _:
                        crashIndex = 18;
                        NMParticles m1 = m as NMParticles;
                        if (m1.levelIndex != DuckNetwork.levelIndex)
                            break;
                        particleManager.OnMessage(m1);
                        break;
                    case NMParticlesRemoved _:
                        crashIndex = 17;
                        NMParticlesRemoved m2 = m as NMParticlesRemoved;
                        if (m2.levelIndex != DuckNetwork.levelIndex)
                            break;
                        particleManager.OnMessage(m2);
                        break;
                    case NMProfileNetData _:
                        crashIndex = 16;
                        NMProfileNetData nmProfileNetData = m as NMProfileNetData;
                        if (nmProfileNetData._profile == null || nmProfileNetData._netData == null)
                            break;
                        nmProfileNetData._profile.netData.Deserialize(nmProfileNetData._netData, nmProfileNetData.connection, false);
                        break;
                    case NMObjectNetData _:
                        crashIndex = 15;
                        NMObjectNetData nmObjectNetData = m as NMObjectNetData;
                        if (nmObjectNetData.thing == null || nmObjectNetData._netData == null)
                            break;
                        nmObjectNetData.thing.GetOrCreateNetData().Deserialize(nmObjectNetData._netData, nmObjectNetData.connection, !nmObjectNetData.thing.TransferControl(nmObjectNetData.connection, nmObjectNetData.authority));
                        break;
                    case NMRemoveGhosts _:
                        crashIndex = 14;
                        NMRemoveGhosts nmRemoveGhosts = m as NMRemoveGhosts;
                        if (nmRemoveGhosts.levelIndex != DuckNetwork.levelIndex)
                            break;
                        receivingDestroyMessage = true;
                        foreach (NetIndex16 id in nmRemoveGhosts.remove)
                        {
                            GhostObject ghost = GetGhost(id);
                            if (ghost != null)
                            {
                                ghost.thing.connection = m.connection;
                                RemoveGhost(ghost);
                            }
                        }
                        receivingDestroyMessage = false;
                        break;
                    case NMGhostData _:
                        specialCode = "NMGD01";
                        NMGhostData nmGhostData = m as NMGhostData;
                        specialCode = "NMGD02";
                        if (nmGhostData.levelIndex != DuckNetwork.levelIndex)
                            break;
                        specialCode = "NMGD03";
                        using (List<NMGhostState>.Enumerator enumerator = nmGhostData.states.GetEnumerator())
                        {
                            specialCode = "NMGD04";
                            while (enumerator.MoveNext())
                                ProcessGhostState(enumerator.Current);
                            break;
                        }
                    case NMGhostState _:
                        specialCode = "NMGS01";
                        NMGhostState pState = m as NMGhostState;
                        specialCode = "NMGS02";
                        if (pState.levelIndex != DuckNetwork.levelIndex)
                            break;
                        specialCode = "NMGS03";
                        ProcessGhostState(pState);
                        break;
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.GhostMan, "@error !! GHOST MANAGER UPDATE EXCEPTION", m.connection);
                DevConsole.Log("");
                DevConsole.Log(DCSection.GhostMan, ex.ToString(), m.connection);
                DevConsole.Log("");
                DevConsole.Log("");
                DevConsole.Log(DCSection.GhostMan, $"NM {m} specialCode:{specialCode} CCI:{crashIndex} hyperSpecial:{Main.SpecialCode2}");
                DevConsole.Log(DCSection.GhostMan, "IF YOU SEE THIS, PLEASE REPORT IT TO A DGR DEV");
                receivingDestroyMessage = false;
            }
        }

        private void ProcessGhostState(NMGhostState pState)
        {
            crashIndex = 0;
            Profile profile = GhostObject.IndexToProfile(pState.id);
            crashIndex = 1;
            if (profile != null && profile.removedGhosts.TryGetValue(pState.id, out GhostObject removedGhost))
            {
                crashIndex = 2;
                if (removedGhost != null)
                {
                    if (removedGhost.removeLogCooldown == 0)
                    {
                        DevConsole.Log(DCSection.GhostMan, "Ignoring removed ghost(" + removedGhost.ToString() + ")", pState.connection);
                        removedGhost.removeLogCooldown = 5;
                    }
                    else
                        --removedGhost.removeLogCooldown;
                }
                else
                    DevConsole.Log(DCSection.GhostMan, "Ignoring removed ghost(" + pState.ToString() + ")", pState.connection);
            }
            else
            {
                crashIndex = 3;
                GhostObject ghostObject = GetGhost(pState.id);
                if (pState.classID == 0)
                {
                    crashIndex = 4;
                    RemoveGhost(ghostObject, pState.id);
                }
                else
                {
                    crashIndex = 5;
                    Type t = Editor.IDToType[pState.classID];
                    long mask = pState.header.delta ? GhostObject.ReadMask(t, pState.data) : long.MaxValue;
                    crashIndex = 6;
                    if (ghostObject != null && (t != ghostObject.thing.GetType() || ghostObject.isDestroyed && mask == long.MaxValue))
                    {
                        crashIndex = 7;
                        receivingDestroyMessage = true;
                        changingGhostType = true;
                        RemoveGhost(ghostObject, ghostObject.ghostObjectIndex);
                        ghostObject = null;
                        receivingDestroyMessage = false;
                        changingGhostType = false;
                        crashIndex = 9;
                    }
                    if (ghostObject == null)
                    {
                        crashIndex = 8;
                        Thing thing = Editor.CreateThing(t);
                        thing.position = new Vec2(-2000f, -2000f);
                        Level.Add(thing);
                        thing.connection = pState.connection;
                        ghostObject = new GhostObject(thing, this, (int)pState.id);
                        ghostObject.ClearStateMask(pState.connection);
                        pState.ghost = ghostObject;
                        crashIndex = 10;
                        AddGhost(ghostObject);
                        if (pState.connection.profile != null && pState.id > pState.connection.profile.latestGhostIndex)
                            pState.connection.profile.latestGhostIndex = pState.id;
                    }
                    else
                    {
                        crashIndex = 100;
                        if (ghostObject.isDestroyed)
                        {
                            DevConsole.Log(DCSection.GhostMan, "Skipped ghost data (DESTROYED)(" + ghostObject.ghostObjectIndex.ToString() + ")", pState.connection);
                            return;
                        }
                        crashIndex = 101;
                        if (ghostObject.thing.isBitBufferCreatedGhostThing)
                        {
                            crashIndex = 102;
                            ghostObject.thing.isBitBufferCreatedGhostThing = false;
                            crashIndex = 103;
                            ghostObject.thing.level = null;
                            crashIndex = 104;
                            Level.Add(ghostObject.thing);
                        }
                        crashIndex = 105;
                        if (pState.header.connection != null)
                        {
                            crashIndex = 800;
                            ghostObject.thing.TransferControl(pState.header.connection, pState.authority);
                        }
                        else
                        {
                            crashIndex = 500;
                            ghostObject.thing.TransferControl(pState.connection, pState.authority);
                        }
                        crashIndex = 106;
                    }
                    if (NetworkDebugger.enabled && pState.connection.profile != null)
                        NetworkDebugger.GetGhost(ghostObject).dataReceivedFrames[pState.connection.profile.persona] = Graphics.frame;
                    crashIndex = 12;
                    if (ghostObject.thing.connection == pState.connection || ghostObject.thing.connection == pState.header.connection)
                    {
                        ghostObject.ReadInNetworkData(pState, mask, pState.connection, false);
                    }
                    else
                    {
                        for (int index = 0; index < Network.connections.Count; ++index)
                        {
                            NetworkConnection connection = Network.connections[index];
                            ghostObject.DirtyStateMask(mask, connection);
                        }
                    }
                    double x = ghostObject.thing.position.x;
                }
            }
            crashIndex = 13;
        }

        public void Notify(StreamManager pManager, NetMessage pMessage, bool pDropped)
        {
            if (pMessage is NMParticles || pMessage is NMParticlesRemoved)
                _particleManager.Notify(pMessage, pDropped);
            if (!pDropped)
                return;
            switch (pMessage)
            {
                case NMGhostState _:
                    NMGhostState nmGhostState = pMessage as NMGhostState;
                    if (nmGhostState.mask == 0L)
                    {
                        nmGhostState.ghost.GetConnectionData(pManager.connection).prevInputState = ushort.MaxValue;
                        break;
                    }
                    long mask1 = ~pManager.GetPendingStates(nmGhostState.ghost) & nmGhostState.mask;
                    nmGhostState.ghost.DirtyStateMask(mask1, nmGhostState.connection);
                    break;
                case NMGhostData _:
                    using (List<NMGhostData.GhostMaskPair>.Enumerator enumerator = (pMessage as NMGhostData).ghostMaskPairs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            NMGhostData.GhostMaskPair current = enumerator.Current;
                            if (current.mask == 0L)
                            {
                                current.ghost.GetConnectionData(pManager.connection).prevInputState = ushort.MaxValue;
                            }
                            else
                            {
                                long mask2 = ~pManager.GetPendingStates(current.ghost) & current.mask;
                                current.ghost.DirtyStateMask(mask2, pMessage.connection);
                            }
                        }
                        break;
                    }
                case NMProfileNetData _:
                    NMProfileNetData nmProfileNetData = pMessage as NMProfileNetData;
                    using (HashSet<int>.Enumerator enumerator = nmProfileNetData._hashes.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            int current = enumerator.Current;
                            nmProfileNetData._profile.netData.MakeDirty(current, nmProfileNetData.connection, nmProfileNetData.syncIndex);
                        }
                        break;
                    }
                case NMObjectNetData _:
                    NMObjectNetData nmObjectNetData = pMessage as NMObjectNetData;
                    if (nmObjectNetData.thing == null)
                        break;
                    using (HashSet<int>.Enumerator enumerator = nmObjectNetData._hashes.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            int current = enumerator.Current;
                            nmObjectNetData.thing._netData.MakeDirty(current, nmObjectNetData.connection, nmObjectNetData.syncIndex);
                        }
                        break;
                    }
                case NMRemoveGhosts _:
                    if (DuckNetwork.levelIndex != (pMessage as NMRemoveGhosts).levelIndex || Level.core.nextLevel != null || Level.current.networkIndex != (pMessage as NMRemoveGhosts).levelIndex)
                        break;
                    Send.Resend(pMessage);
                    break;
            }
        }

        public void IncrementPrediction() => ++(_managerState.thing as GhostManagerState).predictionIndex;

        public void RemoveLater(GhostObject g)
        {
            if (receivingDestroyMessage)
                return;
            _removeList.Add(g);
        }

        public void PostUpdate() => particleManager.Update();

        public void UpdateGhostLerp() //anticrash
        {
            try
            {
                ++_framesSinceClear;
                GhostManager.inGhostLoop = true;
                inGhostLerpLoop = true;
                int num = 0;
                foreach (GhostObject ghost in _ghosts)
                {
                    if (ghost.thing is IComplexUpdate)
                    {
                        if (ghost.IsInitialized())
                            (ghost.thing as IComplexUpdate).OnPreUpdate();
                        ++num;
                    }
                }
                foreach (GhostObject ghost in _ghosts)
                {
                    if (ghost.thing.connection != DuckNetwork.localConnection)
                    {
                        for (int index = 0; index < ghost.constipation; ++index)
                            ghost.UpdateState();
                        ghost.Update();
                    }
                    else
                        ghost.UpdateTick();
                }
                if (num > 0)
                {
                    foreach (GhostObject ghost in _ghosts)
                    {
                        if (ghost.thing is IComplexUpdate && ghost.IsInitialized())
                            (ghost.thing as IComplexUpdate).OnPostUpdate();
                    }
                }
                inGhostLerpLoop = false;
                foreach (GhostObject tempGhost in _tempGhosts)
                    _ghosts.Add(tempGhost);
                _tempGhosts.Clear();
                GhostManager.inGhostLoop = false;
            }
            catch
            { }
        }

        public void PostDraw()
        {
        }

        public void UpdateRemoval()
        {
            if (_destroyedGhosts.Count <= 0 && _destroyResends.Count <= 0)
                return;
            Send.Message(new NMRemoveGhosts(this), NetMessagePriority.Volatile);
        }

        public void RemoveGhost(GhostObject ghost, bool makeOld)
        {
            if (ghost == null)
                return;
            RemoveGhost(ghost, ghost.ghostObjectIndex);
        }

        public void RemoveGhost(GhostObject ghost)
        {
            if (ghost == null)
                return;
            RemoveGhost(ghost, ghost.ghostObjectIndex);
        }

        public void RemoveGhost(GhostObject ghost, NetIndex16 ghostIndex) => RemoveGhost(ghost, ghostIndex, false);

        public void RemoveGhost(GhostObject ghost, NetIndex16 ghostIndex, bool viaGhostMessage = false)
        {
            if (ghost == null)
                return;
            _ghosts.Remove(ghost);
            if (ghost.thing != null && !ghost.thing.removeFromLevel)
                Level.Remove(ghost.thing);
            if (!receivingDestroyMessage && ghost.thing != null && ghost.thing.isServerForObject)
                _destroyedGhosts.Add(ghost);
            _ghostIndexMap.Remove(ghost.ghostObjectIndex);
            if (!changingGhostType && _framesSinceClear > 60)
            {
                Profile profile = GhostObject.IndexToProfile(ghost.ghostObjectIndex);
                if (profile != null && profile.connection != null)
                    profile.removedGhosts[ghost.ghostObjectIndex] = ghost;
            }
            ghost.isOldGhost = true;
            ghost.ReleaseReferences(false);
        }

        public void MapSpecialSync(Thing t, ushort index) => _specialSyncMap[index] = t;

        public Thing GetSpecialSync(ushort index)
        {
            Thing specialSync = null;
            if (!_specialSyncMap.TryGetValue(index, out specialSync))
            {
                specialSync = Level.current.things.First(x => x.specialSyncIndex == index);
                if (specialSync != null)
                    _specialSyncMap[index] = specialSync;
            }
            return specialSync;
        }

        public GhostObject MakeGhost(Thing t, int index = -1, bool initLevel = false)
        {
            Main.SpecialCode2 = "1011801";
            if (t.ghostObject != null)
                return t.ghostObject;
            Main.SpecialCode2 = "1011802";
            GhostObject pGhost = new GhostObject(t, this, index, initLevel);
            Main.SpecialCode2 = "1011803";
            AddGhost(pGhost);
            Main.SpecialCode2 = "1011804";
            return pGhost;
        }

        public GhostObject MakeGhostLater(Thing t, int index = -1, bool initLevel = false)
        {
            bool inGhostLerpLoop = this.inGhostLerpLoop;
            this.inGhostLerpLoop = true;
            GhostObject ghostObject = MakeGhost(t, index, initLevel);
            this.inGhostLerpLoop = inGhostLerpLoop;
            return ghostObject;
        }

        internal void AddGhost(GhostObject pGhost)
        {
            Main.SpecialCode2 = "10118031";
            if (inGhostLerpLoop)
            {
            Main.SpecialCode2 = "10118032";
                _tempGhosts.Add(pGhost);
            }
            else
            {
                Main.SpecialCode2 = "10118033";
                if (_ghosts.Contains(pGhost) || pGhost.thing == null)
                    return;
                _ghosts.Add(pGhost);
                Main.SpecialCode2 = "10118034";
                pGhost.thing.OnGhostObjectAdded();
            }
            Main.SpecialCode2 = "10118035";
        }

        public void MapGhost(Thing pThing, GhostObject pGhost) => AddGhost(pGhost);

        public void RefreshGhosts(Level lev = null)
        {
            if (lev == null)
                lev = Level.current;
            if (lev.things.objectsDirty)
            {
                Thing[] thingArray = new Thing[lev.things.updateList.Count];
                lev.things.updateList.CopyTo(thingArray);
                Array.Sort(thingArray, helperPhysicsIndexSorter);
                if (thingArray != null)
                {
                    int num = thingArray.Length;
                    for (int index = 0; index < num; ++index)
                    {
                        Thing thing = thingArray[index];
                        if (thing.isStateObject && !thing.removeFromLevel && !thing.ignoreGhosting && thing.ghostObject == null)
                            AddGhost(new GhostObject(thing, this));
                    }
                }
                lev.things.objectsDirty = false;
            }
            int num1 = 0;
            List<NetworkConnection> connections = Network.activeNetwork.core.connections;
            foreach (GhostObject ghost in _ghosts)
            {
                if (ghost.thing.isServerForObject)
                    ghost.RefreshStateMask(connections);
                ++num1;
            }
            foreach (GhostObject ghost in _ghosts)
            {
                if (ghost.shouldRemove && ghost.thing != null)
                {
                    if (!ghost.thing.removeFromLevel)
                        Level.Remove(ghost.thing);
                    RemoveLater(ghost);
                }
            }
            foreach (GhostObject remove in _removeList)
            {
                if (remove.thing != null)
                    remove.thing.ghostType = 0;
                RemoveGhost(remove, remove.ghostObjectIndex);
            }
            _removeList.Clear();
            if (Level.core.nextLevel != null || !Level.current.initializeFunctionHasBeenRun)
                return;
            UpdateRemoval();
        }

        public void UpdateInit()
        {
            if (_managerState != null)
                return;
            _managerState = new GhostObject(new GhostManagerState(), this, 0);
        }

        public void OnDisconnect(NetworkConnection connection)
        {
            foreach (GhostObject ghost in _ghosts)
            {
                ghost.DirtyStateMask(long.MaxValue, connection);
                if (ghost.thing._netData != null)
                    ghost.thing._netData.MakeDirty(int.MaxValue, connection, (NetIndex16)0);
            }
            foreach (Profile profile in DuckNetwork.profiles)
                profile.netData.MakeDirty(int.MaxValue, connection, (NetIndex16)0);
        }

        public void PreUpdate()
        {
        }

        public void Update(NetworkConnection connection, bool sendPackets)
        {
            if (!sendPackets)
                return;
            UpdateGhostSync(connection, true, true);
        }

        public void UpdateRemovalMessages()
        {
        }

        public List<NMGhostData> UpdateGhostSync(
          NetworkConnection pConnection,
          bool pDelta,
          bool pSendMessages,
          NetMessagePriority pPriority = NetMessagePriority.Volatile)
        {
            List<NMGhostData> nmGhostDataList = new List<NMGhostData>();
            List<GhostObject> pGhosts = new List<GhostObject>();
            foreach (GhostObject ghost in _ghosts)
            {
                if (ghost.thing.connection == null)
                    ghost.thing.connection = Network.host;
                if (!pDelta || ghost.thing.connection == DuckNetwork.localConnection || ghost.thing.connection == null && Network.isServer)
                {
                    if (!ghost.thing.isInitialized)
                        ghost.thing.DoInitialize();
                    if (pDelta & pSendMessages && ghost.thing._netData != null && ghost.thing._netData.IsDirty(pConnection))
                    {
                        Send.Message(new NMObjectNetData(ghost.thing, pConnection), NetMessagePriority.Volatile, pConnection);
                        ghost.thing._netData.Clean(pConnection);
                    }
                    if (!pDelta || ghost.NeedsSync(pConnection))
                    {
                        if (pDelta)
                        {
                            ghost.previouslySerializedData = ghost.GetNetworkStateData(pConnection, true);
                        }
                        else
                        {
                            ghost.previouslySerializedData = ghost.GetNetworkStateData();
                            ghost.ClearStateMask(pConnection, ghost.thing.authority);
                        }
                        if (pDelta)
                        {
                            int index1 = 0;
                            for (int index2 = 0; index2 < pGhosts.Count; ++index2)
                            {
                                if (ghost.thing is Duck)
                                {
                                    index1 = 0;
                                    break;
                                }
                                if (pGhosts[index2].thing is Duck)
                                    ++index1;
                                if (pGhosts[index2].thing.GetType() == ghost.thing.GetType())
                                {
                                    index1 = index2;
                                    break;
                                }
                            }
                            pGhosts.Insert(index1, ghost);
                        }
                        else
                            pGhosts.Add(ghost);
                    }
                }
            }
            int pStartIndex = 0;
            while (pStartIndex < pGhosts.Count)
            {
                NMGhostData serializedGhostData = NMGhostData.GetSerializedGhostData(pGhosts, pStartIndex);
                nmGhostDataList.Add(serializedGhostData);
                pStartIndex += serializedGhostData.ghostMaskPairs.Count;
                if (pSendMessages)
                    Send.Message(serializedGhostData, pPriority, pConnection);
            }
            return nmGhostDataList;
        }

        private class HelperPhysicsIndexSorter : IComparer<Thing>
        {
            int IComparer<Thing>.Compare(Thing a, Thing b) => a.physicsIndex - b.physicsIndex;
        }
    }
}
