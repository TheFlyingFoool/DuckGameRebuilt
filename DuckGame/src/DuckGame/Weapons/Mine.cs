// Decompiled with JetBrains decompiler
// Type: DuckGame.Mine
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public bool pin => _pin;

        public Mine(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 1;
            _ammoType = new ATShrapnel();
            _type = "gun";
            _sprite = new SpriteMap("mine", 18, 16);
            _sprite.AddAnimation("pickup", 1f, true, new int[1]);
            _sprite.AddAnimation("idle", 0.05f, true, 1, 2);
            _sprite.SetAnimation("pickup");
            graphic = _sprite;
            center = new Vec2(9f, 8f);
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(10f, 9f);
            _mineFlash = new Sprite("mineFlash");
            _mineFlash.CenterOrigin();
            _mineFlash.alpha = 0f;
            bouncy = 0f;
            friction = 0.2f;
            editorTooltip = "Once placed in position, explodes if any curious Ducks walk by.";
        }

        public void Arm()
        {
            if (_armed)
                return;
            _holdingWeight = 0f;
            _armed = true;
            if (!isServerForObject)
                return;
            if (Network.isActive)
                NetSoundEffect.Play("minePullPin");
            else
                SFX.Play("pullPin");
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (_pin)
                return false;
            BlowUp();
            return true;
        }

        public void UpdatePinState()
        {
            if (!_pin)
            {
                canPickUp = false;
                _sprite.SetAnimation("idle");
                collisionOffset = new Vec2(-6f, -2f);
                collisionSize = new Vec2(12f, 3f);
                depth = (Depth)0.8f;
                _hasOldDepth = false;
                thickness = 1f;
                center = new Vec2(9f, 14f);
            }
            else
            {
                canPickUp = true;
                _sprite.SetAnimation("pickup");
                collisionOffset = new Vec2(-5f, -4f);
                collisionSize = new Vec2(10f, 8f);
                thickness = -1f;
            }
        }

        public Dictionary<Duck, float> ducksOnMine => _ducksOnMine;

        public override void Update()
        {
            if (!pin)
            {
                collisionOffset = new Vec2(-6f, -2f);
                collisionSize = new Vec2(12f, 3f);
            }
            base.Update();
            if (!pin && Math.Abs(prevAngle - angle) > 0.1f)
            {
                Vec2 vec2_1 = new Vec2(14f, 3f);
                Vec2 vec2_2 = new Vec2(-7f, -2f);
                Vec2 vec2_3 = new Vec2(4f, 14f);
                Vec2 vec2_4 = new Vec2(-2f, -7f);
                float num = (float)Math.Abs(Math.Sin(angle));
                collisionSize = vec2_1 * (1f - num) + vec2_3 * num;
                collisionOffset = vec2_2 * (1f - num) + vec2_4 * num;
                prevAngle = angle;
            }
            UpdatePinState();
            if (_sprite.imageIndex == 2)
                _mineFlash.alpha = Lerp.Float(_mineFlash.alpha, 0.4f, 0.08f);
            else
                _mineFlash.alpha = Lerp.Float(_mineFlash.alpha, 0f, 0.08f);
            if (_armed)
                _sprite.speed = 2f;
            if (_thrown && owner == null)
            {
                _thrown = false;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 0.4f)
                    angleDegrees = 180f;
            }
            if (_armed)
                ++_framesSinceArm;
            if (!_pin && _grounded && (!_armed || _framesSinceArm > 4))
            {
                canPickUp = false;
                float addWeight = this.addWeight;
                IEnumerable<PhysicsObject> physicsObjects = Level.CheckLineAll<PhysicsObject>(new Vec2(x - 6f, y - 3f), new Vec2(x + 6f, y - 3f));
                List<Duck> duckList1 = new List<Duck>();
                Duck duck = null;
                bool flag1 = false;
                bool flag2 = false;
                foreach (PhysicsObject previousThing in previousThings)
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
                previousThings.Clear();
                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    if (physicsObject != this && physicsObject.owner == null && (!(physicsObject is Holdable) || (physicsObject as Holdable).canPickUp && (physicsObject as Holdable).hoverSpawner == null) && Math.Abs(physicsObject.bottom - bottom) <= 6)
                    {
                        if (physicsObject.isServerForObject)
                            flag1 = true;
                        previousThings.Add(physicsObject);
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
                                    if (!_ducksOnMine.ContainsKey(key))
                                        _ducksOnMine[key] = 0f;
                                    _ducksOnMine[key] += Maths.IncFrameTimer();
                                    duckList1.Add(key);
                                    break;
                                }
                                break;
                            default:
                                addWeight += physicsObject.weight;
                                break;
                        }
                        foreach (PhysicsObject previousThing in previousThings)
                            ;
                    }
                }
                List<Duck> duckList2 = new List<Duck>();
                foreach (KeyValuePair<Duck, float> keyValuePair in _ducksOnMine)
                {
                    if (!duckList1.Contains(keyValuePair.Key))
                        duckList2.Add(keyValuePair.Key);
                    else
                        keyValuePair.Key.profile.stats.timeSpentOnMines += Maths.IncFrameTimer();
                }
                foreach (Duck key in duckList2)
                    _ducksOnMine.Remove(key);
                if (addWeight < _holdingWeight & flag1 && flag2)
                {
                    Fondle(this, DuckNetwork.localConnection);
                    if (!_armed)
                        Arm();
                    else
                        _timer = -1f;
                }
                if (_armed && addWeight > _holdingWeight)
                {
                    if (!_clicked && duck != null)
                        ++duck.profile.stats.minesSteppedOn;
                    _clicked = true;
                    SFX.Play("doubleBeep");
                }
                _holdingWeight = addWeight;
            }
            if (_timer < 0 && isServerForObject)
            {
                _timer = 1f;
                BlowUp();
            }
            addWeight = 0f;
        }

        public void BlowUp()
        {
            if (blownUp)
                return;
            MakeBlowUpHappen(position);
            blownUp = true;
            if (!isServerForObject)
                return;
            foreach (PhysicsObject t in Level.CheckCircleAll<PhysicsObject>(position, 22f))
            {
                if (t != this)
                {
                    Vec2 vec2 = t.position - position;
                    float num1 = (float)(1 - Math.Min(vec2.length, 22f) / 22);
                    float num2 = num1 * 4f;
                    vec2.Normalize();
                    t.hSpeed += num2 * vec2.x;
                    t.vSpeed += -5f * num1;
                    t.sleeping = false;
                    Fondle(t);
                }
            }
            if (!Recorderator.Playing)
            {
                float x = position.x;
                float y = position.y;
                for (int index = 0; index < 20; ++index)
                {
                    float ang = (float)(index * 18 - 5) + Rando.Float(10f);
                    ATShrapnel type = new ATShrapnel
                    {
                        range = 60f + Rando.Float(18f)
                    };
                    Bullet bullet = new Bullet(x, y, type, ang)
                    {
                        firedFrom = this
                    };
                    firedBullets.Add(bullet);
                    Level.Add(bullet);
                }
                bulletFireIndex += 20;
                if (Network.isActive && isServerForObject)
                {
                    Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false), NetMessagePriority.ReliableOrdered);
                    firedBullets.Clear();
                }
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
            }
            Level.Remove(this);
        }

        public void MakeBlowUpHappen(Vec2 pos)
        {
            if (blownUp)
                return;
            if (currentVessel != null && currentVessel is MineVessel mv && !Recorderator.Playing)
            {
                mv.explodeFrame = mv.exFrames;
                mv.v = pos;
            }
            blownUp = true;
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
                Level.Add(new ExplosionPart(x + (float)Math.Cos(Maths.DegToRad(deg)) * num2, y - (float)Math.Sin(Maths.DegToRad(deg)) * num2));
            }
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            MakeBlowUpHappen(pos);
            base.OnNetworkBulletsFired(pos);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null && !canPickUp && _timer > 0)
            {
                Fondle(this, DuckNetwork.localConnection);
                BlowUp();
            }
            return false;
        }

        public override void Draw()
        {
            Material material = Graphics.material;
            Graphics.material = null;
            if (_mineFlash.alpha > 0.01f)
                Graphics.Draw(_mineFlash, x, y - 3f);
            Graphics.material = material;
            base.Draw();
        }

        public override void OnPressAction()
        {
            if (!isServerForObject)
                return;
            if (this.owner == null)
            {
                _pin = false;
                if (heat > 0.5)
                    BlowUp();
            }
            if (!_pin)
                return;
            _pin = false;
            UpdatePinState();
            if (this.owner is Duck owner)
            {
                duckWhoThrew = owner;
                _holdingWeight = 5f;
                owner.doThrow = true;
                _responsibleProfile = owner.profile;
            }
            else
                Arm();
            _thrown = true;
        }
    }
}
