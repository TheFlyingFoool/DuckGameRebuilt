// Decompiled with JetBrains decompiler
// Type: DuckGame.TokenSerializer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            if (TokenSerializer.instance != null)
                this._prevInstance = TokenSerializer.instance;
            TokenSerializer.instance = this;
        }

        public void Dispose()
        {
            if (TokenSerializer.instance != this)
                return;
            TokenSerializer.instance = this._prevInstance;
        }

        public int Token(string pString)
        {
            int num = this._tokens.IndexOf(pString);
            if (num == -1)
            {
                this._tokens.Add(pString);
                num = this._tokens.Count - 1;
            }
            return num;
        }

        public BitBuffer Finish(BitBuffer pBuffer)
        {
            BitBuffer bitBuffer = new BitBuffer();
            bitBuffer.Write(13826924961947138L);
            bitBuffer.Write(this._tokens.Count);
            TokenSerializer.instance = (TokenSerializer)null;
            for (int index = 0; index < this._tokens.Count; ++index)
                bitBuffer.Write(this._tokens[index]);
            bitBuffer.Write(pBuffer, true);
            TokenSerializer.instance = this;
            return bitBuffer;
        }
    }
}
