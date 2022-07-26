// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoTile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public AutoPlatform leftBlock => this._leftBlock;

        public AutoPlatform rightBlock => this._rightBlock;

        public AutoPlatform upBlock => this._upBlock;

        public AutoPlatform downBlock => this._downBlock;

        public override void SetTranslation(Vec2 translation)
        {
            if (this._leftNub != null)
                this._leftNub.SetTranslation(translation);
            if (this._rightNub != null)
                this._rightNub.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public bool pathed
        {
            get => this._pathed;
            set => this._pathed = value;
        }

        public override int frame
        {
            get => this._sprite.frame;
            set
            {
                this._sprite.frame = value;
                this.UpdateCollision();
            }
        }

        public AutoTile(float x, float y, string tileset)
          : base(x, y)
        {
            this._sprite = new SpriteMap(tileset, 16, 16);
            this._tileset = tileset;
            this.graphic = (Sprite)this._sprite;
            this.collisionSize = new Vec2(16f, 16f);
            this.thickness = 0.2f;
            this.centerx = 8f;
            this.centery = 8f;
            this.collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.3f;
            this._canBeGrouped = true;
            this._isStatic = true;
            this._placementCost = 1;
        }

        public void InitializeNeighbors()
        {
            if (this._neighborsInitialized)
                return;
            this._leftBlock = Level.CheckPoint<AutoPlatform>(this.left - 2f, this.position.y, (Thing)this);
            this._rightBlock = Level.CheckPoint<AutoPlatform>(this.right + 2f, this.position.y, (Thing)this);
            this._upBlock = Level.CheckPoint<AutoPlatform>(this.position.x, this.top - 2f, (Thing)this);
            this._downBlock = Level.CheckPoint<AutoPlatform>(this.position.x, this.bottom + 2f, (Thing)this);
            this._neighborsInitialized = true;
        }

        public override void EditorObjectsChanged() => this.PlaceBlock();

        public bool HasNoCollision() => false;

        public override void Update()
        {
            if (this.needsRefresh)
            {
                this.PlaceBlock();
                this.needsRefresh = false;
            }
            if (!this._placed)
                this.PlaceBlock();
            base.Update();
        }

        public override void Terminate()
        {
        }

        public void PlaceBlock()
        {
            this._placed = true;
            this.FindFrame();
            this.UpdateCollision();
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
                    this.collisionSize = new Vec2((float)(8.0 + (double)this.verticalWidth / 2.0), 16f);
                    this.collisionOffset = new Vec2((float)(-(double)this.verticalWidth / 2.0), -8f);
                    break;
                case 37:
                case 43:
                case 45:
                case 52:
                case 60:
                    this.collisionSize = new Vec2((float)(8.0 + (double)this.verticalWidth / 2.0), 16f);
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
                    this._collisionOffset.x = (float)(16.0 - (double)this.verticalWidthThick - 8.0);
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

        public void FindFrame()
        {
            AutoTile autoTile1 = Level.CheckPoint<AutoTile>(this.x, this.y - 16f, (Thing)this);
            AutoTile autoTile2 = Level.CheckPoint<AutoTile>(this.x, this.y + 16f, (Thing)this);
            AutoTile autoTile3 = Level.CheckPoint<AutoTile>(this.x - 16f, this.y, (Thing)this);
            AutoTile autoTile4 = Level.CheckPoint<AutoTile>(this.x + 16f, this.y, (Thing)this);
            AutoTile autoTile5 = Level.CheckPoint<AutoTile>(this.x - 16f, this.y - 16f, (Thing)this);
            AutoTile autoTile6 = Level.CheckPoint<AutoTile>(this.x + 16f, this.y - 16f, (Thing)this);
            AutoTile autoTile7 = Level.CheckPoint<AutoTile>(this.x - 16f, this.y + 16f, (Thing)this);
            AutoTile autoTile8 = Level.CheckPoint<AutoTile>(this.x + 16f, this.y + 16f, (Thing)this);
            if (autoTile1 != null && autoTile1._tileset != this._tileset)
                autoTile1 = (AutoTile)null;
            if (autoTile2 != null && autoTile2._tileset != this._tileset)
                autoTile2 = (AutoTile)null;
            if (autoTile3 != null && autoTile3._tileset != this._tileset)
                autoTile3 = (AutoTile)null;
            if (autoTile4 != null && autoTile4._tileset != this._tileset)
                autoTile4 = (AutoTile)null;
            if (autoTile5 != null && autoTile5._tileset != this._tileset)
                autoTile5 = (AutoTile)null;
            if (autoTile6 != null && autoTile6._tileset != this._tileset)
                autoTile6 = (AutoTile)null;
            if (autoTile7 != null && autoTile7._tileset != this._tileset)
                autoTile7 = (AutoTile)null;
            if (autoTile8 != null && autoTile8._tileset != this._tileset)
                autoTile8 = (AutoTile)null;
            this.leftTile = autoTile3;
            this.rightTile = autoTile4;
            this.upTile = autoTile1;
            this.downTile = autoTile2;
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
                                            this.frame = 11;
                                        else
                                            this.frame = 21;
                                    }
                                    else if (autoTile8 != null)
                                        this.frame = 17;
                                    else
                                        this.frame = 23;
                                }
                                else if (autoTile8 != null)
                                {
                                    if (autoTile7 == null)
                                        return;
                                    this.frame = 12;
                                }
                                else if (autoTile7 != null)
                                    this.frame = 22;
                                else
                                    this.frame = 30;
                            }
                            else if (autoTile6 != null)
                            {
                                if (autoTile8 != null)
                                {
                                    if (autoTile7 != null)
                                        this.frame = 10;
                                    else
                                        this.frame = 16;
                                }
                                else
                                    this.frame = 24;
                            }
                            else if (autoTile8 != null)
                            {
                                if (autoTile7 != null)
                                    this.frame = 3;
                                else
                                    this.frame = 8;
                            }
                            else
                            {
                                if (autoTile7 != null)
                                    return;
                                this.frame = 42;
                            }
                        }
                        else if (autoTile6 != null)
                        {
                            if (autoTile8 != null)
                            {
                                this.frame = 18;
                            }
                            else
                            {
                                if (autoTile5 == null)
                                    ;
                                this.frame = 7;
                            }
                        }
                        else
                        {
                            if (autoTile5 == null)
                            {
                                if (autoTile8 != null)
                                {
                                    this.frame = 2;
                                    return;
                                }
                            }
                            this.frame = 53;
                        }
                    }
                    else if (autoTile3 != null)
                    {
                        if (autoTile5 != null)
                        {
                            if (autoTile6 != null)
                                this.frame = 27;
                            else
                                this.frame = 29;
                        }
                        else if (autoTile6 != null)
                            this.frame = 25;
                        else
                            this.frame = 57;
                    }
                    else if (autoTile6 != null)
                        this.frame = 26;
                    else
                        this.frame = 58;
                }
                else if (autoTile2 != null)
                {
                    if (autoTile3 != null)
                    {
                        if (autoTile5 != null)
                        {
                            if (autoTile7 != null)
                            {
                                this.frame = 20;
                            }
                            else
                            {
                                if (autoTile8 == null)
                                    ;
                                this.frame = 15;
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
                                        this.frame = 4;
                                        return;
                                    }
                                    this.frame = 45;
                                    return;
                                }
                                if (autoTile7 != null)
                                {
                                    this.frame = 4;
                                    return;
                                }
                            }
                            this.frame = 45;
                        }
                    }
                    else
                        this.frame = 50;
                }
                else if (autoTile3 != null)
                {
                    if (autoTile5 != null)
                        this.frame = 28;
                    else
                        this.frame = 60;
                }
                else
                    this.frame = 44;
            }
            else if (autoTile4 != null)
            {
                if (autoTile2 != null)
                {
                    if (autoTile3 != null)
                    {
                        if (autoTile7 == null && autoTile8 == null)
                            this.frame = 34;
                        else if (autoTile5 != null)
                        {
                            if (autoTile6 != null)
                                this.frame = 3;
                            else if (autoTile8 != null)
                            {
                                if (autoTile7 == null)
                                    return;
                                this.frame = 3;
                            }
                            else if (autoTile7 != null)
                                this.frame = 6;
                            else
                                this.frame = 24;
                        }
                        else if (autoTile6 != null)
                        {
                            if (autoTile8 != null)
                            {
                                if (autoTile7 != null)
                                    this.frame = 3;
                                else
                                    this.frame = 0;
                            }
                            else
                            {
                                if (autoTile7 != null)
                                    return;
                                this.frame = 25;
                            }
                        }
                        else if (autoTile8 != null)
                        {
                            if (autoTile7 != null)
                                this.frame = 3;
                            else
                                this.frame = 8;
                        }
                        else if (autoTile7 != null)
                            this.frame = 14;
                        else
                            this.frame = 34;
                    }
                    else if (autoTile5 == null && autoTile6 != null && autoTile7 != null && autoTile8 != null)
                        this.frame = 1;
                    else if (autoTile8 != null)
                        this.frame = 2;
                    else
                        this.frame = 51;
                }
                else if (autoTile3 != null)
                {
                    if ((autoTile7 != null || autoTile5 != null) && (autoTile6 != null || autoTile8 != null))
                        this.frame = 59;
                    else if (autoTile8 != null || autoTile6 != null)
                        this.frame = 33;
                    else if (autoTile7 != null || autoTile5 != null)
                        this.frame = 35;
                    else
                        this.frame = 36;
                }
                else if (autoTile8 != null || autoTile6 != null)
                    this.frame = 41;
                else
                    this.frame = 32;
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
                                    this.frame = 5;
                                    return;
                                }
                                this.frame = 4;
                                return;
                            }
                            this.frame = 52;
                            return;
                        }
                    }
                    else if (autoTile6 == null)
                    {
                        if (autoTile7 != null)
                        {
                            this.frame = 4;
                            return;
                        }
                    }
                    this.frame = 52;
                }
                else
                    this.frame = 49;
            }
            else if (autoTile3 != null)
            {
                if (autoTile7 != null || autoTile5 != null)
                    this.frame = 43;
                else
                    this.frame = 37;
            }
            else
                this.frame = 40;
        }

        public override ContextMenu GetContextMenu() => (ContextMenu)null;
    }
}
