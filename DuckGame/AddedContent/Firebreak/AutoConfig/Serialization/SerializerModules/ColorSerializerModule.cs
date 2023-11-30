using System;
using System.Linq;

namespace DuckGame
{
    [global::AddedContent.Firebreak.Marker.FireSerializer]
    public class ColorSerializerModule : IFireSerializerModule<Microsoft.Xna.Framework.Color>
    {
        public Microsoft.Xna.Framework.Color Deserialize(string s)
        {
            byte[] split = s.Split(',').Select(byte.Parse).ToArray();
            return new Microsoft.Xna.Framework.Color(split[0], split[1], split[2]);
        }

        public string Serialize(Microsoft.Xna.Framework.Color obj)
        {
            return $"{obj.R},{obj.G},{obj.B}";
        }

        public bool CanSerialize(Type t)
        {
            return t.InheritsFrom(typeof(Microsoft.Xna.Framework.Color));
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((Microsoft.Xna.Framework.Color)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}
