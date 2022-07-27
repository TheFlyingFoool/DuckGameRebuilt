// Decompiled with JetBrains decompiler
// Type: DuckGame.NCNetworkImplementation
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public DataLayer dataLayer => this._dataLayer;

        private void RefreshConnections()
        {
            lock (this._connectionHistory)
            {
                this._connectionsInternal = new List<NetworkConnection>();
                this._connectionsInternalAll = new List<NetworkConnection>();
                foreach (NetworkConnection networkConnection in this._connectionHistory)
                {
                    if (networkConnection.status != ConnectionStatus.Disconnected)
                        this._connectionsInternal.Add(networkConnection);
                    this._connectionsInternalAll.Add(networkConnection);
                }
            }
            this._connectionsDirty = false;
        }

        public List<NetworkConnection> connections
        {
            get
            {
                if (this._connectionsDirty)
                    this.RefreshConnections();
                return this._connectionsInternal;
            }
        }

        public List<NetworkConnection> allConnections
        {
            get
            {
                if (this._connectionsDirty)
                    this.RefreshConnections();
                return this._connectionsInternalAll;
            }
        }

        public List<NetworkConnection> sessionConnections
        {
            get
            {
                List<NetworkConnection> sessionConnections = new List<NetworkConnection>((IEnumerable<NetworkConnection>)this.connections);
                if (DuckNetwork.localConnection.status != ConnectionStatus.Disconnected)
                    sessionConnections.Add(DuckNetwork.localConnection);
                return sessionConnections;
            }
        }

        public bool isServer
        {
            get => this._isServer;
            set => this._isServer = value;
        }

        public bool isServerP2P
        {
            get => this._isServerP2P;
            set => this._isServerP2P = value;
        }

        public bool isActive => this._networkThread != null;

        public Lobby lobby => this._lobby;

        public GhostManager ghostManager => this._ghostManager;

        public NetGraph netGraph => this._netGraph;

        public NCNetworkImplementation(Network core, int networkIndex)
        {
            this._core = core;
            this._networkIndex = networkIndex;
            this._ghostManager = new GhostManager();
            if (NetworkDebugger.enabled)
                this._dataLayer = (DataLayer)new DataLayerDebug(this);
            else
                this._dataLayer = new DataLayer(this);
        }

        public void HostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections)
        {
            this._pendingPackets.Clear();
            this._dataLayer.Reset();
            NCError ncError = this.OnHostServer(identifier, port, lobbyType, maxConnections);
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: this._networkIndex);
        }

        public abstract NCError OnHostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections);

        public void JoinServer(string identifier, int port, string ip)
        {
            this._pendingPackets.Clear();
            this._dataLayer.Reset();
            NCError ncError = this.OnJoinServer(identifier, port, ip);
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: this._networkIndex);
        }

        public abstract NCError OnJoinServer(string identifier, int port, string ip);

        protected void StartServerThread()
        {
            this._hardDisconnectTimeout = -1;
            this._isServer = true;
            this._isServerP2P = true;
            this._killThread = false;
            this._networkThread = new Thread(new ThreadStart(this.SpinThread));
            this._networkThread.CurrentCulture = CultureInfo.InvariantCulture;
            this._networkThread.Priority = ThreadPriority.Normal;
            this._networkThread.IsBackground = true;
            this._networkThread.Start();
            Network.MakeActive();
        }

        protected void StartClientThread()
        {
            this._hardDisconnectTimeout = -1;
            this._isServer = false;
            this._isServerP2P = false;
            this._killThread = false;
            this._networkThread = new Thread(new ThreadStart(this.SpinThread));
            this._networkThread.CurrentCulture = CultureInfo.InvariantCulture;
            this._networkThread.Priority = ThreadPriority.Normal;
            this._networkThread.IsBackground = true;
            this._networkThread.Start();
            Network.MakeActive();
        }

        private static uint SwapEndianness(ulong x) => (uint)((ulong)((((long)x & (long)byte.MaxValue) << 24) + (((long)x & 65280L) << 8)) + ((x & 16711680UL) >> 8) + ((x & 4278190080UL) >> 24));

        public void Thread_Loop()
        {
            NCError ncError = !this._isServer ? this.OnSpinClientThread() : this.OnSpinServerThread();
            lock (this._threadPendingMessages)
            {
                foreach (NCError pendingMessage in this._pendingMessages)
                    this._threadPendingMessages.Enqueue(pendingMessage);
                this._pendingMessages.Clear();
            }
            if (ncError == null)
                return;
            DevConsole.Log(ncError.text, ncError.color, index: this._networkIndex);
            int type = (int)ncError.type;
        }

        protected void SpinThread()
        {
            while (!this._killThread)
            {
                Thread.Sleep(8);
                this.Thread_Loop();
            }
            this._pendingMessages.Enqueue(new NCError("|DGBLUE|NETCORE |DGRED|Network thread ended", NCErrorType.Debug));
            this._killThread = false;
        }

        protected abstract NCError OnSpinServerThread();

        protected abstract NCError OnSpinClientThread();

        public NetworkConnection AttemptConnection(object context, bool host = false)
        {
            if (context is string)
            {
                string str = (string)context;
                context = this.GetConnectionObject((string)context);
                if (context == null)
                {
                    DevConsole.Log(DCSection.NetCore, "@errorConnection attempt with" + str + "failed (INVALID)@error", this._networkIndex);
                    return (NetworkConnection)null;
                }
            }
            if (context == null)
            {
                DevConsole.Log(DCSection.NetCore, "@error Null connection attempt, this shouldn't happen!@error", this._networkIndex);
                return (NetworkConnection)null;
            }
            if (DuckNetwork.localConnection.status != ConnectionStatus.Connected)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection ignored due to full disconnect in progress.@error");
                return (NetworkConnection)null;
            }
            NetworkConnection orAddConnection = this.GetOrAddConnection(context);
            if (orAddConnection.status != ConnectionStatus.Connected && !orAddConnection.banned && Network.isServer && orAddConnection.data is User && Options.Data.blockedPlayers.Contains((orAddConnection.data as User).id))
            {
                DevConsole.Log(DCSection.NetCore, "@error Ignoring connection from " + orAddConnection.ToString() + "(blocked)@error");
                orAddConnection.banned = true;
            }
            if (orAddConnection.banned)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection ignored due to ban.@error");
                return (NetworkConnection)null;
            }
            orAddConnection.recentlyReceivedPackets.Clear();
            orAddConnection.recentlyReceivedPacketsArray = new ushort[NetworkConnection.kMaxRecentlyReceivedPackets];
            orAddConnection.recentlyReceivedPacketsArrayIndex = (byte)0;
            if (orAddConnection.status != ConnectionStatus.Disconnected && orAddConnection.status != ConnectionStatus.Disconnecting)
            {
                DevConsole.Log(DCSection.NetCore, "@error Connection attempt skipped (Already Connected)@error", orAddConnection, this._networkIndex);
                return orAddConnection;
            }
            orAddConnection.Reset("Connection Attempt");
            orAddConnection.isHost = host;
            orAddConnection.BeginConnection();
            orAddConnection.StartNewSession();
            if (host)
                DevConsole.Log(DCSection.NetCore, "Attempting connection to |DGGREEN|Host(" + orAddConnection.ToString() + ")", orAddConnection, this._networkIndex);
            else
                DevConsole.Log(DCSection.NetCore, "Attempting connection to |DGYELLOW|Client(" + orAddConnection.ToString() + ")", orAddConnection, this._networkIndex);
            return orAddConnection;
        }

        protected virtual object GetConnectionObject(string identifier) => (object)null;

        protected void OnAttemptConnection(object context)
        {
        }

        public void SendPeerInfo(object context) => this.OnSendPeerInfo(context);

        public virtual NCError OnSendPeerInfo(object context) => (NCError)null;

        public void PushNewConnection(NetworkConnection c)
        {
            lock (this._connectionHistory)
            {
                this._connectionHistory.Add(c);
                this._connectionsDirty = true;
            }
        }

        protected NetworkConnection GetOrAddConnection(object context)
        {
            string id = this.GetConnectionIdentifier(context);
            NetworkConnection c = this.allConnections.FirstOrDefault<NetworkConnection>((Func<NetworkConnection, bool>)(x => x.identifier == id));
            if (c == null)
            {
                c = new NetworkConnection(context);
                this.PushNewConnection(c);
            }
            return c;
        }

        protected NetworkConnection GetConnection(object context)
        {
            string id = this.GetConnectionIdentifier(context);
            return this.allConnections.FirstOrDefault<NetworkConnection>((Func<NetworkConnection, bool>)(x => x.identifier == id));
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
                NCNetworkImplementation.currentError = error;
                connection.BeginDisconnecting(error);
                if (Level.current != null)
                    Level.current.OnDisconnect(connection);
                DuckNetwork.OnDisconnect(connection, reason, kicked || error != null && (error.error == DuckNetError.Kicked || error.error == DuckNetError.Banned));
                NCNetworkImplementation.currentError = (DuckNetErrorInfo)null;
                if (connection.data == null)
                    DevConsole.Log(DCSection.NetCore, "@disconnect You (LOCAL) are disconnecting...|DGRED|(" + reason + ")", this._networkIndex);
                else
                    DevConsole.Log(DCSection.NetCore, "@disconnect Player Disconnecting...|DGRED|(" + reason + ")", connection, this._networkIndex);
                if (connection == DuckNetwork.localConnection)
                {
                    NCNetworkImplementation.currentMainDisconnectError = error;
                    foreach (NetworkConnection sessionConnection in this.sessionConnections)
                    {
                        if (sessionConnection != connection)
                            this.DisconnectClient(sessionConnection, error);
                    }
                    this._hardDisconnectTimeout = 240;
                }
                if (connection.profile == null)
                    return;
                connection.profile.networkStatus = DuckNetStatus.Disconnecting;
            }
            else
            {
                if (connection.data == null)
                    DevConsole.Log(DCSection.NetCore, "@disconnect You (LOCAL) have disconnected.|DGRED|(" + reason + ")(" + connection.sessionID.ToString() + ")", this._networkIndex);
                else
                    DevConsole.Log(DCSection.NetCore, "@disconnect Player Disconnected!|DGRED|(" + reason + ")(" + connection.sessionID.ToString() + ")", connection, this._networkIndex);
                if (connection.profile != null)
                    connection.profile.networkStatus = DuckNetStatus.Disconnected;
                if (GhostManager.context != null)
                    GhostManager.context.OnDisconnect(connection);
                this.Disconnect(connection);
                connection.Reset("Client Disconnected.");
                if (NCNetworkImplementation.currentMainDisconnectError != null && NCNetworkImplementation.currentMainDisconnectError.error == DuckNetError.EveryoneDisconnected)
                    NCNetworkImplementation.currentMainDisconnectError = error;
                if (this.sessionConnections.Count != 0 && (this.sessionConnections.Count != 1 || this.sessionConnections[0] != DuckNetwork.localConnection || Network.InLobby()))
                    return;
                if (!Network.isServer || this.sessionConnections.Count == 0)
                    this.OnSessionEnded(NCNetworkImplementation.currentMainDisconnectError != null ? NCNetworkImplementation.currentMainDisconnectError : error);
                else
                    DuckNetwork.TryPeacefulResolution();
            }
        }

        protected virtual void Disconnect(NetworkConnection c)
        {
        }

        public void OnSessionEnded(DuckNetErrorInfo error)
        {
            this._dataLayer.EndSession();
            if (this._networkThread != null && this._networkThread.IsAlive)
            {
                this._killThread = true;
                for (int index = 0; this._killThread && index < 20; ++index)
                    Thread.Sleep(100);
            }
            if (this._killThread)
            {
                this._networkThread.Abort();
                if (this._timeThread != null)
                    this._timeThread.Abort();
            }
            this._networkThread = (Thread)null;
            this.KillConnection();
            this._pendingPackets.Clear();
            this._pendingMessages.Clear();
            this._ghostManager = new GhostManager();
            Network.activeNetwork.Reset();
            Network.isServer = true;
            Network.MakeInactive();
            DevConsole.Log(DCSection.NetCore, "@disconnect Session has ended (OnSessionEnded called, " + Network.activeNetwork.core.connections.Count.ToString() + " connections)", this._networkIndex);
            DuckNetwork.OnSessionEnded();
            if (Level.current != null)
                Level.current.OnSessionEnded(error);
            else
                Level.current = error == null ? (Level)new ConnectionError("|RED|Disconnected from game.") : (Level)new ConnectionError(error.message);
            if (UIMatchmakerMark2.instance != null)
                UIMatchmakerMark2.instance.Hook_OnSessionEnded(error);
            NCNetworkImplementation.currentMainDisconnectError = (DuckNetErrorInfo)null;
            this._hardDisconnectTimeout = -1;
            lock (this._connectionHistory)
            {
                this._connectionHistory.Clear();
                this._connectionsDirty = true;
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
            DevConsole.Log(DCSection.NetCore, "|LIME|Connection established! (" + connection.sessionID.ToString() + ")", connection, this._networkIndex);
            DuckNetwork.OnConnection(connection);
        }

        protected void OnPacket(byte[] data, object context)
        {
            NetworkConnection connection = this.GetConnection(context);
            if (connection == null)
            {
                if (context != null && context is User)
                    this._pendingMessages.Enqueue(new NCError("|DGBLUE|NETCORE |DGRED|Packet received from unknown connection(" + (context as User).id.ToString() + "," + (context as User).name + ").", NCErrorType.Debug));
                else
                    this._pendingMessages.Enqueue(new NCError("|DGBLUE|NETCORE |DGRED|Packet received from unknown connection.", NCErrorType.Debug));
            }
            else if (connection.banned)
            {
                if (connection.failureNotificationCooldown > 0)
                    return;
                if (Network.activeNetwork.core.lobby != null && connection.data is User)
                    Steam_LobbyMessage.Send("COM_FAIL", connection.data as User);
                this._pendingMessages.Enqueue(new NCError("|DGBLUE|NETCORE |DGRED|Ignoring packet (banned)(" + connection.ToString() + ").", NCErrorType.Debug));
                connection.failureNotificationCooldown = 60;
            }
            else
            {
                NetworkPacket networkPacket = new NetworkPacket(new BitBuffer(data), connection, (ushort)0);
                if (NetworkDebugger.enabled)
                    NetworkDebugger.LogReceive(NetworkDebugger.GetID(this._networkIndex), networkPacket.connection.identifier);
                try
                {
                    networkPacket.sessionID = networkPacket.data.ReadUInt();
                    ushort num1 = networkPacket.data.ReadUShort();
                    ushort num2 = networkPacket.data.ReadUShort();
                    lock (networkPacket.connection.acksReceived)
                    {
                        networkPacket.connection.acksReceived.Add(num1);
                        for (ushort index = 0; index < (ushort)16; ++index)
                        {
                            if (((int)num2 & (int)networkPacket.connection.kAckOffsets[(int)index]) != 0)
                                networkPacket.connection.acksReceived.Add((ushort)((uint)num1 - ((uint)index + 1U)));
                        }
                    }
                    bool flag = networkPacket.data.ReadBool();
                    networkPacket.serverPacket = flag;
                    if (!networkPacket.data.ReadBool())
                        return;
                    networkPacket.order = networkPacket.data.ReadUShort();
                    networkPacket.valid = true;
                    if (this._pendingPackets.Count > 200)
                    {
                        this._pendingMessages.Enqueue(new NCError("|DGRED|Discarding packets due to overflow..", NCErrorType.Debug));
                        //this._discardMessagePlayed = true;
                    }
                    else
                    {
                        //this._discardMessagePlayed = false;
                        lock (this._pendingPackets)
                            this._pendingPackets.Enqueue(networkPacket);
                    }
                }
                catch (Exception ex)
                {
                    this._pendingMessages.Enqueue(new NCError("@error |DGRED|OnPacket exception:", NCErrorType.Debug));
                    this._pendingMessages.Enqueue(new NCError(ex.Message, NCErrorType.Debug));
                }
            }
        }

        public void SendPacket(NetworkPacket packet, NetworkConnection connection)
        {
            if (connection == null)
            {
                DevConsole.Log(DCSection.NetCore, "|DGRED|Trying to send packet with no connection.", this._core.networkIndex);
            }
            else
            {
                if (NetworkDebugger.enabled)
                    NetworkDebugger.LogSend(NetworkDebugger.GetID(this._networkIndex), connection.identifier);
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
                NCError ncError = this._dataLayer.SendPacket(sendData, connection);
                connection.PacketSent();
                if (ncError == null)
                    return;
                DevConsole.Log(ncError.text, ncError.color, index: this._core.networkIndex);
            }
        }

        public abstract NCError OnSendPacket(byte[] data, int length, object connection);

        public abstract string GetConnectionIdentifier(object connection);

        public abstract string GetConnectionName(object connection);

        public string GetLocalName()
        {
            string localName = this.OnGetLocalName();
            if (localName.Length > 18)
                localName = localName.Substring(0, 18);
            return localName;
        }

        protected virtual string OnGetLocalName() => "";

        public virtual void Update()
        {
            if (this._hardDisconnectTimeout > 0)
            {
                --this._hardDisconnectTimeout;
                if (this._hardDisconnectTimeout == 0)
                {
                    this._hardDisconnectTimeout = -1;
                    Network.Terminate();
                    return;
                }
            }
            this._dataLayer.Update();
            lock (this._threadPendingMessages)
            {
                foreach (NCError threadPendingMessage in this._threadPendingMessages)
                    DevConsole.Log(threadPendingMessage.text, threadPendingMessage.color, index: this._networkIndex);
                this._threadPendingMessages.Clear();
            }
            List<NetworkPacket> networkPacketList = (List<NetworkPacket>)null;
            lock (this._pendingPackets)
            {
                networkPacketList = new List<NetworkPacket>((IEnumerable<NetworkPacket>)this._pendingPackets);
                this._pendingPackets.Clear();
            }
            if (networkPacketList.Count > 1)
            {
                for (int index = 1; index < networkPacketList.Count; ++index)
                {
                    NetworkPacket networkPacket1 = networkPacketList[index - 1];
                    NetworkPacket networkPacket2 = networkPacketList[index];
                    if ((int)networkPacket2.order < (int)networkPacket1.order)
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
                        NetworkConnection.context = (NetworkConnection)null;
                    }
                }
                else
                    DevConsole.Log(DCSection.DuckNet, "|DGRED|Ignoring data from " + packet.connection.name + " due to packet.valid flag");
            }
            this._ghostManager.PreUpdate();
            foreach (NetworkConnection sessionConnection in this.sessionConnections)
            {
                (NetworkConnection.context = sessionConnection).Update();
                NetworkConnection.context = (NetworkConnection)null;
            }
            this._ghostManager.UpdateSynchronizedEvents();
        }

        public float averagePing
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0.0f;
                float num = 0.0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.ping;
                return num / (float)connections.Count;
            }
        }

        public float averagePingPeak
        {
            get
            {
                if (this.connections.Count == 0)
                    return 0.0f;
                float num = 0.0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.pingPeak;
                return num / (float)connections.Count;
            }
        }

        public float averageJitter
        {
            get
            {
                float num = 0.0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.jitter;
                return num / (float)connections.Count;
            }
        }

        public float averageJitterPeak
        {
            get
            {
                float num = 0.0f;
                List<NetworkConnection> connections = this.connections;
                foreach (NetworkConnection networkConnection in connections)
                    num += networkConnection.manager.jitterPeak;
                return num / (float)connections.Count;
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
                return num1 == 0 ? 0 : (int)Math.Round((double)(num1 / connections.Count) / (double)(num2 / connections.Count) * 100.0);
            }
        }

        public void PostUpdate()
        {
            if (!Network.isActive)
                return;
            List<NetworkConnection> allConnections = this.allConnections;
            if (DuckNetwork.localProfile != null && Level.current != null && Level.current.levelIsUpdating && Level.current.transferCompleteCalled)
            {
                this._ghostManager.UpdateInit();
                this._ghostManager.UpdateGhostLerp();
                this._ghostManager.RefreshGhosts();
            }
            ++this.frame;
            int num = 0;
            for (int index = 0; index < allConnections.Count; ++index)
            {
                NetworkConnection.connectionLoopIndex = num;
                NetworkConnection networkConnection = allConnections[index];
                NetworkConnection.context = networkConnection;
                if (networkConnection.logTransferSize != 0)
                    ConnectionStatusUI.core.tempShow = 2;
                if (networkConnection.profile == null || !networkConnection.banned)
                    networkConnection.PostUpdate(this.frame);
                NetworkConnection.context = (NetworkConnection)null;
                ++num;
            }
            if (DuckNetwork.localProfile == null)
                return;
            this._ghostManager.PostUpdate();
        }

        public void PostDraw()
        {
            if (!Network.isActive)
                return;
            this._ghostManager.PostDraw();
        }

        protected virtual void KillConnection()
        {
        }

        public void ForcefulTermination()
        {
            this.OnSessionEnded(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Forceful network termination."));
            lock (this._connectionHistory)
            {
                this._connectionHistory.Clear();
                this._connectionsDirty = true;
            }
        }

        public virtual void Terminate()
        {
            this._hardDisconnectTimeout = -1;
            DevConsole.Log(DCSection.NetCore, "|DGRED|@error -----------Beginning Hard Network Termimation-----------@error");
            Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Network was forcefully terminated."));
            foreach (NetworkConnection allConnection in Network.activeNetwork.core.allConnections)
                allConnection.HardTerminate();
            for (int index = 0; index < 60 && this._networkThread != null && this._networkThread.IsAlive; ++index)
            {
                Network.PreUpdate();
                Network.PostUpdate();
                Thread.Sleep(16);
            }
            if (this._networkThread != null && this._networkThread.IsAlive)
            {
                this._networkThread.Abort();
                this._networkThread = (Thread)null;
            }
            DevConsole.Log(DCSection.NetCore, "|DGRED|@error -----------Network termination Complete-----------@error");
        }
    }
}
