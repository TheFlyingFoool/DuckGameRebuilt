namespace DuckGame
{
    public class NMItemSpawned : NMEvent
    {
        private ItemSpawner _spawner;

        public NMItemSpawned()
        {
        }

        public NMItemSpawned(ItemSpawner pSpawner) => _spawner = pSpawner;

        public override void Activate()
        {
            if (_spawner == null)
                return;
            _spawner._spawnWait = 0f;
        }
    }
}
