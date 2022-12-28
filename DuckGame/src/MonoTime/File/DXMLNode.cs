// Decompiled with JetBrains decompiler
// Type: DuckGame.DXMLNode
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DXMLNode
    {
        public static int toStringDeep;
        private string _name = "";
        private string _value = "";
        private List<DXMLNode> _elements = new List<DXMLNode>();
        private List<DXMLAttribute> _attributes = new List<DXMLAttribute>();

        private string StoreValue(string value) => value.Replace("<", "@04202@").Replace(">", "@03019@").Replace("/", "@02032@");

        private string ReadValue(string value) => value.Replace("@04202@", "<").Replace("@03019@", ">").Replace("@02032@", "/");

        public override string ToString()
        {
            string str1 = "";
            string str2 = "";
            for (int index = 0; index < toStringDeep; ++index)
                str2 += "  ";
            if (Name != "")
            {
                str1 = str1 + str2 + "<" + Name;
                foreach (DXMLAttribute attribute in Attributes())
                    str1 = str1 + " " + attribute.Name + "=" + attribute.Value;
            }
            if (NumberOfElements > 0 || Value != "")
            {
                if (Value != "")
                {
                    str1 = str1 + ">" + StoreValue(Value) + "</" + Name + ">\r\n";
                }
                else
                {
                    if (Name != "")
                    {
                        str1 += ">\r\n";
                        ++toStringDeep;
                    }
                    foreach (DXMLNode element in Elements())
                        str1 += element.ToString();
                    if (Name != "")
                    {
                        --toStringDeep;
                        str1 = str1 + str2 + "</" + Name + ">\r\n";
                    }
                }
            }
            else if (Name != "")
                str1 += "/>\r\n";
            return str1;
        }

        public string Name => _name;

        public string Value => _value;

        public int NumberOfElements => _elements.Count;

        public void SetValue(string varValue) => _value = ReadValue(varValue);

        public DXMLNode(string varName) => _name = varName;

        public DXMLNode(string varName, object varValue)
        {
            _name = varName;
            if (varValue == null)
                return;
            _value = varValue.ToString();
        }

        protected static DXMLNode ReadNode(string text, ref int index)
        {
            if (text == null || text.Length <= index)
                return null;
            if (text[index] == '<' || TextParser.ReadNext('<', text, ref index) != null)
            {
                string s = TextParser.ReadTo('>', text, ref index);
                bool flag = false;
                int index1 = 0;
                string varName1 = TextParser.ReadNextWord(s, ref index1, new char?('/'));
                DXMLNode dxmlNode = new DXMLNode(varName1);
                while (index1 != s.Length)
                {
                    string varName2 = TextParser.ReadTo('=', s, ref index1).Trim();
                    if (varName2 == "/")
                    {
                        flag = true;
                        break;
                    }
                    if (varName2 == "?")
                    {
                        flag = true;
                        break;
                    }
                    if (!(varName2 == ""))
                    {
                        string varValue = TextParser.ReadNextWordBetween('"', s, ref index1);
                        DXMLAttribute attribute = new DXMLAttribute(varName2, varValue);
                        dxmlNode.Add(attribute);
                    }
                    else
                        break;
                }
                if (flag)
                    return dxmlNode;
                ++index;
                string varValue1 = TextParser.ReadTo('<', text, ref index).Trim();
                int index2 = index;
                if (TextParser.ReadNextCharacter(text, ref index2) == '!' && TextParser.ReadNextWord(text, ref index2) == "![CDATA[")
                {
                    int num = text.IndexOf("]]>", index2, text.Length - index2);
                    if (num != -1)
                    {
                        varValue1 = text.Substring(index2, num - index2);
                        index = num + 2;
                    }
                }
                dxmlNode.SetValue(varValue1);
                while (TextParser.ReadNext('<', text, ref index) != null)
                {
                    --index;
                    index2 = index;
                    if (TextParser.ReadNextWord(text, ref index2) == "/" + varName1)
                    {
                        index = index2;
                        return dxmlNode;
                    }
                    DXMLNode node = ReadNode(text, ref index);
                    dxmlNode.Add(node);
                }
            }
            return null;
        }

        public void Add(DXMLNode node) => _elements.Add(node);

        public void Add(DXMLAttribute attribute) => _attributes.Add(attribute);

        public IEnumerable<DXMLNode> Elements() => _elements.AsEnumerable();

        public IEnumerable<DXMLNode> Elements(string varName)
        {
            List<DXMLNode> dxmlNodeList = new List<DXMLNode>();
            foreach (DXMLNode element in _elements)
            {
                if (element.Name == varName)
                    dxmlNodeList.Add(element);
            }
            return dxmlNodeList;
        }

        public DXMLNode Element(string varName)
        {
            List<DXMLNode> dxmlNodeList = new List<DXMLNode>();
            foreach (DXMLNode element in _elements)
            {
                if (element.Name == varName)
                    return element;
            }
            return null;
        }

        public IEnumerable<DXMLAttribute> Attributes(string varName)
        {
            List<DXMLAttribute> dxmlAttributeList = new List<DXMLAttribute>();
            foreach (DXMLAttribute attribute in _attributes)
            {
                if (attribute.Name == varName)
                    dxmlAttributeList.Add(attribute);
            }
            return dxmlAttributeList;
        }

        public IEnumerable<DXMLAttribute> Attributes() => _attributes.AsEnumerable();
    }
}
