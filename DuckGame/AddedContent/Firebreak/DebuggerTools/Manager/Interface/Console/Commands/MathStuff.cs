using System;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Name = "+", Description = "Adds 2 numbers.", Hidden = true)]
        public static double Add(double a, double b) => a + b;

        [MMCommand(Name = "-", Description = "Subtracts 2 numbers.", Hidden = true)]
        public static double Subtract(double a, double b) => a - b;

        [MMCommand(Name = "*", Description = "Multiplies 2 numbers.", Hidden = true)]
        public static double Multiply(double a, double b) => a * b;

        [MMCommand(Name = "/", Description = "Divides 2 numbers.", Hidden = true)]
        public static double Divide(double a, double b) => a / b;

        [MMCommand(Name = "^", Description = "Raises a number to the power of another number.", Hidden = true)]
        public static double RaiseToPower(double baseNumber, double exponent)
        {
            return Math.Pow(baseNumber, exponent);
        }

        [MMCommand(Name = ">", Description = "Returns True if a > b, else False.", Hidden = true)]
        public static bool GreaterThan(double a, double b) => a > b;

        [MMCommand(Name = "<", Description = "Returns True if a < b, else False.", Hidden = true)]
        public static bool LessThan(double a, double b) => a < b;

        [MMCommand(Name = "=", Description = "Returns True if a = b, else False.", Hidden = true)]
        public static bool EqualTo(double a, double b) => a == b;

        [MMCommand(Name = ">=", Description = "Returns True if a >= b, else False.", Hidden = true)]
        public static bool GreaterThanOrEqualTo(double a, double b) => a >= b;

        [MMCommand(Name = "<=", Description = "Returns True if a <= b, else False.", Hidden = true)]
        public static bool LessThanOrEqualTo(double a, double b) => a <= b;
    }
}