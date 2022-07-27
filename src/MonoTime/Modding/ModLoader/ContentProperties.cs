// Decompiled with JetBrains decompiler
// Type: DuckGame.ContentProperties
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>
    /// A class for retrieving property bags associated with Types.
    /// </summary>
    public static class ContentProperties
    {
        internal static bool EditingAllowed = true;
        private static readonly Dictionary<System.Type, PropertyBag> _propertyBags = new Dictionary<System.Type, PropertyBag>();
        private static readonly ContentProperties.EmptyBag _emptyBag = new ContentProperties.EmptyBag();

        /// <summary>Initializes the bag of a single type.</summary>
        /// <param name="type">The type.</param>
        internal static void InitializeBag(System.Type type)
        {
            PropertyBag propertyBag = new PropertyBag();
            foreach (BaggedPropertyAttribute propertyAttribute in DG.Reverse<BaggedPropertyAttribute>(type.GetCustomAttributes(typeof(BaggedPropertyAttribute), true).Select<object, BaggedPropertyAttribute>(attrib => (BaggedPropertyAttribute)attrib).ToList<BaggedPropertyAttribute>().ToArray()))
                propertyBag.Set<object>(propertyAttribute.Property, propertyAttribute.Value);
            ContentProperties._propertyBags[type] = propertyBag;
        }

        /// <summary>Initializes the bags of multiple types.</summary>
        /// <param name="types">The types.</param>
        internal static void InitializeBags(IEnumerable<System.Type> types)
        {
            foreach (System.Type type in types)
                ContentProperties.InitializeBag(type);
        }

        /// <summary>
        /// Gets a read-only property bag associated with the type.
        /// </summary>
        /// <param name="t">The type to get the bag from.</param>
        /// <returns>The property bag</returns>
        public static IReadOnlyPropertyBag GetBag(System.Type t)
        {
            PropertyBag propertyBag;
            return ContentProperties._propertyBags.TryGetValue(t, out propertyBag) ? propertyBag : ContentProperties._emptyBag;
        }

        /// <summary>
        /// Gets a read-only property bag associated with the type.
        /// </summary>
        /// <typeparam name="T">The type to get the bag from</typeparam>
        /// <returns>The property bag</returns>
        public static IReadOnlyPropertyBag GetBag<T>() => ContentProperties.GetBag(typeof(T));

        public class EmptyBag : IReadOnlyPropertyBag
        {
            public IEnumerable<string> Properties => Enumerable.Empty<string>();

            public bool Contains(string property) => false;

            public object Get(string property) => throw new PropertyNotFoundException();

            public T Get<T>(string property) => throw new PropertyNotFoundException();

            public T GetOrDefault<T>(string property, T defaultValue) => defaultValue;

            public bool GetOrDefault(string property, bool defaultValue) => defaultValue;

            public bool? IsOfType<T>(string property) => new bool?();

            public T? TryGet<T>(string property) where T : struct => new T?();

            public bool TryGet<T>(string property, out T value)
            {
                value = default(T);
                return false;
            }

            public System.Type TypeOf(string property) => null;
        }
    }
}
