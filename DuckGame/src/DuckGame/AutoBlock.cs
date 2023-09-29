// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoBlock
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public bool corderatorIndexedthemAlready;
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
            if (_bLeftNub != null)
                _bLeftNub.SetTranslation(translation);
            if (_bRightNub != null)
                _bRightNub.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public override int frame
        {
            get => _sprite.frame;
            set
            {
                _sprite.frame = value;
                UpdateNubbers();
            }
        }

        public override void InitializeNeighbors()
        {
            if (_neighborsInitialized)
                return;
            _neighborsInitialized = true;
            if (_leftBlock == null)
            {
                _leftBlock = _level.QuadTreePointFilter<AutoBlock>(new Vec2(left - 2f, position.y), checkFilter);
                if (_leftBlock != null)
                    _leftBlock.InitializeNeighbors();
            }
            if (_rightBlock == null)
            {
                _rightBlock = _level.QuadTreePointFilter<AutoBlock>(new Vec2(right + 2f, position.y), checkFilter);
                if (_rightBlock != null)
                    _rightBlock.InitializeNeighbors();
            }
            if (_upBlock == null)
            {
                _upBlock = _level.QuadTreePointFilter<AutoBlock>(new Vec2(position.x, top - 2f), checkFilter);
                if (_upBlock != null)
                    _upBlock.InitializeNeighbors();
            }
            if (_downBlock != null)
                return;
            _downBlock = _level.QuadTreePointFilter<AutoBlock>(new Vec2(position.x, bottom + 2f), checkFilter);
            if (_downBlock == null)
                return;
            _downBlock.InitializeNeighbors();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            if (Editor.saving)
            {
                _processedByEditor = true;
                InitializeNeighbors();
                if (upBlock != null && !upBlock.processedByEditor)
                    binaryClassChunk.AddProperty("north", upBlock.Serialize());
                if (downBlock != null && !downBlock.processedByEditor)
                    binaryClassChunk.AddProperty("north", downBlock.Serialize());
                if (rightBlock != null && !rightBlock.processedByEditor)
                    binaryClassChunk.AddProperty("east", rightBlock.Serialize());
                if (leftBlock != null && !leftBlock.processedByEditor)
                    binaryClassChunk.AddProperty("west", leftBlock.Serialize());
            }
            binaryClassChunk.AddProperty("frame", _sprite.frame);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            if (Editor.saving)
            {
                _additionalBlocks.Clear();
                _neighborsInitialized = true;
                BinaryClassChunk property1 = node.GetProperty<BinaryClassChunk>("north");
                if (property1 != null)
                {
                    AutoBlock autoBlock = LoadThing(property1) as AutoBlock;
                    _upBlock = autoBlock;
                    autoBlock._downBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property2 = node.GetProperty<BinaryClassChunk>("south");
                if (property2 != null)
                {
                    AutoBlock autoBlock = LoadThing(property2) as AutoBlock;
                    _downBlock = autoBlock;
                    autoBlock._upBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property3 = node.GetProperty<BinaryClassChunk>("east");
                if (property3 != null)
                {
                    AutoBlock autoBlock = LoadThing(property3) as AutoBlock;
                    _rightBlock = autoBlock;
                    autoBlock._leftBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                BinaryClassChunk property4 = node.GetProperty<BinaryClassChunk>("west");
                if (property4 != null)
                {
                    AutoBlock autoBlock = LoadThing(property4) as AutoBlock;
                    _leftBlock = autoBlock;
                    autoBlock._rightBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
            }
            _sprite.frame = node.GetProperty<int>("frame");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            if (Editor.saving)
            {
                _processedByEditor = true;
                InitializeNeighbors();
                if (upBlock != null && !upBlock.processedByEditor)
                    new DXMLNode("north").Add(upBlock.LegacySerialize());
                if (downBlock != null && !downBlock.processedByEditor)
                    new DXMLNode("south").Add(downBlock.LegacySerialize());
                if (rightBlock != null && !rightBlock.processedByEditor)
                    new DXMLNode("east").Add(rightBlock.LegacySerialize());
                if (leftBlock != null && !leftBlock.processedByEditor)
                    new DXMLNode("west").Add(leftBlock.LegacySerialize());
            }
            dxmlNode.Add(new DXMLNode("frame", _sprite.frame));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            if (Editor.saving)
            {
                _additionalBlocks.Clear();
                _neighborsInitialized = true;
                DXMLNode node1 = node.Element("north");
                if (node1 != null)
                {
                    AutoBlock autoBlock = LegacyLoadThing(node1) as AutoBlock;
                    _upBlock = autoBlock;
                    autoBlock._downBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                DXMLNode node2 = node.Element("south");
                if (node2 != null)
                {
                    AutoBlock autoBlock = LegacyLoadThing(node2) as AutoBlock;
                    _downBlock = autoBlock;
                    autoBlock._upBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                DXMLNode node3 = node.Element("east");
                if (node3 != null)
                {
                    AutoBlock autoBlock = LegacyLoadThing(node3) as AutoBlock;
                    _rightBlock = autoBlock;
                    autoBlock._leftBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
                DXMLNode node4 = node.Element("west");
                if (node4 != null)
                {
                    AutoBlock autoBlock = LegacyLoadThing(node4) as AutoBlock;
                    _leftBlock = autoBlock;
                    autoBlock._rightBlock = this;
                    _additionalBlocks.Add(autoBlock);
                }
            }
            DXMLNode dxmlNode = node.Element("frame");
            if (dxmlNode != null)
                _sprite.frame = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override void Added(Level parent)
        {
            foreach (Thing additionalBlock in _additionalBlocks)
                Level.Add(additionalBlock);
            _additionalBlocks.Clear();
            base.Added(parent);
        }

        public AutoBlock(float x, float y, string tileset)
          : base(x, y)
        {
            checkFilter = blok => blok != this && (blok as AutoBlock)._tileset == _tileset;
            if (tileset == null)
                tileset = "";
            if (tileset != "")
            {
                _sprite = new SpriteMap(tileset, 16, 16)
                {
                    frame = 40
                };
            }
            _tileset = tileset;
            graphic = _sprite;
            collisionSize = new Vec2(16f, 16f);
            thickness = 10f;
            centerx = 8f;
            centery = 8f;
            collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.4f;
            flammable = 0.8f;
            _isStatic = true;
            _canBeGrouped = true;
            layer = Layer.Blocks;
            _impactThreshold = 100f;
            blockIndex = _kBlockIndex;
            ++_kBlockIndex;
            _placementCost = 1;
        }

        public void RegisterHit(Vec2 hitPos, bool neighbors = false)
        {
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            RegisterHit(hitPos, true);
            return base.Hit(bullet, hitPos);
        }

        protected override bool OnBurn(Vec2 position, Thing litBy) => false;

        public override void EditorObjectsChanged()
        {
            //this.inObjectsChanged = true;
            PlaceBlock();
            //this.inObjectsChanged = false;
        }

        public void DestroyPuddle(FluidPuddle f)
        {
            if (f == null || f.removeFromLevel)
                return;
            int num1 = (int)(f.collisionSize.x / 8f);
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
        private static AutoBlock CheckBlockGroup(BlockGroup blockGroup, Block Ignore, Vec2 Point)
        {
            AutoBlock block = null;
            if (blockGroup.blocks == null)
            {
                return null;
            }
            foreach (Block thing in blockGroup.blocks)
            {
                AutoBlock autoBlock = thing as AutoBlock;
                if (autoBlock != null && !thing.removeFromLevel && thing != Ignore && Collision.Point(Point, thing))
                {
                    block = autoBlock;
                    break;
                }
            }
            return block;
        }
        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTRocketExplosion))
            {
                return false;
            }
            if (up == null)
            {
                up = Level.CheckPoint<AutoBlock>(x, y - 16f, this);
            }
            else if (up is BlockGroup)
            {
                up = CheckBlockGroup((up as BlockGroup), this, new Vec2(x, y - 16f));
            }
            if (down == null)
            {
                down = Level.CheckPoint<AutoBlock>(x, y + 16f, this);
            }
            else if (down is BlockGroup)
            {
                down = CheckBlockGroup((down as BlockGroup), this, new Vec2(x, y + 16f));
            }
            if (bLeft == null)
            {
                bLeft = Level.CheckPoint<AutoBlock>(x - 16f, y, this);
            }
            else if (bLeft is BlockGroup)
            {
                bLeft = CheckBlockGroup((bLeft as BlockGroup), this, new Vec2(x - 16f, y));
            }
            if (bRight == null)
            {
                bRight = Level.CheckPoint<AutoBlock>(x + 16f, y, this);
            }
            else if (bRight is BlockGroup)
            {
                bRight = CheckBlockGroup((bRight as BlockGroup), this, new Vec2(x + 16f, y));
            }
            if (up != null && up._tileset == _tileset)
            {
                up.brokeDown = true;
                up.hasBroke = true;
                up.downBlock = null;
                up.down = null;
            }
            if (down != null && down._tileset == _tileset)
            {
                down.brokeUp = true;
                down.hasBroke = true;
                down.upBlock = null;
                down.up = null;
            }
            if (bLeft != null && bLeft._tileset == _tileset)
            {
                bLeft.brokeRight = true;
                bLeft.hasBroke = true;
                bLeft.rightBlock = null;
                bLeft.bRight = null;
            }
            if (bRight != null && bRight._tileset == _tileset)
            {
                bRight.brokeLeft = true;
                bRight.hasBroke = true;
                bRight.leftBlock = null;
                bRight.bLeft = null;
            }
            if (structure != null)
            {
                foreach (Block block in structure.blocks)
                {
                    block.hit = false;
                    block.structure = null;
                }
                hit = false;
                structure = null;
            }
            if (up == null)
                DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(x, y - 9f)));
            if (bLeft == null)
                DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(x - 9f, y)));
            if (bRight == null)
                DestroyPuddle(Level.CheckPoint<FluidPuddle>(new Vec2(x + 9f, y)));
            return true;
        }

        public void UpdateNubbers()
        {
            TerminateNubs();
            if (!_hasNubs || removeFromLevel)
                return;
            switch (_sprite.frame)
            {
                case 1:
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 2:
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 4:
                    if (_hasRightNub)
                    {
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        break;
                    }
                    break;
                case 5:
                    if (_hasRightNub)
                    {
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        break;
                    }
                    break;
                case 32:
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 37:
                    if (_hasRightNub)
                    {
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        break;
                    }
                    break;
                case 40:
                    if (_hasRightNub)
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 41:
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 43:
                    if (_hasRightNub)
                    {
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        break;
                    }
                    break;
                case 49:
                    if (_hasRightNub)
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 51:
                    if (_hasLeftNub)
                    {
                        _bLeftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        break;
                    }
                    break;
                case 52:
                    if (_hasRightNub)
                    {
                        _bRightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        break;
                    }
                    break;
            }
            if (_bLeftNub != null)
            {
                Level.Add(_bLeftNub);
                _bLeftNub.depth = depth;
                _bLeftNub.layer = layer;
                _bLeftNub.material = material;
            }
            if (_bRightNub == null)
                return;
            Level.Add(_bRightNub);
            _bRightNub.depth = depth;
            _bRightNub.layer = layer;
            _bRightNub.material = material;
        }

        public override void Update()
        {
            if (skipWreck)
            {
                skipWreck = false;
                if (!shouldbeinupdateloop)
                {
                    _level.AddUpdateOnce(this);
                }
            }
            else
            {
                if (shouldWreck)
                {
                    Destroy(new DTRocketExplosion(null));
                    Level.Remove(this);
                }
            }
            if (needsRefresh)
            {
                PlaceBlock();
                needsRefresh = false;
                //this.neededRefresh = true;
            }
            //if (setLayer)
            //    layer = Layer.Blocks;
            base.Update();
        }

        public BlockGroup GroupWithNeighbors(bool addToLevel = true)
        {
            if (_groupedWithNeighbors)
                return null;
            _groupedWithNeighbors = true;
            AutoBlock autoBlock1 = leftBlock as AutoBlock;
            AutoBlock autoBlock2 = rightBlock as AutoBlock;
            List<AutoBlock> autoBlockList1 = new List<AutoBlock>();
            autoBlockList1.Add(this);
            while (autoBlock1 != null && !autoBlock1._groupedWithNeighbors)
            {
                if (autoBlock1.collisionSize.y == collisionSize.y && autoBlock1.collisionOffset.y == collisionOffset.y)
                {
                    autoBlockList1.Add(autoBlock1);
                    autoBlock1 = autoBlock1.leftBlock as AutoBlock;
                }
                else
                    autoBlock1 = null;
            }
            while (autoBlock2 != null && !autoBlock2._groupedWithNeighbors)
            {
                if (autoBlock2.collisionSize.y == collisionSize.y && autoBlock2.collisionOffset.y == collisionOffset.y)
                {
                    autoBlockList1.Add(autoBlock2);
                    autoBlock2 = autoBlock2.rightBlock as AutoBlock;
                }
                else
                    autoBlock2 = null;
            }
            List<AutoBlock> autoBlockList2 = new List<AutoBlock>();
            autoBlockList2.Add(this);
            AutoBlock autoBlock3 = upBlock as AutoBlock;
            AutoBlock autoBlock4 = downBlock as AutoBlock;
            while (autoBlock3 != null && !autoBlock3._groupedWithNeighbors)
            {
                if (autoBlock3.collisionSize.x == collisionSize.x && autoBlock3.collisionOffset.x == collisionOffset.x)
                {
                    autoBlockList2.Add(autoBlock3);
                    autoBlock3 = autoBlock3.upBlock as AutoBlock;
                }
                else
                    autoBlock3 = null;
            }
            while (autoBlock4 != null && !autoBlock4._groupedWithNeighbors)
            {
                if (autoBlock4.collisionSize.x == collisionSize.x && autoBlock4.collisionOffset.x == collisionOffset.x)
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
        public override void PreLevelInitialize()
        {
        }
        public override void Initialize()
        {
            if (_sprite != null)
                UpdateCollision();
            DoPositioning();
            if (ModLoader.ShouldOptimizations)
            {
                _level.AddUpdateOnce(this);
                shouldbeinupdateloop = false;
            }
        }

        public virtual void DoPositioning()
        {
            //  if (Level.current is Editor || graphic == null)
            //    return;
            //if (!RandomLevelNode.editorLoad)
            cheap = true;
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            (graphic as SpriteMap).ClearCache();
            (graphic as SpriteMap).UpdateFrame();
        }

        public override void Terminate()
        {
            if (!_groupedWithNeighbors && shouldWreck || Level.current is Editor)
            {
                TerminateNubs();
            }
        }

        private void TerminateNubs()
        {
            if (_bLeftNub != null)
            {
                Level.Remove(_bLeftNub);
                _bLeftNub = null;
            }
            if (_bRightNub != null)
            {
                Level.Remove(_bRightNub);
                _bRightNub = null;
            }
        }

        public override void Draw()
        {
            if (DevConsole.showCollision)
            {
                if (leftBlock != null)
                    Graphics.DrawLine(position, position + new Vec2(-8f, 0f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (rightBlock != null)
                    Graphics.DrawLine(position, position + new Vec2(8f, 0f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (upBlock != null)
                    Graphics.DrawLine(position, position + new Vec2(0f, -8f), Color.Red * 0.5f, depth: ((Depth)1f));
                if (downBlock != null)
                    Graphics.DrawLine(position, position + new Vec2(0f, 8f), Color.Red * 0.5f, depth: ((Depth)1f));
            }
            if (hasBroke)
            {
                if (_brokenSprite == null)
                {
                    _brokenSprite = new Sprite("brokeEdge");
                    _brokenSprite.CenterOrigin();
                    _brokenSprite.depth = depth;
                }
                if (brokeLeft)
                {
                    _brokenSprite.angle = 3.14159f;
                    Graphics.Draw(ref _brokenSprite, x - 16f, y);
                }
                if (brokeRight)
                {
                    _brokenSprite.angle = 0f;
                    Graphics.Draw(ref _brokenSprite, x + 16f, y);
                }
                if (brokeUp)
                {
                    _brokenSprite.angle = 4.71239f;
                    Graphics.Draw(ref _brokenSprite, x, y - 16f);
                }
                if (brokeDown)
                {
                    _brokenSprite.angle = 1.57079f;
                    Graphics.Draw(ref _brokenSprite, x, y + 16f);
                }
            }
            if (graphic.position != position)
            {
                (graphic as SpriteMap).ClearCache();
            }
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            graphic.cheapmaterial = material;
            (graphic as SpriteMap).UpdateFrame();
            graphic.UltraCheapStaticDraw(flipHorizontal);
            //  graphic.Draw() FUCK NORMAL DRAWING I AM CHEAP BASTERD 
        }

        public void UpdateCollision()
        {
            switch (_sprite.frame)
            {
                case 32:
                case 41:
                case 51:
                case 53:
                case 58:
                    collisionSize = new Vec2((8f + verticalWidth / 2f), 16f);
                    collisionOffset = new Vec2((-verticalWidth / 2f), -8f);
                    break;
                case 37:
                case 43:
                case 45:
                case 52:
                case 60:
                    collisionSize = new Vec2((8f + verticalWidth / 2f), 16f);
                    collisionOffset = new Vec2(-8f, -8f);
                    break;
                case 40:
                case 44:
                case 49:
                case 50:
                    collisionSize = new Vec2(verticalWidth, 16f);
                    collisionOffset = new Vec2((-verticalWidth / 2f), -8f);
                    break;
                default:
                    collisionSize = new Vec2(16f, 16f);
                    collisionOffset = new Vec2(-8f, -8f);
                    break;
            }
            switch (_sprite.frame)
            {
                case 1:
                case 2:
                case 7:
                case 18:
                case 26:
                    _collisionSize.x = verticalWidthThick;
                    _collisionOffset.x = (float)(16f - verticalWidthThick - 8f);
                    break;
                case 4:
                case 5:
                case 15:
                case 20:
                case 28:
                    _collisionSize.x = verticalWidthThick;
                    _collisionOffset.x = -8f;
                    break;
            }
            switch (_sprite.frame)
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
                    _collisionSize.y = horizontalHeight;
                    break;
                default:
                    _collisionSize.y = 16f;
                    break;
            }
        }

        public void PlaceBlock()
        {
            if (_sprite == null)
                return;
            _placed = true;
            FindFrame();
            UpdateNubbers();
            UpdateCollision();
            DoPositioning();
        }

        public void FindFrame()
        {
            up = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x, y - 16f), checkFilter);
            down = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x, y + 16f), checkFilter);
            bLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x - 16f, y), checkFilter);
            bRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x + 16f, y), checkFilter);
            topbLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x - 16f, y - 16f), checkFilter);
            topbRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x + 16f, y - 16f), checkFilter);
            bottombLeft = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x - 16f, y + 16f), checkFilter);
            bottombRight = Level.current.QuadTreePointFilter<AutoBlock>(new Vec2(x + 16f, y + 16f), checkFilter);
            if (up != null)
            {
                if (bRight != null)
                {
                    if (down != null)
                    {
                        if (bLeft != null)
                        {
                            if (topbLeft != null)
                            {
                                if (topbRight != null)
                                {
                                    if (bottombLeft != null)
                                    {
                                        if (bottombRight != null)
                                            _sprite.frame = 11;
                                        else
                                            _sprite.frame = 21;
                                    }
                                    else if (bottombRight != null)
                                        _sprite.frame = 17;
                                    else
                                        _sprite.frame = 23;
                                }
                                else if (bottombRight != null)
                                {
                                    if (bottombLeft == null)
                                        return;
                                    _sprite.frame = 12;
                                }
                                else if (bottombLeft != null)
                                    _sprite.frame = 22;
                                else
                                    _sprite.frame = 30;
                            }
                            else if (topbRight != null)
                            {
                                if (bottombRight != null)
                                {
                                    if (bottombLeft != null)
                                        _sprite.frame = 10;
                                    else
                                        _sprite.frame = 16;
                                }
                                else
                                {
                                    _sprite.frame = 24;
                                }
                            }
                            else if (bottombRight != null)
                            {
                                if (bottombLeft != null)
                                    _sprite.frame = 3;
                                else
                                    _sprite.frame = 8;
                            }
                            else
                            {
                                if (bottombLeft != null)
                                    return;
                                _sprite.frame = 42;
                            }
                        }
                        else if (topbRight != null)
                        {
                            if (bottombRight != null)
                            {
                                _sprite.frame = 18;
                            }
                            else
                            {
                                if (topbLeft == null)
                                {
                                }
                                _sprite.frame = 7;
                            }
                        }
                        else
                        {
                            if (topbLeft == null)
                            {
                                if (bottombRight != null)
                                {
                                    _sprite.frame = 2;
                                    return;
                                }
                            }
                            _sprite.frame = 53;
                        }
                    }
                    else if (bLeft != null)
                    {
                        if (topbLeft != null)
                        {
                            if (topbRight != null)
                                _sprite.frame = 27;
                            else
                                _sprite.frame = 29;
                        }
                        else if (topbRight != null)
                            _sprite.frame = 25;
                        else
                            _sprite.frame = 57;
                    }
                    else if (topbRight != null)
                        _sprite.frame = 26;
                    else
                        _sprite.frame = 58;
                }
                else if (down != null)
                {
                    if (bLeft != null)
                    {
                        if (topbLeft != null)
                        {
                            if (bottombLeft != null)
                            {
                                _sprite.frame = 20;
                            }
                            else
                            {
                                if (bottombRight == null)
                                {
                                }
                                _sprite.frame = 15;
                            }
                        }
                        else
                        {
                            if (topbRight == null)
                            {
                                if (bottombRight != null)
                                {
                                    if (bottombLeft != null)
                                    {
                                        _sprite.frame = 4;
                                        return;
                                    }
                                    _sprite.frame = 45;
                                    return;
                                }
                                if (bottombLeft != null)
                                {
                                    _sprite.frame = 4;
                                    return;
                                }
                            }
                            _sprite.frame = 45;
                        }
                    }
                    else
                        _sprite.frame = 50;
                }
                else if (bLeft != null)
                {
                    if (topbLeft != null)
                        _sprite.frame = 28;
                    else
                        _sprite.frame = 60;
                }
                else
                    _sprite.frame = 44;
            }
            else if (bRight != null)
            {
                if (down != null)
                {
                    if (bLeft != null)
                    {
                        if (bottombLeft == null && bottombRight == null)
                            _sprite.frame = 34;
                        else if (topbLeft != null)
                        {
                            if (topbRight != null)
                                _sprite.frame = 3;
                            else if (bottombRight != null)
                            {
                                if (bottombLeft == null)
                                    return;
                                _sprite.frame = 3;
                            }
                            else if (bottombLeft != null)
                                _sprite.frame = 6;
                            else
                                _sprite.frame = 24;
                        }
                        else if (topbRight != null)
                        {
                            if (bottombRight != null)
                            {
                                if (bottombLeft != null)
                                    _sprite.frame = 3;
                                else
                                    _sprite.frame = 0;
                            }
                            else
                            {
                                if (bottombLeft != null)
                                    return;
                                _sprite.frame = 25;
                            }
                        }
                        else if (bottombRight != null)
                        {
                            if (bottombLeft != null)
                                _sprite.frame = 3;
                            else
                                _sprite.frame = 8;
                        }
                        else if (bottombLeft != null)
                            _sprite.frame = 14;
                        else
                            _sprite.frame = 34;
                    }
                    else if (topbLeft == null && topbRight != null && bottombLeft != null && bottombRight != null)
                        _sprite.frame = 1;
                    else if (bottombRight != null)
                        _sprite.frame = 2;
                    else
                        _sprite.frame = 51;
                }
                else if (bLeft != null)
                {
                    if ((bottombLeft != null || topbLeft != null) && (topbRight != null || bottombRight != null))
                        _sprite.frame = 59;
                    else if (bottombRight != null || topbRight != null)
                        _sprite.frame = 33;
                    else if (bottombLeft != null || topbLeft != null)
                        _sprite.frame = 35;
                    else
                        _sprite.frame = 36;
                }
                else if (bottombRight != null || topbRight != null)
                    _sprite.frame = 41;
                else
                    _sprite.frame = 32;
            }
            else if (down != null)
            {
                if (bLeft != null)
                {
                    if (topbLeft != null)
                    {
                        if (topbRight == null)
                        {
                            if (bottombLeft != null)
                            {
                                if (bottombRight != null)
                                {
                                    _sprite.frame = 5;
                                    return;
                                }
                                _sprite.frame = 4;
                                return;
                            }
                            _sprite.frame = 52;
                            return;
                        }
                    }
                    else if (topbRight == null)
                    {
                        if (bottombLeft != null)
                        {
                            _sprite.frame = 4;
                            return;
                        }
                    }
                    _sprite.frame = 52;
                }
                else
                    _sprite.frame = 49;
            }
            else if (bLeft != null)
            {
                if (bottombLeft != null || topbLeft != null)
                    _sprite.frame = 43;
                else
                    _sprite.frame = 37;
            }
            else
                _sprite.frame = 40;
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
