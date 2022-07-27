// Decompiled with JetBrains decompiler
// Type: DuckGame.XMLLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    public class XMLLevel : Level
    {
        public bool collectItems;
        public List<Thing> levelItems = new List<Thing>();
        public Texture2D preview;
        private bool _customLevel;
        private bool _clientLevel;
        private bool _customLoad;
        private uint _checksum;
        private string _loadString = "";
        private LevelData _data;
        private byte[] _compressedData;
        private MemoryStream _compressedDataReceived;
        public string synchronizedLevelName;
        public bool ignoreVisibility;
        public volatile bool cancelLoading;
        public bool onlineEnabled;
        private ushort specialSyncIndex;

        public bool customLevel => this._customLevel;

        public bool clientLevel => this._clientLevel;

        public uint checksum => this._checksum;

        public LevelData data
        {
            get => this._data;
            set => this._data = value;
        }

        public byte[] compressedData => this._compressedData;

        public MemoryStream compressedDataReceived => this._compressedDataReceived;

        private void InitializeSeed()
        {
            if (NetworkDebugger.enabled && NetworkDebugger.Recorder.active != null)
                this.seed = NetworkDebugger.Recorder.active.seed;
            else
                this.seed = Rando.Int(2147483646);
        }

        public XMLLevel(string level)
        {
            this.InitializeSeed();
            if (level.EndsWith(".client"))
            {
                this.isCustomLevel = true;
                this._customLevel = true;
                this._clientLevel = true;
                this._customLoad = true;
            }
            if (level.EndsWith(".custom"))
            {
                DevConsole.Log(DCSection.General, "Loading Level " + level);
                this.isCustomLevel = true;
                this._customLevel = true;
                level = level.Substring(0, level.Length - 7);
                if (Network.isActive)
                {
                    LevelData level1 = Content.GetLevel(level);
                    this._checksum = level1.GetChecksum();
                    this._data = level1;
                    this._customLoad = true;
                    if (Network.isServer)
                        this._compressedData = XMLLevel.GetCompressedLevelData(level1, level);
                }
            }
            if (level == "WORKSHOP")
            {
                this._customLevel = true;
                this.isCustomLevel = true;
                level = level.Substring(0, level.Length - 7);
                LevelData nextLevel = RandomLevelDownloader.GetNextLevel();
                this._checksum = nextLevel.GetChecksum();
                this._data = nextLevel;
                this._customLoad = true;
                if (Network.isServer && Network.isActive)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(memoryStream, CompressionMode.Compress));
                    binaryWriter.Write(nextLevel.metaData.guid.ToString());
                    BitBuffer data = nextLevel.GetData();
                    binaryWriter.Write(data.lengthInBytes);
                    binaryWriter.Write(data.buffer, 0, data.lengthInBytes);
                    binaryWriter.Close();
                    this._compressedData = memoryStream.ToArray();
                }
            }
            this._level = level;
        }

        public XMLLevel(LevelData level)
        {
            this.InitializeSeed();
            this._data = level;
        }

        public static byte[] GetCompressedLevelData(LevelData pLevel, string pLevelName)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(memoryStream, CompressionMode.Compress));
            binaryWriter.Write(pLevelName);
            BitBuffer data = pLevel.GetData();
            binaryWriter.Write(data.lengthInBytes);
            binaryWriter.Write(data.buffer, 0, data.lengthInBytes);
            binaryWriter.Close();
            return memoryStream.ToArray();
        }

        public bool ApplyLevelData(ReceivedLevelInfo info)
        {
            this.waitingOnNewData = false;
            this._data = info.data;
            this._level = info.name;
            this._customLevel = true;
            this.isCustomLevel = true;
            string str1 = DuckFile.onlineLevelDirectory;
            if (NetworkDebugger.currentIndex != 0)
                str1 = str1.Insert(str1.Length - 1, NetworkDebugger.currentIndex.ToString());
            string str2 = this._level;
            if (str2.EndsWith(".custom"))
                str2 = str2.Substring(0, this.level.Length - 7);
            DuckFile.EnsureDownloadFileSpaceAvailable();
            return DuckFile.SaveChunk(_data, str1 + str2 + ".lev");
        }

        public string ProcessLevelPath(string path)
        {
            bool flag = false;
            if (path.EndsWith(".online"))
            {
                this.isCustomLevel = true;
                string str1 = path.Substring(0, path.Length - 7);
                string str2 = DuckFile.onlineLevelDirectory;
                if (NetworkDebugger.currentIndex != 0)
                    str2 = str2.Insert(str2.Length - 1, NetworkDebugger.currentIndex.ToString());
                string path1 = str2 + str1 + ".lev";
                if (System.IO.File.Exists(path1))
                    return path1;
                flag = true;
            }
            if (flag || path.EndsWith(".custom"))
            {
                this.isCustomLevel = true;
                string str3 = path.Substring(0, path.Length - 7);
                string str4 = DuckFile.levelDirectory;
                if (NetworkDebugger.currentIndex != 0)
                    str4 = str4.Insert(str4.Length - 1, NetworkDebugger.currentIndex.ToString());
                string path2 = str4 + str3 + ".lev";
                return System.IO.File.Exists(path2) ? path2 : null;
            }
            this._data = Content.GetLevel(path);
            if (this._data != null)
                return path;
            string path3 = DuckFile.levelDirectory + path + ".lev";
            if (System.IO.File.Exists(path3))
                return path3;
            string path4 = Editor.initialDirectory + "/" + path + ".lev";
            return System.IO.File.Exists(path4) ? path4 : null;
        }

        private LevelData LoadLevelDoc()
        {
            if (this._data != null)
                return this._data;
            if (this._level == "WORKSHOP")
                return RandomLevelDownloader.GetNextLevel();
            LevelData levelData;
            if (!this._level.Contains("_tempPlayLevel"))
            {
                this._loadString = this._level;
                levelData = Content.GetLevel(this._level);
                if (levelData == null)
                {
                    bool flag = false;
                    if (this._level.Contains(":/") || this._level.Contains(":\\"))
                        flag = true;
                    if (flag)
                    {
                        this._loadString = this._level;
                        if (!this._loadString.EndsWith(".lev"))
                            this._loadString += ".lev";
                    }
                    else
                    {
                        this._loadString = DuckFile.levelDirectory + this._level;
                        if (!this._loadString.EndsWith(".lev"))
                            this._loadString += ".lev";
                    }
                    levelData = DuckFile.LoadLevel(this._loadString);
                    if (levelData == null && !flag)
                    {
                        this._loadString = Editor.initialDirectory + "/" + this._level + ".lev";
                        if (!this._loadString.EndsWith(".lev"))
                            this._loadString += ".lev";
                        levelData = DuckFile.LoadLevel(this._loadString);
                    }
                    if (levelData == null)
                        levelData = DuckFile.LoadLevel(this._level);
                    if (this is GameLevel)
                        this._customLoad = true;
                }
            }
            else
            {
                if (this is GameLevel && this._level.ToLowerInvariant().Contains(DuckFile.levelDirectory.ToLowerInvariant()))
                    this._customLoad = true;
                this._level = this._level.Replace(Directory.GetCurrentDirectory() + "\\", "");
                levelData = DuckFile.LoadLevel(this._level);
            }
            return levelData;
        }

        public override void Initialize()
        {
            AutoBlock._kBlockIndex = 0;
            if (this.level == "RANDOM" || this.cancelLoading)
                return;
            if (this._data == null)
                this._data = this.LoadLevelDoc();
            if (this.cancelLoading || this._data == null)
                return;
            this._id = this._data.metaData.guid;
            if ((this.level == "WORKSHOP" || this._customLoad || this._customLevel) && !this.bareInitialize)
                Global.PlayCustomLevel(this._id);
            Custom.ClearCustomData();
            Custom.previewData[CustomType.Block][0] = null;
            Custom.previewData[CustomType.Block][1] = null;
            Custom.previewData[CustomType.Block][2] = null;
            Custom.previewData[CustomType.Background][0] = null;
            Custom.previewData[CustomType.Background][1] = null;
            Custom.previewData[CustomType.Background][2] = null;
            Custom.previewData[CustomType.Platform][0] = null;
            Custom.previewData[CustomType.Platform][1] = null;
            Custom.previewData[CustomType.Platform][2] = null;
            Custom.previewData[CustomType.Parallax][0] = null;
            if (this._data.customData != null)
            {
                if (this._data.customData.customTileset01Data != null)
                {
                    Custom.previewData[CustomType.Block][0] = this._data.customData.customTileset01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][0].GetTileData(), 0, CustomType.Block);
                }
                if (this._data.customData.customTileset02Data != null)
                {
                    Custom.previewData[CustomType.Block][1] = this._data.customData.customTileset02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][1].GetTileData(), 1, CustomType.Block);
                }
                if (this._data.customData.customTileset03Data != null)
                {
                    Custom.previewData[CustomType.Block][2] = this._data.customData.customTileset03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][2].GetTileData(), 2, CustomType.Block);
                }
                if (this._data.customData.customBackground01Data != null)
                {
                    Custom.previewData[CustomType.Background][0] = this._data.customData.customBackground01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][0].GetTileData(), 0, CustomType.Background);
                }
                if (this._data.customData.customBackground02Data != null)
                {
                    Custom.previewData[CustomType.Background][1] = this._data.customData.customBackground02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][1].GetTileData(), 1, CustomType.Background);
                }
                if (this._data.customData.customBackground03Data != null)
                {
                    Custom.previewData[CustomType.Background][2] = this._data.customData.customBackground03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][2].GetTileData(), 2, CustomType.Background);
                }
                if (this._data.customData.customPlatform01Data != null)
                {
                    Custom.previewData[CustomType.Platform][0] = this._data.customData.customPlatform01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][0].GetTileData(), 0, CustomType.Platform);
                }
                if (this._data.customData.customPlatform02Data != null)
                {
                    Custom.previewData[CustomType.Platform][1] = this._data.customData.customPlatform02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][1].GetTileData(), 1, CustomType.Platform);
                }
                if (this._data.customData.customPlatform03Data != null)
                {
                    Custom.previewData[CustomType.Platform][2] = this._data.customData.customPlatform03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][2].GetTileData(), 2, CustomType.Platform);
                }
                if (this._data.customData.customParallaxData != null)
                {
                    Custom.previewData[CustomType.Parallax][0] = this._data.customData.customParallaxData;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Parallax][0].GetTileData(), 0, CustomType.Parallax);
                }
            }
            if (this.cancelLoading)
                return;
            if (!this.bareInitialize && !this.isPreview)
                this.preview = Editor.LoadPreview(this._data.previewData.preview);
            Random generator = Rando.generator;
            Rando.generator = new Random(this.seed);
            if (!this.bareInitialize && !this.isPreview)
                GhostManager.context.ResetGhostIndex(this.networkIndex);
            Thing.loadingLevel = this._data;
            int version = this._data.metaData.version;
            this.onlineEnabled = this._data.metaData.online;
            bool flag = true;
            int num = 0;
            foreach (BinaryClassChunk node in this._data.objects.objects)
            {
                if (this.cancelLoading)
                    return;
                Thing thing1 = Thing.LoadThing(node);
                if (thing1 != null && (this._data.metaData.version >= 1 || !Thing.CheckForBozoData(thing1)))
                {
                    if (!ContentProperties.GetBag(thing1.GetType()).GetOrDefault("isOnlineCapable", true) || thing1.serverOnly && !Network.isServer)
                    {
                        flag = false;
                        if (Network.isActive)
                            continue;
                    }
                    if (!this.bareInitialize || thing1 is ArcadeMachine)
                    {
                        if (!thing1.visibleInGame && !this.ignoreVisibility)
                            thing1.visible = false;
                        if (Network.isActive)
                        {
                            if (thing1 is ThingContainer)
                            {
                                foreach (Thing thing2 in (thing1 as ThingContainer).things)
                                    this.NetPrepare(thing2);
                            }
                            this.NetPrepare(thing1);
                        }
                        this.AddThing(thing1);
                    }
                    ++num;
                }
            }
            Rando.generator = generator;
            if (flag)
                this.onlineEnabled = true;
            this._things.RefreshState();
            Thing.loadingLevel = null;
        }

        private void NetPrepare(Thing pThing)
        {
            if (this.bareInitialize || this.isPreview)
                return;
            if (pThing.isStateObject)
            {
                pThing.PrepareForHost();
            }
            else
            {
                if (pThing is IDontMove)
                    return;
                ++this.specialSyncIndex;
                pThing.specialSyncIndex = this.specialSyncIndex;
            }
        }
    }
}
