namespace DuckGame.MMConfig
{
    public sealed class MMKeymapConfig
    {
        [ACHeader] [ACKeybind]
        public MMConsoleKeymapConfig Console;
        
        [ACHeader] [ACKeybind]
        public MMPagerKeymapConfig Pager;
        
        [ACHeader] [ACKeybind]
        public MMGeneralKeymapConfig General;
        
        [ACHeader] [ACKeybind]
        public MMSplitKeymapConfig Split;
    }

    public sealed class MMConsoleKeymapConfig
    {
        public string MoveCaretToBeginning;
        public string MoveCaretToEnd;
        public string MoveCaretLeft;
        public string MoveCaretRight;
        public string MoveCaretLeftByWord;
        public string MoveCaretRightByWord;
        public string RunCommand;
    }

    public sealed class MMGeneralKeymapConfig
    {
        public string ToggleMallardManager;
        public string ReloadConfig;
        public string ZoomIn;
        public string ZoomOut;
        public string ResetZoom;
    }

    public sealed class MMPagerKeymapConfig
    {
        public string NewTab;
        public string CloseTab;
        public string CycleTabForward;
        public string CycleTabBackward;
    }

    public sealed class MMSplitKeymapConfig
    {
        public string ToggleEditMode;
    }
}