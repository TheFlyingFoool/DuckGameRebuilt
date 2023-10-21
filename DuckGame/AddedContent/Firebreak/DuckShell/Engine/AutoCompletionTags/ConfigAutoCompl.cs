using AddedContent.Firebreak;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    public class ConfigAutoCompl : AutoCompl
    {
        public override IList<string> Get(string word)
        {
            string[] suggestions = new string[Marker.AutoConfigAttribute.All.Count + 3];

            suggestions[0] = "%SAVE";
            suggestions[1] = "%LOAD";
            suggestions[2] = "%LIST";
            
            for (int i = 3; i < suggestions.Length; i++)
            {
                suggestions[i] = Marker.AutoConfigAttribute.All[i - 3].Member.Name;
            }

            return suggestions;
        }
    }
}