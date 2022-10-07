// Decompiled with JetBrains decompiler
// Type: DuckGame.Network
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
    public class Network
    {
        private int _networkIndex;
        private static int _simTick;
        private NCNetworkImplementation _core;
        private NCNetworkImplementation _lanCore;
        public static bool lanMode = false;
        private static Network _activeNetwork;
        private NetIndex16 _tickSync = new NetIndex16(1, true);

        public NetIndex16 _lastReceivedTime = new NetIndex16(1, true);

        public NetIndex16 _synchronizedTime = new NetIndex16(1, true);
        private uint _currentTick;
        private static Map<ushort, ConstructorInfo> _constructorToMessageID = new Map<ushort, ConstructorInfo>();
        private static Map<ushort, System.Type> _typeToMessageID = new Map<ushort, System.Type>();
        private static Dictionary<System.Type, ushort> _allMessageTypesToID = new Dictionary<System.Type, ushort>();
        private static IEnumerable<NetMessagePriority> _netMessagePriorities;
        public static List<string> synchronizedTriggers = new List<string>()
    {
      "LEFT",
      "RIGHT",
      "UP",
      "DOWN",
      "SHOOT",
      "JUMP",
      "GRAB",
      "QUACK",
      "START",
      "RAGDOLL",
      "STRAFE"
    };
        private static int _inputDelayFrames = 0;
        public bool _networkActive;
        public static uint messageTypeHash;

        public static bool SimulateBadnet => false;

        public int networkIndex => _networkIndex;

        public Network(int networkIndex = 0) => _networkIndex = networkIndex;

        public static int simTick
        {
            get => Network._simTick;
            set => Network._simTick = value;
        }

        public static bool available => Steam.IsInitialized() && Steam.user != null;

        public NCNetworkImplementation core
        {
            get => !Network.lanMode ? _core : _lanCore;
            set
            {
                if (Network.lanMode)
                    _lanCore = value;
                else
                    _core = value;
            }
        }

        public static Network activeNetwork
        {
            get
            {
                if (Network._activeNetwork == null)
                    Network._activeNetwork = new Network();
                return Network._activeNetwork;
            }
            set => Network._activeNetwork = value;
        }

        public static int frame
        {
            get => Network.activeNetwork.core.frame;
            set => Network.activeNetwork.core.frame = value;
        }

        public static NetGraph netGraph => Network._activeNetwork.core.netGraph;

        public static void ContextSwitch(byte pLevelIndex)
        {
            GhostManager.context.Clear();
            DevConsole.Log(DCSection.GhostMan, "|DGYELLOW|ContextSwitch (" + DuckNetwork.levelIndex.ToString() + "->" + pLevelIndex.ToString() + ")");
            DuckNetwork.levelIndex = pLevelIndex;
            if (pLevelIndex == 0)
                GhostManager.context.ResetGhostIndex(pLevelIndex);
            foreach (Profile profile in Profiles.active)
            {
                if (profile.connection != null)
                    profile.connection.manager.Reset();
            }
        }

        public NetIndex16 synchTime => !core.isServer ? _synchronizedTime : _tickSync;

        public static NetIndex16 synchronizedTime => Network.activeNetwork.synchTime;

        public static void ReceiveHostTime(NetIndex16 pTime)
        {
            if (!(Network.activeNetwork._lastReceivedTime < pTime))
                return;
            Network.activeNetwork._synchronizedTime = pTime + (ushort)(Network.host.manager.ping / 2.0 / Maths.IncFrameTimer());
            Network.activeNetwork._lastReceivedTime = pTime;
        }

        public static double Time => 0.0;

        public static uint Tick => Network.activeNetwork._currentTick;

        public static NetIndex16 TickSync => Network.activeNetwork._tickSync;

        public static float ping => Network.activeNetwork.core.averagePing;

        public static float highestPing
        {
            get
            {
                float highestPing = 0f;
                foreach (NetworkConnection connection in Network.connections)
                {
                    if (connection.status == ConnectionStatus.Connected && connection.manager.ping > highestPing)
                        highestPing = connection.manager.ping;
                }
                return highestPing;
            }
        }

        public static NetworkConnection host
        {
            get
            {
                if (DuckNetwork.hostProfile != null && DuckNetwork.hostProfile.connection != null)
                    return DuckNetwork.hostProfile.connection;
                if (DuckNetwork.localConnection.isHost)
                    return DuckNetwork.localConnection;
                foreach (NetworkConnection connection in Network.connections)
                {
                    if (connection.isHost)
                        return connection;
                }
                return null;
            }
        }

        public static Map<ushort, ConstructorInfo> constructorToMessageID => Network._constructorToMessageID;

        public static Map<ushort, System.Type> typeToMessageID => Network._typeToMessageID;

        public static Dictionary<System.Type, ushort> allMessageTypesToID => Network._allMessageTypesToID;

        public static IEnumerable<NetMessagePriority> netMessagePriorities => Network._netMessagePriorities;

        public static int inputDelayFrames
        {
            get => Network._inputDelayFrames;
            set => Network._inputDelayFrames = value;
        }

        public static bool hasHostConnection
        {
            get
            {
                if (DuckNetwork.localConnection.isHost)
                    return true;
                foreach (NetworkConnection connection in Network.connections)
                {
                    if (connection.isHost)
                        return true;
                }
                return false;
            }
        }

        public static bool canSetObservers
        {
            get
            {
                if (DuckNetwork.isDedicatedServer)
                    return false;
                bool canSetObservers = Network.isServer && Network.lanMode;
                if (Network.isServer && Steam.lobby != null && Steam.lobby.type != SteamLobbyType.Public)
                    canSetObservers = true;
                if (!Network.InLobby())
                    canSetObservers = false;
                return canSetObservers;
            }
        }

        public static bool isServer
        {
            get => Network.activeNetwork.core.isServer;
            set => Network.activeNetwork.core.isServer = value;
        }

        public static bool isClient => !Network.isServer;

        public static void MakeActive() => Network.activeNetwork._networkActive = true;

        public static void MakeInactive() => Network.activeNetwork._networkActive = false;

        public static bool isActive => Network.activeNetwork._networkActive;

        public static bool connected => Network.connections.Count > 0;

        public static List<NetworkConnection> connections => Network.activeNetwork.core.connections;

        public System.Type GetClassType(string name)
        {
            string fullName = typeof(Duck).Assembly.FullName;
            return Editor.GetType("DuckGame." + name + ", " + fullName);
        }

        public static void JoinServer(string nameVal, int portVal = 1337, string ip = "localhost") => Network.activeNetwork.DoJoinServer(nameVal, portVal, ip);

        private void DoJoinServer(string nameVal, int portVal = 1337, string ip = "localhost") => core.JoinServer(nameVal, portVal, ip);

        public static void HostServer(
          NetworkLobbyType lobbyType,
          int maxConnectionsVal = 32,
          string nameVal = "duckGameServer",
          int portVal = 1337)
        {
            Network.activeNetwork.DoHostServer(lobbyType, maxConnectionsVal, nameVal, portVal);
        }

        private void DoHostServer(
          NetworkLobbyType lobbyType,
          int maxConnectionsVal = 32,
          string nameVal = "duckGameServer",
          int portVal = 1337)
        {
            core.HostServer(nameVal, portVal, lobbyType, maxConnectionsVal);
        }

        public static void OnMessageStatic(NetMessage m) => Network._activeNetwork.OnMessage(m);

        private void OnMessage(NetMessage m)
        {
            if (m is NMConsoleMessage)
                DevConsole.Log((m as NMConsoleMessage).message, Color.Lime);
            else if (Network.isServer)
                OnMessageServer(m);
            else
                OnMessageClient(m);
        }

        private void OnMessageServer(NetMessage m) => Level.current.OnMessage(m);

        private void OnMessageClient(NetMessage m) => Level.current.OnMessage(m);

        public void OnConnection(NetworkConnection connection) => UIMatchmakingBox.core.pulseNetwork = true;

        public void ImmediateUnreliableBroadcast(NetMessage pMessage)
        {
            if (!Network.isActive)
                return;
            pMessage.Serialize();
            bool flag = false;
            foreach (NetworkConnection connection in Network.connections)
            {
                NetMessage pMessage1 = pMessage;
                if (flag)
                {
                    pMessage1 = Activator.CreateInstance(pMessage.GetType(), null) as NetMessage;
                    pMessage1.priority = pMessage.priority;
                    pMessage1.SetSerializedData(pMessage.serializedData);
                }
                connection.manager.SendImmediatelyUnreliable(pMessage1);
                flag = true;
            }
        }

        public void ImmediateUnreliableMessage(NetMessage pMessage, NetworkConnection pConnection)
        {
            if (!Network.isActive)
                return;
            pMessage.Serialize();
            pConnection.manager.SendImmediatelyUnreliable(pMessage);
        }

        public void QueueMessage(NetMessage msg, NetworkConnection who = null)
        {
            if (!Network.isActive)
                return;
            if (who == null)
            {
                QueueMessage(msg, Network.connections);
            }
            else
            {
                msg.Serialize();
                who.manager.QueueMessage(msg);
            }
        }

        public void QueueMessage(NetMessage msg, List<NetworkConnection> pConnections)
        {
            if (!Network.isActive)
                return;
            if (msg is SynchronizedNetMessage synchronizedNetMessage)
                GhostManager.context._synchronizedEvents.Add(synchronizedNetMessage);
            msg.Serialize();
            bool flag = false;
            foreach (NetworkConnection pConnection in pConnections)
            {
                if (pConnection.profile != null)
                {
                    NetMessage netMessage = msg;
                    if (flag)
                    {
                        netMessage = Activator.CreateInstance(msg.GetType(), null) as NetMessage;
                        netMessage.priority = msg.priority;
                        netMessage.SetSerializedData(msg.serializedData);
                        msg.CopyTo(netMessage);
                    }
                    pConnection.manager.QueueMessage(netMessage);
                    flag = true;
                }
            }
        }

        public void QueueMessageForAllBut(NetMessage msg, NetworkConnection who)
        {
            if (!Network.isActive)
                return;
            msg.Serialize();
            bool flag = false;
            foreach (NetworkConnection connection in Network.connections)
            {
                if (connection.profile != null && who != connection)
                {
                    NetMessage msg1 = msg;
                    if (flag)
                    {
                        msg1 = Activator.CreateInstance(msg.GetType(), null) as NetMessage;
                        msg1.priority = msg.priority;
                        msg1.SetSerializedData(msg.serializedData);
                    }
                    connection.manager.QueueMessage(msg1);
                    flag = true;
                }
            }
        }

        public void QueueMessage(NetMessage msg, NetMessagePriority priority, NetworkConnection who = null)
        {
            msg.priority = priority;
            QueueMessage(msg, who);
        }

        public void QueueMessage(
          NetMessage msg,
          NetMessagePriority priority,
          List<NetworkConnection> pConnections)
        {
            msg.priority = priority;
            QueueMessage(msg, pConnections);
        }

        public void QueueMessageForAllBut(
          NetMessage msg,
          NetMessagePriority priority,
          NetworkConnection who)
        {
            msg.priority = priority;
            QueueMessageForAllBut(msg, who);
        }

        public static bool InLobby() => Level.current is TeamSelect2;

        public static bool InGameLevel() => Level.current is GameLevel;

        public static bool InMatch()
        {
            switch (Level.current)
            {
                case GameLevel _:
                case RockScoreboard _:
                    return true;
                default:
                    return Level.current is RockIntro;
            }
        }

        public static void EndNetworkingSession(DuckNetErrorInfo error) => Network.activeNetwork.core.DisconnectClient(DuckNetwork.localConnection, error);

        public static void DisconnectClient(NetworkConnection c, DuckNetErrorInfo error) => Network.activeNetwork.core.DisconnectClient(c, error);

        public static void Initialize()
        {
            Network._netMessagePriorities = Enum.GetValues(typeof(NetMessagePriority)).Cast<NetMessagePriority>();
            Network.activeNetwork.DoInitialize();
        }

        public static long gameDataHash
        {
            get
            {
                return Network.messageTypeHash + Editor.thingTypesHash;
            }

        }

        public static void InitializeMessageTypes()
        {
            IEnumerable<System.Type> subclasses = Editor.GetAllSubclasses(typeof(NetMessage));
            Network._typeToMessageID.Clear();
            Network._constructorToMessageID.Clear();
            ushort key = 1;
            foreach (System.Type type in subclasses)
            {
                object[] Attributes = type.GetCustomAttributes(typeof(FixedNetworkID), false);
                if (Attributes.Length != 0)
                {
                    FixedNetworkID customAttribute = (FixedNetworkID)Attributes[0];
                    if (customAttribute != null)
                    {
                        Mod fromTypeIgnoreCore = ModLoader.GetModFromTypeIgnoreCore(type);
                        if (fromTypeIgnoreCore != null && fromTypeIgnoreCore is DisabledMod)
                        {
                            fromTypeIgnoreCore.typeToMessageID.Add(type, customAttribute.FixedID);
                            fromTypeIgnoreCore.constructorToMessageID.Add(type.GetConstructor(System.Type.EmptyTypes), customAttribute.FixedID);
                        }
                        else
                        {
                            Network._typeToMessageID.Add(type, customAttribute.FixedID);
                            Network._constructorToMessageID.Add(type.GetConstructor(System.Type.EmptyTypes), customAttribute.FixedID);
                        }
                        Network._allMessageTypesToID.Add(type, customAttribute.FixedID);
                    }
                }
            }
            string str = "";
            foreach (System.Type type in subclasses)
            {
                if (!Network._allMessageTypesToID.ContainsKey(type))
                {
                    ConstructorInfo constructor = type.GetConstructor(System.Type.EmptyTypes);
                    if (constructor == null)
                    {
                        string message = "NetMessage (" + type.Name + ") has no empty constructor! All NetMessages must allow 'new " + type.Name + "()'";
                        if (MonoMain.modDebugging)
                        {
                            Debugger.Break();
                            Program.crashAssembly = type.Assembly;
                            throw new Exception(message);
                        }
                        DevConsole.Log(DCSection.General, "|DGRED|" + message);
                    }
                    Mod fromTypeIgnoreCore = ModLoader.GetModFromTypeIgnoreCore(type);
                    if (fromTypeIgnoreCore != null && fromTypeIgnoreCore is DisabledMod)
                    {
                        while (fromTypeIgnoreCore.typeToMessageID.ContainsKey(fromTypeIgnoreCore.currentMessageIDIndex))
                            ++fromTypeIgnoreCore.currentMessageIDIndex;
                        fromTypeIgnoreCore.typeToMessageID.Add(type, fromTypeIgnoreCore.currentMessageIDIndex);
                        fromTypeIgnoreCore.constructorToMessageID.Add(type.GetConstructor(System.Type.EmptyTypes), fromTypeIgnoreCore.currentMessageIDIndex);
                        Network._allMessageTypesToID.Add(type, fromTypeIgnoreCore.currentMessageIDIndex);
                        ++fromTypeIgnoreCore.currentMessageIDIndex;
                    }
                    else
                    {
                        while (Network._typeToMessageID.ContainsKey(key))
                            ++key;
                        if (fromTypeIgnoreCore == null && !type.IsDefined(typeof(ClientOnlyAttribute), false))
                            str += type.Name;
                        Network._typeToMessageID.Add(type, key);
                        Network._constructorToMessageID.Add(constructor, key);
                        Network._allMessageTypesToID.Add(type, key);
                        ++key;
                    }
                }
            }
            Network.messageTypeHash = CRC32.Generate(str);
            PhysicsParticle.RegisterNetParticleType(typeof(SmallFire));
            PhysicsParticle.RegisterNetParticleType(typeof(ExtinguisherSmoke));
            PhysicsParticle.RegisterNetParticleType(typeof(Firecracker));
        }

        public void DoInitialize()
        {
            _core = new NCSteam(Network.activeNetwork, _networkIndex);
            if (NetworkDebugger.enabled)
                _lanCore = new NCNetDebug(Network.activeNetwork, _networkIndex);
            else
                _lanCore = new NCBasic(Network.activeNetwork, _networkIndex);
        }

        public static void Terminate() => Network.activeNetwork.core.Terminate();

        public void Reset()
        {
            _currentTick = 0U;
            _synchronizedTime = new NetIndex16(1, true);
            _tickSync = new NetIndex16(1, true);
        }

        public static void PreUpdate()
        {
            Network.activeNetwork._networkActive = Network.activeNetwork.core.isActive;
            Network.activeNetwork.DoPreUpdate();
        }

        public void DoPreUpdate()
        {
            _currentTick += 1U;
            _synchronizedTime++;
            _tickSync += 1;
            core.Update();
            DuckNetwork.Update();
        }

        public static void PostUpdate() => Network.activeNetwork.DoPostUpdate();

        public void DoPostUpdate() => core.PostUpdate();

        public static void PostDraw() => Network.activeNetwork.DoPostDraw();

        public void DoPostDraw() => core.PostDraw();
    }
}
