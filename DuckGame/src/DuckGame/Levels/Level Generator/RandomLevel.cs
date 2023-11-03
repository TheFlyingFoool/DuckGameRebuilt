namespace DuckGame
{
    public class RandomLevel : DeathmatchLevel
    {
        public static int currentComplexityDepth;
        private RandomLevelNode _level;

        public RandomLevel()
          : base("RANDOM")
        {
            _level = LevelGenerator.MakeLevel();
        }

        public override void Initialize()
        {
            _level.LoadParts(0f, 0f, this);
            OfficeBackground officeBackground = new OfficeBackground(0f, 0f)
            {
                visible = false
            };
            Add(officeBackground);
            base.Initialize();
        }

        public override void Update() => base.Update();
    }
}
