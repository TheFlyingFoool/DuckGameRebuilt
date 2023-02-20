// Decompiled with JetBrains decompiler
// Type: XnaToFna.XnaToFnaHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using XnaToFna.ProxyForms;
using System.Text;
using DGShared;
using SDL2;

namespace XnaToFna
{
    public static class XnaToFnaHelper
    {
        public static XnaToFnaGame Game;
        public static int MaximumGamepadCount;
        public static MulticastDelegate fna_ApplyWindowChanges;

        public static void MainHook(string[] args) { }

        public static void Initialize(XnaToFnaGame game)
        {
            Game = game;
            TextInputEXT.TextInput += new Action<char>(KeyboardEvents.CharEntered);
            if (Environment.GetEnvironmentVariable("FNADROID") != "1")
                TextInputEXT.StartTextInput();
            game.Window.ClientSizeChanged += new EventHandler<EventArgs>(SDLWindowSizeChanged);
            string environmentVariable = Environment.GetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS");
            if (string.IsNullOrEmpty(environmentVariable) || !int.TryParse(environmentVariable, out MaximumGamepadCount) || MaximumGamepadCount < 0)
                MaximumGamepadCount = Enum.GetNames(typeof(PlayerIndex)).Length;
            DeviceEvents.IsGamepadConnected = new bool[MaximumGamepadCount];
            PlatformHook("ApplyWindowChanges");
        }

        public static void Log(string s)
        {
            Console.Write("[XnaToFnaHelper] ");
            Console.WriteLine(s);
        }
        public static System.Windows.Forms.Form fillinform;
        public static IntPtr GetProxyFormHandle(this GameWindow window)
        {
            if (GameForm.Instance == null)
            {
                fillinform = new System.Windows.Forms.Form();
                Log("[ProxyForms] Creating game ProxyForms.GameForm");
                GameForm.Instance = new GameForm();
            }
            return fillinform.Handle;//GameForm.Instance.Handle;
        }
        public static DirectoryInfo DirectoryCreateDirectory(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = FixPath(path);
                //path = GetActualCaseForFileName(path); TODO make something for folder name uncasesentive
            }
            return Directory.CreateDirectory(path);
        }
        public static string[] DirectoryGetFiles(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                //Console.WriteLine("DirectoryGetFiles:" + path);
                path = FixPath(path);
            }
            return Directory.GetFiles(path);
        }
        public static bool DirectoryExists(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = FixPath(path);
            }
            return Directory.Exists(path);
        }
        public static void DirectoryDelete(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = FixPath(path);
            }
            Directory.Delete(path);
        }
        public static void FileDelete(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path), true);
            }
            File.Delete(path);
        }
        public static bool FileExists(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path), true);
            }
            return File.Exists(path);
        }
        public static byte[] FileReadAllBytes(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path));
            }
            return File.ReadAllBytes(path);
        }
        public static void FileWriteAllLines(string path, string[] contents)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path), true);
            }
            File.WriteAllLines(path, contents);
        }
        public static void FileWriteAllText(string path, string contents)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path), true);
            }
            File.WriteAllText(path, contents);
        }
        public static string[] FileReadAllLines(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path));
            }
            return File.ReadAllLines(path);
        }
        public static void FileCopy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                sourceFileName = sourceFileName.Replace("//", "/").Replace("\\", "/");
                sourceFileName = GetActualCaseForFileName(FixPath(sourceFileName));
                destFileName = destFileName.Replace("//", "/").Replace("\\", "/");
                destFileName = GetActualCaseForFileName(FixPath(destFileName), true);
            }
            File.Copy(sourceFileName, destFileName, overwrite);
        }
        public static string FileReadAllText(string path)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path));
            }
            return File.ReadAllText(path);
        }
        public static void FileSetAttributes(string path, FileAttributes fileAttributes)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path));
            }
            File.SetAttributes(path, fileAttributes);
        }
        public static string FileReadAllText(string path, Encoding encoding)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = GetActualCaseForFileName(FixPath(path));
            }
            return File.ReadAllText(path, encoding);
        }
        public static string FixPath(string path)
        {
            // Console.WriteLine(path);
            path = path.Replace("\\", "/");
            while (path.Contains("//"))
            {
                path = path.Replace("//", "/");
            }
            if (!Path.IsPathRooted(path) || Directory.Exists(path) || File.Exists(path))
            {
                return path;
            }
            List<int> pathindex = new List<int>() { path.Length };
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '/')
                {
                    pathindex.Add(i);
                }
            }
            for (int k = 0; k < pathindex.Count; k++)
            {
                string folderpath = path.Substring(0, pathindex[k]);
                if (folderpath == "")
                {
                    continue;
                }
                string foldername = Path.GetFileName(folderpath);
                if (!Directory.Exists(folderpath))
                {
                    string subfolder = path.Substring(0, pathindex[k + 1]);
                    if (!Directory.Exists(subfolder))
                    {
                        continue;
                    }
                    string[] fileEntries = Directory.GetDirectories(subfolder);
                    for (int j = 0; j < fileEntries.Length; j++)
                    {
                        string subfolder2 = fileEntries[j];
                        if (Path.GetFileName(subfolder2).ToLower() == foldername.ToLower())
                        {
                            return FixPath(subfolder2 + path.Substring(subfolder2.Length, path.Length - subfolder2.Length));
                        }
                    }
                    return path;
                }
                else
                {
                    return path;
                }
            }
            return path;
        }
        public static bool get_IsTrialMode() => Environment.GetEnvironmentVariable("XNATOFNA_ISTRIALMODE") != "0";

        public static void ApplyChanges(GraphicsDeviceManager self)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_FULLSCREEN");
            if (environmentVariable == "0")
                self.IsFullScreen = false;
            else if (environmentVariable == "1")
                self.IsFullScreen = true;
            int result1;
            if (int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_WIDTH") ?? "", out result1))
                self.PreferredBackBufferWidth = result1;
            int result2;
            if (int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_HEIGHT") ?? "", out result2))
                self.PreferredBackBufferHeight = result2;
            string[] strArray = (Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_SIZE") ?? "").Split('x');
            if (strArray.Length == 2)
            {
                if (int.TryParse(strArray[0], out result1))
                    self.PreferredBackBufferWidth = result1;
                if (int.TryParse(strArray[1], out result2))
                    self.PreferredBackBufferHeight = result2;
            }
            self.ApplyChanges();
        }

        public static void PreUpdate(GameTime time)
        {
            KeyboardEvents.Update();
            MouseEvents.Update();
            DeviceEvents.Update();
        }

        public static T GetService<T>() where T : class => (T)Game.Services.GetService(typeof(T));

        public static B GetService<A, B>()
        where A : class
        where B : class, A
        {
            return Game.Services.GetService(typeof(A)) as B;
        }

        public static void PlatformHook(string name)
        {
            Type type = typeof(XnaToFnaHelper);
            Assembly assembly = Assembly.GetAssembly(typeof(Game));
            FieldInfo field = assembly.GetType("Microsoft.Xna.Framework.FNAPlatform").GetField(name);
            type.GetField(string.Format("fna_{0}", name)).SetValue(null, field.GetValue(null));
            field.SetValue(null, Delegate.CreateDelegate(assembly.GetType(string.Format("Microsoft.Xna.Framework.FNAPlatform+{0}Func", name)), type.GetMethod(name)));
        }
        public static string GetActualCaseForFileName(string pathAndFileName, bool CanBeEmpty = false)
        {

            string ogpathAndFileName = pathAndFileName;
            if (!Path.IsPathRooted(pathAndFileName) || File.Exists(pathAndFileName))
            {
                return pathAndFileName;
            }
            bool didnothaveex = false;
            string pathex = Path.GetExtension(pathAndFileName);
            if (pathex == "")
            {
                didnothaveex = true;
            }
            string directory = Path.GetDirectoryName(pathAndFileName);
            pathAndFileName = pathAndFileName.ToLower();
            bool didfind = false;
            if (Directory.Exists(directory))
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    if (file.Length >= pathAndFileName.Length)
                    {
                        string filename = file;
                        if (didnothaveex)
                        {
                            filename = filename.Substring(0, filename.LastIndexOf("."));
                        }
                        if (filename.ToLower() == pathAndFileName)
                        {
                            pathAndFileName = file;
                            didfind = true;
                            break;
                        }
                    }
                }
            }
            if (!didfind)
            {
                if (!CanBeEmpty)
                {
                    throw new FileNotFoundException("File not found" + pathAndFileName, pathAndFileName);
                }
                return ogpathAndFileName;
            }
            if (didnothaveex)
            {
                pathAndFileName = pathAndFileName.Substring(0, pathAndFileName.LastIndexOf("."));
            }
            return pathAndFileName;
        }
        public static void SDLWindowSizeChanged(object sender, EventArgs e) => GameForm.Instance?.SDLWindowSizeChanged(sender, e);

        public static void ApplyWindowChanges(
          IntPtr window,
          int clientWidth,
          int clientHeight,
          bool wantsFullscreen,
          string screenDeviceName,
          ref string resultDeviceName)
        {
            object[] objArray = new object[6] {
         window,
         clientWidth,
         clientHeight,
         wantsFullscreen,
         screenDeviceName,
         resultDeviceName
      };
            fna_ApplyWindowChanges.DynamicInvoke(objArray);
            resultDeviceName = (string)objArray[5];
            GameForm.Instance?.SDLWindowChanged(window, clientWidth, clientHeight, wantsFullscreen, screenDeviceName, ref resultDeviceName);
            int num = resultDeviceName != screenDeviceName ? 1 : 0;
        }
    }
}