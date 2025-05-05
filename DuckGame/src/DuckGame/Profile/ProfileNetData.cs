using AddedContent.Firebreak;
using RectpackSharp;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class ProfileNetData
    {
        public Dictionary<NetworkConnection, NetIndex16> syncIndex = new Dictionary<NetworkConnection, NetIndex16>();
        private Dictionary<int, NetDataPair> _elements = new Dictionary<int, NetDataPair>();
        private bool _settingFiltered;

        public NetIndex16 GetAndIncrementSyncIndex(NetworkConnection pConnection)
        {
            if (!syncIndex.ContainsKey(pConnection))
                syncIndex[pConnection] = (NetIndex16)0;
            syncIndex[pConnection]++;
            return syncIndex[pConnection];
        }

        public NetIndex16 GetSyncIndex(NetworkConnection pConnection) => !syncIndex.ContainsKey(pConnection) ? (NetIndex16)0 : syncIndex[pConnection];

        /// <summary>This is just for iterating, Don't go modifying it.</summary>
        /// <returns></returns>
        public Dictionary<int, NetDataPair> GetElementList() => _elements;

        public bool IsDirty(NetworkConnection pConnection)
        {
            foreach (KeyValuePair<int, NetDataPair> element in _elements)
            {
                if (element.Value.IsDirty(pConnection))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a property value based on a string. These values are synchronized over the network!
        /// </summary>
        /// <typeparam name="T">The type of the value you're getting.</typeparam>
        /// <param name="pKey">The name of the property you're getting.</param>
        /// <returns></returns>
        public T Get<T>(string pKey) => Get(pKey, default(T));

        /// <summary>
        /// Returns a property value based on a string. These values are synchronized over the network!
        /// </summary>
        /// <typeparam name="T">The type of the value you're getting.</typeparam>
        /// <param name="pKey">The name of the property you're getting.</param>
        /// <param name="pDefault">The value to return if no key is found.</param>
        /// <returns></returns>
        public T Get<T>(string pKey, T pDefault)
        {
            int hashCode = pKey.GetHashCode();
            NetDataPair netDataPair;
            if (AreWeSureThisPersonIsRunningRebuiltLikeOneHundredPercentSure() && MappedNetdata.Contains(pKey))
            {
                hashCode = MappedNetdata[pKey];
            }

            return _elements.TryGetValue(hashCode, out netDataPair) && netDataPair.data is T ? (T)netDataPair.data : pDefault;
        }
        public bool AreWeSureThisPersonIsRunningRebuiltLikeOneHundredPercentSure()
        {
            NetDataPair netDataPair;
            return _elements.TryGetValue(100000000, out netDataPair) && netDataPair.data is bool ? (bool)netDataPair.data : false;
        }
        public static Map<int, string> MappedNetdata = new Map<int, string>();
        [Marker.PostInitialize]
        public static void Initialize()
        {
            //this is a dumbass solution but since DG uses GetHashCode() to transmit the netdata then between linux windows and other builds
            //the hashes wont match resulting in stuff just NOT SYNCING!!!!!!! 

            MappedNetdata.Add("REBUILT", 100000000);
            MappedNetdata.Add("rVer", 100000001);

            MappedNetdata.Add("gamePaused", 100000002);
            MappedNetdata.Add("gameInFocus", 100000003);
            MappedNetdata.Add("chatting", 100000004);
            MappedNetdata.Add("consoleOpen", 100000005);

            MappedNetdata.Add("quackPitch", 100000006);
            MappedNetdata.Add("spectatorFlip", 100000007);
            MappedNetdata.Add("spectatorBeverage", 100000008);
            MappedNetdata.Add("spectatorPersona", 100000009);
            MappedNetdata.Add("spectatorTongue", 100000010);
            MappedNetdata.Add("spectatorBob", 100000011);
            MappedNetdata.Add("spectatorTilt", 100000012);
            MappedNetdata.Add("quack", 100000013);

            MappedNetdata.Add("linux", 100000014);
            MappedNetdata.Add("midgameJoining", 100000015);
        }
        /// <summary>
        /// Set a property to a value. This property will be synchronized over the network
        /// and accessible from this profile on other computers!
        /// </summary>
        /// <typeparam name="T">The type of the value you're setting</typeparam>
        /// <param name="pKey">A unique name for the property. This runs through string.GetHashCode, so try to make it pretty unique.</param>
        /// <param name="pValue">The value!</param>
        public void Set<T>(string pKey, T pValue)
        {
            if (MappedNetdata.Contains(pKey)) DGRSet<T>(pKey, pValue);
            int hashCode = pKey.GetHashCode();
            NetDataPair netDataPair;
            if (!_elements.TryGetValue(hashCode, out netDataPair))
            {
                _elements[hashCode] = netDataPair = new NetDataPair();
                netDataPair.MakeDirty();
                if (_settingFiltered)
                    netDataPair.filtered = true;
            }
            if (netDataPair.id != null && netDataPair.id != pKey)
                throw new Exception("Profile.netData.Set<" + typeof(T).Name + ">(" + pKey + ") error: GetHashCode for (" + pKey + ") is identical to GetHashCode for (" + netDataPair.id + "), a value already set in Profile.netData! Please use a more unique key name.");
            if (Equals(netDataPair.data, pValue) && netDataPair.activeControllingConnection == DuckNetwork.localConnection)
                return;
            netDataPair.data = pValue;
            netDataPair.id = pKey;
            netDataPair.MakeDirty();
        }
        public void DGRSet<T>(string pKey, T pValue)
        {
            int hashCode = MappedNetdata[pKey];
            NetDataPair netDataPair;
            if (!_elements.TryGetValue(hashCode, out netDataPair))
            {
                _elements[hashCode] = netDataPair = new NetDataPair();
                netDataPair.MakeDirty();
                if (_settingFiltered)
                    netDataPair.filtered = true;
            }
            if (netDataPair.id != null && netDataPair.id != pKey)
                throw new Exception("Profile.netData.Set<" + typeof(T).Name + ">(" + pKey + ") error: GetHashCode for (" + pKey + ") is identical to GetHashCode for (" + netDataPair.id + "), a value already set in Profile.netData! Please use a more unique key name.");
            if (Equals(netDataPair.data, pValue) && netDataPair.activeControllingConnection == DuckNetwork.localConnection)
                return;
            netDataPair.data = pValue;
            netDataPair.id = pKey;
            netDataPair.MakeDirty();
        }

        public void SetFiltered<T>(string pKey, T pValue)
        {
            _settingFiltered = true;
            Set(pKey, pValue);
            _settingFiltered = false;
        }

        public void MakeDirty(int pHash, NetworkConnection pConnection, NetIndex16 pSyncIndex)
        {
            if (pHash == int.MaxValue)
            {
                foreach (KeyValuePair<int, NetDataPair> element in _elements)
                    element.Value.Clean(pConnection);
            }
            else
            {
                NetDataPair netDataPair;
                if (!_elements.TryGetValue(pHash, out netDataPair) || (int)netDataPair.GetLastSyncIndex(pConnection) > (int)pSyncIndex)
                    return;
                netDataPair.SetConnectionDirty(pConnection, true);
            }
        }

        public void Clean(NetworkConnection pConnection)
        {
            foreach (KeyValuePair<int, NetDataPair> element in _elements)
                element.Value.SetConnectionDirty(pConnection, false);
        }

        internal void Set(
          int pHash,
          object pValue,
          NetIndex16 pSyncIndex,
          NetworkConnection pConnection)
        {
            NetDataPair netDataPair;
            if (!_elements.TryGetValue(pHash, out netDataPair))
                _elements[pHash] = netDataPair = new NetDataPair();
            if (netDataPair.lastReceivedIndex > pSyncIndex && netDataPair.activeControllingConnection == pConnection)
                return;
            netDataPair.lastReceivedIndex = pSyncIndex;
            netDataPair.activeControllingConnection = pConnection;
            netDataPair.data = pValue;
        }

        public BitBuffer Serialize(NetworkConnection pConnection, HashSet<int> pOutputHashlist)
        {
            BitBuffer bitBuffer = new BitBuffer();
            NetIndex16 incrementSyncIndex = GetAndIncrementSyncIndex(pConnection);
            bitBuffer.Write((object)incrementSyncIndex);
            foreach (KeyValuePair<int, NetDataPair> element in _elements)
            {
                if (element.Value.IsDirty(pConnection))
                {
                    bitBuffer.Write(element.Key);
                    bitBuffer.WriteObject(element.Value.data);
                    element.Value.SetConnectionDirty(pConnection, false);
                    element.Value.lastSyncIndex[pConnection] = incrementSyncIndex;
                    pOutputHashlist.Add(element.Key);
                    element.Value.activeControllingConnection = DuckNetwork.localConnection;
                }
            }
            return bitBuffer;
        }

        public void Deserialize(BitBuffer pBuffer, NetworkConnection pConnection, bool pMakingDirty)
        {
            NetIndex16 pSyncIndex = pBuffer.ReadNetIndex16();
            while (pBuffer.positionInBits != pBuffer.lengthInBits)
            {
                int pHash = pBuffer.ReadInt();
                Type pTypeRead;
                object pValue = pBuffer.ReadObject(out pTypeRead);
                if (!pMakingDirty)
                    Set(pHash, pValue, pSyncIndex, pConnection);
                else
                    MakeDirty(pHash, pConnection, (NetIndex16)0);
            }
        }

        public class NetDataPair
        {
            public NetIndex16 lastReceivedIndex = (NetIndex16)1;
            public NetworkConnection activeControllingConnection;
            public Dictionary<NetworkConnection, NetIndex16> lastSyncIndex = new Dictionary<NetworkConnection, NetIndex16>();
            public bool filtered;
            public string id;
            public object data;
            public Dictionary<NetworkConnection, bool> dirtyConnections = new Dictionary<NetworkConnection, bool>();

            public NetIndex16 GetLastSyncIndex(NetworkConnection pConnection) => !lastSyncIndex.ContainsKey(pConnection) ? (NetIndex16)0 : lastSyncIndex[pConnection];

            public void SetConnectionDirty(NetworkConnection pConnection, bool pValue) => dirtyConnections[pConnection] = pValue;

            public bool IsDirty(NetworkConnection pConnection)
            {
                if (filtered && pConnection.profile != null && pConnection.profile.muteChat)
                    return false;
                bool flag = true;
                if (dirtyConnections.ContainsKey(pConnection))
                    flag = dirtyConnections[pConnection];
                return flag;
            }

            public void MakeDirty()
            {
                for (int i = 0; i < Network.connections.Count; i++)
                {
                    dirtyConnections[Network.connections[i]] = true;
                }
            }

            public void Clean(NetworkConnection pConnection)
            {
                dirtyConnections[pConnection] = true;
                lastSyncIndex[pConnection] = (NetIndex16)0;
            }
        }
    }
}
