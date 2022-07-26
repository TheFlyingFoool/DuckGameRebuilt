// Decompiled with JetBrains decompiler
// Type: DuckGame.Collision
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public static class Collision
    {
        public static void Initialize()
        {
        }

        public static bool Point(Vec2 point, Thing t) => (double)point.x >= (double)t.left && (double)point.x <= (double)t.right && (double)point.y >= (double)t.top && (double)point.y <= (double)t.bottom;

        public static bool Point(Vec2 point, Rectangle r) => (double)point.x >= (double)r.Left && (double)point.x <= (double)r.Right && (double)point.y >= (double)r.Top && (double)point.y <= (double)r.Bottom;

        public static bool Line(Vec2 point1, Vec2 point2, Thing t)
        {
            double left = (double)t.left;
            double top = (double)t.top;
            double right = (double)t.right;
            double bottom = (double)t.bottom;
            double num1 = (double)point1.x;
            double num2 = (double)point2.x;
            if ((double)point1.x > (double)point2.x)
            {
                num1 = (double)point2.x;
                num2 = (double)point1.x;
            }
            if (num2 > right)
                num2 = right;
            if (num1 < left)
                num1 = left;
            if (num1 > num2)
                return false;
            double num3 = (double)point1.y;
            double num4 = (double)point2.y;
            double num5 = (double)point2.x - (double)point1.x;
            if (Math.Abs(num5) > 1E-07)
            {
                double num6 = ((double)point2.y - (double)point1.y) / num5;
                double num7 = (double)point1.y - num6 * (double)point1.x;
                num3 = num6 * num1 + num7;
                num4 = num6 * num2 + num7;
            }
            if (num3 > num4)
            {
                double num8 = num4;
                num4 = num3;
                num3 = num8;
            }
            if (num4 > bottom)
                num4 = bottom;
            if (num3 < top)
                num3 = top;
            return num3 <= num4;
        }

        public static bool Line(Vec2 point1, Vec2 point2, Rectangle rect)
        {
            double x = (double)rect.x;
            double y = (double)rect.y;
            double num1 = (double)rect.x + (double)rect.width;
            double num2 = (double)rect.y + (double)rect.height;
            double num3 = (double)point1.x;
            double num4 = (double)point2.x;
            if ((double)point1.x > (double)point2.x)
            {
                num3 = (double)point2.x;
                num4 = (double)point1.x;
            }
            if (num4 > num1)
                num4 = num1;
            if (num3 < x)
                num3 = x;
            if (num3 > num4)
                return false;
            double num5 = (double)point1.y;
            double num6 = (double)point2.y;
            double num7 = (double)point2.x - (double)point1.x;
            if (Math.Abs(num7) > 1E-07)
            {
                double num8 = ((double)point2.y - (double)point1.y) / num7;
                double num9 = (double)point1.y - num8 * (double)point1.x;
                num5 = num8 * num3 + num9;
                num6 = num8 * num4 + num9;
            }
            if (num5 > num6)
            {
                double num10 = num6;
                num6 = num5;
                num5 = num10;
            }
            if (num6 > num2)
                num6 = num2;
            if (num5 < y)
                num5 = y;
            return num5 <= num6;
        }

        public static bool CCW(Vec2 A, Vec2 B, Vec2 C) => ((double)C.y - (double)A.y) * ((double)B.x - (double)A.x) > ((double)B.y - (double)A.y) * ((double)C.x - (double)A.x);

        public static bool LineIntersect(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4) => Collision.CCW(p1, p3, p4) != Collision.CCW(p2, p3, p4) && Collision.CCW(p1, p2, p3) != Collision.CCW(p1, p2, p4);

        public static bool Circle(Vec2 center, float radius, Thing t)
        {
            Vec2 vec2_1 = new Vec2(center);
            if ((double)center.x < (double)t.left)
                vec2_1.x = t.left;
            else if ((double)center.x > (double)t.right)
                vec2_1.x = t.right;
            if ((double)center.y < (double)t.top)
                vec2_1.y = t.top;
            else if ((double)center.y > (double)t.bottom)
                vec2_1.y = t.bottom;
            Vec2 vec2_2 = vec2_1 - center;
            return (double)vec2_2.x * (double)vec2_2.x + (double)vec2_2.y * (double)vec2_2.y <= (double)radius * (double)radius;
        }

        public static bool Circle(Vec2 center, float radius, Rectangle t)
        {
            Vec2 vec2_1 = new Vec2(center);
            if ((double)center.x < (double)t.Left)
                vec2_1.x = t.Left;
            else if ((double)center.x > (double)t.Right)
                vec2_1.x = t.Right;
            if ((double)center.y < (double)t.Top)
                vec2_1.y = t.Top;
            else if ((double)center.y > (double)t.Bottom)
                vec2_1.y = t.Bottom;
            Vec2 vec2_2 = vec2_1 - center;
            return (double)vec2_2.x * (double)vec2_2.x + (double)vec2_2.y * (double)vec2_2.y <= (double)radius * (double)radius;
        }

        public static bool Rect(Vec2 tl1, Vec2 br1, Thing t) => (double)br1.y >= (double)t.top && (double)tl1.y <= (double)t.bottom && (double)tl1.x <= (double)t.right && (double)br1.x >= (double)t.left;

        public static bool Rect(Vec2 tl1, Vec2 br1, Rectangle t) => (double)br1.y >= (double)t.y && (double)tl1.y <= (double)t.Bottom && (double)tl1.x <= (double)t.Right && (double)br1.x >= (double)t.x;

        public static bool Rect(Rectangle r1, Rectangle r2) => (double)r1.y + (double)r1.height >= (double)r2.y && (double)r1.y <= (double)r2.y + (double)r2.height && (double)r1.x <= (double)r2.x + (double)r2.width && (double)r1.x + (double)r1.width >= (double)r2.x;

        public static bool Rect(Rectangle r1, Vec4 r2) => (double)r1.y + (double)r1.height >= (double)r2.y && (double)r1.y <= (double)r2.y + (double)r2.w && (double)r1.x <= (double)r2.x + (double)r2.z && (double)r1.x + (double)r1.width >= (double)r2.x;

        public static Vec2 LineIntersectPoint(
          Vec2 line1V1,
          Vec2 line1V2,
          Vec2 line2V1,
          Vec2 line2V2)
        {
            float num1 = line1V2.y - line1V1.y;
            float num2 = line1V1.x - line1V2.x;
            float num3 = (float)((double)num1 * (double)line1V1.x + (double)num2 * (double)line1V1.y);
            float num4 = line2V2.y - line2V1.y;
            float num5 = line2V1.x - line2V2.x;
            float num6 = (float)((double)num4 * (double)line2V1.x + (double)num5 * (double)line2V1.y);
            float num7 = (float)((double)num1 * (double)num5 - (double)num4 * (double)num2);
            return (double)num7 == 0.0 ? Vec2.Zero : new Vec2((float)((double)num5 * (double)num3 - (double)num2 * (double)num6) / num7, (float)((double)num1 * (double)num6 - (double)num4 * (double)num3) / num7);
        }

        public static Vec2 LinePoint(Vec2 point1, Vec2 point2, Thing thing)
        {
            Vec2 vec2 = point2 - point1;
            float[] numArray1 = new float[4]
            {
        -vec2.x,
        vec2.x,
        -vec2.y,
        vec2.y
            };
            float[] numArray2 = new float[4]
            {
        point1.x - thing.left,
        thing.right - point1.x,
        point1.y - thing.top,
        thing.bottom - point1.y
            };
            float num1 = float.NegativeInfinity;
            float num2 = float.PositiveInfinity;
            for (int index = 0; index < 4; ++index)
            {
                if ((double)numArray1[index] == 0.0)
                {
                    if ((double)numArray2[index] < 0.0)
                        return Vec2.Zero;
                }
                else
                {
                    float num3 = numArray2[index] / numArray1[index];
                    if ((double)numArray1[index] < 0.0 && (double)num1 < (double)num3)
                        num1 = num3;
                    else if ((double)numArray1[index] > 0.0 && (double)num2 > (double)num3)
                        num2 = num3;
                }
            }
            return (double)num1 > (double)num2 || (double)num1 > 1.0 || (double)num1 < 0.0 ? Vec2.Zero : new Vec2(point1.x + num1 * vec2.x, point1.y + num1 * vec2.y);
        }

        public static Vec2 LinePoint(Vec2 point1, Vec2 point2, Rectangle rect)
        {
            Vec2 vec2 = point2 - point1;
            float[] numArray1 = new float[4]
            {
        -vec2.x,
        vec2.x,
        -vec2.y,
        vec2.y
            };
            float[] numArray2 = new float[4]
            {
        point1.x - rect.x,
        rect.x + rect.width - point1.x,
        point1.y - rect.y,
        rect.y + rect.height - point1.y
            };
            float num1 = float.NegativeInfinity;
            float num2 = float.PositiveInfinity;
            for (int index = 0; index < 4; ++index)
            {
                if ((double)numArray1[index] == 0.0)
                {
                    if ((double)numArray2[index] < 0.0)
                        return Vec2.Zero;
                }
                else
                {
                    float num3 = numArray2[index] / numArray1[index];
                    if ((double)numArray1[index] < 0.0 && (double)num1 < (double)num3)
                        num1 = num3;
                    else if ((double)numArray1[index] > 0.0 && (double)num2 > (double)num3)
                        num2 = num3;
                }
            }
            return (double)num1 > (double)num2 || (double)num1 > 1.0 || (double)num1 < 0.0 ? Vec2.Zero : new Vec2(point1.x + num1 * vec2.x, point1.y + num1 * vec2.y);
        }
    }
}
