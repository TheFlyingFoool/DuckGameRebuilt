// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoPlatform
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public override void Draw()
        {
            this.flipHorizontal = false;
            if (this.cheap)
                this.graphic.UltraCheapStaticDraw(this.flipHorizontal);
            else
                base.Draw();
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
                this.UpdateNubbers();
                this.UpdateCollision();
                this.DoPositioning();
            }
        }

        public AutoPlatform(float x, float y, string tileset)
          : base(x, y)
        {
            if (tileset != "")
                this._sprite = new SpriteMap(tileset, 16, 16);
            this._tileset = tileset;
            this.graphic = _sprite;
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

        public virtual void InitializeNeighbors()
        {
            if (this._neighborsInitialized)
                return;
            this._leftBlock = Level.CheckPointPlacementLayer<AutoPlatform>(this.left - 2f, this.position.y, this, this.placementLayer);
            this._rightBlock = Level.CheckPointPlacementLayer<AutoPlatform>(this.right + 2f, this.position.y, this, this.placementLayer);
            this._upBlock = Level.CheckPointPlacementLayer<AutoPlatform>(this.position.x, this.top - 2f, this, this.placementLayer);
            this._downBlock = Level.CheckPointPlacementLayer<AutoPlatform>(this.position.x, this.bottom + 2f, this, this.placementLayer);
            this._neighborsInitialized = true;
        }

        public override void Initialize() => this.DoPositioning();

        public virtual void DoPositioning()
        {
            if (Level.current is Editor || this.graphic == null)
                return;
            this.cheap = !this.neverCheap && !RandomLevelNode.editorLoad;
            this.graphic.position = this.position;
            this.graphic.scale = this.scale;
            this.graphic.center = this.center;
            this.graphic.depth = this.depth;
            this.graphic.alpha = this.alpha;
            this.graphic.angle = this.angle;
            (this.graphic as SpriteMap).ClearCache();
            (this.graphic as SpriteMap).UpdateFrame();
            if (this._leftNub != null)
            {
                this._leftNub.cheap = this.cheap;
                this._leftNub.DoPositioning();
            }
            if (this._rightNub == null)
                return;
            this._rightNub.cheap = this.cheap;
            this._rightNub.DoPositioning();
        }

        public override void EditorObjectsChanged() => this.PlaceBlock();

        public virtual bool HasNoCollision() => this._sprite.frame == 50;

        public virtual void UpdatePlatform()
        {
            if (this.needsRefresh)
            {
                this.PlaceBlock();
                if ((this._sprite.frame == 50 || (this._sprite.frame == 44 || this._sprite.frame == 26 || this._sprite.frame == 27 || this._sprite.frame == 28) && !this._collideBottom || this._sprite.frame == 10 || this._sprite.frame == 11 || this._sprite.frame == 12 || this._sprite.frame == 18 || this._sprite.frame == 19 || this._sprite.frame == 20 || this._sprite.frame == 6 || this._sprite.frame == 8 || this._sprite.frame == 14 || this._sprite.frame == 16 || this._sprite.frame == 22) && !this._init50 && (this.treeLike || this._sprite.frame != 8 && this._sprite.frame != 14 && this._sprite.frame != 16 && this._sprite.frame != 22))
                {
                    this.solid = false;
                    this._init50 = true;
                }
                this.needsRefresh = false;
            }
            if (!this._placed)
            {
                this.PlaceBlock();
            }
            else
            {
                if (this._sprite.frame != 50 && (this._sprite.frame != 44 && this._sprite.frame != 26 && this._sprite.frame != 27 && this._sprite.frame != 28 || this._collideBottom) && this._sprite.frame != 10 && this._sprite.frame != 11 && this._sprite.frame != 12 && this._sprite.frame != 18 && this._sprite.frame != 19 && this._sprite.frame != 20 && this._sprite.frame != 6 && this._sprite.frame != 8 && this._sprite.frame != 14 && this._sprite.frame != 16 && this._sprite.frame != 22 || this._init50 || !this.treeLike && (this._sprite.frame == 8 || this._sprite.frame == 14 || this._sprite.frame == 16 || this._sprite.frame == 22))
                    return;
                this.solid = false;
                this._init50 = true;
            }
        }

        public override void Update()
        {
            this.UpdatePlatform();
            base.Update();
        }

        public override void Terminate() => this.TerminateNubs();

        private void TerminateNubs()
        {
            if (this._leftNub != null)
            {
                Level.Remove(_leftNub);
                this._leftNub = null;
            }
            if (this._rightNub == null)
                return;
            Level.Remove(_rightNub);
            this._rightNub = null;
        }

        public void PlaceBlock()
        {
            this._placed = true;
            this.FindFrame();
            this.UpdateCollision();
            this.DoPositioning();
            this.UpdateNubbers();
        }

        public virtual void UpdateCollision()
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
                case 8:
                case 14:
                case 16:
                case 22:
                    if (!this.treeLike)
                    {
                        this._collisionSize.x = this.verticalWidthThick;
                        this._collisionOffset.x = (float)(16.0 - verticalWidthThick - 8.0);
                        break;
                    }
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

        public virtual void UpdateNubbers()
        {
            this.TerminateNubs();
            if (!this._hasNubs || this.removeFromLevel)
                return;
            switch (this._sprite.frame)
            {
                case 2:
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 4:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 32:
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 37:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 40:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                    }
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 41:
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 43:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
                case 49:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                    }
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 51:
                    if (this._hasLeftNub)
                    {
                        this._leftNub = new Nubber(this.x - 24f, this.y - 8f, true, this._tileset);
                        Level.Add(_leftNub);
                        break;
                    }
                    break;
                case 52:
                    if (this._hasRightNub)
                    {
                        this._rightNub = new Nubber(this.x + 8f, this.y - 8f, false, this._tileset);
                        Level.Add(_rightNub);
                        break;
                    }
                    break;
            }
            if (this._leftNub != null)
            {
                this._leftNub.depth = this.depth;
                this._leftNub.material = this.material;
            }
            if (this._rightNub == null)
                return;
            this._rightNub.depth = this.depth;
            this._rightNub.material = this.material;
        }

        public virtual void FindFrame()
        {
            float num = 16f;
            if (!this.treeLike)
                num = 10f;
            AutoPlatform autoPlatform1 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x, this.y - 17f, this, this.placementLayer);
            AutoPlatform autoPlatform2 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x, this.y + num, this, this.placementLayer);
            AutoPlatform autoPlatform3 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x - 16f, this.y, this, this.placementLayer);
            AutoPlatform autoPlatform4 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x + 16f, this.y, this, this.placementLayer);
            AutoPlatform autoPlatform5 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x - 16f, this.y - 17f, this, this.placementLayer);
            AutoPlatform autoPlatform6 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x + 16f, this.y - 17f, this, this.placementLayer);
            AutoPlatform autoPlatform7 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x - 16f, this.y + num, this, this.placementLayer);
            AutoPlatform autoPlatform8 = Level.CheckPointPlacementLayer<AutoPlatform>(this.x + 16f, this.y + num, this, this.placementLayer);
            if (autoPlatform1 != null && autoPlatform1._tileset != this._tileset)
                autoPlatform1 = null;
            if (autoPlatform2 != null && autoPlatform2._tileset != this._tileset)
                autoPlatform2 = null;
            if (autoPlatform3 != null && autoPlatform3._tileset != this._tileset)
                autoPlatform3 = null;
            if (autoPlatform4 != null && autoPlatform4._tileset != this._tileset)
                autoPlatform4 = null;
            if (autoPlatform5 != null && autoPlatform5._tileset != this._tileset)
                autoPlatform5 = null;
            if (autoPlatform6 != null && autoPlatform6._tileset != this._tileset)
                autoPlatform6 = null;
            if (autoPlatform7 != null && autoPlatform7._tileset != this._tileset)
                autoPlatform7 = null;
            if (autoPlatform8 != null && autoPlatform8._tileset != this._tileset)
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
                                            this._sprite.frame = 11;
                                        else
                                            this._sprite.frame = 21;
                                    }
                                    else if (autoPlatform8 != null)
                                        this._sprite.frame = 17;
                                    else
                                        this._sprite.frame = 23;
                                }
                                else if (autoPlatform8 != null)
                                {
                                    if (autoPlatform7 == null)
                                        return;
                                    this._sprite.frame = 12;
                                }
                                else if (autoPlatform7 != null)
                                    this._sprite.frame = 22;
                                else
                                    this._sprite.frame = 30;
                            }
                            else if (autoPlatform6 != null)
                            {
                                if (autoPlatform8 != null)
                                {
                                    if (autoPlatform7 != null)
                                        this._sprite.frame = 10;
                                    else
                                        this._sprite.frame = 16;
                                }
                                else
                                    this._sprite.frame = 24;
                            }
                            else if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 != null)
                                    this._sprite.frame = 3;
                                else
                                    this._sprite.frame = 8;
                            }
                            else
                            {
                                if (autoPlatform7 != null)
                                    return;
                                this._sprite.frame = 42;
                            }
                        }
                        else if (autoPlatform6 != null)
                        {
                            if (autoPlatform8 != null)
                            {
                                this._sprite.frame = 18;
                            }
                            else
                            {
                                if (autoPlatform5 == null)
                                    ;
                                this._sprite.frame = 7;
                            }
                        }
                        else
                        {
                            if (autoPlatform5 == null)
                            {
                                if (autoPlatform8 != null)
                                {
                                    this._sprite.frame = 2;
                                    return;
                                }
                            }
                            this._sprite.frame = 53;
                        }
                    }
                    else if (autoPlatform3 != null)
                    {
                        if (autoPlatform5 != null)
                        {
                            if (autoPlatform6 != null)
                                this._sprite.frame = 27;
                            else
                                this._sprite.frame = 29;
                        }
                        else if (autoPlatform6 != null)
                            this._sprite.frame = 25;
                        else
                            this._sprite.frame = 57;
                    }
                    else if (autoPlatform6 != null)
                        this._sprite.frame = 26;
                    else
                        this._sprite.frame = 58;
                }
                else if (autoPlatform2 != null)
                {
                    if (autoPlatform3 != null)
                    {
                        if (autoPlatform5 != null)
                        {
                            if (autoPlatform7 != null)
                            {
                                this._sprite.frame = 20;
                            }
                            else
                            {
                                if (autoPlatform8 == null)
                                    ;
                                this._sprite.frame = 15;
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
                                        this._sprite.frame = 4;
                                        return;
                                    }
                                    this._sprite.frame = 45;
                                    return;
                                }
                                if (autoPlatform7 != null)
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
                else if (autoPlatform3 != null)
                {
                    if (autoPlatform5 != null)
                        this._sprite.frame = 28;
                    else
                        this._sprite.frame = 60;
                }
                else
                    this._sprite.frame = 44;
            }
            else if (autoPlatform4 != null)
            {
                if (autoPlatform2 != null)
                {
                    if (autoPlatform3 != null)
                    {
                        if (autoPlatform7 == null && autoPlatform8 == null)
                            this._sprite.frame = 34;
                        else if (autoPlatform5 != null)
                        {
                            if (autoPlatform6 != null)
                                this._sprite.frame = 3;
                            else if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 == null)
                                    return;
                                this._sprite.frame = 3;
                            }
                            else if (autoPlatform7 != null)
                                this._sprite.frame = 6;
                            else
                                this._sprite.frame = 24;
                        }
                        else if (autoPlatform6 != null)
                        {
                            if (autoPlatform8 != null)
                            {
                                if (autoPlatform7 != null)
                                    this._sprite.frame = 3;
                                else
                                    this._sprite.frame = 0;
                            }
                            else
                            {
                                if (autoPlatform7 != null)
                                    return;
                                this._sprite.frame = 25;
                            }
                        }
                        else if (autoPlatform8 != null)
                        {
                            if (autoPlatform7 != null)
                                this._sprite.frame = 3;
                            else
                                this._sprite.frame = 8;
                        }
                        else if (autoPlatform7 != null)
                            this._sprite.frame = 14;
                        else
                            this._sprite.frame = 34;
                    }
                    else if (autoPlatform5 == null && autoPlatform6 != null && autoPlatform7 != null && autoPlatform8 != null)
                        this._sprite.frame = 1;
                    else if (autoPlatform8 != null)
                        this._sprite.frame = 2;
                    else
                        this._sprite.frame = 51;
                }
                else if (autoPlatform3 != null)
                {
                    if ((autoPlatform7 != null || autoPlatform5 != null) && (autoPlatform6 != null || autoPlatform8 != null))
                        this._sprite.frame = 59;
                    else if (autoPlatform8 != null || autoPlatform6 != null)
                        this._sprite.frame = 33;
                    else if (autoPlatform7 != null || autoPlatform5 != null)
                        this._sprite.frame = 35;
                    else
                        this._sprite.frame = 36;
                }
                else if (autoPlatform8 != null || autoPlatform6 != null)
                    this._sprite.frame = 41;
                else
                    this._sprite.frame = 32;
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
                    else if (autoPlatform6 == null)
                    {
                        if (autoPlatform7 != null)
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
            else if (autoPlatform3 != null)
            {
                if (autoPlatform7 != null || autoPlatform5 != null)
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
