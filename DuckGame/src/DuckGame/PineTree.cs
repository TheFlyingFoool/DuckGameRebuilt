// Decompiled with JetBrains decompiler
// Type: DuckGame.PineTree
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap(tileset, 8, 16);
            graphic = _sprite;
            collisionSize = new Vec2(8f, 16f);
            thickness = 0.2f;
            centerx = 4f;
            centery = 8f;
            collisionOffset = new Vec2(-4f, -8f);
            depth = -0.12f;
            placementLayerOverride = Layer.Foreground;
            forceEditorGrid = 8;
            treeLike = true;
        }

        public override void InitializeNeighbors()
        {
            if (_neighborsInitialized)
                return;
            _leftBlock = Level.CheckPoint<PineTree>(left - 2f, position.y, this);
            _rightBlock = Level.CheckPoint<PineTree>(right + 2f, position.y, this);
            _upBlock = Level.CheckPoint<PineTree>(position.x, top - 2f, this);
            _downBlock = Level.CheckPoint<PineTree>(position.x, bottom + 2f, this);
            _neighborsInitialized = true;
        }

        public virtual void KnockOffSnow(Vec2 dir, bool vertShake) => knocked = true;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            shiftTime = 1f;
            shiftAmount = bullet.travelDirNormalized.x > 0 ? 1 : -1;
            KnockOffSnow(bullet.travelDirNormalized, false);
            return false;
        }

        public override bool HasNoCollision() => false;

        public override void UpdatePlatform()
        {
            if (needsRefresh)
            {
                PlaceBlock();
                if ((_sprite.frame == 0 || _sprite.frame == 2 || _sprite.frame == 3 || _sprite.frame == 4) && !_init50)
                    edge = true;
                solid = false;
                _init50 = true;
                needsRefresh = false;
            }
            if (!_placed)
            {
                PlaceBlock();
            }
            else
            {
                if ((_sprite.frame == 0 || _sprite.frame == 2 || _sprite.frame == 3 || _sprite.frame == 4) && !_init50)
                    edge = true;
                solid = false;
                _init50 = true;
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
            depth = -0.12f;
            if (_vertPush > 0)
                depth = -0.11f;
            if (_graphic != null)
            {
                Sprite graphic = _graphic;
                Vec2 position = this.position;
                Vec2 vec2_1 = new Vec2(0f * shiftTime, _vertPush * 1.5f);
                Vec2 vec2_2 = position + vec2_1;
                graphic.position = vec2_2;
                _graphic.alpha = alpha;
                _graphic.angle = angle;
                _graphic.depth = depth;
                _graphic.scale = scale + new Vec2(Math.Abs(shiftAmount * 0f) * shiftTime, _vertPush * 0.2f);
                _graphic.center = center;
                _graphic.Draw();
            }
            if (shiftTime > 0)
            {
                _graphic.position = position + new Vec2(shiftAmount * 2 * shiftTime, 0f);
                _graphic.alpha = alpha;
                _graphic.angle = angle;
                _graphic.depth = depth + 10;
                _graphic.scale = scale + new Vec2(Math.Abs(shiftAmount * 0f) * shiftTime, 0f);
                _graphic.center = center;
                _graphic.alpha = 0.6f;
                _graphic.Draw();
            }
            shiftTime = Lerp.FloatSmooth(shiftTime, 0f, 0.1f);
            if (shiftTime < 0.05f)
                shiftTime = 0f;
            _vertPush = Lerp.FloatSmooth(_vertPush, 0f, 0.3f);
            if (_vertPush >= 0.05f)
                return;
            _vertPush = 0f;
        }

        public void SpecialFindFrame()
        {
            PineTree leftPine = this.leftPine;
            PineTree rightPine = this.rightPine;
            PineTree pineTree1 = Level.CheckPoint<PineTree>(x, y - 16f, this);
            PineTree pineTree2 = Level.CheckPoint<PineTree>(x, y + 16f, this);
            if (pineTree1 != null && pineTree1._tileset != _tileset)
                pineTree1 = null;
            if (pineTree2 != null && pineTree2._tileset != _tileset)
                pineTree2 = null;
            if (pineTree1 != null)
            {
                if (rightPine != null)
                {
                    if (pineTree2 != null)
                    {
                        if (leftPine != null)
                        {
                            if (orientation == 0)
                            {
                                if (leftPine.leftPine == null && rightPine.rightPine == null)
                                    frame = 1;
                                else
                                    frame = 10;
                            }
                            else if (orientation == -1)
                            {
                                if (leftPine.leftPine == null)
                                    frame = 22;
                                else
                                    frame = 23;
                            }
                            else
                            {
                                if (orientation != 1)
                                    return;
                                if (rightPine.rightPine == null)
                                    frame = 26;
                                else
                                    frame = 25;
                            }
                        }
                        else
                            frame = 0;
                    }
                    else if (leftPine != null)
                    {
                        if (orientation == 0)
                        {
                            if (leftPine.leftPine == null && rightPine.rightPine == null)
                                frame = 1;
                            else
                                frame = 10;
                        }
                        else if (orientation == -1)
                        {
                            if (leftPine.leftPine == null)
                                frame = 8;
                            else
                                frame = 9;
                        }
                        else
                        {
                            if (orientation != 1)
                                return;
                            if (rightPine.rightPine == null)
                                frame = 12;
                            else
                                frame = 11;
                        }
                    }
                    else
                        frame = 3;
                }
                else if (pineTree2 != null)
                {
                    if (leftPine != null)
                        frame = 2;
                    else
                        frame = 5;
                }
                else if (leftPine != null)
                    frame = 4;
                else
                    frame = 5;
            }
            else if (rightPine != null)
            {
                if (pineTree2 != null)
                {
                    if (leftPine != null)
                    {
                        if (orientation == 0)
                        {
                            if (leftPine.leftPine == null && rightPine.rightPine == null)
                                frame = 1;
                            else
                                frame = 31;
                        }
                        else if (orientation == -1)
                        {
                            if (leftPine.leftPine == null)
                                frame = 29;
                            else
                                frame = 30;
                        }
                        else
                        {
                            if (orientation != 1)
                                return;
                            if (rightPine.rightPine == null)
                                frame = 33;
                            else
                                frame = 32;
                        }
                    }
                    else
                        frame = 0;
                }
                else if (leftPine != null)
                {
                    if (orientation == 0)
                    {
                        if (leftPine.leftPine == null && rightPine.rightPine == null)
                            frame = 1;
                        else
                            frame = 17;
                    }
                    else if (orientation == -1)
                    {
                        if (leftPine.leftPine == null)
                            frame = 15;
                        else
                            frame = 16;
                    }
                    else
                    {
                        if (orientation != 1)
                            return;
                        if (rightPine.rightPine == null)
                            frame = 19;
                        else
                            frame = 18;
                    }
                }
                else
                    frame = 3;
            }
            else if (pineTree2 != null)
            {
                if (leftPine != null)
                    frame = 2;
                else
                    frame = 5;
            }
            else if (leftPine != null)
                frame = 4;
            else
                frame = 5;
        }

        public override void FindFrame()
        {
            PineTree pineTree1 = Level.CheckPoint<PineTree>(x - 8f, y, this);
            if (pineTree1 != null && pineTree1._tileset != _tileset)
                pineTree1 = null;
            PineTree ignore = Level.CheckPoint<PineTree>(x + 8f, y, this);
            if (ignore != null && ignore._tileset != _tileset)
                ignore = null;
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
                leftPine = null;
                while (pineTree2 != null)
                {
                    pineTreeList.Add(pineTree2);
                    pineTree2.rightPine = ignore;
                    if (ignore != null)
                    {
                        ignore.leftPine = pineTree2;
                        pineTree2 = ignore;
                        ignore = Level.CheckPoint<PineTree>(ignore.x + 8f, ignore.y, ignore);
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

        public override ContextMenu GetContextMenu() => null;
    }
}
