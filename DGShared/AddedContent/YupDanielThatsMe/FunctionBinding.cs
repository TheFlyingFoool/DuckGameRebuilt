// Decompiled with JetBrains decompiler
// Type: DuckGame.FieldBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Reflection;

namespace DuckGame
{
    public class FunctionBinding
    {
        protected object _thing;
        protected MethodInfo _method;

        public object thing => _thing;

        public FunctionBinding(object thing, string method)
        {
            _thing = thing;
            _method = thing.GetType().GetMethod(method);
        }

        public FunctionBinding(Type thing, string method)
        {
            _thing = thing;
            _method = thing.GetMethod(method);
        }
        public object Call(object[] parameters = null)
        {
            return _method.Invoke(_thing, parameters);
        }
    }
}
