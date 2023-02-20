using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DuckGame.Proxies
{
    internal class TypeNFieldInfoProxy
    {
        public static FieldInfo GetField(Type type, string name, BindingFlags flags)
        {
            if(type == typeof(Game) && name == "drawableComponents")
            {
                var f = typeof(Game).GetField("_drawables", BindingFlags.Instance | BindingFlags.NonPublic);
                var d = f.GetValue(MonoMain.instance) as List<IDrawable>;
                if (d == null)
                    throw new Exception("d was null");
                return f;
            }
            return type.GetField(name, flags);
        }
    }
}