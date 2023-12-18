using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    public class TeamAutoCompl : AutoCompl
    {
        public override IList<string> Get(string word)
        {
            return Teams.all.Select(x => x.name).ToList();
        }
    }
}