// Decompiled with JetBrains decompiler
// Type: DuckGame.Maths
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Maths
    {
        public const float PI = 3.141593f;
        public static uint MaxFloatToInt = 16777216;

        public static float FramesToTravel(float distance, float acceleration, float startSpeed) => (float)((Math.Sqrt(Math.Pow(2.0 * startSpeed + acceleration, 2.0) + 8.0 * acceleration * distance) - 2.0 * startSpeed - acceleration) / (2.0 * acceleration));

        public static float DistanceTravelled(int frames, float acceleration, float startSpeed) => (float)(0.5 * acceleration * ((frames + 1) * frames) + frames * startSpeed);

        public static float TicksToSeconds(int ticks) => ticks / 60f;

        public static int SecondsToTicks(float secs) => (int)Math.Round(secs * 60.0);

        public static float IncFrameTimer() => 0.01666667f;

        public static Vec2 RoundToPixel(Vec2 pos)
        {
            pos.x = (float)Math.Round(pos.x / 1.0) * 1f;
            pos.y = (float)Math.Round(pos.y / 1.0) * 1f;
            return pos;
        }

        public static float FastSin(float rads)
        {
            if (rads < -3.14159274101257)
                rads += 6.283185f;
            else if (rads > 3.14159274101257)
                rads -= 6.283185f;
            return rads < 0.0 ? (float)(1.27323949337006 * rads + 0.405284732580185 * rads * rads) : (float)(1.27323949337006 * rads - 0.405284732580185 * rads * rads);
        }

        public static float FastCos(float rads)
        {
            if (rads < -3.14159274101257)
                rads += 6.283185f;
            else if (rads > 3.14159274101257)
                rads -= 6.283185f;
            rads += 1.570796f;
            if (rads > 3.14159274101257)
                rads -= 6.283185f;
            return rads < 0.0 ? (float)(1.27323949337006 * rads + 0.405284732580185 * rads * rads) : (float)(1.27323949337006 * rads - 0.405284732580185 * rads * rads);
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

        public static float Ratio(int val1, int val2) => val2 == 0.0 ? val1 : val1 / (float)val2;

        public static float NormalizeSection(float value, float sectionMin, float sectionMax) => Maths.Clamp(Maths.Clamp(value - sectionMin, 0f, sectionMax) / (sectionMax - sectionMin), 0f, 1f);

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
            Vec2 vec2_1 = a2 - a1;
            Vec2 vec2_2 = b2 - b1;
            float num1 = (float)(vec2_1.x * vec2_2.y - vec2_1.y * vec2_2.x);
            if (num1 == 0.0)
                return false;
            Vec2 vec2_3 = b1 - a1;
            float num2 = (float)(vec2_3.x * vec2_2.y - vec2_3.y * vec2_2.x) / num1;
            if (num2 < 0.0 || num2 > 1.0)
                return false;
            float num3 = (float)(vec2_3.x * vec2_1.y - vec2_3.y * vec2_1.x) / num1;
            if (num3 < 0.0 || num3 > 1.0)
                return false;
            intersection = a1 + num2 * vec2_1;
            return true;
        }

        public static float DegToRad(float deg) => deg * ((float)Math.PI / 180f);

        public static float RadToDeg(float rad) => rad * 57.29578f;

        public static float PointDirection(Vec2 p1, Vec2 p2) => Maths.RadToDeg((float)Math.Atan2(p1.y - p2.y, p2.x - p1.x));

        public static float PointDirectionRad(Vec2 p1, Vec2 p2) => (float)Math.Atan2(p1.y - p2.y, p2.x - p1.x);

        public static float PointDirection2(Vec2 p1, Vec2 p2) => (float)Math.Atan2(p2.y, p2.x) - (float)Math.Atan2(p1.y, p1.x);

        public static float PointDirection(float x1, float y1, float x2, float y2) => Maths.RadToDeg((float)Math.Atan2(y1 - y2, x2 - x1));

        public static float Clamp(float val, float min, float max) => Math.Min(Math.Max(val, min), max);

        public static int Clamp(int val, int min, int max) => Math.Min(Math.Max(val, min), max);

        public static int Int(bool val) => !val ? 0 : 1;

        public static Vec2 AngleToVec(float radians) => new Vec2((float)Math.Cos(radians), (float)-Math.Sin(radians));

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
            byte[] dst = new byte[val.Length * 2];
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
