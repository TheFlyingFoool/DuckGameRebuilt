namespace DuckGame
{
    public class NMMatchSettingsChanged : NMEvent
    {
        public override void Activate()
        {
            DuckNetwork.OpenMatchSettingsInfo();
            GameMode.numMatchesPlayed = 0;
            base.Activate();
        }
    }
}
