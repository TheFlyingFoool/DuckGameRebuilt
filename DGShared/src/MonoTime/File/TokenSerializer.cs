// Decompiled with JetBrains decompiler
// Type: DuckGame.TokenSerializer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class TokenSerializer : IDisposable
    {
        public const long kTokenizedIdentifier = 13826924961947138;
        public static TokenSerializer instance;
        private TokenSerializer _prevInstance;
        public List<string> _tokens = new List<string>();

        public TokenSerializer()
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

        public BitBuffer Finish(BitBuffer pBuffer)
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(13826924961947138L);
            bitBuffer.Write(_tokens.Count);
            instance = null;
            for (int index = 0; index < _tokens.Count; ++index)
                bitBuffer.Write(_tokens[index]);
            bitBuffer.Write(pBuffer, true);
            instance = this;
            return bitBuffer;
        }
    }
}
