using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace DuckGame
{
    /// <summary>
    /// An entry of mod configuration data contained in the mod and
    /// stored during the loading of a mod.
    /// </summary>
    public class ModConfiguration
    {
        private MapPack _mapPack;
        public bool isExistingReskinMod;
        public bool isHighResReskin;
        /// <summary>The full path to the content directory of this mod.</summary>
        private string _contentDirectory;
        private string _uniqueID;
        private Assembly _assembly;
        public string[] SortedTypeNames = new string[0]; // for DG Compatiablity as DGR rebuilding mods alters the order

        /// <summary>The type of mod this is.</summary>
        public Type modType { get; internal set; }

        public void SetModType(Type pType) => modType = pType;

        public MapPack mapPack => _mapPack;

        public void SetMapPack(MapPack pPack) => _mapPack = pPack;

        internal ModConfiguration()
        {
            softDependencies = (new string[0]);
            hardDependencies = (new string[0]);
        }

        /// <summary>The full path to the root directory of this mod.</summary>
        public string directory { get; internal set; }

        public string contentDirectory
        {
            get => _contentDirectory;
            internal set
            {
                if (value != null)
                    _contentDirectory = value.Replace('\\', '/');
                else
                    _contentDirectory = value;
            }
        }

        /// <summary>
        /// The isolated mod folder name; will be unique, obviously, and is how the
        /// mod will be referenced by other mods.
        /// </summary>
        public string name { get; internal set; }

        public string uniqueID
        {
            get
            {
                _uniqueID = name + "|" + workshopID.ToString();
                if (workshopID == 0UL)
                {
                    if (author != null)
                        _uniqueID = _uniqueID + "_" + author.ToString();
                    if (version != null)
                        _uniqueID = _uniqueID + "_" + version.ToString();
                }
                return _uniqueID;
            }
        }

        /// <summary>
        /// The display name of this mod. Does not have to be unique.
        /// </summary>
        public string displayName { get; internal set; }

        /// <summary>The version of this mod.</summary>
        public Version version { get; internal set; }

        /// <summary>The name of the author.</summary>
        public string author { get; internal set; }

        /// <summary>A descriptive piece of text about the mod.</summary>
        public string description { get; internal set; }

        /// <summary>
        /// A list of mod Name's that this mod may interact with via reflection, but
        /// does not require the mod to be loaded to properly run.
        /// </summary>
        public IEnumerable<string> softDependencies { get; internal set; }

        /// <summary>
        /// A list of mod Name's that this mod requires to load. It may use hard-linked
        /// references to classes contained in the mod.
        /// </summary>
        public IEnumerable<string> hardDependencies { get; internal set; }

        /// <summary>Gets the Steam workshop identifier.</summary>
        /// <value>The Steam workshop identifier.</value>
        internal ulong workshopID { get; set; }

        public ulong assignedWorkshopID => workshopID;

        /// <summary>
        /// Gets a value indicating whether this mod is a local mod.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a Workshop mod; otherwise, <c>false</c>.
        /// </value>
        public bool isWorkshop { get; internal set; }

        /// <summary>
        /// The content manager for this mod. Stores references to any assets used.
        /// </summary>
        public ContentPack content { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:DuckGame.Mod" /> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disabled; otherwise, <c>false</c>.
        /// </value>
        internal bool disabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:DuckGame.ModConfiguration" /> is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool loaded { get; internal set; }

        /// <summary>
        /// Gets an error message string for when <see cref="T:DuckGame.ModConfiguration" /> fails to load.
        /// </summary>
        /// <value>A string representing the error, or 'null' if no error.</value>
        public string error { get; set; }

        /// <summary>
        /// If true, this mod will load regardless of old Duck Game version Harmony related crashes
        /// </summary>
        /// <value>
        ///   <c>true</c> if mod will continue to load, even if it's for an old version of Duck Game and it uses Harmony code patching; otherwise, <c>false</c>.
        /// </value>
        public bool forceHarmonyLegacyLoad { get; set; }

        /// <summary>
        /// The major revision of Duck Game that this mod supports. Pre 2020 is 0, Post 2020 is 1. If Duck Game is automatically disabling your
        /// mod due to incompatibility, you should add a MajorSupportedRevision tag to your mod.conf with a value of 1.
        /// </summary>
        public int majorSupportedRevision { get; set; }

        /// <summary>
        /// If your mod has been flagged as incompatible in DG, set this to true after fixing the issues to bypass the compatibility check.
        /// </summary>
        internal bool linuxFix { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:DuckGame.Mod" /> should not try to compile itself
        /// </summary>
        /// <value>
        ///   <c>true</c> if mod will not try to compile itself; otherwise, <c>false</c>.
        /// </value>
        public bool noRecompilation { get; internal set; }
        public bool noPreloadAssets { get; internal set; }
        public bool noCompilation { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this mod will preload its content.
        /// With preloading enabled, the content in this mod will be loaded at the start of the game. This will
        /// increase load times, but will prevent stuttering when loading custom content in game. preloadContent is true by default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this mod will preload its content; otherwise, <c>false</c>.
        /// </value>
        public bool preloadContent { get; internal set; }

        /// <summary>
        /// When this is set to true, the color (255, 0, 255) will be replaced with transparency in any textures.
        /// This will affect performance when loading, turn it off for better performance. processPinkTransparency is true by default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this mod will perform this; otherwise, <c>false</c>.
        /// </value>
        public bool processPinkTransparency { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this mod was compiled dynamically.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance was compiled dynamically; otherwise, <c>false</c>.
        /// </value>
        public bool isDynamic { get; internal set; }

        private T GetCustomAttribute<T>() where T : Attribute
        {
            object[] customAttributes = _assembly.GetCustomAttributes(typeof(T), false);
            int index = 0;
            return index < customAttributes.Length ? (T)customAttributes[index] : default(T);
        }

        /// <summary>The Assembly that this mod was loaded from.</summary>
        public Assembly assembly
        {
            get => _assembly;
            internal set
            {
                _assembly = value;
                loaded = true;
                AssemblyTitleAttribute customAttribute1 = GetCustomAttribute<AssemblyTitleAttribute>();
                displayName = customAttribute1 == null ? name : customAttribute1.Title;
                version = _assembly.GetName().Version;
                AssemblyDescriptionAttribute customAttribute2 = GetCustomAttribute<AssemblyDescriptionAttribute>();
                if (customAttribute2 != null)
                    description = customAttribute2.Description;
                AssemblyCompanyAttribute customAttribute3 = GetCustomAttribute<AssemblyCompanyAttribute>();
                if (customAttribute3 != null)
                    author = customAttribute3.Company;
                else
                    author = "Unknown";
            }
        }

        internal IManageContent contentManager { get; set; }

        internal string assemblyPath => directory + "/" + name + ".dll";

        internal string tempAssemblyPath => directory + "/" + name + "_compiled.dll";

        internal string hashPath => directory + "/" + name + "_compiled.hash";

        internal string buildLogPath => directory + "/" + name + "_build.log";

        internal string configFilePath => directory + "/mod.conf";

        internal XmlDocument configDocument { get; set; }

        public void LoadOrCreateConfig()
        {
            LoadConfiguration();
            if (!disabled)
                return;
            loaded = false;
        }

        internal void LoadConfiguration()
        {
            disabled = ModLoader.disabledMods.Contains(name) || ModLoader.disabledMods.Contains(uniqueID);
            forceHarmonyLegacyLoad = ModLoader.forceLegacyLoad.Contains(name) || ModLoader.forceLegacyLoad.Contains(uniqueID);
            if (System.IO.File.Exists(configFilePath))
            {
                configDocument = new XmlDocument();
                configDocument.Load(configFilePath);
                XmlElement documentElement = configDocument.DocumentElement;
                XmlElement xmlElement1 = documentElement["SoftDependencies"];
                if (xmlElement1 != null)
                {
                    softDependencies = xmlElement1.InnerText.Split(new char[]
                    {
                        '|'
                    }, StringSplitOptions.RemoveEmptyEntries);
                }
                XmlElement xmlElement2 = documentElement["HardDependencies"];
                if (xmlElement2 != null)
                {
                    hardDependencies = xmlElement2.InnerText.Split(new char[1]
                    {
                        '|'
                    }, StringSplitOptions.RemoveEmptyEntries);
                }
                noRecompilation = documentElement["NoRecompilation"] != null && documentElement["NoRecompilation"].InnerText.ToLower() == "true";
                noPreloadAssets = documentElement["NoPreloadAssets"] != null && documentElement["NoPreloadAssets"].InnerText.ToLower() == "true";
                noCompilation = documentElement["NoCompilation"] != null && documentElement["NoCompilation"].InnerText.ToLower() == "true";
                preloadContent = documentElement["PreloadContent"] == null || !(documentElement["PreloadContent"].InnerText.ToLower() == "false");
                processPinkTransparency = documentElement["PinkTransparency"] == null || !(documentElement["PinkTransparency"].InnerText.ToLower() == "false");
                workshopID = documentElement["WorkshopID"] != null ? ulong.Parse(documentElement["WorkshopID"].InnerText) : 0UL;
                majorSupportedRevision = documentElement["MajorSupportedRevision"] != null ? int.Parse(documentElement["MajorSupportedRevision"].InnerText) : 0;
            }
            else
            {
                configDocument = new XmlDocument();
                configDocument.AppendChild(configDocument.CreateElement("Mod"));
                configDocument.Save(configFilePath);
            }
        }

        internal void Disable()
        {
            if (disabled)
                return;
            DevConsole.Log(DCSection.Mod, name + " Was Disabled!");
            disabled = true;
            ModLoader.SetModDisabled(this, true);
        }

        internal void Enable()
        {
            if (!disabled)
                return;
            disabled = false;
            ModLoader.SetModDisabled(this, false);
        }

        internal void SetWorkshopID(ulong id)
        {
            XmlElement documentElement = configDocument.DocumentElement;
            if (documentElement["WorkshopID"] == null)
                documentElement.AppendChild(configDocument.CreateElement("WorkshopID"));
            documentElement["WorkshopID"].InnerText = id.ToString();
            workshopID = id;
            configDocument.Save(configFilePath);
        }

        public enum Type
        {
            Mod,
            Reskin,
            MapPack,
            HatPack,
        }
    }
}
