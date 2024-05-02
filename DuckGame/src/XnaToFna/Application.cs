using System.Reflection;
using System.Threading;

namespace XnaToFna.ProxyForms
{
    public sealed class Application
    {
        public static event ThreadExceptionEventHandler ThreadException;

        public static string ProductVersion
        {
            get
            {
                Assembly entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return null;
                Module manifestModule = entryAssembly.ManifestModule;
                if (manifestModule != null)
                {
                    AssemblyInformationalVersionAttribute customAttribute1 = manifestModule.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                    if (customAttribute1 != null)
                        return customAttribute1.InformationalVersion;
                    AssemblyVersionAttribute customAttribute2 = manifestModule.GetCustomAttribute<AssemblyVersionAttribute>();
                    if (customAttribute2 != null)
                        return customAttribute2.Version;
                }
                return entryAssembly.GetName().Version.ToString();
            }
        }

        public static string ExecutablePath => Assembly.GetEntryAssembly().Location;

        public static void Run(Form mainForm)
        {
        }

        public static void EnableVisualStyles()
        {
        }
    }
}
