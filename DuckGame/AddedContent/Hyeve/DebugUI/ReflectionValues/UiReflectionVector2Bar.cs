using System;
using System.Reflection;
using AddedContent.Hyeve.DebugUI.Values;
using DuckGame;
using Microsoft.Xna.Framework;

namespace AddedContent.Hyeve.DebugUI.ReflectionValues
{
    public class UiReflectionVector2Bar : UiVector2Bar
    {
        protected readonly FieldInfo Variable;
        protected readonly Object FieldOwner;
        
        private VectorType _fieldType;

        public override Vector2 Value
        {
            get
            {
                object data = Variable.GetValue(FieldOwner);
                switch (_fieldType)
                {
                    case VectorType.Vec2:
                        return (Vec2)data;
                    case VectorType.Vector2:
                        return (Vector2)data;
                }
                return Vector2.Zero;
            }
            set
            {
                switch (_fieldType)
                {
                    case VectorType.Vec2:
                        Variable.SetValue(FieldOwner, (Vec2)value);
                        break;
                    case VectorType.Vector2:
                        Variable.SetValue(FieldOwner, (Vector2)value);
                        break;
                }
            }
        }
        
        
        public UiReflectionVector2Bar(Vector2 position, Vector2 size, FieldInfo variable, Object owner = null, string name = "UiVector2", float scale = 1) : base(position, size, name, scale)
        {
            Type type = variable.FieldType;
            
            //hate this, but can't switch with typeof()
            if (type == typeof(Vec2)) _fieldType = VectorType.Vec2;
            else if (type == typeof(Vector2)) _fieldType = VectorType.Vector2;
            else throw new InvalidCastException($"Can't use {variable.FieldType.Name} field info as vec2!");
            
            FieldOwner = owner;
            Variable = variable;
            Name = FieldOwner is null ? $"{Variable.DeclaringType.Name}.{Variable.Name}" : $"{FieldOwner.GetType().Name}.{Variable.Name} [I{FieldOwner.GetHashCode()}]";
        }
    }
    
    internal enum VectorType
    {
        Vector2, Vec2,
    }
}