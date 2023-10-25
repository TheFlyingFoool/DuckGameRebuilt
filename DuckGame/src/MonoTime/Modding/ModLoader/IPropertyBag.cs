using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// An interface allowing access to a key/value pair of keys mapped to
    /// values of any type.
    /// </summary>
    public interface IPropertyBag : IReadOnlyPropertyBag
    {
        /// <summary>Set a property's value in the bag</summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="property">The property key</param>
        /// <param name="value">The value</param>
        void Set<T>(string property, T value);

        /// <summary>Remove a property value from the bag.</summary>
        /// <param name="property">The value to remove.</param>
        void Remove(string property);

        /// <summary>Set multiple property values in the bag at once.</summary>
        /// <param name="properties">Enumerable set of properties</param>
        void Set(IDictionary<string, object> properties);
    }
}
