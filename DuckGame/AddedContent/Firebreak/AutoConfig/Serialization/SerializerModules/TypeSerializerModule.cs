using System;
using System.Linq;
using AddedContent.Firebreak;

namespace DuckGame
{
    [Marker.FireSerializer]
    public class TypeSerializerModule : IFireSerializerModule<Type>
    {
        public string Serialize(Type obj)
        {
            return obj.FullName;
        }

        public Type Deserialize(string s)
        {
            return ModLoader.modAssemblyArray.First(x => x.GetType(s) != null).GetType(s);
        }

        public bool CanSerialize(Type t)
        {
            return t.InheritsFrom(typeof(Type));
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((Type)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}