// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemCrate
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public List<TypeProbPair> possible => _possible;

        public ItemCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _sprite = new SpriteMap("bigItemCrate", 32, 33);
            graphic = _sprite;
            center = new Vec2(16f, 24f);
            collisionOffset = new Vec2(-16f, -24f);
            collisionSize = new Vec2(32f, 32f);
            depth = -0.7f;
            thickness = 2f;
            weight = 10f;
            _randomMark = new Sprite("itemBoxRandom");
            _randomMark.CenterOrigin();
            flammable = 0.3f;
            collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Metal;
            _containedObjects = new PhysicsObject[4]
            {
        _containedObject1,
        _containedObject2,
        _containedObject3,
        _containedObject4
            };
            editorTooltip = "Chock full of good stuff- if you can get it open..";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            if (randomSpawn)
            {
                List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
            }
            else
            {
                if (possible.Count <= 0 || !(contains == null))
                    return;
                PreparePossibilities();
            }
        }

        public void PreparePossibilities()
        {
            if (possible.Count <= 0)
                return;
            contains = MysteryGun.PickType(chanceGroup, possible);
        }

        public PhysicsObject containedObject
        {
            get => _containedObject;
            set => _containedObject = value;
        }

        public virtual void UpdateContainedObject()
        {
            for (int index = 0; index < 4; ++index)
            {
                if (Network.isActive && isServerForObject && _containedObjects[index] == null)
                {
                    _containedObjects[index] = GetSpawnItem();
                    if (_containedObjects[index] != null)
                    {
                        _containedObjects[index].visible = false;
                        _containedObjects[index].solid = false;
                        _containedObjects[index].active = false;
                        _containedObjects[index].position = position;
                        Level.Add(_containedObjects[index]);
                    }
                    _containedObject1 = _containedObjects[0];
                    _containedObject2 = _containedObjects[1];
                    _containedObject3 = _containedObjects[2];
                    _containedObject4 = _containedObjects[3];
                }
            }
        }

        public virtual PhysicsObject GetSpawnItem()
        {
            if (contains == null)
                return null;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(contains);
            return !Network.isActive || bag.GetOrDefault("isOnlineCapable", true) ? Editor.CreateThing(contains) as PhysicsObject : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as PhysicsObject;
        }

        public override void EditorUpdate()
        {
            UpdatePreview();
            base.EditorUpdate();
        }

        private void UpdatePreview()
        {
            if (_previewThing != null && !(_previewThing.GetType() != contains))
                return;
            _previewThing = GetSpawnItem();
            if (_previewThing != null)
                _containedSprite = _previewThing.GetEditorImage(20, 16, true, null, null, true);
            else
                _containedSprite = null;
        }

        public override void Update()
        {
            UpdateContainedObject();
            _containedObjects[0] = _containedObject1;
            _containedObjects[1] = _containedObject2;
            _containedObjects[2] = _containedObject3;
            _containedObjects[3] = _containedObject4;
            if (isServerForObject)
            {
                if (damageMultiplier > 1.0)
                    damageMultiplier -= 0.2f;
                else
                    damageMultiplier = 1f;
                if (_hitPoints <= 0.0 && !_destroyed)
                    Destroy(new DTImpact(this));
                if (_onFire)
                    _hitPoints = Math.Min(_hitPoints, (1f - burnt) * _maxHealth);
            }
            UpdatePreview();
            if (contains == null)
                buoyancy = 1f;
            else
                buoyancy = 0f;
            base.Update();
        }

        public override void Draw()
        {
            if (randomSpawn && !revealRandom.value)
            {
                _sprite.frame = 4;
                Vec2 vec2 = Offset(new Vec2(0f, -8f));
                _randomMark.angle = angle;
                _randomMark.flipH = offDir <= 0;
                DuckGame.Graphics.Draw(_randomMark, vec2.x, vec2.y, depth + 10);
            }
            else if (_containedSprite != null)
            {
                _sprite.frame = 4;
                _containedSprite.CenterOrigin();
                Vec2 vec2 = Offset(new Vec2(0f, -8f));
                _containedSprite.angle = angle;
                _containedSprite.flipH = offDir <= 0;
                DuckGame.Graphics.Draw(_containedSprite, vec2.x, vec2.y, depth + 10);
            }
            else
                _sprite.frame = 0;
            _sprite.frame += (int)((1.0 - _hitPoints / _maxHealth) * 3.5);
            base.Draw();
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextCheckBox("Random", null, new FieldBinding(this, "randomSpawn")));
            EditorGroupMenu editorGroupMenu1 = new EditorGroupMenu(contextMenu);
            editorGroupMenu1.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            editorGroupMenu1.text = "Contains";
            contextMenu.AddItem(editorGroupMenu1);
            EditorGroupMenu editorGroupMenu2 = new EditorGroupMenu(contextMenu);
            editorGroupMenu2.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), new FieldBinding(this, "possible"));
            editorGroupMenu2.text = "Possible";
            contextMenu.AddItem(editorGroupMenu2);
            return contextMenu;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            binaryClassChunk.AddProperty("randomSpawn", randomSpawn);
            binaryClassChunk.AddProperty("possible", MysteryGun.SerializeTypeProb(possible));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            randomSpawn = node.GetProperty<bool>("randomSpawn");
            _possible = MysteryGun.DeserializeTypeProb(node.GetProperty<string>("possible"));
            return true;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            for (int index = 0; index < 10; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(x - 10f + Rando.Float(20f), y - 10f + Rando.Float(20f));
                woodDebris.hSpeed = Rando.Float(-4f, 4f);
                woodDebris.vSpeed = Rando.Float(-4f, 4f);
                Level.Add(woodDebris);
            }
            for (int index = 0; index < 3; ++index)
            {
                MusketSmoke musketSmoke = new MusketSmoke(x + Rando.Float(-10f, 10f), y + Rando.Float(-10f, 10f));
                musketSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                musketSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(musketSmoke);
            }
            for (int index = 0; index < 4; ++index)
            {
                PhysicsObject physicsObject = _containedObjects[index];
                if (!Network.isActive)
                    physicsObject = GetSpawnItem();
                if (physicsObject != null)
                {
                    if (_onFire)
                        physicsObject.heat = 0.8f;
<<<<<<< Updated upstream
                    physicsObject.position = position + new Vec2(-4f + (float)index * 2.6666667f, 0f);
=======
                    physicsObject.position = position + new Vec2(-4f + index * 2.6666667f, 0f);
>>>>>>> Stashed changes
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
                        Level.Add(physicsObject);
                }
            }
            SFX.Play("crateDestroy");
            Level.Remove(this);
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0f)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < 1f + damageMultiplier / 2f; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                woodDebris.hSpeed = (-bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                woodDebris.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(woodDebris);
            }
            SFX.Play("woodHit");
            if (isServerForObject && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(x, y));
            }
            _hitPoints -= damageMultiplier;
            damageMultiplier += 2f;
            if (_hitPoints <= 0f)
            {
                if (bullet.isLocal)
                    Thing.SuperFondle(this, DuckNetwork.localConnection);
                Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }
    }
}
