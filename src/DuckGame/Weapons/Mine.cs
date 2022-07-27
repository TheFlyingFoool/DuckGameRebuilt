// Decompiled with JetBrains decompiler
// Type: DuckGame.Mine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isFatal", false)]
    public class Mine : Gun
    {
        public StateBinding _mineBinding = new MineFlagBinding();
        private SpriteMap _sprite;
        public bool _pin = true;
        public bool blownUp;
        public float _timer = 1.2f;
        public bool _armed;
        public bool _clicked;
        public float addWeight;
        public int _framesSinceArm;
        public float _holdingWeight;
        public bool _thrown;
        private Sprite _mineFlash;
        private Dictionary<Duck, float> _ducksOnMine = new Dictionary<Duck, float>();
        public List<PhysicsObject> previousThings = new List<PhysicsObject>();
        private float prevAngle;
        public Duck duckWhoThrew;

        public bool pin => this._pin;

        public Mine(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._type = "gun";
            this._sprite = new SpriteMap("mine", 18, 16);
            this._sprite.AddAnimation("pickup", 1f, true, new int[1]);
            this._sprite.AddAnimation("idle", 0.05f, true, 1, 2);
            this._sprite.SetAnimation("pickup");
            this.graphic = _sprite;
            this.center = new Vec2(9f, 8f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 9f);
            this._mineFlash = new Sprite("mineFlash");
            this._mineFlash.CenterOrigin();
            this._mineFlash.alpha = 0.0f;
            this.bouncy = 0.0f;
            this.friction = 0.2f;
            this.editorTooltip = "Once placed in position, explodes if any curious Ducks walk by.";
        }

        public void Arm()
        {
            if (this._armed)
                return;
            this._holdingWeight = 0.0f;
            this._armed = true;
            if (!this.isServerForObject)
                return;
            if (Network.isActive)
                NetSoundEffect.Play("minePullPin");
            else
                SFX.Play("pullPin");
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._pin)
                return false;
            this.BlowUp();
            return true;
        }

        public void UpdatePinState()
        {
            if (!this._pin)
            {
                this.canPickUp = false;
                this._sprite.SetAnimation("idle");
                this.collisionOffset = new Vec2(-6f, -2f);
                this.collisionSize = new Vec2(12f, 3f);
                this.depth = (Depth)0.8f;
                this._hasOldDepth = false;
                this.thickness = 1f;
                this.center = new Vec2(9f, 14f);
            }
            else
            {
                this.canPickUp = true;
                this._sprite.SetAnimation("pickup");
                this.collisionOffset = new Vec2(-5f, -4f);
                this.collisionSize = new Vec2(10f, 8f);
                this.thickness = -1f;
            }
        }

        public Dictionary<Duck, float> ducksOnMine => this._ducksOnMine;

        public override void Update()
        {
            if (!this.pin)
            {
                this.collisionOffset = new Vec2(-6f, -2f);
                this.collisionSize = new Vec2(12f, 3f);
            }
            base.Update();
            if (!this.pin && (double)Math.Abs(this.prevAngle - this.angle) > 0.100000001490116)
            {
                Vec2 vec2_1 = new Vec2(14f, 3f);
                Vec2 vec2_2 = new Vec2(-7f, -2f);
                Vec2 vec2_3 = new Vec2(4f, 14f);
                Vec2 vec2_4 = new Vec2(-2f, -7f);
                float num = (float)Math.Abs(Math.Sin((double)this.angle));
                this.collisionSize = vec2_1 * (1f - num) + vec2_3 * num;
                this.collisionOffset = vec2_2 * (1f - num) + vec2_4 * num;
                this.prevAngle = this.angle;
            }
            this.UpdatePinState();
            if (this._sprite.imageIndex == 2)
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.4f, 0.08f);
            else
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.0f, 0.08f);
            if (this._armed)
                this._sprite.speed = 2f;
            if (this._thrown && this.owner == null)
            {
                this._thrown = false;
                if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.400000005960464)
                    this.angleDegrees = 180f;
            }
            if (this._armed)
                ++this._framesSinceArm;
            if (!this._pin && this._grounded && (!this._armed || this._framesSinceArm > 4))
            {
                this.canPickUp = false;
                float addWeight = this.addWeight;
                IEnumerable<PhysicsObject> physicsObjects = Level.CheckLineAll<PhysicsObject>(new Vec2(this.x - 6f, this.y - 3f), new Vec2(this.x + 6f, this.y - 3f));
                List<Duck> duckList1 = new List<Duck>();
                Duck duck = null;
                bool flag1 = false;
                bool flag2 = false;
                foreach (PhysicsObject previousThing in this.previousThings)
                {
                    if (previousThing.isServerForObject)
                        flag1 = true;
                    bool flag3 = false;
                    foreach (PhysicsObject physicsObject in physicsObjects)
                    {
                        if (physicsObject == previousThing)
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3 && previousThing.isServerForObject)
                        flag2 = true;
                }
                this.previousThings.Clear();
                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    if (physicsObject != this && physicsObject.owner == null && (!(physicsObject is Holdable) || (physicsObject as Holdable).canPickUp && (physicsObject as Holdable).hoverSpawner == null) && (double)Math.Abs(physicsObject.bottom - this.bottom) <= 6.0)
                    {
                        if (physicsObject.isServerForObject)
                            flag1 = true;
                        this.previousThings.Add(physicsObject);
                        switch (physicsObject)
                        {
                            case Duck _:
                            case TrappedDuck _:
                            case RagdollPart _:
                                addWeight += 5f;
                                Duck key = physicsObject as Duck;
                                if (physicsObject is TrappedDuck)
                                    key = (physicsObject as TrappedDuck).captureDuck;
                                else if (physicsObject is RagdollPart && (physicsObject as RagdollPart).doll != null)
                                    key = (physicsObject as RagdollPart).doll.captureDuck;
                                if (key != null)
                                {
                                    duck = key;
                                    if (!this._ducksOnMine.ContainsKey(key))
                                        this._ducksOnMine[key] = 0.0f;
                                    this._ducksOnMine[key] += Maths.IncFrameTimer();
                                    duckList1.Add(key);
                                    break;
                                }
                                break;
                            default:
                                addWeight += physicsObject.weight;
                                break;
                        }
                        foreach (PhysicsObject previousThing in this.previousThings)
                            ;
                    }
                }
                List<Duck> duckList2 = new List<Duck>();
                foreach (KeyValuePair<Duck, float> keyValuePair in this._ducksOnMine)
                {
                    if (!duckList1.Contains(keyValuePair.Key))
                        duckList2.Add(keyValuePair.Key);
                    else
                        keyValuePair.Key.profile.stats.timeSpentOnMines += Maths.IncFrameTimer();
                }
                foreach (Duck key in duckList2)
                    this._ducksOnMine.Remove(key);
                if ((double)addWeight < _holdingWeight & flag1 && flag2)
                {
                    Thing.Fondle(this, DuckNetwork.localConnection);
                    if (!this._armed)
                        this.Arm();
                    else
                        this._timer = -1f;
                }
                if (this._armed && (double)addWeight > _holdingWeight)
                {
                    if (!this._clicked && duck != null)
                        ++duck.profile.stats.minesSteppedOn;
                    this._clicked = true;
                    SFX.Play("doubleBeep");
                }
                this._holdingWeight = addWeight;
            }
            if (_timer < 0.0 && this.isServerForObject)
            {
                this._timer = 1f;
                this.BlowUp();
            }
            this.addWeight = 0.0f;
        }

        public void BlowUp()
        {
            if (this.blownUp)
                return;
            this.MakeBlowUpHappen(this.position);
            this.blownUp = true;
            if (!this.isServerForObject)
                return;
            foreach (PhysicsObject t in Level.CheckCircleAll<PhysicsObject>(this.position, 22f))
            {
                if (t != this)
                {
                    Vec2 vec2 = t.position - this.position;
                    float num1 = (float)(1.0 - (double)Math.Min(vec2.length, 22f) / 22.0);
                    float num2 = num1 * 4f;
                    vec2.Normalize();
                    t.hSpeed += num2 * vec2.x;
                    t.vSpeed += -5f * num1;
                    t.sleeping = false;
                    this.Fondle(t);
                }
            }
            float x = this.position.x;
            float y = this.position.y;
            for (int index = 0; index < 20; ++index)
            {
                float ang = (float)(index * 18.0 - 5.0) + Rando.Float(10f);
                ATShrapnel type = new ATShrapnel
                {
                    range = 60f + Rando.Float(18f)
                };
                Bullet bullet = new Bullet(x, y, type, ang)
                {
                    firedFrom = this
                };
                this.firedBullets.Add(bullet);
                Level.Add(bullet);
            }
            this.bulletFireIndex += 20;
            if (Network.isActive && this.isServerForObject)
            {
                Send.Message(new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false), NetMessagePriority.ReliableOrdered);
                this.firedBullets.Clear();
            }
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogBonus();
            Level.Remove(this);
        }

        public void MakeBlowUpHappen(Vec2 pos)
        {
            if (this.blownUp)
                return;
            this.blownUp = true;
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            Graphics.FlashScreen();
            float x = pos.x;
            float y = pos.y;
            Level.Add(new ExplosionPart(x, y));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add(new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, y - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2));
            }
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this.MakeBlowUpHappen(pos);
            base.OnNetworkBulletsFired(pos);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null && !this.canPickUp && _timer > 0.0)
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                this.BlowUp();
            }
            return false;
        }

        public override void Draw()
        {
            Material material = Graphics.material;
            Graphics.material = null;
            if ((double)this._mineFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._mineFlash, this.x, this.y - 3f);
            Graphics.material = material;
            base.Draw();
        }

        public override void OnPressAction()
        {
            if (!this.isServerForObject)
                return;
            if (this.owner == null)
            {
                this._pin = false;
                if (heat > 0.5)
                    this.BlowUp();
            }
            if (!this._pin)
                return;
            this._pin = false;
            this.UpdatePinState();
            if (this.owner is Duck owner)
            {
                this.duckWhoThrew = owner;
                this._holdingWeight = 5f;
                owner.doThrow = true;
                this._responsibleProfile = owner.profile;
            }
            else
                this.Arm();
            this._thrown = true;
        }
    }
}
