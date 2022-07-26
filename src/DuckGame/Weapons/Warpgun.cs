// Decompiled with JetBrains decompiler
// Type: DuckGame.Warpgun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    [BaggedProperty("isOnlineCapable", true)]
    [BaggedProperty("isInDemo", true)]
    public class Warpgun : Gun
    {
        public StateBinding _gravMultTimeBinding = new StateBinding(nameof(gravMultTime));
        public StateBinding _shotsSinceGroundedBinding = new StateBinding(nameof(shotsSinceGrounded));
        public StateBinding _shotsSinceDuckWasGroundedBinding = new StateBinding(nameof(shotsSinceDuckWasGrounded));
        private SpriteMap _sprite;
        private Sprite _warpLine;
        public int shotsSinceGrounded;
        protected new Sprite _sightHit;
        private Tex2D _laserTex;
        private int maxUngroundedShots = 2;
        public List<WarpLine> warpLines = new List<WarpLine>();
        private Vec2 warpPos;
        private bool onUpdate;
        public List<Warpgun.BlockGlow> blockGlows = new List<Warpgun.BlockGlow>();
        private int shotsSinceDuckWasGrounded;
        private int framesSinceShot;
        private float lerpShut;
        private Vec2 _warpPoint;
        private float gravMultTime;
        private bool warped;
        private Duck lastDuck;

        public Warpgun(float xval, float yval)
          : base(xval, yval)
        {
            this._fireWait = 0.3f;
            this.ammo = 9999;
            this._ammoType = (AmmoType)new ATWagnus();
            this.angleMul = -1f;
            this._type = "gun";
            this._sprite = new SpriteMap("warpgun", 19, 17);
            this._sprite.speed = 0.0f;
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(11f, 8f);
            this.collisionOffset = new Vec2(-6f, -7f);
            this.collisionSize = new Vec2(12f, 14f);
            this._barrelOffsetTL = new Vec2(14f, 4f);
            this._fireSound = "warpgun";
            this._kickForce = 0.3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-1f, -2f);
            this._editorName = "WAGNUS";
            this.editorTooltip = "Science can be horrifying *and* FUN!";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._warpLine = new Sprite("warpLine2");
            this._sightHit = new Sprite("laserSightHit");
            this._sightHit.CenterOrigin();
            this._laserTex = Content.Load<Tex2D>("pointerLaser");
        }

        protected override void PlayFireSound() => this.PlaySFX(this._fireSound, pitch: (0.6f + Rando.Float(0.2f)));

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            if (owner is TargetDuck)
            {
                Block block = Level.CheckLine<Block>(this.position + new Vec2(this.offDir > (sbyte)0 ? -16f : 16f, 0.0f), owner.position + new Vec2(this.offDir > (sbyte)0 ? -16f : 16f, 8f));
                owner.holdObstructed = block != null;
            }
            else
            {
                Block block = Level.CheckLine<Block>(owner.position + new Vec2(this.offDir > (sbyte)0 ? -10f : 10f, 0.0f), owner.position + new Vec2(this.offDir > (sbyte)0 ? -10f : 10f, 10f));
                owner.holdObstructed = block != null;
            }
        }

        public override void UpdateAction()
        {
            if (!this.isServerForObject || this.onUpdate)
                return;
            this.onUpdate = true;
            if (this.duck != null && this.tape == null)
            {
                this.offDir = this.duck.offDir;
                this.CheckIfHoldObstructed();
                if (!this.duck._hovering && this.duck.holdObject != null && (!this.duck.HasEquipment(typeof(Holster)) || !this.duck.inputProfile.Down("UP")))
                {
                    this.duck.UpdateHoldLerp(true, true);
                    this.duck.UpdateHoldPosition();
                }
            }
            base.UpdateAction();
            this.onUpdate = false;
        }

        public override void Update()
        {
            this.ammo = 9999;
            if (this.isServerForObject && !this._triggerHeld)
                this.gravMultTime = 0.0f;
            IPlatform platform = Level.Nearest<IPlatform>(this.x, this.y);
            bool flag = false;
            if (platform != null && (double)((platform as Thing).position - this.position).length < 32.0)
                flag = true;
            if (!flag)
            {
                platform = Level.CheckCircle<IPlatform>(this.position, 18f);
                if (platform != null)
                    flag = true;
            }
            if (platform != null & flag && this.shotsSinceGrounded > 0 && this.framesSinceShot > 2)
            {
                if (this.shotsSinceGrounded > 1)
                    SFX.PlaySynchronized("laserChargeTeeny", 0.8f, 0.3f);
                this.shotsSinceGrounded = 0;
                for (int index1 = 0; index1 < 8; ++index1)
                {
                    float deg = (float)((double)index1 * 45.0 - 5.0) + Rando.Float(20f);
                    Vec2 position;
                    if (Level.CheckLine<IPlatform>(this.position, this.position + new Vec2((float)Math.Cos((double)Maths.DegToRad(deg)), (float)Math.Sin((double)Maths.DegToRad(deg))) * 32f, out position) != null)
                    {
                        this.blockGlows.Add(new Warpgun.BlockGlow()
                        {
                            pos = position
                        });
                        for (int index2 = 0; index2 < 4; ++index2)
                            Level.Add((Thing)WagnusChargeParticle.New(position.x + Rando.Float(-4f, 4f), position.y + Rando.Float(-4f, 4f), (Thing)this));
                    }
                }
            }
            this.ammoType.range = 128f;
            if (this.tape != null && this.tape.gun1 is Warpgun && this.tape.gun2 is Warpgun)
            {
                if (this == this.tape.gun2)
                    this.heat = (this.tape.gun1 as Warpgun).heat;
                this.ammoType.range *= 2f;
            }
            if (this.isServerForObject && (double)this.heat > 1.0)
            {
                this.explode = true;
                this.PressAction();
            }
            if (this.duck != null)
            {
                if (this.isServerForObject)
                {
                    if (this.duck.grounded)
                    {
                        this.shotsSinceDuckWasGrounded = 0;
                        if ((double)this.heat > 0.0)
                            this.heat -= 0.05f;
                    }
                    if (!(bool)this.infinite)
                    {
                        if (this.shotsSinceDuckWasGrounded >= 16)
                            this.heat = 1f;
                        else if (!(bool)this.infinite)
                        {
                            float num = Math.Min((float)this.shotsSinceDuckWasGrounded / 38f, 1f);
                            if ((double)this.heat < (double)num)
                                this.heat = num;
                        }
                    }
                }
                if (this.isServerForObject)
                {
                    if ((double)this.gravMultTime > 0.0 && !this.duck.inPipe)
                    {
                        this.heat += 0.005f;
                        if (this.warped)
                        {
                            this.duck.blendColor = Lerp.Color(Color.White, Color.Purple, this.gravMultTime);
                            this.duck.position = this.warpPos;
                            this.duck.vSpeed = -0.3f;
                            this.duck.hSpeed = -0.3f;
                        }
                    }
                    else
                    {
                        if (this.warped)
                        {
                            this.duck.gravMultiplier = 1f;
                            this.duck.blendColor = Color.White;
                        }
                        this.gravMultTime = 0.0f;
                    }
                }
                this.lastDuck = this.duck;
            }
            else if (this.lastDuck != null)
            {
                this.lastDuck.blendColor = Color.White;
                this.lastDuck.gravMultiplier = 1f;
                this.gravMultTime = 0.0f;
                this.warped = false;
                this.lastDuck = (Duck)null;
            }
            if (this.shotsSinceGrounded == 0 || (bool)this.infinite)
                this._sprite.frame = 0;
            else if (this.shotsSinceGrounded == 1)
            {
                this._sprite.frame = 1;
            }
            else
            {
                this.lerpShut += 0.2f;
                this._sprite.frame = (double)this.lerpShut >= 0.400000005960464 ? ((double)this.lerpShut >= 0.800000011920929 ? 4 : 3) : 2;
            }
            ++this.framesSinceShot;
            base.Update();
        }

        public override void OnPressAction()
        {
            if (!this.isServerForObject)
                return;
            Vec2 position = this.position;
            bool flag = false;
            if (this.duck != null)
            {
                if (this.duck.holdObject is TapedGun)
                    (this.duck.holdObject as TapedGun).UpdatePositioning();
                position = this.duck.position;
                if (this.duck.sliding)
                {
                    position.y += 4f;
                    flag = true;
                }
                else if (this.duck.crouch)
                {
                    position.y += 4f;
                    flag = true;
                }
            }
            if (this.shotsSinceGrounded >= this.maxUngroundedShots && !(bool)this.infinite)
                return;
            this.lerpShut = 0.0f;
            float num1 = -this.angle;
            if (this.offDir < (sbyte)0)
                num1 += 3.141593f;
            Vec2 vec2_1 = Vec2.Zero;
            float num2 = 999999f;
            float num3 = 999999f;
            Vec2 vec2_2 = Vec2.Zero;
            Thing ignore = (Thing)null;
            float num4 = 7f;
            if (flag)
                num4 = 5f;
            float num5 = 8f;
            if ((double)this.angleDegrees != 0.0 && (double)Math.Abs(this.angleDegrees) != 90.0 && (double)Math.Abs(this.angleDegrees) != 180.0)
                num5 = 24f;
            int num6 = 6;
            if ((double)Math.Abs((int)this.angleDegrees) < 70.0 && (double)Math.Abs((int)this.angleDegrees) > 65.0)
                num6 = 12;
            if ((double)Math.Abs((int)this.angleDegrees) > -70.0)
            {
                double num7 = (double)Math.Abs((int)this.angleDegrees);
            }
            for (int index = 0; index < 3; ++index)
            {
                Vec2 vec2_3 = new Vec2((float)Math.Cos((double)num1) * 134f, (float)-Math.Sin((double)num1) * 134f);
                if ((double)Math.Abs(vec2_3.x) < 16.0)
                {
                    num4 = 2f;
                    num6 = 8;
                }
                float num8 = (float)(-(double)num4 + (double)index * (double)num4);
                Vec2 start = position + new Vec2((float)Math.Cos((double)num1 + Math.PI / 2.0) * num8, (float)-Math.Sin((double)num1 + Math.PI / 2.0) * num8);
                Vec2 vec2_4 = start - vec2_3;
                Vec2 vec2_5 = -(start - vec2_4).normalized;
                Vec2 hitPos = Vec2.Zero;
                Thing thing = (Thing)Level.CheckRay<Desk>(start + vec2_5 * 8f, vec2_4 + new Vec2(0.2f, 0.2f), out hitPos);
                if (thing != null && (thing as Desk).flipped == 0)
                    thing = (Thing)null;
                if (thing == null)
                    thing = (Thing)Level.CheckRay<Block>(start, vec2_4 + new Vec2(0.2f, 0.2f), out hitPos);
                if (thing != null)
                {
                    Vec2 vec2_6 = start - (hitPos + vec2_5 * (float)-num6);
                    Vec2 vec2_7 = vec2_1 = hitPos + vec2_5 * (float)-num6;
                    if (index == 1)
                        num3 = vec2_6.length;
                    if ((double)vec2_6.length < (double)num2)
                        num2 = vec2_6.length;
                }
                else if ((double)(start - vec2_4).length < (double)num2)
                {
                    num2 = (start - vec2_4).length - 7f;
                    vec2_1 = start - (vec2_4 + vec2_5 * -7f);
                    if (index == 1)
                        num3 = num2;
                }
                if (index == 1)
                    vec2_2 = vec2_5;
                if (index == 2)
                    ignore = thing;
            }
            if ((double)num3 < 99999.0 && (double)num5 > 9.0 && (double)Math.Abs(num3 - num2) < (double)num5)
                num2 = num3;
            this.warpLines.Add(new WarpLine()
            {
                start = position,
                end = position + vec2_2 * num2,
                lerp = 0.6f,
                wide = this.duck == null || !this.duck.sliding ? 24f : 14f
            });
            if (this.isServerForObject)
            {
                this._ammoType.range = num2 - 8f;
                this._barrelOffsetTL = new Vec2(8f, 3f);
                this.Fire();
                if (Network.isActive)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, true, this.duck != null ? this.duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    this.firedBullets.Clear();
                }
                this._wait = 0.0f;
                this._barrelOffsetTL = new Vec2(8f, 15f);
                this.Fire();
                if (Network.isActive)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, true, this.duck != null ? this.duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    this.firedBullets.Clear();
                }
                this._wait = 0.0f;
                this._barrelOffsetTL = new Vec2(8f, 9f);
                this.Fire();
                if (Network.isActive)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, true, this.duck != null ? this.duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    this.firedBullets.Clear();
                }
                this._wait = 0.0f;
                this._barrelOffsetTL = new Vec2(8f, 4f);
                if (this.duck != null)
                {
                    if ((double)vec2_1.y < (double)this.duck.y - 16.0 && (double)Math.Abs(vec2_1.x - this.duck.x) < 16.0)
                        num2 -= 2f;
                    this.duck.position = this.duck.position + vec2_2 * num2;
                    this.duck.sleeping = false;
                    this.duck._disarmDisable = 10;
                    this.duck.gravMultiplier = 0.0f;
                    this.duck.OnTeleport();
                    this.duck.blendColor = Color.Purple;
                    this.warped = true;
                    this.gravMultTime = 1f;
                    Block block = Level.CheckLine<Block>(new Vec2(this.duck.position.x, this.duck.bottom - 5f), new Vec2(this.duck.position.x, this.duck.bottom - 2f));
                    if (block != null)
                        this.duck.bottom = block.top;
                    IPlatform platform = Level.CheckLine<IPlatform>(new Vec2(this.duck.position.x, this.duck.bottom - 2f), new Vec2(this.duck.position.x, this.duck.bottom + 1f), ignore);
                    if (platform != null && (platform as Thing).solid && (ignore == null || (double)ignore.top < (double)(platform as Thing).top - 0.5))
                        this.duck.bottom = (platform as Thing).top;
                    this.warpPos = this.duck.position;
                }
                else if (this.owner == null)
                {
                    this.position = position + vec2_2 * num2;
                    this.sleeping = false;
                }
                if (this.owner != null)
                {
                    this.owner.hSpeed = this.owner.vSpeed = -0.01f;
                    if (this.owner is MaterialThing)
                    {
                        foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(this.owner.topLeft, this.owner.bottomRight))
                        {
                            materialThing.OnSoftImpact(this.owner as MaterialThing, ImpactedFrom.Top);
                            if (this.owner != null)
                                materialThing.Touch(this.owner as MaterialThing);
                        }
                    }
                }
            }
            this.framesSinceShot = 0;
            ++this.shotsSinceGrounded;
            ++this.shotsSinceDuckWasGrounded;
            if ((double)this.heat > 0.800000011920929)
            {
                this.explode = true;
                this.PressAction();
            }
            if (this.level != null && (double)this.y < (double)this.level.topLeft.y - 256.0)
            {
                this.shotsSinceDuckWasGrounded = 16;
                this.heat = 1f;
            }
            if (this.shotsSinceDuckWasGrounded == 15 && !(bool)this.infinite)
                SFX.PlaySynchronized("wagnusAlert", 0.8f);
            if (this.shotsSinceGrounded != this.maxUngroundedShots || (bool)this.infinite)
                return;
            SFX.PlaySynchronized("laserUnchargeShortLoud", pitch: 0.7f);
        }

        public override void Draw() => base.Draw();

        public new Vec2 laserOffset => this.Offset(new Vec2(16f, 9f) - this.center + new Vec2(-15f, 1f));

        public override void DrawGlow()
        {
            foreach (Warpgun.BlockGlow blockGlow in this.blockGlows)
            {
                Graphics.DrawTexturedLine(this._warpLine.texture, blockGlow.pos, blockGlow.pos + new Vec2(0.0f, -4f), Color.Purple * blockGlow.glow, 0.25f, (Depth)0.9f);
                Graphics.DrawTexturedLine(this._warpLine.texture, blockGlow.pos, blockGlow.pos + new Vec2(0.0f, 4f), Color.Purple * blockGlow.glow, 0.25f, (Depth)0.9f);
                blockGlow.glow -= 0.05f;
            }
            this.blockGlows.RemoveAll((Predicate<Warpgun.BlockGlow>)(x => (double)x.glow < 0.00999999977648258));
            Color purple = Color.Purple;
            foreach (WarpLine warpLine in this.warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.end + vec2_1 * (1f - warpLine.lerp), warpLine.end, purple * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start + vec2_2 * warpLine.lerp, warpLine.start, purple * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.1f;
            }
            this.warpLines.RemoveAll((Predicate<WarpLine>)(v => (double)v.lerp >= 1.0));
            if (this.duck != null && this.visible)
            {
                if ((double)this.gravMultTime > 0.0)
                {
                    Graphics.DrawTexturedLine(this._warpLine.texture, new Vec2(this.duck.x, this.duck.y), new Vec2(this.duck.x, this.duck.top - 8f), purple * (this.gravMultTime + 0.2f), 0.7f, (Depth)0.9f);
                    Graphics.DrawTexturedLine(this._warpLine.texture, new Vec2(this.duck.x, this.duck.y), new Vec2(this.duck.x, this.duck.bottom + 8f), purple * (this.gravMultTime + 0.2f), 0.7f, (Depth)0.9f);
                }
                if (this.shotsSinceGrounded < this.maxUngroundedShots || (bool)this.infinite)
                {
                    float num = -this.angle;
                    if (this.offDir < (sbyte)0)
                        num += 3.141593f;
                    Vec2 laserOffset = this.laserOffset;
                    Vec2 p2 = laserOffset - new Vec2((float)Math.Cos((double)num) * 122f, (float)-Math.Sin((double)num) * 122f);
                    Vec2 vec2 = -(laserOffset - p2).normalized;
                    Vec2 hitPos = Vec2.Zero;
                    if (Level.CheckRay<Block>(laserOffset, p2 + new Vec2(0.2f, 0.2f), out hitPos) != null)
                    {
                        this._warpPoint = hitPos + vec2 * -9f;
                        p2 = hitPos;
                    }
                    else
                        this._warpPoint = p2 + new Vec2(-5f, 0.0f);
                    Graphics.DrawTexturedLine(this._laserTex, laserOffset, p2, Color.Red, 0.5f, this.depth - 1);
                    if (this._sightHit != null)
                    {
                        this._sightHit.color = Color.Red;
                        Graphics.Draw(this._sightHit, p2.x, p2.y);
                    }
                }
            }
            base.DrawGlow();
        }

        public class BlockGlow
        {
            public Block block;
            public float glow = 1f;
            public Vec2 pos;
        }
    }
}
