// Decompiled with JetBrains decompiler
// Type: DuckGame.WireMount
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        public EditorProperty<float> mountAngle = new EditorProperty<float>(0.0f, min: -360f, max: 360f, increment: 5f);
        public bool newFlipType = true;

        public System.Type contains
        {
            get => this._contains;
            set
            {
                if (this._contains != value && value != null)
                {
                    this._containedThing = Editor.CreateObject(value) as Thing;
                    if (this._containedThing != null && this._containedThing is Gun)
                        (this._containedThing as Gun).infinite.value = this.infinite.value;
                }
                this._contains = value;
            }
        }

        public override void PrepareForHost()
        {
            base.PrepareForHost();
            if (this._containedThing == null)
                return;
            this._containedThing.PrepareForHost();
        }

        public WireMount(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("wireMount", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
            this._editorName = "Wire Mount";
            this.editorTooltip = "Specifies an object to trigger whenever a connected Button is pressed.";
            this.layer = Layer.Foreground;
            this._canFlip = true;
            this._placementCost += 4;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(this.contains));
            binaryClassChunk.AddProperty("newFlipType", newFlipType);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            this.newFlipType = node.GetProperty<bool>("newFlipType");
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

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (this.contains != null)
                text = this.contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-(double)Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor) && this._containedThing != null)
            {
                this._containedThing.owner = this;
                Level.Add(this._containedThing);
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (this._containedThing != null)
            {
                this._containedThing.owner = this;
                if (this._containedThing.removeFromLevel)
                {
                    this._containedThing = null;
                }
                else
                {
                    this._containedThing.offDir = this.flipHorizontal ? (sbyte)-1 : (sbyte)1;
                    this._containedThing.position = this.position;
                    this._containedThing.depth = this.depth + 10;
                    this._containedThing.layer = this.layer;
                    if (this.newFlipType)
                        this._containedThing.angleDegrees = this.flipHorizontal ? -this.mountAngle.value : this.mountAngle.value;
                    else
                        this._containedThing.angleDegrees = this.mountAngle.value;
                    if (this._containedThing is Gun)
                    {
                        Gun containedThing1 = this._containedThing as Gun;
                        Vec2 vec2 = -containedThing1.barrelVector * (containedThing1.kick * 5f);
                        Thing containedThing2 = this._containedThing;
                        containedThing2.position += vec2;
                    }
                }
            }
            base.Update();
        }

        public override void Terminate() => base.Terminate();

        public override void Draw()
        {
            if (this._containedThing != null && Level.current is Editor)
            {
                this._containedThing.offDir = this.flipHorizontal ? (sbyte)-1 : (sbyte)1;
                this._containedThing.position = this.position;
                this._containedThing.depth = this.depth + 10;
                this._containedThing.layer = this.layer;
                if (this.newFlipType)
                    this._containedThing.angleDegrees = this.flipHorizontal ? -this.mountAngle.value : this.mountAngle.value;
                else
                    this._containedThing.angleDegrees = this.mountAngle.value;
                this._containedThing.DoEditorUpdate();
                this._containedThing.Draw();
            }
            base.Draw();
        }

        public void Pulse(int type, WireTileset wire)
        {
            Thing.Fondle(this, DuckNetwork.localConnection);
            if (!(this._containedThing is Holdable containedThing))
                return;
            Thing.Fondle(containedThing, DuckNetwork.localConnection);
            switch (type)
            {
                case 0:
                    this.action = true;
                    containedThing.UpdateAction();
                    this.action = false;
                    containedThing.UpdateAction();
                    break;
                case 1:
                    this.action = true;
                    break;
                case 2:
                    this.action = false;
                    break;
            }
        }
    }
}
