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
                IEnumerable<DXMLAttribute> collection = dxmlNode.Elements().Attributes(name);
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
                IEnumerable<DXMLNode> collection = dxmlNode.Elements().Elements();
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
                IEnumerable<DXMLNode> collection = dxmlNode.Elements().Elements(name);
                dxmlNodeList.AddRange(collection);
            }
            return dxmlNodeList;
        }
    }
}
