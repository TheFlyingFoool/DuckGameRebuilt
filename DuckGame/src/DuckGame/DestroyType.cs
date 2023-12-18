using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public abstract class DestroyType
    {
        private static Map<byte, System.Type> _types = new Map<byte, System.Type>();
        private Thing _thing;

        public static Map<byte, System.Type> indexTypeMap => _types;

        public static void InitializeTypes()
        {
            if (MonoMain.moddingEnabled)
            {
                byte key = 0;
                foreach (System.Type sortedType in ManagedContent.DestroyTypes.SortedTypes)
                {
                    _types.Add(key, sortedType);
                    ++key;
                }
            }
            else
            {
                List<System.Type> list = Editor.GetSubclasses(typeof(DestroyType)).ToList();
                byte key = 0;
                foreach (System.Type type in list)
                {
                    _types.Add(key, type);
                    ++key;
                }
            }
        }

        public Thing thing => _thing;

        public System.Type killThingType => _thing == null ? null : _thing.killThingType;

        public Profile responsibleProfile => _thing == null ? null : _thing.responsibleProfile;

        public DestroyType(Thing t = null) => _thing = t;
    }
}
