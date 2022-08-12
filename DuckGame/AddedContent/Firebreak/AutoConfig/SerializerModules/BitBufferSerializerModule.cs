using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame;

[FireSerializerModule]
public class BitBufferSerializerModule : IFireSerializerModule<BitBuffer>
{
    public BitBuffer Deserialize(string s)
    {
        return new BitBuffer(s.Select(Convert.ToByte).ToArray());
    }

    public string Serialize(BitBuffer obj)
    {
        return BitConverter.ToString(obj.buffer);
    }

    public bool CanSerialize(Type t)
    {
        return t == typeof(BitBuffer);
    }
    
    string IFireSerializerModule.Serialize(object obj) => Serialize((BitBuffer) obj);
    object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
}