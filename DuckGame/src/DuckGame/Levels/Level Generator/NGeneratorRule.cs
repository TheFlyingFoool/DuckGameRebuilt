using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NGeneratorRule
    {
        private Func<bool> _check;

        public NGeneratorRule(Func<bool> pCheck) => _check = pCheck;

        public static int Count(HashSet<Thing> pThings, Func<Thing, bool> pCheck)
        {
            int num = 0;
            foreach (Thing pThing in pThings)
            {
                if (pCheck(pThing))
                    ++num;
            }
            return num;
        }

        public virtual bool Check() => _check();
    }
}
