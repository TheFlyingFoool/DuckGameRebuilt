using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModConfigurationException : Exception
    {
        public ModConfigurationException()
        {
        }

        public ModConfigurationException(string message)
          : base(message)
        {
        }

        public ModConfigurationException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModConfigurationException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
