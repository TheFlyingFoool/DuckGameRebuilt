// Decompiled with JetBrains decompiler
// Type: DuckGame.ModException
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ModException : Exception
    {
        public Exception exception;
        public ModConfiguration mod;

        public ModException(string pMessage, ModConfiguration pMod, Exception pRealException)
          : base(pMessage)
        {
            exception = pRealException;
            mod = pMod;
        }
    }
}
