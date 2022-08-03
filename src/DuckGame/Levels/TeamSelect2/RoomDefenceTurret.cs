// Decompiled with JetBrains decompiler
// Type: DuckGame.RoomDefenceTurret
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RoomDefenceTurret : Gun
    {
        public StateBinding _targetBinding = new StateBinding(nameof(_target));
        public StateBinding _friendlyBinding = new StateBinding(nameof(_friendly));
        private Duck _target;
        public Duck _friendly;
        private float _charge;
        private Sprite _base;

        public RoomDefenceTurret(Vec2 pPosition, Duck pOwner)
          : base(pPosition.x, pPosition.y)
        {
            graphic = new Sprite("turretHead");
            center = new Vec2(7f, 4f);
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            _friendly = pOwner;
            canPickUp = false;
            enablePhysics = false;
            _base = new Sprite("turretBase");
            _base.CenterOrigin();
            _barrelOffsetTL = new Vec2(12f, 5f);
            _ammoType = new ATDefenceLaser();
            _fireSound = "phaserSmall";
            _fireSoundPitch = 0.4f;
            weight = 10f;
            _kickForce = 0f;
            ammo = 99;
        }

        public override void Update()
        {
            if (_friendly != null)
            {
                ammo = 99;
                if (isServerForObject && _target == null)
                {
                    Duck associatedDuck = Duck.GetAssociatedDuck(level.NearestThingFilter<IAmADuck>(position, x => (_friendly == null || Duck.GetAssociatedDuck(x) != _friendly) && Level.CheckLine<Block>(position, x.position) == null && Level.CheckLine<TeamBeam>(position, x.position) == null) as PhysicsObject);
                    if (associatedDuck != null && !associatedDuck.dead)
                    {
                        _target = associatedDuck;
                        _charge = 1f;
                        SFX.PlaySynchronized("chaingunSpinUp", 0.95f, 0.1f);
                    }
                }
                if (_target != null)
                {
                    if (isServerForObject && (Level.CheckLine<Block>(position, _target.cameraPosition) != null || Level.CheckLine<TeamBeam>(position, _target.cameraPosition) != null))
                    {
                        LoseTarget();
                    }
                    else
                    {
                        angleDegrees = -Maths.PointDirection(position, _target.cameraPosition);
                        if (offDir < 0)
                            angleDegrees += 180f;
                        if (isServerForObject)
                        {
                            _charge += Maths.IncFrameTimer();
                            if (_charge > 0.200000002980232)
                            {
                                owner = _friendly;
                                _charge = 0f;
                                Fire();
                                owner = null;
                                enablePhysics = false;
                            }
                            Duck associatedDuck = Duck.GetAssociatedDuck(_target);
                            if (associatedDuck != null && associatedDuck.dead)
                                LoseTarget();
                        }
                    }
                }
                else
                    angleDegrees = 10 * offDir;
            }
            base.Update();
        }

        private void LoseTarget()
        {
            _target = null;
            if (_charge > 0.0)
                SFX.PlaySynchronized("chaingunSpinDown", 0.95f, 0.1f);
            _charge = 0f;
        }

        public override void Draw()
        {
            Vec2 vec2 = position + -barrelVector * (kick * 4f);
            graphic.angleDegrees = angleDegrees;
            graphic.center = center;
            graphic.flipH = offDir < 0;
            Graphics.Draw(graphic, vec2.x, vec2.y, depth);
            _base.depth = depth - 10;
            Graphics.Draw(_base, position.x, position.y - 6f);
        }
    }
}
