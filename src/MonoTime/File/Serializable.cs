// Decompiled with JetBrains decompiler
// Type: DuckGame.Serializable
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Reflection;

namespace DuckGame
{
    public class Serializable
    {
        public void SerializeField(BinaryClassChunk element, string name)
        {
            ClassMember member = Editor.GetMember(GetType(), name);
            if (member == null)
                return;
            element.AddProperty(name, member.GetValue(this));
        }

        public void DeserializeField(BinaryClassChunk node, string name) => Editor.GetMember(GetType(), name)?.SetValue(this, node.GetProperty(name));

        public void LegacySerializeField(DXMLNode element, string name)
        {
            FieldInfo field = GetType().GetField(name);
            object name1;
            if (field != null)
            {
                name1 = field.GetValue(this);
            }
            else
            {
                PropertyInfo property = GetType().GetProperty(name);
                if (!(property != null))
                    return;
                name1 = property.GetValue(this, null);
            }
            if (name1.GetType().IsEnum)
                name1 = Enum.GetName(name1.GetType(), name1);
            element.Add(new DXMLNode(name, name1));
        }

        public void LegacyDeserializeField(DXMLNode node, string name)
        {
            try
            {
                DXMLNode dxmlNode = node.Element(name);
                if (dxmlNode == null)
                    return;
                FieldInfo field = GetType().GetField(name);
                if (field != null)
                {
                    if (field.FieldType.IsEnum)
                        field.SetValue(this, Enum.Parse(field.FieldType, dxmlNode.Value));
                    else
                        field.SetValue(this, Convert.ChangeType(dxmlNode.Value, field.FieldType));
                }
                else
                {
                    PropertyInfo property = GetType().GetProperty(name);
                    if (!(property != null))
                        return;
                    if (property.PropertyType.IsEnum)
                        property.SetValue(this, Enum.Parse(property.PropertyType, dxmlNode.Value), null);
                    else
                        property.SetValue(this, Convert.ChangeType(dxmlNode.Value, property.PropertyType), null);
                }
            }
            catch
            {
            }
        }
    }
}
