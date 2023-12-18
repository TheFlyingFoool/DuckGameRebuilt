using System;

namespace DuckGame
{
    /// <summary>Declares which group this Thing is in the editor</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class NetworkAction : Attribute
    {
    }
}
