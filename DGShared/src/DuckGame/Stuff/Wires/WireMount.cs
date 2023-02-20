// Decompiled with JetBrains decompiler
// Type: DuckGame.WireMount
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class WireMount : Thing, IWirePeripheral
    {
        private SpriteMap _sprite;
        public StateBinding _containedThingBinding = new StateBinding(nameof(_containedThing));
        public StateBinding _actionBinding = new WireMountFlagBinding();
        public Thing _containedThing;
        public EditorProperty<bool> infinite = new EditorProperty<bool>(false);
        private System.Type _contains;
        public EditorProperty<float> mountAngle = new EditorProperty<float>(0f, min: -360f, max: 360f, increment: 5f);
        public bool newFlipType = true;

        public System.Type contains
        {
            get => _contains;
            set
            {
                if (_contains != value && value != null)
                {
                    _containedThing = Editor.CreateObject(value) as Thing;
                    if (_containedThing != null && _containedThing is Gun)
                        (_containedThing as Gun).infinite.value = infinite.value;
                }
                _contains = value;
            }
        }

        public override void PrepareForHost()
        {
            base.PrepareForHost();
            if (_containedThing == null)
                return;
            _containedThing.PrepareForHost();
        }

        public WireMount(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wireMount", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Wire Mount";
            editorTooltip = "Specifies an object to trigger whenever a connected Button is pressed.";
            layer = Layer.Foreground;
            _canFlip = true;
            _placementCost += 4;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            binaryClassChunk.AddProperty("newFlipType", newFlipType);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            newFlipType = node.GetProperty<bool>("newFlipType");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("contains", contains != null ? contains.AssemblyQualifiedName : (object)""));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("contains");
            if (dxmlNode != null)
                contains = Editor.GetType(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            return contextMenu;
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (contains != null)
                str = contains.Name;
            return contains == null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (contains != null)
                text = contains.Name;
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor) && _containedThing != null)
            {
                _containedThing.owner = this;
                Level.Add(_containedThing);
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (_containedThing != null)
            {
                _containedThing.owner = this;
                if (_containedThing.removeFromLevel)
                {
                    _containedThing = null;
                }
                else
                {
                    _containedThing.offDir = flipHorizontal ? (sbyte)-1 : (sbyte)1;
                    _containedThing.position = position;
                    _containedThing.depth = depth + 10;
                    _containedThing.layer = layer;
                    if (newFlipType)
                        _containedThing.angleDegrees = flipHorizontal ? -mountAngle.value : mountAngle.value;
                    else
                        _containedThing.angleDegrees = mountAngle.value;
                    if (_containedThing is Gun)
                    {
                        Gun containedThing1 = _containedThing as Gun;
                        Vec2 vec2 = -containedThing1.barrelVector * (containedThing1.kick * 5f);
                        Thing containedThing2 = _containedThing;
                        containedThing2.position += vec2;
                    }
                }
            }
            base.Update();
        }

        public override void Terminate() => base.Terminate();

        public override void Draw()
        {
            if (_containedThing != null && Level.current is Editor)
            {
                _containedThing.offDir = flipHorizontal ? (sbyte)-1 : (sbyte)1;
                _containedThing.position = position;
                _containedThing.depth = depth + 10;
                _containedThing.layer = layer;
                if (newFlipType)
                    _containedThing.angleDegrees = flipHorizontal ? -mountAngle.value : mountAngle.value;
                else
                    _containedThing.angleDegrees = mountAngle.value;
                _containedThing.DoEditorUpdate();
                _containedThing.Draw();
            }
            base.Draw();
        }

        public void Pulse(int type, WireTileset wire)
        {
            Fondle(this, DuckNetwork.localConnection);
            if (!(_containedThing is Holdable containedThing))
                return;
            Fondle(containedThing, DuckNetwork.localConnection);
            switch (type)
            {
                case 0:
                    action = true;
                    containedThing.UpdateAction();
                    action = false;
                    containedThing.UpdateAction();
                    break;
                case 1:
                    action = true;
                    break;
                case 2:
                    action = false;
                    break;
            }
        }
    }
}
