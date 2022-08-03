// Decompiled with JetBrains decompiler
// Type: DuckGame.Mod
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DuckGame
{
    /// <summary>
    /// The base class for mod information. Each mod has a custom instance of this class.
    /// </summary>
    public abstract class Mod
    {
        private Dictionary<System.Type, List<System.Type>> _typeLists = new Dictionary<System.Type, List<System.Type>>();
        private uint _dataHash;
        private uint _thingHash;
        private uint _netMessageHash;
        private WorkshopMetaData _workshopData = new WorkshopMetaData();
        /// <summary>
        /// The property bag for this mod. Other mods may view and read from this collection.
        /// You must not edit this bag while the game is running, only during mod initialization.
        /// </summary>
        protected readonly PropertyBag _properties = new PropertyBag();
        private Priority _priority = Priority.Normal;
        public bool clientMod;
        private Tex2D _previewTexture;
        private Tex2D _screenshot;
        private Map<ushort, System.Type> _typeToMessageID = new Map<ushort, System.Type>();
        private Map<ushort, ConstructorInfo> _constructorToMessageID = new Map<ushort, ConstructorInfo>();
        public ushort currentMessageIDIndex = 1;
        private uint _identifierHash;

        public void System_RuinDatahash()
        {
            _dataHash = (uint)Rando.Int(9999999);
            DevConsole.Log("|DGRED|Mod.System_RuinDatahash called!");
        }

        /// <summary>
        /// A hash of all the Thing type names + NetMessage type names in this mod
        /// </summary>
        public uint dataHash
        {
            get
            {
                if (_dataHash == 0U)
                    _dataHash = (thingHash + netMessageHash) % uint.MaxValue;
                return _dataHash;
            }
        }

        /// <summary>A hash of all the Thing type names in this mod</summary>
        public uint thingHash
        {
            get
            {
                if (_thingHash == 0U)
                {
                    string str = "";
                    foreach (System.Type type in GetTypeList(typeof(Thing)))
                        str += type.Name;
                    _thingHash = CRC32.Generate(str);
                }
                return _thingHash;
            }
        }

        /// <summary>A hash of all the NetMessage type names in this mod</summary>
        public uint netMessageHash
        {
            get
            {
                if (_netMessageHash == 0U)
                {
                    string str = "";
                    foreach (KeyValuePair<ushort, System.Type> keyValuePair in typeToMessageID)
                        str += keyValuePair.Value.Name;
                    _netMessageHash = CRC32.Generate(str);
                }
                return _netMessageHash;
            }
        }

        public List<System.Type> GetTypeList(System.Type pType)
        {
            List<System.Type> typeList;
            if (!_typeLists.TryGetValue(pType, out typeList))
                typeList = _typeLists[pType] = new List<System.Type>();
            return typeList;
        }

        /// <summary>
        /// Used by the mod upload window, you shouldn't need this.
        /// </summary>
        public WorkshopMetaData workshopData => _workshopData;

        public string generateAndGetPathToScreenshot
        {
            get
            {
                string directory = configuration.directory;
                DuckFile.CreatePath(directory);
                string path = directory + "screenshot.png";
                if (!System.IO.File.Exists(path))
                {
                    if (configuration.modType == ModConfiguration.Type.MapPack && configuration.mapPack != null)
                    {
                        path = configuration.mapPack.RegeneratePreviewImage(null);
                    }
                    else
                    {
                        string pathString = configuration.directory + "/content/";
                        DuckFile.CreatePath(pathString);
                        path = pathString + "screenshot.png";
                    }
                }
                if (path == null)
                    return "";
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    Tex2D screenshot = this.screenshot;
                    Stream stream = DuckFile.Create(path);
                    ((Texture2D)screenshot.nativeObject).SaveAsPng(stream, screenshot.width, screenshot.height);
                    stream.Dispose();
                }
                return path;
            }
        }

        /// <summary>
        /// Returns a formatted path that leads to the "asset" parameter in a given mod.
        /// </summary>
        public static string GetPath<T>(string asset) where T : Mod => ModLoader.GetMod<T>().configuration.contentDirectory + asset.Replace('\\', '/');

        /// <summary>
        /// Returns a formatted path that leads to the "asset" parameter in this mod.
        /// </summary>
        public string GetPath(string asset) => configuration.contentDirectory + asset.Replace('\\', '/');

        /// <summary>
        /// The read-only property bag that this mod was initialized with.
        /// </summary>
        public IReadOnlyPropertyBag properties => _properties;

        /// <summary>The configuration class for this mod</summary>
        public ModConfiguration configuration { get; internal set; }

        /// <summary>The priority of this mod as compared to other mods.</summary>
        /// <value>The priority.</value>
        public virtual Priority priority => _priority;

        public void SetPriority(Priority pPriority) => _priority = pPriority;

        /// <summary>
        /// The workshop IDs of this mods parent mod. This is useful for DEV versions of mods, and will allow the parent mod's levels to be played in this mod and vice-versa.
        /// </summary>
        public virtual ulong workshopIDFacade => 0;

        /// <summary>
        /// All objects serialized and deserialized from this mod will use the namsepaceFacade instead of their actual namespace if this is set.
        /// For example, if your namespace is 'MyModDev', you could say namespaceFacade = 'MyModDev:MyMod' to drop the 'Dev' part during serialization.
        /// Therefore the format is 'MYNAMESPACENAME:FAKENAMESPACENAME'
        /// </summary>
        public virtual string namespaceFacade => null;

        /// <summary>
        ///  All objects serialized and deserialized from this mod will use the assemblyNameFacade instead of the actual name of this assembly.
        /// For example, if your Assembly is named 'MyModDEV', you could say assemblyNameFacade = 'MyMod' to drop the 'DEV' part during serialization.
        /// </summary>
        public virtual string assemblyNameFacade => null;

        /// <summary>Gets the preview texture for this mod.</summary>
        /// <value>The preview texture.</value>
        public virtual Tex2D previewTexture
        {
            get
            {
                if (_previewTexture == null)
                {
                    if (configuration.loaded)
                    {
                        if (configuration.contentDirectory != null)
                            _previewTexture = (Tex2D)ContentPack.LoadTexture2D(GetPath("preview") + ".png", false);
                        if (_previewTexture == null)
                            _previewTexture = Content.Load<Tex2D>("notexture");
                    }
                    else
                        _previewTexture = Content.Load<Tex2D>("none");
                }
                return _previewTexture;
            }
            protected set => _previewTexture = value;
        }

        /// <summary>Gets path for screenshot.png from Content folder.</summary>
        /// <value>Path for mod screenshot.png from Content folder.</value>
        public virtual Tex2D screenshot
        {
            get
            {
                if (_screenshot == null)
                {
                    if (configuration.loaded)
                    {
                        if (configuration.contentDirectory != null)
                        {
                            string str = GetPath(nameof(screenshot)) + ".png";
                            if (System.IO.File.Exists(str))
                                _screenshot = Content.Load<Tex2D>(str);
                        }
                        if (_screenshot == null)
                            _screenshot = Content.Load<Tex2D>("defaultMod");
                    }
                    else
                        _screenshot = null;
                }
                return _screenshot;
            }
        }

        /// <summary>
        /// Called on a mod when all mods and the core are finished being created
        /// and are ready to be initialized. You may use game functions and Reflection
        /// in here safely. Note that during this method, not all mods may have ran
        /// their pre-initialization routines and may not have sent their content to
        /// the core. Ideally, you will want to set up your properties here.
        /// </summary>
        protected virtual void OnPreInitialize()
        {
        }

        /// <summary>
        /// Called on a mod after all mods have finished their pre-initialization
        /// and have sent their content to the core.
        /// </summary>
        protected virtual void OnPostInitialize()
        {
        }

        /// <summary>
        /// Called on a mod after everything has been loaded and the first Level has been set
        /// </summary>
        protected virtual void OnStart()
        {
        }

        internal void InvokeOnPreInitialize() => OnPreInitialize();

        internal void InvokeOnPostInitialize() => OnPostInitialize();

        internal void InvokeStart() => OnStart();

        public Map<ushort, System.Type> typeToMessageID => _typeToMessageID;

        public Map<ushort, ConstructorInfo> constructorToMessageID => _constructorToMessageID;

        public uint identifierHash
        {
            get
            {
                if (_identifierHash == 0U && configuration != null)
                    _identifierHash = CRC32.Generate(configuration.uniqueID);
                return _identifierHash;
            }
        }

        /// <summary>Provides some mod debugging logic</summary>
        public static class Debug
        {
            [DllImport("kernel32.dll")]
            private static extern void OutputDebugString(string lpOutputString);

            /// <summary>
            /// Logs the specified line to any attached debuggers.
            /// If "-moddebug" is specified this will also output
            /// to the dev console in ~
            /// </summary>
            /// <param name="format">The format.</param>
            /// <param name="objs">The format parameters.</param>
            public static void Log(string format, params object[] objs)
            {
                if (!MonoMain.modDebugging)
                    return;
                DevConsole.Log(DCSection.Mod, string.Format(format, objs));
            }
        }
    }
}
