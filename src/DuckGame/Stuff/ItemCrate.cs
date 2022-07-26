// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemCrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("canSpawn", false)]
    public class ItemCrate : PhysicsObject, IPlatform, IContainAThing, IContainPossibleThings
    {
        public StateBinding _containedObject1Binding = new StateBinding(nameof(_containedObject1));
        public StateBinding _containedObject2Binding = new StateBinding(nameof(_containedObject2));
        public StateBinding _containedObject3Binding = new StateBinding(nameof(_containedObject3));
        public StateBinding _containedObject4Binding = new StateBinding(nameof(_containedObject4));
        public StateBinding _destroyedBinding = new StateBinding("_destroyed");
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public StateBinding _damageMultiplierBinding = new StateBinding(nameof(damageMultiplier));
        public bool randomSpawn;
        public EditorProperty<bool> revealRandom = new EditorProperty<bool>(false);
        private List<TypeProbPair> _possible = new List<TypeProbPair>();
        private SpriteMap _sprite;
        public PhysicsObject _containedObject1;
        public PhysicsObject _containedObject2;
        public PhysicsObject _containedObject3;
        public PhysicsObject _containedObject4;
        public PhysicsObject[] _containedObjects;
        private Sprite _randomMark;
        private PhysicsObject _containedObject;
        private PhysicsObject _previewThing;
        private Sprite _containedSprite;
        private float damageMultiplier = 1f;

        public System.Type contains { get; set; }

        public List<TypeProbPair> possible => this._possible;

        public ItemCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this._sprite = new SpriteMap("bigItemCrate", 32, 33);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(16f, 24f);
            this.collisionOffset = new Vec2(-16f, -24f);
            this.collisionSize = new Vec2(32f, 32f);
            this.depth = - 0.7f;
            this.thickness = 2f;
            this.weight = 10f;
            this._randomMark = new Sprite("itemBoxRandom");
            this._randomMark.CenterOrigin();
            this.flammable = 0.3f;
            this.collideSounds.Add("rockHitGround2");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._containedObjects = new PhysicsObject[4]
            {
        this._containedObject1,
        this._containedObject2,
        this._containedObject3,
        this._containedObject4
            };
            this.editorTooltip = "Chock full of good stuff- if you can get it open..";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            if (this.randomSpawn)
            {
                List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                this.contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
            }
            else
            {
                if (this.possible.Count <= 0 || !(this.contains == (System.Type)null))
                    return;
                this.PreparePossibilities();
            }
        }

        public void PreparePossibilities()
        {
            if (this.possible.Count <= 0)
                return;
            this.contains = MysteryGun.PickType(this.chanceGroup, this.possible);
        }

        public PhysicsObject containedObject
        {
            get => this._containedObject;
            set => this._containedObject = value;
        }

        public virtual void UpdateContainedObject()
        {
            for (int index = 0; index < 4; ++index)
            {
                if (Network.isActive && this.isServerForObject && this._containedObjects[index] == null)
                {
                    this._containedObjects[index] = this.GetSpawnItem();
                    if (this._containedObjects[index] != null)
                    {
                        this._containedObjects[index].visible = false;
                        this._containedObjects[index].solid = false;
                        this._containedObjects[index].active = false;
                        this._containedObjects[index].position = this.position;
                        Level.Add((Thing)this._containedObjects[index]);
                    }
                    this._containedObject1 = this._containedObjects[0];
                    this._containedObject2 = this._containedObjects[1];
                    this._containedObject3 = this._containedObjects[2];
                    this._containedObject4 = this._containedObjects[3];
                }
            }
        }

        public virtual PhysicsObject GetSpawnItem()
        {
            if (this.contains == (System.Type)null)
                return (PhysicsObject)null;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(this.contains);
            return !Network.isActive || bag.GetOrDefault("isOnlineCapable", true) ? Editor.CreateThing(this.contains) as PhysicsObject : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as PhysicsObject;
        }

        public override void EditorUpdate()
        {
            this.UpdatePreview();
            base.EditorUpdate();
        }

        private void UpdatePreview()
        {
            if (this._previewThing != null && !(this._previewThing.GetType() != this.contains))
                return;
            this._previewThing = this.GetSpawnItem();
            if (this._previewThing != null)
                this._containedSprite = this._previewThing.GetEditorImage(20, 16, true, (Effect)null, (RenderTarget2D)null, true);
            else
                this._containedSprite = (Sprite)null;
        }

        public override void Update()
        {
            this.UpdateContainedObject();
            this._containedObjects[0] = this._containedObject1;
            this._containedObjects[1] = this._containedObject2;
            this._containedObjects[2] = this._containedObject3;
            this._containedObjects[3] = this._containedObject4;
            if (this.isServerForObject)
            {
                if ((double)this.damageMultiplier > 1.0)
                    this.damageMultiplier -= 0.2f;
                else
                    this.damageMultiplier = 1f;
                if ((double)this._hitPoints <= 0.0 && !this._destroyed)
                    this.Destroy((DestroyType)new DTImpact((Thing)this));
                if (this._onFire)
                    this._hitPoints = Math.Min(this._hitPoints, (1f - this.burnt) * this._maxHealth);
            }
            this.UpdatePreview();
            if (this.contains == (System.Type)null)
                this.buoyancy = 1f;
            else
                this.buoyancy = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            if (this.randomSpawn && !this.revealRandom.value)
            {
                this._sprite.frame = 4;
                Vec2 vec2 = this.Offset(new Vec2(0.0f, -8f));
                this._randomMark.angle = this.angle;
                this._randomMark.flipH = this.offDir <= (sbyte)0;
                DuckGame.Graphics.Draw(this._randomMark, vec2.x, vec2.y, this.depth + 10);
            }
            else if (this._containedSprite != null)
            {
                this._sprite.frame = 4;
                this._containedSprite.CenterOrigin();
                Vec2 vec2 = this.Offset(new Vec2(0.0f, -8f));
                this._containedSprite.angle = this.angle;
                this._containedSprite.flipH = this.offDir <= (sbyte)0;
                DuckGame.Graphics.Draw(this._containedSprite, vec2.x, vec2.y, this.depth + 10);
            }
            else
                this._sprite.frame = 0;
            this._sprite.frame += (int)((1.0 - (double)this._hitPoints / (double)this._maxHealth) * 3.5);
            base.Draw();
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding((object)this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem((ContextMenu)new ContextCheckBox("Random", (IContextListener)null, new FieldBinding((object)this, "randomSpawn")));
            EditorGroupMenu editorGroupMenu1 = new EditorGroupMenu((IContextListener)contextMenu);
            editorGroupMenu1.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            editorGroupMenu1.text = "Contains";
            contextMenu.AddItem((ContextMenu)editorGroupMenu1);
            EditorGroupMenu editorGroupMenu2 = new EditorGroupMenu((IContextListener)contextMenu);
            editorGroupMenu2.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), new FieldBinding((object)this, "possible"));
            editorGroupMenu2.text = "Possible";
            contextMenu.AddItem((ContextMenu)editorGroupMenu2);
            return (ContextMenu)contextMenu;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", (object)Editor.SerializeTypeName(this.contains));
            binaryClassChunk.AddProperty("randomSpawn", (object)this.randomSpawn);
            binaryClassChunk.AddProperty("possible", (object)MysteryGun.SerializeTypeProb(this.possible));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            this.randomSpawn = node.GetProperty<bool>("randomSpawn");
            this._possible = MysteryGun.DeserializeTypeProb(node.GetProperty<string>("possible"));
            return true;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            this._hitPoints = 0.0f;
            for (int index = 0; index < 10; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(this.x - 10f + Rando.Float(20f), this.y - 10f + Rando.Float(20f));
                woodDebris.hSpeed = Rando.Float(-4f, 4f);
                woodDebris.vSpeed = Rando.Float(-4f, 4f);
                Level.Add((Thing)woodDebris);
            }
            for (int index = 0; index < 3; ++index)
            {
                MusketSmoke musketSmoke = new MusketSmoke(this.x + Rando.Float(-10f, 10f), this.y + Rando.Float(-10f, 10f));
                musketSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                musketSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add((Thing)musketSmoke);
            }
            for (int index = 0; index < 4; ++index)
            {
                PhysicsObject physicsObject = this._containedObjects[index];
                if (!Network.isActive)
                    physicsObject = this.GetSpawnItem();
                if (physicsObject != null)
                {
                    if (this._onFire)
                        physicsObject.heat = 0.8f;
                    physicsObject.position = this.position + new Vec2((float)((double)index * 2.66666674613953 - 4.0), 0.0f);
                    switch (index)
                    {
                        case 0:
                        case 3:
                            if (index == 0)
                                physicsObject.hSpeed = -2f;
                            else
                                physicsObject.hSpeed = 2f;
                            physicsObject.vSpeed = -2.5f;
                            goto label_20;
                        case 1:
                            physicsObject.hSpeed = -1.2f;
                            break;
                        default:
                            physicsObject.hSpeed = 1.2f;
                            break;
                    }
                    physicsObject.vSpeed = -3.5f;
                label_20:
                    if (Network.isActive)
                    {
                        physicsObject.visible = true;
                        physicsObject.solid = true;
                        physicsObject.active = true;
                    }
                    else
                        Level.Add((Thing)physicsObject);
                }
            }
            SFX.Play("crateDestroy");
            Level.Remove((Thing)this);
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if ((double)this._hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            for (int index = 0; (double)index < 1.0 + (double)this.damageMultiplier / 2.0; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                woodDebris.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                woodDebris.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add((Thing)woodDebris);
            }
            SFX.Play("woodHit");
            if (this.isServerForObject && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                this.Destroy((DestroyType)new DTShot(bullet));
                Level.Add((Thing)new GrenadeExplosion(this.x, this.y));
            }
            this._hitPoints -= this.damageMultiplier;
            this.damageMultiplier += 2f;
            if ((double)this._hitPoints <= 0.0)
            {
                if (bullet.isLocal)
                    Thing.SuperFondle((Thing)this, DuckNetwork.localConnection);
                this.Destroy((DestroyType)new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }
    }
}
