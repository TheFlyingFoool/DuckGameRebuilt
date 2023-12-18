namespace DuckGame
{
    public class DataLayer
    {
        protected NCNetworkImplementation _impl;

        public DataLayer(NCNetworkImplementation pImpl) => _impl = pImpl;

        public virtual NCError SendPacket(BitBuffer sendData, NetworkConnection connection) => _impl.OnSendPacket(sendData.buffer, sendData.lengthInBytes, connection.data);

        public virtual void Update()
        {
        }

        public virtual void EndSession()
        {
        }

        public virtual void Reset()
        {
        }
    }
}
