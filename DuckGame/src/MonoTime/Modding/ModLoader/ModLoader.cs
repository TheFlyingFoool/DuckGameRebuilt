using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
//using XnaToFna;
using File = System.IO.File;

namespace DuckGame
{
    /// <summary>
    /// The class that handles mods to load, and allows mods to retrieve Mod objects.
    /// </summary>
    public static class ModLoader
    {
        //private static XnaToFnaUtil xnaToFnaUtil;
        private static readonly Dictionary<string, Mod> _loadedMods = new Dictionary<string, Mod>();
        internal static readonly Dictionary<string, Mod> _modAssemblyNames = new Dictionary<string, Mod>();
        internal static readonly Dictionary<Assembly, Mod> _modAssemblies = new Dictionary<Assembly, Mod>();
        internal static readonly Dictionary<uint, Mod> _modsByHash = new Dictionary<uint, Mod>();
        internal static readonly Dictionary<ulong, Mod> _modsByWorkshopID = new Dictionary<ulong, Mod>();
        private static Assembly[] _modAssemblyArray;
        internal static readonly Dictionary<Type, Mod> _modTypes = new Dictionary<Type, Mod>();
        private static IList<Mod> _sortedMods = new List<Mod>();
        private static IList<Mod> _sortedAccessibleMods = new List<Mod>();
        private static readonly List<Tuple<string, Exception>> _modLoadErrors = new List<Tuple<string, Exception>>();
        public static readonly Dictionary<string, Type> _typesByName = new Dictionary<string, Type>();
        public static readonly Dictionary<string, Type> _typesByNameUnprocessed = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, string> _namesByType = new Dictionary<Type, string>();
        /// <summary>
        /// Returns whether or not any mods are present and not disabled.
        /// </summary>
        private static int _numModsEnabled = 0;
        private static int _numModsTotal = 0;
        private static bool _preloading = false;
        internal static string _modString;
        private static List<ulong> brokenclientsidemods = new List<ulong>()
        {
            2291455300UL,
            2285058623UL,
            1704010547UL,
            1422187117UL,
            1217536842UL,
            1337905266UL,
            1653126591UL,
            1220996500UL,
            804666647UL,
            1198406315UL,
            1356415641UL
        };
        private static CSharpCodeProvider _provider = null;
        private static CompilerParameters _parameters = null;
        private static string _buildErrorText = null;
        private static string _buildErrorFile = null;
        internal static HashSet<string> disabledMods;
        internal static HashSet<string> forceLegacyLoad;
        internal static bool forceRecompilation = false;
        internal static string modDirectory;
        private static Dictionary<string, ModConfiguration> loadableMods;
        private static List<Mod> removeFromAccessible = new List<Mod>();
        public static bool runningModloadCode;
        private static List<Mod> initializationFailures = new List<Mod>();
        public static bool ignoreLegacyLoad = false;
        public static Mod loadingOldMod = null;
        public static string currentModLoadString = "";

        public static Assembly[] modAssemblyArray => _modAssemblyArray;

        public static void InitializeAssemblyArray()
        {
            if (_modAssemblyArray != null)
                return;
            HashSet<Assembly> source = new HashSet<Assembly>();
            source.Add(Assembly.GetExecutingAssembly());
            foreach (KeyValuePair<Assembly, Mod> modAssembly in _modAssemblies)
                source.Add(modAssembly.Key);
            _modAssemblyArray = source.ToArray();
        }

        internal static void AddMod(object p) =>
          throw new NotImplementedException();

        internal static List<Tuple<string, Exception>> modLoadErrors => _modLoadErrors;

        /// <summary>Get an iterable list of Mods</summary>
        public static IList<Mod> accessibleMods => _sortedAccessibleMods;

        public static IList<Mod> allMods => _sortedMods;

        public static int numModsEnabled => _numModsEnabled;

        public static bool modsEnabled => _numModsEnabled != 0;

        public static int numModsTotal => _numModsTotal;

        /// <summary>Get a loaded Mod instance from its unique name.</summary>
        /// <typeparam name="T">The special Mod subclass to cast to.</typeparam>
        /// <returns>The Mod instance, or null.</returns>
        public static T GetMod<T>() where T : Mod
        {
            Mod mod;
            _modTypes.TryGetValue(typeof(T), out mod);
            return (T)(object)mod;
        }

        /// <summary>Get a loaded Mod instance from its unique name.</summary>
        /// <param name="name">The name of the mod.</param>
        /// <returns>The Mod instance, or null.</returns>
        public static Mod GetMod(string name)
        {
            Mod mod;
            return _loadedMods.TryGetValue(name, out mod) ? mod : null;
        }

        public static void DisableModsAndRestart()
        {
            foreach (Mod mod in allMods)
            {
                if (mod.clientMod || mod.configuration.modType is ModConfiguration.Type.Reskin or ModConfiguration.Type.MapPack)
                    continue;
                
                mod.configuration.Disable();
            }

            RestartGame();
        }

        public static void RestartGame()
        {
            Process.Start(Application.ExecutablePath, Program.commandLine);
            Application.Exit();
            Program.main.KillEverything();
            Program.main.Exit();
        }

        internal static void AddMod(Mod mod)
        {
            if (_loadedMods.ContainsKey(mod.configuration.uniqueID))
                return;
            _loadedMods.Add(mod.configuration.uniqueID, mod);
            if (mod.configuration.disabled || mod is ErrorMod || _modAssemblies.ContainsKey(mod.configuration.assembly))
                return;
            _modAssemblyNames.Add(mod.configuration.assembly.FullName, mod);
            _modAssemblies.Add(mod.configuration.assembly, mod);
            _modsByHash.Add(mod.identifierHash, mod);
            if (mod.configuration.workshopID != 0UL)
                _modsByWorkshopID[mod.configuration.workshopID] = mod;
            _modTypes.Add(mod.GetType(), mod);
        }
        public static void FixLoadAssembly(ModConfiguration modConfig, string path)
        {
            //path = path.Replace("\\", "/");
            //string pathprimer = path.Replace(".dll", "");
            //string RebuiltDataPath = (pathprimer + "RebuiltData.txt");
            //string RebuiltAssemblyPath = (pathprimer + "Rebuilt.dll");
            //string modificationdatetime = File.GetLastWriteTime(path).ToString();
            //string saveddata = "";
            //string[] lines;
            //if (File.Exists(RebuiltDataPath))
            //{
            //    if (File.Exists(RebuiltAssemblyPath))
            //    {
            //        saveddata = File.ReadAllText(RebuiltDataPath);
            //        lines = saveddata.Split( new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //        if (lines.Length  >  1)
            //        {
            //            string saveinfo = lines[0];
            //            if (saveinfo.Contains("|") && lines[1] == "Type Order")
            //            {
            //                string[] splitdata = saveinfo.Split('|');
            //                if (splitdata.Length > 1 && splitdata[0] == modificationdatetime && splitdata[1] == XnaToFnaUtil.RemapVersion.ToString())
            //                {
            //                    byte[] filebytes = File.ReadAllBytes(RebuiltAssemblyPath);
            //                    if (filebytes.Length != 0)
            //                    {
            //                        modConfig.SortedTypeNames = new string[lines.Length - 2];
            //                        for (int i = 2; i < lines.Length; i++)
            //                        {
            //                            modConfig.SortedTypeNames[i - 2] = lines[i];
            //                        }
            //                        modConfig.assembly = Assembly.Load(filebytes);
            //                        return;
            //                    }
            //                }
            //            }
            //        }
            //        File.Delete(RebuiltAssemblyPath);
            //    }
            //    File.Delete(RebuiltDataPath);
            //}
            ////assemblyDefinition = this.ResolveFailure(this, name);
            //if (File.Exists(RebuiltAssemblyPath))
            //{
            //    File.Delete(RebuiltAssemblyPath);
            //}
            //Assembly normalmodassembly = Assembly.Load(File.ReadAllBytes(path));
            //Type[] modtypes = normalmodassembly.GetTypes();
            //modConfig.SortedTypeNames = new string[modtypes.Length];
            //string typeorder = "\nType Order";
            //for (int i = 0; i < modtypes.Length; i++)
            //{
            //    string typename = modtypes[i].FullName;
            //    modConfig.SortedTypeNames[i] = typename;
            //    typeorder += "\n" + typename;
            //}
            //normalmodassembly = null;
            //(modificationdatetime + " | " + XnaToFnaUtil.RemapVersion.ToString())
           // File.WriteAllText(RebuiltDataPath, (modificationdatetime + "|" + XnaToFnaUtil.RemapVersion.ToString()) + typeorder);
           // MonoMain.NloadMessage = "REMAPPING/LOADING MOD " + currentModLoadString + " " + saveddata;
         //   string folderpath = Path.GetDirectoryName(path);
        //    xnaToFnaUtil = new XnaToFnaUtil(folderpath); //Path.GetDirectoryName(path);
        //    //xnaToFnaUtil.ScanPath(Program.GameDirectory + "DGSteamref.dll");

        ////    xnaToFnaUtil.ScanPath("D:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\312530\\2674911202\\BrowseGamesPlus\\content\\0Harmony.dll");
            
        //    xnaToFnaUtil.ScanPath(Program.GameDirectory + "FNA.dll");
        //    xnaToFnaUtil.ScanPath(Program.FilePath);
        //    xnaToFnaUtil.RelinkAll();


        //    really really = new really
        //    {
        //        name = AssemblyName.GetAssemblyName(path)
        //    };
        //    ReaderParameters rp = xnaToFnaUtil.Modder.GenReaderParameters(false);
        //    (xnaToFnaUtil.Modder.AssemblyResolver as DefaultAssemblyResolver).AddSearchDirectory(folderpath);
        //    rp.ReadWrite = path != XnaToFnaUtil.ThisAssembly.Location && !xnaToFnaUtil.Mappings.Exists(new Predicate<XnaToFnaMapping>(really.ScanPath));
        //    rp.ReadSymbols = false;
        //    ModuleDefinition mod = MonoMod.ReadModule(path, rp);
        //    Assembly assembly = xnaToFnaUtil.RelinkToAssemblyToFile(mod, RebuiltAssemblyPath);
        //    (xnaToFnaUtil.Modder.AssemblyResolver as DefaultAssemblyResolver).RemoveSearchDirectory(folderpath);
        //    xnaToFnaUtil.Dispose();
        //    modConfig.assembly = assembly;
        }
        private static ModConfiguration GetDependency(
          string pDependency,
          ref Dictionary<string, ModConfiguration> pLoadableMods)
        {
            ModConfiguration dependency;
            if (!pLoadableMods.TryGetValue(pDependency, out dependency))
            {
                foreach (KeyValuePair<string, ModConfiguration> keyValuePair in pLoadableMods)
                {
                    if (keyValuePair.Key.StartsWith(pDependency))
                        return keyValuePair.Value;
                }
            }
            return dependency;
        }

        private static Mod GetOrLoad(
          ModConfiguration modConfig,
          ref Stack<string> modLoadStack,
          ref Dictionary<string, ModConfiguration> loadableMods)
        {
            if (modLoadStack.Contains(modConfig.uniqueID))
                throw new ModCircularDependencyException(modLoadStack);
            modLoadStack.Push(modConfig.uniqueID);
            try
            {
                Mod mod;
                if (_loadedMods.TryGetValue(modConfig.uniqueID, out mod))
                    return mod;
                if (_preloading)
                {
                    if (DGSave.upgradingFromVanilla)
                    {
                        modConfig.Disable();
                        DGSave.showModsDisabledMessage = true;
                    }
                    if (modConfig.error != null)
                        mod = new ErrorMod();
                    else if (MonoMain.nomodsMode)
                    {
                        mod = new ErrorMod();
                        modConfig.error = "-nomods command line enabled";
                    }
                    else if (modConfig.majorSupportedRevision != 1)
                    {
                        if (modConfig.workshopID == 1198406315UL || modConfig.workshopID == 1354346379UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks EIM64!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 1820667892UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks Yupdaniel!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 1603886916UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks Yupdaniel || Mr. Potatooh!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 796033146UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks TheSpicyChef!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 1425615438UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks EIM64 || Killer-Fackur!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 1704010547UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!Regrettably, this version of QOL is incompatible with Duck Game 2020!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 1657985708UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This mod has been officially implemented, Thanks Yupdaniel!";
                            mod = new DisabledMod();
                        }

                    }
                    /* rebuild mod issues */
                    //if (modConfig.workshopID == 2548159573UL) // custom arcade
                    //{
                    //    modConfig.Disable();
                    //    modConfig.error = "!This mod does not currently work on Rebuilt!";
                    //    mod = new DisabledMod();
                    //}
                    //if (modConfig.workshopID == 2320709295UL) // Better Chat the issue with this mod was its transpiler of Devconsole.Update which we kill now
                    //{
                    //    modConfig.Disable();
                    //    modConfig.error = "!This mod does not currently work on Rebuilt, Patching Issues!";
                    //    mod = new DisabledMod();
                    //}
                    if (modConfig.workshopID == 2480332949UL) //Delta Duck
                    {                 
                        modConfig.Disable();
                        modConfig.error = "!This mod does not currently work on Rebuilt, Just a Mess of Issues! @Tater";
                        mod = new DisabledMod();
                    }
                    else if (modConfig.workshopID == 2758180905UL) // Bug Fixes harmony issue patching menu elements stuff
                    {
                        modConfig.Disable();
                        modConfig.error = "!This mod does not currently work on Rebuilt, Patching Issues!";
                        mod = new DisabledMod();
                    }
                    else if (modConfig.workshopID == 2674911202UL) // BROWSE GAMES+ has a harmony resolve issue with remapper, but also scuffed issues that exist sepreatly
                    {
                        modConfig.Disable();
                        modConfig.error = "!This mod does not currently work on Rebuilt, Patching Issues!";
                        mod = new DisabledMod();
                    }
                    else if (modConfig.workshopID == 1439906266UL && Program.isLinux && !modConfig.linuxFix) // Reskins
                    {
                        modConfig.Disable();
                        modConfig.error = "!This mod does not currently work on Linux!";
                        mod = new DisabledMod();
                    }

                    //Patchs in these mods Dont Like the Debugger
                    if (Debugger.IsAttached)
                    {
                        if (modConfig.name == "QOL")
                        {
                            modConfig.Disable();
                            modConfig.error = "!This is Disabled mod is Disabled when Debugging!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 2411996803UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This is Disabled mod is Disabled when Debugging!";
                            mod = new DisabledMod();
                        }
                        else if (modConfig.workshopID == 2381384850UL)
                        {
                            modConfig.Disable();
                            modConfig.error = "!This is Disabled mod is Disabled when Debugging!";
                            mod = new DisabledMod();
                        }
                    }
                }
                if (mod == null)
                {
                    if (modConfig.disabled)
                        mod = new DisabledMod();
                    else if (modConfig.modType == ModConfiguration.Type.Reskin)
                    {
                        MonoMain.NloadMessage = "LOADING RESKIN " + currentModLoadString;
                        mod = ReskinPack.LoadReskin(modConfig.directory, pExistingConfig: modConfig);
                    }
                    else if (modConfig.modType == ModConfiguration.Type.MapPack)
                    {
                        MonoMain.NloadMessage = "LOADING MAPPACK " + currentModLoadString;
                        mod = MapPack.LoadMapPack(modConfig.directory, pExistingConfig: modConfig);
                    }
                    else if (!_preloading)
                    {
                        MonoMain.NloadMessage = "LOADING MOD " + currentModLoadString;
                        LoadedMods.Add(modConfig);
                        try
                        {
                            foreach (string hardDependency in modConfig.hardDependencies)
                            {
                                ModConfiguration dependency = GetDependency(hardDependency, ref loadableMods);
                                if (dependency == null)
                                    throw new ModDependencyNotFoundException(modConfig.uniqueID, hardDependency);
                                if (dependency.disabled)
                                    throw new ModDependencyNotFoundException(modConfig.uniqueID, hardDependency);
                                GetOrLoad(dependency, ref modLoadStack, ref loadableMods);
                            }
                            foreach (string softDependency in modConfig.softDependencies)
                            {
                                ModConfiguration dependency = GetDependency(softDependency, ref loadableMods);
                                if (dependency != null && !dependency.disabled)
                                    GetOrLoad(dependency, ref modLoadStack, ref loadableMods);
                            }
                            string assemblypath = modConfig.isDynamic ? modConfig.tempAssemblyPath : modConfig.assemblyPath;
                           // if (modConfig.noRecompilation)
                           // {
                            modConfig.assembly = Assembly.Load(File.ReadAllBytes(assemblypath));
                           // }
                            //else
                            //{
                            //    FixLoadAssembly(modConfig, assemblypath);
                            //}
                            MonoMain.loadedModsWithAssemblies.Add(modConfig);
                            Type[] array1 = modConfig.assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(IManageContent)) && type.IsPublic && type.IsClass && !type.IsAbstract).ToArray();
                            modConfig.contentManager = array1.Length <= 1 ? ContentManagers.GetContentManager(array1.Length == 1 ? array1[0] : null) : throw new ModTypeMissingException(modConfig.uniqueID + " has more than one content manager class");
                            Type[] array2 = modConfig.assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(Mod)) && !type.IsAbstract).ToArray();
                            if (array2.Length != 1)
                                throw new ModTypeMissingException(modConfig.uniqueID + " is missing or has more than one Mod subclass");
                            if (!modConfig.noPreloadAssets)
                            {
                                if (MonoMain.preloadModContent && modConfig.preloadContent)
                                    modConfig.content.PreloadContent();
                                else
                                    modConfig.content.PreloadContentPaths();
                            }
                            mod = (Mod)Activator.CreateInstance(array2[0]);
                            if (mod is DisabledMod || mod is CoreMod || mod is ErrorMod)
                            {
                                mod.clientMod = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            mod = new ErrorMod();
                            modConfig.error = ex.ToString();
                        }
                    }
                }
                if (mod == null)
                    return null;
                mod.configuration = modConfig;
                AddMod(mod);
                return mod;
            }
            finally
            {
                modLoadStack.Pop();
            }
        }

        private static string GetModHash()
        {
            if (!modsEnabled)
                return "nomods";
            using (SHA256 shA256Cng = SHA256.Create())
            {
                _modString = string.Join("|", _sortedAccessibleMods.Where(a =>
                {
                    switch (a)
                    {
                        case CoreMod _:
                        case DisabledMod _:
                            return false;
                        default:
                            return !brokenclientsidemods.Contains(a.configuration.workshopID);
                    }
                }).Select(a => a.configuration.uniqueID + "_" + a.configuration.version?.ToString()).OrderBy(x => x));
                return string.IsNullOrEmpty(_modString) ? "nomods" : Convert.ToBase64String(shA256Cng.ComputeHash(Encoding.ASCII.GetBytes(_modString)));
            }
        }

        public static bool ShouldOptimizations;
        internal static string modHash
        {
            get;
            private set;
        }

        public static string SmallTypeName(string fullName)
        {
            int length = fullName.IndexOf(',', fullName.IndexOf(',') + 1);
            return length == -1 ? null : fullName.Substring(0, length);
        }

        internal static string SmallTypeName(Type type)
        {
            if (!MonoMain.moddingEnabled)
                return SmallTypeName(type.AssemblyQualifiedName);
            string str;
            _namesByType.TryGetValue(type, out str);
            return str;
        }

        private static bool AttemptCompile(ModConfiguration config)
        {
            _buildErrorText = null;
            _buildErrorFile = null;
            if (config.noCompilation)
                return false;
            List<string> filesNoCloud = DuckFile.GetFilesNoCloud(config.directory, "*.cs", SearchOption.AllDirectories);
            if (filesNoCloud.Count == 0)
                return false;
            config.isDynamic = true;
            CRC32 crC32 = new CRC32();
            byte[] numArray = new byte[2048];
            foreach (string path in filesNoCloud)
            {
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                {
                    while (fileStream.Position != fileStream.Length)
                    {
                        int blockLen = fileStream.Read(numArray, 0, numArray.Length);
                        crC32.ProcessBlock(numArray, blockLen);
                    }
                }
            }
            uint num = crC32.Finalize();
            if (!forceRecompilation && File.Exists(config.hashPath))
            {
                if (File.Exists(config.tempAssemblyPath))
                {
                    try
                    {
                        if ((int)BitConverter.ToUInt32(File.ReadAllBytes(config.hashPath), 0) == (int)num)
                            return true;
                    }
                    catch { }
                }
            }
            File.WriteAllBytes(config.hashPath, BitConverter.GetBytes(num));
            if (_provider == null)
            {
                _provider = new CSharpCodeProvider();
                _parameters = new CompilerParameters(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.Location).ToArray());
                _parameters.GenerateExecutable = _parameters.GenerateInMemory = false;
            }
            if (File.Exists(config.buildLogPath))
            {
                File.SetAttributes(config.buildLogPath, FileAttributes.Normal);
                File.Delete(config.buildLogPath);
            }
            _parameters.OutputAssembly = config.tempAssemblyPath;
            CompilerResults compilerResults = _provider.CompileAssemblyFromFile(_parameters, filesNoCloud.ToArray());
            if (compilerResults.Errors.Count != 0)
            {
                bool flag = false;
                foreach (CompilerError error in (CollectionBase)compilerResults.Errors)
                {
                    if (!error.IsWarning)
                    {
                        flag = true;
                        _buildErrorFile = DuckFile.PreparePath(error.FileName);
                        _buildErrorText = error.ErrorText;
                        break;
                    }
                }
                File.WriteAllLines(config.buildLogPath, compilerResults.Output.OfType<string>());
                if (flag)
                    return false;
            }
            return true;
        }

        private static ModConfiguration AttemptModLoad(string folder)
        {
            ModConfiguration modConfiguration = new ModConfiguration()
            {
                directory = folder
            };
            modConfiguration.contentDirectory = modConfiguration.directory + "/content/";
            if (File.Exists(modConfiguration.contentDirectory + "/New Text Document.tpconf"))
                modConfiguration.isExistingReskinMod = true;
            else if (File.Exists(folder + "/info.txt") && DuckFile.GetFiles(folder, "*.dll").Length == 0)
                modConfiguration.SetModType(ModConfiguration.Type.Reskin);
            else if (File.Exists(folder + "/mappack_info.txt") && DuckFile.GetFiles(folder, "*.dll").Length == 0)
                modConfiguration.SetModType(ModConfiguration.Type.MapPack);
            else if (File.Exists(folder + "/hatpack_info.txt") && DuckFile.GetFiles(folder, "*.dll").Length == 0)
                modConfiguration.SetModType(ModConfiguration.Type.HatPack);
            modConfiguration.name = Path.GetFileNameWithoutExtension(folder);
            modConfiguration.content = new ContentPack(modConfiguration);
            try
            {
                if (modConfiguration.name == "DuckGame")
                    return null;
                currentModLoadString = modConfiguration.name;
                modConfiguration.LoadConfiguration();
                if (!modConfiguration.disabled && modConfiguration.modType == ModConfiguration.Type.Mod)
                {
                    if (!File.Exists(modConfiguration.assemblyPath) && !MonoMain.nomodsMode)
                    {
                        MonoMain.NloadMessage = "COMPILING MOD " + currentModLoadString;
                        if (!AttemptCompile(modConfiguration))
                            modConfiguration.error = _buildErrorText + "\n\nFile: " + _buildErrorFile + "\nNote: Assembly (" + Path.GetFileName(modConfiguration.assemblyPath) + ") was not found, and a compile was attempted.";
                    }
                    if (modConfiguration.error == null)
                        ++_numModsEnabled;
                }
                ++_numModsTotal;
                return modConfiguration;
            }
            catch (Exception ex)
            {
                _modLoadErrors.Add(Tuple.Create(modConfiguration.uniqueID, ex));
            }
            return null;
        }

        internal static void LoadConfig()
        {
            XmlDocument pDoc = null;
            XmlElement newChild = null;
            bool flag = File.Exists(modConfigFile);
            if (flag)
            {
                try
                {
                    pDoc = DuckFile.LoadSharpXML(modConfigFile);
                    newChild = pDoc["Mods"];
                }
                catch (Exception)
                {
                    LogModFailure("Failure loading main mod config file. Recreating file.");
                    flag = false;
                }
            }
            if (!flag)
            {
                pDoc = new XmlDocument();
                newChild = pDoc.CreateElement("Mods");
                newChild.AppendChild(pDoc.CreateElement("Disabled"));
                newChild.AppendChild(pDoc.CreateElement("ForceLegacyLoad"));
                newChild.AppendChild(pDoc.CreateElement("CompiledFor"));
                newChild["CompiledFor"].InnerText = DG.version;
                pDoc.AppendChild(newChild);
                DuckFile.SaveSharpXML(pDoc, modConfigFile);
            }
            if (newChild["Disabled"] != null)
                disabledMods = new HashSet<string>(newChild["Disabled"].InnerText.Split(new char[1] {
          '|'
        }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            else
                disabledMods = new HashSet<string>();
            if (newChild["ForceLegacyLoad"] != null)
                forceLegacyLoad = new HashSet<string>(newChild["ForceLegacyLoad"].InnerText.Split(new char[1] {
          '|'
        }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            else
                forceLegacyLoad = new HashSet<string>();
            if (newChild["CompiledFor"] == null)
                newChild.AppendChild(pDoc.CreateElement("CompiledFor"));
            if (!(newChild["CompiledFor"].InnerText != DG.version))
                return;
            forceRecompilation = true;
            newChild["CompiledFor"].InnerText = DG.version;
            DuckFile.SaveSharpXML(pDoc, modConfigFile);
        }

        internal static void SetModDisabled(ModConfiguration pMod, bool pDisabled)
        {
            XmlDocument pDoc = DuckFile.LoadSharpXML(modConfigFile);
            XmlElement xmlElement = pDoc["Mods"];
            if (xmlElement["Disabled"] == null)
                xmlElement.AppendChild(pDoc.CreateElement("Disabled"));
            string uniqueId = pMod.uniqueID;
            List<string> list = xmlElement["Disabled"].InnerText.Split('|').ToList();
            if (!pDisabled)
            {
                string str1 = uniqueId;
                char[] chArray = new char[1] {
          '|'
        };
                foreach (string str2 in str1.Split(chArray))
                    list.Remove(str2);
            }
            else
            {
                string str3 = uniqueId;
                char[] chArray = new char[1] {
          '|'
        };
                foreach (string str4 in str3.Split(chArray))
                {
                    if (!list.Contains(str4))
                        list.Add(str4);
                }
            }
            xmlElement["Disabled"].InnerText = string.Join("|", list);
            if (xmlElement["ForceLegacyLoad"] == null)
                xmlElement.AppendChild(pDoc.CreateElement("ForceLegacyLoad"));
            xmlElement["ForceLegacyLoad"].InnerText = string.Join("|", _sortedMods.Where(a => a.configuration.forceHarmonyLegacyLoad).Select(a => a.configuration.uniqueID));
            DuckFile.SaveSharpXML(pDoc, modConfigFile);
        }

        internal static void DisabledModsChanged()
        {
            XmlDocument pDoc = DuckFile.LoadSharpXML(modConfigFile);
            XmlElement xmlElement = pDoc["Mods"];
            if (xmlElement["Disabled"] == null)
                xmlElement.AppendChild(pDoc.CreateElement("Disabled"));
            xmlElement["Disabled"].InnerText = string.Join("|", _sortedMods.Where(a => a.configuration.disabled).Select(a => a.configuration.uniqueID));
            if (xmlElement["ForceLegacyLoad"] == null)
                xmlElement.AppendChild(pDoc.CreateElement("ForceLegacyLoad"));
            xmlElement["ForceLegacyLoad"].InnerText = string.Join("|", _sortedMods.Where(a => a.configuration.forceHarmonyLegacyLoad).Select(a => a.configuration.uniqueID));
            DuckFile.SaveSharpXML(pDoc, modConfigFile);
        }

        internal static string modConfigFile => modDirectory + "/mods.conf";

        private static void ResultFetched(object value0, WorkshopQueryResult result)
        {
            if (result == null || result.details == null)
                return;
            WorkshopItem publishedFile = result.details.publishedFile;
            if (publishedFile == null)
                return;
            try
            {
                if ((publishedFile.stateFlags & WorkshopItemState.Installed) == WorkshopItemState.None || !Directory.Exists(publishedFile.path))
                    return;
                foreach (string folder in DuckFile.GetDirectoriesNoCloud(publishedFile.path))
                {
                    ModConfiguration modConfiguration = AttemptModLoad(folder);
                    if (modConfiguration != null)
                    {
                        try
                        {
                            modConfiguration.isWorkshop = true;
                            loadableMods.Add(modConfiguration.uniqueID, modConfiguration);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception) { }
        }

        public static void FailWithHarmonyException()
        {
            if (loadingOldMod == null)
                throw new OldModUsesHarmonyException("A Mod for the old version of Duck Game uses Harmony. This could be risky! Use 'Force Legacy Load' and restart to load it anyway.");
            if (!loadingOldMod.configuration.forceHarmonyLegacyLoad || ignoreLegacyLoad)
                throw new OldModUsesHarmonyException("Mod is for an old version of Duck Game, and appears to use Harmony patching. This could be risky! Use 'Force Legacy Load' and restart to load it anyway.");
        }
        public static List<ModConfiguration> LoadedMods = new();
        public static void PreLoadMods(string dir)
        {
            modDirectory = dir;
            LoadConfig();
            loadableMods = new Dictionary<string, ModConfiguration>();
            if (Directory.Exists(modDirectory))
            {
                if (Steam.IsInitialized())
                {
                    LoadingAction steamLoad = new LoadingAction();
                    steamLoad.action = () =>
                    {
                        runningModloadCode = true;//WorkshopList.Subscribed
                        WorkshopQueryUser queryUser = Steam.CreateQueryUser(Steam.user.id, WorkshopList.Subscribed, WorkshopType.UsableInGame, WorkshopSortOrder.TitleAsc);
                        queryUser.requiredTags.Add("Mod");
                        queryUser.onlyQueryIDs = true;
                        queryUser.QueryFinished += sender => steamLoad.flag = true;
                        queryUser.ResultFetched += new WorkshopQueryResultFetched(ResultFetched);
                        queryUser.Request();
                        Steam.Update();
                    };
                    steamLoad.waitAction = () =>
                    {
                        Steam.Update();
                        return steamLoad.flag;
                    };
                    steamLoad.label = "Querying WorkShop";
                    MonoMain.currentActionQueue.Enqueue(steamLoad);
                }
                LoadingAction attemptLoadMods = new LoadingAction();
                MonoMain.currentActionQueue.Enqueue(attemptLoadMods);
                attemptLoadMods.action = () =>
                {
                    runningModloadCode = true;
                    List<string> directoriesNoCloud = DuckFile.GetDirectoriesNoCloud(modDirectory);
                    directoriesNoCloud.AddRange(DuckFile.GetDirectoriesNoCloud(DuckFile.globalModsDirectory));
                    MonoMain.totalLoadyBits += directoriesNoCloud.Count * 2;
                    foreach (string str in directoriesNoCloud)
                    {
                        string folder = str;
                        if (!folder.ToLowerInvariant().EndsWith("/texpacks") && !folder.ToLowerInvariant().EndsWith("/mappacks") && !folder.ToLowerInvariant().EndsWith("/hatpacks"))
                            attemptLoadMods.actions.Enqueue(new LoadingAction(() =>
                            {
                                ModConfiguration modConfiguration = AttemptModLoad(folder);
                                MonoMain.loadyBits += 2;
                                if (modConfiguration == null)
                                    return;
                                if (loadableMods.ContainsKey(modConfiguration.uniqueID))
                                {
                                    if (!loadableMods[modConfiguration.uniqueID].disabled || modConfiguration.disabled)
                                        return;
                                    loadableMods.Remove(modConfiguration.uniqueID);
                                }
                                loadableMods.Add(modConfiguration.uniqueID, modConfiguration);
                            }, null, "Loading Mods"));
                    }
                };
                attemptLoadMods.label = "Loading Mod stuff to load mod stuff";
            }
            MonoMain.currentActionQueue.Enqueue(new LoadingAction(() => ReskinPack.InitializeReskins(),null, "Initialize Reskins"));
            MonoMain.currentActionQueue.Enqueue(new LoadingAction(() => MapPack.InitializeMapPacks(), null, "Initialize MapPacks"));
            GetOrLoadMods(true);
        }

        private static void GetOrLoadMods(bool pPreload)
        {
            Stack<string> modLoadStack = new Stack<string>();
            LoadingAction getOrLoadMods = new LoadingAction();
            MonoMain.currentActionQueue.Enqueue(getOrLoadMods);
            getOrLoadMods.action = () =>
            {
                _preloading = pPreload;
                MonoMain.totalLoadyBits += loadableMods.Count * 2;
                //int cluster = 0;
                List<ReskinPack> active = ReskinPack.active;
                foreach (ModConfiguration modConfiguration in loadableMods.Values)
                {
                    ModConfiguration loadable = modConfiguration;
                    getOrLoadMods.actions.Enqueue(new LoadingAction(() =>
                    {
                        try
                        {
                            currentModLoadString = loadable.name;
                            Mod orLoad = GetOrLoad(loadable, ref modLoadStack, ref loadableMods);
                            if (orLoad != null && loadable.isExistingReskinMod && !loadable.disabled && !(orLoad is ErrorMod))
                                ReskinPack.LoadReskin(loadable.contentDirectory + "tp/", orLoad);
                            MonoMain.loadyBits += 2;
                            //++cluster;
                            //if (cluster != 10)
                            //    return;
                            //cluster = 0;
                            //Thread.Sleep(50);
                        }
                        catch (Exception)
                        {
                            if (!Options.Data.disableModOnLoadFailure)
                                return;
                            loadable.Disable();
                        }
                    }, null, "Loading Reskin stuff"));
                }
            };
            getOrLoadMods.label = "Setting up Loading Reskin stuff";
        }

        internal static void LoadMods(string dir)
        {
            GetOrLoadMods(false);
            MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
            {
                InitializeAssemblyArray();
                ReskinPack.FinalizeReskins();
                _sortedMods = _loadedMods.Values.OrderBy(mod => (int)(mod.priority + (!mod.configuration.disabled || mod is ErrorMod ? -1000 : 0))).ToList();
                _sortedAccessibleMods = _sortedMods.Where(mod => !mod.configuration.disabled && !(mod is ErrorMod)).ToList();
                foreach (Mod mod in _sortedMods.Where(a => a.configuration.disabled || a is ErrorMod))
                {
                    if (mod != null && mod.configuration != null)
                        _loadedMods.Remove(mod.configuration.uniqueID);
                }
            }, null, "ModLoader sorting Initialize stuff"));
            LoadingAction preInitializeMods = new LoadingAction();
            MonoMain.currentActionQueue.Enqueue(preInitializeMods);
            preInitializeMods.action = () =>
            {
                foreach (Mod sortedAccessibleMod in (IEnumerable<Mod>)_sortedAccessibleMods)
                {
                    Mod mod = sortedAccessibleMod;
                    preInitializeMods.actions.Enqueue(new LoadingAction(() =>
                    {
                        try
                        {
                            AssemblyName assemblyName = mod.GetType().Assembly.GetReferencedAssemblies().FirstOrDefault(x => x.Name == "DuckGame");
                            if (assemblyName != null && assemblyName.Version.Minor != DG.versionMajor)
                            {
                                loadingOldMod = mod;
                                if (Directory.GetFiles(mod.configuration.directory, "0Harmony.dll", SearchOption.AllDirectories).FirstOrDefault() != null)
                                    FailWithHarmonyException();
                            }
                            mod.InvokeOnPreInitialize();
                        }
                        catch (Exception ex)
                        {
                            mod.configuration.error = ex.ToString();
                            if (Options.Data.disableModOnLoadFailure)
                                mod.configuration.Disable();
                            if (MonoMain.modDebugging)
                                throw new ModException(mod.configuration.name + " OnPreInitialize failed with exception:", mod.configuration, ex);
                        }
                        loadingOldMod = null;
                    }, null, "ModLoader InvokeOnPreInitialize"));
                }
                preInitializeMods.label = "preInitializeMods setup";
            };
            MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
            {
                foreach (Mod initializationFailure in initializationFailures)
                    _sortedAccessibleMods.Remove(initializationFailure);
                
                modHash = GetModHash();
                foreach (Mod sortedAccessibleMod in (IEnumerable<Mod>)_sortedAccessibleMods)
                {
                    string[] strArray = null;
                    if (sortedAccessibleMod.namespaceFacade != null)
                    {
                        try
                        {
                            strArray = sortedAccessibleMod.namespaceFacade.Split(':');
                            strArray[0] = strArray[0].Trim();
                            strArray[1] = strArray[1].Trim();
                        }
                        catch (Exception)
                        {
                            strArray = null;
                        }
                    }
                    try
                    {
                        foreach (Type type in sortedAccessibleMod.configuration.assembly.GetTypes())
                        {
                            string key1 = SmallTypeName(type.AssemblyQualifiedName);
                            _typesByNameUnprocessed[key1] = type;
                            if (Program.isLinux && !key1.Contains("\""))
                            {
                                string key2 = key1.Insert(key1.IndexOf(", ") + 2, "\"") + "\"";
                                _typesByNameUnprocessed[key2] = type;
                            }
                            if ((sortedAccessibleMod.assemblyNameFacade != null || sortedAccessibleMod.namespaceFacade != null) && type.Namespace != null)
                            {
                                int num = key1.IndexOf(',');
                                string str1 = key1.Substring(num + 2, key1.Length - (num + 2));
                                string str2 = "";
                                if (strArray != null)
                                    str2 = type.Namespace.Replace(strArray[0], strArray[1]) + "." + type.Name;
                                if (sortedAccessibleMod.assemblyNameFacade != null)
                                    str1 = sortedAccessibleMod.assemblyNameFacade;
                                key1 = str2 + ", " + str1;
                            }
                            _typesByName[key1] = type;
                            if (Program.isLinux && !key1.Contains("\""))
                            {
                                string key3 = key1.Insert(key1.IndexOf(", ") + 2, "\"") + "\"";
                                _typesByName[key3] = type;
                            }
                            _namesByType[type] = key1;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is ReflectionTypeLoadException)
                        {
                            DevConsole.Log(DCSection.General, "ModLoader.Load Assembly.GetAssemblies crashed with above exception-");
                            throw (ex as ReflectionTypeLoadException).LoaderExceptions.FirstOrDefault();
                        }
                    }
                }
                runningModloadCode = false;
            }, null, "ModLoader Load Mod Types"));
        }

        private static void LogModFailure(string s)
        {
            try
            {
                Program.LogLine("Mod Load Failure (Did not cause crash)\n================================================\n " + s + "\n================================================\n");
            }
            catch (Exception) { }
        }

        internal static void PostLoadMods()
        {
            runningModloadCode = true;
            foreach (Mod sortedAccessibleMod in (IEnumerable<Mod>)_sortedAccessibleMods)
            {
                try
                {
                    sortedAccessibleMod.InvokeOnPostInitialize();
                }
                catch (Exception ex)
                {
                    if (ex is FileNotFoundException)
                    {
                        if ((ex as FileNotFoundException).FileName.StartsWith("Steam,"))
                            continue;
                    }
                    sortedAccessibleMod.configuration.error = ex.ToString();
                    if (Options.Data.disableModOnLoadFailure)
                        sortedAccessibleMod.configuration.Disable();
                    throw new ModException(sortedAccessibleMod.configuration.name + " Mod.OnPostInitialize failed with exception:", sortedAccessibleMod.configuration, ex);
                }
            }
            foreach (Mod mod in removeFromAccessible)
                _sortedAccessibleMods.Remove(mod);
            if (!modsEnabled)
                modHash = "nomods";
            runningModloadCode = false;
            if (Program.lateCrash)
            {
                throw new Exception("Late crash used!");
            }

            ShouldOptimizations = modHash == "nomods" || modHash == "";
        }

        internal static void Start()
        {
            List<Mod> modList = new List<Mod>();
            foreach (Mod sortedAccessibleMod in (IEnumerable<Mod>)_sortedAccessibleMods)
            {
                try
                {
                    sortedAccessibleMod.InvokeStart();
                }
                catch (Exception ex)
                {
                    if (ex is FileNotFoundException)
                    {
                        if ((ex as FileNotFoundException).FileName.StartsWith("Steam,"))
                            continue;
                    }
                    sortedAccessibleMod.configuration.error = ex.ToString();
                    if (Options.Data.disableModOnLoadFailure)
                        sortedAccessibleMod.configuration.Disable();
                    throw new ModException(sortedAccessibleMod.configuration.name + " Mod.InvokeStart failed with exception:", sortedAccessibleMod.configuration, ex);
                }
            }
            foreach (Mod mod in modList)
                _sortedAccessibleMods.Remove(mod);
            if (modsEnabled)
                return;
            modHash = "nomods";
        }

        /// <summary>
        /// Searches core and mods for a fully qualified or short type name.
        /// </summary>
        /// <param name="typeName">Fully qualified, or short, name of the type.</param>
        /// <returns>The type, or null.</returns>
        internal static Type GetType(string typeName)
        {
            if (typeName == null)
                return null;
            if (typeName.LastIndexOf(',') != typeName.IndexOf(','))
                typeName = SmallTypeName(typeName);
            if (typeName == null)
                return null;
            int stringlength = typeName.Length;
            if (Program.gameAssemblyName != "DuckGame" && stringlength > 9 && typeName.Substring(stringlength - 10, 10) == ", DuckGame") // added so the game can handle it when the assembly has a different name 
                typeName = typeName.Substring(0, stringlength - 10) + ", " + Program.gameAssemblyName; // replaces it with current assembly name   
            Type type;
            if (!_typesByName.TryGetValue(typeName, out type) && !_typesByNameUnprocessed.TryGetValue(typeName, out type))
            {
                return Type.GetType(typeName);
            }
            return type;
        }

        /// <summary>Gets a mod from a type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The mod</returns>
        public static Mod GetModFromType(Type type)
        {
            if (type == null)
                return null;
            Mod mod;
            return _modAssemblies.TryGetValue(type.Assembly, out mod) ? mod : null;
        }

        /// <summary>Gets a mod from a type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The mod</returns>
        public static Mod GetModFromTypeIgnoreCore(Type type)
        {
            if (type == null)
                return null;
            Mod mod;
            if (!_modAssemblies.TryGetValue(type.Assembly, out mod))
                return null;
            return mod is CoreMod ? null : mod;
        }

        /// <summary>Gets a mod from a hash value.</summary>
        /// <param name="pHash">A 'Mod.identifierHash' value.</param>
        /// <returns>The mod</returns>
        public static Mod GetModFromHash(uint pHash)
        {
            Mod modFromHash;
            _modsByHash.TryGetValue(pHash, out modFromHash);
            return modFromHash;
        }

        /// <summary>Gets a mod from a workshopID value.</summary>
        /// <param name="pHash">A 'Mod.configuration.workshopID' value.</param>
        /// <returns>The mod</returns>
        public static Mod GetModFromWorkshopID(ulong pID)
        {
            Mod modFromWorkshopId;
            _modsByWorkshopID.TryGetValue(pID, out modFromWorkshopId);
            return modFromWorkshopId;
        }
    }
}
