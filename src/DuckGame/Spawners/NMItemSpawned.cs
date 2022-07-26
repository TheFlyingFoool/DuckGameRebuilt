// Decompiled with JetBrains decompiler
// Type: DuckGame.NMItemSpawned
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMItemSpawned : NMEvent
    {
        private ItemSpawner _spawner;

        public NMItemSpawned()
        {
        }

        public NMItemSpawned(ItemSpawner pSpawner) => this._spawner = pSpawner;

        public override void Activate()
        {
            if (this._spawner == null)
                return;
            this._spawner._spawnWait = 0.0f;
        }
    }
}
