using System;

namespace DuckGame
{
    [ClientOnly]
    public class EnergyChainsaw : Gun
    {
        public SpriteMap sprite;
        public SpriteMap blade;

        public StateBinding _holdBinding = new StateBinding("_hold");
        private float _hold;
        public override float angle
        {
            get => base.angle + _hold * offDir;
            set => _angle = value;
        }
        public EnergyChainsaw(float xpos, float ypos) : base(xpos, ypos)
        {
            tapeable = false;

            ammo = 10;
            _ammoType = new ATG18();

            sprite = new SpriteMap("energychainsaw", 34, 14);

            blade = new SpriteMap("energychainsawblade", 34, 14);

            collisionSize = new Vec2(10);
            _collisionOffset = new Vec2(-9, -2);

            _holdOffset = new Vec2(-4, 1.5f);

            graphic = sprite;
            mt = new MaterialEnergyChainsaw(this);
            bladeColor = properColor;
            _barrelOffsetTL = new Vec2(34, 8);

            _sound = new LoopingSound("scimisawHum");
        }
        private LoopingSound _sound;
        public Color properBladeColor = Color.White;
        public Color properColor = new Color(178, 220, 239);
        public Color bladeColor;
        public MaterialEnergyChainsaw mt;
        private SinWave _idleWave = (SinWave)0.6f;

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
                            else
                            {
                                _hold = Lerp.FloatSmooth(_hold, -0.6f, 0.1f) + (float)Math.Sin(time * 4) / 30;
                            }
                        }
                        else
                        {
                            _hold = Lerp.FloatSmooth(_hold, -0.6f, 0.1f) + (float)Math.Sin(time * 4) / 30;
                        }
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
            base.Update();
        }
        public override void OnPressAction()
        {
        }
        public override void OnHoldAction()
        {
        }
        public override void OnReleaseAction()
        {
        }
        public float glow = 0.2f;
        public override void Draw()
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
            glowInc = Lerp.FloatSmooth(glowInc, 0, 0.1f);
            blade.imageIndex = sprite.imageIndex;
            blade.position = position;
            blade.alpha = alpha;
            blade.angle = angle;
            blade.depth = depth;
            blade.scale = scale;
            blade.center = center;
            blade.flipH = offDir < 0;
            Graphics.Draw(blade, x, y, depth);

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