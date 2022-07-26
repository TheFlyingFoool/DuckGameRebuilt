// Decompiled with JetBrains decompiler
// Type: DuckGame.ModTypeMissingException
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModTypeMissingException : Exception
    {
        public ModTypeMissingException()
        {
        }

        public ModTypeMissingException(string message)
          : base(message)
        {
        }

        public ModTypeMissingException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModTypeMissingException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
