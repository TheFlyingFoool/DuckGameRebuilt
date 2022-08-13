// Decompiled with JetBrains decompiler
// Type: DuckGame.NetworkConnection
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace DuckGame
{
    public class NetworkConnection
    {
        public bool banned;
        public bool kicking;
        public int failureNotificationCooldown;
        private static NetworkConnection _context;
        public List<NetIndex16> _destroyedGhostResends = new List<NetIndex16>();
        private DataLayerDebug.BadConnection _debuggerContext;
        private Profile _profile;
        private uint _sessionID;
        public ushort _lastReceivedPacketOrder;
        private object _data;
        private string _realName;
        private string _identifier;
        private StreamManager _manager;
        private byte _loadingStatus = byte.MaxValue;
        protected ConnectionStatus _internalStatus;
        protected ConnectionStatus _previousStatus;
        private bool _isHost;
        public HashSet<ushort> recentlyReceivedPackets = new HashSet<ushort>();
        public byte recentlyReceivedPacketsArrayIndex;
        public ushort[] recentlyReceivedPacketsArray;
        public static readonly int kMaxRecentlyReceivedPackets = 128;
        private bool _sentThisFrame;
        private uint _lastReceivedTime;
        private uint _lastSentTime;
        private byte _connectsReceived;
        private uint _personalTick;
        private int _disconnectsSent;
        private string _theirVersion = "";
        //private bool _theirModsIncompatible;
        //private bool _connectionTimeout;
        public HashSet<InputDevice> synchronizedInputDevices = new HashSet<InputDevice>();
        public byte lastSynchronizedDeviceType = byte.MaxValue;
        public int triesSinceInputChangeSend = 60;
        public int logTransferSize;
        public int logTransferProgress;
        public int dataTransferSize;
        public int dataTransferProgress;
        public int wantsGhostData = -1;
        public static int connectionLoopIndex;
        private int _currentSessionTicks;
        private int _ticksTillDisconnectAttempt;
        private uint _lastTickReceived;
        private uint _estimatedClientTick;
        private uint _previousReceiveGap;
        public bool sentFilterMessage;
        public HashSet<ushort> acksReceived = new HashSet<ushort>();
        public List<NetworkConnection.FailedGhost> failedGhosts = new List<NetworkConnection.FailedGhost>();
        private static int kconnectionIDInc = 0;
        private int _connectionID;
        public ushort[] kAckOffsets;
        private NetworkPacket[] _packetHistory = new NetworkPacket[17];
        private int _packetHistoryIndex;
        private bool _pongedThisFrame;
        public bool restartPingTimer = true;
        public double averageHeartbeatTime;
        private DuckNetErrorInfo _connectionError;
        private byte pingIndex;
        private Dictionary<byte, NMNewPing> _pings = new Dictionary<byte, NMNewPing>();
        private int pingWait;
        private int _pingsSent;
        //private const int kConnectionTroubleGap = 400;
        //private const int kConnectionFailureGap = 960;
        private int _numErrorLogs;
        public bool sendPacketsNow;
        /// <summary>
        /// DON'T set this variable in realtime! It's used to determine how many input states to
        /// synchronize and that data will be read incorrectly if this is changed while the game runs. It can
        /// be changed only at the start of the game.
        ///  </summary>
        public static int packetsEvery = 1;
        public static float ghostLerpDivisor = 0f;

        public int authorityPower => profile != null ? GameLevel.NumberOfDucks - (profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks + 1 : 1;

        public override string ToString()
        {
            string str1 = "|WHITE|(";
            if (profile != null && profile.persona != null)
                str1 = str1 + profile.persona.colorUsable.ToDGColorString() + "(" + profile.networkIndex.ToString() + ")";
            if (isHost)
                str1 += "(H)";
            string str2 = null;
            if (!hasRealName || data == null)
                str2 = !(data is User) ? (Steam.user == null ? "LAN USER" : Steam.user.id.ToString()) : (data as User).id.ToString();
            else if (Network.activeNetwork.core is NCSteam)
                str2 = name + "," + (data as User).id.ToString();
            else if (Network.activeNetwork.core is NCBasic)
                str2 = name + "," + (data as IPEndPoint).ToString();
            str2.Replace("|", "(");
            str2.Replace("@", "$");
            string str3 = str1 + str2;
            if (profile != null && profile.networkStatus != DuckNetStatus.Connected)
                str3 = str3 + "," + profile.networkStatus.ToString();
            return str3 + "|WHITE|)" + "(" + _connectionID.ToString() + ")";
        }

        public int debuggerIndex
        {
            get
            {
                if (!NetworkDebugger.enabled || _identifier == null)
                    return -1;
                return Convert.ToInt32(_identifier.Split(':')[1]) - 1330;
            }
        }

        public static NetworkConnection context
        {
            get => NetworkConnection._context;
            set => NetworkConnection._context = value;
        }

        public DataLayerDebug.BadConnection debuggerContext
        {
            get
            {
                if (_debuggerContext == null)
                    _debuggerContext = new DataLayerDebug.BadConnection(this);
                return _debuggerContext;
            }
        }

        public void SetDebuggerContext(DataLayerDebug.BadConnection pContext) => _debuggerContext = pContext;

        public Profile profile
        {
            get => _profile;
            set => _profile = value;
        }

        public uint sessionID => _sessionID;

        public object data => _data;

        public string identifier => _identifier;

        public bool hasRealName => _realName != null;

        public string name
        {
            get
            {
                if (_realName != null)
                    return _realName;
                string str = "NULL";
                if (_data != null)
                    str = Network.activeNetwork.core.GetConnectionName(_data);
                return str == null || str == "" || str == "no info" ? _identifier : str;
            }
            set => _realName = value;
        }

        public StreamManager manager => _manager;

        public byte levelIndex
        {
            get => _loadingStatus;
            set => _loadingStatus = value;
        }

        protected ConnectionStatus _status
        {
            get => _internalStatus;
            set
            {
                if (_internalStatus != value && Network.activeNetwork != null && Network.activeNetwork.core != null)
                    Network.activeNetwork.core._connectionsDirty = true;
                _internalStatus = value;
            }
        }

        public ConnectionStatus status => _status;

        public void BeginConnection() => ChangeStatus(_data == null ? ConnectionStatus.Connected : ConnectionStatus.Connecting);

        public void LeaveLobby() => ChangeStatus(ConnectionStatus.Disconnected);

        private void ChangeStatus(ConnectionStatus s)
        {
            if (s != ConnectionStatus.Disconnecting && s != ConnectionStatus.Disconnected && banned)
                return;
            int num = _status != s ? 1 : 0;
            _status = s;
            if (num == 0)
                return;
            DevConsole.Log(DCSection.Connection, "|DGORANGE|Connection Status Changed (" + s.ToString() + ")", this);
            if (_status != ConnectionStatus.Disconnected)
                return;
            Network.activeNetwork.core.DisconnectClient(this, _connectionError);
        }

        public bool isHost
        {
            get => _isHost;
            set => _isHost = value;
        }

        public bool sentThisFrame
        {
            get => _sentThisFrame;
            set => _sentThisFrame = value;
        }

        public uint lastTickReceived
        {
            get => _lastTickReceived;
            set
            {
                _lastTickReceived = value;
                _estimatedClientTick = _lastTickReceived;
            }
        }

        public uint estimatedClientTick
        {
            get => (uint)(_estimatedClientTick + (ulong)(int)(manager.ping / 2.0 * 60.0));
            set => _estimatedClientTick = value;
        }

        public void SetData(object d)
        {
            _data = d;
            if (_data != null)
                _identifier = Network.activeNetwork.core.GetConnectionIdentifier(_data);
            Reset("Connection _data changed");
        }

        public int connectionID => _connectionID;

        public NetworkConnection(object dat, string id = null)
        {
            _connectionID = NetworkConnection.kconnectionIDInc++;
            _identifier = dat == null ? "local" : Network.activeNetwork.core.GetConnectionIdentifier(dat);
            if (id != null)
                _identifier = id;
            Reset("NetworkConnection constructor");
            _data = dat;
            kAckOffsets = new ushort[16];
            for (int index = 0; index < 16; ++index)
                kAckOffsets[index] = (ushort)(1 << index);
        }

        public void Reset(string reason)
        {
            _manager = new StreamManager(this);
            acksReceived = new HashSet<ushort>();
            _isHost = false;
            ChangeStatus(ConnectionStatus.Disconnected);
            levelIndex = byte.MaxValue;
            _sentThisFrame = false;
            _lastReceivedTime = 0U;
            _lastSentTime = 0U;
            _personalTick = 0U;
            _connectsReceived = 0;
            sentFilterMessage = false;
            _numErrorLogs = 0;
            _lastTickReceived = 0U;
            _estimatedClientTick = 0U;
            _currentSessionTicks = 0;
            _ticksTillDisconnectAttempt = 0;
            _disconnectsSent = 0;
            wantsGhostData = -1;
            _lastReceivedPacketOrder = 0;
            _packetHistory = new NetworkPacket[33];
            _packetHistoryIndex = 0;
            _theirVersion = "";
            //this._connectionTimeout = false;
            synchronizedInputDevices.Clear();
            lastSynchronizedDeviceType = byte.MaxValue;
            pingWait = 0;
            _realName = null;
            _pingsSent = 0;
            kicking = false;
            recentlyReceivedPackets.Clear();
            recentlyReceivedPacketsArray = new ushort[NetworkConnection.kMaxRecentlyReceivedPackets];
            failureNotificationCooldown = 0;
            if (_data != null && GhostManager.context != null)
                GhostManager.context.Clear(this);
            if (NetworkDebugger.enabled)
                debuggerContext.Reset();
            DevConsole.Log(DCSection.Connection, "@disconnect Reset called on " + identifier + "(" + (Steam.user != null ? Steam.user.id.ToString() : "local") + ", " + reason + ")");
        }

        public void StartNewSession()
        {
            _sessionID = Rando.UInt();
            _currentSessionTicks = 0;
        }

        public void SynchronizeSession(uint pWith)
        {
            if ((int)pWith == (int)_sessionID + 1)
            {
                _sessionID = pWith;
                BecomeConnected(pWith);
            }
            else
            {
                _sessionID = pWith + 1U;
                DevConsole.Log(DCSection.Connection, "|DGGREEN|Synchronizing Session (" + _sessionID.ToString() + ")", this);
            }
        }

        public ushort GetAck() => _lastReceivedPacketOrder;

        public ushort GetAckBitfield()
        {
            ushort ackBitfield = 0;
            for (int index = 0; index < 17; ++index)
            {
                if (_packetHistory[index] != null)
                {
                    int num = _lastReceivedPacketOrder - _packetHistory[index].order;
                    if (num < 0)
                        num += ushort.MaxValue;
                    if (num > 0 && num < 17)
                        ackBitfield |= kAckOffsets[num - 1];
                }
            }
            return ackBitfield;
        }

        public void PacketReceived(NetworkPacket packet)
        {
            _lastReceivedTime = _personalTick;
            if (NetworkConnection.PacketOrderGreater(packet.order, _lastReceivedPacketOrder))
                _lastReceivedPacketOrder = packet.order;
            _packetHistory[_packetHistoryIndex % 17] = packet;
            ++_packetHistoryIndex;
        }

        private static bool PacketOrderGreater(ushort s1, ushort s2)
        {
            if (s1 > s2 && s1 - s2 <= 32768)
                return true;
            return s1 < s2 && s2 - s1 > 32768;
        }

        public void PacketSent()
        {
            _lastSentTime = _personalTick;
        }
        public void OnNonConnectionMessage(NetMessage message)
        {
        }

        public void HardTerminate()
        {
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Hard Terminate."));
            ChangeStatus(ConnectionStatus.Disconnected);
        }

        public void Disconnect()
        {
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Hard Terminate."));
            ChangeStatus(ConnectionStatus.Disconnected);
        }

        public void BecomeConnected(uint pSession)
        {
            if (status != ConnectionStatus.Connecting)
                return;
            DevConsole.Log(DCSection.NetCore, ToString() + " BecomeConnected on session index " + pSession.ToString());
            ChangeStatus(ConnectionStatus.Connected);
            Network.activeNetwork.core.OnConnection(this);
        }

        private void Disconnect_IncompatibleMods()
        {
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ModsIncompatible, "Host has different Mods enabled!"));
            ConnectionError.joinLobby = Steam.lobby;
        }

        private void Disconnect_DifferentVersion(string theirVersion)
        {
            int num = (int)DuckNetwork.CheckVersion(theirVersion);
            Network.activeNetwork.core.DisconnectClient(this, DuckNetwork.AssembleMismatchError(theirVersion));
        }

        private void Disconnect_ConnectionTimeout() => Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ConnectionTimeout, "Could not connect. (timeout)"));

        public void Disconnect_ConnectionFailure()
        {
            DevConsole.Log(DCSection.Connection, "|DGRED|Disconnect_ConnectionFailure()");
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ConnectionLost, "Connection was lost."));
        }

        public void OnAnyMessage(NetMessage pMessage)
        {
            if ((int)pMessage.session == (int)sessionID)
                BecomeConnected(sessionID);
            _lastReceivedTime = _personalTick;
            if (pMessage is NMNetworkCoreMessage)
                OnMessage(pMessage as NMNetworkCoreMessage);
            else
                OnNonConnectionMessage(pMessage);
        }

        public void OnMessage(NMNetworkCoreMessage message)
        {
            if (_data == null)
            {
                DevConsole.Log(DCSection.Connection, "|RED|Null Connection Data, cannot receive message!");
            }
            else
            {
                if (message is NMDisconnect)
                {
                    DevConsole.Log(DCSection.DuckNet, "@received Received |WHITE|" + message.ToString(), message.connection);
                    Network.DisconnectClient(this, (message as NMDisconnect).GetError());
                }
                else if (status == ConnectionStatus.Disconnecting)
                {
                    DevConsole.Log(DCSection.Connection, "|RED|Received connection message during disconnect...");
                    return;
                }
                if (message is NMConnect)
                {
                    DevConsole.Log(DCSection.DuckNet, "@received Received |WHITE|" + message.ToString(), message.connection);
                    NMConnect nmConnect = message as NMConnect;
                    if (DG.version != nmConnect.version)
                    {
                        _theirVersion = nmConnect.version;
                        Send.Message(new NMWrongVersion(DG.version), NetMessagePriority.UnreliableUnordered, this);
                        Disconnect_DifferentVersion(_theirVersion);
                        return;
                    }
                    if (ModLoader.modHash != nmConnect.modHash)
                    {
                        Send.Message(new NMModIncompatibility(), NetMessagePriority.UnreliableUnordered, this);
                        Disconnect_IncompatibleMods();
                        return;
                    }
                    if (nmConnect.session > sessionID)
                        SynchronizeSession(nmConnect.session);
                }
                if (message is NMNewPing)
                {
                    if (!_pongedThisFrame)
                    {
                        Send.Message(new NMNewPong((message as NMNewPing).index), NetMessagePriority.UnreliableUnordered, this);
                        sendPacketsNow = true;
                        _pongedThisFrame = true;
                    }
                    if (!(message is NMNewPingHost))
                        return;
                    Network.ReceiveHostTime((message as NMNewPingHost).hostSynchronizedTime);
                }
                else if (message is NMNewPong)
                {
                    NMNewPing nmNewPing;
                    if (!_pings.TryGetValue((message as NMNewPong).index, out nmNewPing))
                        return;
                    manager.LogPing(nmNewPing.GetTotalSeconds() - 0.032f);
                }
                else if (message is NMWrongVersion)
                {
                    _theirVersion = (message as NMWrongVersion).version;
                    Disconnect_DifferentVersion(_theirVersion);
                }
                else
                {
                    if (!(message is NMModIncompatibility))
                        return;
                    Disconnect_IncompatibleMods();
                }
            }
        }

        public void TerminateConnection() => ChangeStatus(ConnectionStatus.Disconnected);

        public void BeginDisconnecting(DuckNetErrorInfo error)
        {
            if (_status == ConnectionStatus.Disconnecting)
                return;
            ChangeStatus(ConnectionStatus.Disconnecting);
            _disconnectsSent = 0;
            _ticksTillDisconnectAttempt = 0;
            _connectionError = error;
            Send.ImmediateUnreliableMessage(new NMDisconnect(_connectionError != null ? _connectionError.error : DuckNetError.UnknownError), this);
            Send.ImmediateUnreliableMessage(new NMDisconnect(_connectionError != null ? _connectionError.error : DuckNetError.UnknownError), this);
            Send.ImmediateUnreliableMessage(new NMDisconnect(_connectionError != null ? _connectionError.error : DuckNetError.UnknownError), this);
        }

        private int timeBetweenPings
        {
            get
            {
                if (_pingsSent < 45)
                    return 2;
                return MonoMain.pauseMenu != null || Keyboard.Down(Keys.F1) ? 4 : 10;
            }
        }

        public void Update()
        {
            if (failureNotificationCooldown > 0)
                --failureNotificationCooldown;
            _pongedThisFrame = false;
            if (_debuggerContext != null)
                _debuggerContext.Update(Network.activeNetwork.core);
            if (_status == ConnectionStatus.Disconnected)
                return;
            if (_status == ConnectionStatus.Disconnecting)
            {
                if (_data == null || _disconnectsSent > 10)
                {
                    ChangeStatus(ConnectionStatus.Disconnected);
                    _connectionError = null;
                }
                else
                {
                    --_ticksTillDisconnectAttempt;
                    if (_ticksTillDisconnectAttempt > 0)
                        return;
                    if (!kicking)
                    {
                        Send.Message(new NMDisconnect(_connectionError != null ? _connectionError.error : DuckNetError.UnknownError), NetMessagePriority.UnreliableUnordered, this);
                        DevConsole.Log(DCSection.Connection, "Disconnect send    (" + sessionID.ToString() + ")", this);
                    }
                    ++_disconnectsSent;
                    _ticksTillDisconnectAttempt = 4;
                }
            }
            else
            {
                ++_currentSessionTicks;
                if (_status == ConnectionStatus.Connecting)
                {
                    if (Maths.TicksToSeconds(_currentSessionTicks) > 0.0 && _numErrorLogs == 0)
                    {
                        ++_numErrorLogs;
                        LogSessionDetails();
                    }
                    if (Maths.TicksToSeconds(_currentSessionTicks) > 5.0 && _numErrorLogs == 1)
                    {
                        ++_numErrorLogs;
                        LogSessionDetails();
                    }
                    if (Maths.TicksToSeconds(_currentSessionTicks) > 8.0 && _numErrorLogs == 2)
                    {
                        ++_numErrorLogs;
                        LogSessionDetails();
                    }
                    if (Maths.TicksToSeconds(_currentSessionTicks) > 10.0 && _numErrorLogs == 3)
                    {
                        ++_numErrorLogs;
                        LogSessionDetails();
                    }
                    if (Maths.TicksToSeconds(_currentSessionTicks) > 15.0 && _numErrorLogs == 4)
                    {
                        ++_numErrorLogs;
                        LogSessionDetails();
                    }
                    float num = 18f;
                    if (NetworkDebugger.enabled)
                        num = 8f;
                    if (Maths.TicksToSeconds(_currentSessionTicks) > num && !MonoMain.noConnectionTimeout)
                        Disconnect_ConnectionTimeout();
                }
                if (_data == null)
                    return;
                lock (acksReceived)
                    _manager.DoAcks(acksReceived);
                _manager.Update();
                if (status == ConnectionStatus.Connecting)
                {
                    if (pingWait <= 0)
                    {
                        Send.Message(new NMConnect(_connectsReceived, (NetIndex4)0, DG.version, ModLoader.modHash), NetMessagePriority.UnreliableUnordered, this);
                        DevConsole.Log(DCSection.Connection, "Connect send    (" + sessionID.ToString() + ")", this);
                        pingWait = 20;
                    }
                    --pingWait;
                }
                else
                {
                    if (pingWait > timeBetweenPings)
                    {
                        restartPingTimer = true;
                        NMNewPing msg = !Network.isServer ? new NMNewPing(pingIndex) : new NMNewPingHost(pingIndex);
                        ++_pingsSent;
                        _pings[pingIndex] = msg;
                        pingIndex = (byte)((pingIndex + 1) % 10);
                        Send.Message(msg, NetMessagePriority.UnreliableUnordered, this);
                        pingWait = 0;
                        sendPacketsNow = true;
                    }
                    ++pingWait;
                }
                ++_personalTick;
                ++_estimatedClientTick;
                if (_status == ConnectionStatus.Connecting || _status == ConnectionStatus.Disconnecting)
                    return;
                if (_status == ConnectionStatus.Connected && receiveGap > 960U)
                {
                    DevConsole.Log(DCSection.Connection, "|DGRED|Connection timeout with " + ToString());
                    Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ConnectionLost, "Connection was lost."));
                }
                _previousReceiveGap = receiveGap;
            }
        }

        private void LogSessionDetails()
        {
            if (!(data is User))
                return;
            SessionState sessionState = Steam.GetSessionState(data as User);
            DevConsole.Log("Information for " + ToString() + ":", Colors.DGBlue);
            foreach (FieldInfo field in sessionState.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                DevConsole.Log(field.Name + ": " + field.GetValue(sessionState).ToString(), Colors.DGBlue);
            DevConsole.Log("", Colors.DGBlue);
        }

        public uint receiveGap => _personalTick - _lastReceivedTime;

        public bool isExperiencingConnectionTrouble => receiveGap > 100U;

        public void PostUpdate(int frameCounter)
        {
            NetworkConnection.ghostLerpDivisor = 1f / packetsEvery;
            if (_status == ConnectionStatus.Disconnected)
                return;
            bool flag = (frameCounter + NetworkConnection.connectionLoopIndex) % NetworkConnection.packetsEvery == 0;
            if (DuckNetwork.levelIndex == levelIndex && levelIndex != byte.MaxValue && status == ConnectionStatus.Connected)
            {
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.connection == DuckNetwork.localConnection && profile.netData.IsDirty(this))
                    {
                        Send.Message(new NMProfileNetData(profile, this), NetMessagePriority.Volatile, this);
                        profile.netData.Clean(this);
                    }
                }
                GhostManager.context.Update(this, flag);
            }
            NetSoundEffect.Update();
            _manager.Flush(flag);
            if (flag && !_sentThisFrame)
                Network.activeNetwork.core.SendPacket(null, this);
            _sentThisFrame = false;
        }

        public class FailedGhost
        {
            public ushort ghost;
            public long mask;
        }
    }
}
