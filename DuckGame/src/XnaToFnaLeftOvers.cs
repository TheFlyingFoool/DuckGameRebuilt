using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DuckGame
{
    public static class XnaToFnaHelper
    {
        public static void MainHook(string[] args)
        {
        }

       

        public static void Log(string s)
        {
            Console.Write("[XnaToFnaHelper] ");
            Console.WriteLine(s);
        }

       
        public static DirectoryInfo DirectoryCreateDirectory(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.FixPath(path);
            }
            return Directory.CreateDirectory(path);
        }

        public static string[] DirectoryGetFiles(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.FixPath(path);
            }
            return Directory.GetFiles(path);
        }

        public static bool DirectoryExists(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.FixPath(path);
            }
            return Directory.Exists(path);
        }

        public static void DirectoryDelete(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.FixPath(path);
            }
            Directory.Delete(path);
        }

        public static void FileDelete(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
            }
            File.Delete(path);
        }

        public static bool FileExists(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
            }
            return File.Exists(path);
        }

        public static byte[] FileReadAllBytes(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), false);
            }
            return File.ReadAllBytes(path);
        }

        public static void FileWriteAllLines(string path, string[] contents)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
            }
            File.WriteAllLines(path, contents);
        }

        public static void FileWriteAllText(string path, string contents)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), true);
            }
            File.WriteAllText(path, contents);
        }

        public static string[] FileReadAllLines(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), false);
            }
            return File.ReadAllLines(path);
        }

        public static void FileCopy(string sourceFileName, string destFileName, bool overwrite)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                sourceFileName = sourceFileName.Replace("//", "/").Replace("\\", "/");
                sourceFileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(sourceFileName), false);
                destFileName = destFileName.Replace("//", "/").Replace("\\", "/");
                destFileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(destFileName), true);
            }
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public static void FileCopy(string sourceFileName, string destFileName)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                sourceFileName = sourceFileName.Replace("//", "/").Replace("\\", "/");
                sourceFileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(sourceFileName), false);
                destFileName = destFileName.Replace("//", "/").Replace("\\", "/");
                destFileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(destFileName), true);
            }
            File.Copy(sourceFileName, destFileName, false);
        }

        public static string Combine(string path1, string path2)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path1 = path1.Replace("//", "/").Replace("\\", "/");
                path2 = path2.Replace("//", "/").Replace("\\", "/");
            }
            return Path.Combine(path1, path2);
        }

        public static string FileReadAllText(string path)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), false);
            }
            return File.ReadAllText(path);
        }

        public static void FileSetAttributes(string path, FileAttributes fileAttributes)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), false);
            }
            File.SetAttributes(path, fileAttributes);
        }

        public static string FileReadAllText(string path, Encoding encoding)
        {
            bool flag = Program.IsLinuxD || Program.isLinux;
            if (flag)
            {
                path = path.Replace("//", "/").Replace("\\", "/");
                path = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(path), false);
            }
            return File.ReadAllText(path, encoding);
        }

        public static string FixPath(string path)
        {
            path = path.Replace("\\", "/");
            while (path.Contains("//"))
            {
                path = path.Replace("//", "/");
            }
            bool flag = !Path.IsPathRooted(path) || Directory.Exists(path) || File.Exists(path);
            string text;
            if (flag)
            {
                text = path;
            }
            else
            {
                List<int> list = new List<int> { path.Length };
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    bool flag2 = path[i] == '/';
                    if (flag2)
                    {
                        list.Add(i);
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    string text2 = path.Substring(0, list[j]);
                    bool flag3 = text2 == "";
                    if (!flag3)
                    {
                        string fileName = Path.GetFileName(text2);
                        bool flag4 = !Directory.Exists(text2);
                        if (!flag4)
                        {
                            return path;
                        }
                        string text3 = path.Substring(0, list[j + 1]);
                        bool flag5 = !Directory.Exists(text3);
                        if (!flag5)
                        {
                            foreach (string text4 in Directory.GetDirectories(text3))
                            {
                                bool flag6 = Path.GetFileName(text4).ToLower() == fileName.ToLower();
                                if (flag6)
                                {
                                    return XnaToFnaHelper.FixPath(text4 + path.Substring(text4.Length, path.Length - text4.Length));
                                }
                            }
                            return path;
                        }
                    }
                }
                text = path;
            }
            return text;
        }

        public static bool get_IsTrialMode()
        {
            return Environment.GetEnvironmentVariable("XNATOFNA_ISTRIALMODE") != "0";
        }

        public static void DoNothing()
        {
        }

        public static void ApplyChanges(GraphicsDeviceManager self)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_FULLSCREEN");
            bool flag = environmentVariable == "0";
            if (flag)
            {
                self.IsFullScreen = false;
            }
            else
            {
                bool flag2 = environmentVariable == "1";
                if (flag2)
                {
                    self.IsFullScreen = true;
                }
            }
            int num;
            bool flag3 = int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_WIDTH") ?? "", out num);
            if (flag3)
            {
                self.PreferredBackBufferWidth = num;
            }
            int num2;
            bool flag4 = int.TryParse(Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_HEIGHT") ?? "", out num2);
            if (flag4)
            {
                self.PreferredBackBufferHeight = num2;
            }
            string[] array = (Environment.GetEnvironmentVariable("XNATOFNA_DISPLAY_SIZE") ?? "").Split(new char[] { 'x' });
            bool flag5 = array.Length == 2;
            if (flag5)
            {
                bool flag6 = int.TryParse(array[0], out num);
                if (flag6)
                {
                    self.PreferredBackBufferWidth = num;
                }
                bool flag7 = int.TryParse(array[1], out num2);
                if (flag7)
                {
                    self.PreferredBackBufferHeight = num2;
                }
            }
            self.ApplyChanges();
        }

        public static void PreUpdate(GameTime time)
        {
            
        }

        //public static T GetService<T>() where T : class
        //{
        //    return (T)((object)XnaToFnaHelper.Game.Services.GetService(typeof(T)));
        //}

        //public static B GetService<A, B>() where A : class where B : class, A
        //{
        //    return XnaToFnaHelper.Game.Services.GetService(typeof(A)) as B;
        //}

        public static void PlatformHook(string name)
        {
            Type typeFromHandle = typeof(XnaToFnaHelper);
            Assembly assembly = Assembly.GetAssembly(typeof(Game));
            FieldInfo field = assembly.GetType("Microsoft.Xna.Framework.FNAPlatform").GetField(name);
            typeFromHandle.GetField(string.Format("fna_{0}", name)).SetValue(null, field.GetValue(null));
            field.SetValue(null, Delegate.CreateDelegate(assembly.GetType(string.Format("Microsoft.Xna.Framework.FNAPlatform+{0}Func", name)), typeFromHandle.GetMethod(name)));
        }

        public static string GetActualCaseForFileName(string pathAndFileName, bool CanBeEmpty = false)
        {
            string text = pathAndFileName;
            bool flag = !Path.IsPathRooted(pathAndFileName) || File.Exists(pathAndFileName);
            string text2;
            if (flag)
            {
                text2 = pathAndFileName;
            }
            else
            {
                bool flag2 = false;
                string extension = Path.GetExtension(pathAndFileName);
                bool flag3 = extension == "";
                if (flag3)
                {
                    flag2 = true;
                }
                string directoryName = Path.GetDirectoryName(pathAndFileName);
                pathAndFileName = pathAndFileName.ToLower();
                bool flag4 = false;
                bool flag5 = Directory.Exists(directoryName);
                if (flag5)
                {
                    foreach (string text3 in Directory.GetFiles(directoryName))
                    {
                        bool flag6 = text3.Length >= pathAndFileName.Length;
                        if (flag6)
                        {
                            string text4 = text3;
                            bool flag7 = flag2 && text4.Length > 1;
                            if (flag7)
                            {
                                text4 = text4.Substring(0, text4.LastIndexOf("."));
                            }
                            bool flag8 = text4.ToLower() == pathAndFileName;
                            if (flag8)
                            {
                                pathAndFileName = text3;
                                flag4 = true;
                                break;
                            }
                        }
                    }
                }
                bool flag9 = !flag4;
                if (flag9)
                {
                    bool flag10 = !CanBeEmpty;
                    if (flag10)
                    {
                        throw new FileNotFoundException("File not found" + pathAndFileName, pathAndFileName);
                    }
                    text2 = text;
                }
                else
                {
                    bool flag11 = flag2 && pathAndFileName.Length > 1;
                    if (flag11)
                    {
                        pathAndFileName = pathAndFileName.Substring(0, pathAndFileName.LastIndexOf("."));
                    }
                    text2 = pathAndFileName;
                }
            }
            return text2;
        }

        public static void SDLWindowSizeChanged(object sender, EventArgs e)
        {
            
        }

        public static void ApplyWindowChanges(IntPtr window, int clientWidth, int clientHeight, bool wantsFullscreen, string screenDeviceName, ref string resultDeviceName)
        {
            
        }



        public static int MaximumGamepadCount;

        public static MulticastDelegate fna_ApplyWindowChanges;

        public static System.Windows.Forms.Form fillinform;
    }
}
