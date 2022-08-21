// Decompiled with JetBrains decompiler
// Type: DuckGame.YellowBarrel
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            valid = new EditorProperty<bool>(false, this);
            _maxHealth = 15f;
            _hitPoints = 15f;
            graphic = new Sprite("yellowBarrel");
            center = new Vec2(7f, 8f);
            _melting = new Sprite("yellowBarrelMelting");
            _toreUp = new SpriteMap("yellowBarrelToreUp", 14, 17)
            {
                frame = 1,
                center = new Vec2(0f, -6f)
            };
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            collisionOffset = new Vec2(-7f, -8f);
            collisionSize = new Vec2(14f, 16f);
            depth = -0.1f;
            _editorName = "Barrel (Gasoline)";
            editorTooltip = "Do not smoke near this barrel. In fact, don't smoke at all. It's not cool, kids!";
            thickness = 4f;
            weight = 5f;
            physicsMaterial = PhysicsMaterial.Metal;
            collideSounds.Add("barrelThud");
            _holdOffset = new Vec2(1f, 0f);
            flammable = 0.3f;
            _fluid = Fluid.Gas;
            sequence.isValid = valid.value;
            _placementCost += 6;
        }

        public override void Initialize()
        {
            sequence.isValid = valid.value;
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0.0)
                return false;
            hitPos += bullet.travelDirNormalized * 2f;
            if (1.0 - (hitPos.y - top) / (bottom - top) < _fluidLevel)
            {
                thickness = 2f;
                MakeHole(hitPos, bullet.travelDirNormalized);
                SFX.Play("bulletHitWater", pitch: Rando.Float(-0.2f, 0.2f));
                return base.Hit(bullet, hitPos);
            }
            thickness = 1f;
            return base.Hit(bullet, hitPos);
        }

        public void MakeHole(Vec2 pPos, Vec2 pImpaleDirection)
        {
            Vec2 off = pPos - position;
            bool flag = false;
            foreach (FluidStream hole in _holes)
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
            _holes.Add(new FluidStream(0f, 0f, (-pImpaleDirection).Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos -= bullet.travelDirNormalized * 2f;
            Vec2 off = exitPos - position;
            bool flag = false;
            foreach (FluidStream hole in _holes)
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
            _holes.Add(new FluidStream(0f, 0f, bullet.travelDirNormalized.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void Update()
        {
            base.Update();
            offDir = 1;
            if (_hitPoints <= 0.0)
            {
                if (graphic != _toreUp)
                {
                    float num1 = _fluidLevel * 0.5f;
                    float num2 = _fluidLevel * 0.5f;
                    FluidData fluid = _fluid;
                    fluid.amount = num1 / 20f;
                    for (int index = 0; index < 20; ++index)
                        Level.Add(new Fluid(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f), new Vec2(Rando.Float(-4f, 4f), Rando.Float(-4f, 0f)), fluid));
                    fluid.amount = num2;
                    Level.Add(new Fluid(x, y - 8f, new Vec2(0f, -1f), fluid));
                    Level.Add(SmallSmoke.New(x, y));
                    SFX.Play("bulletHitWater");
                    SFX.Play("crateDestroy");
                }
                graphic = _toreUp;
                _onFire = false;
                burnt = 0f;
                _fluidLevel = 0f;
                _weight = 0.1f;
                _collisionSize.y = 10f;
                _collisionOffset.y = -2f;
            }
            burnSpeed = 0.0015f;
            if (_onFire && burnt < 0.9f)
            {
                if (burnt > 0.3f)
                    graphic = _melting;
                yscale = (float)(0.5 + (1.0 - burnt) * 0.5);
                centery = (float)(8.0 - burnt * 7.0);
                _collisionOffset.y = (float)(burnt * 7.0 - 8.0);
                _collisionSize.y = (float)(16.0 - burnt * 7.0);
            }
            if (!_bottomHoles && burnt > 0.6f)
            {
                _bottomHoles = true;
                _holes.Add(new FluidStream(0f, 0f, new Vec2(-1f, -1f), 1f, new Vec2(-7f, 8f))
                {
                    holeThickness = 2f
                });
                _holes.Add(new FluidStream(0f, 0f, new Vec2(1f, -1f), 1f, new Vec2(7f, 8f))
                {
                    holeThickness = 2f
                });
            }
            if (_owner != null)
            {
                hSpeed = owner.hSpeed;
                vSpeed = owner.vSpeed;
            }
            if (_fluidLevel > 0.0 && _alternate == 0)
            {
                foreach (FluidStream hole in _holes)
                {
                    hole.onFire = onFire;
                    hole.hSpeed = hSpeed;
                    hole.vSpeed = vSpeed;
                    hole.DoUpdate();
                    hole.position = Offset(hole.offset);
                    hole.sprayAngle = OffsetLocal(hole.startSprayAngle);
                    float num3 = (float)(1.0 - (hole.offset.y - topLocal) / (bottomLocal - topLocal));
                    if (hole.x > left - 2.0 && hole.x < right + 2.0 && num3 < _fluidLevel)
                    {
                        float num4 = Maths.Clamp(_fluidLevel - num3, 0.1f, 1f);
                        FluidData fluid = _fluid;
                        float num5 = num4 * 0.008f * hole.holeThickness;
                        fluid.amount = num5;
                        hole.Feed(fluid);
                        _fluidLevel -= num5;
                        _lossAccum += num5;
                        while (_lossAccum > 0.05f)
                        {
                            _lossAccum -= 0.05f;
                            if (sequence != null && sequence.isValid && ChallengeLevel.running)
                            {
                                ++ChallengeLevel.goodiesGot;
                                SFX.Play("tinyTick");
                            }
                        }
                    }
                }
            }
            weight = _fluidLevel * 10f;
            ++_alternate;
            if (_alternate <= 4)
                return;
            _alternate = 0;
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
<<<<<<< Updated upstream
                float ypos = level * (float)this.graphic.height;
                this.graphic.center = this.center - new Vec2(0f, (float)((int)ypos));
                Graphics.Draw(this.graphic, base.x, base.y, new Rectangle(0f, (float)((int)ypos), (float)this.graphic.w, (float)((int)((float)this.graphic.h - ypos))));
=======
                float ypos = level * graphic.height;
                this.graphic.center = this.center - new Vec2(0f, (int)ypos);
                Graphics.Draw(this.graphic, base.x, base.y, new Rectangle(0f, (int)ypos, graphic.w, (int)(graphic.h - ypos)));
>>>>>>> Stashed changes
            }
        }
    }
}
