namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    public class SnubbyPistol : Gun
    {
        public new StateBinding _loadedBinding = new StateBinding(nameof(_loaded));
        public StateBinding _loadBurstBinding = new StateBinding(nameof(_loadBurst));
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        private SpriteMap _sprite;
        public bool _loaded;
        public float _loadBurst;
        public float _angleOffset;

        public override float angle
        {
            get => base.angle + _angleOffset * offDir;
            set => _angle = value;
        }

        public SnubbyPistol(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new AT9mm
            {
                range = 130f,
                rangeVariation = 10f,
                accuracy = 0.95f,
                penetration = 0.4f
            };
            _type = "gun";
            _sprite = new SpriteMap("snubby", 14, 10);
            graphic = _sprite;
            center = new Vec2(7f, 4f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 9f);
            _barrelOffsetTL = new Vec2(13f, 3f);
            _fireSound = "snubbyFire";
            _kickForce = 0f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, -1f);
            _loaded = true;
            editorTooltip = "The world's most adorable gun.";
        }

        public override void OnPressAction()
        {
            if (_loaded)
            {
                base.OnPressAction();
                _loaded = false;
                _sprite.frame = 0;
            }
            else
            {
                _loaded = true;
                _sprite.frame = 1;
                _loadBurst = 1f;
                SFX.Play("snubbyLoad", pitch: Rando.Float(-0.1f, 0.1f));
            }
        }

        public override void Update()
        {
            _angleOffset = -_loadBurst * 0.3f;
            _loadBurst = Lerp.FloatSmooth(_loadBurst, 0f, 0.18f);
            if (_loadBurst < 0.1f)
                _loadBurst = 0f;
            base.Update();
        }
    }
}
