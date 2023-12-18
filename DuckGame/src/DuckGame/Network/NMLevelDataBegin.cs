namespace DuckGame
{
    public class NMLevelDataBegin : NMConditionalEvent
    {
        public new byte levelIndex;

        public NMLevelDataBegin()
        {
        }

        public NMLevelDataBegin(byte pLevelIndex) => levelIndex = pLevelIndex;

        public override bool Update() => Level.current.networkIndex == levelIndex && Level.current.initializeFunctionHasBeenRun;

        public override void Activate()
        {
        }
    }
}
