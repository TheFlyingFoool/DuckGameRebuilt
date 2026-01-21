using System;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class GoodBook : Gun
    {
        public StateBinding _raiseArmBinding = new StateBinding(nameof(_raiseArm));
        public StateBinding _timerBinding = new StateBinding(nameof(_timer));
        public StateBinding _netPreachBinding = new NetSoundBinding(nameof(_netPreach));
        public StateBinding _ringPulseBinding = new StateBinding(nameof(_ringPulse));
        public StateBinding _controlling1Binding = new StateBinding(nameof(controlling1));
        public StateBinding _controlling2Binding = new StateBinding(nameof(controlling2));
        public StateBinding _controlling3Binding = new StateBinding(nameof(controlling3));
        public StateBinding _controlling4Binding = new StateBinding(nameof(controlling4));
        public StateBinding _controlling5Binding = new StateBinding(nameof(controlling5));
        public StateBinding _controlling6Binding = new StateBinding(nameof(controlling6));
        public StateBinding _controlling7Binding = new StateBinding(nameof(controlling7));
        public StateBinding _controlling8Binding = new StateBinding(nameof(controlling8));
        public Duck[] controlling = new Duck[8];
        public Duck[] prevControlling = new Duck[8];
        public NetSoundEffect _netPreach = new NetSoundEffect(new string[6]
        {
          "preach0",
          "preach1",
          "preach2",
          "preach3",
          "preach4",
          "preach5"
        })
        {
            pitchVariationLow = -0.3f,
            pitchVariationHigh = -0.2f
        };
        private SpriteMap _sprite;
        public float _timer = 1.2f;
        public float _raiseArm;
        private Sprite _halo;
        private float _preachWait;
        private float _haloAlpha;
        private SinWave _haloWave;
        public float _ringPulse;
        private readonly Interp _armLerp = new Interp();
        private SpriteMap _armSprite;

        public Duck controlling1
        {
            get => controlling[0];
            set => controlling[0] = value;
        }

        public Duck controlling2
        {
            get => controlling[1];
            set => controlling[1] = value;
        }

        public Duck controlling3
        {
            get => controlling[2];
            set => controlling[2] = value;
        }

        public Duck controlling4
        {
            get => controlling[3];
            set => controlling[3] = value;
        }

        public Duck controlling5
        {
            get => controlling[4];
            set => controlling[4] = value;
        }

        public Duck controlling6
        {
            get => controlling[5];
            set => controlling[5] = value;
        }

        public Duck controlling7
        {
            get => controlling[6];
            set => controlling[6] = value;
        }

        public Duck controlling8
        {
            get => controlling[7];
            set => controlling[7] = value;
        }

        public Duck prevControlling1
        {
            get => prevControlling[0];
            set => prevControlling[0] = value;
        }

        public Duck prevControlling2
        {
            get => prevControlling[1];
            set => prevControlling[1] = value;
        }

        public Duck prevControlling3
        {
            get => prevControlling[2];
            set => prevControlling[2] = value;
        }

        public Duck prevControlling4
        {
            get => prevControlling[3];
            set => prevControlling[3] = value;
        }

        public Duck prevControlling5
        {
            get => prevControlling[4];
            set => prevControlling[4] = value;
        }

        public Duck prevControlling6
        {
            get => prevControlling[5];
            set => prevControlling[5] = value;
        }

        public Duck prevControlling7
        {
            get => prevControlling[6];
            set => prevControlling[6] = value;
        }

        public Duck prevControlling8
        {
            get => prevControlling[7];
            set => prevControlling[7] = value;
        }

        public GoodBook(float xval, float yval)
          : base(xval, yval)
        {
            _haloWave = new SinWave(this, 0.05f);
            ammo = 1;
            _ammoType = new ATShrapnel
            {
                penetration = 0.4f,
                range = 40f
            };
            _type = "gun";
            _sprite = new SpriteMap("goodBook", 17, 12);
            graphic = _sprite;
            center = new Vec2(8f, 6f);
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(10f, 8f);
            _halo = new Sprite("halo");
            _halo.CenterOrigin();
            _holdOffset = new Vec2(3f, 4f);
            handOffset = new Vec2(1f, 1f);
            _hasTrigger = false;
            bouncy = 0.4f;
            friction = 0.05f;
            flammable = 1f;
            _editorName = "Good Book";
            editorTooltip = "Converts enemies to your side. Don't spoil the ending.";
            _bio = "A collection of words, maybe other ducks should hear them?";
            physicsMaterial = PhysicsMaterial.Wood;
            _netPreach.function = new NetSoundEffect.Function(DoPreach);
            controlling = new Duck[8]
            {
                controlling1,
                controlling2,
                controlling3,
                controlling4,
                controlling5,
                controlling6,
                controlling7,
                controlling8
            };
        }

        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is GoodBook && pTaped.gun2 is HugeLaser ? new Propaganda(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }

        public void DoPreach()
        {
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (connection == DuckNetwork.localConnection && value != connection)
                    LoseControl();
                base.connection = value;
            }
        }

        public void LoseControl()
        {
            for (int index = 0; index < 8; ++index)
            {
                Duck duck = controlling[index];
                if (duck != null)
                {
                    duck.listenTime = 0;
                    duck.listening = false;
                    controlling[index] = null;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            _sprite.frame = _owner == null || _raised ? 0 : 1;
            _raiseArm = Lerp.Float(_raiseArm, 0f, 0.05f);
            _preachWait = Lerp.Float(_preachWait, 0f, 0.06f);
            _ringPulse = Lerp.Float(_ringPulse, 0f, 0.05f);
            if (Network.isActive && !Network.isFakeActive)
            {
                if (isServerForObject)
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        Duck t1 = controlling[index];
                        if (t1 != null)
                        {
                            if (t1.listenTime <= 0)
                            {
                                controlling[index] = null;
                                t1.listening = false;
                            }
                            else
                            {
                                Fondle(t1);
                                Fondle(t1.holdObject);
                                foreach (Thing t2 in t1._equipment)
                                    Fondle(t2);
                                Fondle(t1._ragdollInstance);
                                Fondle(t1._trappedInstance);
                                Fondle(t1._cookedInstance);
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        Duck duck = controlling[index];
                        if (duck != null)
                        {
                            duck.listening = true;
                            duck.listenTime = 80;
                            duck.Fondle(duck.holdObject);
                            foreach (Equipment t in duck._equipment)
                                duck.Fondle(t);
                            duck.Fondle(duck._ragdollInstance);
                            duck.Fondle(duck._trappedInstance);
                            duck.Fondle(duck._cookedInstance);
                        }
                        else if (prevControlling[index] != null)
                        {
                            prevControlling[index].listening = false;
                            prevControlling[index].listenTime = 0;
                        }
                    }
                }
            }
            if (_triggerHeld && isServerForObject && duck != null && _preachWait <= 0 & duck.quack < 1 && duck.grounded)
            {
                if (Network.isActive)
                    _netPreach.Play();
                else
                    SFX.Play("preach" + Rando.Int(5).ToString(), Rando.Float(0.8f, 1f), Rando.Float(-0.2f, -0.3f));
                duck.quack = (byte)Rando.Int(12, 30);
                duck.profile.stats.timePreaching += duck.quack / 0.1f * Maths.IncFrameTimer();
                _preachWait = Rando.Float(1.8f, 2.5f);
                _ringPulse = 1f;
                if (Rando.Int(1) == 0)
                    _raiseArm = Rando.Float(1.2f, 2f);
                Ragdoll t3 = Level.Nearest<Ragdoll>(position, _ammoType.range);
                Vec2 vec2;
                if (t3 != null && t3.captureDuck != null && t3.captureDuck.dead && Level.CheckLine<Block>(duck.position, t3.position) == null)
                {
                    vec2 = t3.position - duck.position;
                    if (vec2.length < _ammoType.range)
                    {
                        if (Network.isActive)
                        {
                            Fondle(t3.captureDuck);
                            Fondle(t3);
                            Send.Message(new NMLayToRest(t3.captureDuck));
                        }
                        t3.captureDuck.LayToRest(duck.profile);
                    }
                }
                foreach (Duck duck1 in Level.current.things[typeof(Duck)])
                {
                    if (duck1 is TargetDuck && (duck1 as TargetDuck).stanceSetting == 3)
                    {
                        for (int index = 0; index < 3 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); ++index)
                            Level.Add(new MusketSmoke(duck1.x - 5f + Rando.Float(10f), (float)(duck1.y + 6 - 0 + Rando.Float(6f) - index * 1))
                            {
                                move = {
                  x = (Rando.Float(0.4f) - 0.2f),
                  y = (Rando.Float(0.4f) - 0.2f)
                }
                            });
                        SFX.Play("death");
                        Tombstone tombstone = new Tombstone(duck1.x, duck1.y)
                        {
                            vSpeed = -2.5f
                        };
                        Level.Add(tombstone);
                        Level.Remove(duck1);
                    }
                    if (duck1 != duck && duck1.grounded && !(duck1.holdObject is GoodBook) && Level.CheckLine<Block>(duck.position, duck1.position) == null)
                    {
                        vec2 = duck1.position - duck.position;
                        if (vec2.length < _ammoType.range)
                        {
                            if (duck1.dead)
                            {
                                Fondle(duck1);
                                duck1.LayToRest(duck.profile);
                            }
                            else
                            {
                                Duck duck2 = duck.converted != null ? duck.converted : duck;
                                Duck duck3 = duck1.converted != null ? duck1.converted : duck1;
                                if (duck2 != duck3 && duck2.profile.team != duck3.profile.team)
                                {
                                    if (Network.isActive && duck1.profile.networkIndex >= 0 && duck1.profile.networkIndex < 8)
                                        controlling[duck1.profile.networkIndex] = duck1;
                                    duck1.listening = true;
                                    Fondle(duck1);
                                    Fondle(duck1.holdObject);
                                    foreach (Thing t4 in duck1._equipment)
                                        Fondle(t4);
                                    Fondle(duck1._ragdollInstance);
                                    Fondle(duck1._trappedInstance);
                                    Fondle(duck1._cookedInstance);
                                    duck1.listenTime = 80;
                                    if (owner.x < duck1.x)
                                        duck1.offDir = -1;
                                    else
                                        duck1.offDir = 1;
                                    duck1.ThrowItem(false);
                                    duck1.conversionResistance -= 30;
                                    if (duck1.conversionResistance <= 0)
                                    {
                                        duck1.ConvertDuck(duck2);
                                        if (Network.isActive && !Network.isFakeActive)
                                        {
                                            Send.Message(new NMConversion(duck1, duck2));
                                            controlling[duck1.profile.networkIndex] = null;
                                        }
                                        duck1.conversionResistance = 50;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _haloAlpha = Lerp.Float(_haloAlpha, !_triggerHeld || duck == null || !duck.grounded ? 0f : 1f, 0.05f);
            for (int index = 0; index < 8; ++index)
                prevControlling[index] = controlling[index];
        }

        public override void OnPressAction()
        {
        }

        public override void Draw()
        {
            if (duck != null && !_raised && _raiseArm > 0)
            {
                Material mat = Graphics.material;
                Graphics.material = null;
                SpriteMap spriteArms = duck._spriteArms;
                if (_armSprite == null || _armSprite.texture != spriteArms.texture)
                    _armSprite = spriteArms.CloneMap();
                _armSprite.SetFrameWithoutReset(spriteArms.frame);
                _armSprite.UpdateSpriteBox();
                _armSprite.flipH = offDir * -1 < 0;
                _armSprite.angle = 0.7f * offDir;
                _armSprite.alpha = spriteArms.alpha;
                _armSprite.depth = spriteArms.depth;
                _armSprite.scale = spriteArms.scale;
                _armSprite.center = spriteArms.center;
                _armSprite.color = spriteArms.color;
                Vec2 targetPos = new Vec2(owner.x - 5 * offDir, (float)(owner.y + 3f + (duck.crouch ? 3f : 0f) + (duck.sliding ? 3f : 0f)));
                _armLerp.CanLerp = true;
                _armLerp.UpdateLerpState(targetPos, duck.SkipIntratick > 0 ? 1 : MonoMain.IntraTick, MonoMain.UpdateLerpState);
                _armSprite.position = _armLerp.Position;
                _armSprite.LerpState.CanLerp = false;
                _armSprite.SkipIntraTick = duck.SkipIntratick;
                _armSprite.DrawWithoutUpdate();
                handOffset = new Vec2(9999f, 9999f);
                Graphics.material = mat;
            }
            else
                handOffset = new Vec2(1f, 1f);
            if (owner != null && _haloAlpha > 0.01f)
            {
                _halo.alpha = (float)(_haloAlpha * 0.4f + (float)_haloWave * 0.1f);
                _halo.depth = -0.2f;
                _halo.xscale = _halo.yscale = (float)(0.95f + (float)_haloWave * 0.05f);
                _halo.SkipIntraTick = SkipIntratick;
                if (MonoMain.UpdateLerpState) _halo.angle += 0.01f;
                Graphics.Draw(ref _halo, owner.x, owner.y);
                if (_ringPulse > 0f)
                {
                    int num1 = 16;
                    Vec2 vec2_1 = Vec2.Zero;
                    float num2 = (float)(_ammoType.range * 0.1f + (1f - _ringPulse) * (_ammoType.range * 0.9f));
                    for (int index = 0; index < num1; ++index)
                    {
                        float rad = Maths.DegToRad(360 / (num1 - 1) * index);
                        Vec2 vec2_2 = new Vec2((float)Math.Cos(rad) * num2, (float)-Math.Sin(rad) * num2);
                        if (index > 0)
                            Graphics.DrawLine(owner.position + vec2_2, owner.position + vec2_1, Color.White * (_ringPulse * 0.6f), _ringPulse * 10f);
                        vec2_1 = vec2_2;
                    }
                }
            }
            base.Draw();
        }
    }
}
