// Decompiled with JetBrains decompiler
// Type: DuckGame.YellowBarrel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class YellowBarrel : Holdable, IPlatform, ISequenceItem
    {
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

        public YellowBarrel(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.valid = new EditorProperty<bool>(false, this);
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this.graphic = new Sprite("yellowBarrel");
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite("yellowBarrelMelting");
            this._toreUp = new SpriteMap("yellowBarrelToreUp", 14, 17)
            {
                frame = 1,
                center = new Vec2(0f, -6f)
            };
            this.sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(14f, 16f);
            this.depth = -0.1f;
            this._editorName = "Barrel (Gasoline)";
            this.editorTooltip = "Do not smoke near this barrel. In fact, don't smoke at all. It's not cool, kids!";
            this.thickness = 4f;
            this.weight = 5f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.collideSounds.Add("barrelThud");
            this._holdOffset = new Vec2(1f, 0f);
            this.flammable = 0.3f;
            this._fluid = Fluid.Gas;
            this.sequence.isValid = this.valid.value;
            this._placementCost += 6;
        }

        public override void Initialize()
        {
            this.sequence.isValid = this.valid.value;
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0.0)
                return false;
            hitPos += bullet.travelDirNormalized * 2f;
            if (1.0 - (hitPos.y - this.top) / (this.bottom - this.top) < _fluidLevel)
            {
                this.thickness = 2f;
                this.MakeHole(hitPos, bullet.travelDirNormalized);
                SFX.Play("bulletHitWater", pitch: Rando.Float(-0.2f, 0.2f));
                return base.Hit(bullet, hitPos);
            }
            this.thickness = 1f;
            return base.Hit(bullet, hitPos);
        }

        public void MakeHole(Vec2 pPos, Vec2 pImpaleDirection)
        {
            Vec2 off = pPos - this.position;
            bool flag = false;
            foreach (FluidStream hole in this._holes)
            {
                if ((hole.offset - off).length < 2.0)
                {
                    hole.offset = off;
                    hole.holeThickness += 0.5f;
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            this._holes.Add(new FluidStream(0f, 0f, (-pImpaleDirection).Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos -= bullet.travelDirNormalized * 2f;
            Vec2 off = exitPos - this.position;
            bool flag = false;
            foreach (FluidStream hole in this._holes)
            {
                if ((hole.offset - off).length < 2.0)
                {
                    hole.offset = off;
                    hole.holeThickness += 0.5f;
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            this._holes.Add(new FluidStream(0f, 0f, bullet.travelDirNormalized.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void Update()
        {
            base.Update();
            this.offDir = 1;
            if (_hitPoints <= 0.0)
            {
                if (this.graphic != this._toreUp)
                {
                    float num1 = this._fluidLevel * 0.5f;
                    float num2 = this._fluidLevel * 0.5f;
                    FluidData fluid = this._fluid;
                    fluid.amount = num1 / 20f;
                    for (int index = 0; index < 20; ++index)
                        Level.Add(new Fluid(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f), new Vec2(Rando.Float(-4f, 4f), Rando.Float(-4f, 0f)), fluid));
                    fluid.amount = num2;
                    Level.Add(new Fluid(this.x, this.y - 8f, new Vec2(0f, -1f), fluid));
                    Level.Add(SmallSmoke.New(this.x, this.y));
                    SFX.Play("bulletHitWater");
                    SFX.Play("crateDestroy");
                }
                this.graphic = _toreUp;
                this._onFire = false;
                this.burnt = 0f;
                this._fluidLevel = 0f;
                this._weight = 0.1f;
                this._collisionSize.y = 10f;
                this._collisionOffset.y = -2f;
            }
            this.burnSpeed = 0.0015f;
            if (this._onFire && burnt < 0.899999976158142)
            {
                if (burnt > 0.300000011920929)
                    this.graphic = this._melting;
                this.yscale = (float)(0.5 + (1.0 - burnt) * 0.5);
                this.centery = (float)(8.0 - burnt * 7.0);
                this._collisionOffset.y = (float)(burnt * 7.0 - 8.0);
                this._collisionSize.y = (float)(16.0 - burnt * 7.0);
            }
            if (!this._bottomHoles && burnt > 0.600000023841858)
            {
                this._bottomHoles = true;
                this._holes.Add(new FluidStream(0f, 0f, new Vec2(-1f, -1f), 1f, new Vec2(-7f, 8f))
                {
                    holeThickness = 2f
                });
                this._holes.Add(new FluidStream(0f, 0f, new Vec2(1f, -1f), 1f, new Vec2(7f, 8f))
                {
                    holeThickness = 2f
                });
            }
            if (this._owner != null)
            {
                this.hSpeed = this.owner.hSpeed;
                this.vSpeed = this.owner.vSpeed;
            }
            if (_fluidLevel > 0.0 && this._alternate == 0)
            {
                foreach (FluidStream hole in this._holes)
                {
                    hole.onFire = this.onFire;
                    hole.hSpeed = this.hSpeed;
                    hole.vSpeed = this.vSpeed;
                    hole.DoUpdate();
                    hole.position = this.Offset(hole.offset);
                    hole.sprayAngle = this.OffsetLocal(hole.startSprayAngle);
                    float num3 = (float)(1.0 - (hole.offset.y - this.topLocal) / (this.bottomLocal - this.topLocal));
                    if (hole.x > this.left - 2.0 && hole.x < this.right + 2.0 && num3 < _fluidLevel)
                    {
                        float num4 = Maths.Clamp(this._fluidLevel - num3, 0.1f, 1f);
                        FluidData fluid = this._fluid;
                        float num5 = num4 * 0.008f * hole.holeThickness;
                        fluid.amount = num5;
                        hole.Feed(fluid);
                        this._fluidLevel -= num5;
                        this._lossAccum += num5;
                        while (_lossAccum > 0.0500000007450581)
                        {
                            this._lossAccum -= 0.05f;
                            if (this.sequence != null && this.sequence.isValid && ChallengeLevel.running)
                            {
                                ++ChallengeLevel.goodiesGot;
                                SFX.Play("tinyTick");
                            }
                        }
                    }
                }
            }
            this.weight = this._fluidLevel * 10f;
            ++this._alternate;
            if (this._alternate <= 4)
                return;
            this._alternate = 0;
        }

        public override void Draw()
        {
            float num1 = 1f - this._fluidLevel;
            float num2 = (float)(0.600000023841858 + (1.0 - burnt) * 0.400000005960464);
            this.graphic.color = new Color((byte)(150.0 * num2), (byte)(150.0 * num2), (byte)(150.0 * num2));
            base.Draw();
            if (_hitPoints <= 0.0)
                return;
            this.graphic.color = new Color((byte)(byte.MaxValue * num2), (byte)(byte.MaxValue * num2), (byte)(byte.MaxValue * num2));
            this.graphic.angle = this.angle;
            this.graphic.depth = this.depth + 1;
            this.graphic.scale = this.scale;
            float y = num1 * graphic.height;
            this.graphic.center = this.center - new Vec2(0f, (int)y);
            Graphics.Draw(this.graphic, this.x, this.y, new Rectangle(0f, (int)y, graphic.w, (int)(graphic.h - y)));
        }
    }
}
