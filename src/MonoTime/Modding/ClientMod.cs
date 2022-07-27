// Decompiled with JetBrains decompiler
// Type: DuckGame.ClientMod
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                if (source.Count<string>() >= 3)
                {
                    str1 = source[0];
                    str2 = source[1];
                    str3 = source[2];
                    if (source.Count<string>() > 3 && source[3].Trim() == "hd")
                        flag = true;
                }
            }
            if (pConfig == null)
                this.configuration = new ModConfiguration();
            else
                this.configuration = pConfig;
            this.configuration.assembly = Assembly.GetExecutingAssembly();
            this.configuration.contentManager = ContentManagers.GetContentManager(typeof(DefaultContentManager));
            this.configuration.name = str1;
            this.configuration.displayName = str1;
            this.configuration.description = str3;
            this.configuration.version = new Version(DG.version);
            this.configuration.author = str2;
            this.configuration.contentDirectory = pPath;
            this.configuration.directory = pPath;
            this.configuration.isHighResReskin = flag;
        }
    }
}
