// Decompiled with JetBrains decompiler
// Type: DuckGame.ForegroundTile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ForegroundTile : Thing
    {
        public ForegroundTile(float xpos, float ypos)
          : base(xpos, ypos)
        {
            layer = Layer.Foreground;
            _isStatic = true;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("frame", (graphic as SpriteMap).frame);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            (graphic as SpriteMap).frame = node.GetProperty<int>("frame");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("frame", (graphic as SpriteMap).frame));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("frame");
            if (dxmlNode != null)
                (graphic as SpriteMap).frame = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
