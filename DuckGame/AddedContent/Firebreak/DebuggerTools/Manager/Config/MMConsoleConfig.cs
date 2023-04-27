namespace DuckGame.MMConfig
{
    public sealed class MMConsoleConfig
    {
        [ACMin(0)]
        public int TabWidth;
        public string ShellStartUpCommand;
        
        [ACHeader]
        public MMCaretConfig Caret;
    }
}