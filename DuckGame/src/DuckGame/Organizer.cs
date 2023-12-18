using System.Collections.Generic;

namespace DuckGame
{
    public class Organizer<T, T2>
    {
        private MultiMap<T, T2> _items = new MultiMap<T, T2>();
        private List<T2> _globalItems = new List<T2>();

        public void Add(T2 val) => _globalItems.Add(val);

        public void Add(T2 val, T group) => _items.Add(group, val);

        public void Clear()
        {
            _items = new MultiMap<T, T2>();
            _globalItems = new List<T2>();
        }

        public bool HasGroup(T group) => _globalItems.Count > 0 || _items.ContainsKey(group);

        public T2 GetRandom(T group)
        {
            if (_items.ContainsKey(group))
                return _items[group][Rando.Int(_items[group].Count - 1)];
            return _globalItems.Count > 0 ? _globalItems[Rando.Int(_globalItems.Count - 1)] : default(T2);
        }

        public List<T2> GetList(T group)
        {
            if (_items.ContainsKey(group))
                return _items[group];
            return _globalItems.Count > 0 ? _globalItems : null;
        }
    }
}
