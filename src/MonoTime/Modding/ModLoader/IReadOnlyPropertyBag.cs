// Decompiled with JetBrains decompiler
// Type: DuckGame.IReadOnlyPropertyBag
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// An interface allowing read-only access to a key/value pair of keys mapped to
    /// values of any type.
    /// </summary>
    public interface IReadOnlyPropertyBag
    {
        /// <summary>An enumerator to iterate over property keys.</summary>
        IEnumerable<string> Properties { get; }

        /// <summary>Get the type of a property contained in the bag.</summary>
        /// <param name="property">The property key.</param>
        /// <returns>The type of this property.</returns>
        /// <exception cref="T:DuckGame.PropertyNotFoundException">The property key is not in this property bag.</exception>
        System.Type TypeOf(string property);

        /// <summary>
        /// Check if a property in this bag is of, or assignable to, a certain type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="property">The property key.</param>
        /// <returns>true if the property is this type or is assignable to this type; false if not; null if the key does not exist or if the property value is null</returns>
        bool? IsOfType<T>(string property);

        /// <summary>Check if a property is in this bag.</summary>
        /// <param name="property">The property key.</param>
        /// <returns>true if the property is in this bag; false if not</returns>
        bool Contains(string property);

        /// <summary>Get an untyped property value from the bag.</summary>
        /// <param name="property">The property name.</param>
        /// <returns>The object, if it is in the bag</returns>
        /// <exception cref="T:DuckGame.PropertyNotFoundException">The property key is not in this property bag.</exception>
        object Get(string property);

        /// <summary>Get a property value from the bag.</summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <returns>The object casted to T if it is in the bag and can be converted to the requested type</returns>
        /// <exception cref="T:DuckGame.PropertyNotFoundException">The property key is not in this property bag.</exception>
        T Get<T>(string property);

        /// <summary>
        /// Tries to get a property value from a bag; does not throw, but returns null if the property is not there.
        /// </summary>
        /// <typeparam name="T">The type to unbox to. Must be a value type.</typeparam>
        /// <param name="property">The property name.</param>
        /// <returns>null if it does not exist, otherwise the property value</returns>
        T? TryGet<T>(string property) where T : struct;

        /// <summary>
        /// Tries to get a property value from a bag; does not throw.
        /// </summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The output value.</param>
        /// <returns>true if it was in the property bag</returns>
        bool TryGet<T>(string property, out T value);

        /// <summary>
        /// Get a property value from the bag. Does not throw, returns defaultValue if the key does not exist.
        /// </summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="defaultValue">The default value to use if the key is not in the bag.</param>
        /// <returns>The object casted to T if it is in the bag and can be converted to the requested type</returns>
        T GetOrDefault<T>(string property, T defaultValue);

        bool GetOrDefault(string property, bool defaultValue);
    }
}
