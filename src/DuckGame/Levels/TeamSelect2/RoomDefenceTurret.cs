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
            this.graphic = new Sprite("turretHead");
            this.center = new Vec2(7f, 4f);
            this.collisionSize = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this._friendly = pOwner;
            this.canPickUp = false;
            this.enablePhysics = false;
            this._base = new Sprite("turretBase");
            this._base.CenterOrigin();
            this._barrelOffsetTL = new Vec2(12f, 5f);
            this._ammoType = new ATDefenceLaser();
            this._fireSound = "phaserSmall";
            this._fireSoundPitch = 0.4f;
            this.weight = 10f;
            this._kickForce = 0f;
            this.ammo = 99;
        }

        public override void Update()
        {
            if (this._friendly != null)
            {
                this.ammo = 99;
                if (this.isServerForObject && this._target == null)
                {
                    Duck associatedDuck = Duck.GetAssociatedDuck(this.level.NearestThingFilter<IAmADuck>(this.position, x => (this._friendly == null || Duck.GetAssociatedDuck(x) != this._friendly) && Level.CheckLine<Block>(this.position, x.position) == null && Level.CheckLine<TeamBeam>(this.position, x.position) == null) as PhysicsObject);
                    if (associatedDuck != null && !associatedDuck.dead)
                    {
                        this._target = associatedDuck;
                        this._charge = 1f;
                        SFX.PlaySynchronized("chaingunSpinUp", 0.95f, 0.1f);
                    }
                }
                if (this._target != null)
                {
                    if (this.isServerForObject && (Level.CheckLine<Block>(this.position, this._target.cameraPosition) != null || Level.CheckLine<TeamBeam>(this.position, this._target.cameraPosition) != null))
                    {
                        this.LoseTarget();
                    }
                    else
                    {
                        this.angleDegrees = -Maths.PointDirection(this.position, this._target.cameraPosition);
                        if (this.offDir < 0)
                            this.angleDegrees += 180f;
                        if (this.isServerForObject)
                        {
                            this._charge += Maths.IncFrameTimer();
                            if (_charge > 0.200000002980232)
                            {
                                this.owner = _friendly;
                                this._charge = 0f;
                                this.Fire();
                                this.owner = null;
                                this.enablePhysics = false;
                            }
                            Duck associatedDuck = Duck.GetAssociatedDuck(_target);
                            if (associatedDuck != null && associatedDuck.dead)
                                this.LoseTarget();
                        }
                    }
                }
                else
                    this.angleDegrees = 10 * offDir;
            }
            base.Update();
        }

        private void LoseTarget()
        {
            this._target = null;
            if (_charge > 0.0)
                SFX.PlaySynchronized("chaingunSpinDown", 0.95f, 0.1f);
            this._charge = 0f;
        }

        public override void Draw()
        {
            Vec2 vec2 = this.position + -this.barrelVector * (this.kick * 4f);
            this.graphic.angleDegrees = this.angleDegrees;
            this.graphic.center = this.center;
            this.graphic.flipH = this.offDir < 0;
            Graphics.Draw(this.graphic, vec2.x, vec2.y, this.depth);
            this._base.depth = this.depth - 10;
            Graphics.Draw(this._base, this.position.x, this.position.y - 6f);
        }
    }
}
