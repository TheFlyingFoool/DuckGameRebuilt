// Decompiled with JetBrains decompiler
// Type: DuckGame.ContentManagers
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>The class that stores content managers.</summary>
    public static class ContentManagers
    {
        private static Dictionary<System.Type, IManageContent> _contentManagers = new Dictionary<System.Type, IManageContent>();

        private static IManageContent AddContentManager(System.Type t)
        {
            IManageContent instance = (IManageContent)Activator.CreateInstance(t);
            ContentManagers._contentManagers.Add(t, instance);
            return instance;
        }

        internal static IManageContent GetContentManager(System.Type t)
        {
            if (t == (System.Type)null)
                t = typeof(DefaultContentManager);
            IManageContent manageContent;
            return ContentManagers._contentManagers.TryGetValue(t, out manageContent) ? manageContent : ContentManagers.AddContentManager(t);
        }
    }
}
