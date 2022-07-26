// Decompiled with JetBrains decompiler
// Type: DuckGame.Content
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private static volatile bool previewRendering = false;
        public static volatile bool renderingPreview = false;
        public static XMLLevel previewLevel;
        private static Camera _previewCamera;
        public static volatile bool cancelPreview = false;
        public static int customPreviewWidth = 0;
        public static int customPreviewHeight = 0;
        public static Vec2 customPreviewCenter = Vec2.Zero;
        private static LevelData _previewLevelData;
        private static LayerCore _previewLayerCore = (LayerCore)null;
        private static string _previewPath = (string)null;
        private static MTSpriteBatch _previewBatch;
        private static bool _previewBackground = false;
        private static Thread _previewThread;
        private static RenderTarget2D _currentPreviewTarget;
        private static LevelMetaData.PreviewPair _currentPreviewPair;
        public static bool doingTempSave = false;
        public static byte[] generatePreviewBytes;
        public static bool renderingToTarget = false;
        private static Dictionary<System.Type, string> _extensionList = new Dictionary<System.Type, string>()
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
        public static Exception lastException = (Exception)null;
        private static Dictionary<string, ParallaxBackground.Definition> _parallaxDefinitions = new Dictionary<string, ParallaxBackground.Definition>();

        public static LevelData GetLevel(string guid, LevelLocation location = LevelLocation.Any)
        {
            List<LevelData> list;
            if (guid != null && DuckGame.Content._levels.TryGetValue(guid, out list))
            {
                foreach (LevelData level in list)
                {
                    if (level.GetLocation() == location || location == LevelLocation.Any)
                        return level;
                }
            }
            return (LevelData)null;
        }

        public static List<LevelData> GetAllLevels(string guid)
        {
            List<LevelData> list;
            return DuckGame.Content._levels.TryGetValue(guid, out list) ? list : new List<LevelData>();
        }

        public static List<LevelData> GetAllLevels()
        {
            List<LevelData> allLevels = new List<LevelData>();
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)DuckGame.Content._levels)
                allLevels.AddRange((IEnumerable<LevelData>)level.Value);
            return allLevels;
        }

        public static void MapLevel(string lev, LevelData dat, LevelLocation location)
        {
            lock (DuckGame.Content._levels)
            {
                List<LevelData> list;
                if (DuckGame.Content._levels.TryGetValue(lev, out list))
                {
                    LevelData levelData1 = (LevelData)null;
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
                DuckGame.Content._levels.Add(lev, dat);
            }
        }

        public static List<MTEffect> effectList => DuckGame.Content._effectList;

        public static Dictionary<string, Tex2D> textures => DuckGame.Content._textures;

        public static List<Tex2D> textureList => DuckGame.Content._textureList;

        private static void PreviewThread()
        {
            Level activeLevel = Level.activeLevel;
            Level currentLevel = Level.core.currentLevel;
            LayerCore core = Layer.core;
            try
            {
                DuckGame.Content.renderingPreview = true;
                if (!DuckGame.Content._previewBackground)
                    Thing.skipLayerAdding = true;
                XMLLevel xmlLevel;
                if (DuckGame.Content._previewLevelData == null)
                {
                    xmlLevel = new XMLLevel(DuckGame.Content._previewPath);
                }
                else
                {
                    xmlLevel = new XMLLevel(DuckGame.Content._previewLevelData);
                    DuckGame.Content._previewLevelData = (LevelData)null;
                }
                if (DuckGame.Content.cancelPreview)
                    return;
                DuckGame.Content.previewLevel = xmlLevel;
                DuckGame.Content.previewLevel.ignoreVisibility = true;
                Level.skipInitialize = !DuckGame.Content._previewBackground;
                if (!DuckGame.Content._previewBackground)
                    DuckGame.Content.previewLevel.isPreview = true;
                DuckGame.Content._previewLayerCore = (LayerCore)null;
                if (DuckGame.Content._previewBackground)
                {
                    Layer.core = DuckGame.Content._previewLayerCore = new LayerCore();
                    Layer.core.InitializeLayers();
                }
                Level.core.currentLevel = (Level)DuckGame.Content.previewLevel;
                Level.activeLevel = (Level)DuckGame.Content.previewLevel;
                DuckGame.Content.previewLevel.Initialize();
                Level.activeLevel = activeLevel;
                Level.core.currentLevel = currentLevel;
                if (DuckGame.Content.cancelPreview)
                    return;
                Thing.skipLayerAdding = false;
                Level.skipInitialize = false;
                DuckGame.Content.previewLevel.CalculateBounds();
                DuckGame.Content._previewCamera = DuckGame.Content.customPreviewWidth == 0 ? new Camera(0.0f, 0.0f, 1280f, 1280f * DuckGame.Graphics.aspect) : new Camera(0.0f, 0.0f, (float)DuckGame.Content.customPreviewWidth, (float)DuckGame.Content.customPreviewHeight);
                Vec2 vec2 = (DuckGame.Content.previewLevel.topLeft + DuckGame.Content.previewLevel.bottomRight) / 2f;
                if (DuckGame.Content.cancelPreview)
                    return;
                DuckGame.Content._previewCamera.width /= 2f;
                DuckGame.Content._previewCamera.height /= 2f;
                DuckGame.Content._previewCamera.center = !(DuckGame.Content.customPreviewCenter != Vec2.Zero) ? vec2 : DuckGame.Content.customPreviewCenter;
                DuckGame.Content.readyToRenderPreview = true;
                if (DuckGame.Content._previewThread != null)
                {
                    while (DuckGame.Content.readyToRenderPreview)
                    {
                        if (DuckGame.Content.cancelPreview)
                            return;
                    }
                }
                DuckGame.Content.previewRendering = false;
                DuckGame.Content.renderingPreview = false;
            }
            catch (Exception ex)
            {
                Program.LogLine(ex.ToString());
                DuckGame.Content.renderingPreview = false;
                Thing.skipLayerAdding = false;
                Level.skipInitialize = false;
            }
            if (!DuckGame.Content._previewBackground)
                return;
            Level.activeLevel = activeLevel;
            Level.core.currentLevel = currentLevel;
            Layer.core = core;
        }

        private static void DoPreviewRender(bool pSaveMetadata)
        {
            MTSpriteBatch screen = DuckGame.Graphics.screen;
            DuckGame.Graphics.screen = DuckGame.Content._previewBatch;
            Viewport viewport = DuckGame.Graphics.viewport;
            RenderTarget2D currentRenderTarget = DuckGame.Graphics.currentRenderTarget;
            DuckGame.Graphics.SetRenderTarget(DuckGame.Content._currentPreviewTarget);
            DuckGame.Graphics.viewport = new Viewport(0, 0, DuckGame.Content._currentPreviewTarget.width, DuckGame.Content._currentPreviewTarget.height);
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
            if (DuckGame.Content._previewBackground)
            {
                Level activeLevel = Level.activeLevel;
                Level currentLevel = Level.core.currentLevel;
                LayerCore core = Layer.core;
                if (DuckGame.Content._previewLayerCore != null)
                    Layer.core = DuckGame.Content._previewLayerCore;
                Level.activeLevel = (Level)DuckGame.Content.previewLevel;
                Level.core.currentLevel = (Level)DuckGame.Content.previewLevel;
                try
                {
                    DuckGame.Graphics.defaultRenderTarget = DuckGame.Content._currentPreviewTarget;
                    Layer.HUD.visible = false;
                    DuckGame.Content.previewLevel.camera = DuckGame.Content._previewCamera;
                    DuckGame.Content.previewLevel.simulatePhysics = false;
                    DuckGame.Content.previewLevel.DoUpdate();
                    DuckGame.Content.previewLevel.DoUpdate();
                    DuckGame.Content.previewLevel.DoDraw();
                    Layer.HUD.visible = true;
                    DuckGame.Graphics.defaultRenderTarget = (RenderTarget2D)null;
                    Level.activeLevel = activeLevel;
                    Level.core.currentLevel = currentLevel;
                    Layer.core = core;
                }
                catch (Exception ex)
                {
                    Layer.HUD.visible = true;
                    DuckGame.Graphics.defaultRenderTarget = (RenderTarget2D)null;
                    Level.activeLevel = activeLevel;
                    Level.core.currentLevel = currentLevel;
                    Layer.core = core;
                    throw ex;
                }
            }
            else
            {
                DuckGame.Graphics.Clear(Color.Black);
                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (MTEffect)null, DuckGame.Content._previewCamera.getMatrix());
                foreach (Thing thing in DuckGame.Content.previewLevel.things)
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
                    DuckGame.Graphics.material = (Material)null;
                }
                if (DuckGame.Content.previewLevel.things.Count == 1)
                {
                    ImportMachine importMachine = DuckGame.Content.previewLevel.things.First<Thing>() as ImportMachine;
                }
                DuckGame.Graphics.screen.End();
            }
            DuckGame.Graphics.screen = screen;
            DuckGame.Graphics.SetRenderTarget(currentRenderTarget);
            Custom.data[CustomType.Block][0] = str1;
            Custom.data[CustomType.Block][1] = str2;
            Custom.data[CustomType.Block][2] = str3;
            Custom.data[CustomType.Background][0] = str4;
            Custom.data[CustomType.Background][1] = str5;
            Custom.data[CustomType.Background][2] = str6;
            Custom.data[CustomType.Platform][0] = str7;
            Custom.data[CustomType.Platform][1] = str8;
            Custom.data[CustomType.Platform][2] = str9;
            if (!pSaveMetadata || DuckGame.Content.doingTempSave)
                return;
            LevelMetaData levelMetaData = Editor.ReadLevelMetadata(DuckGame.Content.previewLevel.data);
            if (levelMetaData == null || levelMetaData.guid == null)
                return;
            DuckGame.Content._currentPreviewPair = levelMetaData.SavePreview((Texture2D)(Tex2D)DuckGame.Content._currentPreviewTarget, pInvalidData, pStrange, pChallenge, pArcade);
        }

        public static Thread previewThread => DuckGame.Content._previewThread;

        public static LevelMetaData.PreviewPair GeneratePreview(
          LevelData levelData,
          bool pRefresh = false,
          RenderTarget2D pCustomPreviewTarget = null)
        {
            DuckGame.Content._previewLevelData = levelData;
            return DuckGame.Content.GeneratePreview((string)null, pRefresh, pCustomPreviewTarget);
        }

        public static LevelMetaData.PreviewPair GeneratePreview(
          string levelPath,
          bool pRefresh = false,
          RenderTarget2D pCustomPreviewTarget = null)
        {
            if (DuckGame.Content.generatePreviewBytes != null)
            {
                DuckGame.Content._previewLevelData = DuckFile.LoadLevel(DuckGame.Content.generatePreviewBytes);
                DuckGame.Content.generatePreviewBytes = (byte[])null;
            }
            bool flag = false;
            if (pCustomPreviewTarget != null)
                flag = true;
            else if (!pRefresh && levelPath != null)
            {
                LevelMetaData levelMetaData = DuckGame.Content._previewLevelData == null ? Editor.ReadLevelMetadata(levelPath) : Editor.ReadLevelMetadata(DuckGame.Content._previewLevelData);
                if (levelMetaData != null)
                {
                    LevelMetaData.PreviewPair preview = levelMetaData.LoadPreview();
                    if (preview != null)
                        return preview;
                }
            }
            DevConsole.Log(DCSection.General, "Generating preview data for (" + levelPath + ")...");
            DuckGame.Content._previewBackground = flag;
            DuckGame.Content.readyToRenderPreview = false;
            if (DuckGame.Content._previewThread != null && DuckGame.Content._previewThread.IsAlive)
            {
                DuckGame.Content.cancelPreview = true;
                int num = 250;
                while (DuckGame.Content._previewThread.IsAlive)
                {
                    Tasker.RunTasks();
                    Thread.Sleep(2);
                    --num;
                }
                DuckGame.Content.readyToRenderPreview = false;
            }
            DuckGame.Content._previewThread = (Thread)null;
            DuckGame.Content.cancelPreview = false;
            Thing.skipLayerAdding = false;
            Level.skipInitialize = false;
            if (DuckGame.Content._previewBatch == null)
                DuckGame.Content._previewBatch = new MTSpriteBatch(DuckGame.Graphics.device);
            DuckGame.Content._previewPath = levelPath;
            DuckGame.Content._currentPreviewTarget = pCustomPreviewTarget == null ? new RenderTarget2D(320, 200) : pCustomPreviewTarget;
            DuckGame.Content.renderingToTarget = true;
            DuckGame.Content.renderingPreview = true;
            DuckGame.Content.readyToRenderPreview = true;
            DuckGame.Content.PreviewThread();
            DuckGame.Content.DoPreviewRender(pCustomPreviewTarget == null);
            DuckGame.Content.renderingPreview = false;
            DuckGame.Content.readyToRenderPreview = false;
            DuckGame.Content.renderingToTarget = false;
            return DuckGame.Content._currentPreviewPair;
        }

        public static void SetTextureAtIndex(short index, Tex2D tex)
        {
            while ((int)index >= DuckGame.Content._textureList.Count)
            {
                DuckGame.Content._textureList.Add((Tex2D)null);
                ++DuckGame.Content._currentTextureIndex;
            }
            DuckGame.Content._textureList[(int)index] = tex;
            DuckGame.Content._texture2DMap[tex.nativeObject] = tex;
            DuckGame.Content._textures[tex.textureName] = tex;
            tex.SetTextureIndex(index);
        }

        public static Tex2D AssignTextureIndex(Tex2D tex)
        {
            Tex2D tex2D = (Tex2D)null;
            DuckGame.Content._texture2DMap.TryGetValue((object)tex, out tex2D);
            if (tex2D == null)
            {
                tex.SetTextureIndex(DuckGame.Content._currentTextureIndex);
                ++DuckGame.Content._currentTextureIndex;
                DuckGame.Content._textureList.Add(tex);
                DuckGame.Content._texture2DMap[(object)tex] = tex;
            }
            return tex2D;
        }

        public static Tex2D GetTex2D(object tex) => DuckGame.Content.GetTex2D((Texture2D)tex);

        public static Tex2D GetTex2D(Texture2D tex)
        {
            if (tex == null)
                return (Tex2D)null;
            Tex2D tex2D = (Tex2D)null;
            DuckGame.Content._texture2DMap.TryGetValue((object)tex, out tex2D);
            if (tex2D == null)
            {
                tex2D = new Tex2D(tex, "", DuckGame.Content._currentTextureIndex);
                ++DuckGame.Content._currentTextureIndex;
                DuckGame.Content._textureList.Add(tex2D);
                DuckGame.Content._texture2DMap[(object)tex] = tex2D;
            }
            return tex2D;
        }

        public static void SetEffectAtIndex(short index, MTEffect e)
        {
            while ((int)index > DuckGame.Content._effectList.Count)
            {
                DuckGame.Content._effectList.Add((MTEffect)null);
                ++DuckGame.Content._currentEffectIndex;
            }
            DuckGame.Content._effectList[(int)index] = e;
            DuckGame.Content._effectMap[e.effect] = e;
            DuckGame.Content._effects[e.effectName] = e;
            e.SetEffectIndex(index);
        }

        public static MTEffect GetMTEffect(Effect e)
        {
            MTEffect mtEffect = (MTEffect)null;
            DuckGame.Content._effectMap.TryGetValue(e, out mtEffect);
            if (mtEffect == null)
            {
                mtEffect = new MTEffect(e, "", DuckGame.Content._currentEffectIndex);
                ++DuckGame.Content._currentEffectIndex;
                DuckGame.Content._effectList.Add(mtEffect);
                DuckGame.Content._effectMap[e] = mtEffect;
            }
            return mtEffect;
        }

        public static Tex2D GetTex2DFromIndex(short index) => DuckGame.Content._textureList[(int)index];

        public static MTEffect GetMTEffectFromIndex(short index) => index < (short)0 ? (MTEffect)null : DuckGame.Content._effectList[(int)index];

        public static List<string> GetFiles<T>(string path)
        {
            List<string> files = new List<string>();
            string ext = (string)null;
            if (DuckGame.Content._extensionList.TryGetValue(typeof(T), out ext))
                DuckGame.Content.GetFilesInternal<T>(path, files, ext);
            return files;
        }

        public static List<string> GetFilesInternal<T>(string path, List<string> files, string ext)
        {
            foreach (string file in DuckFile.GetFiles(path, ext))
                files.Add(file);
            foreach (string directory in DuckGame.Content.GetDirectories(path))
                DuckGame.Content.GetFilesInternal<T>(directory, files, ext);
            return files;
        }

        private static void SearchDirLevels(string dir, LevelLocation location)
        {
            foreach (string path in location == LevelLocation.Content ? DuckGame.Content.GetFiles(dir) : DuckFile.GetFiles(dir, "*.*"))
                DuckGame.Content.ProcessLevel(path, location);
            foreach (string dir1 in location == LevelLocation.Content ? DuckGame.Content.GetDirectories(dir) : DuckFile.GetDirectories(dir))
                DuckGame.Content.SearchDirLevels(dir1, location);
        }

        public static void ReloadLevels(string s) => DuckGame.Content.SearchDirLevels("Content/levels/" + s, LevelLocation.Content);

        private static void SearchDirTextures(string dir, bool reverse = false)
        {
            if (reverse)
            {
                foreach (string path in DG.Reverse<string>(DuckGame.Content.GetFiles(dir)))
                    DuckGame.Content.ProcessTexture(path);
                foreach (string dir1 in DG.Reverse<string>(DuckGame.Content.GetDirectories(dir)))
                {
                    if (!dir1.EndsWith("Audio") && !dir1.EndsWith("Shaders"))
                        DuckGame.Content.SearchDirTextures(dir1, reverse);
                }
            }
            else
            {
                foreach (string file in DuckGame.Content.GetFiles(dir))
                    DuckGame.Content.ProcessTexture(file);
                foreach (string directory in DuckGame.Content.GetDirectories(dir))
                {
                    if (!directory.EndsWith("Audio") && !directory.EndsWith("Shaders"))
                        DuckGame.Content.SearchDirTextures(directory);
                }
            }
        }

        private static void SearchDirEffects(string dir)
        {
            foreach (string file in DuckGame.Content.GetFiles(dir))
                DuckGame.Content.ProcessEffect(file);
            foreach (string directory in DuckGame.Content.GetDirectories(dir))
                DuckGame.Content.SearchDirEffects(directory);
        }

        public static string GetLevelID(string path, LevelLocation loc = LevelLocation.Content)
        {
            if (!path.EndsWith(".lev"))
                path += ".lev";
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)DuckGame.Content._levels)
            {
                foreach (LevelData levelData in level.Value)
                {
                    if ((levelData.GetLocation() == loc || loc == LevelLocation.Any) && levelData.GetPath().EndsWith("/" + path))
                        return levelData.metaData.guid;
                }
            }
            string path1 = DuckGame.Content.path + "/levels/" + path;
            if (!path.EndsWith(".lev"))
                path += ".lev";
            LevelData dat = DuckFile.LoadLevel(path1);
            if (dat == null)
                return "";
            DuckGame.Content.MapLevel(dat.metaData.guid, dat, loc);
            return dat.metaData.guid;
        }

        public static List<string> GetLevels(string dir, LevelLocation location) => DuckGame.Content.GetLevels(dir, location, true, false, false);

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
            foreach (KeyValuePair<string, List<LevelData>> level in (MultiMap<string, LevelData, List<LevelData>>)DuckGame.Content._levels)
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

        public static void ProcessLevel(string path, LevelLocation location) => MonoMain.currentActionQueue.Enqueue(new LoadingAction((Action)(() =>
       {
           try
           {
               Main.SpecialCode = "Loading Level " + path != null ? path : "null";
               if (!path.EndsWith(".lev"))
                   return;
               DuckGame.Content.LoadLevelData(path, location);
               ++MonoMain.loadyBits;
           }
           catch (Exception ex)
           {
               DuckGame.Content.LogLevelFailure(ex.ToString());
           }
       })));

        private static LevelData LoadLevelData(string pPath, LevelLocation pLocation)
        {
            pPath = pPath.Replace('\\', '/');
            LevelData dat = pLocation != LevelLocation.Content ? DuckFile.LoadLevel(pPath) : DuckFile.LoadLevel(DuckFile.ReadEntireStream(DuckFile.OpenStream(pPath)));
            if (dat == null)
                return (LevelData)null;
            dat.SetPath(pPath);
            pPath = pPath.Substring(0, pPath.Length - 4);
            pPath.Substring(pPath.IndexOf("/levels/") + 8);
            if (dat.metaData.guid != null)
                DuckGame.Content.MapLevel(dat.metaData.guid, dat, pLocation);
            return dat;
        }

        private static void LogLevelFailure(string s)
        {
            try
            {
                Program.LogLine("Level Load Failure (Did not cause crash)\n================================================\n " + s + "\n================================================\n");
            }
            catch (Exception ex)
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
            MonoMain.loadMessage = "Loading Textures (" + path + ")";
            MonoMain.lazyLoadActions.Enqueue((Action)(() => DuckGame.Content.Load<Tex2D>(path)));
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
                DuckGame.Content.Load<MTEffect>(path);
                ++MonoMain.lazyLoadyBits;
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Failed to load shader (" + path + "):");
                DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
            }
        }

        public static string path => DuckGame.Content._path;

        public static void InitializeBase(ContentManager manager)
        {
            DuckGame.Content._base = manager;
            DuckGame.Content.invalidTexture = DuckGame.Content.Load<Tex2D>("notexture");
            DuckGame.Content._path = Directory.GetCurrentDirectory() + "/Content/";
        }

        public static void InitializeLevels()
        {
            MonoMain.loadMessage = "Loading Levels";
            DuckGame.Content.SearchDirLevels("Content/levels", LevelLocation.Content);
            if (!Steam.IsInitialized())
                return;
            LoadingAction steamLoad = new LoadingAction();
            steamLoad.action = (Action)(() =>
           {
               WorkshopQueryUser queryUser = Steam.CreateQueryUser(Steam.user.id, WorkshopList.Subscribed, WorkshopType.UsableInGame, WorkshopSortOrder.TitleAsc);
               queryUser.requiredTags.Add("Map");
               queryUser.onlyQueryIDs = true;
               queryUser.QueryFinished += (WorkshopQueryFinished)(sender => steamLoad.flag = true);
               queryUser.ResultFetched += (WorkshopQueryResultFetched)((sender, result) =>
         {
             WorkshopItem publishedFile = result.details.publishedFile;
             if ((publishedFile.stateFlags & WorkshopItemState.Installed) == WorkshopItemState.None)
                 return;
             DuckGame.Content.SearchDirLevels(publishedFile.path, LevelLocation.Workshop);
         });
               queryUser.Request();
               Steam.Update();
           });
            steamLoad.waitAction = (Func<bool>)(() =>
           {
               Steam.Update();
               return steamLoad.flag;
           });
            MonoMain.currentActionQueue.Enqueue(steamLoad);
        }

        public static Vec2 GetTextureSize(string pName)
        {
            Vec2 zero = Vec2.Zero;
            return DuckGame.Content._spriteSizeDirectory.TryGetValue(pName, out zero) ? zero : Vec2.Zero;
        }

        public static void InitializeTextureSizeDictionary()
        {
            try
            {
                if (!System.IO.File.Exists(DuckFile.contentDirectory + "texture_size_directory.dat"))
                    return;
                foreach (string readAllLine in System.IO.File.ReadAllLines(DuckFile.contentDirectory + "texture_size_directory.dat"))
                {
                    char[] chArray = new char[1] { ',' };
                    string[] strArray = readAllLine.Split(chArray);
                    DuckGame.Content._spriteSizeDirectory[strArray[0].Trim().Replace('\\', '/')] = new Vec2(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Error initializing texture_size_directory.dat:");
                DevConsole.Log(DCSection.General, "|DGRED|" + ex.Message);
            }
        }

        public static void Initialize(bool reverse)
        {
            MonoMain.loadMessage = "Loading Textures";
            DuckGame.Content.SearchDirTextures("Content/", reverse);
        }

        public static void Initialize() => DuckGame.Content.Initialize(false);

        public static void InitializeEffects()
        {
            MonoMain.loadMessage = "Loading Effects";
            DuckGame.Content.SearchDirEffects("Content/Shaders");
        }

        public static string[] GetFiles(string path, string filter = "*.*")
        {
            path = path.Replace('\\', '/');
            path = path.Trim('/');
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
            path = path.Trim('/');
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
                ParallaxBackground.Definition definition = (ParallaxBackground.Definition)null;
                if (DuckGame.Content._parallaxDefinitions.TryGetValue(pName, out definition))
                    return definition;
                string path = pName;
                if (!pName.Contains(":"))
                    path = DuckFile.contentDirectory + pName;
                string[] strArray1 = (string[])null;
                if (ReskinPack.active.Count > 0)
                    strArray1 = ReskinPack.LoadAsset<string[]>(pName);
                if (strArray1 == null && System.IO.File.Exists(path))
                    strArray1 = System.IO.File.ReadAllLines(path);
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
                                ParallaxBackground.Definition.Zone zone = new ParallaxBackground.Definition.Zone();
                                zone.index = Convert.ToInt32(strArray2[0].Trim());
                                zone.distance = Convert.ToSingle(strArray2[1].Trim());
                                zone.speed = Convert.ToSingle(strArray2[2].Trim());
                                zone.moving = Convert.ToBoolean(strArray2[3].Trim());
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
            catch (Exception ex)
            {
            }
            return (ParallaxBackground.Definition)null;
        }

        public static T Load<T>(string name)
        {
            if (ReskinPack.active.Count > 0)
            {
                try
                {
                    if (typeof(T) == typeof(Tex2D))
                    {
                        Texture2D texture2D = ReskinPack.LoadAsset<Texture2D>(name);
                        if (texture2D != null)
                        {
                            lock (DuckGame.Content._loadLock)
                            {
                                Vec2 textureSize = DuckGame.Content.GetTextureSize(name);
                                Tex2D tex2D;
                                if (textureSize != Vec2.Zero && ((double)texture2D.Width != (double)textureSize.x || (double)texture2D.Height != (double)textureSize.y))
                                    tex2D = (Tex2D)new BigBoyTex2D(texture2D, name, DuckGame.Content._currentTextureIndex)
                                    {
                                        scaleFactor = (textureSize.x / (float)texture2D.Width)
                                    };
                                else
                                    tex2D = new Tex2D(texture2D, name, DuckGame.Content._currentTextureIndex);
                                ++DuckGame.Content._currentTextureIndex;
                                DuckGame.Content._textureList.Add(tex2D);
                                DuckGame.Content._textures[name] = tex2D;
                                DuckGame.Content._texture2DMap[(object)texture2D] = tex2D;
                                return (T)(object)tex2D;
                            }
                        }
                    }
                    else
                    {
                        T obj = ReskinPack.LoadAsset<T>(name);
                        if ((object)obj != null)
                            return obj;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (typeof(T) == typeof(Tex2D))
            {
                Tex2D tex2D = (Tex2D)null;
                lock (DuckGame.Content._textures)
                    DuckGame.Content._textures.TryGetValue(name, out tex2D);
                if (tex2D == null)
                {
                    Texture2D texture2D = (Texture2D)null;
                    bool flag = false;
                    if (MonoMain.moddingEnabled && ModLoader.accessibleMods.Count<Mod>() > 1 && name.Length > 1 && name[1] == ':')
                        flag = true;
                    if (!flag)
                    {
                        try
                        {
                            texture2D = DuckGame.Content._base.Load<Texture2D>(name);
                        }
                        catch (Exception ex)
                        {
                            flag = MonoMain.moddingEnabled && ModLoader.modsEnabled;
                            DuckGame.Content.lastException = ex;
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
                            DuckGame.Content.lastException = ex;
                        }
                    }
                    if (texture2D == null)
                    {
                        texture2D = (Texture2D)DuckGame.Content.invalidTexture;
                        Main.SpecialCode = "Couldn't load texture " + name;
                    }
                    lock (DuckGame.Content._loadLock)
                    {
                        tex2D = new Tex2D(texture2D, name, DuckGame.Content._currentTextureIndex);
                        ++DuckGame.Content._currentTextureIndex;
                        DuckGame.Content._textureList.Add(tex2D);
                        DuckGame.Content._textures[name] = tex2D;
                        DuckGame.Content._texture2DMap[(object)texture2D] = tex2D;
                    }
                }
                return (T)(object)tex2D;
            }
            if (typeof(T) == typeof(MTEffect))
            {
                MTEffect mtEffect = (MTEffect)null;
                lock (DuckGame.Content._effects)
                    DuckGame.Content._effects.TryGetValue(name, out mtEffect);
                if (mtEffect == null)
                {
                    Effect effect = (Effect)null;
                    lock (DuckGame.Content._loadLock)
                        effect = DuckGame.Content._base.Load<Effect>(name);
                    lock (DuckGame.Content._loadLock)
                    {
                        mtEffect = new MTEffect(effect, name, DuckGame.Content._currentEffectIndex);
                        ++DuckGame.Content._currentEffectIndex;
                        DuckGame.Content._effectList.Add(mtEffect);
                        DuckGame.Content._effects[name] = mtEffect;
                        DuckGame.Content._effectMap[effect] = mtEffect;
                    }
                }
                return (T)(object)mtEffect;
            }
            if (typeof(T) == typeof(SoundEffect))
            {
                SoundEffect soundEffect = (SoundEffect)null;
                lock (DuckGame.Content._sounds)
                    DuckGame.Content._sounds.TryGetValue(name, out soundEffect);
                if (soundEffect == null)
                {
                    if (!name.Contains(":") && !name.EndsWith(".wav"))
                    {
                        lock (DuckGame.Content._loadLock)
                        {
                            try
                            {
                                string path = DuckFile.contentDirectory + name + ".wav";
                                soundEffect = SoundEffect.FromStream((Stream)new MemoryStream(System.IO.File.ReadAllBytes(path)));
                                if (soundEffect != null)
                                    soundEffect.file = path;
                            }
                            catch (Exception ex)
                            {
                                DuckGame.Content.lastException = ex;
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
                    DuckGame.Content._sounds[name] = soundEffect;
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
            return typeof(T) == typeof(Microsoft.Xna.Framework.Media.Song) ? (T)(object)DuckGame.Content._base.Load<Microsoft.Xna.Framework.Media.Song>(name) : DuckGame.Content._base.Load<T>(name);
        }
    }
}
