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
