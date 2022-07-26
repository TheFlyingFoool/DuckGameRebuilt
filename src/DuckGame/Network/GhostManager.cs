// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private static GhostManager.HelperPhysicsIndexSorter helperPhysicsIndexSorter = new GhostManager.HelperPhysicsIndexSorter();
        private HashSet<GhostObject> _removeList = new HashSet<GhostObject>();

        public void UpdateSynchronizedEvents()
        {
            for (int index = 0; index < this._synchronizedEvents.Count; ++index)
            {
                if (this._synchronizedEvents[index].Update())
                {
                    DevConsole.Log(DCSection.DuckNet, "@received Activating |WHITE|" + this._synchronizedEvents[index].ToString() + "|PREV|", DuckNetwork.localConnection);
                    if (this._synchronizedEvents[index] is NMSynchronizedEvent)
                        (this._synchronizedEvents[index] as NMSynchronizedEvent).Activate();
                    this._synchronizedEvents.RemoveAt(index);
                    --index;
                }
            }
        }

        public NetParticleManager particleManager => this._particleManager;

        public NetIndex16 predictionIndex
        {
            get => (this._managerState.thing as GhostManagerState).predictionIndex;
            set => (this._managerState.thing as GhostManagerState).predictionIndex = value;
        }

        public void TransferPendingGhosts()
        {
            foreach (GhostObject pendingBitBufferGhost in this.pendingBitBufferGhosts)
                this.AddGhost(pendingBitBufferGhost);
            this.pendingBitBufferGhosts.Clear();
        }

        public static GhostManager context => Network.activeNetwork.core.ghostManager;

        public GhostManager() => this.globalID = GhostManager.kglobalID++;

        public NetIndex16 currentGhostIndex => this.ghostObjectIndex;

        public NetIndex16 GetGhostIndex() => this.GetGhostIndex(false);

        public NetIndex16 GetGhostIndex(bool levelInit)
        {
            int fixedGhostIndex = (int)DuckNetwork.localProfile.fixedGhostIndex;
            if (levelInit)
                fixedGhostIndex = (int)DuckNetwork.hostProfile.fixedGhostIndex;
            NetIndex16 ghostObjectIndex1 = this.ghostObjectIndex;
            while (this._ghostIndexMap.ContainsKey(this.ghostObjectIndex + fixedGhostIndex * GhostManager.kGhostIndexMax))
            {
                ++this.ghostObjectIndex;
                if (this.ghostObjectIndex > GhostManager.kGhostIndexMax - 10)
                    this.ghostObjectIndex = (NetIndex16)32;
                if (this.ghostObjectIndex == ghostObjectIndex1 || ghostObjectIndex1 < 32)
                    break;
            }
            NetIndex16 ghostObjectIndex2 = this.ghostObjectIndex;
            ++this.ghostObjectIndex;
            return ghostObjectIndex2 + fixedGhostIndex * GhostManager.kGhostIndexMax;
        }

        public void SetGhostIndex(NetIndex16 idx)
        {
            this.ghostObjectIndex = idx;
            this.Clear();
        }

        public void ResetGhostIndex(byte levelIndex)
        {
            this.ghostObjectIndex = levelIndex != (byte)0 ? ((int)levelIndex % 2 != 1 ? (NetIndex16)300 : (NetIndex16)(GhostManager.kGhostIndexMax / 2 + 100)) : (DuckNetwork.localProfile == null ? (NetIndex16)(Rando.Int(GhostManager.kGhostIndexMax - 500) + 5) : (NetIndex16)((int)(ushort)(int)DuckNetwork.localProfile.latestGhostIndex + 25));
            this.Clear();
        }

        public void Clear()
        {
            foreach (GhostObject ghost in this._ghosts)
                ghost.ReleaseReferences();
            this._ghosts.Clear();
            this._ghostIndexMap.Clear();
            foreach (Profile profile in DuckNetwork.profiles)
                profile.removedGhosts.Clear();
            if (NetworkDebugger.enabled)
                NetworkDebugger.ClearGhostDebug();
            DevConsole.Log(DCSection.GhostMan, "Clearing all ghost data.");
            this.particleManager.Clear();
            this._specialSyncMap.Clear();
            this._destroyedGhosts.Clear();
            this._framesSinceClear = 0;
        }

        public void Clear(NetworkConnection c)
        {
            bool flag = false;
            foreach (GhostObject ghost in this._ghosts)
            {
                ghost.ClearConnectionData(c);
                flag = true;
            }
            if (Network.host != null)
            {
                foreach (GhostObject ghost in this._ghosts)
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
            GhostObject ghostObject = (GhostObject)null;
            this._ghostIndexMap.TryGetValue(id, out ghostObject);
            return ghostObject == null && this.pendingBitBufferGhosts.Count > 0 ? this.pendingBitBufferGhosts.FirstOrDefault<GhostObject>((Func<GhostObject, bool>)(x => x.ghostObjectIndex == id)) : ghostObject;
        }

        public GhostObject GetGhost(Thing thing) => thing.ghostObject;

        public void OnMessage(NetMessage m)
        {
            try
            {
                switch (m)
                {
                    case NMParticles _:
                        NMParticles m1 = m as NMParticles;
                        if ((int)m1.levelIndex != (int)DuckNetwork.levelIndex)
                            break;
                        this.particleManager.OnMessage((NetMessage)m1);
                        break;
                    case NMParticlesRemoved _:
                        NMParticlesRemoved m2 = m as NMParticlesRemoved;
                        if ((int)m2.levelIndex != (int)DuckNetwork.levelIndex)
                            break;
                        this.particleManager.OnMessage((NetMessage)m2);
                        break;
                    case NMProfileNetData _:
                        NMProfileNetData nmProfileNetData = m as NMProfileNetData;
                        if (nmProfileNetData._profile == null || nmProfileNetData._netData == null)
                            break;
                        nmProfileNetData._profile.netData.Deserialize(nmProfileNetData._netData, nmProfileNetData.connection, false);
                        break;
                    case NMObjectNetData _:
                        NMObjectNetData nmObjectNetData = m as NMObjectNetData;
                        if (nmObjectNetData.thing == null || nmObjectNetData._netData == null)
                            break;
                        nmObjectNetData.thing.GetOrCreateNetData().Deserialize(nmObjectNetData._netData, nmObjectNetData.connection, !nmObjectNetData.thing.TransferControl(nmObjectNetData.connection, nmObjectNetData.authority));
                        break;
                    case NMRemoveGhosts _:
                        NMRemoveGhosts nmRemoveGhosts = m as NMRemoveGhosts;
                        if ((int)nmRemoveGhosts.levelIndex != (int)DuckNetwork.levelIndex)
                            break;
                        GhostManager.receivingDestroyMessage = true;
                        foreach (NetIndex16 id in nmRemoveGhosts.remove)
                        {
                            GhostObject ghost = this.GetGhost(id);
                            if (ghost != null)
                            {
                                ghost.thing.connection = m.connection;
                                this.RemoveGhost(ghost);
                            }
                        }
                        GhostManager.receivingDestroyMessage = false;
                        break;
                    case NMGhostData _:
                        NMGhostData nmGhostData = m as NMGhostData;
                        if ((int)nmGhostData.levelIndex != (int)DuckNetwork.levelIndex)
                            break;
                        using (List<NMGhostState>.Enumerator enumerator = nmGhostData.states.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                                this.ProcessGhostState(enumerator.Current);
                            break;
                        }
                    case NMGhostState _:
                        NMGhostState pState = m as NMGhostState;
                        if ((int)pState.levelIndex != (int)DuckNetwork.levelIndex)
                            break;
                        this.ProcessGhostState(pState);
                        break;
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.GhostMan, "@error !! GHOST MANAGER UPDATE EXCEPTION", m.connection);
                DevConsole.Log(DCSection.GhostMan, ex.ToString(), m.connection);
                GhostManager.receivingDestroyMessage = false;
            }
        }

        private void ProcessGhostState(NMGhostState pState)
        {
            Profile profile = GhostObject.IndexToProfile(pState.id);
            if (profile != null && profile.removedGhosts.ContainsKey(pState.id))
            {
                GhostObject removedGhost = profile.removedGhosts[pState.id];
                if (removedGhost != null)
                {
                    if (removedGhost.removeLogCooldown == (byte)0)
                    {
                        DevConsole.Log(DCSection.GhostMan, "Ignoring removed ghost(" + removedGhost.ToString() + ")", pState.connection);
                        removedGhost.removeLogCooldown = (byte)5;
                    }
                    else
                        --removedGhost.removeLogCooldown;
                }
                else
                    DevConsole.Log(DCSection.GhostMan, "Ignoring removed ghost(" + pState.ToString() + ")", pState.connection);
            }
            else
            {
                GhostObject ghostObject = this.GetGhost(pState.id);
                if (pState.classID == (ushort)0)
                {
                    this.RemoveGhost(ghostObject, pState.id);
                }
                else
                {
                    System.Type t = Editor.IDToType[pState.classID];
                    long mask = pState.header.delta ? GhostObject.ReadMask(t, pState.data) : long.MaxValue;
                    if (ghostObject != null && (t != ghostObject.thing.GetType() || ghostObject.isDestroyed && mask == long.MaxValue))
                    {
                        GhostManager.receivingDestroyMessage = true;
                        GhostManager.changingGhostType = true;
                        this.RemoveGhost(ghostObject, ghostObject.ghostObjectIndex);
                        ghostObject = (GhostObject)null;
                        GhostManager.receivingDestroyMessage = false;
                        GhostManager.changingGhostType = false;
                    }
                    if (ghostObject == null)
                    {
                        Thing thing = Editor.CreateThing(t);
                        thing.position = new Vec2(-2000f, -2000f);
                        Level.Add(thing);
                        thing.connection = pState.connection;
                        ghostObject = new GhostObject(thing, this, (int)pState.id);
                        ghostObject.ClearStateMask(pState.connection);
                        pState.ghost = ghostObject;
                        this.AddGhost(ghostObject);
                        if (pState.connection.profile != null && pState.id > pState.connection.profile.latestGhostIndex)
                            pState.connection.profile.latestGhostIndex = pState.id;
                    }
                    else
                    {
                        if (ghostObject.isDestroyed)
                        {
                            DevConsole.Log(DCSection.GhostMan, "Skipped ghost data (DESTROYED)(" + ghostObject.ghostObjectIndex.ToString() + ")", pState.connection);
                            return;
                        }
                        if (ghostObject.thing.isBitBufferCreatedGhostThing)
                        {
                            ghostObject.thing.isBitBufferCreatedGhostThing = false;
                            ghostObject.thing.level = (Level)null;
                            Level.Add(ghostObject.thing);
                        }
                        if (pState.header.connection != null)
                            ghostObject.thing.TransferControl(pState.header.connection, pState.authority);
                        else
                            ghostObject.thing.TransferControl(pState.connection, pState.authority);
                    }
                    if (NetworkDebugger.enabled && pState.connection.profile != null)
                        NetworkDebugger.GetGhost(ghostObject).dataReceivedFrames[pState.connection.profile.persona] = Graphics.frame;
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
                    double x = (double)ghostObject.thing.position.x;
                }
            }
        }

        public void Notify(StreamManager pManager, NetMessage pMessage, bool pDropped)
        {
            if (pMessage is NMParticles || pMessage is NMParticlesRemoved)
                this._particleManager.Notify(pMessage, pDropped);
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
                    if ((int)DuckNetwork.levelIndex != (int)(pMessage as NMRemoveGhosts).levelIndex || Level.core.nextLevel != null || (int)Level.current.networkIndex != (int)(pMessage as NMRemoveGhosts).levelIndex)
                        break;
                    Send.Resend(pMessage);
                    break;
            }
        }

        public void IncrementPrediction() => ++(this._managerState.thing as GhostManagerState).predictionIndex;

        public void RemoveLater(GhostObject g)
        {
            if (GhostManager.receivingDestroyMessage)
                return;
            this._removeList.Add(g);
        }

        public void PostUpdate() => this.particleManager.Update();

        public void UpdateGhostLerp()
        {
            ++this._framesSinceClear;
            GhostManager.inGhostLoop = true;
            this.inGhostLerpLoop = true;
            int num = 0;
            foreach (GhostObject ghost in this._ghosts)
            {
                if (ghost.thing is IComplexUpdate)
                {
                    if (ghost.IsInitialized())
                        (ghost.thing as IComplexUpdate).OnPreUpdate();
                    ++num;
                }
            }
            foreach (GhostObject ghost in this._ghosts)
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
                foreach (GhostObject ghost in this._ghosts)
                {
                    if (ghost.thing is IComplexUpdate && ghost.IsInitialized())
                        (ghost.thing as IComplexUpdate).OnPostUpdate();
                }
            }
            this.inGhostLerpLoop = false;
            foreach (GhostObject tempGhost in this._tempGhosts)
                this._ghosts.Add(tempGhost);
            this._tempGhosts.Clear();
            GhostManager.inGhostLoop = false;
        }

        public void PostDraw()
        {
        }

        public void UpdateRemoval()
        {
            if (this._destroyedGhosts.Count <= 0 && this._destroyResends.Count <= 0)
                return;
            Send.Message((NetMessage)new NMRemoveGhosts(this), NetMessagePriority.Volatile);
        }

        public void RemoveGhost(GhostObject ghost, bool makeOld)
        {
            if (ghost == null)
                return;
            this.RemoveGhost(ghost, ghost.ghostObjectIndex);
        }

        public void RemoveGhost(GhostObject ghost)
        {
            if (ghost == null)
                return;
            this.RemoveGhost(ghost, ghost.ghostObjectIndex);
        }

        public void RemoveGhost(GhostObject ghost, NetIndex16 ghostIndex) => this.RemoveGhost(ghost, ghostIndex, false);

        public void RemoveGhost(GhostObject ghost, NetIndex16 ghostIndex, bool viaGhostMessage = false)
        {
            if (ghost == null)
                return;
            this._ghosts.Remove(ghost);
            if (ghost.thing != null && !ghost.thing.removeFromLevel)
                Level.Remove(ghost.thing);
            if (!GhostManager.receivingDestroyMessage && ghost.thing != null && ghost.thing.isServerForObject)
                this._destroyedGhosts.Add(ghost);
            this._ghostIndexMap.Remove(ghost.ghostObjectIndex);
            if (!GhostManager.changingGhostType && this._framesSinceClear > 60)
            {
                Profile profile = GhostObject.IndexToProfile(ghost.ghostObjectIndex);
                if (profile != null && profile.connection != null)
                    profile.removedGhosts[ghost.ghostObjectIndex] = ghost;
            }
            ghost.isOldGhost = true;
            ghost.ReleaseReferences(false);
        }

        public void MapSpecialSync(Thing t, ushort index) => this._specialSyncMap[index] = t;

        public Thing GetSpecialSync(ushort index)
        {
            Thing specialSync = (Thing)null;
            if (!this._specialSyncMap.TryGetValue(index, out specialSync))
            {
                specialSync = Level.current.things.First<Thing>((Func<Thing, bool>)(x => (int)x.specialSyncIndex == (int)index));
                if (specialSync != null)
                    this._specialSyncMap[index] = specialSync;
            }
            return specialSync;
        }

        public GhostObject MakeGhost(Thing t, int index = -1, bool initLevel = false)
        {
            if (t.ghostObject != null)
                return t.ghostObject;
            GhostObject pGhost = new GhostObject(t, this, index, initLevel);
            this.AddGhost(pGhost);
            return pGhost;
        }

        public GhostObject MakeGhostLater(Thing t, int index = -1, bool initLevel = false)
        {
            bool inGhostLerpLoop = this.inGhostLerpLoop;
            this.inGhostLerpLoop = true;
            GhostObject ghostObject = this.MakeGhost(t, index, initLevel);
            this.inGhostLerpLoop = inGhostLerpLoop;
            return ghostObject;
        }

        internal void AddGhost(GhostObject pGhost)
        {
            if (this.inGhostLerpLoop)
            {
                this._tempGhosts.Add(pGhost);
            }
            else
            {
                if (this._ghosts.Contains(pGhost) || pGhost.thing == null)
                    return;
                this._ghosts.Add(pGhost);
                pGhost.thing.OnGhostObjectAdded();
            }
        }

        public void MapGhost(Thing pThing, GhostObject pGhost) => this.AddGhost(pGhost);

        public void RefreshGhosts(Level lev = null)
        {
            if (lev == null)
                lev = Level.current;
            if (lev.things.objectsDirty)
            {
                Thing[] thingArray = new Thing[lev.things.updateList.Count];
                lev.things.updateList.CopyTo(thingArray);
                Array.Sort<Thing>(thingArray, (IComparer<Thing>)GhostManager.helperPhysicsIndexSorter);
                if (thingArray != null)
                {
                    int num = ((IEnumerable<Thing>)thingArray).Count<Thing>();
                    for (int index = 0; index < num; ++index)
                    {
                        Thing thing = thingArray[index];
                        if (thing.isStateObject && !thing.removeFromLevel && !thing.ignoreGhosting && thing.ghostObject == null)
                            this.AddGhost(new GhostObject(thing, this));
                    }
                }
                lev.things.objectsDirty = false;
            }
            int num1 = 0;
            List<NetworkConnection> connections = Network.activeNetwork.core.connections;
            foreach (GhostObject ghost in this._ghosts)
            {
                if (ghost.thing.isServerForObject)
                    ghost.RefreshStateMask(connections);
                ++num1;
            }
            foreach (GhostObject ghost in this._ghosts)
            {
                if (ghost.shouldRemove && ghost.thing != null)
                {
                    if (!ghost.thing.removeFromLevel)
                        Level.Remove(ghost.thing);
                    this.RemoveLater(ghost);
                }
            }
            foreach (GhostObject remove in this._removeList)
            {
                if (remove.thing != null)
                    remove.thing.ghostType = (ushort)0;
                this.RemoveGhost(remove, remove.ghostObjectIndex);
            }
            this._removeList.Clear();
            if (Level.core.nextLevel != null || !Level.current.initializeFunctionHasBeenRun)
                return;
            this.UpdateRemoval();
        }

        public void UpdateInit()
        {
            if (this._managerState != null)
                return;
            this._managerState = new GhostObject((Thing)new GhostManagerState(), this, 0);
        }

        public void OnDisconnect(NetworkConnection connection)
        {
            foreach (GhostObject ghost in this._ghosts)
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
            this.UpdateGhostSync(connection, true, true);
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
            foreach (GhostObject ghost in this._ghosts)
            {
                if (ghost.thing.connection == null)
                    ghost.thing.connection = Network.host;
                if (!pDelta || ghost.thing.connection == DuckNetwork.localConnection || ghost.thing.connection == null && Network.isServer)
                {
                    if (!ghost.thing.isInitialized)
                        ghost.thing.DoInitialize();
                    if (pDelta & pSendMessages && ghost.thing._netData != null && ghost.thing._netData.IsDirty(pConnection))
                    {
                        Send.Message((NetMessage)new NMObjectNetData(ghost.thing, pConnection), NetMessagePriority.Volatile, pConnection);
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
                    Send.Message((NetMessage)serializedGhostData, pPriority, pConnection);
            }
            return nmGhostDataList;
        }

        private class HelperPhysicsIndexSorter : IComparer<Thing>
        {
            int IComparer<Thing>.Compare(Thing a, Thing b) => (int)a.physicsIndex - (int)b.physicsIndex;
        }
    }
}
