// Decompiled with JetBrains decompiler
// Type: DuckGame.DG
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DuckGame
{
    public static class DG
    {
        private static int _versionHigh = 7717;
        private static int _versionLow = 16376;
        private static int _versionMajor = 1;
        private static ulong _localID = 0;
        public static int MaxPlayers = 8;
        public static int MaxSpectators = 4;
        private static bool _drmFailure = false;
        private static bool _devBuild = false;
        private static bool _betaBuild = true;
        private static bool _pressBuild = true;

        public static string version => MakeVersionString(_versionMajor, _versionHigh, _versionLow);

        public static int versionHigh => _versionHigh;

        public static int versionLow => _versionLow;

        public static int versionMajor => _versionMajor;

        public static string MakeVersionString(int pMajor, int pHigh, int pLow) => "1." + pMajor.ToString() + "." + pHigh.ToString() + "." + pLow.ToString();

        public static bool isHalloween
        {
            get
            {
                DateTime localTime = MonoMain.GetLocalTime();
                if (localTime.Month != 10)
                    return false;
                return localTime.Day == 28 || localTime.Day == 29 || localTime.Day == 30 || localTime.Day == 31;
            }
        }

        public static Assembly[] assemblies => ModLoader.modAssemblyArray;

        public static string platform
        {
            get
            {
                //Get Operating system information.
                OperatingSystem os = Environment.OSVersion;
                //Get version information about the os.
                Version vs = os.Version;
                //Variable to hold our return value
                string operatingSystem = ""; //OS Mystery Edition

                if (os.Platform == PlatformID.Win32Windows)
                {
                    //This is a pre-NT version of Windows
                    switch (vs.Minor)
                    {
                        case 0:
                            operatingSystem = "Windows 95";
                            break;
                        case 10:
                            if (vs.Revision.ToString() == "2222A")
                                operatingSystem = "Windows 98SE";
                            else
                                operatingSystem = "Windows 98";
                            break;
                        case 90:
                            operatingSystem = "Windows Me";
                            break;
                        default:
                            break;
                    }
                }
                else if (os.Platform == PlatformID.Win32NT)
                {
                    switch (vs.Major)
                    {
                        case 3:
                            operatingSystem = "Windows NT 3.51";
                            break;
                        case 4:
                            operatingSystem = "Windows NT 4.0";
                            break;
                        case 5:
                            if (vs.Minor == 0)
                                operatingSystem = "Windows 2000";
                            else if (vs.Minor == 2)
                                operatingSystem = "Windows XP 64-Bit Edition";
                            else
                                operatingSystem = "Windows XP";
                            break;
                        case 6:
                            if (vs.Minor == 0)
                                operatingSystem = "Windows Vista";
                            else if (vs.Minor == 1)
                                operatingSystem = "Windows 7";
                            else if (vs.Minor == 2)
                                operatingSystem = "Windows 8";
                            else
                                operatingSystem = "Windows 8.1";
                            break;
                        case 10:
                            operatingSystem = "Windows 10";
                            break;
                        default:
                            break;
                    }
                }
                if (Program.wineVersion != null)
                    operatingSystem = operatingSystem + " (Linux Wine v" + Program.wineVersion + ")";
                //    //See if there's a service pack installed. extra info? later mabye
                //    if (os.ServicePack != "")
                //    {
                //        //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                //        operatingSystem += " " + os.ServicePack;
                //    }
                //    //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //    //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
                if (operatingSystem == "")
                {
                    string linuxsysteminfo = "/etc/os-release";
                    if (Program.IsLinuxD && File.Exists(linuxsysteminfo))
                    {
                        string[] lines = File.ReadAllLines(linuxsysteminfo);
                        DevConsole.Log(lines.Length);
                        string prettyname = "";
                        string name = "";
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("PRETTY_NAME="))
                            {
                                prettyname = line.Substring(13);
                                prettyname = prettyname.Substring(0, prettyname.Length - 1);
                            }
                            else if (line.StartsWith("NAME="))
                            {
                                name = line.Substring(6);
                                name = name.Substring(0, name.Length - 1);
                            }
                        }
                        if (prettyname == "" && name != "")
                        {
                            operatingSystem += name + " ";
                        }
                        else if (prettyname != "")
                        {
                            operatingSystem += prettyname + " ";
                        }
                    }
                    operatingSystem += os.ToString(); // fall back for unhandled we do want something other that "Windows Mystery Edition" Landon
                }
                return operatingSystem;
            }
        }

        public static ulong localID
        {
            get
            {
                if (NetworkDebugger.enabled)
                    return (ulong)(1330 + NetworkDebugger.currentIndex);
                return Steam.user != null ? Steam.user.id : _localID;
            }
        }

        public static void SetVersion(string v) => _localID = (ulong)Rando.Long();

        //public static bool InitializeDRM() => true;

        public static bool drmFailure => _drmFailure;

        public static bool devBuild => _devBuild;

        public static bool betaBuild => _betaBuild;

        public static bool pressBuild => _pressBuild;

        public static bool buildExpired => false;

        public static int di => NetworkDebugger.currentIndex;

        public static string GetCrashWindowString(
          Exception pException,
          ModConfiguration pModConfig,
          string pLogMessage)
        {
            return GetCrashWindowString(pException, pModConfig.name, pLogMessage);
        }

        public static string GetCrashWindowString(
          Exception pException,
          Assembly pAssembly,
          string pLogMessage)
        {
            return GetCrashWindowString(pException, pAssembly != null ? pAssembly.GetName().Name : null, pLogMessage);
        }

        public static string GetCrashWindowString(
          Exception pException,
          string pAssemblyName,
          string pLogMessage)
        {
            int num = versionMajor;
            string str1 = num.ToString();
            num = versionHigh;
            string str2 = num.ToString();
            num = versionLow;
            string str3 = num.ToString();
            string str4 = str1 + str2 + str3;
            string str5 = "";
            string plainText1 = pAssemblyName != null ? pAssemblyName : "DuckGame";
            string plainText2 = pException != null ? pException.GetType().ToString() : "Unknown";
            try
            {
                foreach (Mod allMod in (IEnumerable<Mod>)ModLoader.allMods)
                {
                    if (!(allMod is CoreMod) && allMod.configuration.loaded)
                        str5 = str5 + allMod.configuration.workshopID.ToString() + ",";
                }
            }
            catch (Exception ex)
            {
                plainText2 = "SENDFAIL " + ex.ToString();
            }
            string str6 = CrashWindow.CrashWindow.Base64Encode(pLogMessage == "" ? "none" : pLogMessage);
            return " -pVersion " + str4 + " -pMods " + CrashWindow.CrashWindow.Base64Encode(str5 == "" ? "none" : str5) + " -pAssembly " + CrashWindow.CrashWindow.Base64Encode(plainText1) + " -pException " + CrashWindow.CrashWindow.Base64Encode(plainText2) + " -pLogMessage " + str6;
        }

        public static string Reduced(this string str, int pMaxLength) => str.Length > pMaxLength ? str.Substring(0, pMaxLength - 2) + ".." : str;

        public static string Padded(this string str, int pMinLength)
        {
            while (str.Length < pMinLength)
                str += " ";
            return str;
        }

        public static IEnumerable<T> Reverse<T>(T[] pArray)
        {
            List<T> objList = new List<T>();
            for (int index = pArray.Length - 1; index >= 0; --index)
                objList.Add(pArray[index]);
            return objList;
        }
    }
}
