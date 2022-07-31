// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private static Map<byte, System.Type> _netParticleTypes = new Map<byte, System.Type>();
        private static byte _netParticleTypeIndex = 0;
        protected Vec2 netLerpPosition = Vec2.Zero;
        public bool customGravity;
        public bool onlyDieWhenGrounded;

        public float spinAngle
        {
            get => this._spinAngle;
            set => this._spinAngle = value;
        }

        public float life
        {
            get => this._life;
            set => this._life = value;
        }

        public PhysicsParticle(float xpos, float ypos)
          : base(xpos, ypos)
        {
        }

        public void LerpPosition(Vec2 pos) => this.lerpPos = pos;

        public void LerpSpeed(Vec2 speed) => this.lerpSpeed = speed;

        public static void RegisterNetParticleType(System.Type pType)
        {
            if (PhysicsParticle._netParticleTypes.ContainsValue(pType))
                return;
            PhysicsParticle._netParticleTypes[PhysicsParticle._netParticleTypeIndex] = pType;
            ++PhysicsParticle._netParticleTypeIndex;
        }

        public static byte TypeToNetTypeIndex(System.Type pType) => PhysicsParticle._netParticleTypes.ContainsValue(pType) ? PhysicsParticle._netParticleTypes.Get(pType) : byte.MaxValue;

        public static System.Type NetTypeToTypeIndex(byte pNetType)
        {
            if (pNetType >= 254)
                return null;
            return PhysicsParticle._netParticleTypes.ContainsKey(pNetType) ? PhysicsParticle._netParticleTypes.Get(pNetType) : null;
        }

        public virtual void NetSerialize(BitBuffer b)
        {
            b.Write((short)this.x);
            b.Write((short)this.y);
        }

        public virtual void NetDeserialize(BitBuffer d) => this.netLerpPosition = new Vec2(d.ReadShort(), d.ReadShort());

        public override void ResetProperties()
        {
            this._life = 1f;
            this._grounded = false;
            this._spinAngle = 0f;
            this._foreverGrounded = false;
            this.alpha = 1f;
            this._airFriction = 0.03f;
            this.vSpeed = 0f;
            this.hSpeed = 0f;
            this._framesAlive = 0f;
            this._waitForNoCollide = false;
            this.globalIndex = Thing.GetGlobalIndex();
            this.gotMessage = false;
            this.isLocal = true;
            this.netIndex = 0;
            this.updateOrder = byte.MaxValue;
            this.netRemove = false;
            base.ResetProperties();
        }

        public override void Update()
        {
            if (!this.isLocal)
            {
                Vec2 position = this.position;
                Vec2 netLerpPosition = this.netLerpPosition;
                if ((position - netLerpPosition).lengthSq > 2048.0 || (position - netLerpPosition).lengthSq < 1.0)
                    this.position = netLerpPosition;
                else
                    this.position = Lerp.Vec2Smooth(position, netLerpPosition, 0.5f);
            }
            else if (Network.isActive && (this.y < Level.current.highestPoint - 200.0 || this.y > Level.current.lowestPoint + 200.0))
            {
                Level.Remove(this);
            }
            else
            {
                this._hit = false;
                this._touchedFloor = false;
                ++this._framesAlive;
                if (!this.onlyDieWhenGrounded || this._grounded || _framesAlive > 400.0)
                {
                    this._life -= 0.005f;
                    if (_life < 0.0)
                    {
                        this.alpha -= 0.1f;
                        if (this.alpha < 0.0)
                            Level.Remove(this);
                    }
                }
                if (this._foreverGrounded)
                {
                    this._grounded = true;
                    if (Rando.Float(250f) < 1.0 - _sticky)
                    {
                        this._foreverGrounded = false;
                        this._grounded = false;
                        this.hSpeed = -this._stickDir * Rando.Float(0.8f);
                    }
                }
                if (!this._grounded)
                {
                    if (this.hSpeed > 0.0)
                        this.hSpeed -= this._airFriction;
                    if (this.hSpeed < 0.0)
                        this.hSpeed += this._airFriction;
                    if (this.hSpeed < _airFriction && this.hSpeed > -this._airFriction)
                        this.hSpeed = 0f;
                    if (this.vSpeed < 4.0)
                        this.vSpeed += 0.1f * this._gravMult;
                    if (float.IsNaN(this.hSpeed))
                        this.hSpeed = 0f;
                    this._spinAngle -= 10 * Math.Sign(this.hSpeed);
                    Thing thing = Level.CheckPoint<Block>(this.x + this.hSpeed, this.y + this.vSpeed);
                    if (thing != null && _framesAlive < 2.0)
                        this._waitForNoCollide = true;
                    if (thing != null && this._waitForNoCollide)
                        thing = null;
                    else if (thing == null && this._waitForNoCollide)
                        this._waitForNoCollide = false;
                    if (thing != null)
                    {
                        this._touchedFloor = true;
                        if (this._bounceSound != "" && (Math.Abs(this.vSpeed) > 1.0 || Math.Abs(this.hSpeed) > 1.0))
                            SFX.Play(this._bounceSound, 0.5f, Rando.Float(0.2f) - 0.1f);
                        if (this.vSpeed > 0.0 && thing.top > this.y)
                        {
                            this.vSpeed = (float)-(this.vSpeed * _bounceEfficiency);
                            this._hit = true;
                            if (Math.Abs(this.vSpeed) < 0.5)
                            {
                                this.vSpeed = 0f;
                                this._grounded = true;
                            }
                        }
                        else if (this.vSpeed < 0.0 && thing.bottom < this.y)
                        {
                            this.vSpeed = (float)-(this.vSpeed * _bounceEfficiency);
                            this._hit = true;
                        }
                        if (this.hSpeed > 0.0 && thing.left > this.x)
                        {
                            this.hSpeed = (float)-(this.hSpeed * _bounceEfficiency);
                            this._hit = true;
                            if (_sticky > 0.0 && Rando.Float(1f) < _sticky)
                            {
                                this.hSpeed = 0f;
                                this.vSpeed = 0f;
                                this._foreverGrounded = true;
                                this._stickDir = 1f;
                            }
                        }
                        else if (this.hSpeed < 0.0 && thing.right < this.x)
                        {
                            this.hSpeed = (float)-(this.hSpeed * _bounceEfficiency);
                            this._hit = true;
                            if (_sticky > 0.0 && Rando.Float(1f) < _sticky)
                            {
                                this.hSpeed = 0f;
                                this.vSpeed = 0f;
                                this._foreverGrounded = true;
                                this._stickDir = -1f;
                            }
                        }
                        if (!this._hit)
                            this._grounded = true;
                    }
                    else
                    {
                        this.x += this.hSpeed;
                        this.y += this.vSpeed;
                    }
                }
                if (_spinAngle > 360.0)
                    this._spinAngle -= 360f;
                if (_spinAngle >= 0.0)
                    return;
                this._spinAngle += 360f;
            }
        }
    }
}
