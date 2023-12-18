namespace DuckGame
{
    public class NoteworthyEvent
    {
        public static string GoodKillDeathRatio = nameof(GoodKillDeathRatio);
        public static string BadKillDeathRatio = nameof(BadKillDeathRatio);
        public static string ManyFallDeaths = nameof(ManyFallDeaths);
        public string eventTag;
        public Profile who;
        public float quality;

        public NoteworthyEvent(string tag, Profile owner, float q)
        {
            eventTag = tag;
            who = owner;
            quality = q;
        }
    }
}
