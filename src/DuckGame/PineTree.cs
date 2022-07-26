// Decompiled with JetBrains decompiler
// Type: DuckGame.PineTree
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public abstract class PineTree : AutoPlatform
    {
        public bool knocked;
        private float shiftTime;
        private int shiftAmount;
        public float _vertPush;
        public bool edge;
        public bool iterated;
        public int orientation;
        public PineTree leftPine;
        public PineTree rightPine;

        public PineTree(float x, float y, string tileset)
          : base(x, y, tileset)
        {
            this._sprite = new SpriteMap(tileset, 8, 16);
            this.graphic = (Sprite)this._sprite;
            this.collisionSize = new Vec2(8f, 16f);
            this.thickness = 0.2f;
            this.centerx = 4f;
            this.centery = 8f;
            this.collisionOffset = new Vec2(-4f, -8f);
            this.depth = - 0.12f;
            this.placementLayerOverride = Layer.Foreground;
            this.forceEditorGrid = 8;
            this.treeLike = true;
        }

        public override void InitializeNeighbors()
        {
            if (this._neighborsInitialized)
                return;
            this._leftBlock = (AutoPlatform)Level.CheckPoint<PineTree>(this.left - 2f, this.position.y, (Thing)this);
            this._rightBlock = (AutoPlatform)Level.CheckPoint<PineTree>(this.right + 2f, this.position.y, (Thing)this);
            this._upBlock = (AutoPlatform)Level.CheckPoint<PineTree>(this.position.x, this.top - 2f, (Thing)this);
            this._downBlock = (AutoPlatform)Level.CheckPoint<PineTree>(this.position.x, this.bottom + 2f, (Thing)this);
            this._neighborsInitialized = true;
        }

        public virtual void KnockOffSnow(Vec2 dir, bool vertShake) => this.knocked = true;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            this.shiftTime = 1f;
            this.shiftAmount = (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1;
            this.KnockOffSnow(bullet.travelDirNormalized, false);
            return false;
        }

        public override bool HasNoCollision() => false;

        public override void UpdatePlatform()
        {
            if (this.needsRefresh)
            {
                this.PlaceBlock();
                if ((this._sprite.frame == 0 || this._sprite.frame == 2 || this._sprite.frame == 3 || this._sprite.frame == 4) && !this._init50)
                    this.edge = true;
                this.solid = false;
                this._init50 = true;
                this.needsRefresh = false;
            }
            if (!this._placed)
            {
                this.PlaceBlock();
            }
            else
            {
                if ((this._sprite.frame == 0 || this._sprite.frame == 2 || this._sprite.frame == 3 || this._sprite.frame == 4) && !this._init50)
                    this.edge = true;
                this.solid = false;
                this._init50 = true;
            }
        }

        public override void UpdateCollision()
        {
        }

        public override void UpdateNubbers()
        {
        }

        public override void Draw()
        {
            this.depth = - 0.12f;
            if ((double)this._vertPush > 0.0)
                this.depth = - 0.11f;
            if (this._graphic != null)
            {
                Sprite graphic = this._graphic;
                Vec2 position = this.position;
                int shiftAmount = this.shiftAmount;
                Vec2 vec2_1 = new Vec2(0.0f * this.shiftTime, this._vertPush * 1.5f);
                Vec2 vec2_2 = position + vec2_1;
                graphic.position = vec2_2;
                this._graphic.alpha = this.alpha;
                this._graphic.angle = this.angle;
                this._graphic.depth = this.depth;
                this._graphic.scale = this.scale + new Vec2(Math.Abs((float)this.shiftAmount * 0.0f) * this.shiftTime, this._vertPush * 0.2f);
                this._graphic.center = this.center;
                this._graphic.Draw();
            }
            if ((double)this.shiftTime > 0.0)
            {
                this._graphic.position = this.position + new Vec2((float)(this.shiftAmount * 2) * this.shiftTime, 0.0f);
                this._graphic.alpha = this.alpha;
                this._graphic.angle = this.angle;
                this._graphic.depth = this.depth + 10;
                this._graphic.scale = this.scale + new Vec2(Math.Abs((float)this.shiftAmount * 0.0f) * this.shiftTime, 0.0f);
                this._graphic.center = this.center;
                this._graphic.alpha = 0.6f;
                this._graphic.Draw();
            }
            this.shiftTime = Lerp.FloatSmooth(this.shiftTime, 0.0f, 0.1f);
            if ((double)this.shiftTime < 0.0500000007450581)
                this.shiftTime = 0.0f;
            this._vertPush = Lerp.FloatSmooth(this._vertPush, 0.0f, 0.3f);
            if ((double)this._vertPush >= 0.0500000007450581)
                return;
            this._vertPush = 0.0f;
        }

        public void SpecialFindFrame()
        {
            PineTree leftPine = this.leftPine;
            PineTree rightPine = this.rightPine;
            PineTree pineTree1 = Level.CheckPoint<PineTree>(this.x, this.y - 16f, (Thing)this);
            PineTree pineTree2 = Level.CheckPoint<PineTree>(this.x, this.y + 16f, (Thing)this);
            if (pineTree1 != null && pineTree1._tileset != this._tileset)
                pineTree1 = (PineTree)null;
            if (pineTree2 != null && pineTree2._tileset != this._tileset)
                pineTree2 = (PineTree)null;
            if (pineTree1 != null)
            {
                if (rightPine != null)
                {
                    if (pineTree2 != null)
                    {
                        if (leftPine != null)
                        {
                            if (this.orientation == 0)
                            {
                                if (leftPine.leftPine == null && rightPine.rightPine == null)
                                    this.frame = 1;
                                else
                                    this.frame = 10;
                            }
                            else if (this.orientation == -1)
                            {
                                if (leftPine.leftPine == null)
                                    this.frame = 22;
                                else
                                    this.frame = 23;
                            }
                            else
                            {
                                if (this.orientation != 1)
                                    return;
                                if (rightPine.rightPine == null)
                                    this.frame = 26;
                                else
                                    this.frame = 25;
                            }
                        }
                        else
                            this.frame = 0;
                    }
                    else if (leftPine != null)
                    {
                        if (this.orientation == 0)
                        {
                            if (leftPine.leftPine == null && rightPine.rightPine == null)
                                this.frame = 1;
                            else
                                this.frame = 10;
                        }
                        else if (this.orientation == -1)
                        {
                            if (leftPine.leftPine == null)
                                this.frame = 8;
                            else
                                this.frame = 9;
                        }
                        else
                        {
                            if (this.orientation != 1)
                                return;
                            if (rightPine.rightPine == null)
                                this.frame = 12;
                            else
                                this.frame = 11;
                        }
                    }
                    else
                        this.frame = 3;
                }
                else if (pineTree2 != null)
                {
                    if (leftPine != null)
                        this.frame = 2;
                    else
                        this.frame = 5;
                }
                else if (leftPine != null)
                    this.frame = 4;
                else
                    this.frame = 5;
            }
            else if (rightPine != null)
            {
                if (pineTree2 != null)
                {
                    if (leftPine != null)
                    {
                        if (this.orientation == 0)
                        {
                            if (leftPine.leftPine == null && rightPine.rightPine == null)
                                this.frame = 1;
                            else
                                this.frame = 31;
                        }
                        else if (this.orientation == -1)
                        {
                            if (leftPine.leftPine == null)
                                this.frame = 29;
                            else
                                this.frame = 30;
                        }
                        else
                        {
                            if (this.orientation != 1)
                                return;
                            if (rightPine.rightPine == null)
                                this.frame = 33;
                            else
                                this.frame = 32;
                        }
                    }
                    else
                        this.frame = 0;
                }
                else if (leftPine != null)
                {
                    if (this.orientation == 0)
                    {
                        if (leftPine.leftPine == null && rightPine.rightPine == null)
                            this.frame = 1;
                        else
                            this.frame = 17;
                    }
                    else if (this.orientation == -1)
                    {
                        if (leftPine.leftPine == null)
                            this.frame = 15;
                        else
                            this.frame = 16;
                    }
                    else
                    {
                        if (this.orientation != 1)
                            return;
                        if (rightPine.rightPine == null)
                            this.frame = 19;
                        else
                            this.frame = 18;
                    }
                }
                else
                    this.frame = 3;
            }
            else if (pineTree2 != null)
            {
                if (leftPine != null)
                    this.frame = 2;
                else
                    this.frame = 5;
            }
            else if (leftPine != null)
                this.frame = 4;
            else
                this.frame = 5;
        }

        public override void FindFrame()
        {
            PineTree pineTree1 = Level.CheckPoint<PineTree>(this.x - 8f, this.y, (Thing)this);
            if (pineTree1 != null && pineTree1._tileset != this._tileset)
                pineTree1 = (PineTree)null;
            PineTree ignore = Level.CheckPoint<PineTree>(this.x + 8f, this.y, (Thing)this);
            if (ignore != null && ignore._tileset != this._tileset)
                ignore = (PineTree)null;
            if (pineTree1 != null && ignore != null)
            {
                pineTree1.FindFrame();
            }
            else
            {
                if (pineTree1 != null)
                    return;
                List<PineTree> pineTreeList = new List<PineTree>();
                PineTree pineTree2 = this;
                this.leftPine = (PineTree)null;
                while (pineTree2 != null)
                {
                    pineTreeList.Add(pineTree2);
                    pineTree2.rightPine = ignore;
                    if (ignore != null)
                    {
                        ignore.leftPine = pineTree2;
                        pineTree2 = ignore;
                        ignore = Level.CheckPoint<PineTree>(ignore.x + 8f, ignore.y, (Thing)ignore);
                    }
                    else
                        break;
                }
                bool flag = pineTreeList.Count % 2 == 0;
                foreach (PineTree pineTree3 in pineTreeList)
                {
                    int num1 = pineTreeList.Count / 2;
                    int num2 = pineTreeList.IndexOf(pineTree3);
                    pineTree3.orientation = !flag ? (num2 != num1 ? (num2 >= num1 ? 1 : -1) : 0) : (num2 >= num1 ? 1 : -1);
                    pineTree3.SpecialFindFrame();
                }
            }
        }

        public override ContextMenu GetContextMenu() => (ContextMenu)null;
    }
}
