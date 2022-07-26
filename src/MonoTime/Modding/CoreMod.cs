// Decompiled with JetBrains decompiler
// Type: DuckGame.CoreMod
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Reflection;

namespace DuckGame
{
    /// <summary>The core "mod", for consistency sake.</summary>
    public sealed class CoreMod : Mod
    {
        /// <summary>The core mod instance, for quick comparisons.</summary>
        public static CoreMod coreMod { get; internal set; }

        internal CoreMod()
        {
            this.configuration = new ModConfiguration();
            this.configuration.assembly = Assembly.GetExecutingAssembly();
            this.configuration.contentManager = ContentManagers.GetContentManager(typeof(DefaultContentManager));
            this.configuration.name = "DuckGame";
            this.configuration.displayName = "Core";
            this.configuration.description = "The core mod for Duck Game content. This is no touchy. Bad. BAD duck. You will break your game, and I swear if you do I'm not picking that up. I just don't have the time these days to pick up after all of your goddamn mistakes. Seriously, just leave it alone. (it okay just do your best)";
            this.configuration.version = new Version(DG.version);
            this.configuration.author = "CORPTRON";
        }

        public override Priority priority => Priority.Inconsequential;
    }
}
