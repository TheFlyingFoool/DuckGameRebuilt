using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModTypeMissingException : Exception
    {
        public ModTypeMissingException()
        {
        }

        public ModTypeMissingException(string message)
          : base(message)
        {
        }

        public ModTypeMissingException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModTypeMissingException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
