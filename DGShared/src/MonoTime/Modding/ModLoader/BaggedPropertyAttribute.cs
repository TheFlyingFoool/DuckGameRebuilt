// Decompiled with JetBrains decompiler
// Type: DuckGame.BaggedPropertyAttribute
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// Mark a property to be added to the initial property bag for this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class BaggedPropertyAttribute : Attribute
    {
        internal string Property { get; private set; }

        internal object Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DuckGame.BaggedPropertyAttribute" /> class.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="val">The value.</param>
        public BaggedPropertyAttribute(string prop, object val)
        {
            Property = prop;
            Value = val;
        }
    }
}
