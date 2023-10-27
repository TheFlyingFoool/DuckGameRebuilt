using System;

namespace DuckGame
{
    /// <summary>An attribute to mark the priority of something.</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class PriorityAttribute : Attribute
    {
        /// <summary>Gets the priority of this target.</summary>
        /// <value>The priority.</value>
        public Priority Priority { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DuckGame.PriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The priority.</param>
        public PriorityAttribute(Priority priority) => Priority = priority;
    }
}
