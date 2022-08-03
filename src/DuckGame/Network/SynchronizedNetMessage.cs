// Decompiled with JetBrains decompiler
// Type: DuckGame.SynchronizedNetMessage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                numWaitFrames = syncWaitFrames - (int)Math.Min(Math.Min((float)(connection.manager.ping / 2.0 * 60.0), 30f), syncWaitFrames);
            --numWaitFrames;
            return numWaitFrames <= 0;
        }

        protected override void OnSerialize()
        {
            syncWaitFrames = numWaitFrames = Math.Min((int)(Network.highestPing / 2.0 * 60.0) + 2, 30);
            base.OnSerialize();
        }
    }
}
