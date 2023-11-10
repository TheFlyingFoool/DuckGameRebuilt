using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModDependencyNotFoundException : Exception
    {
        public ModDependencyNotFoundException()
        {
        }

        public ModDependencyNotFoundException(string message)
          : base(message)
        {
        }

        public ModDependencyNotFoundException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModDependencyNotFoundException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        public ModDependencyNotFoundException(string mod, string missing)
          : base("Mod " + mod + " is missing hard dependency " + missing)
        {
        }
    }
}
