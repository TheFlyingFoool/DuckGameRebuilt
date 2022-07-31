// Decompiled with JetBrains decompiler
// Type: DuckGame.Lerp
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public static class Lerp
    {
        public static float Float(float current, float to, float amount)
        {
            if ((double)to > (double)current)
            {
                current += amount;
                if ((double)to < (double)current)
                    current = to;
            }
            else if ((double)to < (double)current)
            {
                current -= amount;
                if ((double)to > (double)current)
                    current = to;
            }
            return current;
        }

        public static float FloatSmooth(float current, float to, float amount, float toMul = 1f)
        {
            float num1 = to - (1f - toMul) * to;
            if ((double)to < (double)current)
                num1 = to + (1f - toMul) * to;
            float num2 = current + amount * (num1 - current);
            if ((double)to >= (double)current && (double)num2 > (double)to || (double)to <= (double)current && (double)num2 < (double)to)
                num2 = to;
            return num2;
        }

        public static Vec2 Vec2(Vec2 current, Vec2 to, float amount)
        {
            Vec2 vec2_1 = current;
            Vec2 vec2_2 = to;
            Vec2 vec2_3 = vec2_2 - vec2_1;
            if ((double)vec2_3.Length() < 0.0001f)
                return current;
            vec2_3.Normalize();
            Vec2 vec2_4 = vec2_1 + vec2_3 * amount;
            if (vec2_2.x > (double)vec2_1.x && vec2_4.x > (double)vec2_2.x)
                vec2_4.x = vec2_2.x;
            if (vec2_2.x < (double)vec2_1.x && vec2_4.x < (double)vec2_2.x)
                vec2_4.x = vec2_2.x;
            if (vec2_2.y > (double)vec2_1.y && vec2_4.y > (double)vec2_2.y)
                vec2_4.y = vec2_2.y;
            if (vec2_2.y < (double)vec2_1.y && vec2_4.y < (double)vec2_2.y)
                vec2_4.y = vec2_2.y;
            return vec2_4;
        }

        public static Vec2 Vec2Smooth(Vec2 current, Vec2 to, float amount) => current + amount * (to - current);

        public static Vec2 Vec2Smooth(Vec2 current, Vec2 to, float amount, float thresh = 0f)
        {
            Vec2 vec2 = current + amount * (to - current);
            return (double)(vec2 - to).length < (double)thresh ? to : vec2;
        }

        public static T Generic<T>(T current, T to, float amount)
        {
            if ((object)current is Vec2)
                return (T)(object)(ValueType)Lerp.Vec2Smooth((Vec2)(object)current, (Vec2)(object)to, amount);
            return (object)current is float ? (T)(object)(ValueType)Lerp.FloatSmooth((float)(object)current, (float)(object)to, amount) : current;
        }

        public static Vec3 Vec3(Vec3 current, Vec3 to, float amount)
        {
            Vec3 vec3_1 = current;
            Vec3 vec3_2 = to;
            Vec3 vec3_3 = vec3_2 - vec3_1;
            if ((double)vec3_3.Length() < 0.0001f)
                return current;
            vec3_3.Normalize();
            Vec3 vec3_4 = vec3_1 + vec3_3 * amount;
            if (vec3_2.x > vec3_1.x && vec3_4.x > vec3_2.x)
                vec3_4.x = vec3_2.x;
            if (vec3_2.x < vec3_1.x && vec3_4.x < vec3_2.x)
                vec3_4.x = vec3_2.x;
            if (vec3_2.y > vec3_1.y && vec3_4.y > vec3_2.y)
                vec3_4.y = vec3_2.y;
            if (vec3_2.y < vec3_1.y && vec3_4.y < vec3_2.y)
                vec3_4.y = vec3_2.y;
            if (vec3_2.z > vec3_1.z && vec3_4.z > vec3_2.z)
                vec3_4.z = vec3_2.z;
            if (vec3_2.z < vec3_1.z && vec3_4.z < vec3_2.z)
                vec3_4.z = vec3_2.z;
            return vec3_4;
        }

        public static Color Color(Color current, Color to, float amount)
        {
            Vec4 vector4_1 = current.ToVector4();
            Vec4 vector4_2 = to.ToVector4();
            Vec4 vec4_1 = vector4_2 - vector4_1;
            if ((double)vec4_1.Length() < 0.0001f)
                return current;
            vec4_1.Normalize();
            Vec4 vec4_2 = vector4_1 + vec4_1 * amount;
            if (vector4_2.x > vector4_1.x && vec4_2.x > vector4_2.x)
                vec4_2.x = vector4_2.x;
            if (vector4_2.x < vector4_1.x && vec4_2.x < vector4_2.x)
                vec4_2.x = vector4_2.x;
            if (vector4_2.y > vector4_1.y && vec4_2.y > vector4_2.y)
                vec4_2.y = vector4_2.y;
            if (vector4_2.y < vector4_1.y && vec4_2.y < vector4_2.y)
                vec4_2.y = vector4_2.y;
            if (vector4_2.z > vector4_1.z && vec4_2.z > vector4_2.z)
                vec4_2.z = vector4_2.z;
            if (vector4_2.z < vector4_1.z && vec4_2.z < vector4_2.z)
                vec4_2.z = vector4_2.z;
            if (vector4_2.w > vector4_1.w && vec4_2.w > vector4_2.w)
                vec4_2.w = vector4_2.w;
            if (vector4_2.w < vector4_1.w && vec4_2.w < vector4_2.w)
                vec4_2.w = vector4_2.w;
            return new Color(vec4_2.x, vec4_2.y, vec4_2.z, vec4_2.w);
        }

        public static Color ColorSmooth(Color current, Color to, float amount)
        {
            Vec4 vector4_1 = current.ToVector4();
            Vec4 vector4_2 = to.ToVector4();
            Vec4 vec4 = vector4_1 + (vector4_2 - vector4_1) * amount;
            return new Color(vec4.x, vec4.y, vec4.z, vec4.w);
        }

        public static Color ColorSmoothNoAlpha(Color current, Color to, float amount)
        {
            Vec4 vector4_1 = current.ToVector4();
            Vec4 vector4_2 = to.ToVector4();
            Vec4 vec4 = (vector4_1 + (vector4_2 - vector4_1) * amount);
            vec4.w = 1f;
            return new Color(vec4.x, vec4.y, vec4.z, vec4.w);
        }
    }
}
