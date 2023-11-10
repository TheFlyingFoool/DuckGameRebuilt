namespace DuckGame
{
    public class CorkObject : PhysicsObject, ISwing, IPullBack
    {
        private Thing _gun;
        public Sprite _ropeSprite;
        private Rope _rope;
        private Harpoon _sticker;

        public CorkObject(float pX, float pY, Thing pOwner)
          : base(pX, pY)
        {
            graphic = new Sprite("cork");
            _collisionSize = new Vec2(4f, 4f);
            _collisionOffset = new Vec2(-2f, -3f);
            center = new Vec2(3f, 3f);
            _gun = pOwner;
            weight = 0.1f;
            bouncy = 0.5f;
            airFrictionMult = 0f;
            _ropeSprite = new Sprite("grappleWire")
            {
                center = new Vec2(8f, 0f)
            };
        }

        public Rope GetRopeParent(Thing child)
        {
            for (Rope ropeParent = _rope; ropeParent != null; ropeParent = ropeParent.attach2 as Rope)
            {
                if (ropeParent.attach2 == child)
                    return ropeParent;
            }
            return null;
        }

        public override void Initialize()
        {
            if (_gun != null)
            {
                _sticker = new Harpoon(this);
                level.AddThing(_sticker);
                _sticker.SetStuckPoint(_gun.position);
                _rope = new Rope(x, y, null, _sticker, this, tex: _ropeSprite, belongsTo: this);
                Level.Add(_rope);
            }
            base.Initialize();
        }

        public override void Terminate()
        {
            if (_sticker != null)
                Level.Remove(_sticker);
            if (_rope != null)
                _rope.RemoveRope();
            base.Terminate();
        }

        public float WindUp(float pAmount)
        {
            if (pAmount <= 0f || _rope.startLength <= 0f)
                return 100f;
            _rope.Pull(-pAmount);
            _rope.startLength -= pAmount;
            return _rope.startLength;
        }

        public override void Update()
        {
            if (_rope != null)
            {
                if (!grounded)
                    specialFrictionMod = 0f;
                else
                    specialFrictionMod = 1f;
                _rope.position = position;
                _rope.SetServer(isServerForObject);
                Vec2 vec2_1 = _rope.attach1.position - _rope.attach2.position;
                bool flag = true;
                if (_rope.properLength < 0f)
                {
                    _rope.startLength = _rope.properLength = 100f;
                    flag = false;
                }
                if (vec2_1.length > _rope.properLength)
                {
                    vec2_1 = vec2_1.normalized;
                    Vec2 position2 = position;
                    Vec2 vec2_2 = _rope.attach2.position + vec2_1 * _rope.properLength;
                    Vec2 end = vec2_2;
                    Vec2 vec2_3;
                    // ref Vec2 local = ref vec2_3;
                    Level.CheckRay<Block>(position2, end, out vec2_3);
                    if (flag)
                    {
                        hSpeed = vec2_2.x - position.x;
                        vSpeed = vec2_2.y - position.y;
                        gravMultiplier = 0f;
                        float specialFrictionMod = this.specialFrictionMod;
                        this.specialFrictionMod = 0f;
                        airFrictionMult = 0f;
                        Vec2 lastPosition = this.lastPosition;
                        UpdatePhysics();
                        gravMultiplier = 1f;
                        this.specialFrictionMod = specialFrictionMod;
                        Vec2 vec2_4 = vec2_2 - lastPosition;
                        if (vec2_4.length > 32f)
                            position = vec2_2;
                        else if (vec2_4.length > 6f)
                        {
                            hSpeed = Rando.Float(-2f, 2f);
                            vSpeed = Rando.Float(-2f, 2f);
                        }
                        else
                        {
                            hSpeed = vec2_4.x;
                            vSpeed = vec2_4.y;
                        }
                    }
                    else
                        position = vec2_2;
                }
                _sticker.SetStuckPoint((_gun as Gun).barrelPosition);
            }
            base.Update();
        }
    }
}
