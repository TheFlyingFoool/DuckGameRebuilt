using System;

namespace DuckGame
{
    /// <summary>
    /// Declares a magic number to be written for identifying a BinaryClassChunk
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MagicNumberAttribute : Attribute
    {
        public readonly long magicNumber;

        public MagicNumberAttribute(long number) => magicNumber = number;
    }
}
