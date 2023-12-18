using System;

namespace DuckGame
{
    public class SubBackgroundTile : Thing, IStaticRender
    {
        public SubBackgroundTile(float xpos, float ypos)
          : base(xpos, ypos)
        {
        }

        public override void Initialize()
        {
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
