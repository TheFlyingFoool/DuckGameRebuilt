using DuckGame;
using HarmonyLib;
using HarmonyLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XnaToFna
{
    public static class ExtraStuff2Replacements
    {
        private static Assembly ExtraStuff2Assembly;
        public static void PatchReplacement()
        {
           
            if (ExtraStuff2Assembly == null)
            {
                ExtraStuff2Assembly = Assembly.GetCallingAssembly();
            }
 
     
            Type handlerType = ExtraStuff2Assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == "MAutoPatchHandler");

            if (handlerType == null)
                return;

            Type attributeType = ExtraStuff2Assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == "MAutoPatchAttribute");

            if (attributeType == null)
                return;
            MethodInfo getPatchesMethod = handlerType.GetMethod(
                "GetAllAutoPatches",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (getPatchesMethod == null)
                return;

            var allPatches = getPatchesMethod.Invoke(null, null) as IEnumerable<MethodInfo>;

            if (allPatches == null)
                return;

            FieldInfo typeField = attributeType.GetField("Type");
            FieldInfo methodField = attributeType.GetField("Method");
            FieldInfo patchTypeField = attributeType.GetField("PatchType");
            FieldInfo paramsField = attributeType.GetField("parameters");

            if (typeField == null || methodField == null || patchTypeField == null)
                return;


            Harmony harmony = Loader.harmonyInstance;
            foreach (var methodInfo in allPatches)
            {
                object[] attrs = methodInfo.GetCustomAttributes(attributeType, false);

                foreach (var attr in attrs)
                {
                    Type targetType = typeField.GetValue(attr) as Type;
                    string methodName = methodField.GetValue(attr) as string;
                    object patchTypeObj = patchTypeField.GetValue(attr);
                    Type[] parameters = paramsField?.GetValue(attr) as Type[];

                    if (targetType == null || string.IsNullOrEmpty(methodName))
                        continue;

                    MethodBase target = null;

                    if (methodName == ".ctor" || methodName == ".cctor")
                    {
                        target = AccessTools.DeclaredConstructor(targetType, parameters);
                    }
                    else
                    {
                        target = AccessTools.DeclaredMethod(targetType, methodName, parameters);
                    }

                    if (target == null)
                        continue;

                    HarmonyMethod harmonyMethod = new HarmonyMethod(methodInfo);

                    HarmonyMethod prefix = null;
                    HarmonyMethod postfix = null;
                    HarmonyMethod transpiler = null;

                    string patchTypeName = patchTypeObj.ToString();

                    if (patchTypeName.Contains("Prefix"))
                        prefix = harmonyMethod;
                    else if (patchTypeName.Contains("Postfix"))
                        postfix = harmonyMethod;
                    else if (patchTypeName.Contains("Transpiler"))
                        transpiler = harmonyMethod;

                    harmony.Patch(target, prefix, postfix, transpiler);
                }
            }

        }
    }
}
