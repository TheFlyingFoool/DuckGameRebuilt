// Decompiled with JetBrains decompiler
// Type: DuckGame.WireTrapDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private bool _lastFlip;
        public EditorProperty<bool> fallthrough;
        private bool _lastFallthrough = true;
        public bool newTrapdoorVersion = true;

        public override bool flipHorizontal
        {
            get => base.flipHorizontal;
            set
            {
                base.flipHorizontal = value;
                if (this.flipHorizontal)
                    this.offDir = (sbyte)-1;
                else
                    this.offDir = (sbyte)1;
                if (!this._initialized)
                    return;
                this.CreateShutter();
            }
        }

        public override void EditorPropertyChanged(object property)
        {
            if (!this._initialized)
                return;
            this._open = this.open.value;
            this.UpdateShutter();
            this.newTrapdoorVersion = true;
        }

        private void UpdateShutter()
        {
            if (this._lastFallthrough != this.fallthrough.value)
            {
                Level.Remove(this._shutter);
                this._shutter = (Thing)null;
            }
            bool flag = false;
            if (this._shutter == null)
            {
                flag = true;
                this.CreateShutter();
            }
            this._lastFallthrough = this.fallthrough.value;
            if (flag || Level.current is Editor)
            {
                if (this._open)
                    this._shutter.angleDegrees = 90f * (float)this.offDir;
                else
                    this._shutter.angleDegrees = 0.0f;
            }
            else if (this._open)
                this._shutter.angleDegrees = Lerp.Float(this._shutter.angleDegrees, 90f * (float)this.offDir, 10f);
            else
                this._shutter.angleDegrees = Lerp.Float(this._shutter.angleDegrees, 0.0f, 10f);
            (this._shutter as IShutter).UpdateSprite();
        }

        public WireTrapDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.fallthrough = new EditorProperty<bool>(true, (Thing)this);
            this.length = new EditorProperty<int>(2, (Thing)this, 1f, 4f, 1f);
            this.color = new EditorProperty<int>(2, (Thing)this, max: 3f, increment: 1f);
            this.open = new EditorProperty<bool>(false, (Thing)this);
            this._sprite = new SpriteMap("wireBlock", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
            this._editorName = "Wire Trapdoor";
            this.editorTooltip = "Opens and closes when a connected Button is pressed.";
            this.thickness = 4f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.layer = Layer.Foreground;
            this._canFlip = true;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("newTrapdoorVersion", (object)this.newTrapdoorVersion);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.newTrapdoorVersion = node.GetProperty<bool>("newTrapdoorVersion");
            if (!this.newTrapdoorVersion)
            {
                this.length.value = 2;
                this.color.value = !(bool)this.fallthrough ? 3 : 2;
                this.newTrapdoorVersion = true;
            }
            return true;
        }

        public void CreateShutter()
        {
            if (this._shutter != null)
                Level.Remove(this._shutter);
            this._shutter = !this.fallthrough.value ? (Thing)new WireTrapDoorShutterSolid(this.x + (float)(4 * (int)this.offDir), this.y - 5f, this) : (Thing)new WireTrapDoorShutter(this.x + (float)(4 * (int)this.offDir), this.y - 5f, this);
            this._shutter.depth = this.depth + 5;
            this._shutter.offDir = this.offDir;
            Level.Add(this._shutter);
        }

        public override void Initialize()
        {
            this._open = this.open.value;
            this.CreateShutter();
            (this._shutter as IShutter).UpdateSprite();
            base.Initialize();
        }

        public override void Update()
        {
            this.UpdateShutter();
            base.Update();
        }

        public override void Terminate()
        {
            Level.Remove(this._shutter);
            base.Terminate();
        }

        public void Pulse(int type, WireTileset wire)
        {
            Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            this._open = !this._open;
            SFX.Play("click");
        }
    }
}
