using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace DuckGame
{
    [Serializable]
    public struct Vec3 : IEquatable<Vec3>
    {
        private static Vec3 zero = new Vec3(0f, 0f, 0f);
        private static Vec3 one = new Vec3(1f, 1f, 1f);
        private static Vec3 unitX = new Vec3(1f, 0f, 0f);
        private static Vec3 unitY = new Vec3(0f, 1f, 0f);
        private static Vec3 unitZ = new Vec3(0f, 0f, 1f);
        private static Vec3 up = new Vec3(0f, 1f, 0f);
        private static Vec3 down = new Vec3(0f, -1f, 0f);
        private static Vec3 right = new Vec3(1f, 0f, 0f);
        private static Vec3 left = new Vec3(-1f, 0f, 0f);
        private static Vec3 forward = new Vec3(0f, 0f, -1f);
        private static Vec3 backward = new Vec3(0f, 0f, 1f);
        public float x;
        public float y;
        public float z;

        public static Vec3 Zero => zero;

        public static Vec3 One => one;

        public static Vec3 UnitX => unitX;

        public static Vec3 UnitY => unitY;

        public static Vec3 UnitZ => unitZ;

        public static Vec3 Up => up;

        public static Vec3 Down => down;

        public static Vec3 Right => right;

        public static Vec3 Left => left;

        public static Vec3 Forward => forward;

        public static Vec3 Backward => backward;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(float value)
        {
            x = value;
            y = value;
            z = value;
        }

        public Vec3(Vec2 value, float z)
        {
            x = value.x;
            y = value.y;
            this.z = z;
        }

        public static Vec3 Add(Vec3 value1, Vec3 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            value1.z += value2.z;
            return value1;
        }

        public static void Add(ref Vec3 value1, ref Vec3 value2, out Vec3 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
            result.z = value1.z + value2.z;
        }

        public static Vec3 Barycentric(
          Vec3 value1,
          Vec3 value2,
          Vec3 value3,
          float amount1,
          float amount2)
        {
            return new Vec3(MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2), MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2), MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2));
        }

        public static void Barycentric(
          ref Vec3 value1,
          ref Vec3 value2,
          ref Vec3 value3,
          float amount1,
          float amount2,
          out Vec3 result)
        {
            result = new Vec3(MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2), MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2), MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2));
        }

        public static Vec3 CatmullRom(
          Vec3 value1,
          Vec3 value2,
          Vec3 value3,
          Vec3 value4,
          float amount)
        {
            return new Vec3(MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount), MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount), MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount));
        }

        public static void CatmullRom(
          ref Vec3 value1,
          ref Vec3 value2,
          ref Vec3 value3,
          ref Vec3 value4,
          float amount,
          out Vec3 result)
        {
            result = new Vec3(MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount), MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount), MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount));
        }

        public static Vec3 Clamp(Vec3 value1, Vec3 min, Vec3 max) => new Vec3(MathHelper.Clamp(value1.x, min.x, max.x), MathHelper.Clamp(value1.y, min.y, max.y), MathHelper.Clamp(value1.z, min.z, max.z));

        public static void Clamp(ref Vec3 value1, ref Vec3 min, ref Vec3 max, out Vec3 result) => result = new Vec3(MathHelper.Clamp(value1.x, min.x, max.x), MathHelper.Clamp(value1.y, min.y, max.y), MathHelper.Clamp(value1.z, min.z, max.z));

        public static Vec3 Cross(Vec3 vector1, Vec3 Vec2)
        {
            Vec3 vec3;
            vec3.x = vector1.y * Vec2.z - Vec2.y * vector1.z;
            vec3.y = Vec2.x * vector1.z - vector1.x * Vec2.z;
            vec3.z = vector1.x * Vec2.y - Vec2.x * vector1.y;
            return vec3;
        }

        public static void Cross(ref Vec3 vector1, ref Vec3 Vec2, out Vec3 result)
        {
            result.x = vector1.y * Vec2.z - Vec2.y * vector1.z;
            result.y = Vec2.x * vector1.z - vector1.x * Vec2.z;
            result.z = vector1.x * Vec2.y - Vec2.x * vector1.y;
        }

        public static float Distance(Vec3 value1, Vec3 value2) => (float)Math.Sqrt((value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y) + (value1.z - value2.z) * (value1.z - value2.z));

        public static void Distance(ref Vec3 value1, ref Vec3 value2, out float result) => result = (float)Math.Sqrt((value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y) + (value1.z - value2.z) * (value1.z - value2.z));

        public static float DistanceSquared(Vec3 value1, Vec3 value2) => (float)((value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y) + (value1.z - value2.z) * (value1.z - value2.z));

        public static void DistanceSquared(ref Vec3 value1, ref Vec3 value2, out float result) => result = (float)((value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y) + (value1.z - value2.z) * (value1.z - value2.z));

        public static Vec3 Divide(Vec3 value1, Vec3 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            value1.z /= value2.z;
            return value1;
        }

        public static Vec3 Divide(Vec3 value1, float value2)
        {
            float num = 1f / value2;
            value1.x *= num;
            value1.y *= num;
            value1.z *= num;
            return value1;
        }

        public static void Divide(ref Vec3 value1, float divisor, out Vec3 result)
        {
            float num = 1f / divisor;
            result.x = value1.x * num;
            result.y = value1.y * num;
            result.z = value1.z * num;
        }

        public static void Divide(ref Vec3 value1, ref Vec3 value2, out Vec3 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
            result.z = value1.z / value2.z;
        }

        public static float Dot(Vec3 vector1, Vec3 Vec2) => (float)(vector1.x * Vec2.x + vector1.y * Vec2.y + vector1.z * Vec2.z);

        public static void Dot(ref Vec3 vector1, ref Vec3 Vec2, out float result) => result = (float)(vector1.x * Vec2.x + vector1.y * Vec2.y + vector1.z * Vec2.z);

        public override bool Equals(object obj) => obj is Vec3 vec3 && this == vec3;

        public bool Equals(Vec3 other) => this == other;

        public override int GetHashCode() => (int)(x + y + z);

        public static Vec3 Hermite(
          Vec3 value1,
          Vec3 tangent1,
          Vec3 value2,
          Vec3 tangent2,
          float amount)
        {
            value1.x = MathHelper.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
            value1.y = MathHelper.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
            value1.z = MathHelper.Hermite(value1.z, tangent1.z, value2.z, tangent2.z, amount);
            return value1;
        }

        public static void Hermite(
          ref Vec3 value1,
          ref Vec3 tangent1,
          ref Vec3 value2,
          ref Vec3 tangent2,
          float amount,
          out Vec3 result)
        {
            result.x = MathHelper.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
            result.y = MathHelper.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
            result.z = MathHelper.Hermite(value1.z, tangent1.z, value2.z, tangent2.z, amount);
        }

        public float Length() => (float)Math.Sqrt(x * x + y * y + z * z);

        public float LengthSquared() => (float)(x * x + y * y + z * z);

        public static Vec3 Lerp(Vec3 value1, Vec3 value2, float amount) => new Vec3(MathHelper.Lerp(value1.x, value2.x, amount), MathHelper.Lerp(value1.y, value2.y, amount), MathHelper.Lerp(value1.z, value2.z, amount));

        public static void Lerp(ref Vec3 value1, ref Vec3 value2, float amount, out Vec3 result) => result = new Vec3(MathHelper.Lerp(value1.x, value2.x, amount), MathHelper.Lerp(value1.y, value2.y, amount), MathHelper.Lerp(value1.z, value2.z, amount));

        public static Vec3 Max(Vec3 value1, Vec3 value2) => new Vec3(MathHelper.Max(value1.x, value2.x), MathHelper.Max(value1.y, value2.y), MathHelper.Max(value1.z, value2.z));

        public static void Max(ref Vec3 value1, ref Vec3 value2, out Vec3 result) => result = new Vec3(MathHelper.Max(value1.x, value2.x), MathHelper.Max(value1.y, value2.y), MathHelper.Max(value1.z, value2.z));

        public static Vec3 Min(Vec3 value1, Vec3 value2) => new Vec3(MathHelper.Min(value1.x, value2.x), MathHelper.Min(value1.y, value2.y), MathHelper.Min(value1.z, value2.z));

        public static void Min(ref Vec3 value1, ref Vec3 value2, out Vec3 result) => result = new Vec3(MathHelper.Min(value1.x, value2.x), MathHelper.Min(value1.y, value2.y), MathHelper.Min(value1.z, value2.z));

        public static Vec3 Multiply(Vec3 value1, Vec3 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            value1.z *= value2.z;
            return value1;
        }

        public static Vec3 Multiply(Vec3 value1, float scaleFactor)
        {
            value1.x *= scaleFactor;
            value1.y *= scaleFactor;
            value1.z *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vec3 value1, float scaleFactor, out Vec3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }

        public static void Multiply(ref Vec3 value1, ref Vec3 value2, out Vec3 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
            result.z = value1.z * value2.z;
        }

        public static Vec3 Negate(Vec3 value)
        {
            value.x = -value.x;
            value.y = -value.y;
            value.z = -value.z;
            return value;
        }

        public static void Negate(ref Vec3 value, out Vec3 result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
        }

        public void Normalize()
        {
            float num = 1f / (float)Math.Sqrt(x * x + y * y + z * z);
            x *= num;
            y *= num;
            z *= num;
        }

        public static Vec3 Normalize(Vec3 value)
        {
            float num = 1f / (float)Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
            value.x *= num;
            value.y *= num;
            value.z *= num;
            return value;
        }

        public static void Normalize(ref Vec3 value, out Vec3 result)
        {
            float num = 1f / (float)Math.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }

        public static Vec3 Reflect(Vec3 vector, Vec3 normal)
        {
            float num = 2f * Dot(vector, normal);
            vector.x -= num * normal.x;
            vector.y -= num * normal.y;
            vector.z -= num * normal.z;
            return vector;
        }

        public static void Reflect(ref Vec3 vector, ref Vec3 normal, out Vec3 result)
        {
            float num = 2f * Dot(vector, normal);
            result.x = vector.x - num * normal.x;
            result.y = vector.y - num * normal.y;
            result.z = vector.z - num * normal.z;
        }

        public static Vec3 SmoothStep(Vec3 value1, Vec3 value2, float amount) => new Vec3(MathHelper.SmoothStep(value1.x, value2.x, amount), MathHelper.SmoothStep(value1.y, value2.y, amount), MathHelper.SmoothStep(value1.z, value2.z, amount));

        public static void SmoothStep(ref Vec3 value1, ref Vec3 value2, float amount, out Vec3 result) => result = new Vec3(MathHelper.SmoothStep(value1.x, value2.x, amount), MathHelper.SmoothStep(value1.y, value2.y, amount), MathHelper.SmoothStep(value1.z, value2.z, amount));

        public static Vec3 Subtract(Vec3 value1, Vec3 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            value1.z -= value2.z;
            return value1;
        }

        public static void Subtract(ref Vec3 value1, ref Vec3 value2, out Vec3 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
            result.z = value1.z - value2.z;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append("{X:");
            stringBuilder.Append(x);
            stringBuilder.Append(" Y:");
            stringBuilder.Append(y);
            stringBuilder.Append(" Z:");
            stringBuilder.Append(z);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public static Vec3 Transform(Vec3 position, Matrix matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vec3 position, ref Matrix matrix, out Vec3 result) => result = new Vec3((float)(position.x * matrix.M11 + position.y * matrix.M21 + position.z * matrix.M31) + matrix.M41, (float)(position.x * matrix.M12 + position.y * matrix.M22 + position.z * matrix.M32) + matrix.M42, (float)(position.x * matrix.M13 + position.y * matrix.M23 + position.z * matrix.M33) + matrix.M43);

        public static Vec3 Transform(Vec3 value, Quaternion rotation) => throw new NotImplementedException();

        public static void Transform(Vec3[] sourceArray, ref Matrix matrix, Vec3[] destinationArray) => throw new NotImplementedException();

        public static void Transform(
          Vec3[] sourceArray,
          ref Quaternion rotation,
          Vec3[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void Transform(
          Vec3[] sourceArray,
          int sourceIndex,
          ref Matrix matrix,
          Vec3[] destinationArray,
          int destinationIndex,
          int length)
        {
            throw new NotImplementedException();
        }

        public static void Transform(
          Vec3[] sourceArray,
          int sourceIndex,
          ref Quaternion rotation,
          Vec3[] destinationArray,
          int destinationIndex,
          int length)
        {
            throw new NotImplementedException();
        }

        public static void Transform(ref Vec3 value, ref Quaternion rotation, out Vec3 result) => throw new NotImplementedException();

        public static void TransformNormal(
          Vec3[] sourceArray,
          ref Matrix matrix,
          Vec3[] destinationArray)
        {
            throw new NotImplementedException();
        }

        public static void TransformNormal(
          Vec3[] sourceArray,
          int sourceIndex,
          ref Matrix matrix,
          Vec3[] destinationArray,
          int destinationIndex,
          int length)
        {
            throw new NotImplementedException();
        }

        public static Vec3 TransformNormal(Vec3 normal, Matrix matrix)
        {
            TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        public static void TransformNormal(ref Vec3 normal, ref Matrix matrix, out Vec3 result) => result = new Vec3((float)(normal.x * matrix.M11 + normal.y * matrix.M21 + normal.z * matrix.M31), (float)(normal.x * matrix.M12 + normal.y * matrix.M22 + normal.z * matrix.M32), (float)(normal.x * matrix.M13 + normal.y * matrix.M23 + normal.z * matrix.M33));

        public static bool operator ==(Vec3 value1, Vec3 value2) => value1.x == value2.x && value1.y == value2.y && value1.z == value2.z;

        public static bool operator !=(Vec3 value1, Vec3 value2) => value1.x != value2.x || value1.y != value2.y || value1.z != value2.z;

        public static Vec3 operator +(Vec3 value1, Vec3 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            value1.z += value2.z;
            return value1;
        }

        public static Vec3 operator -(Vec3 value)
        {
            value = new Vec3(-value.x, -value.y, -value.z);
            return value;
        }

        public static Vec3 operator -(Vec3 value1, Vec3 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            value1.z -= value2.z;
            return value1;
        }

        public static Vec3 operator *(Vec3 value1, Vec3 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            value1.z *= value2.z;
            return value1;
        }

        public static Vec3 operator *(Vec3 value, float scaleFactor)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            value.z *= scaleFactor;
            return value;
        }

        public static Vec3 operator *(float scaleFactor, Vec3 value)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            value.z *= scaleFactor;
            return value;
        }

        public static Vec3 operator /(Vec3 value1, Vec3 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            value1.z /= value2.z;
            return value1;
        }

        public static Vec3 operator /(Vec3 value, float divider)
        {
            float num = 1f / divider;
            value.x *= num;
            value.y *= num;
            value.z *= num;
            return value;
        }

        public Color ToColor() => new Color((int)x, (int)y, (int)z);

        public static implicit operator Vector3(Vec3 vec) => new Vector3(vec.x, vec.y, vec.z);

        public static implicit operator Vec3(Vector3 vec) => new Vec3(vec.X, vec.Y, vec.Z);
    }
}
