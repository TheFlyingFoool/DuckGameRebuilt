using System;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class ClientMod : DisabledMod
    {
        public ClientMod()
        {
        }

        public ClientMod(string pPath, ModConfiguration pConfig = null, string pInfoFile = "info.txt")
        {
            string str1 = "null";
            string str2 = "null";
            string str3 = "null";
            bool flag = false;
            if (DuckFile.FileExists(pPath + pInfoFile))
            {
                string[] source = DuckFile.ReadAllLines(pPath + pInfoFile);
                if (source.Length >= 3)
                {
                    str1 = source[0];
                    str2 = source[1];
                    str3 = source[2];
                    if (source.Length > 3 && source[3].Trim() == "hd")
                        flag = true;
                }
            }
            if (pConfig == null)
                configuration = new ModConfiguration();
            else
                configuration = pConfig;
            configuration.assembly = Assembly.GetExecutingAssembly();
            configuration.contentManager = ContentManagers.GetContentManager(typeof(DefaultContentManager));
            configuration.name = str1;
            configuration.displayName = str1;
            configuration.description = str3;
            configuration.version = new Version(DG.version);
            configuration.author = str2;
            configuration.contentDirectory = pPath;
            configuration.directory = pPath;
            configuration.isHighResReskin = flag;
        }
    }
}
