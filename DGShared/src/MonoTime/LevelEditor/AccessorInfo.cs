// Decompiled with JetBrains decompiler
// Type: DuckGame.AccessorInfo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
