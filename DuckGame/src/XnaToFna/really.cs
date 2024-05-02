using System.Reflection;

namespace XnaToFna
{
    public class really
    {
        public AssemblyName name;

        internal bool ScanPath(XnaToFnaMapping mappings) => name.Name == mappings.Target;
    }
}
