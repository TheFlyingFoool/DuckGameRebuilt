using System;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    public class CameraZoom : Thing
    {
        private float _zoomMult = 1f;
        public EditorProperty<bool> overFollow = new EditorProperty<bool>(false);
        public EditorProperty<bool> allowWarps = new EditorProperty<bool>(false);

        public float zoomMult
        {
            get => _zoomMult;
            set => _zoomMult = value;
        }

        public CameraZoom()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("zoom", _zoomMult);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            _zoomMult = node.GetProperty<float>("zoom");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("zoom", Change.ToString(_zoomMult)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("zoom");
            if (dxmlNode != null)
                _zoomMult = Convert.ToSingle(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu contextMenu = base.GetContextMenu();
            if (!(this is CameraZoomNew))
                contextMenu.AddItem(new ContextSlider("Zoom", null, new FieldBinding(this, "zoomMult", 0.5f, 4f), 0.1f));
            return contextMenu;
        }
    }
}
