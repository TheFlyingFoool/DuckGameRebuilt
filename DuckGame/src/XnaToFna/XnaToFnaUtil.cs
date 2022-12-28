// Decompiled with JetBrains decompiler
// Type: XnaToFna.XnaToFnaUtil
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using DuckGame;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Xml.Serialization;
using XnaToFna.ProxyForms;
//using XnaToFna.XEX;

namespace XnaToFna
{
    public class XnaToFnaUtil : IDisposable
    {
        public static ConstructorInfo m_UnverifiableCodeAttribute_ctor = typeof(UnverifiableCodeAttribute).GetConstructor(Type.EmptyTypes);
        public static ConstructorInfo m_XmlIgnore_ctor = typeof(XmlIgnoreAttribute).GetConstructor(Type.EmptyTypes);
        public static MethodInfo m_XnaToFnaHelper_PreUpdate = typeof(XnaToFnaHelper).GetMethod("PreUpdate");
        public static MethodInfo m_XnaToFnaHelper_MainHook = typeof(XnaToFnaHelper).GetMethod("MainHook");
        public static MethodInfo m_FileSystemHelper_FixPath = typeof(FileSystemHelper).GetMethod("FixPath");
        public static readonly byte[] DotNetFrameworkKeyToken = new byte[8] {
           183,
           122,
           92,
           86,
           25,
           52,
           224,
           137
        };
        public static readonly Version DotNetFramework4Version = new Version(4, 0, 0, 0);
        public static readonly Version DotNetFramework2Version = new Version(2, 0, 0, 0);
        public static readonly Version DotNetX360Version = new Version(2, 0, 5, 0);
        public static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();
        public static readonly string ThisAssemblyName = ThisAssembly.GetName().Name;
        public static readonly Version Version = ThisAssembly.GetName().Version;
        public readonly ModuleDefinition ThisModule;
        public List<XnaToFnaMapping> Mappings;
        public XnaToFnaModder Modder;
        public CustomAssemblyResolver AssemblyResolver;
        public List<string> Directories;
        public List<string> ContentDirectoryNames;
        public List<string> ContentDirectories;
        public List<ModuleDefinition> Modules;
        public Dictionary<ModuleDefinition, string> ModulePaths;
        public HashSet<string> RemoveDeps;
        public List<ModuleDefinition> ModulesToStub;
        public List<string> ExtractedXEX;
        public bool HookEntryPoint;
        public bool PatchXNB;
        public bool PatchXACT;
        public bool PatchWindowsMedia;
        public bool DestroyLocks;
        public bool FixOldMonoXML;
        public bool DestroyMixedDeps;
        public bool StubMixedDeps;
        public bool HookIsTrialMode;
        public bool HookBinaryFormatter;
        public bool HookReflection;
        public List<string> DestroyPublicKeyTokens;
        public List<string> FixPathsFor;
        public ILPlatform PreferredPlatform;
        public static Assembly Aassembly;
        public static int RemapVersion = 16;
        public void Stub(ModuleDefinition mod)
        {
            Log(string.Format("[Stub] Stubbing {0}", mod.Assembly.Name.Name));
            Modder.Module = mod;
            ApplyCommonChanges(mod, nameof(Stub));
            Log("[Stub] Mapping dependencies for MonoMod");
            Modder.MapDependencies(mod);
            Log("[Stub] Stubbing");
            foreach (TypeDefinition type in mod.Types)
                StubType(type);
            Log("[Stub] Pre-processing");
            foreach (TypeDefinition type in mod.Types)
                PreProcessType(type);
            Log("[Stub] Relinking (MonoMod PatchRefs pass)");
            Modder.PatchRefs();
            Log("[Stub] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(type);
            Log("[Stub] Rewriting and disposing module\n");
            Modder.Module.Write(Modder.WriterParameters);
            Modder.Module.Dispose();
            Modder.Module = null;
            Modder.ClearCaches(moduleSpecific: true);
        }

        public void StubType(TypeDefinition type)
        {
            foreach (FieldDefinition field in type.Fields)
                field.Attributes &= ~Mono.Cecil.FieldAttributes.HasFieldRVA;
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.HasPInvokeInfo)
                    method.PInvokeInfo = null;
                method.IsManaged = true;
                method.IsIL = true;
                method.IsNative = false;
                method.PInvokeInfo = null;
                method.IsPreserveSig = false;
                method.IsInternalCall = false;
                method.IsPInvokeImpl = false;
                Mono.Cecil.Cil.MethodBody methodBody = method.Body = new Mono.Cecil.Cil.MethodBody(method);
                methodBody.InitLocals = true;
                ILProcessor ilProcessor = methodBody.GetILProcessor();
                for (int index = 0; index < method.Parameters.Count; ++index)
                {
                    ParameterDefinition parameter = method.Parameters[index];
                    if (parameter.IsOut || parameter.IsReturnValue)
                    {
                        ilProcessor.Emit(OpCodes.Ldarg, index);
                        ilProcessor.EmitDefault(parameter.ParameterType, true);
                    }
                }
                ilProcessor.EmitDefault(method.ReturnType ?? method.Module.TypeSystem.Void);
                ilProcessor.Emit(OpCodes.Ret);
            }
            foreach (TypeDefinition nestedType in type.NestedTypes)
                StubType(nestedType);
        }

        public void SetupHelperRelinker()
        {
            Modder.RelinkMap["System.Void Microsoft.Xna.Framework.Game::.ctor()"] = new RelinkMapEntry("XnaToFna.XnaToFnaGame", "System.Void .ctor()");
            foreach (MethodInfo method in typeof(XnaToFnaGame).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                Modder.RelinkMap[method.GetFindableID(type: "Microsoft.Xna.Framework.Game")] = new RelinkMapEntry("XnaToFna.XnaToFnaGame", method.GetFindableID(withType: false));
            Modder.RelinkMap["System.IntPtr Microsoft.Xna.Framework.GameWindow::get_Handle()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IntPtr GetProxyFormHandle(Microsoft.Xna.Framework.GameWindow)");
            Modder.RelinkMap["System.Void Microsoft.Xna.Framework.GraphicsDeviceManager::ApplyChanges()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void ApplyChanges(Microsoft.Xna.Framework.GraphicsDeviceManager)");
            foreach (Type type in typeof(Form).Assembly.GetTypes())
            {
                 //else if (fullName.StartsWith("XnaToFna.ProxyDrawing."))
                 //   this.Modder.RelinkMap["System.Drawing." + fullName.Substring(22)] = fullName;
                string fullName = type.FullName;
                //if (fullName.StartsWith("XnaToFna.ProxyForms."))
                //    this.Modder.RelinkMap["System.Windows.Forms." + fullName.Substring(20)] = fullName;
                //else if (fullName.StartsWith("XnaToFna.ProxyDInput."))
                //    this.Modder.RelinkMap[fullName.Substring(21)] = fullName;
                if (fullName.StartsWith("XnaToFna.StubXDK."))
                {
                    Modder.RelinkMap["Microsoft.Xna.Framework." + fullName.Substring(17)] = fullName;
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        MonoModHook customAttribute = method.GetCustomAttribute<MonoModHook>();
                        if (customAttribute != null)
                            Modder.RelinkMap[customAttribute.FindableID] = new RelinkMapEntry(fullName, method.GetFindableID(withType: false));
                    }
                    foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        MonoModHook customAttribute = constructor.GetCustomAttribute<MonoModHook>();
                        if (customAttribute != null)
                            Modder.RelinkMap[customAttribute.FindableID] = new RelinkMapEntry(fullName, constructor.GetFindableID(withType: false));
                    }
                }
            }
            //this.Modder.RelinkMap["System.Windows.Forms.FormClosingEventHandler"] = "XnaToFna.ProxyForms.FormClosingEventHandler";
            //this.Modder.RelinkMap["System.Windows.Forms.Form"] = "XnaToFna.ProxyForms.Form";
            //this.Modder.RelinkMap["System.Windows.Forms.Control System.Windows.Forms.Control::FromHandle(System.IntPtr)"] = new RelinkMapEntry("XnaToFna.ProxyForms.Forms", "XnaToFna.ProxyForms.Forms.Control FromHandle(System.IntPtr)");
            //// this.Modder.RelinkMap["System.Windows.Forms.Control"] = "XnaToFna.ProxyForms.Control";
            //Dans thing ReadAllLines 
            Modder.RelinkMap["System.IO.DirectoryInfo System.IO.Directory::CreateDirectory(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IO.DirectoryInfo DirectoryCreateDirectory(System.String)");
            Modder.RelinkMap["System.Boolean System.IO.Directory::Exists(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Boolean DirectoryExists(System.String)");
            Modder.RelinkMap["System.String[] System.IO.Directory::GetFiles(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String[] DirectoryGetFiles(System.String)");
            Modder.RelinkMap["System.Void System.IO.Directory::Delete(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void DirectoryDelete(System.String)");

            Modder.RelinkMap["System.Void System.IO.File::SetAttributes(System.String,System.IO.FileAttributes)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileSetAttributes(System.String,System.IO.FileAttributes)");
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String,System.Text.Encoding)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileReadAllText(System.String,System.Text.Encoding)");

            Modder.RelinkMap["System.Void System.IO.File::Delete(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileDelete(System.String)");
            Modder.RelinkMap["System.Void System.IO.File::WriteAllText(System.String,System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileWriteAllText(System.String,System.String)");
            Modder.RelinkMap["System.Void System.IO.File::WriteAllLines(System.String,System.String[])"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileWriteAllLines(System.String,System.String[])");
            Modder.RelinkMap["System.String[] System.IO.File::ReadAllLines(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String[] FileReadAllLines(System.String)");
            Modder.RelinkMap["System.Boolean System.IO.File::Exists(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Boolean FileExists(System.String)");
            Modder.RelinkMap["System.Void System.IO.File::Copy(System.String,System.String,System.Boolean)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileCopy(System.String,System.String,System.Boolean)");
            Modder.RelinkMap["System.Byte[] System.IO.File::ReadAllBytes(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Byte[] FileReadAllBytes(System.String)"); //idk
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String FileReadAllText(System.String)");
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String FileReadAllText(System.String)");


            Modder.RelinkMap["System.Reflection.MethodInfo System.Type::GetMethod(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.MethodInfo GetMethod(System.Type,System.String,System.Reflection.BindingFlags)");

            Modder.RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String,System.Reflection.BindingFlags)");



            if (HookIsTrialMode)
                Modder.RelinkMap["System.Boolean Microsoft.Xna.Framework.GamerServices.Guide::get_IsTrialMode()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IntPtr get_IsTrialMode()");
            if (HookBinaryFormatter)
            {
                Modder.RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::.ctor()"] = new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Create()");
                Modder.RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::.ctor(System.Runtime.Serialization.ISurrogateSelector,System.Runtime.Serialization.StreamingContext)"] = new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Create(System.Runtime.Serialization.ISurrogateSelector,System.Runtime.Serialization.StreamingContext)");
                Modder.RelinkMap["System.Runtime.Serialization.SerializationBinder System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::get_Binder()"] = new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Runtime.Serialization.SerializationBinder get_Binder(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter)");
                Modder.RelinkMap["System.Void System.Runtime.Serialization.Formatters.Binary.BinaryFormatter::set_Binder(System.Runtime.Serialization.SerializationBinder)"] = new RelinkMapEntry("XnaToFna.BinaryFormatterHelper", "System.Void set_Binder(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter,System.Runtime.Serialization.SerializationBinder)");
            }
            Modder.RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String,System.Reflection.BindingFlags)");

            if (HookReflection)
            {
                Modder.RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String,System.Reflection.BindingFlags)");
                Modder.RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String)");
            }
            Modder.RelinkMap["System.Void System.Threading.Thread::SetProcessorAffinity(System.Int32[])"] = new RelinkMapEntry("XnaToFna.X360Helper", "System.Void SetProcessorAffinity(System.Threading.Thread,System.Int32[])");
            foreach (XnaToFnaMapping mapping in Mappings)
            {
                if (mapping.IsActive && mapping.Setup != null)
                    mapping.Setup(this, mapping);
            }
        }

        public static void SetupGSRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping) => SetupDirectRelinkMap(xtf, mapping, (_, type) =>
        {
            xtf.Modder.RelinkMap[type.FullName] = type;
            if (!type.FullName.Contains(".Net."))
                return;
            xtf.Modder.RelinkMap[type.FullName.Replace(".Net.", ".GamerServices.")] = type;
        });

        public static void SetupGSRelinkMap2(XnaToFnaUtil xtf, XnaToFnaMapping mapping) => SetupDirectRelinkMap(xtf, mapping, (_, type) =>
        {
            xtf.Modder.RelinkMap[type.FullName] = type;
            if (!type.FullName.Contains(".Net."))
                return;
            xtf.Modder.RelinkMap[type.FullName.Replace(".Net.", ".GamerServices.")] = type;
        });
        public static void SetupDirectRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping) => SetupDirectRelinkMap(xtf, mapping, null);

        public static void SetupDirectRelinkMap(
          XnaToFnaUtil xtf,
          XnaToFnaMapping mapping,
          Action<XnaToFnaUtil, TypeDefinition> action)
        {
            foreach (TypeDefinition type in mapping.Module.Types)
                SetupDirectRelinkMapType(xtf, type, action);
        }

        public static void SetupDirectRelinkMapType(
          XnaToFnaUtil xtf,
          TypeDefinition type,
          Action<XnaToFnaUtil, TypeDefinition> action)
        {
            if (action != null)
                action(xtf, type);
            else
                xtf.Modder.RelinkMap[type.FullName] = type;
            foreach (TypeDefinition nestedType in type.NestedTypes)
                SetupDirectRelinkMapType(xtf, nestedType, action);
        }

        public void PreProcessType(TypeDefinition type)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.HasPInvokeInfo && (method.PInvokeInfo.Module.Name.EndsWith("32.dll") || method.PInvokeInfo.Module.Name.EndsWith("32")))
                {
                    string str = method.PInvokeInfo.EntryPoint ?? method.Name;
                    if (typeof(PInvokeHooks).GetMethod(str) != null)
                    {
                        Log(string.Format("[PreProcess] [PInvokeHooks] Remapping call to {0} ({1})", str, method.GetFindableID()));
                        Modder.RelinkMap[method.GetFindableID(simple: true)] = new RelinkMapEntry("XnaToFna.PInvokeHooks", str);
                    }
                    else
                        Log(string.Format("[PreProcess] [PInvokeHooks] Found unhooked call to {0} ({1})", str, method.GetFindableID()));
                }
            }
            Stack<TypeDefinition> source = new Stack<TypeDefinition>();
            try
            {
                for (TypeDefinition typeDefinition = type.BaseType?.Resolve(); typeDefinition != null; typeDefinition = typeDefinition.BaseType?.Resolve())
                    source.Push(typeDefinition);
            }
            catch { }
            foreach (FieldDefinition field in type.Fields)
            {
                string name = field.Name;
                if (FixOldMonoXML && source.Any(baseType => baseType.FindField(name) != null || baseType.FindProperty(name) != null))
                {
                    Log(string.Format("[PreProcess] Renaming field name collison {0} in {1}", name, type.FullName));
                    field.Name = string.Format("{0}_{1}", name, type.Name);
                    Modder.RelinkMap[string.Format("{0}::{1}", type.FullName, name)] = field.FullName;
                }
            }
            foreach (TypeDefinition nestedType in type.NestedTypes)
                PreProcessType(nestedType);
        }

        public void PostProcessType(TypeDefinition type)
        {
            bool flag = false;
            if (type.BaseType?.FullName == "Microsoft.Xna.Framework.Game")
            {
                Log(string.Format("[PostProcess] Found type overriding Game: {0})", type.FullName));
                type.BaseType = type.Module.ImportReference(typeof(XnaToFnaGame));
                flag = true;
            }
            foreach (MethodDefinition method in type.Methods)
            {
                if (method.HasBody)
                {
                    string findableId = method.GetFindableID(withType: false);
                    if (flag && findableId == "System.Void Update(Microsoft.Xna.Framework.GameTime)")
                    {
                        Log("[PostProcess] Injecting call to XnaToFnaHelper.PreUpdate into game Update");
                        ILProcessor ilProcessor = method.Body.GetILProcessor();
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldarg_1));
                        ilProcessor.InsertAfter(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Callvirt, method.Module.ImportReference(m_XnaToFnaHelper_PreUpdate)));
                        method.Body.UpdateOffsets(1, 2);
                    }
                    for (int instri = 0; instri < method.Body.Instructions.Count; ++instri)
                    {
                        Instruction instruction = method.Body.Instructions[instri];
                        if (instruction.OpCode == OpCodes.Callvirt && ((MemberReference)instruction.Operand).DeclaringType.FullName == "XnaToFna.XnaToFnaHelper")
                            instruction.OpCode = OpCodes.Call;
                        if (DestroyLocks)
                            CheckAndDestroyLock(method, instri);
                        if (FixPathsFor.Count != 0)
                            CheckAndInjectFixPath(method, ref instri);
                    }
                    int num1 = 0;
                    for (int index = 0; index < method.Body.Instructions.Count; ++index)
                    {
                        Instruction instruction = method.Body.Instructions[index];
                        instruction.Offset = num1;
                        num1 += instruction.GetSize();
                    }
                    for (int index = 0; index < method.Body.Instructions.Count; ++index)
                    {
                        Instruction instruction = method.Body.Instructions[index];
                        if (instruction.Operand is Instruction)
                        {
                            int num2 = ((Instruction)instruction.Operand).Offset - instruction.Offset;
                            instruction.OpCode = num2 <= -127 || num2 >= sbyte.MaxValue ? ShortToLongOp(instruction.OpCode) : LongToShortOp(instruction.OpCode);
                        }
                    }
                }
            }
            foreach (TypeDefinition nestedType in type.NestedTypes)
                PostProcessType(nestedType);
        }

        public OpCode ShortToLongOp(OpCode op)
        {
            string name = Enum.GetName(typeof(Code), op.Code);
            return !name.EndsWith("_S") ? op : (OpCode?)typeof(OpCodes).GetField(name.Substring(0, name.Length - 2))?.GetValue(null) ?? op;
        }

        public OpCode LongToShortOp(OpCode op)
        {
            string name = Enum.GetName(typeof(Code), op.Code);
            return name.EndsWith("_S") ? op : (OpCode?)typeof(OpCodes).GetField(name + "_S")?.GetValue(null) ?? op;
        }

        public void CheckAndDestroyLock(MethodDefinition method, int instri)
        {
            Instruction instruction = method.Body.Instructions[instri];
            if (instri >= 1 && (instruction.OpCode == OpCodes.Brfalse || instruction.OpCode == OpCodes.Brfalse_S || instruction.OpCode == OpCodes.Brtrue || instruction.OpCode == OpCodes.Brtrue_S) && ((Instruction)instruction.Operand).Offset < instruction.Offset && instruction.Previous.Operand != null)
            {
                string lowerInvariant = ((instruction.Previous.Operand is FieldReference operand1 ? operand1.Name : null) ?? (instruction.Previous.Operand is MethodReference operand2 ? operand2.Name : null) ?? instruction.Previous.Operand.ToString()).ToLowerInvariant();
                if (instri - method.Body.Instructions.IndexOf((Instruction)instruction.Operand) <= 3 && lowerInvariant != null && (lowerInvariant.Contains("load") || lowerInvariant.Contains("content")))
                {
                    Log(string.Format("[PostProcess] [HACK!!!] NOPing possible content loading waiting loop in {0}", method.GetFindableID()));
                    OpCode? opCode1 = instruction.Previous?.Previous.OpCode;
                    OpCode opCode2 = OpCodes.Volatile;
                    if ((opCode1.HasValue ? (opCode1.HasValue ? (opCode1.GetValueOrDefault() == opCode2 ? 1 : 0) : 1) : 0) != 0)
                        instruction.Previous.Previous.OpCode = OpCodes.Nop;
                    instruction.Previous.OpCode = OpCodes.Nop;
                    instruction.Previous.Operand = null;
                    instruction.OpCode = OpCodes.Nop;
                    instruction.Operand = null;
                }
            }
            if (instri < 3 || !(instruction.OpCode == OpCodes.Call) || !(((MethodReference)instruction.Operand).GetFindableID() == "System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)"))
                return;
            if (method.DeclaringType.FullName.ToLowerInvariant().Contains("content") || method.Name.ToLowerInvariant().Contains("load") || method.Name.ToLowerInvariant().Contains("content"))
            {
                Log(string.Format("[PostProcess] [HACK!!!] Destroying possible content loading lock in {0}", method.GetFindableID()));
                DestroyMonitorLock(method, instri);
            }
            else
            {
                for (int index = instri; 0 < index && instri - 4 <= index; --index)
                {
                    string lowerInvariant = ((method.Body.Instructions[index].Operand is FieldReference operand3 ? operand3.Name : null) ?? (method.Body.Instructions[index].Operand is MethodReference operand4 ? operand4.Name : null) ?? method.Body.Instructions[index].Operand?.ToString())?.ToLowerInvariant();
                    if (lowerInvariant != null && (lowerInvariant.Contains("load") || lowerInvariant.Contains("content")))
                    {
                        Log(string.Format("[PostProcess] [HACK!!!] Destroying possible content loading lock in {0}", method.GetFindableID()));
                        DestroyMonitorLock(method, instri);
                        break;
                    }
                }
            }
        }

        public void DestroyMonitorLock(MethodDefinition method, int instri)
        {
            Instruction instruction = method.Body.Instructions[instri];
            instruction.Operand = Modder.Module.ImportReference(Modder.FindTypeDeep("XnaToFna.FakeMonitor").Resolve().FindMethod("System.Void Enter(System.Object,System.Boolean&)"));
            int num;
            for (num = 1; instri < method.Body.Instructions.Count && num > 0; ++instri)
            {
                instruction = method.Body.Instructions[instri];
                if (instruction.OpCode == OpCodes.Call)
                {
                    string findableId = ((MethodReference)instruction.Operand).GetFindableID();
                    if (findableId == "System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)")
                        ++num;
                    else if (findableId == "System.Void System.Threading.Monitor::Exit(System.Object)")
                        --num;
                }
            }
            if (num != 0)
                return;
            instruction.Operand = Modder.Module.ImportReference(Modder.FindTypeDeep("XnaToFna.FakeMonitor").Resolve().FindMethod("System.Void Exit(System.Object)"));
        }

        public void CheckAndInjectFixPath(MethodDefinition method, ref int instri)
        {
            Collection<Instruction> instructions = method.Body.Instructions;
            Instruction instruction = instructions[instri];
            if (instruction.OpCode != OpCodes.Call && instruction.OpCode != OpCodes.Callvirt && instruction.OpCode != OpCodes.Newobj)
                return;
            MethodReference operand = (MethodReference)instruction.Operand;
            if (!FixPathsFor.Contains(operand.GetFindableID()) && !FixPathsFor.Contains(operand.GetFindableID(withType: false)) && !FixPathsFor.Contains(operand.GetFindableID(simple: true)) && !FixPathsFor.Contains(operand.GetFindableID(withType: false, simple: true)))
                return;
            MethodReference method1 = method.Module.ImportReference(StackOpHelper.m_Push);
            MethodReference method2 = method.Module.ImportReference(StackOpHelper.m_Pop);
            MethodReference method3 = method.Module.ImportReference(m_FileSystemHelper_FixPath);
            ILProcessor ilProcessor = method.Body.GetILProcessor();
            for (int index = operand.Parameters.Count - 1; index > -1; --index)
            {
                if (operand.Parameters[index].ParameterType.MetadataType == MetadataType.String)
                {
                    ilProcessor.InsertBefore(instructions[instri], ilProcessor.Create(OpCodes.Call, method3));
                    ++instri;
                }
                ilProcessor.InsertBefore(instructions[instri], ilProcessor.Create(OpCodes.Call, new GenericInstanceMethod(method1)
                {
                    GenericArguments = {
            operand.Parameters[index].ParameterType
          }
                }));
                ++instri;
            }
            for (int index = 0; index < operand.Parameters.Count; ++index)
            {
                ilProcessor.InsertBefore(instructions[instri], ilProcessor.Create(OpCodes.Call, new GenericInstanceMethod(method2)
                {
                    GenericArguments = {
            operand.Parameters[index].ParameterType
          }
                }));
                ++instri;
            }
        }
        public static AssemblyDefinition GetAssembly(IAssemblyResolver assemblyResolver, string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
            {
                parameters.AssemblyResolver = assemblyResolver;
            }
            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }

        public XnaToFnaUtil()
        {
            Mappings = new List<XnaToFnaMapping>() {
            new XnaToFnaMapping("System", new string[1] {
                "System.Net"
          }),
          new XnaToFnaMapping("FNA", new string[9] {
            "Microsoft.Xna.Framework",
            "Microsoft.Xna.Framework.Avatar",
            "Microsoft.Xna.Framework.Content.Pipeline",
            "Microsoft.Xna.Framework.Game",
            "Microsoft.Xna.Framework.Graphics",
            "Microsoft.Xna.Framework.Input.Touch",
            "Microsoft.Xna.Framework.Storage",
            "Microsoft.Xna.Framework.Video",
            "Microsoft.Xna.Framework.Xact"
          }),
          new XnaToFnaMapping("MonoGame.Framework.Net", new string[3] {
            "Microsoft.Xna.Framework.GamerServices",
            "Microsoft.Xna.Framework.Net",
            "Microsoft.Xna.Framework.Xdk"
          }, new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap)),
          new XnaToFnaMapping("FNA.Steamworks", new string[4] {
            "FNA.Steamworks",
            "Microsoft.Xna.Framework.GamerServices",
            "Microsoft.Xna.Framework.Net",
            "Microsoft.Xna.Framework.Xdk"
          }, new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap)),
          new XnaToFnaMapping("Steam", new string[2] {
            "Steam",
            "DGSteam"

          }, new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap2))
      };
            AssemblyResolver = new CustomAssemblyResolver();
            Directories = new List<string>();
            ContentDirectoryNames = new List<string>() {
                "Content"
            };
            ContentDirectories = new List<string>();
            Modules = new List<ModuleDefinition>();
            ModulePaths = new Dictionary<ModuleDefinition, string>();
            RemoveDeps = new HashSet<string>() 
            {
                null,
                "",
                "Microsoft.DirectX.DirectInput",
                "Microsoft.VisualC"
            };
            ModulesToStub = new List<ModuleDefinition>();
            ExtractedXEX = new List<string>();
            HookEntryPoint = true;
            PatchXNB = true;
            PatchXACT = true;
            PatchWindowsMedia = true;
            DestroyLocks = true;
            StubMixedDeps = true;
            HookBinaryFormatter = true;
            HookReflection = true;
            DestroyPublicKeyTokens = new List<string>();
            FixPathsFor = new List<string>();
            PreferredPlatform = ILPlatform.AnyCPU;
            Modder = new XnaToFnaModder(this);
            string path = Program.FilePath;

            Modder.ReadingMode = ReadingMode.Immediate;
            Modder.Strict = false;
            Modder.AssemblyResolver = AssemblyResolver;
            Modder.DependencyDirs = Directories;
            Modder.MissingDependencyResolver = new MissingDependencyResolver(MissingDependencyResolver);

            using (FileStream fileStream = new FileStream(Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
            {
                ThisModule = MonoModExt.ReadModule(fileStream, new ReaderParameters(ReadingMode.Immediate));
            }
            Modder.DependencyCache[ThisModule.Assembly.Name.Name] = ThisModule;
            Modder.DependencyCache[ThisModule.Assembly.Name.FullName] = ThisModule;
            //this.Modder.DependencyCache.Add("DuckGame", MonoModExt.ReadModule(path, this.Modder.GenReaderParameters(false, path)));
        }
        public string modpath = "";
        public XnaToFnaUtil(string[] paths) : this()
        {
            //this.ScanPaths(paths);
        }
        public XnaToFnaUtil(string path) : this()
        {
            CustomAssemblyResolver.searchdirectorpath = path;
           // this.ScanPaths(path);
        }
        public void Log(string txt)
        {
            Console.Write("[XnaToFna] ");
            Console.WriteLine(txt);
        }

        public ModuleDefinition MissingDependencyResolver(
          MonoModder modder,
          ModuleDefinition main,
          string name,
          string fullName)
        {
            Modder.Log(string.Format("Cannot map dependency {0} -> (({1}), ({2})) - not found", main.Name, fullName, name));
            return null;
        }

        public void ScanPaths(params string[] paths)
        {
            foreach (string path in paths)
                ScanPath(path);
        }

        public void ScanPath(string path)
        {
            if (Directory.Exists(path))
            {
                if (Directories.Contains(path))
                    return;
                RestoreBackup(path);
                Log(string.Format("[ScanPath] Scanning directory {0}", path));
                Directories.Add(path);
                AssemblyResolver.AddSearchDirectory(path);
                foreach (string contentDirectoryName in ContentDirectoryNames)
                {
                    string str1;
                    if (Directory.Exists(str1 = Path.Combine(path, contentDirectoryName)))
                    {
                        if (ContentDirectories.Count == 0)
                        {
                            string str2 = Path.Combine(path, Path.GetFileName(ThisAssembly.Location));
                            if (Path.GetDirectoryName(ThisAssembly.Location) != path)
                            {
                                Log("[ScanPath] Found separate game directory - copying XnaToFna.exe and FNA.dll");
                                File.Copy(ThisAssembly.Location, str2, true);
                                string extension = null;
                                if (File.Exists(Path.ChangeExtension(ThisAssembly.Location, "pdb")))
                                    extension = "pdb";
                                if (File.Exists(Path.ChangeExtension(ThisAssembly.Location, "mdb")))
                                    extension = "mdb";
                                if (extension != null)
                                    File.Copy(Path.ChangeExtension(ThisAssembly.Location, extension), Path.ChangeExtension(str2, extension), true);
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll")))
                                    File.Copy(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll"), Path.Combine(path, "FNA.dll"), true);
                                else if (File.Exists(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll.tmp")))
                                    File.Copy(Path.Combine(Path.GetDirectoryName(ThisAssembly.Location), "FNA.dll.tmp"), Path.Combine(path, "FNA.dll"), true);
                            }
                        }
                        Log(string.Format("[ScanPath] Found Content directory: {0}", str1));
                        ContentDirectories.Add(str1);
                    }
                }
                ScanPaths(Directory.GetFiles(path));
            }
            else if (File.Exists(path + ".xex"))
            {
                if (ExtractedXEX.Contains(path))
                    return;
                File.Delete(path);
            }
            else
            {
                //if (path.EndsWith(".xex"))
                //{
                //  string path1 = path.Substring(0, path.Length - 4);
                //  if (string.IsNullOrEmpty(Path.GetExtension(path1)))
                //    return;
                //  using (Stream input1 = (Stream) File.OpenRead(path))
                //  {
                //    using (BinaryReader reader = new BinaryReader(input1))
                //    {
                //      using (Stream stream = (Stream) File.OpenWrite(path1))
                //      {
                //        XEXImageData xexImageData = new XEXImageData(reader);
                //        int offset = 0;
                //        int count = xexImageData.m_memorySize;
                //        if (xexImageData.m_memorySize > 65536)
                //        {
                //          using (MemoryStream input2 = new MemoryStream(xexImageData.m_memoryData))
                //          {
                //            using (BinaryReader binaryReader = new BinaryReader((Stream) input2))
                //            {
                //              if (binaryReader.ReadUInt32() == 9460301U)
                //              {
                //                input2.Seek(640L, SeekOrigin.Begin);
                //                if (binaryReader.ReadUInt64() == 107152478071086UL)
                //                {
                //                  input2.Seek(648L, SeekOrigin.Begin);
                //                  binaryReader.ReadInt32();
                //                  offset = binaryReader.ReadInt32();
                //                  count = xexImageData.m_memorySize - offset;
                //                }
                //              }
                //            }
                //          }
                //        }
                //        stream.Write(xexImageData.m_memoryData, offset, count);
                //      }
                //    }
                //  }
                //  path = path1;
                //  this.ExtractedXEX.Add(path1);
                //}
                if (!path.EndsWith(".dll") && !path.EndsWith(".exe"))
                    return;
                AssemblyName name;
                try
                {
                    name = AssemblyName.GetAssemblyName(path);
                }
                catch
                {
                    return;
                }
                ReaderParameters rp = Modder.GenReaderParameters(false);
                rp.ReadWrite = path != ThisAssembly.Location && !Mappings.Exists(mappings => name.Name == mappings.Target);
                if (!File.Exists(path + ".mdb") && !File.Exists(Path.ChangeExtension(path, "pdb")))
                    rp.ReadSymbols = false;
                Log(string.Format("[ScanPath] Checking assembly {0} ({1})", name.Name, rp.ReadWrite ?
                  "rw" : (object)
                  "r-"));
                ModuleDefinition key;
                try
                {
                    key = MonoModExt.ReadModule(path, rp);
                }
                catch (Exception ex)
                {
                    Log(string.Format("[ScanPath] WARNING: Cannot load assembly: {0}", ex));
                    return;
                }
                bool flag = !rp.ReadWrite || name.Name == ThisAssemblyName;
                if ((key.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly)
                {
                    Log(string.Format("[ScanPath] WARNING: Cannot handle mixed mode assembly {0}", name.Name));
                    if (StubMixedDeps)
                    {
                        ModulesToStub.Add(key);
                        flag = true;
                    }
                    else
                    {
                        if (DestroyMixedDeps)
                            RemoveDeps.Add(name.Name);
                        key.Dispose();
                        return;
                    }
                }
                if (flag && !rp.ReadWrite)
                {
                    foreach (XnaToFnaMapping mapping in Mappings)
                    {
                        if (name.Name == mapping.Target)
                        {
                            mapping.IsActive = true;
                            mapping.Module = key;
                            foreach (string source in mapping.Sources)
                            {
                                Log(string.Format("[ScanPath] Mapping {0} -> {1}", source, name.Name));
                                Modder.RelinkModuleMap[source] = key;
                            }
                        }
                    }
                }
                else if (!flag)
                {
                    foreach (XnaToFnaMapping mapping1 in Mappings)
                    {
                        XnaToFnaMapping mapping = mapping1;
                        if (key.AssemblyReferences.Any(dep => mapping.Sources.Contains(dep.Name)))
                        {
                            flag = true;
                            Log(string.Format("[ScanPath] XnaToFna-ing {0}", name.Name));
                            break;
                        }
                    }
                }
                if (flag)
                {
                    Modules.Add(key);
                    ModulePaths[key] = path;
                }
                else
                    key.Dispose();
            }
        }

        public void RestoreBackup(string root)
        {
            string str = Path.Combine(root, "orig");
            if (!Directory.Exists(str))
                return;
            RestoreBackup(root, str);
        }

        public void RestoreBackup(string root, string origRoot)
        {
            Log(string.Format("[RestoreBackup] Restoring from {0} to {1}", origRoot, root));
            foreach (string enumerateFile in Directory.EnumerateFiles(origRoot, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(root + enumerateFile.Substring(origRoot.Length)));
                File.Copy(enumerateFile, root + enumerateFile.Substring(origRoot.Length), true);
            }
        }

        public void OrderModules()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            fuller fuller = new fuller();
            List<ModuleDefinition> moduleDefinitionList = new List<ModuleDefinition>(Modules);
            Log("[OrderModules] Unordered: ");
            for (int index = 0; index < Modules.Count; ++index)
                Log(string.Format("[OrderModules] #{0}: {1}", index + 1, Modules[index].Assembly.Name.Name));
            // ISSUE: reference to a compiler-generated field
            fuller.dep = null;
            foreach (ModuleDefinition module in Modules)
            {
                foreach (AssemblyNameReference assemblyReference in module.AssemblyReferences)
                {
                    // ISSUE: object of a compiler-generated type is created
                    // ISSUE: variable of a compiler-generated type
                    filler2 filler2 = new filler2
                    {
                        // ISSUE: reference to a compiler-generated field
                        filler = fuller,
                        // ISSUE: reference to a compiler-generated field
                        depName = assemblyReference
                    };
                    // ISSUE: reference to a compiler-generated method
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    if (Modules.Exists(new Predicate<ModuleDefinition>(filler2.OrderModules)) && moduleDefinitionList.IndexOf(filler2.filler.dep) > moduleDefinitionList.IndexOf(module))
                    {
                        // ISSUE: reference to a compiler-generated field
                        // ISSUE: reference to a compiler-generated field
                        Log(string.Format("[OrderModules] Reordering {0} dependency {1}", module.Assembly.Name.Name, filler2.filler.dep.Name));
                        moduleDefinitionList.Remove(module);
                        // ISSUE: reference to a compiler-generated field
                        // ISSUE: reference to a compiler-generated field
                        moduleDefinitionList.Insert(moduleDefinitionList.IndexOf(filler2.filler.dep) + 1, module);
                    }
                }
            }
            Modules = moduleDefinitionList;
            Log("[OrderModules] Reordered: ");
            for (int index = 0; index < Modules.Count; ++index)
                Log(string.Format("[OrderModules] #{0}: {1}", index + 1, Modules[index].Assembly.Name.Name));
        }

        public void RelinkAll()
        {
            SetupHelperRelinker();
            foreach (ModuleDefinition module in Modules)
                Modder.DependencyCache[module.Assembly.Name.Name] = module;
            foreach (ModuleDefinition mod in ModulesToStub)
                Stub(mod);
            foreach (ModuleDefinition module in Modules)
                Relink(module);
        }

        public void Relink(ModuleDefinition mod)
        {
            if (Mappings.Exists(new Predicate<XnaToFnaMapping>(mm => mod.Assembly.Name.Name == mm.Target)) || ModulesToStub.Contains(mod) || mod.Assembly.Name.Name == ThisAssemblyName)
                return;
            Log(string.Format("[Relink] Relinking {0}", mod.Assembly.Name.Name));
            Modder.Module = mod;
            ApplyCommonChanges(mod);
            Log("[Relink] Pre-processing");
            foreach (TypeDefinition type in mod.Types)
                PreProcessType(type);
            Log("[Relink] Relinking (MonoMod PatchRefs pass)");
            Modder.PatchRefs();
            Log("[Relink] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(type);
            if (HookEntryPoint && mod.EntryPoint != null)
            {
                Log("[Relink] Injecting XnaToFna entry point hook");
                ILProcessor ilProcessor = mod.EntryPoint.Body.GetILProcessor();
                Instruction instruction = ilProcessor.Create(OpCodes.Call, mod.ImportReference(m_XnaToFnaHelper_MainHook));
                ilProcessor.InsertBefore(mod.EntryPoint.Body.Instructions[0], instruction);
                ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldarg_0));
            }
            Log("[Relink] Rewriting and disposing module\n");
        }

        public void ApplyCommonChanges(ModuleDefinition mod, string tag = "Relink")
        {
            if (DestroyPublicKeyTokens.Contains(mod.Assembly.Name.Name))
            {
                Log(string.Format("[{0}] Destroying public key token for module {1}", tag, mod.Assembly.Name.Name));
                mod.Assembly.Name.PublicKeyToken = new byte[0];
            }
            Log(string.Format("[{0}] Updating dependencies", tag));
        label_20:
            for (int index = 0; index < mod.AssemblyReferences.Count; ++index)
            {
                AssemblyNameReference dep = mod.AssemblyReferences[index];
                foreach (XnaToFnaMapping mapping1 in Mappings)
                {
                    XnaToFnaMapping mapping = mapping1;
                    if (mapping.Sources.Contains(dep.Name) && Modder.DependencyCache.ContainsKey(mapping.Target))
                    {
                        if (mod.AssemblyReferences.Any(existingDep => existingDep.Name == mapping.Target))
                        {
                            mod.AssemblyReferences.RemoveAt(index);
                            --index;
                            goto label_20;
                        }
                        else
                        {
                            Log(string.Format("[{0}] Replacing dependency {1} -> {2}", tag, dep.Name, mapping.Target));
                            mod.AssemblyReferences[index] = Modder.DependencyCache[mapping.Target].Assembly.Name;
                            goto label_20;
                        }
                    }
                }
                if (RemoveDeps.Contains(dep.Name))
                {
                    Log(string.Format("[{0}] Removing unwanted dependency {1}", tag, dep.Name));
                    mod.AssemblyReferences.RemoveAt(index);
                    --index;
                }
                else
                {
                    if (DestroyPublicKeyTokens.Contains(dep.Name))
                    {
                        Log(string.Format("[{0}] Destroying public key token for dependency {1}", tag, dep.Name));
                        dep.PublicKeyToken = new byte[0];
                    }
                    if (ModulesToStub.Any(stub => stub.Assembly.Name.Name == dep.Name))
                    {
                        Log(string.Format("[{0}] Fixing stubbed dependency {1}", tag, dep.Name));
                        dep.IsWindowsRuntime = false;
                        dep.HasPublicKey = false;
                    }
                    if (dep.Version == DotNetX360Version)
                    {
                        dep.PublicKeyToken = DotNetFrameworkKeyToken;
                        dep.Version = DotNetFramework4Version;
                    }
                }
            }
            if (!mod.AssemblyReferences.Any(dep => dep.Name == ThisAssemblyName))
            {
                Log(string.Format("[{0}] Adding dependency XnaToFna", tag));
                mod.AssemblyReferences.Add(Modder.DependencyCache[ThisAssemblyName].Assembly.Name);
            }
            if (mod.Runtime < TargetRuntime.Net_4_0)
                mod.Runtime = TargetRuntime.Net_4_0;
            Log(string.Format("[{0}] Updating module attributes", tag));
            mod.Attributes &= ~ModuleAttributes.StrongNameSigned;
            if (PreferredPlatform != ILPlatform.Keep)
            {
                mod.Architecture = TargetArchitecture.I386;
                mod.Attributes &= ~(ModuleAttributes.Required32Bit | ModuleAttributes.Preferred32Bit);
                switch (PreferredPlatform)
                {
                    case ILPlatform.x86:
                        mod.Architecture = TargetArchitecture.I386;
                        mod.Attributes |= ModuleAttributes.Required32Bit;
                        break;
                    case ILPlatform.x64:
                        mod.Architecture = TargetArchitecture.AMD64;
                        break;
                    case ILPlatform.x86Pref:
                        mod.Architecture = TargetArchitecture.I386;
                        mod.Attributes |= ModuleAttributes.Preferred32Bit;
                        break;
                }
            }
            if (ModulesToStub.Count != 0 | (mod.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly)
            {
                Log(string.Format("[{0}] Making assembly unsafe", tag));
                mod.Attributes |= ModuleAttributes.ILOnly;
                for (int index = 0; index < mod.Assembly.CustomAttributes.Count; ++index)
                {
                    if (mod.Assembly.CustomAttributes[index].AttributeType.FullName == "System.CLSCompliantAttribute")
                    {
                        mod.Assembly.CustomAttributes.RemoveAt(index);
                        --index;
                    }
                }
                if (!mod.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Security.UnverifiableCodeAttribute"))
                    mod.AddAttribute(mod.ImportReference(m_UnverifiableCodeAttribute_ctor));
            }
            Log(string.Format("[{0}] Mapping dependencies for MonoMod", tag));
            Modder.MapDependencies(mod);
        }

        public void LoadModules()
        {
            foreach (ModuleDefinition module in Modules)
            {
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type
                ugh ugh = new ugh
                {
                    // ISSUE: reference to a compiler-generated field
                    mod = module
                };
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type
                thing thing = new thing();
                // ISSUE: reference to a compiler-generated method
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                if (!Mappings.Exists(new Predicate<XnaToFnaMapping>(ugh.ifnamenot)) && !(ugh.mod.Assembly.Name.Name == ThisAssemblyName) && !ModulesToStub.Contains(ugh.mod))
                {
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    thing.asm = Assembly.LoadFile(ModulePaths[ugh.mod]);
                    // ISSUE: reference to a compiler-generated method
                    AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(thing.TypeResolve);
                    // ISSUE: reference to a compiler-generated method
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(thing.AssemblyResolve);
                }
            }
        }

        //public void UpdateContent()
        //{
        //  if (this.ContentDirectories.Count == 0)
        //  {
        //    this.Log("[UpdateContent] No content directory found!");
        //  }
        //  else
        //  {
        //    using (ContentHelper.Game = new ContentHelperGame())
        //    {
        //      ContentHelper.Game.ActionQueue.Enqueue((Action) (() =>
        //      {
        //        foreach (string contentDirectory in this.ContentDirectories)
        //        {
        //          foreach (string enumerateFile in Directory.EnumerateFiles(contentDirectory, "*", SearchOption.AllDirectories))
        //            ContentHelper.UpdateContent(enumerateFile, this.PatchXNB, this.PatchXACT, this.PatchWindowsMedia);
        //        }
        //      }));
        //      ContentHelper.Game.Run();
        //    }
        //    ContentHelper.Game = (ContentHelperGame) null;
        //  }
        //}

        public void Dispose()
        {
            Modder?.Dispose();
            foreach (ModuleDefinition module in Modules)
                module.Dispose();
            Modules.Clear();
            ModulesToStub.Clear();
            Directories.Clear();
        }

        public Assembly RelinkToAssemblyInMemory(ModuleDefinition mod)
        {
            if (Mappings.Exists(new Predicate<XnaToFnaMapping>(mappings => mod.Assembly.Name.Name == mappings.Target)))
                return null;
            if (ModulesToStub.Contains(mod))
                return null;
            if (mod.Assembly.Name.Name == ThisAssemblyName)
                return null;
            Log(string.Format("[Relink] Relinking {0}", mod.Assembly.Name.Name));
            Modder.Module = mod;
            ApplyCommonChanges(mod);
            Log("[Relink] Pre-processing");
            foreach (TypeDefinition type in mod.Types)
                PreProcessType(type);
            Log("[Relink] Relinking (MonoMod PatchRefs pass)");
            Modder.PatchRefs();
            Log("[Relink] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(type);
            if (HookEntryPoint && mod.EntryPoint != null)
            {
                Log("[Relink] Injecting XnaToFna entry point hook");
                ILProcessor ilProcessor = mod.EntryPoint.Body.GetILProcessor();
                Instruction instruction = ilProcessor.Create(OpCodes.Call, mod.ImportReference(m_XnaToFnaHelper_MainHook));
                ilProcessor.InsertBefore(mod.EntryPoint.Body.Instructions[0], instruction);
                ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldarg_0));
            }
            Log("[Relink] Rewriting and disposing module\n");
            byte[] assemblybytes;
            using (MemoryStream memStream = new MemoryStream())
            {
                FieldInfo ImageField = typeof(ModuleDefinition).GetField("Image", BindingFlags.NonPublic | BindingFlags.Instance);
                Modder.Module.Write(memStream, Modder.WriterParameters, Modder.Module.Image.Stream.value.GetFileName());//this.Modder.Module.Image.Stream.value.GetFileName()
                //mod.Assembly.MainModule.Name
                assemblybytes = memStream.ToArray();
            }

            //DuckGame.Program.GameDirectory + mod.Assembly.MainModule.Name
            //XnaToFnaUtil.Aassembly = 
            // For checking the changed assembly File.WriteAllBytes(DuckGame.Program.GameDirectory + mod.Assembly.MainModule.Name, assemblybytes);
            //File.WriteAllBytes("C:\\Users\\daniel\\Desktop\\Release\\demo\\QolMod2.dll", (byte[])assembly.GetType().GetMethod("GetRawBytes", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)assembly, (object[])null));
            return Assembly.Load(assemblybytes);
        }
        public Assembly RelinkToAssemblyToFile(ModuleDefinition mod, string path)
        {
            if (Mappings.Exists(new Predicate<XnaToFnaMapping>(mappings => mod.Assembly.Name.Name == mappings.Target)))
                return null;
            if (ModulesToStub.Contains(mod))
                return null;
            if (mod.Assembly.Name.Name == ThisAssemblyName)
                return null;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Log(string.Format("[Relink] Relinking {0}", mod.Assembly.Name.Name));
            Modder.Module = mod;
            ApplyCommonChanges(mod);
            Log("[Relink] Pre-processing");
            foreach (TypeDefinition type in mod.Types)
                PreProcessType(type);
            Log("[Relink] Relinking (MonoMod PatchRefs pass)");
            Modder.PatchRefs();
            Log("[Relink] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(type);
            if (HookEntryPoint && mod.EntryPoint != null)
            {
                Log("[Relink] Injecting XnaToFna entry point hook");
                ILProcessor ilProcessor = mod.EntryPoint.Body.GetILProcessor();
                Instruction instruction = ilProcessor.Create(OpCodes.Call, mod.ImportReference(m_XnaToFnaHelper_MainHook));
                ilProcessor.InsertBefore(mod.EntryPoint.Body.Instructions[0], instruction);
                ilProcessor.InsertBefore(instruction, ilProcessor.Create(OpCodes.Ldarg_0));
            }
            Log("[Relink] Rewriting and disposing module\n");
            byte[] assemblybytes;
            using (FileStream fs = File.Create(path))
            {
                //FieldInfo ImageField = typeof(ModuleDefinition).GetField("Image", BindingFlags.NonPublic | BindingFlags.Instance);
                Modder.Module.Write(fs, Modder.WriterParameters);//this.Modder.Module.Image.Stream.value.GetFileName()
                //mod.Assembly.MainModule.Name
                //assemblybytes = memStream.ToArray();
            }

            //DuckGame.Program.GameDirectory + mod.Assembly.MainModule.Name
            //XnaToFnaUtil.Aassembly = 
            // For checking the changed assembly File.WriteAllBytes(DuckGame.Program.GameDirectory + mod.Assembly.MainModule.Name, assemblybytes);
            //File.WriteAllBytes("C:\\Users\\daniel\\Desktop\\Release\\demo\\QolMod2.dll", (byte[])assembly.GetType().GetMethod("GetRawBytes", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)assembly, (object[])null));
            return Assembly.LoadFile(path);
        }
        //public static void SetupDirectRelinkMap(
        //  XnaToFnaUtil xtf,
        //  XnaToFnaMapping mapping,
        //  Action<XnaToFnaUtil, TypeDefinition> action)
        //{
        //}

        //public static void SetupDirectRelinkMapType(
        //  XnaToFnaUtil xtf,
        //  TypeDefinition type,
        //  Action<XnaToFnaUtil, TypeDefinition> action)
        //{
        //}
        private sealed class fuller
        {
            public fuller() { }

            public ModuleDefinition dep;
        }
        private sealed class filler2
        {
            public filler2() { }

            internal bool OrderModules(ModuleDefinition other)
            {
                filler.dep = other;
                return other.Assembly.Name.Name == depName.Name;
            }

            public AssemblyNameReference depName;

            public fuller filler;
        }
        private sealed class thing
        {
            public thing() { }

            internal Assembly TypeResolve(object sender, ResolveEventArgs args)
            {
                if (!(asm.GetType(args.Name) != null))
                {
                    DevConsole.Log("fckk122kty" + args.Name + "fckk122kty");
                    return null;
                }
                return asm;
            }

            internal Assembly AssemblyResolve(object sender, ResolveEventArgs args)
            {
                if (!(args.Name == asm.FullName) && !(args.Name == asm.GetName().Name))
                {
                    DevConsole.Log("fckk122k" + args.Name + "fckk122k");
                    return null;
                }
                return asm;
            }

            public Assembly asm;
        }
        private sealed class ugh
        {
            public ugh() { }

            internal bool ifnamenot(XnaToFnaMapping mappings)
            {
                return mod.Assembly.Name.Name == mappings.Target;
            }

            public ModuleDefinition mod;
        }
        private sealed class N
        {
            public ModuleDefinition mod;

            internal bool Relinkthing(XnaToFnaMapping mappings) => mod.Assembly.Name.Name == mappings.Target;
        }
    }

}