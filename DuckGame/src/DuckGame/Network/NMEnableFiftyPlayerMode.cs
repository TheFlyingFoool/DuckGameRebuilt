namespace DuckGame
{
    [ClientOnly]
    public class NMEnableFiftyPlayerMode : NMEvent
    {
        public NMEnableFiftyPlayerMode()
        {
        }
        public NMEnableFiftyPlayerMode(int num)
        {
        }
        public int extraplayercount = 50;
        public override void Activate()
        {
            DG.ExtraPlayerCount = extraplayercount;
            if (!DG.FiftyPlayerMode) 
                DG.FiftyPlayerMode = true;
            base.Activate();
        }
    }
}
