using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace DuckGame
{
    public abstract class NCNetworkImplementation
    {
        public Network _core;
        private DataLayer _dataLayer;
        public HashSet<object> connectionWalls = new HashSet<object>();
        public bool _connectionsDirty = true;
        private List<NetworkConnection> _connectionsInternal;
        private List<NetworkConnection> _connectionsInternalAll;
        public bool firstPrediction = true;
        private List<NetworkConnection> _connectionHistory = new List<NetworkConnection>();
        private Queue<NetworkPacket> _pendingPackets = new Queue<NetworkPacket>();
        private Thread _networkThread;
        private Thread _timeThread;
        private bool _isServer = true;
        private bool _isServerP2P = true;
        protected List<NCError> _pendingMessagesList = new List<NCError>();
        protected Queue<NCError> _pendingMessages = new Queue<NCError>();
        protected Lobby _lobby;
        private GhostManager _ghostManager;
        private NetGraph _netGraph = new NetGraph();
        protected int _networkIndex;
        private volatile bool _killThread;
        private volatile Queue<NCError> _threadPendingMessages = new Queue<NCError>();
        private int _hardDisconnectTimeout = -1;
        public static DuckNetErrorInfo currentError;
        public static DuckNetErrorInfo currentMainDisconnectError;
        //private bool _discardMessagePlayed;
        //private const int kCompressionTag = 696143206;
        //private int _packetHeat;
        //private int _shownHeatMessage;
        public int frame;

        public DataLayer dataLayer => _dataLayer;

        private void RefreshConnections()
        {
            lock (_connectionHistory)
            {
                _connectionsInternal = new List<NetworkConnection>();
                _connectionsInternalAll = new List<NetworkConnection>();
                foreach (NetworkConnection networkConnection in _connectionHistory)
                {
                    if (networkConnection.status != ConnectionStatus.Disconnected)
                        _connectionsInternal.Add(networkConnection);
                    _connectionsInternalAll.Add(networkConnection);
                }
            }
            _connectionsDirty = false;
        }

        public List<NetworkConnection> connections
        {
            get
            {
                if (_connectionsDirty)
                    RefreshConnections();
                return _connectionsInternal;
            }
        }

        public List<NetworkConnection> allConnections
        {
            get
            {
                if (_connectionsDirty)
                    RefreshConnections();
                return _connectionsInternalAll;
            }
        }

        public List<NetworkConnection> sessionConnections
        {
            get
            {
                List<NetworkConnection> sessionConnections = new List<NetworkConnection>(connections);
                if (DuckNetwork.localConnection.status != ConnectionStatus.Disconnected)
                    sessionConnections.Add(DuckNetwork.localConnection);
                return sessionConnections;
            }
        }

        public bool isServer
        {
            get => _isServer;
            set => _isServer = value;
        }

        public bool isServerP2P
        {
            get => _isServerP2P;
            set => _isServerP2P = value;
        }

        public bool isActive => _networkThread != null;

        public Lobby lobby => _lobby;

        public GhostManager ghostManager => _ghostManager;

        public NetGraph netGraph => _netGraph;

        public NCNetworkImplementation(Network core, int networkIndex)
        {
            _core = core;
            _networkIndex = networkIndex;
            _ghostManager = new GhostManager();
            if (NetworkDebugger.enabled)
                _dataLayer = new DataLayerDebug(this);
            else
                _dataLayer = new DataLayer(this);
        }

        public void HostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections)
        {
            _pendingPackets.Clear();
            _dataLayer.Reset();
            NCError ncError = OnHostServer(identifier, port, lobbyType, maxConnections);
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: _networkIndex);
        }

        public abstract NCError OnHostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections);

        public void JoinServer(string identifier, int port, string ip)
        {
            _pendingPackets.Clear();
            _dataLayer.Reset();
            NCError ncError = OnJoinServer(identifier, port, ip);
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: _networkIndex);
        }

        public abstract NCError OnJoinServer(string identifier, int port, string ip);

        protected void StartServerThread()
        {
            _hardDisconnectTimeout = -1;
            _isServer = true;
            _isServerP2P = true;
            _killThread = false;
            _networkThread = new Thread(new ThreadStart(SpinThread))
            {
                CurrentCulture = CultureInfo.InvariantCulture,
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };
            _networkThread.Start();
            Network.MakeActive();
        }

        protected void StartClientThread()
        {
            _hardDisconnectTimeout = -1;
            _isServer = false;
            _isServerP2P = false;
            _killThread = false;
            _networkThread = new Thread(new ThreadStart(SpinThread))
            {
                CurrentCulture = CultureInfo.InvariantCulture,
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };
            _networkThread.Start();
            Network.MakeActive();
        }

        //private static uint SwapEndianness(ulong x) => (uint)((ulong)((((long)x & (long)byte.MaxValue) << 24) + (((long)x & 65280L) << 8)) + ((x & 16711680UL) >> 8) + ((x & 4278190080UL) >> 24));

        public void Thread_Loop()
        {
            NCError ncError = !_isServer ? OnSpinClientThread() : OnSpinServerThread();
            lock (_threadPendingMessages)
            {
                if (_pendingMessages.Count > 0)
                {
                    foreach (NCError pendingMessage in _pendingMessages)
                        _threadPendingMessages.Enqueue(pendingMessage);
                    _pendingMessages.Clear();
                }
                for (int i = 0; i < _pendingMessagesList.Count; i++)
                {
                    _threadPendingMessages.Enqueue(_pendingMessagesList[i]);
                }
                _pendingMessagesList.Clear();
            }
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: _networkIndex);
            int type = (int)ncError.type;
        }

        protected void SpinThread()
        {
            while (!_killThread)
            {
                Thread.Sleep(8);
                Thread_Loop();
            }
            _pendingMessagesList.Add(new NCError("|DGBLUE|NETCORE |DGRED|Network thread ended", NCErrorType.Debug));
            _killThread = false;
        }

        protected abstract NCError OnSpinServerThread();

        protected abstract NCError OnSpinClientThread();

        public NetworkConnection AttemptConnection(object context, bool host = false)
        {
            if (context is string)
            {
                string str = (string)context;
                context = GetConnectionObject((string)context);
                if (context == null)
                {
                    DevConsole.Log(DCSection.NetCore, "@errorConnection attempt with" + str + "failed (INVALID)@error", _networkIndex);
                    return null;
                }
            }
            if (context == null)
            {
                DevConsole.Log(DCSection.NetCore, "@error Null connection attempt, this shouldn't happen!@error", _networkIndex);
                return null;
            }
            if (DuckNetwork.localConnection.status != ConnectionStatus.Connected)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection ignored due to full disconnect in progress.@error");
                return null;
            }
            NetworkConnection orAddConnection = GetOrAddConnection(context);
            if (orAddConnection.status != ConnectionStatus.Connected && !orAddConnection.banned && Network.isServer && orAddConnection.data is User && Options.Data.blockedPlayers.Contains((orAddConnection.data as User).id))
            {
                DevConsole.Log(DCSection.NetCore, "@error Ignoring connection from " + orAddConnection.ToString() + "(blocked)@error");
                orAddConnection.banned = true;
            }
            if (orAddConnection.banned)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection ignored due to ban.@error");
                return null;
            }
            orAddConnection.recentlyReceivedPackets.Clear();
            orAddConnection.recentlyReceivedPacketsArray = new ushort[NetworkConnection.kMaxRecentlyReceivedPackets];
            orAddConnection.recentlyReceivedPacketsArrayIndex = 0;
            if (orAddConnection.status != ConnectionStatus.Disconnected && orAddConnection.status != ConnectionStatus.Disconnecting)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection attempt skipped (Already Connected)@error", orAddConnection, _networkIndex);
                return orAddConnection;
            }
            orAddConnection.Reset("Connection Attempt");
            orAddConnection.isHost = host;
            orAddConnection.BeginConnection();
            orAddConnection.StartNewSession();
            if (host)
                DevConsole.Log(DCSection.NetCore, "Attempting connection to |DGGREEN|Host(" + orAddConnection.ToString() + ")", orAddConnection, _networkIndex);
            else
                DevConsole.Log(DCSection.NetCore, "Attempting connection to |DGYELLOW|Client(" + orAddConnection.ToString() + ")", orAddConnection, _networkIndex);
            return orAddConnection;
        }

        protected virtual object GetConnectionObject(string identifier) => null;

        protected void OnAttemptConnection(object context)
        {
        }

        public void SendPeerInfo(object context) => OnSendPeerInfo(context);

        public virtual NCError OnSendPeerInfo(object context) => null;

        public void PushNewConnection(NetworkConnection c)
        {
            lock (_connectionHistory)
            {
                _connectionHistory.Add(c);
                _connectionsDirty = true;
            }
        }

        protected NetworkConnection GetOrAddConnection(object context)
        {
            string id = GetConnectionIdentifier(context);
            NetworkConnection c = allConnections.FirstOrDefault(x => x.identifier == id);
            if (c == null)
            {
                c = new NetworkConnection(context);
                PushNewConnection(c);
            }
            return c;
        }

        protected NetworkConnection GetConnection(object context)
        {
            string id = GetConnectionIdentifier(context);
            NetworkConnection[] _allConnections = allConnections.ToArray();
            for (int i = 0; i < _allConnections.Length; i++)
            {
                if (_allConnections[i].identifier == id)
                {
                    return _allConnections[i];
                }
            }
            return default;
        }

        public void DisconnectClient(NetworkConnection connection, DuckNetErrorInfo error, bool kicked = false)
        {
            if (connection == null)
                return;
            string reason = "|LIME|You have disconnected.";
            if (error != null)
            {
                string[] strArray = error.message.Split('\n');
                if (strArray.Length != 0)
                    reason = strArray[0];
            }
            if (connection.status != ConnectionStatus.Disconnected)
            {
                if (connection.status == ConnectionStatus.Disconnecting)
                    return;
                currentError = error;
                connection.BeginDisconnecting(error);
                if (Level.current != null)
                    Level.current.OnDisconnect(connection);
                DuckNetwork.OnDisconnect(connection, reason, kicked || error != null && (error.error == DuckNetError.Kicked || error.error == DuckNetError.Banned));
                currentError = null;
                if (connection.data == null)
                    DevConsole.Log(DCSection.NetCore, "@disconnect You (LOCAL) are disconnecting...|DGRED|(" + reason + ")", _networkIndex);
                else
                    DevConsole.Log(DCSection.NetCore, "@disconnect Player Disconnecting...|DGRED|(" + reason + ")", connection, _networkIndex);
                if (connection == DuckNetwork.localConnection)
                {
                    currentMainDisconnectError = error;
                    foreach (NetworkConnection sessionConnection in sessionConnections)
                    {
                        if (sessionConnection != connection)
                            DisconnectClient(sessionConnection, error);
                    }
                    _hardDisconnectTimeout = 240;
                }
                if (connection.profile == null)
                    return;
                connection.profile.networkStatus = DuckNetStatus.Disconnecting;
            }
            else
            {
                if (connection.data == null)
                    DevConsole.Log(DCSection.NetCore, "@disconnect You (LOCAL) have disconnected.|DGRED|(" + reason + ")(" + connection.sessionID.ToString() + ")", _networkIndex);
                else
                    DevConsole.Log(DCSection.NetCore, "@disconnect Player Disconnected!|DGRED|(" + reason + ")(" + connection.sessionID.ToString() + ")", connection, _networkIndex);
                if (connection.profile != null)
                    connection.profile.networkStatus = DuckNetStatus.Disconnected;
                if (GhostManager.context != null)
                    GhostManager.context.OnDisconnect(connection);
                Disconnect(connection);
                connection.Reset("Client Disconnected.");
                if (currentMainDisconnectError != null && currentMainDisconnectError.error == DuckNetError.EveryoneDisconnected)
                    currentMainDisconnectError = error;
                if (sessionConnections.Count != 0 && (sessionConnections.Count != 1 || sessionConnections[0] != DuckNetwork.localConnection || Network.inLobby))
                    return;
                if (!Network.isServer || sessionConnections.Count == 0)
                    OnSessionEnded(currentMainDisconnectError != null ? currentMainDisconnectError : error);
                else
                    DuckNetwork.TryPeacefulResolution();
            }
        }

        protected virtual void Disconnect(NetworkConnection c)
        {
        }

        public void OnSessionEnded(DuckNetErrorInfo error)
        {
            _dataLayer.EndSession();
            if (_networkThread != null && _networkThread.IsAlive)
            {
                _killThread = true;
                for (int index = 0; _killThread && index < 20; ++index)
                    Thread.Sleep(100);
            }
            if (_killThread)
            {
                _networkThread.Abort();
                if (_timeThread != null)
                    _timeThread.Abort();
            }
            _networkThread = null;
            KillConnection();
            _pendingPackets.Clear();
            _pendingMessages.Clear();
            _ghostManager = new GhostManager();
            Network.activeNetwork.Reset();
            Network.isServer = true;
            Network.MakeInactive();
            DevConsole.Log(DCSection.NetCore, "@disconnect Session has ended (OnSessionEnded called, " + Network.activeNetwork.core.connections.Count.ToString() + " connections)", _networkIndex);
            DuckNetwork.OnSessionEnded();
            if (Level.current != null)
                Level.current.OnSessionEnded(error);
            else
                Level.current = error == null ? new ConnectionError("|RED|Disconnected from game.") : (Level)new ConnectionError(error.message);
            if (UIMatchmakerMark2.instance != null)
                UIMatchmakerMark2.instance.Hook_OnSessionEnded(error);
            currentMainDisconnectError = null;
            _hardDisconnectTimeout = -1;
            lock (_connectionHistory)
            {
                _connectionHistory.Clear();
                _connectionsDirty = true;
            }
        }

        /// <summary>
        /// Begin searching for lobby games. (Abstracted during Switch port)
        /// </summary>
        public abstract void SearchForLobby();

        public abstract void AddLobbyStringFilter(string key, string value, LobbyFilterComparison op);

        public abstract void AddLobbyNumericalFilter(string key, int value, LobbyFilterComparison op);

        public abstract void ApplyTS2LobbyFilters();

        public abstract void RequestGlobalStats();

        public abstract int NumLobbiesFound();

        public abstract bool TryRequestDailyKills(out long numKills);

        public abstract Lobby GetSearchLobbyAtIndex(int i);

        public virtual void ApplyLobbyData()
        {
        }

        public void UpdateRandomID(Lobby l)
        {
            l.randomID = Rando.Int(2147483646);
            l.SetLobbyData("randomID", l.randomID.ToString());
        }

        /// <summary>
        /// Update the search for lobby games. (Abstracted during Switch port)
        /// </summary>
        /// <returns>True when lobby search has completed</returns>
        public abstract bool IsLobbySearchComplete();

        public void OnConnection(NetworkConnection connection)
        {
            DevConsole.Log(DCSection.NetCore, "|LIME|Connection established! (" + connection.sessionID.ToString() + ")", connection, _networkIndex);
            DuckNetwork.OnConnection(connection);
        }

        protected void OnPacket(byte[] data, object context)
        {
            NetworkConnection connection = GetConnection(context);
            if (connection == null)
            {
                if (context != null && context is User)
                    _pendingMessagesList.Add(new NCError("|DGBLUE|NETCORE |DGRED|Packet received from unknown connection(" + (context as User).id.ToString() + "," + (context as User).name + ").", NCErrorType.Debug));
                else
                    _pendingMessagesList.Add(new NCError("|DGBLUE|NETCORE |DGRED|Packet received from unknown connection.", NCErrorType.Debug));
            }
            else if (connection.banned)
            {
                if (connection.failureNotificationCooldown > 0)
                    return;
                if (Network.activeNetwork.core.lobby != null && connection.data is User)
                    Steam_LobbyMessage.Send("COM_FAIL", connection.data as User);
                _pendingMessagesList.Add(new NCError("|DGBLUE|NETCORE |DGRED|Ignoring packet (banned)(" + connection.ToString() + ").", NCErrorType.Debug));
                connection.failureNotificationCooldown = 60;
            }
            else
            {
                NetworkPacket networkPacket = new NetworkPacket(new BitBuffer(data), connection, 0);
                if (NetworkDebugger.enabled)
                    NetworkDebugger.LogReceive(NetworkDebugger.GetID(_networkIndex), networkPacket.connection.identifier);
                try
                {
                    networkPacket.sessionID = networkPacket.data.ReadUInt();
                    ushort num1 = networkPacket.data.ReadUShort();
                    ushort num2 = networkPacket.data.ReadUShort();
                    lock (networkPacket.connection.acksReceived)
                    {
                        networkPacket.connection.acksReceived.Add(num1);
                        for (ushort index = 0; index < 16; ++index)
                        {
                            if ((num2 & networkPacket.connection.kAckOffsets[index]) != 0)
                                networkPacket.connection.acksReceived.Add((ushort)(num1 - (index + 1U)));
                        }
                    }
                    bool flag = networkPacket.data.ReadBool();
                    networkPacket.serverPacket = flag;
                    if (!networkPacket.data.ReadBool())
                        return;
                    networkPacket.order = networkPacket.data.ReadUShort();
                    networkPacket.valid = true;
                    if (_pendingPackets.Count > 200)
                    {
                        _pendingMessagesList.Add(new NCError("|DGRED|Discarding packets due to overflow..", NCErrorType.Debug));
                        //this._discardMessagePlayed = true;
                    }
                    else
                    {
                        //this._discardMessagePlayed = false;
                        try
                        {
                            networkPacket.Unpack();
                        }
                        catch (Exception ex)
                        {
                            DevConsole.Log(DCSection.NetCore, "|DGRED|Message unpack failure, possible corruption");
                            DevConsole.Log("");
                            DevConsole.Log(DCSection.NetCore, ex.ToString());
                            DevConsole.Log("");
                            DevConsole.Log("");
                            DevConsole.Log(DCSection.NetCore, $"special:{Main.SpecialCode} hyperSpecial:{Main.SpecialCode2}");
                            DevConsole.Log(DCSection.NetCore, "IF YOU SEE THIS, PLEASE REPORT IT TO A DGR DEV");
                            Program.LogLine("Message unpack failure, possible corruption.");
                            return;
                        }
                        lock (_pendingPackets)
                            _pendingPackets.Enqueue(networkPacket);
                    }
                }
                catch (Exception ex)
                {
                    _pendingMessagesList.Add(new NCError("@error |DGRED|OnPacket exception:", NCErrorType.Debug));
                    _pendingMessagesList.Add(new NCError(ex.Message, NCErrorType.Debug));
                }
            }
        }

        public void SendPacket(NetworkPacket packet, NetworkConnection connection)
        {
            if (connection == null)
            {
                DevConsole.Log(DCSection.NetCore, "|DGRED|Trying to send packet with no connection.", _core.networkIndex);
            }
            else
            {
                if (NetworkDebugger.enabled)
                    NetworkDebugger.LogSend(NetworkDebugger.GetID(_networkIndex), connection.identifier);
                UIMatchmakingBox.core.pulseLocal = true;
                BitBuffer sendData = new BitBuffer();
                sendData.Write(connection.sessionID);
                sendData.Write(connection.GetAck());
                sendData.Write(connection.GetAckBitfield());
                if (Network.isServer)
                    sendData.Write(true);
                else
                    sendData.Write(false);
                if (packet != null)
                {
                    sendData.Write(true);
                    sendData.Write(packet.order);
                    sendData.Write(packet.data, false);
                    sendData.Write(true);
                    sendData.Write((int)Network.synchronizedTime);
                }
                else
                    sendData.Write(false);
                connection.sentThisFrame = true;
                NCError ncError = _dataLayer.SendPacket(sendData, connection);
                connection.PacketSent();
                if (ncError == null)
                    return;
                DevConsole.Log(ncError.text, ncError.color, index: _core.networkIndex);
            }
        }

        public abstract NCError OnSendPacket(byte[] data, int length, object connection);

        public abstract string GetConnectionIdentifier(object connection);

        public abstract string GetConnectionName(object connection);

        public string GetLocalName()
        {
            string localName = OnGetLocalName();
            if (localName.Length > 18)
                localName = localName.Substring(0, 18);
            return localName;
        }

        protected virtual string OnGetLocalName() => "";

        public virtual void Update()
        {
            if (_hardDisconnectTimeout > 0)
            {
                --_hardDisconnectTimeout;
                if (_hardDisconnectTimeout == 0)
                {
                    _hardDisconnectTimeout = -1;
                    Network.Terminate();
                    return;
                }
            }
            _dataLayer.Update();
            lock (_threadPendingMessages)
            {
                foreach (NCError threadPendingMessage in _threadPendingMessages)
                    DevConsole.Log(threadPendingMessage.text, threadPendingMessage.color, index: _networkIndex);
                _threadPendingMessages.Clear();
            }
            List<NetworkPacket> networkPacketList = null;
            lock (_pendingPackets)
            {
                networkPacketList = new List<NetworkPacket>(_pendingPackets);
                _pendingPackets.Clear();
            }
            if (networkPacketList.Count > 1)
            {
                for (int index = 1; index < networkPacketList.Count; ++index)
                {
                    NetworkPacket networkPacket1 = networkPacketList[index - 1];
                    NetworkPacket networkPacket2 = networkPacketList[index];
                    if (networkPacket2.order < networkPacket1.order)
                    {
                        networkPacketList[index] = networkPacket1;
                        networkPacketList[index - 1] = networkPacket2;
                        index = 0;
                    }
                }
            }
            foreach (NetworkPacket packet in networkPacketList)
            {
                if (packet.valid)
                {
                    NetworkConnection.context = packet.connection;
                    if (packet.connection.banned)
                        DevConsole.Log(DCSection.NetCore, "|DGRED|Ignoring message received from BANNED client(" + packet.connection.ToString() + ").");
                    else if (packet.serverPacket && !packet.connection.isHost)
                    {
                        DevConsole.Log(DCSection.NetCore, "|DGRED|Ignoring message from invalid host (host migration)(" + packet.connection.ToString() + ").");
                    }
                    else
                    {
                        try
                        {
                            packet.Unpack();
                        }
                        catch (Exception ex)
                        {
                            DevConsole.Log(DCSection.NetCore, "|DGRED|Message unpack failure, possible corruption");
                            DevConsole.Log("");
                            DevConsole.Log(DCSection.NetCore, ex.ToString());
                            DevConsole.Log("");
                            DevConsole.Log("");
                            DevConsole.Log(DCSection.NetCore, $"special:{Main.SpecialCode} hyperSpecial:{Main.SpecialCode2}");
                            DevConsole.Log(DCSection.NetCore, "IF YOU SEE THIS, PLEASE REPORT IT TO A DGR DEV");
                            Program.LogLine("Message unpack failure, possible corruption.");
                            continue;
                        }
                        List<NetMessage> allMessages = packet.GetAllMessages();
                        //this._packetHeat += allMessages.Count<NetMessage>();
                        foreach (NetMessage pMessage in allMessages)
                            packet.connection.OnAnyMessage(pMessage);
                        if (packet.connection.status == ConnectionStatus.Disconnecting || packet.connection.status == ConnectionStatus.Disconnected)
                        {
                            DevConsole.Log(DCSection.NetCore, "Dropping packet from client (" + packet.connection.status.ToString() + ")");
                            packet.dropPacket = true;
                        }
                        if (packet.IsValidSession())
                        {
                            if (!packet.dropPacket)
                            {
                                packet.connection.PacketReceived(packet);
                                packet.connection.manager.MessagesReceived(packet.GetAllMessages());
                            }
                        }
                        else if (packet.connection.status != ConnectionStatus.Connecting)
                            DevConsole.Log(DCSection.NetCore, "Dropping packet, invalid session (" + packet.sessionID.ToString() + " vs " + packet.connection.sessionID.ToString() + ")");
                        NetworkConnection.context = null;
                    }
                }
                else
                    DevConsole.Log(DCSection.DuckNet, "|DGRED|Ignoring data from " + packet.connection.name + " due to packet.valid flag");
            }
            _ghostManager.PreUpdate();
            foreach (NetworkConnection sessionConnection in sessionConnections)
            {
                (NetworkConnection.context = sessionConnection).Update();
                NetworkConnection.context = null;
            }
            _ghostManager.UpdateSynchronizedEvents();
        }

        public float averagePing
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0f;
                float num = 0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.ping;
                return num / connections.Count;
            }
        }

        public float averagePingPeak
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0f;
                float num = 0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.pingPeak;
                return num / connections.Count;
            }
        }

        public float averageJitter
        {
            get
            {
                float num = 0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.jitter;
                return num / connections.Count;
            }
        }

        public float averageJitterPeak
        {
            get
            {
                float num = 0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.jitterPeak;
                return num / connections.Count;
            }
        }

        public int averagePacketLoss
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0;
                int num = 0;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.losses;
                return num / connections.Count;
            }
        }

        public int averagePacketLossPercent
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0;
                int num1 = 0;
                int num2 = 0;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                {
                    num1 += networkConnection.manager.losses;
                    num2 += networkConnection.manager.sent;
                }
                return num1 == 0 ? 0 : (int)Math.Round(num1 / connections.Count / (num2 / connections.Count) * 100f);
            }
        }

        public void PostUpdate()
        {
            if (!Network.isActive)
                return;
            List<NetworkConnection> allConnections = this.allConnections;
            if (DuckNetwork.localProfile != null && Level.current != null && Level.current.levelIsUpdating && Level.current.transferCompleteCalled)
            {
                _ghostManager.UpdateInit();
                _ghostManager.UpdateGhostLerp();
                _ghostManager.RefreshGhosts();
            }
            ++frame;
            int num = 0;
            for (int index = 0; index < allConnections.Count; ++index)
            {
                NetworkConnection.connectionLoopIndex = num;
                NetworkConnection networkConnection = allConnections[index];
                NetworkConnection.context = networkConnection;
                if (networkConnection.logTransferSize != 0)
                    ConnectionStatusUI.core.tempShow = 2;
                if (networkConnection.profile == null || !networkConnection.banned)
                    networkConnection.PostUpdate(frame);
                NetworkConnection.context = null;
                ++num;
            }
            if (DuckNetwork.localProfile == null)
                return;
            _ghostManager.PostUpdate();
        }

        public void PostDraw()
        {
            if (!Network.isActive)
                return;
            _ghostManager.PostDraw();
        }

        protected virtual void KillConnection()
        {
        }

        public void ForcefulTermination()
        {
            OnSessionEnded(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Forceful network termination."));
            lock (_connectionHistory)
            {
                _connectionHistory.Clear();
                _connectionsDirty = true;
            }
        }

        public virtual void Terminate()
        {
            _hardDisconnectTimeout = -1;
            DevConsole.Log(DCSection.NetCore, "|DGRED|@error -----------Beginning Hard Network Termimation-----------@error");
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Network was forcefully terminated."));
            foreach (NetworkConnection allConnection in Network.activeNetwork.core.allConnections)
                allConnection.HardTerminate();
            for (int index = 0; index < 60 && _networkThread != null && _networkThread.IsAlive; ++index)
            {
                Network.PreUpdate();
                Network.PostUpdate();
                Thread.Sleep(16);
            }
            if (_networkThread != null && _networkThread.IsAlive)
            {
                _networkThread.Abort();
                _networkThread = null;
            }
            DevConsole.Log(DCSection.NetCore, "|DGRED|@error -----------Network termination Complete-----------@error");
        }
    }
}
