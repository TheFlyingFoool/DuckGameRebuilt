using AddedContent.Firebreak;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Drawing;
using System.Web.UI.WebControls;

namespace DuckGame
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if there are any of <typeparamref name="T"/> in a thick line, the more iterations the more accurate it will be
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="thickness"></param>
        /// <param name="iters"></param>
        /// <returns></returns>
        public static T ThickCheckLine<T>(Vec2 p1, Vec2 p2, float thickness, int iters = 5)
        {
            Vec2 vec = Maths.AngleToVec(Maths.PointDirectionRad(p1, p2)).Rotate(1.5708f, Vec2.Zero) *
                       (thickness / iters);
            Vec2 v = vec * (float)Math.Floor(iters / 2f);
            T th = Level.CheckLine<T>(p1 + v, p2 + v);
            int op = 0;
            while (th == null && op < iters)
            {
                op++;
                v -= vec;
                th = Level.CheckLine<T>(p1 + v, p2 + v);
            }

            return th;
        }

        /// <summary>
        /// Checks all the <typeparamref name="T"/> in a thick line, the more iterations the more accurate it will be
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="thickness"></param>
        /// <param name="iters"></param>
        /// <returns>Returns a List of <typeparamref name="T"/> found</returns>
        public static List<T> ThickCheckLineAll<T>(Vec2 p1, Vec2 p2, float thickness, int iters = 4)
        {
            Vec2 vec = Maths.AngleToVec(Maths.PointDirectionRad(p1, p2)).Rotate(1.5708f, Vec2.Zero) *
                       (thickness / iters);
            Vec2 v = vec * (float)Math.Floor(iters / 2f);
            List<T> th = new List<T>();
            for (int i = 0; i < iters; i++)
            {
                v -= vec;
                List<T> epic = Level.CheckLineAll<T>(p1 + v, p2 + v).ToList();
                for (int x = 0; x < epic.Count; x++)
                {
                    if (!th.Contains(epic[x])) th.Add(epic[x]);
                }
            }

            return th;
        }

        public static int AutoBlockSortred(AutoBlock b1, AutoBlock b2)
        {
            var xx = b1.blockIndex;
            var yy = b2.blockIndex;
            if (xx < yy) return -1;
            else if (yy < xx) return 1;
            return 0;
        }

        public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        public static IEnumerable<Type> GetSubclasses(Type t)
        {
            List<Type> subclasses = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(t))
                        {
                            subclasses.Add(type);
                        }
                    }
                }
                catch
                {
                }
            }

            return subclasses;
        }
        
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static List<Type> GetSubclassesList(Type t)
        {
            List<Type> subclasses = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(t))
                        {
                            subclasses.Add(type);
                        }
                    }
                }
                catch
                {
                }
            }

            return subclasses;
        }

        public static List<T> GetListOfThings<T>()
        {
            if (Level.current == null) return null;

            //check if cant convert T to other thing to avoid crash
            if (!(typeof(T).IsAssignableFrom(typeof(IConvertible))))
            {
                try
                {
                    return Level.current.things[typeof(T)].Cast<T>().ToList();
                }
                catch
                {
                    return Level.current.things[typeof(T)].OfType<T>().ToList();
                }
            }

            return Level.current.things[typeof(T)].Select(t => (T)Convert.ChangeType(t, typeof(T))).ToList();
        }

        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }

            if (fi == null)
                throw new ArgumentOutOfRangeException("propName",
                    string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            fi.SetValue(obj, val);
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        [Marker.DevConsoleCommand(Name = "playvgm")]
        public static void PlayVGM()
        {
            VGMSong vs = new VGMSong(DuckFile.contentDirectory + "/Audio/test.vgm");
            vs.Play();
            vs.looped = false;
        }

        [Marker.DevConsoleCommand(Name = "playvgz")]
        public static void PlayVGZ()
        {
            VGMSong vs = new VGMSong(DuckFile.contentDirectory + "/Audio/test.vgz");
            vs.Play();
            vs.looped = false;
        }

        [Marker.DevConsoleCommand(Name = "playdgm")]
        public static void PlayDGM()
        {
            DGMSong vs = new DGMSong("Content/Audio/test.dgm");
            vs.Play();
            vs.looped = false;
        }

        static Type PatchProcessorT;
        static MethodInfo GetPatchInfoM;
        static FieldInfo DynOwner;
        static FieldInfo DynTOwner;
        static FieldInfo prefixes;
        static FieldInfo postfixes;
        static FieldInfo transpilers;
        static FieldInfo Patch;
        static bool HarmonyActive;

        static bool HarmonyLoaded()
        {
            if (HarmonyActive)
                return true;
            Assembly a = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.GetName().Name == "HarmonyLoader");
            if (a == null)
                return false;
            PatchProcessorT = Type.GetType($"Harmony.PatchProcessor, {a.FullName}");

            GetPatchInfoM = PatchProcessorT?.GetMethod("GetPatchInfo");
            Type ps = Type.GetType($"Harmony.Patches, {a.FullName}");
            prefixes = ps?.GetField("Prefixes");
            postfixes = ps?.GetField("Postfixes");
            transpilers = ps?.GetField("Transpilers");

            Type p = Type.GetType($"Harmony.Patch, {a.FullName}");
            if (p == null)
            {
                return false;
            }

            Patch = p.GetField("patch");

            if (GetPatchInfoM == null)
                return false;

            Type dt = Type.GetType("System.Reflection.Emit.DynamicMethod+RTDynamicMethod");
            DynOwner = dt.GetField("m_owner", BindingFlags.Instance | BindingFlags.NonPublic);
            DynTOwner = typeof(DynamicMethod).GetField("m_typeOwner", BindingFlags.Instance | BindingFlags.NonPublic);
            return HarmonyActive = true;
        }

        public static string GetPatches(this MethodBase mb)
        {
            if (!HarmonyLoaded())
                return "";
            if (mb.DeclaringType != null)
                return "";
            int i = mb.Name.IndexOf("_Patch");
            if (i == -1)
                return "";
            string name = mb.Name.Substring(0, i);
            DynamicMethod o = DynOwner.GetValue(mb) as DynamicMethod;
            Type to = DynTOwner.GetValue(o) as Type;
            MethodInfo ogmeth = to.GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            object patches = GetPatchInfoM.Invoke(null, new object[] {ogmeth});
            string ret = "     " + ogmeth.GetFullName2() + Environment.NewLine;
            List<object> patchs = new();
            IEnumerable<object> pre = (prefixes.GetValue(patches) as IEnumerable).Cast<object>();
            IEnumerable<object> post = (postfixes.GetValue(patches) as IEnumerable).Cast<object>();
            IEnumerable<object> tra = (transpilers.GetValue(patches) as IEnumerable).Cast<object>();
            patchs.InsertRange(0, pre);
            patchs.InsertRange(patchs.Count, post);
            patchs.InsertRange(patchs.Count, tra);
            foreach (MethodBase p in patchs.Select(x => Patch.GetValue(x) as MethodBase))
            {
                ret += "     -" + p.GetFullName2() + Environment.NewLine;
            }

            return ret;
        }

        public static Vec2 stopPoint(Vec2 v, AmmoType at, float ang)
        {
            Bullet b = new Bullet(v.x, v.y, at, ang, null, false, -1, true);
            return b.end;
        }

        public static bool TryFirst<T>(this IEnumerable<T> collection, Func<T, bool> condition, out T first)
        {
            first = default;

            foreach (T item in collection)
            {
                if (!condition(item))
                    continue;

                first = item;
                return true;
            }

            return false;
        }

        public static void Deconstruct(this SizeF size, out float width, out float height)
        {
            width = size.Width;
            height = size.Height;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        public static string[] TrimSplit(this string str, params char[] characterToSplitAt)
        {
            string[] split = str.Split(characterToSplitAt);
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }

            return split;
        }

        private static readonly Regex _defaultWordLineBreakRegex = new(@"(.{0,30})(?:\s+|$)", RegexOptions.Compiled);

        public static string[] SplitByLength(this string str, int maxLength = 30, bool breakAtWordEnding = true)
        {
            if (breakAtWordEnding)
            {
                return (maxLength == 30
                        ? _defaultWordLineBreakRegex
                        : new Regex($@"(.{{0,{maxLength}}})(?:\s+|$)")).Matches(str)
                    .Cast<Match>()
                    .SelectMany(x => x.Groups[1].Value.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();
            }

            StringBuilder stringBuilder = new();
            int i = 0;
            for (int j = 0; j < str.Length; j++)
            {
                char currentCharacter = str[j];
                if (currentCharacter == '\n')
                    i = -1;

                stringBuilder.Append(currentCharacter);
                if (i++ != maxLength)
                    continue;
                stringBuilder.Append('\n');
                i = 0;
            }

            return stringBuilder.ToString().Split('\n');
        }

        public static string GetFullName(this MemberInfo mi) => $"{mi.DeclaringType}:{mi.Name}";

        static PropertyInfo InternalFullName =
            typeof(MethodBase).GetProperty("FullName", BindingFlags.NonPublic | BindingFlags.Instance);

        public static string GetFullName2(this MethodBase mi)
        {
            try
            {
                return (string)InternalFullName.GetValue(mi);
            }
            catch
            {
                return mi.Name;
            }
        }

        public static bool InheritsFrom(this Type derivedType, Type baseType)
        {
            return baseType.IsAssignableFrom(derivedType);
        }

        /// ChatGPT generated this beauty :D
        public static bool IsGenericallyApplicableTo(this Type derivedType, Type baseType)
        {
            return derivedType.IsGenericType && baseType.IsAssignableFrom(derivedType.GetGenericTypeDefinition());
        }

        public static bool CaselessEquals(this string str, string str2) =>
            string.Equals(str, str2, StringComparison.CurrentCultureIgnoreCase);

        public static List<T> WhereExcess<T>(
            this IEnumerable<T> collection,
            Predicate<T> comparer,
            out List<T> excess)
        {
            excess = new List<T>();
            List<T> wanted = new();

            foreach (T item in collection)
            {
                if (comparer(item))
                {
                    wanted.Add(item);
                }
                else
                {
                    excess.Add(item);
                }
            }

            return wanted;
        }

        public static string ToReadableString(this IEnumerable<object> collection, int indentationLevel = 0,
            bool doIndent = true)
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

        public static void DrawCenteredOutlinedString(string text, Vec2 position, Color color, Color outline,
            Depth depth = default, InputProfile pro = null, float scale = 1f)
        {
            Graphics.DrawStringOutline(text,
                new Vec2(position.x - ((Graphics.GetStringWidth(text) / 2) * scale), position.y), color, outline, depth,
                pro, scale);
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
                throw new ArgumentOutOfRangeException(nameof(propName),
                    $"Field {propName} was not found in Type {obj.GetType().FullName}");
            return (T)fi.GetValue(obj);
        }

        public static float Distance(this Vec2 pos1, Vec2 pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
        }

        public static Profile GetProfSafe(string playerName)
        {
            switch (playerName)
            {
                case "p1":
                case "player1":
                    return Profiles.DefaultPlayer1;

                case "p2":
                case "player2":
                    return Profiles.DefaultPlayer2;

                case "p3":
                case "player3":
                    return Profiles.DefaultPlayer3;

                case "p4":
                case "player4":
                    return Profiles.DefaultPlayer4;

                case "p5":
                case "player5":
                    return Profiles.DefaultPlayer5;

                case "p6":
                case "player6":
                    return Profiles.DefaultPlayer6;

                case "p7":
                case "player7":
                    return Profiles.DefaultPlayer7;

                case "p8":
                case "player8":
                    return Profiles.DefaultPlayer8;

                case "o1":
                case "observer1":
                    return Profiles.alllist[8];

                case "o2":
                case "observer2":
                    return Profiles.alllist[9];

                case "o3":
                case "observer3":
                    return Profiles.alllist[10];

                case "o4":
                case "observer4":
                    return Profiles.alllist[11];

                case "white":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 0);

                case "gray":
                case "grey":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 1);

                case "yellow":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 2);

                case "orange":
                case "brown":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 3);

                case "green":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 4);

                case "pink":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 5);

                case "blue":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 6);

                case "purple":
                    return Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 7);

                case "me":
                    return GetMe();

                case "nearest":
                    return Profiles.activeNonSpectators
                        .OrderBy(x => Vec2.Distance(x.duck.position, GetMe().duck.position))
                        .FirstOrDefault(x => x != GetMe());

                default:
                    return Profiles.active.TryFirst(
                        x => x.name.CleanFormatting().CaselessEquals(playerName.CleanFormatting()), out Profile prof)
                        ? prof
                        : null;
            }
        }

        public static Profile GetProf(string playerName) =>
            GetProfSafe(playerName) ?? throw new($"No profile with name {playerName} found");

        public enum PlayerName
        {
            Player1,
            Player2,
            Player3,
            Player4,
            Player5,
            Player6,
            Player7,
            Player8,
            Observer1,
            Observer2,
            Observer3,
            Observer4,
            Me,
        }

        public static Profile GetProf(PlayerName playerName)
        {
            if (PlayerName.Me == playerName)
            {
                return GetMe();
            }

            return Profiles.alllist[(int)playerName];
        }

        public static Profile GetMe()
        {
            if (Level.current != null)
            {
                return Network.isActive ? DuckNetwork.localProfile : GetProf(PlayerName.Player1);
            }

            return null;
        }

        public static T ChooseRandom<T>(this IEnumerable<T> collection)
        {
            int length = collection.Count();
            int randomIndex = Rando.Int(length - 1);

            using IEnumerator<T> enumerator = collection.GetEnumerator();

            for (int i = 0; i < randomIndex + 1; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }

        public static T[] ChooseRandom<T>(this IEnumerable<T> collection, int count, bool unique = false)
        {
            int length = collection.Count();

            if (count > length && unique)
                throw new IndexOutOfRangeException("Not enough unique items in collection");

            T[] randomItems = new T[count];
            List<T> validCollection = collection.ToList();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Rando.Int(validCollection.Count - 1);
                randomItems[i] = validCollection[randomIndex];

                if (unique)
                    validCollection.RemoveAt(randomIndex);
            }

            return randomItems;
        }

        public enum CleanMethod { Color, Mojis, Both }

        // :D
        public static readonly Regex ColorFormattingRegex = new(
            @"\|(?:(?:(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5])))|(?:AQUA)|(?:RED)|(?:WHITE)|(?:BLACK)|(?:DARKNESS)|(?:BLUE)|(?:DGBLUE)|(?:DGRED)|(?:DGREDDD)|(?:DGGREEN)|(?:DGGREENN)|(?:DGYELLOW)|(?:DGYELLO)|(?:DGORANGE)|(?:ORANGE)|(?:MENUORANGE)|(?:YELLOW)|(?:GREEN)|(?:LIME)|(?:TIMELIME)|(?:GRAY)|(?:LIGHTGRAY)|(?:CREDITSGRAY)|(?:BLUEGRAY)|(?:PINK)|(?:PURPLE)|(?:DGPURPLE)|(?:CBRONZE)|(?:CSILVER)|(?:CGOLD)|(?:CPLATINUM)|(?:CDEV)|(?:DUCKCOLOR1)|(?:DUCKCOLOR2)|(?:DUCKCOLOR3)|(?:DUCKCOLOR4)|(?:RBOW_1)|(?:RBOW_2)|(?:RBOW_3)|(?:RBOW_4)|(?:RBOW_5)|(?:RBOW_6)|(?:RBOW_7))\|");

        public static MatchCollection GetStringColorFormatting(string text) =>
            ColorFormattingRegex.Matches(text);

        public static string CleanStringFormatting(string str, CleanMethod cleanMethod = CleanMethod.Both) =>
            cleanMethod switch
            {
                CleanMethod.Color => ColorFormattingRegex.Replace(str, ""),
                CleanMethod.Mojis => Regex.Replace(str, @"(@|:).*?\1", ""),
                CleanMethod.Both => CleanStringFormatting(CleanStringFormatting(str, CleanMethod.Color),
                    CleanMethod.Mojis),
                _ => throw new NotImplementedException()
            };

        public static string CleanFormatting(this string str, CleanMethod cleanMethod = CleanMethod.Both) =>
            CleanStringFormatting(str, cleanMethod);

        public static SizeF GetStringSize(string text, float fontSize = 1f) => new(0, 0)
        {
            Width = Graphics.GetStringWidth(text, false, fontSize), Height = Graphics.GetStringHeight(text) * fontSize
        };
        
        public static SizeF GetFancyStringSize(string text, float fontSize = 1f) => new()
        {
            Width = Graphics.GetFancyStringWidth(text, false, fontSize), Height = Graphics.GetStringHeight(text) * fontSize
        };

        public static bool MultiPlayerTeamsExist()
        {
            HashSet<int> hashedTeams = new();

            foreach (Profile prof in Profiles.activeNonSpectators)
            {
                int hashCode = prof.team.GetHashCode();
                if (!hashedTeams.Add(hashCode)) // hashsets return false if adding duplicates
                    return true;
            }

            return false;
        }
    }
}