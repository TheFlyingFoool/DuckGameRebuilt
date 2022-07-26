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
            ClassMember member = Editor.GetMember(this.GetType(), name);
            if (member == null)
                return;
            element.AddProperty(name, member.GetValue((object)this));
        }

        public void DeserializeField(BinaryClassChunk node, string name) => Editor.GetMember(this.GetType(), name)?.SetValue((object)this, node.GetProperty(name));

        public void LegacySerializeField(DXMLNode element, string name)
        {
            FieldInfo field = this.GetType().GetField(name);
            object name1;
            if (field != (FieldInfo)null)
            {
                name1 = field.GetValue((object)this);
            }
            else
            {
                PropertyInfo property = this.GetType().GetProperty(name);
                if (!(property != (PropertyInfo)null))
                    return;
                name1 = property.GetValue((object)this, (object[])null);
            }
            if (name1.GetType().IsEnum)
                name1 = (object)Enum.GetName(name1.GetType(), name1);
            element.Add(new DXMLNode(name, name1));
        }

        public void LegacyDeserializeField(DXMLNode node, string name)
        {
            try
            {
                DXMLNode dxmlNode = node.Element(name);
                if (dxmlNode == null)
                    return;
                FieldInfo field = this.GetType().GetField(name);
                if (field != (FieldInfo)null)
                {
                    if (field.FieldType.IsEnum)
                        field.SetValue((object)this, Enum.Parse(field.FieldType, dxmlNode.Value));
                    else
                        field.SetValue((object)this, Convert.ChangeType((object)dxmlNode.Value, field.FieldType));
                }
                else
                {
                    PropertyInfo property = this.GetType().GetProperty(name);
                    if (!(property != (PropertyInfo)null))
                        return;
                    if (property.PropertyType.IsEnum)
                        property.SetValue((object)this, Enum.Parse(property.PropertyType, dxmlNode.Value), (object[])null);
                    else
                        property.SetValue((object)this, Convert.ChangeType((object)dxmlNode.Value, property.PropertyType), (object[])null);
                }
            }
            catch
            {
            }
        }
    }
}
