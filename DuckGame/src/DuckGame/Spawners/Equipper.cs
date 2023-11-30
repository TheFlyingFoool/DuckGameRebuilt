using System.Linq;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isOnlineCapable", true)]
    public class Equipper : Thing
    {
        private System.Type _previewType;
        private System.Type _contains;
        public EditorProperty<int> radius = new EditorProperty<int>(0, max: 128f, increment: 1f, minSpecial: "INF");
        public EditorProperty<bool> infinite = new EditorProperty<bool>(false);
        public EditorProperty<bool> holstered = new EditorProperty<bool>(false);
        public EditorProperty<bool> powerHolstered = new EditorProperty<bool>(false);
        public EditorProperty<bool> holsterChained = new EditorProperty<bool>(false);
        private RenderTarget2D _preview;
        private Sprite _previewSprite;

        public System.Type contains
        {
            get => _contains;
            set => _contains = value;
        }

        public Equipper(float xpos, float ypos)
          : base(xpos, ypos)
        {
            serverOnly = true;
            graphic = new Sprite("equipper");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(14f, 14f);
            collisionOffset = new Vec2(-7f, -7f);
            depth = (Depth)0.5f;
            _canFlip = false;
            _visibleInGame = false;
            editorTooltip = "Allows equipment to automatically be equipped to all ducks on level start.";
            _placementCost += 4;
        }

        public Thing GetContainedInstance(Vec2 pos = default(Vec2))
        {
            if (contains == null)
                return null;
            object[] constructorParameters = Editor.GetConstructorParameters(contains);
            if (constructorParameters.Length > 1)
            {
                constructorParameters[0] = pos.x;
                constructorParameters[1] = pos.y;
            }
            PhysicsObject thing = Editor.CreateThing(contains, constructorParameters) as PhysicsObject;
            if (thing is Gun)
                (thing as Gun).infinite = infinite;
            return thing;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
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

        public override void EditorUpdate()
        {
            if (_previewSprite == null || _previewType != contains)
            {
                if (_preview == null)
                    _preview = new RenderTarget2D(32, 32);
                Thing containedInstance = GetContainedInstance();
                if (containedInstance != null)
                    _previewSprite = containedInstance.GetEditorImage(32, 32, true, target: _preview);
                _previewType = contains;
            }
            base.EditorUpdate();
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (contains != null)
                text = contains.Name;
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2), -16f), Color.White, (Depth)0.9f);
            if (radius.value == 0)
                return;
            Graphics.DrawCircle(position, radius.value, Color.Red, depth: ((Depth)0.9f));
        }

        public override void Draw()
        {
            base.Draw();
            if (_previewSprite == null)
                return;
            _previewSprite.depth = depth + 1;
            _previewSprite.scale = new Vec2(0.5f, 0.5f);
            _previewSprite.CenterOrigin();
            Graphics.Draw(_previewSprite, x, y);
        }
    }
}
