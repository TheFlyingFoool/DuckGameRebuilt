// Decompiled with JetBrains decompiler
// Type: DuckGame.Map`2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>Maps a key and value to each other.</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class Map<TKey, TValue> :
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
    {
        private Dictionary<TKey, TValue> _byKey = new Dictionary<TKey, TValue>();
        private Dictionary<TValue, TKey> _byValue = new Dictionary<TValue, TKey>();

        public ICollection<TKey> Keys => _byKey.Keys;

        public ICollection<TValue> Values => _byKey.Values;

        public int Count => this._byKey.Count;

        public bool IsReadOnly => false;

        public TValue this[TKey key]
        {
            get => this._byKey[key];
            set
            {
                this._byKey[key] = value;
                this._byValue[value] = key;
            }
        }

        public TKey this[TValue val]
        {
            get => this._byValue[val];
            set
            {
                this._byValue[val] = value;
                this._byKey[value] = val;
            }
        }

        public void Add(TKey key, TValue value)
        {
            this._byKey.Add(key, value);
            this._byValue.Add(value, key);
        }

        public void Add(TValue value, TKey key)
        {
            this._byValue.Add(value, key);
            this._byKey.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            TValue key1;
            if (!this._byKey.TryGetValue(key, out key1))
                return false;
            this._byKey.Remove(key);
            this._byValue.Remove(key1);
            return true;
        }

        public bool Remove(TValue value)
        {
            TKey key;
            if (!this._byValue.TryGetValue(value, out key))
                return false;
            this._byKey.Remove(key);
            this._byValue.Remove(value);
            return true;
        }

        public TValue Get(TKey key) => this._byKey[key];

        public TKey Get(TValue value) => this._byValue[value];

        public bool ContainsKey(TKey key) => this._byKey.ContainsKey(key);

        public bool ContainsValue(TValue value) => this._byValue.ContainsKey(value);

        public bool Contains(TKey key) => this.ContainsKey(key);

        public bool Contains(TValue value) => this.ContainsValue(value);

        public bool TryGetValue(TKey key, out TValue value) => this._byKey.TryGetValue(key, out value);

        public bool TryGetKey(TValue value, out TKey key) => this._byValue.TryGetValue(value, out key);

        public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

        public void Add(KeyValuePair<TValue, TKey> item) => this.Add(item.Key, item.Value);

        public void Clear()
        {
            this._byKey.Clear();
            this._byValue.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this._byKey).Contains(item);

        public bool Contains(KeyValuePair<TValue, TKey> item) => ((ICollection<KeyValuePair<TValue, TKey>>)this._byValue).Contains(item);

        public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

        public bool Remove(KeyValuePair<TValue, TKey> item) => this.Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this._byKey).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this._byKey).GetEnumerator();

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
          KeyValuePair<TKey, TValue>[] array,
          int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}
