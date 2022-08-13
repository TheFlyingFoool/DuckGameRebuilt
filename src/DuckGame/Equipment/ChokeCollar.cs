// Decompiled with JetBrains decompiler
// Type: DuckGame.ChokeCollar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ChokeCollar : Equipment
    {
        private SpriteMap _sprite;
        private Sprite _pickupSprite;
        protected WeightBall _ball;
        public EditorProperty<bool> mace = new EditorProperty<bool>(false);

        public WeightBall ball => _ball;

        public ChokeCollar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _pickupSprite = new Sprite("chokeCollar");
            _sprite = new SpriteMap("chokeCollar", 8, 8);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -2f);
            collisionSize = new Vec2(8f, 4f);
            _sprite.CenterOrigin();
            thickness = 0.1f;
            impactThreshold = 0.01f;
            _equippedDepth = 5;
            _wearOffset = new Vec2(6f, 15f);
            editorTooltip = "A heavy ball & chain that can be swung with great force. For fun!";
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Level.current is Editor)
                return;
            _ball = new WeightBall(x, y, this, this, mace.value);
            ReturnItemToWorld(_ball);
            Level.Add(_ball);
        }

        public override void Update()
        {
            _ball.clip = clip;
            if (_equippedDuck != null && !destroyed)
            {
                collisionOffset = new Vec2(-6f, -6f);
                collisionSize = new Vec2(12f, 12f);
                center = new Vec2(8f, 8f);
                solid = false;
                _sprite.flipH = _equippedDuck._sprite.flipH;
                graphic = _sprite;
                _ball.SetAttach(owner as PhysicsObject);
            }
            else
            {
                collisionOffset = new Vec2(-4f, -2f);
                collisionSize = new Vec2(8f, 4f);
                center = new Vec2(_pickupSprite.w / 2, _pickupSprite.h / 2);
                solid = true;
                _sprite.frame = 0;
                _sprite.flipH = false;
                graphic = _pickupSprite;
                _ball.SetAttach(this);
            }
            if (destroyed)
                alpha -= 0.05f;
            if (alpha < 0f)
                Level.Remove(this);
            base.Update();
        }
    }
}
