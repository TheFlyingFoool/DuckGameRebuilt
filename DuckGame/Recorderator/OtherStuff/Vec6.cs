using System;

namespace DuckGame
{
	[Serializable]
	public struct Vec6 : IEquatable<Vec6>
	{
		public static Vec6 Zero
		{
			get
			{
				return zeroVector;
			}
		}
        public override string ToString()
        {
            return a + ":" + b + ":" + c + ":" + d + ":" + e + ":" + f;
        }

        public Vec6(float x, float y, float z, float w, float v, float u)
		{
			a = x;
			b = y;
			c = z;
			d = w;
			e = v;
			f = u;
		}
		public static Vec6 Add(Vec6 value1, Vec6 value2)
		{
			value1.d += value2.d;
			value1.a += value2.a;
			value1.b += value2.b;
			value1.c += value2.c;
			value1.f += value2.f;
			value1.e += value2.e;
			return value1;
		}

		public static void Add(ref Vec6 value1, ref Vec6 value2, out Vec6 result)
		{
			result.d = value1.d + value2.d;
			result.a = value1.a + value2.a;
			result.b = value1.b + value2.b;
			result.c = value1.c + value2.c;
			result.f = value1.f + value2.f;
			result.e = value1.e + value2.e;
		}

		public static Vec6 Divide(Vec6 value1, Vec6 value2)
		{
			value1.d /= value2.d;
			value1.a /= value2.a;
			value1.b /= value2.b;
			value1.c /= value2.c;
			return value1;
		}

		public static Vec6 Divide(Vec6 value1, float divider)
		{
			float factor = 1f / divider;
			value1.d *= factor;
			value1.a *= factor;
			value1.b *= factor;
			value1.c *= factor;
			value1.e *= factor;
			value1.f *= factor;
			return value1;
		}

		public static void Divide(ref Vec6 value1, float divider, out Vec6 result)
		{
			float factor = 1f / divider;
			result.d = value1.d * factor;
			result.a = value1.a * factor;
			result.b = value1.b * factor;
			result.c = value1.c * factor;
			result.e = value1.e * factor;
			result.f = value1.f * factor;
		}

		public static void Divide(ref Vec6 value1, ref Vec6 value2, out Vec6 result)
		{
			result.d = value1.d / value2.d;
			result.a = value1.a / value2.a;
			result.b = value1.b / value2.b;
			result.c = value1.c / value2.c;
			result.e = value1.e / value2.e;
			result.f = value1.f / value2.f;
		}

		public static float Dot(Vec6 vector1, Vec6 vector2)
		{
			return vector1.a * vector2.a + vector1.b * vector2.b + vector1.c * vector2.c + vector1.d * vector2.d;
		}

		public static void Dot(ref Vec6 vector1, ref Vec6 vector2, out float result)
		{
			result = vector1.a * vector2.a + vector1.b * vector2.b + vector1.c * vector2.c + vector1.d * vector2.d;
		}

		public override bool Equals(object obj)
		{
			return obj is Vec6 v && Equals(v);
		}

		public bool Equals(Vec6 other)
		{
			return a == other.a && b == other.b && c == other.c && d == other.d && e == other.e && f == other.f;
		}
		public static Vec6 Multiply(Vec6 value1, Vec6 value2)
		{
			value1.d *= value2.d;
			value1.a *= value2.a;
			value1.b *= value2.b;
			value1.c *= value2.c;
			value1.e *= value2.e;
			value1.f *= value2.f;
			return value1;
		}

		public static Vec6 Multiply(Vec6 value1, float scaleFactor)
		{
			value1.d *= scaleFactor;
			value1.a *= scaleFactor;
			value1.b *= scaleFactor;
			value1.c *= scaleFactor;
			value1.f *= scaleFactor;
			value1.e *= scaleFactor;
			return value1;
		}

		public static void Multiply(ref Vec6 value1, float scaleFactor, out Vec6 result)
		{
			result.d = value1.d * scaleFactor;
			result.a = value1.a * scaleFactor;
			result.b = value1.b * scaleFactor;
			result.c = value1.c * scaleFactor;
			result.e = value1.e * scaleFactor;
			result.f = value1.f * scaleFactor;
		}

		public static void Multiply(ref Vec6 value1, ref Vec6 value2, out Vec6 result)
		{
			result.d = value1.d * value2.d;
			result.a = value1.a * value2.a;
			result.b = value1.b * value2.b;
			result.c = value1.c * value2.c;
			result.e = value1.e * value2.e;
			result.f = value1.f * value2.f;
		}

		public static Vec6 Negate(Vec6 value)
		{
			value.a = -value.a;
			value.b = -value.b;
			value.c = -value.c;
			value.d = -value.d;
			value.f = -value.f;
			value.e = -value.e;
			return value;
		}

		public static void Negate(ref Vec6 value, out Vec6 result)
		{
			result.a = -value.a;
			result.b = -value.b;
			result.c = -value.c;
			result.d = -value.d;
			result.e = -value.e;
			result.f = -value.f;
		}
		public static Vec6 Subtract(Vec6 value1, Vec6 value2)
		{
			value1.d -= value2.d;
			value1.a -= value2.a;
			value1.b -= value2.b;
			value1.c -= value2.c;
			value1.f -= value2.f;
			value1.e -= value2.e;
			return value1;
		}

		public static void Subtract(ref Vec6 value1, ref Vec6 value2, out Vec6 result)
		{
			result.d = value1.d - value2.d;
			result.a = value1.a - value2.a;
			result.b = value1.b - value2.b;
			result.c = value1.c - value2.c;

			result.e = value1.e - value2.e;
			result.f = value1.f - value2.f;
		}
        public override int GetHashCode()
        {
            int hashCode = -584342449;
            hashCode = hashCode * -1521134295 + a.GetHashCode();
            hashCode = hashCode * -1521134295 + b.GetHashCode();
            hashCode = hashCode * -1521134295 + c.GetHashCode();
            hashCode = hashCode * -1521134295 + d.GetHashCode();
            hashCode = hashCode * -1521134295 + e.GetHashCode();
            hashCode = hashCode * -1521134295 + f.GetHashCode();
            return hashCode;
        }

        public static Vec6 operator -(Vec6 value)
		{
			value.a = -value.a;
			value.b = -value.b;
			value.c = -value.c;
			value.d = -value.d;
			value.f = -value.f;
			value.e = -value.e;
			return value;
		}

		public static bool operator ==(Vec6 value1, Vec6 value2)
		{
			return value1.e == value2.d && value1.a == value2.a && value1.b == value2.b && value1.e == value2.e && value1.f == value2.f;
		}

		public static bool operator !=(Vec6 value1, Vec6 value2)
		{
			return value1.d != value2.d || value1.a != value2.a || value1.b != value2.b || value1.c != value2.c || value1.e != value2.e || value1.f != value2.f;
		}

		public static Vec6 operator +(Vec6 value1, Vec6 value2)
		{
			value1.d += value2.d;
			value1.a += value2.a;
			value1.b += value2.b;
			value1.c += value2.c;
			value1.f += value2.f;
			value1.e += value2.e;
			return value1;
		}

		public static Vec6 operator -(Vec6 value1, Vec6 value2)
		{
			value1.d -= value2.d;
			value1.a -= value2.a;
			value1.b -= value2.b;
			value1.c -= value2.c;
			value1.f -= value2.f;
			value1.e -= value2.e;
			return value1;
		}

		public static Vec6 operator *(Vec6 value1, Vec6 value2)
		{
			value1.d *= value2.d;
			value1.a *= value2.a;
			value1.b *= value2.b;
			value1.c *= value2.c;
			return value1;
		}

		public static Vec6 operator *(Vec6 value1, float scaleFactor)
		{
			value1.d *= scaleFactor;
			value1.a *= scaleFactor;
			value1.b *= scaleFactor;
			value1.c *= scaleFactor;
			return value1;
		}

		public static Vec6 operator *(float scaleFactor, Vec6 value1)
		{
			value1.d *= scaleFactor;
			value1.a *= scaleFactor;
			value1.b *= scaleFactor;
			value1.c *= scaleFactor;
			return value1;
		}

		public static Vec6 operator /(Vec6 value1, Vec6 value2)
		{
			value1.d /= value2.d;
			value1.a /= value2.a;
			value1.b /= value2.b;
			value1.c /= value2.c;
			return value1;
		}

		public static Vec6 operator / (Vec6 value1, float divider)
		{
			float factor = 1f / divider;
			value1.d *= factor;
			value1.a *= factor;
			value1.b *= factor;
			value1.c *= factor;
			return value1;
		}

		private static Vec6 zeroVector = default;


		public float a;

		public float b;

		public float c;

		public float d;

        public float e;

		public float f;
    }
}
