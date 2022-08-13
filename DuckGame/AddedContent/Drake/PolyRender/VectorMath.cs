using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.PolyRender;

public class VectorMath
{
        public const float DegToRad = (float)0.0174533;

        public const float RadToDeg = (float)57.2958;

        public static Vector2 CalcHitPoint(Vector2 start, Vector2 end, Thing thing)
        {
            Vector2[] intersects = {
            CalcIntersection(start, end, thing.topLeft, thing.bottomLeft),
            CalcIntersection(start, end, thing.topLeft, thing.topRight),
            CalcIntersection(start, end, thing.topRight, thing.bottomRight),
            CalcIntersection(start, end, thing.bottomRight, thing.bottomLeft)
            };

            var nearest = end;
            float distance = (end - start).Length();

            foreach (var vec in intersects)
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

            var intersect = new Vec2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

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
            float dist1 = Dist(s, point);
            float dist2 = Dist(e, point);
            float dist3 = Dist(s, e);
            float dist4 = dist1 + dist2;

            return dist4 - dist3 <= accuracy;
        }

        public static float Dist(Vector2 s, Vector2 e) => (s - e).Length();
        public static float DistSqr(Vector2 s, Vector2 e) => (s - e).LengthSquared();
        public static Vector2 CalcPerpCw(Vec2 vec) => new Vec2(vec.y, -vec.x);

        public static Vector2 CalcPerpCcw(Vec2 vec) => new Vec2(-vec.y, vec.x);

        public static Vector2 CalcPerpCw(Vec2 start, Vec2 end) => CalcPerpCw(end - start);

        public static Vector2 CalcPerpCcw(Vec2 start, Vec2 end) => CalcPerpCcw(end - start);

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


        public static Vector2[] CalcClosestPoints(Vector2[] points, Vector2 origin, int number)
        {
            if (number > points.Length)
            {
                throw new Exception();
            }

            points = points.OrderBy(x => -DistSqr(x, origin)).ToArray();
            points = points.Reverse().ToArray();

            return new ArraySegment<Vector2>(points, 0, number).Array;
        }

        public static Vector2 CalcClosestPoint(Vector2[] points, Vector2 origin, out int index)
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

        public static float CalcRadians(Vector2 vec) => (float)(Math.Tan(vec.X / vec.Y) * -1);

        public static float CalcDegreesBetween(Vec2 start, Vec2 end) => CalcRadians(start - end) * RadToDeg;

        public static double Cbrt(double d) => Math.Pow(d, 1f / 3f);
}