namespace DuckGame.MMConfig
{
    public sealed class MMKeymapConfig
    {
        [ACHeader]
        public MMDefaultKeymapSettingsConfig DefaultSettings = new();
        
        [ACHeader]
        public MMConsoleKeymapConfig Console = new();
        
        [ACHeader]
        public MMPagerKeymapConfig Pager = new();
        
        [ACHeader]
        public MMGeneralKeymapConfig General = new();
        
        [ACHeader]
        public MMSplitKeymapConfig Split = new();
        
        [ACHeader]
        public MMConfigKeymapConfig Config = new();
    }

    public sealed class MMDefaultKeymapSettingsConfig
    {
        [ACKeybind] public int SequenceGraceMilliseconds = 200;
        [ACKeybind] public int MultiInputGraceMilliseconds = 50;
    }

    public sealed class MMConsoleKeymapConfig
    {
        [ACKeybind] public string ScrollLinesUp = "pageup";
        [ACKeybind] public string ScrollLinesDown = "pagedown";
        [ACKeybind] public string MoveCaretToBeginning = "_ctrl+up";
        [ACKeybind] public string MoveCaretToEnd = "_ctrl+down";
        [ACKeybind] public string RunCommand = "enter";
        [ACKeybind] public string PasteText = "_ctrl+v";
        [ACKeybind] public string CopySelection = "_ctrl+c";
    }

    public sealed class MMGeneralKeymapConfig
    {
        [ACKeybind] public string ReloadConfig = "f5";
        [ACKeybind] public string ResetZoom = "_ctrl+d0";
        [ACKeybind] public string ZoomIn = "_ctrl+oemplus";
        [ACKeybind] public string ZoomOut = "_ctrl+oemminus";
        [ACKeybind] public string ToggleMallardManager = "numpad1,numpad8";
    }

    public sealed class MMPagerKeymapConfig
    {
        [ACKeybind] public string CycleTabBackward = "*";
        [ACKeybind] public string CycleTabForward = "_ctrl+tab";
        [ACKeybind] public string NewTab = "_ctrl+t";
        [ACKeybind] public string CloseTab = "_ctrl+w";
    }

    public sealed class MMSplitKeymapConfig
    {
        [ACKeybind] public string ToggleEditMode = "_ctrl+e";
    }

    public sealed class MMConfigKeymapConfig
    {
        [ACKeybind] public string UseSecondarySliderStep = "_shift";
    }
}