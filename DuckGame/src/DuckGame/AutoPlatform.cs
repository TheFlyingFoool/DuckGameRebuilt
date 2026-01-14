
namespace DuckGame
{
    public abstract class AutoPlatform :
    MaterialThing,
    IAutoTile,
    IPlatform,
    IDontMove,
    IPathNodeBlocker
    {
        protected SpriteMap _sprite;
        public Nubber _leftNub;
        public Nubber _rightNub;
        protected string _tileset;
        public float verticalWidth = 16f;
        public float verticalWidthThick = 16f;
        public float horizontalHeight = 16f;
        protected bool _hasNubs = true;
        protected bool _init50;
        protected bool _collideBottom;
        protected bool _neighborsInitialized;
        protected AutoPlatform _leftBlock;
        protected AutoPlatform _rightBlock;
        protected AutoPlatform _upBlock;
        protected AutoPlatform _downBlock;
        private bool _pathed;
        public bool treeLike;
        public bool cheap;
        public bool neverCheap;
        public bool needsRefresh;
        public bool _hasLeftNub = true;
        public bool _hasRightNub = true;
        private Color _prevcolor = Color.White;

        public AutoPlatform leftBlock => _leftBlock;

        public AutoPlatform rightBlock => _rightBlock;

        public AutoPlatform upBlock => _upBlock;

        public AutoPlatform downBlock => _downBlock;

        public override void SetTranslation(Vec2 translation)
        {
            if (_leftNub != null)
                _leftNub.SetTranslation(translation);
            if (_rightNub != null)
                _rightNub.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public override void Draw()
        {
            flipHorizontal = false;
            //if (cheap && !Editor.editorDraw)
            //    graphic.UltraCheapStaticDraw(flipHorizontal);
            //else
            //    base.Draw();
            if (graphic.position != position || _prevcolor != graphic.color)
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

        public bool pathed
        {
            get => _pathed;
            set => _pathed = value;
        }

        public override int frame
        {
            get => _sprite.frame;
            set
            {
                _sprite.frame = value;
                UpdateNubbers();
                UpdateCollision();
                DoPositioning();
            }
        }

        public AutoPlatform(float x, float y, string tileset)
          : base(x, y)
        {
            if (tileset != "")
                _sprite = new SpriteMap(tileset, 16, 16);
            _tileset = tileset;
            graphic = _sprite;
            collisionSize = new Vec2(16f, 16f);
            thickness = 0.2f;
            centerx = 8f;
            centery = 8f;
            collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.3f;
            _canBeGrouped = true;
            _isStatic = true;
            _placementCost = 1;
        }

        public virtual void InitializeNeighbors()
        {
            if (_neighborsInitialized)
                return;
            _leftBlock = Level.CheckPointPlacementLayer<AutoPlatform>(left - 2f, position.y, this, placementLayer);
            _rightBlock = Level.CheckPointPlacementLayer<AutoPlatform>(right + 2f, position.y, this, placementLayer);
            _upBlock = Level.CheckPointPlacementLayer<AutoPlatform>(position.x, top - 2f, this, placementLayer);
            _downBlock = Level.CheckPointPlacementLayer<AutoPlatform>(position.x, bottom + 2f, this, placementLayer);
            _neighborsInitialized = true;
        }

        public override void Initialize()
        {
            DoPositioning();
            if (ModLoader.ShouldOptimizations)
            {
                _level.AddUpdateOnce(this);
                shouldbeinupdateloop = false;
            }
        }
        public virtual void DoPositioning()
        {
            //if (Level.current is Editor || graphic == null)
            //    return;
            if (graphic == null)
                return;
            cheap = !neverCheap && !RandomLevelNode.editorLoad;
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            (graphic as SpriteMap).ClearCache();
            (graphic as SpriteMap).UpdateFrame();
            if (_leftNub != null)
            {
                _leftNub.cheap = cheap;
                _leftNub.DoPositioning();
            }
            if (_rightNub == null)
                return;
            _rightNub.cheap = cheap;
            _rightNub.DoPositioning();
        }
        public override void PreLevelInitialize()
        {
            // this.UpdatePlatform();
        }
        public override void EditorObjectsChanged() => PlaceBlock();

        public virtual bool HasNoCollision() => _sprite.frame == 50;

        public virtual void UpdatePlatform()
        {
            if (needsRefresh)
            {
                PlaceBlock();
                if ((_sprite.frame == 50 || (_sprite.frame == 44 || _sprite.frame == 26 || _sprite.frame == 27 || _sprite.frame == 28) && !_collideBottom || _sprite.frame == 10 || _sprite.frame == 11 || _sprite.frame == 12 || _sprite.frame == 18 || _sprite.frame == 19 || _sprite.frame == 20 || _sprite.frame == 6 || _sprite.frame == 8 || _sprite.frame == 14 || _sprite.frame == 16 || _sprite.frame == 22) && !_init50 && (treeLike || _sprite.frame != 8 && _sprite.frame != 14 && _sprite.frame != 16 && _sprite.frame != 22))
                {
                    solid = false;
                    _init50 = true;
                }
                needsRefresh = false;
            }
            if (!_placed)
            {
                PlaceBlock();
            }
            else
            {
                if (_sprite.frame != 50 && (_sprite.frame != 44 && _sprite.frame != 26 && _sprite.frame != 27 && _sprite.frame != 28 || _collideBottom) && _sprite.frame != 10 && _sprite.frame != 11 && _sprite.frame != 12 && _sprite.frame != 18 && _sprite.frame != 19 && _sprite.frame != 20 && _sprite.frame != 6 && _sprite.frame != 8 && _sprite.frame != 14 && _sprite.frame != 16 && _sprite.frame != 22 || _init50 || !treeLike && (_sprite.frame == 8 || _sprite.frame == 14 || _sprite.frame == 16 || _sprite.frame == 22))
                    return;
                solid = false;
                _init50 = true;
            }
        }

        public override void Update()
        {
            UpdatePlatform();
            base.Update();
        }

        public override void Terminate() => TerminateNubs();

        private void TerminateNubs()
        {
            if (_leftNub != null)
            {
                Level.Remove(_leftNub);
                _leftNub = null;
            }
            if (_rightNub == null)
                return;
            Level.Remove(_rightNub);
            _rightNub = null;
        }

        public void PlaceBlock()
        {
            _placed = true;
            FindFrame();
            UpdateCollision();
            DoPositioning();
            UpdateNubbers();
        }

        public virtual void UpdateCollision()
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
                    _collisionOffset.x = 16f - verticalWidthThick - 8f;
                    break;
                case 4:
                case 5:
                case 15:
                case 20:
                case 28:
                    _collisionSize.x = verticalWidthThick;
                    _collisionOffset.x = -8f;
                    break;
                case 8:
                case 14:
                case 16:
                case 22:
                    if (!treeLike)
                    {
                        _collisionSize.x = verticalWidthThick;
                        _collisionOffset.x = (16f - verticalWidthThick - 8f);
                        break;
                    }
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

        public virtual void UpdateNubbers()
        {
            TerminateNubs();
            if (!_hasNubs || removeFromLevel)
                return;
            switch (_sprite.frame)
            {
                case 2:
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 4:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 32:
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 37:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 40:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                    }
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 41:
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 43:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 49:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                    }
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 51:
                    if (_hasLeftNub)
                    {
                        _leftNub = new Nubber(x - 24f, y - 8f, true, _tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 52:
                    if (_hasRightNub)
                    {
                        _rightNub = new Nubber(x + 8f, y - 8f, false, _tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
            }
            if (_leftNub != null)
            {
                _leftNub.depth = depth;
                _leftNub.material = material;
            }
            if (_rightNub == null)
                return;
            _rightNub.depth = depth;
            _rightNub.material = material;
        }

        public static readonly byte[] neighborFrameLookupList = new byte[256]
{
            40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40,
            37, 37, 43, 43, 37, 37, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43,
            49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49,
            52, 52, 4, 4, 52, 52, 52, 52, 52, 52, 4, 5, 52, 52, 52, 52,
            32, 41, 32, 41, 41, 41, 41, 41, 32, 41, 32, 41, 41, 41, 41, 41,
            36, 33, 35, 59, 33, 33, 59, 59, 35, 59, 35, 59, 59, 59, 59, 59,
            51, 2, 51, 2, 51, 2, 51, 1, 51, 2, 51, 2, 51, 2, 51, 2,
            34, 8, 14, 3, 34, 0, 255, 3, 34, 255, 6, 3, 34, 3, 3, 3,
            44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 44,
            60, 60, 60, 60, 60, 60, 60, 60, 28, 28, 28, 28, 28, 28, 28, 28,
            50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50,
            45, 45, 4, 4, 45, 45, 45, 45, 15, 15, 20, 20, 15, 15, 20, 20,
            58, 58, 58, 58, 26, 26, 26, 26, 58, 58, 58, 58, 26, 26, 26, 26,
            57, 57, 57, 57, 25, 25, 25, 25, 29, 29, 29, 29, 27, 27, 27, 27,
            53, 2, 53, 2, 7, 18, 7, 18, 53, 53, 53, 53, 7, 18, 7, 18,
            42, 8, 255, 3, 24, 16, 24, 10, 30, 255, 22, 12, 23, 17, 21, 11
};
        public virtual void FindFrame()
        {
            float num = 16f;
            if (!treeLike)
                num = 10f;
            AutoPlatform up = Level.CheckPointPlacementLayer<AutoPlatform>(x, y - 17f, this, placementLayer);
            AutoPlatform down = Level.CheckPointPlacementLayer<AutoPlatform>(x, y + num, this, placementLayer);
            AutoPlatform bLeft = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y, this, placementLayer);
            AutoPlatform bRight = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y, this, placementLayer);
            AutoPlatform topbLeft = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y - 17f, this, placementLayer);
            AutoPlatform topbRight = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y - 17f, this, placementLayer);
            AutoPlatform bottombLeft = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y + num, this, placementLayer);
            AutoPlatform bottombRight = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y + num, this, placementLayer);

            if (up != null && up._tileset != _tileset) up = null;
            if (down != null && down._tileset != _tileset) down = null;
            if (bLeft != null && bLeft._tileset != _tileset) bLeft = null;
            if (bRight != null && bRight._tileset != _tileset) bRight = null;
            if (topbLeft != null && topbLeft._tileset != _tileset) topbLeft = null;
            if (topbRight != null && topbRight._tileset != _tileset) topbRight = null;
            if (bottombLeft != null && bottombLeft._tileset != _tileset) bottombLeft = null;
            if (bottombRight != null && bottombRight._tileset != _tileset) bottombRight = null;

            bool[] neighbors = new bool[8] { up != null, bRight != null, down != null, bLeft != null, topbLeft != null, topbRight != null, bottombLeft != null, bottombRight != null };
            byte neighborValue = 0;
            for (int i = neighbors.Length - 1; i >= 0; i--)
            {
                if (neighbors[i])
                {
                    neighborValue |= (byte)(1 << (neighbors.Length - 1 - i));
                }
            }
            byte newFrame = neighborFrameLookupList[neighborValue];
            if (newFrame != 255)
                _sprite.frame = newFrame;
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
