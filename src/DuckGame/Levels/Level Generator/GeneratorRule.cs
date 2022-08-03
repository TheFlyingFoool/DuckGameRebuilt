// Decompiled with JetBrains decompiler
// Type: DuckGame.GeneratorRule
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class GeneratorRule
    {
        public Func<RandomLevelData, bool> problem;
        public Func<RandomLevelData, bool> solution;
        public float chance = 1f;
        public bool mandatory;
        public SpecialRule special;

        public GeneratorRule(
          Func<RandomLevelData, bool> varProblem,
          Func<RandomLevelData, bool> varSolution,
          float varChance = 1f,
          bool varMandatory = false,
          SpecialRule varSpecial = SpecialRule.None)
        {
            problem = varProblem;
            solution = varSolution;
            chance = varChance;
            mandatory = varMandatory;
            special = varSpecial;
        }
    }
}
