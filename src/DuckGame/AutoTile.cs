// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoTile
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class AutoTile : MaterialThing, IAutoTile, IDontMove, IPathNodeBlocker
    {
        protected SpriteMap _sprite;
        protected Nubber _leftNub;
        protected Nubber _rightNub;
        private string _tileset;
        public float verticalWidth = 16f;
        public float verticalWidthThick = 16f;
        public float horizontalHeight = 16f;
        protected bool _hasNubs = true;
        protected bool _init50;
        private bool _neighborsInitialized;
        private AutoPlatform _leftBlock;
        private AutoPlatform _rightBlock;
        private AutoPlatform _upBlock;
        private AutoPlatform _downBlock;
        public AutoTile leftTile;
        public AutoTile rightTile;
        public AutoTile upTile;
        public AutoTile downTile;
        private bool _pathed;
        public bool needsRefresh;

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
                UpdateCollision();
            }
        }

        public AutoTile(float x, float y, string tileset)
          : base(x, y)
        {
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

        public void InitializeNeighbors()
        {
            if (_neighborsInitialized)
                return;
            _leftBlock = Level.CheckPoint<AutoPlatform>(left - 2f, position.y, this);
            _rightBlock = Level.CheckPoint<AutoPlatform>(right + 2f, position.y, this);
            _upBlock = Level.CheckPoint<AutoPlatform>(position.x, top - 2f, this);
            _downBlock = Level.CheckPoint<AutoPlatform>(position.x, bottom + 2f, this);
            _neighborsInitialized = true;
        }

        public override void EditorObjectsChanged() => PlaceBlock();

        public bool HasNoCollision() => false;

        public override void Update()
        {
            if (needsRefresh)
            {
                PlaceBlock();
                needsRefresh = false;
            }
            if (!_placed)
                PlaceBlock();
            base.Update();
        }

        public override void Terminate()
        {
        }

        public void PlaceBlock()
        {
            _placed = true;
            FindFrame();
            UpdateCollision();
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
                    _collisionOffset.x = (float)(16.0 - verticalWidthThick - 8.0);
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

        public void FindFrame()
        {
            AutoTile autoTile1 = Level.CheckPoint<AutoTile>(x, y - 16f, this);
            AutoTile autoTile2 = Level.CheckPoint<AutoTile>(x, y + 16f, this);
            AutoTile autoTile3 = Level.CheckPoint<AutoTile>(x - 16f, y, this);
            AutoTile autoTile4 = Level.CheckPoint<AutoTile>(x + 16f, y, this);
            AutoTile autoTile5 = Level.CheckPoint<AutoTile>(x - 16f, y - 16f, this);
            AutoTile autoTile6 = Level.CheckPoint<AutoTile>(x + 16f, y - 16f, this);
            AutoTile autoTile7 = Level.CheckPoint<AutoTile>(x - 16f, y + 16f, this);
            AutoTile autoTile8 = Level.CheckPoint<AutoTile>(x + 16f, y + 16f, this);
            if (autoTile1 != null && autoTile1._tileset != _tileset)
                autoTile1 = null;
            if (autoTile2 != null && autoTile2._tileset != _tileset)
                autoTile2 = null;
            if (autoTile3 != null && autoTile3._tileset != _tileset)
                autoTile3 = null;
            if (autoTile4 != null && autoTile4._tileset != _tileset)
                autoTile4 = null;
            if (autoTile5 != null && autoTile5._tileset != _tileset)
                autoTile5 = null;
            if (autoTile6 != null && autoTile6._tileset != _tileset)
                autoTile6 = null;
            if (autoTile7 != null && autoTile7._tileset != _tileset)
                autoTile7 = null;
            if (autoTile8 != null && autoTile8._tileset != _tileset)
                autoTile8 = null;
            leftTile = autoTile3;
            rightTile = autoTile4;
            upTile = autoTile1;
            downTile = autoTile2;
            if (autoTile1 != null)
            {
                if (autoTile4 != null)
                {
                    if (autoTile2 != null)
                    {
                        if (autoTile3 != null)
                        {
                            if (autoTile5 != null)
                            {
                                if (autoTile6 != null)
                                {
                                    if (autoTile7 != null)
                                    {
                                        if (autoTile8 != null)
                                            frame = 11;
                                        else
                                            frame = 21;
                                    }
                                    else if (autoTile8 != null)
                                        frame = 17;
                                    else
                                        frame = 23;
                                }
                                else if (autoTile8 != null)
                                {
                                    if (autoTile7 == null)
                                        return;
                                    frame = 12;
                                }
                                else if (autoTile7 != null)
                                    frame = 22;
                                else
                                    frame = 30;
                            }
                            else if (autoTile6 != null)
                            {
                                if (autoTile8 != null)
                                {
                                    if (autoTile7 != null)
                                        frame = 10;
                                    else
                                        frame = 16;
                                }
                                else
                                    frame = 24;
                            }
                            else if (autoTile8 != null)
                            {
                                if (autoTile7 != null)
                                    frame = 3;
                                else
                                    frame = 8;
                            }
                            else
                            {
                                if (autoTile7 != null)
                                    return;
                                frame = 42;
                            }
                        }
                        else if (autoTile6 != null)
                        {
                            if (autoTile8 != null)
                            {
                                frame = 18;
                            }
                            else
                            {
                                if (autoTile5 == null)
                                    ;
                                frame = 7;
                            }
                        }
                        else
                        {
                            if (autoTile5 == null)
                            {
                                if (autoTile8 != null)
                                {
                                    frame = 2;
                                    return;
                                }
                            }
                            frame = 53;
                        }
                    }
                    else if (autoTile3 != null)
                    {
                        if (autoTile5 != null)
                        {
                            if (autoTile6 != null)
                                frame = 27;
                            else
                                frame = 29;
                        }
                        else if (autoTile6 != null)
                            frame = 25;
                        else
                            frame = 57;
                    }
                    else if (autoTile6 != null)
                        frame = 26;
                    else
                        frame = 58;
                }
                else if (autoTile2 != null)
                {
                    if (autoTile3 != null)
                    {
                        if (autoTile5 != null)
                        {
                            if (autoTile7 != null)
                            {
                                frame = 20;
                            }
                            else
                            {
                                if (autoTile8 == null)
                                    ;
                                frame = 15;
                            }
                        }
                        else
                        {
                            if (autoTile6 == null)
                            {
                                if (autoTile8 != null)
                                {
                                    if (autoTile7 != null)
                                    {
                                        frame = 4;
                                        return;
                                    }
                                    frame = 45;
                                    return;
                                }
                                if (autoTile7 != null)
                                {
                                    frame = 4;
                                    return;
                                }
                            }
                            frame = 45;
                        }
                    }
                    else
                        frame = 50;
                }
                else if (autoTile3 != null)
                {
                    if (autoTile5 != null)
                        frame = 28;
                    else
                        frame = 60;
                }
                else
                    frame = 44;
            }
            else if (autoTile4 != null)
            {
                if (autoTile2 != null)
                {
                    if (autoTile3 != null)
                    {
                        if (autoTile7 == null && autoTile8 == null)
                            frame = 34;
                        else if (autoTile5 != null)
                        {
                            if (autoTile6 != null)
                                frame = 3;
                            else if (autoTile8 != null)
                            {
                                if (autoTile7 == null)
                                    return;
                                frame = 3;
                            }
                            else if (autoTile7 != null)
                                frame = 6;
                            else
                                frame = 24;
                        }
                        else if (autoTile6 != null)
                        {
                            if (autoTile8 != null)
                            {
                                if (autoTile7 != null)
                                    frame = 3;
                                else
                                    frame = 0;
                            }
                            else
                            {
                                if (autoTile7 != null)
                                    return;
                                frame = 25;
                            }
                        }
                        else if (autoTile8 != null)
                        {
                            if (autoTile7 != null)
                                frame = 3;
                            else
                                frame = 8;
                        }
                        else if (autoTile7 != null)
                            frame = 14;
                        else
                            frame = 34;
                    }
                    else if (autoTile5 == null && autoTile6 != null && autoTile7 != null && autoTile8 != null)
                        frame = 1;
                    else if (autoTile8 != null)
                        frame = 2;
                    else
                        frame = 51;
                }
                else if (autoTile3 != null)
                {
                    if ((autoTile7 != null || autoTile5 != null) && (autoTile6 != null || autoTile8 != null))
                        frame = 59;
                    else if (autoTile8 != null || autoTile6 != null)
                        frame = 33;
                    else if (autoTile7 != null || autoTile5 != null)
                        frame = 35;
                    else
                        frame = 36;
                }
                else if (autoTile8 != null || autoTile6 != null)
                    frame = 41;
                else
                    frame = 32;
            }
            else if (autoTile2 != null)
            {
                if (autoTile3 != null)
                {
                    if (autoTile5 != null)
                    {
                        if (autoTile6 == null)
                        {
                            if (autoTile7 != null)
                            {
                                if (autoTile8 != null)
                                {
                                    frame = 5;
                                    return;
                                }
                                frame = 4;
                                return;
                            }
                            frame = 52;
                            return;
                        }
                    }
                    else if (autoTile6 == null)
                    {
                        if (autoTile7 != null)
                        {
                            frame = 4;
                            return;
                        }
                    }
                    frame = 52;
                }
                else
                    frame = 49;
            }
            else if (autoTile3 != null)
            {
                if (autoTile7 != null || autoTile5 != null)
                    frame = 43;
                else
                    frame = 37;
            }
            else
                frame = 40;
        }

        public override ContextMenu GetContextMenu() => null;
    }
}
