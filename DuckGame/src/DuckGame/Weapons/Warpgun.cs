// Decompiled with JetBrains decompiler
// Type: DuckGame.Warpgun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public List<BlockGlow> blockGlows = new List<BlockGlow>();
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
            _fireWait = 0.3f;
            ammo = 9999;
            _ammoType = new ATWagnus();
            angleMul = -1f;
            _type = "gun";
            _sprite = new SpriteMap("warpgun", 19, 17)
            {
                speed = 0f
            };
            graphic = _sprite;
            center = new Vec2(11f, 8f);
            collisionOffset = new Vec2(-6f, -7f);
            collisionSize = new Vec2(12f, 14f);
            _barrelOffsetTL = new Vec2(14f, 4f);
            _fireSound = "warpgun";
            _kickForce = 0.3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, -2f);
            _editorName = "WAGNUS";
            editorTooltip = "Science can be horrifying *and* FUN!";
            physicsMaterial = PhysicsMaterial.Metal;
            _warpLine = new Sprite("warpLine2");
            _sightHit = new Sprite("laserSightHit");
            _sightHit.CenterOrigin();
            _laserTex = Content.Load<Tex2D>("pointerLaser");
        }

        protected override void PlayFireSound() => PlaySFX(_fireSound, pitch: (0.6f + Rando.Float(0.2f)));

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            if (owner is TargetDuck)
            {
                Block block = Level.CheckLine<Block>(position + new Vec2(offDir > 0 ? -16f : 16f, 0f), owner.position + new Vec2(offDir > 0 ? -16f : 16f, 8f));
                owner.holdObstructed = block != null;
            }
            else
            {
                Block block = Level.CheckLine<Block>(owner.position + new Vec2(offDir > 0 ? -10f : 10f, 0f), owner.position + new Vec2(offDir > 0 ? -10f : 10f, 10f));
                owner.holdObstructed = block != null;
            }
        }

        public override void UpdateAction()
        {
            if (!isServerForObject || onUpdate)
                return;
            onUpdate = true;
            if (duck != null && tape == null)
            {
                offDir = duck.offDir;
                CheckIfHoldObstructed();
                if (!duck._hovering && duck.holdObject != null && (!duck.HasEquipment(typeof(Holster)) || !duck.inputProfile.Down("UP")))
                {
                    duck.UpdateHoldLerp(true, true);
                    duck.UpdateHoldPosition();
                }
            }
            base.UpdateAction();
            onUpdate = false;
        }

        public override void Update()
        {
            ammo = 9999;
            if (isServerForObject && !_triggerHeld)
                gravMultTime = 0f;
            IPlatform platform = Level.Nearest<IPlatform>(position, 32.0f);
            bool flag = false;
            if (platform != null) //((platform as Thing).position - position).length < 32.0
                flag = true;
            if (!flag)
            {
                platform = Level.CheckCircle<IPlatform>(position, 18f);
                if (platform != null)
                    flag = true;
            }
            if (platform != null & flag && shotsSinceGrounded > 0 && framesSinceShot > 2)
            {
                if (shotsSinceGrounded > 1)
                    SFX.PlaySynchronized("laserChargeTeeny", 0.8f, 0.3f);
                shotsSinceGrounded = 0;
                for (int index1 = 0; index1 < 8; ++index1)
                {
                    float deg = (float)(index1 * 45.0 - 5.0) + Rando.Float(20f);
                    Vec2 position;
                    if (Level.CheckLine<IPlatform>(this.position, this.position + new Vec2((float)Math.Cos(Maths.DegToRad(deg)), (float)Math.Sin(Maths.DegToRad(deg))) * 32f, out position) != null)
                    {
                        blockGlows.Add(new BlockGlow()
                        {
                            pos = position
                        });
                        for (int index2 = 0; index2 < DGRSettings.ActualParticleMultiplier * 4; ++index2)
                            Level.Add(WagnusChargeParticle.New(position.x + Rando.Float(-4f, 4f), position.y + Rando.Float(-4f, 4f), this));
                    }
                }
            }
            ammoType.range = 128f;
            if (tape != null && tape.gun1 is Warpgun && tape.gun2 is Warpgun)
            {
                if (this == tape.gun2)
                    heat = (tape.gun1 as Warpgun).heat;
                ammoType.range *= 2f;
            }
            if (isServerForObject && heat > 1.0)
            {
                explode = true;
                PressAction();
            }
            if (duck != null)
            {
                if (isServerForObject)
                {
                    if (duck.grounded)
                    {
                        shotsSinceDuckWasGrounded = 0;
                        if (heat > 0.0)
                            heat -= 0.05f;
                    }
                    if (!(bool)infinite)
                    {
                        if (shotsSinceDuckWasGrounded >= 16)
                            heat = 1f;
                        else if (!(bool)infinite)
                        {
                            float num = Math.Min(shotsSinceDuckWasGrounded / 38f, 1f);
                            if (heat < num)
                                heat = num;
                        }
                    }
                }
                if (isServerForObject)
                {
                    if (gravMultTime > 0.0 && !duck.inPipe)
                    {
                        heat += 0.005f;
                        if (warped)
                        {
                            duck.blendColor = Lerp.Color(Color.White, Color.Purple, gravMultTime);
                            duck.position = warpPos;
                            duck.vSpeed = -0.3f;
                            duck.hSpeed = -0.3f;
                        }
                    }
                    else
                    {
                        if (warped)
                        {
                            duck.gravMultiplier = 1f;
                            duck.blendColor = Color.White;
                        }
                        gravMultTime = 0f;
                    }
                }
                lastDuck = duck;
            }
            else if (lastDuck != null)
            {
                lastDuck.blendColor = Color.White;
                lastDuck.gravMultiplier = 1f;
                gravMultTime = 0f;
                warped = false;
                lastDuck = null;
            }
            if (shotsSinceGrounded == 0 || (bool)infinite)
                _sprite.frame = 0;
            else if (shotsSinceGrounded == 1)
            {
                _sprite.frame = 1;
            }
            else
            {
                lerpShut += 0.2f;
                _sprite.frame = lerpShut >= 0.4f ? (lerpShut >= 0.8f ? 4 : 3) : 2;
            }
            ++framesSinceShot;
            base.Update();
        }

        public override void OnPressAction()
        {
            if (!isServerForObject)
                return;
            Vec2 position = this.position;
            bool flag = false;
            if (duck != null)
            {
                if (duck.holdObject is TapedGun)
                    (duck.holdObject as TapedGun).UpdatePositioning();
                position = duck.position;
                if (duck.sliding)
                {
                    position.y += 4f;
                    flag = true;
                }
                else if (duck.crouch)
                {
                    position.y += 4f;
                    flag = true;
                }
            }
            if (shotsSinceGrounded >= maxUngroundedShots && !(bool)infinite)
                return;
            lerpShut = 0f;
            float num1 = -angle;
            if (offDir < 0)
                num1 += 3.141593f;
            Vec2 vec2_1 = Vec2.Zero;
            float num2 = 999999f;
            float num3 = 999999f;
            Vec2 vec2_2 = Vec2.Zero;
            Thing ignore = null;
            float num4 = 7f;
            if (flag)
                num4 = 5f;
            float num5 = 8f;
            if (angleDegrees != 0.0 && Math.Abs(angleDegrees) != 90.0 && Math.Abs(angleDegrees) != 180.0)
                num5 = 24f;
            int num6 = 6;
            if (Math.Abs((int)angleDegrees) < 70.0 && Math.Abs((int)angleDegrees) > 65.0)
                num6 = 12;
            if (Math.Abs((int)angleDegrees) > -70.0)
            {
                double num7 = Math.Abs((int)angleDegrees);
            }
            for (int index = 0; index < 3; ++index)
            {
                Vec2 vec2_3 = new Vec2((float)Math.Cos(num1) * 134f, (float)-Math.Sin(num1) * 134f);
                if (Math.Abs(vec2_3.x) < 16.0)
                {
                    num4 = 2f;
                    num6 = 8;
                }
                float num8 = (float)(-num4 + index * num4);
                Vec2 start = position + new Vec2((float)Math.Cos(num1 + Math.PI / 2.0) * num8, (float)-Math.Sin(num1 + Math.PI / 2.0) * num8);
                Vec2 vec2_4 = start - vec2_3;
                Vec2 vec2_5 = -(start - vec2_4).normalized;
                Vec2 hitPos = Vec2.Zero;
                Thing thing = Level.CheckRay<Desk>(start + vec2_5 * 8f, vec2_4 + new Vec2(0.2f, 0.2f), out hitPos);
                if (thing != null && (thing as Desk).flipped == 0)
                    thing = null;
                if (thing == null)
                    thing = Level.CheckRay<Block>(start, vec2_4 + new Vec2(0.2f, 0.2f), out hitPos);
                if (thing != null)
                {
                    Vec2 vec2_6 = start - (hitPos + vec2_5 * -num6);
                    Vec2 vec2_7 = vec2_1 = hitPos + vec2_5 * -num6;
                    if (index == 1)
                        num3 = vec2_6.length;
                    if (vec2_6.length < num2)
                        num2 = vec2_6.length;
                }
                else if ((start - vec2_4).length < num2)
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
            if (num3 < 99999.0 && num5 > 9.0 && Math.Abs(num3 - num2) < num5)
                num2 = num3;
            warpLines.Add(new WarpLine()
            {
                start = position,
                end = position + vec2_2 * num2,
                lerp = 0.6f,
                wide = duck == null || !duck.sliding ? 24f : 14f
            });
            if (isServerForObject)
            {
                _ammoType.range = num2 - 8f;
                _barrelOffsetTL = new Vec2(8f, 3f);
                Fire();
                if (Network.isActive)
                {
                    Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, true, duck != null ? duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    firedBullets.Clear();
                }
                _wait = 0f;
                _barrelOffsetTL = new Vec2(8f, 15f);
                Fire();
                if (Network.isActive)
                {
                    Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, true, duck != null ? duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    firedBullets.Clear();
                }
                _wait = 0f;
                _barrelOffsetTL = new Vec2(8f, 9f);
                Fire();
                if (Network.isActive)
                {
                    Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, true, duck != null ? duck.netProfileIndex : (byte)4), NetMessagePriority.Urgent);
                    firedBullets.Clear();
                }
                _wait = 0f;
                _barrelOffsetTL = new Vec2(8f, 4f);
                if (duck != null)
                {
                    if (vec2_1.y < duck.y - 16.0 && Math.Abs(vec2_1.x - duck.x) < 16.0)
                        num2 -= 2f;
                    duck.position = duck.position + vec2_2 * num2;
                    duck.sleeping = false;
                    duck._disarmDisable = 10;
                    duck.gravMultiplier = 0f;
                    duck.OnTeleport();
                    duck.blendColor = Color.Purple;
                    warped = true;
                    gravMultTime = 1f;
                    Block block = Level.CheckLine<Block>(new Vec2(duck.position.x, duck.bottom - 5f), new Vec2(duck.position.x, duck.bottom - 2f));
                    if (block != null)
                        duck.bottom = block.top;
                    IPlatform platform = Level.CheckLine<IPlatform>(new Vec2(duck.position.x, duck.bottom - 2f), new Vec2(duck.position.x, duck.bottom + 1f), ignore);
                    if (platform != null && (platform as Thing).solid && (ignore == null || ignore.top < (platform as Thing).top - 0.5))
                        duck.bottom = (platform as Thing).top;
                    warpPos = duck.position;
                }
                else if (owner == null)
                {
                    this.position = position + vec2_2 * num2;
                    sleeping = false;
                }
                if (owner != null)
                {
                    owner.hSpeed = owner.vSpeed = -0.01f;
                    if (owner is MaterialThing)
                    {
                        foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(owner.topLeft, owner.bottomRight))
                        {
                            materialThing.OnSoftImpact(owner as MaterialThing, ImpactedFrom.Top);
                            if (owner != null)
                                materialThing.Touch(owner as MaterialThing);
                        }
                    }
                }
            }
            framesSinceShot = 0;
            ++shotsSinceGrounded;
            ++shotsSinceDuckWasGrounded;
            if (heat > 0.8f)
            {
                explode = true;
                PressAction();
            }
            if (level != null && y < level.topLeft.y - 256.0f)
            {
                shotsSinceDuckWasGrounded = 16;
                heat = 1f;
            }
            if (shotsSinceDuckWasGrounded == 15 && !(bool)infinite)
                SFX.PlaySynchronized("wagnusAlert", 0.8f);
            if (shotsSinceGrounded != maxUngroundedShots || (bool)infinite)
                return;
            SFX.PlaySynchronized("laserUnchargeShortLoud", pitch: 0.7f);
        }

        public override void Draw() => base.Draw();

        public new Vec2 laserOffset => Offset(new Vec2(16f, 9f) - center + new Vec2(-15f, 1f));

        public override void DrawGlow()
        {
            foreach (BlockGlow blockGlow in blockGlows)
            {
                Graphics.DrawTexturedLine(_warpLine.texture, blockGlow.pos, blockGlow.pos + new Vec2(0f, -4f), Color.Purple * blockGlow.glow, 0.25f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, blockGlow.pos, blockGlow.pos + new Vec2(0f, 4f), Color.Purple * blockGlow.glow, 0.25f, (Depth)0.9f);
                blockGlow.glow -= 0.05f;
            }
            blockGlows.RemoveAll(x => x.glow < 0.01f);
            Color purple = Color.Purple;
            foreach (WarpLine warpLine in warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.end + vec2_1 * (1f - warpLine.lerp), warpLine.end, purple * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start + vec2_2 * warpLine.lerp, warpLine.start, purple * 0.8f, warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.1f;
            }
            warpLines.RemoveAll(v => v.lerp >= 1.0f);
            if (duck != null && visible)
            {
                if (gravMultTime > 0.0f)
                {
                    Graphics.DrawTexturedLine(_warpLine.texture, new Vec2(duck.x, duck.y), new Vec2(duck.x, duck.top - 8f), purple * (gravMultTime + 0.2f), 0.7f, (Depth)0.9f);
                    Graphics.DrawTexturedLine(_warpLine.texture, new Vec2(duck.x, duck.y), new Vec2(duck.x, duck.bottom + 8f), purple * (gravMultTime + 0.2f), 0.7f, (Depth)0.9f);
                }
                if (shotsSinceGrounded < maxUngroundedShots || (bool)infinite)
                {
                    float num = -angle;
                    if (offDir < 0)
                        num += 3.141593f;
                    Vec2 laserOffset = this.laserOffset;
                    Vec2 p2 = laserOffset - new Vec2((float)Math.Cos(num) * 122f, (float)-Math.Sin(num) * 122f);
                    Vec2 vec2 = -(laserOffset - p2).normalized;
                    Vec2 hitPos = Vec2.Zero;
                    if (Level.CheckRay<Block>(laserOffset, p2 + new Vec2(0.2f, 0.2f), out hitPos) != null)
                    {
                        _warpPoint = hitPos + vec2 * -9f;
                        p2 = hitPos;
                    }
                    else
                        _warpPoint = p2 + new Vec2(-5f, 0f);
                    Graphics.DrawTexturedLine(_laserTex, laserOffset, p2, Color.Red, 0.5f, depth - 1);
                    if (_sightHit != null)
                    {
                        _sightHit.color = Color.Red;
                        Graphics.Draw(_sightHit, p2.x, p2.y);
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
