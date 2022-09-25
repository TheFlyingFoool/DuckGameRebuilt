using System;

namespace DuckGame
{
    [FireSerializerModule]
    public class StringSerializerModule : IFireSerializerModule<string>
    {
        public string Serialize(string obj)
        {
            return obj;
        }

        public string Deserialize(string s)
        {
            return s;
        }

        public bool CanSerialize(Type t)
        {
            return typeof(string) == t;
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((string)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}