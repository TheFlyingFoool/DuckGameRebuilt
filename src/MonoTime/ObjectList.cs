// Decompiled with JetBrains decompiler
// Type: DuckGame.ObjectList
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ObjectList : IEnumerable<Thing>, IEnumerable
    {
        private List<Thing> _bigList = new List<Thing>();
        private MultiMap<string, Thing> _objects = new MultiMap<string, Thing>();
        private MultiMap<string, Thing> _addThings = new MultiMap<string, Thing>();
        private MultiMap<string, Thing> _removeThings = new MultiMap<string, Thing>();
        private MultiMap<System.Type, Thing> _objectsByType = new MultiMap<System.Type, Thing>();
        private bool _autoRefresh;

        public ObjectList(bool automatic = false) => _autoRefresh = automatic;

        public List<Thing> ToList()
        {
            List<Thing> list = new List<Thing>();
            foreach (List<Thing> collection in _objects.Values)
                list.AddRange(collection);
            return list;
        }

        public IEnumerable<Thing> this[System.Type key] => _objectsByType.ContainsKey(key) ? _objectsByType[key] : Enumerable.Empty<Thing>();

        public IEnumerable<Thing> this[string key] => _objects.ContainsKey(key) ? _objects[key] : Enumerable.Empty<Thing>();

        public Thing this[int key] => key < _bigList.Count ? _bigList[key] : null;

        public int Count => _bigList.Count;

        public void Add(Thing obj)
        {
            removeItem(_removeThings, obj);
            addItem(_addThings, obj);
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
            addItem(_removeThings, obj);
            removeItem(_addThings, obj);
            if (!_autoRefresh)
                return;
            RefreshState();
        }

        public void Clear()
        {
            for (int key = 0; key < Count; ++key)
                Remove(this[key]);
        }

        public bool Contains(Thing obj) => _bigList.Contains(obj);

        public void RefreshState()
        {
            foreach (KeyValuePair<string, List<Thing>> removeThing in (MultiMap<string, Thing, List<Thing>>)_removeThings)
            {
                foreach (Thing thing in removeThing.Value)
                {
                    _bigList.Remove(thing);
                    removeItem(_objects, thing);
                    removeItem(_objectsByType, thing);
                }
            }
            _removeThings.Clear();
            foreach (KeyValuePair<string, List<Thing>> addThing in (MultiMap<string, Thing, List<Thing>>)_addThings)
            {
                foreach (Thing thing in addThing.Value)
                {
                    _bigList.Add(thing);
                    addItem(_objects, thing);
                    addItem(_objectsByType, thing);
                }
            }
            _addThings.Clear();
        }

        private void addItem(MultiMap<string, Thing> list, Thing obj)
        {
            if (list.Contains(obj.type, obj))
                return;
            list.Add(obj.type, obj);
        }

        private void removeItem(MultiMap<string, Thing> list, Thing obj) => list.Remove(obj.type, obj);

        private void addItem(MultiMap<System.Type, Thing> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
            {
                if (list.Contains(key, obj))
                    break;
                list.Add(key, obj);
            }
        }

        private void removeItem(MultiMap<System.Type, Thing> list, Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
                list.Remove(key, obj);
        }

        public IEnumerator<Thing> GetEnumerator() => _bigList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
