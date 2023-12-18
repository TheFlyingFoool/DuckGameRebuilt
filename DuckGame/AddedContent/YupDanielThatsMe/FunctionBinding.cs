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
