// Decompiled with JetBrains decompiler
// Type: DuckGame.Script
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{
    public class Script
    {
        private static Profile _activeProfile;
        private static int _currentPosition;
        private static List<List<Profile>> _positions;
        private static PropertyInfo _activeProfileProperty;
        private static DuckNews _activeNewsStory;
        private static Dictionary<string, MethodInfo> _availableFunctions = new Dictionary<string, MethodInfo>();
        private static List<string> _highlightRatings = new List<string>()
    {
      "Ughhhh...",
      "I fell asleep",
      "Really Boring",
      "Kinda Boring",
      "Not Terribly Exciting",
      "About Average",
      "Mildly Entertaining",
      "Pretty Exciting",
      "Awesome",
      "Super Awesome",
      "Heart Stopping Action",
      "Insane Non-stop Insanity"
    };

        public static Profile activeProfile
        {
            get => _activeProfile;
            set => _activeProfile = value;
        }

        public static int currentPosition
        {
            get => _currentPosition;
            set => _currentPosition = value;
        }

        public static List<List<Profile>> positions
        {
            get => _positions;
            set => _positions = value;
        }

        public static DuckNews activeNewsStory
        {
            get => _activeNewsStory;
            set => _activeNewsStory = value;
        }

        public static MethodInfo GetMethod(string name)
        {
            MethodInfo method;
            _availableFunctions.TryGetValue(name, out method);
            return method;
        }

        public static object CallMethod(string name, object value)
        {
            MethodInfo method = GetMethod(name);
            if (!(method != null))
                return null;
            MethodInfo methodInfo = method;
            object[] parameters;
            if (value == null)
                parameters = null;
            else
                parameters = new object[1] { value };
            return methodInfo.Invoke(null, parameters);
        }

        public static void Initialize()
        {
            _activeProfile = Profiles.DefaultPlayer1;
            _activeProfileProperty = typeof(Script).GetProperty("activeProfile", BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in typeof(Script).GetMethods(BindingFlags.Static | BindingFlags.Public))
                _availableFunctions[method.Name] = method;
        }

        public static int profileScore() => activeProfile.endOfRoundStats.GetProfileScore();

        public static int negProfileScore() => -activeProfile.endOfRoundStats.GetProfileScore();

        public static ScriptObject stat(string statName)
        {
            PropertyInfo property = typeof(ProfileStats).GetProperty(statName);
            if (!(property != null))
                return null;
            return new ScriptObject()
            {
                obj = activeProfile.endOfRoundStats,
                objectProperty = property
            };
        }

        public static ScriptObject statNegative(string statName)
        {
            PropertyInfo property = typeof(ProfileStats).GetProperty(statName);
            if (!(property != null))
                return null;
            return new ScriptObject()
            {
                obj = activeProfile.endOfRoundStats,
                objectProperty = property,
                negative = true
            };
        }

        public static string coolnessString() => activeProfile.endOfRoundStats.GetCoolnessString();

        public static ScriptObject prevStat(string statName)
        {
            PropertyInfo property = typeof(ProfileStats).GetProperty(statName);
            if (!(property != null))
                return null;
            return new ScriptObject()
            {
                obj = activeProfile.prevStats,
                objectProperty = property
            };
        }

        public static string previousTitleOwner(string name)
        {
            DuckTitle title = DuckTitle.GetTitle(name);
            return title != null ? title.previousOwner : "";
        }

        public static float sin(float val) => (float)Math.Sin(val);

        public static float cos(float val) => (float)Math.Cos(val);

        public static float round(float val) => (float)Math.Round(val);

        public static float toFloat(int val) => val;

        public static int place() => currentPosition;

        public static float random() => Rando.Float(1f);

        public static string winner() => Results.winner.name;

        public static string RatingsString(int wow)
        {
            if (wow > Global.data.highestNewsCast)
                Global.data.highestNewsCast = wow;
            int num1 = 60;
            int num2 = 250 + (int)(Global.data.highestNewsCast * Rando.Float(0.1f, 0.25f));
            if (wow < num1)
                wow = num1;
            if (wow > num2)
                wow = num2;
            wow -= num1;
            float num3 = wow / (float)(num2 - num1);
            return _highlightRatings[(int)Math.Round(num3 * (_highlightRatings.Count - 1))];
        }

        public static string highlightRating()
        {
            float num = 0f;
            List<Recording> highlights = Highlights.GetHighlights();
            foreach (Recording recording in highlights)
                num += recording.highlightScore;
            return RatingsString((int)(num / highlights.Count * 1.5f));
        }

        public static float floatVALUE()
        {
            if (_activeNewsStory != null && _activeNewsStory.valueCalculation != null)
            {
                object result = _activeNewsStory.valueCalculation.result;
                if (result != null)
                    return Change.ToSingle(result);
            }
            return 0f;
        }

        public static float floatVALUE2()
        {
            if (_activeNewsStory != null && _activeNewsStory.valueCalculation != null)
            {
                object result = _activeNewsStory.valueCalculation2.result;
                if (result != null)
                    return Change.ToSingle(result);
            }
            return 0f;
        }

        public static int numInPlace(int p) => positions == null || p < 0 || p >= positions.Count ? 0 : positions[positions.Count - 1 - p].Count;

        public static bool skippedNewscast() => HighlightLevel.didSkip;

        public static bool hasPurchaseInfo() => Main.foundPurchaseInfo;

        public static bool doesNotHavePurchaseInfo() => !Main.foundPurchaseInfo;

        //public static bool isDemo() => Main.isDemo;

        // public static bool isNotDemo() => !Main.isDemo;

        public static float greatest(string val)
        {
            float num1 = -99999f;
            foreach (Profile profile in Profiles.active)
            {
                float num2 = -99999f;
                ScriptObject scriptObject = stat(val);
                if (scriptObject != null)
                    num2 = Change.ToSingle(scriptObject.objectProperty.GetValue(scriptObject.obj, null)) * (scriptObject.negative ? -1f : 1f);
                if (num2 > num1)
                    num1 = num2;
            }
            return num1;
        }

        public static bool hasGreatest(string val)
        {
            float num1 = -999999f;
            Profile profile1 = null;
            foreach (Profile profile2 in Profiles.active)
            {
                float num2 = -999999f;
                Profile activeProfile = Script.activeProfile;
                Script.activeProfile = profile2;
                if (_activeNewsStory != null && val == "VALUE")
                {
                    object result = _activeNewsStory.valueCalculation.result;
                    if (result != null)
                        num2 = Change.ToSingle(result);
                }
                else if (val != "VALUE")
                {
                    ScriptObject scriptObject = stat(val);
                    if (scriptObject != null)
                        num2 = Change.ToSingle(scriptObject.objectProperty.GetValue(scriptObject.obj, null)) * (scriptObject.negative ? -1f : 1f);
                }
                Script.activeProfile = activeProfile;
                if (num2 > num1)
                {
                    num1 = num2;
                    profile1 = profile2;
                }
            }
            return profile1 == activeProfile;
        }
    }
}
