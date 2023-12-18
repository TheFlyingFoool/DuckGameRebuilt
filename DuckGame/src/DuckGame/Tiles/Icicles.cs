namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("previewPriority", true)]
    public class Icicles : MaterialThing
    {
        public StateBinding _deadlyIcicleInstanceBinding = new StateBinding(nameof(_deadlyIcicleInstance));
        public StateBinding _frameBinding = new StateBinding(nameof(frame));
        private Thing _deadlyIcicleInstance;
        public EditorProperty<int> style;
        public EditorProperty<bool> background;
        public bool kill;

        public new int frame
        {
            get => (graphic as SpriteMap).frame;
            set => (graphic as SpriteMap).frame = value;
        }

        public override void EditorPropertyChanged(object property)
        {
            (graphic as SpriteMap).frame = style.value;
            if ((int)style == 3)
                collisionSize = new Vec2(10f, 18f);
            else
                collisionSize = new Vec2(10f, 8f);
            if (background.value)
                depth = -0.1f;
            else
                depth = (Depth)0.1f;
        }

        public Icicles(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            style = new EditorProperty<int>(0, this, max: 3f, increment: 1f);
            background = new EditorProperty<bool>(false, this);
            graphic = new SpriteMap("icicles", 16, 21);
            hugWalls = WallHug.Ceiling;
            center = new Vec2(8f, 5f);
            collisionSize = new Vec2(10f, 8f);
            collisionOffset = new Vec2(-5f, -3f);
            thickness = 0.1f;
            physicsMaterial = PhysicsMaterial.Glass;
            layer = Layer.Blocks;
            depth = (Depth)0.1f;
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                if (background.value)
                {
                    depth = -0.8f;
                    layer = Layer.Game;
                }
                else
                {
                    depth = (Depth)0.1f;
                    layer = Layer.Blocks;
                }
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (Network.isActive)
            {
                if ((graphic as SpriteMap).frame == 3 && _deadlyIcicleInstance == null && Network.isServer)
                {
                    _deadlyIcicleInstance = new DeadlyIcicle(x, y + 8f)
                    {
                        active = false,
                        visible = false,
                        solid = false
                    };
                    Level.Add(_deadlyIcicleInstance);
                }
                if ((graphic as SpriteMap).frame == 3 && _deadlyIcicleInstance != null && _deadlyIcicleInstance.visible)
                    (graphic as SpriteMap).frame = 7;
                else if ((graphic as SpriteMap).frame == 7 && (_deadlyIcicleInstance == null || !_deadlyIcicleInstance.visible))
                    (graphic as SpriteMap).frame = 3;
            }
            if ((graphic as SpriteMap).frame == 3)
                thickness = 4f;
            else
                thickness = 0.01f;
            base.Update();
        }

        public override bool Hurt(float points)
        {
            if (isServerForObject)
                Break(isServerForObject);
            return true;
        }

        public override void HeatUp(Vec2 location)
        {
            if (isServerForObject)
                Break(isServerForObject);
            base.HeatUp(location);
        }

        private void Break(bool pLocal)
        {
            if ((graphic as SpriteMap).frame >= 4)
                return;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(x + Rando.Int(-3, 3), y + Rando.Int(-3, 3), Vec2.Zero);
                Level.Add(glassParticle);
                glassParticle.hSpeed = Rando.Float(-1f, 1f);
                glassParticle.vSpeed = Rando.Float(-1f, 1f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            if ((graphic as SpriteMap).frame == 3 & pLocal)
            {
                if (Network.isActive)
                {
                    if (_deadlyIcicleInstance != null)
                    {
                        _deadlyIcicleInstance.visible = true;
                        _deadlyIcicleInstance.active = true;
                        _deadlyIcicleInstance.solid = true;
                        Fondle(this, DuckNetwork.localConnection);
                        Fondle(_deadlyIcicleInstance, DuckNetwork.localConnection);
                    }
                }
                else
                    Level.Add(new DeadlyIcicle(x, y + 8f));
            }
          (graphic as SpriteMap).frame += 4;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Break(bullet.isLocal);
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (isServerForObject)
                Break(true);
            return false;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.impactPowerV > 2 && (graphic as SpriteMap).frame != 3)
            {
                Break(with.isLocal);
            }
            else
            {
                switch (with)
                {
                    case Net _:
                    case Dart _:
                    case CampingBall _:
                    case Sword _:
                    case EnergyScimitar _:
                        Break(with.isLocal);
                        break;
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            base.Draw();
        }
    }
}
