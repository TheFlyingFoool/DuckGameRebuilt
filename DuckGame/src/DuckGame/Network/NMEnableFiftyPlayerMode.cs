namespace DuckGame
{
    [ClientOnly]
    public class NMEnableFiftyPlayerMode : NMEvent
    {

        public NMEnableFiftyPlayerMode()
        {
        }

        public override void Activate()
        {
            if (!DuckNetwork.FiftyPlayerMode) DuckNetwork.FiftyPlayerMode = true;
            base.Activate();
        }
    }
}
