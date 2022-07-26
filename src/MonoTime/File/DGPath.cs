// Decompiled with JetBrains decompiler
// Type: DuckGame.DGPath
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuckGame
{
    public struct DGPath
    {
        public const char Slash = '/';
        private object _specialData;
        private string _path;
        private Dictionary<string, DGPath[]> _filesAndDirectories;
        private DGPath[] _directories;
        private Dictionary<string, DGPath[]> _files;
        private bool _file;
        private bool _rooted;
        private static StringBuilder kBuilder = new StringBuilder();

        public object specialData => this._specialData;

        public string path => this._path;

        public bool exists
        {
            get
            {
                if (this._specialData != null)
                    return false;
                return this._file ? System.IO.File.Exists(this._path) : Directory.Exists(this._path);
            }
        }

        /// <summary>Gets the current directory represented by the path</summary>
        public DGPath directory
        {
            get
            {
                for (int index = this._path.Length - 1; index >= 0; --index)
                {
                    if (this._path[index] == '/')
                        return this.CopyNewPath(this._path.Substring(0, index + 1));
                }
                return (DGPath)this._path;
            }
        }

        /// <summary>
        /// Returns directoryName if this is a directory, otherwise returns fileName.
        /// </summary>
        public string name
        {
            get
            {
                if (this._specialData != null)
                    return this._path;
                return this._file ? this.fileName : this.directoryName;
            }
        }

        public string directoryName
        {
            get
            {
                if (this._specialData != null)
                    return this._path;
                bool flag = false;
                int length = this._path.Length - 1;
                for (int index = this._path.Length - 1; index >= 0; --index)
                {
                    if (this._path[index] == '/')
                    {
                        if (flag)
                            return this._path.Substring(index + 1, length - index - 1);
                        flag = true;
                        length = index;
                    }
                }
                return !flag ? "" : this._path.Substring(0, length);
            }
        }

        public string fileName
        {
            get
            {
                if (this._specialData != null)
                    return this._path;
                if (!this._file)
                    return "";
                for (int index = this._path.Length - 1; index >= 0; --index)
                {
                    if (this._path[index] == '/')
                        return this._path.Substring(index + 1, this._path.Length - index - 1);
                }
                return this._path;
            }
        }

        public DGPath[] GetFilesAndDirectories(params string[] pExtensions)
        {
            if (this._filesAndDirectories == null)
                this._filesAndDirectories = new Dictionary<string, DGPath[]>();
            string key = "";
            foreach (string pExtension in pExtensions)
                key += pExtension;
            DGPath[] filesAndDirectories = (DGPath[])null;
            if (!this._filesAndDirectories.TryGetValue(key, out filesAndDirectories))
            {
                List<DGPath> dgPathList = new List<DGPath>();
                dgPathList.AddRange((IEnumerable<DGPath>)this.GetDirectories());
                dgPathList.AddRange((IEnumerable<DGPath>)this.GetFiles(pExtensions));
                filesAndDirectories = dgPathList.ToArray();
            }
            if (filesAndDirectories == null)
                filesAndDirectories = new DGPath[0];
            this._filesAndDirectories[key] = filesAndDirectories;
            return filesAndDirectories;
        }

        public DGPath[] GetDirectories()
        {
            if (this._directories == null)
            {
                if (!this.isDirectory)
                    throw new DGPath.DGPathException("DGPath.GetDirectories() does not work on file paths, only on directory paths.");
                if (!this.exists)
                {
                    this._directories = new DGPath[0];
                }
                else
                {
                    List<DGPath> dgPathList = new List<DGPath>();
                    foreach (string directory in Directory.GetDirectories(this._path))
                        dgPathList.Add((DGPath)directory);
                    this._directories = dgPathList.ToArray();
                }
            }
            return this._directories;
        }

        public DGPath[] GetFiles(params string[] pExtensions)
        {
            if (this._files == null)
                this._files = new Dictionary<string, DGPath[]>();
            string key = "";
            foreach (string pExtension in pExtensions)
                key += pExtension;
            DGPath[] files = (DGPath[])null;
            if (!this._files.TryGetValue(key, out files))
            {
                if (!this.isDirectory)
                    throw new DGPath.DGPathException("DGPath.GetFiles() does not work on file paths, only on directory paths.");
                if (!this.exists)
                    files = new DGPath[0];
                List<DGPath> dgPathList = new List<DGPath>();
                foreach (string file in Directory.GetFiles(this._path))
                {
                    if (pExtensions.Length != 0)
                    {
                        foreach (string pExtension in pExtensions)
                        {
                            if (file.EndsWith(pExtension, StringComparison.InvariantCultureIgnoreCase))
                            {
                                dgPathList.Add((DGPath)file);
                                break;
                            }
                        }
                    }
                    else
                        dgPathList.Add((DGPath)file);
                }
                files = dgPathList.ToArray();
                this._files[key] = files;
            }
            return files;
        }

        public bool isDirectory => !this._file;

        public bool isFullPath => !this._file;

        internal void CheckFileValidity(bool pMustBeFile = true, bool pWriting = false)
        {
            if (pMustBeFile && !this._file)
                throw new DGPath.DGPathException("DGPath.ReadText(" + this._path + ") failed: path is a directory, not a file.");
            if (pWriting)
                Directory.CreateDirectory((string)this.directory);
            else if (!this.exists)
                throw new DGPath.DGPathException("DGPath.ReadText(" + this._path + ") failed: file does not exist.");
        }

        public DGPath(string pPath)
        {
            bool flag = false;
            this._file = false;
            this._rooted = false;
            this._directories = (DGPath[])null;
            this._files = (Dictionary<string, DGPath[]>)null;
            this._filesAndDirectories = (Dictionary<string, DGPath[]>)null;
            this._specialData = (object)null;
            int index = 0;
            DGPath.kBuilder.Clear();
            if (pPath.Length > 1 && pPath[1] == ':')
            {
                this._rooted = true;
                DGPath.kBuilder.Append(char.ToUpper(pPath[0]));
                ++index;
            }
            for (; index < pPath.Length; ++index)
            {
                char ch = pPath[index];
                switch (ch)
                {
                    case '.':
                        this._file = true;
                        goto default;
                    case '/':
                    case '\\':
                        if (!flag)
                        {
                            DGPath.kBuilder.Append('/');
                            flag = true;
                        }
                        this._file = false;
                        break;
                    default:
                        DGPath.kBuilder.Append(ch);
                        flag = false;
                        break;
                }
            }
            this._path = DGPath.kBuilder.ToString();
            if (this._file || this._path[this._path.Length - 1] == '/')
                return;
            this._path += "/";
        }

        public static DGPath Special(string pName, object pSpecialData) => new DGPath()
        {
            _path = pName,
            _specialData = pSpecialData
        };

        public DGPath Up()
        {
            bool flag = false;
            for (int index = this._path.Length - 1; index >= 0; --index)
            {
                if (this._path[index] == '/')
                {
                    if (flag)
                        return this.CopyNewPath(this._path.Substring(0, index + 1));
                    flag = true;
                }
            }
            return this;
        }

        public DGPath Down(string pFolder) => pFolder == null ? this : this + (DGPath)pFolder;

        public DGPath Move(string pFile) => pFile == null ? this : this + (DGPath)pFile;

        public static implicit operator string(DGPath pPath) => pPath._path;

        public static implicit operator DGPath(string pPath) => new DGPath(pPath);

        public static bool operator ==(DGPath value1, DGPath value2) => value1._path == value2._path;

        public static bool operator !=(DGPath value1, DGPath value2) => value1._path != value2._path;

        public static DGPath operator +(DGPath value1, DGPath value2)
        {
            if (value2._rooted)
                throw new DGPath.DGPathException("(DGPath1 + DGPath2) failed- DGPath2 must be a subfolder and not a fully rooted path (C:/ type paths are not allowed)");
            if (value1._file)
                throw new DGPath.DGPathException("(DGPath1 + DGPath2) failed- DGPath1 must NOT be a file!");
            return new DGPath()
            {
                _path = value1._path + value2._path,
                _rooted = value1._rooted,
                _file = value2._file
            };
        }

        public static bool operator <(DGPath value1, DGPath value2) => value2._path.Contains(value1._path);

        public static bool operator >(DGPath value1, DGPath value2) => value1._path.Contains(value2._path);

        /// <summary>Removes the root path from the current path</summary>
        /// <param name="pRoot">The current root to remove.</param>
        /// <returns>The unrooted path.</returns>
        public DGPath Unroot(DGPath pRoot) => this._path.Contains(pRoot._path) ? this.CopyNewPath(this._path.Replace(pRoot._path, "")) : this;

        public static DGPath operator -(DGPath value1, DGPath value2) => value1._path.Length < value2._path.Length ? value2.Unroot(value1) : value1.Unroot(value2);

        internal DGPath CopyNewPath(string pPath) => new DGPath()
        {
            _path = pPath,
            _rooted = this._rooted
        };

        public override string ToString() => this._path;

        public void Delete()
        {
            this.CheckFileValidity(false);
            if (this._file)
                System.IO.File.Delete(this._path);
            else
                Directory.Delete(this._path);
        }

        public string ReadText()
        {
            this.CheckFileValidity();
            return System.IO.File.ReadAllText(this._path);
        }

        public void WriteText(string pText)
        {
            this.CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllText(this._path, pText);
        }

        public string[] ReadLines()
        {
            this.CheckFileValidity();
            return System.IO.File.ReadAllLines(this._path);
        }

        public void WriteLines(string[] pLines)
        {
            this.CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllLines(this._path, pLines);
        }

        public byte[] ReadBytes()
        {
            this.CheckFileValidity();
            return System.IO.File.ReadAllBytes(this._path);
        }

        public void WriteBytes(byte[] pBytes)
        {
            this.CheckFileValidity(pWriting: true);
            System.IO.File.WriteAllBytes(this._path, pBytes);
        }

        public void CreatePath() => this.CheckFileValidity(pWriting: true);

        public class DGPathException : Exception
        {
            public DGPathException(string pMessage)
              : base(pMessage)
            {
            }
        }
    }
}
