// Decompiled with JetBrains decompiler
// Type: DuckGame.NGeneratorRule
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
