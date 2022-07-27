// Decompiled with JetBrains decompiler
// Type: DuckGame.DG
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
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

        public static string version => DG.MakeVersionString(DG._versionMajor, DG._versionHigh, DG._versionLow);

        public static int versionHigh => DG._versionHigh;

        public static int versionLow => DG._versionLow;

        public static int versionMajor => DG._versionMajor;

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
                string str = Environment.OSVersion.ToString();
                string platform = "Windows Mystery Edition";
                if (str.Contains("5.0"))
                    platform = "Windows 2000";
                else if (str.Contains("5.1"))
                    platform = "Windows XP";
                else if (str.Contains("5.2"))
                    platform = "Windows XP 64-Bit Edition";
                else if (str.Contains("6.0"))
                    platform = "Windows Vista";
                else if (str.Contains("6.1"))
                    platform = "Windows 7";
                else if (str.Contains("6.2"))
                    platform = "Windows 8";
                else if (str.Contains("6.3"))
                    platform = "Windows 8.1";
                else if (str.Contains("10.0"))
                    platform = "Windows 10";
                if (Program.wineVersion != null)
                    platform = platform + " (Linux Wine v" + Program.wineVersion + ")";
                return platform;
            }
        }

        public static ulong localID
        {
            get
            {
                if (NetworkDebugger.enabled)
                    return (ulong)(1330 + NetworkDebugger.currentIndex);
                return Steam.user != null ? Steam.user.id : DG._localID;
            }
        }

        public static void SetVersion(string v) => DG._localID = (ulong)Rando.Long();

        public static bool InitializeDRM() => true;

        public static bool drmFailure => DG._drmFailure;

        public static bool devBuild => DG._devBuild;

        public static bool betaBuild => DG._betaBuild;

        public static bool pressBuild => DG._pressBuild;

        public static bool buildExpired => false;

        public static int di => NetworkDebugger.currentIndex;

        public static string GetCrashWindowString(
          Exception pException,
          ModConfiguration pModConfig,
          string pLogMessage)
        {
            return DG.GetCrashWindowString(pException, pModConfig.name, pLogMessage);
        }

        public static string GetCrashWindowString(
          Exception pException,
          Assembly pAssembly,
          string pLogMessage)
        {
            return DG.GetCrashWindowString(pException, pAssembly != null ? pAssembly.GetName().Name : null, pLogMessage);
        }

        public static string GetCrashWindowString(
          Exception pException,
          string pAssemblyName,
          string pLogMessage)
        {
            int num = DG.versionMajor;
            string str1 = num.ToString();
            num = DG.versionHigh;
            string str2 = num.ToString();
            num = DG.versionLow;
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
