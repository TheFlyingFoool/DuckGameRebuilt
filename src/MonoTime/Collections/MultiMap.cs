// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiMap`3
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>A map of key -&gt; collection&lt;element&gt;</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TList">The type collection to use as backing storage.</typeparam>
    public class MultiMap<TKey, TElement, TList> :
    IEnumerable<KeyValuePair<TKey, ICollection<TElement>>>,
    IEnumerable
    where TList : ICollection<TElement>, new()
    {
        private Dictionary<TKey, TList> _map = new Dictionary<TKey, TList>();

        public int Count => _map.Count;

        public int CountValues
        {
            get
            {
                int countValues = 0;
                foreach (KeyValuePair<TKey, TList> keyValuePair in _map)
                    countValues += keyValuePair.Value.Count;
                return countValues;
            }
        }

        public void Add(TKey key, TElement element)
        {
            TList list;
            if (!_map.TryGetValue(key, out list))
                _map.Add(key, list = new TList());
            list.Add(element);
        }

        public void Insert(TKey key, int index, TElement value)
        {
            TList list;
            if (!_map.TryGetValue(key, out list))
                _map.Add(key, list = new TList());
            ((object)list as IList<TElement>).Insert(index, value);
        }

        public void AddRange(TKey key, ICollection<TElement> value)
        {
            TList list1;
            if (!_map.TryGetValue(key, out list1))
            {
                try
                {
                    _map.Add(key, (TList)Activator.CreateInstance(typeof(TList), value));
                }
                catch
                {
                    TList list2;
                    _map.Add(key, list2 = new TList());
                    foreach (TElement element in (IEnumerable<TElement>)value)
                        list2.Add(element);
                }
            }
            else
            {
                foreach (TElement element in (IEnumerable<TElement>)value)
                    list1.Add(element);
            }
        }

        public bool Remove(TKey key, TElement element)
        {
            TList list;
            if (!_map.TryGetValue(key, out list))
                return false;
            int num = list.Remove(element) ? 1 : 0;
            if (list.Count != 0)
                return num != 0;
            _map.Remove(key);
            return num != 0;
        }

        public void Remove(TKey key) => _map.Remove(key);

        public TList this[TKey key] => _map[key];

        public bool Contains(TKey key, TElement value)
        {
            TList list;
            return _map.TryGetValue(key, out list) && list.Contains(value);
        }

        public bool ContainsKey(TKey key) => _map.ContainsKey(key);

        public bool TryGetValue(TKey key, out TList list) => _map.TryGetValue(key, out list);

        public void Clear() => _map.Clear();

        public IEnumerable<TKey> Keys => _map.Keys;

        public IEnumerable<TList> Values => _map.Values;

        public IEnumerator<KeyValuePair<TKey, TList>> GetEnumerator() => _map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<TKey, ICollection<TElement>>> IEnumerable<KeyValuePair<TKey, ICollection<TElement>>>.GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TList> keyValuePair in _map)
                yield return new KeyValuePair<TKey, ICollection<TElement>>(keyValuePair.Key, keyValuePair.Value);
        }
    }
}
