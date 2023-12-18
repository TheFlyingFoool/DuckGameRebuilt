using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class Basketball : Holdable
    {
        private SpriteMap _sprite;
        private int _framesInHand;
        private int _walkFrames;
        private Duck _bounceDuck;

        public Basketball(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("basketBall", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(15f, 15f);
            depth = -0.5f;
            thickness = 1f;
            weight = 3f;
            flammable = 0.3f;
            collideSounds.Add("basketball");
            physicsMaterial = PhysicsMaterial.Rubber;
            _bouncy = 0.8f;
            friction = 0.03f;
            _impactThreshold = 0.1f;
            _holdOffset = new Vec2(6f, 0f);
            handOffset = new Vec2(0f, -0f);
            editorTooltip = "Perfect for playing the world's greatest sport! Also basketball.";
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
            if (!owner.action)
                return;
            owner.holdObstructed = true;
        }

        public override void OnPressAction()
        {
        }

        public override void Update()
        {
            if (!isServerForObject)
                _bounceDuck = null;
            else if (owner == null)
            {
                _walkFrames = 0;
                --_framesInHand;
                if (_framesInHand < -60)
                    _bounceDuck = null;
                if (_bounceDuck != null)
                {
                    float length = (_bounceDuck.position - position).length;
                    if (length < 16f)
                        hSpeed = _bounceDuck.hSpeed;
                    if (_bounceDuck.holdObject == null && vSpeed < 1f && _bounceDuck.top + 8f > y && length < 16f)
                    {
                        _bounceDuck.GiveHoldable(this);
                        _framesInHand = 0;
                    }
                }
            }
            else if (duck != null && duck.holdObject == this)
            {
                if (_framesInHand < 0)
                    _framesInHand = 0;
                if (!owner.action && Math.Abs(owner.hSpeed) > 0.5f && _framesInHand > 6)
                {
                    _bounceDuck = duck;
                    float hSpeed = duck.hSpeed;
                    duck.ThrowItem(false);
                    vSpeed = 2f;
                    this.hSpeed = hSpeed * 1.1f;
                    _framesInHand = 0;
                }
                else
                {
                    if (Math.Abs(owner.hSpeed) > 0.5f && duck.grounded)
                        ++_walkFrames;
                    else if (duck.grounded)
                        --_walkFrames;
                    if (_walkFrames < 0)
                        _walkFrames = 0;
                    if (_walkFrames > 20)
                    {
                        SFX.PlaySynchronized("basketballWhistle");
                        duck.ThrowItem(false);
                        _walkFrames = 0;
                    }
                    _bounceDuck = null;
                    ++_framesInHand;
                }
            }
            base.Update();
        }
    }
}
