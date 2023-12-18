using System.Collections.Generic;

namespace DuckGame
{
    public class ChallengeGroup
    {
        public List<string> challenges = new List<string>();
        public List<string> required = new List<string>();
        public int trophiesRequired;
        public string name;

        public string GetNameForDisplay() => name.ToUpperInvariant();
    }
}
