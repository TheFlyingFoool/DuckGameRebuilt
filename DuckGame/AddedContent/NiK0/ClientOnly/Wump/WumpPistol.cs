/*using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Pistols")]
    public class WumpPistol : Gun
    {
        public WumpPistol(float xval, float yval) : base(xval, yval)
        {
            ammo = 9;
            _ammoType = new AT9mm();
            wideBarrel = true;
            barrelInsertOffset = new Vec2(0f, -1f);
            _type = "gun";
            _sprite = new SpriteMap("wumppistol", 32, 32);
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
            _sprite.AddAnimation("fire", 0.8f, false, new int[]
            {
                1,
                2,
                2,
                3,
                3
            });
            _sprite.AddAnimation("empty", 1f, true, new int[]
            {
                2
            });
            _sprite.AddAnimation("broken", 1f, true, new int[]
            {
                4
            });
            graphic = _sprite;
            center = new Vec2(16);
            collisionOffset = new Vec2(-8f, -7f);
            collisionSize = new Vec2(16f, 12f);
            _barrelOffsetTL = new Vec2(24f, 11f);
            _holdOffset = new Vec2(0, 4);
            _fireSound = "pistolFire";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(0f, 4f);
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _editorName = "Pistol";
            editorTooltip = "A frozen pistol that holds dark secrets within.";
            physicsMaterial = PhysicsMaterial.Metal;
        }
        public StateBinding _brokenBinding = new StateBinding("broken");
        public StateBinding _breakAgainCooldown = new StateBinding("breakAgainCooldown");
        public float breakAgainCooldown;
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && connection != DuckNetwork.localConnection)
            {
                return;
            }
            if (with.isBlock && CalculateImpactPower(with, from) > 4 && breakAgainCooldown <= 0)
            {
                if (broken)
                {
                    Level.Remove(this);
                }
                ammo = 1;
                SFX.PlaySynchronized("glassBreak");
                broken = true;
                _sprite.SetAnimation("broken");
                breakAgainCooldown = 1;
                for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 14; i++)
                {
                    Level.Add(new GlassParticle(x, y, new Vec2(Rando.Float(-1, 1), Rando.Float(-1, 1))));
                }
            }
            base.OnSoftImpact(with, from);
        }
        public override void Update()
        {
            if (isServerForObject)
            {
                breakAgainCooldown -= 0.017f;
            }
            if (broken && (Math.Abs(hSpeed) > 3 || Math.Abs(vSpeed) > 2) && isServerForObject)
            {
                _sprite.SetAnimation("broken");
                List<IAmADuck> iaads = Level.CheckCircleAll<IAmADuck>(position + velocity, 12).ToList();
                for (int i = 0; i < iaads.Count; i++)
                {
                    MaterialThing iaad = (MaterialThing)iaads[i];
                    if (Duck.GetAssociatedDuck(iaad) == lastThrownBy && _framesSinceThrown < 8) continue;
                    iaad.velocity = velocity;
                    iaad.Destroy(new DTImpale(this));
                }
            }
            if (_sprite.currentAnimation == "fire" && _sprite.finished)
            {
                _sprite.SetAnimation("idle");
            }
            base.Update();
        }
        protected override void PlayFireSound()
        {
            SFX.PlaySynchronized(_fireSound, pitch: (Rando.Float(0.2f) - 0.1f + _fireSoundPitch));
        }
        public override void Fire()
        {
            if (broken) return;
            if (!loaded)
            {
                return;
            }
            if (ammo > 0 && _wait == 0f)
            {
                firedBullets.Clear();
                if (duck != null)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                }
                ApplyKick();
                for (int i = 0; i < _numBulletsPerFire; i++)
                {
                    IceSpike ss = new IceSpike(barrelPosition.x, barrelPosition.y)
                    {
                        velocity = barrelVector.Rotate(Rando.Float(-0.1f, 0.1f), Vec2.Zero) * 12,
                        ignore = duck
                    };
                    if (duck != null) ss.velocity += duck.velocity;
                    Level.Add(ss);
                }
                _smokeWait = 3f;
                loaded = false;
                _flareAlpha = 1.5f;
                if (!_manualLoad)
                {
                    Reload(true);
                }
                firing = true;
                _wait = _fireWait;
                PlayFireSound();
                if (owner == null)
                {
                    Vec2 vec3 = barrelVector * Rando.Float(1f, 3f);
                    vec3.y += Rando.Float(2f);
                    hSpeed -= vec3.x;
                    vSpeed -= vec3.y;
                }
                _accuracyLost += loseAccuracy;
                if (_accuracyLost > maxAccuracyLost)
                {
                    _accuracyLost = maxAccuracyLost;
                    return;
                }
            }
            else if (ammo <= 0 && _wait == 0f)
            {
                firedBullets.Clear();
                DoAmmoClick();
                _wait = _fireWait;
            }
        }
        public override void OnPressAction()
        {
            if (broken) return;
            if (ammo > 0)
            {
                _sprite.SetAnimation("fire");
                for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 3; i++)
                {
                    Vec2 vec = Offset(new Vec2(-9f, 0f));
                    Vec2 hitAngle = barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add(Spark.New(vec.x, vec.y, hitAngle, 0.1f));
                }
            }
            else
            {
                _sprite.SetAnimation("empty");
            }
            Fire();
        }

        public bool broken;
        private SpriteMap _sprite;
    }
}*/