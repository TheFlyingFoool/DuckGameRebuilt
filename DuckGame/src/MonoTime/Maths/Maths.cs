using System;

namespace DuckGame
{
    public class Maths
    {
        public const float PI = 3.141593f;
        public static uint MaxFloatToInt = 16777216;

        public static float FramesToTravel(float distance, float acceleration, float startSpeed)
        {
            return ((float)Math.Sqrt(Math.Pow((double)(2f * startSpeed + acceleration), 2f) + (double)(8f * acceleration * distance)) - 2f * startSpeed - acceleration) / (2f * acceleration);
        }

        public static float DistanceTravelled(int frames, float acceleration, float startSpeed)
        {
            return 0.5f * acceleration * ((frames + 1) * frames) + frames * startSpeed;
        }

        public static float TicksToSeconds(int ticks)
        {
            return ticks / 60f;
        }

        public static int SecondsToTicks(float secs)
        {
            return (int)Math.Round((double)(secs * 60f));
        }

        public static float IncFrameTimer()
        {
            return 0.0166666666666666f;
        }

        public static Vec2 RoundToPixel(Vec2 pos)
        {
            pos.x = (float)Math.Round((double)(pos.x / 1f)) * 1f;
            pos.y = (float)Math.Round((double)(pos.y / 1f)) * 1f;
            return pos;
        }

        public static float FastSin(float rads)
        {
            if (rads < -3.14159265f)
                rads += 6.28318531f;
            else if (rads > 3.14159265f)
                rads -= 6.28318531f;
            if (rads < 0.0f)
                return 1.27323954f * rads + .405284735f * rads * rads;
            else
                return 1.27323954f * rads - 0.405284735f * rads * rads;
        }

        public static float FastCos(float rads)
        {
            if (rads < -3.14159265f)
                rads += 6.28318531f;
            else if (rads > 3.14159265f)
                rads -= 6.28318531f;

            rads += 1.57079632f;
            if (rads > 3.14159265f)
                rads -= 6.28318531f;

            if (rads < 0)
                return 1.27323954f * rads + 0.405284735f * rads * rads;
            else
                return 1.27323954f * rads - 0.405284735f * rads * rads;
        }

        public static float LerpTowards(float current, float to, float amount)
        {
            if (to > current)
            {
                current += amount;
                if (to < current)
                    current = to;
            }
            else if (to < current)
            {
                current -= amount;
                if (to > current)
                    current = to;
            }
            return current;
        }

        public static float Ratio(int val1, int val2)
        {
            if (val2 == 0f)
            {
                return val1;
            }
            return val1 / (float)val2;
        }

        public static float NormalizeSection(float value, float sectionMin, float sectionMax)
        {
            return Clamp(Clamp(value - sectionMin, 0f, sectionMax) / (sectionMax - sectionMin), 0f, 1f);
        }

        public static float CountDown(float value, float amount, float min = 0f)
        {
            if (value > min)
                value -= amount;
            else
                value = min;
            return value;
        }

        public static int CountDown(int value, int amount, int min = 0)
        {
            if (value > min)
                value -= amount;
            else
                value = min;
            return value;
        }

        public static float CountUp(float value, float amount, float max = 1f)
        {
            if (value < max)
                value += amount;
            else
                value = max;
            return value;
        }

        public static bool Intersects(Vec2 a1, Vec2 a2, Vec2 b1, Vec2 b2, out Vec2 intersection)
        {
            intersection = Vec2.Zero;
            Vec2 b3 = a2 - a1;
            Vec2 d = b2 - b1;
            float bDotDPerp = b3.x * d.y - b3.y * d.x;
            if (bDotDPerp == 0f)
            {
                return false;
            }
            Vec2 c = b1 - a1;
            float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
            if (t < 0f || t > 1f)
            {
                return false;
            }
            float u = (c.x * b3.y - c.y * b3.x) / bDotDPerp;
            if (u < 0f || u > 1f)
            {
                return false;
            }
            intersection = a1 + t * b3;
            return true;
        }

        public static float DegToRad(float deg)
        {
            return deg * ((float)Math.PI / 180f);
        }

        public static float RadToDeg(float rad)
        {
            return rad * (180.0f / (float)Math.PI);
        }

        public static float PointDirection(Vec2 p1, Vec2 p2)
        {
            return RadToDeg((float)Math.Atan2(p1.y - p2.y, p2.x - p1.x));
        }

        public static float PointDirectionRad(Vec2 p1, Vec2 p2)
        {
            return (float)Math.Atan2(p1.y - p2.y, p2.x - p1.x);
        }

        public static float PointDirection2(Vec2 p1, Vec2 p2)
        {
            return (float)Math.Atan2(p2.y, p2.x) - (float)Math.Atan2(p1.y, p1.x);
        }

        public static float PointDirection(float x1, float y1, float x2, float y2)
        {
            return RadToDeg((float)Math.Atan2(y1 - y2, x2 - x1));
        }

        public static float Clamp(float val, float min, float max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int Int(bool val)
        {
            return val ? 1 : 0;
        }

        public static Vec2 AngleToVec(float radians)
        {
            return new Vec2((float)Math.Cos(radians), (float)-Math.Sin(radians));
        }

        public static Vec2 Snap(Vec2 pPosition, float xSnap, float ySnap)
        {
            pPosition.x = (int)Math.Floor(pPosition.x / xSnap) * xSnap;
            pPosition.y = (int)Math.Floor(pPosition.y / ySnap) * ySnap;
            return pPosition;
        }

        public static Vec2 SnapRound(Vec2 pPosition, float xSnap, float ySnap)
        {
            pPosition.x = (int)Math.Round(pPosition.x / xSnap) * xSnap;
            pPosition.y = (int)Math.Round(pPosition.y / ySnap) * ySnap;
            return pPosition;
        }

        public static float Snap(float pValue, float pSnap)
        {
            pValue = (int)Math.Floor(pValue / pSnap) * pSnap;
            return pValue;
        }

        public static int Hash(string val)
        {
            byte[] dst = new byte[val.Length * sizeof(char)];
            Buffer.BlockCopy(val.ToCharArray(), 0, dst, 0, dst.Length);
            int length = dst.Length;
            if (length == 0)
                return 0;
            uint num1 = Convert.ToUInt32(length);
            int num2 = length & 3;
            int num3 = length >> 2;
            int startIndex = 0;
            for (; num3 > 0; --num3)
            {
                uint num4 = num1 + BitConverter.ToUInt16(dst, startIndex);
                uint num5 = (uint)BitConverter.ToUInt16(dst, startIndex + 2) << 11 ^ num4;
                uint num6 = num4 << 16 ^ num5;
                num1 = num6 + (num6 >> 11);
                startIndex += 4;
            }
            switch (num2)
            {
                case 1:
                    uint num7 = num1 + dst[startIndex];
                    uint num8 = num7 ^ num7 << 10;
                    num1 = num8 + (num8 >> 1);
                    break;
                case 2:
                    uint num9 = num1 + BitConverter.ToUInt16(dst, startIndex);
                    uint num10 = num9 ^ num9 << 11;
                    num1 = num10 + (num10 >> 17);
                    break;
                case 3:
                    uint num11 = num1 + BitConverter.ToUInt16(dst, startIndex);
                    uint num12 = num11 ^ num11 << 16 ^ (uint)dst[startIndex + 2] << 18;
                    num1 = num12 + (num12 >> 11);
                    break;
            }
            uint num13 = num1 ^ num1 << 3;
            uint num14 = num13 + (num13 >> 5);
            uint num15 = num14 ^ num14 << 4;
            uint num16 = num15 + (num15 >> 17);
            uint num17 = num16 ^ num16 << 25;
            return (int)(num17 + (num17 >> 6));
        }
    }
}
