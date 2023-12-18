using System.Collections.Generic;

namespace DuckGame
{
    public class LevelPlaylist
    {
        public List<string> levels = new List<string>();

        public DXMLNode Serialize()
        {
            DXMLNode dxmlNode = new DXMLNode("playlist");
            foreach (object level in levels)
            {
                DXMLNode node = new DXMLNode("element", level);
                dxmlNode.Add(node);
            }
            return dxmlNode;
        }

        public void Deserialize(DXMLNode node)
        {
            levels.Clear();
            foreach (DXMLNode element in node.Elements())
            {
                if (element.Name == "element" && DuckFile.FileExists(element.Value))
                    levels.Add(element.Value);
            }
        }
    }
}
