// Decompiled with JetBrains decompiler
// Type: System.Collections.Concurrent.ConcurrentStack`1
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Collections.Concurrent
{
    [DebuggerDisplay("Count = {Count}")]
    public class ConcurrentStack<T> :
    IProducerConsumerCollection<T>,
    IEnumerable<T>,
    IEnumerable,
    ICollection
    {
        private ConcurrentStack<T>.Node head;
        private int count;
        private object syncRoot = new object();

        public ConcurrentStack()
        {
        }

        public ConcurrentStack(IEnumerable<T> collection)
        {
            foreach (T obj in collection)
                this.Push(obj);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T elem)
        {
            this.Push(elem);
            return true;
        }

        public void Push(T item)
        {
            ConcurrentStack<T>.Node node = new ConcurrentStack<T>.Node
            {
                Value = item
            };
            do
            {
                node.Next = this.head;
            }
            while (Interlocked.CompareExchange<ConcurrentStack<T>.Node>(ref this.head, node, node.Next) != node.Next);
            Interlocked.Increment(ref this.count);
        }

        public void PushRange(T[] items) => this.PushRange(items, 0, items.Length);

        public void PushRange(T[] items, int startIndex, int count)
        {
            ConcurrentStack<T>.Node node1 = null;
            ConcurrentStack<T>.Node node2 = null;
            for (int index = startIndex; index < count; ++index)
            {
                ConcurrentStack<T>.Node node3 = new ConcurrentStack<T>.Node
                {
                    Value = items[index],
                    Next = node1
                };
                node1 = node3;
                if (node2 == null)
                    node2 = node3;
            }
            do
            {
                node2.Next = this.head;
            }
            while (Interlocked.CompareExchange<ConcurrentStack<T>.Node>(ref this.head, node1, node2.Next) != node2.Next);
            Interlocked.Add(ref count, count);
        }

        public bool TryPop(out T result)
        {
            ConcurrentStack<T>.Node head;
            do
            {
                head = this.head;
                if (head == null)
                {
                    result = default(T);
                    return false;
                }
            }
            while (Interlocked.CompareExchange<ConcurrentStack<T>.Node>(ref this.head, head.Next, head) != head);
            Interlocked.Decrement(ref this.count);
            result = head.Value;
            return true;
        }

        public int TryPopRange(T[] items) => this.TryPopRange(items, 0, items.Length);

        public int TryPopRange(T[] items, int startIndex, int count)
        {
            ConcurrentStack<T>.Node comparand;
            ConcurrentStack<T>.Node node;
            do
            {
                comparand = this.head;
                if (comparand == null)
                    return -1;
                node = comparand;
                for (int index = 0; index < count - 1; ++index)
                {
                    node = node.Next;
                    if (node == null)
                        break;
                }
            }
            while (Interlocked.CompareExchange<ConcurrentStack<T>.Node>(ref this.head, node, comparand) != comparand);
            int index1;
            for (index1 = startIndex; index1 < count && comparand != null; ++index1)
            {
                items[index1] = comparand.Value;
                comparand = comparand.Next;
            }
            return index1 - 1;
        }

        public bool TryPeek(out T result)
        {
            ConcurrentStack<T>.Node head = this.head;
            if (head == null)
            {
                result = default(T);
                return false;
            }
            result = head.Value;
            return true;
        }

        public void Clear()
        {
            this.count = 0;
            this.head = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.InternalGetEnumerator();

        public IEnumerator<T> GetEnumerator() => this.InternalGetEnumerator();

        private IEnumerator<T> InternalGetEnumerator()
        {
            ConcurrentStack<T>.Node my_head = this.head;
            if (my_head != null)
            {
                do
                {
                    yield return my_head.Value;
                }
                while ((my_head = my_head.Next) != null);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (!(array is T[] array1))
                return;
            this.CopyTo(array1, index);
        }

        public void CopyTo(T[] array, int index)
        {
            IEnumerator<T> enumerator = this.InternalGetEnumerator();
            int num = index;
            while (enumerator.MoveNext())
                array[num++] = enumerator.Current;
        }

        bool ICollection.IsSynchronized => true;

        bool IProducerConsumerCollection<T>.TryTake(out T item) => this.TryPop(out item);

        object ICollection.SyncRoot => this.syncRoot;

        public T[] ToArray()
        {
            T[] array = new T[this.count];
            this.CopyTo(array, 0);
            return array;
        }

        public int Count => this.count;

        public bool IsEmpty => this.count == 0;

        private class Node
        {
            public T Value;
            public ConcurrentStack<T>.Node Next;
        }
    }
}
