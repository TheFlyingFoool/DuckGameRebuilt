using System;

namespace DuckGame
{
    public class AccessorInfo
    {
        public Action<object, object> setAccessor;
        public Func<object, object> getAccessor;
        public Type type;

        public T Get<T>(object o) => getAccessor != null ? (T)getAccessor(o) : default(T);

        public void Set<T>(object o, T value)
        {
            if (setAccessor == null)
                return;
            setAccessor(o, value);
        }
    }
}
