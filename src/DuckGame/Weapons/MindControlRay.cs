// Decompiled with JetBrains decompiler
// Type: DuckGame.MindControlRay
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public Duck controlledDuck => this._controlledDuck;

        public MindControlRay(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap("mindControlGun", 16, 16)
            {
                frame = 2
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(14f, 10f);
            this._hat = new SpriteMap("mindControlHelmet", 32, 32)
            {
                center = new Vec2(16f, 16f)
            };
            this._barrelOffsetTL = new Vec2(18f, 8f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 0.8f;
            this.editorTooltip = "Best friend of Mind Control Steve. Takes control of enemy Ducks.";
        }
        public override void Initialize()
        {
            _beamSound = new LoopingSound("mindBeam");
            base.Initialize();
        }
        public override void Terminate()
        {
            this._beamSound.Kill();
            base.Terminate();
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (this.connection == DuckNetwork.localConnection && value != this.connection)
                    this.LoseControl();
                base.connection = value;
            }
        }

        public override void Update()
        {
            this._sprite.frame = this.owner == null ? 1 : 0;
            this.immobilizeOwner = this._controlledDuck != null;
            if (this.isServerForObject)
            {
                if (_beamTime > 1.0 || this.owner == null)
                {
                    this._beamTime = 0.0f;
                    this._triggerHeld = false;
                    this.LoseControl();
                }
                if (this._controlledDuck != null && this.owner is Duck owner)
                {
                    if (Network.isActive)
                    {
                        this._controlledDuck.mindControl = owner.inputProfile;
                        owner.Fondle(_controlledDuck);
                        owner.Fondle(_controlledDuck.holdObject);
                        foreach (Equipment t in this._controlledDuck._equipment)
                            owner.Fondle(t);
                        owner.Fondle(_controlledDuck._ragdollInstance);
                        owner.Fondle(_controlledDuck._trappedInstance);
                        owner.Fondle(_controlledDuck._cookedInstance);
                    }
                    if (owner.inputProfile.Pressed("QUACK") || this._controlledDuck.dead || this._controlledDuck.HasEquipment(typeof(TinfoilHat)))
                    {
                        this._beamTime = 0.0f;
                        this._triggerHeld = false;
                        this.LoseControl();
                        return;
                    }
                    this._triggerHeld = true;
                    if ((double)this._controlledDuck.x < (double)owner.x)
                        owner.offDir = -1;
                    else
                        owner.offDir = 1;
                }
            }
            else
            {
                Duck owner = this.owner as Duck;
                if (this._controlledDuck != null && owner != null)
                {
                    this._controlledDuck.mindControl = owner.inputProfile;
                    owner.Fondle(_controlledDuck.holdObject);
                    foreach (Equipment t in this._controlledDuck._equipment)
                        owner.Fondle(t);
                    owner.Fondle(_controlledDuck._ragdollInstance);
                    owner.Fondle(_controlledDuck._trappedInstance);
                    owner.Fondle(_controlledDuck._cookedInstance);
                }
                if (this._controlledDuck == null && this._prevControlDuck != null)
                    this._prevControlDuck.mindControl = null;
                this._prevControlDuck = this._controlledDuck;
            }
            if (this._triggerHeld && this._controlledDuck != null)
            {
                this._beamTime += 0.005f;
                this._beamSound.pitch = Maths.NormalizeSection(this._beamTime, 0.5f, 1f) * 0.6f;
            }
            else
                this._beamSound.pitch = 0.0f;
            base.Update();
            if (this._triggerHeld && this._beamTimer.hit)
            {
                Vec2 vec2 = this.Offset(this.barrelOffset);
                Level.Add(new ControlWave(vec2.x, vec2.y, this.barrelAngle, this, this.isServerForObject));
                if (this._controlledDuck != null)
                {
                    ++this._boltWait;
                    if (this._boltWait > 2)
                    {
                        Level.Add(new MindControlBolt(vec2.x, vec2.y, this._controlledDuck));
                        this._boltWait = 0;
                    }
                }
                else
                    this._boltWait = 0;
            }
            this._beamSound.lerpVolume = this._triggerHeld ? 0.55f : 0.0f;
            this._beamSound.Update();
            if (_canConvert > 0.0)
                this._canConvert -= 0.02f;
            else
                this._canConvert = 0.0f;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            this.onFire = true;
            return true;
        }

        public override void Draw()
        {
            base.Draw();
            if (this.owner == null || !(this.owner is Duck owner) || owner.HasEquipment(typeof(Hat)))
                return;
            this._hat.alpha = owner._sprite.alpha;
            this._hat.flipH = owner._sprite.flipH;
            this._hat.depth = owner.depth + 1;
            if (owner._sprite.imageIndex > 11 && owner._sprite.imageIndex < 14)
                this._hat.angleDegrees = owner._sprite.flipH ? 90f : -90f;
            else
                this._hat.angleDegrees = 0.0f;
            Vec2 hatPoint = DuckRig.GetHatPoint(owner._sprite.imageIndex);
            Graphics.Draw(_hat, owner.x + hatPoint.x * owner._sprite.flipMultH, owner.y + hatPoint.y * owner._sprite.flipMultV);
        }

        public void ControlDuck(Duck d)
        {
            if (d == null || _canConvert > 0.01f || d.dead)
                return;
            this.LoseControl();
            if (!(this.owner is Duck owner) || owner == d)
                return;
            owner.resetAction = true;
            ++d.profile.stats.timesMindControlled;
            this._controlledDuck = d;
            if (Network.isActive)
            {
                owner.Fondle(d);
                owner.Fondle(_controlledDuck.holdObject);
                foreach (Equipment t in this._controlledDuck._equipment)
                    owner.Fondle(t);
                owner.Fondle(_controlledDuck._ragdollInstance);
                owner.Fondle(_controlledDuck._trappedInstance);
                owner.Fondle(_controlledDuck._cookedInstance);
            }
            this._controlledDuck.resetAction = true;
            this._controlledDuck.mindControl = owner.inputProfile;
            this._controlledDuck.controlledBy = owner;
            this.immobilizeOwner = true;
            SFX.Play("radioNoise", 0.8f);
            Event.Log(new MindControlEvent(this.responsibleProfile, d.profile));
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogBonus();
        }

        public void LoseControl()
        {
            if (this._controlledDuck == null)
                return;
            if (!(this.owner is Duck duck))
                duck = this.prevOwner as Duck;
            if (duck != null)
                duck.immobilized = false;
            if (this._controlledDuck != null)
            {
                this._controlledDuck.mindControl = null;
                this._controlledDuck.controlledBy = null;
            }
            this._controlledDuck = null;
            this._canConvert = 1f;
        }

        public override void OnPressAction()
        {
            this._beamTime = 0f;
            this._beamTimer.SetToEnd();
        }

        public override void OnHoldAction()
        {
        }

        public override void Fire()
        {
        }
    }
}
