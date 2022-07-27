// Decompiled with JetBrains decompiler
// Type: DuckGame.NetworkConnection
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        public static float ghostLerpDivisor = 0.0f;

        public int authorityPower => this.profile != null ? GameLevel.NumberOfDucks - (profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks + 1 : 1;

        public override string ToString()
        {
            string str1 = "|WHITE|(";
            if (this.profile != null && this.profile.persona != null)
                str1 = str1 + this.profile.persona.colorUsable.ToDGColorString() + "(" + this.profile.networkIndex.ToString() + ")";
            if (this.isHost)
                str1 += "(H)";
            string str2 = null;
            if (!this.hasRealName || this.data == null)
                str2 = !(this.data is User) ? (Steam.user == null ? "LAN USER" : Steam.user.id.ToString()) : (this.data as User).id.ToString();
            else if (Network.activeNetwork.core is NCSteam)
                str2 = this.name + "," + (this.data as User).id.ToString();
            else if (Network.activeNetwork.core is NCBasic)
                str2 = this.name + "," + (this.data as IPEndPoint).ToString();
            str2.Replace("|", "(");
            str2.Replace("@", "$");
            string str3 = str1 + str2;
            if (this.profile != null && this.profile.networkStatus != DuckNetStatus.Connected)
                str3 = str3 + "," + this.profile.networkStatus.ToString();
            return str3 + "|WHITE|)" + "(" + this._connectionID.ToString() + ")";
        }

        public int debuggerIndex
        {
            get
            {
                if (!NetworkDebugger.enabled || this._identifier == null)
                    return -1;
                return Convert.ToInt32(this._identifier.Split(':')[1]) - 1330;
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
                if (this._debuggerContext == null)
                    this._debuggerContext = new DataLayerDebug.BadConnection(this);
                return this._debuggerContext;
            }
        }

        public void SetDebuggerContext(DataLayerDebug.BadConnection pContext) => this._debuggerContext = pContext;

        public Profile profile
        {
            get => this._profile;
            set => this._profile = value;
        }

        public uint sessionID => this._sessionID;

        public object data => this._data;

        public string identifier => this._identifier;

        public bool hasRealName => this._realName != null;

        public string name
        {
            get
            {
                if (this._realName != null)
                    return this._realName;
                string str = "NULL";
                if (this._data != null)
                    str = Network.activeNetwork.core.GetConnectionName(this._data);
                return str == null || str == "" || str == "no info" ? this._identifier : str;
            }
            set => this._realName = value;
        }

        public StreamManager manager => this._manager;

        public byte levelIndex
        {
            get => this._loadingStatus;
            set => this._loadingStatus = value;
        }

        protected ConnectionStatus _status
        {
            get => this._internalStatus;
            set
            {
                if (this._internalStatus != value && Network.activeNetwork != null && Network.activeNetwork.core != null)
                    Network.activeNetwork.core._connectionsDirty = true;
                this._internalStatus = value;
            }
        }

        public ConnectionStatus status => this._status;

        public void BeginConnection() => this.ChangeStatus(this._data == null ? ConnectionStatus.Connected : ConnectionStatus.Connecting);

        public void LeaveLobby() => this.ChangeStatus(ConnectionStatus.Disconnected);

        private void ChangeStatus(ConnectionStatus s)
        {
            if (s != ConnectionStatus.Disconnecting && s != ConnectionStatus.Disconnected && this.banned)
                return;
            int num = this._status != s ? 1 : 0;
            this._status = s;
            if (num == 0)
                return;
            DevConsole.Log(DCSection.Connection, "|DGORANGE|Connection Status Changed (" + s.ToString() + ")", this);
            if (this._status != ConnectionStatus.Disconnected)
                return;
            Network.activeNetwork.core.DisconnectClient(this, this._connectionError);
        }

        public bool isHost
        {
            get => this._isHost;
            set => this._isHost = value;
        }

        public bool sentThisFrame
        {
            get => this._sentThisFrame;
            set => this._sentThisFrame = value;
        }

        public uint lastTickReceived
        {
            get => this._lastTickReceived;
            set
            {
                this._lastTickReceived = value;
                this._estimatedClientTick = this._lastTickReceived;
            }
        }

        public uint estimatedClientTick
        {
            get => (uint)(_estimatedClientTick + (ulong)(int)((double)this.manager.ping / 2.0 * 60.0));
            set => this._estimatedClientTick = value;
        }

        public void SetData(object d)
        {
            this._data = d;
            if (this._data != null)
                this._identifier = Network.activeNetwork.core.GetConnectionIdentifier(this._data);
            this.Reset("Connection _data changed");
        }

        public int connectionID => this._connectionID;

        public NetworkConnection(object dat, string id = null)
        {
            this._connectionID = NetworkConnection.kconnectionIDInc++;
            this._identifier = dat == null ? "local" : Network.activeNetwork.core.GetConnectionIdentifier(dat);
            if (id != null)
                this._identifier = id;
            this.Reset("NetworkConnection constructor");
            this._data = dat;
            this.kAckOffsets = new ushort[16];
            for (int index = 0; index < 16; ++index)
                this.kAckOffsets[index] = (ushort)(1 << index);
        }

        public void Reset(string reason)
        {
            this._manager = new StreamManager(this);
            this.acksReceived = new HashSet<ushort>();
            this._isHost = false;
            this.ChangeStatus(ConnectionStatus.Disconnected);
            this.levelIndex = byte.MaxValue;
            this._sentThisFrame = false;
            this._lastReceivedTime = 0U;
            this._lastSentTime = 0U;
            this._personalTick = 0U;
            this._connectsReceived = 0;
            this.sentFilterMessage = false;
            this._numErrorLogs = 0;
            this._lastTickReceived = 0U;
            this._estimatedClientTick = 0U;
            this._currentSessionTicks = 0;
            this._ticksTillDisconnectAttempt = 0;
            this._disconnectsSent = 0;
            this.wantsGhostData = -1;
            this._lastReceivedPacketOrder = 0;
            this._packetHistory = new NetworkPacket[33];
            this._packetHistoryIndex = 0;
            this._theirVersion = "";
            //this._connectionTimeout = false;
            this.synchronizedInputDevices.Clear();
            this.lastSynchronizedDeviceType = byte.MaxValue;
            this.pingWait = 0;
            this._realName = null;
            this._pingsSent = 0;
            this.kicking = false;
            this.recentlyReceivedPackets.Clear();
            this.recentlyReceivedPacketsArray = new ushort[NetworkConnection.kMaxRecentlyReceivedPackets];
            this.failureNotificationCooldown = 0;
            if (this._data != null && GhostManager.context != null)
                GhostManager.context.Clear(this);
            if (NetworkDebugger.enabled)
                this.debuggerContext.Reset();
            DevConsole.Log(DCSection.Connection, "@disconnect Reset called on " + this.identifier + "(" + (Steam.user != null ? Steam.user.id.ToString() : "local") + ", " + reason + ")");
        }

        public void StartNewSession()
        {
            this._sessionID = Rando.UInt();
            this._currentSessionTicks = 0;
        }

        public void SynchronizeSession(uint pWith)
        {
            if ((int)pWith == (int)this._sessionID + 1)
            {
                this._sessionID = pWith;
                this.BecomeConnected(pWith);
            }
            else
            {
                this._sessionID = pWith + 1U;
                DevConsole.Log(DCSection.Connection, "|DGGREEN|Synchronizing Session (" + this._sessionID.ToString() + ")", this);
            }
        }

        public ushort GetAck() => this._lastReceivedPacketOrder;

        public ushort GetAckBitfield()
        {
            ushort ackBitfield = 0;
            for (int index = 0; index < 17; ++index)
            {
                if (this._packetHistory[index] != null)
                {
                    int num = _lastReceivedPacketOrder - _packetHistory[index].order;
                    if (num < 0)
                        num += ushort.MaxValue;
                    if (num > 0 && num < 17)
                        ackBitfield |= this.kAckOffsets[num - 1];
                }
            }
            return ackBitfield;
        }

        public void PacketReceived(NetworkPacket packet)
        {
            this._lastReceivedTime = this._personalTick;
            if (NetworkConnection.PacketOrderGreater(packet.order, this._lastReceivedPacketOrder))
                this._lastReceivedPacketOrder = packet.order;
            this._packetHistory[this._packetHistoryIndex % 17] = packet;
            ++this._packetHistoryIndex;
        }

        private static bool PacketOrderGreater(ushort s1, ushort s2)
        {
            if (s1 > s2 && s1 - s2 <= 32768)
                return true;
            return s1 < s2 && s2 - s1 > 32768;
        }

        public void PacketSent()
        { 
            this._lastSentTime = this._personalTick;
        }
        public void OnNonConnectionMessage(NetMessage message)
        {
        }

        public void HardTerminate()
        {
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Hard Terminate."));
            this.ChangeStatus(ConnectionStatus.Disconnected);
        }

        public void Disconnect()
        {
            Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Hard Terminate."));
            this.ChangeStatus(ConnectionStatus.Disconnected);
        }

        public void BecomeConnected(uint pSession)
        {
            if (this.status != ConnectionStatus.Connecting)
                return;
            DevConsole.Log(DCSection.NetCore, this.ToString() + " BecomeConnected on session index " + pSession.ToString());
            this.ChangeStatus(ConnectionStatus.Connected);
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
            if ((int)pMessage.session == (int)this.sessionID)
                this.BecomeConnected(this.sessionID);
            this._lastReceivedTime = this._personalTick;
            if (pMessage is NMNetworkCoreMessage)
                this.OnMessage(pMessage as NMNetworkCoreMessage);
            else
                this.OnNonConnectionMessage(pMessage);
        }

        public void OnMessage(NMNetworkCoreMessage message)
        {
            if (this._data == null)
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
                else if (this.status == ConnectionStatus.Disconnecting)
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
                        this._theirVersion = nmConnect.version;
                        Send.Message(new NMWrongVersion(DG.version), NetMessagePriority.UnreliableUnordered, this);
                        this.Disconnect_DifferentVersion(this._theirVersion);
                        return;
                    }
                    if (ModLoader.modHash != nmConnect.modHash)
                    {
                        Send.Message(new NMModIncompatibility(), NetMessagePriority.UnreliableUnordered, this);
                        this.Disconnect_IncompatibleMods();
                        return;
                    }
                    if (nmConnect.session > this.sessionID)
                        this.SynchronizeSession(nmConnect.session);
                }
                if (message is NMNewPing)
                {
                    if (!this._pongedThisFrame)
                    {
                        Send.Message(new NMNewPong((message as NMNewPing).index), NetMessagePriority.UnreliableUnordered, this);
                        this.sendPacketsNow = true;
                        this._pongedThisFrame = true;
                    }
                    if (!(message is NMNewPingHost))
                        return;
                    Network.ReceiveHostTime((message as NMNewPingHost).hostSynchronizedTime);
                }
                else if (message is NMNewPong)
                {
                    NMNewPing nmNewPing;
                    if (!this._pings.TryGetValue((message as NMNewPong).index, out nmNewPing))
                        return;
                    this.manager.LogPing(nmNewPing.GetTotalSeconds() - 0.032f);
                }
                else if (message is NMWrongVersion)
                {
                    this._theirVersion = (message as NMWrongVersion).version;
                    this.Disconnect_DifferentVersion(this._theirVersion);
                }
                else
                {
                    if (!(message is NMModIncompatibility))
                        return;
                    this.Disconnect_IncompatibleMods();
                }
            }
        }

        public void TerminateConnection() => this.ChangeStatus(ConnectionStatus.Disconnected);

        public void BeginDisconnecting(DuckNetErrorInfo error)
        {
            if (this._status == ConnectionStatus.Disconnecting)
                return;
            this.ChangeStatus(ConnectionStatus.Disconnecting);
            this._disconnectsSent = 0;
            this._ticksTillDisconnectAttempt = 0;
            this._connectionError = error;
            Send.ImmediateUnreliableMessage(new NMDisconnect(this._connectionError != null ? this._connectionError.error : DuckNetError.UnknownError), this);
            Send.ImmediateUnreliableMessage(new NMDisconnect(this._connectionError != null ? this._connectionError.error : DuckNetError.UnknownError), this);
            Send.ImmediateUnreliableMessage(new NMDisconnect(this._connectionError != null ? this._connectionError.error : DuckNetError.UnknownError), this);
        }

        private int timeBetweenPings
        {
            get
            {
                if (this._pingsSent < 45)
                    return 2;
                return MonoMain.pauseMenu != null || Keyboard.Down(Keys.F1) ? 4 : 10;
            }
        }

        public void Update()
        {
            if (this.failureNotificationCooldown > 0)
                --this.failureNotificationCooldown;
            this._pongedThisFrame = false;
            if (this._debuggerContext != null)
                this._debuggerContext.Update(Network.activeNetwork.core);
            if (this._status == ConnectionStatus.Disconnected)
                return;
            if (this._status == ConnectionStatus.Disconnecting)
            {
                if (this._data == null || this._disconnectsSent > 10)
                {
                    this.ChangeStatus(ConnectionStatus.Disconnected);
                    this._connectionError = null;
                }
                else
                {
                    --this._ticksTillDisconnectAttempt;
                    if (this._ticksTillDisconnectAttempt > 0)
                        return;
                    if (!this.kicking)
                    {
                        Send.Message(new NMDisconnect(this._connectionError != null ? this._connectionError.error : DuckNetError.UnknownError), NetMessagePriority.UnreliableUnordered, this);
                        DevConsole.Log(DCSection.Connection, "Disconnect send    (" + this.sessionID.ToString() + ")", this);
                    }
                    ++this._disconnectsSent;
                    this._ticksTillDisconnectAttempt = 4;
                }
            }
            else
            {
                ++this._currentSessionTicks;
                if (this._status == ConnectionStatus.Connecting)
                {
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > 0.0 && this._numErrorLogs == 0)
                    {
                        ++this._numErrorLogs;
                        this.LogSessionDetails();
                    }
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > 5.0 && this._numErrorLogs == 1)
                    {
                        ++this._numErrorLogs;
                        this.LogSessionDetails();
                    }
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > 8.0 && this._numErrorLogs == 2)
                    {
                        ++this._numErrorLogs;
                        this.LogSessionDetails();
                    }
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > 10.0 && this._numErrorLogs == 3)
                    {
                        ++this._numErrorLogs;
                        this.LogSessionDetails();
                    }
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > 15.0 && this._numErrorLogs == 4)
                    {
                        ++this._numErrorLogs;
                        this.LogSessionDetails();
                    }
                    float num = 18f;
                    if (NetworkDebugger.enabled)
                        num = 8f;
                    if ((double)Maths.TicksToSeconds(this._currentSessionTicks) > (double)num && !MonoMain.noConnectionTimeout)
                        this.Disconnect_ConnectionTimeout();
                }
                if (this._data == null)
                    return;
                lock (this.acksReceived)
                    this._manager.DoAcks(this.acksReceived);
                this._manager.Update();
                if (this.status == ConnectionStatus.Connecting)
                {
                    if (this.pingWait <= 0)
                    {
                        Send.Message(new NMConnect(this._connectsReceived, (NetIndex4)0, DG.version, ModLoader.modHash), NetMessagePriority.UnreliableUnordered, this);
                        DevConsole.Log(DCSection.Connection, "Connect send    (" + this.sessionID.ToString() + ")", this);
                        this.pingWait = 20;
                    }
                    --this.pingWait;
                }
                else
                {
                    if (this.pingWait > this.timeBetweenPings)
                    {
                        this.restartPingTimer = true;
                        NMNewPing msg = !Network.isServer ? new NMNewPing(this.pingIndex) : new NMNewPingHost(this.pingIndex);
                        ++this._pingsSent;
                        this._pings[this.pingIndex] = msg;
                        this.pingIndex = (byte)((pingIndex + 1) % 10);
                        Send.Message(msg, NetMessagePriority.UnreliableUnordered, this);
                        this.pingWait = 0;
                        this.sendPacketsNow = true;
                    }
                    ++this.pingWait;
                }
                ++this._personalTick;
                ++this._estimatedClientTick;
                if (this._status == ConnectionStatus.Connecting || this._status == ConnectionStatus.Disconnecting)
                    return;
                if (this._status == ConnectionStatus.Connected && this.receiveGap > 960U)
                {
                    DevConsole.Log(DCSection.Connection, "|DGRED|Connection timeout with " + this.ToString());
                    Network.activeNetwork.core.DisconnectClient(this, new DuckNetErrorInfo(DuckNetError.ConnectionLost, "Connection was lost."));
                }
                this._previousReceiveGap = this.receiveGap;
            }
        }

        private void LogSessionDetails()
        {
            if (!(this.data is User))
                return;
            SessionState sessionState = Steam.GetSessionState(this.data as User);
            DevConsole.Log("Information for " + this.ToString() + ":", Colors.DGBlue);
            foreach (FieldInfo field in sessionState.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                DevConsole.Log(field.Name + ": " + field.GetValue(sessionState).ToString(), Colors.DGBlue);
            DevConsole.Log("", Colors.DGBlue);
        }

        public uint receiveGap => this._personalTick - this._lastReceivedTime;

        public bool isExperiencingConnectionTrouble => this.receiveGap > 100U;

        public void PostUpdate(int frameCounter)
        {
            NetworkConnection.ghostLerpDivisor = 1f / packetsEvery;
            if (this._status == ConnectionStatus.Disconnected)
                return;
            bool flag = (frameCounter + NetworkConnection.connectionLoopIndex) % NetworkConnection.packetsEvery == 0;
            if (DuckNetwork.levelIndex == levelIndex && this.levelIndex != byte.MaxValue && this.status == ConnectionStatus.Connected)
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
            this._manager.Flush(flag);
            if (flag && !this._sentThisFrame)
                Network.activeNetwork.core.SendPacket(null, this);
            this._sentThisFrame = false;
        }

        public class FailedGhost
        {
            public ushort ghost;
            public long mask;
        }
    }
}
