using System;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("previewPriority", true)]
    public class Jetpack : Equipment
    {
        public StateBinding _onBinding = new StateBinding(nameof(_on));
        public StateBinding _heatBinding = new StateBinding(nameof(_heat));
        protected SpriteMap _sprite;
        public bool _on;
        public float _heat;

        public Jetpack(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("jetpack", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(11f, 12f);
            _offset = new Vec2(-3f, 3f);
            _equippedDepth = -15;
            _jumpMod = true;
            thickness = 0.1f;
            _wearOffset = new Vec2(-2f, 0f);
            editorTooltip = "Allows you to fly like some kind of soaring bird.";
        }

        public override void OnPressAction() => _on = true;

        public override void OnReleaseAction() => _on = false;

        public override void Update()
        {
            base.Update();
            _sprite.frame = (int)(_heat * 7f);
            if (_equippedDuck != null)
            {
                float smokeOff = 0f;
                _offset = new Vec2(-3f, 3f);
                angle = 0f;
                if (_equippedDuck.sliding && _equippedDuck._trapped == null)
                {
                    if (_equippedDuck.offDir > 0)
                        angle = -(float)(Math.PI / 2.0f);
                    else
                        angle = (float)(Math.PI / 2.0f);
                    _offset.y += 12f;
                    smokeOff -= 6f;
                }
                if (_equippedDuck.crouch && !_equippedDuck.sliding)
                    _offset.y += 4f;
                collisionOffset = new Vec2(0f, -9999f);
                collisionSize = new Vec2(0f, 0f);
                solid = false;
                PhysicsObject propel = _equippedDuck;
                if (_equippedDuck._trapped != null)
                    propel = _equippedDuck._trapped;
                else if (_equippedDuck.ragdoll != null && _equippedDuck.ragdoll.part1 != null)
                    propel = _equippedDuck.ragdoll.part1;
                _sprite.flipH = _equippedDuck._sprite.flipH;
                if (_on && _heat < 1f)
                {
                    if (_equippedDuck._trapped == null && _equippedDuck.crouch)
                        _equippedDuck.sliding = true;
                    if (isServerForObject)
                        Global.data.jetFuelUsed.valueFloat += Maths.IncFrameTimer();
                    _heat += 0.011f;
                    if (propel is RagdollPart)
                    {
                        ++Global.data.timeJetpackedAsRagdoll;
                        float angle = this.angle;
                        this.angle = propel.angle;
                        Vec2 offset = Offset(new Vec2(0f, 8f));
                        for (float i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(new JetpackSmoke(offset.x, offset.y));
                        this.angle = angle;
                        if (propel.velocity.length < 7f)
                        {
                            RagdollPart ragdollPart = propel as RagdollPart;
                            ragdollPart.addWeight = 0.2f;
                            _equippedDuck.ragdoll.jetting = true;
                            float ang = -(propel.angle - (float)(Math.PI / 2f));
                            Vec2 dir = Vec2.Zero;
                            if (_equippedDuck.inputProfile.leftStick.length > 0.1f)
                            {
                                dir = new Vec2(_equippedDuck.inputProfile.leftStick.x, -_equippedDuck.inputProfile.leftStick.y);
                            }
                            else
                            {
                                dir = new Vec2(0f, 0f);
                                if (_equippedDuck.inputProfile.Down(Triggers.Left))
                                    --dir.x;
                                if (_equippedDuck.inputProfile.Down(Triggers.Right))
                                    ++dir.x;
                                if (_equippedDuck.inputProfile.Down(Triggers.Up))
                                    --dir.y;
                                if (_equippedDuck.inputProfile.Down(Triggers.Down))
                                    ++dir.y;
                            }
                            if (dir.length < 0.1f)
                                dir = new Vec2((float)Math.Cos(ang), (float)-Math.Sin(ang));
                            PhysicsObject physicsObject2 = propel;
                            physicsObject2.velocity += dir * 1.5f;
                            if (ragdollPart.doll != null && ragdollPart.doll.part1 != null && ragdollPart.doll.part2 != null && ragdollPart.doll.part3 != null)
                            {
                                ragdollPart.doll.part1.extraGravMultiplier = 0.4f;
                                ragdollPart.doll.part2.extraGravMultiplier = 0.4f;
                                ragdollPart.doll.part3.extraGravMultiplier = 0.7f;
                            }
                        }
                    }
                    else
                    {
                        for (float i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(new JetpackSmoke(x, y + 8f + smokeOff));
                        if (angle > 0f)
                        {
                            if (propel.hSpeed < 6f)
                                propel.hSpeed += 0.9f;
                        }
                        else if (angle < 0f)
                        {
                            if (propel.hSpeed > -6f)
                                propel.hSpeed -= 0.9f;
                        }
                        else if (propel.vSpeed > -4.5f)
                            propel.vSpeed -= 0.38f;
                    }
                }
                if (_heat >= 1f)
                    _on = false;
                if (!propel.grounded)
                    return;
                if (_heat > 0f)
                    _heat -= 0.25f;
                else
                    _heat = 0f;
            }
            else
            {
                _sprite.flipH = false;
                collisionOffset = new Vec2(-5f, -5f);
                collisionSize = new Vec2(11f, 12f);
                solid = true;
            }
        }
    }
}
