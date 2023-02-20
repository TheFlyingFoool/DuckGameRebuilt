using System;
using System.Reflection;
using AddedContent.Hyeve.DebugUI.Values;
using Microsoft.Xna.Framework;

namespace AddedContent.Hyeve.DebugUI.ReflectionValues
{
    public class UiReflectionNumberBar : UiNumberBar
    {
        protected readonly FieldInfo Variable;
        protected readonly Object FieldOwner;

        private NumberType _fieldType;
        
        public override double Value
        {
            get
            {
                object data = Variable.GetValue(FieldOwner);
                switch (_fieldType)
                {
                    case NumberType.Float:
                        return (float)data;
                    case NumberType.Double:
                        return (double)data;
                    case NumberType.Int:
                        return (int)data;
                    case NumberType.Long:
                        return (long)data;
                }
                return 0;
            }
            set
            {
                switch (_fieldType)
                {
                    case NumberType.Float:
                        Variable.SetValue(FieldOwner, (float)value);
                        break;
                    case NumberType.Double:
                        Variable.SetValue(FieldOwner, value);
                        break;
                    case NumberType.Int:
                        Variable.SetValue(FieldOwner, (int)value);
                        break;
                    case NumberType.Long:
                        Variable.SetValue(FieldOwner, (long)value);
                        break;
                }
            }
        }
        
        
        public UiReflectionNumberBar(Vector2 position, Vector2 size, FieldInfo variable, Object owner = null, string name = "UiSeamless", float scale = 1) : base(position, size, name, scale)
        {
            Type type = variable.FieldType;
            

            //hate this, but can't switch with typeof()
            if (type == typeof(int)) _fieldType = NumberType.Int;
            else if (type == typeof(long)) _fieldType = NumberType.Long;
            else if (type == typeof(float)) _fieldType = NumberType.Float;
            else if (type == typeof(double)) _fieldType = NumberType.Double;
            else throw new InvalidCastException($"Can't use {variable.FieldType.Name} field info as an int, long, float, or double!");

            if (_fieldType is NumberType.Int or NumberType.Long) Formatter = "F0";

            FieldOwner = owner;
            Variable = variable;
            Name = FieldOwner is null ? $"{Variable.DeclaringType.Name}.{Variable.Name}" : $"{FieldOwner.GetType().Name}.{Variable.Name} [I{FieldOwner.GetHashCode()}]";
        }
    }

    internal enum NumberType
    {
        Float, Double, Int, Long
    }
}