using System;
using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class CMD
    {
        public int commandQueueWait;
        public Func<bool> commandQueueWaitFunction;
        public bool cancrash;
        public bool hidden;
        public bool cheat;
        public string keyword;
        public List<string> aliases;
        public Argument[] arguments;
        public Action<CMD> action;
        public Action alternateAction;
        public CMD subcommand;
        public CMD parent;
        public int priority;
        public string description = "";
        public string logMessage;
        public bool noDsh = false;

        public object[] GetParameterValues(string[] stringValues)
        {
            return arguments.Select((x, i) => x.Parse(stringValues[i])).ToArray();
        }

        public bool HasArg(string pName)
        {
            Argument obj = arguments.FirstOrDefault(x => x.name == pName);
            return obj is not null;
        }

        public T Arg<T>(string pName)
        {
            Argument obj = arguments.FirstOrDefault(x => x.name == pName);
            if (obj is null)
                return default;

            return (T) (obj.optional && obj.value is null ? obj.defaultValue : obj.value);
        }

        public string fullCommandName => parent != null ? $"{parent.fullCommandName} {keyword}" : keyword;

        public CMD(string pKeyword, Argument[] pArguments, Action<CMD> pAction)
        {
            keyword = pKeyword.ToLowerInvariant();
            arguments = pArguments;
            action = pAction;
        }

        public CMD(string pKeyword, Action<CMD> pAction)
        {
            keyword = pKeyword.ToLowerInvariant();
            action = pAction;
        }

        public CMD(string pKeyword, Action pAction)
        {
            keyword = pKeyword.ToLowerInvariant();
            alternateAction = pAction;
        }

        public CMD(string pKeyword, CMD pSubCommand)
        {
            keyword = pKeyword.ToLowerInvariant();
            subcommand = pSubCommand;
            pSubCommand.parent = this;
            priority = -1;
        }

        public string info
        {
            get
            {
                string str = $"{keyword}(";
                if (arguments != null)
                {
                    bool flag = true;
                    foreach (Argument obj in arguments)
                    {
                        if (!flag)
                            str += ", ";
                        flag = false;
                        str = $"{str}{obj.type.Name} {obj.name}";
                    }
                }
                string info = $"{str})";
                if (description != "")
                    info = $"{info}\n|DGBLUE|{description}";
                return info;
            }
        }

        public bool Run(string pArguments)
        {
            logMessage = null;
            string[] source = pArguments.Split(' ');
            if (subcommand != null)
                return Error($"|DGRED|Command ({keyword}) requires a sub command.");
            if (source.Any() && source[0] == "?")
                return Help(info);
            if (source.Any() && source[0].Trim().Length > 0 && arguments == null)
                return Error($"|DGRED|Command ({keyword}) takes no arguments.");
            if (arguments != null)
            {
                int index1 = 0;
                int num = -1;
                foreach (Argument obj in arguments)
                {
                    if (obj.optional)
                    {
                        if (num < 0)
                            num = index1;
                    }
                    else if (num >= 0)
                        return Error("|DGRED|Command implementation error: 'optional' arguments must appear last.");
                    if (source.Length > index1)
                    {
                        if (!string.IsNullOrWhiteSpace(source[index1]))
                        {
                            try
                            {
                                if (arguments[index1].takesMultispaceString)
                                {
                                    string str = "";
                                    for (int index2 = index1; index2 < source.Length; ++index2)
                                        str = $"{str}{source[index2]} ";
                                    obj.value = arguments[index1].Parse(str.Trim());
                                    index1 = source.Length;
                                }
                                else
                                    obj.value = arguments[index1].Parse(source[index1]);
                                if (obj.value == null)
                                    return Error($"|DGRED|{obj.GetParseFailedMessage()} |GRAY|({obj.name})");
                                goto label_26;
                            }
                            catch (Exception ex)
                            {
                                return Error($"|DGRED|Error parsing argument ({obj.name}): {ex.Message}");
                            }
                        }
                    }
                    if (!obj.optional)
                        return Error($"|DGRED|Missing argument ({obj.name})");
                    label_26:
                    ++index1;
                }
            }
            if (cancrash)
            {
                if (action != null)
                    action(this);
                else if (alternateAction != null)
                    alternateAction();
            }
            else
            {
                try
                {
                    if (action != null)
                        action(this);
                    else if (alternateAction != null)
                        alternateAction();
                }
                catch (Exception ex)
                {
                    FinishExecution();
                    return Error($"|DGRED|Error: {ex.Message}");
                }
            }

            FinishExecution();
            return true;
        }

        private void FinishExecution()
        {
            if (arguments == null)
                return;
            foreach (Argument obj in arguments)
                obj.value = null;
        }

        protected bool Error(string pError = null)
        {
            logMessage = $"|DGRED|{pError}";
            return false;
        }

        protected bool Help(string pMessage = null)
        {
            logMessage = $"|DGBLUE|{pMessage}";
            return true;
        }

        protected bool Message(string pMessage = null)
        {
            logMessage = pMessage;
            return true;
        }

        public abstract class Argument
        {
            public Type type;
            public object value;
            public string name;
            public bool optional;
            public bool takesMultispaceString;
            protected string _parseFailMessage = "Argument was in wrong format. ";
            public object defaultValue;

            public Argument(string pName, bool pOptional)
            {
                name = pName;
                optional = pOptional;
            }

            public virtual string GetParseFailedMessage() => _parseFailMessage;

            public abstract object Parse(string pValue);

            protected object Error(string pError = null)
            {
                if (pError != null)
                    _parseFailMessage = pError;
                return null;
            }
        }

        public class String : Argument
        {
            public String(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(string);
            }

            public override object Parse(string pValue) => pValue;
        }

        public class Duck : Argument
        {
            public Duck(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(Duck);
            }

            public override object Parse(string pValue)
            {
                return Extensions.GetProf(pValue).duck;
            }
        }

        public class Profile : Argument
        {
            public Profile(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(Profile);
            }

            public override object Parse(string pValue)
            {
                return Extensions.GetProf(pValue);
            }
        }

        public class Integer : Argument
        {
            public Integer(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(int);
            }

            public override object Parse(string pValue)
            {
                return ChangePlus.ToInt32(pValue);
            }
        }

        public class Boolean : Argument
        {
            public Boolean(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(bool);
            }

            public override object Parse(string pValue)
            {
                return ChangePlus.ToBoolean(pValue);
            }
        }

        public class Vec2 : Argument
        {
            public Vec2(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(Vec2);
            }

            public override object Parse(string pValue)
            {
                return ChangePlus.ToVec2(pValue);
            }
        }

        public class Float : Argument
        {
            public Float(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(float);
            }

            public override object Parse(string pValue)
            {
                return ChangePlus.ToSingle(pValue);
            }
        }

        public class Array : Argument
        {
            private readonly Type _arrayType;

            public Array(string pName, Type arrayType, bool pOptional) : base(pName, pOptional)
            {
                _arrayType = arrayType;
            }

            public override object Parse(string pValue)
            {
                ITypeInterpreter arrayInterpreter = Commands.console.Shell.TypeInterpreterModulesMap[typeof(System.Array)];
                ValueOrException<object> parseResult = arrayInterpreter.ParseString(pValue, _arrayType, new(Commands.console.Shell, null));

                if (parseResult.Failed)
                    return Error(parseResult.Error.Message);

                return parseResult.Value;
            }
        }

        public class Enum : Argument
        {
            public Enum(string pName, Type pEnumType, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = pEnumType;
            }

            public override object Parse(string pValue) =>
                System.Enum.Parse(type, pValue, true)
                ?? Error("Argument value must be an floating point number.");
        }

        public class Font : Argument
        {
            private Func<int> defaultFontSize;

            public Font(string pName, Func<int> pDefaultFontSize = null, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(string);
                takesMultispaceString = true;
                defaultFontSize = pDefaultFontSize;
            }

            public override object Parse(string pValue)
            {
                if (pValue is "clear" or "default" or "none")
                    return "";
                bool flag1 = false;
                bool flag2 = false;
                if (pValue.EndsWith(" bold"))
                {
                    flag1 = true;
                    pValue = pValue.Substring(0, pValue.Length - 5);
                }
                if (pValue.EndsWith(" italic"))
                {
                    flag2 = true;
                    pValue = pValue.Substring(0, pValue.Length - 7);
                }
                if (pValue == "comic sans")
                    pValue = "comic sans ms";
                string name = RasterFont.GetName(pValue);
                if (name == null)
                    return Error($"Font ({pValue}) was not found.");
                if (flag1)
                    name += "@BOLD@";
                if (flag2)
                    name += "@ITALIC@";
                return name;
            }
        }

        public class Layer : Argument
        {
            public Layer(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(Layer);
            }

            public override object Parse(string pValue) => (object)DuckGame.Layer.core._layers.FirstOrDefault(x => x.name.ToLower() == pValue) ?? Error(
                $"Layer named ({pValue}) was not found.");
        }

        public class Level : Argument
        {
            public Level(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(Level);
                takesMultispaceString = true;
            }
            
            // this is so things outside can get this list too ~FB
            public static Dictionary<string, Func<DuckGame.Level>> SpecialLevelLookup = new()
            {
                {"fb", () => new SimRenderer()},
                // {"fbtest", () => new TestLev()}, // apparently someone booted up dg and got this level somehow
                {"hatpreview", () => new HatPreviewLevel()},
                {"ff", () => new FeatherFashion()},
                {"cord", () => new RecorderationSelector()},
                {"dev", () => new DevTestLev()},
                {"title", () => new TitleScreen()},
                {"rockintro", () => new RockIntro(new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel)))},
                {"rockthrow", () => new RockScoreboard()},
                {"finishscreen", () => new RockScoreboard(mode: ScoreBoardMode.ShowWinner, afterHighlights: true)},
                {"highlights", () => new HighlightLevel()},
                {"next", () => new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel))},
                {"editor", () => Main.editor},
                {"arcade", () => new ArcadeLevel(Content.GetLevelID("arcade"))}
            };

            public override object Parse(string pValue)
            {
                if (pValue == "pyramid" || pValue.StartsWith("pyramid") && pValue.Contains("|"))
                {
                    int seedVal = 0;

                    if (pValue.Contains("|")
                        && int.TryParse(pValue.Split('|')[1], out seedVal)) ;

                    return new GameLevel("RANDOM", seedVal);
                }

                if (SpecialLevelLookup.TryGetValue(pValue, out Func<DuckGame.Level> specialLevelInitializer))
                    return specialLevelInitializer.Invoke();

                if (!pValue.EndsWith(".lev"))
                    pValue += ".lev";
                LevelData levelData1 = DuckFile.LoadLevel($"{Content.path}/levels/{("deathmatch/" + pValue)}");
                if (levelData1 != null)
                    return new GameLevel(levelData1.metaData.guid);
                if (DuckFile.LoadLevel(DuckFile.levelDirectory + pValue) != null)
                    return new GameLevel(pValue);
                LevelData levelData2 = DuckFile.LoadLevel($"{Content.path}/levels/{pValue}");
                if (levelData2 != null)
                    return new GameLevel(levelData2.metaData.guid);
                foreach (Mod accessibleMod in ModLoader.accessibleMods)
                {
                    if (accessibleMod.configuration.content == null)
                        continue;

                    foreach (string level in accessibleMod.configuration.content.levels
                                 .Where(level => level.ToLowerInvariant().EndsWith(pValue)))
                    {
                        return new GameLevel(level);
                    }
                }
                return Error($"Level ({pValue}) was not found.");
            }
        }

        public class Thing<T> : String where T : Thing
        {
            public Thing(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(T);
            }

            public override object Parse(string pValue)
            {
                pValue = pValue.ToLowerInvariant();

                if (pValue == "gun")
                    return ItemBoxRandom.GetRandomItem();

                if (type == typeof(TeamHat))
                {
                    Team t = Teams.all.FirstOrDefault(x => x.name.ToLower() == pValue);
                    return t != null ? new TeamHat(0f, 0f, t) : Error(
                        $"Argument ({pValue}) should be the name of a team");
                }

                foreach (Type thingType in Editor.ThingTypes
                             .Where(thingType => thingType.Name.ToLowerInvariant() == pValue))
                {
                    if (!Editor.HasConstructorParameter(thingType))
                        return Error($"{thingType.Name} can not be spawned this way.");
                    return !typeof(T).IsAssignableFrom(thingType) ? Error(
                        $"Wrong object type (requires {typeof(T).Name}).") : Editor.CreateThing(thingType) as T;
                }

                return Error($"{typeof(T).Name} of type ({pValue}) was not found.");
            }
        }

        public static Argument GetArgument(Type type, string name, bool optional, bool stringTakesMultispace = false)
        {
            Argument arg;
            
            if (type == typeof(Int32))
                arg = new Integer(name, optional);
            else if (type == typeof(Single))
                arg = new Float(name, optional);
            else if (type == typeof(System.Boolean))
                arg = new Boolean(name, optional);
            else if (type == typeof(DuckGame.Vec2))
                arg = new Vec2(name, optional);
            else if (type == typeof(System.String))
                arg = new String(name, optional) { takesMultispaceString = stringTakesMultispace };
            else if (type == typeof(DuckGame.Duck))
                arg = new Duck(name, optional);
            else if (type == typeof(DuckGame.Profile))
                arg = new Profile(name, optional);
            else if (typeof(Thing).IsAssignableFrom(type))
                arg = new Thing<Thing>(name, optional);
            else if (typeof(DuckGame.Level).IsAssignableFrom(type))
                arg = new Level(name, optional);
            else if (typeof(DuckGame.Layer).IsAssignableFrom(type))
                arg = new Layer(name, optional);
            else if (typeof(System.Enum).IsAssignableFrom(type))
                arg = new Enum(name, type, optional);
            else if (typeof(System.Array).IsAssignableFrom(type))
                arg = new Array(name, type, optional);
            else
                throw new Exception($"Parameter type of [{type.FullName}] is not supported");

            return arg;
        }
    }
}
