// Decompiled with JetBrains decompiler
// Type: DuckGame.DataClass
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class DataClass
    {
        protected string _nodeName = "DataNode";

        private static DXMLNode SerializeDict(string name, IDictionary dict)
        {
            if (dict.Keys.Count <= 0)
                return null;
            string varValue = "";
            foreach (object key in (IEnumerable)dict.Keys)
                varValue = varValue + Convert.ToString(key) + "|" + Convert.ToString(dict[key]) + "@";
            DXMLNode dxmlNode = new DXMLNode(name);
            dxmlNode.Add(new DXMLNode("valueString", varValue));
            return dxmlNode;
        }

        private static DXMLNode SerializeCollection(string name, IList coll)
        {
            if (coll.Count <= 0)
                return null;
            string str = "";
            foreach (object obj in (IEnumerable)coll)
                str = str + Convert.ToString(obj) + "|";
            string varValue = str.Substring(0, str.Length - 1);
            DXMLNode dxmlNode = new DXMLNode(name);
            dxmlNode.Add(new DXMLNode("valueString", varValue));
            return dxmlNode;
        }

        public static DXMLNode SerializeClass(object o, string nodeName)
        {
            DXMLNode dxmlNode1 = new DXMLNode(nodeName);
            PropertyInfo[] properties = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(RasterFont))
                {
                    if (!(propertyInfo.GetValue(o, null) is RasterFont none))
                        none = RasterFont.None;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, none.Serialize()));
                }
                else if (propertyInfo.PropertyType == typeof(StatBinding))
                {
                    StatBinding statBinding = propertyInfo.GetValue(o, null) as StatBinding;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, statBinding.value));
                }
                else if (propertyInfo.PropertyType == typeof(Resolution))
                {
                    Resolution resolution = propertyInfo.GetValue(o, null) as Resolution;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, resolution.x.ToString() + "x" + resolution.y.ToString() + "x" + ((int)resolution.mode).ToString()));
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    IDictionary dict = propertyInfo.GetValue(o, null) as IDictionary;
                    DXMLNode node = SerializeDict(propertyInfo.Name, dict);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    IList coll = propertyInfo.GetValue(o, null) as IList;
                    DXMLNode node = SerializeCollection(propertyInfo.Name, coll);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList coll = propertyInfo.GetValue(o, null) as IList;
                    DXMLNode node = SerializeCollection(propertyInfo.Name, coll);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.Equals(typeof(string)))
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, propertyInfo.GetValue(o, null)));
            }
            foreach (FieldInfo fieldInfo in fields)
            {
                if (!fieldInfo.Name.Contains("k__BackingField") && !fieldInfo.Name.StartsWith("__"))
                {
                    if (fieldInfo.FieldType == typeof(RasterFont))
                    {
                        if (!(fieldInfo.GetValue(o) is RasterFont none))
                            none = RasterFont.None;
                        dxmlNode1.Add(new DXMLNode(fieldInfo.Name, none.Serialize()));
                    }
                    else if (fieldInfo.FieldType == typeof(StatBinding))
                    {
                        StatBinding statBinding = fieldInfo.GetValue(o) as StatBinding;
                        dxmlNode1.Add(new DXMLNode(fieldInfo.Name, statBinding.value));
                    }
                    else if (fieldInfo.FieldType == typeof(Resolution))
                    {
                        Resolution resolution = fieldInfo.GetValue(o) as Resolution;
                        DXMLNode dxmlNode2 = dxmlNode1;
                        string name = fieldInfo.Name;
                        string[] strArray = new string[5];
                        int num = resolution.x;
                        strArray[0] = num.ToString();
                        strArray[1] = "x";
                        num = resolution.y;
                        strArray[2] = num.ToString();
                        strArray[3] = "x";
                        num = (int)resolution.mode;
                        strArray[4] = num.ToString();
                        string varValue = string.Concat(strArray);
                        DXMLNode node = new DXMLNode(name, varValue);
                        dxmlNode2.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        IDictionary dict = fieldInfo.GetValue(o) as IDictionary;
                        DXMLNode node = SerializeDict(fieldInfo.Name, dict);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        IList coll = fieldInfo.GetValue(o) as IList;
                        DXMLNode node = SerializeCollection(fieldInfo.Name, coll);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList coll = fieldInfo.GetValue(o) as IList;
                        DXMLNode node = SerializeCollection(fieldInfo.Name, coll);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.Equals(typeof(string)))
                        dxmlNode1.Add(new DXMLNode(fieldInfo.Name, fieldInfo.GetValue(o)));
                }
            }
            return dxmlNode1;
        }

        public static object ReadValue(string value, Type t)
        {
            if (t == typeof(string))
                return value;
            if (t == typeof(float))
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            if (t == typeof(double))
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            if (t == typeof(byte))
                return Convert.ToByte(value, CultureInfo.InvariantCulture);
            if (t == typeof(short))
                return Convert.ToInt16(value, CultureInfo.InvariantCulture);
            if (t == typeof(int))
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            if (t == typeof(long))
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            return t == typeof(ulong) ? Convert.ToUInt64(value, CultureInfo.InvariantCulture) : null;
        }

        private static void DeserializeDict(
          IDictionary dict,
          DXMLNode element,
          Type keyType,
          Type valType)
        {
            dict.Clear();
            DXMLNode dxmlNode = element.Element("valueString");
            if (dxmlNode != null)
            {
                string str1 = dxmlNode.Value;
                char[] chArray1 = new char[1] { '@' };
                foreach (string str2 in str1.Split(chArray1))
                {
                    char[] chArray2 = new char[1] { '|' };
                    string[] strArray = str2.Split(chArray2);
                    if (strArray.Length == 2)
                    {
                        try
                        {
                            dict[ReadValue(strArray[0].Trim(), keyType)] = ReadValue(strArray[1].Trim(), valType);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            else
            {
                foreach (DXMLNode element1 in element.Elements())
                {
                    if (element1.Elements().Count() == 2)
                    {
                        object key = ReadValue(element1.Elements().ElementAt(0).Value, keyType);
                        object obj = ReadValue(element1.Elements().ElementAt(1).Value, valType);
                        if (key != null && obj != null)
                            dict[key] = obj;
                    }
                }
            }
        }

        private static void DeserializeCollection(IList dict, DXMLNode element, Type keyType)
        {
            dict.Clear();
            DXMLNode dxmlNode = element.Element("valueString");
            if (dxmlNode == null)
                return;
            string str1 = dxmlNode.Value;
            char[] chArray = new char[1] { '|' };
            foreach (string str2 in str1.Split(chArray))
            {
                try
                {
                    dict.Add(ReadValue(str2.Trim(), keyType));
                }
                catch (Exception)
                {
                }
            }
        }

        public static void DeserializeClass(object output, DXMLNode node)
        {
            if (output == null)
                return;
            Type type = output.GetType();
            foreach (DXMLNode element in node.Elements())
            {
                try
                {
                    PropertyInfo property = type.GetProperty(element.Name);
                    if (property != null)
                    {
                        if (property.PropertyType == typeof(RasterFont))
                            property.SetValue(output, RasterFont.Deserialize(element.Value));
                        else if (property.PropertyType == typeof(StatBinding))
                        {
                            if (Steam.IsInitialized())
                            {
                                if (Steam.GetStat(property.Name) >= -99999.0f)
                                    continue;
                            }
                            if (property.GetValue(output, null) is StatBinding statBinding)
                            {
                                if (statBinding.isFloat)
                                    statBinding.valueFloat = Convert.ToSingle(element.Value);
                                else
                                    statBinding.valueInt = Convert.ToInt32(element.Value);
                            }
                        }
                        else if (property.PropertyType == typeof(Resolution))
                            property.SetValue(output, Resolution.Load(element.Value, property.Name));
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            Type[] genericArguments = property.PropertyType.GetGenericArguments();
                            Type keyType = genericArguments[0];
                            Type valType = genericArguments[1];
                            DeserializeDict(property.GetValue(output, null) as IDictionary, element, keyType, valType);
                        }
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            Type genericArgument = property.PropertyType.GetGenericArguments()[0];
                            DeserializeCollection(property.GetValue(output, null) as IList, element, genericArgument);
                        }
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type genericArgument = property.PropertyType.GetGenericArguments()[0];
                            DeserializeCollection(property.GetValue(output, null) as IList, element, genericArgument);
                        }
                        else
                        {
                            if (!property.PropertyType.IsPrimitive)
                            {
                                if (!property.PropertyType.Equals(typeof(string)))
                                    continue;
                            }
                            property.SetValue(output, Convert.ChangeType(element.Value, property.PropertyType, CultureInfo.InvariantCulture), null);
                        }
                    }
                    else
                    {
                        FieldInfo field = type.GetField(element.Name);
                        if (field != null)
                        {
                            if (field.FieldType == typeof(RasterFont))
                                field.SetValue(output, RasterFont.Deserialize(element.Value));
                            else if (field.FieldType == typeof(StatBinding))
                            {
                                if (Steam.IsInitialized())
                                {
                                    if (Steam.GetStat(field.Name) >= -99999f)
                                        continue;
                                }
                                if (field.GetValue(output) is StatBinding statBinding)
                                {
                                    if (statBinding.isFloat)
                                        statBinding.valueFloat = Convert.ToSingle(element.Value);
                                    else
                                        statBinding.valueInt = Convert.ToInt32(element.Value);
                                }
                            }
                            else if (field.FieldType == typeof(Resolution))
                                field.SetValue(output, Resolution.Load(element.Value, property.Name));
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                            {
                                Type[] genericArguments = field.FieldType.GetGenericArguments();
                                Type keyType = genericArguments[0];
                                Type valType = genericArguments[1];
                                DeserializeDict(field.GetValue(output) as IDictionary, element, keyType, valType);
                            }
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                Type genericArgument = field.FieldType.GetGenericArguments()[0];
                                DeserializeCollection(field.GetValue(output) as IList, element, genericArgument);
                            }
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                Type genericArgument = field.FieldType.GetGenericArguments()[0];
                                DeserializeCollection(field.GetValue(output) as IList, element, genericArgument);
                            }
                            else
                            {
                                if (!field.FieldType.IsPrimitive)
                                {
                                    if (!field.FieldType.Equals(typeof(string)))
                                        continue;
                                }
                                field.SetValue(output, Convert.ChangeType(element.Value, field.FieldType, CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
                catch
                {
                    Program.LogLine("Error parsing data value in " + type.ToString() + " (" + element.Name + ")");
                }
            }
        }

        public virtual DXMLNode Serialize() => SerializeClass(this, _nodeName);

        public virtual bool Deserialize(DXMLNode node)
        {
            DeserializeClass(this, node);
            return true;
        }

        public static DataClass operator -(DataClass value1, DataClass value2)
        {
            DataClass instance = Activator.CreateInstance(value1.GetType(), null) as DataClass;
            foreach (PropertyInfo property in value1.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(int))
                {
                    int num1 = (int)property.GetValue(value1, null);
                    int num2 = (int)property.GetValue(value2, null);
                    property.SetValue(instance, num1 - num2, null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    float num3 = (float)property.GetValue(value1, null);
                    float num4 = (float)property.GetValue(value2, null);
                    property.SetValue(instance, num3 - num4, null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime1 = (DateTime)property.GetValue(value1, null);
                    DateTime dateTime2 = (DateTime)property.GetValue(value2, null);
                    property.SetValue(instance, dateTime2, null);
                }
                else
                    property.SetValue(instance, property.GetValue(value2, null), null);
            }
            return instance;
        }

        public static DataClass operator +(DataClass value1, DataClass value2)
        {
            DataClass instance = Activator.CreateInstance(value1.GetType(), null) as DataClass;
            foreach (PropertyInfo property in value1.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(int))
                {
                    int num1 = (int)property.GetValue(value1, null);
                    int num2 = (int)property.GetValue(value2, null);
                    property.SetValue(instance, num1 + num2, null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    float num3 = (float)property.GetValue(value1, null);
                    float num4 = (float)property.GetValue(value2, null);
                    property.SetValue(instance, num3 + num4, null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime1 = (DateTime)property.GetValue(value1, null);
                    DateTime dateTime2 = (DateTime)property.GetValue(value2, null);
                    if (dateTime1 > dateTime2)
                        property.SetValue(instance, dateTime1, null);
                    else
                        property.SetValue(instance, dateTime2, null);
                }
                else
                    property.SetValue(instance, property.GetValue(value2, null), null);
            }
            return instance;
        }
    }
}
