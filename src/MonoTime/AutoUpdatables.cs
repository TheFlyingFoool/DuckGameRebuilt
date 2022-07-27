// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoUpdatables
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class AutoUpdatables
    {
        private static AutoUpdatables.Core _core = new AutoUpdatables.Core();

        public static AutoUpdatables.Core core
        {
            get => AutoUpdatables._core;
            set => AutoUpdatables._core = value;
        }

        private static List<WeakReference> _updateables
        {
            get => AutoUpdatables._core._updateables;
            set => AutoUpdatables._core._updateables = value;
        }

        public static bool ignoreAdditions
        {
            get => AutoUpdatables._core.ignoreAdditions;
            set => AutoUpdatables._core.ignoreAdditions = value;
        }

        public static void Add(IAutoUpdate update)
        {
            if (AutoUpdatables.ignoreAdditions)
                return;
            AutoUpdatables._updateables.Add(new WeakReference(update));
        }

        public static void Clear() => AutoUpdatables._updateables.Clear();

        public static void ClearSounds()
        {
            for (int index = 0; index < AutoUpdatables._updateables.Count; ++index)
            {
                if (AutoUpdatables._updateables[index] != null && AutoUpdatables._updateables[index].Target != null && AutoUpdatables._updateables[index].Target is ConstantSound)
                    (AutoUpdatables._updateables[index].Target as ConstantSound).Kill();
            }
        }

        public static void MuteSounds()
        {
            for (int index = 0; index < AutoUpdatables._updateables.Count; ++index)
            {
                if (AutoUpdatables._updateables[index] != null && AutoUpdatables._updateables[index].Target != null && AutoUpdatables._updateables[index].Target is ConstantSound)
                    (AutoUpdatables._updateables[index].Target as ConstantSound).Mute();
            }
        }

        public static void Update()
        {
            int num = 25;
            for (int index = 0; index < AutoUpdatables._updateables.Count; ++index)
            {
                if (AutoUpdatables._updateables[index] == null)
                {
                    if (num > 0)
                    {
                        AutoUpdatables._updateables.RemoveAt(index);
                        --index;
                        --num;
                    }
                }
                else
                {
                    IAutoUpdate target = AutoUpdatables._updateables[index].Target as IAutoUpdate;
                    if (!AutoUpdatables._updateables[index].IsAlive || target == null)
                        AutoUpdatables._updateables[index] = null;
                    else
                        target.Update();
                }
            }
        }

        public class Core
        {
            public List<WeakReference> _updateables = new List<WeakReference>();
            public bool ignoreAdditions;
        }
    }
}
