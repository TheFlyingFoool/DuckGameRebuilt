using System;
using System.Reflection;
using AddedContent.Hyeve.DebugUI.Values;
using Microsoft.Xna.Framework;

namespace AddedContent.Hyeve.DebugUI.ReflectionValues
{
    public class UiReflectionToggleButton : UiToggleButton
    {
        protected readonly FieldInfo Variable;
        protected readonly Object FieldOwner;

        public override bool Toggled
        {
            get => (bool)Variable.GetValue(FieldOwner);
            set => Variable.SetValue(FieldOwner, value);
        }
        
        public UiReflectionToggleButton(Vector2 position, Vector2 size, FieldInfo variable, Object owner = null, string name = "UiReflectionToggle", float scale = 1) : base(position, size, name, scale)
        {
            if (variable.FieldType != typeof(bool)) throw new InvalidCastException($"Can't use {variable.FieldType.Name} field info as a bool!");
            FieldOwner = owner;
            Variable = variable;
            Name = FieldOwner is null ? $"{Variable.DeclaringType.Name}.{Variable.Name}" : $"{FieldOwner.GetType().Name}.{Variable.Name} [I{FieldOwner.GetHashCode()}]";
        }
    }
}