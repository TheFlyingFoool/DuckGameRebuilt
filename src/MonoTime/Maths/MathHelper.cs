// Decompiled with JetBrains decompiler
// Type: DuckGame.MathHelper
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// Contains commonly used precalculated values and mathematical operations.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>Represents the mathematical constant e(2.71828175).</summary>
        public const float E = 2.718282f;
        /// <summary>Represents the log base ten of e(0.4342945).</summary>
        public const float Log10E = 0.4342945f;
        /// <summary>Represents the log base two of e(1.442695).</summary>
        public const float Log2E = 1.442695f;
        /// <summary>Represents the value of pi(3.14159274).</summary>
        public const float Pi = 3.141593f;
        /// <summary>
        /// Represents the value of pi divided by two(1.57079637).
        /// </summary>
        public const float PiOver2 = 1.570796f;
        /// <summary>
        /// Represents the value of pi divided by four(0.7853982).
        /// </summary>
        public const float PiOver4 = 0.7853982f;
        /// <summary>Represents the value of pi times two(6.28318548).</summary>
        public const float TwoPi = 6.283185f;

        /// <summary>
        /// Returns the Cartesian coordinate for one axis of a point that is defined by a given triangle and two normalized barycentric (areal) coordinates.
        /// </summary>
        /// <param name="value1">The coordinate on one axis of vertex 1 of the defining triangle.</param>
        /// <param name="value2">The coordinate on the same axis of vertex 2 of the defining triangle.</param>
        /// <param name="value3">The coordinate on the same axis of vertex 3 of the defining triangle.</param>
        /// <param name="amount1">The normalized barycentric (areal) coordinate b2, equal to the weighting factor for vertex 2, the coordinate of which is specified in value2.</param>
        /// <param name="amount2">The normalized barycentric (areal) coordinate b3, equal to the weighting factor for vertex 3, the coordinate of which is specified in value3.</param>
        /// <returns>Cartesian coordinate of the specified point with respect to the axis being used.</returns>
        public static float Barycentric(
          float value1,
          float value2,
          float value3,
          float amount1,
          float amount2)
        {
            return (float)((double)value1 + ((double)value2 - (double)value1) * (double)amount1 + ((double)value3 - (double)value1) * (double)amount2);
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A position that is the result of the Catmull-Rom interpolation.</returns>
        public static float CatmullRom(
          float value1,
          float value2,
          float value3,
          float value4,
          float amount)
        {
            double num1 = (double)amount * (double)amount;
            double num2 = num1 * (double)amount;
            return (float)(0.5 * (2.0 * (double)value2 + ((double)value3 - (double)value1) * (double)amount + (2.0 * (double)value1 - 5.0 * (double)value2 + 4.0 * (double)value3 - (double)value4) * num1 + (3.0 * (double)value2 - (double)value1 - 3.0 * (double)value3 + (double)value4) * num2));
        }

        /// <summary>Restricts a value to be within a specified range.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            value = (double)value > (double)max ? max : value;
            value = (double)value < (double)min ? min : value;
            return value;
        }

        /// <summary>Restricts a value to be within a specified range.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(int value, int min, int max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>Distance between the two values.</returns>
        public static float Distance(float value1, float value2) => Math.Abs(value1 - value2);

        /// <summary>Performs a Hermite spline interpolation.</summary>
        /// <param name="value1">Source position.</param>
        /// <param name="tangent1">Source tangent.</param>
        /// <param name="value2">Source position.</param>
        /// <param name="tangent2">Source tangent.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static float Hermite(
          float value1,
          float tangent1,
          float value2,
          float tangent2,
          float amount)
        {
            double num1 = (double)value1;
            double num2 = (double)value2;
            double num3 = (double)tangent1;
            double num4 = (double)tangent2;
            double num5 = (double)amount;
            double num6 = num5 * num5 * num5;
            double num7 = num5 * num5;
            return (double)amount != 0.0 ? ((double)amount != 1.0 ? (float)((2.0 * num1 - 2.0 * num2 + num4 + num3) * num6 + (3.0 * num2 - 3.0 * num1 - 2.0 * num3 - num4) * num7 + num3 * num5 + num1) : value2) : value1;
        }

        /// <summary>Linearly interpolates between two values.</summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula.
        /// <c>value1 + (value2 - value1) * amount</c>
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// </remarks>
        public static float Lerp(float value1, float value2, float amount) => value1 + (value2 - value1) * amount;

        /// <summary>Returns the greater of two values.</summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The greater value.</returns>
        public static float Max(float value1, float value2) => Math.Max(value1, value2);

        /// <summary>Returns the lesser of two values.</summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The lesser value.</returns>
        public static float Min(float value1, float value2) => Math.Min(value1, value2);

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Interpolated value.</returns>
        public static float SmoothStep(float value1, float value2, float amount)
        {
            float amount1 = MathHelper.Clamp(amount, 0f, 1f);
            return MathHelper.Hermite(value1, 0f, value2, 0f, amount1);
        }

        /// <summary>Converts radians to degrees.</summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = 180 / pi
        /// </remarks>
        public static float ToDegrees(float radians) => radians * 57.29578f;

        /// <summary>Converts degrees to radians.</summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = pi / 180
        /// </remarks>
        public static float ToRadians(float degrees) => degrees * ((float)Math.PI / 180f);

        /// <summary>Reduces a given angle to a value between π and -π.</summary>
        /// <param name="angle">The angle to reduce, in radians.</param>
        /// <returns>The new angle, in radians.</returns>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.28318548202515);
            if ((double)angle <= -3.14159274101257)
                angle += 6.283185f;
            else if ((double)angle > 3.14159274101257)
                angle -= 6.283185f;
            return angle;
        }

        /// <summary>Determines if value is powered by two.</summary>
        /// <param name="value">A value.</param>
        /// <returns><c>true</c> if <c>value</c> is powered by two; otherwise <c>false</c>.</returns>
        public static bool IsPowerOfTwo(int value) => value > 0 && (value & value - 1) == 0;
    }
}
