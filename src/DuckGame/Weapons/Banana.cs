// Decompiled with JetBrains decompiler
// Type: DuckGame.Banana
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isFatal", false)]
    public class Banana : Gun
    {
        public StateBinding _bananaStateBinding = new BananaFlagBinding();
        private SpriteMap _sprite;
        public bool _pin = true;
        public bool _thrown;
        private bool _fade;
        private bool _splatted;

        public bool pin => _pin;

        public override float angle
        {
            get
            {
                if (owner == null)
                    return base.angle;
                return offDir > 0 ? base.angle + 1.570796f : base.angle - 1.570796f;
            }
            set => _angle = value;
        }

        public Banana(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 1;
            _ammoType = new ATShrapnel();
            _type = "gun";
            _sprite = new SpriteMap("banana", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 13f);
            collisionOffset = new Vec2(-6f, -3f);
            collisionSize = new Vec2(12f, 5f);
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, 2f);
            bouncy = 0.4f;
            friction = 0.05f;
            physicsMaterial = PhysicsMaterial.Rubber;
            editorTooltip = "A tactically placed banana peel can cause major injuries.";
            isFatal = false;
        }

        public override void Update()
        {
            base.Update();
            if (_thrown && owner == null)
            {
                _thrown = false;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 0.4f)
                    angleDegrees = 180f;
            }
            if (!_pin && owner == null && !_fade)
            {
                _sprite.frame = 2;
                weight = 0.1f;
            }
            if (_fade)
            {
                alpha -= 0.1f;
                if (alpha <= 0f)
                {
                    Level.Remove(this);
                    alpha = 0f;
                }
            }
            if (!_pin && owner == null)
                canPickUp = false;
            if (!_pin && _grounded && !_fade)
            {
                if (!_splatted)
                {
                    _splatted = true;
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            NetSoundEffect.Play("bananaSplat");
                    }
                    else
                        SFX.Play("smallSplat", pitch: Rando.Float(-0.2f, 0.2f));
                }
                angleDegrees = 0f;
                canPickUp = false;
                foreach (Duck duck in Level.CheckLineAll<Duck>(new Vec2(x - 5f, y + 2f), new Vec2(x + 5f, y + 2f)))
                {
                    if (duck.grounded && !duck.crouch && !duck.sliding && duck.bottom <= bottom + 2f && duck.isServerForObject && Math.Abs(duck.hSpeed) > 2.5f)
                    {
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                        duck.Fondle(this);
                        if (Network.isActive)
                        {
                            if (isServerForObject)
                                NetSoundEffect.Play("bananaSlip");
                            if (Teams.active.Count > 1 && Rando.Int(100) == 1 && duck.connection == DuckNetwork.localConnection)
                                DuckNetwork.GiveXP("Banana Man", 0, 5, firstCap: 20, secondCap: 30, finalCap: 40);
                        }
                        else
                            SFX.Play("slip", pitch: Rando.Float(-0.2f, 0.2f));
                        if (duck.hSpeed < 0f)
                            duck.hSpeed -= 1.5f;
                        else
                            duck.hSpeed += 1.5f;
                        duck.vSpeed -= 2.5f;
                        hSpeed = -duck.hSpeed * 0.4f;
                        friction = 0.05f;
                        weight = 0.01f;
                        duck.crippleTimer = 1.5f;
                        PhysicsObject holdObject = duck.holdObject;
                        if (holdObject != null)
                        {
                            duck.ThrowItem(false);
                            holdObject.vSpeed -= 4f;
                            holdObject.hSpeed = duck.hSpeed * 0.8f;
                            holdObject.clip.Add(duck);
                            duck.clip.Add(holdObject);
                        }
                        duck.GoRagdoll();
                        if (duck.ragdoll != null && duck.ragdoll.part1 != null && duck.ragdoll.part2 != null && duck.ragdoll.part3 != null)
                        {
                            if (holdObject != null)
                            {
                                duck.ragdoll.part1.clip.Add(holdObject);
                                duck.ragdoll.part2.clip.Add(holdObject);
                                duck.ragdoll.part3.clip.Add(holdObject);
                                holdObject.clip.Add(duck.ragdoll.part1);
                                holdObject.clip.Add(duck.ragdoll.part2);
                                holdObject.clip.Add(duck.ragdoll.part3);
                            }
                            duck.ragdoll.part1.hSpeed *= 0.5f;
                            duck.ragdoll.part3.hSpeed *= 1.5f;
                        }
                        _sprite.frame = 3;
                        _fade = true;
                        Level.Add(new BananaSlip(x, y + 2f, duck.offDir > 0));
                    }
                }
            }
            if (_triggerHeld)
            {
                if (duck == null)
                    return;
                duck.quack = 20;
                if (offDir > 0)
                {
                    handAngle = -1.099557f;
                    handOffset = new Vec2(8f, -1f);
                    _holdOffset = new Vec2(-1f, 10f);
                }
                else
                {
                    handAngle = 1.099557f;
                    handOffset = new Vec2(8f, -1f);
                    _holdOffset = new Vec2(-1f, 10f);
                }
            }
            else
            {
                handAngle = 0f;
                handOffset = new Vec2(0f, 0f);
                _holdOffset = new Vec2(-1f, 2f);
            }
        }

        public override void HeatUp(Vec2 location)
        {
        }

        public void EatBanana()
        {
            _sprite.frame = 1;
            _pin = false;
            _holdOffset = new Vec2(-2f, 3f);
            collisionOffset = new Vec2(-4f, -2f);
            collisionSize = new Vec2(8f, 4f);
            weight = 0.01f;
            if (duck != null)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
            if (Network.isActive)
            {
                if (isServerForObject)
                    NetSoundEffect.Play("bananaEat");
            }
            else
                SFX.Play("smallSplat", pitch: Rando.Float(-0.6f, 0.6f));
            bouncy = 0f;
            friction = 0.3f;
        }

        public override void OnPressAction()
        {
            if (!pin)
                return;
            EatBanana();
        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
        }
    }
}
