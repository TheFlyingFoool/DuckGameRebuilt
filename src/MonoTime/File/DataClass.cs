// Decompiled with JetBrains decompiler
// Type: DuckGame.DataClass
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                return (DXMLNode)null;
            string varValue = "";
            foreach (object key in (IEnumerable)dict.Keys)
                varValue = varValue + Convert.ToString(key) + "|" + Convert.ToString(dict[key]) + "@";
            DXMLNode dxmlNode = new DXMLNode(name);
            dxmlNode.Add(new DXMLNode("valueString", (object)varValue));
            return dxmlNode;
        }

        private static DXMLNode SerializeCollection(string name, IList coll)
        {
            if (coll.Count <= 0)
                return (DXMLNode)null;
            string str = "";
            foreach (object obj in (IEnumerable)coll)
                str = str + Convert.ToString(obj) + "|";
            string varValue = str.Substring(0, str.Length - 1);
            DXMLNode dxmlNode = new DXMLNode(name);
            dxmlNode.Add(new DXMLNode("valueString", (object)varValue));
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
                    if (!(propertyInfo.GetValue(o, (object[])null) is RasterFont none))
                        none = RasterFont.None;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, (object)none.Serialize()));
                }
                else if (propertyInfo.PropertyType == typeof(StatBinding))
                {
                    StatBinding statBinding = propertyInfo.GetValue(o, (object[])null) as StatBinding;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, statBinding.value));
                }
                else if (propertyInfo.PropertyType == typeof(Resolution))
                {
                    Resolution resolution = propertyInfo.GetValue(o, (object[])null) as Resolution;
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, (object)(resolution.x.ToString() + "x" + resolution.y.ToString() + "x" + ((int)resolution.mode).ToString())));
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    IDictionary dict = propertyInfo.GetValue(o, (object[])null) as IDictionary;
                    DXMLNode node = DataClass.SerializeDict(propertyInfo.Name, dict);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    IList coll = propertyInfo.GetValue(o, (object[])null) as IList;
                    DXMLNode node = DataClass.SerializeCollection(propertyInfo.Name, coll);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList coll = propertyInfo.GetValue(o, (object[])null) as IList;
                    DXMLNode node = DataClass.SerializeCollection(propertyInfo.Name, coll);
                    if (node != null)
                        dxmlNode1.Add(node);
                }
                else if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.Equals(typeof(string)))
                    dxmlNode1.Add(new DXMLNode(propertyInfo.Name, propertyInfo.GetValue(o, (object[])null)));
            }
            foreach (FieldInfo fieldInfo in fields)
            {
                if (!fieldInfo.Name.Contains("k__BackingField") && !fieldInfo.Name.StartsWith("__"))
                {
                    if (fieldInfo.FieldType == typeof(RasterFont))
                    {
                        if (!(fieldInfo.GetValue(o) is RasterFont none))
                            none = RasterFont.None;
                        dxmlNode1.Add(new DXMLNode(fieldInfo.Name, (object)none.Serialize()));
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
                        DXMLNode node = new DXMLNode(name, (object)varValue);
                        dxmlNode2.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        IDictionary dict = fieldInfo.GetValue(o) as IDictionary;
                        DXMLNode node = DataClass.SerializeDict(fieldInfo.Name, dict);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        IList coll = fieldInfo.GetValue(o) as IList;
                        DXMLNode node = DataClass.SerializeCollection(fieldInfo.Name, coll);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList coll = fieldInfo.GetValue(o) as IList;
                        DXMLNode node = DataClass.SerializeCollection(fieldInfo.Name, coll);
                        if (node != null)
                            dxmlNode1.Add(node);
                    }
                    else if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.Equals(typeof(string)))
                        dxmlNode1.Add(new DXMLNode(fieldInfo.Name, fieldInfo.GetValue(o)));
                }
            }
            return dxmlNode1;
        }

        public static object ReadValue(string value, System.Type t)
        {
            if (t == typeof(string))
                return (object)value;
            if (t == typeof(float))
                return (object)Convert.ToSingle(value, (IFormatProvider)CultureInfo.InvariantCulture);
            if (t == typeof(double))
                return (object)Convert.ToDouble(value, (IFormatProvider)CultureInfo.InvariantCulture);
            if (t == typeof(byte))
                return (object)Convert.ToByte(value, (IFormatProvider)CultureInfo.InvariantCulture);
            if (t == typeof(short))
                return (object)Convert.ToInt16(value, (IFormatProvider)CultureInfo.InvariantCulture);
            if (t == typeof(int))
                return (object)Convert.ToInt32(value, (IFormatProvider)CultureInfo.InvariantCulture);
            if (t == typeof(long))
                return (object)Convert.ToInt64(value, (IFormatProvider)CultureInfo.InvariantCulture);
            return t == typeof(ulong) ? (object)Convert.ToUInt64(value, (IFormatProvider)CultureInfo.InvariantCulture) : (object)null;
        }

        private static void DeserializeDict(
          IDictionary dict,
          DXMLNode element,
          System.Type keyType,
          System.Type valType)
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
                            dict[DataClass.ReadValue(strArray[0].Trim(), keyType)] = DataClass.ReadValue(strArray[1].Trim(), valType);
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
                    if (element1.Elements().Count<DXMLNode>() == 2)
                    {
                        object key = DataClass.ReadValue(element1.Elements().ElementAt<DXMLNode>(0).Value, keyType);
                        object obj = DataClass.ReadValue(element1.Elements().ElementAt<DXMLNode>(1).Value, valType);
                        if (key != null && obj != null)
                            dict[key] = obj;
                    }
                }
            }
        }

        private static void DeserializeCollection(IList dict, DXMLNode element, System.Type keyType)
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
                    dict.Add(DataClass.ReadValue(str2.Trim(), keyType));
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
            System.Type type = output.GetType();
            foreach (DXMLNode element in node.Elements())
            {
                try
                {
                    PropertyInfo property = type.GetProperty(element.Name);
                    if (property != (PropertyInfo)null)
                    {
                        if (property.PropertyType == typeof(RasterFont))
                            property.SetValue(output, (object)RasterFont.Deserialize(element.Value));
                        else if (property.PropertyType == typeof(StatBinding))
                        {
                            if (Steam.IsInitialized())
                            {
                                if ((double)Steam.GetStat(property.Name) >= -99999.0)
                                    continue;
                            }
                            if (property.GetValue(output, (object[])null) is StatBinding statBinding)
                            {
                                if (statBinding.isFloat)
                                    statBinding.valueFloat = Convert.ToSingle(element.Value);
                                else
                                    statBinding.valueInt = Convert.ToInt32(element.Value);
                            }
                        }
                        else if (property.PropertyType == typeof(Resolution))
                            property.SetValue(output, (object)Resolution.Load(element.Value, property.Name));
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            System.Type[] genericArguments = property.PropertyType.GetGenericArguments();
                            System.Type keyType = genericArguments[0];
                            System.Type valType = genericArguments[1];
                            DataClass.DeserializeDict(property.GetValue(output, (object[])null) as IDictionary, element, keyType, valType);
                        }
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            System.Type genericArgument = property.PropertyType.GetGenericArguments()[0];
                            DataClass.DeserializeCollection(property.GetValue(output, (object[])null) as IList, element, genericArgument);
                        }
                        else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            System.Type genericArgument = property.PropertyType.GetGenericArguments()[0];
                            DataClass.DeserializeCollection(property.GetValue(output, (object[])null) as IList, element, genericArgument);
                        }
                        else
                        {
                            if (!property.PropertyType.IsPrimitive)
                            {
                                if (!property.PropertyType.Equals(typeof(string)))
                                    continue;
                            }
                            property.SetValue(output, Convert.ChangeType((object)element.Value, property.PropertyType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
                        }
                    }
                    else
                    {
                        FieldInfo field = type.GetField(element.Name);
                        if (field != (FieldInfo)null)
                        {
                            if (field.FieldType == typeof(RasterFont))
                                field.SetValue(output, (object)RasterFont.Deserialize(element.Value));
                            else if (field.FieldType == typeof(StatBinding))
                            {
                                if (Steam.IsInitialized())
                                {
                                    if ((double)Steam.GetStat(field.Name) >= -99999.0)
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
                                field.SetValue(output, (object)Resolution.Load(element.Value, property.Name));
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                            {
                                System.Type[] genericArguments = field.FieldType.GetGenericArguments();
                                System.Type keyType = genericArguments[0];
                                System.Type valType = genericArguments[1];
                                DataClass.DeserializeDict(field.GetValue(output) as IDictionary, element, keyType, valType);
                            }
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                System.Type genericArgument = field.FieldType.GetGenericArguments()[0];
                                DataClass.DeserializeCollection(field.GetValue(output) as IList, element, genericArgument);
                            }
                            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                System.Type genericArgument = field.FieldType.GetGenericArguments()[0];
                                DataClass.DeserializeCollection(field.GetValue(output) as IList, element, genericArgument);
                            }
                            else
                            {
                                if (!field.FieldType.IsPrimitive)
                                {
                                    if (!field.FieldType.Equals(typeof(string)))
                                        continue;
                                }
                                field.SetValue(output, Convert.ChangeType((object)element.Value, field.FieldType, (IFormatProvider)CultureInfo.InvariantCulture));
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

        public virtual DXMLNode Serialize() => DataClass.SerializeClass((object)this, this._nodeName);

        public virtual bool Deserialize(DXMLNode node)
        {
            DataClass.DeserializeClass((object)this, node);
            return true;
        }

        public static DataClass operator -(DataClass value1, DataClass value2)
        {
            DataClass instance = Activator.CreateInstance(value1.GetType(), (object[])null) as DataClass;
            foreach (PropertyInfo property in value1.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(int))
                {
                    int num1 = (int)property.GetValue((object)value1, (object[])null);
                    int num2 = (int)property.GetValue((object)value2, (object[])null);
                    property.SetValue((object)instance, (object)(num1 - num2), (object[])null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    float num3 = (float)property.GetValue((object)value1, (object[])null);
                    float num4 = (float)property.GetValue((object)value2, (object[])null);
                    property.SetValue((object)instance, (object)(float)((double)num3 - (double)num4), (object[])null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime1 = (DateTime)property.GetValue((object)value1, (object[])null);
                    DateTime dateTime2 = (DateTime)property.GetValue((object)value2, (object[])null);
                    property.SetValue((object)instance, (object)dateTime2, (object[])null);
                }
                else
                    property.SetValue((object)instance, property.GetValue((object)value2, (object[])null), (object[])null);
            }
            return instance;
        }

        public static DataClass operator +(DataClass value1, DataClass value2)
        {
            DataClass instance = Activator.CreateInstance(value1.GetType(), (object[])null) as DataClass;
            foreach (PropertyInfo property in value1.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(int))
                {
                    int num1 = (int)property.GetValue((object)value1, (object[])null);
                    int num2 = (int)property.GetValue((object)value2, (object[])null);
                    property.SetValue((object)instance, (object)(num1 + num2), (object[])null);
                }
                else if (property.PropertyType == typeof(float))
                {
                    float num3 = (float)property.GetValue((object)value1, (object[])null);
                    float num4 = (float)property.GetValue((object)value2, (object[])null);
                    property.SetValue((object)instance, (object)(float)((double)num3 + (double)num4), (object[])null);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime1 = (DateTime)property.GetValue((object)value1, (object[])null);
                    DateTime dateTime2 = (DateTime)property.GetValue((object)value2, (object[])null);
                    if (dateTime1 > dateTime2)
                        property.SetValue((object)instance, (object)dateTime1, (object[])null);
                    else
                        property.SetValue((object)instance, (object)dateTime2, (object[])null);
                }
                else
                    property.SetValue((object)instance, property.GetValue((object)value2, (object[])null), (object[])null);
            }
            return instance;
        }
    }
}
