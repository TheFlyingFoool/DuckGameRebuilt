namespace DuckGame
{
    public class VirtualTransition
    {
        private static VirtualTransitionCore _core = new VirtualTransitionCore();

        public static VirtualTransitionCore core
        {
            get => _core;
            set => _core = value;
        }

        public static void Initialize() => _core.Initialize();

        public static void Update() => _core.Update();

        public static void Draw() => _core.Draw();

        public static bool doingVirtualTransition => _core.doingVirtualTransition;

        public static bool isVirtual => _core._virtualMode;

        public static void GoVirtual() => _core.GoVirtual();

        public static void GoUnVirtual() => _core.GoUnVirtual();

        public static bool active => _core.active;
    }
}
