// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemSpawner
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class ItemSpawner : Thing, IContainAThing, IContainPossibleThings
    {
        protected bool _hasContainedItem = true;
        protected SpriteMap _sprite;
        public float _spawnWait;
        public float initialDelay;
        public float spawnTime = 10f;
        public bool spawnOnStart = true;
        public bool randomSpawn;
        public bool keepRandom;
        public int spawnNum = -1;
        private Holdable hoverItem;
        private SinWaveManualUpdate _hoverSin = (SinWaveManualUpdate)0.05f;
        public SpawnerBall _ball1;
        public SpawnerBall _ball2;
        protected int _numSpawned;
        public bool _seated;
        //private bool _triedSeating;
        private bool _isClassicSpawner;
        private int _seatingTries;
        private Thing previewThing;
        private Sprite previewSprite;
        private float _bob;
        private List<TypeProbPair> _possible = new List<TypeProbPair>();

        public override void SetTranslation(Vec2 translation)
        {
            if (_ball1 != null)
                _ball1.SetTranslation(translation);
            if (_ball2 != null)
                _ball2.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public void PreparePossibilities()
        {
            if (possible.Count <= 0)
                return;
            contains = MysteryGun.PickType(chanceGroup, possible);
        }

        public System.Type contains { get; set; }

        public Holdable _hoverItem
        {
            get => hoverItem;
            set => SetHoverItem(value);
        }

        public ItemSpawner(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("gunSpawner", 14, 10);
            graphic = _sprite;
            center = new Vec2(7f, 0f);
            collisionSize = new Vec2(14f, 2f);
            collisionOffset = new Vec2(-7f, 0f);
            depth = -0.35f;
            contains = c;
            hugWalls = WallHug.Floor;
            _placementCost += 4;
            editorTooltip = "Spawns a copy of the specified item after a specified duration.";
        }

        public override void Initialize()
        {
            if (GetType() == typeof(ItemSpawner))
                _isClassicSpawner = true;
            _ball1 = new SpawnerBall(x, y - 1f, false);
            _ball2 = new SpawnerBall(x, y - 1f, true);
            _ball1.shouldbeinupdateloop = false;
            _ball2.shouldbeinupdateloop = false;
            Level.Add(_ball1);
            Level.Add(_ball2);
            if (spawnOnStart)
                _spawnWait = spawnTime;
            if (Level.current is Editor)
                return;
            if (randomSpawn && keepRandom)
            {
                List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                randomSpawn = false;
            }
            else
            {
                if (possible.Count <= 0 || !(contains == null))
                    return;
                PreparePossibilities();
            }
        }

        public void BreakHoverBond()
        {
            if (_hoverItem == null)
                return;
            _hoverItem.gravMultiplier = 1f;
            _hoverItem.hoverSpawner = null;
            _hoverItem = null;
        }

        public virtual void SpawnItem()
        {
            _spawnWait = 0f;
            if (Network.isActive && isServerForObject)
                Send.Message(new NMItemSpawned(this));
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(contains);
            PhysicsObject hover = !Network.isActive || bag.GetOrDefault("isOnlineCapable", true) ? Editor.CreateThing(contains) as PhysicsObject : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as PhysicsObject;
            if (hover == null)
                return;
            hover.x = x;
            hover.y = (float)(top + (hover.y - hover.bottom) - 6.0);
            hover.vSpeed = -2f;
            hover.spawnAnimation = true;
            hover.isSpawned = true;
            hover.offDir = offDir;
            Level.Add(hover);
            if (!_seated)
                return;
            SetHoverItem(hover as Holdable);
        }

        public virtual void SetHoverItem(Holdable hover)
        {
            if (_hoverItem == hover)
                return;
            if (_hoverItem != null)
            {
                _hoverItem.hoverSpawner = null;
                _hoverItem.grounded = false;
            }
            hoverItem = hover;
            if (_hoverItem == null)
                return;
            _hoverItem.hoverSpawner = this;
            _hoverItem.grounded = true;
        }

        public void TrySeating()
        {
            if (_seatingTries >= 3 || _ball1 == null)
                return;
            if (Level.CheckPoint<IPlatform>(position + new Vec2(0f, 6f)) != null)
            {
                _seated = true;
                _seatingTries = 3;
            }
            else
                _seated = false;
            ++_seatingTries;
        }

        public override void EditorUpdate()
        {
            _hoverSin.Update();
            if (contains != null && !randomSpawn && Level.current is Editor && (previewThing == null || previewThing.GetType() != contains))
            {
                previewThing = Editor.GetThing(contains);
                if (previewThing != null)
                    previewSprite = previewThing.GeneratePreview(32, 32, true);
            }
            collisionSize = new Vec2(14f, 8f);
            collisionOffset = new Vec2(-7f, -6f);
            base.EditorUpdate();
        }

        public override void Update()
        {
            _hoverSin.Update();
            TrySeating();
            if (_hoverItem == null)
            {
                if (_seated)
                {
                    Holdable hover = Level.current.NearestThingFilter<Holdable>(position, d => !(d is TeamHat) && (d as Holdable).canPickUp, 16f);
                    if (hover != null && hover.owner == null && hover != null && hover.canPickUp && Math.Abs(hover.hSpeed) + Math.Abs(hover.vSpeed) < 2.5 && (!(hover is Gun) || (hover as Gun).ammo > 0))
                        SetHoverItem(hover);
                }
                _ball1.desiredOrbitDistance = 3f;
                _ball2.desiredOrbitDistance = 3f;
                _ball1.desiredOrbitHeight = 1f;
                _ball2.desiredOrbitHeight = 1f;
                if (Level.current.simulatePhysics)
                    _spawnWait += 0.0166666f;
            }
            else if (Math.Abs(_hoverItem.hSpeed) + Math.Abs(_hoverItem.vSpeed) > 2.0 || (_hoverItem.collisionCenter - position).length > 18.0 || _hoverItem.destroyed || _hoverItem.removeFromLevel || _hoverItem.owner != null || !_hoverItem.visible)
            {
                BreakHoverBond();
            }
            else
            {
                _hoverItem.position = Lerp.Vec2Smooth(_hoverItem.position, position + new Vec2(0f, (float)(-(_hoverItem.bottom - _hoverItem.y) - 2.0 + (float)_hoverSin * 2.0)), 0.2f);
                _hoverItem.vSpeed = 0f;
                _hoverItem.gravMultiplier = 0f;
                _ball1.desiredOrbitDistance = _hoverItem.collisionSize.x / 2f;
                _ball2.desiredOrbitDistance = _hoverItem.collisionSize.x / 2f;
                _ball1.desiredOrbitHeight = 4f;
                _ball2.desiredOrbitHeight = 4f;
            }
            if (!Network.isServer || _numSpawned >= spawnNum && spawnNum != -1 || _hoverItem != null || !(contains != null) && !randomSpawn || _spawnWait < spawnTime)
                return;
            if (initialDelay > 0.0)
            {
                initialDelay -= 0.0166666f;
            }
            else
            {
                if (randomSpawn)
                {
                    List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                    contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                }
                ++_numSpawned;
                SpawnItem();
            }
        }

        public override void Draw()
        {
            if (Level.current is Editor)
                TrySeating();
            if (_isClassicSpawner)
                _sprite.frame = (_seated ? 0 : 1) + (keepRandom ? 4 : (randomSpawn ? 2 : 0));
            if (contains != null && !randomSpawn && Level.current is Editor && previewThing != null)
            {
                _bob += 0.05f;
                previewSprite.CenterOrigin();
                previewSprite.alpha = 0.5f;
                previewSprite.flipH = offDir < 0;
                Graphics.Draw(previewSprite, x, (float)(y - 8.0 + Math.Sin(_bob) * 2.0));
            }
            if (_isClassicSpawner && (_sprite.frame == 1 || _sprite.frame == 3))
            {
                y -= 2f;
                base.Draw();
                y += 2f;
            }
            else
                base.Draw();
        }

        public override void Terminate()
        {
            Level.Remove(_ball1);
            Level.Remove(_ball2);
        }

        public List<TypeProbPair> possible => _possible;

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (_hasContainedItem)
            {
                binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
                binaryClassChunk.AddProperty("randomSpawn", randomSpawn);
                binaryClassChunk.AddProperty("keepRandom", keepRandom);
            }
            binaryClassChunk.AddProperty("possible", MysteryGun.SerializeTypeProb(possible));
            binaryClassChunk.AddProperty("spawnTime", spawnTime);
            binaryClassChunk.AddProperty("initialDelay", initialDelay);
            binaryClassChunk.AddProperty("spawnOnStart", spawnOnStart);
            binaryClassChunk.AddProperty("spawnNum", spawnNum);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            if (_hasContainedItem)
            {
                contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
                randomSpawn = node.GetProperty<bool>("randomSpawn");
                keepRandom = node.GetProperty<bool>("keepRandom");
            }
            _possible = MysteryGun.DeserializeTypeProb(node.GetProperty<string>("possible"));
            spawnTime = node.GetProperty<float>("spawnTime");
            initialDelay = node.GetProperty<float>("initialDelay");
            spawnOnStart = node.GetProperty<bool>("spawnOnStart");
            spawnNum = node.GetProperty<int>("spawnNum");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            if (_hasContainedItem)
                dxmlNode.Add(new DXMLNode("contains", contains != null ? contains.AssemblyQualifiedName : (object)""));
            dxmlNode.Add(new DXMLNode("spawnTime", Change.ToString(spawnTime)));
            dxmlNode.Add(new DXMLNode("initialDelay", Change.ToString(initialDelay)));
            dxmlNode.Add(new DXMLNode("spawnOnStart", Change.ToString(spawnOnStart)));
            if (_hasContainedItem)
                dxmlNode.Add(new DXMLNode("randomSpawn", Change.ToString(randomSpawn)));
            if (_hasContainedItem)
                dxmlNode.Add(new DXMLNode("keepRandom", Change.ToString(keepRandom)));
            dxmlNode.Add(new DXMLNode("spawnNum", Change.ToString(spawnNum)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            if (_hasContainedItem)
            {
                DXMLNode dxmlNode = node.Element("contains");
                if (dxmlNode != null)
                    contains = Editor.GetType(dxmlNode.Value);
            }
            DXMLNode dxmlNode1 = node.Element("spawnTime");
            if (dxmlNode1 != null)
                spawnTime = Change.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("initialDelay");
            if (dxmlNode2 != null)
                initialDelay = Change.ToSingle(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("spawnOnStart");
            if (dxmlNode3 != null)
                spawnOnStart = Convert.ToBoolean(dxmlNode3.Value);
            if (_hasContainedItem)
            {
                DXMLNode dxmlNode4 = node.Element("randomSpawn");
                if (dxmlNode4 != null)
                    randomSpawn = Convert.ToBoolean(dxmlNode4.Value);
                DXMLNode dxmlNode5 = node.Element("keepRandom");
                if (dxmlNode5 != null)
                    keepRandom = Convert.ToBoolean(dxmlNode5.Value);
            }
            DXMLNode dxmlNode6 = node.Element("spawnNum");
            if (dxmlNode6 != null)
                spawnNum = Convert.ToInt32(dxmlNode6.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextSlider("Delay", null, new FieldBinding(this, "spawnTime", 0.25f, 100f)));
            contextMenu.AddItem(new ContextSlider("Initial Delay", null, new FieldBinding(this, "initialDelay", max: 100f)));
            contextMenu.AddItem(new ContextCheckBox("Start Spawned", null, new FieldBinding(this, "spawnOnStart")));
            if (_hasContainedItem)
            {
                contextMenu.AddItem(new ContextCheckBox("Random", null, new FieldBinding(this, "randomSpawn")));
                contextMenu.AddItem(new ContextCheckBox("Keep Random", null, new FieldBinding(this, "keepRandom")));
            }
            contextMenu.AddItem(new ContextSlider("Number", null, new FieldBinding(this, "spawnNum", -1f, 100f), 1f, "INF"));
            if (_hasContainedItem)
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(contextMenu);
                editorGroupMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
                editorGroupMenu.text = "Contains";
                contextMenu.AddItem(editorGroupMenu);
            }
            EditorGroupMenu editorGroupMenu1 = new EditorGroupMenu(contextMenu);
            editorGroupMenu1.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), new FieldBinding(this, "possible"));
            editorGroupMenu1.text = "Possible";
            contextMenu.AddItem(editorGroupMenu1);
            return contextMenu;
        }

        public override void DrawHoverInfo()
        {
            if (possible.Count > 0)
            {
                float num = 0f;
                foreach (TypeProbPair typeProbPair in possible)
                {
                    if (typeProbPair.probability > 0f)
                    {
                        Color white = Color.White;
                        Color color = typeProbPair.probability != 0f ? (typeProbPair.probability >= 0.3f ? (typeProbPair.probability >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                        string text = typeProbPair.type.Name + ": " + typeProbPair.probability.ToString("0.000");
                        Graphics.DrawString(text, position + new Vec2((-Graphics.GetStringWidth(text, scale: 0.5f) / 2f), -(16f + num)), color, (Depth)0.9f, scale: 0.5f);
                        num += 4f;
                    }
                }
            }
            else
            {
                string text = "EMPTY";
                if (contains != null)
                    text = contains.Name;
                Graphics.DrawString(text, position + new Vec2((-Graphics.GetStringWidth(text) / 2f), -16f), Color.White, (Depth)0.9f);
            }
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (contains != null)
                str = contains.Name;
            if (contains == null && spawnTime == 10.0)
                return base.GetDetailsString();
            return base.GetDetailsString() + "Contains: " + str + "\nTime: " + spawnTime.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
