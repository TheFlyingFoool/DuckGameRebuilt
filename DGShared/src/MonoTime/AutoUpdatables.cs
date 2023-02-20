// Decompiled with JetBrains decompiler
// Type: DuckGame.AutoUpdatables
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class AutoUpdatables
    {
        private static Core _core = new Core();

        public static Core core
        {
            get => _core;
            set => _core = value;
        }

        private static List<WeakReference> _updateables
        {
            get => _core._updateables;
            set => _core._updateables = value;
        }

        public static bool ignoreAdditions
        {
            get => _core.ignoreAdditions;
            set => _core.ignoreAdditions = value;
        }

        public static void Add(IAutoUpdate update)
        {
            if (ignoreAdditions)
                return;
            _updateables.Add(new WeakReference(update));
        }

        public static void Clear() => _updateables.Clear();

        public static void ClearSounds()
        {
            for (int index = 0; index < _updateables.Count; ++index)
            {
                if (_updateables[index] != null && _updateables[index].Target != null && _updateables[index].Target is ConstantSound)
                    (_updateables[index].Target as ConstantSound).Kill();
            }
        }

        public static void MuteSounds()
        {
            for (int index = 0; index < _updateables.Count; ++index)
            {
                if (_updateables[index] != null && _updateables[index].Target != null && _updateables[index].Target is ConstantSound)
                    (_updateables[index].Target as ConstantSound).Mute();
            }
        }

        public static void Update()
        {
            int num = 25;
            for (int index = 0; index < _updateables.Count; ++index)
            {
                if (_updateables[index] == null)
                {
                    if (num > 0)
                    {
                        _updateables.RemoveAt(index);
                        --index;
                        --num;
                    }
                }
                else
                {
                    IAutoUpdate target = _updateables[index].Target as IAutoUpdate;
                    if (!_updateables[index].IsAlive || target == null)
                        _updateables[index] = null;
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
