using System;

namespace DuckGame;

[FireSerializerModule]
public class Vec2SerializerModule : IFireSerializerModule<Vec2>
{
    public string Serialize(Vec2 obj)
    {
        var (x, y) = obj;
        return $"{x},{y}";
    }

    public Vec2 Deserialize(string s)
    {
        return Vec2.Parse(s);
    }

    public bool CanSerialize(Type t)
    {
        return t == typeof(Vec2);
    }

    string IFireSerializerModule.Serialize(object obj) => Serialize((Vec2) obj);
    object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
}