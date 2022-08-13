// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoPlatform
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (cheap)
                graphic.UltraCheapStaticDraw(flipHorizontal);
            else
                base.Draw();
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
            _level.AddUpdateOnce(this);
            shouldbeinupdateloop = false;
        }
        public virtual void DoPositioning()
        {
            if (Level.current is Editor || graphic == null)
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

        public virtual void FindFrame()
        {
            float num = 16f;
            if (!treeLike)
                num = 10f;
            AutoPlatform autoPlatform1 = Level.CheckPointPlacementLayer<AutoPlatform>(x, y - 17f, this, placementLayer);
            AutoPlatform autoPlatform2 = Level.CheckPointPlacementLayer<AutoPlatform>(x, y + num, this, placementLayer);
            AutoPlatform autoPlatform3 = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y, this, placementLayer);
            AutoPlatform autoPlatform4 = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y, this, placementLayer);
            AutoPlatform autoPlatform5 = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y - 17f, this, placementLayer);
            AutoPlatform autoPlatform6 = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y - 17f, this, placementLayer);
            AutoPlatform autoPlatform7 = Level.CheckPointPlacementLayer<AutoPlatform>(x - 16f, y + num, this, placementLayer);
            AutoPlatform autoPlatform8 = Level.CheckPointPlacementLayer<AutoPlatform>(x + 16f, y + num, this, placementLayer);
            if (autoPlatform1 != null && autoPlatform1._tileset != _tileset)
                autoPlatform1 = null;
            if (autoPlatform2 != null && autoPlatform2._tileset != _tileset)
                autoPlatform2 = null;
            if (autoPlatform3 != null && autoPlatform3._tileset != _tileset)
                autoPlatform3 = null;
            if (autoPlatform4 != null && autoPlatform4._tileset != _tileset)
                autoPlatform4 = null;
            if (autoPlatform5 != null && autoPlatform5._tileset != _tileset)
                autoPlatform5 = null;
            if (autoPlatform6 != null && autoPlatform6._tileset != _tileset)
                autoPlatform6 = null;
            if (autoPlatform7 != null && autoPlatform7._tileset != _tileset)
                autoPlatform7 = null;
            if (autoPlatform8 != null && autoPlatform8._tileset != _tileset)
                autoPlatform8 = null;
            if (autoPlatform1 != null)
            {
                if (autoPlatform4 != null)
                {
                    if (autoPlatform2 != null)
                    {
                        if (autoPlatform3 != null)
                        {
                            if (autoPlatform5 != null)
                            {
                                if (autoPlatform6 != null)
                                {
                                    if (autoPlatform7 != null)
                                    {
                                        if (autoPlatform8 != null)
                                            _sprite.frame = 11;
                                        else
                                            _sprite.frame = 21;
                                    }
                                    else if (autoPlatform8 != null)
                                        _sprite.frame = 17;
                                    else
                                        _sprite.frame = 23;
                                }
                                else if (autoPlatform8 != null)
                                {
                                    if (autoPlatform7 == null)
                                        return;
                                    _sprite.frame = 12;
                                }
                                else if (autoPlatform7 != null)
                                    _sprite.frame = 22;
                                else
                                    _sprite.frame = 30;
                            }
                            else if (autoPlatform6 != null)
                            {
                                if (autoPlatform8 != null)
                                {
                                    if (autoPlatform7 != null)
                                        _sprite.frame = 10;
                                    else
                                        _sprite.frame = 16;
                                }
                                else
                                    _sprite.frame = 24;
                            }
                            else if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 != null)
                                    _sprite.frame = 3;
                                else
                                    _sprite.frame = 8;
                            }
                            else
                            {
                                if (autoPlatform7 != null)
                                    return;
                                _sprite.frame = 42;
                            }
                        }
                        else if (autoPlatform6 != null)
                        {
                            if (autoPlatform8 != null)
                            {
                                _sprite.frame = 18;
                            }
                            else
                            {
                                if (autoPlatform5 == null)
                                    ;
                                _sprite.frame = 7;
                            }
                        }
                        else
                        {
                            if (autoPlatform5 == null)
                            {
                                if (autoPlatform8 != null)
                                {
                                    _sprite.frame = 2;
                                    return;
                                }
                            }
                            _sprite.frame = 53;
                        }
                    }
                    else if (autoPlatform3 != null)
                    {
                        if (autoPlatform5 != null)
                        {
                            if (autoPlatform6 != null)
                                _sprite.frame = 27;
                            else
                                _sprite.frame = 29;
                        }
                        else if (autoPlatform6 != null)
                            _sprite.frame = 25;
                        else
                            _sprite.frame = 57;
                    }
                    else if (autoPlatform6 != null)
                        _sprite.frame = 26;
                    else
                        _sprite.frame = 58;
                }
                else if (autoPlatform2 != null)
                {
                    if (autoPlatform3 != null)
                    {
                        if (autoPlatform5 != null)
                        {
                            if (autoPlatform7 != null)
                            {
                                _sprite.frame = 20;
                            }
                            else
                            {
                                if (autoPlatform8 == null)
                                    ;
                                _sprite.frame = 15;
                            }
                        }
                        else
                        {
                            if (autoPlatform6 == null)
                            {
                                if (autoPlatform8 != null)
                                {
                                    if (autoPlatform7 != null)
                                    {
                                        _sprite.frame = 4;
                                        return;
                                    }
                                    _sprite.frame = 45;
                                    return;
                                }
                                if (autoPlatform7 != null)
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
                else if (autoPlatform3 != null)
                {
                    if (autoPlatform5 != null)
                        _sprite.frame = 28;
                    else
                        _sprite.frame = 60;
                }
                else
                    _sprite.frame = 44;
            }
            else if (autoPlatform4 != null)
            {
                if (autoPlatform2 != null)
                {
                    if (autoPlatform3 != null)
                    {
                        if (autoPlatform7 == null && autoPlatform8 == null)
                            _sprite.frame = 34;
                        else if (autoPlatform5 != null)
                        {
                            if (autoPlatform6 != null)
                                _sprite.frame = 3;
                            else if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 == null)
                                    return;
                                _sprite.frame = 3;
                            }
                            else if (autoPlatform7 != null)
                                _sprite.frame = 6;
                            else
                                _sprite.frame = 24;
                        }
                        else if (autoPlatform6 != null)
                        {
                            if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 != null)
                                    _sprite.frame = 3;
                                else
                                    _sprite.frame = 0;
                            }
                            else
                            {
                                if (autoPlatform7 != null)
                                    return;
                                _sprite.frame = 25;
                            }
                        }
                        else if (autoPlatform8 != null)
                        {
                            if (autoPlatform7 != null)
                                _sprite.frame = 3;
                            else
                                _sprite.frame = 8;
                        }
                        else if (autoPlatform7 != null)
                            _sprite.frame = 14;
                        else
                            _sprite.frame = 34;
                    }
                    else if (autoPlatform5 == null && autoPlatform6 != null && autoPlatform7 != null && autoPlatform8 != null)
                        _sprite.frame = 1;
                    else if (autoPlatform8 != null)
                        _sprite.frame = 2;
                    else
                        _sprite.frame = 51;
                }
                else if (autoPlatform3 != null)
                {
                    if ((autoPlatform7 != null || autoPlatform5 != null) && (autoPlatform6 != null || autoPlatform8 != null))
                        _sprite.frame = 59;
                    else if (autoPlatform8 != null || autoPlatform6 != null)
                        _sprite.frame = 33;
                    else if (autoPlatform7 != null || autoPlatform5 != null)
                        _sprite.frame = 35;
                    else
                        _sprite.frame = 36;
                }
                else if (autoPlatform8 != null || autoPlatform6 != null)
                    _sprite.frame = 41;
                else
                    _sprite.frame = 32;
            }
            else if (autoPlatform2 != null)
            {
                if (autoPlatform3 != null)
                {
                    if (autoPlatform5 != null)
                    {
                        if (autoPlatform6 == null)
                        {
                            if (autoPlatform7 != null)
                            {
                                if (autoPlatform8 != null)
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
                    else if (autoPlatform6 == null)
                    {
                        if (autoPlatform7 != null)
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
            else if (autoPlatform3 != null)
            {
                if (autoPlatform7 != null || autoPlatform5 != null)
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
