// Decompiled with JetBrains decompiler
// Type: DuckGame.Ragdoll
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class Ragdoll : Thing
    {
        public bool inSleepingBag;
        public StateBinding _positionBinding = new InterpolatedVec2Binding("position");
        public StateBinding _part1Binding = new StateBinding(nameof(part1));
        public StateBinding _part2Binding = new StateBinding(nameof(part2));
        public StateBinding _part3Binding = new StateBinding(nameof(part3));
        public StateBinding _tongueStuckBinding = new StateBinding(nameof(tongueStuck));
        public StateBinding _sleepingBagHealthBinding = new StateBinding(nameof(sleepingBagHealth));
        public byte sleepingBagHealth;
        public StateBinding _physicsStateBinding = new RagdollFlagBinding();
        public Vec2 tongueStuck = Vec2.Zero;
        public Thing tongueStuckThing;
        private bool _zekeBear;
        public RagdollPart _part1;
        public RagdollPart _part2;
        public RagdollPart _part3;
        private Duck _theDuck;
        protected Thing _zapper;
        private float _zap;
        public DuckPersona persona;
        public bool _slide;
        public float _timeSinceNudge;
        public float partSep = 6f;
        public int npi;
        public int tongueShakes;
        //private bool _didSmoke;
        public bool jetting;
        private bool _wasZapping;
        public bool _makeActive;
        private float sleepingBagTimer;

        public Thing holdingOwner
        {
            get
            {
                if (_part1 != null && _part1.owner != null)
                    return _part1.owner;
                if (_part2 != null && _part2.owner != null)
                    return _part2.owner;
                return _part3 != null && _part3.owner != null ? _part3.owner : null;
            }
        }

        public void MakeZekeBear()
        {
            if (_part1 != null)
                _part1.MakeZekeBear();
            if (_part2 != null)
                _part2.MakeZekeBear();
            if (_part3 != null)
                _part3.MakeZekeBear();
            _zekeBear = true;
        }

        public RagdollPart part1
        {
            get => _part1;
            set
            {
                _part1 = value;
                if (_part1 == null)
                    return;
                _part1.doll = this;
                _part1.part = 0;
            }
        }

        public RagdollPart part2
        {
            get => _part2;
            set
            {
                _part2 = value;
                if (_part2 == null)
                    return;
                _part2.doll = this;
                _part2.part = 2;
            }
        }

        public RagdollPart part3
        {
            get => _part3;
            set
            {
                _part3 = value;
                if (_part3 == null)
                    return;
                _part3.doll = this;
                _part3.part = 1;
            }
        }

        public Duck _duck
        {
            get => _theDuck;
            set => _theDuck = value;
        }

        public void Zap(Thing zapper)
        {
            _zapper = zapper;
            _zap = 1f;
        }

        public override bool visible
        {
            get => base.visible;
            set
            {
                if (!visible && value)
                {
                    _makeActive = false;
                    if (_part1 != null)
                    {
                        _part1.owner = null;
                        _part1.framesSinceGrounded = 99;
                    }
                    if (_part2 != null)
                    {
                        _part2.owner = null;
                        _part2.framesSinceGrounded = 99;
                    }
                    if (_part3 != null)
                    {
                        _part3.owner = null;
                        _part3.framesSinceGrounded = 99;
                    }
                }
                base.visible = value;
                if (_part1 != null)
                    _part1.visible = value;
                if (_part2 != null)
                    _part2.visible = value;
                if (_part3 == null)
                    return;
                _part3.visible = value;
            }
        }

        public override bool active
        {
            get => base.active;
            set
            {
                base.active = value;
                if (_part1 != null)
                    _part1.active = value;
                if (_part2 != null)
                    _part2.active = value;
                if (_part3 == null)
                    return;
                _part3.active = value;
            }
        }

        public override bool enablePhysics
        {
            get => base.enablePhysics;
            set
            {
                base.enablePhysics = value;
                if (_part1 != null)
                    _part1.enablePhysics = value;
                if (_part2 != null)
                    _part2.enablePhysics = value;
                if (_part3 == null)
                    return;
                _part3.enablePhysics = value;
            }
        }

        public override bool solid
        {
            get => base.solid;
            set
            {
                base.solid = value;
                if (_part1 != null)
                    _part1.solid = value;
                if (_part2 != null)
                    _part2.solid = value;
                if (_part3 == null)
                    return;
                _part3.solid = value;
            }
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                base.connection = value;
                if (_part1 != null)
                    _part1.connection = value;
                if (_part2 != null)
                    _part2.connection = value;
                if (_part3 == null)
                    return;
                _part3.connection = value;
            }
        }

        public override NetIndex8 authority
        {
            get => base.authority;
            set
            {
                base.authority = value;
                if (_part1 != null)
                    _part1.authority = value;
                if (_part2 != null)
                    _part2.authority = value;
                if (_part3 == null)
                    return;
                _part3.authority = value;
            }
        }

        public Duck captureDuck
        {
            get => _duck;
            set
            {
                _duck = value;
                if (_duck == null)
                    return;
                if (_part1 != null)
                    _part1.part = 0;
                if (_part2 != null)
                    _part2.part = 2;
                if (_part3 == null)
                    return;
                _part3.part = 1;
            }
        }

        public void Extinguish()
        {
            if (part1 != null)
                part1.Extinquish();
            if (part2 != null)
                part2.Extinquish();
            if (part3 == null)
                return;
            part3.Extinquish();
        }

        public Ragdoll(
          float xpos,
          float ypos,
          Duck who,
          bool slide,
          float degrees,
          int off,
          Vec2 v,
          DuckPersona p = null)
          : base(xpos, ypos)
        {
            _duck = who;
            _slide = slide;
            offDir = (sbyte)off;
            angleDegrees = degrees;
            velocity = v;
            persona = p;
        }

        public bool PartHeld() => _part1 != null && _part2 != null && _part3 != null && (_part1.owner != null || _part2.owner != null || _part3.owner != null);

        public Profile PartHeldProfile()
        {
            if (_part1 == null || _part2 == null || _part3 == null)
                return null;
            if (_part1.duck != null)
                return _part1.duck.profile;
            if (_part2.duck != null)
                return _part2.duck.profile;
            return _part3.duck != null ? _part3.duck.profile : null;
        }

        public void SortOutParts(
          float xpos,
          float ypos,
          Duck who,
          bool slide,
          float degrees,
          int off,
          Vec2 v)
        {
            _duck = who;
            _slide = slide;
            offDir = (sbyte)off;
            angleDegrees = degrees;
            velocity = v;
            _makeActive = false;
            RunInit();
        }

        public virtual void Organize()
        {
            Vec2 vec = Maths.AngleToVec(angle);
            if (_part1 == null)
            {
                _part1 = new RagdollPart(x - vec.x * partSep, y - vec.y * partSep, 0, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part1);
                _part2 = new RagdollPart(x, y, 2, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part2);
                _part3 = new RagdollPart(x + vec.x * partSep, y + vec.y * partSep, 1, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part3);
                Level.Add(_part1);
                Level.Add(_part2);
                Level.Add(_part3);
            }
            else
            {
                _part1.SortOutDetails(x - vec.x * partSep, y - vec.y * partSep, 0, _duck != null ? _duck.persona : persona, offDir, this);
                _part2.SortOutDetails(x, y, 2, _duck != null ? _duck.persona : persona, offDir, this);
                _part3.SortOutDetails(x + vec.x * partSep, y + vec.y * partSep, 1, _duck != null ? _duck.persona : persona, offDir, this);
            }
            _part1.joint = _part2;
            _part3.joint = _part2;
            _part1.connect = _part3;
            _part3.connect = _part1;
            _part1.framesSinceGrounded = 99;
            _part2.framesSinceGrounded = 99;
            _part3.framesSinceGrounded = 99;
            if (_duck == null)
                return;
            if (!(Level.current is GameLevel) || !(Level.current as GameLevel).isRandom)
            {
                _duck.ReturnItemToWorld(_part1);
                _duck.ReturnItemToWorld(_part2);
                _duck.ReturnItemToWorld(_part3);
            }
            _part3.depth = new Depth(_duck.depth.value);
            _part1.depth = _part3.depth - 1;
        }

        public override void Initialize() => base.Initialize();

        public void RunInit()
        {
            Organize();
            if (Network.isActive && !GhostManager.inGhostLoop)
                GhostManager.context.MakeGhost(this);
            if (Math.Abs(hSpeed) < 0.2f)
                hSpeed = NetRand.Float(0.3f, 1f) * (NetRand.Float(1f) >= 0.5 ? 1f : -1f);
            float num1 = _slide ? 1f : 1.05f;
            float num2 = _slide ? 1f : 0.95f;
            _part1.hSpeed = hSpeed * num1;
            _part1.vSpeed = vSpeed;
            _part2.hSpeed = hSpeed;
            _part2.vSpeed = vSpeed;
            _part3.hSpeed = hSpeed * num2;
            _part3.vSpeed = vSpeed;
            _part1.enablePhysics = false;
            _part2.enablePhysics = false;
            _part3.enablePhysics = false;
            _part1.Update();
            _part2.Update();
            _part3.Update();
            _part1.enablePhysics = true;
            _part2.enablePhysics = true;
            _part3.enablePhysics = true;
            if (Network.isActive)
            {
                Fondle(this, DuckNetwork.localConnection);
                Fondle(_part1, DuckNetwork.localConnection);
                Fondle(_part2, DuckNetwork.localConnection);
                Fondle(_part3, DuckNetwork.localConnection);
            }
            if (_duck == null || !_duck.onFire)
                return;
            _part1.Burn(_part1.position, _duck.lastBurnedBy);
            _part2.Burn(_part2.position, _duck.lastBurnedBy);
        }

        public void LightOnFire()
        {
            if (_duck == null)
                return;
            _part1.Burn(_part1.position, _duck.lastBurnedBy);
            _part2.Burn(_part2.position, _duck.lastBurnedBy);
        }

        public void Unragdoll()
        {
            if (isInPipe)
                return;
            int num = _duck.HasEquipment(typeof(FancyShoes)) ? 1 : 0;
            _duck.visible = true;
            if (Network.isActive)
            {
                _part2.UpdateLastReasonablePosition(_part2.position);
                _duck.position = _part2._lastReasonablePosition;
            }
            else
                _duck.position = _part2.position;
            if (num == 0)
                _duck.position.y -= 20f;
            _duck.hSpeed = _part2.hSpeed;
            _duck.immobilized = false;
            _duck.enablePhysics = true;
            _duck._jumpValid = 0;
            _duck._lastHoldItem = null;
            _makeActive = false;
            _part2.ReturnItemToWorld(_duck);
            if (Network.isActive)
            {
                active = false;
                visible = false;
                owner = null;
                if (y > -1000.0)
                {
                    y = -9999f;
                    _part1.y = -9999f;
                    _part2.y = -9999f;
                    _part3.y = -9999f;
                }
                _part1.owner = null;
                _part2.owner = null;
                _part3.owner = null;
                if (_duck.isServerForObject)
                {
                    Fondle(this, _duck.connection);
                    Fondle(_part1, _duck.connection);
                    Fondle(_part2, _duck.connection);
                    Fondle(_part3, _duck.connection);
                }
            }
            else
                Level.Remove(this);
            _duck.ragdoll = null;
            if (num == 0)
                _duck.vSpeed = -2f;
            else
                _duck.vSpeed = _part2.vSpeed;
        }

        public void Shot(Bullet bullet)
        {
            if (_duck == null || _duck.dead)
                return;
            _duck.position = _part2.position;
            _duck.Kill(new DTShot(bullet));
            _duck.y -= 5000f;
        }

        public void Killed(DestroyType t)
        {
            if (_duck == null || _duck.dead || t == null)
                return;
            _duck.position = _part2.position;
            _duck.Destroy(t);
            _duck.y -= 5000f;
        }

        public void LitOnFire(Thing litBy)
        {
            if (_duck == null || _duck.onFire)
                return;
            _duck.Burn(position, litBy);
        }

        public override void Terminate()
        {
            if (_part1 == null || _part2 == null || _part3 == null)
                return;
            Level.Remove(_part1);
            Level.Remove(_part2);
            Level.Remove(_part3);
        }

        public virtual void Solve(PhysicsObject body1, PhysicsObject body2, float dist)
        {
            float num1 = dist;
            Vec2 vec2_1 = body2.position - body1.position;
            float num2 = vec2_1.length;
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(body1.hSpeed, body1.vSpeed);
            Vec2 vec2_4 = new Vec2(body2.hSpeed, body2.vSpeed);
            double num3 = Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.1f;
            float num6 = 2.1f;
            if (body1 == part1 && jetting)
                num5 = 6f;
            else if (body2 == part1 && jetting)
                num6 = 6f;
            double num7 = num4;
            float num8 = (float)((num3 + num7) / (num5 + num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            Vec2 vec2_7 = vec2_4 - vec2_5 * num6;
            if (body1.owner == null)
            {
                body1.hSpeed = vec2_6.x;
                body1.vSpeed = vec2_6.y;
            }
            if (body2.owner != null)
                return;
            body2.hSpeed = vec2_7.x;
            body2.vSpeed = vec2_7.y;
        }

        public override bool ShouldUpdate() => false;

        //public void ProcessInput(InputProfile input)
        //{
        //}

        public virtual float SpecialSolve(PhysicsObject b1, PhysicsObject b2, float dist)
        {
            Thing thing1 = b1.owner != null ? b1.owner : b1;
            Thing thing2 = b2.owner != null ? b2.owner : b2;
            float num1 = dist;
            Vec2 vec2_1 = b2.position - b1.position;
            float num2 = vec2_1.length;
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            if (num2 < num1)
                return 0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing1.hSpeed, thing1.vSpeed);
            Vec2 vec2_4 = new Vec2(thing2.hSpeed, thing2.vSpeed);
            double num3 = Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing1 is ChainLink && !(thing2 is ChainLink))
            {
                num5 = 10f;
                num6 = 0f;
            }
            else if (thing2 is ChainLink && !(thing1 is ChainLink))
            {
                num5 = 0f;
                num6 = 10f;
            }
            else if (thing1 is ChainLink && thing2 is ChainLink)
            {
                num5 = 10f;
                num6 = 10f;
            }
            if (thing1 is RagdollPart)
                num5 = !_zekeBear ? 10f : 4f;
            else if (thing2 is RagdollPart)
                num6 = !_zekeBear ? 10f : 4f;
            double num7 = num4;
            float num8 = (float)((num3 + num7) / (num5 + num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            vec2_4 -= vec2_5 * num6;
            thing1.hSpeed = vec2_6.x;
            thing1.vSpeed = vec2_6.y;
            thing2.hSpeed = vec2_4.x;
            thing2.vSpeed = vec2_4.y;
            if (thing1 is ChainLink && (thing2.position - thing1.position).length > num1 * 12.0)
                thing1.position = position;
            if (thing2 is ChainLink && (thing2.position - thing1.position).length > num1 * 12.0)
                thing2.position = position;
            return num8;
        }

        public virtual float SpecialSolve(PhysicsObject b1, Vec2 stuck, float dist)
        {
            Thing thing = b1.owner != null ? b1.owner : b1;
            float num1 = dist;
            Vec2 vec2_1 = stuck - b1.position;
            float num2 = vec2_1.length;
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            if (num2 < num1)
                return 0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing.hSpeed, thing.vSpeed);
            Vec2 vec2_4 = new Vec2(0f, 0f);
            double num3 = Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing is RagdollPart)
                num5 = !_zekeBear ? 10f : 4f;
            double num7 = num4;
            float num8 = (float)((num3 + num7) / (num5 + num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            Vec2 vec2_7 = vec2_4 - vec2_5 * num6;
            thing.hSpeed = vec2_6.x;
            thing.vSpeed = vec2_6.y;
            return num8;
        }

        public bool makeActive
        {
            get => _makeActive;
            set => _makeActive = value;
        }

        public override void Update()
        {
            if (removeFromLevel || isOffBottomOfLevel && captureDuck != null && captureDuck.dead)
                return;
            _timeSinceNudge += 0.07f;
            if (_part1 == null || _part2 == null || _part3 == null)
                return;
            if (_zap > 0.0)
            {
                _part1.vSpeed += Rando.Float(-1f, 0.5f);
                _part1.hSpeed += Rando.Float(-0.5f, 0.5f);
                _part2.vSpeed += Rando.Float(-1f, 0.5f);
                _part2.hSpeed += Rando.Float(-0.5f, 0.5f);
                _part3.vSpeed += Rando.Float(-1f, 0.5f);
                _part3.hSpeed += Rando.Float(-0.5f, 0.5f);
                _part1.x += Rando.Int(-2, 2);
                _part1.y += Rando.Int(-2, 2);
                _part2.x += Rando.Int(-2, 2);
                _part2.y += Rando.Int(-2, 2);
                _part3.x += Rando.Int(-2, 2);
                _part3.y += Rando.Int(-2, 2);
                _zap -= 0.05f;
                _wasZapping = true;
            }
            else if (_wasZapping)
            {
                _wasZapping = false;
                if (captureDuck != null)
                {
                    if (captureDuck.dead)
                    {
                        captureDuck.Ressurect();
                        return;
                    }
                    captureDuck.Kill(new DTElectrocute(_zapper));
                    return;
                }
            }
            if (captureDuck != null && captureDuck.inputProfile != null && captureDuck.isServerForObject)
            {
                if (captureDuck.inputProfile.Pressed(Triggers.Jump) && captureDuck.HasEquipment(typeof(Jetpack)))
                    captureDuck.GetEquipment(typeof(Jetpack)).PressAction();
                if (captureDuck.inputProfile.Released(Triggers.Jump) && captureDuck.HasEquipment(typeof(Jetpack)))
                    captureDuck.GetEquipment(typeof(Jetpack)).ReleaseAction();
            }
            partSep = 6f;
            if (_zekeBear)
                partSep = 4f;
            Vec2 vec2_1 = _part1.position - _part3.position;
            if (vec2_1.length > partSep * 5.0)
            {
                if (_part1.owner != null)
                {
                    RagdollPart part2 = _part2;
                    _part3.position = vec2_1 = _part1.position;
                    Vec2 vec2_2 = vec2_1;
                    part2.position = vec2_2;
                }
                else if (_part3.owner != null)
                {
                    RagdollPart part1 = _part1;
                    _part2.position = vec2_1 = _part3.position;
                    Vec2 vec2_3 = vec2_1;
                    part1.position = vec2_3;
                }
                else
                {
                    RagdollPart part1 = _part1;
                    _part3.position = vec2_1 = _part2.position;
                    Vec2 vec2_4 = vec2_1;
                    part1.position = vec2_4;
                }
                _part1.vSpeed = _part2.vSpeed = _part3.vSpeed = 0f;
                _part1.hSpeed = _part2.hSpeed = _part3.hSpeed = 0f;
                Solve(_part1, _part2, partSep);
                Solve(_part2, _part3, partSep);
                Solve(_part1, _part3, partSep * 2f);
            }
            Solve(_part1, _part2, partSep);
            Solve(_part2, _part3, partSep);
            Solve(_part1, _part3, partSep * 2f);
            if (_part1.owner is Duck && _part3.owner is Duck)
            {
                double num1 = SpecialSolve(_part3, _part1.owner as Duck, 16f);
                double num2 = SpecialSolve(_part1, _part3.owner as Duck, 16f);
            }
            if (tongueStuck != Vec2.Zero && captureDuck != null)
            {
                Vec2 vec2_5 = tongueStuck + new Vec2(captureDuck.offDir * -4, -6f);
                if (_part1.owner is Duck)
                {
                    double num3 = SpecialSolve(_part3, _part1.owner as Duck, 16f);
                    double num4 = SpecialSolve(_part1, vec2_5, 16f);
                }
                if (_part3.owner is Duck)
                {
                    double num5 = SpecialSolve(_part1, _part3.owner as Duck, 16f);
                    double num6 = SpecialSolve(_part3, vec2_5, 16f);
                }
                vec2_1 = part1.position - vec2_5;
                if (vec2_1.length > 4.0)
                {
                    double num = SpecialSolve(_part1, vec2_5, 4f);
                    vec2_1 = vec2_5 - part1.position;
                    Vec2 normalized = vec2_1.normalized;
                    vec2_1 = part1.position - vec2_5;
                    if (vec2_1.length > 12.0)
                        part1.position = Lerp.Vec2Smooth(part1.position, vec2_5, 0.2f);
                }
            }
            position = (_part1.position + _part2.position + _part3.position) / 3f;
            if (_duck == null || _zap > 0.0)
                return;
            if (_duck.eyesClosed)
                _part1.frame = 20;
            if (!_duck.fancyShoes || _duck.framesSinceRagdoll >= 1)
                UpdateInput();
            bool flag = false;
            if (isServerForObject)
            {
                if (isOffBottomOfLevel && !_duck.dead)
                {
                    flag = _duck.Kill(new DTFall());
                    ++_duck.profile.stats.fallDeaths;
                }
                jetting = false;
            }
            if (!flag)
                return;
            _duck.y = _part2.y - 9999f;
            _duck.x = _part2.x;
        }

        public override void Draw()
        {
        }

        public bool TryingToControl()
        {
            if (captureDuck == null)
                return false;
            return captureDuck.inputProfile.Pressed(Triggers.Left) || captureDuck.inputProfile.Pressed(Triggers.Right) || captureDuck.inputProfile.Pressed(Triggers.Up) || captureDuck.inputProfile.Pressed(Triggers.Ragdoll) || captureDuck.inputProfile.Pressed(Triggers.Jump);
        }

        public void UpdateInput()
        {
            sleepingBagTimer -= Maths.IncFrameTimer();
            if (sleepingBagTimer < 0.0 && sleepingBagHealth > 20)
            {
                sleepingBagHealth -= 4;
                sleepingBagTimer = 1f;
            }
            if (!_duck.dead)
            {
                if (_duck.HasEquipment(typeof(FancyShoes)) && !jetting)
                {
                    if (captureDuck.inputProfile.Pressed(Triggers.Right))
                    {
                        Vec2 velVec = _part1.position - _part2.position;
                        velVec = velVec.Rotate((float)Math.PI / 2, Vec2.Zero);
                        part1.velocity += velVec * 0.2f;

                        velVec = _part3.position - _part2.position;
                        velVec = velVec.Rotate((float)Math.PI / 2, Vec2.Zero);
                        part3.velocity += velVec * 0.2f;
                    }
                    else if (captureDuck.inputProfile.Pressed(Triggers.Left))
                    {
                        Vec2 velVec = _part1.position - _part2.position;
                        velVec = velVec.Rotate((float)Math.PI / 2, Vec2.Zero);
                        part1.velocity += velVec * -0.2f;

                        velVec = _part3.position - _part2.position;
                        velVec = velVec.Rotate((float)Math.PI / 2, Vec2.Zero);
                        part3.velocity += velVec * -0.2f;
                    }
                }
                else if (_timeSinceNudge > 1.0 && !jetting)
                {
                    if (captureDuck.inputProfile.Pressed(Triggers.Left))
                    {
                        _part1.vSpeed += NetRand.Float(-2f, 2f);
                        _part3.vSpeed += NetRand.Float(-2f, 2f);
                        _part2.hSpeed += NetRand.Float(-2f, -1.2f);
                        _part2.vSpeed -= NetRand.Float(1f, 1.5f);
                        _timeSinceNudge = 0f;
                        ShakeOutOfSleepingBag();
                    }
                    else if (captureDuck.inputProfile.Pressed(Triggers.Right))
                    {
                        _part1.vSpeed += NetRand.Float(-2f, 2f);
                        _part3.vSpeed += NetRand.Float(-2f, 2f);
                        _part2.hSpeed += NetRand.Float(1.2f, 2f);
                        _part2.vSpeed -= NetRand.Float(1f, 1.5f);
                        _timeSinceNudge = 0f;
                        ShakeOutOfSleepingBag();
                    }
                    else if (captureDuck.inputProfile.Pressed(Triggers.Up))
                    {
                        _part1.vSpeed += NetRand.Float(-2f, 1f);
                        _part3.vSpeed += NetRand.Float(-2f, 1f);
                        _part2.vSpeed -= NetRand.Float(1.5f, 2f);
                        _timeSinceNudge = 0f;
                        ShakeOutOfSleepingBag();
                    }
                }
            }
            bool flag = false;
            if (captureDuck.HasEquipment(typeof(FancyShoes)) && Math.Abs(_part1.x - _part3.x) < 9.0 && _part1.y < _part3.y)
                flag = true;
            if (tongueStuckThing != null && tongueStuckThing.removeFromLevel)
            {
                tongueStuck = Vec2.Zero;
                if (Network.isActive)
                    Fondle(this, DuckNetwork.localConnection);
                _makeActive = true;
            }
            if (_duck.dead || !captureDuck.inputProfile.Pressed(Triggers.Ragdoll) && !captureDuck.inputProfile.Pressed(Triggers.Jump) || ((_part1.framesSinceGrounded < 5 || _part2.framesSinceGrounded < 5 || _part3.framesSinceGrounded < 5 ? 1 : (_part1.doFloat || part2.doFloat ? 1 : (_part3.doFloat ? 1 : 0))) | (flag ? 1 : 0)) == 0 && _part1.owner == null && _part2.owner == null && _part3.owner == null)
                return;
            if (inSleepingBag)
            {
                if (_timeSinceNudge <= 1.0)
                    return;
                _part1.vSpeed += NetRand.Float(-2f, 1f);
                _part3.vSpeed += NetRand.Float(-2f, 1f);
                _part2.vSpeed -= NetRand.Float(1.5f, 2f);
                _timeSinceNudge = 0f;
                ShakeOutOfSleepingBag();
            }
            else
            {
                if (_part1.held || _part2.held || _part3.held || !(tongueStuck == Vec2.Zero) && tongueShakes <= 5 || !isServerForObject)
                    return;
                tongueStuck = Vec2.Zero;
                if (Network.isActive)
                    Fondle(this, DuckNetwork.localConnection);
                _makeActive = true;
            }
        }

        public void ShakeOutOfSleepingBag()
        {
            ++tongueShakes;
            if (_part1 != null && _part1.owner == null && _part2 != null && _part2.owner == null && _part3 != null && _part3.owner == null && tongueStuck != Vec2.Zero && tongueShakes > 5)
                tongueStuck = Vec2.Zero;
            if (sleepingBagHealth < 0 || captureDuck == null)
                return;
            sleepingBagHealth = (byte)Math.Max(sleepingBagHealth - 5, 0);
            if (sleepingBagHealth != 0)
                return;
            if (inSleepingBag && captureDuck.isServerForObject)
                _makeActive = true;
            inSleepingBag = false;
        }

        private bool isInPipe
        {
            get
            {
                if (part1 != null && part1.inPipe || part2 != null && part2.inPipe || part3 != null && part3.inPipe || inPipe)
                    return true;
                return captureDuck != null && captureDuck.inPipe;
            }
        }

        public void UpdateUnragdolling()
        {
            if (isInPipe || captureDuck == null || !captureDuck.isServerForObject || !_makeActive)
                return;
            Unragdoll();
        }
    }
}
