using System;
using static System.Math;

namespace DuckGame;

// Functions yoinked from https://easings.net/
// Ported to C# by Firebreak

public static class Ease
{
    public static class In
    {
        public static ProgressValue Sine(ProgressValue x)
        {
            return 1 - Cos(x * PI / 2);
        }

        public static ProgressValue Quad(ProgressValue x)
        {
            return x * x;
        }

        public static ProgressValue Cubic(ProgressValue x)
        {
            return x * x * x;
        }

        public static ProgressValue Quart(ProgressValue x)
        {
            return x * x * x;
        }

        public static ProgressValue Quint(ProgressValue x)
        {
            return x * x * x;
        }

        public static ProgressValue Exponential(ProgressValue x)
        {
            return x == 0 ? 0 : Pow(2, 10 * x - 10);
        }

        public static ProgressValue Circular(ProgressValue x)
        {
            return 1 - Sqrt(1 - Pow(x, 2));
        }

        public static ProgressValue Back(ProgressValue x)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }

        public static ProgressValue Elastic(ProgressValue x)
        {
            const double c4 = 2 * PI / 3;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : -Pow(2, 10 * x - 10) * Sin((x * 10 - 10.75) * c4);
        }

        public static ProgressValue Bounce(ProgressValue x)
        {
            return 1 - Out.Bounce(1 - x);
        }
    }
    public static class Out
    {
        public static ProgressValue Sine(ProgressValue x)
        {
            return Sin(x * PI / 2);
        }

        public static ProgressValue Quad(ProgressValue x)
        {
            return 1 - Pow(1 - x, 2);
        }

        public static ProgressValue Cubic(ProgressValue x)
        {
            return 1 - Pow(1 - x, 3);
        }

        public static ProgressValue Quart(ProgressValue x)
        {
            return 1 - Pow(1 - x, 4);
        }

        public static ProgressValue Quint(ProgressValue x)
        {
            return 1 - Pow(1 - x, 5);
        }

        public static ProgressValue Exponential(ProgressValue x)
        {
            return x == 1 ? 1 : 1 - Pow(2, -10 * x);
        }

        public static ProgressValue Circular(ProgressValue x)
        {
            return Sqrt(1 - Pow(x - 1, 2));
        }

        public static ProgressValue Back(ProgressValue x)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1;

            return 1 + c3 * Pow(x - 1, 3) + c1 * Pow(x - 1, 2);
        }

        public static ProgressValue Elastic(ProgressValue x)
        {
            const double c4 = 2 * PI / 3;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : Pow(2, -10 * x) * Sin((x * 10 - 0.75) * c4) + 1;
        }

        public static ProgressValue Bounce(ProgressValue x)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            return x < 1 / d1 
                ? n1 * x * x 
                : x < 2 / d1 
                    ? n1 * (x -= 1.5 / d1) * x + 0.75 
                    : x < 2.5 / d1 
                        ? n1 * (x -= 2.25 / d1) * x + 0.9375 
                        : n1 * (x -= 2.625 / d1) * x + 0.984375;
        }
    }
    
    public static class InOut
    {
        public static ProgressValue Sine(ProgressValue x)
        {
            return -(Cos(PI * x) - 1) / 2;
        }

        public static ProgressValue Quad(ProgressValue x)
        {
            return x < 0.5 ? 2 * x * x : 1 - Pow(-2 * x + 2, 2) / 2;
        }

        public static ProgressValue Cubic(ProgressValue x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - Pow(-2 * x + 2, 3) / 2;
        }

        public static ProgressValue Quart(ProgressValue x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - Pow(-2 * x + 2, 4) / 2;
        }

        public static ProgressValue Quint(ProgressValue x)
        {
            return x < 0.5 ? 8 * x * x * x * x * x : 1 - Pow(-2 * x + 2, 4) / 2;
        }

        public static ProgressValue Exponential(ProgressValue x)
        {
            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : x < 0.5 ? Pow(2, 20 * x - 10) / 2
                        : (2 - Pow(2, -20 * x + 10)) / 2;
        }

        public static ProgressValue Circular(ProgressValue x)
        {
            return x < 0.5
                ? (1 - Sqrt(1 - Pow(2 * x, 2))) / 2
                : (Sqrt(1 - Pow(-2 * x + 2, 2)) + 1) / 2;
        }

        public static ProgressValue Back(ProgressValue x)
        {
            const double c1 = 1.70158;
            const double c2 = c1 * 1.525;

            return x < 0.5
                ? Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2
                : (Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        public static ProgressValue Elastic(ProgressValue x)
        {
            const double c5 = 2 * PI / 4.5;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : x < 0.5
                        ? -(Pow(2, 20 * x - 10) * Sin((20 * x - 11.125) * c5)) / 2
                        : Pow(2, -20 * x + 10) * Sin((20 * x - 11.125) * c5) / 2 + 1;
        }

        public static ProgressValue Bounce(ProgressValue x)
        {
            return x < 0.5
                ? (1 - Out.Bounce(1 - 2 * x)) / 2
                : (1 + Out.Bounce(2 * x - 1)) / 2;
        }
    }
}