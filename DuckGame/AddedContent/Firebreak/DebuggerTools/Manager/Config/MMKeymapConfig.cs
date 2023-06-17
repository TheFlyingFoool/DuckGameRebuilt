namespace DuckGame.MMConfig
{
    public sealed class MMKeymapConfig
    {
        [ACHeader]
        public MMDefaultKeymapSettingsConfig DefaultSettings = new();
        
        [ACHeader] [ACKeybind]
        public MMConsoleKeymapConfig Console = new();
        
        [ACHeader] [ACKeybind]
        public MMPagerKeymapConfig Pager = new();
        
        [ACHeader] [ACKeybind]
        public MMGeneralKeymapConfig General = new();
        
        [ACHeader] [ACKeybind]
        public MMSplitKeymapConfig Split = new();
        
        [ACHeader] [ACKeybind]
        public MMConfigKeymapConfig Config = new();
    }

    public sealed class MMDefaultKeymapSettingsConfig
    {
        public int SequenceGraceMilliseconds = 300;
        public int MultiInputGraceMilliseconds = 100;
    }

    public sealed class MMConsoleKeymapConfig
    {
        public string MoveCaretToBeginning = "_Ctrl+Up";
        public string MoveCaretToEnd = "_Ctrl+Down";
        public string MoveCaretLeft = "Left";
        public string MoveCaretRight = "Right";
        public string MoveCaretLeftByWord = "_Ctrl+Left";
        public string MoveCaretRightByWord = "_Ctrl+Right";
        public string RunCommand = "Enter";
    }

    public sealed class MMGeneralKeymapConfig
    {
        public string ReloadConfig = "F5";
        public string ResetZoom = "_Ctrl+D0";
        public string ZoomIn = "_Ctrl+OemPlus";
        public string ZoomOut = "_Ctrl+OemMinus";
        public string ToggleMallardManager = "_Ctrl+OemTilde";
    }

    public sealed class MMPagerKeymapConfig
    {
        public string CycleTabBackward = "None";
        public string CycleTabForward = "_Ctrl+Tab";
        public string NewTab = "_Ctrl+T";
        public string CloseTab = "_Ctrl+W";
    }

    public sealed class MMSplitKeymapConfig
    {
        public string ToggleEditMode = "_Ctrl+E";
    }

    public sealed class MMConfigKeymapConfig
    {
        public string UseSecondarySliderStep = "_Shift";
    }
}