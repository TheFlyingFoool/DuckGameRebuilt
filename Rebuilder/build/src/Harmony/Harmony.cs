using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DuckGame.Cobalt
{
    public static class Harmony
    {
        public static Assembly s_harmonyInstance = null;
        
        public static void Initialize()
        {
            FieldInfo _modAssembliesInfo = typeof(ModLoader).GetField("_modAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<Assembly, Mod> _modAssemblies = (Dictionary<Assembly, Mod>) _modAssembliesInfo!.GetValue(null);
            
            // look around to see if harmony's already been loaded by another mod
            foreach (Assembly assembly in _modAssemblies.Keys)
            {
                if (assembly.GetType("HarmonyLoader.Loader") == null)
                    continue;
                
                s_harmonyInstance = assembly;
                break;
            }

            // fine, i'll do it myself
            if (s_harmonyInstance is null)
            {
                s_harmonyInstance = Assembly.Load(File.ReadAllBytes(Mod.GetPath<Rebuilder>("HarmonyLoader") + ".dll"));
                _modAssemblies.Add(s_harmonyInstance, new DisabledMod());
            }
            
            Patches.Apply();
        }

        public static void Patch(MethodInfo original, MethodInfo applied, Fix patchType = Fix.Prefix)
        {
            switch (patchType)
            {
                case Fix.Prefix:
                    Patch(original, applied, null, null);
                    break;

                case Fix.Postfix:
                    Patch(original, null, applied, null);
                    break;

                default:
                    // me when transpilers
                    throw new ArgumentOutOfRangeException(nameof(patchType), patchType, null);
            }
        }

        public static void Patch(MethodInfo original, MethodInfo prefix, MethodInfo postfix, MethodInfo transpiler)
        {                                             
            try
            {
                Type harmonyLoaderType = s_harmonyInstance.GetType("HarmonyLoader.Loader");
                MethodInfo patchMethod = harmonyLoaderType.GetMethod("Patch2", BindingFlags.Static | BindingFlags.Public)!;

                patchMethod.Invoke(null, new object[] { original, prefix, postfix, transpiler });
            }
            catch (Exception ex)
            {
                DevConsole.Log("Patch failure", Color.Lime);
                DevConsole.LogComplexMessage(ex.ToString(), Color.LimeGreen);
            }
        }

        public enum Fix
        {
            Prefix,
            Postfix
        }
    }
}