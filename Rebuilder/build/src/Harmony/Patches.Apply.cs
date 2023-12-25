using System.Reflection;

namespace DuckGame.Cobalt
{
    public static partial class Patches
    {
        public static void Apply()
        {
            foreach (MethodInfo patchMethod in typeof(Patches).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                PatchAttribute attribute = patchMethod.GetCustomAttribute<PatchAttribute>();
                
                if (attribute is null)
                    continue;
                
                Harmony.Patch(attribute.MethodToPatch, patchMethod, attribute.PatchType);
            }
        }
    }
}