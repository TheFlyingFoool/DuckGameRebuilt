using System.Collections.Generic;

namespace DuckGame
{
    public class Send
    {
        public static void Resend(NetMessage msg) => Network.activeNetwork.QueueMessage(msg, msg.connection);

        public static void Message(NetMessage msg)
        {
            msg.priority = NetMessagePriority.ReliableOrdered;
            Network.activeNetwork.QueueMessage(msg);
        }

        public static void Message(NetMessage msg, NetworkConnection who)
        {
            if (who == DuckNetwork.localConnection)
                return;
            msg.priority = NetMessagePriority.ReliableOrdered;
            Network.activeNetwork.QueueMessage(msg, who);
        }

        public static void Message(NetMessage msg, NetMessagePriority priority, NetworkConnection who = null)
        {
            if (who == DuckNetwork.localConnection)
                return;
            Network.activeNetwork.QueueMessage(msg, priority, who);
        }

        public static void Message(
          NetMessage msg,
          NetMessagePriority priority,
          List<NetworkConnection> pConnections)
        {
            Network.activeNetwork.QueueMessage(msg, priority, pConnections);
        }

        public static void MessageToAllBut(
          NetMessage msg,
          NetMessagePriority priority,
          NetworkConnection who)
        {
            if (who == DuckNetwork.localConnection)
                return;
            Network.activeNetwork.QueueMessageForAllBut(msg, priority, who);
        }

        /// <summary>
        /// Use this for emergency disconnect broadcasts and the like. It's mostly meant for sending one more message
        /// before disconnects or crashes.
        /// </summary>
        /// <param name="msg">Oh, it's a NetMessage!</param>
        public static void ImmediateUnreliableBroadcast(NetMessage msg)
        {
            msg.priority = NetMessagePriority.UnreliableUnordered;
            Network.activeNetwork.ImmediateUnreliableBroadcast(msg);
        }

        public static void ImmediateUnreliableMessage(NetMessage msg, NetworkConnection pConnection)
        {
            msg.priority = NetMessagePriority.UnreliableUnordered;
            Network.activeNetwork.ImmediateUnreliableMessage(msg, pConnection);
        }
    }
}
