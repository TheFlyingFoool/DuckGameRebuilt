using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class YellowBarrel : Holdable, IPlatform, ISequenceItem
    {
        public YellowBarrel(float xpos, float ypos) : base(xpos, ypos)
        {
            this.valid = new EditorProperty<bool>(false, this, 0f, 1f, 0.1f, null, false, false);
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this.graphic = new Sprite("yellowBarrel", 0f, 0f);
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite("yellowBarrelMelting", 0f, 0f);
            this._toreUp = new SpriteMap("yellowBarrelToreUp", 14, 17, false);
            this._toreUp.frame = 1;
            this._toreUp.center = new Vec2(0f, -6f);
            base.sequence = new SequenceItem(this);
            base.sequence.type = SequenceItemType.Goody;
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(14f, 16f);
            base.depth = -0.1f;
            this._editorName = "Barrel (Gasoline)";
            this.editorTooltip = "Do not smoke near this barrel. In fact, don't smoke at all. It's not cool, kids!";
            this.thickness = 4f;
            this.weight = 5f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            base.collideSounds.Add("barrelThud");
            this._holdOffset = new Vec2(1f, 0f);
            this.flammable = 0.3f;
            this._fluid = Fluid.Gas;
            base.sequence.isValid = this.valid.value;
            this._placementCost += 6;
        }

        public override void Initialize()
        {
            base.sequence.isValid = this.valid.value;
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._hitPoints <= 0f)
            {
                return false;
            }
            hitPos += bullet.travelDirNormalized * 2f;
            if (1f - (hitPos.y - base.top) / (base.bottom - base.top) < this._fluidLevel)
            {
                this.thickness = 2f;
                this.MakeHole(hitPos, bullet.travelDirNormalized);
                SFX.Play("bulletHitWater", 1f, Rando.Float(-0.2f, 0.2f), 0f, false);
                return base.Hit(bullet, hitPos);
            }
            this.thickness = 1f;
            return base.Hit(bullet, hitPos);
        }

        public void MakeHole(Vec2 pPos, Vec2 pImpaleDirection)
        {
            Vec2 offset = pPos - this.position;
            bool found = false;
            foreach (FluidStream hole in this._holes)
            {
                if ((hole.offset - offset).length < 2f)
                {
                    hole.offset = offset;
                    hole.holeThickness += 0.5f;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Vec2 holeVec = (-pImpaleDirection).Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero);
                this._holes.Add(new FluidStream(0f, 0f, holeVec, 1f, offset));
            }
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos -= bullet.travelDirNormalized * 2f;
            Vec2 offset = exitPos - this.position;
            bool found = false;
            foreach (FluidStream hole in this._holes)
            {
                if ((hole.offset - offset).length < 2f)
                {
                    hole.offset = offset;
                    hole.holeThickness += 0.5f;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Vec2 holeVec = bullet.travelDirNormalized;
                holeVec = holeVec.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero);
                this._holes.Add(new FluidStream(0f, 0f, holeVec, 1f, offset));
            }
        }

        public override void Update()
        {
            base.Update();
            this.offDir = 1;
            if (this._hitPoints <= 0f)
            {
                if (this.graphic != this._toreUp)
                {
                    float spray = this._fluidLevel * 0.5f;
                    float glob = this._fluidLevel * 0.5f;
                    FluidData dat = this._fluid;
                    dat.amount = spray / 20f;
                    for (int i = 0; i < 20; i++)
                    {
                        Level.Add(new Fluid(base.x + Rando.Float(-4f, 4f), base.y + Rando.Float(-4f, 4f), new Vec2(Rando.Float(-4f, 4f), Rando.Float(-4f, 0f)), dat, null, 1f));
                    }
                    dat.amount = glob;
                    Level.Add(new Fluid(base.x, base.y - 8f, new Vec2(0f, -1f), dat, null, 1f));
                    if (DGRSettings.S_ParticleMultiplier != 0) Level.Add(SmallSmoke.New(base.x, base.y));
                    SFX.Play("bulletHitWater", 1f, 0f, 0f, false);
                    SFX.Play("crateDestroy", 1f, 0f, 0f, false);
                }
                this.graphic = this._toreUp;
                this._onFire = false;
                this.burnt = 0f;
                this._fluidLevel = 0f;
                this._weight = 0.1f;
                this._collisionSize.y = 10f;
                this._collisionOffset.y = -2f;
            }
            this.burnSpeed = 0.0015f;
            if (this._onFire && this.burnt < 0.9f)
            {
                if (this.burnt > 0.3f)
                {
                    this.graphic = this._melting;
                }
                base.yscale = 0.5f + (1f - this.burnt) * 0.5f;
                base.centery = 8f - this.burnt * 7f;
                this._collisionOffset.y = -8f + this.burnt * 7f;
                this._collisionSize.y = 16f - this.burnt * 7f;
            }
            if (!this._bottomHoles && this.burnt > 0.6f)
            {
                this._bottomHoles = true;
                FluidStream hole = new FluidStream(0f, 0f, new Vec2(-1f, -1f), 1f, new Vec2(-7f, 8f));
                hole.holeThickness = 2f;
                this._holes.Add(hole);
                hole = new FluidStream(0f, 0f, new Vec2(1f, -1f), 1f, new Vec2(7f, 8f));
                hole.holeThickness = 2f;
                this._holes.Add(hole);
            }
            if (this._owner != null)
            {
                this.hSpeed = this.owner.hSpeed;
                this.vSpeed = this.owner.vSpeed;
            }
            if (this._fluidLevel > 0f && this._alternate == 0)
            {
                foreach (FluidStream hole2 in this._holes)
                {
                    hole2.onFire = base.onFire;
                    hole2.hSpeed = this.hSpeed;
                    hole2.vSpeed = this.vSpeed;
                    hole2.DoUpdate();
                    hole2.position = this.Offset(hole2.offset);
                    hole2.sprayAngle = this.OffsetLocal(hole2.startSprayAngle);
                    float level = 1f - (hole2.offset.y - base.topLocal) / (base.bottomLocal - base.topLocal);
                    if (hole2.x > base.left - 2f && hole2.x < base.right + 2f && level < this._fluidLevel)
                    {
                        level = Maths.Clamp(this._fluidLevel - level, 0.1f, 1f);
                        FluidData f = this._fluid;
                        float loss = level * 0.008f * hole2.holeThickness;
                        f.amount = loss;
                        hole2.Feed(f);
                        this._fluidLevel -= loss;
                        this._lossAccum += loss;
                        while (this._lossAccum > 0.05f)
                        {
                            this._lossAccum -= 0.05f;
                            if (base.sequence != null && base.sequence.isValid && ChallengeLevel.running)
                            {
                                ChallengeLevel.goodiesGot++;
                                SFX.Play("tinyTick", 1f, 0f, 0f, false);
                            }
                        }
                    }
                }
            }
            this.weight = this._fluidLevel * 10f;
            this._alternate++;
            if (this._alternate > 4)
            {
                this._alternate = 0;
            }
        }

        public override void Draw()
        {
            float level = 1f - this._fluidLevel;
            float darken = 0.6f + (1f - this.burnt) * 0.4f;
            this.graphic.color = new Color((byte)(150f * darken), (byte)(150f * darken), (byte)(150f * darken));
            base.Draw();
            if (this._hitPoints > 0f)
            {
                this.graphic.color = new Color((byte)(255f * darken), (byte)(255f * darken), (byte)(255f * darken));
                this.graphic.angle = this.angle;
                this.graphic.depth = base.depth + 1;
                this.graphic.scale = base.scale;
                float ypos = level * graphic.height;
                this.graphic.center = this.center - new Vec2(0f, (int)ypos);
                Graphics.Draw(this.graphic, base.x, base.y, new Rectangle(0f, (int)ypos, graphic.w, (int)(graphic.h - ypos)));
            }
        }

        public EditorProperty<bool> valid;

        protected float damageMultiplier = 1f;

        protected SpriteMap _sprite;

        protected float _fluidLevel = 1f;

        protected int _alternate;

        private List<FluidStream> _holes = new List<FluidStream>();

        protected FluidData _fluid;

        protected Sprite _melting;

        protected SpriteMap _toreUp;

        private bool _bottomHoles;

        private float _lossAccum;
    }
}
