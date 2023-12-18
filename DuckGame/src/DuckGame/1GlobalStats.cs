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
            return !_achievementStatus.TryGetValue(pAchievement, out flag) ? Steam.GetAchievement(pAchievement) : flag;
        }

        public static void GiveAchievement(string pAchievement)
        {
            if (HasAchievement(pAchievement))
                return;
            Steam.SetAchievement(pAchievement);
            _achievementStatus[pAchievement] = true;
        }

        public static GlobalData data
        {
            get => _data;
            set => _data = value;
        }

        public static void Initialize()
        {
            foreach (string achievement in _achievementList)
                _achievementStatus[achievement] = Steam.GetAchievement(achievement);
            data.unlockListIndex = Rando.Int(500);
            data.flag = 0;
            Load();
        }

        public static void Kill(Duck d, DestroyType type)
        {
            if (d.team == null || !(d.team.name == "SWACK"))
                return;
            ++data.killsAsSwack;
        }

        public static void WinLevel(Team t)
        {
        }

        public static void WinMatch(Team t)
        {
            if (!_data.hatWins.TryGetValue(t.name, out int n))
            {
                _data.hatWins[t.name] = 1;
                return;
            }
            _data.hatWins[t.name] = n + 1;
        }

        public static void PlayCustomLevel(string lev)
        {
            if (!_data.customMapPlayCount.TryGetValue(lev, out int n))
            {
                _data.customMapPlayCount[lev] = 1;
                return;
            }
            _data.customMapPlayCount[lev] = n + 1;
        }

        public static void Save()
        {
            if (!loadCalled)
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
                _data.boughtHats = "";
                foreach (string boughtHat in boughtHats)
                {
                    GlobalData data = _data;
                    data.boughtHats = data.boughtHats + boughtHat + "|";
                }
                node.Add(_data.Serialize());
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
            loadCalled = true;
            string globalFileName = Global.globalFileName;
            DXMLNode dxmlNode = _customLoadDoc ?? DuckFile.LoadDuckXML(globalFileName);
            if (dxmlNode != null)
            {
                Profile profile = new Profile("");
                IEnumerable<DXMLNode> source = dxmlNode.Elements("GlobalData");
                if (source != null)
                {
                    foreach (DXMLNode element in source.Elements())
                    {
                        if (element.Name == nameof(Global))
                        {
                            _data.Deserialize(element);
                            break;
                        }
                    }
                }
            }
            string boughtHats = _data.boughtHats;
            char[] chArray = new char[1] { '|' };
            foreach (string str in boughtHats.Split(chArray))
            {
                if (str != "")
                    Global.boughtHats.Add(str);
            }
        }
    }
}
