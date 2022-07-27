// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckFile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace DuckGame
{
    public class DuckFile
    {
        private static string _saveRoot;
        private static string _saveDirectory;
        private static string _levelDirectory;
        private static string _editorPreviewDirectory;
        private static string _workshopDirectory;
        private static string _onlineLevelDirectory;
        private static string _albumDirectory;
        private static string _profileDirectory;
        private static string _optionsDirectory;
        private static string _modsDirectory;
        private static string _challengeDirectory;
        private static string _scriptsDirectory;
        private static string _skinsDirectory;
        private static string _mappackDirectory;
        private static string _hatpackDirectory;
        private static string _customBlockDirectory;
        private static string _downloadedBlockDirectory;
        private static string _customBackgroundDirectory;
        private static string _downloadedBackgroundDirectory;
        private static string _customPlatformDirectory;
        private static string _downloadedPlatformDirectory;
        private static string _customParallaxDirectory;
        private static string _downloadedParallaxDirectory;
        private static string _customArcadeDirectory;
        private static string _customMojiDirectory;
        private static string _logDirectory;
        private static string _contentDirectory = "Content/";
        private static string _musicDirectory;
        private static List<string> _allPaths = new List<string>();
        private static Dictionary<char, string> _invalidPathCharConversions = new Dictionary<char, string>()
    {
      {
        '/',
        "!53029662!"
      },
      {
        '\\',
        "!52024921!"
      },
      {
        '?',
        "!54030923!"
      },
      {
        '%',
        "!50395932!"
      },
      {
        '*',
        "!31040256!"
      },
      {
        ':',
        "!40205341!"
      },
      {
        '|',
        "!95302943!"
      },
      {
        '"',
        "!41302950!"
      },
      {
        '<',
        "!21493928!"
      },
      {
        '>',
        "!95828381!"
      },
      {
        '.',
        "!34910294!"
      }
    };
        public static bool freshInstall;
        public static bool appdataSave = false;
        public static bool _flaggedForBackup = false;
        public static bool mojimode = true;
        private static Dictionary<string, Sprite> _mojis = new Dictionary<string, Sprite>();
        private static Dictionary<string, Dictionary<string, Sprite>> _profileMojis = new Dictionary<string, Dictionary<string, Sprite>>();
        private static int waitTry = 0;
        private static bool _suppressCommit = false;
        private static Dictionary<string, LevelData> _levelCache = new Dictionary<string, LevelData>();
        private static Dictionary<uint, string> _conversionGUIDMap = new Dictionary<uint, string>();
        public static volatile bool LegacyLoadLock = false;

        public static string saveRoot => DuckFile._saveRoot;

        public static string saveDirectory => DuckFile._saveRoot + DuckFile._saveDirectory;

        public static string levelDirectory => DuckFile.saveDirectory + DuckFile._levelDirectory;

        public static string cloudLevelDirectory => DuckFile.userDirectory + DuckFile._levelDirectory;

        public static string editorPreviewDirectory => DuckFile.saveDirectory + DuckFile._editorPreviewDirectory;

        public static string workshopDirectory => DuckFile.saveDirectory + DuckFile._workshopDirectory;

        public static string onlineLevelDirectory => DuckFile.saveDirectory + DuckFile._onlineLevelDirectory;

        public static string albumDirectory => DuckFile.saveDirectory + DuckFile._albumDirectory;

        public static string profileDirectory => DuckFile.userDirectory + DuckFile._profileDirectory;

        public static string optionsDirectory
        {
            get
            {
                string pPath = DuckFile.userDirectory + DuckFile._optionsDirectory;
                return !DuckFile.DirectoryExists(pPath) && !MonoMain.atPostCloudLogic ? DuckFile.globalOptionsDirectory : pPath;
            }
        }

        public static string modsDirectory => DuckFile.userDirectory + DuckFile._modsDirectory;

        public static string globalProfileDirectory => DuckFile.saveDirectory + DuckFile._profileDirectory;

        public static string globalOptionsDirectory => DuckFile.saveDirectory + DuckFile._optionsDirectory;

        public static string globalModsDirectory => DuckFile.saveDirectory + DuckFile._modsDirectory;

        public static string globalSkinsDirectory => DuckFile.globalModsDirectory + DuckFile._skinsDirectory;

        public static string globalMappackDirectory => DuckFile.globalModsDirectory + DuckFile._mappackDirectory;

        public static string globalHatpackDirectory => DuckFile.globalModsDirectory + DuckFile._hatpackDirectory;

        public static string challengeDirectory => DuckFile.saveDirectory + DuckFile._challengeDirectory;

        public static string scriptsDirectory => DuckFile.saveDirectory + DuckFile._scriptsDirectory;

        public static string skinsDirectory => DuckFile.modsDirectory + DuckFile._skinsDirectory;

        public static string mappackDirectory => DuckFile.modsDirectory + DuckFile._mappackDirectory;

        public static string hatpackDirectory => DuckFile.modsDirectory + DuckFile._hatpackDirectory;

        public static string userDirectory => Steam.user != null ? DuckFile.saveDirectory + Steam.user.id.ToString() + "/" : DuckFile.saveDirectory;

        public static string customBlockDirectory => DuckFile.saveDirectory + DuckFile._customBlockDirectory;

        public static string downloadedBlockDirectory => DuckFile.saveDirectory + DuckFile._downloadedBlockDirectory;

        public static string customBackgroundDirectory => DuckFile.saveDirectory + DuckFile._customBackgroundDirectory;

        public static string downloadedBackgroundDirectory => DuckFile.saveDirectory + DuckFile._downloadedBackgroundDirectory;

        public static string customPlatformDirectory => DuckFile.saveDirectory + DuckFile._customPlatformDirectory;

        public static string downloadedPlatformDirectory => DuckFile.saveDirectory + DuckFile._downloadedPlatformDirectory;

        public static string customParallaxDirectory => DuckFile.saveDirectory + DuckFile._customParallaxDirectory;

        public static string downloadedParallaxDirectory => DuckFile.saveDirectory + DuckFile._downloadedParallaxDirectory;

        public static string customArcadeDirectory => DuckFile.saveDirectory + DuckFile._customArcadeDirectory;

        public static string customMojiDirectory => DuckFile.saveDirectory + DuckFile._customMojiDirectory;

        public static string logDirectory => DuckFile.saveDirectory + DuckFile._logDirectory;

        public static string contentDirectory => DuckFile._contentDirectory;

        public static string musicDirectory => DuckFile.contentDirectory + DuckFile._musicDirectory;

        public static string GetCustomDownloadDirectory(CustomType t)
        {
            switch (t)
            {
                case CustomType.Block:
                    return DuckFile.downloadedBlockDirectory;
                case CustomType.Platform:
                    return DuckFile.downloadedPlatformDirectory;
                case CustomType.Background:
                    return DuckFile.downloadedBackgroundDirectory;
                case CustomType.Parallax:
                    return DuckFile.downloadedParallaxDirectory;
                default:
                    return "";
            }
        }

        public static string GetCustomDirectory(CustomType t)
        {
            switch (t)
            {
                case CustomType.Block:
                    return DuckFile.customBlockDirectory;
                case CustomType.Platform:
                    return DuckFile.customPlatformDirectory;
                case CustomType.Background:
                    return DuckFile.customBackgroundDirectory;
                case CustomType.Parallax:
                    return DuckFile.customParallaxDirectory;
                default:
                    return "";
            }
        }

        public static string oldSaveLocation => (Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/").Replace('\\', '/');

        public static string newSaveLocation => (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/").Replace('\\', '/');

        public static void Initialize()
        {
            DuckFile._saveDirectory = "DuckGame/";
            bool flag = true;
            string oldSaveLocation = DuckFile.oldSaveLocation;
            DuckFile._saveRoot = DuckFile.newSaveLocation;
            if (!DuckFile.DirectoryExists(DuckFile._saveRoot + DuckFile._saveDirectory) && !Program.alternateSaveLocation && DuckFile.DirectoryExists(oldSaveLocation + DuckFile._saveDirectory))
            {
                DuckFile._saveRoot = oldSaveLocation;
                flag = false;
                DuckFile.appdataSave = false;
            }
            if (flag)
            {
                DuckFile.appdataSave = true;
                try
                {
                    string str1 = oldSaveLocation + DuckFile._saveDirectory;
                    if (Program.alternateSaveLocation && DuckFile.DirectoryExists(str1) && !DuckFile.DirectoryExists(DuckFile.saveDirectory))
                        DuckFile.DirectoryCopy(str1, DuckFile.saveDirectory, true);
                    string path = str1 + "where_is_my_save.txt";
                    if (!System.IO.File.Exists(path))
                    {
                        DuckFile.CreatePath(str1);
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        using (StreamWriter streamWriter = new StreamWriter(str1 + "Save Data.url"))
                        {
                            string location = Assembly.GetExecutingAssembly().Location;
                            streamWriter.WriteLine("[InternetShortcut]");
                            streamWriter.WriteLine("URL=file:///" + DuckFile.saveDirectory);
                            streamWriter.WriteLine("IconIndex=0");
                            string str2 = location.Replace('\\', '/');
                            streamWriter.WriteLine("IconFile=" + str2);
                        }
                        System.IO.File.WriteAllText(path, "Hey! Keeping save data in the Documents folder was causing all kinds\nof issues for people, and it's with great sadness that I had to move your data.\nDon't worry, it still exists- your data is now located here:\n\n" + DuckFile.saveDirectory + "\n\nAny save data still located in this folder is for the old version (pre-2020) of Duck Game.");
                    }
                }
                catch (Exception)
                {
                }
            }
            DevConsole.Log(DCSection.General, "DuckFile.Initialize().. " + (DuckFile._saveRoot.Contains("OneDrive/") ? "Ah, a |DGBLUE|OneDrive|WHITE| user, I see.." : ""));
            if (!DuckFile.DirectoryExists(DuckFile.saveDirectory))
                DuckFile.freshInstall = true;
            DuckFile._levelDirectory = "Levels/";
            DuckFile._allPaths.Add(DuckFile._levelDirectory);
            DuckFile._editorPreviewDirectory = "EditorPreviews/";
            DuckFile._allPaths.Add(DuckFile._editorPreviewDirectory);
            DuckFile._onlineLevelDirectory = "Online/Levels/";
            DuckFile._allPaths.Add(DuckFile._onlineLevelDirectory);
            DuckFile._optionsDirectory = "Options/";
            DuckFile._allPaths.Add(DuckFile._optionsDirectory);
            DuckFile._albumDirectory = "Album/";
            DuckFile._allPaths.Add(DuckFile._albumDirectory);
            DuckFile._profileDirectory = "Profiles/";
            DuckFile._allPaths.Add(DuckFile._profileDirectory);
            DuckFile._challengeDirectory = "ChallengeData/";
            DuckFile._allPaths.Add(DuckFile._challengeDirectory);
            DuckFile._modsDirectory = "Mods/";
            DuckFile._allPaths.Add(DuckFile._modsDirectory);
            DuckFile._skinsDirectory = "Texpacks/";
            DuckFile._allPaths.Add(DuckFile._skinsDirectory);
            DuckFile._mappackDirectory = "Mappacks/";
            DuckFile._allPaths.Add(DuckFile._mappackDirectory);
            DuckFile._hatpackDirectory = "Hatpacks/";
            DuckFile._allPaths.Add(DuckFile._hatpackDirectory);
            DuckFile._scriptsDirectory = "Scripts/";
            DuckFile._allPaths.Add(DuckFile._scriptsDirectory);
            DuckFile._workshopDirectory = "Workshop/";
            DuckFile._allPaths.Add(DuckFile._workshopDirectory);
            DuckFile._customBlockDirectory = "Custom/Blocks/";
            DuckFile._allPaths.Add(DuckFile._customBlockDirectory);
            DuckFile.CreatePath(DuckFile.customBlockDirectory);
            DuckFile._downloadedBlockDirectory = "Custom/Blocks/Downloaded/";
            DuckFile._allPaths.Add(DuckFile._downloadedBlockDirectory);
            DuckFile._customBackgroundDirectory = "Custom/Background/";
            DuckFile._allPaths.Add(DuckFile._customBackgroundDirectory);
            DuckFile.CreatePath(DuckFile.customBackgroundDirectory);
            DuckFile._downloadedBackgroundDirectory = "Custom/Background/Downloaded/";
            DuckFile._allPaths.Add(DuckFile._downloadedBackgroundDirectory);
            DuckFile._customPlatformDirectory = "Custom/Platform/";
            DuckFile._allPaths.Add(DuckFile._customPlatformDirectory);
            DuckFile.CreatePath(DuckFile.customPlatformDirectory);
            DuckFile._downloadedPlatformDirectory = "Custom/Platform/Downloaded/";
            DuckFile._allPaths.Add(DuckFile._downloadedPlatformDirectory);
            DuckFile._customParallaxDirectory = "Custom/Parallax/";
            DuckFile._allPaths.Add(DuckFile._customParallaxDirectory);
            DuckFile.CreatePath(DuckFile.customParallaxDirectory);
            DuckFile._downloadedParallaxDirectory = "Custom/Parallax/Downloaded/";
            DuckFile._allPaths.Add(DuckFile._downloadedParallaxDirectory);
            DuckFile._customArcadeDirectory = "Custom/Arcade/";
            DuckFile._allPaths.Add(DuckFile.customArcadeDirectory);
            DuckFile.CreatePath(DuckFile.customArcadeDirectory);
            try
            {
                DuckFile._customMojiDirectory = "Custom/Moji/";
                DuckFile._allPaths.Add(DuckFile.customMojiDirectory);
                DuckFile.CreatePath(DuckFile.customMojiDirectory);
                DuckFile.CreatePath(DuckFile.saveDirectory + "Custom/Hats/");
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Could not create moji path, disabling custom mojis :(");
                DuckFile.mojimode = false;
            }
            DuckFile._logDirectory = "Logs/";
            DuckFile._allPaths.Add(DuckFile.logDirectory);
            DuckFile._musicDirectory = "Audio/Music/";
        }

        public static void FlagForBackup()
        {
            if (!MonoMain.started)
                return;
            DuckFile._flaggedForBackup = true;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] directoryInfoArray = directoryInfo1.Exists ? directoryInfo1.GetDirectories() : throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            Directory.CreateDirectory(destDirName);
            foreach (FileInfo file in directoryInfo1.GetFiles())
            {
                string destFileName = Path.Combine(destDirName, file.Name);
                file.CopyTo(destFileName, false);
            }
            if (!copySubDirs)
                return;
            foreach (DirectoryInfo directoryInfo2 in directoryInfoArray)
            {
                string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
                DuckFile.DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
            }
        }

        public static Dictionary<string, Sprite> mojis => DuckFile._mojis;

        public static Sprite GetMoji(string moji, NetworkConnection pConnection = null)
        {
            if (!DuckFile.mojimode)
                return (Sprite)null;
            if (Options.Data.mojiFilter == 1 && pConnection != null && pConnection.data is User && (pConnection.data as User).relationship != FriendRelationship.Friend)
                return (Sprite)null;
            Sprite s1 = (Sprite)null;
            if (pConnection != null)
            {
                Dictionary<string, Sprite> dictionary = (Dictionary<string, Sprite>)null;
                if (DuckFile._profileMojis.TryGetValue(pConnection.identifier, out dictionary))
                    dictionary.TryGetValue(moji, out s1);
            }
            else
                DuckFile._mojis.TryGetValue(moji, out s1);
            if ((pConnection == DuckNetwork.localConnection || pConnection == null) && Network.isActive)
            {
                if (s1 == null)
                {
                    try
                    {
                        foreach (NetworkConnection connection in Network.connections)
                        {
                            if (connection != DuckNetwork.localConnection)
                            {
                                s1 = DuckFile.GetMoji(moji, connection);
                                if (s1 != null && pConnection == DuckNetwork.localConnection)
                                {
                                    DuckFile.SaveString(Editor.TextureToString((Texture2D)s1.texture), DuckFile.customMojiDirectory + moji + ".moj");
                                    DuckFile.RegisterMoji(moji, s1);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (s1 == null && DuckFile.waitTry <= 0)
            {
                DuckFile.waitTry = 60;
                try
                {
                    string fileName = DuckFile.customMojiDirectory + moji + ".png";
                    if (!System.IO.File.Exists(fileName))
                        fileName = DuckFile.customMojiDirectory + moji + ".jpg";
                    if (!System.IO.File.Exists(fileName))
                        fileName = DuckFile.customMojiDirectory + moji + ".jpeg";
                    if (!System.IO.File.Exists(fileName))
                        fileName = DuckFile.customMojiDirectory + moji + ".bmp";
                    if (System.IO.File.Exists(fileName))
                    {
                        Texture2D t = TextureConverter.LoadPNGWithPinkAwesomenessAndMaxDimensions(DuckGame.Graphics.device, fileName, true, new Vec2(28f, 28f));
                        if (t != null)
                        {
                            if (t.Width <= 28 && t.Height <= 28)
                            {
                                Sprite s2 = new Sprite((Tex2D)t);
                                DuckFile.RegisterMoji(moji, s2);
                                if (TextureConverter.lastLoadResultedInResize)
                                {
                                    try
                                    {
                                        DuckFile.TryFileOperation((Action)(() =>
                                       {
                                           string str = fileName;
                                           DuckFile.Delete(str);
                                           if (str.EndsWith(".jpg"))
                                               str = str.Replace(".jpg", ".png");
                                           if (str.EndsWith(".bmp"))
                                               str = str.Replace(".bmp", ".png");
                                           if (str.EndsWith(".jpeg"))
                                               str = str.Replace(".jpeg", ".png");
                                           t.SaveAsPng((Stream)System.IO.File.Create(str), t.Width, t.Height);
                                       }), "InitializeMojis.Resize");
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            else
                                DevConsole.Log("Error loading " + fileName + " MOJI (must be smaller than 28x28)", Color.Red);
                        }
                    }
                    else
                    {
                        fileName = DuckFile.customMojiDirectory + moji + ".moj";
                        if (System.IO.File.Exists(fileName))
                        {
                            Texture2D texture = Editor.StringToTexture(DuckFile.ReadAllText(fileName));
                            if (texture != null)
                            {
                                if (texture.Width <= 28 && texture.Height <= 28)
                                {
                                    Sprite s3 = new Sprite((Tex2D)texture);
                                    DuckFile.RegisterMoji(moji, s3);
                                }
                                else
                                    DevConsole.Log("Error loading " + fileName + " MOJI (must be smaller than 28x28)", Color.Red);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            if (DuckFile.waitTry > 0)
                --DuckFile.waitTry;
            return s1;
        }

        public static void StealMoji(string moji)
        {
            foreach (NetworkConnection connection in Network.connections)
            {
                if (connection != DuckNetwork.localConnection)
                {
                    Sprite moji1 = DuckFile.GetMoji(moji, connection);
                    if (moji1 != null)
                    {
                        DuckFile.SaveString(Editor.TextureToString((Texture2D)moji1.texture), DuckFile.customMojiDirectory + moji + ".moj");
                        DuckFile.RegisterMoji(moji, moji1);
                    }
                }
            }
        }

        public static void RegisterMoji(string moji, Sprite s, NetworkConnection pConnection = null)
        {
            if (!DuckFile.mojimode)
                return;
            if (s.width <= 16)
                s.scale = new Vec2(2f, 2f);
            else
                s.scale = new Vec2(1f, 1f);
            if (s.width > 28 || s.height > 28)
                return;
            s.moji = true;
            if (pConnection != null)
            {
                Dictionary<string, Sprite> dictionary = (Dictionary<string, Sprite>)null;
                if (!DuckFile._profileMojis.TryGetValue(pConnection.identifier, out dictionary))
                    dictionary = DuckFile._profileMojis[pConnection.identifier] = new Dictionary<string, Sprite>();
                dictionary[moji] = s;
            }
            else
                DuckFile._mojis[moji] = s;
        }

        public static void InitializeMojis()
        {
            if (!DuckFile.mojimode)
                return;
            List<string> list = ((IEnumerable<string>)DuckFile.GetFiles(DuckFile.customMojiDirectory, "*.png")).ToList<string>();
            list.AddRange((IEnumerable<string>)DuckFile.GetFiles(DuckFile.customMojiDirectory, "*.jpg"));
            list.AddRange((IEnumerable<string>)DuckFile.GetFiles(DuckFile.customMojiDirectory, "*.jpeg"));
            list.AddRange((IEnumerable<string>)DuckFile.GetFiles(DuckFile.customMojiDirectory, "*.bmp"));
            foreach (string str1 in list)
            {
                string s = str1;
                try
                {
                    Texture2D t = TextureConverter.LoadPNGWithPinkAwesomenessAndMaxDimensions(DuckGame.Graphics.device, s, true, new Vec2(28f, 28f));
                    if (t != null)
                    {
                        if (t.Width <= 28 && t.Height <= 28)
                        {
                            Sprite s1 = new Sprite((Tex2D)t);
                            DuckFile.RegisterMoji(Path.GetFileNameWithoutExtension(s), s1);
                            if (TextureConverter.lastLoadResultedInResize)
                            {
                                try
                                {
                                    DuckFile.TryFileOperation((Action)(() =>
                                   {
                                       string str2 = s;
                                       DuckFile.Delete(str2);
                                       if (str2.EndsWith(".jpg"))
                                           str2 = str2.Replace(".jpg", ".png");
                                       if (str2.EndsWith(".bmp"))
                                           str2 = str2.Replace(".bmp", ".png");
                                       if (str2.EndsWith(".jpeg"))
                                           str2 = str2.Replace(".jpeg", ".png");
                                       t.SaveAsPng((Stream)System.IO.File.Create(str2), t.Width, t.Height);
                                   }), "InitializeMojis.Resize");
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        else
                            DevConsole.Log("Error loading " + Path.GetFileName(s) + " MOJI (must be smaller than 28x28)", Color.Red);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            foreach (string str in ((IEnumerable<string>)DuckFile.GetFiles(DuckFile.customMojiDirectory, "*.moj")).ToList<string>())
            {
                try
                {
                    Texture2D texture = Editor.StringToTexture(DuckFile.ReadAllText(str));
                    if (texture != null)
                    {
                        if (texture.Width <= 28 && texture.Height <= 28)
                        {
                            Sprite s = new Sprite((Tex2D)texture);
                            DuckFile.RegisterMoji(Path.GetFileNameWithoutExtension(str), s);
                        }
                        else
                            DevConsole.Log("Error loading " + Path.GetFileName(str) + " MOJI (must be smaller than 28x28)", Color.Red);
                    }
                }
                catch (Exception)
                {
                }
            }
            DuckFile._mojis = DuckFile._mojis.OrderByDescending<KeyValuePair<string, Sprite>, string>((Func<KeyValuePair<string, Sprite>, string>)(x => x.Key)).ToDictionary<KeyValuePair<string, Sprite>, string, Sprite>((Func<KeyValuePair<string, Sprite>, string>)(pair => pair.Key), (Func<KeyValuePair<string, Sprite>, Sprite>)(pair => pair.Value));
        }

        public static void BeginDataCommit() => DuckFile._suppressCommit = true;

        public static void EndDataCommit() => DuckFile._suppressCommit = false;

        private static void Commit(string pPath, bool pDelete = false)
        {
            int num = DuckFile._suppressCommit ? 1 : 0;
            if (pPath == null)
                return;
            if (pDelete)
                Cloud.Delete(pPath);
            else
                Cloud.Write(pPath);
        }

        public static void DeleteAllSaveData() => Directory.Delete(DuckFile.userDirectory, true);

        public static void CreatePath(string pathString) => DuckFile.CreatePath(pathString, false);

        public static void CreatePath(string pathString, bool ignoreLast)
        {
            pathString = pathString.Replace('\\', '/');
            string[] source = pathString.Split('/');
            string str = "";
            for (int index = 0; index < ((IEnumerable<string>)source).Count<string>(); ++index)
            {
                if (!(source[index] == "") && !(source[index] == "/") && (!(source[index].Contains<char>('.') | ignoreLast) || index != ((IEnumerable<string>)source).Count<string>() - 1))
                {
                    string path = str + source[index];
                    if (!Directory.Exists(path))
                    {
                        if (MonoMain.logFileOperations)
                            DevConsole.Log(DCSection.General, "DuckFile.CreatePath(" + path + ")");
                        Directory.CreateDirectory(path);
                        DuckFile.Commit((string)null);
                    }
                    str = path + "/";
                }
            }
        }

        public static FileStream Create(string path)
        {
            DuckFile.CreatePath(path);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.Create(" + path + ")");
            return System.IO.File.Create(path);
        }

        public static string ReadAllText(string pPath)
        {
            pPath = DuckFile.PreparePath(pPath);
            DuckFile.TryClearAttributes(pPath);
            string text = "";
            DuckFile.TryFileOperation((Action)(() => text = System.IO.File.ReadAllText(pPath)), "ReadAllText(" + pPath + ")");
            return text;
        }

        public static string[] ReadAllLines(string path)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.ReadAllLines(" + path + ")");
            return System.IO.File.ReadAllLines(path);
        }

        public static string GetShortPath(string path)
        {
            path.Replace('\\', '/');
            string saveDirectory = DuckFile.saveDirectory;
            int num = path.IndexOf(saveDirectory);
            return num != -1 ? path.Substring(num + saveDirectory.Length, path.Length - num - saveDirectory.Length) : path;
        }

        public static string FixInvalidPath(string pPath, bool pRemoveDirectoryCharacters = false)
        {
            foreach (char ch in new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))
            {
                if (pRemoveDirectoryCharacters || ch != '\\' && ch != '/' && ch != ':')
                    pPath = pPath.Replace(ch.ToString(), "");
            }
            return pPath;
        }

        public static bool IsUserPath(string path)
        {
            try
            {
                return Steam.user != null && Path.GetDirectoryName(path).Contains(Steam.user.id.ToString());
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetLocalSavePath(string path)
        {
            path.Replace('\\', '/');
            if (DuckFile.IsUserPath(path))
            {
                string userDirectory = DuckFile.userDirectory;
                int num = path.IndexOf(userDirectory);
                if (num != -1)
                    return path.Substring(num + userDirectory.Length, path.Length - num - userDirectory.Length);
            }
            else
            {
                string saveDirectory = DuckFile.saveDirectory;
                int num = path.IndexOf(saveDirectory);
                if (num != -1)
                    return path.Substring(num + saveDirectory.Length, path.Length - num - saveDirectory.Length);
            }
            return (string)null;
        }

        public static string GetShortDirectory(string path)
        {
            path.Replace('\\', '/');
            string saveRoot = DuckFile.saveRoot;
            int num = path.IndexOf(saveRoot);
            return num != -1 ? path.Substring(num + saveRoot.Length, path.Length - num - saveRoot.Length) : path;
        }

        public static Stream OpenStream(string path)
        {
            if (MonoMain.logLevelOperations)
                DevConsole.Log(DCSection.General, "DuckFile.OpenStream(" + path + ")");
            return TitleContainer.OpenStream(path);
        }

        public static List<string> GetFilesNoCloud(string path, string filter = "*.*", SearchOption so = SearchOption.TopDirectoryOnly)
        {
            List<string> filesNoCloud1 = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(path, filter, SearchOption.TopDirectoryOnly))
                    filesNoCloud1.Add(file);
            }
            catch (Exception)
            {
            }
            if (so == SearchOption.AllDirectories)
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        List<string> filesNoCloud2 = DuckFile.GetFilesNoCloud(directory, filter, so);
                        filesNoCloud1.AddRange((IEnumerable<string>)filesNoCloud2);
                    }
                }
                catch (Exception)
                {
                }
            }
            return filesNoCloud1;
        }

        public static List<string> GetAllSavegameFiles(
          List<string> pFolderFilters,
          List<string> pRet = null,
          string pSubFolder = null,
          bool pRecurseFiltered = false,
          bool pDontAddFilteredFolders = false)
        {
            bool pRecurseFiltered1 = pRecurseFiltered;
            if (!pRecurseFiltered1 && pFolderFilters != null && pSubFolder != null)
            {
                string str = pSubFolder.Replace(DuckFile.userDirectory, "").Replace('\\', '/');
                foreach (string pFolderFilter in pFolderFilters)
                {
                    if (str.StartsWith(pFolderFilter))
                    {
                        if (pDontAddFilteredFolders)
                            return pRet;
                        pRecurseFiltered1 = true;
                    }
                }
            }
            string path = pSubFolder != null ? pSubFolder : DuckFile.userDirectory;
            List<string> pRet1 = pRet != null ? pRet : new List<string>();
            try
            {
                foreach (string file2 in Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly))
                {
                    string file = file2;
                    if (pRecurseFiltered1)
                        file += "?";
                    pRet1.Add(file);
                }
            }
            catch (Exception)
            {
            }
            foreach (string directory in Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    DuckFile.GetAllSavegameFiles(pFolderFilters, pRet1, directory, pRecurseFiltered1, pDontAddFilteredFolders);
                }
                catch (Exception)
                {
                }
            }
            return pRet1;
        }

        public static List<string> GetDirectoriesNoCloud(string path, string filter = "*.*")
        {
            List<string> directoriesNoCloud = new List<string>();
            try
            {
                foreach (string directory in Directory.GetDirectories(path, filter, SearchOption.TopDirectoryOnly))
                    directoriesNoCloud.Add(directory);
            }
            catch (Exception ex)
            {
            }
            return directoriesNoCloud;
        }

        public static string[] GetFiles(string path, string filter) => DuckFile.GetFiles(path, filter, SearchOption.TopDirectoryOnly);

        public static string[] GetFiles(string path, string filter = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            List<string> stringList = new List<string>();
            if (Directory.Exists(path))
            {
                stringList = DuckFile.GetFilesNoCloud(path, filter, option);
                for (int index = 0; index < stringList.Count; ++index)
                    stringList[index] = stringList[index].Replace('\\', '/');
            }
            return stringList.ToArray();
        }

        public static string[] GetDirectories(string path)
        {
            path = path.Replace('\\', '/');
            List<string> stringList = new List<string>();
            path = path.Trim('/');
            if (Directory.Exists(path))
            {
                foreach (string path1 in DuckFile.GetDirectoriesNoCloud(path))
                {
                    if (!Path.GetFileName(path1).Contains("._"))
                    {
                        string str = path1.Replace('\\', '/');
                        if (!stringList.Contains(str))
                            stringList.Add(str);
                    }
                }
            }
            return stringList.ToArray();
        }

        public static string ReplaceInvalidCharacters(string path)
        {
            string str1 = "";
            foreach (char key in path)
            {
                string str2 = "";
                str1 = !DuckFile._invalidPathCharConversions.TryGetValue(key, out str2) ? str1 + key.ToString() : str1 + str2;
            }
            return str1;
        }

        public static string RestoreInvalidCharacters(string path)
        {
            foreach (KeyValuePair<char, string> pathCharConversion in DuckFile._invalidPathCharConversions)
                path = path.Replace(pathCharConversion.Value, pathCharConversion.Key.ToString() ?? "");
            return path;
        }

        public static LevelData LoadLevelHeaderCached(string path)
        {
            LevelData levelData = (LevelData)null;
            if (!DuckFile._levelCache.TryGetValue(path, out levelData))
                levelData = DuckFile._levelCache[path] = DuckFile.LoadLevel(path, true);
            return levelData;
        }

        public static LevelData LoadLevel(string path) => DuckFile.LoadLevel(path, false);

        public static bool FileExists(string pPath) => System.IO.File.Exists(pPath);

        public static bool DirectoryExists(string pPath) => Directory.Exists(pPath);

        public static LevelData LoadLevel(string path, bool pHeaderOnly)
        {
            Cloud.ReplaceLocalFileWithCloudFile(path);
            if (!System.IO.File.Exists(path))
                return (LevelData)null;
            if (MonoMain.logLevelOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadLevel(" + path + ")");
            LevelData levelData = DuckFile.LoadLevel(System.IO.File.ReadAllBytes(path), pHeaderOnly);
            levelData?.SetPath(path);
            return levelData;
        }

        private static LevelData ConvertLevel(byte[] data)
        {
            if (MonoMain.logLevelOperations)
                DevConsole.Log(DCSection.General, "DuckFile.ConvertLevel()");
            LevelData levelData = (LevelData)null;
            Editor editor = new Editor();
            bool skipInitialize = Level.skipInitialize;
            Level.skipInitialize = true;
            Level currentLevel = Level.core.currentLevel;
            Level.core.currentLevel = (Level)editor;
            try
            {
                editor.minimalConversionLoad = true;
                DuckXML doc = DuckXML.Load(data);
                editor.LegacyLoadLevelParts(doc);
                editor.things.RefreshState();
                levelData = editor.CreateSaveData();
                if (!editor.hadGUID)
                {
                    uint key = Editor.Checksum(data);
                    string str = (string)null;
                    if (!DuckFile._conversionGUIDMap.TryGetValue(key, out str))
                    {
                        str = levelData.metaData.guid;
                        DuckFile._conversionGUIDMap[key] = str;
                    }
                    levelData.metaData.guid = str;
                }
            }
            catch
            {
            }
            Level.core.currentLevel = currentLevel;
            Level.skipInitialize = skipInitialize;
            return levelData;
        }

        public static LevelData LoadLevel(byte[] data) => DuckFile.LoadLevel(data, false);

        public static LevelData LoadLevel(byte[] data, bool pHeaderOnly)
        {
            LevelData levelData = BinaryClassChunk.FromData<LevelData>(new BitBuffer(data, false), pHeaderOnly);
            if (!pHeaderOnly && levelData == null || levelData != null && levelData.GetResult() == DeserializeResult.InvalidMagicNumber)
            {
                Promise<LevelData> promise = Tasker.Task<LevelData>((Func<LevelData>)(() => DuckFile.ConvertLevel(data)));
                promise.WaitForComplete();
                return promise.Result;
            }
            if (levelData != null && levelData.GetExtraHeaderInfo() is LevelMetaData)
            {
                levelData.RerouteMetadata(levelData.GetExtraHeaderInfo() as LevelMetaData);
            }
            else
            {
                LevelMetaData metaData = levelData.metaData;
            }
            return levelData;
        }

        public static void WriteAllTextSafe(string path, string contents)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.WriteAllTextSafe(" + path + ")");
            string tempFileName = Path.GetTempFileName();
            byte[] bytes = Encoding.UTF8.GetBytes(contents);
            using (FileStream fileStream = System.IO.File.Create(tempFileName, 4096, FileOptions.WriteThrough))
                fileStream.Write(bytes, 0, bytes.Length);
            System.IO.File.Replace(tempFileName, path, (string)null);
        }

        public static void WriteAllText(string pPath, string pContents)
        {
            pPath = DuckFile.PreparePath(pPath, true);
            DuckFile.TryClearAttributes(pPath);
            DuckFile.TryFileOperation((Action)(() => System.IO.File.WriteAllText(pPath, pContents)), "WriteAllText(" + pPath + ")");
            DuckFile.Commit(pPath);
        }

        public static DuckXML LoadDuckXML(string path)
        {
            Cloud.ReplaceLocalFileWithCloudFile(path);
            if (!System.IO.File.Exists(path))
                return (DuckXML)null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadDuckXML(" + path + ")");
            DuckXML duckXml = (DuckXML)null;
            try
            {
                duckXml = DuckXML.Load(path);
            }
            catch
            {
            }
            return duckXml;
        }

        public static void SaveDuckXML(DuckXML doc, string path)
        {
            path = DuckFile.PreparePath(path, true);
            string docString = doc.ToString();
            if (string.IsNullOrWhiteSpace(docString))
                throw new Exception("Blank XML (" + path + ")");
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveDuckXML(" + path + ")");
            DuckFile.TryClearAttributes(path);
            DuckFile.TryFileOperation((Action)(() => System.IO.File.WriteAllText(path, docString)), "SaveDuckXML(" + path + ")");
            DuckFile.Commit(path);
        }

        public static void TryClearAttributes(string pFilename)
        {
            try
            {
                if (!System.IO.File.Exists(pFilename))
                    return;
                System.IO.File.SetAttributes(pFilename, FileAttributes.Normal);
            }
            catch (Exception ex)
            {
            }
        }

        public static bool TryFileOperation(Action pAction, string pActionName)
        {
            for (int index = 0; index < 3; ++index)
            {
                try
                {
                    pAction();
                    return true;
                }
                catch (Exception ex)
                {
                    if (index == 2)
                        throw ex;
                    DevConsole.Log(DCSection.General, ex.Message);
                    DevConsole.Log(DCSection.General, "Exception running " + pActionName + ", retrying...");
                    Thread.Sleep(50);
                }
            }
            return false;
        }

        public static XDocument LoadXDocument(string path)
        {
            DuckFile.CreatePath(Path.GetDirectoryName(path));
            if (!System.IO.File.Exists(path))
                return (XDocument)null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadXDocument(" + path + ")");
            try
            {
                return XDocument.Load(path);
            }
            catch
            {
                return (XDocument)null;
            }
        }

        public static void SaveXDocument(XDocument doc, string path)
        {
            path = DuckFile.PreparePath(path, true);
            string docString = doc.ToString();
            if (string.IsNullOrWhiteSpace(docString))
                throw new Exception("Blank XML (" + path + ")");
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveXDocument(" + path + ")");
            DuckFile.TryClearAttributes(path);
            DuckFile.TryFileOperation((Action)(() => System.IO.File.WriteAllText(path, docString)), "SaveXDocument(" + path + ")");
            DuckFile.Commit(path);
        }

        public static string LoadString(string pPath)
        {
            DuckFile.CreatePath(Path.GetDirectoryName(pPath));
            if (!System.IO.File.Exists(pPath))
                return (string)null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadString(" + pPath + ")");
            try
            {
                return System.IO.File.ReadAllText(pPath);
            }
            catch
            {
                return (string)null;
            }
        }

        public static void SaveString(string pString, string pPath)
        {
            pPath = DuckFile.PreparePath(pPath, true);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveString(" + pPath + ")");
            DuckFile.TryFileOperation((Action)(() =>
           {
               DuckFile.TryClearAttributes(pPath);
               System.IO.File.WriteAllText(pPath, pString);
           }), "SaveString(" + pPath + ")");
            DuckFile.Commit(pPath);
        }

        public static string PreparePath(string pPath, bool pCreatePath = false)
        {
            pPath = pPath.Replace("//", "/");
            pPath = pPath.Replace('\\', '/');
            if (pPath.Length > 1 && pPath[1] == ':')
                pPath = char.ToUpper(pPath[0]).ToString() + pPath.Substring(1, pPath.Length - 1);
            if (pCreatePath)
                DuckFile.CreatePath(Path.GetDirectoryName(pPath));
            try
            {
                if (System.IO.File.Exists(pPath))
                    System.IO.File.SetAttributes(pPath, FileAttributes.Normal);
            }
            catch (Exception)
            {
            }
            return pPath;
        }

        public static XmlDocument LoadSharpXML(string pPath)
        {
            Cloud.ReplaceLocalFileWithCloudFile(pPath);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadSharpXML(" + pPath + ")");
            pPath = DuckFile.PreparePath(pPath);
            if (!System.IO.File.Exists(pPath))
                return (XmlDocument)null;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(pPath);
            return xmlDocument;
        }

        public static void SaveSharpXML(XmlDocument pDoc, string pPath)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveSharpXML(" + pPath + ")");
            DuckFile.TryFileOperation((Action)(() =>
           {
               pPath = DuckFile.PreparePath(pPath, true);
               DuckFile.TryClearAttributes(pPath);
               pDoc.Save(pPath);
           }), "SaveSharpXML(" + pPath + ")");
            DuckFile.Commit(pPath);
        }

        public static T LoadChunk<T>(string path) where T : BinaryClassChunk
        {
            DuckFile.CreatePath(Path.GetDirectoryName(path));
            if (!System.IO.File.Exists(path))
                return default(T);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadChunk(" + path + ")");
            return BinaryClassChunk.FromData<T>(new BitBuffer(System.IO.File.ReadAllBytes(path), 0, false));
        }

        public static bool GetLevelSpacePercentUsed(ref float percent) => false;

        public static void EnsureDownloadFileSpaceAvailable()
        {
        }

        public static bool SaveChunk(BinaryClassChunk doc, string path)
        {
            path = DuckFile.PreparePath(path, true);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveChunk(" + path + ")");
            BitBuffer data = (BitBuffer)null;
            data = doc.Serialize();
            DuckFile.TryFileOperation((Action)(() =>
           {
               FileStream fileStream = System.IO.File.Create(path);
               fileStream.Write(data.buffer, 0, data.lengthInBytes);
               fileStream.Close();
           }), "SaveChunk(" + path + ")");
            DuckFile.Commit(path);
            return true;
        }

        public static void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "DuckFile.DeleteFolder(" + folder + ")");
                foreach (string directory in DuckFile.GetDirectories(folder))
                    DuckFile.DeleteFolder(directory);
                foreach (string file in DuckFile.GetFiles(folder))
                {
                    if (file.EndsWith(".lev"))
                        Editor.Delete(file);
                    else
                        DuckFile.Delete(file);
                }
                Directory.Delete(folder);
            }
            DuckFile.Commit((string)null);
        }

        public static byte[] ReadAllBytes(BinaryReader reader)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    memoryStream.Write(buffer, 0, count);
                return memoryStream.ToArray();
            }
        }

        public static byte[] ReadEntireStream(Stream stream)
        {
            long num1 = 0;
            if (stream.CanSeek)
            {
                num1 = stream.Position;
                stream.Position = 0L;
            }
            try
            {
                byte[] numArray = new byte[4096];
                int length = 0;
                int num2;
                while ((num2 = stream.Read(numArray, length, numArray.Length - length)) > 0)
                {
                    length += num2;
                    if (length == numArray.Length)
                    {
                        int num3 = stream.ReadByte();
                        if (num3 != -1)
                        {
                            byte[] dst = new byte[numArray.Length * 2];
                            Buffer.BlockCopy((Array)numArray, 0, (Array)dst, 0, numArray.Length);
                            Buffer.SetByte((Array)dst, length, (byte)num3);
                            numArray = dst;
                            ++length;
                        }
                    }
                }
                byte[] dst1 = numArray;
                if (numArray.Length != length)
                {
                    dst1 = new byte[length];
                    Buffer.BlockCopy((Array)numArray, 0, (Array)dst1, 0, length);
                }
                return dst1;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = num1;
            }
        }

        public static void Delete(string file)
        {
            file = DuckFile.PreparePath(file);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.Delete(" + file + ")");
            DuckFile.TryFileOperation((Action)(() =>
           {
               if (!System.IO.File.Exists(file))
                   return;
               DuckFile.TryClearAttributes(file);
               System.IO.File.Delete(file);
           }), "DuckFile.Delete(" + file + ")");
            DuckFile.Commit(file, true);
        }
    }
}
