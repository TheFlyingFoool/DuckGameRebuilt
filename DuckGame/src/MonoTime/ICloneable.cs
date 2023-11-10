using System;

namespace DuckGame
{
    /// <summary>
    /// Represents an object that can be cloned into a specific type.
    /// </summary>
    /// <typeparam name="T">The type it can be cloned into.</typeparam>
    public interface ICloneable<T> : ICloneable
    {
        /// <summary>Clones this instance.</summary>
        /// <returns>The new instance.</returns>
        T Clone();
    }
}
