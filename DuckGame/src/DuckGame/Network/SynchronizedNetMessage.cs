using System;

namespace DuckGame
{
    public class SynchronizedNetMessage : ConditionalMessage
    {
        public int syncWaitFrames;
        private int numWaitFrames = -1;

        public override bool Update()
        {
            if (numWaitFrames == -1 && !Network.isServer)
                numWaitFrames = syncWaitFrames - (int)Math.Min(Math.Min((float)(connection.manager.ping / 2 * 60), 30f), syncWaitFrames);
            --numWaitFrames;
            return numWaitFrames <= 0;
        }

        protected override void OnSerialize()
        {
            syncWaitFrames = numWaitFrames = Math.Min((int)(Network.highestPing / 2 * 60) + 2, 30);
            base.OnSerialize();
        }
    }
}
