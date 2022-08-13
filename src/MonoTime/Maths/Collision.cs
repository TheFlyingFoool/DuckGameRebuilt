// Decompiled with JetBrains decompiler
// Type: DuckGame.Collision
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public static bool Point(Vec2 point, Thing t) => point.x >= t.left && point.x <= t.right && point.y >= t.top && point.y <= t.bottom;

        public static bool Point(Vec2 point, Rectangle r) => point.x >= r.Left && point.x <= r.Right && point.y >= r.Top && point.y <= r.Bottom;

        public static bool Line(Vec2 point1, Vec2 point2, Thing t)
        {
            double left = t.left;
            double top = t.top;
            double right = t.right;
            double bottom = t.bottom;
            double num1 = point1.x;
            double num2 = point2.x;
            if (point1.x > point2.x)
            {
                num1 = point2.x;
                num2 = point1.x;
            }
            if (num2 > right)
                num2 = right;
            if (num1 < left)
                num1 = left;
            if (num1 > num2)
                return false;
            double num3 = point1.y;
            double num4 = point2.y;
            double num5 = point2.x - point1.x;
            if (Math.Abs(num5) > 1E-07)
            {
                double num6 = (point2.y - point1.y) / num5;
                double num7 = point1.y - num6 * point1.x;
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
            double x = rect.x;
            double y = rect.y;
            double num1 = rect.x + rect.width;
            double num2 = rect.y + rect.height;
            double num3 = point1.x;
            double num4 = point2.x;
            if (point1.x > point2.x)
            {
                num3 = point2.x;
                num4 = point1.x;
            }
            if (num4 > num1)
                num4 = num1;
            if (num3 < x)
                num3 = x;
            if (num3 > num4)
                return false;
            double num5 = point1.y;
            double num6 = point2.y;
            double num7 = point2.x - point1.x;
            if (Math.Abs(num7) > 1E-07)
            {
                double num8 = (point2.y - point1.y) / num7;
                double num9 = point1.y - num8 * point1.x;
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

        public static bool CCW(Vec2 A, Vec2 B, Vec2 C) => (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);

        public static bool LineIntersect(Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4) => Collision.CCW(p1, p3, p4) != Collision.CCW(p2, p3, p4) && Collision.CCW(p1, p2, p3) != Collision.CCW(p1, p2, p4);

        public static bool Circle(Vec2 center, float radius, Thing t)
        {
            Vec2 vec2_1 = new Vec2(center);
            if (center.x < t.left)
                vec2_1.x = t.left;
            else if (center.x > t.right)
                vec2_1.x = t.right;
            if (center.y < t.top)
                vec2_1.y = t.top;
            else if (center.y > t.bottom)
                vec2_1.y = t.bottom;
            Vec2 vec2_2 = vec2_1 - center;
            return vec2_2.x * vec2_2.x + vec2_2.y * vec2_2.y <= radius * radius;
        }

        public static bool Circle(Vec2 center, float radius, Rectangle t)
        {
            Vec2 vec2_1 = new Vec2(center);
            if (center.x < t.Left)
                vec2_1.x = t.Left;
            else if (center.x > t.Right)
                vec2_1.x = t.Right;
            if (center.y < t.Top)
                vec2_1.y = t.Top;
            else if (center.y > t.Bottom)
                vec2_1.y = t.Bottom;
            Vec2 vec2_2 = vec2_1 - center;
            return vec2_2.x * vec2_2.x + vec2_2.y * vec2_2.y <= radius * radius;
        }

        public static bool Rect(Vec2 tl1, Vec2 br1, Thing t)
        {
            return br1.y >= t.top && tl1.y <= t.bottom && tl1.x <= t.right && br1.x >= t.left;//!(br1.y < t.top || tl1.y > t.bottom || tl1.x > t.right || br1.x < t.left); // return br1.y >= t.top && tl1.y <= t.bottom && tl1.x <= t.right && br1.x >= t.left;
        }

        public static bool Rect(Vec2 tl1, Vec2 br1, Rectangle t)
        {
            return !(br1.y < t.y && tl1.y > t.Bottom && tl1.x > t.Right && br1.x < t.x);//br1.y >= t.y && tl1.y <= t.Bottom && tl1.x <= t.Right && br1.x >= t.x;
        }

        public static bool Rect(Rectangle r1, Rectangle r2)
        {
            return !(r1.y + r1.height < r2.y || r1.y > r2.y + r2.height || r1.x > r2.x + r2.width || r1.x + r1.width < r2.x);//r1.y + r1.height >= r2.y && r1.y <= r2.y + r2.height && r1.x <= r2.x + r2.width && r1.x + r1.width >= r2.x;
        }

        public static bool Rect(Rectangle r1, Vec4 r2) => r1.y + r1.height >= r2.y && r1.y <= r2.y + r2.w && r1.x <= r2.x + r2.z && r1.x + r1.width >= r2.x;

        public static Vec2 LineIntersectPoint(
          Vec2 line1V1,
          Vec2 line1V2,
          Vec2 line2V1,
          Vec2 line2V2)
        {
            float num1 = line1V2.y - line1V1.y;
            float num2 = line1V1.x - line1V2.x;
            float num3 = (num1 * line1V1.x + num2 * line1V1.y);
            float num4 = line2V2.y - line2V1.y;
            float num5 = line2V1.x - line2V2.x;
            float num6 = (num4 * line2V1.x + num5 * line2V1.y);
            float num7 = (num1 * num5 - num4 * num2);
            return num7 == 0f ? Vec2.Zero : new Vec2((num5 * num3 - num2 * num6) / num7, (num1 * num6 - num4 * num3) / num7);
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
                if (numArray1[index] == 0f)
                {
                    if (numArray2[index] < 0f)
                        return Vec2.Zero;
                }
                else
                {
                    float num3 = numArray2[index] / numArray1[index];
                    if (numArray1[index] < 0f && num1 < num3)
                        num1 = num3;
                    else if (numArray1[index] > 0f && num2 > num3)
                        num2 = num3;
                }
            }
            return num1 > num2 || num1 > 1f || num1 < 0f ? Vec2.Zero : new Vec2(point1.x + num1 * vec2.x, point1.y + num1 * vec2.y);
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
                if (numArray1[index] == 0f)
                {
                    if (numArray2[index] < 0f)
                        return Vec2.Zero;
                }
                else
                {
                    float num3 = numArray2[index] / numArray1[index];
                    if (numArray1[index] < 0f && num1 < num3)
                        num1 = num3;
                    else if (numArray1[index] > 0f && num2 > num3)
                        num2 = num3;
                }
            }
            return num1 > num2 || num1 > 1f || num1 < 0f ? Vec2.Zero : new Vec2(point1.x + num1 * vec2.x, point1.y + num1 * vec2.y);
        }
    }
}
