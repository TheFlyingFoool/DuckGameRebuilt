// Decompiled with JetBrains decompiler
// Type: DuckGame.ModConfiguration
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        /// <summary>The type of mod this is.</summary>
        public ModConfiguration.Type modType { get; internal set; }

        public void SetModType(ModConfiguration.Type pType) => this.modType = pType;

        public MapPack mapPack => this._mapPack;

        public void SetMapPack(MapPack pPack) => this._mapPack = pPack;

        internal ModConfiguration()
        {
            this.softDependencies = (IEnumerable<string>)new string[0];
            this.hardDependencies = (IEnumerable<string>)new string[0];
        }

        /// <summary>The full path to the root directory of this mod.</summary>
        public string directory { get; internal set; }

        public string contentDirectory
        {
            get => this._contentDirectory;
            internal set
            {
                if (value != null)
                    this._contentDirectory = value.Replace('\\', '/');
                else
                    this._contentDirectory = value;
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
                this._uniqueID = this.name + "|" + this.workshopID.ToString();
                if (this.workshopID == 0UL)
                {
                    if (this.author != null)
                        this._uniqueID = this._uniqueID + "_" + this.author.ToString();
                    if (this.version != (Version)null)
                        this._uniqueID = this._uniqueID + "_" + this.version.ToString();
                }
                return this._uniqueID;
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

        public ulong assignedWorkshopID => this.workshopID;

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
            object[] customAttributes = this._assembly.GetCustomAttributes(typeof(T), false);
            int index = 0;
            return index < customAttributes.Length ? (T)(object)customAttributes[index] : default(T);
        }

        /// <summary>The Assembly that this mod was loaded from.</summary>
        public Assembly assembly
        {
            get => this._assembly;
            internal set
            {
                this._assembly = value;
                this.loaded = true;
                AssemblyTitleAttribute customAttribute1 = this.GetCustomAttribute<AssemblyTitleAttribute>();
                this.displayName = customAttribute1 == null ? this.name : customAttribute1.Title;
                this.version = this._assembly.GetName().Version;
                AssemblyDescriptionAttribute customAttribute2 = this.GetCustomAttribute<AssemblyDescriptionAttribute>();
                if (customAttribute2 != null)
                    this.description = customAttribute2.Description;
                AssemblyCompanyAttribute customAttribute3 = this.GetCustomAttribute<AssemblyCompanyAttribute>();
                if (customAttribute3 != null)
                    this.author = customAttribute3.Company;
                else
                    this.author = "Unknown";
            }
        }

        internal IManageContent contentManager { get; set; }

        internal string assemblyPath => this.directory + "/" + this.name + ".dll";

        internal string tempAssemblyPath => this.directory + "/" + this.name + "_compiled.dll";

        internal string hashPath => this.directory + "/" + this.name + "_compiled.hash";

        internal string buildLogPath => this.directory + "/" + this.name + "_build.log";

        internal string configFilePath => this.directory + "/mod.conf";

        internal XmlDocument configDocument { get; set; }

        public void LoadOrCreateConfig()
        {
            this.LoadConfiguration();
            if (!this.disabled)
                return;
            this.loaded = false;
        }

        internal void LoadConfiguration()
        {
            this.disabled = ModLoader.disabledMods.Contains(this.name) || ModLoader.disabledMods.Contains(this.uniqueID);
            this.forceHarmonyLegacyLoad = ModLoader.forceLegacyLoad.Contains(this.name) || ModLoader.forceLegacyLoad.Contains(this.uniqueID);
            if (System.IO.File.Exists(this.configFilePath))
            {
                this.configDocument = new XmlDocument();
                this.configDocument.Load(this.configFilePath);
                XmlElement documentElement = this.configDocument.DocumentElement;
                XmlElement xmlElement1 = documentElement["SoftDependencies"];
                if (xmlElement1 != null)
                    this.softDependencies = (IEnumerable<string>)xmlElement1.InnerText.Split(new char[1]
                    {
            '|'
                    }, StringSplitOptions.RemoveEmptyEntries);
                XmlElement xmlElement2 = documentElement["HardDependencies"];
                if (xmlElement2 != null)
                    this.hardDependencies = (IEnumerable<string>)xmlElement2.InnerText.Split(new char[1]
                    {
            '|'
                    }, StringSplitOptions.RemoveEmptyEntries);
                this.noCompilation = documentElement["NoCompilation"] != null && documentElement["NoCompilation"].InnerText.ToLower() == "true";
                this.preloadContent = documentElement["PreloadContent"] == null || !(documentElement["PreloadContent"].InnerText.ToLower() == "false");
                this.processPinkTransparency = documentElement["PinkTransparency"] == null || !(documentElement["PinkTransparency"].InnerText.ToLower() == "false");
                this.workshopID = documentElement["WorkshopID"] != null ? ulong.Parse(documentElement["WorkshopID"].InnerText) : 0UL;
                this.majorSupportedRevision = documentElement["MajorSupportedRevision"] != null ? int.Parse(documentElement["MajorSupportedRevision"].InnerText) : 0;
            }
            else
            {
                this.configDocument = new XmlDocument();
                this.configDocument.AppendChild((XmlNode)this.configDocument.CreateElement("Mod"));
                this.configDocument.Save(this.configFilePath);
            }
        }

        internal void Disable()
        {
            if (this.disabled)
                return;
            DevConsole.Log(DCSection.Mod, this.name + " Was Disabled!");
            this.disabled = true;
            ModLoader.SetModDisabled(this, true);
        }

        internal void Enable()
        {
            if (!this.disabled)
                return;
            this.disabled = false;
            ModLoader.SetModDisabled(this, false);
        }

        internal void SetWorkshopID(ulong id)
        {
            XmlElement documentElement = this.configDocument.DocumentElement;
            if (documentElement["WorkshopID"] == null)
                documentElement.AppendChild((XmlNode)this.configDocument.CreateElement("WorkshopID"));
            documentElement["WorkshopID"].InnerText = id.ToString();
            this.workshopID = id;
            this.configDocument.Save(this.configFilePath);
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
