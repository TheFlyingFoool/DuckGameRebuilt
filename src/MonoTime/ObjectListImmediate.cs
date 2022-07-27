// Decompiled with JetBrains decompiler
// Type: DuckGame.ObjectListImmediate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    public class ObjectListImmediate : IEnumerable<Thing>, IEnumerable
    {
        private HashSet<Thing> _emptyList = new HashSet<Thing>();
        private HashSet<Thing> _bigList = new HashSet<Thing>();
        private MultiMap<System.Type, Thing, HashSet<Thing>> _objectsByType = new MultiMap<System.Type, Thing, HashSet<Thing>>();

        public List<Thing> ToList()
        {
            List<Thing> list = new List<Thing>();
            list.AddRange(_bigList);
            return list;
        }

        public HashSet<Thing> this[System.Type key]
        {
            get
            {
                if (key == typeof(Thing))
                    return this._bigList;
                return this._objectsByType.ContainsKey(key) ? this._objectsByType[key] : this._emptyList;
            }
        }

        public int Count => this._bigList.Count;

        public void Add(Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
                this._objectsByType.Add(key, obj);
            this._bigList.Add(obj);
        }

        public void AddRange(ObjectListImmediate list)
        {
            foreach (Thing thing in list)
                this.Add(thing);
        }

        public void Remove(Thing obj)
        {
            foreach (System.Type key in Editor.AllBaseTypes[obj.GetType()])
                this._objectsByType.Remove(key, obj);
            this._bigList.Remove(obj);
        }

        public void Clear()
        {
            this._bigList.Clear();
            this._objectsByType.Clear();
        }

        public bool Contains(Thing obj) => this._bigList.Contains(obj);

        public IEnumerator<Thing> GetEnumerator() => this._bigList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
