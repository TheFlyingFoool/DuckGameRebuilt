namespace DuckGame.MMConfig
{
    [AdvancedConfig("mallardManager")]
    public sealed class MallardManagerConfig : IAdvancedConfig
    {
        public bool Enabled;
        
        [ACMin(0)] [ACMax(8)] [ACIncrementValue(1)]
        public float Zoom;
        
        [ACMin(0)] [ACMax(1)] [ACIncrementValue(0.05)]
        public float Opacity;
        
        [ACHeader] [ACColor]
        public MMColorsConfig Colors;
        
        [ACHeader]
        public MMConsoleConfig Console;
        
        [ACHeader]
        public MMKeymapConfig Keymap;
        
        public void RevertToDefaults()
        {
            #if DEBUG
            Enabled = true;
            #else
            Enabled = false;
            #endif
            
            Opacity = 1f;
            Zoom = 1f;
            Console = new MMConsoleConfig()
            {
                TabWidth = 4,
                ShellStartUpCommand = "",
                Caret = new MMCaretConfig()
                {
                    BlinkSpeed = 1f,
                    IsHorizontal = false,
                    ThicknessPercentage = 1f,
                    MovementSmoothness = 20f,
                },
            };
            Colors = new MMColorsConfig()
            {
                UserText = "#eeeeee",
                PrimarySystemText = "#0a7e07",
                SecondarySystemText = "#d01716",
                UserOverlay = "#eeeeee",
                Primary = "#546e7a",
                PrimarySub = "#283238",
                Secondary = "#78909c",
                SecondarySub = "#546e7a",
                PrimaryBackground = "#b0bec5",
                SecondaryBackground = "#90a4ae",
            };
            Keymap = new MMKeymapConfig()
            {
                General = new MMGeneralKeymapConfig()
                {
                    ReloadConfig = "F5",
                    ResetZoom = "Ctrl+D0",
                    ZoomIn = "Ctrl+OemPlus",
                    ZoomOut = "Ctrl+OemMinus",
                    ToggleMallardManager = "Ctrl+OemTilde"
                },
                Console = new MMConsoleKeymapConfig()
                {
                    MoveCaretToBeginning = "Ctrl+Up",
                    MoveCaretToEnd = "Ctrl+Down",
                    MoveCaretLeft = "Left",
                    MoveCaretRight = "Right",
                    MoveCaretLeftByWord = "Ctrl+Left",
                    MoveCaretRightByWord = "Ctrl+Right",
                    RunCommand = "Enter",
                },
                Pager = new MMPagerKeymapConfig()
                {
                    CycleTabBackward = "None",
                    CycleTabForward = "Ctrl+Tab",
                    NewTab = "Ctrl+T",
                    CloseTab = "Ctrl+W",
                },
                Split = new MMSplitKeymapConfig()
                {
                    ToggleEditMode = "Ctrl+E"
                }
            };
        }
    }
}