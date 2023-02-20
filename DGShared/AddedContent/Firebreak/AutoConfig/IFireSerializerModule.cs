using System;

namespace DuckGame
{
    public interface IFireSerializerModule<T> : IFireSerializerModule
    {
        new string Serialize(T? obj);

        new T Deserialize(string s);
    }

    public interface IFireSerializerModule
    {
        string Serialize(object? obj);

        object Deserialize(string s);

        bool CanSerialize(Type t);
    }
}