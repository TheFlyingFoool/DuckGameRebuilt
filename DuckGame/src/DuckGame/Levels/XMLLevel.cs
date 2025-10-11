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

        public bool customLevel => _customLevel;

        public bool clientLevel => _clientLevel;

        public uint checksum => _checksum;

        public LevelData data
        {
            get => _data;
            set => _data = value;
        }

        public byte[] compressedData => _compressedData;

        public MemoryStream compressedDataReceived => _compressedDataReceived;

        private void InitializeSeed()
        {
            if (NetworkDebugger.enabled && NetworkDebugger.Recorder.active != null) seed = NetworkDebugger.Recorder.active.seed;
            else seed = Rando.Int(2147483646);
        }

        public XMLLevel(string level)
        {
            InitializeSeed(); //added these special codes here becuase crashes happen often around here -Lucky
            Main.SpecialCode = ".client";
            if (level.EndsWith(".client"))
            {
                isCustomLevel = true;
                _customLevel = true;
                _clientLevel = true;
                _customLoad = true;
            }
            Main.SpecialCode = ".custom";
            if (level.EndsWith(".custom"))
            {
                Main.SpecialCode2 = "010";
                DevConsole.Log(DCSection.General, "Loading Level " + level);
                isCustomLevel = true;
                _customLevel = true;
                Main.SpecialCode2 = "020";
                level = level.Substring(0, level.Length - 7);
                if (Network.isActive)
                {
                    Main.SpecialCode2 = "021";
                    LevelData level1 = Content.GetLevel(level);
                    Main.SpecialCode2 = "022";
                    _checksum = level1.GetChecksum();
                    Main.SpecialCode2 = "023";
                    _data = level1;
                    _customLoad = true;
                    Main.SpecialCode2 = "024";
                    if (Network.isServer) _compressedData = GetCompressedLevelData(level1, level);
                }
            }
            Main.SpecialCode = "WORKSHOP";
            if (level == "WORKSHOP")
            {
                Main.SpecialCode2 = "000";
                _customLevel = true;
                isCustomLevel = true;
                Main.SpecialCode2 = "100";
                level = level.Substring(0, level.Length - 7);
                LevelData nextLevel = RandomLevelDownloader.GetNextLevel();
                Main.SpecialCode2 = "101";
                _checksum = nextLevel.GetChecksum();
                _data = nextLevel;
                _customLoad = true;
                Main.SpecialCode2 = "200";
                if (Network.isServer && Network.isActive)
                {
                    Main.SpecialCode2 = "201";
                    MemoryStream memoryStream = new MemoryStream();
                    BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(memoryStream, CompressionMode.Compress));
                    Main.SpecialCode2 = "202";
                    binaryWriter.Write(nextLevel.metaData.guid.ToString());
                    BitBuffer data = nextLevel.GetData();
                    Main.SpecialCode2 = "203";
                    binaryWriter.Write(data.lengthInBytes);
                    binaryWriter.Write(data.buffer, 0, data.lengthInBytes);
                    Main.SpecialCode2 = "204";
                    binaryWriter.Close();
                    _compressedData = memoryStream.ToArray();
                    Main.SpecialCode2 = "205";
                }
            }
            Main.SpecialCode = "END";
            Main.SpecialCode2 = "";
            _level = level;
        }

        public XMLLevel(LevelData level)
        {
            InitializeSeed();
            _data = level;
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
            waitingOnNewData = false;
            _data = info.data;
            _level = info.name;
            _customLevel = true;
            isCustomLevel = true;
            string str1 = DuckFile.onlineLevelDirectory;
            if (NetworkDebugger.currentIndex != 0)
                str1 = str1.Insert(str1.Length - 1, NetworkDebugger.currentIndex.ToString());
            string str2 = _level;
            if (str2.EndsWith(".custom"))
                str2 = str2.Substring(0, level.Length - 7);
            DuckFile.EnsureDownloadFileSpaceAvailable();
            return DuckFile.SaveChunk(_data, str1 + str2 + ".lev");
        }

        public string ProcessLevelPath(string path)
        {
            bool flag = false;
            if (path.EndsWith(".online"))
            {
                isCustomLevel = true;
                string str1 = path.Substring(0, path.Length - 7);
                string str2 = DuckFile.onlineLevelDirectory;
                if (NetworkDebugger.currentIndex != 0)
                    str2 = str2.Insert(str2.Length - 1, NetworkDebugger.currentIndex.ToString());
                string path1 = str2 + str1 + ".lev";
                if (File.Exists(path1))
                    return path1;
                flag = true;
            }
            if (flag || path.EndsWith(".custom"))
            {
                isCustomLevel = true;
                string str3 = path.Substring(0, path.Length - 7);
                string str4 = DuckFile.levelDirectory;
                if (NetworkDebugger.currentIndex != 0)
                    str4 = str4.Insert(str4.Length - 1, NetworkDebugger.currentIndex.ToString());
                string path2 = str4 + str3 + ".lev";
                return File.Exists(path2) ? path2 : null;
            }
            _data = Content.GetLevel(path);
            if (_data != null)
                return path;
            string path3 = DuckFile.levelDirectory + path + ".lev";
            if (File.Exists(path3))
                return path3;
            string path4 = Editor.initialDirectory + "/" + path + ".lev";
            return File.Exists(path4) ? path4 : null;
        }

        private LevelData LoadLevelDoc()
        {
            if (_data != null)
                return _data;
            if (_level == "WORKSHOP")
                return RandomLevelDownloader.GetNextLevel();
            LevelData levelData;
            if (!_level.Contains("_tempPlayLevel"))
            {
                _loadString = _level;
                levelData = Content.GetLevel(_level);
                if (levelData == null)
                {
                    bool flag = false;
                    if (_level.Contains(":/") || _level.Contains(":\\"))
                        flag = true;
                    if (flag)
                    {
                        _loadString = _level;
                        if (!_loadString.EndsWith(".lev"))
                            _loadString += ".lev";
                    }
                    else
                    {
                        _loadString = DuckFile.levelDirectory + _level;
                        if (!_loadString.EndsWith(".lev"))
                            _loadString += ".lev";
                    }
                    levelData = DuckFile.LoadLevel(_loadString);
                    if (levelData == null && !flag)
                    {
                        _loadString = Editor.initialDirectory + "/" + _level + ".lev";
                        if (!_loadString.EndsWith(".lev"))
                            _loadString += ".lev";
                        levelData = DuckFile.LoadLevel(_loadString);
                    }
                    if (levelData == null)
                        levelData = DuckFile.LoadLevel(_level);
                    if (this is GameLevel)
                        _customLoad = true;
                }
            }
            else
            {
                if (this is GameLevel && _level.ToLowerInvariant().Contains(DuckFile.levelDirectory.ToLowerInvariant()))
                    _customLoad = true;
                _level = _level.Replace(Directory.GetCurrentDirectory() + "\\", "");
                levelData = DuckFile.LoadLevel(_level);
            }
            return levelData;
        }

        public override void Initialize()
        {
            AutoBlock._kBlockIndex = 0;
            if (level == "RANDOM" || cancelLoading)
                return;
            if (_data == null)
                _data = LoadLevelDoc();
            if (cancelLoading || _data == null)
                return;
            _id = _data.metaData.guid;
            if ((level == "WORKSHOP" || _customLoad || _customLevel) && !bareInitialize)
                Global.PlayCustomLevel(_id);
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
            if (_data.customData != null)
            {
                if (_data.customData.customTileset01Data != null)
                {
                    Custom.previewData[CustomType.Block][0] = _data.customData.customTileset01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][0].GetTileData(), 0, CustomType.Block);
                }
                if (_data.customData.customTileset02Data != null)
                {
                    Custom.previewData[CustomType.Block][1] = _data.customData.customTileset02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][1].GetTileData(), 1, CustomType.Block);
                }
                if (_data.customData.customTileset03Data != null)
                {
                    Custom.previewData[CustomType.Block][2] = _data.customData.customTileset03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Block][2].GetTileData(), 2, CustomType.Block);
                }
                if (_data.customData.customBackground01Data != null)
                {
                    Custom.previewData[CustomType.Background][0] = _data.customData.customBackground01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][0].GetTileData(), 0, CustomType.Background);
                }
                if (_data.customData.customBackground02Data != null)
                {
                    Custom.previewData[CustomType.Background][1] = _data.customData.customBackground02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][1].GetTileData(), 1, CustomType.Background);
                }
                if (_data.customData.customBackground03Data != null)
                {
                    Custom.previewData[CustomType.Background][2] = _data.customData.customBackground03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Background][2].GetTileData(), 2, CustomType.Background);
                }
                if (_data.customData.customPlatform01Data != null)
                {
                    Custom.previewData[CustomType.Platform][0] = _data.customData.customPlatform01Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][0].GetTileData(), 0, CustomType.Platform);
                }
                if (_data.customData.customPlatform02Data != null)
                {
                    Custom.previewData[CustomType.Platform][1] = _data.customData.customPlatform02Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][1].GetTileData(), 1, CustomType.Platform);
                }
                if (_data.customData.customPlatform03Data != null)
                {
                    Custom.previewData[CustomType.Platform][2] = _data.customData.customPlatform03Data;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][2].GetTileData(), 2, CustomType.Platform);
                }
                if (_data.customData.customParallaxData != null)
                {
                    Custom.previewData[CustomType.Parallax][0] = _data.customData.customParallaxData;
                    Custom.ApplyCustomData(Custom.previewData[CustomType.Parallax][0].GetTileData(), 0, CustomType.Parallax);
                }
            }
            if (cancelLoading)
                return;
            if (!bareInitialize && !isPreview)
                preview = Editor.LoadPreview(_data.previewData.preview);
            Random generator = Rando.generator;
            Rando.generator = new Random(seed);
            if (!bareInitialize && !isPreview)
                GhostManager.context.ResetGhostIndex(networkIndex);
            Thing.loadingLevel = _data;
            //int version = _data.metaData.version; useless code -Lucky
            onlineEnabled = _data.metaData.online;
            bool flag = true;
            int num = 0;
            foreach (BinaryClassChunk node in _data.objects.objects)
            {
                if (cancelLoading)
                    return;
                Thing thing1 = Thing.LoadThing(node);
                if (thing1 != null && (_data.metaData.version >= 1 || !Thing.CheckForBozoData(thing1)))
                {
                    if (!ContentProperties.GetBag(thing1.GetType()).GetOrDefault("isOnlineCapable", true) || thing1.serverOnly && !Network.isServer)
                    {
                        flag = false;
                        if (Network.isActive)
                            continue;
                    }
                    if (!bareInitialize || thing1 is ArcadeMachine)
                    {
                        if (!thing1.visibleInGame && !ignoreVisibility)
                            thing1.visible = false;
                        if (Network.isActive)
                        {
                            if (thing1 is ThingContainer)
                            {
                                foreach (Thing thing2 in (thing1 as ThingContainer).things)
                                    NetPrepare(thing2);
                            }
                            NetPrepare(thing1);
                        }
                        AddThing(thing1);
                    }
                    ++num;
                }
            }
            Rando.generator = generator;
            if (flag)
                onlineEnabled = true;
            _things.RefreshState();
            Thing.loadingLevel = null;
        }

        private void NetPrepare(Thing pThing)
        {
            if (bareInitialize || isPreview)
                return;
            if (pThing.isStateObject)
            {
                pThing.PrepareForHost();
            }
            else
            {
                if (pThing is IDontMove)
                    return;
                ++specialSyncIndex;
                pThing.specialSyncIndex = specialSyncIndex;
            }
        }
    }
}
