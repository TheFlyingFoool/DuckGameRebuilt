using System;

namespace DuckGame
{
    /// <summary>
    /// Declares a version number to be written for identifying the version of a BinaryClassChunk
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ChunkVersionAttribute : Attribute
    {
        public readonly ushort version;

        public ChunkVersionAttribute(ushort version) => this.version = version;
    }
}
