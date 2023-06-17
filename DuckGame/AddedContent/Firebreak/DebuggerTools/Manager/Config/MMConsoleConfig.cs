namespace DuckGame.MMConfig
{
    public sealed class MMConsoleConfig
    {
        [ACMin(0)]
        public int TabWidth = 4;
        public string ShellStartUpCommand = "";
        
        [ACHeader]
        public MMCaretConfig Caret = new();
    }
}