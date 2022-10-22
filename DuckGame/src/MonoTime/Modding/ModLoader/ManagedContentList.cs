// Decompiled with JetBrains decompiler
// Type: DuckGame.ManagedContentList`1
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>Represents a list of mod-managed content of T's</summary>
    /// <typeparam name="T">The base type of content to store</typeparam>
    public class ManagedContentList<T>
    {
        private readonly HashSet<System.Type> _types = new HashSet<System.Type>();
        private readonly Dictionary<System.Type, System.Type> _redirections = new Dictionary<System.Type, System.Type>();
        internal void Add(System.Type type)
        {
            _types.Add(type);
        }

        internal IEnumerable<System.Type> AllSortedTypes => _types.OrderBy<System.Type, string>(t => t.FullName).OrderBy(type => type.IsDefined(typeof(ClientOnlyAttribute), false) ? 1 : 0); // Dan
        //(type.IsSubclassOf(parentType) && (Editor.clientonlycontent || !type.IsDefined(typeof(ClientOnlyAttribute), false)))
        internal IEnumerable<System.Type> SortedTypes => _types.Where<System.Type>(type => (Editor.clientonlycontent || !type.IsDefined(typeof(ClientOnlyAttribute), false))).OrderBy<System.Type, string>(t => t.FullName).OrderBy(type => type.IsDefined(typeof(ClientOnlyAttribute), false) ? 1 : 0);

        /// <summary>Gets the registered types.</summary>
        /// <value>The types registered.</value>
        public IEnumerable<System.Type> Types => _types;

        /// <summary>Removes a type from the type pool.</summary>
        /// <param name="type">The type.</param>
        public void Remove(System.Type type)
        {
            if (!_types.Contains(type) || type.GetCustomAttributes(typeof(LockedContentAttribute), true).Length != 0)
                return;
            _types.Remove(type);
        }

        /// <summary>Removes a generic type from the type pool.</summary>
        /// <typeparam name="E">The type to remove</typeparam>
        public void Remove<E>() where E : T => Remove(typeof(E));

        /// <summary>
        /// Redirects the a type to another type. Attempts to create an Old
        /// will result in a New being created instead.
        /// </summary>
        /// <param name="oldType">Old type, being redirected.</param>
        /// <param name="newType">The new type to redirect to.</param>
        public void Redirect(System.Type oldType, System.Type newType)
        {
            if (oldType.GetCustomAttributes(typeof(LockedContentAttribute), true).Length != 0)
                return;
            _redirections[oldType] = newType;
        }

        /// <summary>
        /// Redirects the generic Old type to the New type. Attempts to create an Old
        /// will result in a New being created instead.
        /// </summary>
        /// <typeparam name="Old">Old type, being redirected.</typeparam>
        /// <typeparam name="New">The new type to redirect to.</typeparam>
        public void Redirect<Old, New>()
          where Old : T
          where New : Old
        {
            Redirect(typeof(Old), typeof(New));
        }
    }
}
