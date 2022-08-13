// Decompiled with JetBrains decompiler
// Type: DuckGame.Promise`1
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Promise<T> : Promise
    {
        public T Result { get; private set; }

        public Promise(Func<T> function)
          : base(function)
        {
        }

        public override void Execute()
        {
            Result = (T)_delegate.Method.Invoke(_delegate.Target, null);
            Finished = true;
        }
    }
}
