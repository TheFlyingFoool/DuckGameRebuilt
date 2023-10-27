using System;

namespace DuckGame
{
    /// <summary>
    /// Indicates that this type is locked in the content list and cannot be modified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class LockedContentAttribute : Attribute
    {
    }
}
