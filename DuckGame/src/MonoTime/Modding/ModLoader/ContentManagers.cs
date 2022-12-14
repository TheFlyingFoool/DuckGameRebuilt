// Decompiled with JetBrains decompiler
// Type: DuckGame.ContentManagers
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private static Dictionary<Type, IManageContent> _contentManagers = new Dictionary<Type, IManageContent>();

        private static IManageContent AddContentManager(Type t)
        {
            IManageContent instance = (IManageContent)Activator.CreateInstance(t);
            _contentManagers.Add(t, instance);
            return instance;
        }

        internal static IManageContent GetContentManager(Type t)
        {
            if (t == null)
                t = typeof(DefaultContentManager);
            IManageContent manageContent;
            return _contentManagers.TryGetValue(t, out manageContent) ? manageContent : AddContentManager(t);
        }
    }
}
