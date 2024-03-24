using System;

namespace DuckGame.ConsoleEngine
{
    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class PrintSerializedAttribute : Attribute { }
}