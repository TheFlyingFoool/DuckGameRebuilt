// Decompiled with JetBrains decompiler
// Type: DuckGame.ProfileNetData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class ProfileNetData
    {
        public Dictionary<NetworkConnection, NetIndex16> syncIndex = new Dictionary<NetworkConnection, NetIndex16>();
        private Dictionary<int, ProfileNetData.NetDataPair> _elements = new Dictionary<int, ProfileNetData.NetDataPair>();
        private bool _settingFiltered;

        public NetIndex16 GetAndIncrementSyncIndex(NetworkConnection pConnection)
        {
            if (!this.syncIndex.ContainsKey(pConnection))
                this.syncIndex[pConnection] = (NetIndex16)0;
            this.syncIndex[pConnection]++;
            return this.syncIndex[pConnection];
        }

        public NetIndex16 GetSyncIndex(NetworkConnection pConnection) => !this.syncIndex.ContainsKey(pConnection) ? (NetIndex16)0 : this.syncIndex[pConnection];

        /// <summary>This is just for iterating, Don't go modifying it.</summary>
        /// <returns></returns>
        public Dictionary<int, ProfileNetData.NetDataPair> GetElementList() => this._elements;

        public bool IsDirty(NetworkConnection pConnection)
        {
            foreach (KeyValuePair<int, ProfileNetData.NetDataPair> element in this._elements)
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
        public T Get<T>(string pKey) => this.Get<T>(pKey, default(T));

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
            return this._elements.TryGetValue(hashCode, out netDataPair) && netDataPair.data is T ? (T)netDataPair.data : pDefault;
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
            int hashCode = pKey.GetHashCode();
            NetDataPair netDataPair;
            if (!this._elements.TryGetValue(hashCode, out netDataPair))
            {
                this._elements[hashCode] = netDataPair = new ProfileNetData.NetDataPair();
                netDataPair.MakeDirty();
                if (this._settingFiltered)
                    netDataPair.filtered = true;
            }
            if (netDataPair.id != null && netDataPair.id != pKey)
                throw new Exception("Profile.netData.Set<" + typeof(T).Name + ">(" + pKey + ") error: GetHashCode for (" + pKey + ") is identical to GetHashCode for (" + netDataPair.id + "), a value already set in Profile.netData! Please use a more unique key name.");
            if (object.Equals(netDataPair.data, pValue) && netDataPair.activeControllingConnection == DuckNetwork.localConnection)
                return;
            netDataPair.data = pValue;
            netDataPair.id = pKey;
            netDataPair.MakeDirty();
        }

        public void SetFiltered<T>(string pKey, T pValue)
        {
            this._settingFiltered = true;
            this.Set<T>(pKey, pValue);
            this._settingFiltered = false;
        }

        public void MakeDirty(int pHash, NetworkConnection pConnection, NetIndex16 pSyncIndex)
        {
            if (pHash == int.MaxValue)
            {
                foreach (KeyValuePair<int, ProfileNetData.NetDataPair> element in this._elements)
                    element.Value.Clean(pConnection);
            }
            else
            {
                NetDataPair netDataPair;
                if (!this._elements.TryGetValue(pHash, out netDataPair) || (int)netDataPair.GetLastSyncIndex(pConnection) > (int)pSyncIndex)
                    return;
                netDataPair.SetConnectionDirty(pConnection, true);
            }
        }

        public void Clean(NetworkConnection pConnection)
        {
            foreach (KeyValuePair<int, ProfileNetData.NetDataPair> element in this._elements)
                element.Value.SetConnectionDirty(pConnection, false);
        }

        internal void Set(
          int pHash,
          object pValue,
          NetIndex16 pSyncIndex,
          NetworkConnection pConnection)
        {
            NetDataPair netDataPair;
            if (!this._elements.TryGetValue(pHash, out netDataPair))
                this._elements[pHash] = netDataPair = new ProfileNetData.NetDataPair();
            if (netDataPair.lastReceivedIndex > pSyncIndex && netDataPair.activeControllingConnection == pConnection)
                return;
            netDataPair.lastReceivedIndex = pSyncIndex;
            netDataPair.activeControllingConnection = pConnection;
            netDataPair.data = pValue;
        }

        public BitBuffer Serialize(NetworkConnection pConnection, HashSet<int> pOutputHashlist)
        {
            BitBuffer bitBuffer = new BitBuffer();
            NetIndex16 incrementSyncIndex = this.GetAndIncrementSyncIndex(pConnection);
            bitBuffer.Write((object)incrementSyncIndex);
            foreach (KeyValuePair<int, ProfileNetData.NetDataPair> element in this._elements)
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
                    this.Set(pHash, pValue, pSyncIndex, pConnection);
                else
                    this.MakeDirty(pHash, pConnection, (NetIndex16)0);
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

            public NetIndex16 GetLastSyncIndex(NetworkConnection pConnection) => !this.lastSyncIndex.ContainsKey(pConnection) ? (NetIndex16)0 : this.lastSyncIndex[pConnection];

            public void SetConnectionDirty(NetworkConnection pConnection, bool pValue) => this.dirtyConnections[pConnection] = pValue;

            public bool IsDirty(NetworkConnection pConnection)
            {
                if (this.filtered && pConnection.profile != null && pConnection.profile.muteChat)
                    return false;
                bool flag = true;
                if (this.dirtyConnections.ContainsKey(pConnection))
                    flag = this.dirtyConnections[pConnection];
                return flag;
            }

            public void MakeDirty()
            {
                foreach (NetworkConnection connection in Network.connections)
                    this.dirtyConnections[connection] = true;
            }

            public void Clean(NetworkConnection pConnection)
            {
                this.dirtyConnections[pConnection] = true;
                this.lastSyncIndex[pConnection] = (NetIndex16)0;
            }
        }
    }
}
