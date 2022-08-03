// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNewPing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [FixedNetworkID(30014)]
    public class NMNewPing : NMNetworkCoreMessage
    {
        private Timer _pingTimer;
        public byte index;

        public float GetTotalSeconds() => _pingTimer == null ? 1f : (float)_pingTimer.elapsed.TotalSeconds;

        public NMNewPing(byte pIndex) => index = pIndex;

        public NMNewPing()
        {
        }

        protected override void OnSerialize()
        {
            if (_pingTimer == null)
            {
                _pingTimer = new Timer();
                _pingTimer.Start();
            }
            base.OnSerialize();
        }
    }
}
