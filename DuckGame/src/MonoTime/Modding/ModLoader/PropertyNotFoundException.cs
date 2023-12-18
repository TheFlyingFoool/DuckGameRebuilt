using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    /// <summary>
    /// This exception is thrown when a property was not found in a PropertyBag.
    /// </summary>
    [Serializable]
    public class PropertyNotFoundException : Exception
    {
        /// <summary>Initialize a new instance of this exception.</summary>
        public PropertyNotFoundException()
        {
        }

        /// <summary>
        /// Initialize a new instance of this exception with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public PropertyNotFoundException(string message)
          : base(message)
        {
        }

        /// <summary>
        /// Initialize a new instance of this exception with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public PropertyNotFoundException(string message, Exception inner)
          : base(message, inner)
        {
        }

        /// <summary>
        /// Initialize a new instance of this exception for deserialization.
        /// </summary>
        /// <param name="info">Serialized info</param>
        /// <param name="context">Serialization context</param>
        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
