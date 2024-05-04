using Mono.Cecil;

namespace XnaToFna
{
    public class NN
    {
        public ModuleDefinition mod;

        public bool Relinkthing(XnaToFnaMapping mappings) => mod.Assembly.Name.Name == mappings.Target;
    }
}
