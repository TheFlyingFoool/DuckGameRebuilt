// Decompiled with JetBrains decompiler
// Type: DuckGame.PropertyBag
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>Implementation of property bag</summary>
    public class PropertyBag : IPropertyBag, IReadOnlyPropertyBag
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        internal PropertyBag()
        {
        }

        /// <summary>An enumerator to iterate over property keys.</summary>
        public IEnumerable<string> Properties => _dictionary.Keys;

        /// <summary>Check if a property is in this bag.</summary>
        /// <param name="property">The property key.</param>
        /// <returns>true if the property is in this bag; false if not</returns>
        public bool Contains(string property) => this._dictionary.ContainsKey(property);

        /// <summary>Get an untyped property value from the bag.</summary>
        /// <param name="property">The property name.</param>
        /// <returns>The object, if it is in the bag</returns>
        /// <exception cref="T:DuckGame.PropertyNotFoundException">The property key is not in this property bag.</exception>
        public object Get(string property)
        {
            object obj;
            if (!this._dictionary.TryGetValue(property, out obj))
                throw new PropertyNotFoundException("key " + property + " not found in bag");
            return obj;
        }

        /// <summary>Get a property value from the bag.</summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <returns>
        /// The object casted to T if it is in the bag and can be converted to the requested type
        /// </returns>
        /// <exception cref="T:DuckGame.PropertyNotFoundException">The property key is not in this property bag.</exception>
        public T Get<T>(string property) => (T)this.Get(property);

        /// <summary>
        /// Check if a property in this bag is of, or assignable to, a certain type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="property">The property key.</param>
        /// <returns>
        /// true if the property is this type or is assignable to this type; false if not; null if the key does not exist or if the property value is null
        /// </returns>
        public bool? IsOfType<T>(string property)
        {
            object obj;
            return !this._dictionary.TryGetValue(property, out obj) || obj == null ? new bool?() : new bool?(obj.GetType().IsAssignableFrom(typeof(T)));
        }

        /// <summary>Get the type of a property contained in the bag.</summary>
        /// <param name="property">The property key.</param>
        /// <returns>The type of this property.</returns>
        public System.Type TypeOf(string property)
        {
            object obj;
            return !this._dictionary.TryGetValue(property, out obj) || obj == null ? null : obj.GetType();
        }

        /// <summary>Set a property's value in the bag</summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="property">The property key</param>
        /// <param name="value">The value</param>
        public void Set<T>(string property, T value) => this._dictionary[property] = value;

        /// <summary>Remove a property value from the bag.</summary>
        /// <param name="property">The value to remove.</param>
        public void Remove(string property) => this._dictionary.Remove(property);

        /// <summary>Set multiple property values in the bag at once.</summary>
        /// <param name="properties">Enumerable set of properties</param>
        public void Set(IDictionary<string, object> properties)
        {
            foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>)properties)
                this.Set<object>(property.Key, property.Value);
        }

        /// <summary>
        /// Tries to get a property value from a bag; does not throw, but returns null if the property is not there.
        /// </summary>
        /// <typeparam name="T">The type to unbox to. Must be a value type.</typeparam>
        /// <param name="property">The property name.</param>
        /// <returns>null if it does not exist, otherwise the property value</returns>
        public T? TryGet<T>(string property) where T : struct
        {
            object obj;
            return !this._dictionary.TryGetValue(property, out obj) ? new T?() : new T?((T)obj);
        }

        /// <summary>
        /// Tries to get a property value from a bag; does not throw.
        /// </summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="value">The output value.</param>
        /// <returns>true if it was in the property bag</returns>
        public bool TryGet<T>(string property, out T value)
        {
            object obj;
            if (!this._dictionary.TryGetValue(property, out obj))
            {
                value = default(T);
                return false;
            }
            value = (T)obj;
            return true;
        }

        /// <summary>
        /// Get a property value from the bag. Does not throw, returns defaultValue if the key does not exist.
        /// </summary>
        /// <typeparam name="T">The type to unbox to.</typeparam>
        /// <param name="property">The property name.</param>
        /// <param name="defaultValue">The default value to use if the key is not in the bag.</param>
        /// <returns>The object casted to T if it is in the bag and can be converted to the requested type</returns>
        public T GetOrDefault<T>(string property, T defaultValue)
        {
            object obj;
            return !this._dictionary.TryGetValue(property, out obj) ? defaultValue : (T)obj;
        }

        public bool GetOrDefault(string property, bool defaultValue)
        {
            object obj;
            return !this._dictionary.TryGetValue(property, out obj) ? defaultValue : (bool)obj;
        }
    }
}
