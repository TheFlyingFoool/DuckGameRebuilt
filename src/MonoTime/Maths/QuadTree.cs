// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadTree
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class QuadTree
    {
        private QuadTree _parent;
        private int _depth;
        private List<QuadTree> _children = new List<QuadTree>();
        private ObjectListImmediate _objects = new ObjectListImmediate();
        private List<Vec2> _corners = new List<Vec2>();
        private Stack<QuadTree> recurse = new Stack<QuadTree>();
        private Vec2 _position;
        private Vec2 _center;
        private float _width;
        private float _halfWidth;
        private int _max;
        private Rectangle _rectangle;
        private bool _split;
        private static int quadTreeIDX;
        private int _personalIDX;

        public Rectangle rectangle => this._rectangle;

        public QuadTree(int depth, Vec2 position, float width, int max = 4, QuadTree parent = null)
        {
            this._depth = depth;
            this._position = position;
            this._width = width;
            this._halfWidth = this._width / 2f;
            this._max = max;
            this._parent = parent;
            this._center = this._position + new Vec2(this._halfWidth, this._halfWidth);
            this._rectangle = new Rectangle((int)position.x, (int)position.y, (int)width, (int)width);
            if (this._depth != 0)
            {
                for (int index = 0; index < 4; ++index)
                {
                    this._corners.Add(new Vec2());
                    Vec2 position1 = new Vec2(this._position);
                    if (index == 1 || index == 3)
                        position1.x += this._halfWidth;
                    if (index == 2 || index == 3)
                        position1.y += this._halfWidth;
                    this._children.Add(new QuadTree(this._depth - 1, position1, this._halfWidth, this._max, this));
                }
            }
            this._personalIDX = QuadTree.quadTreeIDX;
            ++QuadTree.quadTreeIDX;
        }

        public Thing CheckPoint(System.Type pType, Vec2 pos, Thing ignore)
        {
            QuadTree quadTree = this;
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[pType])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Point(pos, t))
                            return t;
                    }
                    return null;
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return null;
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return null;
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return null;
        }

        public T CheckPoint<T>(Vec2 pos, Thing ignore, Layer layer)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Point(pos, t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public T CheckPoint<T>(Vec2 pos, Thing ignore)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Point(pos, t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public T CheckPointFilter<T>(Vec2 pos, Func<Thing, bool> pFilter)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Point(pos, t) && pFilter(t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public T CheckPoint<T>(Vec2 pos)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Point(pos, t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public Thing CheckPoint(Vec2 pos, System.Type typer, Thing ignore, Layer layer)
        {
            QuadTree quadTree = this;
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[typer])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Point(pos, t))
                            return t;
                    }
                    return null;
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return null;
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return null;
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return null;
        }

        public Thing CheckPoint(Vec2 pos, System.Type typer, Thing ignore)
        {
            QuadTree quadTree = this;
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[typer])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Point(pos, t))
                            return t;
                    }
                    return null;
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return null;
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return null;
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return null;
        }

        public Thing CheckPoint(Vec2 pos, System.Type typer)
        {
            QuadTree quadTree = this;
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[typer])
                    {
                        if (!t.removeFromLevel && Collision.Point(pos, t))
                            return t;
                    }
                    return null;
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return null;
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return null;
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return null;
        }

        public T CheckPointPlacementLayer<T>(Vec2 pos, Thing ignore = null, Layer layer = null)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.placementLayer) && Collision.Point(pos, t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public T CheckPointFilter<T>(Vec2 pos, Predicate<Thing> filter)
        {
            QuadTree quadTree = this;
            System.Type key = typeof(T);
            int index;
            for (; quadTree != null; quadTree = quadTree._children[index])
            {
                if (!quadTree._split)
                {
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && filter(t) && Collision.Point(pos, t))
                            return (T)(object)t;
                    }
                    return default(T);
                }
                if (pos.x > quadTree._position.x + (double)quadTree._width)
                    return default(T);
                if (pos.y > quadTree._position.y + (double)quadTree._width)
                    return default(T);
                index = 0;
                if (pos.x > quadTree._position.x + (double)quadTree._halfWidth)
                    index = 1;
                if (pos.y > quadTree._position.y + (double)quadTree._halfWidth)
                    index += 2;
            }
            return default(T);
        }

        public T CheckLine<T>(Vec2 p1, Vec2 p2, Thing ignore, Layer layer)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Line(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckLine<T>(Vec2 p1, Vec2 p2, Thing ignore)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Line(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckLine<T>(Vec2 p1, Vec2 p2)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Line(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public List<T> CheckLineAll<T>(Vec2 p1, Vec2 p2, Thing ignore, Layer layer)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            List<T> objList = new List<T>();
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Line(p1, p2, t))
                            objList.Add((T)(object)t);
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return objList;
        }

        public List<T> CheckLineAll<T>(Vec2 p1, Vec2 p2, Thing ignore)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            List<T> objList = new List<T>();
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Line(p1, p2, t))
                            objList.Add((T)(object)t);
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return objList;
        }

        public List<T> CheckLineAll<T>(Vec2 p1, Vec2 p2)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            List<T> objList = new List<T>();
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Line(p1, p2, t))
                            objList.Add((T)(object)t);
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return objList;
        }

        public T CheckLinePoint<T>(Vec2 p1, Vec2 p2, out Vec2 hit, Thing ignore, Layer layer)
        {
            hit = new Vec2();
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing thing in quadTree._objects[key])
                    {
                        if (!thing.removeFromLevel && thing != ignore && (layer == null || layer == thing.layer))
                        {
                            Vec2 vec2 = Collision.LinePoint(p1, p2, thing);
                            if (vec2 != Vec2.Zero)
                            {
                                hit = vec2;
                                return (T)(object)thing;
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckLinePoint<T>(Vec2 p1, Vec2 p2, out Vec2 hit, Thing ignore)
        {
            hit = new Vec2();
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing thing in quadTree._objects[key])
                    {
                        if (!thing.removeFromLevel && thing != ignore)
                        {
                            Vec2 vec2 = Collision.LinePoint(p1, p2, thing);
                            if (vec2 != Vec2.Zero)
                            {
                                hit = vec2;
                                return (T)(object)thing;
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckLinePoint<T>(Vec2 p1, Vec2 p2, out Vec2 hit)
        {
            hit = new Vec2();
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing thing in quadTree._objects[key])
                    {
                        if (!thing.removeFromLevel)
                        {
                            Vec2 vec2 = Collision.LinePoint(p1, p2, thing);
                            if (vec2 != Vec2.Zero)
                            {
                                hit = vec2;
                                return (T)(object)thing;
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Line(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckRectangleFilter<T>(Vec2 p1, Vec2 p2, Predicate<T> filter, Layer layer)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && (layer == null || layer == t.layer) && Collision.Rect(p1, p2, t) && filter((T)(object)t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckRectangleFilter<T>(Vec2 p1, Vec2 p2, Predicate<T> filter)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Rect(p1, p2, t) && filter((T)(object)t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckRectangle<T>(Vec2 p1, Vec2 p2, Thing ignore, Layer layer)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Rect(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckRectangle<T>(Vec2 p1, Vec2 p2, Thing ignore)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Rect(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckRectangle<T>(Vec2 p1, Vec2 p2)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Rect(p1, p2, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public void CheckRectangleAll<T>(Vec2 p1, Vec2 p2, List<T> outList)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Rect(p1, p2, t))
                            outList.Add((T)(object)t);
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        QuadTree child = quadTree._children[index];
                        if (Collision.Rect(p1, p2, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
        }

        public T CheckCircle<T>(Vec2 p1, float radius, Thing ignore, Layer layer)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && (layer == null || layer == t.layer) && Collision.Circle(p1, radius, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Circle(p1, radius, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckCircle<T>(Vec2 p1, float radius, Thing ignore)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && t != ignore && Collision.Circle(p1, radius, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Circle(p1, radius, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public T CheckCircle<T>(Vec2 p1, float radius)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Circle(p1, radius, t))
                            return (T)(object)t;
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Circle(p1, radius, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
            return default(T);
        }

        public void CheckCircleAll<T>(Vec2 p1, float radius, List<object> outList)
        {
            this.recurse.Clear();
            this.recurse.Push(this);
            while (this.recurse.Count > 0)
            {
                QuadTree quadTree = this.recurse.Pop();
                if (!quadTree._split)
                {
                    System.Type key = typeof(T);
                    foreach (Thing t in quadTree._objects[key])
                    {
                        if (!t.removeFromLevel && Collision.Circle(p1, radius, t))
                            outList.Add((T)(object)t);
                    }
                }
                else
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        if (Collision.Circle(p1, radius, quadTree._children[index].rectangle))
                            this.recurse.Push(quadTree._children[index]);
                    }
                }
            }
        }

        private void GetUniqueChildren(List<Thing> things)
        {
            foreach (Thing thing in this._objects)
            {
                if (!things.Contains(thing))
                    things.Add(thing);
            }
            if (!this._split)
                return;
            foreach (QuadTree child in this._children)
                child.GetUniqueChildren(things);
        }

        //private int Count() => this._objects.Count;

        private void Divide()
        {
            if (this._split || this._depth == 0)
                return;
            this._split = true;
            foreach (Thing t in this._objects)
                this.Add(t);
        }

        private void Combine()
        {
            if (!this._split)
                return;
            foreach (QuadTree child in this._children)
            {
                child.Combine();
                this._objects.AddRange(child._objects);
                child._objects.Clear();
            }
            this._split = false;
        }

        public void Add(Thing t)
        {
            this._objects.Add(t);
            if (!this._split)
            {
                if (this._objects.Count <= this._max || this._depth <= 0)
                    return;
                this.Divide();
            }
            else
            {
                Rectangle rectangle = t.rectangle;
                foreach (QuadTree child in this._children)
                {
                    if (Collision.Rect(child.rectangle, rectangle))
                        child.Add(t);
                }
            }
        }

        public void Remove(Thing t)
        {
            this._objects.Remove(t);
            if (!this._split)
                return;
            Rectangle rectangle = t.rectangle;
            foreach (QuadTree child in this._children)
            {
                if (Collision.Rect(child.rectangle, rectangle))
                    child.Remove(t);
            }
            if (this._objects.Count > this._max)
                return;
            this.Combine();
        }

        public void Draw()
        {
            Graphics.DrawRect(this._position, this._position + new Vec2(this._width, this._width), Color.Red, (Depth)1f, false);
            if (!this._split)
            {
                Graphics.DrawString(Change.ToString(_objects.Count), this._position + new Vec2(2f, 2f), Color.White, (Depth)0.9f);
                Graphics.DrawString(Change.ToString(_personalIDX), this._position + new Vec2(2f, 16f), Color.White, (Depth)0.9f, scale: 0.5f);
                foreach (Thing thing in this._objects)
                {
                    Graphics.DrawRect(thing.rectangle, Color.Blue, (Depth)0.0f, false);
                    Graphics.DrawString(Change.ToString(_personalIDX), thing.position, Color.Green, (Depth)0.9f, scale: 0.5f);
                }
            }
            if (this._depth == 0 || !this._split)
                return;
            foreach (QuadTree child in this._children)
                child.Draw();
        }

        public void Clear()
        {
            foreach (QuadTree child in this._children)
                child.Clear();
            this._objects.Clear();
        }
    }
}
