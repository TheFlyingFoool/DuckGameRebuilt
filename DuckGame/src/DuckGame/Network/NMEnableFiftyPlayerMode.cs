﻿namespace DuckGame
{
    [ClientOnly]
    public class NMEnableFiftyPlayerMode : NMEvent
    {

        public NMEnableFiftyPlayerMode()
        {
        }

        public override void Activate()
        {
            if (!DG.FiftyPlayerMode) DG.FiftyPlayerMode = true;
            base.Activate();
        }
    }
}