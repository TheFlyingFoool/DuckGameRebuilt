// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckXMLExtensionMethods
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public static class DuckXMLExtensionMethods
    {
        public static IEnumerable<DXMLAttribute> Attributes<T>(
          this IEnumerable<T> source,
          string name)
          where T : DXMLNode
        {
            List<DXMLAttribute> dxmlAttributeList = new List<DXMLAttribute>();
            foreach (T obj in source)
            {
                DXMLNode dxmlNode = obj;
                foreach (DXMLAttribute attribute in dxmlNode.Attributes())
                {
                    if (attribute.Name == name)
                        dxmlAttributeList.Add(attribute);
                }
                IEnumerable<DXMLAttribute> collection = dxmlNode.Elements().Attributes<DXMLNode>(name);
                dxmlAttributeList.AddRange(collection);
            }
            return dxmlAttributeList;
        }

        public static IEnumerable<DXMLNode> Elements<T>(this IEnumerable<T> source) where T : DXMLNode
        {
            List<DXMLNode> dxmlNodeList = new List<DXMLNode>();
            foreach (T obj in source)
            {
                DXMLNode dxmlNode = obj;
                dxmlNodeList.Add(dxmlNode);
                IEnumerable<DXMLNode> collection = dxmlNode.Elements().Elements<DXMLNode>();
                dxmlNodeList.AddRange(collection);
            }
            return dxmlNodeList;
        }

        public static IEnumerable<DXMLNode> Elements<T>(
          this IEnumerable<T> source,
          string name)
          where T : DXMLNode
        {
            List<DXMLNode> dxmlNodeList = new List<DXMLNode>();
            foreach (T obj in source)
            {
                DXMLNode dxmlNode = obj;
                if (dxmlNode.Name == name)
                    dxmlNodeList.Add(dxmlNode);
                IEnumerable<DXMLNode> collection = dxmlNode.Elements().Elements<DXMLNode>(name);
                dxmlNodeList.AddRange(collection);
            }
            return dxmlNodeList;
        }
    }
}
