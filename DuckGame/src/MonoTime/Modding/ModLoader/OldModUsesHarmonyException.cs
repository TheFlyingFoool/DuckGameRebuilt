using System;

namespace DuckGame
{
    public class OldModUsesHarmonyException : Exception
    {
        public OldModUsesHarmonyException(string pMessage)
          : base(pMessage)
        {
        }
    }
}
