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

        public string fullCommandName => this.parent != null ? this.parent.fullCommandName + " " + this.keyword : this.keyword;

        public CMD(string pKeyword, CMD.Argument[] pArguments, Action<CMD> pAction)
        {
            this.keyword = pKeyword.ToLowerInvariant();
            this.arguments = pArguments;
            this.action = pAction;
        }

        public CMD(string pKeyword, Action<CMD> pAction)
        {
            this.keyword = pKeyword.ToLowerInvariant();
            this.action = pAction;
        }

        public CMD(string pKeyword, Action pAction)
        {
            this.keyword = pKeyword.ToLowerInvariant();
            this.alternateAction = pAction;
        }

        public CMD(string pKeyword, CMD pSubCommand)
        {
            this.keyword = pKeyword.ToLowerInvariant();
            this.subcommand = pSubCommand;
            pSubCommand.parent = this;
            this.priority = -1;
        }

        public string info
        {
            get
            {
                string str = this.keyword + "(";
                if (this.arguments != null)
                {
                    bool flag = true;
                    foreach (CMD.Argument obj in this.arguments)
                    {
                        if (!flag)
                            str += ", ";
                        flag = false;
                        str = str + obj.type.Name + " " + obj.name;
                    }
                }
                string info = str + ")";
                if (this.description != "")
                    info = info + "\n|DGBLUE|" + this.description;
                return info;
            }
        }

        public bool Run(string pArguments)
        {
            this.logMessage = null;
            string[] source = pArguments.Split(' ');
            if (this.subcommand != null)
                return this.Error("|DGRED|Command (" + this.keyword + ") requires a sub command.");
            if (source.Count<string>() > 0 && source[0] == "?")
                return this.Help(this.info);
            if (source.Count<string>() > 0 && source[0].Trim().Length > 0 && this.arguments == null)
                return this.Error("|DGRED|Command (" + this.keyword + ") takes no arguments.");
            if (this.arguments != null)
            {
                int index1 = 0;
                int num = -1;
                foreach (CMD.Argument obj in this.arguments)
                {
                    if (obj.optional)
                    {
                        if (num < 0)
                            num = index1;
                    }
                    else if (num >= 0)
                        return this.Error("|DGRED|Command implementation error: 'optional' arguments must appear last.");
                    if (source.Count<string>() > index1)
                    {
                        if (!string.IsNullOrWhiteSpace(source[index1]))
                        {
                            try
                            {
                                if (this.arguments[index1].takesMultispaceString)
                                {
                                    string str = "";
                                    for (int index2 = index1; index2 < source.Length; ++index2)
                                        str = str + source[index2] + " ";
                                    obj.value = this.arguments[index1].Parse(str.Trim());
                                    index1 = source.Length;
                                }
                                else
                                    obj.value = this.arguments[index1].Parse(source[index1]);
                                if (obj.value == null)
                                    return this.Error("|DGRED|" + obj.GetParseFailedMessage() + " |GRAY|(" + obj.name + ")");
                                goto label_26;
                            }
                            catch (Exception ex)
                            {
                                return this.Error("|DGRED|Error parsing argument (" + obj.name + "): " + ex.Message);
                            }
                        }
                    }
                    if (!obj.optional)
                        return this.Error("|DGRED|Missing argument (" + obj.name + ")");
                    label_26:
                    ++index1;
                }
            }
            try
            {
                if (this.action != null)
                    this.action(this);
                else if (this.alternateAction != null)
                    this.alternateAction();
            }
            catch (Exception ex)
            {
                this.FinishExecution();
                return this.Error("|DGRED|Error running command: " + ex.Message);
            }
            this.FinishExecution();
            return true;
        }

        private void FinishExecution()
        {
            if (this.arguments == null)
                return;
            foreach (CMD.Argument obj in this.arguments)
                obj.value = null;
        }

        protected bool Error(string pError = null)
        {
            this.logMessage = "|DGRED|" + pError;
            return false;
        }

        protected bool Help(string pMessage = null)
        {
            this.logMessage = "|DGBLUE|" + pMessage;
            return true;
        }

        protected bool Message(string pMessage = null)
        {
            this.logMessage = pMessage;
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
                this.name = pName;
                this.optional = pOptional;
            }

            public virtual string GetParseFailedMessage() => this._parseFailMessage;

            public abstract object Parse(string pValue);

            protected object Error(string pError = null)
            {
                if (pError != null)
                    this._parseFailMessage = pError;
                return null;
            }
        }

        public class String : CMD.Argument
        {
            public String(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                this.type = typeof(string);
            }

            public override object Parse(string pValue) => pValue;
        }

        public class Integer : CMD.Argument
        {
            public Integer(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                this.type = typeof(int);
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
                return this.Error("Argument value must be an integer.");
            }
        }

        public class Font : CMD.Argument
        {
            private Func<int> defaultFontSize;

            public Font(string pName, Func<int> pDefaultFontSize = null, bool pOptional = false)
              : base(pName, pOptional)
            {
                this.type = typeof(string);
                this.takesMultispaceString = true;
                this.defaultFontSize = pDefaultFontSize;
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
                    return this.Error("Font (" + pValue + ") was not found.");
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
                this.type = typeof(CMD.Layer);
            }

            public override object Parse(string pValue) => (object)DuckGame.Layer.core._layers.FirstOrDefault<DuckGame.Layer>(x => x.name.ToLower() == pValue) ?? this.Error("Layer named (" + pValue + ") was not found.");
        }

        public class Level : CMD.Argument
        {
            public Level(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                this.type = typeof(CMD.Level);
                this.takesMultispaceString = true;
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
                return this.Error("Level (" + pValue + ") was not found.");
            }
        }

        public class Thing<T> : CMD.String where T : Thing
        {
            public Thing(string pName, bool pOptional = false)
              : base(pName, pOptional)
            {
                this.type = typeof(T);
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
                        return this.Error("Argument (" + pValue + ") should be the name of a player.");
                    return profile.duck == null ? this.Error("Player (" + pValue + ") is not present in the game.") : profile.duck;
                }
                if (typeof(T) == typeof(TeamHat))
                {
                    Team t = Teams.all.FirstOrDefault<Team>(x => x.name.ToLower() == pValue);
                    return t != null ? new TeamHat(0f, 0f, t) : this.Error("Argument (" + pValue + ") should be the name of a team");
                }
                foreach (System.Type thingType in Editor.ThingTypes)
                {
                    if (thingType.Name.ToLowerInvariant() == pValue)
                    {
                        if (!Editor.HasConstructorParameter(thingType))
                            return this.Error(thingType.Name + " can not be spawned this way.");
                        return !typeof(T).IsAssignableFrom(thingType) ? this.Error("Wrong object type (requires " + typeof(T).Name + ").") : Editor.CreateThing(thingType) as T;
                    }
                }
                return this.Error(typeof(T).Name + " of type (" + pValue + ") was not found.");
            }
        }
    }
}
