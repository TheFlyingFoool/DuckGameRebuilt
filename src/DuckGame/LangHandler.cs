using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DuckGame
{
    public static class LangHandler
    {
        public static List<string> drawnstrings = new List<string>();
        public static string[] supportedlangs = new string[0];
        public static Dictionary<string, Dictionary<string, string>> langmap = new Dictionary<string, Dictionary<string, string>>();
        public static string lang = "en";
        public static bool reverse;
        public static void Initialize()
        {
            if (lang != "en")
            {
                reverse = true;
            }
            string[] textfile = System.IO.File.ReadAllText("lang.txt").Split('\n');
            string lastenkey = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                string line = textfile[i].Replace("\r","");
                int startkeyword = line.IndexOf("<");
                int endkeyword = line.IndexOf(">");
                if (startkeyword > endkeyword || startkeyword < 0)
                {
                    continue;
                }
                string keyword = line.Substring(startkeyword + 1, endkeyword - 1);
                string value = line.Substring(keyword.Length + 2, line.Length - keyword.Length * 2 - 5);
                if (keyword == "supported")
                {
                    supportedlangs = value.Split(',');
                    foreach (string lang in supportedlangs)
                    {
                        langmap[lang] = new Dictionary<string, string>();
                    }
                }
                else if (supportedlangs.Contains(keyword))
                {
                    if (keyword == "en")
                    {
                        lastenkey = value;
                    }
                    langmap[keyword][lastenkey] = value;
                }
            }
        }
        public static string Convert(string input)
        {
            if (lang != "en" && input != null && langmap[lang].TryGetValue(input,out string converted))
            {
                return converted;
            }
            return input;
        }
    }
}
