// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraZoom
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => this._zoomMult;
            set => this._zoomMult = value;
        }

        public CameraZoom()
          : base()
        {
            this.graphic = new Sprite("swirl");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._canFlip = false;
            this._visibleInGame = false;
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
            this._zoomMult = node.GetProperty<float>("zoom");
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
                this._zoomMult = Convert.ToSingle(dxmlNode.Value);
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
