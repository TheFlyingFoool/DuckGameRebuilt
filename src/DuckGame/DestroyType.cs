// Decompiled with JetBrains decompiler
// Type: DuckGame.DestroyType
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public abstract class DestroyType
    {
        private static Map<byte, System.Type> _types = new Map<byte, System.Type>();
        private Thing _thing;

        public static Map<byte, System.Type> indexTypeMap => DestroyType._types;

        public static void InitializeTypes()
        {
            if (MonoMain.moddingEnabled)
            {
                byte key = 0;
                foreach (System.Type sortedType in ManagedContent.DestroyTypes.SortedTypes)
                {
                    DestroyType._types.Add(key, sortedType);
                    ++key;
                }
            }
            else
            {
                List<System.Type> list = Editor.GetSubclasses(typeof(DestroyType)).ToList<System.Type>();
                byte key = 0;
                foreach (System.Type type in list)
                {
                    DestroyType._types.Add(key, type);
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
