// Decompiled with JetBrains decompiler
// Type: DuckGame.Organizer`2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Organizer<T, T2>
    {
        private MultiMap<T, T2> _items = new MultiMap<T, T2>();
        private List<T2> _globalItems = new List<T2>();

        public void Add(T2 val) => this._globalItems.Add(val);

        public void Add(T2 val, T group) => this._items.Add(group, val);

        public void Clear()
        {
            this._items = new MultiMap<T, T2>();
            this._globalItems = new List<T2>();
        }

        public bool HasGroup(T group) => this._globalItems.Count > 0 || this._items.ContainsKey(group);

        public T2 GetRandom(T group)
        {
            if (this._items.ContainsKey(group))
                return this._items[group][Rando.Int(this._items[group].Count - 1)];
            return this._globalItems.Count > 0 ? this._globalItems[Rando.Int(this._globalItems.Count - 1)] : default(T2);
        }

        public List<T2> GetList(T group)
        {
            if (this._items.ContainsKey(group))
                return this._items[group];
            return this._globalItems.Count > 0 ? this._globalItems : null;
        }
    }
}
