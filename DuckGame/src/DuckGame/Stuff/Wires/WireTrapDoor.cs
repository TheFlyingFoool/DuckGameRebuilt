namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class WireTrapDoor : Block, IWirePeripheral
    {
        public StateBinding _openBinding = new StateBinding(nameof(_open));
        public bool _open;
        private Thing _shutter;
        private SpriteMap _sprite;
        public EditorProperty<int> length;
        public EditorProperty<int> color;
        public EditorProperty<bool> open;
        //private bool _lastFlip;
        public EditorProperty<bool> fallthrough;
        private bool _lastFallthrough = true;
        public bool newTrapdoorVersion = true;

        public override bool flipHorizontal
        {
            get => base.flipHorizontal;
            set
            {
                base.flipHorizontal = value;
                if (flipHorizontal)
                    offDir = -1;
                else
                    offDir = 1;
                if (!_initialized)
                    return;
                CreateShutter();
            }
        }

        public override void EditorPropertyChanged(object property)
        {
            if (!_initialized)
                return;
            _open = open.value;
            UpdateShutter();
            newTrapdoorVersion = true;
        }

        private void UpdateShutter()
        {
            if (_lastFallthrough != fallthrough.value)
            {
                Level.Remove(_shutter);
                _shutter = null;
            }
            bool flag = false;
            if (_shutter == null)
            {
                flag = true;
                CreateShutter();
            }
            _lastFallthrough = fallthrough.value;
            if (flag || Level.current is Editor)
            {
                if (_open)
                    _shutter.angleDegrees = 90f * offDir;
                else
                    _shutter.angleDegrees = 0f;
            }
            else if (_open)
                _shutter.angleDegrees = Lerp.Float(_shutter.angleDegrees, 90f * offDir, 10f);
            else
                _shutter.angleDegrees = Lerp.Float(_shutter.angleDegrees, 0f, 10f);
            (_shutter as IShutter).UpdateSprite();
        }

        public WireTrapDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            fallthrough = new EditorProperty<bool>(true, this);
            length = new EditorProperty<int>(2, this, 1f, 4f, 1f);
            color = new EditorProperty<int>(2, this, max: 3f, increment: 1f);
            open = new EditorProperty<bool>(false, this);
            _sprite = new SpriteMap("wireBlock", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Wire Trapdoor";
            editorTooltip = "Opens and closes when a connected Button is pressed.";
            thickness = 4f;
            physicsMaterial = PhysicsMaterial.Metal;
            layer = Layer.Foreground;
            _canFlip = true;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("newTrapdoorVersion", newTrapdoorVersion);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            newTrapdoorVersion = node.GetProperty<bool>("newTrapdoorVersion");
            if (!newTrapdoorVersion)
            {
                length.value = 2;
                color.value = !(bool)fallthrough ? 3 : 2;
                newTrapdoorVersion = true;
            }
            return true;
        }

        public void CreateShutter()
        {
            if (_shutter != null)
                Level.Remove(_shutter);
            _shutter = !fallthrough.value ? new WireTrapDoorShutterSolid(x + 4 * offDir, y - 5f, this) : new WireTrapDoorShutter(x + 4 * offDir, y - 5f, this);
            _shutter.depth = depth + 5;
            _shutter.offDir = offDir;
            Level.Add(_shutter);
        }

        public override void Initialize()
        {
            _open = open.value;
            CreateShutter();
            (_shutter as IShutter).UpdateSprite();
            base.Initialize();
        }

        public override void Update()
        {
            UpdateShutter();
            base.Update();
        }

        public override void Terminate()
        {
            Level.Remove(_shutter);
            base.Terminate();
        }

        public void Pulse(int type, WireTileset wire)
        {
            Fondle(this, DuckNetwork.localConnection);
            _open = !_open;
            SFX.Play("click");
        }
    }
}
