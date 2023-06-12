namespace DuckGame.MMConfig
{
    [AdvancedConfig("mallardManager")]
    public sealed class MallardManagerConfig : IAdvancedConfig
    {
        [ACHidden]
        public bool Enabled;
        
        [ACHidden]
        public float Zoom;
        
        [ACMin(0)] [ACMax(1)] [ACSlider(0.1, SecondaryStep = 0.01)]
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
            
            Opacity = 0.9f;
            Zoom = 0;
            Console = new MMConsoleConfig()
            {
                TabWidth = 4,
                ShellStartUpCommand = "",
                Caret = new MMCaretConfig()
                {
                    BlinkSpeed = 1f,
                    IsHorizontal = false,
                    ThicknessPercentage = 0.2f,
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
                    ResetZoom = "_Ctrl+D0",
                    ZoomIn = "_Ctrl+OemPlus",
                    ZoomOut = "_Ctrl+OemMinus",
                    ToggleMallardManager = "_Ctrl+OemTilde"
                },
                Console = new MMConsoleKeymapConfig()
                {
                    MoveCaretToBeginning = "_Ctrl+Up",
                    MoveCaretToEnd = "_Ctrl+Down",
                    MoveCaretLeft = "Left",
                    MoveCaretRight = "Right",
                    MoveCaretLeftByWord = "_Ctrl+Left",
                    MoveCaretRightByWord = "_Ctrl+Right",
                    RunCommand = "Enter",
                },
                Pager = new MMPagerKeymapConfig()
                {
                    CycleTabBackward = "None",
                    CycleTabForward = "_Ctrl+Tab",
                    NewTab = "_Ctrl+T",
                    CloseTab = "_Ctrl+W",
                },
                Split = new MMSplitKeymapConfig()
                {
                    ToggleEditMode = "_Ctrl+E"
                },
                Config = new MMConfigKeymapConfig
                {
                    UseSecondarySliderStep = "_Shift"
                }
            };
        }
    }
}