// Decompiled with JetBrains decompiler
// Type: DuckGame.Quaternion
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Text;

namespace DuckGame
{
    [Serializable]
    public struct Quaternion : IEquatable<Quaternion>
    {
        public float x;
        public float y;
        public float z;
        public float w;
        private static Quaternion identity = new Quaternion(0f, 0f, 0f, 1f);

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vec3 vectorPart, float scalarPart)
        {
            x = vectorPart.x;
            y = vectorPart.y;
            z = vectorPart.z;
            w = scalarPart;
        }

        public static Quaternion Identity => Quaternion.identity;

        public static Quaternion Add(Quaternion quaternion1, Quaternion quaternion2)
        {
            quaternion1.x += quaternion2.x;
            quaternion1.y += quaternion2.y;
            quaternion1.z += quaternion2.z;
            quaternion1.w += quaternion2.w;
            return quaternion1;
        }

        public static void Add(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          out Quaternion result)
        {
            result.w = quaternion1.w + quaternion2.w;
            result.x = quaternion1.x + quaternion2.x;
            result.y = quaternion1.y + quaternion2.y;
            result.z = quaternion1.z + quaternion2.z;
        }

        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            Quaternion quaternion;
            quaternion.x = (float)(value2.x * value1.w + value1.x * value2.w + value2.y * value1.z - value2.z * value1.y);
            quaternion.y = (float)(value2.y * value1.w + value1.y * value2.w + value2.z * value1.x - value2.x * value1.z);
            quaternion.z = (float)(value2.z * value1.w + value1.z * value2.w + value2.x * value1.y - value2.y * value1.x);
            quaternion.w = (float)(value2.w * value1.w - (value2.x * value1.x + value2.y * value1.y) + value2.z * value1.z);
            return quaternion;
        }

        public void Conjugate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public static Quaternion Conjugate(Quaternion value)
        {
            Quaternion quaternion;
            quaternion.x = -value.x;
            quaternion.y = -value.y;
            quaternion.z = -value.z;
            quaternion.w = value.w;
            return quaternion;
        }

        public static void Conjugate(ref Quaternion value, out Quaternion result)
        {
            result.x = -value.x;
            result.y = -value.y;
            result.z = -value.z;
            result.w = value.w;
        }

        public static void Concatenate(
          ref Quaternion value1,
          ref Quaternion value2,
          out Quaternion result)
        {
            result.x = (float)(value2.x * value1.w + value1.x * value2.w + value2.y * value1.z - value2.z * value1.y);
            result.y = (float)(value2.y * value1.w + value1.y * value2.w + value2.z * value1.x - value2.x * value1.z);
            result.z = (float)(value2.z * value1.w + value1.z * value2.w + value2.x * value1.y - value2.y * value1.x);
            result.w = (float)(value2.w * value1.w - (value2.x * value1.x + value2.y * value1.y) + value2.z * value1.z);
        }

        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion fromYawPitchRoll;
            fromYawPitchRoll.x = (float)(Math.Cos(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Cos(roll * 0.5) + Math.Sin(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Sin(roll * 0.5));
            fromYawPitchRoll.y = (float)(Math.Sin(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Cos(roll * 0.5) - Math.Cos(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Sin(roll * 0.5));
            fromYawPitchRoll.z = (float)(Math.Cos(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Sin(roll * 0.5) - Math.Sin(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Cos(roll * 0.5));
            fromYawPitchRoll.w = (float)(Math.Cos(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Cos(roll * 0.5) + Math.Sin(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Sin(roll * 0.5));
            return fromYawPitchRoll;
        }

        public static void CreateFromYawPitchRoll(
          float yaw,
          float pitch,
          float roll,
          out Quaternion result)
        {
            result.x = (float)(Math.Cos(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Cos(roll * 0.5) + Math.Sin(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Sin(roll * 0.5));
            result.y = (float)(Math.Sin(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Cos(roll * 0.5) - Math.Cos(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Sin(roll * 0.5));
            result.z = (float)(Math.Cos(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Sin(roll * 0.5) - Math.Sin(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Cos(roll * 0.5));
            result.w = (float)(Math.Cos(yaw * 0.5) * Math.Cos(pitch * 0.5) * Math.Cos(roll * 0.5) + Math.Sin(yaw * 0.5) * Math.Sin(pitch * 0.5) * Math.Sin(roll * 0.5));
        }

        public static Quaternion CreateFromAxisAngle(Vec3 axis, float angle)
        {
            float num = (float)Math.Sin(angle / 2.0);
            return new Quaternion(axis.x * num, axis.y * num, axis.z * num, (float)Math.Cos(angle / 2.0));
        }

        public static void CreateFromAxisAngle(ref Vec3 axis, float angle, out Quaternion result)
        {
            float num = (float)Math.Sin(angle / 2.0);
            result.x = axis.x * num;
            result.y = axis.y * num;
            result.z = axis.z * num;
            result.w = (float)Math.Cos(angle / 2.0);
        }

        public static Quaternion CreateFromRotationMatrix(Matrix matrix)
        {
            if (matrix.M11 + matrix.M22 + matrix.M33 > 0.0)
            {
                float num1 = (float)Math.Sqrt(matrix.M11 + matrix.M22 + matrix.M33 + 1.0);
                Quaternion fromRotationMatrix;
                fromRotationMatrix.w = num1 * 0.5f;
                float num2 = 0.5f / num1;
                fromRotationMatrix.x = (matrix.M23 - matrix.M32) * num2;
                fromRotationMatrix.y = (matrix.M31 - matrix.M13) * num2;
                fromRotationMatrix.z = (matrix.M12 - matrix.M21) * num2;
                return fromRotationMatrix;
            }
            if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
            {
                float num3 = (float)Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
                float num4 = 0.5f / num3;
                Quaternion fromRotationMatrix;
                fromRotationMatrix.x = 0.5f * num3;
                fromRotationMatrix.y = (matrix.M12 + matrix.M21) * num4;
                fromRotationMatrix.z = (matrix.M13 + matrix.M31) * num4;
                fromRotationMatrix.w = (matrix.M23 - matrix.M32) * num4;
                return fromRotationMatrix;
            }
            if (matrix.M22 > matrix.M33)
            {
                float num5 = (float)Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
                float num6 = 0.5f / num5;
                Quaternion fromRotationMatrix;
                fromRotationMatrix.x = (matrix.M21 + matrix.M12) * num6;
                fromRotationMatrix.y = 0.5f * num5;
                fromRotationMatrix.z = (matrix.M32 + matrix.M23) * num6;
                fromRotationMatrix.w = (matrix.M31 - matrix.M13) * num6;
                return fromRotationMatrix;
            }
            float num7 = (float)Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
            float num8 = 0.5f / num7;
            Quaternion fromRotationMatrix1;
            fromRotationMatrix1.x = (matrix.M31 + matrix.M13) * num8;
            fromRotationMatrix1.y = (matrix.M32 + matrix.M23) * num8;
            fromRotationMatrix1.z = 0.5f * num7;
            fromRotationMatrix1.w = (matrix.M12 - matrix.M21) * num8;
            return fromRotationMatrix1;
        }

        public static void CreateFromRotationMatrix(ref Matrix matrix, out Quaternion result)
        {
            if (matrix.M11 + matrix.M22 + matrix.M33 > 0.0)
            {
                float num1 = (float)Math.Sqrt(matrix.M11 + matrix.M22 + matrix.M33 + 1.0);
                result.w = num1 * 0.5f;
                float num2 = 0.5f / num1;
                result.x = (matrix.M23 - matrix.M32) * num2;
                result.y = (matrix.M31 - matrix.M13) * num2;
                result.z = (matrix.M12 - matrix.M21) * num2;
            }
            else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
            {
                float num3 = (float)Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
                float num4 = 0.5f / num3;
                result.x = 0.5f * num3;
                result.y = (matrix.M12 + matrix.M21) * num4;
                result.z = (matrix.M13 + matrix.M31) * num4;
                result.w = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                float num5 = (float)Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
                float num6 = 0.5f / num5;
                result.x = (matrix.M21 + matrix.M12) * num6;
                result.y = 0.5f * num5;
                result.z = (matrix.M32 + matrix.M23) * num6;
                result.w = (matrix.M31 - matrix.M13) * num6;
            }
            else
            {
                float num7 = (float)Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
                float num8 = 0.5f / num7;
                result.x = (matrix.M31 + matrix.M13) * num8;
                result.y = (matrix.M32 + matrix.M23) * num8;
                result.z = 0.5f * num7;
                result.w = (matrix.M12 - matrix.M21) * num8;
            }
        }

        public static Quaternion Divide(Quaternion quaternion1, Quaternion quaternion2)
        {
            float num1 = (float)(1.0 / (quaternion2.x * quaternion2.x + quaternion2.y * quaternion2.y + quaternion2.z * quaternion2.z + quaternion2.w * quaternion2.w));
            float num2 = -quaternion2.x * num1;
            float num3 = -quaternion2.y * num1;
            float num4 = -quaternion2.z * num1;
            float num5 = quaternion2.w * num1;
            Quaternion quaternion;
            quaternion.x = (float)(quaternion1.x * num5 + num2 * quaternion1.w + (quaternion1.y * num4 - quaternion1.z * num3));
            quaternion.y = (float)(quaternion1.y * num5 + num3 * quaternion1.w + (quaternion1.z * num2 - quaternion1.x * num4));
            quaternion.z = (float)(quaternion1.z * num5 + num4 * quaternion1.w + (quaternion1.x * num3 - quaternion1.y * num2));
            quaternion.w = (float)(quaternion1.w * quaternion2.w * num1 - (quaternion1.x * num2 + quaternion1.y * num3 + quaternion1.z * num4));
            return quaternion;
        }

        public static void Divide(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          out Quaternion result)
        {
            float num1 = (float)(1.0 / (quaternion2.x * quaternion2.x + quaternion2.y * quaternion2.y + quaternion2.z * quaternion2.z + quaternion2.w * quaternion2.w));
            float num2 = -quaternion2.x * num1;
            float num3 = -quaternion2.y * num1;
            float num4 = -quaternion2.z * num1;
            float num5 = quaternion2.w * num1;
            result.x = (float)(quaternion1.x * num5 + num2 * quaternion1.w + (quaternion1.y * num4 - quaternion1.z * num3));
            result.y = (float)(quaternion1.y * num5 + num3 * quaternion1.w + (quaternion1.z * num2 - quaternion1.x * num4));
            result.z = (float)(quaternion1.z * num5 + num4 * quaternion1.w + (quaternion1.x * num3 - quaternion1.y * num2));
            result.w = (float)(quaternion1.w * quaternion2.w * num1 - (quaternion1.x * num2 + quaternion1.y * num3 + quaternion1.z * num4));
        }

        public static float Dot(Quaternion quaternion1, Quaternion quaternion2) => (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w);

        public static void Dot(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          out float result)
        {
            result = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w);
        }

        public override bool Equals(object obj) => obj is Quaternion quaternion && this == quaternion;

        public bool Equals(Quaternion other) => x == other.x && y == other.y && z == other.z && w == other.w;

        public override int GetHashCode() => x.GetHashCode() + y.GetHashCode() + z.GetHashCode() + w.GetHashCode();

        public static Quaternion Inverse(Quaternion quaternion)
        {
            float num = (float)(1.0 / (quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w));
            Quaternion quaternion1;
            quaternion1.x = -quaternion.x * num;
            quaternion1.y = -quaternion.y * num;
            quaternion1.z = -quaternion.z * num;
            quaternion1.w = quaternion.w * num;
            return quaternion1;
        }

        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            float num = (float)(1.0 / (quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w));
            result.x = -quaternion.x * num;
            result.y = -quaternion.y * num;
            result.z = -quaternion.z * num;
            result.w = quaternion.w * num;
        }

        public float Length() => (float)Math.Sqrt(x * x + y * y + z * z + w * w);

        public float LengthSquared() => (float)(x * x + y * y + z * z + w * w);

        public static Quaternion Lerp(
          Quaternion quaternion1,
          Quaternion quaternion2,
          float amount)
        {
            float num1 = 1f - amount;
            Quaternion quaternion;
            if (quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w >= 0.0)
            {
                quaternion.x = (float)(num1 * quaternion1.x + amount * quaternion2.x);
                quaternion.y = (float)(num1 * quaternion1.y + amount * quaternion2.y);
                quaternion.z = (float)(num1 * quaternion1.z + amount * quaternion2.z);
                quaternion.w = (float)(num1 * quaternion1.w + amount * quaternion2.w);
            }
            else
            {
                quaternion.x = (float)(num1 * quaternion1.x - amount * quaternion2.x);
                quaternion.y = (float)(num1 * quaternion1.y - amount * quaternion2.y);
                quaternion.z = (float)(num1 * quaternion1.z - amount * quaternion2.z);
                quaternion.w = (float)(num1 * quaternion1.w - amount * quaternion2.w);
            }
            float num2 = 1f / (float)Math.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
            quaternion.x *= num2;
            quaternion.y *= num2;
            quaternion.z *= num2;
            quaternion.w *= num2;
            return quaternion;
        }

        public static void Lerp(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          float amount,
          out Quaternion result)
        {
            float num1 = 1f - amount;
            if (quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w >= 0.0)
            {
                result.x = (float)(num1 * quaternion1.x + amount * quaternion2.x);
                result.y = (float)(num1 * quaternion1.y + amount * quaternion2.y);
                result.z = (float)(num1 * quaternion1.z + amount * quaternion2.z);
                result.w = (float)(num1 * quaternion1.w + amount * quaternion2.w);
            }
            else
            {
                result.x = (float)(num1 * quaternion1.x - amount * quaternion2.x);
                result.y = (float)(num1 * quaternion1.y - amount * quaternion2.y);
                result.z = (float)(num1 * quaternion1.z - amount * quaternion2.z);
                result.w = (float)(num1 * quaternion1.w - amount * quaternion2.w);
            }
            float num2 = 1f / (float)Math.Sqrt(result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w);
            result.x *= num2;
            result.y *= num2;
            result.z *= num2;
            result.w *= num2;
        }

        public static Quaternion Slerp(
          Quaternion quaternion1,
          Quaternion quaternion2,
          float amount)
        {
            float d = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w);
            bool flag = false;
            if (d < 0.0)
            {
                flag = true;
                d = -d;
            }
            float num1;
            float num2;
            if (d > 0.999999f)
            {
                num1 = 1f - amount;
                num2 = flag ? -amount : amount;
            }
            else
            {
                float a = (float)Math.Acos(d);
                float num3 = (float)(1.0 / Math.Sin(a));
                num1 = (float)Math.Sin((1.0 - amount) * a) * num3;
                num2 = flag ? (float)-Math.Sin(amount * a) * num3 : (float)Math.Sin(amount * a) * num3;
            }
            Quaternion quaternion;
            quaternion.x = (float)(num1 * quaternion1.x + num2 * quaternion2.x);
            quaternion.y = (float)(num1 * quaternion1.y + num2 * quaternion2.y);
            quaternion.z = (float)(num1 * quaternion1.z + num2 * quaternion2.z);
            quaternion.w = (float)(num1 * quaternion1.w + num2 * quaternion2.w);
            return quaternion;
        }

        public static void Slerp(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          float amount,
          out Quaternion result)
        {
            float d = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w);
            bool flag = false;
            if (d < 0.0)
            {
                flag = true;
                d = -d;
            }
            float num1;
            float num2;
            if (d > 0.999999f)
            {
                num1 = 1f - amount;
                num2 = flag ? -amount : amount;
            }
            else
            {
                float a = (float)Math.Acos(d);
                float num3 = (float)(1.0 / Math.Sin(a));
                num1 = (float)Math.Sin((1.0 - amount) * a) * num3;
                num2 = flag ? (float)-Math.Sin(amount * a) * num3 : (float)Math.Sin(amount * a) * num3;
            }
            result.x = (float)(num1 * quaternion1.x + num2 * quaternion2.x);
            result.y = (float)(num1 * quaternion1.y + num2 * quaternion2.y);
            result.z = (float)(num1 * quaternion1.z + num2 * quaternion2.z);
            result.w = (float)(num1 * quaternion1.w + num2 * quaternion2.w);
        }

        public static Quaternion Subtract(Quaternion quaternion1, Quaternion quaternion2)
        {
            quaternion1.x -= quaternion2.x;
            quaternion1.y -= quaternion2.y;
            quaternion1.z -= quaternion2.z;
            quaternion1.w -= quaternion2.w;
            return quaternion1;
        }

        public static void Subtract(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          out Quaternion result)
        {
            result.x = quaternion1.x - quaternion2.x;
            result.y = quaternion1.y - quaternion2.y;
            result.z = quaternion1.z - quaternion2.z;
            result.w = quaternion1.w - quaternion2.w;
        }

        public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        {
            float num1 = (float)(quaternion1.y * quaternion2.z - quaternion1.z * quaternion2.y);
            float num2 = (float)(quaternion1.z * quaternion2.x - quaternion1.x * quaternion2.z);
            float num3 = (float)(quaternion1.x * quaternion2.y - quaternion1.y * quaternion2.x);
            float num4 = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z);
            Quaternion quaternion;
            quaternion.x = (float)(quaternion1.x * quaternion2.w + quaternion2.x * quaternion1.w) + num1;
            quaternion.y = (float)(quaternion1.y * quaternion2.w + quaternion2.y * quaternion1.w) + num2;
            quaternion.z = (float)(quaternion1.z * quaternion2.w + quaternion2.z * quaternion1.w) + num3;
            quaternion.w = quaternion1.w * quaternion2.w - num4;
            return quaternion;
        }

        public static Quaternion Multiply(Quaternion quaternion1, float scaleFactor)
        {
            quaternion1.x *= scaleFactor;
            quaternion1.y *= scaleFactor;
            quaternion1.z *= scaleFactor;
            quaternion1.w *= scaleFactor;
            return quaternion1;
        }

        public static void Multiply(
          ref Quaternion quaternion1,
          float scaleFactor,
          out Quaternion result)
        {
            result.x = quaternion1.x * scaleFactor;
            result.y = quaternion1.y * scaleFactor;
            result.z = quaternion1.z * scaleFactor;
            result.w = quaternion1.w * scaleFactor;
        }

        public static void Multiply(
          ref Quaternion quaternion1,
          ref Quaternion quaternion2,
          out Quaternion result)
        {
            float num1 = (float)(quaternion1.y * quaternion2.z - quaternion1.z * quaternion2.y);
            float num2 = (float)(quaternion1.z * quaternion2.x - quaternion1.x * quaternion2.z);
            float num3 = (float)(quaternion1.x * quaternion2.y - quaternion1.y * quaternion2.x);
            float num4 = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z);
            result.x = (float)(quaternion1.x * quaternion2.w + quaternion2.x * quaternion1.w) + num1;
            result.y = (float)(quaternion1.y * quaternion2.w + quaternion2.y * quaternion1.w) + num2;
            result.z = (float)(quaternion1.z * quaternion2.w + quaternion2.z * quaternion1.w) + num3;
            result.w = quaternion1.w * quaternion2.w - num4;
        }

        public static Quaternion Negate(Quaternion quaternion)
        {
            Quaternion quaternion1;
            quaternion1.x = -quaternion.x;
            quaternion1.y = -quaternion.y;
            quaternion1.z = -quaternion.z;
            quaternion1.w = -quaternion.w;
            return quaternion1;
        }

        public static void Negate(ref Quaternion quaternion, out Quaternion result)
        {
            result.x = -quaternion.x;
            result.y = -quaternion.y;
            result.z = -quaternion.z;
            result.w = -quaternion.w;
        }

        public void Normalize()
        {
            float num = 1f / (float)Math.Sqrt(x * x + y * y + z * z + w * w);
            x *= num;
            y *= num;
            z *= num;
            w *= num;
        }

        public static Quaternion Normalize(Quaternion quaternion)
        {
            float num = 1f / (float)Math.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
            Quaternion quaternion1;
            quaternion1.x = quaternion.x * num;
            quaternion1.y = quaternion.y * num;
            quaternion1.z = quaternion.z * num;
            quaternion1.w = quaternion.w * num;
            return quaternion1;
        }

        public static void Normalize(ref Quaternion quaternion, out Quaternion result)
        {
            float num = 1f / (float)Math.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
            result.x = quaternion.x * num;
            result.y = quaternion.y * num;
            result.z = quaternion.z * num;
            result.w = quaternion.w * num;
        }

        public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2)
        {
            quaternion1.x += quaternion2.x;
            quaternion1.y += quaternion2.y;
            quaternion1.z += quaternion2.z;
            quaternion1.w += quaternion2.w;
            return quaternion1;
        }

        public static Quaternion operator /(Quaternion quaternion1, Quaternion quaternion2)
        {
            float num1 = (float)(1.0 / (quaternion2.x * quaternion2.x + quaternion2.y * quaternion2.y + quaternion2.z * quaternion2.z + quaternion2.w * quaternion2.w));
            float num2 = -quaternion2.x * num1;
            float num3 = -quaternion2.y * num1;
            float num4 = -quaternion2.z * num1;
            float num5 = quaternion2.w * num1;
            Quaternion quaternion;
            quaternion.x = (float)(quaternion1.x * num5 + num2 * quaternion1.w + (quaternion1.y * num4 - quaternion1.z * num3));
            quaternion.y = (float)(quaternion1.y * num5 + num3 * quaternion1.w + (quaternion1.z * num2 - quaternion1.x * num4));
            quaternion.z = (float)(quaternion1.z * num5 + num4 * quaternion1.w + (quaternion1.x * num3 - quaternion1.y * num2));
            quaternion.w = (float)(quaternion1.w * quaternion2.w * num1 - (quaternion1.x * num2 + quaternion1.y * num3 + quaternion1.z * num4));
            return quaternion;
        }

        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2) => quaternion1.x == quaternion2.x && quaternion1.y == quaternion2.y && quaternion1.z == quaternion2.z && quaternion1.w == quaternion2.w;

        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2) => quaternion1.x != quaternion2.x || quaternion1.y != quaternion2.y || quaternion1.z != quaternion2.z || quaternion1.w != quaternion2.w;

        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            float num1 = (float)(quaternion1.y * quaternion2.z - quaternion1.z * quaternion2.y);
            float num2 = (float)(quaternion1.z * quaternion2.x - quaternion1.x * quaternion2.z);
            float num3 = (float)(quaternion1.x * quaternion2.y - quaternion1.y * quaternion2.x);
            float num4 = (float)(quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y + quaternion1.z * quaternion2.z);
            Quaternion quaternion;
            quaternion.x = (float)(quaternion1.x * quaternion2.w + quaternion2.x * quaternion1.w) + num1;
            quaternion.y = (float)(quaternion1.y * quaternion2.w + quaternion2.y * quaternion1.w) + num2;
            quaternion.z = (float)(quaternion1.z * quaternion2.w + quaternion2.z * quaternion1.w) + num3;
            quaternion.w = quaternion1.w * quaternion2.w - num4;
            return quaternion;
        }

        public static Quaternion operator *(Quaternion quaternion1, float scaleFactor)
        {
            quaternion1.x *= scaleFactor;
            quaternion1.y *= scaleFactor;
            quaternion1.z *= scaleFactor;
            quaternion1.w *= scaleFactor;
            return quaternion1;
        }

        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
        {
            quaternion1.x -= quaternion2.x;
            quaternion1.y -= quaternion2.y;
            quaternion1.z -= quaternion2.z;
            quaternion1.w -= quaternion2.w;
            return quaternion1;
        }

        public static Quaternion operator -(Quaternion quaternion)
        {
            quaternion.x = -quaternion.x;
            quaternion.y = -quaternion.y;
            quaternion.z = -quaternion.z;
            quaternion.w = -quaternion.w;
            return quaternion;
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
            stringBuilder.Append(" W:");
            stringBuilder.Append(w);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
