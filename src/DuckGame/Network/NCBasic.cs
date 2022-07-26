// Decompiled with JetBrains decompiler
// Type: DuckGame.NCBasic
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private byte[] _receiveBuffer = new byte[4096];
        private string _serverIdentifier = "";
        private List<NCBasicConnection> _basicConnections = new List<NCBasicConnection>();
        public int bytesThisFrame;
        public int headerBytes;
        public int ghostBytes;
        public int ackBytes;
        private const long kDuckGameLANHeader = 5892070176735;
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
            this._socket.Send(numArray, length + 8, connection as IPEndPoint);
            this.bytesThisFrame += length + 8;
            return (NCError)null;
        }

        protected override object GetConnectionObject(string identifier) => (object)this.MakeConnection(NCBasic.CreateIPEndPoint(identifier)).connection;

        private void BroadcastServerHeader()
        {
            if (this._socket == null)
                return;
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(5892070176735L);
            bitBuffer.Write(DG.versionMajor);
            bitBuffer.Write(DG.versionHigh);
            bitBuffer.Write(DG.versionLow);
            bitBuffer.Write((byte)DuckNetwork.profiles.Where<Profile>((Func<Profile, bool>)(x => x.connection != null)).Count<Profile>());
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
            this._socket.Send(bitBuffer.buffer, bitBuffer.lengthInBytes, "255.255.255.255", this._port);
        }

        public override NCError OnHostServer(
          string identifier,
          int port,
          NetworkLobbyType lobbyType,
          int maxConnections)
        {
            if (this._socket != null)
                return new NCError("server is already started...", NCErrorType.Error);
            this._basicConnections.Clear();
            this._serverIdentifier = identifier;
            this._socket = new UdpClient();
            this._socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this._socket.Client.Bind((EndPoint)new IPEndPoint(IPAddress.Any, port));
            this._socket.AllowNatTraversal(true);
            this.localEndPoint = !NetworkDebugger.enabled ? new IPEndPoint(IPAddress.Parse("127.0.0.1"), this._port) : new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1330 + NetworkDebugger.currentIndex);
            this._port = port;
            this._socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.StartServerThread();
            return new NCError("server started on port " + port.ToString() + ".", NCErrorType.Success);
        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
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
            if (!int.TryParse(strArray[strArray.Length - 1], NumberStyles.None, (IFormatProvider)NumberFormatInfo.CurrentInfo, out result))
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
                    if (this._socket != null)
                        return new NCError("client is already started...", NCErrorType.Error);
                    IPEndPoint endPoint = !(ip == "netdebug") ? NCBasic.CreateIPEndPoint(ip) : new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1330 + NetworkDebugger.CurrentServerIndex());
                    this._serverIdentifier = identifier;
                    this._basicConnections.Clear();
                    this._socket = new UdpClient();
                    this._socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    this._port = port;
                    int port1 = 1336;
                    if (NetworkDebugger.enabled)
                    {
                        int port2 = 1330 + NetworkDebugger.currentIndex;
                        this.localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port2);
                    }
                    else
                        this.localEndPoint = new IPEndPoint(IPAddress.Any, port1);
                    this._socket.Client.Bind((EndPoint)this.localEndPoint);
                    this._socket.AllowNatTraversal(true);
                    this.MakeConnection(endPoint, true);
                    this.StartClientThread();
                    return (NCError)null;
            }
        }

        public NCBasicConnection MakeConnection(IPEndPoint endPoint, bool isHost = false)
        {
            lock (this._basicConnections)
            {
                NCBasicConnection ncBasicConnection1 = this._basicConnections.FirstOrDefault<NCBasicConnection>((Func<NCBasicConnection, bool>)(x => x.connection.ToString() == endPoint.ToString()));
                if (ncBasicConnection1 != null)
                    return ncBasicConnection1;
                NCBasicConnection ncBasicConnection2 = new NCBasicConnection()
                {
                    connection = endPoint,
                    status = NCBasicStatus.TryingToConnect
                };
                ncBasicConnection2.isHost = isHost;
                this._basicConnections.Add(ncBasicConnection2);
                this._pendingMessages.Enqueue(new NCError("client connecting to " + endPoint.ToString() + ".", NCErrorType.Message));
                return ncBasicConnection2;
            }
        }

        public override string GetConnectionIdentifier(object connection) => (connection as IPEndPoint).ToString();

        public override string GetConnectionName(object connection) => (connection as IPEndPoint).ToString().ToString();

        protected override string OnGetLocalName() => NCBasic._localName;

        protected override NCError OnSpinServerThread()
        {
            if (this._socket == null)
                return NetworkDebugger.enabled ? (NCError)null : new NCError("NCBasic connection was lost.", NCErrorType.CriticalError);
            for (int index = 0; index < this._basicConnections.Count; ++index)
            {
                NCBasicConnection basicConnection = this._basicConnections[index];
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
                            this.OnSendPacket((byte[])null, 0, (object)basicConnection.connection);
                            if (basicConnection.status == NCBasicStatus.TryingToConnect)
                                basicConnection.status = NCBasicStatus.Connecting;
                        }
                    }
                }
            }
            Queue<NCBasicPacket> packets = new Queue<NCBasicPacket>();
            this.ReceivePackets(packets);
            foreach (NCBasicPacket ncBasicPacket in packets)
            {
                IPEndPoint sender = ncBasicPacket.sender;
                byte[] data = ncBasicPacket.data;
                string address = sender.ToString();
                lock (this._basicConnections)
                {
                    if (data.Length >= 8)
                    {
                        if (new BitBuffer(data).ReadLong() == 2449832521355936907L)
                        {
                            NCBasicConnection ncBasicConnection = this._basicConnections.FirstOrDefault<NCBasicConnection>((Func<NCBasicConnection, bool>)(x => x.connection.ToString() == address));
                            if (ncBasicConnection == null)
                            {
                                ncBasicConnection = new NCBasicConnection()
                                {
                                    connection = sender,
                                    status = NCBasicStatus.Connecting
                                };
                                this._basicConnections.Add(ncBasicConnection);
                                this._pendingMessages.Enqueue(new NCError("connection attempt from " + ncBasicConnection.connection.ToString(), NCErrorType.Message));
                            }
                            if (ncBasicConnection != null)
                            {
                                ++ncBasicConnection.packets;
                                if (ncBasicConnection.status == NCBasicStatus.Connected)
                                {
                                    if (data.Length > 8)
                                    {
                                        byte[] numArray = new byte[data.Length - 8];
                                        Array.Copy((Array)data, 8, (Array)numArray, 0, data.Length - 8);
                                        this.OnPacket(numArray, (object)ncBasicConnection.connection);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return (NCError)null;
        }

        protected virtual void ReceivePackets(Queue<NCBasicPacket> packets)
        {
            try
            {
                IPEndPoint remoteEP = (IPEndPoint)null;
                while (this._socket.Available > 0)
                {
                    byte[] numArray = this._socket.Receive(ref remoteEP);
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

        protected override NCError OnSpinClientThread() => this.OnSpinServerThread();

        public override void Update()
        {
            if (Network.isActive)
            {
                if (Network.isServer)
                {
                    this._broadcastWait += Maths.IncFrameTimer();
                    if ((double)this._broadcastWait > 0.75)
                    {
                        this._broadcastWait = 0.0f;
                        this.BroadcastServerHeader();
                    }
                }
                for (int index = 0; index < this._basicConnections.Count; ++index)
                {
                    NCBasicConnection basicConnection = this._basicConnections[index];
                    if (basicConnection.status != NCBasicStatus.Connected && basicConnection.packets > 0 && basicConnection.status != NCBasicStatus.Disconnecting)
                    {
                        this._pendingMessages.Enqueue(new NCError("connection to " + basicConnection.connection.ToString() + " succeeded!", NCErrorType.Success));
                        basicConnection.status = NCBasicStatus.Connected;
                        this.AttemptConnection((object)basicConnection.connection, basicConnection.isHost);
                        basicConnection.isHost = false;
                    }
                }
            }
            base.Update();
        }

        protected override void KillConnection()
        {
            this._basicConnections.Clear();
            if (this._socket != null)
                this._socket.Close();
            this._socket = (UdpClient)null;
            base.KillConnection();
        }

        protected override void Disconnect(NetworkConnection c)
        {
            if (c != null)
            {
                NCBasicConnection ncBasicConnection = this._basicConnections.FirstOrDefault<NCBasicConnection>((Func<NCBasicConnection, bool>)(x => x.connection.ToString() == c.identifier));
                if (ncBasicConnection != null)
                    ncBasicConnection.status = NCBasicStatus.Disconnecting;
            }
            base.Disconnect(c);
        }

        public override void Terminate() => base.Terminate();

        public override void AddLobbyStringFilter(string key, string value, LobbyFilterComparison op) => Steam.AddLobbyStringFilter(key, value, (SteamLobbyComparison)op);

        public override bool IsLobbySearchComplete()
        {
            if (this._lobbyThreadRunning)
                return false;
            if (this._threadLobbies != null)
            {
                lock (this.lobbyLock)
                {
                    this._foundLobbies = this._threadLobbies;
                    this._threadLobbies = (List<UIServerBrowser.LobbyData>)null;
                }
            }
            return true;
        }

        private void SearchForLobbyThread()
        {
            this._lobbyThreadRunning = true;
            lock (this.lobbyLock)
            {
                this._threadLobbies = new List<UIServerBrowser.LobbyData>();
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    udpClient.Client.Bind((EndPoint)new IPEndPoint(IPAddress.Any, NCBasic.lobbySearchPort));
                    udpClient.Client.ReceiveTimeout = 100;
                    IPEndPoint remoteEP = new IPEndPoint(0L, 0);
                    for (int index = 0; index < 15; ++index)
                    {
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
                                    if (this._threadLobbies.FirstOrDefault<UIServerBrowser.LobbyData>((Func<UIServerBrowser.LobbyData, bool>)(x => x.lanAddress == address)) == null)
                                    {
                                        UIServerBrowser.LobbyData lobbyData1 = new UIServerBrowser.LobbyData();
                                        lobbyData1.lanAddress = address;
                                        int pMajor = bitBuffer.ReadInt();
                                        int pHigh = bitBuffer.ReadInt();
                                        int pLow = bitBuffer.ReadInt();
                                        lobbyData1.version = DG.MakeVersionString(pMajor, pHigh, pLow);
                                        lobbyData1._userCount = (int)bitBuffer.ReadByte();
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
                                        if ((long)bitBuffer.positionInBits != (long)bitBuffer.lengthInBits && bitBuffer.ReadBool())
                                            lobbyData1.datahash = bitBuffer.ReadLong();
                                        this._threadLobbies.Add(lobbyData1);
                                        --index;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    udpClient.Close();
                }
            }
            this._lobbyThreadRunning = false;
        }

        public override void SearchForLobby()
        {
            if (this._lobbyThreadRunning)
                return;
            this._foundLobbies.Clear();
            new Thread((ThreadStart)(() => this.SearchForLobbyThread())).Start();
        }

        public override int NumLobbiesFound() => this._foundLobbies.Count;

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
