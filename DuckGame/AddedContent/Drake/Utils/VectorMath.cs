using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.Utils
{
    public static class VectorMath
    {
        public const float DegToRad = 0.0174533f;

        public const float RadToDeg = 57.2958f;

        public static Vector2 CalcHitPoint(Vector2 start, Vector2 end, Thing thing)
        {
            Vector2[] intersects = {
            CalcIntersection(start, end, thing.topLeft, thing.bottomLeft),
            CalcIntersection(start, end, thing.topLeft, thing.topRight),
            CalcIntersection(start, end, thing.topRight, thing.bottomRight),
            CalcIntersection(start, end, thing.bottomRight, thing.bottomLeft)
            };

            Vector2 nearest = end;
            float distance = (end - start).Length();

            foreach (Vector2 vec in intersects)
            {
                if (float.IsNaN(vec.X) || float.IsNaN(vec.Y))
                {
                    continue;
                }

                float dist = (vec - start).Length();
                if (!(dist < distance)) continue;
                distance = dist;
                nearest = vec;

            }

            return nearest;
        }

        public static Vector2 CalcIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, bool zeroIfNotOnLine = true, float accuracy = 0f)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.X;

            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                return Vec2.Zero;
            }

            Vec2 intersect = new Vec2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

            if (!zeroIfNotOnLine)
            {
                return intersect;
            }
            else if (PointOnLine(intersect, s1, e1, accuracy) && PointOnLine(intersect, s2, e2, accuracy))
            {
                return intersect;
            }

            return Vec2.Zero;
        }

        public static bool PointOnLine(Vector2 point, Vector2 s, Vector2 e, float accuracy = 0f)
        {
            float dist1 = s.DistTo(point);
            float dist2 = e.DistTo(point);
            float dist3 = s.DistTo(e);
            float dist4 = dist1 + dist2;

            return dist4 - dist3 <= accuracy;
        }

        public static float DistTo(this Vector2 s, Vector2 e) => (s - e).Length();
        public static float DistSqr(Vector2 s, Vector2 e) => (s - e).LengthSquared();
        public static Vector2 PerpCw(this Vec2 vec) => new Vec2(vec.y, -vec.x);

        public static Vector2 PerpCcw(this Vec2 vec) => new Vec2(-vec.y, vec.x);

        public static Vector2 PerpCw(this Vec2 start, Vec2 end) => (end - start).PerpCw();

        public static Vector2 PerpCcw(this Vec2 start, Vec2 end) => (end - start).PerpCcw();

        public static Vector2 CalcVec(float degrees, float magnitude, int offDir)
        {
            if (offDir == -1) degrees = 180 - degrees;
            return CalcVec(degrees, magnitude);
        }

        public static Vector2 CalcVec(float degrees, float magnitude)
        {
            degrees *= DegToRad;
            return new Vec2((float)(magnitude * Math.Cos(degrees)), (float)(magnitude * Math.Sin(degrees)));
        }


        public static Vector2[] CalcClosestPoints(this Vector2 origin, Vector2[] points, int number)
        {
            if (number > points.Length)
            {
                throw new Exception();
            }

            points = points.OrderBy(x => -DistSqr(x, origin)).ToArray();
            points = points.Reverse().ToArray();

            return new ArraySegment<Vector2>(points, 0, number).Array;
        }

        public static Vector2 CalcClosestPoint(this Vector2 origin, Vector2[] points, out int index)
        {
            float closest = float.MaxValue;
            index = -1;

            for (int i = 1; i < points.Length; i++)
            {
                float dist = (points[i] - origin).Length();
                if (dist < closest)
                {
                    closest = dist;
                    index = i;
                }
            }

            return index != -1 ? points[index] : origin;
        }



        public static float CalcRadians(this Vector2 vec) => (float)(Math.Tan(vec.X / vec.Y) * -1);

        public static float CalcDegreesBetween(this Vec2 start, Vec2 end) => CalcRadians(start - end) * RadToDeg;

        public static bool IsInsideRect(this Vector2 point, Vector2 rectOrigin, Vector2 rectSize)
        {
            return point.X > rectOrigin.X && point.X < (rectOrigin + rectSize).X && point.Y > rectOrigin.Y &&
                   point.Y < (rectOrigin + rectSize).Y;
        }

        private static Vector2 _negateX = new Vector2(-1, 1);
        private static Vector2 _negateY = new Vector2(1, -1);

        public static Vector2 NegateX(this Vector2 self)
        {
            return self * _negateX;
        }

        public static Vector2 NegateY(this Vector2 self)
        {
            return self * _negateY;
        }

        public static Vector2 ZeroX(this Vector2 self)
        {
            return self * Vector2.UnitY;
        }

        public static Vector2 ZeroY(this Vector2 self)
        {
            return self * Vector2.UnitX;
        }

        public static Vector2 ReplaceX(this Vector2 self, float x)
        {
            return new Vector2(x, self.Y);
        }

        public static Vector2 ReplaceY(this Vector2 self, float y)
        {
            return new Vector2(self.X, y);
        }

        public static Vector2 SubtractX(this Vector2 self, float amount)
        {
            return new Vector2(self.X - amount, self.Y);
        }

        public static Vector2 SubtractY(this Vector2 self, float amount)
        {
            return new Vector2(self.X, self.Y - amount);
        }

        public static Vector2 AddX(this Vector2 self, float amount)
        {
            return new Vector2(self.X + amount, self.Y);
        }

        public static Vector2 AddY(this Vector2 self, float amount)
        {
            return new Vector2(self.X, self.Y + amount);
        }

        public static Vector2 MultiplyX(this Vector2 self, float amount)
        {
            return new Vector2(self.X * amount, self.Y);
        }

        public static Vector2 MultiplyY(this Vector2 self, float amount)
        {
            return new Vector2(self.X, self.Y * amount);
        }


        public static Vector2 XX(this Vector2 self)
        {
            return new Vector2(self.X, self.X);
        }

        public static Vector2 YY(this Vector2 self)
        {
            return new Vector2(self.Y, self.Y);
        }
    }
}