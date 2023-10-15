using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "+", Description = "Adds 2 numbers.", To = ImplementTo.DuckShell)]
        public static double Add(double a, double b) => a + b;

        [Marker.DevConsoleCommand(Name = "-", Description = "Subtracts 2 numbers.", To = ImplementTo.DuckShell)]
        public static double Subtract(double a, double b) => a - b;

        [Marker.DevConsoleCommand(Name = "*", Description = "Multiplies 2 numbers.", To = ImplementTo.DuckShell)]
        public static double Multiply(double a, double b) => a * b;

        [Marker.DevConsoleCommand(Name = "/", Description = "Divides 2 numbers.", To = ImplementTo.DuckShell)]
        public static double Divide(double a, double b) => a / b;

        [Marker.DevConsoleCommand(Name = "^", Description = "Raises a number to the power of another number.", To = ImplementTo.DuckShell)]
        public static double RaiseToPower(double baseNumber, double exponent)
        {
            return Math.Pow(baseNumber, exponent);
        }

        [Marker.DevConsoleCommand(Name = ">", Description = "Returns True if a > b, else False.", To = ImplementTo.DuckShell)]
        public static bool GreaterThan(double a, double b) => a > b;

        [Marker.DevConsoleCommand(Name = "<", Description = "Returns True if a < b, else False.", To = ImplementTo.DuckShell)]
        public static bool LessThan(double a, double b) => a < b;

        [Marker.DevConsoleCommand(Name = "=", Description = "Returns True if a = b, else False.", To = ImplementTo.DuckShell)]
        public static bool EqualTo(double a, double b) => a == b;
        
        [Marker.DevConsoleCommand(Name = "!=", Description = "Returns False if a = b, else True.", To = ImplementTo.DuckShell)]
        public static bool NotEqualTo(double a, double b) => a != b;

        [Marker.DevConsoleCommand(Name = ">=", Description = "Returns True if a >= b, else False.", To = ImplementTo.DuckShell)]
        public static bool GreaterThanOrEqualTo(double a, double b) => a >= b;

        [Marker.DevConsoleCommand(Name = "<=", Description = "Returns True if a <= b, else False.", To = ImplementTo.DuckShell)]
        public static bool LessThanOrEqualTo(double a, double b) => a <= b;
    }
}