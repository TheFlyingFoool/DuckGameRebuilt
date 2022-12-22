using Mono.Cecil;
using System.IO;

namespace XnaToFna
{
    public class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        public static string searchdirectorpath = "";
        public CustomAssemblyResolver() : base()
        {
            ResolveFailure += AssemblyResolveEventHandler;
        }
        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return base.Resolve(name);
        }
        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return base.Resolve(name, parameters);
        }
        public AssemblyDefinition AssemblyResolveEventHandler(object sender, AssemblyNameReference reference)
        {
            string[] dllmatchs = Directory.GetFiles(searchdirectorpath, reference.Name + ".dll", SearchOption.AllDirectories);
            if (reference != null && reference.FullName != null && reference.FullName.StartsWith("Steam,") || reference.FullName.StartsWith("Steam.Debug,"))
            {
                string path = DuckGame.Program.GameDirectory + "DGSteam.dll";
                return GetAssembly(sender as IAssemblyResolver, path, new ReaderParameters()); //MonoModExt.ReadModule(path, (sender as DefaultAssemblyResolver).GetAssembly(path, new ReaderParameters()));
            }
            else if (dllmatchs.Length > 0)
            {
                return GetAssembly(sender as IAssemblyResolver, dllmatchs[0], new ReaderParameters());
            }
            return null;
        }
        public static AssemblyDefinition GetAssembly(IAssemblyResolver assemblyResolver, string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
            {
                parameters.AssemblyResolver = assemblyResolver;
            }
            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }
    }
}
