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
//using XnaToFna;

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

        public static string saveRoot => _saveRoot;

        public static string saveDirectory => _saveRoot + _saveDirectory;

        public static string levelDirectory => saveDirectory + _levelDirectory;

        public static string cloudLevelDirectory => userDirectory + _levelDirectory;

        public static string editorPreviewDirectory => saveDirectory + _editorPreviewDirectory;

        public static string workshopDirectory => saveDirectory + _workshopDirectory;

        public static string onlineLevelDirectory => saveDirectory + _onlineLevelDirectory;

        public static string albumDirectory => saveDirectory + _albumDirectory;

        public static string profileDirectory => userDirectory + _profileDirectory;

        public static string optionsDirectory
        {
            get
            {
                string pPath = userDirectory + _optionsDirectory;
                return !DirectoryExists(pPath) && !MonoMain.atPostCloudLogic ? globalOptionsDirectory : pPath;
            }
        }

        public static string modsDirectory => userDirectory + _modsDirectory;

        public static string globalProfileDirectory => saveDirectory + _profileDirectory;

        public static string globalOptionsDirectory => saveDirectory + _optionsDirectory;

        public static string globalModsDirectory => saveDirectory + _modsDirectory;

        public static string globalSkinsDirectory => globalModsDirectory + _skinsDirectory;

        public static string globalMappackDirectory => globalModsDirectory + _mappackDirectory;

        public static string globalHatpackDirectory => globalModsDirectory + _hatpackDirectory;

        public static string challengeDirectory => saveDirectory + _challengeDirectory;

        public static string scriptsDirectory => saveDirectory + _scriptsDirectory;

        public static string skinsDirectory => modsDirectory + _skinsDirectory;

        public static string mappackDirectory => modsDirectory + _mappackDirectory;

        public static string hatpackDirectory => modsDirectory + _hatpackDirectory;

        public static string userDirectory => Steam.user != null ? saveDirectory + Steam.user.id.ToString() + "/" : saveDirectory;

        public static string customBlockDirectory => saveDirectory + _customBlockDirectory;

        public static string downloadedBlockDirectory => saveDirectory + _downloadedBlockDirectory;

        public static string customBackgroundDirectory => saveDirectory + _customBackgroundDirectory;

        public static string downloadedBackgroundDirectory => saveDirectory + _downloadedBackgroundDirectory;

        public static string customPlatformDirectory => saveDirectory + _customPlatformDirectory;

        public static string downloadedPlatformDirectory => saveDirectory + _downloadedPlatformDirectory;

        public static string customParallaxDirectory => saveDirectory + _customParallaxDirectory;

        public static string downloadedParallaxDirectory => saveDirectory + _downloadedParallaxDirectory;

        public static string customArcadeDirectory => saveDirectory + _customArcadeDirectory;

        public static string customMojiDirectory => saveDirectory + _customMojiDirectory;

        public static string logDirectory => saveDirectory + _logDirectory;

        public static string contentDirectory => _contentDirectory;

        public static string musicDirectory => contentDirectory + _musicDirectory;

        public static string GetCustomDownloadDirectory(CustomType t)
        {
            switch (t)
            {
                case CustomType.Block:
                    return downloadedBlockDirectory;
                case CustomType.Platform:
                    return downloadedPlatformDirectory;
                case CustomType.Background:
                    return downloadedBackgroundDirectory;
                case CustomType.Parallax:
                    return downloadedParallaxDirectory;
                default:
                    return "";
            }
        }

        public static string GetCustomDirectory(CustomType t)
        {
            switch (t)
            {
                case CustomType.Block:
                    return customBlockDirectory;
                case CustomType.Platform:
                    return customPlatformDirectory;
                case CustomType.Background:
                    return customBackgroundDirectory;
                case CustomType.Parallax:
                    return customParallaxDirectory;
                default:
                    return "";
            }
        }

        public static string oldSaveLocation => (Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/").Replace('\\', '/');

        public static string newSaveLocation => (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/").Replace('\\', '/');

        public static void Initialize()
        {
            _saveDirectory = "DuckGame/";
            bool flag = true;
            string oldSaveLocation = DuckFile.oldSaveLocation;
            _saveRoot = newSaveLocation;
            if (!DirectoryExists(_saveRoot + _saveDirectory) && !Program.alternateSaveLocation && DirectoryExists(oldSaveLocation + _saveDirectory))
            {
                _saveRoot = oldSaveLocation;
                flag = false;
                appdataSave = false;
            }
            if (flag)
            {
                appdataSave = true;
                try
                {
                    string str1 = oldSaveLocation + _saveDirectory;
                    if (Program.alternateSaveLocation && DirectoryExists(str1) && !DirectoryExists(saveDirectory))
                        DirectoryCopy(str1, saveDirectory, true);
                    string path = str1 + "where_is_my_save.txt";
                    if (!File.Exists(path))
                    {
                        CreatePath(str1);
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        using (StreamWriter streamWriter = new StreamWriter(str1 + "Save Data.url"))
                        {
                            string location = Assembly.GetExecutingAssembly().Location;
                            streamWriter.WriteLine("[InternetShortcut]");
                            streamWriter.WriteLine("URL=file:///" + saveDirectory);
                            streamWriter.WriteLine("IconIndex=0");
                            string str2 = location.Replace('\\', '/');
                            streamWriter.WriteLine("IconFile=" + str2);
                        }
                        File.WriteAllText(path, "Hey! Keeping save data in the Documents folder was causing all kinds\nof issues for people, and it's with great sadness that I had to move your data.\nDon't worry, it still exists- your data is now located here:\n\n" + saveDirectory + "\n\nAny save data still located in this folder is for the old version (pre-2020) of Duck Game.");
                    }
                }
                catch (Exception)
                {
                }
            }
            DevConsole.Log(DCSection.General, "DuckFile.Initialize().. " + (_saveRoot.Contains("OneDrive/") ? "Ah, a |DGBLUE|OneDrive|WHITE| user, I see.." : ""));
            if (!DirectoryExists(saveDirectory))
                freshInstall = true;
            _levelDirectory = "Levels/";
            _allPaths.Add(_levelDirectory);
            _editorPreviewDirectory = "EditorPreviews/";
            _allPaths.Add(_editorPreviewDirectory);
            _onlineLevelDirectory = "Online/Levels/";
            _allPaths.Add(_onlineLevelDirectory);
            _optionsDirectory = "Options/";
            _allPaths.Add(_optionsDirectory);
            _albumDirectory = "Album/";
            _allPaths.Add(_albumDirectory);
            _profileDirectory = "Profiles/";
            _allPaths.Add(_profileDirectory);
            _challengeDirectory = "ChallengeData/";
            _allPaths.Add(_challengeDirectory);
            _modsDirectory = "Mods/";
            _allPaths.Add(_modsDirectory);
            _skinsDirectory = "Texpacks/";
            _allPaths.Add(_skinsDirectory);
            _mappackDirectory = "Mappacks/";
            _allPaths.Add(_mappackDirectory);
            _hatpackDirectory = "Hatpacks/";
            _allPaths.Add(_hatpackDirectory);
            _scriptsDirectory = "Scripts/";
            _allPaths.Add(_scriptsDirectory);
            _workshopDirectory = "Workshop/";
            _allPaths.Add(_workshopDirectory);
            _customBlockDirectory = "Custom/Blocks/";
            _allPaths.Add(_customBlockDirectory);
            CreatePath(customBlockDirectory);
            _downloadedBlockDirectory = "Custom/Blocks/Downloaded/";
            _allPaths.Add(_downloadedBlockDirectory);
            _customBackgroundDirectory = "Custom/Background/";
            _allPaths.Add(_customBackgroundDirectory);
            CreatePath(customBackgroundDirectory);
            _downloadedBackgroundDirectory = "Custom/Background/Downloaded/";
            _allPaths.Add(_downloadedBackgroundDirectory);
            _customPlatformDirectory = "Custom/Platform/";
            _allPaths.Add(_customPlatformDirectory);
            CreatePath(customPlatformDirectory);
            _downloadedPlatformDirectory = "Custom/Platform/Downloaded/";
            _allPaths.Add(_downloadedPlatformDirectory);
            _customParallaxDirectory = "Custom/Parallax/";
            _allPaths.Add(_customParallaxDirectory);
            CreatePath(customParallaxDirectory);
            _downloadedParallaxDirectory = "Custom/Parallax/Downloaded/";
            _allPaths.Add(_downloadedParallaxDirectory);
            _customArcadeDirectory = "Custom/Arcade/";
            _allPaths.Add(customArcadeDirectory);
            CreatePath(customArcadeDirectory);
            try
            {
                _customMojiDirectory = "Custom/Moji/";
                _allPaths.Add(customMojiDirectory);
                CreatePath(customMojiDirectory);
                CreatePath(saveDirectory + "Custom/Hats/");
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General, "|DGRED|Could not create moji path, disabling custom mojis :(");
                mojimode = false;
            }
            _logDirectory = "Logs/";
            _allPaths.Add(logDirectory);
            _musicDirectory = "Audio/Music/";
        }

        public static void FlagForBackup()
        {
            if (!MonoMain.started)
                return;
            _flaggedForBackup = true;
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
                DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
            }
        }

        public static Dictionary<string, Sprite> mojis => _mojis;

        public static Sprite GetMoji(string moji, NetworkConnection pConnection = null)
        {
            if (!mojimode)
                return null;
            if (Options.Data.mojiFilter == 1 && pConnection != null && pConnection.data is User && (pConnection.data as User).relationship != FriendRelationship.Friend)
                return null;
            Sprite s1 = null;
            if (pConnection != null)
            {
                Dictionary<string, Sprite> dictionary = null;
                if (_profileMojis.TryGetValue(pConnection.identifier, out dictionary))
                    dictionary.TryGetValue(moji, out s1);
            }
            else
                _mojis.TryGetValue(moji, out s1);
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
                                s1 = GetMoji(moji, connection);
                                if (s1 != null && pConnection == DuckNetwork.localConnection)
                                {
                                    SaveString(Editor.TextureToString((Texture2D)s1.texture), customMojiDirectory + moji + ".moj");
                                    RegisterMoji(moji, s1);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (s1 == null && waitTry <= 0)
            {
                waitTry = 60;
                try
                {

                    string basefilepath = customMojiDirectory + moji;
                    if (Program.IsLinuxD || Program.isLinux)
                    {
                        basefilepath = basefilepath.Replace("//", "/").Replace("\\", "/");
                        basefilepath = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(basefilepath), true);
                    }
                    string fileName = basefilepath + ".png";
                    if (!File.Exists(fileName))
                        fileName = basefilepath + ".jpg";
                    if (!File.Exists(fileName))
                        fileName = basefilepath + ".jpeg";
                    if (!File.Exists(fileName))
                        fileName = basefilepath + ".bmp";
                    if (File.Exists(fileName))
                    {
                        Texture2D t = TextureConverter.LoadPNGWithPinkAwesomenessAndMaxDimensions(Graphics.device, fileName, true, new Vec2(28f, 28f));
                        if (t != null)
                        {
                            if (t.Width <= 28 && t.Height <= 28)
                            {
                                Sprite s2 = new Sprite((Tex2D)t);
                                RegisterMoji(moji, s2);
                                if (TextureConverter.lastLoadResultedInResize)
                                {
                                    try
                                    {
                                        TryFileOperation(() =>
                                        {
                                            string str = fileName;
                                            Delete(str);
                                            if (str.EndsWith(".jpg"))
                                                str = str.Replace(".jpg", ".png");
                                            if (str.EndsWith(".bmp"))
                                                str = str.Replace(".bmp", ".png");
                                            if (str.EndsWith(".jpeg"))
                                                str = str.Replace(".jpeg", ".png");
                                            t.SaveAsPng(File.Create(str), t.Width, t.Height);
                                        }, "InitializeMojis.Resize");
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
                        fileName = customMojiDirectory + moji + ".moj";
                        if (Program.IsLinuxD || Program.isLinux)
                        {
                            fileName = fileName.Replace("//", "/").Replace("\\", "/");
                            fileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(fileName), true);
                        }
                        if (File.Exists(fileName))
                        {
                            Texture2D texture = Editor.StringToTexture(ReadAllText(fileName));
                            if (texture != null)
                            {
                                if (texture.Width <= 28 && texture.Height <= 28)
                                {
                                    Sprite s3 = new Sprite((Tex2D)texture);
                                    RegisterMoji(moji, s3);
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
            if (waitTry > 0)
                --waitTry;
            return s1;
        }

        public static void StealMoji(string moji)
        {
            foreach (NetworkConnection connection in Network.connections)
            {
                if (connection != DuckNetwork.localConnection)
                {
                    Sprite moji1 = GetMoji(moji, connection);
                    if (moji1 != null)
                    {
                        SaveString(Editor.TextureToString((Texture2D)moji1.texture), customMojiDirectory + moji + ".moj");
                        RegisterMoji(moji, moji1);
                    }
                }
            }
        }

        public static void RegisterMoji(string moji, Sprite s, NetworkConnection pConnection = null)
        {
            if (!mojimode)
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
                Dictionary<string, Sprite> dictionary;
                if (!_profileMojis.TryGetValue(pConnection.identifier, out dictionary))
                    dictionary = _profileMojis[pConnection.identifier] = new Dictionary<string, Sprite>();
                dictionary[moji] = s;
            }
            else
                _mojis[moji] = s;
        }

        public static void InitializeMojis()
        {
            if (!mojimode)
                return;
            List<string> list = GetFiles(customMojiDirectory, "*.png").ToList();
            list.AddRange(GetFiles(customMojiDirectory, "*.jpg"));
            list.AddRange(GetFiles(customMojiDirectory, "*.jpeg"));
            list.AddRange(GetFiles(customMojiDirectory, "*.bmp"));
            foreach (string str1 in list)
            {
                string s = str1;
                try
                {
                    Texture2D t = TextureConverter.LoadPNGWithPinkAwesomenessAndMaxDimensions(Graphics.device, s, true, new Vec2(28f, 28f));
                    if (t != null)
                    {
                        if (t.Width <= 28 && t.Height <= 28)
                        {
                            Sprite s1 = new Sprite((Tex2D)t);
                            RegisterMoji(Path.GetFileNameWithoutExtension(s), s1);
                            if (TextureConverter.lastLoadResultedInResize)
                            {
                                try
                                {
                                    TryFileOperation(() =>
                                   {
                                       string str2 = s;
                                       Delete(str2);
                                       if (str2.EndsWith(".jpg"))
                                           str2 = str2.Replace(".jpg", ".png");
                                       if (str2.EndsWith(".bmp"))
                                           str2 = str2.Replace(".bmp", ".png");
                                       if (str2.EndsWith(".jpeg"))
                                           str2 = str2.Replace(".jpeg", ".png");
                                       t.SaveAsPng(File.Create(str2), t.Width, t.Height);
                                   }, "InitializeMojis.Resize");
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
                catch (Exception)
                {
                }
            }
            foreach (string str in GetFiles(customMojiDirectory, "*.moj").ToList())
            {
                try
                {
                    Texture2D texture = Editor.StringToTexture(ReadAllText(str));
                    if (texture != null)
                    {
                        if (texture.Width <= 28 && texture.Height <= 28)
                        {
                            Sprite s = new Sprite((Tex2D)texture);
                            RegisterMoji(Path.GetFileNameWithoutExtension(str), s);
                        }
                        else
                            DevConsole.Log("Error loading " + Path.GetFileName(str) + " MOJI (must be smaller than 28x28)", Color.Red);
                    }
                }
                catch (Exception)
                {
                }
            }
            _mojis = _mojis.OrderByDescending(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static void BeginDataCommit() => _suppressCommit = true;

        public static void EndDataCommit() => _suppressCommit = false;

        private static void Commit(string pPath, bool pDelete = false)
        {
            int num = _suppressCommit ? 1 : 0;
            if (pPath == null)
                return;
            if (pDelete)
                Cloud.Delete(pPath);
            else
                Cloud.Write(pPath);
        }

        public static void DeleteAllSaveData() => Directory.Delete(userDirectory, true);

        public static void CreatePath(string pathString) => CreatePath(pathString, false);

        public static void CreatePath(string pathString, bool ignoreLast)
        {
            pathString = pathString.Replace('\\', '/');
            string[] source = pathString.Split('/');
            string str = "";
            if (Program.IsLinuxD || Program.isLinux)
            {
                str = "/";
            }
            for (int index = 0; index < source.Length; ++index)
            {
                if (!(source[index] == "") && !(source[index] == "/") && (!(source[index].Contains('.') | ignoreLast) || index != source.Length - 1))
                {
                    string path = str + source[index];
                    if (!Directory.Exists(path))
                    {
                        if (MonoMain.logFileOperations)
                            DevConsole.Log(DCSection.General, "DuckFile.CreatePath(" + path + ")");
                        Directory.CreateDirectory(path);
                        Commit(null);
                    }
                    str = path + "/";
                }
            }
        }

        public static FileStream Create(string path)
        {
            CreatePath(path);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.Create(" + path + ")");
            return File.Create(path);
        }

        public static string ReadAllText(string pPath)
        {
            pPath = PreparePath(pPath);
            TryClearAttributes(pPath);
            string text = "";
            TryFileOperation(() => text = File.ReadAllText(pPath), "ReadAllText(" + pPath + ")");
            return text;
        }

        public static string[] ReadAllLines(string path)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.ReadAllLines(" + path + ")");
            return File.ReadAllLines(path);
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
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetLocalSavePath(string path)
        {
            path.Replace('\\', '/');
            if (IsUserPath(path))
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
            return null;
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
                        List<string> filesNoCloud2 = GetFilesNoCloud(directory, filter, so);
                        filesNoCloud1.AddRange(filesNoCloud2);
                    }
                }
                catch (Exception)
                {
                }
            }
            filesNoCloud1.Sort();
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
                string str = pSubFolder.Replace(userDirectory, "").Replace('\\', '/');
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
            string path = pSubFolder != null ? pSubFolder : userDirectory;
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
                    GetAllSavegameFiles(pFolderFilters, pRet1, directory, pRecurseFiltered1, pDontAddFilteredFolders);
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
            catch (Exception)
            {
            }
            return directoriesNoCloud;
        }

        public static string[] GetFiles(string path, string filter) => GetFiles(path, filter, SearchOption.TopDirectoryOnly);

        public static string[] GetFiles(string path, string filter = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            List<string> stringList = new List<string>();
            if (Directory.Exists(path))
            {
                stringList = GetFilesNoCloud(path, filter, option);
                for (int index = 0; index < stringList.Count; ++index)
                    stringList[index] = stringList[index].Replace('\\', '/');
            }
            stringList.Sort();
            return stringList.ToArray();
        }

        public static List<string> ReGetFiles(string path, string filter = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
        {
            List<string> stringList = new List<string>();
            if (Directory.Exists(path))
            {
                stringList = GetFilesNoCloud(path, filter, option);
                for (int index = 0; index < stringList.Count; ++index)
                    stringList[index] = stringList[index].Replace('\\', '/');
            }
            if (DGRSettings.SortLevels) stringList.Sort();
            return stringList;
        }

        public static List<string> ReGetDirectories(string path)
        {
            path = path.Replace('\\', '/');
            List<string> stringList = new List<string>();
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
            if (Directory.Exists(path))
            {
                List<string> paths = GetDirectoriesNoCloud(path);
                for (int i = 0; i < paths.Count; i++)
                {
                    string path1 = paths[i];
                    if (!Path.GetFileName(path1).Contains("._"))
                    {
                        string str = path1.Replace('\\', '/');
                        if (!stringList.Contains(str))
                            stringList.Add(str);
                    }
                }
            }
            return stringList;
        }

        public static string[] GetDirectories(string path)
        {
            path = path.Replace('\\', '/');
            List<string> stringList = new List<string>();
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
            if (Directory.Exists(path))
            {
                foreach (string path1 in GetDirectoriesNoCloud(path))
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
                str1 = !_invalidPathCharConversions.TryGetValue(key, out str2) ? str1 + key.ToString() : str1 + str2;
            }
            return str1;
        }

        public static string RestoreInvalidCharacters(string path)
        {
            foreach (KeyValuePair<char, string> pathCharConversion in _invalidPathCharConversions)
                path = path.Replace(pathCharConversion.Value, pathCharConversion.Key.ToString() ?? "");
            return path;
        }

        public static LevelData LoadLevelHeaderCached(string path)
        {
            LevelData levelData;
            if (!_levelCache.TryGetValue(path, out levelData))
                levelData = _levelCache[path] = LoadLevel(path, true);
            return levelData;
        }

        public static LevelData LoadLevel(string path)
        {
            return LoadLevel(path, false);
        }
        public static bool FileExists(string pPath)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                pPath = pPath.Replace("//", "/").Replace("\\", "/");
                pPath = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(pPath), true);
            }
            return File.Exists(pPath);
        }

        public static bool DirectoryExists(string pPath) => Directory.Exists(pPath);

        public static LevelData LoadLevel(string path, bool pHeaderOnly)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
            }
            Cloud.ReplaceLocalFileWithCloudFile(path);
            if (!File.Exists(path))
                return null;
            if (MonoMain.logLevelOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadLevel(" + path + ")");
            LevelData levelData = LoadLevel(File.ReadAllBytes(path), pHeaderOnly);
            levelData?.SetPath(path);
            return levelData;
        }

        private static LevelData ConvertLevel(byte[] data)
        {
            if (MonoMain.logLevelOperations)
                DevConsole.Log(DCSection.General, "DuckFile.ConvertLevel()");
            LevelData levelData = null;
            Editor editor = new Editor();
            bool skipInitialize = Level.skipInitialize;
            Level.skipInitialize = true;
            Level currentLevel = Level.core.currentLevel;
            Level.core.currentLevel = editor;
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
                    string str = null;
                    if (!_conversionGUIDMap.TryGetValue(key, out str))
                    {
                        str = levelData.metaData.guid;
                        _conversionGUIDMap[key] = str;
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

        public static LevelData LoadLevel(byte[] data) => LoadLevel(data, false);

        public static LevelData LoadLevel(byte[] data, bool pHeaderOnly)
        {
            LevelData levelData = BinaryClassChunk.FromData<LevelData>(new BitBuffer(data, false), pHeaderOnly);
            if (!pHeaderOnly && levelData == null || levelData != null && levelData.GetResult() == DeserializeResult.InvalidMagicNumber)
            {
                Promise<LevelData> promise = Tasker.Task(() => ConvertLevel(data));
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
            using (FileStream fileStream = File.Create(tempFileName, 4096, FileOptions.WriteThrough))
                fileStream.Write(bytes, 0, bytes.Length);
            File.Replace(tempFileName, path, null);
        }

        public static void WriteAllText(string pPath, string pContents)
        {
            pPath = PreparePath(pPath, true);
            TryClearAttributes(pPath);
            TryFileOperation(() => File.WriteAllText(pPath, pContents), "WriteAllText(" + pPath + ")");
            Commit(pPath);
        }

        public static DuckXML LoadDuckXML(string path)
        {
            Cloud.ReplaceLocalFileWithCloudFile(path);
            if (!File.Exists(path))
                return null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadDuckXML(" + path + ")");
            DuckXML duckXml = null;
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
            path = PreparePath(path, true);
            string docString = doc.ToString();
            if (string.IsNullOrWhiteSpace(docString))
                throw new Exception("Blank XML (" + path + ")");
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveDuckXML(" + path + ")");
            TryClearAttributes(path);
            TryFileOperation(() => File.WriteAllText(path, docString), "SaveDuckXML(" + path + ")");
            Commit(path);
        }

        public static void TryClearAttributes(string pFilename)
        {
            try
            {
                if (Program.IsLinuxD || Program.isLinux)
                {
                    pFilename = pFilename.Replace("//", "/").Replace("\\", "/");
                    pFilename = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(pFilename), true);
                }
                if (!File.Exists(pFilename))
                    return;
                File.SetAttributes(pFilename, FileAttributes.Normal);
            }
            catch (Exception)
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
            CreatePath(Path.GetDirectoryName(path));
            if (!File.Exists(path))
                return null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadXDocument(" + path + ")");
            try
            {
                return XDocument.Load(path);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveXDocument(XDocument doc, string path)
        {
            path = PreparePath(path, true);
            string docString = doc.ToString();
            if (string.IsNullOrWhiteSpace(docString))
                throw new Exception("Blank XML (" + path + ")");
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveXDocument(" + path + ")");
            TryClearAttributes(path);
            TryFileOperation(() => File.WriteAllText(path, docString), "SaveXDocument(" + path + ")");
            Commit(path);
        }

        public static string LoadString(string pPath)
        {
            CreatePath(Path.GetDirectoryName(pPath));
            if (!File.Exists(pPath))
                return null;
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadString(" + pPath + ")");
            try
            {
                return File.ReadAllText(pPath);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveString(string pString, string pPath)
        {
            pPath = PreparePath(pPath, true);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveString(" + pPath + ")");
            TryFileOperation(() =>
           {
               TryClearAttributes(pPath);
               File.WriteAllText(pPath, pString);
           }, "SaveString(" + pPath + ")");
            Commit(pPath);
        }

        public static string PreparePath(string pPath, bool pCreatePath = false)
        {
            pPath = pPath.Replace("//", "/");
            pPath = pPath.Replace('\\', '/');
            if (Program.IsLinuxD)
            {
                pPath = pPath.Replace("//", "/").Replace("\\", "/");
                pPath = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(pPath), true);
            }
            else
            {
                if (pPath.Length > 1 && pPath[1] == ':')
                {
                    pPath = char.ToUpper(pPath[0]).ToString() + pPath.Substring(1, pPath.Length - 1);
                }
            }
            //DevConsole.Log("do create path " + pCreatePath.ToString() + " " + pPath + " " + Path.GetDirectoryName(pPath), Color.Green);
            if (pCreatePath)
            {
                CreatePath(Path.GetDirectoryName(pPath));
            }
            try
            {
                if (File.Exists(pPath))
                    File.SetAttributes(pPath, FileAttributes.Normal);
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
            pPath = PreparePath(pPath);
            if (!File.Exists(pPath))
                return null;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(pPath);
            return xmlDocument;
        }

        public static void SaveSharpXML(XmlDocument pDoc, string pPath)
        {
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveSharpXML(" + pPath + ")");
            TryFileOperation(() =>
           {
               pPath = PreparePath(pPath, true);
               TryClearAttributes(pPath);
               pDoc.Save(pPath);
           }, "SaveSharpXML(" + pPath + ")");
            Commit(pPath);
        }

        public static T LoadChunk<T>(string path) where T : BinaryClassChunk
        {
            CreatePath(Path.GetDirectoryName(path));
            if (!File.Exists(path))
                return default(T);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.LoadChunk(" + path + ")");
            return BinaryClassChunk.FromData<T>(new BitBuffer(File.ReadAllBytes(path), 0, false));
        }

        public static bool GetLevelSpacePercentUsed(ref float percent) => false;

        public static void EnsureDownloadFileSpaceAvailable()
        {
        }

        public static bool SaveChunk(BinaryClassChunk doc, string path)
        {
            path = PreparePath(path, true);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.SaveChunk(" + path + ")");
            BitBuffer data = null;
            data = doc.Serialize();
            TryFileOperation(() =>
            {
                FileStream fileStream = File.Create(path);
                fileStream.Write(data.buffer, 0, data.lengthInBytes);
                fileStream.Close();
            }, "SaveChunk(" + path + ")");
            Commit(path);
            return true;
        }

        public static void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                if (MonoMain.logFileOperations)
                    DevConsole.Log(DCSection.General, "DuckFile.DeleteFolder(" + folder + ")");
                foreach (string directory in GetDirectories(folder))
                    DeleteFolder(directory);
                foreach (string file in GetFiles(folder))
                {
                    if (file.EndsWith(".lev"))
                        Editor.Delete(file);
                    else
                        Delete(file);
                }
                Directory.Delete(folder);
            }
            Commit(null);
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
                            Buffer.BlockCopy(numArray, 0, dst, 0, numArray.Length);
                            Buffer.SetByte(dst, length, (byte)num3);
                            numArray = dst;
                            ++length;
                        }
                    }
                }
                byte[] dst1 = numArray;
                if (numArray.Length != length)
                {
                    dst1 = new byte[length];
                    Buffer.BlockCopy(numArray, 0, dst1, 0, length);
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
            file = PreparePath(file);
            if (MonoMain.logFileOperations)
                DevConsole.Log(DCSection.General, "DuckFile.Delete(" + file + ")");
            TryFileOperation(() =>
           {
               if (!File.Exists(file))
                   return;
               TryClearAttributes(file);
               File.Delete(file);
           }, "DuckFile.Delete(" + file + ")");
            Commit(file, true);
        }
    }
}
