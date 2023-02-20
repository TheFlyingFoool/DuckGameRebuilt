using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DuckGame.Proxies
{
    public class ListProxy<T> : List<T>
    {
        Action<T> action;

        public ListProxy(Action<T> action)
        {
            this.action = action;
        }

        public void Add(T item)
        {
            if(action == null)
            {
                base.Add(item);
                return;
            }
            action(item);
        }
    }
}