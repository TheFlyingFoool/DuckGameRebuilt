namespace DuckGame
{
    public class NMLogEvent : NMEvent
    {
        public string description;

        public NMLogEvent(string pDescription) => description = pDescription;

        public NMLogEvent()
        {
        }

        public override void Activate() => DevConsole.LogEvent(description, connection);
    }
}
