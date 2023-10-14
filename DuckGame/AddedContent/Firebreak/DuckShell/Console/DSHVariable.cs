using System.Diagnostics;

namespace AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console
{
    [DebuggerDisplay("{Name}: {Value}")]
    public class DSHVariable
    {
        public string Name;
        public string Value;
    }
}