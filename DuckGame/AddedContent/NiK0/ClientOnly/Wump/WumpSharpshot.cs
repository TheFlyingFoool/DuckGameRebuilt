using System;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Rifles")]
    public class WumpSharpshot : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        public int _loadState = -1;
        public int _loadAnimation = -1;
        public float _angleOffset;

        public WumpSharpshot(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 3;
            _ammoType = new ATDeathCaliber();
            _type = "gun";
            graphic = new Sprite("wumpsharpshot");
            center = new Vec2(16f, 7.5f);
            collisionOffset = new Vec2(-8f, -5f);
            collisionSize = new Vec2(20f, 9f);
            _holdOffset = new Vec2(1f, 0f);
            _barrelOffsetTL = new Vec2(40, 5.25f);
            _fireSound = "sniper";
            _fireSoundPitch = -0.4f;
            _kickForce = 10f;
            _fireRumble = RumbleIntensity.Heavy;
            laserSight = true;
            _laserOffsetTL = new Vec2(41, 5.25f);
            _manualLoad = true;
            editorTooltip = "Like a sharpshot, but way more dangerous and explosive.";
        }

        public float shake;
        public override void Update()
        {
            base.Update();
            if (_loadState > -1)
            {
                if (owner == null)
                {
                    if (_loadState == 3)
                        loaded = true;
                    _loadState = -1;
                    _angleOffset = 0f;
                    handOffset = Vec2.Zero;
                }
                if (_loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            NetSoundEffect.Play("sniperLoad");
                    }
                    else
                        SFX.Play("loadSniper");
                    ++_loadState;
                }
                else if (_loadState == 1)
                {
                    if (_angleOffset < 0.16f)
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0.25f, 0.25f);
                    else
                        ++_loadState;
                }
                else if (_loadState == 2)
                {
                    handOffset.x += 0.8f;
                    if (handOffset.x > 4f)
                    {
                        ++_loadState;
                        Reload();
                        loaded = false;
                    }
                }
                else if (_loadState == 3)
                {
                    handOffset.x -= 0.8f;
                    if (handOffset.x <= 0f)
                    {
                        ++_loadState;
                        handOffset.x = 0f;
                    }
                }
                else if (_loadState == 4)
                {
                    if (_angleOffset > 0.04f)
                    {
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.25f);
                    }
                    else
                    {
                        _loadState = -1;
                        loaded = true;
                        _angleOffset = 0f;
                    }
                }
            }
            if (loaded && owner != null && _loadState == -1)
                laserSight = true;
            else
                laserSight = false;
        }

        public override void DrawGlow()
        {
            if (laserSight && held && _laserTex != null)
            {
                fUp += muli;
                other += Rando.Float(-0.5f, 0.5f);
                float num = 1f;
                if (!Options.Data.fireGlow)
                {
                    num = 0.4f;
                }
                Vec2 vec = barrelPosition;
                Vec2 vec2 = Extensions.stopPoint(barrelPosition, ammoType, Maths.PointDirection(Vec2.Zero, barrelVector));
                Vec2 normalized = (vec2 - vec).normalized;
                float f = 0.5f;

                Color c = Color.Red;

                float length = (vec - vec2).length;

                c *= num;

                Graphics.DrawTexturedLine(_laserTex, vec, vec2, c, f, depth - 1);
                
                if (length < _ammoType.range)
                {
                    _sightHit.color = Color.Red * alpha;
                    Graphics.Draw(_sightHit, vec2.x, vec2.y);

                    /*for (float z = 0; z < 12f + Math.Cos(other) * 6; z += r)
                    {
                        Vec2 reNorm = -normalized.Rotate((float)Math.Sin(fUp + z), Vec2.Zero);
                        Vec2 vec3 = vec2 + reNorm * 8;


                        for (int i = 1; i < 4; i++)
                        {
                            Graphics.DrawTexturedLine(_laserTex, vec3, vec3 + reNorm * ((float)Math.Cos((float)i + (float)z + (float)Math.Sin(fUp - other)) * 9), c * (1f - i * 0.2f), f, depth - 1);
                            vec3 += reNorm * ((float)Math.Cos(fUp * normalized.x * (Math.Sin(other * z / 12) + 1)) * 3);
                        }
                    }*/
                }
                for (float z = 0; z < 9 + Math.Cos(other) * 3; z += r)
                {
                    Vec2 reNorm = barrelVector.Rotate((float)Math.Sin(fUp + z), Vec2.Zero);
                    Vec2 vec3 = barrelPosition + reNorm * 8;


                    for (int i = 1; i < 4; i++)
                    {
                        Graphics.DrawTexturedLine(_laserTex, vec3, vec3 + reNorm * ((float)Math.Cos(i + (float)z + (float)Math.Sin(fUp - other)) * 15), c * (1f - i * 0.2f), f, depth - 1);
                        vec3 += reNorm * ((float)Math.Cos(fUp * normalized.x * (Math.Sin(other * z / 12) + 1)) * 3);
                    }
                }
            }
        }
        public override void Initialize()
        {
            muli = Rando.Float(0.05f, 0.1f);
            r = Rando.Float(1, 3f);
            base.Initialize();
        }
        public float other;
        public float muli;
        public float r;
        public float fUp;
        public override void ApplyKick()
        {
            base.ApplyKick();
            if (duck == null || !duck._hovering)
                return;
            duck.vSpeed *= 0.5f;
        }

        public override void OnPressAction()
        {
            if (loaded)
            {
                base.OnPressAction();
            }
            else
            {
                if (ammo <= 0 || _loadState != -1)
                    return;
                _loadState = 0;
                _loadAnimation = 0;
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if (offDir > 0)
                this.angle -= _angleOffset;
            else
                this.angle += _angleOffset;
            base.Draw();
            this.angle = angle;
        }
    }
}
