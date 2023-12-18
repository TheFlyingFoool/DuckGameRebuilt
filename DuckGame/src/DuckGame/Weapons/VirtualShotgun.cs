namespace DuckGame
{
    [EditorGroup("Guns|Shotguns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class VirtualShotgun : Shotgun
    {
        public StateBinding _roomIndexBinding = new StateBinding(nameof(roomIndex), 4);
        private byte _roomIndex;

        public byte roomIndex
        {
            get => _roomIndex;
            set
            {
                _roomIndex = value;
                if (!Network.isClient || !Network.inLobby || _roomIndex >= 4)
                    return;
                (Level.current as TeamSelect2).GetBox(_roomIndex).gun = this;
            }
        }

        public VirtualShotgun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            graphic = new Sprite("virtualShotgun");
            _loaderSprite = new SpriteMap("virtualShotgunLoader", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            editorTooltip = "The perfect shotgun for life inside a computer simulation. Virtually infinite ammo.";
        }

        public override void Update()
        {
            ammo = 99;
            base.Update();
        }
    }
}
