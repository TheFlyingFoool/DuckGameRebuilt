using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DuckGame
{
    public static class Extensions
    {
        public static IEnumerable<(TInfoType Member, IEnumerable<TAttribute> Attributes)>
        GetAllMembersWithAttribute<TInfoType, TAttribute>(Type? inType = null) where TInfoType : MemberInfo
        where TAttribute : Attribute
        {
            MemberTypes memberType = MemberTypes.All;

            if (typeof(TInfoType).IsAssignableFrom(typeof(FieldInfo)))
                memberType = MemberTypes.Field;
            else if (typeof(TInfoType).IsAssignableFrom(typeof(MethodInfo)))
                memberType = MemberTypes.Method;
            else if (typeof(TInfoType).IsAssignableFrom(typeof(PropertyInfo)))
                memberType = MemberTypes.Property;
            else if (typeof(TInfoType).IsAssignableFrom(typeof(ConstructorInfo)))
                memberType = MemberTypes.Constructor;
            else if (typeof(TInfoType).IsAssignableFrom(typeof(TypeInfo)))
                memberType = MemberTypes.TypeInfo;

            return GetAllMembersWithAttribute<TAttribute>(memberType, inType)
                .Select<(MemberInfo Member, IEnumerable<TAttribute> Attributes), (TInfoType, IEnumerable<TAttribute>)>
                    (x => ((TInfoType)x.Member, x.Attributes));
        }

        public static IEnumerable<(MemberInfo Member, IEnumerable<TAttribute> Attributes)>
            GetAllMembersWithAttribute<TAttribute>(MemberTypes filter = MemberTypes.All, Type? inType = null) 
            where TAttribute : Attribute
        {
            if (inType is { })
                return inType.GetMembers()
                    .Where(x => x.GetCustomAttributes<TAttribute>(false).Any()
                                && x.MemberType.HasFlag(filter))
                    .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));

            var types = Assembly.GetExecutingAssembly().GetTypes();
            
            return types
                .SelectMany(x => x.GetMembers())
                .Concat(types.Select(x => x.GetTypeInfo()))
                .Where(x => x.GetCustomAttributes<TAttribute>().Any())
                .Where(x => x.MemberType.HasFlag(filter))
                .Select(x => (x, x.GetCustomAttributes<TAttribute>(false)));
        }

        public const BindingFlags ALL_BINDING_FLAGS =
            BindingFlags.CreateInstance
            | BindingFlags.DeclaredOnly
            | BindingFlags.FlattenHierarchy
            | BindingFlags.IgnoreCase
            | BindingFlags.Instance
            | BindingFlags.NonPublic
            | BindingFlags.Public
            | BindingFlags.Static;

        public static bool TryFirst<T>(this IEnumerable<T> collection, Func<T, bool> condition, out T first)
        {
            first = default;
            
            foreach (var item in collection)
            {
                if (!condition(item)) 
                    continue;
                
                first = item;
                return true;
            }

            return false;
        }

        public static string GetFullName(this MemberInfo mi) => $"{mi.DeclaringType}:{mi.Name}";

        public static bool InheritsFrom(this Type t1, Type t2) => t2.IsAssignableFrom(t1);

        public static Vec2 ButX(this Vec2 vec2, Func<float, float> function) => new(function(vec2.x), vec2.y);
        public static Vec2 ButX(this Vec2 vec2, float newX, bool add = false) => vec2.ButX(x => add ? x + newX : newX);

        public static Vec2 ButY(this Vec2 vec2, Func<float, float> function) => new(vec2.x, function(vec2.y));
        public static Vec2 ButY(this Vec2 vec2, float newY, bool add = false) => vec2.ButY(y => add ? y + newY : newY);

        public static Vec2 ButBoth(this Vec2 vec2, Func<float, float> function) => vec2.ButX(function).ButY(function);
        public static Vec2 ButBoth(this Vec2 vec2, float newValue, bool add = false) => vec2.ButX(newValue, add).ButY(newValue, add);

        public static bool CaselessEquals(this string str, string str2) =>
            string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);

        public static string ToReadableString(this IEnumerable<object> collection, int indentationLevel = 0, bool doIndent = true)
        {
            object[] arr = collection.ToArray();
            StringBuilder stringBuilder = new();

            stringBuilder.Append("[\n");
            indentationLevel++;

            for (int i = 0; i < arr.Length; i++)
            {
                object o = arr[i];
                indentedAppend(o is IEnumerable subCollection and not string
                    ? subCollection.Cast<object>().ToReadableString(indentationLevel)
                    : o.ToString());

                if (i != arr.Length)
                    stringBuilder.Append(",\n");
            }

            indentationLevel--;
            indentedAppend("]");

            return stringBuilder.ToString();

            void indentedAppend(string s) =>
                stringBuilder.Append($"{(doIndent ? new string(' ', indentationLevel * 2) : "")}{s}");
        }

        public static bool Try(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool Try<T>(Func<T> action, out T result)
        {
            result = default;

            try
            {
                result = action();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public static T GetPrivateFieldValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null) 
                throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
            return (T)fi.GetValue(obj);
        }
        public static float Distance(this Vec2 pos1, Vec2 pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
        }
    }
}
