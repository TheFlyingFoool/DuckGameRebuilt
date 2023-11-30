namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class DebugMount : Thing
    {
        private SpriteMap _sprite;
        public StateBinding _containedThingBinding = new StateBinding(nameof(_containedThing));
        public StateBinding _actionBinding = new WireMountFlagBinding();
        public Thing _containedThing;
        public EditorProperty<bool> enabled = new EditorProperty<bool>(false);
        public EditorProperty<bool> infinite = new EditorProperty<bool>(false);
        public EditorProperty<bool> triggeraction = new EditorProperty<bool>(false);
        private System.Type _contains;
        public EditorProperty<int> framestopdelay = new EditorProperty<int>(0, min: 0f, max: 400f, increment: 1f);
        public EditorProperty<int> framestopduration = new EditorProperty<int>(0, min: 0f, max: 400f, increment: 1f);
        public EditorProperty<float> mountAngle = new EditorProperty<float>(0f, min: -360f, max: 360f, increment: 1f);
        public EditorProperty<float> rotation = new EditorProperty<float>(0f, min: -360f, max: 360f, increment: 1f);
        public bool newFlipType = true;

        public int framedelay = 1;
        public int framedelaycounter = 0;

        public int framestopdelaycounter = 0;
        public int framestopdurationcounter = 0;
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

        public DebugMount(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wireMount", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Debug Mount";
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
            binaryClassChunk.AddProperty("framedelay", framedelay);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            newFlipType = node.GetProperty<bool>("newFlipType");
            framedelay = node.GetProperty<int>("framedelay");
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
            contextMenu.AddItem(new ContextSlider("FireFrameDelay", null, new FieldBinding(this, "framedelay", 0f, 100f)));
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
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2f), -16f), Color.White, (Depth)0.9f);
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
            if (!enabled)
                return;
            mountAngle.value += rotation;
            if (mountAngle.value > 360f)
            {
                mountAngle.value -= 360f;
            }
            else if (mountAngle.value < -360f)
            {
                mountAngle.value += 360f;
            }
            if (_containedThing is Holdable containedThing)
            {
                framedelaycounter += 1;
                framestopdelaycounter += 1;
                if (framedelaycounter > framedelay && framestopdurationcounter == 0)
                {
                    framedelaycounter = 0;
                    // action = true;
                    if (triggeraction)
                    {
                        containedThing.triggerAction = true;
                    }
                    else
                    {
                        containedThing.PressAction();
                    }
                }
                else
                {
                    if (containedThing.triggerAction )
                        containedThing.triggerAction = false;
                }
                if (framestopdelaycounter > framestopdelay)
                {
                    framestopdurationcounter += 1;
                    if (framestopdurationcounter > (int)framestopduration)
                    {
                        framestopdurationcounter = 0;
                        framestopdelaycounter = 0;
                    }

                }
            }
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
