// Decompiled with JetBrains decompiler
// Type: DuckGame.Equipper
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => this._contains;
            set => this._contains = value;
        }

        public Equipper(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.serverOnly = true;
            this.graphic = new Sprite("equipper");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(14f, 14f);
            this.collisionOffset = new Vec2(-7f, -7f);
            this.depth = (Depth)0.5f;
            this._canFlip = false;
            this._visibleInGame = false;
            this.editorTooltip = "Allows equipment to automatically be equipped to all ducks on level start.";
            this._placementCost += 4;
        }

        public Thing GetContainedInstance(Vec2 pos = default(Vec2))
        {
            if (this.contains == null)
                return null;
            object[] constructorParameters = Editor.GetConstructorParameters(this.contains);
            if (constructorParameters.Count<object>() > 1)
            {
                constructorParameters[0] = pos.x;
                constructorParameters[1] = pos.y;
            }
            PhysicsObject thing = Editor.CreateThing(this.contains, constructorParameters) as PhysicsObject;
            if (thing is Gun)
                (thing as Gun).infinite = this.infinite;
            return thing;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(this.contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("contains", this.contains != null ? contains.AssemblyQualifiedName : (object)""));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("contains");
            if (dxmlNode != null)
                this.contains = Editor.GetType(dxmlNode.Value);
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
            if (this.contains != null)
                str = this.contains.Name;
            return this.contains == null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void EditorUpdate()
        {
            if (this._previewSprite == null || this._previewType != this.contains)
            {
                if (this._preview == null)
                    this._preview = new RenderTarget2D(32, 32);
                Thing containedInstance = this.GetContainedInstance();
                if (containedInstance != null)
                    this._previewSprite = containedInstance.GetEditorImage(32, 32, true, target: this._preview);
                this._previewType = this.contains;
            }
            base.EditorUpdate();
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (this.contains != null)
                text = this.contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
            if (this.radius.value == 0)
                return;
            Graphics.DrawCircle(this.position, radius.value, Color.Red, depth: ((Depth)0.9f));
        }

        public override void Draw()
        {
            base.Draw();
            if (this._previewSprite == null)
                return;
            this._previewSprite.depth = this.depth + 1;
            this._previewSprite.scale = new Vec2(0.5f, 0.5f);
            this._previewSprite.CenterOrigin();
            Graphics.Draw(this._previewSprite, this.x, this.y);
        }
    }
}
