using System;
using System.Collections.Generic;

namespace DuckGame
{
    public enum WarpStance
    {
        None,
        Idle,
        Swung,
        Tear
    }
    [ClientOnly]
    public class WarpScimitar : Gun
    {
        public StateBinding _lerpedAngleBinding = new StateBinding("_lerpedAngle");
        public StateBinding _netStanceBinding = new StateBinding("netStance");
        public StateBinding _markAlphaBinding = new StateBinding("markAlpha");
        public StateBinding _travelBinding = new StateBinding("travel");
        public StateBinding _flyingBinding = new StateBinding("flying");

        public override float angle
        {
            get
            {
                if (owner is WireMount) return _angle;
                return flying ? _angle : !held && owner != null ? Maths.DegToRad(_lerpedAngle) * offDir : _angle;
            }
            set => _angle = value;
        }


        public byte netStance
        {
            get
            {
                return (byte)stance;
            }
            set
            {
                stance = (WarpStance)value;
            }
        }
        public float _lerpedAngle;
        public Sprite warpFade;
        public Sprite blade;
        public MaterialEnergy mt;

        public Duck reGrab;
        public Vec2 travel;
        public List<Vec2> trailPos = new List<Vec2>();
        public List<float> trails = new List<float>();
        public List<float> fades = new List<float>();
        public List<Vec2> scals = new List<Vec2>();
        public float glowInc;
        public float IdleFloatCharge;
        public Vec2 bladeScale;
        public float warpSlash;
        //2 //1.5f //0.8f //0.6f
        //2 RED SCAL 2
        public float glow;

        public byte lastUnstance;
        public WarpStance stance;
        public WarpStance rolling;
        public float _swordAngle;
        public float _swingDif;
        public bool _swordFlip;
        public float _lerpBoost;

        public float onCool;

        public bool flying;
        public Block stuckOnto;

        public float lsScale;

        public List<LaserDiskParticle> trailer = new List<LaserDiskParticle>();

        public List<Vec2> flyTrail = new List<Vec2>();
        public List<Vec2> flyTrail2 = new List<Vec2>();

        public List<Vec2> reMark = new List<Vec2>();
        public List<Vec2> reMark2 = new List<Vec2>();
        public float markAlpha;
        public int swungTimer;


        public int trailDel;
        public int tearFrames;
        public WarpScimitar(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 3;//16 35
            graphic = new Sprite("energyWagnusHilt");
            blade = new Sprite("energyWagnusBlade");//warpFade
            warpFade = new Sprite("warpFade");
            center = new Vec2(8, 17.5f);
            collisionSize = new Vec2(4, 31);
            _collisionOffset = new Vec2(-3, -14);

            mt = new MaterialEnergy(this);
            mt.color = new Color(83, 14, 144);
            mt.timeMulti = 0.3f;

            _canRaise = false;
            glow = 0.6f;
            shouldbegraphicculled = false;
            _editorPreviewRotation = 90.0f;
            _editorPreviewOffset.x += 1;
            _editorPreviewWidth = 34;
        }//gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg
        public override void Thrown()
        {
            if (!isServerForObject || duck == null || duck.destroyed)
                return;
            if (duck == null || !duck.inputProfile.Down(Triggers.Grab))
                return;

            if (duck.inputProfile.Down(Triggers.Left) && duck.inputProfile.Down(Triggers.Right)) return;
            if (duck.inputProfile.Down(Triggers.Down) && duck.inputProfile.Down(Triggers.Up)) return;

            travel = Vec2.Zero;

            if (duck.inputProfile.Down(Triggers.Right)) travel.x += 1;
            if (duck.inputProfile.Down(Triggers.Left)) travel.x -= 1;
            if (duck.inputProfile.Down(Triggers.Up)) travel.y -= 1;
            if (duck.inputProfile.Down(Triggers.Down)) travel.y += 1;
            if (travel != Vec2.Zero)
            {
                reGrab = duck;
                flying = true;
                reMark.Clear();
                reMark2.Clear();
            }
        }
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
            if (stance == WarpStance.Idle && (duck == null || !duck._hovering))
            {
                stance = WarpStance.Swung;
            }
            rolling = stance;
            base.OnPressAction();
        }

        public override void OnHoldAction()
        {
            if (stance == WarpStance.Tear && tearFrames < -20 && _lerpedAngle > 70 )
            {
                if (duck != null)
                {
                    if (isServerForObject)
                    {
                        duck.hSpeed = offDir * 3;
                        SFX.Play("warpGun", 1, Rando.Float(0.6f, 0.7f));
                        for (int i = 0; i < 6; i++)
                        {
                            Level.Add(new WarpSlash() { unspeedMult = 3, ignore = duck, color = new Color(147, 64, 221), hSpeed = 4 * offDir, alpha = 0.7f, killFrames = 10, position = position, angle = angle, offDir = offDir });

                            mt.color = new Color(151, 89, 255);
                            trails.Add(_lerpedAngle);
                            trailPos.Add(position);
                            fades.Add(0.6f);
                            scals.Add(new Vec2(1, 2));
                            mt.glow = 1.5f;
                            x += 16 * offDir;
                            duck.x += 16 * offDir;
                        }
                    }
                    bladeScale = new Vec2(0, 2);
                    glowInc = 2;
                }     
               
                tearFrames = 10;
            }

            if (rolling == WarpStance.Idle && (duck != null && duck._hovering) && rolling == stance && onCool <= 0.3f)
            {
                IdleFloatCharge = Lerp.Float(IdleFloatCharge, 1, 0.03f);
                glow = IdleFloatCharge * 3;
                bladeScale = new Vec2(IdleFloatCharge * 0.3f, IdleFloatCharge);
            }
            else
            {
                IdleFloatCharge = 0;
                bladeScale = Lerp.Vec2Smooth(bladeScale, Vec2.Zero, 0.1f);
            }
            base.OnHoldAction();
        }
        public override void OnReleaseAction()
        {
            if (IdleFloatCharge > 0)
            {
                bladeScale = Vec2.Zero;
                warpSlash = IdleFloatCharge;
                IdleFloatCharge = 0;
            }
            rolling = WarpStance.None;
            base.OnReleaseAction();
        }
        public override void Draw()
        {
            base.Draw();

            Graphics.material = mt;
            blade.position = position;
            blade.alpha = alpha;
            blade.angle = angle;
            blade.depth = depth;
            blade.scale = scale + bladeScale;
            blade.center = center + new Vec2(0.5f, 0);
            blade.flipH = _swordFlip;
            Graphics.Draw(blade, x, y, depth - 2);


            Graphics.material = null;
            Vec2 v = position;
            for (int i = 0; i < trails.Count; i++)
            {
                warpFade.position = position;
                warpFade.depth = depth - 4;
                warpFade.center = center + new Vec2(0.5f, 0);
                warpFade.scale = scals[i];
                warpFade.angle = Maths.DegToRad(trails[i]) * offDir;
                warpFade.alpha = fades[i];
                warpFade.color = new Color(147, 64, 221);
                warpFade.flipH = _swordFlip;

                Vec2 vec = position;
                if (trailPos[i] != Vec2.Zero)
                {
                    vec = trailPos[i];
                }
                if (MonoMain.UpdateLerpState) fades[i] -= 0.05f;
                Graphics.Draw(warpFade, vec.x, vec.y, depth - 4);

                if (fades[i] < 0)
                {
                    scals.RemoveAt(i);
                    trailPos.RemoveAt(i);
                    trails.RemoveAt(i);
                    fades.RemoveAt(i);
                }
            }
            position = v;

            float mulfh = 0;
            for (int i = 1; i < flyTrail.Count; i++)
            {
                mulfh += 0.1f;
                Graphics.DrawLine(flyTrail[i - 1], flyTrail[i], mt.color * mulfh, 1.5f, depth + 1);
            }
            mulfh = 0;
            for (int i = 1; i < flyTrail2.Count; i++)
            {
                mulfh += 0.1f;
                Graphics.DrawLine(flyTrail2[i - 1], flyTrail2[i], mt.color * 1.2f * mulfh, 2f, depth + 1);
            }
            if (markAlpha > 0)
            {
                for (int i = 1; i < reMark.Count; i++)
                {
                    Graphics.DrawLine(reMark[i - 1], reMark[i], mt.color * 1.5f * markAlpha, 1, depth + 1);
                }
                for (int i = 1; i < reMark2.Count; i++)
                {
                    Graphics.DrawLine(reMark2[i - 1], reMark2[i], mt.color * 2 * markAlpha, markAlpha * 3, depth + 2);
                }
            }
        }
        public override void Update()
        {
            if (!_triggerHeld)
            {
                bladeScale = Lerp.Vec2Smooth(bladeScale, Vec2.Zero, 0.1f);
            }
            mt.Update();

            if (flying)
            {
                trailPos.Clear();
                trails.Clear();
                fades.Clear();
                scals.Clear();
                _skipPlatforms = true;
                bladeScale = new Vec2(1);
                mt.glow = 3;
                weight = 0;
                markAlpha = 0;

                int mlt = Rando.ChooseInt(1, -1);
                Vec2 v1 = position + travel.Rotate(1.5708f, Vec2.Zero) * Rando.Float(8) * mlt;
                Vec2 v2 = position + travel.Rotate(1.5708f, Vec2.Zero) * Rando.Float(16) * -mlt;
                flyTrail.Add(v1);
                flyTrail2.Add(v2);
                reMark.Add(v1);
                reMark2.Add(v2);

                if (flyTrail.Count > 15)
                {
                    flyTrail.RemoveAt(0);
                    flyTrail2.RemoveAt(0);
                }
                gravMultiplier = 0;
                friction = 0;

                collisionSize = new Vec2(4, 4);
                _collisionOffset = new Vec2(-2, -2);


                if (Level.CheckRay<Block>(position, position + travel * 192) != null)
                {
                    lsScale = Lerp.FloatSmooth(lsScale, 0.5f, 0.4f);
                    LaserDiskParticle lsd = new LaserDiskParticle(x, y, new Color(83, 14, 144))
                    {
                        scale = new Vec2(lsScale),
                        alpha = 1,
                        spd = travel * 15,
                        depth = 1,
                        angle = angle - 1.57f
                    };
                    trailer.Add(lsd);
                    Level.Add(lsd);
                }

                if (isServerForObject)
                {
                    canPickUp = false;
                    _angle = -Maths.PointDirectionRad(Vec2.Zero, travel);
                    _angle += 1.57f;
                    velocity = travel * 24;
                    hMax = 24;
                    vMax = 16;

                    Level.Add(new WarpSlash() { ignore = reGrab, color = new Color(147, 64, 221), scale = new Vec2(1, 1), velocity = travel, alpha = 1, alphMult = 10, killFrames = 3, position = position, angle = angle, offDir = offDir });



                    stuckOnto = Level.CheckRay<Block>(position, position + velocity);
                    if (stuckOnto != null)
                    {
                        markAlpha = 1;
                        canPickUp = true;
                        if (!reGrab.dead)
                        {
                            reGrab.velocity = -travel * 4;
                            reGrab.position = position;
                            reGrab._disarmDisable = 15;
                            reGrab.GiveHoldable(this);
                            reGrab = null;
                            SFX.Play("warpGun", 1, Rando.Float(0.6f, 0.7f));
                        }
                        flying = false;
                        flyTrail.Clear();
                        flyTrail2.Clear();
                        bladeScale = Vec2.Zero;
                    }

                    base.UpdatePhysics();
                }
                return;
            }
            else
            {
                flyTrail.Clear();
                flyTrail2.Clear();
                _skipPlatforms = false;
                weight = 5;
                gravMultiplier = 1;
                friction = 0.1f;
                if (isServerForObject) markAlpha = Lerp.Float(markAlpha, 0, 0.05f);
                if (markAlpha <= 0)
                {
                    reMark.Clear();
                    reMark2.Clear();
                }
                for (int i = 0; i < trailer.Count; i++)
                {
                    trailer[i].scale *= 1.2f;
                    trailer[i].spd *= 0.3f;
                    trailer[i].alpha -= 0.1f;
                }
                lsScale = 1.2f;
                hMax = 12;
                vMax = 8;
                collisionSize = new Vec2(4, 31);
                _collisionOffset = new Vec2(-3, -14);
            }


            glowInc = Lerp.Float(glowInc, 0, 0.01f);
            mt.glow = glow + glowInc;
            mt.color = new Color(83, 14, 144);

            onCool = Lerp.Float(onCool, 0, 0.05f);
            if (!held || owner == null)
            {
                lastUnstance = 0;
                _lerpedAngle = owner != null ? 0f : 90;
                stance = WarpStance.None;

                center = new Vec2(8, 17.5f);
            }
            else
            {
                bool saveTrailPos = false;
                bool InstantWarp = false;


                tearFrames--;

                center = new Vec2(10, 24);
                if (duck.inputProfile.Down(Triggers.Down) && ((duck.offDir > 0 && duck.inputProfile.Down(Triggers.Left)) || (duck.offDir < 0 && duck.inputProfile.Down(Triggers.Right))))
                {
                    stance = WarpStance.Tear;
                }
                else
                {
                    if (stance == WarpStance.Tear || stance == WarpStance.None) stance = WarpStance.Idle;
                }

                if (stance == WarpStance.Tear)
                {
                    glow = Lerp.Float(glow, 2, 0.03f);
                    lastUnstance = 3;
                    _swordAngle = 90;
                    handAngle = Maths.DegToRad(_swordAngle) * offDir;
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(0f, (float)(8 - _swingDif * 0.55f)), 0.4f, 0.1f);
                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2((float)(-8 + _swingDif * 0.35f), 3f), 0.4f, 0.1f);
                    _swordFlip = offDir < 0;
                }
                else if (stance == WarpStance.Idle)
                {
                    glow = Lerp.Float(glow, 0.6f, 0.03f);
                    if (duck._hovering)
                    {
                        lastUnstance = 2;
                        if (onCool <= 0.5f)
                        {
                            if (onCool <= 0)
                            {
                                if (_swordAngle != -90)
                                {
                                    glow = 4;
                                    trailPos.Clear();
                                    trails.Clear();
                                    fades.Clear();
                                    scals.Clear();
                                }

                                InstantWarp = true;

                            }
                            _swordAngle = -90;
                        }
                        _swordFlip = offDir < 0;

                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(0f, (float)(_swingDif * 0.55f - 2)), 0.1f);
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2((float)(_swingDif * 0.35f), -3f), 0.1f);

                        handAngle = Maths.DegToRad(_swordAngle) * offDir;

                    }
                    else
                    {
                        if (lastUnstance != 1)
                        {
                            trailDel = 30;
                            trailPos.Clear();
                            trails.Clear();
                            fades.Clear();
                            scals.Clear();
                        }
                        lastUnstance = 1;
                        _swordAngle = 25f;
                        handAngle = Maths.DegToRad(_swordAngle) * offDir;
                        _holdOffset = new Vec2(0f, (float)(-2 - _swingDif * 0.55f));
                        handOffset = new Vec2((float)(0 + _swingDif * 0.35f), 3f);
                        _swordFlip = offDir < 0;
                    }
                }
                else if (stance == WarpStance.Swung)
                {
                    if (isServerForObject)
                    {
                        if (lastUnstance != 5) swungTimer = Rando.Int(4);
                        swungTimer++;
                        if (swungTimer > 3)
                        {
                            swungTimer = 0;
                            Level.Add(new WarpSlash() { unspeedMult = 3, ignore = duck, color = new Color(147, 64, 221), hSpeed = 4 * offDir, alpha = 0.7f, killFrames = 10, position = position, angle = angle, offDir = offDir });
                        }
                    }
                    lastUnstance = 5;
                    _swordAngle = 120;
                    handAngle = Maths.DegToRad(_swordAngle) * offDir;
                    _holdOffset = new Vec2(_swingDif * -0.5f, (float)(-2 - _swingDif * 0.55f));
                    handOffset = new Vec2((float)(0 + _swingDif * 0.35f), 3f);
                    _swordFlip = offDir < 0;
                    if (_lerpedAngle > 110) stance = WarpStance.Idle;
                }

                if (warpSlash > 0 && isServerForObject)
                {
                    float wsl = 1;
                    SFX.PlaySynchronized("warpgun", 1, warpSlash);
                    for (float i = 0; i < warpSlash; i += 0.1f)
                    {
                        duck.x += 16 * offDir;
                        wsl += 0.15f;
                        Level.Add(new WarpSlash() { ignore = duck, color = new Color(147, 64, 221), scale = new Vec2(1, wsl), hSpeed = i * offDir, alpha = i + 0.3f, killFrames = 3, position = duck.position, angleDegrees = (_lerpedAngle + 90 * i) * offDir, offDir = offDir });
                    }
                    duck.x += 20 * offDir;

                    ReturnItemToWorld(duck);
                    if (Level.CheckPoint<Block>(duck.position) != null) duck.Kill(new DTImpale(this));
                    onCool = 1;
                    InstantWarp = true;
                    _swordAngle = 25f;
                    handAngle = Maths.DegToRad(_swordAngle) * offDir;
                    _holdOffset = new Vec2(0f, (float)(-2 - _swingDif * 0.55f));
                    handOffset = new Vec2((float)(0 + _swingDif * 0.35f), 3f);
                    _swordFlip = offDir < 0;
                    warpSlash = 0;
                }

                if (InstantWarp)
                {
                    float fade = 0.4f;
                    int trail = 5;
                    float last = _lerpedAngle;
                    float scal = 1;
                    while (Math.Abs((_lerpedAngle - _swordAngle)) > 0.6f)
                    {
                        trail++;
                        if (Math.Abs(_lerpedAngle - last) > 1)
                        {
                            last = _lerpedAngle;
                            trailPos.Add(Vec2.Zero);
                            trails.Add(_lerpedAngle);
                            scals.Add(new Vec2(scal));

                            scal += 0.03f;
                            fade += 0.2f;
                            fades.Add(fade);
                            trail = 0;
                        }

                        _lerpedAngle = Lerp.FloatSmooth(_lerpedAngle, _swordAngle, 0.25f + _lerpBoost);
                        _swingDif = Math.Min(Math.Abs(_lerpedAngle - _swordAngle), 35f);
                    }

                    _lerpedAngle = Lerp.FloatSmooth(_lerpedAngle, _swordAngle, 0.25f + _lerpBoost);
                    _swingDif = Math.Min(Math.Abs(_lerpedAngle - _swordAngle), 35f);
                }
                else
                {
                    trailDel++;
                    if (trailDel > 1)
                    {
                        trailPos.Add(saveTrailPos ? position : Vec2.Zero);
                        trails.Add(_lerpedAngle);
                        scals.Add(scale);
                        fades.Add(1.3f);
                        trailDel = 0;
                    }

                    _lerpedAngle = Lerp.FloatSmooth(_lerpedAngle, _swordAngle, 0.25f + _lerpBoost);
                    _swingDif = Math.Min(Math.Abs(_lerpedAngle - _swordAngle), 35f);
                }
            }

            base.Update();
        }
    }
}
