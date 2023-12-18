using System;

namespace DuckGame
{
    public class TampingWeapon : Gun
    {
        public StateBinding _tampedBinding = new TampingFlagBinding();
        public StateBinding _tampIncBinding = new StateBinding(nameof(_tampInc));
        public StateBinding _tampTimeBinding = new StateBinding(nameof(_tampTime));
        public StateBinding _offsetYBinding = new StateBinding(nameof(_offsetY));
        public StateBinding _rotAngleBinding = new StateBinding(nameof(_rotAngle));
        public bool _tamped = true;
        public float _tampInc;
        public float _tampTime;
        public bool _rotating;
        public float _offsetY;
        public float _rotAngle;
        public float _timeToTamp = 1f;
        public float _tampBoost = 1f;
        private Sprite _tampingHand;
        private bool _puffed;
        private Duck _prevDuckOwner;

        public override float angle
        {
            get => base.angle + Maths.DegToRad(-_rotAngle);
            set => _angle = value;
        }

        public TampingWeapon(float xval, float yval)
          : base(xval, yval)
        {
            _tampingHand = new Sprite("tampingHand")
            {
                center = new Vec2(4f, 8f)
            };
        }

        public override void Update()
        {
            base.Update();
            _tampBoost = Lerp.Float(_tampBoost, 1f, 0.01f);
            if (this.owner is Duck owner && owner.inputProfile != null && duck != null && duck.profile != null)
            {
                _prevDuckOwner = owner;
                if (owner.inputProfile.Pressed(Triggers.Shoot))
                    _tampBoost += 0.14f;
                if (duck.immobilized)
                    duck.profile.stats.timeSpentReloadingOldTimeyWeapons += Maths.IncFrameTimer();
                if (_rotating)
                {
                    if (offDir < 0)
                    {
                        if (_rotAngle > -90)
                            _rotAngle -= 3f;
                        if (_rotAngle <= -90)
                        {
                            tamping = true;
                            _tampInc += 0.2f * _tampBoost;
                            tampPos = (float)Math.Sin(_tampInc) * 2f;
                            if (tampPos < -1 && !_puffed)
                            {
                                Vec2 vec2 = Offset(barrelOffset) - barrelVector * 8f;
                                if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                _puffed = true;
                            }
                            if (tampPos > -1)
                                _puffed = false;
                            _tampTime += 0.005f * _tampBoost;
                        }
                        if (_tampTime >= _timeToTamp)
                        {
                            _rotAngle += 8f;
                            if (_offsetY > 0)
                                _offsetY -= 2f;
                            tamping = false;
                            if (_rotAngle >= 0)
                            {
                                _rotAngle = 0f;
                                _rotating = false;
                                _tamped = true;
                                _offsetY = 0f;
                                owner.immobilized = false;
                            }
                        }
                    }
                    else
                    {
                        if (_rotAngle < 90)
                            _rotAngle += 3f;
                        if (_rotAngle >= 90)
                        {
                            tamping = true;
                            _tampInc += 0.2f * _tampBoost;
                            tampPos = (float)Math.Sin(_tampInc) * 2f;
                            if (tampPos < -1 && !_puffed)
                            {
                                Vec2 vec2 = Offset(barrelOffset) - barrelVector * 8f;
                                if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                                _puffed = true;
                            }
                            if (tampPos > -1)
                                _puffed = false;
                            _tampTime += 0.005f * _tampBoost;
                        }
                        if (_tampTime >= _timeToTamp)
                        {
                            _rotAngle -= 8f;
                            if (_offsetY > 0)
                                _offsetY -= 2f;
                            tamping = false;
                            if (_rotAngle <= 0)
                            {
                                _rotAngle = 0f;
                                _rotating = false;
                                _tamped = true;
                                _offsetY = 0f;
                                owner.immobilized = false;
                            }
                        }
                    }
                    if (_offsetY >= 10f)
                        return;
                    ++_offsetY;
                }
                else
                    _tampBoost = 1f;
            }
            else
            {
                if (_prevDuckOwner == null)
                    return;
                _prevDuckOwner.immobilized = false;
                tamping = false;
                _rotAngle = 0f;
                _rotating = false;
                _offsetY = 0f;
                _prevDuckOwner = null;
            }
        }

        public override void Draw()
        {
            y += _offsetY;
            base.Draw();
            if (duck != null && tamping)
            {
                if (offDir < 0)
                {
                    _tampingHand.x = x + 3f;
                    _tampingHand.y = y - 16f + tampPos;
                    _tampingHand.flipH = true;
                }
                else
                {
                    _tampingHand.x = x - 3f;
                    _tampingHand.y = y - 16f + tampPos;
                    _tampingHand.flipH = false;
                }
                _tampingHand.depth = depth - 1;
                float angle = duck._spriteArms.angle;
                Vec2 vec2 = Offset(barrelOffset);
                Vec2 p2 = vec2 + barrelVector * (float)(tampPos * 2 + 3);
                Graphics.DrawLine(vec2 - barrelVector * 6f, p2, Color.Gray, depth: (depth - 2));
                duck._spriteArms.depth = depth - 1;
                duck._spriteArms.LerpState.CanLerp = false;
                Graphics.Draw(duck._spriteArms, p2.x, p2.y);
                duck._spriteArms.angle = angle;
            }
            position = new Vec2(position.x, position.y - _offsetY);
        }
    }
}
