// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelPlaylist
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
