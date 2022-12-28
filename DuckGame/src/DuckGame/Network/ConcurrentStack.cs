// Decompiled with JetBrains decompiler
// Type: System.Collections.Concurrent.ConcurrentStack`1
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private Node head;
        private int count;
        private object syncRoot = new object();

        public ConcurrentStack()
        {
        }

        public ConcurrentStack(IEnumerable<T> collection)
        {
            foreach (T obj in collection)
                Push(obj);
        }

        bool IProducerConsumerCollection<T>.TryAdd(T elem)
        {
            Push(elem);
            return true;
        }

        public void Push(T item)
        {
            Node node = new Node
            {
                Value = item
            };
            do
            {
                node.Next = head;
            }
            while (Interlocked.CompareExchange(ref head, node, node.Next) != node.Next);
            Interlocked.Increment(ref count);
        }

        public void PushRange(T[] items) => PushRange(items, 0, items.Length);

        public void PushRange(T[] items, int startIndex, int count)
        {
            Node node1 = null;
            Node node2 = null;
            for (int index = startIndex; index < count; ++index)
            {
                Node node3 = new Node
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
                node2.Next = head;
            }
            while (Interlocked.CompareExchange(ref head, node1, node2.Next) != node2.Next);
            Interlocked.Add(ref count, count);
        }

        public bool TryPop(out T result)
        {
            Node head;
            do
            {
                head = this.head;
                if (head == null)
                {
                    result = default(T);
                    return false;
                }
            }
            while (Interlocked.CompareExchange(ref this.head, head.Next, head) != head);
            Interlocked.Decrement(ref count);
            result = head.Value;
            return true;
        }

        public int TryPopRange(T[] items) => TryPopRange(items, 0, items.Length);

        public int TryPopRange(T[] items, int startIndex, int count)
        {
            Node comparand;
            Node node;
            do
            {
                comparand = head;
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
            while (Interlocked.CompareExchange(ref head, node, comparand) != comparand);
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
            Node head = this.head;
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
            count = 0;
            head = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => InternalGetEnumerator();

        public IEnumerator<T> GetEnumerator() => InternalGetEnumerator();

        private IEnumerator<T> InternalGetEnumerator()
        {
            Node my_head = head;
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
            CopyTo(array1, index);
        }

        public void CopyTo(T[] array, int index)
        {
            IEnumerator<T> enumerator = InternalGetEnumerator();
            int num = index;
            while (enumerator.MoveNext())
                array[num++] = enumerator.Current;
        }

        bool ICollection.IsSynchronized => true;

        bool IProducerConsumerCollection<T>.TryTake(out T item) => TryPop(out item);

        object ICollection.SyncRoot => syncRoot;

        public T[] ToArray()
        {
            T[] array = new T[count];
            CopyTo(array, 0);
            return array;
        }

        public int Count => count;

        public bool IsEmpty => count == 0;

        private class Node
        {
            public T Value;
            public Node Next;
        }
    }
}
