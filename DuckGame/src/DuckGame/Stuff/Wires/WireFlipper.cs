namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class WireFlipper : Block, IWirePeripheral
    {
        private SpriteMap _sprite;

        public WireFlipper(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wireFlipper", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Wire Flipper";
            editorTooltip = "Alternates the direction a current will pass through when a connected Button is pressed.";
            thickness = 4f;
            physicsMaterial = PhysicsMaterial.Metal;
            layer = Layer.Foreground;
            _canFlip = true;
        }

        public override void Initialize() => base.Initialize();

        public override void Draw()
        {
            bool flipHorizontal = this.flipHorizontal;
            _sprite.frame = offDir >= 0 ? 0 : 1;
            this.flipHorizontal = false;
            base.Draw();
            this.flipHorizontal = flipHorizontal;
        }

        public override void Update() => base.Update();

        public override void Terminate() => base.Terminate();

        public void Pulse(int type, WireTileset wire)
        {
            SFX.Play("click");
            if (flipHorizontal)
                wire.dullSignalLeft = true;
            else
                wire.dullSignalRight = true;
            flipHorizontal = !flipHorizontal;
        }
    }
}
