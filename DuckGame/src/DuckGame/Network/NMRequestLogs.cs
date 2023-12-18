namespace DuckGame
{
    public class NMRequestLogs : NMEvent
    {
        public override void Activate()
        {
            if (connection.profile == null)
                return;
            DevConsole.Log("@error@" + connection.ToString() + " is requesting your Netlog!", Color.Red);
            DevConsole.Log("@error@Only accept this request if it's from someone you trust!", Color.Red);
            DevConsole.Log("@error@type |WHITE|accept " + connection.profile.networkIndex.ToString() + " |RED|to begin transfer.", Color.Red);
            DevConsole.core.transferRequestsPending.Add(connection);
        }
    }
}
