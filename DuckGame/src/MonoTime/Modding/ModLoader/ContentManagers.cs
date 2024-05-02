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
