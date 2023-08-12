using System;

namespace DuckGame
{
    [ClientOnly]
    public class WarpSword : Gun
    {
        public WarpSword(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("wagsword");
            center = new Vec2(8.5f, 18.5f);//17 37

            ammo = 8;
            _ammoType = new ATLaser();

            physicsMaterial = PhysicsMaterial.Metal;

            _holdOffset = new Vec2(-6, 4.5f);
        }
        public bool _drawing;
        public override float angle
        {
            get => _drawing ? _angle : base.angle + (_swing + _hold) * offDir;
            set => _angle = value;
        }
        public float _swing;
        public float _hold;
        public float _throwSpin;
        public override void Update()
        {
            base.Update();
            if (duck != null)
            {
                center = new Vec2(8.5f, 37f);
                collisionSize = new Vec2(4);
                _collisionOffset = new Vec2(0, 0);
                if (duck.inputProfile.Down("DOWN"))
                {
                    _hold = Lerp.Float(_hold, 0, 0.3f);
                }
                else
                {
                    _hold = Lerp.Float(_hold, -0.4f, 0.3f);

                }
            }
            else
            {
                center = new Vec2(8.5f, 18.5f);//17 37
                collisionSize = new Vec2(10, 4);
                _collisionOffset = new Vec2(-5, -2);
                _hold = 0;

                bool flag1 = false;
                bool flag2 = false;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2.0f || !grounded)
                {
                    if (!grounded && Level.CheckRect<Block>(position + new Vec2(-6f, -6f), position + new Vec2(6f, -2f)) != null)
                    {
                        flag2 = true;
                    }
                    if (!flag2 && !_grounded && !initemspawner && (Level.CheckPoint<IPlatform>(position + new Vec2(0f, 8f)) == null || vSpeed < 0.0f))
                    {
                        PerformAirSpin();
                        flag1 = true;
                    }
                }
                if (!flag1 | flag2)
                {
                    _throwSpin %= 360f;
                    if (flag2)
                        _throwSpin = Math.Abs(_throwSpin - 90f) >= Math.Abs(_throwSpin + 90f) ? Lerp.Float(-90f, 0f, 16f) : Lerp.Float(_throwSpin, 90f, 16f);
                    else if (_throwSpin > 90.0f && _throwSpin < 270.0f)
                    {
                        _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                    }
                    else
                    {
                        if (_throwSpin > 180.0f)
                            _throwSpin -= 360f;
                        else if (_throwSpin < -180.0f)
                            _throwSpin += 360f;
                        _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                    }
                }
                angleDegrees = _throwSpin + 90;
            }
        }
        public void PerformAirSpin()
        {
            if (hSpeed > 0f)
            {
                _throwSpin += (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f;
                return;
            }
            _throwSpin -= (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f;
        }
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
        }
        public override void OnHoldAction()
        {
        }
        public override void OnReleaseAction()
        {
        }
    }
}
