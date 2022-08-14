// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadTreeObjectList
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
namespace DuckGame
{
    public class QuadTreeObjectList : IEnumerable<Thing>, IEnumerable
    {
        private int hashcodeindex;
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

        public HashSet<Thing> RealupdateList = new HashSet<Thing>();
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

        //static int Radius = 10;
        //private List<int> GetIdForObj(Vec2 position, float Radius)
        //{
        //    List<int> bucketsObjIsIn = new List<int>();

        //    Vec2 min = new Vec2(
        //        position.x - (Radius),
        //        position.y - (Radius));
        //    Vec2 max = new Vec2(
        //        position.x + (Radius),
        //        position.y + (Radius));

        //    float width = CellSize;
        //    //TopLeft
        //    Vec2 Vholder = min;
        //    int cellPosition = (int)(
        //              (Math.Floor(Vholder.x / CellSize)) +
        //              (Math.Floor(Vholder.y / CellSize)) *
        //              width
        //   );
        //    if (!bucketsObjIsIn.Contains(cellPosition))
        //        bucketsObjIsIn.Add(cellPosition);


        //    //TopRight
        //    Vholder = new Vec2(max.x, min.y);
        //    cellPosition = (int)(
        //              (Math.Floor(Vholder.x / CellSize)) +
        //              (Math.Floor(Vholder.y / CellSize)) *
        //              width
        //   );
        //    if (!bucketsObjIsIn.Contains(cellPosition))
        //        bucketsObjIsIn.Add(cellPosition);

        //    //BottomRight
        //    Vholder = new Vec2(max.x, max.y);
        //    cellPosition = (int)(
        //              (Math.Floor(Vholder.x / CellSize)) +
        //              (Math.Floor(Vholder.y / CellSize)) *
        //              width
        //   );
        //    if (!bucketsObjIsIn.Contains(cellPosition))
        //        bucketsObjIsIn.Add(cellPosition);
        //    Vholder = new Vec2(min.x, max.y);
        //    cellPosition = (int)(
        //              (Math.Floor(Vholder.x / CellSize)) +
        //              (Math.Floor(Vholder.y / CellSize)) *
        //              width
        //   );
        //    if (!bucketsObjIsIn.Contains(cellPosition))
        //        bucketsObjIsIn.Add(cellPosition);

        //    return bucketsObjIsIn;
        //}

        //private static int[] GetIdForObj(Vec2 position, float Radius, Thing thing = null) // there were  * width; here btw future dan and some screen stuff idk  //int width = CellSize;
        //{
        //    //float mxcell = (position.x - Radius) / CellSize;
        //    //float mycell = (position.y - Radius) / CellSize;
        //    //float Mxcell = (position.x + Radius) / CellSize;
        //    //float Mycell = (position.y + Radius) / CellSize;
        //    int mxcell = (int)((position.x - Radius) / CellSize);
        //    int mycell = (int)((position.y - Radius) / CellSize);
        //    int Mxcell = (int)((position.x + Radius) / CellSize);
        //    int Mycell = (int)((position.y + Radius) / CellSize); //TopLeft - BottomRight

        //    int TopLeft = (int)(int(mxcell + mycell) * CellSize);
        //    int BottomRight = (int)(Mxcell + Mycell * CellSize);
        //    int BottomLeft = (int)(mxcell + Mycell * CellSize);
        //    int TopRight = (int)(Mxcell + mycell * CellSize);
        //    int localarea = (Math.Abs(TopRight - BottomLeft ) +Math.Abs( BottomRight - TopLeft)) / 2;
        //    if (Math.Abs(TopLeft - BottomRight) > 1 && thing != null) // BottomRight
        //    {
        //        if (!RadiusC.Contains(Radius) || !postionsC.Contains(position))
        //        {

        //            RadiusC.Add(Radius);
        //            postionsC.Add(position);
        //            DevConsole.Log(TopLeft.ToString() + "  " + TopRight.ToString() + "  " + BottomRight.ToString() + "  " + BottomLeft.ToString() + " " + thing.ToString());
        //        }
        //    }



        //    if (TopLeft != BottomLeft)
        //    {
        //        if (BottomLeft != BottomRight)
        //        {
        //            return new int[] { TopLeft, BottomRight, BottomLeft, (int)(Mxcell + mycell * CellSize )};
        //        }
        //        return new int[] { TopLeft, BottomLeft };
        //    }
        //    else if (TopLeft != BottomRight)
        //    {
        //        return new int[] { TopLeft, BottomRight };
        //    }
        //    return new int[] { TopLeft };
        //}
        //private int[] GetIdForObj(Vec2 Position, float Radius, Thing thing = null)
        //{
        //    List<int> bucketsObjIsIn = new List<int>();

        //    Vec2 min = new Vec2(
        //        Position.x - (Radius),
        //        Position.y - (Radius));
        //    Vec2 max = new Vec2(
        //        Position.x + (Radius),
        //        Position.y + (Radius));
        //    //  ( (Math.Floor(Position.x / CellSize)) + (Math.Floor(Position.y / CellSize)) * width);
        //    float width = CellSize;// (CellSize * 4.0f) / CellSize;/*SceneWidth / CellSize;*/
        //    //
        //    int cellPosition = (int)((Math.Floor(min.x / CellSize)) + (Math.Floor(min.y / CellSize)) * width);
        //    if (!bucketsObjIsIn.Contains(cellPosition))
        //        bucketsObjIsIn.Add(cellPosition);

        //    int cellPosition2 = (int)((Math.Floor(max.x / CellSize)) + (Math.Floor(min.y / CellSize)) * width);
        //    if (!bucketsObjIsIn.Contains(cellPosition2))
        //        bucketsObjIsIn.Add(cellPosition2);

        //    int cellPosition3 = (int)((Math.Floor(max.x / CellSize)) + (Math.Floor(max.y / CellSize)) * width); //BottomRight
        //    if (!bucketsObjIsIn.Contains(cellPosition3))
        //        bucketsObjIsIn.Add(cellPosition3);

        //    int cellPosition4  = (int)((Math.Floor(min.x / CellSize)) + (Math.Floor(max.y / CellSize)) * width); //BottomRight
        //    if (!bucketsObjIsIn.Contains(cellPosition4))
        //        bucketsObjIsIn.Add(cellPosition4);


        //    if (Math.Abs(cellPosition - cellPosition2) > 1 && thing != null) // BottomRight
        //    {
        //        if (!RadiusC.Contains(Radius) || !postionsC.Contains(Position))
        //        {

        //            RadiusC.Add(Radius);
        //            postionsC.Add(Position);
        //            DevConsole.Log(cellPosition.ToString() + "  " + cellPosition2.ToString() + "  " + cellPosition3.ToString() + "  " + cellPosition4.ToString() + " " + thing.ToString());
        //        }
        //    }

        //    return bucketsObjIsIn.ToArray();
        //    }
        //         if (false)//thing is BlockGroup
        //            {
        //                List<Vec2> BlockGBuckets = new List<Vec2>();
        //                foreach (Block b in (thing as BlockGroup).blocks) // float size = Math.Max(Math.Max(b.right - b.left, b.bottom - b.top), 16);
        //                {
        //                    Vec2[] _buckets = GetIdForObj(thing.position, b.right - b.left, b.bottom - b.top);
        //    thing.oldposition = thing.position;
        //                    foreach (Vec2 item in _buckets)
        //                    {
        //                        if (Buckets.TryGetValue(item, out List<Thing> output))
        //                        {
        //                            output.Add(thing);
        //                            continue;
        //                        }
        //Buckets.Add(item, new List<Thing>() { thing
        //});
        //                    }
        //                    BlockGBuckets.AddRange(_buckets);
        //                }
        //                thing.Buckets = BlockGBuckets.ToArray();

        //            }
        public static float offset = 4000000.0f;
        public static float cellsize = 100f;
        private Vec2[] GetIdForObj(Vec2 Topleft , Vec2 Bottomright)
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
        private Vec2[] GetIdForObj(Vec2 Position, float width, float height)
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
        public static bool ValueinList(Vec2[] array, Vec2 value, int count)
        {
            int startIndex = 0;
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (array[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool NotinList(Vec2[] array, Vec2[] value, int count)
        {
            int startIndex = 0;
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (ValueinList(value,array[i], value.Length))
                {
                    return false;
                }
            }
            return true;
        }
        //if (item.Buckets.Length == 1)
        //{
        //    objects.Add(item);
        //}
        //else if (item.Buckets.Length == 2 && ids.Length == 2 && !((item.Buckets[0] != ids[0] || item.Buckets[0] != ids[1]) || (item.Buckets[0] != ids[0] || item.Buckets[0] != ids[1])))
        //{
        //    objects.Add(item);
        //}
        //else if (!ints.Contains(item.hashcodeindex))
        //{
        //    ints.Add(item.hashcodeindex);
        //    objects.Add(item);
        //}
        private static Vec2 cantbe = new Vec2(-0.5f,0.5f);
        public ICollection<Thing> GetThings(Vec2 Position, float width, float height, Type t)
        {
            //if (t == typeof(Thing))
            //{
            //    cantbe = new Vec2(-0.5f, 0.5f);
            //}
            Vec2[] ids = GetIdForObj(Position, width, height);
            if (ids.Length == 1)
            {
                if (Buckets.TryGetValue(ids[0], out Dictionary<Type, List<Thing>> output))
                {
                    if (output.TryGetValue(t, out List<Thing> outputthings))
                    {
                        return outputthings;
                    }
                }
                return new List<Thing>();
            }
            Vec2[] usedids = new Vec2[ids.Length];
            List<Thing> objects = new List<Thing>();
            int n = -1;
            foreach (Vec2 bucket in ids)
            {
                n += 1;
                if (Buckets.TryGetValue(bucket, out Dictionary<Type, List<Thing>> output))
                {
                    if (output.TryGetValue(t, out List<Thing> outputthings))
                    {
                        foreach (Thing item in outputthings)
                        {
                            if (item.Buckets.Length == 1)
                            {
                                objects.Add(item);
                            }
                            else if (NotinList(usedids, item.Buckets, usedids.Length)) //n
                            {
                                objects.Add(item);
                            }
                        }
                    }
                }
                usedids[n] = bucket;
            }
            return objects;
        }

        // public Dictionary<Vec2, List<Thing>> Buckets = new Dictionary<Vec2, List<Thing>>();
        public Dictionary<Vec2, Dictionary<Type, List<Thing>>> Buckets = new Dictionary<Vec2, Dictionary<Type, List<Thing>>>();
        public void UpdateObject(Thing thing)  //float size = Math.Max(Math.Max(thing.right - thing.left, thing.bottom - thing.top), 16);
        {
            Vec2[] buckets = GetIdForObj(thing.topLeft, thing.bottomRight);//GetIdForObj(thing.position, thing.right - thing.left, thing.bottom - thing.top);
            thing.oldposition = thing.position;
            if (thing.Buckets.SequenceEqual(buckets))
            {
                return;
            }
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<Type, List<Thing>> output))
                {
                    foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        if (output.TryGetValue(key, out List<Thing> output2))
                        {
                            output2.Remove(thing);
                        }
                        else
                        {
                            output[key] = new List<Thing>() { };
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    //output[typeof(Thing)].Add(thing);
                    continue;
                }
                output = new Dictionary<Type, List<Thing>>();
                output[typeof(Thing)] = new List<Thing>() { };
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
                if (Buckets.TryGetValue(item, out Dictionary<Type, List<Thing>> output))
                {
                    foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        if (output.TryGetValue(key, out List<Thing> output2))
                        {
                            output2.Add(thing);
                        }
                        else
                        {
                            output[key] = new List<Thing>() { thing };
                        }

                        //_allObjectsByType.Add(key, obj);
                    }
                    output[typeof(Thing)].Add(thing);
                    continue;
                }
                output = new Dictionary<Type, List<Thing>>();
                output[typeof(Thing)] = new List<Thing>() {thing };
                foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                {
                    //output[key].Add(thing);
                    if (output.TryGetValue(key, out List<Thing> output2))
                    {
                        output2.Add(thing);
                    }
                    else
                    {
                        output[key] = new List<Thing>() { thing };
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



            thing.Buckets = GetIdForObj(thing.topLeft, thing.bottomRight); // float size = Math.Max(Math.Max(thing.right - thing.left, thing.bottom - thing.top), 16);
            thing.oldposition = thing.position;
            foreach (Vec2 item in thing.Buckets)
            {
                if (Buckets.TryGetValue(item, out Dictionary<Type, List<Thing>> TypeList))
                {
                    foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        //output[key].Add(thing);
                        if (TypeList.TryGetValue(key, out List<Thing> ThingsWithin))
                        {
                            ThingsWithin.Add(thing);
                        }
                        else
                        {
                            TypeList[key] = new List<Thing>() { thing };
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    TypeList[typeof(Thing)].Add(thing);
                    continue;
                }
                TypeList = new Dictionary<Type, List<Thing>>();
                TypeList[typeof(Thing)] = new List<Thing>() { thing };
                foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                {
                    //output[key].Add(thing);
                    if (TypeList.TryGetValue(key, out List<Thing> ThingsWithin))
                    {
                        ThingsWithin.Add(thing);
                    }
                    else
                    {
                        TypeList[key] = new List<Thing>() { thing };
                    }
                    
                    //_allObjectsByType.Add(key, obj);
                }
                Buckets[item] = TypeList;
                //Console.WriteLine(item);
            }
        }
        // HashSet<int> objects;
        public ICollection<Thing> GetNearby(Thing thing,Type t)
        {
            return GetThings(thing.position, thing.right - thing.left, thing.bottom - thing.top,t);
        }
        public ICollection<Thing> CollisionRectAll(Vec2 p1, Vec2 p2, Type t)
        {
            float Width = p1.x - p2.x;
            float Height = p1.y - p2.y;
            Width = Width > 0 ? Width : -Width;
            Height = Height > 0 ? Height : -Height;
            Vec2 position = new Vec2(p1.x + (Width / 2), p1.y + (Height / 2));
            return GetThings(position, Width, Height, t);
        }
        public ICollection<Thing> CollisionCircleAll(Vec2 position, float radius, Type t)
        {
            return GetThings(position, radius * 2, radius * 2, t);
        }
        public ICollection<Thing> CollisionPointAll(Vec2 point, Type t)
        {
            if (Buckets.TryGetValue(new Vec2((int)((point.x + offset )/ cellsize), (int)((point.y + offset) / cellsize)), out Dictionary<Type, List<Thing>> output))
            {
                if (output.TryGetValue(t, out List<Thing> output2))
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
                if (Buckets.TryGetValue(item, out Dictionary<Type, List<Thing>> output))
                {
                    foreach (System.Type key in Editor.AllBaseTypes[thing.GetType()])
                    {
                        if (output.TryGetValue(key, out List<Thing> output2))
                        {
                            output2.Remove(thing);
                        }
                        //_allObjectsByType.Add(key, obj);
                    }
                    if (output.TryGetValue(typeof(Thing), out List<Thing> output3))
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
            if (!removeThing.shouldbeinupdateloop) //|removeThing is Block || removeThing is AutoPlatform || removeThing is Nubber)
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
                hashcodeindex += 1;
                addThing.hashcodeindex = hashcodeindex;
                AddUpdateList(addThing);
                RegisterObject(addThing);
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
