// Decompiled with JetBrains decompiler
// Type: DuckGame.GoodBook
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private SinWave _haloWave = (SinWave)0.05f;
        public float _ringPulse;

        public Duck controlling1
        {
            get => this.controlling[0];
            set => this.controlling[0] = value;
        }

        public Duck controlling2
        {
            get => this.controlling[1];
            set => this.controlling[1] = value;
        }

        public Duck controlling3
        {
            get => this.controlling[2];
            set => this.controlling[2] = value;
        }

        public Duck controlling4
        {
            get => this.controlling[3];
            set => this.controlling[3] = value;
        }

        public Duck controlling5
        {
            get => this.controlling[4];
            set => this.controlling[4] = value;
        }

        public Duck controlling6
        {
            get => this.controlling[5];
            set => this.controlling[5] = value;
        }

        public Duck controlling7
        {
            get => this.controlling[6];
            set => this.controlling[6] = value;
        }

        public Duck controlling8
        {
            get => this.controlling[7];
            set => this.controlling[7] = value;
        }

        public Duck prevControlling1
        {
            get => this.prevControlling[0];
            set => this.prevControlling[0] = value;
        }

        public Duck prevControlling2
        {
            get => this.prevControlling[1];
            set => this.prevControlling[1] = value;
        }

        public Duck prevControlling3
        {
            get => this.prevControlling[2];
            set => this.prevControlling[2] = value;
        }

        public Duck prevControlling4
        {
            get => this.prevControlling[3];
            set => this.prevControlling[3] = value;
        }

        public Duck prevControlling5
        {
            get => this.prevControlling[4];
            set => this.prevControlling[4] = value;
        }

        public Duck prevControlling6
        {
            get => this.prevControlling[5];
            set => this.prevControlling[5] = value;
        }

        public Duck prevControlling7
        {
            get => this.prevControlling[6];
            set => this.prevControlling[6] = value;
        }

        public Duck prevControlling8
        {
            get => this.prevControlling[7];
            set => this.prevControlling[7] = value;
        }

        public GoodBook(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._ammoType.range = 40f;
            this._type = "gun";
            this._sprite = new SpriteMap("goodBook", 17, 12);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 6f);
            this.collisionOffset = new Vec2(-5f, -4f);
            this.collisionSize = new Vec2(10f, 8f);
            this._halo = new Sprite("halo");
            this._halo.CenterOrigin();
            this._holdOffset = new Vec2(3f, 4f);
            this.handOffset = new Vec2(1f, 1f);
            this._hasTrigger = false;
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this.flammable = 1f;
            this._editorName = "Good Book";
            this.editorTooltip = "Converts enemies to your side. Don't spoil the ending.";
            this._bio = "A collection of words, maybe other ducks should hear them?";
            this.physicsMaterial = PhysicsMaterial.Wood;
            this._netPreach.function = new NetSoundEffect.Function(this.DoPreach);
            this.controlling = new Duck[8]
            {
        this.controlling1,
        this.controlling2,
        this.controlling3,
        this.controlling4,
        this.controlling5,
        this.controlling6,
        this.controlling7,
        this.controlling8
            };
        }

        public void DoPreach()
        {
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (this.connection == DuckNetwork.localConnection && value != this.connection)
                    this.LoseControl();
                base.connection = value;
            }
        }

        public void LoseControl()
        {
            for (int index = 0; index < 8; ++index)
            {
                Duck duck = this.controlling[index];
                if (duck != null)
                {
                    duck.listenTime = 0;
                    duck.listening = false;
                    this.controlling[index] = null;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            this._sprite.frame = this._owner == null || this._raised ? 0 : 1;
            this._raiseArm = Lerp.Float(this._raiseArm, 0f, 0.05f);
            this._preachWait = Lerp.Float(this._preachWait, 0f, 0.06f);
            this._ringPulse = Lerp.Float(this._ringPulse, 0f, 0.05f);
            if (Network.isActive)
            {
                if (this.isServerForObject)
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        Duck t1 = this.controlling[index];
                        if (t1 != null)
                        {
                            if (t1.listenTime <= 0)
                            {
                                this.controlling[index] = null;
                                t1.listening = false;
                            }
                            else
                            {
                                this.Fondle(t1);
                                this.Fondle(t1.holdObject);
                                foreach (Thing t2 in t1._equipment)
                                    this.Fondle(t2);
                                this.Fondle(t1._ragdollInstance);
                                this.Fondle(t1._trappedInstance);
                                this.Fondle(t1._cookedInstance);
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        Duck duck = this.controlling[index];
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
                        else if (this.prevControlling[index] != null)
                        {
                            this.prevControlling[index].listening = false;
                            this.prevControlling[index].listenTime = 0;
                        }
                    }
                }
            }
            if (this._triggerHeld && this.isServerForObject && this.duck != null && _preachWait <= 0.0 & this.duck.quack < 1 && this.duck.grounded)
            {
                if (Network.isActive)
                    this._netPreach.Play();
                else
                    SFX.Play("preach" + Rando.Int(5).ToString(), Rando.Float(0.8f, 1f), Rando.Float(-0.2f, -0.3f));
                this.duck.quack = (byte)Rando.Int(12, 30);
                this.duck.profile.stats.timePreaching += duck.quack / 0.1f * Maths.IncFrameTimer();
                this._preachWait = Rando.Float(1.8f, 2.5f);
                this._ringPulse = 1f;
                if (Rando.Int(1) == 0)
                    this._raiseArm = Rando.Float(1.2f, 2f);
                Ragdoll t3 = Level.Nearest<Ragdoll>(this.x, this.y, this);
                Vec2 vec2;
                if (t3 != null && t3.captureDuck != null && t3.captureDuck.dead && Level.CheckLine<Block>(this.duck.position, t3.position) == null)
                {
                    vec2 = t3.position - this.duck.position;
                    if (vec2.length < _ammoType.range)
                    {
                        if (Network.isActive)
                        {
                            this.Fondle(t3.captureDuck);
                            this.Fondle(t3);
                            Send.Message(new NMLayToRest(t3.captureDuck));
                        }
                        t3.captureDuck.LayToRest(this.duck.profile);
                    }
                }
                foreach (Duck duck1 in Level.current.things[typeof(Duck)])
                {
                    if (duck1 is TargetDuck && (duck1 as TargetDuck).stanceSetting == 3)
                    {
                        for (int index = 0; index < 3; ++index)
                            Level.Add(new MusketSmoke(duck1.x - 5f + Rando.Float(10f), (float)(duck1.y + 6.0 - 3.0 + Rando.Float(6f) - index * 1.0))
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
                    if (duck1 != this.duck && duck1.grounded && !(duck1.holdObject is GoodBook) && Level.CheckLine<Block>(this.duck.position, duck1.position) == null)
                    {
                        vec2 = duck1.position - this.duck.position;
                        if (vec2.length < _ammoType.range)
                        {
                            if (duck1.dead)
                            {
                                this.Fondle(duck1);
                                duck1.LayToRest(this.duck.profile);
                            }
                            else
                            {
                                Duck duck2 = this.duck.converted != null ? this.duck.converted : this.duck;
                                Duck duck3 = duck1.converted != null ? duck1.converted : duck1;
                                if (duck2 != duck3 && duck2.profile.team != duck3.profile.team)
                                {
                                    if (Network.isActive && duck1.profile.networkIndex >= 0 && duck1.profile.networkIndex < 8)
                                        this.controlling[duck1.profile.networkIndex] = duck1;
                                    duck1.listening = true;
                                    this.Fondle(duck1);
                                    this.Fondle(duck1.holdObject);
                                    foreach (Thing t4 in duck1._equipment)
                                        this.Fondle(t4);
                                    this.Fondle(duck1._ragdollInstance);
                                    this.Fondle(duck1._trappedInstance);
                                    this.Fondle(duck1._cookedInstance);
                                    duck1.listenTime = 80;
                                    if (this.owner.x < duck1.x)
                                        duck1.offDir = -1;
                                    else
                                        duck1.offDir = 1;
                                    duck1.ThrowItem(false);
                                    duck1.conversionResistance -= 30;
                                    if (duck1.conversionResistance <= 0)
                                    {
                                        duck1.ConvertDuck(duck2);
                                        if (Network.isActive)
                                        {
                                            Send.Message(new NMConversion(duck1, duck2));
                                            this.controlling[duck1.profile.networkIndex] = null;
                                        }
                                        duck1.conversionResistance = 50;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this._haloAlpha = Lerp.Float(this._haloAlpha, !this._triggerHeld || this.duck == null || !this.duck.grounded ? 0f : 1f, 0.05f);
            for (int index = 0; index < 8; ++index)
                this.prevControlling[index] = this.controlling[index];
        }

        public override void OnPressAction()
        {
        }

        public override void Draw()
        {
            if (this.duck != null && !this._raised && _raiseArm > 0.0)
            {
                SpriteMap spriteArms = this.duck._spriteArms;
                bool flipH = spriteArms.flipH;
                float angle = spriteArms.angle;
                spriteArms.flipH = offDir * -1 < 0;
                spriteArms.angle = 0.7f * offDir;
                Graphics.Draw(spriteArms, this.owner.x - 5 * offDir, (float)(this.owner.y + 3f + (this.duck.crouch ? 3f : 0f) + (this.duck.sliding ? 3f : 0f)));
                spriteArms.angle = angle;
                spriteArms.flipH = flipH;
                this.handOffset = new Vec2(9999f, 9999f);
            }
            else
                this.handOffset = new Vec2(1f, 1f);
            if (this.owner != null && _haloAlpha > 0.01f)
            {
                this._halo.alpha = (float)(_haloAlpha * 0.4f + (float)this._haloWave * 0.1f);
                this._halo.depth = -0.2f;
                this._halo.xscale = this._halo.yscale = (float)(0.95f + (float)this._haloWave * 0.05f);
                this._halo.angle += 0.01f;
                Graphics.Draw(this._halo, this.owner.x, this.owner.y);
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
                            Graphics.DrawLine(this.owner.position + vec2_2, this.owner.position + vec2_1, Color.White * (this._ringPulse * 0.6f), this._ringPulse * 10f);
                        vec2_1 = vec2_2;
                    }
                }
            }
            base.Draw();
        }
    }
}
