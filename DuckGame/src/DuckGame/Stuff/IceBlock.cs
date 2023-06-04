// Decompiled with JetBrains decompiler
// Type: DuckGame.IceBlock
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private Type _previewType;
        private Sprite _previewSprite;
        private float breakPoints = 15f;
        private Thing _containedThing;
        private float damageMultiplier;
        private MaterialFrozen _frozen;

        public Type contains { get; set; }

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
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override void EditorUpdate()
        {
            if (contains != null && (_previewSprite == null || _previewType != contains))
            {
                Thing thing = Editor.GetThing(contains);
                if (thing != null)
                {
                    _previewSprite = thing.GeneratePreview(48, 48, true);
                    _previewSprite.CenterOrigin();
                }
                _previewType = contains;
            }
            base.EditorUpdate();
        }

        public override void EditorRender()
        {
            if (contains != null && _previewSprite != null)
            {
                if (_frozen == null)
                    _frozen = new MaterialFrozen(this)
                    {
                        intensity = 1f
                    };
                Material material = Graphics.material;
                Graphics.material = _frozen;
                _previewSprite.alpha = 0.5f;
                Graphics.Draw(_previewSprite, x, y, depth + 10);
                Graphics.material = material;
            }
            base.EditorRender();
        }

        public IceBlock(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("iceBlock", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Ice Block";
            editorTooltip = "Slippery, slidery, fun. Also great for keeping your (gigantic) drinks cold.";
            thickness = 2f;
            weight = 5f;
            buoyancy = 1f;
            _hitPoints = 1f;
            impactThreshold = -1f;
            physicsMaterial = PhysicsMaterial.Glass;
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0f;
            collideSounds.Add("glassHit");
            superNonFlammable = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Network.isActive)
                return;
            UpdateContainedThing();
        }

        private void UpdateContainedThing()
        {
            if (!(contains != null) || Level.current is Editor)
                return;
            _containedThing = Editor.CreateThing(contains);
            _containedThing.active = false;
            _containedThing.visible = false;
            Level.Add(_containedThing);
        }

        public override void PrepareForHost()
        {
            UpdateContainedThing();
            if (_containedThing != null)
                _containedThing.PrepareForHost();
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
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (-bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            if (bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Fondle(this, DuckNetwork.localConnection);
                if (duck != null)
                    duck.ThrowItem();
                Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(x, y));
            }
            if (isServerForObject && bullet.isLocal)
            {
                breakPoints -= damageMultiplier;
                damageMultiplier += 2f;
                if (breakPoints <= 0f)
                    Destroy(new DTShot(bullet));
                --vSpeed;
                hSpeed += bullet.travelDirNormalized.x;
                vSpeed += bullet.travelDirNormalized.y;
            }
            return base.Hit(bullet, hitPos);
        }

        public override bool Hurt(float points)
        {
            if (carved >= 0f)
                carved += points * 0.05f;
            return true;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            Level.Remove(this);
            SFX.Play("glassHit");
            Vec2 hitAngle = Vec2.Zero;
            if (type is DTShot)
                hitAngle = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f), hitAngle);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (hitAngle.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-6f, 6f), y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            ReleaseContainedObject();
            return true;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(exitPos.x, exitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
        }

        private void ReleaseContainedObject()
        {
            if (!isServerForObject || _containedThing == null)
                return;
            Fondle(_containedThing);
            _containedThing.alpha = 1f;
            _containedThing.active = true;
            _containedThing.material = null;
            _containedThing.visible = true;
            _containedThing.velocity = velocity + new Vec2(0f, -2f);
            if (duck == null)
                return;
            duck.GiveHoldable(_containedThing as Holdable);
        }

        public override void HeatUp(Vec2 location)
        {
            _hitPoints -= 0.01f;
            if (_hitPoints < 0.05f)
            {
                Level.Remove(this);
                _destroyed = true;
                ReleaseContainedObject();
                for (int index = 0; index < 16; ++index)
                {
                    FluidData water = Fluid.Water;
                    water.amount = 1f / 1000f;
                    Fluid fluid = new Fluid(x + Rando.Int(-6, 6), y + Rando.Int(-6, 6), Vec2.Zero, water)
                    {
                        hSpeed = (index / 16f - 0.5f) * Rando.Float(0.3f, 0.4f),
                        vSpeed = Rando.Float(-1.5f, 0.5f)
                    };
                    Level.Add(fluid);
                }
            }
            FluidData water1 = Fluid.Water;
            water1.amount = 1f / 1000f;
            Fluid fluid1 = new Fluid(x + Rando.Int(-6, 6), y + Rando.Int(-6, 6), Vec2.Zero, water1)
            {
                hSpeed = Rando.Float(-0.1f, 0.1f),
                vSpeed = Rando.Float(-0.3f, 0.3f)
            };
            Level.Add(fluid1);
            base.HeatUp(location);
        }

        public override void Draw()
        {
            if (_containedThing != null)
            {
                Depth depth = this.depth;
                this.depth = depth - 8;
                base.Draw();
                if (_frozen == null)
                {
                    _frozen = new MaterialFrozen(_containedThing)
                    {
                        intensity = 1f
                    };
                }
                Material material = Graphics.material;
                Graphics.material = _frozen;
                _containedThing.position = position;
                _containedThing.alpha = 1f;
                _containedThing.depth = depth - 4;
                _containedThing.angle = angle;
                _containedThing.offDir = offDir;
                _containedThing.Draw();
                Graphics.material = material;
                this.depth = depth;
                alpha = 0.5f;
                base.Draw();
                alpha = 1f;
                this.depth = depth;
            }
            else if (didCarve)
            {
                float y1 = y;
                graphic.flipH = offDir <= 0;
                _graphic.position = position;
                _graphic.alpha = alpha;
                _graphic.angle = angle;
                _graphic.depth = depth;
                _graphic.scale = scale;
                _graphic.center = center;
                int y2 = (int)((1 - _hitPoints) * 12);
                Graphics.Draw(_graphic.texture, position + new Vec2(0f, y2), new Rectangle?(new Rectangle(0f, 0f, 16f, 24 - y2)), Color.White, angle, _graphic.center, scale, graphic.flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None, depth);
                y = y1;
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
                if (!isServerForObject)
                    return;
                Fondle(_containedThing);
            }
        }

        public override void Update()
        {
            base.Update();
            heat = -1f;
            if (_containedThing != null && _containedThing is Holdable)
            {
                if ((_containedThing as MaterialThing).weight > weight)
                    weight = (_containedThing as MaterialThing).weight;
                (_containedThing as MaterialThing).heat = -1f;
                (_containedThing as Holdable).UpdateMaterial();
            }
            if (carved >= 1 && !didCarve)
            {
                if (_containedThing != null)
                {
                    Destroy(new DTImpale(this));
                }
                else
                {
                    _sprite = new SpriteMap("iceSculpture", 16, 24);
                    graphic = _sprite;
                    center = new Vec2(8f, 15f);
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 12; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-9f, 9f), y + Rando.Float(-9f, 9f));
                        smallSmoke._sprite.color = Color.White;
                        Level.Add(smallSmoke);
                    }
                    SFX.Play("crateDestroy", pitch: Rando.Float(0.1f, 0.3f));
                    didCarve = true;
                }
            }
            if (damageMultiplier > 1)
            {
                damageMultiplier -= 0.2f;
            }
            else
            {
                damageMultiplier = 1f;
                breakPoints = 15f;
            }
            if (!didCarve)
            {
                _sprite.frame = (int)Math.Floor((1f - _hitPoints / 1f) * 5f);
                if (_sprite.frame == 0)
                {
                    collisionOffset = new Vec2(-8f, -8f);
                    collisionSize = new Vec2(16f, 16f);
                }
                else if (_sprite.frame == 1)
                {
                    collisionOffset = new Vec2(-8f, -7f);
                    collisionSize = new Vec2(16f, 15f);
                }
                else if (_sprite.frame == 2)
                {
                    collisionOffset = new Vec2(-7f, -4f);
                    collisionSize = new Vec2(14f, 11f);
                }
                else if (_sprite.frame == 3)
                {
                    collisionOffset = new Vec2(-6f, -2f);
                    collisionSize = new Vec2(12f, 7f);
                }
                else
                {
                    if (_sprite.frame != 4)
                        return;
                    collisionOffset = new Vec2(-6f, -1f);
                    collisionSize = new Vec2(12f, 5f);
                }
            }
            else
            {
                int num = (int)((1 - _hitPoints) * 12);
                collisionOffset = new Vec2(-8f, num - 8);
                collisionSize = new Vec2(16f, 16 - num);
            }
        }
    }
}
