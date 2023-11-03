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
            _sprite = new SpriteMap("rock01", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -5f);
            collisionSize = new Vec2(16f, 12f);
            depth = -0.5f;
            thickness = 4f;
            weight = 7f;
            flammable = 0f;
            collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Metal;
            editorTooltip = "Don't throw rocks!";
            holsterAngle = 90f;
        }

        public override void Initialize()
        {
            if (gold.value)
            {
                material = new MaterialGold(this);
                isGoldRock = true;
            }
            base.Initialize();
        }

        public override void EditorRender()
        {
            if (gold.value)
            {
                if (material == null)
                {
                    material = new MaterialGold(this);
                    isGoldRock = true;
                }
            }
            else
            {
                material = null;
                isGoldRock = false;
            }
            base.EditorRender();
        }

        [NetworkAction]
        private void StartRockSong()
        {
            if (_winSound != null)
                _winSound.Stop();
            _killWait = 0;
            _didKill = true;
            _winSound = SFX.Play("winzone");
        }

        [NetworkAction]
        private void StopRockSong()
        {
            if (_winSound != null)
                _winSound.Stop();
            _killWait = 0;
            _didKill = false;
            _winSound = null;
        }

        public override void Update()
        {
            if (isGoldRock && !(material is MaterialGold))
            {
                material = new MaterialGold(this);
            }
            if (isServerForObject)
            {
                if (isGoldRock)
                {
                    if (duck != null && !_didKill)
                        SyncNetworkAction(new NetAction(StartRockSong));
                    if (duck == null)
                    {
                        _didKill = false;
                        _killWait = 0;
                        if (_winSound != null)
                            SyncNetworkAction(new NetAction(StopRockSong));
                    }
                }
                if (_didKill && duck != null)
                {
                    ++_killWait;
                    if (_killWait == 108)
                    {
                        foreach (Duck duck in Level.current.things[typeof(Duck)])
                        {
                            if (duck != null && duck.team != this.duck.team)
                                duck.Kill(new DTCrush(this));
                        }
                    }
                }
            }
            if (duck == null)
            {
                _didKill = false;
                _killWait = 0;
            }
            if (raised)
            {
                if (!_changedCollision)
                {
                    collisionSize = new Vec2(collisionSize.y, collisionSize.x);
                    collisionOffset = new Vec2(collisionOffset.y, collisionOffset.x);
                    _changedCollision = true;
                }
            }
            else if (_changedCollision)
            {
                collisionSize = new Vec2(collisionSize.y, collisionSize.x);
                collisionOffset = new Vec2(collisionOffset.y, collisionOffset.x);
                _changedCollision = false;
            }
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            if (isServerForObject && bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                if (duck != null)
                    duck.ThrowItem();
                Destroy(new DTShot(bullet));
                Level.Remove(this);
                Level.Add(new GrenadeExplosion(x, y));
            }
            return base.Hit(bullet, hitPos);
        }
    }
}
