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
