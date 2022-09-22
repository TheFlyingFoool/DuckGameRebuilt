using System;
using System.Linq;

namespace DuckGame
{

    [FireSerializerModule]
    public class DateTimeSerializeModule : IFireSerializerModule<DateTime>
    {
        public DateTime Deserialize(string s)
        {
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(s)).DateTime;
        }

        public string Serialize(DateTime obj)
        {
            return new DateTimeOffset(obj).ToUnixTimeSeconds().ToString();
        }

        public bool CanSerialize(Type t)
        {
            return t.InheritsFrom(typeof(DateTime));
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((DateTime)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}