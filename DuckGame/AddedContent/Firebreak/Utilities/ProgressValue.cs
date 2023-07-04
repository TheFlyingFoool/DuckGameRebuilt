using System;

namespace DuckGame
{

    public class ProgressValue
    {
        public double Value = 0;

        public double NormalizedValue
        {
            get => (Value - MinimumValue) / (MaximumValue - MinimumValue);
            set => Value = value * (MaximumValue - MinimumValue) + MinimumValue;
        }

        public double MaximumValue = 1;
        public double MinimumValue = 0;
        public double IncrementSize = 0.05;

        public bool Completed => NormalizedValue >= 1;

        public ProgressValue() : this(0D) { }

        public ProgressValue(ProgressValue p) : this(p.Value, p.IncrementSize, p.MinimumValue, p.MaximumValue) { }

        public ProgressValue(double value, double incrementSize = 0.05, double min = 0, double max = 1)
        {
            if (MinimumValue > MaximumValue)
                throw new Exception("Minimum size cannot be less than the maximum size");

            Value = value;
            MaximumValue = max;
            MinimumValue = min;
            IncrementSize = incrementSize;
        }

        public string GenerateBar(int characterCount = 30, char filled = '#', char empty = '-', Func<string, string, string>? formatFunction = null)
        {
            formatFunction ??= (done, left) => $"{done}{left}";
            Value = ~this;

            double fillPercentage = NormalizedValue * characterCount;

            string whiteBar = new(filled, (int)fillPercentage);
            string blackBar = new(empty, (int)(characterCount - fillPercentage));

            string fullBar = formatFunction(whiteBar, blackBar);
            fullBar = fullBar.Length == characterCount - 1
                ? fullBar + empty
                : fullBar;

            return fullBar;
        }

        // From Progress to T
        public static implicit operator float(ProgressValue f) => (float)f.Value;
        public static implicit operator double(ProgressValue f) => f.Value;
        public static implicit operator int(ProgressValue f) => (int)f.Value;

        // From T to Progress
        public static implicit operator ProgressValue(float f) => new((double)f);
        public static implicit operator ProgressValue(double f) => new(f);
        public static implicit operator ProgressValue(int f) => new((double)f);

        // Positive/Negative
        public static ProgressValue operator +(ProgressValue f) => f;
        public static ProgressValue operator -(ProgressValue f) => f * -1;

        // Arithmetic

        public static ProgressValue operator +(ProgressValue a, ProgressValue b)
        {
            a.Value = a.Value + b.Value;
            return a;
        }

        public static ProgressValue operator -(ProgressValue a, ProgressValue b)
        {

            a.Value = a.Value - b.Value;
            return a;
        }

        public static ProgressValue operator *(ProgressValue a, ProgressValue b)
        {
            a.Value = a.Value * b.Value;
            return a;
        }

        public static ProgressValue operator /(ProgressValue a, ProgressValue b)
        {
            a.Value = a.Value / b.Value;
            return a;
        }

        // Equality
        public static bool operator ==(ProgressValue a, ProgressValue b) => a.Value == b.Value;
        public static bool operator !=(ProgressValue a, ProgressValue b) => a.Value != b.Value;
        public static bool operator >(ProgressValue a, ProgressValue b) => a.Value > b.Value;
        public static bool operator <(ProgressValue a, ProgressValue b) => a.Value < b.Value;
        public static bool operator >=(ProgressValue a, ProgressValue b) => a.Value >= b.Value;
        public static bool operator <=(ProgressValue a, ProgressValue b) => a.Value <= b.Value;

        // Increment/Decrement
        public static ProgressValue operator ++(ProgressValue p) => p += p.IncrementSize;
        public static ProgressValue operator --(ProgressValue p) => p -= p.IncrementSize;

        // Inversion
        public static ProgressValue operator !(ProgressValue p)
        {
            p.Value = p.MaximumValue - p.Value + p.MinimumValue;
            return p;
        }

        // Clamping
        public static ProgressValue operator ~(ProgressValue p)
        {
            p.Value = Maths.Clamp(p.Value, p.MinimumValue, p.MaximumValue);
            return p;
        }

        // Method Overrides
        public override bool Equals(object obj)
        {
            return obj is ProgressValue p && p.Equals(this);
        }

        public bool Equals(ProgressValue other)
        {
            return Value.Equals(other.Value)
                   && MaximumValue.Equals(other.MaximumValue)
                   && MinimumValue.Equals(other.MinimumValue)
                   && IncrementSize.Equals(other.IncrementSize);
        }
        public ProgressValue Clone()
        {
            ProgressValue progressValue = new ProgressValue
            {
                Value = Value,
                MaximumValue = MaximumValue,
                MinimumValue = MinimumValue,
                IncrementSize = IncrementSize
            };
            return progressValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ MaximumValue.GetHashCode();
                hashCode = (hashCode * 397) ^ MinimumValue.GetHashCode();
                hashCode = (hashCode * 397) ^ IncrementSize.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return MinimumValue == 0
                ? $"{Value}/{MaximumValue}"
                : $"{MinimumValue}/{Value}/{MaximumValue}";
        }
    }
}