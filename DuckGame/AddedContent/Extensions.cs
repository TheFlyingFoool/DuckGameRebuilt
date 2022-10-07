using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static DuckGame.CMD;

namespace DuckGame
{
    public static class Extensions
    {
        public static void TryUse<T1, T2>(this Dictionary<T1, T2> dic, T1 requestedKey, T2 defaultValue, Action<T2> action)
        {
            if (!dic.ContainsKey(requestedKey))
                dic.Add(requestedKey, defaultValue);

            action(dic[requestedKey]);
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
                    .SelectMany(x => x.Groups[1].Value.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
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

        public static bool InheritsFrom(this Type t1, Type t2) => t2.IsAssignableFrom(t1);
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
                    return Profiles.all[8];
                case "o2":
                case "observer2":
                    return Profiles.all[9];
                case "o3":
                case "observer3":
                    return Profiles.all[10];
                case "o4":
                case "observer4":
                    return Profiles.all[11];
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
                    return Profiles.activeNonSpectators.OrderBy(x => Vec2.Distance(x.duck.position, GetMe().duck.position)).FirstOrDefault(x => x != GetMe());
                default:
                    return Profiles.active.TryFirst(x => x.name.CleanFormatting().CaselessEquals(playerName.CleanFormatting()), out Profile prof) ? prof : null;
            }
        }

        public static Profile GetProf(string playerName) => GetProfSafe(playerName) ?? throw new($"No profile with name {playerName} found");

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
            return Profiles.all[(int)playerName];
        }

        public static Profile GetMe()
        {
            if (Level.current != null)
            {
                return Network.isActive ? DuckNetwork.localProfile : GetProf(PlayerName.Player1);
            }

            return null;
        }
        public static string[] ProfileKeywords = {
            "p1",
            "p2",
            "p3",
            "p4",
            "p5",
            "p6",
            "p7",
            "p8",
            "o1",
            "o2",
            "o3",
            "o4",
            "player1",
            "player2",
            "player3",
            "player4",
            "player5",
            "player6",
            "player7",
            "player8",
            "observer1",
            "observer2",
            "observer3",
            "observer4",
            "white",
            "gray",
            "grey",
            "yellow",
            "orange",
            "brown",
            "green",
            "pink",
            "blue",
            "purple",
            "me",
            "nearest"
        };

        public static string[] BooleanKeywords = {
            "true",
            "false",
            "t",
            "f",
            "1",
            "0",
            "yes",
            "no",
            "y",
            "n",
        };

        public enum CleanMethod { Color, Mojis, Both }

        // :D
        public static readonly Regex ColorFormattingRegex = new(@"\|(?:(?:(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5])))|(?:AQUA)|(?:RED)|(?:WHITE)|(?:BLACK)|(?:DARKNESS)|(?:BLUE)|(?:DGBLUE)|(?:DGRED)|(?:DGREDDD)|(?:DGGREEN)|(?:DGGREENN)|(?:DGYELLOW)|(?:DGYELLO)|(?:DGORANGE)|(?:ORANGE)|(?:MENUORANGE)|(?:YELLOW)|(?:GREEN)|(?:LIME)|(?:TIMELIME)|(?:GRAY)|(?:LIGHTGRAY)|(?:CREDITSGRAY)|(?:BLUEGRAY)|(?:PINK)|(?:PURPLE)|(?:DGPURPLE)|(?:CBRONZE)|(?:CSILVER)|(?:CGOLD)|(?:CPLATINUM)|(?:CDEV)|(?:DUCKCOLOR1)|(?:DUCKCOLOR2)|(?:DUCKCOLOR3)|(?:DUCKCOLOR4)|(?:RBOW_1)|(?:RBOW_2)|(?:RBOW_3)|(?:RBOW_4)|(?:RBOW_5)|(?:RBOW_6)|(?:RBOW_7))\|");

        public static MatchCollection GetStringColorFormatting(string text) =>
            ColorFormattingRegex.Matches(text);

        public static string CleanStringFormatting(string str, CleanMethod cleanMethod = CleanMethod.Both) => cleanMethod switch
        {
            CleanMethod.Color => ColorFormattingRegex.Replace(str, ""),
            CleanMethod.Mojis => Regex.Replace(str, @"(@|:).*?\1", ""),
            CleanMethod.Both => CleanStringFormatting(CleanStringFormatting(str, CleanMethod.Color), CleanMethod.Mojis),
            _ => throw new NotImplementedException()
        };

        public static string CleanFormatting(this string String, CleanMethod cleanMethod = CleanMethod.Both) => CleanStringFormatting(String, cleanMethod);
    }
}
