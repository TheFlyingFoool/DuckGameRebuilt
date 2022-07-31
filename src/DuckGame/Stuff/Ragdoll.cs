// Decompiled with JetBrains decompiler
// Type: DuckGame.Ragdoll
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                if (this._part1 != null && this._part1.owner != null)
                    return this._part1.owner;
                if (this._part2 != null && this._part2.owner != null)
                    return this._part2.owner;
                return this._part3 != null && this._part3.owner != null ? this._part3.owner : null;
            }
        }

        public void MakeZekeBear()
        {
            if (this._part1 != null)
                this._part1.MakeZekeBear();
            if (this._part2 != null)
                this._part2.MakeZekeBear();
            if (this._part3 != null)
                this._part3.MakeZekeBear();
            this._zekeBear = true;
        }

        public RagdollPart part1
        {
            get => this._part1;
            set
            {
                this._part1 = value;
                if (this._part1 == null)
                    return;
                this._part1.doll = this;
                this._part1.part = 0;
            }
        }

        public RagdollPart part2
        {
            get => this._part2;
            set
            {
                this._part2 = value;
                if (this._part2 == null)
                    return;
                this._part2.doll = this;
                this._part2.part = 2;
            }
        }

        public RagdollPart part3
        {
            get => this._part3;
            set
            {
                this._part3 = value;
                if (this._part3 == null)
                    return;
                this._part3.doll = this;
                this._part3.part = 1;
            }
        }

        public Duck _duck
        {
            get => this._theDuck;
            set => this._theDuck = value;
        }

        public void Zap(Thing zapper)
        {
            this._zapper = zapper;
            this._zap = 1f;
        }

        public override bool visible
        {
            get => base.visible;
            set
            {
                if (!this.visible && value)
                {
                    this._makeActive = false;
                    if (this._part1 != null)
                    {
                        this._part1.owner = null;
                        this._part1.framesSinceGrounded = 99;
                    }
                    if (this._part2 != null)
                    {
                        this._part2.owner = null;
                        this._part2.framesSinceGrounded = 99;
                    }
                    if (this._part3 != null)
                    {
                        this._part3.owner = null;
                        this._part3.framesSinceGrounded = 99;
                    }
                }
                base.visible = value;
                if (this._part1 != null)
                    this._part1.visible = value;
                if (this._part2 != null)
                    this._part2.visible = value;
                if (this._part3 == null)
                    return;
                this._part3.visible = value;
            }
        }

        public override bool active
        {
            get => base.active;
            set
            {
                base.active = value;
                if (this._part1 != null)
                    this._part1.active = value;
                if (this._part2 != null)
                    this._part2.active = value;
                if (this._part3 == null)
                    return;
                this._part3.active = value;
            }
        }

        public override bool enablePhysics
        {
            get => base.enablePhysics;
            set
            {
                base.enablePhysics = value;
                if (this._part1 != null)
                    this._part1.enablePhysics = value;
                if (this._part2 != null)
                    this._part2.enablePhysics = value;
                if (this._part3 == null)
                    return;
                this._part3.enablePhysics = value;
            }
        }

        public override bool solid
        {
            get => base.solid;
            set
            {
                base.solid = value;
                if (this._part1 != null)
                    this._part1.solid = value;
                if (this._part2 != null)
                    this._part2.solid = value;
                if (this._part3 == null)
                    return;
                this._part3.solid = value;
            }
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                base.connection = value;
                if (this._part1 != null)
                    this._part1.connection = value;
                if (this._part2 != null)
                    this._part2.connection = value;
                if (this._part3 == null)
                    return;
                this._part3.connection = value;
            }
        }

        public override NetIndex8 authority
        {
            get => base.authority;
            set
            {
                base.authority = value;
                if (this._part1 != null)
                    this._part1.authority = value;
                if (this._part2 != null)
                    this._part2.authority = value;
                if (this._part3 == null)
                    return;
                this._part3.authority = value;
            }
        }

        public Duck captureDuck
        {
            get => this._duck;
            set
            {
                this._duck = value;
                if (this._duck == null)
                    return;
                if (this._part1 != null)
                    this._part1.part = 0;
                if (this._part2 != null)
                    this._part2.part = 2;
                if (this._part3 == null)
                    return;
                this._part3.part = 1;
            }
        }

        public void Extinguish()
        {
            if (this.part1 != null)
                this.part1.Extinquish();
            if (this.part2 != null)
                this.part2.Extinquish();
            if (this.part3 == null)
                return;
            this.part3.Extinquish();
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
            this._duck = who;
            this._slide = slide;
            this.offDir = (sbyte)off;
            this.angleDegrees = degrees;
            this.velocity = v;
            this.persona = p;
        }

        public bool PartHeld() => this._part1 != null && this._part2 != null && this._part3 != null && (this._part1.owner != null || this._part2.owner != null || this._part3.owner != null);

        public Profile PartHeldProfile()
        {
            if (this._part1 == null || this._part2 == null || this._part3 == null)
                return null;
            if (this._part1.duck != null)
                return this._part1.duck.profile;
            if (this._part2.duck != null)
                return this._part2.duck.profile;
            return this._part3.duck != null ? this._part3.duck.profile : null;
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
            this._duck = who;
            this._slide = slide;
            this.offDir = (sbyte)off;
            this.angleDegrees = degrees;
            this.velocity = v;
            this._makeActive = false;
            this.RunInit();
        }

        public void Organize()
        {
            Vec2 vec = Maths.AngleToVec(this.angle);
            if (this._part1 == null)
            {
                this._part1 = new RagdollPart(this.x - vec.x * this.partSep, this.y - vec.y * this.partSep, 0, this._duck != null ? this._duck.persona : this.persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part1);
                this._part2 = new RagdollPart(this.x, this.y, 2, this._duck != null ? this._duck.persona : this.persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part2);
                this._part3 = new RagdollPart(this.x + vec.x * this.partSep, this.y + vec.y * this.partSep, 1, this._duck != null ? this._duck.persona : this.persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part3);
                Level.Add(_part1);
                Level.Add(_part2);
                Level.Add(_part3);
            }
            else
            {
                this._part1.SortOutDetails(this.x - vec.x * this.partSep, this.y - vec.y * this.partSep, 0, this._duck != null ? this._duck.persona : this.persona, offDir, this);
                this._part2.SortOutDetails(this.x, this.y, 2, this._duck != null ? this._duck.persona : this.persona, offDir, this);
                this._part3.SortOutDetails(this.x + vec.x * this.partSep, this.y + vec.y * this.partSep, 1, this._duck != null ? this._duck.persona : this.persona, offDir, this);
            }
            this._part1.joint = this._part2;
            this._part3.joint = this._part2;
            this._part1.connect = this._part3;
            this._part3.connect = this._part1;
            this._part1.framesSinceGrounded = 99;
            this._part2.framesSinceGrounded = 99;
            this._part3.framesSinceGrounded = 99;
            if (this._duck == null)
                return;
            if (!(Level.current is GameLevel) || !(Level.current as GameLevel).isRandom)
            {
                this._duck.ReturnItemToWorld(_part1);
                this._duck.ReturnItemToWorld(_part2);
                this._duck.ReturnItemToWorld(_part3);
            }
            this._part3.depth = new Depth(this._duck.depth.value);
            this._part1.depth = this._part3.depth - 1;
        }

        public override void Initialize() => base.Initialize();

        public void RunInit()
        {
            this.Organize();
            if (Network.isActive && !GhostManager.inGhostLoop)
                GhostManager.context.MakeGhost(this);
            if ((double)Math.Abs(this.hSpeed) < 0.200000002980232)
                this.hSpeed = NetRand.Float(0.3f, 1f) * ((double)NetRand.Float(1f) >= 0.5 ? 1f : -1f);
            float num1 = this._slide ? 1f : 1.05f;
            float num2 = this._slide ? 1f : 0.95f;
            this._part1.hSpeed = this.hSpeed * num1;
            this._part1.vSpeed = this.vSpeed;
            this._part2.hSpeed = this.hSpeed;
            this._part2.vSpeed = this.vSpeed;
            this._part3.hSpeed = this.hSpeed * num2;
            this._part3.vSpeed = this.vSpeed;
            this._part1.enablePhysics = false;
            this._part2.enablePhysics = false;
            this._part3.enablePhysics = false;
            this._part1.Update();
            this._part2.Update();
            this._part3.Update();
            this._part1.enablePhysics = true;
            this._part2.enablePhysics = true;
            this._part3.enablePhysics = true;
            if (Network.isActive)
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                Thing.Fondle(_part1, DuckNetwork.localConnection);
                Thing.Fondle(_part2, DuckNetwork.localConnection);
                Thing.Fondle(_part3, DuckNetwork.localConnection);
            }
            if (this._duck == null || !this._duck.onFire)
                return;
            this._part1.Burn(this._part1.position, this._duck.lastBurnedBy);
            this._part2.Burn(this._part2.position, this._duck.lastBurnedBy);
        }

        public void LightOnFire()
        {
            if (this._duck == null)
                return;
            this._part1.Burn(this._part1.position, this._duck.lastBurnedBy);
            this._part2.Burn(this._part2.position, this._duck.lastBurnedBy);
        }

        public void Unragdoll()
        {
            if (this.isInPipe)
                return;
            int num = this._duck.HasEquipment(typeof(FancyShoes)) ? 1 : 0;
            this._duck.visible = true;
            if (Network.isActive)
            {
                this._part2.UpdateLastReasonablePosition(this._part2.position);
                this._duck.position = this._part2._lastReasonablePosition;
            }
            else
                this._duck.position = this._part2.position;
            if (num == 0)
                this._duck.position.y -= 20f;
            this._duck.hSpeed = this._part2.hSpeed;
            this._duck.immobilized = false;
            this._duck.enablePhysics = true;
            this._duck._jumpValid = 0;
            this._duck._lastHoldItem = null;
            this._makeActive = false;
            this._part2.ReturnItemToWorld(_duck);
            if (Network.isActive)
            {
                this.active = false;
                this.visible = false;
                this.owner = null;
                if ((double)this.y > -1000.0)
                {
                    this.y = -9999f;
                    this._part1.y = -9999f;
                    this._part2.y = -9999f;
                    this._part3.y = -9999f;
                }
                this._part1.owner = null;
                this._part2.owner = null;
                this._part3.owner = null;
                if (this._duck.isServerForObject)
                {
                    Thing.Fondle(this, this._duck.connection);
                    Thing.Fondle(_part1, this._duck.connection);
                    Thing.Fondle(_part2, this._duck.connection);
                    Thing.Fondle(_part3, this._duck.connection);
                }
            }
            else
                Level.Remove(this);
            this._duck.ragdoll = null;
            if (num == 0)
                this._duck.vSpeed = -2f;
            else
                this._duck.vSpeed = this._part2.vSpeed;
        }

        public void Shot(Bullet bullet)
        {
            if (this._duck == null || this._duck.dead)
                return;
            this._duck.position = this._part2.position;
            this._duck.Kill(new DTShot(bullet));
            this._duck.y -= 5000f;
        }

        public void Killed(DestroyType t)
        {
            if (this._duck == null || this._duck.dead || t == null)
                return;
            this._duck.position = this._part2.position;
            this._duck.Destroy(t);
            this._duck.y -= 5000f;
        }

        public void LitOnFire(Thing litBy)
        {
            if (this._duck == null || this._duck.onFire)
                return;
            this._duck.Burn(this.position, litBy);
        }

        public override void Terminate()
        {
            if (this._part1 == null || this._part2 == null || this._part3 == null)
                return;
            Level.Remove(_part1);
            Level.Remove(_part2);
            Level.Remove(_part3);
        }

        public void Solve(PhysicsObject body1, PhysicsObject body2, float dist)
        {
            float num1 = dist;
            Vec2 vec2_1 = body2.position - body1.position;
            float num2 = vec2_1.length;
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(body1.hSpeed, body1.vSpeed);
            Vec2 vec2_4 = new Vec2(body2.hSpeed, body2.vSpeed);
            double num3 = (double)Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.1f;
            float num6 = 2.1f;
            if (body1 == this.part1 && this.jetting)
                num5 = 6f;
            else if (body2 == this.part1 && this.jetting)
                num6 = 6f;
            double num7 = (double)num4;
            float num8 = (float)((num3 + num7) / ((double)num5 + (double)num6));
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

        public float SpecialSolve(PhysicsObject b1, PhysicsObject b2, float dist)
        {
            Thing thing1 = b1.owner != null ? b1.owner : b1;
            Thing thing2 = b2.owner != null ? b2.owner : b2;
            float num1 = dist;
            Vec2 vec2_1 = b2.position - b1.position;
            float num2 = vec2_1.length;
            if ((double)num2 < 0.0001f)
                num2 = 0.0001f;
            if ((double)num2 < (double)num1)
                return 0.0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing1.hSpeed, thing1.vSpeed);
            Vec2 vec2_4 = new Vec2(thing2.hSpeed, thing2.vSpeed);
            double num3 = (double)Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing1 is ChainLink && !(thing2 is ChainLink))
            {
                num5 = 10f;
                num6 = 0.0f;
            }
            else if (thing2 is ChainLink && !(thing1 is ChainLink))
            {
                num5 = 0.0f;
                num6 = 10f;
            }
            else if (thing1 is ChainLink && thing2 is ChainLink)
            {
                num5 = 10f;
                num6 = 10f;
            }
            if (thing1 is RagdollPart)
                num5 = !this._zekeBear ? 10f : 4f;
            else if (thing2 is RagdollPart)
                num6 = !this._zekeBear ? 10f : 4f;
            double num7 = (double)num4;
            float num8 = (float)((num3 + num7) / ((double)num5 + (double)num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            vec2_4 -= vec2_5 * num6;
            thing1.hSpeed = vec2_6.x;
            thing1.vSpeed = vec2_6.y;
            thing2.hSpeed = vec2_4.x;
            thing2.vSpeed = vec2_4.y;
            if (thing1 is ChainLink && (double)(thing2.position - thing1.position).length > (double)num1 * 12.0)
                thing1.position = this.position;
            if (thing2 is ChainLink && (double)(thing2.position - thing1.position).length > (double)num1 * 12.0)
                thing2.position = this.position;
            return num8;
        }

        public float SpecialSolve(PhysicsObject b1, Vec2 stuck, float dist)
        {
            Thing thing = b1.owner != null ? b1.owner : b1;
            float num1 = dist;
            Vec2 vec2_1 = stuck - b1.position;
            float num2 = vec2_1.length;
            if ((double)num2 < 0.0001f)
                num2 = 0.0001f;
            if ((double)num2 < (double)num1)
                return 0.0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing.hSpeed, thing.vSpeed);
            Vec2 vec2_4 = new Vec2(0.0f, 0.0f);
            double num3 = (double)Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing is RagdollPart)
                num5 = !this._zekeBear ? 10f : 4f;
            double num7 = (double)num4;
            float num8 = (float)((num3 + num7) / ((double)num5 + (double)num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            Vec2 vec2_7 = vec2_4 - vec2_5 * num6;
            thing.hSpeed = vec2_6.x;
            thing.vSpeed = vec2_6.y;
            return num8;
        }

        public bool makeActive
        {
            get => this._makeActive;
            set => this._makeActive = value;
        }

        public override void Update()
        {
            if (this.removeFromLevel || this.isOffBottomOfLevel && this.captureDuck != null && this.captureDuck.dead)
                return;
            this._timeSinceNudge += 0.07f;
            if (this._part1 == null || this._part2 == null || this._part3 == null)
                return;
            if (_zap > 0.0)
            {
                this._part1.vSpeed += Rando.Float(-1f, 0.5f);
                this._part1.hSpeed += Rando.Float(-0.5f, 0.5f);
                this._part2.vSpeed += Rando.Float(-1f, 0.5f);
                this._part2.hSpeed += Rando.Float(-0.5f, 0.5f);
                this._part3.vSpeed += Rando.Float(-1f, 0.5f);
                this._part3.hSpeed += Rando.Float(-0.5f, 0.5f);
                this._part1.x += Rando.Int(-2, 2);
                this._part1.y += Rando.Int(-2, 2);
                this._part2.x += Rando.Int(-2, 2);
                this._part2.y += Rando.Int(-2, 2);
                this._part3.x += Rando.Int(-2, 2);
                this._part3.y += Rando.Int(-2, 2);
                this._zap -= 0.05f;
                this._wasZapping = true;
            }
            else if (this._wasZapping)
            {
                this._wasZapping = false;
                if (this.captureDuck != null)
                {
                    if (this.captureDuck.dead)
                    {
                        this.captureDuck.Ressurect();
                        return;
                    }
                    this.captureDuck.Kill(new DTElectrocute(this._zapper));
                    return;
                }
            }
            if (this.captureDuck != null && this.captureDuck.inputProfile != null && this.captureDuck.isServerForObject)
            {
                if (this.captureDuck.inputProfile.Pressed("JUMP") && this.captureDuck.HasEquipment(typeof(Jetpack)))
                    this.captureDuck.GetEquipment(typeof(Jetpack)).PressAction();
                if (this.captureDuck.inputProfile.Released("JUMP") && this.captureDuck.HasEquipment(typeof(Jetpack)))
                    this.captureDuck.GetEquipment(typeof(Jetpack)).ReleaseAction();
            }
            this.partSep = 6f;
            if (this._zekeBear)
                this.partSep = 4f;
            Vec2 vec2_1 = this._part1.position - this._part3.position;
            if ((double)vec2_1.length > partSep * 5.0)
            {
                if (this._part1.owner != null)
                {
                    RagdollPart part2 = this._part2;
                    this._part3.position = vec2_1 = this._part1.position;
                    Vec2 vec2_2 = vec2_1;
                    part2.position = vec2_2;
                }
                else if (this._part3.owner != null)
                {
                    RagdollPart part1 = this._part1;
                    this._part2.position = vec2_1 = this._part3.position;
                    Vec2 vec2_3 = vec2_1;
                    part1.position = vec2_3;
                }
                else
                {
                    RagdollPart part1 = this._part1;
                    this._part3.position = vec2_1 = this._part2.position;
                    Vec2 vec2_4 = vec2_1;
                    part1.position = vec2_4;
                }
                this._part1.vSpeed = this._part2.vSpeed = this._part3.vSpeed = 0.0f;
                this._part1.hSpeed = this._part2.hSpeed = this._part3.hSpeed = 0.0f;
                this.Solve(_part1, _part2, this.partSep);
                this.Solve(_part2, _part3, this.partSep);
                this.Solve(_part1, _part3, this.partSep * 2f);
            }
            this.Solve(_part1, _part2, this.partSep);
            this.Solve(_part2, _part3, this.partSep);
            this.Solve(_part1, _part3, this.partSep * 2f);
            if (this._part1.owner is Duck && this._part3.owner is Duck)
            {
                double num1 = (double)this.SpecialSolve(_part3, this._part1.owner as Duck, 16f);
                double num2 = (double)this.SpecialSolve(_part1, this._part3.owner as Duck, 16f);
            }
            if (this.tongueStuck != Vec2.Zero && this.captureDuck != null)
            {
                Vec2 vec2_5 = this.tongueStuck + new Vec2(captureDuck.offDir * -4, -6f);
                if (this._part1.owner is Duck)
                {
                    double num3 = (double)this.SpecialSolve(_part3, this._part1.owner as Duck, 16f);
                    double num4 = (double)this.SpecialSolve(_part1, vec2_5, 16f);
                }
                if (this._part3.owner is Duck)
                {
                    double num5 = (double)this.SpecialSolve(_part1, this._part3.owner as Duck, 16f);
                    double num6 = (double)this.SpecialSolve(_part3, vec2_5, 16f);
                }
                vec2_1 = this.part1.position - vec2_5;
                if ((double)vec2_1.length > 4.0)
                {
                    double num = (double)this.SpecialSolve(_part1, vec2_5, 4f);
                    vec2_1 = vec2_5 - this.part1.position;
                    Vec2 normalized = vec2_1.normalized;
                    vec2_1 = this.part1.position - vec2_5;
                    if ((double)vec2_1.length > 12.0)
                        this.part1.position = Lerp.Vec2Smooth(this.part1.position, vec2_5, 0.2f);
                }
            }
            this.position = (this._part1.position + this._part2.position + this._part3.position) / 3f;
            if (this._duck == null || _zap > 0.0)
                return;
            if (this._duck.eyesClosed)
                this._part1.frame = 20;
            if (!this._duck.fancyShoes || this._duck.framesSinceRagdoll >= 1)
                this.UpdateInput();
            bool flag = false;
            if (this.isServerForObject)
            {
                if (this.isOffBottomOfLevel && !this._duck.dead)
                {
                    flag = this._duck.Kill(new DTFall());
                    ++this._duck.profile.stats.fallDeaths;
                }
                this.jetting = false;
            }
            if (!flag)
                return;
            this._duck.y = this._part2.y - 9999f;
            this._duck.x = this._part2.x;
        }

        public override void Draw()
        {
        }

        public bool TryingToControl()
        {
            if (this.captureDuck == null)
                return false;
            return this.captureDuck.inputProfile.Pressed("LEFT") || this.captureDuck.inputProfile.Pressed("RIGHT") || this.captureDuck.inputProfile.Pressed("UP") || this.captureDuck.inputProfile.Pressed("RAGDOLL") || this.captureDuck.inputProfile.Pressed("JUMP");
        }

        public void UpdateInput()
        {
            this.sleepingBagTimer -= Maths.IncFrameTimer();
            if (sleepingBagTimer < 0.0 && this.sleepingBagHealth > 20)
            {
                this.sleepingBagHealth -= 4;
                this.sleepingBagTimer = 1f;
            }
            if (!this._duck.dead)
            {
                if (this._duck.HasEquipment(typeof(FancyShoes)) && !this.jetting)
                {
                    if (this.captureDuck.inputProfile.Pressed("RIGHT"))
                    {
                        Vec2 vec2_1 = (this._part1.position - this._part2.position).Rotate(1.570796f, Vec2.Zero);
                        RagdollPart part1 = this.part1;
                        part1.velocity += vec2_1 * 0.2f;
                        Vec2 vec2_2 = (this._part3.position - this._part2.position).Rotate(1.570796f, Vec2.Zero);
                        RagdollPart part3 = this.part3;
                        part3.velocity += vec2_2 * 0.2f;
                    }
                    else if (this.captureDuck.inputProfile.Pressed("LEFT"))
                    {
                        Vec2 vec2_3 = (this._part1.position - this._part2.position).Rotate(1.570796f, Vec2.Zero);
                        RagdollPart part1 = this.part1;
                        part1.velocity += vec2_3 * -0.2f;
                        Vec2 vec2_4 = (this._part3.position - this._part2.position).Rotate(1.570796f, Vec2.Zero);
                        RagdollPart part3 = this.part3;
                        part3.velocity += vec2_4 * -0.2f;
                    }
                }
                else if (_timeSinceNudge > 1.0 && !this.jetting)
                {
                    if (this.captureDuck.inputProfile.Pressed("LEFT"))
                    {
                        this._part1.vSpeed += NetRand.Float(-2f, 2f);
                        this._part3.vSpeed += NetRand.Float(-2f, 2f);
                        this._part2.hSpeed += NetRand.Float(-2f, -1.2f);
                        this._part2.vSpeed -= NetRand.Float(1f, 1.5f);
                        this._timeSinceNudge = 0.0f;
                        this.ShakeOutOfSleepingBag();
                    }
                    else if (this.captureDuck.inputProfile.Pressed("RIGHT"))
                    {
                        this._part1.vSpeed += NetRand.Float(-2f, 2f);
                        this._part3.vSpeed += NetRand.Float(-2f, 2f);
                        this._part2.hSpeed += NetRand.Float(1.2f, 2f);
                        this._part2.vSpeed -= NetRand.Float(1f, 1.5f);
                        this._timeSinceNudge = 0.0f;
                        this.ShakeOutOfSleepingBag();
                    }
                    else if (this.captureDuck.inputProfile.Pressed("UP"))
                    {
                        this._part1.vSpeed += NetRand.Float(-2f, 1f);
                        this._part3.vSpeed += NetRand.Float(-2f, 1f);
                        this._part2.vSpeed -= NetRand.Float(1.5f, 2f);
                        this._timeSinceNudge = 0.0f;
                        this.ShakeOutOfSleepingBag();
                    }
                }
            }
            bool flag = false;
            if (this.captureDuck.HasEquipment(typeof(FancyShoes)) && (double)Math.Abs(this._part1.x - this._part3.x) < 9.0 && (double)this._part1.y < (double)this._part3.y)
                flag = true;
            if (this.tongueStuckThing != null && this.tongueStuckThing.removeFromLevel)
            {
                this.tongueStuck = Vec2.Zero;
                if (Network.isActive)
                    Thing.Fondle(this, DuckNetwork.localConnection);
                this._makeActive = true;
            }
            if (this._duck.dead || !this.captureDuck.inputProfile.Pressed("RAGDOLL") && !this.captureDuck.inputProfile.Pressed("JUMP") || ((this._part1.framesSinceGrounded < 5 || this._part2.framesSinceGrounded < 5 || this._part3.framesSinceGrounded < 5 ? 1 : (this._part1.doFloat || this.part2.doFloat ? 1 : (this._part3.doFloat ? 1 : 0))) | (flag ? 1 : 0)) == 0 && this._part1.owner == null && this._part2.owner == null && this._part3.owner == null)
                return;
            if (this.inSleepingBag)
            {
                if (_timeSinceNudge <= 1.0)
                    return;
                this._part1.vSpeed += NetRand.Float(-2f, 1f);
                this._part3.vSpeed += NetRand.Float(-2f, 1f);
                this._part2.vSpeed -= NetRand.Float(1.5f, 2f);
                this._timeSinceNudge = 0.0f;
                this.ShakeOutOfSleepingBag();
            }
            else
            {
                if (this._part1.held || this._part2.held || this._part3.held || !(this.tongueStuck == Vec2.Zero) && this.tongueShakes <= 5 || !this.isServerForObject)
                    return;
                this.tongueStuck = Vec2.Zero;
                if (Network.isActive)
                    Thing.Fondle(this, DuckNetwork.localConnection);
                this._makeActive = true;
            }
        }

        public void ShakeOutOfSleepingBag()
        {
            ++this.tongueShakes;
            if (this._part1 != null && this._part1.owner == null && this._part2 != null && this._part2.owner == null && this._part3 != null && this._part3.owner == null && this.tongueStuck != Vec2.Zero && this.tongueShakes > 5)
                this.tongueStuck = Vec2.Zero;
            if (this.sleepingBagHealth < 0 || this.captureDuck == null)
                return;
            this.sleepingBagHealth = (byte)Math.Max(sleepingBagHealth - 5, 0);
            if (this.sleepingBagHealth != 0)
                return;
            if (this.inSleepingBag && this.captureDuck.isServerForObject)
                this._makeActive = true;
            this.inSleepingBag = false;
        }

        private bool isInPipe
        {
            get
            {
                if (this.part1 != null && this.part1.inPipe || this.part2 != null && this.part2.inPipe || this.part3 != null && this.part3.inPipe || this.inPipe)
                    return true;
                return this.captureDuck != null && this.captureDuck.inPipe;
            }
        }

        public void UpdateUnragdolling()
        {
            if (this.isInPipe || this.captureDuck == null || !this.captureDuck.isServerForObject || !this._makeActive)
                return;
            this.Unragdoll();
        }
    }
}
