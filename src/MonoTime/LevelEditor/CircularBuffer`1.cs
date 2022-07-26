// Decompiled with JetBrains decompiler
// Type: DuckGame.CircularBuffer`1
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class CircularBuffer<T>
    {
        protected T[] _data;
        protected int _size;
        protected int _begin;
        protected int _length;

        public CircularBuffer(int size = 100)
        {
            this._data = new T[size];
            this._size = size;
            this._begin = 0;
            this._length = 0;
        }

        public void Add(T val)
        {
            if (this._length >= this._size)
                this.AdvanceBuffer();
            this._data[(this._begin + this._length) % this._size] = val;
            ++this._length;
        }

        public void AdvanceBuffer()
        {
            this._begin = (this._begin + 1) % this._size;
            --this._length;
            if (this._length >= 0)
                return;
            this._length = 0;
        }

        public T this[int key]
        {
            get
            {
                if (key >= this._length || key < 0)
                    throw new Exception("Array Index Out Of Range");
                return this._data[(this._begin + key) % this._size];
            }
            set => this._data[(this._begin + key) % this._size] = value;
        }

        public int Count => this._length;

        public void Clear() => this._length = 0;
    }
}
