namespace DuckGame
{
    public class SendToLevel : Level
    {
        private readonly Level _nextLevel;

        public SendToLevel(Level nextLevel)
        {
            _nextLevel = nextLevel;
        }

        public override void Initialize()
        {
            base.Initialize();
            current = _nextLevel;
        }
    }
}