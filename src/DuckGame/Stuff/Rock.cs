// Decompiled with JetBrains decompiler
// Type: DuckGame.Rock
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isInDemo", true)]
    public class Rock : Holdable, IPlatform
    {
        public StateBinding _isGoldBinding = new StateBinding(nameof(isGoldRock));
        public EditorProperty<bool> gold = new EditorProperty<bool>(false);
        private SpriteMap _sprite;
        public bool isGoldRock;
        private bool _changedCollision;
        private bool _didKill;
        private int _killWait;
        private Sound _winSound;

        public Rock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("rock01", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -5f);
            this.collisionSize = new Vec2(16f, 12f);
            this.depth = -0.5f;
            this.thickness = 4f;
            this.weight = 7f;
            this.flammable = 0.0f;
            this.collideSounds.Add("rockHitGround2");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorTooltip = "Don’t throw rocks!";
            this.holsterAngle = 90f;
        }

        public override void Initialize()
        {
            if (this.gold.value)
            {
                this.material = new MaterialGold(this);
                this.isGoldRock = true;
            }
            base.Initialize();
        }

        public override void EditorRender()
        {
            if (this.gold.value)
            {
                if (this.material == null)
                {
                    this.material = new MaterialGold(this);
                    this.isGoldRock = true;
                }
            }
            else
            {
                this.material = null;
                this.isGoldRock = false;
            }
            base.EditorRender();
        }

        [NetworkAction]
        private void StartRockSong()
        {
            if (this._winSound != null)
                this._winSound.Stop();
            this._killWait = 0;
            this._didKill = true;
            this._winSound = SFX.Play("winzone");
        }

        [NetworkAction]
        private void StopRockSong()
        {
            if (this._winSound != null)
                this._winSound.Stop();
            this._killWait = 0;
            this._didKill = false;
            this._winSound = null;
        }

        public override void Update()
        {
            if (this.isGoldRock && !(this.material is MaterialGold))
                this.material = new MaterialGold(this);
            if (this.isServerForObject)
            {
                if (this.isGoldRock)
                {
                    if (this.duck != null && !this._didKill)
                        this.SyncNetworkAction(new PhysicsObject.NetAction(this.StartRockSong));
                    if (this.duck == null)
                    {
                        this._didKill = false;
                        this._killWait = 0;
                        if (this._winSound != null)
                            this.SyncNetworkAction(new PhysicsObject.NetAction(this.StopRockSong));
                    }
                }
                if (this._didKill && this.duck != null)
                {
                    ++this._killWait;
                    if (this._killWait == 108)
                    {
                        foreach (Duck duck in Level.current.things[typeof(Duck)])
                        {
                            if (duck != null && duck.team != this.duck.team)
                                duck.Kill(new DTCrush(this));
                        }
                    }
                }
            }
            if (this.duck == null)
            {
                this._didKill = false;
                this._killWait = 0;
            }
            if (this.raised)
            {
                if (!this._changedCollision)
                {
                    this.collisionSize = new Vec2(this.collisionSize.y, this.collisionSize.x);
                    this.collisionOffset = new Vec2(this.collisionOffset.y, this.collisionOffset.x);
                    this._changedCollision = true;
                }
            }
            else if (this._changedCollision)
            {
                this.collisionSize = new Vec2(this.collisionSize.y, this.collisionSize.x);
                this.collisionOffset = new Vec2(this.collisionOffset.y, this.collisionOffset.x);
                this._changedCollision = false;
            }
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (this.isServerForObject && bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                if (this.duck != null)
                    this.duck.ThrowItem();
                this.Destroy(new DTShot(bullet));
                Level.Remove(this);
                Level.Add(new GrenadeExplosion(this.x, this.y));
            }
            return base.Hit(bullet, hitPos);
        }
    }
}
