namespace DuckGame
{
    public class EditorTestLevel : Thing
    {
        private Editor _editor;
        protected bool _quitTesting;

        public Editor editor => _editor;

        public EditorTestLevel(Editor editor)
          : base()
        {
            _editor = editor;
        }

        public override void Update()
        {
            if (!_quitTesting || Level.current is ChallengeLevel)
                return;
            Level.current = _editor;
        }
    }
}
