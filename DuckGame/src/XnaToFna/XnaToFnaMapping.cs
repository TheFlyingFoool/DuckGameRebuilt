using Mono.Cecil;

namespace XnaToFna
{
    public class XnaToFnaMapping
    {
        public bool IsActive;
        public ModuleDefinition Module;
        public string Target;
        public string[] Sources;
        public SetupDelegate Setup;

        public XnaToFnaMapping(string target, string[] sources, SetupDelegate setup = null)
        {
            Target = target;
            Sources = sources;
            Setup = setup;
        }

        public delegate void SetupDelegate(XnaToFnaUtil xtf, XnaToFnaMapping mapping);
    }
}
