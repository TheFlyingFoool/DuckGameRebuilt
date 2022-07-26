// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualTransition
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class VirtualTransition
    {
        private static VirtualTransitionCore _core = new VirtualTransitionCore();

        public static VirtualTransitionCore core
        {
            get => VirtualTransition._core;
            set => VirtualTransition._core = value;
        }

        public static void Initialize() => VirtualTransition._core.Initialize();

        public static void Update() => VirtualTransition._core.Update();

        public static void Draw() => VirtualTransition._core.Draw();

        public static bool doingVirtualTransition => VirtualTransition._core.doingVirtualTransition;

        public static bool isVirtual => VirtualTransition._core._virtualMode;

        public static void GoVirtual() => VirtualTransition._core.GoVirtual();

        public static void GoUnVirtual() => VirtualTransition._core.GoUnVirtual();

        public static bool active => VirtualTransition._core.active;
    }
}
