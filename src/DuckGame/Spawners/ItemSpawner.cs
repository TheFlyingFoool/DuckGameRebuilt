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
        private bool _triedSeating;
        private bool _isClassicSpawner;
        private int _seatingTries;
        private Thing previewThing;
        private Sprite previewSprite;
        private float _bob;
        private List<TypeProbPair> _possible = new List<TypeProbPair>();

        public override void SetTranslation(Vec2 translation)
        {
            if (this._ball1 != null)
                this._ball1.SetTranslation(translation);
            if (this._ball2 != null)
                this._ball2.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public void PreparePossibilities()
        {
            if (this.possible.Count <= 0)
                return;
            this.contains = MysteryGun.PickType(this.chanceGroup, this.possible);
        }

        public System.Type contains { get; set; }

        public Holdable _hoverItem
        {
            get => this.hoverItem;
            set => this.SetHoverItem(value);
        }

        public ItemSpawner(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("gunSpawner", 14, 10);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 0.0f);
            this.collisionSize = new Vec2(14f, 2f);
            this.collisionOffset = new Vec2(-7f, 0.0f);
            this.depth = - 0.35f;
            this.contains = c;
            this.hugWalls = WallHug.Floor;
            this._placementCost += 4;
            this.editorTooltip = "Spawns a copy of the specified item after a specified duration.";
        }

        public override void Initialize()
        {
            if (this.GetType() == typeof(ItemSpawner))
                this._isClassicSpawner = true;
            this._ball1 = new SpawnerBall(this.x, this.y - 1f, false);
            this._ball2 = new SpawnerBall(this.x, this.y - 1f, true);
            Level.Add((Thing)this._ball1);
            Level.Add((Thing)this._ball2);
            if (this.spawnOnStart)
                this._spawnWait = this.spawnTime;
            if (Level.current is Editor)
                return;
            if (this.randomSpawn && this.keepRandom)
            {
                List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                this.contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                this.randomSpawn = false;
            }
            else
            {
                if (this.possible.Count <= 0 || !(this.contains == (System.Type)null))
                    return;
                this.PreparePossibilities();
            }
        }

        public void BreakHoverBond()
        {
            if (this._hoverItem == null)
                return;
            this._hoverItem.gravMultiplier = 1f;
            this._hoverItem.hoverSpawner = (ItemSpawner)null;
            this._hoverItem = (Holdable)null;
        }

        public virtual void SpawnItem()
        {
            this._spawnWait = 0.0f;
            if (Network.isActive && this.isServerForObject)
                Send.Message((NetMessage)new NMItemSpawned(this));
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(this.contains);
            PhysicsObject hover = !Network.isActive || bag.GetOrDefault("isOnlineCapable", true) ? Editor.CreateThing(this.contains) as PhysicsObject : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as PhysicsObject;
            if (hover == null)
                return;
            hover.x = this.x;
            hover.y = (float)((double)this.top + ((double)hover.y - (double)hover.bottom) - 6.0);
            hover.vSpeed = -2f;
            hover.spawnAnimation = true;
            hover.isSpawned = true;
            hover.offDir = this.offDir;
            Level.Add((Thing)hover);
            if (!this._seated)
                return;
            this.SetHoverItem(hover as Holdable);
        }

        public virtual void SetHoverItem(Holdable hover)
        {
            if (this._hoverItem == hover)
                return;
            if (this._hoverItem != null)
            {
                this._hoverItem.hoverSpawner = (ItemSpawner)null;
                this._hoverItem.grounded = false;
            }
            this.hoverItem = hover;
            if (this._hoverItem == null)
                return;
            this._hoverItem.hoverSpawner = this;
            this._hoverItem.grounded = true;
        }

        public void TrySeating()
        {
            if (this._seatingTries >= 3 || this._ball1 == null)
                return;
            if (Level.CheckPoint<IPlatform>(this.position + new Vec2(0.0f, 6f)) != null)
            {
                this._seated = true;
                this._seatingTries = 3;
            }
            else
                this._seated = false;
            ++this._seatingTries;
        }

        public override void EditorUpdate()
        {
            this._hoverSin.Update();
            if (this.contains != (System.Type)null && !this.randomSpawn && Level.current is Editor && (this.previewThing == null || this.previewThing.GetType() != this.contains))
            {
                this.previewThing = Editor.GetThing(this.contains);
                if (this.previewThing != null)
                    this.previewSprite = this.previewThing.GeneratePreview(32, 32, true);
            }
            this.collisionSize = new Vec2(14f, 8f);
            this.collisionOffset = new Vec2(-7f, -6f);
            base.EditorUpdate();
        }

        public override void Update()
        {
            this._hoverSin.Update();
            this.TrySeating();
            if (this._hoverItem == null)
            {
                if (this._seated)
                {
                    Holdable hover = Level.current.NearestThingFilter<Holdable>(this.position, (Predicate<Thing>)(d => !(d is TeamHat) && (d as Holdable).canPickUp), 16f);
                    if (hover != null && hover.owner == null && hover != null && hover.canPickUp && (double)Math.Abs(hover.hSpeed) + (double)Math.Abs(hover.vSpeed) < 2.5 && (!(hover is Gun) || (hover as Gun).ammo > 0))
                        this.SetHoverItem(hover);
                }
                this._ball1.desiredOrbitDistance = 3f;
                this._ball2.desiredOrbitDistance = 3f;
                this._ball1.desiredOrbitHeight = 1f;
                this._ball2.desiredOrbitHeight = 1f;
                if (Level.current.simulatePhysics)
                    this._spawnWait += 0.0166666f;
            }
            else if ((double)Math.Abs(this._hoverItem.hSpeed) + (double)Math.Abs(this._hoverItem.vSpeed) > 2.0 || (double)(this._hoverItem.collisionCenter - this.position).length > 18.0 || this._hoverItem.destroyed || this._hoverItem.removeFromLevel || this._hoverItem.owner != null || !this._hoverItem.visible)
            {
                this.BreakHoverBond();
            }
            else
            {
                this._hoverItem.position = Lerp.Vec2Smooth(this._hoverItem.position, this.position + new Vec2(0.0f, (float)(-((double)this._hoverItem.bottom - (double)this._hoverItem.y) - 2.0 + (double)(float)this._hoverSin * 2.0)), 0.2f);
                this._hoverItem.vSpeed = 0.0f;
                this._hoverItem.gravMultiplier = 0.0f;
                this._ball1.desiredOrbitDistance = this._hoverItem.collisionSize.x / 2f;
                this._ball2.desiredOrbitDistance = this._hoverItem.collisionSize.x / 2f;
                this._ball1.desiredOrbitHeight = 4f;
                this._ball2.desiredOrbitHeight = 4f;
            }
            if (!Network.isServer || this._numSpawned >= this.spawnNum && this.spawnNum != -1 || this._hoverItem != null || !(this.contains != (System.Type)null) && !this.randomSpawn || (double)this._spawnWait < (double)this.spawnTime)
                return;
            if ((double)this.initialDelay > 0.0)
            {
                this.initialDelay -= 0.0166666f;
            }
            else
            {
                if (this.randomSpawn)
                {
                    List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
                    this.contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
                }
                ++this._numSpawned;
                this.SpawnItem();
            }
        }

        public override void Draw()
        {
            if (Level.current is Editor)
                this.TrySeating();
            if (this._isClassicSpawner)
                this._sprite.frame = (this._seated ? 0 : 1) + (this.keepRandom ? 4 : (this.randomSpawn ? 2 : 0));
            if (this.contains != (System.Type)null && !this.randomSpawn && Level.current is Editor && this.previewThing != null)
            {
                this._bob += 0.05f;
                this.previewSprite.CenterOrigin();
                this.previewSprite.alpha = 0.5f;
                this.previewSprite.flipH = this.offDir < (sbyte)0;
                Graphics.Draw(this.previewSprite, this.x, (float)((double)this.y - 8.0 + Math.Sin((double)this._bob) * 2.0));
            }
            if (this._isClassicSpawner && (this._sprite.frame == 1 || this._sprite.frame == 3))
            {
                this.y -= 2f;
                base.Draw();
                this.y += 2f;
            }
            else
                base.Draw();
        }

        public override void Terminate()
        {
            Level.Remove((Thing)this._ball1);
            Level.Remove((Thing)this._ball2);
        }

        public List<TypeProbPair> possible => this._possible;

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (this._hasContainedItem)
            {
                binaryClassChunk.AddProperty("contains", (object)Editor.SerializeTypeName(this.contains));
                binaryClassChunk.AddProperty("randomSpawn", (object)this.randomSpawn);
                binaryClassChunk.AddProperty("keepRandom", (object)this.keepRandom);
            }
            binaryClassChunk.AddProperty("possible", (object)MysteryGun.SerializeTypeProb(this.possible));
            binaryClassChunk.AddProperty("spawnTime", (object)this.spawnTime);
            binaryClassChunk.AddProperty("initialDelay", (object)this.initialDelay);
            binaryClassChunk.AddProperty("spawnOnStart", (object)this.spawnOnStart);
            binaryClassChunk.AddProperty("spawnNum", (object)this.spawnNum);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            if (this._hasContainedItem)
            {
                this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
                this.randomSpawn = node.GetProperty<bool>("randomSpawn");
                this.keepRandom = node.GetProperty<bool>("keepRandom");
            }
            this._possible = MysteryGun.DeserializeTypeProb(node.GetProperty<string>("possible"));
            this.spawnTime = node.GetProperty<float>("spawnTime");
            this.initialDelay = node.GetProperty<float>("initialDelay");
            this.spawnOnStart = node.GetProperty<bool>("spawnOnStart");
            this.spawnNum = node.GetProperty<int>("spawnNum");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            if (this._hasContainedItem)
                dxmlNode.Add(new DXMLNode("contains", this.contains != (System.Type)null ? (object)this.contains.AssemblyQualifiedName : (object)""));
            dxmlNode.Add(new DXMLNode("spawnTime", (object)Change.ToString((object)this.spawnTime)));
            dxmlNode.Add(new DXMLNode("initialDelay", (object)Change.ToString((object)this.initialDelay)));
            dxmlNode.Add(new DXMLNode("spawnOnStart", (object)Change.ToString((object)this.spawnOnStart)));
            if (this._hasContainedItem)
                dxmlNode.Add(new DXMLNode("randomSpawn", (object)Change.ToString((object)this.randomSpawn)));
            if (this._hasContainedItem)
                dxmlNode.Add(new DXMLNode("keepRandom", (object)Change.ToString((object)this.keepRandom)));
            dxmlNode.Add(new DXMLNode("spawnNum", (object)Change.ToString((object)this.spawnNum)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            if (this._hasContainedItem)
            {
                DXMLNode dxmlNode = node.Element("contains");
                if (dxmlNode != null)
                    this.contains = Editor.GetType(dxmlNode.Value);
            }
            DXMLNode dxmlNode1 = node.Element("spawnTime");
            if (dxmlNode1 != null)
                this.spawnTime = Change.ToSingle((object)dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("initialDelay");
            if (dxmlNode2 != null)
                this.initialDelay = Change.ToSingle((object)dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("spawnOnStart");
            if (dxmlNode3 != null)
                this.spawnOnStart = Convert.ToBoolean(dxmlNode3.Value);
            if (this._hasContainedItem)
            {
                DXMLNode dxmlNode4 = node.Element("randomSpawn");
                if (dxmlNode4 != null)
                    this.randomSpawn = Convert.ToBoolean(dxmlNode4.Value);
                DXMLNode dxmlNode5 = node.Element("keepRandom");
                if (dxmlNode5 != null)
                    this.keepRandom = Convert.ToBoolean(dxmlNode5.Value);
            }
            DXMLNode dxmlNode6 = node.Element("spawnNum");
            if (dxmlNode6 != null)
                this.spawnNum = Convert.ToInt32(dxmlNode6.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding((object)this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem((ContextMenu)new ContextSlider("Delay", (IContextListener)null, new FieldBinding((object)this, "spawnTime", 0.25f, 100f)));
            contextMenu.AddItem((ContextMenu)new ContextSlider("Initial Delay", (IContextListener)null, new FieldBinding((object)this, "initialDelay", max: 100f)));
            contextMenu.AddItem((ContextMenu)new ContextCheckBox("Start Spawned", (IContextListener)null, new FieldBinding((object)this, "spawnOnStart")));
            if (this._hasContainedItem)
            {
                contextMenu.AddItem((ContextMenu)new ContextCheckBox("Random", (IContextListener)null, new FieldBinding((object)this, "randomSpawn")));
                contextMenu.AddItem((ContextMenu)new ContextCheckBox("Keep Random", (IContextListener)null, new FieldBinding((object)this, "keepRandom")));
            }
            contextMenu.AddItem((ContextMenu)new ContextSlider("Number", (IContextListener)null, new FieldBinding((object)this, "spawnNum", -1f, 100f), 1f, "INF"));
            if (this._hasContainedItem)
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu((IContextListener)contextMenu);
                editorGroupMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
                editorGroupMenu.text = "Contains";
                contextMenu.AddItem((ContextMenu)editorGroupMenu);
            }
            EditorGroupMenu editorGroupMenu1 = new EditorGroupMenu((IContextListener)contextMenu);
            editorGroupMenu1.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), new FieldBinding((object)this, "possible"));
            editorGroupMenu1.text = "Possible";
            contextMenu.AddItem((ContextMenu)editorGroupMenu1);
            return (ContextMenu)contextMenu;
        }

        public override void DrawHoverInfo()
        {
            if (this.possible.Count > 0)
            {
                float num = 0.0f;
                foreach (TypeProbPair typeProbPair in this.possible)
                {
                    if ((double)typeProbPair.probability > 0.0)
                    {
                        Color white = Color.White;
                        Color color = (double)typeProbPair.probability != 0.0 ? ((double)typeProbPair.probability >= 0.300000011920929 ? ((double)typeProbPair.probability >= 0.699999988079071 ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                        string text = typeProbPair.type.Name + ": " + typeProbPair.probability.ToString("0.000");
                        Graphics.DrawString(text, this.position + new Vec2((float)(-(double)Graphics.GetStringWidth(text, scale: 0.5f) / 2.0), (float)-(16.0 + (double)num)), color, (Depth)0.9f, scale: 0.5f);
                        num += 4f;
                    }
                }
            }
            else
            {
                string text = "EMPTY";
                if (this.contains != (System.Type)null)
                    text = this.contains.Name;
                Graphics.DrawString(text, this.position + new Vec2((float)(-(double)Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
            }
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (this.contains != (System.Type)null)
                str = this.contains.Name;
            if (this.contains == (System.Type)null && (double)this.spawnTime == 10.0)
                return base.GetDetailsString();
            return base.GetDetailsString() + "Contains: " + str + "\nTime: " + this.spawnTime.ToString("0.00", (IFormatProvider)CultureInfo.InvariantCulture);
        }
    }
}
