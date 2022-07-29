// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadTreeObjectList
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class QuadTreeObjectList : IEnumerable<Thing>, IEnumerable
    {
        private HashSet<Thing> _emptyList = new HashSet<Thing>();
        private HashSet<Thing> _bigList = new HashSet<Thing>();
        private HashSet<Thing> _addThings = new HashSet<Thing>();
        private HashSet<Thing> _removeThings = new HashSet<Thing>();
        private MultiMap<System.Type, Thing, HashSet<Thing>> _objectsByType = new MultiMap<System.Type, Thing, HashSet<Thing>>();
        private MultiMap<System.Type, Thing, HashSet<Thing>> _staticObjectsByType = new MultiMap<System.Type, Thing, HashSet<Thing>>();
        private MultiMap<System.Type, Thing, HashSet<Thing>> _allObjectsByType = new MultiMap<System.Type, Thing, HashSet<Thing>>();
        private QuadTree _quadTree = new QuadTree(4, new Vec2(-2304f, -2304f), 4608f, 64);
        private List<CollisionIsland> _islands = new List<CollisionIsland>();
        private bool _autoRefresh;
        private bool _useTree;
        public bool objectsDirty;

        public HashSet<Thing> updateList => this._bigList;

        public void RandomizeObjectOrder() => this._bigList = new HashSet<Thing>(this._bigList.OrderBy<Thing, int>(x => Rando.Int(999999)).ToList<Thing>());

        public QuadTree quadTree => this._quadTree;

        public List<CollisionIsland> GetIslands(Vec2 point)
        {
            List<CollisionIsland> islands = new List<CollisionIsland>();
            foreach (CollisionIsland island in this._islands)
            {
                if (!island.willDie && (double)(point - island.owner.position).lengthSq < island.radiusSquared)
                    islands.Add(island);
            }
            return islands;
        }

        public List<CollisionIsland> GetIslandsForCollisionCheck(Vec2 point)
        {
            List<CollisionIsland> forCollisionCheck = new List<CollisionIsland>();
            foreach (CollisionIsland island in this._islands)
            {
                if (!island.willDie && (double)(point - island.owner.position).lengthSq < island.radiusCheckSquared)
                    forCollisionCheck.Add(island);
            }
            return forCollisionCheck;
        }

        public CollisionIsland GetIsland(Vec2 point, CollisionIsland ignore = null)
        {
            foreach (CollisionIsland island in this._islands)
            {
                if (!island.willDie && island != ignore && (double)(point - island.owner.position).lengthSq < island.radiusSquared)
                    return island;
            }
            return null;
        }

        public void AddIsland(MaterialThing t) => this._islands.Add(new CollisionIsland(t, this));

        public void RemoveIsland(CollisionIsland i)
        {
            if (i.things.Count != 0)
            {
                i.owner = i.things.First<MaterialThing>();
                for (int index = 0; index < i.things.Count; ++index)
                {
                    MaterialThing materialThing = i.things.ElementAt<MaterialThing>(index);
                    if (materialThing != i.owner)
                    {
                        int count = i.things.Count;
                        materialThing.UpdateIsland();
                        if (i.things.Count != count)
                            --index;
                    }
                }
            }
            else
                i.willDie = true;
        }

        public void UpdateIslands()
        {
        }

        public bool useTree
        {
            get => this._useTree;
            set => this._useTree = value;
        }

        public QuadTreeObjectList(bool automatic = false, bool tree = true)
        {
            this._autoRefresh = automatic;
            this._useTree = tree;
        }

        public List<Thing> ToList()
        {
            List<Thing> list = new List<Thing>();
            list.AddRange(_bigList);
            return list;
        }

        public IEnumerable<Thing> this[System.Type key]
        {
            get
            {
                if (key == typeof(Thing))
                    return _bigList;
                HashSet<Thing> list;
                return this._allObjectsByType.TryGetValue(key, out list) ? list : (IEnumerable<Thing>)this._emptyList;
            }
        }

        public int CountType<T>()
        {
            HashSet<Thing> list;
            return this._allObjectsByType.TryGetValue(typeof(T), out list) ? list.Count : 0;
        }

        public HashSet<Thing> GetDynamicObjects(System.Type key)
        {
            if (key == typeof(Thing))
                return this._bigList;
            HashSet<Thing> list;
            return this._objectsByType.TryGetValue(key, out list) ? list : this._emptyList;
        }

        public HashSet<Thing> GetStaticObjects(System.Type key)
        {
            if (key == typeof(Thing))
                return this._bigList;
            HashSet<Thing> list;
            return this._staticObjectsByType.TryGetValue(key, out list) ? list : this._emptyList;
        }

        //private IEnumerable<Thing> GetIslandObjects(System.Type t, Vec2 pos, float radiusSq)
        //{
        //    IEnumerable<Thing> first = new List<Thing>();
        //    foreach (CollisionIsland island in this._islands)
        //    {
        //        if ((double)(island.owner.position - pos).lengthSq - (double)radiusSq < island.radiusCheckSquared)
        //            first = first.Concat<Thing>(island.things);
        //    }
        //    return first;
        //}

        public bool HasStaticObjects(System.Type key) => key == typeof(Thing) || this._staticObjectsByType.ContainsKey(key);

        public int Count => this._bigList.Count;

        public void Add(Thing obj)
        {
            this._addThings.Add(obj);
            if (!this._autoRefresh)
                return;
            this.RefreshState();
        }

        public void AddRange(ObjectList list)
        {
            foreach (Thing thing in list)
                this.Add(thing);
        }

        public void Remove(Thing obj)
        {
            this._removeThings.Add(obj);
            if (!this._autoRefresh)
                return;
            this.RefreshState();
        }

        public void Clear()
        {
            this._bigList.Clear();
            this._addThings.Clear();
            this._objectsByType.Clear();
            this._staticObjectsByType.Clear();
            this._quadTree.Clear();
            this._allObjectsByType.Clear();
        }

        public bool Contains(Thing obj) => this._bigList.Contains(obj);

        public void CleanAddList()
        {
            foreach (Thing removeThing in this._removeThings)
                this._addThings.Remove(removeThing);
        }

        public void RefreshState()
        {
            foreach (Thing removeThing in this._removeThings)
            {
                removeThing.level = null;
                if (removeThing is IDontMove && this._useTree)
                {
                    this.removeItem(this._staticObjectsByType, removeThing);
                    this._quadTree.Remove(removeThing);
                }
                else
                    this.removeItem(this._objectsByType, removeThing);
                this._bigList.Remove(removeThing);
                this.objectsDirty = true;
            }
            this._removeThings.Clear();
            foreach (Thing addThing in this._addThings)
            {
                this._bigList.Add(addThing);
                addThing.level = Level.current;
                if (addThing is IDontMove && this._useTree)
                {
                    this.addItem(this._staticObjectsByType, addThing);
                    this._quadTree.Add(addThing);
                }
                else
                    this.addItem(this._objectsByType, addThing);
                this.objectsDirty = true;
            }
            this._addThings.Clear();
        }

        private void addItem(MultiMap<System.Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                list.Add(key, obj);
                this._allObjectsByType.Add(key, obj);
            }
        }

        private void removeItem(MultiMap<System.Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                list.Remove(key, obj);
                this._allObjectsByType.Remove(key, obj);
            }
        }

        public IEnumerator<Thing> GetEnumerator() => this._bigList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._bigList.GetEnumerator();

        public void Draw()
        {
            if (!DevConsole.showIslands)
                return;
            int num = 0;
            foreach (CollisionIsland island in this._islands)
            {
                Graphics.DrawCircle(island.owner.position, island.radiusCheck, Color.Red * 0.7f, depth: ((Depth)0.9f), iterations: 64);
                Graphics.DrawCircle(island.owner.position, island.radius, Color.Blue * 0.3f, depth: ((Depth)0.9f), iterations: 64);
                Graphics.DrawString(Convert.ToString(num), island.owner.position, Color.Red, (Depth)1f);
                foreach (Thing thing in island.things)
                {
                    if (thing != island.owner)
                        Graphics.DrawString(Convert.ToString(num), thing.position, Color.White, (Depth)1f);
                }
                ++num;
            }
        }
    }
}
