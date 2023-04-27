using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Gives the current date and time's total milliseconds")]
        public static long Time() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}