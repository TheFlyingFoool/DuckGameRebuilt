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

        public HashSet<Thing> updateList => _bigList;

        public void RandomizeObjectOrder() => _bigList = new HashSet<Thing>(_bigList.OrderBy<Thing, int>(x => Rando.Int(999999)).ToList<Thing>());

        public QuadTree quadTree => _quadTree;

        public List<CollisionIsland> GetIslands(Vec2 point)
        {
            List<CollisionIsland> islands = new List<CollisionIsland>();
            foreach (CollisionIsland island in _islands)
            {
                if (!island.willDie && (point - island.owner.position).lengthSq < island.radiusSquared)
                    islands.Add(island);
            }
            return islands;
        }

        public List<CollisionIsland> GetIslandsForCollisionCheck(Vec2 point)
        {
            List<CollisionIsland> forCollisionCheck = new List<CollisionIsland>();
            foreach (CollisionIsland island in _islands)
            {
                if (!island.willDie && (point - island.owner.position).lengthSq < island.radiusCheckSquared)
                    forCollisionCheck.Add(island);
            }
            return forCollisionCheck;
        }

        public CollisionIsland GetIsland(Vec2 point, CollisionIsland ignore = null)
        {
            foreach (CollisionIsland island in _islands)
            {
                if (!island.willDie && island != ignore && (point - island.owner.position).lengthSq < island.radiusSquared)
                    return island;
            }
            return null;
        }

        public void AddIsland(MaterialThing t) => _islands.Add(new CollisionIsland(t, this));

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
            get => _useTree;
            set => _useTree = value;
        }

        public QuadTreeObjectList(bool automatic = false, bool tree = true)
        {
            _autoRefresh = automatic;
            _useTree = tree;
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
                return _allObjectsByType.TryGetValue(key, out list) ? list : (IEnumerable<Thing>)_emptyList;
            }
        }

        public int CountType<T>()
        {
            HashSet<Thing> list;
            return _allObjectsByType.TryGetValue(typeof(T), out list) ? list.Count : 0;
        }

        public HashSet<Thing> GetDynamicObjects(System.Type key)
        {
            if (key == typeof(Thing))
                return _bigList;
            HashSet<Thing> list;
            return _objectsByType.TryGetValue(key, out list) ? list : _emptyList;
        }

        public HashSet<Thing> GetStaticObjects(System.Type key)
        {
            if (key == typeof(Thing))
                return _bigList;
            HashSet<Thing> list;
            return _staticObjectsByType.TryGetValue(key, out list) ? list : _emptyList;
        }

        //private IEnumerable<Thing> GetIslandObjects(System.Type t, Vec2 pos, float radiusSq)
        //{
        //    IEnumerable<Thing> first = new List<Thing>();
        //    foreach (CollisionIsland island in this._islands)
        //    {
        //        if ((island.owner.position - pos).lengthSq - radiusSq < island.radiusCheckSquared)
        //            first = first.Concat<Thing>(island.things);
        //    }
        //    return first;
        //}

        public bool HasStaticObjects(System.Type key) => key == typeof(Thing) || _staticObjectsByType.ContainsKey(key);

        public int Count => _bigList.Count;

        public void Add(Thing obj)
        {
            _addThings.Add(obj);
            if (!_autoRefresh)
                return;
            RefreshState();
        }

        public void AddRange(ObjectList list)
        {
            foreach (Thing thing in list)
                Add(thing);
        }

        public void Remove(Thing obj)
        {
            _removeThings.Add(obj);
            if (!_autoRefresh)
                return;
            RefreshState();
        }

        public void Clear()
        {
            _bigList.Clear();
            _addThings.Clear();
            _objectsByType.Clear();
            _staticObjectsByType.Clear();
            _quadTree.Clear();
            _allObjectsByType.Clear();
        }

        public bool Contains(Thing obj) => _bigList.Contains(obj);

        public void CleanAddList()
        {
            foreach (Thing removeThing in _removeThings)
                _addThings.Remove(removeThing);
        }

        public void RefreshState()
        {
            foreach (Thing removeThing in _removeThings)
            {
                removeThing.level = null;
                if (removeThing is IDontMove && _useTree)
                {
                    removeItem(_staticObjectsByType, removeThing);
                    _quadTree.Remove(removeThing);
                }
                else
                    removeItem(_objectsByType, removeThing);
                _bigList.Remove(removeThing);
                objectsDirty = true;
            }
            _removeThings.Clear();
            foreach (Thing addThing in _addThings)
            {
                _bigList.Add(addThing);
                addThing.level = Level.current;
                if (addThing is IDontMove && _useTree)
                {
                    addItem(_staticObjectsByType, addThing);
                    _quadTree.Add(addThing);
                }
                else
                    addItem(_objectsByType, addThing);
                objectsDirty = true;
            }
            _addThings.Clear();
        }

        private void addItem(MultiMap<System.Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                list.Add(key, obj);
                _allObjectsByType.Add(key, obj);
            }
        }

        private void removeItem(MultiMap<System.Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                list.Remove(key, obj);
                _allObjectsByType.Remove(key, obj);
            }
        }

        public IEnumerator<Thing> GetEnumerator() => _bigList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _bigList.GetEnumerator();

        public void Draw()
        {
            if (!DevConsole.showIslands)
                return;
            int num = 0;
            foreach (CollisionIsland island in _islands)
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
