// Decompiled with JetBrains decompiler
// Type: DuckGame.TrappedDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class TrappedDuck : Holdable, IPlatform, IAmADuck
    {
        public StateBinding _duckOwnerBinding = new StateBinding(nameof(_duckOwner));
        public Duck _duckOwner;
        public float _trapTime = 1f;
        public float _shakeMult;
        private float _shakeInc;
        public byte funNum;
        public bool infinite;
        private bool extinguishing;
        private float jumpCountdown;
        private bool _prevVisible;
        private int framesInvisible;
        private Vec2 _stickLerp;
        private Vec2 _stickSlowLerp;

        public Duck captureDuck => this._duckOwner;

        public override bool visible
        {
            get => base.visible;
            set
            {
                if (value && _trapTime < 0.0)
                {
                    this._trapTime = 1f;
                    this.owner = null;
                }
                base.visible = value;
            }
        }

        public TrappedDuck(float xpos, float ypos, Duck duckowner)
          : base(xpos, ypos)
        {
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
            this.thickness = 0.5f;
            this.weight = 5f;
            this.flammable = 1f;
            this.burnSpeed = 0.0f;
            this._duckOwner = duckowner;
            this.tapeable = false;
            this.InitializeStuff();
        }

        public void InitializeStuff() => this._trapTime = 1f;

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (this._duckOwner != null)
                this._duckOwner.Burn(firePosition, litBy);
            return base.OnBurn(firePosition, litBy);
        }

        public override void Extinquish()
        {
            if (this.extinguishing)
                return;
            this.extinguishing = true;
            if (this._duckOwner != null)
                this._duckOwner.Extinquish();
            base.Extinquish();
            this.extinguishing = false;
        }

        public override void Terminate() => base.Terminate();

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._duckOwner == null)
                return false;
            if (!this.destroyed)
            {
                this._duckOwner.hSpeed = this.hSpeed;
                bool flag = type != null;
                if (!flag && jumpCountdown > 0.00999999977648258)
                    this._duckOwner.vSpeed = Duck.JumpSpeed;
                else
                    this._duckOwner.vSpeed = flag ? this.vSpeed - 1f : -3f;
                this._duckOwner.x = this.x;
                this._duckOwner.y = this.y - 10f;
                for (int index = 0; index < 4; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f));
                    smallSmoke.hSpeed += this.hSpeed * Rando.Float(0.3f, 0.5f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
                if (this.duck != null)
                {
                    if (this.held)
                    {
                        if (this.duck.holdObject == this)
                            this.duck.holdObject = null;
                    }
                    else if (this.duck.holstered == this)
                        this.duck.holstered = null;
                }
                if (Network.isActive)
                {
                    if (!flag)
                    {
                        this._duckOwner.Fondle(this);
                        this.authority += 30;
                    }
                    this.active = false;
                    this.visible = false;
                    this.owner = null;
                }
                else
                    Level.Remove(this);
                if (this._duckOwner.owner == this)
                    this._duckOwner.owner = null;
                if (flag && !this._duckOwner.killingNet)
                {
                    this._duckOwner.killingNet = true;
                    this._duckOwner.Destroy(type);
                }
                this._duckOwner._trapped = null;
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && (this._duckOwner == null || !this._duckOwner.HitArmor(bullet, hitPos)))
                this.OnDestroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
        }

        public override void InactiveUpdate()
        {
            if (!this.isServerForObject)
                return;
            this.y = -9999f;
            this.visible = false;
        }

        public override void Update()
        {
            if (Network.isActive && this._prevVisible && !this.visible)
            {
                for (int index = 0; index < 4; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f));
                    smallSmoke.hSpeed += this.hSpeed * Rando.Float(0.3f, 0.5f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
            }
            if (this._duckOwner == null)
                return;
            ++this._framesSinceTransfer;
            base.Update();
            if (this.isOffBottomOfLevel)
                this.OnDestroy(new DTFall());
            this.jumpCountdown -= Maths.IncFrameTimer();
            this._prevVisible = this.visible;
            this._shakeInc += 0.8f;
            this._shakeMult = Lerp.Float(this._shakeMult, 0.0f, 0.05f);
            if (Network.isActive && this._duckOwner._trapped == this && !this._duckOwner.isServerForObject && this._duckOwner.inputProfile.Pressed("JUMP"))
                this._shakeMult = 1f;
            if (this._duckOwner.isServerForObject && this._duckOwner._trapped == this)
            {
                if (!this.visible && this.owner == null)
                {
                    ++this.framesInvisible;
                    if (this.framesInvisible > 30)
                    {
                        this.framesInvisible = 0;
                        this.y = -9999f;
                    }
                }
                if (!this.infinite)
                {
                    this._duckOwner.profile.stats.timeInNet += Maths.IncFrameTimer();
                    if (this._duckOwner.inputProfile.Pressed("JUMP"))
                    {
                        this._shakeMult = 1f;
                        this._trapTime -= 0.007f;
                        this.jumpCountdown = 0.25f;
                    }
                    if (this.grounded && this._duckOwner.inputProfile.Pressed("JUMP"))
                    {
                        this._shakeMult = 1f;
                        this._trapTime -= 0.028f;
                        if (this.owner == null)
                        {
                            if ((double)Math.Abs(this.hSpeed) < 1.0 && this._framesSinceTransfer > 30)
                                this._duckOwner.Fondle(this);
                            this.vSpeed -= Rando.Float(0.8f, 1.1f);
                            if (this._duckOwner.inputProfile.Down("LEFT") && (double)this.hSpeed > -1.0)
                                this.hSpeed -= Rando.Float(0.6f, 0.8f);
                            if (this._duckOwner.inputProfile.Down("RIGHT") && (double)this.hSpeed < 1.0)
                                this.hSpeed += Rando.Float(0.6f, 0.8f);
                        }
                    }
                    if (this._duckOwner.inputProfile.Pressed("JUMP") && this._duckOwner.HasEquipment(typeof(Jetpack)))
                        this._duckOwner.GetEquipment(typeof(Jetpack)).PressAction();
                    if (this._duckOwner.inputProfile.Released("JUMP") && this._duckOwner.HasEquipment(typeof(Jetpack)))
                        this._duckOwner.GetEquipment(typeof(Jetpack)).ReleaseAction();
                    this._trapTime -= 0.0028f;
                    if ((_trapTime <= 0.0 || this._duckOwner.dead) && !this.inPipe)
                        this.OnDestroy(null);
                }
                this._duckOwner.UpdateSkeleton();
                this.weight = 5f;
            }
            if (this._duckOwner._trapped == this)
                this._duckOwner.position = this.position;
            if (this.owner != null)
                return;
            this.depth = this._duckOwner.depth - 10;
        }

        public override void Draw()
        {
            if (this._duckOwner == null)
                return;
            this._duckOwner._sprite.SetAnimation("netted");
            this._duckOwner._sprite.imageIndex = 14;
            this._duckOwner._spriteQuack.frame = this._duckOwner._sprite.frame;
            this._duckOwner._sprite.depth = this.depth;
            this._duckOwner._spriteQuack.depth = this.depth;
            if (Network.isActive)
                this._duckOwner.DrawConnectionIndicators();
            float num1 = 0.0f;
            if (this.owner != null)
                num1 = (float)(Math.Sin(_shakeInc) * _shakeMult * 1.0);
            if (this._duckOwner.quack > 0)
            {
                Vec2 tounge = this._duckOwner.tounge;
                if (!this._duckOwner._spriteQuack.flipH && tounge.x < 0.0)
                    tounge.x = 0.0f;
                if (this._duckOwner._spriteQuack.flipH && tounge.x > 0.0)
                    tounge.x = 0.0f;
                if (tounge.y < -0.300000011920929)
                    tounge.y = -0.3f;
                if (tounge.y > 0.400000005960464)
                    tounge.y = 0.4f;
                this._stickLerp = Lerp.Vec2Smooth(this._stickLerp, tounge, 0.2f);
                this._stickSlowLerp = Lerp.Vec2Smooth(this._stickSlowLerp, tounge, 0.1f);
                Vec2 stickLerp = this._stickLerp;
                stickLerp.y *= -1f;
                Vec2 stickSlowLerp = this._stickSlowLerp;
                stickSlowLerp.y *= -1f;
                int num2 = 0;
                double length = (double)stickLerp.length;
                if (length > 0.5)
                    num2 = 72;
                Graphics.Draw(this._duckOwner._spriteQuack, this._duckOwner._sprite.imageIndex + num2, this.x + num1, this.y - 8f);
                if (length > 0.0500000007450581)
                {
                    Vec2 vec2_1 = this.position + new Vec2(num1 + (this._duckOwner._spriteQuack.flipH ? -1f : 1f), -2f);
                    List<Vec2> vec2List = Curve.Bezier(8, vec2_1, vec2_1 + stickSlowLerp * 6f, vec2_1 + stickLerp * 6f);
                    Vec2 vec2_2 = Vec2.Zero;
                    float num3 = 1f;
                    foreach (Vec2 p2 in vec2List)
                    {
                        if (vec2_2 != Vec2.Zero)
                        {
                            Vec2 vec2_3 = vec2_2 - p2;
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num3, this.depth + 1);
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2 - vec2_3.normalized * 0.4f, Color.Black, 0.3f * num3, this.depth - 1);
                        }
                        num3 -= 0.1f;
                        vec2_2 = p2;
                    }
                    if (this._duckOwner._spriteQuack != null)
                    {
                        this._duckOwner._spriteQuack.alpha = this.alpha;
                        this._duckOwner._spriteQuack.angle = this.angle;
                        this._duckOwner._spriteQuack.depth = this.depth + 2;
                        this._duckOwner._spriteQuack.scale = this.scale;
                        this._duckOwner._spriteQuack.frame += 36;
                        this._duckOwner._spriteQuack.Draw();
                        this._duckOwner._spriteQuack.frame -= 36;
                    }
                }
            }
            else
                Graphics.Draw(_duckOwner._sprite, this.x + num1, this.y - 8f);
            base.Draw();
        }
    }
}
