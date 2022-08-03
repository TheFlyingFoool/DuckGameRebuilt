// Decompiled with JetBrains decompiler
// Type: DuckGame.PriorityAttribute
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
