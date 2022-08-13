// Decompiled with JetBrains decompiler
// Type: DuckGame.IPropertyBag
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
