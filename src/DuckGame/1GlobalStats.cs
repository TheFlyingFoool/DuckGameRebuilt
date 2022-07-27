// Decompiled with JetBrains decompiler
// Type: DuckGame.Global
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public static class Global
    {
        public static HashSet<string> boughtHats = new HashSet<string>();
        private static List<string> _achievementList = new List<string>()
    {
      "play100",
      "chancy",
      "play1000",
      "kill1000",
      "endurance",
      "online10",
      "basement",
      "editor",
      "drawbreaker",
      "fire",
      "crate",
      "book",
      "mine",
      "laser",
      "finish50",
      "myboy",
      "jukebox",
      "kingme"
    };
        private static Dictionary<string, bool> _achievementStatus = new Dictionary<string, bool>();
        private static GlobalData _data = new GlobalData();
        private static bool loadCalled = false;
        public static DXMLNode _customLoadDoc = null;

        public static bool HasAchievement(string pAchievement)
        {
            bool flag;
            return !Global._achievementStatus.TryGetValue(pAchievement, out flag) ? Steam.GetAchievement(pAchievement) : flag;
        }

        public static void GiveAchievement(string pAchievement)
        {
            if (Global.HasAchievement(pAchievement))
                return;
            Steam.SetAchievement(pAchievement);
            Global._achievementStatus[pAchievement] = true;
        }

        public static GlobalData data
        {
            get => Global._data;
            set => Global._data = value;
        }

        public static void Initialize()
        {
            foreach (string achievement in Global._achievementList)
                Global._achievementStatus[achievement] = Steam.GetAchievement(achievement);
            Global.data.unlockListIndex = Rando.Int(500);
            Global.data.flag = 0;
            Global.Load();
        }

        public static void Kill(Duck d, DestroyType type)
        {
            if (d.team == null || !(d.team.name == "SWACK"))
                return;
            ++Global.data.killsAsSwack;
        }

        public static void WinLevel(Team t)
        {
        }

        public static void WinMatch(Team t)
        {
            if (!Global._data.hatWins.ContainsKey(t.name))
                Global._data.hatWins[t.name] = 0;
            Global._data.hatWins[t.name]++;
        }

        public static void PlayCustomLevel(string lev)
        {
            if (!Global._data.customMapPlayCount.ContainsKey(lev))
                Global._data.customMapPlayCount[lev] = 0;
            Global._data.customMapPlayCount[lev]++;
        }

        public static void Save()
        {
            if (!Global.loadCalled)
            {
                if (!MonoMain.logFileOperations)
                    return;
                DevConsole.Log(DCSection.General, "Global.Save() skipped (loadCalled == false)");
            }
            else
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "Global.Save()");
                DuckXML doc = new DuckXML();
                DXMLNode node = new DXMLNode("GlobalData");
                Global._data.boughtHats = "";
                foreach (string boughtHat in Global.boughtHats)
                {
                    GlobalData data = Global._data;
                    data.boughtHats = data.boughtHats + boughtHat + "|";
                }
                node.Add(Global._data.Serialize());
                doc.Add(node);
                string globalFileName = Global.globalFileName;
                DuckFile.SaveDuckXML(doc, globalFileName);
            }
        }

        public static string globalFileName => DuckFile.optionsDirectory + "/global.dat";

        public static void Load()
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "Global.Load()");
            Global.loadCalled = true;
            string globalFileName = Global.globalFileName;
            DXMLNode dxmlNode = Global._customLoadDoc ?? DuckFile.LoadDuckXML(globalFileName);
            if (dxmlNode != null)
            {
                Profile profile = new Profile("");
                IEnumerable<DXMLNode> source = dxmlNode.Elements("GlobalData");
                if (source != null)
                {
                    foreach (DXMLNode element in source.Elements<DXMLNode>())
                    {
                        if (element.Name == nameof(Global))
                        {
                            Global._data.Deserialize(element);
                            break;
                        }
                    }
                }
            }
            string boughtHats = Global._data.boughtHats;
            char[] chArray = new char[1] { '|' };
            foreach (string str in boughtHats.Split(chArray))
            {
                if (str != "")
                    Global.boughtHats.Add(str);
            }
        }
    }
}
