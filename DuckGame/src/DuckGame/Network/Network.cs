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
        private static Map<ushort, Type> _typeToMessageID = new Map<ushort, Type>();
        private static Dictionary<Type, ushort> _allMessageTypesToID = new Dictionary<Type, ushort>();
        private static IEnumerable<NetMessagePriority> _netMessagePriorities;
        public static List<string> synchronizedTriggers = new List<string>()
        {
          Triggers.Left,
          Triggers.Right,
          Triggers.Up,
          Triggers.Down,
          Triggers.Shoot,
          Triggers.Jump,
          Triggers.Grab,
          Triggers.Quack,
          Triggers.Start,
          Triggers.Ragdoll,
          Triggers.Strafe
        };
        private static int _inputDelayFrames = 0;
        public bool _networkActive;
        public static uint messageTypeHash;

        public static bool SimulateBadnet => false;

        public int networkIndex => _networkIndex;

        public Network(int networkIndex = 0) => _networkIndex = networkIndex;

        public static int simTick
        {
            get => _simTick;
            set => _simTick = value;
        }

        public static bool available => Steam.IsInitialized() && Steam.user != null;

        public NCNetworkImplementation core
        {
            get => !lanMode ? _core : _lanCore;
            set
            {
                if (lanMode)
                    _lanCore = value;
                else
                    _core = value;
            }
        }

        public static Network activeNetwork
        {
            get
            {
                if (_activeNetwork == null)
                    _activeNetwork = new Network();
                return _activeNetwork;
            }
            set => _activeNetwork = value;
        }

        public static int frame
        {
            get => activeNetwork.core.frame;
            set => activeNetwork.core.frame = value;
        }

        public static NetGraph netGraph => _activeNetwork.core.netGraph;

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

        public static NetIndex16 synchronizedTime => activeNetwork.synchTime;

        public static void ReceiveHostTime(NetIndex16 pTime)
        {
            if (!(activeNetwork._lastReceivedTime < pTime))
                return;
            activeNetwork._synchronizedTime = pTime + (ushort)(host.manager.ping / 2.0 / Maths.IncFrameTimer());
            activeNetwork._lastReceivedTime = pTime;
        }

        public static double Time => 0.0;

        public static uint Tick => activeNetwork._currentTick;

        public static NetIndex16 TickSync => activeNetwork._tickSync;

        public static float ping => activeNetwork.core.averagePing;

        public static float highestPing
        {
            get
            {
                float highestPing = 0f;
                foreach (NetworkConnection connection in connections)
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
                foreach (NetworkConnection connection in connections)
                {
                    if (connection.isHost)
                        return connection;
                }
                return null;
            }
        }

        public static Map<ushort, ConstructorInfo> constructorToMessageID => _constructorToMessageID;

        public static Map<ushort, Type> typeToMessageID => _typeToMessageID;

        public static Dictionary<Type, ushort> allMessageTypesToID => _allMessageTypesToID;

        public static IEnumerable<NetMessagePriority> netMessagePriorities => _netMessagePriorities;

        public static int inputDelayFrames
        {
            get => _inputDelayFrames;
            set => _inputDelayFrames = value;
        }

        public static bool hasHostConnection
        {
            get
            {
                if (DuckNetwork.localConnection.isHost)
                    return true;
                foreach (NetworkConnection connection in connections)
                {
                    if (connection.isHost)
                        return true;
                }
                return false;
            }
        }

        public static bool canSetObservers => inLobby && isServer && !DuckNetwork.isDedicatedServer;

        public static bool isServer
        {
            get => activeNetwork.core.isServer;
            set => activeNetwork.core.isServer = value;
        }

        public static bool isClient => !isServer;

        public static void MakeActive() => activeNetwork._networkActive = true;

        public static void MakeInactive() => activeNetwork._networkActive = false;

        public static bool isActive => activeNetwork._networkActive;

        public static bool connected => connections.Count > 0;

        public static List<NetworkConnection> connections => activeNetwork.core.connections;

        public Type GetClassType(string name)
        {
            string fullName = typeof(Duck).Assembly.FullName;
            return Editor.GetType("DuckGame." + name + ", " + fullName);
        }

        public static void JoinServer(string nameVal, int portVal = 1337, string ip = "localhost") => activeNetwork.DoJoinServer(nameVal, portVal, ip);

        private void DoJoinServer(string nameVal, int portVal = 1337, string ip = "localhost") => core.JoinServer(nameVal, portVal, ip);

        public static void HostServer(
          NetworkLobbyType lobbyType,
          int maxConnectionsVal = 32,
          string nameVal = "duckGameServer",
          int portVal = 1337)
        {
            activeNetwork.DoHostServer(lobbyType, maxConnectionsVal, nameVal, portVal);
        }

        private void DoHostServer(
          NetworkLobbyType lobbyType,
          int maxConnectionsVal = 32,
          string nameVal = "duckGameServer",
          int portVal = 1337)
        {
            core.HostServer(nameVal, portVal, lobbyType, maxConnectionsVal);
        }

        public static void OnMessageStatic(NetMessage m) => _activeNetwork.OnMessage(m);

        private void OnMessage(NetMessage m)
        {
            if (m is NMConsoleMessage)
                DevConsole.Log((m as NMConsoleMessage).message, Color.Lime);
            else if (isServer)
                OnMessageServer(m);
            else
                OnMessageClient(m);
        }

        private void OnMessageServer(NetMessage m) => Level.current.OnMessage(m);

        private void OnMessageClient(NetMessage m) => Level.current.OnMessage(m);

        public void OnConnection(NetworkConnection connection) => UIMatchmakingBox.core.pulseNetwork = true;

        public void ImmediateUnreliableBroadcast(NetMessage pMessage)
        {
            if (!isActive)
                return;
            pMessage.Serialize();
            bool flag = false;
            foreach (NetworkConnection connection in connections)
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
            if (!isActive)
                return;
            pMessage.Serialize();
            pConnection.manager.SendImmediatelyUnreliable(pMessage);
        }

        public void QueueMessage(NetMessage msg, NetworkConnection who = null)
        {
            if (!isActive)
                return;
            if (who == null)
            {
                QueueMessage(msg, connections);
            }
            else
            {
                msg.Serialize();
                who.manager.QueueMessage(msg);
            }
        }

        public void QueueMessage(NetMessage msg, List<NetworkConnection> pConnections)
        {
            if (!isActive)
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
            if (!isActive)
                return;
            msg.Serialize();
            bool flag = false;
            foreach (NetworkConnection connection in connections)
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

        public static bool inLobby => Level.current is TeamSelect2;

        public static bool inGameLevel => Level.current is GameLevel;

        public static bool inMatch
        {
            get
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
        }

        public static void EndNetworkingSession(DuckNetErrorInfo error) => activeNetwork.core.DisconnectClient(DuckNetwork.localConnection, error);

        public static void DisconnectClient(NetworkConnection c, DuckNetErrorInfo error) => activeNetwork.core.DisconnectClient(c, error);

        public static void Initialize()
        {
            _netMessagePriorities = Enum.GetValues(typeof(NetMessagePriority)).Cast<NetMessagePriority>();
            activeNetwork.DoInitialize();
        }

        public static long gameDataHash
        {
            get
            {
                return messageTypeHash + Editor.thingTypesHash;
            }

        }

        public static void InitializeMessageTypes()
        {
            IEnumerable<Type> subclasses = Editor.GetAllSubclasses(typeof(NetMessage));
            _typeToMessageID.Clear();
            _constructorToMessageID.Clear();
            ushort key = 1;
            foreach (Type type in subclasses)
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
                            fromTypeIgnoreCore.constructorToMessageID.Add(type.GetConstructor(Type.EmptyTypes), customAttribute.FixedID);
                        }
                        else
                        {
                            _typeToMessageID.Add(type, customAttribute.FixedID);
                            _constructorToMessageID.Add(type.GetConstructor(Type.EmptyTypes), customAttribute.FixedID);
                        }
                        _allMessageTypesToID.Add(type, customAttribute.FixedID);
                    }
                }
            }
            string str = "";
            foreach (Type type in subclasses)
            {
                if (!_allMessageTypesToID.ContainsKey(type))
                {
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
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
                        fromTypeIgnoreCore.constructorToMessageID.Add(type.GetConstructor(Type.EmptyTypes), fromTypeIgnoreCore.currentMessageIDIndex);
                        _allMessageTypesToID.Add(type, fromTypeIgnoreCore.currentMessageIDIndex);
                        ++fromTypeIgnoreCore.currentMessageIDIndex;
                    }
                    else
                    {
                        while (_typeToMessageID.ContainsKey(key))
                            ++key;
                        if (fromTypeIgnoreCore == null && !type.IsDefined(typeof(ClientOnlyAttribute), false))
                            str += type.Name;
                        _typeToMessageID.Add(type, key);
                        _constructorToMessageID.Add(constructor, key);
                        _allMessageTypesToID.Add(type, key);
                        ++key;
                    }
                }
            }
            messageTypeHash = CRC32.Generate(str);
            PhysicsParticle.RegisterNetParticleType(typeof(SmallFire));
            PhysicsParticle.RegisterNetParticleType(typeof(ExtinguisherSmoke));
            PhysicsParticle.RegisterNetParticleType(typeof(Firecracker));
        }

        public void DoInitialize()
        {
            _core = new NCSteam(activeNetwork, _networkIndex);
            if (NetworkDebugger.enabled)
                _lanCore = new NCNetDebug(activeNetwork, _networkIndex);
            else
                _lanCore = new NCBasic(activeNetwork, _networkIndex);
        }

        public static void Terminate()
        {
            if (activeNetwork.core != null)
            {
                activeNetwork.core.Terminate();
            }
        }

        public void Reset()
        {
            _currentTick = 0U;
            _synchronizedTime = new NetIndex16(1, true);
            _tickSync = new NetIndex16(1, true);
        }

        public static void PreUpdate()
        {
            activeNetwork._networkActive = activeNetwork.core.isActive;
            activeNetwork.DoPreUpdate();
        }

        public void DoPreUpdate()
        {
            _currentTick += 1U;
            _synchronizedTime++;
            _tickSync += 1;
            core.Update();
            DuckNetwork.Update();
        }

        public static void PostUpdate() => activeNetwork.DoPostUpdate();

        public void DoPostUpdate() => core.PostUpdate();

        public static void PostDraw() => activeNetwork.DoPostDraw();

        public void DoPostDraw() => core.PostDraw();
    }
}
