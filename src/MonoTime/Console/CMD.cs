// Decompiled with JetBrains decompiler
// Type: DuckGame.CMD
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class CMD
    {
        public int commandQueueWait;
        public Func<bool> commandQueueWaitFunction;
        public bool hidden;
        public bool cheat;
        public string keyword;
        public List<string> aliases;
        public CMD.Argument[] arguments;
        public Action<CMD> action;
        public Action alternateAction;
        public CMD subcommand;
        public CMD parent;
        public int priority;
        public string description = "";
        public string logMessage;

        public bool HasArg<T>(string pName)
        {
            CMD.Argument obj = arguments.FirstOrDefault<CMD.Argument>(x => x.name == pName);
            return obj != null && obj.value != null;
        }

        public T Arg<T>(string pName)
        {
            CMD.Argument obj = arguments.FirstOrDefault<CMD.Argument>(x => x.name == pName);
            return obj != null && obj.value != null ? (T)obj.value : default(T);
        }

        public string fullCommandName => parent != null ? parent.fullCommandName + " " + keyword : keyword;

        public CMD(string pKeyword, CMD.Argument[] pArguments, Action<CMD> pAction)
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
                string str = keyword + "(";
                if (arguments != null)
                {
                    bool flag = true;
                    foreach (CMD.Argument obj in arguments)
                    {
                        if (!flag)
                            str += ", ";
                        flag = false;
                        str = str + obj.type.Name + " " + obj.name;
                    }
                }
                string info = str + ")";
                if (description != "")
                    info = info + "\n|DGBLUE|" + description;
                return info;
            }
        }

        public bool Run(string pArguments)
        {
            logMessage = null;
            string[] source = pArguments.Split(' ');
            if (subcommand != null)
                return Error("|DGRED|Command (" + keyword + ") requires a sub command.");
            if (source.Count<string>() > 0 && source[0] == "?")
                return Help(info);
            if (source.Count<string>() > 0 && source[0].Trim().Length > 0 && arguments == null)
                return Error("|DGRED|Command (" + keyword + ") takes no arguments.");
            if (arguments != null)
            {
                int index1 = 0;
                int num = -1;
                foreach (CMD.Argument obj in arguments)
                {
                    if (obj.optional)
                    {
                        if (num < 0)
                            num = index1;
                    }
                    else if (num >= 0)
                        return Error("|DGRED|Command implementation error: 'optional' arguments must appear last.");
                    if (source.Count<string>() > index1)
                    {
                        if (!string.IsNullOrWhiteSpace(source[index1]))
                        {
                            try
                            {
                                if (arguments[index1].takesMultispaceString)
                                {
                                    string str = "";
                                    for (int index2 = index1; index2 < source.Length; ++index2)
                                        str = str + source[index2] + " ";
                                    obj.value = arguments[index1].Parse(str.Trim());
                                    index1 = source.Length;
                                }
                                else
                                    obj.value = arguments[index1].Parse(source[index1]);
                                if (obj.value == null)
                                    return Error("|DGRED|" + obj.GetParseFailedMessage() + " |GRAY|(" + obj.name + ")");
                                goto label_26;
                            }
                            catch (Exception ex)
                            {
                                return Error("|DGRED|Error parsing argument (" + obj.name + "): " + ex.Message);
                            }
                        }
                    }
                    if (!obj.optional)
                        return Error("|DGRED|Missing argument (" + obj.name + ")");
                    label_26:
                    ++index1;
                }
            }
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
                return Error("|DGRED|Error running command: " + ex.Message);
            }
            FinishExecution();
            return true;
        }

        private void FinishExecution()
        {
            if (arguments == null)
                return;
            foreach (CMD.Argument obj in arguments)
                obj.value = null;
        }

        protected bool Error(string pError = null)
        {
            logMessage = "|DGRED|" + pError;
            return false;
        }

        protected bool Help(string pMessage = null)
        {
            logMessage = "|DGBLUE|" + pMessage;
            return true;
        }

        protected bool Message(string pMessage = null)
        {
            logMessage = pMessage;
            return true;
        }

        public abstract class Argument
        {
            public System.Type type;
            public object value;
            public string name;
            public bool optional;
            public bool takesMultispaceString;
            protected string _parseFailMessage = "Argument was in wrong format. ";

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

        public class String : CMD.Argument
        {
            public String(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(string);
            }

            public override object Parse(string pValue) => pValue;
        }

        public class Integer : CMD.Argument
        {
            public Integer(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(int);
            }

            public override object Parse(string pValue)
            {
                try
                {
                    return Convert.ToInt32(pValue);
                }
                catch (Exception)
                {
                }
                return Error("Argument value must be an integer.");
            }
        }

        public class Font : CMD.Argument
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
                if (pValue == "clear" || pValue == "default" || pValue == "none")
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
                    return Error("Font (" + pValue + ") was not found.");
                if (flag1)
                    name += "@BOLD@";
                if (flag2)
                    name += "@ITALIC@";
                return name;
            }
        }

        public class Layer : CMD.Argument
        {
            public Layer(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(CMD.Layer);
            }

            public override object Parse(string pValue) => (object)DuckGame.Layer.core._layers.FirstOrDefault<DuckGame.Layer>(x => x.name.ToLower() == pValue) ?? Error("Layer named (" + pValue + ") was not found.");
        }

        public class Level : CMD.Argument
        {
            public Level(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                type = typeof(CMD.Level);
                takesMultispaceString = true;
            }

            public override object Parse(string pValue)
            {
                if (pValue == "pyramid" || pValue.StartsWith("pyramid") && pValue.Contains("|"))
                {
                    int seedVal = 0;
                    if (pValue.Contains("|"))
                    {
                        try
                        {
                            seedVal = Convert.ToInt32(pValue.Split('|')[1]);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    return new GameLevel("RANDOM", seedVal);
                }
                if (pValue == "title")
                    return new TitleScreen();
                if (pValue == "rockintro")
                    return new RockIntro(new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel)));
                if (pValue == "rockthrow")
                    return new RockScoreboard();
                if (pValue == "finishscreen")
                    return new RockScoreboard(mode: ScoreBoardMode.ShowWinner, afterHighlights: true);
                if (pValue == "highlights")
                    return new HighlightLevel();
                if (pValue == "next")
                    return new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel));
                if (pValue == "editor")
                    return Main.editor;
                if (pValue == "arcade")
                    return new ArcadeLevel(Content.GetLevelID("arcade"));
                if (!pValue.EndsWith(".lev"))
                    pValue += ".lev";
                LevelData levelData1 = DuckFile.LoadLevel(Content.path + "/levels/" + ("deathmatch/" + pValue));
                if (levelData1 != null)
                    return new GameLevel(levelData1.metaData.guid);
                if (DuckFile.LoadLevel(DuckFile.levelDirectory + pValue) != null)
                    return new GameLevel(pValue);
                LevelData levelData2 = DuckFile.LoadLevel(Content.path + "/levels/" + pValue);
                if (levelData2 != null)
                    return new GameLevel(levelData2.metaData.guid);
                foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                {
                    if (accessibleMod.configuration.content != null)
                    {
                        foreach (string level in accessibleMod.configuration.content.levels)
                        {
                            if (level.ToLowerInvariant().EndsWith(pValue))
                                return new GameLevel(level);
                        }
                    }
                }
                return Error("Level (" + pValue + ") was not found.");
            }
        }

        public class Thing<T> : CMD.String where T : Thing
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
                if (typeof(T) == typeof(Duck))
                {
                    Profile profile = DevConsole.ProfileByName(pValue);
                    if (profile == null)
                        return Error("Argument (" + pValue + ") should be the name of a player.");
                    return profile.duck == null ? Error("Player (" + pValue + ") is not present in the game.") : profile.duck;
                }
                if (typeof(T) == typeof(TeamHat))
                {
                    Team t = Teams.all.FirstOrDefault<Team>(x => x.name.ToLower() == pValue);
                    return t != null ? new TeamHat(0f, 0f, t) : Error("Argument (" + pValue + ") should be the name of a team");
                }
                foreach (System.Type thingType in Editor.ThingTypes)
                {
                    if (thingType.Name.ToLowerInvariant() == pValue)
                    {
                        if (!Editor.HasConstructorParameter(thingType))
                            return Error(thingType.Name + " can not be spawned this way.");
                        return !typeof(T).IsAssignableFrom(thingType) ? Error("Wrong object type (requires " + typeof(T).Name + ").") : Editor.CreateThing(thingType) as T;
                    }
                }
                return Error(typeof(T).Name + " of type (" + pValue + ") was not found.");
            }
        }
    }
}
