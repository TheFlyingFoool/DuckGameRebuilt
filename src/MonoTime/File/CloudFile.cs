// Decompiled with JetBrains decompiler
// Type: DuckGame.CloudFile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DuckGame
{
    [DebuggerDisplay("{cloudPath}")]
    public class CloudFile
    {
        private static Dictionary<string, CloudFile> _index = new Dictionary<string, CloudFile>();
        public const string kCloudstring = "nq500000_";
        public const string kReadstring = "nq403216_";
        public const string kBackupPrefix = "_dgbalooga_save";
        public static List<string> _cloudFolderFilters = new List<string>()
    {
      "EditorPreviews",
      "Online",
      "Workshop",
      "Custom/Arcade/Downloaded",
      "Custom/Background/Downloaded",
      "Custom/Blocks/Downloaded",
      "Custom/Moji/Downloaded",
      "Custom/Parallax/Downloaded",
      "Custom/Platform/Downloaded"
    };
        public string localPath;
        public string cloudPath;
        public string oldCloudPath;
        private DateTime _cloudDate = DateTime.MinValue;
        private DateTime _localDate = DateTime.MinValue;

        public bool isOld
        {
            get
            {
                if (this.cloudPath == null)
                    return true;
                return this.cloudPath.StartsWith("nq403216_") && !this.cloudPath.EndsWith(".lev");
            }
        }

        /// <summary>
        /// The last time the file was saved to the cloud. If cloudDate == DateTime.MinValue, the file is not indexed.
        /// </summary>
        public DateTime cloudDate
        {
            get => this._cloudDate;
            set => this._cloudDate = value;
        }

        /// <summary>
        /// The last time steam uploaded the file.  If steamTimestamp == DateTime.MinValue, the file does not exist on the cloud.
        /// </summary>
        public DateTime steamTimestamp => Steam.FileExists(this.cloudPath) ? Steam.FileTimestamp(this.cloudPath) : DateTime.MinValue;

        /// <summary>
        /// The last time the file was modified locally. If localData == DateTime.MinValue, a local version of this file does not exist.
        /// </summary>
        public DateTime localDate
        {
            get => this._localDate;
            set => this._localDate = value;
        }

        public CloudFile(string pCloudPath)
        {
            this.cloudPath = pCloudPath;
            this.localPath = CloudFile.CloudPathToFilePath(pCloudPath);
            this.oldCloudPath = pCloudPath.Replace("nq500000_", "nq403216_");
        }

        public CloudFile(string pCloudPath, string pLocalPath)
        {
            this.cloudPath = pCloudPath;
            this.localPath = pLocalPath;
            this.oldCloudPath = pCloudPath.Replace("nq500000_", "nq403216_");
        }

        public static void Initialize()
        {
        }

        public static void Clear()
        {
        }

        public static CloudFile GetLocal(string pLocalPath, bool pDelete = false)
        {
            if (pLocalPath.EndsWith(".lev") && !pLocalPath.Contains(DuckFile.levelDirectory) && !pDelete)
                return null;
            bool flag = DuckFile.IsUserPath(pLocalPath);
            if (!pLocalPath.Contains(":"))
                return null;
            pLocalPath = DuckFile.GetLocalSavePath(pLocalPath);
            if (pLocalPath == null)
                return null;
            pLocalPath = pLocalPath.Replace('\\', '/');
            if (pLocalPath[pLocalPath.Length - 1] == '?')
                return null;
            foreach (string cloudFolderFilter in CloudFile._cloudFolderFilters)
            {
                if (pLocalPath.StartsWith(cloudFolderFilter))
                    return null;
            }
            return CloudFile.Get((pLocalPath.EndsWith(".lev") || !flag ? "nq403216_" : "nq500000_") + pLocalPath, pDelete);
        }

        public static CloudFile Get(string pCloudPath, bool pDelete = false)
        {
            if (pDelete)
                CloudFile._index.Remove(pCloudPath);
            CloudFile cloudFile;
            if (!CloudFile._index.TryGetValue(pCloudPath, out cloudFile))
            {
                if (!pDelete)
                {
                    if (pCloudPath == "nq500000_" || pCloudPath.Contains("localsettings.dat") || pCloudPath.Contains("_dgbalooga_save"))
                        return null;
                    bool flag1 = pCloudPath.StartsWith("nq403216_");
                    if (!pCloudPath.Contains("nq500000_") && !flag1)
                        return null;
                    bool flag2 = pCloudPath.EndsWith(".lev");
                    if (flag2 && !flag1)
                        return null;
                    if (flag2 && !pCloudPath.StartsWith("nq403216_Levels") && !pDelete)
                        return null;
                    if (flag1 && !flag2 && Steam.FileExists(pCloudPath.Replace("nq403216_", "nq500000_")))
                        return null;
                }
                string filePath = CloudFile.CloudPathToFilePath(pCloudPath);
                cloudFile = new CloudFile(pCloudPath, filePath);
                if (System.IO.File.Exists(filePath))
                    cloudFile.localDate = System.IO.File.GetLastWriteTime(filePath);
            }
            return cloudFile;
        }

        public static string CloudPathToFilePath(string pPath) => pPath.EndsWith(".lev") || pPath.Contains("nq403216_") ? DuckFile.saveDirectory + pPath.Replace("nq403216_", "") : DuckFile.userDirectory + pPath.Replace("nq500000_", "");
    }
}
