// Decompiled with JetBrains decompiler
// Type: DuckGame.Curve
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Curve
    {
        private static double[] kFactorialLookup;

        public static void System_Initialize() => CreateFactorialTable();

        private static double factorial(int n)
        {
            if (n < 0)
                throw new Exception("n is less than 0");
            return n <= 32 ? kFactorialLookup[n] : throw new Exception("n is greater than 32");
        }

        private static void CreateFactorialTable() => kFactorialLookup = new double[33]
        {
      1,
      1,
      2,
      6,
      24,
      120,
      720,
      5040,
      40320,
      362880,
      3628800,
      39916800,
      479001600,
      6227020800,
      87178291200,
      1307674368000,
      20922789888000,
      355687428096000,
      6.402373705728E+15,
      1.21645100408832E+17,
      2.43290200817664E+18,
      5.10909421717094E+19,
      1.12400072777761E+21,
      2.5852016738885E+22,
      6.20448401733239E+23,
      1.5511210043331E+25,
      4.03291461126606E+26,
      1.08888694504184E+28,
      3.04888344611714E+29,
      8.8417619937397E+30,
      2.65252859812191E+32,
      8.22283865417792E+33,
      2.63130836933694E+35
        };

        private static double Ni(int n, int i) => factorial(n) / (factorial(i) * factorial(n - i));

        private static double Bernstein(int n, int i, double t)
        {
            double num1 = t != 0d || i != 0 ? Math.Pow(t, i) : 1d;
            double num2 = n != i || t != 1d ? Math.Pow(1d - t, n - i) : 1d;
            return Ni(n, i) * num1 * num2;
        }

        public static Vec2 Calculate(Vec2 start, Vec2 end, float lerp, float arcSizeMult = 1f)
        {
            Vec2 vec2_1 = (start + end) / 2f;
            if (end.x == start.x)
            {
                vec2_1.x = end.y <= start.y ? start.x + 6f * arcSizeMult : start.x - 6f * arcSizeMult;
                arcSizeMult *= 0.2f;
            }
            vec2_1.y = end.y <= start.y ? end.y - 16f * arcSizeMult : start.y - 22f * arcSizeMult;
            List<Vec2> vec2List = Bezier(8, start, vec2_1, end);
            float num1 = 0f;
            for (int index = 1; index < vec2List.Count; ++index)
                num1 += (vec2List[index] - vec2List[index - 1]).length;
            double num2 = num1 / vec2List.Count;
            int index1 = (int)Math.Floor(lerp * vec2List.Count) + 1;
            if (index1 >= vec2List.Count)
                return end;
            Vec2 vec2_2 = vec2List[index1 - 1];
            Vec2 vec2_3 = vec2List[index1];
            float num3 = lerp % (1f / vec2List.Count) * vec2List.Count;
            end = vec2_2 + (vec2_3 - vec2_2) * num3;
            return end;
        }

        public static List<Vec2> Bezier(int cpts, params Vec2[] points)
        {
            int length = points.Length;
            double t = 0d;
            double num1 = 1d / (cpts - 1);
            List<Vec2> vec2List = new List<Vec2>();
            for (int index = 0; index != cpts; ++index)
            {
                Vec2 vec2 = new Vec2();
                if (1d - t < 5E-06)
                    t = 1d;
                for (int i = 0; i != length; ++i)
                {
                    float num2 = (float)Bernstein(length - 1, i, t);
                    vec2.x += num2 * points[i].x;
                    vec2.y += num2 * points[i].y;
                }
                t += num1;
                vec2List.Add(vec2);
            }
            return vec2List;
        }
    }
}
