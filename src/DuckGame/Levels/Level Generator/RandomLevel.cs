// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RandomLevel : DeathmatchLevel
    {
        public static int currentComplexityDepth;
        private RandomLevelNode _level;

        public RandomLevel()
          : base("RANDOM")
        {
            this._level = LevelGenerator.MakeLevel();
        }

        public override void Initialize()
        {
            this._level.LoadParts(0.0f, 0.0f, this);
            OfficeBackground officeBackground = new OfficeBackground(0.0f, 0.0f)
            {
                visible = false
            };
            Level.Add(officeBackground);
            base.Initialize();
        }

        public override void Update() => base.Update();
    }
}
