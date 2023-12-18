namespace DuckGame
{
    [FixedNetworkID(30000)]
    public class NMResetGameSettings : NMDuckNetworkEvent
    {
        public override void Activate()
        {
            DuckNetwork.ResetScores();
            base.Activate();
        }
    }
}
