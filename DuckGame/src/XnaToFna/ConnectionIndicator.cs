using System.Collections.Generic;

namespace DuckGame
{
    public class ConnectionIndicator // Add for duckutils? from old old DG?
    {
        private static Dictionary<NetworkConnection, ConnectionIndicatorElement> _connections = new Dictionary<NetworkConnection, ConnectionIndicatorElement>();
    }
    public class ConnectionIndicatorElement  // Add for duckutils? from old old DG?
    {
        public Duck duck;
    }

}
