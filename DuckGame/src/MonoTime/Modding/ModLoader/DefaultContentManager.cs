using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    /// <summary>
    /// The quick and easy default implementation. Pulls all exported types
    /// that are subclassed by the requested Type.
    /// </summary>
    internal class DefaultContentManager : IManageContent
    {
        public IEnumerable<System.Type> Compile<T>(Mod mod)
        {
            return mod.configuration.assembly.SaferGetTypes().Where(type => type.IsSubclassOf(typeof(T)));
        }
    }
}
