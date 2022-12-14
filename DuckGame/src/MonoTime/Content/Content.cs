// Decompiled with JetBrains decompiler
// Type: DuckGame.Content
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using XnaToFna;

namespace DuckGame
{
    public class Content
    {
        private static MultiMap<string, LevelData> _levels = new MultiMap<string, LevelData>();
        private static MultiMap<string, LevelData> _levelPreloadList = new MultiMap<string, LevelData>();
        private static Dictionary<string, MTEffect> _effects = new Dictionary<string, MTEffect>();
        private static Dictionary<Effect, MTEffect> _effectMap = new Dictionary<Effect, MTEffect>();
        private static List<MTEffect> _effectList = new List<MTEffect>();
        private static short _currentEffectIndex = 0;
        private static Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Tex2D> _textures = new Dictionary<string, Tex2D>();
        private static Dictionary<object, Tex2D> _texture2DMap = new Dictionary<object, Tex2D>();
        private static List<Tex2D> _textureList = new List<Tex2D>();
        public static short _currentTextureIndex = 0;
        public static Tex2D invalidTexture;
        public static volatile bool readyToRenderPreview = false;
        //private static volatile bool previewRendering = false;
        public static volatile bool renderingPreview = false;
        public static XMLLevel previewLevel;
        private static Camera _previewCamera;
        public static volatile bool cancelPreview = false;
        public static int customPreviewWidth = 0;
        public static int customPreviewHeight = 0;
        public static Vec2 customPreviewCenter = Vec2.Zero;
        private static LevelData _previewLevelData;
        private static LayerCore _previewLayerCore = null;
        private static string _previewPath = null;
        private static MTSpriteBatch _previewBatch;
        private static bool _previewBackground = false;
        private static Thread _previewThread;
        private static RenderTarget2D _currentPreviewTarget;
        private static LevelMetaData.PreviewPair _currentPreviewPair;
        public static bool doingTempSave = false;
        public static byte[] generatePreviewBytes;
        public static bool renderingToTarget = false;
        private static Dictionary<Type, string> _extensionList = new Dictionary<Type, string>()
    {
      {
        typeof (Tex2D),
        "*.png"
      },
      {
        typeof (Texture2D),
        "*.png"
      },
      {
        typeof (SoundEffect),
        "*.wav"
      },
      {
        typeof (Song),
        "*.ogg"
      },
      {
        typeof (Level),
        "*.lev"
      },
      {
        typeof (Effect),
        "*.xnb"
      }
    };
        private static List<string> _texturesToProcess = new List<string>();
        private static ContentManager _base;
        private static string _path = "";
        private static Dictionary<string, Vec2> _spriteSizeDirectory = new Dictionary<string, Vec2>();
        public static object _loadLock = new object();
        public static Exception lastException = null;
        private static Dictionary<string, ParallaxBackground.Definition> _parallaxDefinitions = new Dictionary<string, ParallaxBackground.Definition>();


        public static LevelData GetLevel(string guid, LevelLocation location = LevelLocation.Any)
        {
            List<LevelData> list;
            if (guid != null && _levels.TryGetValue(guid, out list))
            {
                foreach (LevelData level in list)
                {
                    if (level.GetLocation() == location || location == LevelLocation.Any)
                        return level;
                }
            }
            return null;
        }

        public static List<LevelData> GetAllLevels(string guid)
        {
            List<LevelData> list;
            return _levels.TryGetValue(guid, out list) ? list : new List<LevelData>();
        }

        public static List<LevelData> GetAllLevels()
        {
            List<LevelData> allLevels = new List<LevelData>();
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)_levels)
                allLevels.AddRange(level.Value);
            return allLevels;
        }

        public static void MapLevel(string lev, LevelData dat, LevelLocation location)
        {
            lock (_levels)
            {
                List<LevelData> list;
                if (_levels.TryGetValue(lev, out list))
                {
                    LevelData levelData1 = null;
                    foreach (LevelData levelData2 in list)
                    {
                        if (levelData2.GetLocation() == location)
                        {
                            levelData1 = levelData2;
                            break;
                        }
                    }
                    if (levelData1 != null)
                        list.Remove(levelData1);
                }
                dat.SetLocation(location);
                _levels.Add(lev, dat);
            }
        }

        public static List<MTEffect> effectList => _effectList;

        public static Dictionary<string, Tex2D> textures => _textures;

        public static List<Tex2D> textureList => _textureList;

        private static void PreviewThread()
        {
            Level activeLevel = Level.activeLevel;
            Level currentLevel = Level.core.currentLevel;
            LayerCore core = Layer.core;
            try
            {
                renderingPreview = true;
                if (!_previewBackground)
                    Thing.skipLayerAdding = true;
                XMLLevel xmlLevel;
                if (_previewLevelData == null)
                {
                    xmlLevel = new XMLLevel(_previewPath);
                }
                else
                {
                    xmlLevel = new XMLLevel(_previewLevelData);
                    _previewLevelData = null;
                }
                if (cancelPreview)
                    return;
                previewLevel = xmlLevel;
                previewLevel.ignoreVisibility = true;
                Level.skipInitialize = !_previewBackground;
                if (!_previewBackground)
                    previewLevel.isPreview = true;
                _previewLayerCore = null;
                if (_previewBackground)
                {
                    Layer.core = _previewLayerCore = new LayerCore();
                    Layer.core.InitializeLayers();
                }
                Level.core.currentLevel = previewLevel;
                Level.activeLevel = previewLevel;
                previewLevel.Initialize();
                Level.activeLevel = activeLevel;
                Level.core.currentLevel = currentLevel;
                if (cancelPreview)
                    return;
                Thing.skipLayerAdding = false;
                Level.skipInitialize = false;
                previewLevel.CalculateBounds();
                _previewCamera = customPreviewWidth == 0 ? new Camera(0f, 0f, 1280f, 1280f * Graphics.aspect) : new Camera(0f, 0f, customPreviewWidth, customPreviewHeight);
                Vec2 vec2 = (previewLevel.topLeft + previewLevel.bottomRight) / 2f;
                if (cancelPreview)
                    return;
                _previewCamera.width /= 2f;
                _previewCamera.height /= 2f;
                _previewCamera.center = !(customPreviewCenter != Vec2.Zero) ? vec2 : customPreviewCenter;
                readyToRenderPreview = true;
                if (_previewThread != null)
                {
                    while (readyToRenderPreview)
                    {
                        if (cancelPreview)
                            return;
                    }
                }
                //DuckGame.Content.previewRendering = false;
                renderingPreview = false;
            }
            catch (Exception ex)
            {
                Program.LogLine(ex.ToString());
                renderingPreview = false;
                Thing.skipLayerAdding = false;
                Level.skipInitialize = false;
            }
            if (!_previewBackground)
                return;
            Level.activeLevel = activeLevel;
            Level.core.currentLevel = currentLevel;
            Layer.core = core;
        }

        private static void DoPreviewRender(bool pSaveMetadata)
        {
            MTSpriteBatch screen = Graphics.screen;
            Graphics.screen = _previewBatch;
            Viewport viewport = Graphics.viewport;
            RenderTarget2D currentRenderTarget = Graphics.currentRenderTarget;
            Graphics.SetRenderTarget(_currentPreviewTarget);
            Graphics.viewport = new Viewport(0, 0, _currentPreviewTarget.width, _currentPreviewTarget.height);
            string str1 = Custom.data[CustomType.Block][0];
            if (Custom.previewData[CustomType.Block][0] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Block][0].GetTileData(), 0, CustomType.Block);
            string str2 = Custom.data[CustomType.Block][1];
            if (Custom.previewData[CustomType.Block][1] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Block][1].GetTileData(), 1, CustomType.Block);
            string str3 = Custom.data[CustomType.Block][2];
            if (Custom.previewData[CustomType.Block][2] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Block][2].GetTileData(), 2, CustomType.Block);
            string str4 = Custom.data[CustomType.Background][0];
            if (Custom.previewData[CustomType.Background][0] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Background][0].GetTileData(), 0, CustomType.Background);
            string str5 = Custom.data[CustomType.Background][1];
            if (Custom.previewData[CustomType.Background][1] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Background][1].GetTileData(), 1, CustomType.Background);
            string str6 = Custom.data[CustomType.Background][2];
            if (Custom.previewData[CustomType.Background][2] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Background][2].GetTileData(), 2, CustomType.Background);
            string str7 = Custom.data[CustomType.Platform][0];
            if (Custom.previewData[CustomType.Platform][0] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][0].GetTileData(), 0, CustomType.Platform);
            string str8 = Custom.data[CustomType.Platform][1];
            if (Custom.previewData[CustomType.Platform][1] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][1].GetTileData(), 1, CustomType.Platform);
            string str9 = Custom.data[CustomType.Platform][2];
            if (Custom.previewData[CustomType.Platform][2] != null)
                Custom.ApplyCustomData(Custom.previewData[CustomType.Platform][2].GetTileData(), 2, CustomType.Platform);
            bool pChallenge = false;
            bool pStrange = true;
            bool pArcade = false;
            Dictionary<string, int> pInvalidData = new Dictionary<string, int>();
            if (_previewBackground)
            {
                Level activeLevel = Level.activeLevel;
                Level currentLevel = Level.core.currentLevel;
                LayerCore core = Layer.core;
                if (_previewLayerCore != null)
                    Layer.core = _previewLayerCore;
                Level.activeLevel = previewLevel;
                Level.core.currentLevel = previewLevel;
                try
                {
                    Graphics.defaultRenderTarget = _currentPreviewTarget;
                    Layer.HUD.visible = false;
                    previewLevel.camera = _previewCamera;
                    previewLevel.simulatePhysics = false;
                    previewLevel.DoUpdate();
                    previewLevel.DoUpdate();
                    previewLevel.DoDraw();
                    Layer.HUD.visible = true;
                    Graphics.defaultRenderTarget = null;
                    Level.activeLevel = activeLevel;
                    Level.core.currentLevel = currentLevel;
                    Layer.core = core;
                }
                catch (Exception ex)
                {
                    Layer.HUD.visible = true;
                    Graphics.defaultRenderTarget = null;
                    Level.activeLevel = activeLevel;
                    Level.core.currentLevel = currentLevel;
                    Layer.core = core;
                    throw ex;
                }
            }
            else
            {
                Graphics.Clear(Color.Black);
                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, _previewCamera.getMatrix());
                foreach (Thing thing in previewLevel.things)
                {
                    if (thing.layer == Layer.Game || thing.layer == Layer.Blocks || thing.layer == null)
                        thing.Draw();
                    if (pSaveMetadata)
                    {
                        switch (thing)
                        {
                            case ChallengeMode _:
                                pChallenge = true;
                                break;
                            case SpawnPoint _:
                                pStrange = false;
                                break;
                            case ArcadeMode _:
                                pArcade = true;
                                break;
                        }
                        if (!ContentProperties.GetBag(thing.GetType()).GetOrDefault("isOnlineCapable", true))
                        {
                            if (!pInvalidData.ContainsKey(thing.editorName))
                                pInvalidData[thing.editorName] = 1;
                            else
                                ++pInvalidData[thing.editorName];
                        }
                    }
                    Graphics.material = null;
                }
                if (previewLevel.things.Count == 1)
                {
                    ImportMachine importMachine = previewLevel.things.First() as ImportMachine;
                }
                Graphics.screen.End();
            }
            Graphics.screen = screen;
            Graphics.SetRenderTarget(currentRenderTarget);
            Custom.data[CustomType.Block][0] = str1;
            Custom.data[CustomType.Block][1] = str2;
            Custom.data[CustomType.Block][2] = str3;
            Custom.data[CustomType.Background][0] = str4;
            Custom.data[CustomType.Background][1] = str5;
            Custom.data[CustomType.Background][2] = str6;
            Custom.data[CustomType.Platform][0] = str7;
            Custom.data[CustomType.Platform][1] = str8;
            Custom.data[CustomType.Platform][2] = str9;
            if (!pSaveMetadata || doingTempSave)
                return;
            LevelMetaData levelMetaData = Editor.ReadLevelMetadata(previewLevel.data);
            if (levelMetaData == null || levelMetaData.guid == null)
                return;
            _currentPreviewPair = levelMetaData.SavePreview((Texture2D)_currentPreviewTarget, pInvalidData, pStrange, pChallenge, pArcade);
        }

        public static Thread previewThread => _previewThread;

        public static LevelMetaData.PreviewPair GeneratePreview(
          LevelData levelData,
          bool pRefresh = false,
          RenderTarget2D pCustomPreviewTarget = null)
        {
            _previewLevelData = levelData;
            return GeneratePreview((string)null, pRefresh, pCustomPreviewTarget);
        }

        public static LevelMetaData.PreviewPair GeneratePreview(
          string levelPath,
          bool pRefresh = false,
          RenderTarget2D pCustomPreviewTarget = null)
        {
            if (generatePreviewBytes != null)
            {
                _previewLevelData = DuckFile.LoadLevel(generatePreviewBytes);
                generatePreviewBytes = null;
            }
            bool flag = false;
            if (pCustomPreviewTarget != null)
                flag = true;
            else if (!pRefresh && levelPath != null)
            {
                LevelMetaData levelMetaData = _previewLevelData == null ? Editor.ReadLevelMetadata(levelPath) : Editor.ReadLevelMetadata(_previewLevelData);
                if (levelMetaData != null)
                {
                    LevelMetaData.PreviewPair preview = levelMetaData.LoadPreview();
                    if (preview != null)
                        return preview;
                }
            }
            DevConsole.Log(DCSection.General, "Generating preview data for (" + levelPath + ")...");
            _previewBackground = flag;
            readyToRenderPreview = false;
            if (_previewThread != null && _previewThread.IsAlive)
            {
                cancelPreview = true;
                int num = 250;
                while (_previewThread.IsAlive)
                {
                    Tasker.RunTasks();
                    Thread.Sleep(2);
                    --num;
                }
                readyToRenderPreview = false;
            }
            _previewThread = null;
            cancelPreview = false;
            Thing.skipLayerAdding = false;
            Level.skipInitialize = false;
            if (_previewBatch == null)
                _previewBatch = new MTSpriteBatch(Graphics.device);
            _previewPath = levelPath;
            _currentPreviewTarget = pCustomPreviewTarget == null ? new RenderTarget2D(320, 200) : pCustomPreviewTarget;
            renderingToTarget = true;
            renderingPreview = true;
            readyToRenderPreview = true;
            PreviewThread();
            DoPreviewRender(pCustomPreviewTarget == null);
            renderingPreview = false;
            readyToRenderPreview = false;
            renderingToTarget = false;
            return _currentPreviewPair;
        }

        public static void SetTextureAtIndex(short index, Tex2D tex)
        {
            while (index >= _textureList.Count)
            {
                _textureList.Add(null);
                ++_currentTextureIndex;
            }
            _textureList[index] = tex;
            _texture2DMap[tex.nativeObject] = tex;
            _textures[tex.textureName] = tex;
            tex.SetTextureIndex(index);
        }

        public static Tex2D AssignTextureIndex(Tex2D tex)
        {
            Tex2D tex2D;
            _texture2DMap.TryGetValue(tex, out tex2D);
            if (tex2D == null)
            {
                tex.SetTextureIndex(_currentTextureIndex);
                ++_currentTextureIndex;
                _textureList.Add(tex);
                _texture2DMap[tex] = tex;
            }
            return tex2D;
        }

        public static Tex2D GetTex2D(object tex) => GetTex2D((Texture2D)tex);

        public static Tex2D GetTex2D(Texture2D tex)
        {
            if (tex == null)
                return null;
            Tex2D tex2D;
            _texture2DMap.TryGetValue(tex, out tex2D);
            if (tex2D == null)
            {
                tex2D = new Tex2D(tex, "", _currentTextureIndex);
                ++_currentTextureIndex;
                _textureList.Add(tex2D);
                _texture2DMap[tex] = tex2D;
            }
            return tex2D;
        }

        public static void SetEffectAtIndex(short index, MTEffect e)
        {
            while (index > _effectList.Count)
            {
                _effectList.Add(null);
                ++_currentEffectIndex;
            }
            _effectList[index] = e;
            _effectMap[e.effect] = e;
            _effects[e.effectName] = e;
            e.SetEffectIndex(index);
        }

        public static MTEffect GetMTEffect(Effect e)
        {
            MTEffect mtEffect;
            _effectMap.TryGetValue(e, out mtEffect);
            if (mtEffect == null)
            {
                mtEffect = new MTEffect(e, "", _currentEffectIndex);
                ++_currentEffectIndex;
                _effectList.Add(mtEffect);
                _effectMap[e] = mtEffect;
            }
            return mtEffect;
        }

        public static Tex2D GetTex2DFromIndex(short index) => _textureList[index];

        public static MTEffect GetMTEffectFromIndex(short index) => index < 0 ? null : _effectList[index];

        public static List<string> GetFiles<T>(string path)
        {
            List<string> files = new List<string>();
            string ext;
            if (_extensionList.TryGetValue(typeof(T), out ext))
                GetFilesInternal<T>(path, files, ext);
            return files;
        }

        public static List<string> GetFilesInternal<T>(string path, List<string> files, string ext)
        {
            foreach (string file in DuckFile.GetFiles(path, ext))
                files.Add(file);
            foreach (string directory in GetDirectories(path))
                GetFilesInternal<T>(directory, files, ext);
            return files;
        }

        private static void SearchDirLevels(string dir, LevelLocation location)
        {
            foreach (string path in location == LevelLocation.Content ? GetFiles(dir) : DuckFile.GetFiles(dir, "*.*"))
                ProcessLevel(path, location);
            foreach (string dir1 in location == LevelLocation.Content ? GetDirectories(dir) : DuckFile.GetDirectories(dir))
                SearchDirLevels(dir1, location);
        }

        public static void ReloadLevels(string s) => SearchDirLevels("Content/levels/" + s, LevelLocation.Content);

        private static void SearchDirTextures(string dir, bool reverse = false)
        {
            if (reverse)
            {
                foreach (string path in DG.Reverse(GetFiles(dir)))
                    ProcessTexture(path);
                foreach (string dir1 in DG.Reverse(GetDirectories(dir)))
                {
                    if (!dir1.EndsWith("Audio") && !dir1.EndsWith("Shaders"))
                        SearchDirTextures(dir1, reverse);
                }
            }
            else
            {
                foreach (string file in GetFiles(dir))
                    ProcessTexture(file);
                foreach (string directory in GetDirectories(dir))
                {
                    if (!directory.EndsWith("Audio") && !directory.EndsWith("Shaders"))
                        SearchDirTextures(directory);
                }
            }
        }

        private static void SearchDirEffects(string dir)
        {
            foreach (string file in GetFiles(dir))
                ProcessEffect(file);
            foreach (string directory in GetDirectories(dir))
                SearchDirEffects(directory);
        }

        public static string GetLevelID(string path, LevelLocation loc = LevelLocation.Content)
        {
            if (!path.EndsWith(".lev"))
                path += ".lev";
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)_levels)
            {
                foreach (LevelData levelData in level.Value)
                {
                    if ((levelData.GetLocation() == loc || loc == LevelLocation.Any) && levelData.GetPath().EndsWith("/" + path))
                        return levelData.metaData.guid;
                }
            }
            string path1 = Content.path + "/levels/" + path;
            if (!path.EndsWith(".lev"))
                path += ".lev";
            LevelData dat = DuckFile.LoadLevel(path1);
            if (dat == null)
                return "";
            MapLevel(dat.metaData.guid, dat, loc);
            return dat.metaData.guid;
        }

        public static List<string> GetLevels(string dir, LevelLocation location) => GetLevels(dir, location, true, false, false);

        public static List<string> GetLevels(
          string dir,
          LevelLocation location,
          bool pRecursive,
          bool pOnline,
          bool pEightPlayer,
          bool pAllowNonRestrictedEightPlayer = false,
          bool pSkipFilters = false)
        {
            List<string> levels = new List<string>();
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)_levels)
            {
                foreach (LevelData levelData in level.Value)
                {
                    if ((levelData.GetLocation() == location || location == LevelLocation.Any) && (pSkipFilters || (!pOnline || levelData.metaData.online) && (!pEightPlayer || levelData.metaData.eightPlayer) && (pEightPlayer || !levelData.metaData.eightPlayer || pAllowNonRestrictedEightPlayer && !levelData.metaData.eightPlayerRestricted)))
                    {
                        string path = levelData.GetPath();
                        int num = path.IndexOf(dir + "/");
                        if (num >= 0 && (pRecursive || path.LastIndexOf("/") == num + dir.Length))
                            levels.Add(level.Key);
                    }
                }
            }
            return levels;
        }

        public static void ProcessLevel(string path, LevelLocation location) => MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
       {
           try
           {
               Main.SpecialCode = "Loading Level " + path != null ? path : "null";
               if (!path.EndsWith(".lev"))
                   return;
               LoadLevelData(path, location);
               ++MonoMain.loadyBits;
           }
           catch (Exception ex)
           {
               LogLevelFailure(ex.ToString());
           }
       },null, "Loading Level"));

        private static LevelData LoadLevelData(string pPath, LevelLocation pLocation)
        {
            pPath = pPath.Replace('\\', '/');
            if (Program.IsLinuxD || Program.isLinux)
            {
                pPath = pPath.Replace('\\', '/');
                pPath = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(pPath), true);
            }
            LevelData dat = pLocation != LevelLocation.Content ? DuckFile.LoadLevel(pPath) : DuckFile.LoadLevel(DuckFile.ReadEntireStream(DuckFile.OpenStream(pPath)));
            if (dat == null)
                return null;
            dat.SetPath(pPath);
            pPath = pPath.Substring(0, pPath.Length - 4);
            pPath.Substring(pPath.IndexOf("/levels/") + 8);
            if (dat.metaData.guid != null)
                MapLevel(dat.metaData.guid, dat, pLocation);
            return dat;
        }

        private static void LogLevelFailure(string s)
        {
            try
            {
                Program.LogLine("Level Load Failure (Did not cause crash)\n================================================\n " + s + "\n================================================\n");
            }
            catch (Exception)
            {
            }
        }

        private static void ProcessTexture(string path)
        {
            if (!path.EndsWith(".xnb"))
                return;
            path = path.Replace('\\', '/');
            if (path.StartsWith("Content/"))
                path = path.Substring(8);
            path = path.Substring(0, path.Length - 4);
            MonoMain.NloadMessage = "Loading Textures (" + path + ")";
            MonoMain.lazyLoadActions.Enqueue(() => Load<Tex2D>(path));
            ++MonoMain.lazyLoadyBits;
        }

        private static void ProcessEffect(string path)
        {
            try
            {
                if (!path.EndsWith(".xnb"))
                    return;
                path = path.Replace('\\', '/');
                if (path.StartsWith("Content/"))
                    path = path.Substring(8);
                path = path.Substring(0, path.Length - 4);
                Load<MTEffect>(path);
                ++MonoMain.lazyLoadyBits;
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Failed to load shader (" + path + "):");
                DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
            }
        }

        public static string path => _path;

        public static void InitializeBase(ContentManager manager)
        {
            _base = manager;
            invalidTexture = Load<Tex2D>("notexture");
            _path = Directory.GetCurrentDirectory() + "/Content/";
        }

        public static void InitializeLevels()
        {
            MonoMain.NloadMessage = "Loading Levels";
            SearchDirLevels("Content/levels", LevelLocation.Content);
            if (!Steam.IsInitialized())
                return;
            //   LoadingAction steamLoad = new LoadingAction();
            //   steamLoad.action = () =>
            //  {
            //      WorkshopQueryUser queryUser = Steam.CreateQueryUser(Steam.user.id, WorkshopList.Subscribed, WorkshopType.UsableInGame, WorkshopSortOrder.TitleAsc);
            //      queryUser.requiredTags.Add("Map");
            //      queryUser.onlyQueryIDs = true;
            //      queryUser.QueryFinished += sender => steamLoad.flag = true;
            //      queryUser.ResultFetched += (sender, result) =>
            //{
            //    WorkshopItem publishedFile = result.details.publishedFile;
            //    if ((publishedFile.stateFlags & WorkshopItemState.Installed) == WorkshopItemState.None)
            //        return;
            //    DuckGame.Content.SearchDirLevels(publishedFile.path, LevelLocation.Workshop);
            //};
            //      queryUser.Request();
            //      Steam.Update();
            //  };
            //   steamLoad.waitAction = () =>
            //  {
            //      Steam.Update();
            //      return steamLoad.flag;
            //  };
            //   MonoMain.currentActionQueue.Enqueue(steamLoad);
        }

        public static Vec2 GetTextureSize(string pName)
        {
            Vec2 zero = Vec2.Zero;
            return _spriteSizeDirectory.TryGetValue(pName, out zero) ? zero : Vec2.Zero;
        }

        public static void InitializeTextureSizeDictionary()
        {
            try
            {
                if (!File.Exists(DuckFile.contentDirectory + "texture_size_directory.dat"))
                    return;
                foreach (string readAllLine in File.ReadAllLines(DuckFile.contentDirectory + "texture_size_directory.dat"))
                {
                    char[] chArray = new char[1] { ',' };
                    string[] strArray = readAllLine.Split(chArray);
                    _spriteSizeDirectory[strArray[0].Trim().Replace('\\', '/')] = new Vec2(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Error initializing texture_size_directory.dat:");
                DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
            }
        }
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color((byte)(r * a / 255), (byte)(g * a / 255), (byte)(b * a / 255), (byte)(a));
        }

        public static Texture2D SpriteAtlasTextureFromStream(string FilePath, GraphicsDevice device)
        {
            Texture2D texture;
            FileStream titleStream = File.OpenRead(FilePath);
            texture = Texture2D.FromStream(device, titleStream);
            titleStream.Close();
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = FromNonPremultiplied(buffer[i].r, buffer[i].g, buffer[i].b, buffer[i].a); // Needs to handle transparent textures that use other types of draw calls
            texture.SetData(buffer);
            return texture;
        }
        public static List<string> RSplit(string stringInput, char target, int maxsplits = -1)
        {
            int splitcount = 0;
            List<string> split = new List<string>();
            int lastindex = stringInput.Length;
            for (int i = stringInput.Length; i-- > 0;)
            {

                if (stringInput[i] == target && (maxsplits > splitcount || maxsplits == -1))
                {
                    string s = stringInput.Substring(i + 1, lastindex - i - 1);
                    split.Add(s);
                    lastindex = i;
                    splitcount += 1;
                }
            }
            split.Add(stringInput.Substring(0, lastindex));
            split.Reverse();
            return split;
        }
        public static bool didsetbigboi;
        public static Tex2D Thick;
        public static Dictionary<string, Microsoft.Xna.Framework.Rectangle> offests = new Dictionary<string, Microsoft.Xna.Framework.Rectangle>();
        public static void Initialize(bool reverse)
        {
            MonoMain.NloadMessage = "Loading Textures";

            SearchDirTextures("Content/", reverse);
        }

        public static void Initialize() => Initialize(false);

        public static void InitializeEffects()
        {
            MonoMain.NloadMessage = "Loading Effects";
            SearchDirEffects("Content/Shaders");
        }

        public static string[] GetFiles(string path, string filter = "*.*")
        {
            path = path.Replace('\\', '/');
            if (Path.IsPathRooted(path) && Program.IsLinuxD)
            {
                while (path.EndsWith("/"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }
            else
            {
                path = path.Trim('/');
            }
            string str1 = (Directory.GetCurrentDirectory() + "/").Replace('\\', '/');
            List<string> stringList = new List<string>();
            foreach (string path1 in DuckFile.GetFilesNoCloud(path, filter))
            {
                if (!Path.GetFileName(path1).Contains("._"))
                {
                    string str2 = path1.Replace('\\', '/');
                    int startIndex = str2.IndexOf(str1);
                    if (startIndex != -1)
                        str2 = str2.Remove(startIndex, str1.Length);
                    stringList.Add(str2);
                }
            }
            return stringList.ToArray();
        }

        public static string[] GetDirectories(string path, string filter = "*.*")
        {
            path = path.Replace('\\', '/');
            if (Path.IsPathRooted(path) && Program.IsLinuxD)
            {
                while (path.EndsWith("/"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }
            else
            {
                path = path.Trim('/');
            }
            List<string> stringList = new List<string>();
            foreach (string path1 in DuckFile.GetDirectoriesNoCloud(path))
            {
                if (!Path.GetFileName(path1).Contains("._"))
                    stringList.Add(path1);
            }
            return stringList.ToArray();
        }

        public static void Update()
        {
        }

        public static ParallaxBackground.Definition LoadParallaxDefinition(
          string pName)
        {
            try
            {
                pName.Replace(".png", ".txt");
                if (!pName.EndsWith(".txt"))
                    pName += ".txt";
                ParallaxBackground.Definition definition = null;
                if (_parallaxDefinitions.TryGetValue(pName, out definition))
                    return definition;
                string path = pName;
                if (!pName.Contains(":"))
                    path = DuckFile.contentDirectory + pName;
                string[] strArray1 = null;
                if (ReskinPack.active.Count > 0)
                    strArray1 = ReskinPack.LoadAsset<string[]>(pName);
                if (strArray1 == null && File.Exists(path))
                    strArray1 = File.ReadAllLines(path);
                if (strArray1 != null)
                {
                    try
                    {
                        definition = new ParallaxBackground.Definition();
                        foreach (string str in strArray1)
                        {
                            if (!str.StartsWith("[") && !string.IsNullOrWhiteSpace(str))
                            {
                                string[] strArray2 = str.Split(',');
                                ParallaxBackground.Definition.Zone zone = new ParallaxBackground.Definition.Zone
                                {
                                    index = Convert.ToInt32(strArray2[0].Trim()),
                                    distance = Convert.ToSingle(strArray2[1].Trim()),
                                    speed = Convert.ToSingle(strArray2[2].Trim()),
                                    moving = Convert.ToBoolean(strArray2[3].Trim())
                                };
                                if (strArray2.Length > 4)
                                {
                                    zone.sprite = new Sprite(strArray2[4].Trim());
                                    if (strArray2.Length > 6)
                                        zone.sprite.position = new Vec2(Convert.ToSingle(strArray2[5].Trim()), Convert.ToSingle(strArray2[6].Trim()));
                                    if (strArray2.Length > 7)
                                        zone.sprite.depth = (Depth)Convert.ToSingle(strArray2[7].Trim());
                                }
                                if (zone.sprite != null)
                                    definition.sprites.Add(zone);
                                else
                                    definition.zones.Add(zone);
                            }
                        }
                        return definition;
                    }
                    catch (Exception ex)
                    {
                        DevConsole.Log(DCSection.General, "|DGRED|LoadParallaxDefinition error (" + pName + "):");
                        DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static T Load<T>(string name)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                name = name.Replace("//", "/").Replace("\\", "/");
                try
                {
                    name = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(name));
                }
                catch
                {
                    DevConsole.Log("couldnt fix path mabye file isnt real " + name);
                }
            }
            if (ReskinPack.active.Count > 0)
            {
                try
                {
                    if (typeof(T) == typeof(Tex2D))
                    {
                        Texture2D texture2D = ReskinPack.LoadAsset<Texture2D>(name);
                        if (texture2D != null)
                        {
                            lock (_loadLock)
                            {
                                Vec2 textureSize = GetTextureSize(name);
                                Tex2D tex2D;
                                if (textureSize != Vec2.Zero && (texture2D.Width != textureSize.x || texture2D.Height != textureSize.y))
                                    tex2D = new BigBoyTex2D(texture2D, name, _currentTextureIndex)
                                    {
                                        scaleFactor = (textureSize.x / texture2D.Width)
                                    };
                                else
                                    tex2D = new Tex2D(texture2D, name, _currentTextureIndex);
                                ++_currentTextureIndex;
                                _textureList.Add(tex2D);
                                _textures[name] = tex2D;
                                _texture2DMap[texture2D] = tex2D;
                                return (T)(object)tex2D;
                            }
                        }
                    }
                    else
                    {
                        T obj = ReskinPack.LoadAsset<T>(name);
                        if (obj != null)
                            return obj;
                    }
                }
                catch (Exception)
                {
                }
            }
            if (typeof(T) == typeof(Tex2D))
            {
                Tex2D tex2D = null;
                lock (_textures)
                    _textures.TryGetValue(name, out tex2D);
                if (tex2D == null)
                {
                    Texture2D texture2D = null;
                    bool flag = false;
                    if (MonoMain.moddingEnabled && ModLoader.accessibleMods.Count() > 1 && name.Length > 1 && name[1] == ':')
                        flag = true;
                    if (!flag)
                    {
                        try
                        {
                            texture2D = _base.Load<Texture2D>(name);
                        }
                        catch (Exception ex)
                        {
                            flag = MonoMain.moddingEnabled && ModLoader.modsEnabled;
                            lastException = ex;
                        }
                    }
                    if (flag)
                    {
                        foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                        {
                            if (accessibleMod.configuration != null && accessibleMod.configuration.content != null)
                                texture2D = accessibleMod.configuration.content.Load<Texture2D>(name);
                            if (texture2D != null)
                                break;
                        }
                    }
                    else if (texture2D == null)
                    {
                        try
                        {
                            texture2D = ContentPack.LoadTexture2D(name);
                        }
                        catch (Exception ex)
                        {
                            lastException = ex;
                        }
                    }
                    if (texture2D == null)
                    {
                        texture2D = (Texture2D)invalidTexture;
                        Main.SpecialCode = "Couldn't load texture " + name;
                    }
                    lock (_loadLock)
                    {
                        tex2D = new Tex2D(texture2D, name, _currentTextureIndex);
                        ++_currentTextureIndex;
                        _textureList.Add(tex2D);
                        _textures[name] = tex2D;
                        _texture2DMap[texture2D] = tex2D;
                    }
                }
                return (T)(object)tex2D;
            }
            if (typeof(T) == typeof(MTEffect))
            {
                MTEffect mtEffect = null;
                lock (_effects)
                    _effects.TryGetValue(name, out mtEffect);
                if (mtEffect == null)
                {
                    Effect effect = null;
                    lock (_loadLock)
                        effect = _base.Load<Effect>(name);
                    lock (_loadLock)
                    {
                        mtEffect = new MTEffect(effect, name, _currentEffectIndex);
                        ++_currentEffectIndex;
                        _effectList.Add(mtEffect);
                        _effects[name] = mtEffect;
                        _effectMap[effect] = mtEffect;
                    }
                }
                return (T)(object)mtEffect;
            }
            if (typeof(T) == typeof(SoundEffect))
            {
                SoundEffect soundEffect = null;
                lock (_sounds)
                    _sounds.TryGetValue(name, out soundEffect);
                if (soundEffect == null)
                {
                    if (!name.Contains(":") && !name.EndsWith(".wav"))
                    {
                        lock (_loadLock)
                        {
                            try
                            {
                                string path = DuckFile.contentDirectory + name + ".wav";
                                soundEffect = SoundEffect.FromStream(new MemoryStream(File.ReadAllBytes(path)));
                                if (soundEffect != null)
                                    soundEffect.file = path;
                            }
                            catch (Exception ex)
                            {
                                lastException = ex;
                            }
                        }
                    }
                    if (soundEffect == null && MonoMain.moddingEnabled && ModLoader.modsEnabled)
                    {
                        foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                        {
                            if (accessibleMod.configuration != null && accessibleMod.configuration.content != null)
                                soundEffect = accessibleMod.configuration.content.Load<SoundEffect>(name);
                            if (soundEffect != null)
                                break;
                        }
                    }
                }
                if (soundEffect == null)
                    Main.SpecialCode = "Couldn't load sound (" + soundEffect?.ToString() + ")";
                else
                    _sounds[name] = soundEffect;
                return (T)(object)soundEffect;
            }
            if (typeof(T) == typeof(Song))
            {
                if (MonoMain.moddingEnabled && ModLoader.modsEnabled)
                {
                    foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                    {
                        if (accessibleMod.configuration != null && accessibleMod.configuration.content != null)
                        {
                            Song song = accessibleMod.configuration.content.Load<Song>(name);
                            if (song != null)
                                return (T)(object)song;
                        }
                    }
                }
                return default(T);
            }
            return typeof(T) == typeof(Microsoft.Xna.Framework.Media.Song) ? (T)(object)_base.Load<Microsoft.Xna.Framework.Media.Song>(name) : _base.Load<T>(name);
        }
    }
}
