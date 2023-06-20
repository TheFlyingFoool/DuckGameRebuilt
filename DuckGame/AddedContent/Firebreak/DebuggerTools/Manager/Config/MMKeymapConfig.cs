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
        public int SequenceGraceMilliseconds = 200;
        public int MultiInputGraceMilliseconds = 50;
    }

    public sealed class MMConsoleKeymapConfig
    {
        public string MoveCaretToBeginning = "_ctrl+up";
        public string MoveCaretToEnd = "_ctrl+down";
        public string MoveCaretLeft = "left";
        public string MoveCaretRight = "right";
        public string MoveCaretLeftByWord = "_ctrl+left";
        public string MoveCaretRightByWord = "_ctrl+right";
        public string RunCommand = "enter";
    }

    public sealed class MMGeneralKeymapConfig
    {
        public string ReloadConfig = "f5";
        public string ResetZoom = "_ctrl+d0";
        public string ZoomIn = "_ctrl+oemplus";
        public string ZoomOut = "_ctrl+oemminus";
        public string ToggleMallardManager = "_ctrl+oemtilde";
    }

    public sealed class MMPagerKeymapConfig
    {
        public string CycleTabBackward = "*";
        public string CycleTabForward = "_ctrl+tab";
        public string NewTab = "_ctrl+t";
        public string CloseTab = "_ctrl+w";
    }

    public sealed class MMSplitKeymapConfig
    {
        public string ToggleEditMode = "_ctrl+E";
    }

    public sealed class MMConfigKeymapConfig
    {
        public string UseSecondarySliderStep = "_Shift";
    }
}