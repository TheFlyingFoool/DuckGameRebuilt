using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace MonoMod.Utils
{
    public static class DynDll
    {
        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string filename, int flags);

        [DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string symbol);

        [DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dlerror();

        public static IntPtr OpenLibrary(string name)
        {
            string mapped;
            if (name != null && DllMap.TryGetValue(name, out mapped))
            {
                name = mapped;
            }
            IntPtr lib;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                lib = GetModuleHandle(name);
                if (lib == IntPtr.Zero)
                {
                    lib = LoadLibrary(name);
                }
                return lib;
            }
            IntPtr e = IntPtr.Zero;
            lib = dlopen(name, 2);
            if ((e = dlerror()) != IntPtr.Zero)
            {
                Console.WriteLine(string.Format("DynDll can't access {0}!", name ?? "entry point"));
                Console.WriteLine("dlerror: " + Marshal.PtrToStringAnsi(e));
                return IntPtr.Zero;
            }
            return lib;
        }

        public static IntPtr GetFunction(this IntPtr lib, string name)
        {
            if (lib == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return GetProcAddress(lib, name);
            }
            IntPtr s = dlsym(lib, name);
            IntPtr e;
            if ((e = dlerror()) != IntPtr.Zero)
            {
                Console.WriteLine("DynDll can't access " + name + "!");
                Console.WriteLine("dlerror: " + Marshal.PtrToStringAnsi(e));
                return IntPtr.Zero;
            }
            return s;
        }

        public static T AsDelegate<T>(this IntPtr s) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(s, typeof(T)) as T;
        }

        public static void ResolveDynDllImports(this Type type)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bool found = true;
                foreach (DynDllImportAttribute attrib in field.GetCustomAttributes(typeof(DynDllImportAttribute), true))
                {
                    found = false;
                    IntPtr asm = OpenLibrary(attrib.DLL);
                    if (!(asm == IntPtr.Zero))
                    {
                        foreach (string ep in attrib.EntryPoints)
                        {
                            IntPtr func = asm.GetFunction(ep);
                            if (!(func == IntPtr.Zero))
                            {
                                field.SetValue(null, Marshal.GetDelegateForFunctionPointer(func, field.FieldType));
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                }
                if (!found)
                {
                    throw new EntryPointNotFoundException(string.Format("No matching entry point found for {0} in {1}", field.Name, field.DeclaringType.FullName));
                }
            }
        }

        // Note: this type is marked as 'beforefieldinit'.
        static DynDll()
        {
        }

        public static Dictionary<string, string> DllMap = new Dictionary<string, string>();

        private const int RTLD_NOW = 2;
    }
}


namespace MonoMod.Utils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DynDllImportAttribute : Attribute
    {
        [Obsolete("Pass the entry points as parameters instead.")]
        public string EntryPoint
        {
            set
            {
                this.EntryPoints = new string[]
                {
                    value
                };
            }
        }

        public DynDllImportAttribute(string dll, params string[] entryPoints)
        {
            this.DLL = dll;
            this.EntryPoints = entryPoints;
        }

        public string DLL;

        public string[] EntryPoints;
    }
}
//using System;
//using System.IO;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using MonoMod;
namespace MonoMod.Utils
{
    //[MonoMod.MonoMod__OldName__("MonoMod.Helpers.PlatformHelper")]
    public static class PlatformHelper
    {
        static PlatformHelper()
        {
            PropertyInfo property_platform = typeof(Environment).GetProperty("Platform", BindingFlags.Static | BindingFlags.NonPublic);
            string platID;
            if (property_platform != null)
            {
                platID = property_platform.GetValue(null, new object[0]).ToString();
            }
            else
            {
                platID = Environment.OSVersion.Platform.ToString();
            }
            platID = platID.ToLowerInvariant();
            Current = Platform.Unknown;
            if (platID.Contains("win"))
            {
                Current = Platform.Windows;
            }
            else if (platID.Contains("mac") || platID.Contains("osx"))
            {
                Current = Platform.MacOS;
            }
            else if (platID.Contains("lin") || platID.Contains("unix"))
            {
                Current = Platform.Linux;
            }
            if (Directory.Exists("/data") && File.Exists("/system/build.prop"))
            {
                Current = Platform.Android;
            }
            else if (Directory.Exists("/Applications") && Directory.Exists("/System"))
            {
                Current = Platform.iOS;
            }
            Current |= ((IntPtr.Size == 4) ? Platform.X86 : Platform.X64);
        }

        public static Platform Current
        {
            get
            {
                return k__BackingField;
            }
            private set
            {
                k__BackingField = value;
            }
        }

        public static bool Is(Platform platform)
        {
            return (Current & platform) == platform;
        }

        private static Platform k__BackingField;
    }
}
namespace MonoMod.Utils
{
    public enum Platform
    {
        OS = 1,
        X86 = 0,
        X64 = 2,
        NT = 4,
        Unix = 8,
        Unknown = 17,
        Windows = 37,
        MacOS = 73,
        Linux = 137,
        Android = 393,
        iOS = 585,
        Unknown64 = 19,
        Windows64 = 39,
        MacOS64 = 75,
        Linux64 = 139,
        Android64 = 395,
        iOS64 = 587
    }
}

namespace MonoMod.Utils
{
    //[MonoMod__OldName__("MonoMod.MonoModExt")]
    public static class MonoModExt
    {
        public static ModuleDefinition ReadModule(string path, ReaderParameters rp)
        {
            ModuleDefinition result;
            for (; ; )
            {
                try
                {
                    result = ModuleDefinition.ReadModule(path, rp);
                }
                catch (Exception ex)
                {
                    if (rp.ReadSymbols)
                    {
                        rp.ReadSymbols = false;
                        continue;
                    }
                    throw;
                }
                break;
            }
            return result;
        }
        public static ModuleDefinition ReadModule(Stream input, ReaderParameters rp)
        {
            ModuleDefinition result;
            for (; ; )
            {
                try
                {
                    result = ModuleDefinition.ReadModule(input, rp);
                }
                catch
                {
                    if (rp.ReadSymbols)
                    {
                        rp.ReadSymbols = false;
                        continue;
                    }
                    throw;
                }
                break;
            }
            return result;
        }
        public static CustomAttribute GetCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
        {
            if (cap == null || !cap.HasCustomAttributes)
            {
                return null;
            }
            foreach (CustomAttribute attrib in cap.CustomAttributes)
            {
                if (attrib.AttributeType.FullName == attribute)
                {
                    return attrib;
                }
            }
            return null;
        }
        private static Hashtable GetPropertyHash(object properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                foreach (object obj in TypeDescriptor.GetProperties(properties))
                {
                    PropertyDescriptor prop = (PropertyDescriptor)obj;
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }
            return values;
        }
        public static string Inject(this string formatString, object injectionObject)
        {
            if (injectionObject is IDictionary)
            {
                return formatString.Inject(new Hashtable((IDictionary)injectionObject));
            }
            return formatString.Inject(GetPropertyHash(injectionObject));
        }
        public static bool HasCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
        {
            return cap.GetCustomAttribute(attribute) != null;
        }
        public static CustomAttribute GetMMAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
        {
            return cap.GetCustomAttribute("MonoMod.MonoMod" + attribute);
        }
        private static string GetPatchName(this Mono.Cecil.ICustomAttributeProvider cap)
        {
            CustomAttribute patchAttrib = cap.GetMMAttribute("Patch");
            if (patchAttrib != null)
            {
                return ((string)patchAttrib.ConstructorArguments[0].Value).Inject(SharedData);
            }
            string name = ((MemberReference)cap).Name;
            if (!name.StartsWith("patch_"))
            {
                return name;
            }
            return name.Substring(6);
        }
        private static string GetPatchFullName(this Mono.Cecil.ICustomAttributeProvider cap, MemberReference mr)
        {
            if (cap is TypeReference)
            {
                TypeReference type = (TypeReference)cap;
                string name = cap.GetPatchName();
                if (name.StartsWith("global::"))
                {
                    name = name.Substring(8);
                }
                else if (!name.Contains(".") && !name.Contains("/"))
                {
                    if (!string.IsNullOrEmpty(type.Namespace))
                    {
                        name = type.Namespace + "." + name;
                    }
                    else if (type.IsNested)
                    {
                        name = type.DeclaringType.GetPatchFullName() + "/" + name;
                    }
                }
                if (mr is TypeSpecification)
                {
                    List<TypeSpecification> formats = new List<TypeSpecification>();
                    TypeSpecification ts = (TypeSpecification)mr;
                    do
                    {
                        formats.Add(ts);
                    }
                    while ((ts = (ts.ElementType as TypeSpecification)) != null);
                    StringBuilder builder = new StringBuilder(name.Length + formats.Count * 4);
                    builder.Append(name);
                    for (int formati = formats.Count - 1; formati > -1; formati--)
                    {
                        ts = formats[formati];
                        if (ts.IsByReference)
                        {
                            builder.Append("&");
                        }
                        else if (ts.IsPointer)
                        {
                            builder.Append("*");
                        }
                        else if (!ts.IsPinned && !ts.IsSentinel)
                        {
                            if (ts.IsArray)
                            {
                                ArrayType array = (ArrayType)ts;
                                if (array.IsVector)
                                {
                                    builder.Append("[]");
                                }
                                else
                                {
                                    builder.Append("[");
                                    for (int i = 0; i < array.Dimensions.Count; i++)
                                    {
                                        if (i > 0)
                                        {
                                            builder.Append(",");
                                        }
                                        builder.Append(array.Dimensions[i].ToString());
                                    }
                                    builder.Append("]");
                                }
                            }
                            else if (ts.IsRequiredModifier)
                            {
                                builder.Append("modreq(").Append(((RequiredModifierType)ts).ModifierType).Append(")");
                            }
                            else if (ts.IsOptionalModifier)
                            {
                                builder.Append("modopt(").Append(((OptionalModifierType)ts).ModifierType).Append(")");
                            }
                            else if (ts.IsGenericInstance)
                            {
                                GenericInstanceType gen = (GenericInstanceType)ts;
                                builder.Append("<");
                                for (int j = 0; j < gen.GenericArguments.Count; j++)
                                {
                                    if (j > 0)
                                    {
                                        builder.Append(",");
                                    }
                                    builder.Append(gen.GenericArguments[j].GetPatchFullName());
                                }
                                builder.Append(">");
                            }
                            else
                            {
                                if (!ts.IsFunctionPointer)
                                {
                                    throw new NotSupportedException(string.Format("MonoMod can't handle TypeSpecification: {0} ({1})", type.FullName, type.GetType()));
                                }
                                FunctionPointerType fpt = (FunctionPointerType)ts;
                                builder.Append(" ").Append(fpt.ReturnType.GetPatchFullName()).Append(" *(");
                                if (fpt.HasParameters)
                                {
                                    for (int k = 0; k < fpt.Parameters.Count; k++)
                                    {
                                        ParameterDefinition parameter = fpt.Parameters[k];
                                        if (k > 0)
                                        {
                                            builder.Append(",");
                                        }
                                        if (parameter.ParameterType.IsSentinel)
                                        {
                                            builder.Append("...,");
                                        }
                                        builder.Append(parameter.ParameterType.FullName);
                                    }
                                }
                                builder.Append(")");
                            }
                        }
                    }
                    name = builder.ToString();
                }
                return name;
            }
            if (cap is FieldReference)
            {
                FieldReference field = (FieldReference)cap;
                return string.Format("{0} {1}::{2}", field.FieldType.GetPatchFullName(), field.DeclaringType.GetPatchFullName(), cap.GetPatchName());
            }
            if (cap is MethodReference)
            {
                throw new InvalidOperationException("GetPatchFullName not supported on MethodReferences - use GetFindableID instead");
            }
            throw new InvalidOperationException(string.Format("GetPatchFullName not supported on type {0}", cap.GetType()));
        }
        public static string GetPatchName(this MemberReference mr)
        {
            Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
            return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchName() : null) ?? mr.Name;
        }
        public static string GetPatchFullName(this MemberReference mr)
        {
            Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
            return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchFullName(mr) : null) ?? mr.FullName;
        }
        public static string GetFindableID(this MethodReference method, string name = null, string type = null, bool withType = true, bool simple = false)
        {
            while (method.IsGenericInstance)
            {
                method = ((GenericInstanceMethod)method).ElementMethod;
            }
            StringBuilder builder = new StringBuilder();
            if (simple)
            {
                if (withType)
                {
                    builder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
                }
                builder.Append(name ?? method.Name);
                return builder.ToString();
            }
            builder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
            if (withType)
            {
                builder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
            }
            builder.Append(name ?? method.Name);
            if (method.GenericParameters.Count != 0)
            {
                builder.Append("<");
                Collection<GenericParameter> arguments = method.GenericParameters;
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(arguments[i].Name);
                }
                builder.Append(">");
            }
            builder.Append("(");
            if (method.HasParameters)
            {
                Collection<ParameterDefinition> parameters = method.Parameters;
                for (int j = 0; j < parameters.Count; j++)
                {
                    ParameterDefinition parameter = parameters[j];
                    if (j > 0)
                    {
                        builder.Append(",");
                    }
                    if (parameter.ParameterType.IsSentinel)
                    {
                        builder.Append("...,");
                    }
                    builder.Append(parameter.ParameterType.GetPatchFullName());
                }
            }
            builder.Append(")");
            return builder.ToString();
        }
        public static IDictionary<string, object> SharedData = new Dictionary<string, object>
        {
            {
                "Platform",
                (PlatformHelper.Current & ~Platform.X64).ToString()
            },
            {
                "PlatformPrefix",
                (PlatformHelper.Current & ~Platform.X64).ToString().ToLowerInvariant() + "_"
            },
            {
                "Arch",
                (PlatformHelper.Current & Platform.X64).ToString()
            },
            {
                "Architecture",
                (PlatformHelper.Current & Platform.X64).ToString()
            },
            {
                "ArchPrefix",
                (PlatformHelper.Current & Platform.X64).ToString().ToLowerInvariant() + "_"
            },
            {
                "ArchitecturePrefix",
                (PlatformHelper.Current & Platform.X64).ToString().ToLowerInvariant() + "_"
            }
        };
        public static string GetFindableID(this MethodBase method, string name = null, string type = null, bool withType = true, bool proxyMethod = false, bool simple = false)
        {
            while (method is MethodInfo && method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                method = ((MethodInfo)method).GetGenericMethodDefinition();
            }
            StringBuilder builder = new StringBuilder();
            if (simple)
            {
                if (withType)
                {
                    builder.Append(type ?? method.DeclaringType.FullName).Append("::");
                }
                builder.Append(name ?? method.Name);
                return builder.ToString();
            }
            StringBuilder stringBuilder = builder;
            MethodInfo methodInfo = method as MethodInfo;
            string text;
            if (methodInfo == null)
            {
                text = null;
            }
            else
            {
                Type returnType = methodInfo.ReturnType;
                text = ((returnType != null) ? returnType.FullName : null);
            }
            stringBuilder.Append(text ?? "System.Void").Append(" ");
            if (withType)
            {
                builder.Append(type ?? method.DeclaringType.FullName.Replace("+", "/")).Append("::");
            }
            builder.Append(name ?? method.Name);
            if (method.ContainsGenericParameters)
            {
                builder.Append("<");
                Type[] arguments = method.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(arguments[i].Name);
                }
                builder.Append(">");
            }
            builder.Append("(");
            ParameterInfo[] parameters = method.GetParameters();
            for (int j = proxyMethod ? 1 : 0; j < parameters.Length; j++)
            {
                ParameterInfo parameter = parameters[j];
                if (j > (proxyMethod ? 1 : 0))
                {
                    builder.Append(",");
                }
                if (Attribute.IsDefined(parameter, t_ParamArrayAttribute))
                {
                    builder.Append("...,");
                }
                builder.Append(parameter.ParameterType.FullName);
            }
            builder.Append(")");
            return builder.ToString();
        }
        public static void UpdateOffsets(this Mono.Cecil.Cil.MethodBody body, int instri, int delta)
        {
            int offsi = body.Instructions.Count - 1;
            while (instri <= offsi)
            {
                body.Instructions[offsi].Offset += delta;
                offsi--;
            }
        }
        private static Type t_ParamArrayAttribute = typeof(ParamArrayAttribute);
        public static FieldDefinition FindField(this TypeDefinition type, string name)
        {
            foreach (FieldDefinition field in type.Fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return null;
        }
        public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
        {
            foreach (PropertyDefinition prop in type.Properties)
            {
                if (prop.Name == name)
                {
                    return prop;
                }
            }
            return null;
        }
        public static MethodDefinition FindMethod(this TypeDefinition type, string findableID, bool simple = true)
        {
            if (simple && !findableID.Contains(" "))
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.GetFindableID(null, null, true, true) == findableID)
                    {
                        return method;
                    }
                }
                foreach (MethodDefinition method2 in type.Methods)
                {
                    if (method2.GetFindableID(null, null, false, true) == findableID)
                    {
                        return method2;
                    }
                }
            }
            foreach (MethodDefinition method3 in type.Methods)
            {
                if (method3.GetFindableID(null, null, true, false) == findableID)
                {
                    return method3;
                }
            }
            foreach (MethodDefinition method4 in type.Methods)
            {
                if (method4.GetFindableID(null, null, false, false) == findableID)
                {
                    return method4;
                }
            }
            return null;
        }
        public static void AddAttribute(this Mono.Cecil.ICustomAttributeProvider cap, MethodReference constructor)
        {
            cap.AddAttribute(new CustomAttribute(constructor));
        }
        public static void AddAttribute(this Mono.Cecil.ICustomAttributeProvider cap, CustomAttribute attr)
        {
            cap.CustomAttributes.Add(attr);
        }
    }
}