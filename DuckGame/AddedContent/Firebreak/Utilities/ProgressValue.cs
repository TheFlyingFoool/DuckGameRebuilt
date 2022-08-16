using System;

namespace DuckGame;

public struct ProgressValue
{
    private double _value = 0;
    
    public double Value
    {
        get => Maths.Clamp(_value, MinimumValue, MaximumValue);
        set => _value = value;
    }

    public double NormalizedValue
    {
        get => Value - MinimumValue / MaximumValue - MinimumValue;
        set => Value = value * (MaximumValue - MinimumValue) + MinimumValue;
    }
    
    public double MaximumValue = 1;
    public double MinimumValue = 0;
    public double IncrementSize = 0.05;

    public ProgressValue(double value, double incrementSize = 0.05, double min = 0, double max = 1)
    {
        Value = value;
        MaximumValue = max;
        MinimumValue = min;
        IncrementSize = incrementSize;
    }

    public string GenerateBar(int characterCount = 30, char filled = '#', char empty = '-')
    {
        double fillPercentage = NormalizedValue * characterCount;

        string whiteBar = $"{(fillPercentage > 0 ? new string(filled, (int)fillPercentage) : "")}";
        string blackBar = $"{new string(empty, (int)(characterCount - fillPercentage))}";
        
        string fullBar = $"{whiteBar}{blackBar}";
        fullBar = fullBar.Length == characterCount - 1 
            ? fullBar + empty
            : fullBar;
        
        return fullBar;
    }

    // From Progress to T
    public static implicit operator float(ProgressValue f) => (float) f.Value;
    public static implicit operator double(ProgressValue f) => f.Value;
    public static implicit operator int(ProgressValue f) => (int) f.Value;
    
    // From T to Progress
    public static implicit operator ProgressValue(float f) => new(f);
    public static implicit operator ProgressValue(double f) => new(f);
    public static implicit operator ProgressValue(int f) => new(f);
    
    // Positive/Negative
    public static ProgressValue operator +(ProgressValue f) => f;
    public static ProgressValue operator -(ProgressValue f) => new(-f.Value, f.IncrementSize, f.MinimumValue, f.MaximumValue);
    
    // Arithmetic
    public static ProgressValue operator +(ProgressValue a, ProgressValue b) => new(a.Value + b.Value, a.IncrementSize, a.MinimumValue, a.MaximumValue);
    public static ProgressValue operator -(ProgressValue a, ProgressValue b) => new(a.Value - b.Value, a.IncrementSize, a.MinimumValue, a.MaximumValue);
    public static ProgressValue operator *(ProgressValue a, ProgressValue b) => new(a.Value * b.Value, a.IncrementSize, a.MinimumValue, a.MaximumValue);
    public static ProgressValue operator /(ProgressValue a, ProgressValue b) => new(a.Value / b.Value, a.IncrementSize, a.MinimumValue, a.MaximumValue);
    
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
    public static ProgressValue operator !(ProgressValue p) => p.MaximumValue - p.Value + p.MinimumValue;
    
    // Method Overrides
    public override bool Equals(object obj)
    {
        return obj is ProgressValue p && p.Value == Value;
    }

    public bool Equals(ProgressValue other)
    {
        return Value.Equals(other.Value) 
               && MaximumValue.Equals(other.MaximumValue) 
               && MinimumValue.Equals(other.MinimumValue) 
               && IncrementSize.Equals(other.IncrementSize);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = _value.GetHashCode();
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