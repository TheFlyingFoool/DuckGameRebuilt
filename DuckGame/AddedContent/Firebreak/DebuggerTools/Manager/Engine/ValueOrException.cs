using System;

namespace DuckGame.ConsoleEngine;

public class ValueOrException<T>
{
    public T Value { get; set; } = default!;
    public bool Failed { get; set; }
    public Exception Error { get; set; } = default!;

    public static implicit operator ValueOrException<T>(Exception e)
    {
        return FromError(e);
    }

    public static implicit operator ValueOrException<T>(T value)
    {
        return FromValue(value);
    }
    
    public static ValueOrException<T> FromError(Exception e)
    {
        return new ValueOrException<T>()
        {
            Failed = true,
            Error = e
        };
    }

    public static ValueOrException<T> FromValue(T value)
    {
        return new ValueOrException<T>()
        {
            Value = value
        };
    }

    public void TryUse(Action<T> usage, Action<Exception> failureUsage)
    {
        if (Failed) 
            failureUsage(Error!);
        else usage(Value);
    }

    public override string ToString()
    {
        if (Failed)
            return Error.Message;

        return Value?.ToString() ?? "";
    }
}