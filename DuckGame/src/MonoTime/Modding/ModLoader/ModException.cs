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
