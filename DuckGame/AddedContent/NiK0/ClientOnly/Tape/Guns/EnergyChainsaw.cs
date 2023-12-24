using System;

namespace DuckGame
{
    [ClientOnly]
    public class EnergyChainsaw : Gun
    {
        public SpriteMap sprite;
        public SpriteMap blade;

        public StateBinding _holdBinding = new StateBinding("_hold");
        public StateBinding _spinningBinding = new StateBinding("Spinning");
        private float _hold;
        public override float angle
        {
            get => base.angle + _hold * offDir;
            set => _angle = value;
        }
        public SpriteMap spin;
        public EnergyChainsaw(float xpos, float ypos) : base(xpos, ypos)
        {
            _idleWave = new SinWave(this, 0.6f);
            tapeable = false;

            ammo = 10;
            _ammoType = new ATG18();

            sprite = new SpriteMap("energychainsaw", 34, 14);

            blade = new SpriteMap("energychainsawblade", 34, 14);
            spin = new SpriteMap("energyChainsawSpin", 41, 41);
            spin.AddAnimation("spin", 0.6f, true, 0, 1, 2);
            spin.SetAnimation("spin");
            spin.center = new Vec2(20.5f);
            spin.canMultiframeSkip = true;

            collisionSize = new Vec2(10);
            _collisionOffset = new Vec2(-9, -2);

            _holdOffset = new Vec2(-4, 1.5f);

            graphic = sprite;
            mt = new MaterialEnergyChainsaw(this);
            bladeColor = properColor;
            _barrelOffsetTL = new Vec2(34, 8);

            _editorPreviewOffset.y -= 6;
            _editorPreviewOffset.x -= 16;
            _sound = new LoopingSound("scimisawHum");
            tapeable = false;
        }

        public bool Spinning;
        private LoopingSound _sound;
        public Color properBladeColor = Color.White;
        public Color properColor = new Color(178, 220, 239);
        public Color bladeColor;
        public MaterialEnergyChainsaw mt;
        private SinWave _idleWave;

        public float animTime;
        public float time;
        private bool _playedChargeUp;
        public int surgeDel;

        public float glowInc;
        public override void Update()
        {
            surgeDel--;
            time += 0.1f;
            _sound.Update();

            mt._time += (glow - 0.25f) / 12;
            if (owner != null)
            {
                if (_triggerHeld)
                {
                    glow = Lerp.Float(glow, 0.5f, 0.01f);
                    _sound.volume = Lerp.Float(_sound.volume, 1, 0.02f);


                    Block block = Level.CheckLine<Block>(Offset(new Vec2(7, 0)), Offset(new Vec2(27, 0)));

                    if (block != null)
                    {
                        if (isServerForObject)
                        {
                            Vec2 reVec = Maths.AngleToVec(Maths.PointDirectionRad(owner.position, block.position)) * new Vec2(-1, 1);
                            reVec.y -= 0.5f;
                            reVec.x *= 1.3f;
                            owner.velocity = reVec * 10;
                        }
                        SFX.Play("scimisawClash");
                        glowInc = 2.5f;

                        Vec2 vec2 = position + barrelVector * 5f;
                        for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 14; ++index)
                        {
                            Spark spark = Spark.New(vec2.x, vec2.y, new Vec2(offDir * Rando.Float(0f, 3f), Rando.Float(-1.5f, 1.5f)));
                            spark._color = bladeColor;
                            spark._width = 1f;
                            Level.Add(spark);
                        }
                    }
                    if (isServerForObject)
                    {

                        bool playsurge = false;
                        foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(Offset(new Vec2(7, 0)), Offset(new Vec2(27, 0))))
                        {
                            if (materialThing == owner) continue;
                            Fondle(materialThing);
                            if (owner.velocity.length > 0 && materialThing is PhysicsObject && materialThing.owner == null)
                            {
                                materialThing.velocity += owner.velocity;
                                owner.hSpeed *= 0.8f;
                                if (owner.hSpeed < 0) owner.hSpeed *= 0.8f;


                                playsurge = true;
                            }
                            if (materialThing is EnergyScimitar scimi && scimi.velocity.length > 6)
                            {
                                scimi.StartFlying(Maths.PointDirection(Vec2.Zero, scimi.velocity), true);
                                scimi.TravelThroughAir(1);
                            }
                            materialThing.Destroy(new DTIncinerate(this));
                        }
                        if (playsurge && surgeDel <= 0)
                        {
                            surgeDel = 10;
                            SFX.Play("scimiSurge", 1, Rando.Float(0.2f));
                        }

                        if (duck != null)
                        {
                            duck._disarmDisable = 5;
                            if (duck.crouch)
                            {
                                _hold = Lerp.FloatSmooth(_hold, 0, 0.3f);
                                Thing best = null;
                                foreach (IPlatform ipl in Level.CheckLineAll<IPlatform>(Offset(new Vec2(7, 4)), Offset(new Vec2(23, 4))))
                                {
                                    best = (MaterialThing)ipl;
                                    if (ipl is Block b)
                                    {
                                        best = b;
                                        break;
                                    }
                                }

                                if (best != null && duck.grounded)
                                {
                                    float hs;
                                    if (best is Block)
                                    {
                                        hs = 10;
                                    }
                                    else hs = 5;

                                    Vec2 vec2 = position + barrelVector * 5f;
                                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                                    {
                                        Spark spark = Spark.New(vec2.x, vec2.y, new Vec2(offDir * Rando.Float(0f, 3f), Rando.Float(0.5f, 1.5f)));
                                        spark._color = bladeColor;
                                        spark._width = 1f;
                                        Level.Add(spark);
                                    }

                                    duck.hSpeed = hs * offDir;
                                }
                            }
                            else _hold = Lerp.FloatSmooth(_hold, -0.6f, 0.1f) + (float)Math.Sin(time * 4) / 30;
                        }
                        else _hold = Lerp.FloatSmooth(_hold, -0.6f, 0.1f) + (float)Math.Sin(time * 4) / 30;
                    }


                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(2f, 0f + _idleWave.normalized), 0.23f);
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(-3f, 2.5f + _idleWave.normalized), 0.23f);
                }
                else
                {

                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(-4f, 3.5f + _idleWave * 0.5f), 0.23f);
                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(0f, 2f + _idleWave * 0.5f), 0.23f);
                    glow = Lerp.Float(glow, 0.25f, 0.01f);
                    _sound.volume = Lerp.Float(_sound.volume, 0, 0.03f);
                    _hold = Lerp.FloatSmooth(_hold, -0.2f, 0.1f);
                }
                if (!_playedChargeUp)
                {
                    _playedChargeUp = true;
                    SFX.Play("laserChargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                center = new Vec2(8, 7);
            }
            else
            {
                glow = Lerp.Float(glow, 0.25f, 0.01f);
                _sound.volume = Lerp.Float(_sound.volume, 0, 0.03f);
                if (_playedChargeUp)
                {
                    _playedChargeUp = false;
                    SFX.Play("laserUnchargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                _hold = 0;
                if (grounded)
                {
                    y -= (float)Math.Sin(time) / 5;
                    ReturnItemToWorld(this);
                }
                center = new Vec2(17, 7);
            }
            if (Spinning)
            {
                del++;
                if (del > 1)
                {
                    del = 0;
                    sparkPart += 16;
                    float part = sparkPart;
                    int iters = (int)Math.Ceiling(2f * DGRSettings.ActualParticleMultiplier);
                    float add = 360f / iters;
                    for (int i = 0; i < iters; i++)
                    {
                        Vec2 vec2 = position + Maths.AngleToVec(Maths.DegToRad(part)) * 16;
                        Vec2 speed = Maths.AngleToVec(Maths.DegToRad(part + 90)) * 2;
                        Spark spark = Spark.New(vec2.x, vec2.y, speed, 0.05f);
                        spark._color = bladeColor;
                        spark._width = 1f;
                        Level.Add(spark);
                        part += add;
                    }
                }

                UpdateSpin();
            }
            else
            {
                colMult = 0;
                collisionSize = new Vec2(10);
                _collisionOffset = new Vec2(-9, -2);
                spinFade = Lerp.Float(spinFade, 0, 0.015f);
            }
            glowInc = Lerp.FloatSmooth(glowInc, 0, 0.1f);
            base.Update();
        }
        public int del;
        public override void OnPressAction()
        {
        }
        public override void OnHoldAction()
        {
        }
        public override void OnReleaseAction()
        {
        }
        public float sparkPart;
        public Vec2 assignSpeed;
        public override void Thrown()
        {
            if (duck == null) return;
            x = duck.x;
            safeTime = 10;
            _oldDepth = depth = -0.1f;
            if (!isServerForObject || duck == null || duck.destroyed) return;
            if (!duck.inputProfile.Down(Triggers.Grab)) return;
            if (duck.inputProfile.Down(Triggers.Left) && duck.offDir < 0 || duck.inputProfile.Down(Triggers.Right) && duck.offDir > 0)
            {
                assignSpeed = barrelVector;

                boomeraming = false;

                spinPower = 50;
                Spinning = true;
            }
        }
        public override bool PlayCollideSound(ImpactedFrom from)
        {
            return base.PlayCollideSound(from);
        }
        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Spinning)
            {
                boomeraming = false;
                spinPower -= 5;
                SFX.PlaySynchronized("scimisawClash", 1, Rando.Float(0.1f));
                return;
            }
            base.OnSolidImpact(with, from);
        }
        public float faded;
        public float spinFade;

        public StateBinding _spinPowerBinding = new StateBinding("spinPower");
        public StateBinding _fadedBinding = new StateBinding("faded");
        public override bool Sprung(Thing pSpringer)
        {
            boomeraming = false;
            spinPower = 50;
            return base.Sprung(pSpringer);
        }

        public bool boomeraming;
        public Vec2 larp;

        public float pull;
        public void UpdateSpin()
        {
            if (assignSpeed != Vec2.Zero)
            {
                velocity = assignSpeed * 9;
                assignSpeed = Vec2.Zero;
            }
            if (spinPower <= 0 || owner != null)
            {
                if (isServerForObject)
                {
                    if (owner != null && faded > 1) faded = 1;
                    if (faded < 1f)
                    {
                        gravMultiplier = 1;
                        bouncy = 0.5f;
                        friction = 0.1f;
                        if (faded < 0.5f)
                        {
                            spinFade = 0.9f;
                            Spinning = false;
                        }
                    }
                    else
                    {
                        velocity = Lerp.Vec2(velocity, Vec2.Zero, 0.2f);
                    }
                }
                glowInc = Lerp.Float(glowInc, 0, 0.01f);
                faded = Lerp.Float(faded, 0, 0.2f);
                if (isServerForObject) angleDegrees += faded;
                return;
            }
            else faded = 3;
            glowInc = Rando.Float(0.2f, 0.3f) * (spinPower/50f);
            gravMultiplier = 0;
            friction = 0;
            bouncy = 1.1f;

            collisionSize = new Vec2(26) * colMult;
            _collisionOffset = new Vec2(-13) * colMult;

            colMult = Lerp.Float(colMult, 1, 0.1f);

            if (isServerForObject)
            {
                if (Level.CheckLine<Block>(position, position + velocity * 480) == null && (x < Level.current.topLeft.x || x > Level.current.bottomRight.x || y < Level.current.topLeft.y || y > Level.current.bottomRight.y))
                {
                    larp = Maths.AngleToVec(Maths.PointDirectionRad(position, (Level.current.topLeft + Level.current.bottomRight) / 2f)) * 9;
                    velocity = Maths.AngleToVec(Lerp.RadAngleLerp(Maths.PointDirectionRad(Vec2.Zero, velocity), Maths.PointDirectionRad(position, (Level.current.topLeft + Level.current.bottomRight) / 2f), 0.05f)) * 9;
                    boomeraming = true;
                }
                else if (boomeraming)
                {
                    pull = 0;
                    velocity = Lerp.Vec2(velocity, larp, 0.4f);
                    if (velocity == larp) boomeraming = false;
                }
                else pull = 0;

                angleDegrees += spinPower;

                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 17))
                {
                    if (materialThing == lastThrownBy && safeTime > 0) continue;
                    Fondle(materialThing);
                    if (materialThing is EnergyScimitar scimi && scimi.velocity.length > 6)
                    {
                        scimi.StartFlying(Maths.PointDirection(Vec2.Zero, velocity), true);
                        scimi.TravelThroughAir(1);
                    }
                    materialThing.Destroy(new DTIncinerate(this));
                }
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position + velocity * 2, 16))
                {
                    if (Level.CheckLine<Block>(position, materialThing.position) != null) continue;
                    if (materialThing == lastThrownBy && safeTime > 0) continue;
                    Fondle(materialThing);
                    if (materialThing is EnergyScimitar scimi && scimi.velocity.length > 6)
                    {
                        scimi.StartFlying(Maths.PointDirection(Vec2.Zero, velocity), true);
                        scimi.TravelThroughAir(1);
                    }
                    materialThing.Destroy(new DTIncinerate(this));
                }
                safeTime--;

                //mts = Level.CheckCircleAll<MaterialThing>(position + velocity * 1.5f, 16);
            }

        }
        public float colMult;
        public int safeTime;
        public float spinPower;
        public float glow = 0.2f;
        public override void Draw()
        {
            if (Spinning)
            {
                if (DevConsole.showCollision)
                {
                    Graphics.DrawCircle(position, 17, Color.Red, 2, 1);
                    Graphics.DrawCircle(position + velocity * 1.5f, 16, Color.DarkRed, 2, 1);

                    Graphics.DrawLine(position, position + velocity.normalized * 480, Color.White, 3, depth - 1);
                }

                Graphics.material = mt;
                mt.glow = glow + glowInc;

                spin.position = position;
                spin.alpha = alpha;
                spin.depth = depth + 1;
                spin.scale = scale;
                spin.angle = angle;
                spin.flipH = offDir < 0;
                spin.speed = 1;
                Graphics.Draw(spin, x, y, depth);

                if (glowInc > 0.1f)
                {
                    spin.scale += new Vec2(glowInc * 2, glowInc * 2);
                    spin.alpha = 0.2f;
                    Graphics.Draw(spin, x, y, depth);


                }

                Graphics.material = null;
            }
            else
            {
                if (DevConsole.showCollision)
                {
                    Graphics.DrawLine(Offset(new Vec2(7, 4)), Offset(new Vec2(23, 4)), Color.Blue, 1, 1); //block slide check
                    Graphics.DrawLine(Offset(new Vec2(7, 0)), Offset(new Vec2(27, 0)), Color.Red, 1, 1); //kill box check and block fling check
                }
                animTime += _sound.volume / 4f;
                if (animTime > 0.5f)
                {
                    animTime = 0;
                    sprite.imageIndex++;
                    if (sprite.imageIndex > 1)
                    {
                        sprite.imageIndex = 0;
                    }
                }

                Graphics.material = mt;
                mt.glow = glow + glowInc;
                blade.imageIndex = sprite.imageIndex;
                blade.position = position;
                blade.alpha = alpha;
                blade.angle = angle;
                blade.depth = depth;
                blade.scale = scale;
                blade.center = center;
                blade.flipH = offDir < 0;
                Graphics.Draw(blade, x, y, depth);

                if (spinFade > 0)
                {
                    spin.speed = spinFade;
                    spin.scale = new Vec2(Math.Abs(spinFade - 2));
                    spin.position = position;
                    spin.alpha = spinFade - 0.2f;
                    spin.angle = angle;
                    spin.flipH = offDir < 0;
                    spin.speed = 1;
                    Graphics.Draw(spin, x, y, depth + 3);

                }

                Graphics.material = null;
                if (glowInc > 0.1f)
                {
                    blade.scale += new Vec2(glowInc / 3, glowInc * 2);
                    blade.alpha = 0.3f;
                    Graphics.Draw(blade, x, y, depth);

                }

                base.Draw();
            }
        }
    }
}