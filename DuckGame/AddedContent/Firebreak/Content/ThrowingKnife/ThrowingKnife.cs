using System;
using System.Drawing.Drawing2D;

namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Rebuilt|Guns")]
    public sealed class ThrowingKnife : Gun, IPlatform
    {
        private SpriteMap _sprite;
        private Duck? _tempSpareDuck = null;
        public bool Stuck;
        private bool _wasStuck;
        private float _stuckAngle;
        private ThrowingRopeHandle _ropeHandle;
        private bool _hadHandle;
        private byte _incapacitated; // todo: change to datetime
        public MaterialThing? StuckThing = null;
        private Vec2 _stuckThingEtchOffset = default;

        public ThrowingKnife(float xval, float yval) : base(xval, yval)
        {
            _sprite = new SpriteMap("quasik", 17, 7);
            graphic = _sprite;
            editorTooltip = "Turn your enemies into unwilling partners for a quick game of interdimensional tug-of-war!";
            _editorName = "SwingCut";
            
            center = new Vec2(8f, 3.5f);
            center = new Vec2(6f, 3.5f);
            collisionSize = new Vec2(8, 6);
            collisionOffset = new Vec2(-4, -3);
            
            depth = new Depth(0.9f);
            // _canFlip = false;
            
            thickness = 4f;
            weight = 0.9f;
            friction = 0.1f;
            physicsMaterial = PhysicsMaterial.Metal;
            
            ammo = 1;
            _type = "gun";
            _fireRumble = RumbleIntensity.Light;
        }

        public override bool CanTapeTo(Thing pThing)
        {
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_ropeHandle is not null && !_ropeHandle.Retracting)
            {
                _ropeHandle.Retracting = true;
                Stuck = false;
                _incapacitated = 120;
                _sprite.frame = 1;
                SFX.Play("breakTV");
            }
            
            return base.Hit(bullet, hitPos);
        }

        public override void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            if (with is IPlatform && _ropeHandle is not null && _ropeHandle.Retracting)
                return;
            
            if (_ropeHandle is not null && _ropeHandle.Retracting)
                goto SkipStuck;
            
            if (held || grounded || !(with is Block || (with is IPlatform && ((with is Holdable && from != ImpactedFrom.Top) || from == ImpactedFrom.Bottom))))
                goto SkipStuck;
            
            if (with is ThrowingKnife)
                goto SkipStuck; // :)
            
            if (with is ItemBox && from == ImpactedFrom.Top)
                goto SkipStuck;
            
            if (with is Window && velocity.lengthSq > 16)
            {
                with.Destroy(new DTImpact(this));
                goto SkipStuck;
            }

            _stuckAngle = angle;
            Stuck = true;
            _wasStuck = false;
            vSpeed = 0f;
            gravMultiplier = 0f;
            grounded = true;
            _wasStuck = true;
            StuckThing = with;
            _stuckThingEtchOffset = position - StuckThing.position;

            SFX.Play("dartStick", 0.8f, -0.1f + Rando.Float(0.2f), 0f, false);

            SkipStuck:
            base.Impact(with, from, solidImpact);
        }

        public override void Thrown()
        {
            if (owner is Duck d)
                _tempSpareDuck = d;
            
            base.Thrown();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if ((with == _tempSpareDuck || (with is RagdollPart && ((RagdollPart)with).doll._duck == _tempSpareDuck)) && (/*_wasStuck ||*/ (_ropeHandle is not null && _ropeHandle.Retracting)))
                ;
            else if ((_incapacitated == 0 || _ropeHandle is null) && !held && !grounded && velocity.length > 2f && with is IAmADuck && (with != _tempSpareDuck || _framesSinceThrown > 2))
            {
                with.Destroy(new DTImpale(this));
                velocity *= 0.75f;
            }

            base.OnSoftImpact(with, from);
        }

        private void DestroyHandle()
        {
            if (_tempSpareDuck.holdObject == _ropeHandle)
            {
                _tempSpareDuck.ObjectThrown(_ropeHandle);
                _tempSpareDuck.GiveHoldable(this);
            }
            else if (_tempSpareDuck.HasEquipment(typeof(PowerHolster)))
            {
                PowerHolster currentPowerHolster = (PowerHolster)_tempSpareDuck.GetEquipment(typeof(PowerHolster));
                while (currentPowerHolster.containedObject is PowerHolster nextPowerHolster)
                {
                    currentPowerHolster = nextPowerHolster;
                }
                
                currentPowerHolster.EjectItem();
                currentPowerHolster.SetContainedObject(this);
            }

            canPickUp = true;
            _ropeHandle.y = -999999;
            velocity = Vec2.Zero;
            vSpeed = 0;
            hSpeed = 0;
            Level.Remove(_ropeHandle);
            _ropeHandle = null;

            if (_tempSpareDuck.inputProfile.Down(_tempSpareDuck.HasEquipment(typeof(PowerHolster)) && ((PowerHolster)_tempSpareDuck.GetEquipment(typeof(PowerHolster))).trigger ? Triggers.Quack : Triggers.Shoot))
                _hadHandle = true;
            
            _tempSpareDuck = null;
        }

        public override void Update()
        {
            if (_ropeHandle is not null && _ropeHandle.Retracting && Distance(_ropeHandle) < 8f)
                DestroyHandle();
            
            if (held)
            {
                Stuck = false;
                if (_sprite.frame == 2)
                    _sprite.frame = 0;
                if (_incapacitated > 0)
                {
                    _incapacitated--;
                    if (_incapacitated % 2 == 0 && _incapacitated > 30)
                    {
                        Rectangle airManaBox = new(duck.topLeft - new Vec2(16f), duck.bottomRight + new Vec2(16f));
                        Vec2 randomManaPos = new(Rando.Float(airManaBox.Left, airManaBox.Right), Rando.Float(airManaBox.Top, airManaBox.Bottom));
                        Level.Add(ThrowingKnifeConnectionParticle.New(randomManaPos.x, randomManaPos.y, duck));
                    }
                }
                else
                {
                    if (_sprite.frame == 1)
                        _sprite.frame = 0;
                }
            }
            
            if (Stuck)
            {
                vSpeed = 0f;
                hSpeed = 0f;
                grounded = true;
                gravMultiplier = 0f;
                enablePhysics = false;
                
                if (StuckThing is not null && !StuckThing.removeFromLevel && StuckThing.collisionSize != Vec2.Zero)
                {
                    position = StuckThing.position + _stuckThingEtchOffset;
                }
                else
                {
                    Stuck = false;
                }

                _wasStuck = true;
            }
            else if (_wasStuck)
            {
                enablePhysics = true;
                gravMultiplier = 1f;
                StuckThing = null;
            }

            base.Update();
        }

        public override void OnPressAction()
        {
            if (_hadHandle)
            {
                _hadHandle = false;
                return;
            }
            
            if (_incapacitated > 0)
                return;

            Vec2 directionVector = Maths.AngleToVec(graphic.angle);
            if (graphic.flipH)
                directionVector *= -1;

            directionVector.y *= -1;
            
            if (owner is not Duck)
                goto Launch;

            _tempSpareDuck = (Duck)owner;

            _ropeHandle = new ThrowingRopeHandle(this);
            Level.Add(_ropeHandle);
            
            if (!(_tempSpareDuck.HasEquipment(typeof(PowerHolster)) && ((PowerHolster)_tempSpareDuck.GetEquipment(typeof(PowerHolster))).trigger))
            {
                _tempSpareDuck.ObjectThrown(this);
                _tempSpareDuck.GiveHoldable(_ropeHandle);
            }
            else
            {
                PowerHolster currentPowerHolster = (PowerHolster)_tempSpareDuck.GetEquipment(typeof(PowerHolster));
                while (currentPowerHolster.containedObject is PowerHolster nextPowerHolster)
                {
                    currentPowerHolster = nextPowerHolster;
                }
                
                currentPowerHolster.EjectItem();
                currentPowerHolster.SetContainedObject(_ropeHandle);
            }

            Launch:
            canPickUp = false;

            hSpeed = 0;
            vSpeed = 0;
            velocity = new Vec2(directionVector.x * 8, directionVector.y * 8 - 2);
            _sprite.frame = 2;
        }

        // public override void Draw()
        // {
        //     Graphics.DrawRect(new Rectangle(position - (Vec2.One / 2), position + (Vec2.One / 2)), Color.Red, 2f);
        //     
        //     // Graphics.DrawString($"v=({velocity.x:0.0}, {velocity.y:0.0})", position + new Vec2(16, -16), Color.Red, 2f);
        //     // Graphics.DrawString($"angle={Maths.RadToDeg(angle):00}", position + new Vec2(16, -32), Color.Red, 2f);
        //     // Graphics.DrawString($"flip={graphic.flipH}", position + new Vec2(16, -48), Color.Red, 2f);
        //     
        //     // Graphics.DrawLine(position, position + velocity.normalized * (velocity.length * 4f), Color.Red, 2f);
        //     
        //     base.Draw();
        // }

        public override float angle 
        { 
            get
            {
                if (Stuck)
                    return _stuckAngle;

                if (held || grounded)
                    return float.IsNaN(base.angle) ? 0 : base.angle;

                float dynAngle = (float) Math.Atan(velocity.y / (velocity.x * (graphic.flipH ? 1 : 1)));
                return float.IsNaN(dynAngle) ? 0 : dynAngle;
            }
            set => base.angle = value;
        }
    }
}