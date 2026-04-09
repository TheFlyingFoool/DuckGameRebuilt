using DuckGame;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod;
using MonoMod.Utils;
using src.XnaToFna;
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
        public static readonly byte[] DotNetFrameworkKeyToken = new byte[8]
        {
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
        public static int RemapVersion = 37;
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
                PostProcessType(mod, type);
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
        //public static List<Instruction> JamTranspile(List<Instruction> instructions) example
        //{
        //    return new List<Instruction>() { new Instruction(OpCodes.Ret, null) };
        //}
        public void SetupHelperRelinker()
        {
            Modder.RelinkMap["System.Void Microsoft.Xna.Framework.Game::.ctor()"] = new RelinkMapEntry("XnaToFna.XnaToFnaGame", "System.Void .ctor()");
            foreach (MethodInfo method in typeof(XnaToFnaGame).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                Modder.RelinkMap[method.GetFindableID(type: "Microsoft.Xna.Framework.Game")] = new RelinkMapEntry("XnaToFna.XnaToFnaGame", method.GetFindableID(withType: false));
            Modder.RelinkMap["System.IntPtr Microsoft.Xna.Framework.GameWindow::get_Handle()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IntPtr GetProxyFormHandle(Microsoft.Xna.Framework.GameWindow)");
            Modder.RelinkMap["System.Void Microsoft.Xna.Framework.GraphicsDeviceManager::ApplyChanges()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void ApplyChanges(Microsoft.Xna.Framework.GraphicsDeviceManager)");
            foreach (Type type in typeof(Form).Assembly.SaferGetTypes())
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



            Modder.RelinkMap["System.Windows.Forms.FormClosingEventHandler"] = "XnaToFna.ProxyForms.FormClosingEventHandler";
            Modder.RelinkMap["System.Windows.Forms.Form"] = "XnaToFna.ProxyForms.Form";
            Modder.RelinkMap["System.Windows.Forms.Control"] = "XnaToFna.ProxyForms.Control";
            Modder.RelinkMap["System.Windows.Forms.Control System.Windows.Forms.Control::FromHandle(System.IntPtr)"] = new RelinkMapEntry("XnaToFna.ProxyForms.Control", "XnaToFna.ProxyForms.Control FromHandle(System.IntPtr)");
            //Dans thing ReadAllLines 
            Modder.RelinkMap["System.IO.DirectoryInfo System.IO.Directory::CreateDirectory(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IO.DirectoryInfo DirectoryCreateDirectory(System.String)");
            Modder.RelinkMap["System.Boolean System.IO.Directory::Exists(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Boolean DirectoryExists(System.String)");
            Modder.RelinkMap["System.String[] System.IO.Directory::GetFiles(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String[] DirectoryGetFiles(System.String)");
            Modder.RelinkMap["System.Void System.IO.Directory::Delete(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void DirectoryDelete(System.String)");
            Modder.RelinkMap["System.String System.IO.Directory::GetCurrentDirectory()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String GetCurrentDirectory()");
            Modder.RelinkMap["System.String[] System.IO.Directory::GetFiles(System.String,System.String,System.IO.SearchOption)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String[] GetFiles(System.String,System.String,System.IO.SearchOption)");
    
            Modder.RelinkMap["System.IO.FileInfo[] System.IO.DirectoryInfo::GetFiles()"] = 
                new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IO.FileInfo[] DirectoryInfoGetFiles(System.IO.DirectoryInfo)");

            Modder.RelinkMap["System.IO.FileInfo[] System.IO.DirectoryInfo::GetFiles(System.String)"] = 
                new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IO.FileInfo[] DirectoryInfoGetFiles(System.IO.DirectoryInfo,System.String)");

            Modder.RelinkMap["System.IO.FileInfo[] System.IO.DirectoryInfo::GetFiles(System.String,System.IO.SearchOption)"] = 
                new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.IO.FileInfo[] DirectoryInfoGetFiles(System.IO.DirectoryInfo,System.String,System.IO.SearchOption)");



            Modder.RelinkMap["System.Void System.IO.File::SetAttributes(System.String,System.IO.FileAttributes)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileSetAttributes(System.String,System.IO.FileAttributes)");
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String,System.Text.Encoding)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileReadAllText(System.String,System.Text.Encoding)");

            Modder.RelinkMap["System.Void System.IO.File::Delete(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileDelete(System.String)");
            Modder.RelinkMap["System.Void System.IO.File::WriteAllText(System.String,System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileWriteAllText(System.String,System.String)");
            Modder.RelinkMap["System.Void System.IO.File::WriteAllLines(System.String,System.String[])"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileWriteAllLines(System.String,System.String[])");
            Modder.RelinkMap["System.String[] System.IO.File::ReadAllLines(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String[] FileReadAllLines(System.String)");
            Modder.RelinkMap["System.Boolean System.IO.File::Exists(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Boolean FileExists(System.String)");

            Modder.RelinkMap["System.Void System.IO.File::Copy(System.String,System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileCopy(System.String,System.String)");
            Modder.RelinkMap["System.Void System.IO.File::Copy(System.String,System.String,System.Boolean)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void FileCopy(System.String,System.String,System.Boolean)");

            Modder.RelinkMap["System.String System.IO.Path::Combine(System.String,System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String Combine(System.String,System.String)");

            Modder.RelinkMap["System.Byte[] System.IO.File::ReadAllBytes(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Byte[] FileReadAllBytes(System.String)"); //idk
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String FileReadAllText(System.String)");
            Modder.RelinkMap["System.String System.IO.File::ReadAllText(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.String FileReadAllText(System.String)");


            Modder.RelinkMap["System.Reflection.Assembly System.Reflection.Assembly::Load(System.Byte[])"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Reflection.Assembly AssemblyLoad(System.Byte[])");
            Modder.RelinkMap["System.Reflection.Assembly System.Reflection.Assembly::LoadFile(System.String)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Reflection.Assembly AssemblyLoadFile(System.String)");


            Modder.RelinkMap["System.Reflection.MethodInfo System.Type::GetMethod(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.MethodInfo GetMethod(System.Type,System.String,System.Reflection.BindingFlags)");

            Modder.RelinkMap["System.Reflection.FieldInfo System.Type::GetField(System.String,System.Reflection.BindingFlags)"] = new RelinkMapEntry("XnaToFna.ProxyReflection.FieldInfoHelper", "System.Reflection.FieldInfo GetField(System.Type,System.String,System.Reflection.BindingFlags)");
            Modder.RelinkMap["System.Object System.Activator::CreateInstance(System.Type)"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Object ActivatorCreateInstance(System.Type)");



            //Mod Stuff 
            Modder.RelinkMap["System.Void DuckGame.HaloWeapons.Resources::LoadShaders()"] = new RelinkMapEntry("XnaToFna.XnaToFnaHelper", "System.Void DoNothing()");

            MethodInfo TryCatchPatch = typeof(XnaToFnaUtil).GetMethod(nameof(AddTryCatchPatch), BindingFlags.NonPublic | BindingFlags.Static);
            Modder.TranspilerMap["System.Void DuckGame.HaloWeapons.Skins::AddCredits(System.Int32)"] = new TranspilerMapEntry(TryCatchPatch);

            Modder.TranspilerMap["System.Single DuckGame.ExtraStuff.EMusic::get_progress()"] = new TranspilerMapEntry(TryCatchPatch);
            Modder.TranspilerMap["System.TimeSpan DuckGame.ExtraStuff.EMusic::get_length()"] = new TranspilerMapEntry(TryCatchPatch);
            Modder.TranspilerMap["System.TimeSpan DuckGame.ExtraStuff.EMusic::get_position()"] = new TranspilerMapEntry(TryCatchPatch);



            MethodInfo ReturnImmediatelyPatch_ = typeof(XnaToFnaUtil).GetMethod(nameof(ReturnImmediatelyPatch), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo ReturnDefaultTypePatch_ = typeof(XnaToFnaUtil).GetMethod(nameof(ReturnDefaultTypePatch), BindingFlags.NonPublic | BindingFlags.Static);

            //Modder.RelinkMap["System.Void AncientMysteries.AncientMysteriesMod::<Hooks_OnUpdate>g__UpdateModDisplayName|17_0()"] = new RelinkMapEntry("XnaToFna.AncientMysteriesReplacements", "System.Void UpdateModDisplayName()");
            Modder.RelinkMap["System.String AncientMysteries.Hook.Patches.LSItem_Draw::<Postfix>g__GetName|2_0(System.Single)"] = new RelinkMapEntry("XnaToFna.AncientMysteriesReplacements", "System.String GetName(System.Single)");

            Modder.TranspilerMap["System.Void AncientMysteries.AncientMysteriesMod::<Hooks_OnUpdate>g__UpdateModDisplayName|17_0()"] = new TranspilerMapEntry(ReturnImmediatelyPatch_);

            Modder.TranspilerMap[
                "System.Boolean DuckGame.BrutalDG.DuckGib/<>c__DisplayClass0_0::<.ctor>b__0(System.Collections.Generic.KeyValuePair`2<DuckGame.DuckPersona,DuckGame.Tex2D>)"
            ] = new TranspilerMapEntry(
                typeof(XnaToFnaUtil).GetMethod(nameof(Fix_DuckPersona_Equality), BindingFlags.NonPublic | BindingFlags.Static)
);

            //"System.String AncientMysteries.Hook.Patches.LSItem_Draw::<Postfix>g__GetName|2_0(System.Single)"
            // Patch CosmicDisruption_AmmoType constructor - change infinity to 1e20
            //Modder.TranspilerMap["System.Void AncientMysteries.Items.CosmicDisruption_AmmoType::.ctor()"] =
            //    new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(
            //        nameof(PatchRemoveInfinity),
            //        BindingFlags.NonPublic | BindingFlags.Static));
            Modder.TranspilerMap["System.String AncientMysteries.Hook.Patches.LSItem_Draw::<Postfix>g__GetName|2_0(System.Single)"] = new TranspilerMapEntry(ReturnDefaultTypePatch_);

            Modder.TranspilerMap["System.Void AncientMysteries.Module::Initialize()"] = new TranspilerMapEntry(ReturnImmediatelyPatch_);
            Modder.TranspilerMap["System.Void AncientMysteries.AncientMysteriesMod::Hooks_OnUpdate()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(AncientMysteries_Hooks_Update), BindingFlags.NonPublic | BindingFlags.Static));

            // ExtraStuff2 2586315559 Reimplement patcher due to nasty dynamic link code eww
            Modder.RelinkMap["System.Void DuckGame.ExtraStuff2.MAutoPatchHandler::Patch()"] = new RelinkMapEntry("XnaToFna.ExtraStuff2Replacements", "System.Void PatchReplacement()");

            //Balloon Net Gun & Mind Swap Grenade 1971284863 Null Exception 
            Modder.TranspilerMap["System.Void DuckGame.FunGun.NetBalloon::Update()"] = new TranspilerMapEntry(TryCatchPatch);
            Modder.TranspilerMap["System.Void DuckGame.FunGun.NetBalloon::Draw()"] = new TranspilerMapEntry(TryCatchPatch);

            //Johns Weapon 1268274761
            // Some kinda linux related crash something to do with System.UnauthorizedAccessException: Access to the path probly something wrong with doing
            //public static string logPath = "C:\\Users\\" + Environment.UserName + "\\Documents\\DuckGame\\DuckDebug\\"; for pathing
            // as logging isnt as imporant as any other function going to try catch it
            Modder.TranspilerMap["System.Void DuckGame.DuckDebug.DuckDebug::Write(System.String)"] = new TranspilerMapEntry(TryCatchPatch);
            Modder.TranspilerMap["System.Void DuckGame.DuckDebug.DuckDebug::clearLog()"] = new TranspilerMapEntry(TryCatchPatch);



            //DWEP [945664816]   SpecialDrawer::Update System.NullReferenceException: Special Code: draw5 
            Modder.TranspilerMap["System.Void DuckGame.DWEP.SpecialDrawer::Update()"] = new TranspilerMapEntry(TryCatchPatch);


            //Gatling Guns [2395356716]   Phasaber.OnPressAction
            Modder.TranspilerMap["System.Void DuckGame.GatlingGuns.Phasaber::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerDuckCheck), BindingFlags.NonPublic | BindingFlags.Static));

            Modder.TranspilerMap["System.Void DuckGame.C44P.C4::Update()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(C4PP_C4_Update), BindingFlags.NonPublic | BindingFlags.Static));

            //JamMod 898850588
            Modder.TranspilerMap["System.Void DuckGame.JamMod.Banjoo::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(JamMod_Banjoo_OnPressAction), BindingFlags.NonPublic | BindingFlags.Static));
            Modder.TranspilerMap["System.Void DuckGame.JamMod.Schnitzel::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerDuckCheck), BindingFlags.NonPublic | BindingFlags.Static));



            //OstrichMod [2956579195]
            Modder.TranspilerMap["System.Void DuckGame.OstrichMod.Impacto::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerNullCheck), BindingFlags.NonPublic | BindingFlags.Static));
            Modder.TranspilerMap["System.Void DuckGame.OstrichMod.Sonyblade::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerNullCheck), BindingFlags.NonPublic | BindingFlags.Static));

            //IconicWeapons [1629158033]
            Modder.TranspilerMap["System.Void DuckGame.IconicWeapons.Scar::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerDuckCheck), BindingFlags.NonPublic | BindingFlags.Static));
            Modder.TranspilerMap["System.Void DuckGame.IconicWeapons.M16::OnPressAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod(nameof(OwnerDuckCheck), BindingFlags.NonPublic | BindingFlags.Static));


            //JamMod_Schnitzel_OnPressAction
            //Modder.TranspilerMap["System.Void DuckGame.JamMod.AK47W::OnHoldAction()"] = new TranspilerMapEntry(typeof(XnaToFnaUtil).GetMethod("JamTranspile")); example

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
        public static void SetupHarmonyRelinkMap(XnaToFnaUtil xtf, XnaToFnaMapping mapping)
        {
            // Map HarmonyInstance (Harmony 1) → HarmonyLib.Harmony (Harmony 2)
            TypeDefinition harmonyLibHarmony = mapping.Module.Types
                .FirstOrDefault(t => t.FullName == "HarmonyLib.Harmony");

            if (harmonyLibHarmony != null)
            {
                xtf.Modder.RelinkMap["Harmony.HarmonyInstance"] = harmonyLibHarmony;
            }

            // Types in Harmony 1 that moved or got renamed in Harmony 2
            var knownRelocations = new Dictionary<string, string>
            {
                { "ByteBuffer", "ILCopying" },
                { "Emitter", "ILCopying" },
                { "ExceptionBlock", "ILCopying" },
                { "ExceptionBlockType", "ILCopying" },
                { "ILInstruction", "ILCopying" },
                { "LeaveTry", "ILCopying" },
                { "Memory", "ILCopying" },
                { "MethodBodyReader", "ILCopying" },
                { "MethodCopier", "ILCopying" },
                { "SelfPatching", "Tools" }
            };

            // Relink all Harmony types in the module
            SetupDirectRelinkMap(xtf, mapping, (_, type) =>
            {
                if (!type.Namespace.StartsWith("HarmonyLib"))
                    return;

                string shortTypeName = type.Name;

                // Skip Harmony itself; handled above
                if (shortTypeName == "Harmony")
                    return;

                // Determine old Harmony 1 namespace
                string oldNamespace = knownRelocations.TryGetValue(shortTypeName, out string relocatedNs)
                    ? $"Harmony.{relocatedNs}"  // e.g., Harmony.ILCopying, Harmony.Tools
                    : "Harmony";

                string fullOldTypeName = $"{oldNamespace}.{shortTypeName}";

                // Map old Harmony 1 references → Harmony 2 type
                xtf.Modder.RelinkMap[fullOldTypeName] = type;

                // Also shorthand mapping just by type name
                xtf.Modder.RelinkMap[$"Harmony.{shortTypeName}"] = type;
            });
        }
        public static void SetupHarmonyRelinkMap_old(XnaToFnaUtil xtf, XnaToFnaMapping mapping)
        {
            TypeDefinition harmonyInstance =  mapping.Module.Types.FirstOrDefault(t => t.FullName == "Harmony.HarmonyInstance");

            if (harmonyInstance != null)
            {
                xtf.Modder.RelinkMap["HarmonyLib.Harmony"] = harmonyInstance;
            }
            Dictionary<string, string> knownRelocations = new Dictionary<string, string>()
            {
                { "ByteBuffer", "Harmony.ILCopying" },
                { "Emitter", "Harmony.ILCopying" },
                { "ExceptionBlock", "Harmony.ILCopying" },
                { "ExceptionBlockType", "Harmony.ILCopying" },
                { "ILInstruction", "Harmony.ILCopying" },
                { "LeaveTry", "Harmony.ILCopying" },
                { "Memory", "Harmony.ILCopying" },
                { "MethodBodyReader", "Harmony.ILCopying" },
                { "MethodCopier", "Harmony.ILCopying" },
    
                { "SelfPatching", "Harmony.Tools" }
            };

            SetupDirectRelinkMap(xtf, mapping, (_, type) =>
            {
                if (!type.Namespace.StartsWith("Harmony"))
                    return;

                string shortTypeName = type.Name;

                if (shortTypeName == "HarmonyInstance")
                    return;

                string oldNamespace = knownRelocations.TryGetValue(shortTypeName, out string relocatedNs)
                    ? $"HarmonyLib.{relocatedNs.Split('.').Last()}" 
                    : "HarmonyLib";

                string fullOldTypeName = $"{oldNamespace}.{shortTypeName}";
                xtf.Modder.RelinkMap[fullOldTypeName] = type;

                xtf.Modder.RelinkMap[$"HarmonyLib.{shortTypeName}"] = type;
            });
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
            catch
            { }
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
        private static List<Instruction> ReturnImmediatelyPatch(List<Instruction> old)
        {
            return new List<Instruction> { Instruction.Create(OpCodes.Ret) };
        }
        private static List<Instruction> ReturnDefaultTypePatch(MethodDefinition originalMethod)
        {
            var il = new List<Instruction>();
            TypeReference returnType = originalMethod.ReturnType;

            if (returnType.IsValueType)
            {
                // For value types (structs, primitives), use Initobj to zero-initialize
                VariableDefinition tempVar = new VariableDefinition(returnType);
                originalMethod.Body.Variables.Add(tempVar);

                il.Add(Instruction.Create(OpCodes.Ldloca, tempVar));
                il.Add(Instruction.Create(OpCodes.Initobj, returnType));
                il.Add(Instruction.Create(OpCodes.Ldloc, tempVar));
            }
            else
            {
                // For reference types (classes), simply load null
                il.Add(Instruction.Create(OpCodes.Ldnull));
            }

            il.Add(Instruction.Create(OpCodes.Ret));
            return il;
        }

        //private static void AddTryCatchPatch(MethodDefinition m)
        //{
        //    if (m.Body.ExceptionHandlers.Count != 0)  // check into if this a good idea to do?
        //    { 
        //        return;
        //    }
        //    var body = m.Body;
        //    body.InitLocals = true;
        //    var il = body.GetILProcessor();

        //    /* ─── locals ────────────────────────────────────────────── */
        //    var excType = m.Module.ImportReference(typeof(Exception)); // Exception ex
        //    var exVar = new VariableDefinition(excType);
        //    body.Variables.Add(exVar);

        //    VariableDefinition retVar = null;
        //    bool hasReturn = m.ReturnType.MetadataType != MetadataType.Void;
        //    if (hasReturn)
        //    {
        //        retVar = new VariableDefinition(m.ReturnType);
        //        body.Variables.Add(retVar);
        //    }

        //    /* ─── labels / scaffolding ──────────────────────────────── */
        //    var epilogue = il.Create(OpCodes.Nop);          // unified return point
        //    var fallThroughLeave = il.Create(OpCodes.Leave, epilogue);
        //    il.Append(fallThroughLeave);                    // end-of-try sentinel

        //    /* ─── rewrite existing “ret” instructions ───────────────── */
        //    foreach (var ret in body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToList())
        //    {
        //        if (hasReturn)
        //            il.InsertBefore(ret, il.Create(OpCodes.Stloc, retVar));

        //        ret.OpCode = OpCodes.Leave;
        //        ret.Operand = epilogue;
        //    }

        //    /* ─── catch block (store ex, ignore, continue) ──────────── */
        //    var catchStart = il.Create(OpCodes.Stloc, exVar);    // catch(Exception ex)
        //    il.Append(catchStart);
        //    var catchEnd = il.Create(OpCodes.Leave, epilogue); // swallow & continue
        //    il.Append(catchEnd);

        //    /* ─── common epilogue ───────────────────────────────────── */
        //    il.Append(epilogue);
        //    if (hasReturn) il.Append(il.Create(OpCodes.Ldloc, retVar)); // default(T)
        //    il.Append(il.Create(OpCodes.Ret));

        //    /* ─── EH table entry ────────────────────────────────────── */
        //    body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
        //    {
        //        CatchType = excType,
        //        TryStart = body.Instructions.First(),
        //        TryEnd = catchStart,   // first instr *after* try
        //        HandlerStart = catchStart,
        //        HandlerEnd = epilogue      // first instr *after* catch
        //    });
        //}

        private static List<Instruction> OwnerDuckCheck(MethodDefinition method, List<Instruction> instructions)
        {
            if (!method.HasBody)
                return instructions;
            // Skip static methods and constructors
            if (method.IsStatic || method.IsConstructor)
                return instructions;

            StaticLog($"[DuckCheck] Injecting 'if (!(this.owner is Duck)) return;' into {method.GetFindableID()}");

            ILProcessor processor = method.Body.GetILProcessor();
            List<Instruction> newInstructions = new List<Instruction>();

            // Create continue label
            Instruction continueLabel = processor.Create(OpCodes.Nop);

            // Build: if (!(this.owner is Duck)) return;
            newInstructions.Add(processor.Create(OpCodes.Ldarg_0));                    // this

            MethodReference getOwnerMethod = method.Module.ImportReference(typeof(Thing).GetProperty(nameof(Thing.owner)).GetGetMethod());

            newInstructions.Add(processor.Create(OpCodes.Callvirt, getOwnerMethod));       // this.owner (getter)

            // Resolve the Duck type from the module
            TypeReference duckType = method.Module.ImportReference(typeof(Duck));                // import Duck type

            newInstructions.Add(processor.Create(OpCodes.Isinst, duckType));          // owner as Duck (null if not Duck)
            newInstructions.Add(processor.Create(OpCodes.Brtrue_S, continueLabel));   // if Duck → continue
            newInstructions.Add(processor.Create(OpCodes.Ret));                       // return early (not a Duck)
            newInstructions.Add(continueLabel);                                       // original code continues

            // Add original instructions
            newInstructions.AddRange(instructions);

            return newInstructions;
        }
        private static List<Instruction> OwnerNullCheck(MethodDefinition method, List<Instruction> instructions)
        {
            if (!method.HasBody)
                return instructions;
            // Skip static methods and constructors
            if (method.IsStatic || method.IsConstructor)
                return instructions;

            StaticLog($"[NullCheck] Injecting 'if (this.owner == null) return;' into {method.GetFindableID()}");

            ILProcessor processor = method.Body.GetILProcessor();
            var newInstructions = new List<Instruction>();

            // Create continue label
            Instruction continueLabel = processor.Create(OpCodes.Nop);

            // Build: if (this.owner == null) return;
            newInstructions.Add(processor.Create(OpCodes.Ldarg_0));                    // this

            // Call the property getter: get_owner()
            MethodReference getOwnerMethod = method.Module.ImportReference(typeof(Thing).GetProperty(nameof(Thing.owner)).GetGetMethod());

            newInstructions.Add(processor.Create(OpCodes.Callvirt, getOwnerMethod));       // this.owner (getter)

            newInstructions.Add(processor.Create(OpCodes.Brtrue_S, continueLabel));    // if not null → continue
            newInstructions.Add(processor.Create(OpCodes.Ret));                        // return early
            newInstructions.Add(continueLabel);                                        // original code

            // Add original instructions
            newInstructions.AddRange(instructions);

            return newInstructions;
        }
        private static List<Instruction> Fix_DuckPersona_Equality(MethodDefinition method, List<Instruction> instructions)
        {
            if (!method.HasBody)
                return instructions;
            List<Instruction> newInstructions = new List<Instruction>();
            MethodReference opEquality = method.Module.ImportReference(
                 typeof(DuckGame.DuckPersona).GetMethod(
                     "op_Equality",
                     BindingFlags.Public | BindingFlags.Static,
                     null,
                     new[] { typeof(DuckGame.DuckPersona), typeof(DuckGame.DuckPersona) },
                     null
                 )
             );

            for (int i = 0; i < instructions.Count; i++)
            {
                // Look for: ldfld DuckPersona persona + ceq
                if (i >= 1 &&  instructions[i].OpCode == OpCodes.Ceq && instructions[i - 1].OpCode == OpCodes.Ldfld)
                {
                    var ldfld = instructions[i - 1].Operand as Mono.Cecil.FieldReference;

                    if (ldfld != null && ldfld.FieldType.FullName == typeof(DuckGame.DuckPersona).FullName)
                    {
                        // Replace ceq with operator ==
                        newInstructions.Add(new Instruction(OpCodes.Call, opEquality));
                        continue;
                    }
                }

                newInstructions.Add(instructions[i]);
            }

            return newInstructions;
        }

        // Patch: Add top-level try/catch(Exception) to a method, regardless of existing handlers
        private static void AddTryCatchPatch(MethodDefinition method)
        {
            if (!method.HasBody)
                return;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            ILProcessor il = body.GetILProcessor();
            body.InitLocals = true;

            // (1) Create a new variable to hold Exception ex
            TypeReference excType = method.Module.ImportReference(typeof(Exception));
            var exVar = new VariableDefinition(excType);
            body.Variables.Add(exVar);

            // (2) (If needed) Create a variable to store the return value
            VariableDefinition retVar = null;
            bool hasReturn = method.ReturnType.MetadataType != MetadataType.Void;
            if (hasReturn)
            {
                retVar = new VariableDefinition(method.ReturnType);
                body.Variables.Add(retVar);
            }

            // (3) Add a new NOP as a unified epilogue (method exit)
            Instruction epilogue = il.Create(OpCodes.Nop);
            il.Append(epilogue);

            // (4) Rewrite all ret instructions to go to the epilogue, storing the return value if needed
            var originalRets = body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToList();
            foreach (Instruction ret in originalRets)
            {
                if (hasReturn)
                    il.InsertBefore(ret, il.Create(OpCodes.Stloc, retVar));
                ret.OpCode = OpCodes.Leave;
                ret.Operand = epilogue;
            }

            // (5) Add handlers for your catch block
            Instruction catchStart = il.Create(OpCodes.Stloc, exVar);
            il.Append(catchStart);

            // (Optional) Insert your error handling/logging here, e.g. call a logger method or just swallow
            // il.Append(il.Create(OpCodes.Call, logExceptionMethod)); // if you have one

            Instruction catchLeave = il.Create(OpCodes.Leave, epilogue);
            il.Append(catchLeave);

            // (6) Add the top-level ExceptionHandler (which covers all instructions up to the catch)
            var allInstructions = body.Instructions.ToList();
            Instruction tryStart = allInstructions.First();
            Instruction tryEnd = catchStart;
            Instruction handlerStart = catchStart;
            Instruction handlerEnd = epilogue;

            var topHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = excType,
                TryStart = tryStart,
                TryEnd = tryEnd,
                HandlerStart = handlerStart,
                HandlerEnd = handlerEnd
            };
            body.ExceptionHandlers.Add(topHandler);

            // (7) Patch up all existing ExceptionHandlers if needed, 
            // so that HandlerEnd/TryEnd that pointed to "the end" now point to the new epilogue
            foreach (ExceptionHandler eh in body.ExceptionHandlers.ToArray())
            {
                if (eh != topHandler)
                {
                    if (eh.TryEnd == null || eh.TryEnd == allInstructions.Last() || eh.TryEnd == null)
                        eh.TryEnd = catchStart; // Try block must end at catch start
                    if (eh.HandlerEnd == null || eh.HandlerEnd == allInstructions.Last() || eh.HandlerEnd == null)
                        eh.HandlerEnd = epilogue;
                }
            }

            // (8) At the epilogue, load return value if necessary, then return
            il.Append(epilogue);
            if (hasReturn)
                il.Append(il.Create(OpCodes.Ldloc, retVar));
            il.Append(il.Create(OpCodes.Ret));

        }
        private static List<Instruction> AncientMysteries_Hooks_Update(MethodDefinition module)
        {
            MethodInfo method = typeof(AncientMysteriesReplacements).GetMethod(
                "UpdateModDisplayName",
                BindingFlags.Public | BindingFlags.Static);

            if (method == null)
                throw new InvalidOperationException("Could not find AncientMysteriesReplacements.UpdateModDisplayName");
  
            MethodReference imported = module.Module.ImportReference(method);

            return new List<Instruction>
            {
                Instruction.Create(OpCodes.Call, imported),
                Instruction.Create(OpCodes.Ret)
            };
        }
        private static List<Instruction> C4PP_C4_Update(List<Instruction> instructions)
        {
            List<Instruction> new_instructions = new List<Instruction>();
            object JumpLocation = null;
            bool Packed = false;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode == OpCodes.Ble_Un_S)
                {
                    JumpLocation = instruction.Operand;
                }
                if (i > 0 && !Packed)
                {
                    Instruction prev = instructions[i - 1];

                    if (prev.OpCode == OpCodes.Call &&
                        prev.Operand != null &&
                        prev.Operand.ToString() ==
                        "!!0 DuckGame.Level::Nearest<DuckGame.C44P.GM_Fuse>(System.Single,System.Single)")
                    {
                        Packed = true;
                        new_instructions.Add(new Instruction(OpCodes.Stloc_0, null));
                        new_instructions.Add(new Instruction(OpCodes.Ldloc_0, null));
                        new_instructions.Add(new Instruction(OpCodes.Brfalse_S, JumpLocation));
                        continue;
                    }
                }
                new_instructions.Add(instruction);
            }

            return new_instructions;

        }
        private static List<Instruction> PatchRemoveInfinity(MethodDefinition method, List<Instruction> instructions)
        {
            if (!method.HasBody)
                return instructions;
            StaticLog("[Transpiler] Patching " + method.FullName + " - replacing Infinity with 1e20");

            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction instr = instructions[i];
                if (instr.Operand != null && instr.Operand is float)
                {
                    float value = (float)instr.Operand;

                    // Check if it's positive infinity (the value that was (00 00 80 7F))
                    // Then set it to 100000000000000000000f
                    if (float.IsPositiveInfinity(value))
                    {
                        StaticLog($"[Transpiler] Replaced +Infinity with 1e20 at offset IL_{instr.Offset:X4}");

                        // Replace with 1 × 10^20
                        instr.Operand = 1e20f;        // or 100000000000000000000f
                    }
                    else if (float.IsNegativeInfinity(value))
                    {
                        StaticLog($"[Transpiler] Replaced -Infinity with 1e20 at offset IL_{instr.Offset:X4}");

                        // Replace with 1 × 10^20
                        instr.Operand = -1e20f;        // or 100000000000000000000f
                    }
                }
            }

            return instructions;
        }
        private static List<Instruction> JamMod_Banjoo_OnPressAction(List<Instruction> instructions)
        {
            List<Instruction> new_instructions = new List<Instruction>();
            object JumpLocation = null;
            bool Packed = false;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                if (!Packed)
                {
                    if (instruction.OpCode == OpCodes.Stfld &&
                        instruction.Operand != null && instruction.Operand.ToString() == "DuckGame.Duck DuckGame.JamMod.Banjoo::duc")
                    {
                        Packed = true;
                        new_instructions.Add(instruction);

                        new_instructions.Add(new Instruction(OpCodes.Ldarg_0, null));
                        new_instructions.Add(new Instruction(OpCodes.Ldfld, instruction.Operand));
                        new_instructions.Add(new Instruction(OpCodes.Brfalse_S, instructions[i + 6]));
                        continue;
                    }
                }
                new_instructions.Add(instruction);
            }

            return new_instructions;

        }
        private void NukeAMStringHandler(ModuleDefinition module, TypeDefinition type)
        {
            if (type.FullName != "AncientMysteries.Utilities.AMStringHandler")
                return;

            Log("[NUKE] Clearing AncientMysteries.Utilities.AMStringHandler");

            // Make it as invisible / unusable as possible
            type.IsPublic = false;
            type.IsNotPublic = true;
            type.IsAbstract = true;     // prevent instantiation
            type.IsSealed = true;
            type.IsBeforeFieldInit = true;

            // Remove everything inside
            type.Fields.Clear();
            type.Methods.Clear();
            type.Properties.Clear();
            type.Events.Clear();
            type.NestedTypes.Clear();
            // If it's nested:
            if (type.DeclaringType != null)
                type.DeclaringType.NestedTypes.Remove(type);
            else
                module.Types.Remove(type);
            // Optional: rename so even reflection by name fails
            // type.Name = "Dead_AMStringHandler_Dead";
        }
        public void PostProcessType(ModuleDefinition module, TypeDefinition type)
        {
            NukeAMStringHandler(module, type);
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
                    string methodId = method.GetFindableID(withType: true);
                    if (flag && findableId == "System.Void Update(Microsoft.Xna.Framework.GameTime)")
                    {
                        Log("[PostProcess] Injecting call to XnaToFnaHelper.PreUpdate into game Update");
                        ILProcessor ilProcessor = method.Body.GetILProcessor();
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldarg_1));
                        ilProcessor.InsertAfter(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Callvirt, method.Module.ImportReference(m_XnaToFnaHelper_PreUpdate)));
                        method.Body.UpdateOffsets(1, 2);
                    }
                    if (Modder.TranspilerMap.TryGetValue(methodId, out TranspilerMapEntry mapEntry))
                    {
                        InstructionCollection instructions = mapEntry.ProcessILCode(method.Body.Instructions.ToList(), method);
                        if (instructions == null)
                        {
                            continue;
                        }
                        method.Body.Instructions = instructions;
                    }
                    for (int instri = 0; instri < method.Body.Instructions.Count; ++instri)
                    {
                        Instruction instruction = method.Body.Instructions[instri];
                        if (instruction.OpCode == OpCodes.Callvirt && ((MemberReference)instruction.Operand).DeclaringType.FullName == "XnaToFna.XnaToFnaHelper")
                            instruction.OpCode = OpCodes.Call;
                        if (DestroyLocks)
                            CheckAndDestroyLock(method, instri);
                        {
                            try
                            {
                                CheckAndDestroyLock(method, instri);
                            }
                            catch{}
                        }
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
                PostProcessType(module, nestedType);
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
            if (method == null || method.Body == null || method.Body.Instructions == null)
            {
                return;
            }
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
            Mono.Collections.Generic.Collection<Instruction> instructions = method.Body.Instructions;
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
            Mappings = new List<XnaToFnaMapping>
            {
                new XnaToFnaMapping("System", new string[1]
                {
                        "System.Net"
                }),
                new XnaToFnaMapping("FNA", new string[10]
                {
                    "WineMono.FNA", // Added for a random mod crash seems to fix it
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
                new XnaToFnaMapping("MonoGame.Framework.Net", new string[3]
                {
                    "Microsoft.Xna.Framework.GamerServices",
                    "Microsoft.Xna.Framework.Net",
                    "Microsoft.Xna.Framework.Xdk"
                },
                new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap)),
                new XnaToFnaMapping("FNA.Steamworks", new string[4]
                    {
                        "FNA.Steamworks",
                        "Microsoft.Xna.Framework.GamerServices",
                        "Microsoft.Xna.Framework.Net",
                        "Microsoft.Xna.Framework.Xdk"
                    },
               new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap)),
                new XnaToFnaMapping("Steam", new string[2]
                    {
                        "Steam",
                        "DGSteam"

                }, new XnaToFnaMapping.SetupDelegate(SetupGSRelinkMap2)),
                new XnaToFnaMapping("0Harmony", new[] { "HarmonyLoader" }, //HarmonyLoader 0Harmony
                new XnaToFnaMapping.SetupDelegate(SetupHarmonyRelinkMap))
            };

            AssemblyResolver = new CustomAssemblyResolver();
            Directories = new List<string>();
            ContentDirectoryNames = new List<string>()
            {
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
            DestroyPublicKeyTokens.Add("0Harmony");
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
        public static void StaticLog(string txt)
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
                if (key.Assembly.Name.Name == "0Harmony") // HarmonyLoader
                {
                    Log("[ScanPath] Registering HarmonyLoader in DependencyCache");
                    Modder.DependencyCache["0Harmony"] = key;
                    Modder.DependencyCache[key.Assembly.Name.FullName] = key;
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
                PostProcessType(mod, type);
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
        private static readonly HashSet<string> DangerousAttributePrefixes = new()
        {
            "System.Runtime.CompilerServices.IsReadOnlyAttribute"
        };
     
        static void ProcessCustomAttributes(Mono.Cecil.ICustomAttributeProvider provider, string context)
        {
            if (provider == null || !provider.HasCustomAttributes)
                return;

            Collection<CustomAttribute> attrs = provider.CustomAttributes;

            for (int i = attrs.Count - 1; i >= 0; i--)
            {
                CustomAttribute ca = attrs[i];
                string attrName = ca.AttributeType?.FullName ?? "(unknown attribute type)";

                bool remove = false; 

                //if (attrName.Contains("Nullable") ||
                //    attrName.Contains("CompilerGenerated") ||
                //    attrName.Contains("Debugger") ||
                //    attrName.Contains("TargetFramework") ||
                //    attrName.StartsWith("System.Reflection.Assembly") ||
                //    attrName.Contains("Harmony")) 
                //{
                //    remove = false;
                //} AncientMysteries.MetaInfoAttribute System.


                //"AncientMysteries.Hook.Attributes.HookAfterAttribute"
                //"AncientMysteries.MetaTypeAttribute"
                //"AncientMysteries.MetaImageAttribute"
                //"DuckGame.BaggedPropertyAttribute"
                //AncientMysteries.MetaOrderAttribute
                // attrName.Contains("AncientMysteries.MetaInfoAttribute")



                //attrName.Contains("DuckGame.BaggedPropertyAttribute")
                //|| attrName.Contains("System.")
                //DuckGame.EditorGroupAttribute
                //if ((attrName.Contains("System.") && !attrName.Contains("System.Reflection") && !attrName.Contains("System.Runtime")) || attrName.Contains("HarmonyLib.HarmonyPatch"))
                //{
                //    remove = true;
                //}
                //else
                //{
                //    remove = false;
                //}
                //var attributes = new List<string>
                //{
                //    //"System.Runtime.Versioning.TargetFrameworkAttribute",
                //    //"System.Runtime.CompilerServices.RuntimeCompatibilityAttribute",
                //    //"System.Runtime.CompilerServices.CompilationRelaxationsAttribute",
                //    //"System.Runtime.CompilerServices.ExtensionAttribute",
                //    //"System.Runtime.CompilerServices.CompilerGeneratedAttribute",

                //    //"System.Runtime.CompilerServices.NullableAttribute",
                //    //"System.Runtime.CompilerServices.NullableContextAttribute",

                //   // "System.Runtime.CompilerServices.ModuleInitializerAttribute",

                //    "System.Runtime.CompilerServices.IsReadOnlyAttribute",// <----

                //    //"System.Runtime.CompilerServices.IteratorStateMachineAttribute",
                //    "HarmonyLib.HarmonyPatch"
                //};
                //foreach(string attr in attributes)
                //{
                //    if (attrName.Contains(attr))
                //    {
                //        remove = true;
                //        break;
                //    }
                //}
                //if (attrName.Contains("System.Runtime"))
                //{
                //    remove = true;
                //}
                //else
                //{
                //    remove = false;
                //}

                // AncientMysteries.MetaImageAttribute AncientMysteries.MetaInfoAttribute

                // Or only remove specific ones (whitelist style):
                // bool remove = attrName.Contains("AncientMysteries") ||
                //               attrName.Contains("Obsolete") ||
                //               attrName.Contains("EditorBrowsable");

                bool shouldRemove = DangerousAttributePrefixes.Any(prefix => attrName.StartsWith(prefix));
                if (shouldRemove)
                {
                    remove = true;
                }
                else
                {
                    remove = false;
                }
                if (remove)
                {
                    attrs.RemoveAt(i);
                }
                else
                {
                    // Console.WriteLine($"[KEPT]    {attrName}");
                }
            }
        }
        static void CleanReferencesToType(ModuleDefinition module, TypeDefinition deletedType)
        {
            var deletedRef = deletedType as TypeReference;

            foreach (TypeDefinition type in module.Types)
            {
                for (int i = type.Fields.Count - 1; i >= 0; i--)
                {
                    if (type.Fields[i].FieldType.FullName == deletedRef.FullName)
                    {
                        type.Fields.RemoveAt(i);
                        Console.WriteLine($"Removed field typed as {deletedRef.Name}");
                    }
                }

                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        Collection<Instruction> instructions = method.Body.Instructions;
                        for (int i = instructions.Count - 1; i >= 0; i--)
                        {
                            Instruction ins = instructions[i];
                            if (ins.Operand is FieldReference fr && fr.FieldType.FullName == deletedRef.FullName)
                                instructions.RemoveAt(i);
                            else if (ins.Operand is TypeReference tr && tr.FullName == deletedRef.FullName)
                                instructions.RemoveAt(i);
                        }
                    }

                    if (method.ReturnType.FullName == deletedRef.FullName)
                    {
                        method.ReturnType = module.TypeSystem.Object;
                        Console.WriteLine($"Changed return type of {method.Name} from deleted type to object");
                    }

                    for (int i = method.Parameters.Count - 1; i >= 0; i--)
                    {
                        if (method.Parameters[i].ParameterType.FullName == deletedRef.FullName)
                        {
                            method.Parameters.RemoveAt(i);
                            Console.WriteLine($"Removed parameter typed as {deletedRef.Name} in {method.Name}");
                        }
                    }
                }
            }

            Console.WriteLine("Reference cleanup pass finished. May still miss some (generics, locals, constraints).");
        }
        private static void StripDangerousAttributesOnWholeModule(ModuleDefinition module)
        {
            ProcessCustomAttributes(module.Assembly, "Assembly");

            ProcessCustomAttributes(module, "Module");
            foreach (TypeDefinition type in module.Types.ToArray())
            {
                // ────────────────────────────────────────────────
                // Fully delete the struct if found (and clean refs)
                // ────────────────────────────────────────────────

                if (type.FullName == "AncientMysteries.Utilities.AMStringHandler")
                {
                    Console.WriteLine("Attempting full removal of struct: AncientMysteries.Utilities.AMStringHandler");

                    bool removed = module.Types.Remove(type);
                    if (!removed)
                    {
                        foreach (TypeDefinition t in module.Types)
                            t.NestedTypes.Remove(type);
                    }

                    Console.WriteLine(removed ? "Type removed from collection." : "Type was nested or not found in top-level.");

                    CleanReferencesToType(module, type);
                    continue
                    ;
                }

                // ────────────────────────────────────────────────
                // Process attributes on remaining items
                // ────────────────────────────────────────────────
                ProcessCustomAttributes(type, $"Type: {type.FullName}");

                foreach (TypeDefinition nested in type.NestedTypes)
                    ProcessCustomAttributes(nested, $"Nested: {nested.FullName}");

                foreach (FieldDefinition f in type.Fields)
                    ProcessCustomAttributes(f, $"Field: {type.FullName}.{f.Name}");

                foreach (PropertyDefinition p in type.Properties)
                    ProcessCustomAttributes(p, $"Property: {type.FullName}.{p.Name}");

                foreach (EventDefinition e in type.Events)
                    ProcessCustomAttributes(e, $"Event: {type.FullName}.{e.Name}");

                foreach (MethodDefinition method in type.Methods)
                {
                    //Messy but functional causes issue otherwise related to 'AncientMysteries.Utilities.AMStringHandler' is declared in another module and needs to be imported
                    bool isTarget =
                        (type.FullName == "AncientMysteries.AncientMysteriesMod" &&
                         method.Name.Contains("g__UpdateModDisplayName|17_0")) ||

                        (type.FullName == "AncientMysteries.Hook.Patches.LSItem_Draw" &&
                         method.Name.Contains("g__GetName|2_0"));

                    if (isTarget)
                    {
                        Console.WriteLine($"Emptying: {method.Name} ({type.FullName})");

                        method.Body.Instructions.Clear();
                        method.Body.Variables.Clear();
                        method.Body.ExceptionHandlers.Clear();

                        ILProcessor il = method.Body.GetILProcessor();

                        if (method.Name.Contains("g__GetName|2_0"))
                        {
                            il.Append(il.Create(OpCodes.Ldstr, ""));
                            il.Append(il.Create(OpCodes.Ret));
                        }
                        else
                        {
                            il.Append(il.Create(OpCodes.Ret));
                        }
                    }

                    //"AncientMysteries.Module.Initialize
                    //FieldInfo field = typeof(AppDomain).GetField("_AssemblyResolve", BindingFlags.Instance | BindingFlags.NonPublic);
                    //ResolveEventHandler handlers = (ResolveEventHandler)field.GetValue(AppDomain.CurrentDomain);
                    //field.SetValue(AppDomain.CurrentDomain, Delegate.Combine(new ResolveEventHandler(Module.CurrentDomain_AssemblyResolve), handlers));
                    if (method.DeclaringType.FullName == "AncientMysteries.Module" &&
                        method.Name == "Initialize")
                    {
                        Console.WriteLine("Nopping AncientMysteries.Module.Initialize to avoid NRE");

                        method.Body.Instructions.Clear();
                        method.Body.Variables.Clear();
                        method.Body.ExceptionHandlers.Clear();

                        ILProcessor il = method.Body.GetILProcessor();
                        il.Append(il.Create(OpCodes.Ret));
                    }

                    // Process method attributes (after possible body changes)
                    ProcessCustomAttributes(method, $"Method: {type.FullName}.{method.Name}");

                    // ────────────────────────────────────────────────
                    // Dangerous ones still commented out
                    // ────────────────────────────────────────────────
                    // ProcessCustomAttributes(method.MethodReturnType, $"Return: {method.Name}");
                    // foreach (var param in method.Parameters)
                    //     ProcessCustomAttributes(param, $"Param in {method.Name}");
                    // foreach (var gp in method.GenericParameters)
                    //     ProcessCustomAttributes(gp, $"Generic param in {method.Name}");
                }
            }
        }
        public Assembly RelinkToAssemblyInMemory(ModuleDefinition mod) //unused
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

            Log("[Relink] Stripping dangerous attributes");
            StripDangerousAttributesOnWholeModule(mod);
            Log("[Relink] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(mod,type);
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
                Modder.Module.Write(memStream, Modder.WriterParameters, Modder.Module.Image.Stream.value.GetFileName()); //this.Modder.Module.Image.Stream.value.GetFileName()
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
            Log("[Relink] Stripping dangerous attributes");
            StripDangerousAttributesOnWholeModule(mod);
            Log("[Relink] Post-processing");
            foreach (TypeDefinition type in mod.Types)
                PostProcessType(mod, type);
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
                Modder.Module.Write(fs, Modder.WriterParameters); //this.Modder.Module.Image.Stream.value.GetFileName()
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
            public fuller()
            { }

            public ModuleDefinition dep;
        }
        private sealed class filler2
        {
            public filler2()
            { }

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
            public thing()
            { }

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
            public ugh()
            { }

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