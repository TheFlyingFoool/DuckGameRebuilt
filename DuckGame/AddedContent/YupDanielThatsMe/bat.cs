//// Decompiled with JetBrains decompiler
//// Type: DuckGame.Sword
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System;
//using System.Collections.Generic;

//namespace DuckGame
//{
//    //[EditorGroup("Guns|Melee")] //idk man ill come back here later im rusty with making items
//    [ClientOnly]
//    public class Bat : Gun
//    {
//        public StateBinding _swingBinding = new StateBinding(true, nameof(_swing));
//        public StateBinding _holdBinding = new StateBinding(true, nameof(_hold));
//        public StateBinding _stanceBinding = new SwordFlagBinding();
//        public StateBinding _pullBackBinding = new StateBinding(true, nameof(_pullBack));
//        public StateBinding _throwSpinBinding = new StateBinding(true, nameof(_throwSpin));
//        public StateBinding _addOffsetXBinding = new StateBinding(nameof(_addOffsetX));
//        public StateBinding _addOffsetYBinding = new StateBinding(nameof(_addOffsetY));
//        public float _swing;
//        public float _hold;
//        protected bool _drawing;
//        protected bool _clashWithWalls = true;
//        public bool _pullBack;
//        public bool _jabStance;
//        public bool _crouchStance;
//        public bool _slamStance;
//        public bool _swinging;
//        public float _addOffsetX;
//        public float _addOffsetY;
//        public bool _swingPress;
//        public bool _shing;
//        public static bool _playedShing;
//        public bool _atRest = true;
//        public bool _swung;
//        public bool _wasLifted;
//        public float _throwSpin;
//        public int _framesExisting;
//        public int _hitWait;
//        protected bool _enforceJabSwing = true;
//        protected bool _allowJabMotion = true;
//        private SpriteMap _swordSwing;
//        protected float _stickWait;
//        protected Vec2 additionalHoldOffset = Vec2.Zero;
//        private int _unslam;
//        protected float _afterSwingWait;
//        private float _afterSwingCounter;
//        private Vec2 _tapeOffset = Vec2.Zero;
//        private byte blocked;
//        public bool _volatile;
//        protected Vec2 centerHeld = new Vec2(4f, 21f);
//        protected Vec2 centerUnheld = new Vec2(4f, 11f);
//        protected bool _stayVolatile;
//        private bool bayonetLethal;
//        private float _prevAngle;
//        private Vec2 _prevPos;
//        private int _prevOffdir = -1;
//        protected float[] _lastAngles = new float[8];
//        protected Vec2[] _lastPositions = new Vec2[8];
//        protected int _lastIndex;
//        protected int _lastSize;
//        private Thing _prevHistoryOwner;
//        private Vec2 _lastHistoryPos = Vec2.Zero;
//        private float _lastHistoryAngle;
//        protected string _swingSound = "swipe";
//        protected float _timeSinceSwing;

//        public override float angle
//        {
//            get => _drawing ? _angle : base.angle + (_swing + _hold) * offDir;
//            set => _angle = value;
//        }

//        public bool jabStance => _jabStance;

//        public bool crouchStance => _crouchStance;

//        public virtual Vec2 barrelStartPos
//        {
//            get
//            {
//                if (owner == null)
//                    return position - (Offset(barrelOffset) - position).normalized * 6f;
//                return _slamStance ? position + (Offset(barrelOffset) - position).normalized * 12f : position + (Offset(barrelOffset) - position).normalized * 2f;
//            }
//        }

//        public Bat(float xval, float yval)
//          : base(xval, yval)
//        {
//            ammo = 4;
//            _ammoType = new ATLaser();
//            _ammoType.range = 170f;
//            _ammoType.accuracy = 0.8f;
//            _type = "gun";
//            graphic = new Sprite("test/bat");
//            center = new Vec2(4f, 21f);
//            collisionOffset = new Vec2(-4f, -21f);
//            collisionSize = new Vec2(6f, 25f);
//            _barrelOffsetTL = new Vec2(4f, 1f);
//            _fireSound = "smg";
//            _fullAuto = true;
//            _fireWait = 1f;
//            _kickForce = 3f;
//            _holdOffset = new Vec2(-4f, 4f);
//            weight = 0.9f;
//            physicsMaterial = PhysicsMaterial.Metal;
//            _swordSwing = new SpriteMap("swordSwipe", 32, 32);
//            _swordSwing.AddAnimation("swing", 1f, false, 0, 1, 1, 2);
//            _swordSwing.currentAnimation = "swing";
//            _swordSwing.speed = 0f;
//            _swordSwing.center = new Vec2(9f, 25f);
//            holsterAngle = 180f;
//            tapedIndexPreference = 0;
//            _bouncy = 0.5f;
//            _impactThreshold = 0.3f;
//            editorTooltip = "Basically a giant letter opener.";
//        }

//        public override void Initialize() => base.Initialize();

//        public override Vec2 tapedOffset => tapedCompatriot is Gun ? (tapedCompatriot as Gun).barrelOffset + new Vec2(-14f, 2f) : new Vec2(-6f, -3f);

//        public override void UpdateTapedPositioning(TapedGun pTaped)
//        {
//            if (pTaped.gun1 != null && pTaped.gun2 != null)
//                angleDegrees = pTaped.angleDegrees - 90 * offDir;
//            if (tapedCompatriot is Gun)
//            {
//                (tapedCompatriot as Gun).addVerticalTapeOffset = false;
//                tape._holdOffset = (tapedCompatriot as Gun)._holdOffset;
//                tape.handOffset = (tapedCompatriot as Gun).handOffset;
//            }
//            collisionOffset = new Vec2(-4f, 0f);
//            collisionSize = new Vec2(4f, 4f);
//            center = centerHeld;
//            thickness = 0f;
//        }

//        public override bool CanTapeTo(Thing pThing)
//        {
//            switch (pThing)
//            {
//                case TapedSword _:
//                case EnergyScimitar _:
//                    return false;
//                default:
//                    return true;
//            }
//        }

//        public override Holdable BecomeTapedMonster(TapedGun pTaped) => pTaped.gun1 is Sword && pTaped.gun2 is Sword ? new TapedSword(x, y) : (Holdable)null;

//        public override void CheckIfHoldObstructed()
//        {
//            if (!(this.owner is Duck owner))
//                return;
//            owner.holdObstructed = false;
//        }

//        public override bool HolsterActivate(Holster pHolster)
//        {
//            pHolster.EjectItem();
//            return true;
//        }

//        public override void Thrown()
//        {
//        }

//        public override bool Hit(Bullet bullet, Vec2 hitPos)
//        {
//            if (duck == null)
//                return false;
//            if (blocked == 0)
//            {
//                duck.AddCoolness(1);
//            }
//            else
//            {
//                ++blocked;
//                if (blocked > 4)
//                {
//                    blocked = 1;
//                    duck.AddCoolness(1);
//                }
//            }
//            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
//            SFX.Play("ting");
//            return base.Hit(bullet, hitPos);
//        }

//        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
//        {
//            if (tape != null && tape.owner != null || !_wasLifted || owner != null)
//                return;
//            switch (with)
//            {
//                case Block _:
//                label_3:
//                    _framesSinceThrown = 25;
//                    break;
//                case IPlatform _:
//                    if (from != ImpactedFrom.Bottom || vSpeed <= 0.0)
//                        break;
//                    goto label_3;
//            }
//        }

//        public override void ReturnToWorld()
//        {
//            _throwSpin = 90f;
//        }


//        protected virtual void PerformAirSpin()
//        {
//            if (this.hSpeed > 0f)
//            {
//                this._throwSpin += (Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed)) * 2f + 4f;
//                return;
//            }
//            this._throwSpin -= (Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed)) * 2f + 4f;
//        }


//        public virtual DestroyType destroyType => new DTImpale(this);

//        private bool _held;
//        private bool _pressed;
//        public float restangle = Maths.DegToRad(315);
//        public override void Update()
//        {
//            if (base.duck != null)
//            {
//                if ((base.duck.Held(this, true) ? base.duck.action : this.triggerAction) && !this._held && this._swing == 0.0f)
//                {
//                    this._pressed = true;
//                    this._held = true;
//                }
//                if (!base.duck.action)
//                {
//                    this._pressed = false;
//                    this._held = false;
//                }
//                if (this.handAngle != restangle && !this._pressed)
//                {
//                    this.handAngle = Lerp.FloatSmooth(this.handAngle, restangle, 0.3f);
//                }
//                else
//                {
//                    this.handAngle = Lerp.FloatSmooth(this.handAngle, Maths.DegToRad(20), 0.3f);
//                }
//            }
//            if (owner != null && owner.offDir < 0)
//            {
//                //this._sprite.angle = -this._sprite.angle;
//                this.handAngle = -this.handAngle;
//            }
//            //this.handAngle = Maths.DegToRad(315);
//            base.Update();
//        }

        
//        public override void Draw()
//        {
//            if (DevConsole.showCollision)
//                Graphics.DrawLine(barrelStartPos, barrelPosition, Color.Red, depth: ((Depth)1f));
//            //if (_swordSwing.speed > 0.0)
//            //{
//            //    if (duck != null)
//            //        _swordSwing.flipH = duck.offDir <= 0;
//            //    _swordSwing.alpha = 0.4f;
//            //    _swordSwing.position = this.position;
//            //    _swordSwing.depth = this.depth + 1;
//            //    _swordSwing.Draw();
//            //}
//            Vec2 position = this.position;
//            Depth depth = this.depth;
//           // graphic.color = Color.White;
//            base.Draw();
//        }

//        protected virtual void OnSwing()
//        {
//        }

//        public override void OnPressAction()
//        {
//            if (_crouchStance && _jabStance && !_swinging || !_crouchStance && !_swinging && _swing < 0.1f)
//            {
//                if (_jabStance && !_allowJabMotion)
//                    return;
//                _afterSwingCounter = 0f;
//                _pullBack = true;
//                _swung = true;
//                _shing = false;
//                _timeSinceSwing = 0f;
//                OnSwing();
//                if (_swingSound != null)
//                    SFX.Play(_swingSound, Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f));
//                if (_jabStance)
//                    return;
//                _swordSwing.speed = 1f;
//                _swordSwing.frame = 0;
//            }
//            else
//            {
//                if (!_crouchStance || _jabStance || duck == null || duck.grounded)
//                    return;
//                _slamStance = true;
//            }
//        }

//        public override void Fire()
//        {
//        }
//    }
//}
