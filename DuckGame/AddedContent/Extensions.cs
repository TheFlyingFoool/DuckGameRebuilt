using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        
        public static Profile? GetProfSafe(string playerName) => playerName switch
        {
            "p1" or "player1" => Profiles.DefaultPlayer1,
            "p2" or "player2" => Profiles.DefaultPlayer2,
            "p3" or "player3" => Profiles.DefaultPlayer3,
            "p4" or "player4" => Profiles.DefaultPlayer4,
            "p5" or "player5" => Profiles.DefaultPlayer5,
            "p6" or "player6" => Profiles.DefaultPlayer6,
            "p7" or "player7" => Profiles.DefaultPlayer7,
            "p8" or "player8" => Profiles.DefaultPlayer8,
            "o1" or "observer1" => Profiles.all.ElementAt(8),
            "o2" or "observer2" => Profiles.all.ElementAt(9),
            "o3" or "observer3" => Profiles.all.ElementAt(10),
            "o4" or "observer4" => Profiles.all.ElementAt(11),
            "white" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 0),
            "gray" or "grey" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 1),
            "yellow" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 2),
            "orange" or "brown" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 3),
            "green" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 4),
            "pink" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 5),
            "blue" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 6),
            "purple" => Profiles.activeNonSpectators.FirstOrDefault(x => x.persona.index == 7),
            "me" => GetMe(),
            "nearest" => Profiles.activeNonSpectators
                .OrderBy(x => Vec2.Distance(x.duck.position, GetMe().duck.position))
                .FirstOrDefault(x => x != GetMe()),
            _ => Profiles.active.TryFirst(
                x => x.name.CleanFormatting().CaselessEquals(playerName.CleanFormatting()),
                out var prof)
                ? prof
                : null,
        };
        
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
            return playerName switch
            {
                PlayerName.Player1 => Profiles.DefaultPlayer1,
                PlayerName.Player2 => Profiles.DefaultPlayer2,
                PlayerName.Player3 => Profiles.DefaultPlayer3,
                PlayerName.Player4 => Profiles.DefaultPlayer4,
                PlayerName.Player5 => Profiles.DefaultPlayer5,
                PlayerName.Player6 => Profiles.DefaultPlayer6,
                PlayerName.Player7 => Profiles.DefaultPlayer7,
                PlayerName.Player8 => Profiles.DefaultPlayer8,
                PlayerName.Observer1 => Profiles.all.ToList()[08],
                PlayerName.Observer2 => Profiles.all.ToList()[09],
                PlayerName.Observer3 => Profiles.all.ToList()[10],
                PlayerName.Observer4 => Profiles.all.ToList()[11],
                PlayerName.Me => GetMe(),
                _ => null,
            };
        }
        
        public static Profile GetMe()
        {
            if (Level.current != null)
            {
                return Network.isActive 
                    ? DuckNetwork.localProfile
                    : GetProf(PlayerName.Player1);
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
