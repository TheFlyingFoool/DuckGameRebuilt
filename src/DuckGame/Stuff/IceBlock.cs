// Decompiled with JetBrains decompiler
// Type: DuckGame.IceBlock
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isInDemo", false)]
    public class IceBlock : Holdable, IPlatform
    {
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public StateBinding _carvedBinding = new StateBinding(nameof(carved));
        public float carved;
        public bool didCarve;
        private SpriteMap _sprite;
        private System.Type _previewType;
        private Sprite _previewSprite;
        private float breakPoints = 15f;
        private Thing _containedThing;
        private float damageMultiplier;
        private MaterialFrozen _frozen;

        public System.Type contains { get; set; }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            EditorGroupMenu editorGroupMenu = new EditorGroupMenu(contextMenu);
            editorGroupMenu.InitializeGroups(new EditorGroup(typeof(Holdable)), radioBinding);
            editorGroupMenu.text = "Contains";
            contextMenu.AddItem(editorGroupMenu);
            return contextMenu;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(this.contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override void EditorUpdate()
        {
            if (this.contains != null && (this._previewSprite == null || this._previewType != this.contains))
            {
                Thing thing = Editor.GetThing(this.contains);
                if (thing != null)
                {
                    this._previewSprite = thing.GeneratePreview(48, 48, true);
                    this._previewSprite.CenterOrigin();
                }
                this._previewType = this.contains;
            }
            base.EditorUpdate();
        }

        public override void EditorRender()
        {
            if (this.contains != null && this._previewSprite != null)
            {
                if (this._frozen == null)
                    this._frozen = new MaterialFrozen(this)
                    {
                        intensity = 1f
                    };
                Material material = DuckGame.Graphics.material;
                DuckGame.Graphics.material = _frozen;
                this._previewSprite.alpha = 0.5f;
                DuckGame.Graphics.Draw(this._previewSprite, this.x, this.y, this.depth + 10);
                DuckGame.Graphics.material = material;
            }
            base.EditorRender();
        }

        public IceBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("iceBlock", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = -0.5f;
            this._editorName = "Ice Block";
            this.editorTooltip = "Slippery, slidery, fun. Also great for keeping your (gigantic) drinks cold.";
            this.thickness = 2f;
            this.weight = 5f;
            this.buoyancy = 1f;
            this._hitPoints = 1f;
            this.impactThreshold = -1f;
            this.physicsMaterial = PhysicsMaterial.Glass;
            this._holdOffset = new Vec2(2f, 0.0f);
            this.flammable = 0.0f;
            this.collideSounds.Add("glassHit");
            this.superNonFlammable = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Network.isActive)
                return;
            this.UpdateContainedThing();
        }

        private void UpdateContainedThing()
        {
            if (!(this.contains != null) || Level.current is Editor)
                return;
            this._containedThing = Editor.CreateThing(this.contains);
            this._containedThing.active = false;
            this._containedThing.visible = false;
            Level.Add(this._containedThing);
        }

        public override void PrepareForHost()
        {
            this.UpdateContainedThing();
            if (this._containedThing != null)
                this._containedThing.PrepareForHost();
            base.PrepareForHost();
        }

        protected override float CalculatePersonalImpactPower(MaterialThing with, ImpactedFrom from) => base.CalculatePersonalImpactPower(with, from) - 1.5f;

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicsObject)
            {
                (with as PhysicsObject).specialFrictionMod = 0.16f;
                (with as PhysicsObject).modFric = true;
            }
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                glassParticle.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            if (bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                if (this.duck != null)
                    this.duck.ThrowItem();
                this.Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(this.x, this.y));
            }
            if (this.isServerForObject && bullet.isLocal)
            {
                this.breakPoints -= this.damageMultiplier;
                this.damageMultiplier += 2f;
                if (breakPoints <= 0.0)
                    this.Destroy(new DTShot(bullet));
                --this.vSpeed;
                this.hSpeed += bullet.travelDirNormalized.x;
                this.vSpeed += bullet.travelDirNormalized.y;
            }
            return base.Hit(bullet, hitPos);
        }

        public override bool Hurt(float points)
        {
            if (carved >= 0.0)
                this.carved += points * 0.05f;
            return true;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            this._hitPoints = 0.0f;
            Level.Remove(this);
            SFX.Play("glassHit");
            Vec2 hitAngle = Vec2.Zero;
            if (type is DTShot)
                hitAngle = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < 8; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(-4f, 4f), hitAngle);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (float)(hitAngle.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                glassParticle.vSpeed = (float)(hitAngle.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-6f, 6f), this.y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            this.ReleaseContainedObject();
            return true;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; index < 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(exitPos.x, exitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (float)(bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                glassParticle.vSpeed = (float)(bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
        }

        private void ReleaseContainedObject()
        {
            if (!this.isServerForObject || this._containedThing == null)
                return;
            this.Fondle(this._containedThing);
            this._containedThing.alpha = 1f;
            this._containedThing.active = true;
            this._containedThing.material = null;
            this._containedThing.visible = true;
            this._containedThing.velocity = this.velocity + new Vec2(0.0f, -2f);
            if (this.duck == null)
                return;
            this.duck.GiveHoldable(this._containedThing as Holdable);
        }

        public override void HeatUp(Vec2 location)
        {
            this._hitPoints -= 0.01f;
            if (_hitPoints < 0.0500000007450581)
            {
                Level.Remove(this);
                this._destroyed = true;
                this.ReleaseContainedObject();
                for (int index = 0; index < 16; ++index)
                {
                    FluidData water = Fluid.Water;
                    water.amount = 1f / 1000f;
                    Fluid fluid = new Fluid(this.x + Rando.Int(-6, 6), this.y + Rando.Int(-6, 6), Vec2.Zero, water)
                    {
                        hSpeed = (float)(index / 16.0 - 0.5) * Rando.Float(0.3f, 0.4f),
                        vSpeed = Rando.Float(-1.5f, 0.5f)
                    };
                    Level.Add(fluid);
                }
            }
            FluidData water1 = Fluid.Water;
            water1.amount = 1f / 1000f;
            Fluid fluid1 = new Fluid(this.x + Rando.Int(-6, 6), this.y + Rando.Int(-6, 6), Vec2.Zero, water1)
            {
                hSpeed = Rando.Float(-0.1f, 0.1f),
                vSpeed = Rando.Float(-0.3f, 0.3f)
            };
            Level.Add(fluid1);
            base.HeatUp(location);
        }

        public override void Draw()
        {
            if (this._containedThing != null)
            {
                Depth depth = this.depth;
                this.depth = depth - 8;
                base.Draw();
                if (this._frozen == null)
                {
                    this._frozen = new MaterialFrozen(this._containedThing)
                    {
                        intensity = 1f
                    };
                }
                Material material = DuckGame.Graphics.material;
                DuckGame.Graphics.material = _frozen;
                this._containedThing.position = this.position;
                this._containedThing.alpha = 1f;
                this._containedThing.depth = depth - 4;
                this._containedThing.angle = this.angle;
                this._containedThing.offDir = this.offDir;
                this._containedThing.Draw();
                DuckGame.Graphics.material = material;
                this.depth = depth;
                this.alpha = 0.5f;
                base.Draw();
                this.alpha = 1f;
                this.depth = depth;
            }
            else if (this.didCarve)
            {
                float y1 = this.y;
                this.graphic.flipH = this.offDir <= 0;
                this._graphic.position = this.position;
                this._graphic.alpha = this.alpha;
                this._graphic.angle = this.angle;
                this._graphic.depth = this.depth;
                this._graphic.scale = this.scale;
                this._graphic.center = this.center;
                int y2 = (int)((1.0 - _hitPoints) * 12.0);
                DuckGame.Graphics.Draw(this._graphic.texture, this.position + new Vec2(0.0f, y2), new Rectangle?(new Rectangle(0.0f, 0.0f, 16f, 24 - y2)), Color.White, this.angle, this._graphic.center, this.scale, this.graphic.flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.depth);
                this.y = y1;
            }
            else
                base.Draw();
        }

        public override void UpdateMaterial()
        {
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                base.connection = value;
                if (!this.isServerForObject)
                    return;
                this.Fondle(this._containedThing);
            }
        }

        public override void Update()
        {
            base.Update();
            this.heat = -1f;
            if (this._containedThing != null && this._containedThing is Holdable)
            {
                if ((double)(this._containedThing as MaterialThing).weight > (double)this.weight)
                    this.weight = (this._containedThing as MaterialThing).weight;
                (this._containedThing as MaterialThing).heat = -1f;
                (this._containedThing as Holdable).UpdateMaterial();
            }
            if (carved >= 1.0 && !this.didCarve)
            {
                if (this._containedThing != null)
                {
                    this.Destroy(new DTImpale(this));
                }
                else
                {
                    this._sprite = new SpriteMap("iceSculpture", 16, 24);
                    this.graphic = _sprite;
                    this.center = new Vec2(8f, 15f);
                    for (int index = 0; index < 12; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-9f, 9f), this.y + Rando.Float(-9f, 9f));
                        smallSmoke._sprite.color = Color.White;
                        Level.Add(smallSmoke);
                    }
                    SFX.Play("crateDestroy", pitch: Rando.Float(0.1f, 0.3f));
                    this.didCarve = true;
                }
            }
            if (damageMultiplier > 1.0)
            {
                this.damageMultiplier -= 0.2f;
            }
            else
            {
                this.damageMultiplier = 1f;
                this.breakPoints = 15f;
            }
            if (!this.didCarve)
            {
                this._sprite.frame = (int)Math.Floor((1.0 - _hitPoints / 1.0) * 5.0);
                if (this._sprite.frame == 0)
                {
                    this.collisionOffset = new Vec2(-8f, -8f);
                    this.collisionSize = new Vec2(16f, 16f);
                }
                else if (this._sprite.frame == 1)
                {
                    this.collisionOffset = new Vec2(-8f, -7f);
                    this.collisionSize = new Vec2(16f, 15f);
                }
                else if (this._sprite.frame == 2)
                {
                    this.collisionOffset = new Vec2(-7f, -4f);
                    this.collisionSize = new Vec2(14f, 11f);
                }
                else if (this._sprite.frame == 3)
                {
                    this.collisionOffset = new Vec2(-6f, -2f);
                    this.collisionSize = new Vec2(12f, 7f);
                }
                else
                {
                    if (this._sprite.frame != 4)
                        return;
                    this.collisionOffset = new Vec2(-6f, -1f);
                    this.collisionSize = new Vec2(12f, 5f);
                }
            }
            else
            {
                int num = (int)((1.0 - _hitPoints) * 12.0);
                this.collisionOffset = new Vec2(-8f, num - 8);
                this.collisionSize = new Vec2(16f, 16 - num);
            }
        }
    }
}
