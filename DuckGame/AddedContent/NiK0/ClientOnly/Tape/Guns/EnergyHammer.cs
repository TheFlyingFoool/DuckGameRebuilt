using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class EnergyHammer : Gun
    {
        public StateBinding _handAngleBinding = new StateBinding("handAngle");
        public StateBinding _spinBinding = new StateBinding("spin");
        public StateBinding _stuckBinding = new StateBinding("stuck");
        public StateBinding _dragSpinBinding = new StateBinding("dragSpin");
 
        public override float angle 
        {
            get => base.angle + _hold * offDir;
            set => _angle = value;
        }
        public float _hold;

        public Sprite hammer;
        public MaterialEnergy mt;

        public float glow;
        public float glowInc;

        public bool valid;

        public float holdTime;
        public float swing;
        public bool preMach;

        public Color proper = new Color(178, 220, 239);
        public Color hammerColor;

        public int tilMach;

        public float alphGlow;

        public float addHandAngle;

        public int reversal;
        public bool appliedSwungen;
        public bool machSwing;

        public List<Vec2> trLs = new List<Vec2>();
        public List<float> trANG = new List<float>();
        public List<float> trLF = new List<float>();
        public bool spin;
        public bool dragSpin;

        public float timer;

        public List<Vec2> trailPos = new List<Vec2>();
        public List<float> xscalTrail = new List<float>();
        public Duck dragging;

        public bool stuck;
        public int safeframes;

        public int machTime;

        public LoopingSound _hum;
        public LoopingSound _spinHum;
        public EnergyHammer(float xpos, float ypos) : base(xpos, ypos)
        {
            _hum = new LoopingSound("scimiHum")
            {
                volume = 0f
            };
            _hum.lerpSpeed = 1;

            _spinHum = new LoopingSound("schammerThrow")
            {
                volume = 0f
            };
            _spinHum.lerpSpeed = 1;

            graphic = new Sprite("EnergyHammerHandle");
            hammer = new Sprite("EnergyHammer");

            center = new Vec2(16);
            ammo = 2;
            mt = new MaterialEnergy(this);
            hammerColor = proper;
            glow = 0.2f;
            collisionSize = new Vec2(4, 29);
            _collisionOffset = new Vec2(-2, -16);

            _holdOffset = new Vec2(-5, 0);
            _canRaise = false;

            _barrelOffsetTL = new Vec2(19, 23.5f);
            tapeable = false;

            _editorPreviewRotation = -90;
        }
        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (isServerForObject && dragSpin && with.isBlock)
            {
                dragSpin = false;
                yscale = 1;
                angle = 0;
                dragging = null;
                bouncy = 0;
                SFX.PlaySynchronized("scimisawClash");
                for (int i = 0; i < 13 * DGRSettings.ActualParticleMultiplier; i++)
                {
                    Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-3f, 3f), Rando.Float(-3f, 3f)));
                    spark._color = hammerColor;
                    spark._width = 1f;
                    Level.Add(spark);
                }
            }
            if (isServerForObject && spin && with.isBlock)
            {
                SFX.PlaySynchronized("schammerHitsAWall");
                stuck = true;
                spin = false;

                for (int i = 0; i < 13 * DGRSettings.ActualParticleMultiplier; i++)
                {
                    Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-3f, 3f), Rando.Float(-3f, 3f)));
                    spark._color = hammerColor;
                    spark._width = 1f;
                    Level.Add(spark);
                }
            }
            else
            {

                alphGlow = 1;
                glowInc = 0.5f;
                SFX.PlaySynchronized("scimisawClash");

                for (int i = 0; i < 13 * DGRSettings.ActualParticleMultiplier; i++)
                {
                    Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-2f, 2f), Rando.Float(-2f, 2f)));
                    spark._color = hammerColor;
                    spark._width = 1f;
                    Level.Add(spark);
                }
            }

            base.OnSolidImpact(with, from);
        }
        public override void OnPressAction()
        {
            if (duck != null)
            {
                valid = Math.Abs(duck.hSpeed) > 0.1f && Math.Abs(addHandAngle) < 0.3f;
                if (valid)
                {
                    preMach = machTime == 0;
                    holdTime = 0.2f;
                }
                else if (Math.Abs(addHandAngle) < 0.2f)
                {
                    swing = 0.3f;
                }
            }
            else valid = false;
        }
        public override void OnHoldAction()
        {
            if (valid)
            {
                valid = (Math.Abs(duck.hSpeed) > 0.1f && Math.Abs(addHandAngle) < 0.3f) || machTime > 0;
                holdTime += 0.005f;
                if (holdTime > 0.5f) holdTime = 0.5f;
            }
            else holdTime = 0;
        }
        public override void OnReleaseAction()
        {
            if (valid)
            {
                if (machTime > 0 && (holdTime > 0.3f || preMach))
                {
                    swing = holdTime * 2 + 1;
                }
                else
                {
                    swing = holdTime * 2;
                }
                holdTime = 0;
            }
        }
        public override void Fire()
        {
        }
        public override void Draw()//quadchaingun
        {
            base.Draw();

            if (trailPos.Count > 0)
            {
                Vec2 ps = position;
                float ysc = yscale;
                float alph = alpha;
                Depth d = depth;
                Vec2 scal = scale;
                depth -= 1;
                alpha = 0.02f;
                for (int i = 0; i < trailPos.Count; i++)
                {
                    Vec2 v = trailPos[i];
                    float yscal = xscalTrail[i];

                    position = v;
                    //scale = scal * (1 + i / 12f);
                    yscale = yscal;
                    alpha *= 2;
                    depth -= i;
                    base.Draw();
                }
                scale = scal;
                depth = d;
                alpha = alph;
                position = ps;
                yscale = ysc;
            }

            Graphics.material = mt;

            if (trailPos.Count > 0)
            {
                mt.glow = 0.5f;
                float alph = alpha;
                alpha = 0.02f;
                for (int i = 0; i < trailPos.Count; i++)
                {
                    Vec2 v = trailPos[i];
                    float yscal = xscalTrail[i];

                    alpha *= 2;
                    hammer.alpha = alpha;
                    hammer.scale = new Vec2(xscale, yscal);
                    //hammer.scale = scale * (1 + i / 12f);
                    hammer.center = center;
                    hammer.flipH = offDir < 0;
                    Graphics.Draw(hammer, v.x, v.y, depth - i - 1);
                }
                alpha = alph;
            }

            mt.color = hammerColor;
            mt.glow = glow + glowInc;
            hammer.position = position;
            hammer.alpha = alpha;
            hammer.angle = angle;
            hammer.depth = depth;
            hammer.scale = scale;
            hammer.center = center;
            hammer.flipH = offDir < 0;
            Graphics.Draw(hammer, x, y, depth);

            for (int i = 0; i < trLs.Count; i++)
            {
                Vec2 v = trLs[i];
                float ang = trANG[i];
                float fade = trLF[i];

                if (MonoMain.UpdateLerpState)
                {
                    fade -= 0.05f;
                    trLF[i] = fade;
                }

                mt.color = Color.White * fade;
                hammer.position = v;
                hammer.alpha = fade;
                hammer.angle = ang;
                hammer.depth = depth;
                hammer.scale = scale * 1.1f;
                hammer.center = center;
                hammer.flipH = offDir < 0;
                Graphics.Draw(hammer, v.x, v.y, depth);
                if (fade <= 0)
                {
                    trLs.RemoveAt(i);
                    trANG.RemoveAt(i);
                    trLF.RemoveAt(i);
                }
            }

            if (alphGlow > 0.05f || holdTime > 0)
            {
                mt.color = hammerColor;
                hammer.xscale *= 1 + alphGlow + holdTime / 2;
                hammer.yscale *= 1.5f + alphGlow + holdTime / 2;
                hammer.alpha = alphGlow + holdTime / 2;
                hammer.center = new Vec2(16, 25.5f);
                Vec2 vec = Offset(new Vec2(0, 10));
                Graphics.Draw(hammer, vec.x, vec.y, depth - 2);
            }
            Graphics.material = mt;
        }
        
        public override void Thrown()
        {
            base.Thrown();
            if (!isServerForObject || duck == null || duck.destroyed) return;
            if (!duck.inputProfile.Down(Triggers.Grab)) return;
            if (duck.inputProfile.Down(Triggers.Left) && duck.offDir < 0 || duck.inputProfile.Down(Triggers.Right) && duck.offDir > 0)
            {
                if (machTime > 0)
                {
                    dragSpin = true;
                    dragging = duck;
                }
                else
                {
                    safeframes = 10;
                    spin = true;
                }
            }
        }
        public override void Update()
        {
            glowInc = Lerp.Float(glowInc, 0, 0.01f);
            if (_spinHum != null)
            {
                _spinHum.Update();
                if (spin) _spinHum.volume = 1;
                else _spinHum.volume = Lerp.Float(_spinHum.volume, 0, 0.1f);
            }
            if (_hum != null)
            {
                _hum.Update();
                if (dragSpin) _hum.volume = 1;
                else _hum.volume = Lerp.Float(_hum.volume, 0, 0.1f);
            }

            if (x > Level.current.ExtendedRight || x < Level.current.ExtendedLeft)
            {
                if (spin)
                {
                    offDir *= -1;
                }
                else Level.Remove(this);
            }
            if (stuck)
            {
                alphGlow = 0;
                if (owner != null)
                {
                    stuck = false;
                    spin = false;
                }
                return;
            }
            if (spin)
            {
                if (safeframes > 0) safeframes--;
                _hold = 0;
                angleDegrees += 30 * offDir;
                hSpeed = offDir * 11;
                vSpeed = 0;
                trLs.Add(position);
                trANG.Add(angle);
                trLF.Add(0.6f);
                collisionSize = new Vec2(10);
                _collisionOffset = new Vec2(-5);
                center = new Vec2(16);
                alphGlow += 0.01f;
                if (alphGlow > 1) alphGlow = 1;

                if (isServerForObject)
                {
                    base.UpdatePhysics();
                    foreach (MaterialThing mt in Level.CheckCircleAll<MaterialThing>(position, 9))
                    {
                        if (Duck.GetAssociatedDuck(mt) == lastThrownBy && safeframes > 0) continue;
                        Fondle(mt);
                        mt.Hurt(10);
                        mt.hSpeed = hSpeed;
                        mt.Destroy(new DTImpact(this));
                    }
                }
                return;
            }
            if (dragSpin)
            {
                if (isServerForObject)
                {
                    friction = 0;
                    gravMultiplier = 0;
                    hSpeed = offDir * 10;
                    vSpeed = 0;
                    _hold = 0;
                    angle = 1.57f;
                    if (dragging != null)
                    {
                        if (dragging.ragdoll == null) dragging.GoRagdoll();

                        if (dragging.ragdoll != null)
                        {
                            Vec2 p = position + new Vec2(14 * yscale, -6);
                            dragging.ragdoll.part2.position = p;
                        }
                    }

                    foreach (MaterialThing mt in Level.CheckCircleAll<MaterialThing>(position, 12))
                    {
                        if (Duck.GetAssociatedDuck(mt) == dragging) continue;
                        Fondle(mt);
                        mt.hSpeed = hSpeed;
                        mt.Destroy(new DTImpact(this));
                    }
                }

                timer += 0.21f;
                yscale = (float)Math.Sin(timer);


                collisionSize = new Vec2(8, 4);
                _collisionOffset = new Vec2(-4, -2);

                trailPos.Add(position);
                xscalTrail.Add(yscale);
                if (trailPos.Count > 6)
                {
                    trailPos.RemoveAt(0);
                    xscalTrail.RemoveAt(0);
                }
                base.UpdatePhysics();
                return;
            }
            else
            {
                _hold = 0;
                yscale = 1;
                friction = 0.1f;
                gravMultiplier = 1;
                collisionSize = new Vec2(4, 29);
                _collisionOffset = new Vec2(-2, -16);
                if (trailPos.Count > 0)
                {
                    trailPos.RemoveAt(0);
                    xscalTrail.RemoveAt(0);
                }
            }
            mt.Update();
            if (owner != null || spin) center = new Vec2(16, 0);
            else center = new Vec2(16);

            if (dragSpin) weight = 2;
            else if (owner != null)
            {
                weight = 5;
                if (duck != null) duck._disarmDisable = 10;
            }
            else weight = 10;

            if (swing > 0)
            {
                if (swing <= 1)
                {
                    machSwing = false;
                    addHandAngle = Lerp.FloatSmooth(addHandAngle, -2.5f, 0.6f * swing, 0.5f);
                    if (addHandAngle < -1 && !appliedSwungen)
                    {
                        appliedSwungen = true;
                        if (duck != null)
                        {
                            duck.hSpeed += offDir * swing * 8 + offDir * 2;
                            duck.vSpeed -= 4 * swing + 2;
                            Level.Add(new EnergyForceWave(x + offDir * 4f + owner.hSpeed, y + 14f, offDir, 0.08f, 5f + Math.Abs(owner.hSpeed), owner.vSpeed, duck));
                        }
                    }
                    if (addHandAngle == -2.5f)
                    {
                        reversal = 6 + (int)(swing * 5);
                        swing = 0;
                    }
                }
                else
                {
                    SFX.PlaySynchronized("schammerDashSwing");
                    machSwing = true;
                    addHandAngle = -2.5f;
                    if (duck != null)
                    {
                        Level.Add(new EnergyForceWave(x + offDir * 12, y + 8f, offDir, 0.04f, 7 + Math.Abs(owner.hSpeed), owner.vSpeed, duck));
                        duck.hSpeed += offDir * swing * 16;
                        duck.vSpeed -= 5 * swing;
                    }
                    reversal = 12;
                    swing = 0;
                }
            }
            else
            {
                if (reversal > 0 && reversal % 2 == 0 && machSwing)
                {
                    if (isServerForObject && reversal % 4 == 0 && duck != null)
                    {
                        SFX.PlaySynchronized("schammerDashSwing", 1, Rando.Float(-0.1f, 0.1f));
                        Level.Add(new EnergyForceWave(x + offDir * 12, y + 8f, offDir, 0.02f, 4 + Math.Abs(owner.hSpeed), owner.vSpeed * 0.3f, duck));
                    }
                    trLs.Add(position);
                    trANG.Add(angle);
                    trLF.Add(1);
                    if (isServerForObject) addHandAngle -= reversal / 40f;
                }
                appliedSwungen = false;
                if (isServerForObject)
                {
                    reversal--;
                    if (reversal <= 0) addHandAngle = Lerp.FloatSmooth(addHandAngle, 0, 0.1f);
                    else
                    {
                        addHandAngle -= reversal / 50f;
                    }
                }
            }
            if (isServerForObject)
            {
                handAngle = 1.3f + addHandAngle;
                if (owner != null && owner.offDir < 0) handAngle = -handAngle;
            }

            glow = 0.2f;
            handOffset = new Vec2(0, -1);
            alphGlow = Lerp.FloatSmooth(alphGlow, 0, 0.1f);
            if (duck != null)
            {
                duck.hAccMulti = 0.7f;
                if ((duck.offDir < 0 && duck.hSpeed < 0) || (duck.offDir > 0 && duck.hSpeed > 0))
                {
                    if (Rando.Float(10) / Math.Abs(duck.hSpeed) < DGRSettings.ActualParticleMultiplier && duck.grounded)
                    {
                        Spark spark = Spark.New(barrelPosition.x, barrelPosition.y - 6f, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                        spark._color = hammerColor;
                        spark._width = 1f;
                        Level.Add(spark);
                    }
                    if (Math.Abs(duck.hSpeed) > 2.6f && (duck.grounded || Math.Abs(addHandAngle) > 0.1f))
                    {

                        tilMach++;
                        if (tilMach > 30 || machTime > 0)
                        {
                            if (machTime == 0)
                            {
                                machTime = 15;
                                SFX.Play("schammerDashStart", 1, Rando.Float(0.1f));
                            }
                            if (machTime % 5 == 0 && machTime < 30 && tilMach > 30)
                            {
                                Level.Add(new LaserDiskParticle(duck.x, duck.y + 3, hammerColor)
                                {
                                    scale = new Vec2(machTime / 20f + 0.1f),
                                    alpha = 0.7f,
                                    spd = new Vec2(duck.offDir, duck.vSpeed * 0.3f),
                                    depth = 1
                                });
                            }
                            machTime++;
                            if (machTime > 45) machTime = 45;
                            duck.hAccMulti = 1;
                            duck.runmaxMulti = 2;
                        }
                    }
                    else
                    {
                        if (machTime > 0)
                        {
                            machTime--;
                            if (duck.vSpeed > -0.1f)
                            {
                                duck.hAccMulti = 1.2f;
                                duck.runmaxMulti = 2.5f;
                            }
                            else machTime = 0;
                        }
                        tilMach = 0;
                    }

                    if (duck.grounded || Math.Abs(addHandAngle) > 0.1f)
                    {
                        float num = duck.hSpeed - Math.Sign(duck.hSpeed);
                        if (Math.Sign(num) != Math.Sign(duck.hSpeed))
                            num = 0f;
                        if (duck.crouch) num = 0;
                        num = Maths.Clamp(num, -5, 5);
                        duck.tilt = num;
                        duck.verticalOffset = Math.Abs(num);
                    }
                }
                else
                {
                    tilMach = 0;
                    if (machTime > 0) machTime--;
                }
            }
            else
            {
                tilMach = 0;
                machTime = 0;
            }
            base.Update();
        }
    }
}