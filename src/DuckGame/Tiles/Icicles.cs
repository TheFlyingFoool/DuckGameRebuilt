// Decompiled with JetBrains decompiler
// Type: DuckGame.Icicles
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => (this.graphic as SpriteMap).frame;
            set => (this.graphic as SpriteMap).frame = value;
        }

        public override void EditorPropertyChanged(object property)
        {
            (this.graphic as SpriteMap).frame = this.style.value;
            if ((int)this.style == 3)
                this.collisionSize = new Vec2(10f, 18f);
            else
                this.collisionSize = new Vec2(10f, 8f);
            if (this.background.value)
                this.depth = -0.1f;
            else
                this.depth = (Depth)0.1f;
        }

        public Icicles(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            this.style = new EditorProperty<int>(0, this, max: 3f, increment: 1f);
            this.background = new EditorProperty<bool>(false, this);
            this.graphic = new SpriteMap("icicles", 16, 21);
            this.hugWalls = WallHug.Ceiling;
            this.center = new Vec2(8f, 5f);
            this.collisionSize = new Vec2(10f, 8f);
            this.collisionOffset = new Vec2(-5f, -3f);
            this.thickness = 0.1f;
            this.physicsMaterial = PhysicsMaterial.Glass;
            this.layer = Layer.Blocks;
            this.depth = (Depth)0.1f;
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                if (this.background.value)
                {
                    this.depth = -0.8f;
                    this.layer = Layer.Game;
                }
                else
                {
                    this.depth = (Depth)0.1f;
                    this.layer = Layer.Blocks;
                }
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (Network.isActive)
            {
                if ((this.graphic as SpriteMap).frame == 3 && this._deadlyIcicleInstance == null && Network.isServer)
                {
                    this._deadlyIcicleInstance = new DeadlyIcicle(this.x, this.y + 8f);
                    this._deadlyIcicleInstance.active = false;
                    this._deadlyIcicleInstance.visible = false;
                    this._deadlyIcicleInstance.solid = false;
                    Level.Add(this._deadlyIcicleInstance);
                }
                if ((this.graphic as SpriteMap).frame == 3 && this._deadlyIcicleInstance != null && this._deadlyIcicleInstance.visible)
                    (this.graphic as SpriteMap).frame = 7;
                else if ((this.graphic as SpriteMap).frame == 7 && (this._deadlyIcicleInstance == null || !this._deadlyIcicleInstance.visible))
                    (this.graphic as SpriteMap).frame = 3;
            }
            if ((this.graphic as SpriteMap).frame == 3)
                this.thickness = 4f;
            else
                this.thickness = 0.01f;
            base.Update();
        }

        public override bool Hurt(float points)
        {
            if (this.isServerForObject)
                this.Break(this.isServerForObject);
            return true;
        }

        public override void HeatUp(Vec2 location)
        {
            if (this.isServerForObject)
                this.Break(this.isServerForObject);
            base.HeatUp(location);
        }

        private void Break(bool pLocal)
        {
            if ((this.graphic as SpriteMap).frame >= 4)
                return;
            for (int index = 0; index < 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(this.x + Rando.Int(-3, 3), this.y + Rando.Int(-3, 3), Vec2.Zero);
                Level.Add(glassParticle);
                glassParticle.hSpeed = Rando.Float(-1f, 1f);
                glassParticle.vSpeed = Rando.Float(-1f, 1f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            if ((this.graphic as SpriteMap).frame == 3 & pLocal)
            {
                if (Network.isActive)
                {
                    if (this._deadlyIcicleInstance != null)
                    {
                        this._deadlyIcicleInstance.visible = true;
                        this._deadlyIcicleInstance.active = true;
                        this._deadlyIcicleInstance.solid = true;
                        Thing.Fondle(this, DuckNetwork.localConnection);
                        Thing.Fondle(this._deadlyIcicleInstance, DuckNetwork.localConnection);
                    }
                }
                else
                    Level.Add(new DeadlyIcicle(this.x, this.y + 8f));
            }
          (this.graphic as SpriteMap).frame += 4;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            this.Break(bullet.isLocal);
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this.isServerForObject)
                this.Break(true);
            return false;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.impactPowerV > 2.0 && (this.graphic as SpriteMap).frame != 3)
            {
                this.Break(with.isLocal);
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
                        this.Break(with.isLocal);
                        break;
                }
            }
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            this.graphic.flipH = this.flipHorizontal;
            base.Draw();
        }
    }
}
