using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class TokenDeserializer : IDisposable
    {
        public static TokenDeserializer instance;
        private TokenDeserializer _prevInstance;
        public List<string> _tokens = new List<string>();

        public TokenDeserializer()
        {
            if (instance != null)
                _prevInstance = instance;
            instance = this;
        }

        public void Dispose()
        {
            if (instance != this)
                return;
            instance = _prevInstance;
        }

        public int Token(string pString)
        {
            int num = _tokens.IndexOf(pString);
            if (num == -1)
            {
                _tokens.Add(pString);
                num = _tokens.Count - 1;
            }
            return num;
        }

        public BitBuffer Start(BitBuffer pBuffer)
        {
            if (pBuffer.ReadLong() != 13826924961947138L)
            {
                instance = _prevInstance;
                pBuffer.position = 0;
                return pBuffer;
            }
            instance = null;
            int num = pBuffer.ReadInt();
            for (int index = 0; index < num; ++index)
                _tokens.Add(pBuffer.ReadString());
            BitBuffer bitBuffer = pBuffer.ReadBitBuffer(false);
            instance = this;
            return bitBuffer;
        }
    }
}
