// Decompiled with JetBrains decompiler
// Type: DuckGame.MagicNumberAttribute
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
