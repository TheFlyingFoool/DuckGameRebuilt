// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoBlock
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public abstract class AutoBlock : Block, IAutoTile, IDontMove, IPathNodeBlocker, IDontUpdate
    {
        public bool indestructable;
        protected bool _hasNubs = true;
        protected SpriteMap _sprite;
        public Nubber _bLeftNub;
        public Nubber _bRightNub;
        public string _tileset;
        public float verticalWidth = 16f;
        public float verticalWidthThick = 16f;
        public float horizontalHeight = 16f;
        private AutoBlock bLeft;
        private AutoBlock bRight;
        private AutoBlock up;
        private AutoBlock down;
        public ushort blockIndex;
        public static ushort _kBlockIndex;
        private Sprite _brokenSprite;
        public int northIndex;
        public int southIndex;
        public int eastIndex;
        public int westIndex;
        private List<Thing> _additionalBlocks = new List<Thing>();
        private Func<Thing, bool> checkFilter;
        //private bool inObjectsChanged;
        protected bool brokeLeft;
        protected bool brokeRight;
        protected bool brokeUp;
        protected bool brokeDown;
        protected bool hasBroke;
        public bool _hasLeftNub = true;
        public bool _hasRightNub = true;
        public bool needsRefresh;
        //private bool neededRefresh;
        public bool setLayer = true;
        public bool cheap;
        public bool isFlipped;
        private AutoBlock topbLeft;
        private AutoBlock topbRight;
        private AutoBlock bottombLeft;
        private AutoBlock bottombRight;

        public override void SetTranslation(Vec2 translation)
        {
            if (this._bLeftNub != null)
                this._bLeftNub.SetTranslation(translation);
            if (this._bRightNub != null)
                this._bRightNub.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public override int frame
        {
            get => this._sprite.frame;
            set
            {
                this._sprite.frame = value;
                this.UpdateNubbers();
            }
        }

        public override void InitializeNeighbors()
        {
            if (this._neighborsInitialized)
                return;
            this._neighborsInitialized = true;
            if (this._leftBlock == null)
            {
                this._leftBlock = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.left - 2f, this.position.y), this.checkFilter);
                if (this._leftBlock != null)
                    this._leftBlock.InitializeNeighbors();
            }
            if (this._rightBlock == null)
            {
                this._rightBlock = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.right + 2f, this.position.y), this.checkFilter);
                if (this._rightBlock != null)
                    this._rightBlock.InitializeNeighbors();
            }
            if (this._upBlock == null)
            {
                this._upBlock = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.position.x, this.top - 2f), this.checkFilter);
                if (this._upBlock != null)
                    this._upBlock.InitializeNeighbors();
            }
            if (this._downBlock != null)
                return;
            this._downBlock = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.position.x, this.bottom + 2f), this.checkFilter);
            if (this._downBlock == null)
                return;
            this._downBlock.InitializeNeighbors();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (Editor.saving)
            {
                this._processedByEditor = true;
                this.InitializeNeighbors();
                if (this.upBlock != null && !this.upBlock.processedByEditor)
                    binaryClassChunk.AddProperty("north", this.upBlock.Serialize());
                if (this.downBlock != null && !this.downBlock.processedByEditor)
                    binaryClassChunk.AddProperty("north", this.downBlock.Serialize());
                if (this.rightBlock != null && !this.rightBlock.processedByEditor)
                    binaryClassChunk.AddProperty("east", this.rightBlock.Serialize());
                if (this.leftBlock != null && !this.leftBlock.processedByEditor)
                    binaryClassChunk.AddProperty("west", this.leftBlock.Serialize());
            }
            binaryClassChunk.AddProperty("frame", _sprite.frame);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            if (Editor.saving)
            {
                this._additionalBlocks.Clear();
                this._neighborsInitialized = true;
                BinaryClassChunk property1 = node.GetProperty<BinaryClassChunk>("north");
                if (property1 != null)
                {
                    AutoBlock autoBlock = Thing.LoadThing(property1) as AutoBlock;
                    this._upBlock = autoBlock;
                    autoBlock._downBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property2 = node.GetProperty<BinaryClassChunk>("south");
                if (property2 != null)
                {
                    AutoBlock autoBlock = Thing.LoadThing(property2) as AutoBlock;
                    this._downBlock = autoBlock;
                    autoBlock._upBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property3 = node.GetProperty<BinaryClassChunk>("east");
                if (property3 != null)
                {
                    AutoBlock autoBlock = Thing.LoadThing(property3) as AutoBlock;
                    this._rightBlock = autoBlock;
                    autoBlock._leftBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property4 = node.GetProperty<BinaryClassChunk>("west");
                if (property4 != null)
                {
                    AutoBlock autoBlock = Thing.LoadThing(property4) as AutoBlock;
                    this._leftBlock = autoBlock;
                    autoBlock._rightBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
            }
            this._sprite.frame = node.GetProperty<int>("frame");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            if (Editor.saving)
            {
                this._processedByEditor = true;
                this.InitializeNeighbors();
                if (this.upBlock != null && !this.upBlock.processedByEditor)
                    new DXMLNode("north").Add(this.upBlock.LegacySerialize());
                if (this.downBlock != null && !this.downBlock.processedByEditor)
                    new DXMLNode("south").Add(this.downBlock.LegacySerialize());
                if (this.rightBlock != null && !this.rightBlock.processedByEditor)
                    new DXMLNode("east").Add(this.rightBlock.LegacySerialize());
                if (this.leftBlock != null && !this.leftBlock.processedByEditor)
                    new DXMLNode("west").Add(this.leftBlock.LegacySerialize());
            }
            dxmlNode.Add(new DXMLNode("frame", _sprite.frame));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            if (Editor.saving)
            {
                this._additionalBlocks.Clear();
                this._neighborsInitialized = true;
                DXMLNode node1 = node.Element("north");
                if (node1 != null)
                {
                    AutoBlock autoBlock = Thing.LegacyLoadThing(node1) as AutoBlock;
                    this._upBlock = autoBlock;
                    autoBlock._downBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                DXMLNode node2 = node.Element("south");
                if (node2 != null)
                {
                    AutoBlock autoBlock = Thing.LegacyLoadThing(node2) as AutoBlock;
                    this._downBlock = autoBlock;
                    autoBlock._upBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                DXMLNode node3 = node.Element("east");
                if (node3 != null)
                {
                    AutoBlock autoBlock = Thing.LegacyLoadThing(node3) as AutoBlock;
                    this._rightBlock = autoBlock;
                    autoBlock._leftBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
                DXMLNode node4 = node.Element("west");
                if (node4 != null)
                {
                    AutoBlock autoBlock = Thing.LegacyLoadThing(node4) as AutoBlock;
                    this._leftBlock = autoBlock;
                    autoBlock._rightBlock = this;
                    this._additionalBlocks.Add(autoBlock);
                }
            }
            DXMLNode dxmlNode = node.Element("frame");
            if (dxmlNode != null)
                this._sprite.frame = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override void Added(Level parent)
        {
            foreach (Thing additionalBlock in this._additionalBlocks)
                Level.Add(additionalBlock);
            this._additionalBlocks.Clear();
            base.Added(parent);
        }

        public AutoBlock(float x, float y, string tileset)
          : base(x, y)
        {
            this.checkFilter = blok => blok != this && (blok as AutoBlock)._tileset == this._tileset;
            if (tileset == null)
                tileset = "";
            if (tileset != "")
            {
                this._sprite = new SpriteMap(tileset, 16, 16)
                {
                    frame = 40
                };
            }
            this._tileset = tileset;
            this.graphic = _sprite;
            this.collisionSize = new Vec2(16f, 16f);
            this.thickness = 10f;
            this.centerx = 8f;
            this.centery = 8f;
            this.collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.4f;
            this.flammable = 0.8f;
            this._isStatic = true;
            this._canBeGrouped = true;
            this.layer = Layer.Blocks;
            this._impactThreshold = 100f;
            this.blockIndex = AutoBlock._kBlockIndex;
            ++AutoBlock._kBlockIndex;
            this._placementCost = 1;
        }

        public void RegisterHit(Vec2 hitPos, bool neighbors = false)
        {
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            this.RegisterHit(hitPos, true);
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnBurn(Vec2 position, Thing litBy) => false;

        public override void EditorObjectsChanged()
        {
            //this.inObjectsChanged = true;
            this.PlaceBlock();
            //this.inObjectsChanged = false;
        }

        public void DestroyPuddle(FluidPuddle f)
        {
            if (f == null || f.removeFromLevel)
                return;
            int num1 = (int)(f.collisionSize.x / 8.0);
            float num2 = f.data.amount / num1;
            for (int index = 0; index < num1; ++index)
            {
                FluidData data = f.data;
                data.amount = num2;
                Fluid fluid = new Fluid(f.left + 8f + index * 8, (f.top - 4f + (float)Math.Sin(index * 0.7f) * 2f), new Vec2(0f, 1f), data)
                {
                    vSpeed = -2f
                };
                Level.Add(fluid);
            }
            Level.Remove(f);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTRocketExplosion))
                return false;
            if (this.up == null)
                this.up = Level.CheckPoint<AutoBlock>(this.x, this.y - 16f, this);
            if (this.down == null)
                this.down = Level.CheckPoint<AutoBlock>(this.x, this.y + 16f, this);
            if (this.bLeft == null)
                this.bLeft = Level.CheckPoint<AutoBlock>(this.x - 16f, this.y, this);
            if (this.bRight == null)
                this.bRight = Level.CheckPoint<AutoBlock>(this.x + 16f, this.y, this);
            if (this.up != null && this.up._tileset == this._tileset)
            {
                this.up.brokeDown = true;
                this.up.hasBroke = true;
                this.up.downBlock = null;
                this.up.down = null;
            }
            if (this.down != null && this.down._tileset == this._tileset)
            {
                this.down.brokeUp = true;
                this.down.hasBroke = true;
                this.down.upBlock = null;
                this.down.up = null;
            }
            if (this.bLeft != null && this.bLeft._tileset == this._tileset)
            {
                this.bLeft.brokeRight = true;
                this.bLeft.hasBroke = true;
                this.bLeft.rightBlock = null;
                this.bLeft.bRight = null;
            }
            if (this.bRight != null && this.bRight._tileset == this._tileset)
            {
                this.bRight.brokeLeft = true;
                this.bRight.hasBroke = true;
                this.bRight.leftBlock = null;
                this.bRight.bLeft = null;
            }
            if (this.structure != null)
            {
                foreach (Block block in this.structure.blocks)
                    block.structure = null;
                this.structure = null;
            }
            if (this.up == null)
                this.DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(this.x, this.y - 9f)));
            if (this.bLeft == null)
                this.DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(this.x - 9f, this.y)));
            if (this.bRight == null)
                this.DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(this.x + 9f, this.y)));
            return true;
        }

        public void UpdateNubbers()
        {
            this.TerminateNubs();
            if (!this._hasNubs || this.removeFromLevel)
                return;
            switch (this._sprite.frame)
            {
                case 1:
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 2:
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 4:
                    if (this._hasRightNub)
                    {
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        break;
                    }
                    break;
                case 5:
                    if (this._hasRightNub)
                    {
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        break;
                    }
                    break;
                case 32:
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 37:
                    if (this._hasRightNub)
                    {
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        break;
                    }
                    break;
                case 40:
                    if (this._hasRightNub)
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 41:
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 43:
                    if (this._hasRightNub)
                    {
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        break;
                    }
                    break;
                case 49:
                    if (this._hasRightNub)
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 51:
                    if (this._hasLeftNub)
                    {
                        this._bLeftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        break;
                    }
                    break;
                case 52:
                    if (this._hasRightNub)
                    {
                        this._bRightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        break;
                    }
                    break;
            }
            if (this._bLeftNub != null)
            {
                Level.Add(_bLeftNub);
                this._bLeftNub.depth = this.depth;
                this._bLeftNub.layer = this.layer;
                this._bLeftNub.material = this.material;
            }
            if (this._bRightNub == null)
                return;
            Level.Add(_bRightNub);
            this._bRightNub.depth = this.depth;
            this._bRightNub.layer = this.layer;
            this._bRightNub.material = this.material;
        }

        public override void Update()
        {
            if (this.skipWreck)
            {
                this.skipWreck = false;
            }
            else
            {
                if (this.shouldWreck)
                {
                    this.Destroy(new DTRocketExplosion(null));
                    Level.Remove(this);
                }
                if (this.needsRefresh)
                {
                    this.PlaceBlock();
                    this.needsRefresh = false;
                    //this.neededRefresh = true;
                }
            }
            if (this.setLayer)
                this.layer = Layer.Blocks;
            base.Update();
        }

        public BlockGroup GroupWithNeighbors(bool addToLevel = true)
        {
            if (this._groupedWithNeighbors)
                return null;
            this._groupedWithNeighbors = true;
            AutoBlock autoBlock1 = this.leftBlock as AutoBlock;
            AutoBlock autoBlock2 = this.rightBlock as AutoBlock;
            List<AutoBlock> autoBlockList1 = new List<AutoBlock>();
            autoBlockList1.Add(this);
            while (autoBlock1 != null && !autoBlock1._groupedWithNeighbors)
            {
                if (autoBlock1.collisionSize.y == (double)this.collisionSize.y && autoBlock1.collisionOffset.y == (double)this.collisionOffset.y)
                {
                    autoBlockList1.Add(autoBlock1);
                    autoBlock1 = autoBlock1.leftBlock as AutoBlock;
                }
                else
                    autoBlock1 = null;
            }
            while (autoBlock2 != null && !autoBlock2._groupedWithNeighbors)
            {
                if (autoBlock2.collisionSize.y == (double)this.collisionSize.y && autoBlock2.collisionOffset.y == (double)this.collisionOffset.y)
                {
                    autoBlockList1.Add(autoBlock2);
                    autoBlock2 = autoBlock2.rightBlock as AutoBlock;
                }
                else
                    autoBlock2 = null;
            }
            List<AutoBlock> autoBlockList2 = new List<AutoBlock>();
            autoBlockList2.Add(this);
            AutoBlock autoBlock3 = this.upBlock as AutoBlock;
            AutoBlock autoBlock4 = this.downBlock as AutoBlock;
            while (autoBlock3 != null && !autoBlock3._groupedWithNeighbors)
            {
                if (autoBlock3.collisionSize.x == (double)this.collisionSize.x && autoBlock3.collisionOffset.x == (double)this.collisionOffset.x)
                {
                    autoBlockList2.Add(autoBlock3);
                    autoBlock3 = autoBlock3.upBlock as AutoBlock;
                }
                else
                    autoBlock3 = null;
            }
            while (autoBlock4 != null && !autoBlock4._groupedWithNeighbors)
            {
                if (autoBlock4.collisionSize.x == (double)this.collisionSize.x && autoBlock4.collisionOffset.x == (double)this.collisionOffset.x)
                {
                    autoBlockList2.Add(autoBlock4);
                    autoBlock4 = autoBlock4.downBlock as AutoBlock;
                }
                else
                    autoBlock4 = null;
            }
            List<AutoBlock> autoBlockList3 = autoBlockList1;
            if (autoBlockList2.Count > autoBlockList3.Count)
                autoBlockList3 = autoBlockList2;
            if (autoBlockList3.Count <= 1)
                return null;
            BlockGroup blockGroup = new BlockGroup();
            foreach (AutoBlock b in autoBlockList3)
            {
                b._groupedWithNeighbors = true;
                blockGroup.Add(b);
                if (addToLevel)
                    Level.Remove(b);
            }
            blockGroup.CalculateSize();
            if (addToLevel)
                Level.Add(blockGroup);
            return blockGroup;
        }

        public override void Initialize()
        {
            if (this._sprite != null)
                this.UpdateCollision();
            this.DoPositioning();
        }

        public virtual void DoPositioning()
        {
            if (Level.current is Editor || this.graphic == null)
                return;
            if (!RandomLevelNode.editorLoad)
                this.cheap = true;
            this.graphic.position = this.position;
            this.graphic.scale = this.scale;
            this.graphic.center = this.center;
            this.graphic.depth = this.depth;
            this.graphic.alpha = this.alpha;
            this.graphic.angle = this.angle;
            (this.graphic as SpriteMap).ClearCache();
            (this.graphic as SpriteMap).UpdateFrame();
        }

        public override void Terminate()
        {
            if (this._groupedWithNeighbors && !this.shouldWreck)
                return;
            this.TerminateNubs();
        }

        private void TerminateNubs()
        {
            if (this._bLeftNub != null)
            {
                Level.Remove(_bLeftNub);
                this._bLeftNub = null;
            }
            if (this._bRightNub == null)
                return;
            Level.Remove(_bRightNub);
            this._bRightNub = null;
        }

        public override void Draw()
        {
            if (DevConsole.showCollision)
            {
                if (this.leftBlock != null)
                    Graphics.DrawLine(this.position, this.position + new Vec2(-8f, 0f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (this.rightBlock != null)
                    Graphics.DrawLine(this.position, this.position + new Vec2(8f, 0f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (this.upBlock != null)
                    Graphics.DrawLine(this.position, this.position + new Vec2(0f, -8f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (this.downBlock != null)
                    Graphics.DrawLine(this.position, this.position + new Vec2(0f, 8f), Color.Red * 0.5f, depth: ((Depth)1f));
            }
            if (this.hasBroke)
            {
                if (this._brokenSprite == null)
                {
                    this._brokenSprite = new Sprite("brokeEdge");
                    this._brokenSprite.CenterOrigin();
                    this._brokenSprite.depth = this.depth;
                }
                if (this.brokeLeft)
                {
                    this._brokenSprite.angleDegrees = 180f;
                    Graphics.Draw(this._brokenSprite, this.x - 16f, this.y);
                }
                if (this.brokeRight)
                {
                    this._brokenSprite.angleDegrees = 0f;
                    Graphics.Draw(this._brokenSprite, this.x + 16f, this.y);
                }
                if (this.brokeUp)
                {
                    this._brokenSprite.angleDegrees = 270f;
                    Graphics.Draw(this._brokenSprite, this.x, this.y - 16f);
                }
                if (this.brokeDown)
                {
                    this._brokenSprite.angleDegrees = 90f;
                    Graphics.Draw(this._brokenSprite, this.x, this.y + 16f);
                }
            }
            if (this.cheap)
                this.graphic.UltraCheapStaticDraw(this.flipHorizontal);
            else
                base.Draw();
        }

        public void UpdateCollision()
        {
            switch (this._sprite.frame)
            {
                case 32:
                case 41:
                case 51:
                case 53:
                case 58:
                    this.collisionSize = new Vec2((float)(8.0 + verticalWidth / 2.0), 16f);
                    this.collisionOffset = new Vec2((float)(-(double)this.verticalWidth / 2.0), -8f);
                    break;
                case 37:
                case 43:
                case 45:
                case 52:
                case 60:
                    this.collisionSize = new Vec2((float)(8.0 + verticalWidth / 2.0), 16f);
                    this.collisionOffset = new Vec2(-8f, -8f);
                    break;
                case 40:
                case 44:
                case 49:
                case 50:
                    this.collisionSize = new Vec2(this.verticalWidth, 16f);
                    this.collisionOffset = new Vec2((float)(-(double)this.verticalWidth / 2.0), -8f);
                    break;
                default:
                    this.collisionSize = new Vec2(16f, 16f);
                    this.collisionOffset = new Vec2(-8f, -8f);
                    break;
            }
            switch (this._sprite.frame)
            {
                case 1:
                case 2:
                case 7:
                case 18:
                case 26:
                    this._collisionSize.x = this.verticalWidthThick;
                    this._collisionOffset.x = (float)(16.0 - verticalWidthThick - 8.0);
                    break;
                case 4:
                case 5:
                case 15:
                case 20:
                case 28:
                    this._collisionSize.x = this.verticalWidthThick;
                    this._collisionOffset.x = -8f;
                    break;
            }
            switch (this._sprite.frame)
            {
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 32:
                case 33:
                case 35:
                case 36:
                case 37:
                case 40:
                case 41:
                case 43:
                case 44:
                case 57:
                case 58:
                case 59:
                case 60:
                    this._collisionSize.y = this.horizontalHeight;
                    break;
                default:
                    this._collisionSize.y = 16f;
                    break;
            }
        }

        public void PlaceBlock()
        {
            if (this._sprite == null)
                return;
            this._placed = true;
            this.FindFrame();
            this.UpdateNubbers();
            this.UpdateCollision();
            this.DoPositioning();
        }

        public void FindFrame()
        {
            this.up = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x, this.y - 16f), this.checkFilter);
            this.down = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x, this.y + 16f), this.checkFilter);
            this.bLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x - 16f, this.y), this.checkFilter);
            this.bRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x + 16f, this.y), this.checkFilter);
            this.topbLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x - 16f, this.y - 16f), this.checkFilter);
            this.topbRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x + 16f, this.y - 16f), this.checkFilter);
            this.bottombLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x - 16f, this.y + 16f), this.checkFilter);
            this.bottombRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(this.x + 16f, this.y + 16f), this.checkFilter);
            if (this.up != null)
            {
                if (this.bRight != null)
                {
                    if (this.down != null)
                    {
                        if (this.bLeft != null)
                        {
                            if (this.topbLeft != null)
                            {
                                if (this.topbRight != null)
                                {
                                    if (this.bottombLeft != null)
                                    {
                                        if (this.bottombRight != null)
                                            this._sprite.frame = 11;
                                        else
                                            this._sprite.frame = 21;
                                    }
                                    else if (this.bottombRight != null)
                                        this._sprite.frame = 17;
                                    else
                                        this._sprite.frame = 23;
                                }
                                else if (this.bottombRight != null)
                                {
                                    if (this.bottombLeft == null)
                                        return;
                                    this._sprite.frame = 12;
                                }
                                else if (this.bottombLeft != null)
                                    this._sprite.frame = 22;
                                else
                                    this._sprite.frame = 30;
                            }
                            else if (this.topbRight != null)
                            {
                                if (this.bottombRight != null)
                                {
                                    if (this.bottombLeft != null)
                                        this._sprite.frame = 10;
                                    else
                                        this._sprite.frame = 16;
                                }
                                else
                                {
                                    this._sprite.frame = 24;
                                }
                            }
                            else if (this.bottombRight != null)
                            {
                                if (this.bottombLeft != null)
                                    this._sprite.frame = 3;
                                else
                                    this._sprite.frame = 8;
                            }
                            else
                            {
                                if (this.bottombLeft != null)
                                    return;
                                this._sprite.frame = 42;
                            }
                        }
                        else if (this.topbRight != null)
                        {
                            if (this.bottombRight != null)
                            {
                                this._sprite.frame = 18;
                            }
                            else
                            {
                                if (this.topbLeft == null)
                                {
                                }
                                this._sprite.frame = 7;
                            }
                        }
                        else
                        {
                            if (this.topbLeft == null)
                            {
                                if (this.bottombRight != null)
                                {
                                    this._sprite.frame = 2;
                                    return;
                                }
                            }
                            this._sprite.frame = 53;
                        }
                    }
                    else if (this.bLeft != null)
                    {
                        if (this.topbLeft != null)
                        {
                            if (this.topbRight != null)
                                this._sprite.frame = 27;
                            else
                                this._sprite.frame = 29;
                        }
                        else if (this.topbRight != null)
                            this._sprite.frame = 25;
                        else
                            this._sprite.frame = 57;
                    }
                    else if (this.topbRight != null)
                        this._sprite.frame = 26;
                    else
                        this._sprite.frame = 58;
                }
                else if (this.down != null)
                {
                    if (this.bLeft != null)
                    {
                        if (this.topbLeft != null)
                        {
                            if (this.bottombLeft != null)
                            {
                                this._sprite.frame = 20;
                            }
                            else
                            {
                                if (this.bottombRight == null)
                                {
                                }
                                this._sprite.frame = 15;
                            }
                        }
                        else
                        {
                            if (this.topbRight == null)
                            {
                                if (this.bottombRight != null)
                                {
                                    if (this.bottombLeft != null)
                                    {
                                        this._sprite.frame = 4;
                                        return;
                                    }
                                    this._sprite.frame = 45;
                                    return;
                                }
                                if (this.bottombLeft != null)
                                {
                                    this._sprite.frame = 4;
                                    return;
                                }
                            }
                            this._sprite.frame = 45;
                        }
                    }
                    else
                        this._sprite.frame = 50;
                }
                else if (this.bLeft != null)
                {
                    if (this.topbLeft != null)
                        this._sprite.frame = 28;
                    else
                        this._sprite.frame = 60;
                }
                else
                    this._sprite.frame = 44;
            }
            else if (this.bRight != null)
            {
                if (this.down != null)
                {
                    if (this.bLeft != null)
                    {
                        if (this.bottombLeft == null && this.bottombRight == null)
                            this._sprite.frame = 34;
                        else if (this.topbLeft != null)
                        {
                            if (this.topbRight != null)
                                this._sprite.frame = 3;
                            else if (this.bottombRight != null)
                            {
                                if (this.bottombLeft == null)
                                    return;
                                this._sprite.frame = 3;
                            }
                            else if (this.bottombLeft != null)
                                this._sprite.frame = 6;
                            else
                                this._sprite.frame = 24;
                        }
                        else if (this.topbRight != null)
                        {
                            if (this.bottombRight != null)
                            {
                                if (this.bottombLeft != null)
                                    this._sprite.frame = 3;
                                else
                                    this._sprite.frame = 0;
                            }
                            else
                            {
                                if (this.bottombLeft != null)
                                    return;
                                this._sprite.frame = 25;
                            }
                        }
                        else if (this.bottombRight != null)
                        {
                            if (this.bottombLeft != null)
                                this._sprite.frame = 3;
                            else
                                this._sprite.frame = 8;
                        }
                        else if (this.bottombLeft != null)
                            this._sprite.frame = 14;
                        else
                            this._sprite.frame = 34;
                    }
                    else if (this.topbLeft == null && this.topbRight != null && this.bottombLeft != null && this.bottombRight != null)
                        this._sprite.frame = 1;
                    else if (this.bottombRight != null)
                        this._sprite.frame = 2;
                    else
                        this._sprite.frame = 51;
                }
                else if (this.bLeft != null)
                {
                    if ((this.bottombLeft != null || this.topbLeft != null) && (this.topbRight != null || this.bottombRight != null))
                        this._sprite.frame = 59;
                    else if (this.bottombRight != null || this.topbRight != null)
                        this._sprite.frame = 33;
                    else if (this.bottombLeft != null || this.topbLeft != null)
                        this._sprite.frame = 35;
                    else
                        this._sprite.frame = 36;
                }
                else if (this.bottombRight != null || this.topbRight != null)
                    this._sprite.frame = 41;
                else
                    this._sprite.frame = 32;
            }
            else if (this.down != null)
            {
                if (this.bLeft != null)
                {
                    if (this.topbLeft != null)
                    {
                        if (this.topbRight == null)
                        {
                            if (this.bottombLeft != null)
                            {
                                if (this.bottombRight != null)
                                {
                                    this._sprite.frame = 5;
                                    return;
                                }
                                this._sprite.frame = 4;
                                return;
                            }
                            this._sprite.frame = 52;
                            return;
                        }
                    }
                    else if (this.topbRight == null)
                    {
                        if (this.bottombLeft != null)
                        {
                            this._sprite.frame = 4;
                            return;
                        }
                    }
                    this._sprite.frame = 52;
                }
                else
                    this._sprite.frame = 49;
            }
            else if (this.bLeft != null)
            {
                if (this.bottombLeft != null || this.topbLeft != null)
                    this._sprite.frame = 43;
                else
                    this._sprite.frame = 37;
            }
            else
                this._sprite.frame = 40;
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
