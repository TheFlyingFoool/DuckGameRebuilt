using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckGame
{
    public class QuadTreeObjectList : IEnumerable<Thing>, IEnumerable
    {
        private HashSet<Thing> _emptyList = new HashSet<Thing>();
        private HashSet<Thing> _bigList = new HashSet<Thing>();
        private HashSet<Thing> _addThings = new HashSet<Thing>();
        private HashSet<Thing> _removeThings = new HashSet<Thing>();
        private MultiMap<Type, Thing, HashSet<Thing>> _objectsByType = new MultiMap<Type, Thing, HashSet<Thing>>();
        private MultiMap<Type, Thing, HashSet<Thing>> _staticObjectsByType = new MultiMap<Type, Thing, HashSet<Thing>>();
        private MultiMap<Type, Thing, HashSet<Thing>> _allObjectsByType = new MultiMap<Type, Thing, HashSet<Thing>>();
        private QuadTree _quadTree = new QuadTree(4, new Vec2(-2304f, -2304f), 4608f, 64);
        private List<CollisionIsland> _islands = new List<CollisionIsland>();
        private bool _autoRefresh;
        private bool _useTree;
        public bool objectsDirty;

        public HashSet<Thing> RealupdateList = new HashSet<Thing>();
        public HashSet<Thing> updateList => _bigList;

        public void RandomizeObjectOrder() => _bigList = new HashSet<Thing>(_bigList.OrderBy(x => Rando.Int(999999)).ToList());

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
                i.owner = i.things.First();
                for (int index = 0; index < i.things.Count; ++index)
                {
                    MaterialThing materialThing = i.things.ElementAt(index);
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

        public IEnumerable<Thing> this[Type key]
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

        public HashSet<Thing> GetDynamicObjects(Type key)
        {
            if (key == typeof(Thing))
                return _bigList;
            HashSet<Thing> list;
            return _objectsByType.TryGetValue(key, out list) ? list : _emptyList;
        }

        public HashSet<Thing> GetStaticObjects(Type key)
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

        public bool HasStaticObjects(Type key) => key == typeof(Thing) || _staticObjectsByType.ContainsKey(key);

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
            RealupdateList.Clear();
            _bigList.Clear();
            _addThings.Clear();
            _objectsByType.Clear();
            _staticObjectsByType.Clear();
            _quadTree.Clear();
            _allObjectsByType.Clear();
            Buckets.Clear();
        }

        public bool Contains(Thing obj) => _bigList.Contains(obj);

        public void CleanAddList()
        {
            foreach (Thing removeThing in _removeThings)
                _addThings.Remove(removeThing);
        }

        public static float offset = 4000000.0f;
        public static float cellsize = 100f;
        private Vec2[] GetIdForObj(Vec2 Topleft, Vec2 Bottomright)
        {
            int top = (int)((Bottomright.y + offset) / cellsize);
            int left = (int)((Topleft.x + offset) / cellsize);
            int bottom = (int)((Topleft.y + offset) / cellsize);
            int right = (int)((Bottomright.x + offset) / cellsize);
            Vec2[] Chunk = new Vec2[(right - left + 1) * (top - bottom + 1)];
            int N = -1;
            for (int x = left; x <= right; x++)
            {
                for (int y = bottom; y <= top; y++)
                {
                    N += 1;
                    Chunk[N] = new Vec2(x, y);
                }
            }
            return Chunk;
        }
        private Vec2[] GetIdForObj(Vec2 Position, float width, float height) // rect
        {
            height = height * 0.5f;
            width = width * 0.5f;

            int top = (int)((Position.y + height + offset) / cellsize);
            int bottom = (int)((Position.y - height + offset) / cellsize);
            int right = (int)((Position.x + width + offset) / cellsize);
            int left = (int)((Position.x - width + offset) / cellsize);
            Vec2[] Chunk = new Vec2[(right - left + 1) * (top - bottom + 1)];
            int N = -1;
            for (int x = left; x <= right; x++)
            {
                for (int y = bottom; y <= top; y++)
                {
                    N += 1;
                    Chunk[N] = new Vec2(x, y);
                }
            }
            return Chunk;
        }
        //public bool IsNotInHashSet(Vec2[] values)
        //{
        //    foreach (var value in values)
        //    {
        //        if (usedIds.Contains(value))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public IEnumerable<Thing> GetThings(Vec2 Position, float width, float height, Type t)
        {
            Vec2[] ids = GetIdForObj(Position, width, height);
            int typekey = t.GetHashCode();
            if (ids.Length == 1)
            {
                if (Buckets.TryGetValue(ids[0], out Dictionary<int, List<Thing>> output) && 
                    output.TryGetValue(typekey, out List<Thing> outputthings))
                {
                    
                    foreach (Thing thing in outputthings)
                    {
                        yield return thing;
                    }
                   
                }
            }
            else
            {
                if (_queryIds > Int32.MaxValue - 1)
                {
                    _queryIds = 0;
                }
                int queryId = _queryIds++;
                for (int i = 0; i < ids.Length; i++)
                {
                    Vec2 bucket = ids[i];
                    if (Buckets.TryGetValue(bucket, out Dictionary<int, List<Thing>> output) && 
                        output.TryGetValue(typekey, out List<Thing> outputthings))
                    {
                       foreach (Thing item in outputthings)
                       {
                            if (item._queryId != queryId)
                            {
                                item._queryId = queryId;
                                yield return item;
                            }
                       }
                        
                    }
                }
            }
        }
        private int _queryIds = 0;
        public IEnumerable<Thing> GetThings(Vec2 p1, Vec2 p2, Type t) //Line
        {
            Vec2[] ids = GetIdForLine(p1, p2);
            int typekey = t.GetHashCode();
            if (ids.Length == 1)
            {
                if (Buckets.TryGetValue(ids[0], out Dictionary<int, List<Thing>> output) &&
                    output.TryGetValue(typekey, out List<Thing> outputthings))
                {
                    foreach (Thing thing in outputthings)
                    {
                        yield return thing;
                    }
                }
            }
            else
            {
                if (_queryIds > Int32.MaxValue - 1)
                {
                    _queryIds = 0;
                }
                int queryId = _queryIds++;
                for (int i = 0; i < ids.Length; i++)
                {
                    Vec2 bucket = ids[i];
                    if (Buckets.TryGetValue(bucket, out Dictionary<int, List<Thing>> output) &&
                        output.TryGetValue(typekey, out List<Thing> outputthings))
                    {
                        foreach (Thing item in outputthings)
                        {
                            if (item._queryId != queryId)
                            {
                                item._queryId = queryId;
                                yield return item;
                            }
                        }
                    }
                }
            }
        }
        // public Dictionary<Vec2, List<Thing>> Buckets = new Dictionary<Vec2, List<Thing>>();
        public Dictionary<Vec2, Dictionary<int, List<Thing>>> Buckets = new Dictionary<Vec2, Dictionary<int, List<Thing>>>();
        public static float Leniancy = 9f;
        public static int LineLeniancy = 0;
        public void UpdateObject(Thing thing)  //float size = Math.Max(Math.Max(thing.right - thing.left, thing.bottom - thing.top), 16);
        {
            Vec2[] buckets = GetIdForObj(thing.topLeft - new Vec2(Leniancy), thing.bottomRight + new Vec2(Leniancy));//GetIdForObj(thing.position, thing.right - thing.left, thing.bottom - thing.top);
            if (thing.Buckets.SequenceEqual(buckets))
            {
                return;
            }
            int ThingHashCode = typeof(Thing).GetHashCode();
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<int, List<Thing>> output))
                {
                    foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        int hashcode = key.GetHashCode();
                        if (output.TryGetValue(hashcode, out List<Thing> output2))
                        {
                            output2.Remove(thing);
                        }
                        else
                        {
                            output[hashcode] = new List<Thing>() { };
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    if (output.TryGetValue(ThingHashCode, out List<Thing> output3))
                    {
                        output[ThingHashCode].Remove(thing);
                    }
                    continue;
                }
                output = new Dictionary<int, List<Thing>>();
                output[ThingHashCode] = new List<Thing>() { };
                //foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                //{
                //    //output[key].Add(thing);
                //    if (output.TryGetValue(key, out List<Thing> output2))
                //    {
                //        output2.Remove(thing);
                //    }
                //    else
                //    {
                //        output[key] = new List<Thing>() {  };
                //    }
                //    //_allObjectsByType.Add(key, obj);
                //}
                Buckets[item] = output;
                //Console.WriteLine(item);
            }
            thing.Buckets = buckets;
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<int, List<Thing>> output))
                {
                    foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        int hashcode = key.GetHashCode();
                        if (output.TryGetValue(hashcode, out List<Thing> output2))
                        {
                            output2.Add(thing);
                        }
                        else
                        {
                            output[hashcode] = new List<Thing>() { thing };
                        }

                        //_allObjectsByType.Add(key, obj);
                    }
                    output[ThingHashCode].Add(thing);
                    continue;
                }
                output = new Dictionary<int, List<Thing>>();
                output[ThingHashCode] = new List<Thing>() { thing };
                foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                {
                    //output[key].Add(thing);
                    int hashcode = key.GetHashCode();
                    if (output.TryGetValue(hashcode, out List<Thing> output2))
                    {
                        output2.Add(thing);
                    }
                    else
                    {
                        output[hashcode] = new List<Thing>() { thing };
                    }
                    //_allObjectsByType.Add(key, obj);
                }
                Buckets[item] = output;
                //Console.WriteLine(item);
            }

        }
        public void RegisterObject(Thing thing)
        {
            //if (thing.position == Vec2.Zero)
            //{
            //    DevConsole.Log(thing.ToString());
            //}

            if (thing is Fluid)
            {
                return;
            }

            thing.Buckets = GetIdForObj(thing.topLeft, thing.bottomRight); // float size = Math.Max(Math.Max(thing.right - thing.left, thing.bottom - thing.top), 16);
            thing.oldposition = thing.position;
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<int, List<Thing>> TypeList))
                {
                    foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        int hashcode = key.GetHashCode();
                        if (TypeList.TryGetValue(hashcode, out List<Thing> ThingsWithin))
                        {
                            ThingsWithin.Add(thing);
                        }
                        else
                        {
                            TypeList[hashcode] = new List<Thing>() { thing };
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    TypeList[typeof(Thing).GetHashCode()].Add(thing);
                    continue;
                }
                TypeList = new Dictionary<int, List<Thing>>();
                TypeList[typeof(Thing).GetHashCode()] = new List<Thing>() { thing };
                foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                {
                    //output[key].Add(thing);
                    int hashcode = key.GetHashCode();
                    if (TypeList.TryGetValue(hashcode, out List<Thing> ThingsWithin))
                    {
                        ThingsWithin.Add(thing);
                    }
                    else
                    {
                        TypeList[hashcode] = new List<Thing>() { thing };
                    }

                    //_allObjectsByType.Add(key, obj);
                }
                Buckets[item] = TypeList;
                //Console.WriteLine(item);
            }
        }
        // HashSet<int> objects;
        public IEnumerable<Thing> GetNearby(Thing thing, Type t)
        {
            return GetThings(thing.position, thing.right - thing.left, thing.bottom - thing.top, t);
        }
        public IEnumerable<Thing> CollisionRectAll(Vec2 p1, Vec2 p2, Type t)
        {
            float Width = p1.x - p2.x;
            float Height = p1.y - p2.y;
            Width = Width > 0 ? Width : -Width;
            Height = Height > 0 ? Height : -Height;
            Width += cellsize * 2;
            Height += cellsize * 2;
            Vec2 position = new Vec2(p1.x + (Width / 2), p1.y + (Height / 2));
            return GetThings(position, Width, Height, t);
        }
        public IEnumerable<Thing> CollisionLineAll(Vec2 p1, Vec2 p2, Type t)
        {
            return GetThings(p1, p2, t);
        }

        public Vec2[] GetIdForLine(Vec2 p1, Vec2 p2)
        {
            //Vec2[] Chunk = new Vec2[0];
            List<Vec2> Chunks = new List<Vec2>();
            int y0 = (int)((p2.y + offset) / cellsize);
            int y1 = (int)((p1.y + offset) / cellsize);
            int x0 = (int)((p2.x + offset) / cellsize);
            int x1 = (int)((p1.x + offset) / cellsize);
            int top = y1 < y0 ? y1 : y0;
            int bottom = y0 < y1 ? y1 : y0;
            int left = x1 < x0 ? x1 : x0;
            int right = x0 < x1 ? x1 : x0;
            if (LineLeniancy != 0)
            {
                top -= LineLeniancy;
                bottom += LineLeniancy;
                left -= LineLeniancy;
                right += LineLeniancy;
            }
            foreach (Vec2 Bucket in Buckets.Keys)
            {
                if (left <= Bucket.x && right >= Bucket.x && top <= Bucket.y && bottom >= Bucket.y )
                {
                    Chunks.Add(Bucket);
                }
            }
            return Chunks.ToArray();
            //int y1 = (int)((p1.y + offset) / cellsize);
            //int x0 = (int)((p2.x + offset) / cellsize);
            //int x1 = (int)((p1.x + offset) / cellsize);
            //int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            //int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            //int err = (dx > dy ? dx : -dy) / 2, e2;
            //List<Vec2> Chunk = new List<Vec2>();
            //for (; ; )
            //{
            //    Chunk.Add(new Vec2(x0, y0));
            //    //bitmap.SetPixel(x0, y0, color);
            //    if (x0 == x1 && y0 == y1) break;
            //    e2 = err;
            //    if (e2 > -dx) { err -= dy; x0 += sx; }
            //    if (e2 < dy) { err += dx; y0 += sy; }
            //}
            ////Vec2[] Chunk = new Vec2[(right - left + 1) * (top - bottom + 1)];
            ////int N = -1;
            ////for (int x = left; x <= right; x++)
            ////{
            ////    for (int y = bottom; y <= top; y++)
            ////    {
            ////        N += 1;
            ////        Chunk[N] = new Vec2(x, y);
            ////    }
            ////}
            //return Chunk.ToArray();
        }
        public IEnumerable<Thing> CollisionCircleAll(Vec2 position, float radius, Type t)
        {
            return GetThings(position, radius * 2, radius * 2, t);
        }
        public ICollection<Thing> CollisionPointAll(Vec2 point, Type t)
        {
            if (Buckets.TryGetValue(new Vec2((int)((point.x + offset) / cellsize), (int)((point.y + offset) / cellsize)), out Dictionary<int, List<Thing>> output))
            {
                if (output.TryGetValue(t.GetHashCode(), out List<Thing> output2))
                {
                    return output2;
                }
            }
            return new List<Thing>();
        }
        public List<Thing> CollisionPointAllFast(Vec2 point, Type t)
        {
            if (Buckets.TryGetValue(new Vec2((int)((point.x + offset) / cellsize), (int)((point.y + offset) / cellsize)), out Dictionary<int, List<Thing>> output))
            {
                if (output.TryGetValue(t.GetHashCode(), out List<Thing> output2))
                {
                    return output2;
                }
            }
            return new List<Thing>();
        }
        public void RemoveDan(Thing thing)
        {
            //Vec2 p1 = thing.topLeft;
            //Vec2 p2 = thing.bottomRight;
            //float Length = Math.Abs(p1.x - p2.x);
            //float Height = Math.Abs(p1.y - p2.y);
            //float max = Math.Max(Length, Height);
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<int, List<Thing>> output))
                {
                    foreach (Type key in Editor.AllBaseTypes[thing.GetType()])
                    {

                        if (output.TryGetValue(key.GetHashCode(), out List<Thing> output2))
                        {
                            output2.Remove(thing);
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    if (output.TryGetValue(typeof(Thing).GetHashCode(), out List<Thing> output3))
                    {
                        output3.Remove(thing);
                    }
                    //output.Remove(thing);
                }
            }
            thing.Buckets = new Vec2[0];
        }
        private void RemoveUpdateList(Thing removeThing)
        {
            if (!removeThing.shouldbeinupdateloop) //|| removeThing is Block || removeThing is AutoPlatform || removeThing is Nubber || WindowFrame)
                return;
            RealupdateList.Remove(removeThing);
        }
        private void AddUpdateList(Thing removeThing)
        {
            if (!removeThing.shouldbeinupdateloop && ModLoader.ShouldOptimizations) //|removeThing is Block || removeThing is AutoPlatform || removeThing is Nubber)
                return;
            RealupdateList.Add(removeThing);
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
                RemoveUpdateList(removeThing);
                RemoveDan(removeThing);
                _bigList.Remove(removeThing);
                objectsDirty = true;
            }
            _removeThings.Clear();
            foreach (Thing addThing in _addThings)
            {
                AddUpdateList(addThing);
                if (!_bigList.Contains(addThing))
                {
                    RegisterObject(addThing);
                    _bigList.Add(addThing);
                }
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

        private void addItem(MultiMap<Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                list.Add(key, obj);
                _allObjectsByType.Add(key, obj);
            }
        }

        private void removeItem(MultiMap<Type, Thing, HashSet<Thing>> list, Thing obj)
        {
            foreach (Type key in Editor.AllBaseTypes[obj.GetType()])
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
