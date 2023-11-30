namespace DuckGame
{
    public enum DuckNetStatus
    {
        Disconnected,
        EstablishingCommunicationWithServer,
        ConnectingToServer,
        ConnectingToClients,
        Connected,
        Disconnecting,
        Failure,
    }
}
