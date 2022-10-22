// Decompiled with JetBrains decompiler
// Type: DuckGame.MindControlRay
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class MindControlRay : Gun
    {
        public StateBinding _controlledDuckBinding = new StateBinding(nameof(_controlledDuck));
        private Duck _prevControlDuck;
        private SpriteMap _sprite;
        private SpriteMap _hat;
        private ActionTimer _beamTimer = (ActionTimer)0.2f;
        public Duck _controlledDuck;
        private float _beamTime;
        private float _canConvert;
        private int _boltWait;
        private LoopingSound _beamSound;

        public Duck controlledDuck => _controlledDuck;

        public MindControlRay(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATLaser();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.8f;
            _type = "gun";
            _sprite = new SpriteMap("mindControlGun", 16, 16)
            {
                frame = 2
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 10f);
            _hat = new SpriteMap("mindControlHelmet", 32, 32)
            {
                center = new Vec2(16f, 16f)
            };
            _barrelOffsetTL = new Vec2(18f, 8f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            flammable = 0.8f;
            editorTooltip = "Best friend of Mind Control Steve. Takes control of enemy Ducks.";
        }
        public override void Initialize()
        {
            _beamSound = new LoopingSound("mindBeam");
            base.Initialize();
        }
        public override void Terminate()
        {
            _beamSound.Kill();
            base.Terminate();
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (connection == DuckNetwork.localConnection && value != connection)
                    LoseControl();
                base.connection = value;
            }
        }

        public override void Update()
        {
            _sprite.frame = owner == null ? 1 : 0;
            immobilizeOwner = _controlledDuck != null;
            if (isServerForObject)
            {
                if (_beamTime > 1.0 || this.owner == null)
                {
                    _beamTime = 0f;
                    _triggerHeld = false;
                    LoseControl();
                }
                if (_controlledDuck != null && this.owner is Duck owner)
                {
                    if (Network.isActive)
                    {
                        _controlledDuck.mindControl = owner.inputProfile;
                        owner.Fondle(_controlledDuck);
                        owner.Fondle(_controlledDuck.holdObject);
                        foreach (Equipment t in _controlledDuck._equipment)
                            owner.Fondle(t);
                        owner.Fondle(_controlledDuck._ragdollInstance);
                        owner.Fondle(_controlledDuck._trappedInstance);
                        owner.Fondle(_controlledDuck._cookedInstance);
                    }
                    if (owner.inputProfile.Pressed("QUACK") || _controlledDuck.dead || _controlledDuck.HasEquipment(typeof(TinfoilHat)))
                    {
                        _beamTime = 0f;
                        _triggerHeld = false;
                        LoseControl();
                        return;
                    }
                    _triggerHeld = true;
                    if (_controlledDuck.x < owner.x)
                        owner.offDir = -1;
                    else
                        owner.offDir = 1;
                }
            }
            else
            {
                Duck owner = this.owner as Duck;
                if (_controlledDuck != null && owner != null)
                {
                    _controlledDuck.mindControl = owner.inputProfile;
                    owner.Fondle(_controlledDuck.holdObject);
                    foreach (Equipment t in _controlledDuck._equipment)
                        owner.Fondle(t);
                    owner.Fondle(_controlledDuck._ragdollInstance);
                    owner.Fondle(_controlledDuck._trappedInstance);
                    owner.Fondle(_controlledDuck._cookedInstance);
                }
                if (_controlledDuck == null && _prevControlDuck != null)
                    _prevControlDuck.mindControl = null;
                _prevControlDuck = _controlledDuck;
            }
            if (_triggerHeld && _controlledDuck != null)
            {
                _beamTime += 0.005f;
                _beamSound.pitch = Maths.NormalizeSection(_beamTime, 0.5f, 1f) * 0.6f;
            }
            else
                _beamSound.pitch = 0f;
            base.Update();
            if (_triggerHeld && _beamTimer.hit)
            {
                Vec2 vec2 = Offset(barrelOffset);
                Level.Add(new ControlWave(vec2.x, vec2.y, barrelAngle, this, isServerForObject));
                if (_controlledDuck != null)
                {
                    ++_boltWait;
                    if (_boltWait > 2)
                    {
                        Level.Add(new MindControlBolt(vec2.x, vec2.y, _controlledDuck));
                        _boltWait = 0;
                    }
                }
                else
                    _boltWait = 0;
            }
            _beamSound.lerpVolume = _triggerHeld ? 0.55f : 0f;
            _beamSound.Update();
            if (_canConvert > 0.0)
                _canConvert -= 0.02f;
            else
                _canConvert = 0f;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            onFire = true;
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            if (this.owner == null || !(this.owner is Duck owner) || owner.HasEquipment(typeof(Hat)))
                return;
            _hat.alpha = owner._sprite.alpha;
            _hat.flipH = owner._sprite.flipH;
            _hat.depth = owner.depth + 1;
            if (owner._sprite.imageIndex > 11 && owner._sprite.imageIndex < 14)
                _hat.angleDegrees = owner._sprite.flipH ? 90f : -90f;
            else
                _hat.angleDegrees = 0f;
            Vec2 hatPoint = DuckRig.GetHatPoint(owner._sprite.imageIndex);
            Graphics.Draw(_hat, owner.x + hatPoint.x * owner._sprite.flipMultH, owner.y + hatPoint.y * owner._sprite.flipMultV);
        }

        public void ControlDuck(Duck d)
        {
            if (d == null || _canConvert > 0.01f || d.dead)
                return;
            LoseControl();
            if (!(this.owner is Duck owner) || owner == d)
                return;
            owner.resetAction = true;
            ++d.profile.stats.timesMindControlled;
            _controlledDuck = d;
            if (Network.isActive)
            {
                owner.Fondle(d);
                owner.Fondle(_controlledDuck.holdObject);
                foreach (Equipment t in _controlledDuck._equipment)
                    owner.Fondle(t);
                owner.Fondle(_controlledDuck._ragdollInstance);
                owner.Fondle(_controlledDuck._trappedInstance);
                owner.Fondle(_controlledDuck._cookedInstance);
            }
            _controlledDuck.resetAction = true;
            _controlledDuck.mindControl = owner.inputProfile;
            _controlledDuck.controlledBy = owner;
            immobilizeOwner = true;
            SFX.Play("radioNoise", 0.8f);
            Event.Log(new MindControlEvent(responsibleProfile, d.profile));
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogBonus();
        }

        public void LoseControl()
        {
            if (_controlledDuck == null)
                return;
            if (!(owner is Duck duck))
                duck = prevOwner as Duck;
            if (duck != null)
                duck.immobilized = false;
            if (_controlledDuck != null)
            {
                _controlledDuck.mindControl = null;
                _controlledDuck.controlledBy = null;
            }
            _controlledDuck = null;
            _canConvert = 1f;
        }

        public override void OnPressAction()
        {
            _beamTime = 0f;
            _beamTimer.SetToEnd();
        }

        public override void OnHoldAction()
        {
        }

        public override void Fire()
        {
        }
    }
}
