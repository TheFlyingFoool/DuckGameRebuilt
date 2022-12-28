// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public abstract class PhysicsParticle : Thing
    {
        public ushort netIndex;
        public byte updateOrder = byte.MaxValue;
        public bool netRemove;
        public bool _grounded;
        protected float _spinAngle;
        protected string _bounceSound = "";
        protected float _bounceEfficiency = 0.5f;
        protected float _gravMult = 1f;
        protected float _sticky;
        protected bool _foreverGrounded;
        protected float _stickDir;
        public bool gotMessage;
        public bool needsSynchronization;
        protected bool _hit;
        protected bool _touchedFloor;
        private float _framesAlive;
        private bool _waitForNoCollide;
        protected float _airFriction = 0.03f;
        protected float _life = 1f;
        public Vec2 lerpPos = Vec2.Zero;
        public Vec2 lerpSpeed = Vec2.Zero;
        private static Map<byte, Type> _netParticleTypes = new Map<byte, Type>();
        private static byte _netParticleTypeIndex = 0;
        protected Vec2 netLerpPosition = Vec2.Zero;
        public bool customGravity;
        public bool onlyDieWhenGrounded;

        public float spinAngle
        {
            get => _spinAngle;
            set => _spinAngle = value;
        }

        public float life
        {
            get => _life;
            set => _life = value;
        }

        public PhysicsParticle(float xpos, float ypos)
          : base(xpos, ypos)
        {
        }

        public void LerpPosition(Vec2 pos) => lerpPos = pos;

        public void LerpSpeed(Vec2 speed) => lerpSpeed = speed;

        public static void RegisterNetParticleType(Type pType)
        {
            if (_netParticleTypes.ContainsValue(pType))
                return;
            _netParticleTypes[_netParticleTypeIndex] = pType;
            ++_netParticleTypeIndex;
        }

        public static byte TypeToNetTypeIndex(Type pType) => _netParticleTypes.ContainsValue(pType) ? _netParticleTypes.Get(pType) : byte.MaxValue;

        public static Type NetTypeToTypeIndex(byte pNetType)
        {
            if (pNetType >= 254)
                return null;
            return _netParticleTypes.ContainsKey(pNetType) ? _netParticleTypes.Get(pNetType) : null;
        }

        public virtual void NetSerialize(BitBuffer b)
        {
            b.Write((short)x);
            b.Write((short)y);
        }

        public virtual void NetDeserialize(BitBuffer d) => netLerpPosition = new Vec2(d.ReadShort(), d.ReadShort());

        public override void ResetProperties()
        {
            _life = 1f;
            _grounded = false;
            _spinAngle = 0f;
            _foreverGrounded = false;
            alpha = 1f;
            _airFriction = 0.03f;
            vSpeed = 0f;
            hSpeed = 0f;
            _framesAlive = 0f;
            _waitForNoCollide = false;
            globalIndex = GetGlobalIndex();
            gotMessage = false;
            isLocal = true;
            netIndex = 0;
            updateOrder = byte.MaxValue;
            netRemove = false;
            base.ResetProperties();
        }

        public override void Update()
        {
            if (!isLocal)
            {
                Vec2 position = this.position;
                Vec2 netLerpPosition = this.netLerpPosition;
                if ((position - netLerpPosition).lengthSq > 2048.0 || (position - netLerpPosition).lengthSq < 1.0)
                    this.position = netLerpPosition;
                else
                    this.position = Lerp.Vec2Smooth(position, netLerpPosition, 0.5f);
            }
            else if (Network.isActive && (y < Level.current.highestPoint - 200.0 || y > Level.current.lowestPoint + 200.0))
            {
                Level.Remove(this);
            }
            else
            {
                _hit = false;
                _touchedFloor = false;
                ++_framesAlive;
                if (!onlyDieWhenGrounded || _grounded || _framesAlive > 400.0)
                {
                    _life -= 0.005f;
                    if (_life < 0.0)
                    {
                        alpha -= 0.1f;
                        if (alpha < 0.0)
                            Level.Remove(this);
                    }
                }
                if (_foreverGrounded)
                {
                    _grounded = true;
                    if (Rando.Float(250f) < 1.0 - _sticky)
                    {
                        _foreverGrounded = false;
                        _grounded = false;
                        hSpeed = -_stickDir * Rando.Float(0.8f);
                    }
                }
                if (!_grounded)
                {
                    if (hSpeed > 0.0)
                        hSpeed -= _airFriction;
                    if (hSpeed < 0.0)
                        hSpeed += _airFriction;
                    if (hSpeed < _airFriction && hSpeed > -_airFriction)
                        hSpeed = 0f;
                    if (vSpeed < 4.0)
                        vSpeed += 0.1f * _gravMult;
                    if (float.IsNaN(hSpeed))
                        hSpeed = 0f;
                    _spinAngle -= 10 * Math.Sign(hSpeed);
                    Thing thing = Level.CheckPoint<Block>(x + hSpeed, y + vSpeed);
                    if (thing != null && _framesAlive < 2.0)
                        _waitForNoCollide = true;
                    if (thing != null && _waitForNoCollide)
                        thing = null;
                    else if (thing == null && _waitForNoCollide)
                        _waitForNoCollide = false;
                    if (thing != null)
                    {
                        _touchedFloor = true;
                        if (_bounceSound != "" && (Math.Abs(vSpeed) > 1.0 || Math.Abs(hSpeed) > 1.0))
                            SFX.Play(_bounceSound, 0.5f, Rando.Float(0.2f) - 0.1f);
                        if (vSpeed > 0.0 && thing.top > y)
                        {
                            vSpeed = (float)-(vSpeed * _bounceEfficiency);
                            _hit = true;
                            if (Math.Abs(vSpeed) < 0.5)
                            {
                                vSpeed = 0f;
                                _grounded = true;
                            }
                        }
                        else if (vSpeed < 0.0 && thing.bottom < y)
                        {
                            vSpeed = (float)-(vSpeed * _bounceEfficiency);
                            _hit = true;
                        }
                        if (hSpeed > 0.0 && thing.left > x)
                        {
                            hSpeed = (float)-(hSpeed * _bounceEfficiency);
                            _hit = true;
                            if (_sticky > 0.0 && Rando.Float(1f) < _sticky)
                            {
                                hSpeed = 0f;
                                vSpeed = 0f;
                                _foreverGrounded = true;
                                _stickDir = 1f;
                            }
                        }
                        else if (hSpeed < 0.0 && thing.right < x)
                        {
                            hSpeed = (float)-(hSpeed * _bounceEfficiency);
                            _hit = true;
                            if (_sticky > 0.0 && Rando.Float(1f) < _sticky)
                            {
                                hSpeed = 0f;
                                vSpeed = 0f;
                                _foreverGrounded = true;
                                _stickDir = -1f;
                            }
                        }
                        if (!_hit)
                            _grounded = true;
                    }
                    else
                    {
                        x += hSpeed;
                        y += vSpeed;
                    }
                }
                if (_spinAngle > 360.0)
                    _spinAngle -= 360f;
                if (_spinAngle >= 0.0)
                    return;
                _spinAngle += 360f;
            }
        }
    }
}
