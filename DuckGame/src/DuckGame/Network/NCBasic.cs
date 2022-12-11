// Decompiled with JetBrains decompiler
// Type: DuckGame.NCBasic
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DuckGame
{
    public class NCBasic : NCNetworkImplementation
    {
        private bool _initializedSettings = true;
        public const long kLanMessageHeader = 2449832521355936907;
        protected UdpClient _socket;
        //private byte[] _receiveBuffer = new byte[4096];
        private string _serverIdentifier = "";
        private List<NCBasicConnection> _basicConnections = new List<NCBasicConnection>();
        public int bytesThisFrame;
        public int headerBytes;
        public int ghostBytes;
        public int ackBytes;
        //private const long kDuckGameLANHeader = 5892070176735;
        private int _port;
        protected IPEndPoint localEndPoint;
        public static string _localName = "";
        private float _broadcastWait;
        public List<UIServerBrowser.LobbyData> _foundLobbies = new List<UIServerBrowser.LobbyData>();
        public List<UIServerBrowser.LobbyData> _threadLobbies;
        private object lobbyLock = new object();
        private bool _lobbyThreadRunning;
        public static int lobbySearchPort = 1337;

        public NCBasic(Network c, int networkIndex)
          : base(c, networkIndex)
        {
        }

        public override NCError OnSendPacket(byte[] data, int length, object connection)
        {
            byte[] numArray = new byte[length + 8];
            BitBuffer bitBuffer = new BitBuffer(numArray, false);
            bitBuffer.Write(2449832521355936907L);
            if (data != null)
                bitBuffer.Write(data, length: length);
            _socket.Send(numArray, length + 8, connection as IPEndPoint);
            bytesThisFrame += length + 8;
            return null;
        }

        protected override object GetConnectionObject(string identifier) => MakeConnection(CreateIPEndPoint(identifier)).connection;

        private void BroadcastServerHeader()
        {
            if (_socket == null)
                return;
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(5892070176735L);
            bitBuffer.Write(DG.versionMajor);
            bitBuffer.Write(DG.versionHigh);
            bitBuffer.Write(DG.versionLow);
            bitBuffer.Write((byte)DuckNetwork.profiles.Where<Profile>(x => x.connection != null).Count<Profile>());
            bitBuffer.Write(TeamSelect2.GetOnlineSetting("name").value);
            bitBuffer.Write(ModLoader.modHash);
            bitBuffer.Write((byte)TeamSelect2.GetSettingInt("requiredwins"));
            bitBuffer.Write((byte)TeamSelect2.GetSettingInt("restsevery"));
            bitBuffer.Write(TeamSelect2.GetSettingBool("wallmode"));
            bitBuffer.Write(Editor.customLevelCount);
            bitBuffer.Write(!(Level.current is TeamSelect2));
            bitBuffer.Write(TeamSelect2.UpdateModifierStatus());
            bitBuffer.Write(DuckNetwork.numSlots);
            bitBuffer.Write((string)TeamSelect2.GetOnlineSetting("password").value != "");
            bitBuffer.Write((bool)TeamSelect2.GetOnlineSetting("dedicated").value);
            bitBuffer.Write(true);
            bitBuffer.Write(Network.gameDataHash);
            if (!TrySend(bitBuffer, "255.255.255.255"))
            {
                List<IPAddress> ips = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList();
                foreach (IPAddress ip in ips)
                {
                    if (TrySend(bitBuffer, ip.ToString())) break;
                }
            }
        }

        bool TrySend(BitBuffer bitBuffer, string endpoint)
        {
            try
            {
                _socket.Send(bitBuffer.buffer, bitBuffer.lengthInBytes, endpoint, _port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override NCError OnHostServer(string identifier, int port, NetworkLobbyType lobbyType, int maxConnections)
        {
            if (_socket != null)
                return new NCError("server is already started...", NCErrorType.Error);
            _basicConnections.Clear();
            _serverIdentifier = identifier;
            _socket = new UdpClient();
            _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            try
            {
                _socket.AllowNatTraversal(true); //There are rare cases this just cases a crash like, proton, Linux, etc so im just going catch it, as it seems fine when it doesnt run, and im unsure what it even does
            }
            catch (Exception ex)
            {
                DevConsole.Log("AllowNatTraversal didnt want to work should be fine still :)");
            }
            localEndPoint = !NetworkDebugger.enabled ? new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port) : new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1330 + NetworkDebugger.currentIndex);
            _port = port;
            _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            StartServerThread();
            return new NCError("server started on port " + port.ToString() + ".", NCErrorType.Success);
        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            if (!endPoint.Contains(":"))
            {
                endPoint += ":1337";
            }
            string[] strArray = endPoint.Split(':');
            if (strArray.Length < 2)
                throw new FormatException("Invalid endpoint format");
            IPAddress address;
            if (strArray.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", strArray, 0, strArray.Length - 1), out address))
                    throw new FormatException("Invalid ip-adress");
            }
            else if (!IPAddress.TryParse(strArray[0], out address))
                throw new FormatException("Invalid ip-adress");
            int result;
            if (!int.TryParse(strArray[strArray.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out result))
                throw new FormatException("Invalid port");
            return new IPEndPoint(address, result);
        }

        public override NCError OnJoinServer(string identifier, int port, string ip)
        {
            switch (ip)
            {
                case "":
                case "localhost":
                case null:
                    return new NCError("Invalid LAN IP String, format must be IP:PORT", NCErrorType.CriticalError);
                default:
                    if (_socket != null)
                        return new NCError("client is already started...", NCErrorType.Error);
                    IPEndPoint endPoint = !(ip == "netdebug") ? NCBasic.CreateIPEndPoint(ip) : new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1330 + NetworkDebugger.CurrentServerIndex());
                    _serverIdentifier = identifier;
                    _basicConnections.Clear();
                    _socket = new UdpClient();
                    _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _port = port;
                    int port1 = 1336;
                    if (NetworkDebugger.enabled)
                    {
                        int port2 = 1330 + NetworkDebugger.currentIndex;
                        localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port2);
                    }
                    else
                        localEndPoint = new IPEndPoint(IPAddress.Any, port1);
                    _socket.Client.Bind(localEndPoint);
                    try
                    {
                        _socket.AllowNatTraversal(true); //There are rare cases this just cases a crash like, proton, Linux, etc so im just going catch it, as it seems fine when it doesnt run, and im unsure what it even does
                    }
                    catch (Exception ex)
                    {
                        DevConsole.Log("AllowNatTraversal didnt want to work should be fine still :)");
                    }
                    MakeConnection(endPoint, true);
                    StartClientThread();
                    return null;
            }
        }

        public NCBasicConnection MakeConnection(IPEndPoint endPoint, bool isHost = false)
        {
            lock (_basicConnections)
            {
                NCBasicConnection ncBasicConnection1 = _basicConnections.FirstOrDefault<NCBasicConnection>(x => x.connection.ToString() == endPoint.ToString());
                if (ncBasicConnection1 != null)
                    return ncBasicConnection1;
                NCBasicConnection ncBasicConnection2 = new NCBasicConnection()
                {
                    connection = endPoint,
                    status = NCBasicStatus.TryingToConnect
                };
                ncBasicConnection2.isHost = isHost;
                _basicConnections.Add(ncBasicConnection2);
                _pendingMessages.Enqueue(new NCError("client connecting to " + endPoint.ToString() + ".", NCErrorType.Message));
                return ncBasicConnection2;
            }
        }

        public override string GetConnectionIdentifier(object connection) => (connection as IPEndPoint).ToString();

        public override string GetConnectionName(object connection) => (connection as IPEndPoint).ToString().ToString();

        protected override string OnGetLocalName() => NCBasic._localName;

        protected override NCError OnSpinServerThread()
        {
            if (_socket == null)
                return NetworkDebugger.enabled ? null : new NCError("NCBasic connection was lost.", NCErrorType.CriticalError);
            for (int index = 0; index < _basicConnections.Count; ++index)
            {
                NCBasicConnection basicConnection = _basicConnections[index];
                if (basicConnection.status != NCBasicStatus.Disconnected)
                {
                    TimeSpan elapsed = basicConnection.heartbeat.elapsed;
                    if (elapsed.TotalSeconds > 1.0 || basicConnection.status == NCBasicStatus.TryingToConnect)
                    {
                        if (basicConnection.status == NCBasicStatus.Disconnecting)
                        {
                            elapsed = basicConnection.heartbeat.elapsed;
                            if (elapsed.TotalSeconds > 3.0)
                            {
                                basicConnection.status = NCBasicStatus.Disconnected;
                                basicConnection.packets = 0;
                            }
                        }
                        else
                        {
                            basicConnection.heartbeat.Restart();
                            BitBuffer bitBuffer = new BitBuffer();
                            OnSendPacket(null, 0, basicConnection.connection);
                            if (basicConnection.status == NCBasicStatus.TryingToConnect)
                                basicConnection.status = NCBasicStatus.Connecting;
                        }
                    }
                }
            }
            Queue<NCBasicPacket> packets = new Queue<NCBasicPacket>();
            ReceivePackets(packets);
            foreach (NCBasicPacket ncBasicPacket in packets)
            {
                IPEndPoint sender = ncBasicPacket.sender;
                byte[] data = ncBasicPacket.data;
                string address = sender.ToString();
                lock (_basicConnections)
                {
                    if (data.Length >= 8)
                    {
                        if (new BitBuffer(data).ReadLong() == 2449832521355936907L)
                        {
                            NCBasicConnection ncBasicConnection = _basicConnections.FirstOrDefault<NCBasicConnection>(x => x.connection.ToString() == address);
                            if (ncBasicConnection == null)
                            {
                                ncBasicConnection = new NCBasicConnection()
                                {
                                    connection = sender,
                                    status = NCBasicStatus.Connecting
                                };
                                _basicConnections.Add(ncBasicConnection);
                                _pendingMessages.Enqueue(new NCError("connection attempt from " + ncBasicConnection.connection.ToString(), NCErrorType.Message));
                            }
                            if (ncBasicConnection != null)
                            {
                                ++ncBasicConnection.packets;
                                if (ncBasicConnection.status == NCBasicStatus.Connected)
                                {
                                    if (data.Length > 8)
                                    {
                                        byte[] numArray = new byte[data.Length - 8];
                                        Array.Copy(data, 8, numArray, 0, data.Length - 8);
                                        OnPacket(numArray, ncBasicConnection.connection);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        protected virtual void ReceivePackets(Queue<NCBasicPacket> packets)
        {
            try
            {
                IPEndPoint remoteEP = null;
                while (_socket.Available > 0)
                {
                    byte[] numArray = _socket.Receive(ref remoteEP);
                    if (numArray != null)
                        packets.Enqueue(new NCBasicPacket()
                        {
                            data = numArray,
                            sender = remoteEP
                        });
                }
            }
            catch
            {
            }
        }

        protected override NCError OnSpinClientThread() => OnSpinServerThread();

        public override void Update()
        {
            if (Network.isActive)
            {
                if (Network.isServer)
                {
                    _broadcastWait += Maths.IncFrameTimer();
                    if (_broadcastWait > 0.75)
                    {
                        _broadcastWait = 0f;
                        BroadcastServerHeader();
                    }
                }
                for (int index = 0; index < _basicConnections.Count; ++index)
                {
                    NCBasicConnection basicConnection = _basicConnections[index];
                    if (basicConnection.status != NCBasicStatus.Connected && basicConnection.packets > 0 && basicConnection.status != NCBasicStatus.Disconnecting)
                    {
                        _pendingMessages.Enqueue(new NCError("connection to " + basicConnection.connection.ToString() + " succeeded!", NCErrorType.Success));
                        basicConnection.status = NCBasicStatus.Connected;
                        AttemptConnection(basicConnection.connection, basicConnection.isHost);
                        basicConnection.isHost = false;
                    }
                }
            }
            base.Update();
        }

        protected override void KillConnection()
        {
            _basicConnections.Clear();
            if (_socket != null)
                _socket.Close();
            _socket = null;
            base.KillConnection();
        }

        protected override void Disconnect(NetworkConnection c)
        {
            if (c != null)
            {
                NCBasicConnection ncBasicConnection = _basicConnections.FirstOrDefault<NCBasicConnection>(x => x.connection.ToString() == c.identifier);
                if (ncBasicConnection != null)
                    ncBasicConnection.status = NCBasicStatus.Disconnecting;
            }
            base.Disconnect(c);
        }

        public override void Terminate() => base.Terminate();

        public override void AddLobbyStringFilter(string key, string value, LobbyFilterComparison op) => Steam.AddLobbyStringFilter(key, value, (SteamLobbyComparison)op);

        public override bool IsLobbySearchComplete()
        {
            if (_lobbyThreadRunning)
                return false;
            if (_threadLobbies != null)
            {
                lock (lobbyLock)
                {
                    _foundLobbies = _threadLobbies;
                    _threadLobbies = null;
                }
            }
            return true;
        }

        private void SearchForLobbyThread()
        {
            _lobbyThreadRunning = true;
            lock (lobbyLock)
            {
                _threadLobbies = new List<UIServerBrowser.LobbyData>();
                SearchForLobbyClient(null);
                if (_threadLobbies.Count > 0)
                {
                    _lobbyThreadRunning = false;
                    return;
                }
                List<IPAddress> ips = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList();
                foreach (IPAddress ip in ips)
                {
                    SearchForLobbyClient(ip);
                    if (_threadLobbies.Count > 0)
                    {
                        _lobbyThreadRunning = false;
                        return;
                    }
                }
            }
            _lobbyThreadRunning = false;
        }

        void SearchForLobbyClient(IPAddress endpoint = null)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(endpoint == null ? IPAddress.Any : endpoint, NCBasic.lobbySearchPort));
                udpClient.Client.ReceiveTimeout = 100;
                IPEndPoint remoteEP = new IPEndPoint(0L, 0);
                for (int index = 0; index < 15; ++index)
                {
                    if (udpClient.Client.Available == 0) // added this because can. prevent crashs when possible :)
                    {
                        Thread.Sleep(100);
                    }
                    if (udpClient.Client.Available == 0)
                    {
                        continue;
                    }//
                    try
                    {
                        byte[] data = udpClient.Receive(ref remoteEP);
                        if (data != null)
                        {
                            BitBuffer bitBuffer = new BitBuffer(data);
                            long num1 = bitBuffer.ReadLong();
                            string address = remoteEP.ToString();
                            if (num1 == 5892070176735L)
                            {
                                if (_threadLobbies.FirstOrDefault<UIServerBrowser.LobbyData>(x => x.lanAddress == address) == null)
                                {
                                    UIServerBrowser.LobbyData lobbyData1 = new UIServerBrowser.LobbyData
                                    {
                                        lanAddress = address
                                    };
                                    int pMajor = bitBuffer.ReadInt();
                                    int pHigh = bitBuffer.ReadInt();
                                    int pLow = bitBuffer.ReadInt();
                                    lobbyData1.version = DG.MakeVersionString(pMajor, pHigh, pLow);
                                    lobbyData1._userCount = bitBuffer.ReadByte();
                                    lobbyData1.name = bitBuffer.ReadString();
                                    lobbyData1.modHash = bitBuffer.ReadString();
                                    lobbyData1.requiredWins = bitBuffer.ReadByte().ToString();
                                    lobbyData1.restsEvery = bitBuffer.ReadByte().ToString();
                                    lobbyData1.wallMode = bitBuffer.ReadBool() ? "true" : "false";
                                    UIServerBrowser.LobbyData lobbyData2 = lobbyData1;
                                    int num2 = bitBuffer.ReadInt();
                                    string str = num2.ToString();
                                    lobbyData2.customLevels = str;
                                    lobbyData1.started = bitBuffer.ReadBool() ? "true" : "false";
                                    lobbyData1.hasModifiers = bitBuffer.ReadBool() ? "true" : "false";
                                    UIServerBrowser.LobbyData lobbyData3 = lobbyData1;
                                    lobbyData1.maxPlayers = num2 = bitBuffer.ReadInt();
                                    int num3 = num2;
                                    lobbyData3.numSlots = num3;
                                    lobbyData1.hasPassword = bitBuffer.ReadBool();
                                    lobbyData1.dedicated = bitBuffer.ReadBool();
                                    if (bitBuffer.positionInBits != bitBuffer.lengthInBits && bitBuffer.ReadBool())
                                        lobbyData1.datahash = bitBuffer.ReadLong();
                                    _threadLobbies.Add(lobbyData1);
                                    --index;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                udpClient.Close();
            }
        }

        public override void SearchForLobby()
        {
            if (_lobbyThreadRunning)
                return;
            _foundLobbies.Clear();
            new Thread(() => SearchForLobbyThread()).Start();
        }

        public override int NumLobbiesFound() => _foundLobbies.Count;

        public override bool TryRequestDailyKills(out long kills)
        {
            kills = 0L;
            if (Steam.waitingForGlobalStats)
                return false;
            kills = (long)Steam.GetDailyGlobalStat(nameof(kills));
            return true;
        }

        public override void ApplyTS2LobbyFilters()
        {
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.value is int)
                {
                    if (matchSetting.filtered)
                        Steam.AddLobbyNumericalFilter(matchSetting.id, (int)matchSetting.value, (SteamLobbyComparison)matchSetting.filterMode);
                    else if (!matchSetting.filtered)
                        Steam.AddLobbyNearFilter(matchSetting.id, (int)matchSetting.defaultValue);
                }
                if (matchSetting.value is bool)
                {
                    if (matchSetting.filtered)
                        Steam.AddLobbyNumericalFilter(matchSetting.id, (bool)matchSetting.value ? 1 : 0, (SteamLobbyComparison)matchSetting.filterMode);
                    else if (!matchSetting.filtered)
                        Steam.AddLobbyNearFilter(matchSetting.id, (bool)matchSetting.defaultValue ? 1 : 0);
                }
            }
            foreach (MatchSetting onlineSetting in TeamSelect2.onlineSettings)
            {
                if (onlineSetting.value is int)
                {
                    if (onlineSetting.filtered)
                        Steam.AddLobbyNumericalFilter(onlineSetting.id, (int)onlineSetting.value, (SteamLobbyComparison)onlineSetting.filterMode);
                    else if (!onlineSetting.filtered)
                        Steam.AddLobbyNearFilter(onlineSetting.id, (int)onlineSetting.defaultValue);
                }
                if (onlineSetting.value is bool)
                {
                    if (onlineSetting.id == "modifiers")
                    {
                        if (onlineSetting.filtered)
                            Steam.AddLobbyStringFilter(onlineSetting.id, (bool)onlineSetting.value ? "true" : "false", SteamLobbyComparison.Equal);
                    }
                    else if (onlineSetting.id == "customlevelsenabled")
                    {
                        if (onlineSetting.filtered)
                        {
                            if ((bool)onlineSetting.value)
                                Steam.AddLobbyNumericalFilter(onlineSetting.id, 0, SteamLobbyComparison.GreaterThan);
                            else
                                Steam.AddLobbyNumericalFilter(onlineSetting.id, 0, SteamLobbyComparison.Equal);
                        }
                    }
                    else if (onlineSetting.filtered)
                        Steam.AddLobbyNumericalFilter(onlineSetting.id, (bool)onlineSetting.value ? 1 : 0, (SteamLobbyComparison)onlineSetting.filterMode);
                    else if (!onlineSetting.filtered)
                        Steam.AddLobbyNearFilter(onlineSetting.id, (bool)onlineSetting.defaultValue ? 1 : 0);
                }
            }
        }

        public override void AddLobbyNumericalFilter(string key, int value, LobbyFilterComparison op) => Steam.AddLobbyNumericalFilter(key, value, (SteamLobbyComparison)op);

        public override void RequestGlobalStats() => Steam.RequestGlobalStats();

        public override Lobby GetSearchLobbyAtIndex(int i) => Steam.GetSearchLobbyAtIndex(i);
    }
}
